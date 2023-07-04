using Cake.Common.Build;
using Cake.Common.IO;
using Cake.Common.Tools.GitVersion;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using Octokit;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LlamaCpp.Net.Build.Tasks;

[TaskName("PublishReleaseTask")]
public class PublishReleaseTask : AsyncFrostingTask<BuildContext>
{
    string[] releaseBranches = new[] { "master", "develop" };

    public override bool ShouldRun(BuildContext context)
    {
        var version = context.GitVersion();

        if (releaseBranches.Contains(version.BranchName))
        {
            return true;
        }

        context.Log.Information($"Not on a release branch, skipping release. Branch: {version.BranchName}");
        return false;

    }

    public override async Task RunAsync(BuildContext context)
    {
        var version = context.GitVersion();
        var githubClient = GetGitHubClient(context);
        var githubActions = context.GitHubActions();


        var owner = githubActions.Environment.Workflow.RepositoryOwner;

        var repo = githubActions.Environment.Workflow.Repository.Replace(owner + "/", "");

        var runsLocally = true;

        var releaseExists = false;
        if (context.GitHubActions().IsRunningOnGitHubActions)
        {
            runsLocally = false;
            releaseExists = await ReleaseExists(githubClient, owner, repo, version);
        }


        if (releaseExists)
        {
            context.Log.Information($"Release {version.SemVer} already exists, skipping");
            return;
        }


        var newRelease = new NewRelease(version.SemVer)
        {
            Name = version.SemVer,
            Draft = true,
            Prerelease = false,
            GenerateReleaseNotes = true
        };

        if (version.PreReleaseLabel != null)
        {
            newRelease.Prerelease = true;
        }

        var nugetTemp = context.TmpDir.Combine("nuget");

        var releaseFiles = context.GetFiles(nugetTemp.CombineWithFilePath("*.nupkg").FullPath);

        if (runsLocally)
        {
            context.Log.Information($"Would create release {version.SemVer} on GitHub");
            foreach (var releaseFile in releaseFiles)
            {
                context.Log.Information($"Would add {releaseFile.GetFilename().ToString()} to release");
            }
        }
        else
        {
            context.Log.Information($"Creating release {version.SemVer} on GitHub");
            var release = await githubClient.Repository.Release.Create(owner, repo, newRelease);


            foreach (var releaseFile in releaseFiles)
            {
                await AddFileToRelease(releaseFile, githubClient, release);
            }
        }
    }

    private static async Task AddFileToRelease(FilePath releaseFile, GitHubClient githubClient, Release release)
    {
        await using var fileStream = File.OpenRead(releaseFile.FullPath);

        var releaseAssetUpload = new ReleaseAssetUpload
        {
            FileName = releaseFile.GetFilename().ToString(),
            ContentType = "application/octet-stream",
            RawData = fileStream
        };

        await githubClient.Repository.Release.UploadAsset(release, releaseAssetUpload);
    }

    private static async Task<bool> ReleaseExists(IGitHubClient githubClient, string owner, string repo,
        GitVersion version)
    {
        var releases = await githubClient.Repository.Release.GetAll(owner, repo);

        var releaseExists = releases.Any(r => r.TagName == version.SemVer);
        return releaseExists;
    }

    private static GitHubClient GetGitHubClient(ICakeContext context)
    {
        if (context.GitHubActions().IsRunningOnGitHubActions)
        {
            var githubActions = context.GitHubActions();


            var githubClient = new GitHubClient(new ProductHeaderValue("CakeBuild"))
            {
                Credentials = new Credentials(githubActions.Environment.Runtime.Token)
            };

            return githubClient;
        }

        else
        {
            var githubClient = new GitHubClient(new ProductHeaderValue("CakeBuild"));
            return githubClient;
        }
    }
}