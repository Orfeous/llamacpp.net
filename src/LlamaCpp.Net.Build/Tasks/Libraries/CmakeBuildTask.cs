﻿using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks.Libraries
{
    [TaskName("Cmake.Build")]
    [TaskDescription("Builds the C++ project")]
    /*[IsDependentOn(typeof(ConfigureTask))]
    [IsDependentOn(typeof(BuildTask))]
    [IsDependentOn(typeof(MoveToRuntimeDirectoryTask))]*/
    public class CmakeBuildTask : FrostingTask<BuildContext>
    {
    }
}