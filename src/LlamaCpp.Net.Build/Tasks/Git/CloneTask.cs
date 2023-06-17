using System;
using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using LibGit2Sharp;

namespace LlamaCpp.Net.Build.Tasks.Git
{
    [TaskName("Git.Clone")]
    [TaskDescription("Downloads the LlamaCpp repository and checks out the correct commit")]
    public sealed class CloneTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.EnsureDirectoryExists(context.LibPath);

            CloneAndCheckout(context, context.LlamaRepositoryName, context.LlamaRepositoryUrl,
                context.LlamaCppCommitSha);
        }

        private static void CloneAndCheckout(BuildContext context, string repositoryName, string repositoryUrl,
            string commitId)
        {
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
            if (currentCommit.Id.Sha == context.LlamaCppCommitSha)
            {
                return;
            }


            context.Log.Information($"Checking out {commitId} of {repositoryName}");
            var localCommit = repository.Lookup<Commit>(commitId);
            Commands.Checkout(repository, localCommit);
        }

        public override bool ShouldRun(BuildContext context)
        {
            var repositoryPath = context.LibPath.Combine(context.LlamaRepositoryName);

            if (!context.DirectoryExists(repositoryPath))
            {
                return true;
            }

            var repository = new Repository(repositoryPath.FullPath);

            var currentCommit = repository.Head.Tip;

            return currentCommit.Id.Sha != context.LlamaCppCommitSha;

        }
    }
}
