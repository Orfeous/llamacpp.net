using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Cmake;

namespace LlamaCpp.Net.Build.Tasks
{
    [TaskName("Build")]
    [TaskDescription("Builds the complete project, C++ and .net")]
    [IsDependentOn(typeof(CmakeBuildTask))]
    [IsDependentOn(typeof(Dotnet.BuildTask))]
    public class BuildTask : FrostingTask<BuildContext>
    {
    }
}
