using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Libraries;
using LlamaCpp.Net.Build.Tasks.Packaging;

namespace LlamaCpp.Net.Build.Tasks;

[TaskDescription("Builds everything" +
                 "Runs for the CI pipeline")]
[IsDependentOn(typeof(CleanTask))]
[IsDependentOn(typeof(BuildLibrariesTask))]
[IsDependentOn(typeof(Dotnet.BuildTask))]
[IsDependentOn(typeof(PackageNugetTask))]
public class CompleteBuildTask : FrostingTask<BuildContext>
{
}