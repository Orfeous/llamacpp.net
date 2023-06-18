using Cake.Common;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Cmake;

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
