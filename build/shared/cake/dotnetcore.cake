#module "nuget:?package=Cake.DotNetTool.Module&version=0.4.0"
#addin "nuget:?package=Cake.Coverlet&version=2.4.2"
#tool "dotnet:?package=GitVersion.Tool&version=5.1.3"

public static partial class BuildConstants
{
    public const string TempVersionPropsFile = "build/version.generated.props";
    public const string TempSourceSolution = "build/sources.generated.sln";
    public const string TempTestSolution = "build/tests.generated.sln";
}

public partial class BuildParameters
{
    public BuildParametersDotNetCore DotNetCore { get; } = new BuildParametersDotNetCore();
}

public class BuildParametersDotNetCore
{
    public string SourceSolution { get; set; }
    public string SourceProjectsPattern { get; set; } = "**/src/**/*.csproj";
    
    public string TestSolution { get; set; }
    public string TestProjectsPattern { get; set; } = "**/test/**/*.csproj";
    
    public bool CollectCoverage { get; set; } = true;
    public bool CollectTestAssemblyCoverage { get; set; } = false;
    public CoverletOutputFormat CoverletOutputFormat { get; set; } = CoverletOutputFormat.cobertura;
}

public bool DotNetCoreNewSolution(string solutionFile, FilePathCollection projectFiles)
{
    bool needsUpdate = true;
    
    if (FileExists(solutionFile))
    {
        var solution = ParseSolution(solutionFile);
        var existingProjects = solution.Projects.Select(p => System.IO.Path.GetFullPath(p.Path.ToString())).OrderBy(p => p);
        var projects = projectFiles.Select(p => System.IO.Path.GetFullPath(p.ToString())).OrderBy(p => p);
        needsUpdate = !existingProjects.SequenceEqual(projects);
    }

    if (needsUpdate)
    {
        Context.StartProcess("dotnet", new ProcessSettings{ Arguments = $"new sln -o {System.IO.Path.GetDirectoryName(solutionFile)} -n {System.IO.Path.GetFileNameWithoutExtension(solutionFile)} --force" });
        var projects = String.Join(" ", projectFiles.Select(p => String.Concat("\"", p.ToString(), "\"")));
        Context.StartProcess("dotnet", new ProcessSettings{ Arguments = $"sln {solutionFile} add {projects}" });
    }
    
    return needsUpdate;
}

Task("DotNetCore.InitVersion")
    .Does<BuildParameters>(p =>
{
    var file = BuildConstants.TempVersionPropsFile;
    if (UpdateVersionProps(file, p.Version))
    {
        Information($"Generated '{file}'.");
    }
    else
    {
        Information($"'{file}' is up-to-date.");
    }
});

Task("DotNetCore.InitSolution")
    .Does<BuildParameters>(p =>
{
    if (String.IsNullOrEmpty(p.DotNetCore.SourceSolution))
    {
        var sourceProjects = GetFiles(p.DotNetCore.SourceProjectsPattern);
        var file = p.DotNetCore.SourceSolution = BuildConstants.TempSourceSolution;
        if (DotNetCoreNewSolution(file, sourceProjects))
        {
            Information($"Generated '{file}'.");
        }
        else
        {
            Information($"'{file}' is up-to-date.");
        }
    }
});

Task("DotNetCore.InitSolution.Test")
    .Does<BuildParameters>(p =>
{
    if (String.IsNullOrEmpty(p.DotNetCore.TestSolution))
    {
        var sourceProjects = GetFiles(p.DotNetCore.TestProjectsPattern);
        var file = p.DotNetCore.TestSolution = BuildConstants.TempTestSolution;
        if (DotNetCoreNewSolution(file, sourceProjects))
        {
            Information($"Generated '{file}'.");
        }
        else
        {
            Information($"'{file}' is up-to-date.");
        }
    }
});

Task("DotNetCore.Restore")
    .IsDependentOn("DotNetCore.InitSolution")
    .CanBeSkipped("Restore", "DotNetCore.Restore", "Build", "DotNetCore.Build")
    .Does<BuildParameters>(p =>
{
    DotNetCoreRestore(p.DotNetCore.SourceSolution.ToString());
});

Task("DotNetCore.Restore.Test")
    .IsDependentOn("DotNetCore.InitSolution.Test")
    .CanBeSkipped("Restore.Test","DotNetCore.Restore.Test")
    .Does<BuildParameters>(p =>
{
    DotNetCoreRestore(p.DotNetCore.TestSolution.ToString());
});

Task("DotNetCore.Build")
  .IsDependentOn("DotNetCore.InitVersion")
  .IsDependentOn("DotNetCore.InitSolution")
  .IsDependentOn("DotNetCore.Restore")
  .CanBeSkipped("Build", "DotNetCore.Build")
  .Does<BuildParameters>(p =>
{
    DotNetCoreBuild(p.DotNetCore.SourceSolution.ToString(), new DotNetCoreBuildSettings
    {
        Configuration = p.Configuration,
        NoRestore = true
    });
});

Task("DotNetCore.Build.Test")
  .IsDependentOn("DotNetCore.InitVersion")
  .IsDependentOn("DotNetCore.InitSolution.Test")
  .IsDependentOn("DotNetCore.Restore.Test")
  .CanBeSkipped("Build.Test", "DotNetCore.Build.Test")
  .Does<BuildParameters>(p =>
{
    DotNetCoreBuild(p.DotNetCore.TestSolution.ToString(), new DotNetCoreBuildSettings
    {
        Configuration = p.Configuration,
        NoRestore = true
    });
});

Task("DotNetCore.Test")
    .IsDependentOn("DotNetCore.Build")
    .IsDependentOn("DotNetCore.Build.Test")
    .Does<BuildParameters>(p =>
{
    var testResultsDir = System.IO.Path.Combine(p.ArtifactsDir, "test-results");
    CreateDirectory(testResultsDir);
    CleanDirectory(testResultsDir);

    var timestamp = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss-FFF}";
    
    var coverletSettings = new CoverletSettings {
        CollectCoverage = p.DotNetCore.CollectCoverage,
        CoverletOutputFormat = CoverletOutputFormat.json|p.DotNetCore.CoverletOutputFormat,
        CoverletOutputDirectory = Directory(testResultsDir),
        CoverletOutputName = $"coverage-{timestamp}",
        MergeWithFile = $"coverage-{timestamp}.json",
        IncludeTestAssembly = p.DotNetCore.CollectTestAssemblyCoverage,
        Exclude = new List<string>() { "[xunit.*]*", "[*]*Tests*" }
    };
    
    DotNetCoreTest(p.DotNetCore.TestSolution.ToString(), new DotNetCoreTestSettings
    {
        ArgumentCustomization = args=> args.Append($"/maxcpucount:1"),
        Configuration = p.Configuration,
        ResultsDirectory = testResultsDir,
        Logger = "trx",
        NoRestore = true,
        NoBuild = true
    }, coverletSettings);
});

Task("DotNetCore.Pack")
    .IsDependentOn("DotNetCore.Build")
    .Does<BuildParameters>(p =>
{
    var packageDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(p.ArtifactsDir, "packages"));
    DotNetCorePack(p.DotNetCore.SourceSolution.ToString(), new DotNetCorePackSettings
    {
        Configuration = p.Configuration,
        OutputDirectory = packageDir,
        NoRestore = true,
        NoBuild = true
    });
});

Task("DotNetCore.Clean")
    .IsDependentOn("DotNetCore.InitSolution")
    .IsDependentOn("DotNetCore.InitSolution.Test")
    .Does<BuildParameters>(p =>
{
    DotNetCoreClean(p.DotNetCore.SourceSolution.ToString());
    DotNetCoreClean(p.DotNetCore.TestSolution.ToString());
    
    CleanFile(BuildConstants.TempVersionPropsFile);
    CleanFile(BuildConstants.TempSourceSolution);
    CleanFile(BuildConstants.TempTestSolution);
});
