#addin "nuget:?package=Cake.DocFx&version=0.13.1"
#tool "nuget:?package=docfx.console&version=2.50.0"

public partial class BuildParameters
{
    public BuildParametersDocFx DocFx { get; } = new BuildParametersDocFx();
}

public class BuildParametersDocFx
{
    public string ProjectFile { get; set; } = "./docs/docfx.json";
    public bool WarningsAsErrors { get; set; } = false;
}

Task("DocFx.Build")
  .Does(() =>
{
    DocFxMetadata(Parameters.DocFx.ProjectFile);
    DocFxBuild(Parameters.DocFx.ProjectFile, new DocFxBuildSettings
    {
        WarningsAsErrors = Parameters.DocFx.WarningsAsErrors
    });
});
