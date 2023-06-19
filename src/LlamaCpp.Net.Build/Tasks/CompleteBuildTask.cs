using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks
{
    [TaskName("CompleteBuildTask")]
    [TaskDescription("Builds the complete project, C++ and .net")]
    [IsDependentOn(typeof(Git.CloneTask))]
    [IsDependentOn(typeof(CleanTask))]
    [IsDependentOn(typeof(Cmake.CmakeBuildTask))]

    public class CompleteBuildTask : FrostingTask<BuildContext>
    {
    }
}
