using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Cmake.Msvc;

namespace LlamaCpp.Net.Build.Tasks.Cmake
{
    [TaskName("Cmake.Build")]
    [TaskDescription("Builds the C++ project")]
    [IsDependentOn(typeof(ConfigureTask))]
    [IsDependentOn(typeof(BuildTask))]
    [IsDependentOn(typeof(MoveToRuntimeDirectoryTask))]
    public class CmakeBuildTask : FrostingTask<BuildContext>
    {
    }
}
