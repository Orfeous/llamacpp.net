using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Libraries;
using System;
using System.IO;

namespace LlamaCpp.Net.Build.Tasks
{
    [TaskName("Default")]
    [TaskDescription("A friendly entrypoint for the build system")]
    [IsDependentOn(typeof(BuildLibrariesTask))]
    public class DefaultTask : FrostingTask
    {
        public override void Run(ICakeContext context)
        {
            var filePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "DefaultMessage.txt");
            var message = File.ReadAllText(filePath);

            context.Log.Information(message);
        }
    }

    [TaskName("Build.Libraries")]
    [IsDependentOn(typeof(Libraries.OpenBlas.Cmake.BuildTask))]
    [IsDependentOn(typeof(Libraries.Llama.Cmake.BuildTask))]
    [IsDependentOn(typeof(Libraries.OpenCl.Cmake.BuildTask))]
    [IsDependentOn(typeof(Libraries.ClBlast.Cmake.BuildTask))]
    public class BuildLibrariesTask : FrostingTask
    {
        public override void Run(ICakeContext context)
        {
        }
    }

    [TaskName("SetupDevelopmentEnvironment")]
    [TaskDescription("Sets up the development environment")]
    [IsDependentOn(typeof(BuildLibrariesTask))]
    [IsDependentOn(typeof(MoveToRuntimeDirectoryTask))]
    public class SetupDevelopmentEnvironmentTask : FrostingTask
    {
        public override void Run(ICakeContext context)
        {
        }
    }

    [TaskName("CompleteBuildTask")]
    [TaskDescription("Builds the complete project, C++ and .net")]
    [IsDependentOn(typeof(BuildLibrariesTask))]
    public class CompleteBuildTask : FrostingTask<BuildContext>
    {
    }
}