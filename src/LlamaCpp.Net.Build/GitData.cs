using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

namespace LlamaCpp.Net.Build
{
    public class GitData
    {
        public GitData(BuildContext context)
        {
            var repo = new Repository(context.RepositoryRoot.FullPath);

            Authors = repo.Commits.Select(commit => commit.Author.Name).Distinct();
        }


        public string NugetVersion { get; set; } = "1.0.0";
        public IEnumerable<string> Authors { get; set; }
    }
}
