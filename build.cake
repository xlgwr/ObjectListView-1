var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release45");
var buildVerbosity = Verbosity.Minimal;

var solutionFile = "./ObjectListView.sln";
var nugetOuput = "./output/package";

var assemblyInfo = ParseAssemblyInfo("./ObjectListView/Properties/AssemblyInfo.cs");    
var assemblyVersion = new Version(assemblyInfo.AssemblyFileVersion);
var libVersion = assemblyVersion.ToString(3);
var nugetVersion = libVersion;


Task("Clean")
    .Does(() =>
{
    EnsureDirectoryExists(nugetOuput);
    CleanDirectory(nugetOuput);
});

// NUGET
Task("Restore-NuGet-Packages")    
    .Does(() =>
{
    NuGetRestore(solutionFile);
});


Task("Build")        
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
      MSBuild(solutionFile, settings => 
        settings.SetConfiguration(configuration)        
                .SetVerbosity(buildVerbosity));                
});


Task("Pack")
  .Does(() =>
{ 
    Information("LibVersion : " + libVersion );
    Information("NugetVersion : " + nugetVersion );

    var settings = new NuGetPackSettings
    {
        Version         = nugetVersion, 
        OutputDirectory = nugetOuput
    };

    var nuspec = "./ObjectListView/ObjectListView.nuspec";
    NuGetPack(nuspec, settings);
});

// STARTUP
Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Pack");

RunTarget(target);
