using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using LibGit2Sharp;
using System;

namespace LlamaCpp.Net.Build.Tasks.Libraries.Abstractions;

public abstract class CloneTask : FrostingTask<BuildContext>
{
    protected void CloneAndCheckout(BuildContext context, DependencyInfo info)
    {
        var repositoryName = info.Name;
        var repositoryUrl = info.RepositoryUrl;
        var commitId = info.DesiredCommit;

        var repositoryPath = context.LibPath.Combine(repositoryName);

        if (!context.DirectoryExists(repositoryPath))
        {
            context.Log.Information($"Cloning {repositoryName} from {repositoryUrl}");

            Repository.Clone(repositoryUrl, repositoryPath.FullPath, new CloneOptions { RecurseSubmodules = true });
        }

        var repository = new Repository(repositoryPath.FullPath);

        context.Log.Information($"Fetching {repositoryName}");

        Commands.Fetch(repository, "origin", Array.Empty<string>(),
            new FetchOptions { TagFetchMode = TagFetchMode.All }, null);

        var currentCommit = repository.Head.Tip;
        context.Log.Information($"Current commit: {currentCommit.Id.Sha}");
        if (currentCommit.Id.Sha == commitId)
        {
            return;
        }


        context.Log.Information($"Checking out {commitId} of {repositoryName}");
        var localCommit = repository.Lookup<Commit>(commitId);

        Commands.Checkout(repository, localCommit);
    }

    public bool ShouldRun(BuildContext context, DependencyInfo info)
    {
        var repositoryName = info.Name;
        var desiredCommit = info.DesiredCommit;
        var repositoryPath = context.LibPath.Combine(repositoryName);

        if (!context.DirectoryExists(repositoryPath))
        {
            return true;
        }

        var repository = new Repository(repositoryPath.FullPath);

        var currentCommit = repository.Head.Tip;

        return currentCommit.Id.Sha != desiredCommit;
    }
}