#addin "nuget:?package=Cake.Figlet&version=1.3.1"

#load "build/shared/cake/common.cake"
#load "build/shared/cake/dotnetcore.cake"

Setup<BuildParameters>(context =>
{
   var parameters = BuildParameters.Create(Context);
   parameters.Initialize();
   
   Information(Figlet("AppCore.XYZ"));
   Information($"Version: {parameters.Version.InformationalVersion}");
   
   return parameters;
});

Task("Restore")
   .IsDependentOn("DotNetCore.Restore");

Task("Build")
   .IsDependentOn("DotNetCore.Build");

Task("Test")
   .IsDependentOn("DotNetCore.Test");

Task("Clean")
   .IsDependentOn("Common.Clean")
   .IsDependentOn("DotNetCore.Clean");

Task("Rebuild")
   .IsDependentOn("Clean")
   .IsDependentOn("Build");

Task("Publish")
   .IsDependentOn("DotNetCore.Pack");
   
RunTarget(Context.Argument("target", "Build"));
