using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks;

[TaskName("CompleteBuildTask")]
[TaskDescription("Builds the complete project, C++ and .net")]
[IsDependentOn(typeof(BuildLibrariesTask))]
public class CompleteBuildTask : FrostingTask<BuildContext>
{
}