using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Tools.Octopus;
using Nuke.Common.Tools.Xunit;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

using Octokit;
using System.IO;
using static Nuke.Common.IO.XmlTasks;
using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;
using static Nuke.Common.Tools.Xunit.XunitTasks;

[GitHubActions("CI",
    GitHubActionsImage.WindowsLatest,
    CacheKeyFiles = new string[] { },
    EnableGitHubToken = true,
    FetchDepth = 0,
    OnPullRequestBranches = new[] { "*" },
    OnPushBranches = new[] { "develop", "main", "master", "release/*" },
    PublishArtifacts = true,
    InvokedTargets = [nameof(CI)])]
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
    
    [Parameter("Github Token")]
    readonly string GithubToken;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    // DIRECTORIES
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath SourcesDirectory => RootDirectory / "src";
    AbsolutePath DeploymentDirectory => RootDirectory.Parent / "Announcements";
    AbsolutePath WebsiteDirectory => RootDirectory.Parent.Parent;

    // Files
    AbsolutePath ModuleAssembly => SourcesDirectory / "bin" / "DotNetNuke.Modules.Announcements.dll";

    Target Clean => _ => _
        .Before(Restore)
        .Before(Compile)
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
        .DependsOn(SetVersion)
        .Executes(() =>
        {
            MSBuild(s => s
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution)
                .SetAssemblyVersion(GitVersion.MajorMinorPatch));
        });

    Target SetVersion => _ => _
        .Executes(() =>
        {
            Serilog.Log.Information($"Version: {GitVersion.ToJson()}");
            SetManifestVersion();
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var testAssembly = RootDirectory / "tests" / "bin" / Configuration / "DotNetNuke.Announcements.Tests.dll";
            Xunit2(s => s
                .AddTargetAssemblies(testAssembly)
                .SetFramework("net472")
                .SetReporter(Xunit2ReporterType.Verbose));
        });

    Target Package => _ => _
        .DependsOn(Clean)
        .DependsOn(Compile)
        .Produces(ArtifactsDirectory)
        .Executes(() =>
        {
            var packagingDir = ArtifactsDirectory / "packaging";

            // Resources
            var resourcesDir = packagingDir / "resources";
            resourcesDir.CreateOrCleanDirectory();
            DeployResourcesTo(resourcesDir);
            resourcesDir.ZipTo(packagingDir / "resources.zip");
            resourcesDir.DeleteDirectory();

            // Other files
            (SourcesDirectory / "Documentation").CopyToDirectory(packagingDir);
            (SourcesDirectory / "Images").CopyToDirectory(packagingDir);
            (SourcesDirectory / "Providers").CopyToDirectory(packagingDir);
            SourcesDirectory.GlobFiles("*.dnn").ForEach(f => f.CopyToDirectory(packagingDir));
            SourcesDirectory.GlobFiles("*.txt").ForEach(f => f.CopyToDirectory(packagingDir));

            // Assembly
            ModuleAssembly.CopyToDirectory(packagingDir / "bin", createDirectories: true);

            // Package
            packagingDir.ZipTo(ArtifactsDirectory / $"Announcements_Install_v{GitVersion.SemVer}.zip");
            packagingDir.DeleteDirectory();
        });

    Target CI => _ => _
        .DependsOn(Test)
        .DependsOn(Package)
        .Executes(() =>
        {
            if (GitRepository.IsOnMainBranch() || GitRepository.IsOnReleaseBranch())
            {
                CreateRelease();
            }
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

    private void SetManifestVersion()
    {
        var manifests = SourcesDirectory.GlobFiles("*.dnn");
        manifests.ForEach(manifest =>
        {
            XmlPoke(
                manifest,
                "//packages/package/@version",
                GitVersion.MajorMinorPatch);
            Serilog.Log.Information($"Updated version in {manifest} manifest file to {GitVersion.MajorMinorPatch}");
        });
    }

    private void CreateRelease()
    {
        // Tag the release
        var version = GitRepository.IsOnMainBranch() ? GitVersion.MajorMinorPatch : GitVersion.SemVer;
        Git($"tag v{version}");
        Git("push --tags");

        // Setup client (octokit)
        var client = new GitHubClient(new ProductHeaderValue("Nuke"));
        var tokenAuth = new Credentials(GithubToken);
        client.Credentials = tokenAuth;

        // Release
        var newRelease = new NewRelease($"v{version}")
        {
            Body = $"Release {version}",
            Draft = true,
            GenerateReleaseNotes = true,
            Name = $"v{version}",
            Prerelease = GitRepository.IsOnReleaseBranch(),
            TargetCommitish = GitVersion.Sha,
        };
        var release = client.Repository.Release
            .Create(
                GitRepository.GetGitHubOwner(),
                GitRepository.GetGitHubName(),
                newRelease)
            .Result;
        Serilog.Log.Information($"Release created: {release.HtmlUrl}");

        // Add artifacts
        var artifacts = ArtifactsDirectory.GlobFiles("*.zip");
        artifacts.ForEach(artifact =>
        {
            var fileContent = File.OpenRead(artifact);
            var fileInfo = new FileInfo(artifact);
            var assetUpload = new ReleaseAssetUpload
            {
                FileName = fileInfo.Name,
                ContentType = "application/zip",
                RawData = fileContent,
            };
            var asset = client.Repository.Release
                .UploadAsset(release, assetUpload)
                .Result;
            Serilog.Log.Information($"Asset {asset.Name} uploaded at {asset.BrowserDownloadUrl}");
        });
    }
}
