public partial class BuildParameters
{
    private readonly ICakeContext _context;
    
    public string Target { get; private set; }
    public HashSet<string> SkipTargets { get; private set; } = new HashSet<string>();
    
    public string Configuration { get; private set; }
    public string ArtifactsDir { get; private set; }
    public BuildVersion Version { get; private set; }
    
    private BuildParameters(ICakeContext context)
    {
        _context = context;
    }
    
    public static BuildParameters Create(ICakeContext context, Action<BuildParameters> init = null)
    {
        var p = new BuildParameters(context)
        {
            Target = context.Argument("target", "Build"),
            SkipTargets = new HashSet<string>(context.Argument("skip-target", "").Split(";")),
            Configuration = context.Argument("configuration", "Debug"),
            ArtifactsDir = context.Argument("artifactsDir", "artifacts"),
            Version = BuildVersion.Create(context)
        };
        
        init?.Invoke(p);
        return p;
    }
    
    public void Initialize()
    {
        Version.Initialize();
    }
    
    public bool ShouldSkip(params string[] targets)
    {
        return SkipTargets.Overlaps(targets);
    }
}

public class BuildVersion
{
    private readonly ICakeContext _context;
    
    public string Version { get; private set; }
    public string AssemblyVersion { get; private set; }
    public string FileVersion { get; private set; }
    public string InformationalVersion { get; private set; }
        
    private BuildVersion(ICakeContext context)
    {
        _context = context;
    }
    
    public static BuildVersion Create(ICakeContext context)
    {
        return new BuildVersion(context);
    }
    
    public void Initialize()
    {
        var versionInfo = _context.GitVersion();
        Version = versionInfo.SemVer;
        AssemblyVersion =  versionInfo.AssemblySemVer;
        FileVersion = versionInfo.AssemblySemFileVer;
        InformationalVersion = versionInfo.InformationalVersion;
    }
}
