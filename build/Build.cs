using System;
using System.Linq;
using Microsoft.Build.Tasks;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;

[GitHubActions("CI",
    GitHubActionsImage.WindowsLatest,
    CacheKeyFiles = new string[] { },
    EnableGitHubToken = true,
    FetchDepth = 0,
    OnPullRequestBranches = new[] { "*" },
    OnPushBranches = new[] { "develop", "main", "master", "release/*" },
    PublishArtifacts = true)]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    // DIRECTORIES
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath SourcesDirectory => RootDirectory / "src";
    AbsolutePath DeploymentDirectory => RootDirectory.Parent / "Announcements";
    AbsolutePath WebsiteDirectory = RootDirectory.Parent.Parent;

    // Files
    AbsolutePath ModuleAssembly => SourcesDirectory / "bin" / "DotNetNuke.Modules.Announcements.dll";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            NuGetRestore(s => s
                .SetTargetPath(Solution));
        });

    Target Deploy => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DeployResourcesTo(DeploymentDirectory);
            var binDir = WebsiteDirectory / "bin";
            Serilog.Log.Information($"Copying {ModuleAssembly} to {binDir}");
            ModuleAssembly.CopyToDirectory(binDir, ExistsPolicy.FileOverwriteIfNewer);
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            MSBuild(s => s
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution));
        });

    private void DeployResourcesTo(AbsolutePath destination)
    {
        var dirs = new[] { "App_LocalResources", "Documentation", "Images", "Providers", "Templates" };
        dirs.ForEach(dir =>
        {
            Serilog.Log.Information($"Copying {dir} to {destination}");
            (SourcesDirectory / dir).CopyToDirectory(destination, ExistsPolicy.MergeAndOverwrite);
        });
        var extensions = new[] { ".ascx", ".css" };
        extensions.ForEach(ext =>
        {
            SourcesDirectory.GlobFiles($"*{ext}")
                .ForEach(file =>
                {
                    Serilog.Log.Information($"Copying {file} to {destination}");
                    file.CopyToDirectory(destination, ExistsPolicy.MergeAndOverwrite);
                });
        });
    }
}
