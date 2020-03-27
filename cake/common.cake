#addin "nuget:?package=Cake.FileHelpers&version=3.2.1"

#load "parameters.cake"
#load "constants.cake"

public static CakeTaskBuilder CanBeSkipped(this CakeTaskBuilder builder, params string[] tasks)
{
    return builder.WithCriteria<BuildParameters>((c,p) => !p.SkipTargets.Overlaps(tasks));
}

public void CleanFile(string filePath)
{
    if (Context.FileExists(filePath))
        Context.DeleteFile(filePath);
}

public bool UpdateVersionProps(string projectFile, BuildVersion version)
{
    var file = Context.File(projectFile);
    bool updated = false;
    if (!Context.FileExists(file))
    {
        Context.FileWriteText(file, "<Project><PropertyGroup><Version /><AssemblyVersion /><FileVersion /><InformationalVersion /></PropertyGroup></Project>");
    }
    
    if (Context.XmlPeek(file, "/Project/PropertyGroup/Version") != version.Version)
    {
        Context.XmlPoke(file, "/Project/PropertyGroup/Version", version.Version);
        updated = true;
    }
    
    if (Context.XmlPeek(file, "/Project/PropertyGroup/AssemblyVersion") != version.AssemblyVersion)
    {
        Context.XmlPoke(file, "/Project/PropertyGroup/AssemblyVersion", version.AssemblyVersion);
        updated = true;
    }
    
    if (Context.XmlPeek(file, "/Project/PropertyGroup/FileVersion") != version.FileVersion)
    {
        Context.XmlPoke(file, "/Project/PropertyGroup/FileVersion", version.FileVersion);
        updated = true;
    }
    
    if (Context.XmlPeek(file, "/Project/PropertyGroup/InformationalVersion") != version.InformationalVersion)
    {
        Context.XmlPoke(file, "/Project/PropertyGroup/InformationalVersion", version.InformationalVersion);
        updated = true;
    }
    
    return updated;
}

Task("Common.Clean")
    .Does<BuildParameters>(p =>
{
    CleanDirectory(p.ArtifactsDir);
});
