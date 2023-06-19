using System.Linq;
using Cake.Common;
using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using LlamaCpp.Net.Build.Configuration;

namespace LlamaCpp.Net.Build.Tasks.Cmake
{
    [TaskName("Cmake.MoveToRuntimeDirectory")]
    [TaskDescription("Moves the C++ build output to the runtime directory, so it can be used by the application")]
    [IsDependentOn(typeof(Msvc.BuildTask))]

    public sealed class MoveToRuntimeDirectoryTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            foreach (var setting in context.BuildSettings)
            {
                Run(context, setting);
            }
        }

        private static void Run(BuildContext context, BuildSettings setting)
        {
            context.EnsureDirectoryExists(context.RuntimeDirectory);


            var runtimeIdentifier = "win-x64";

            var runtimeDirectory = context.RuntimeDirectory.Combine(runtimeIdentifier);

            context.EnsureDirectoryExists(runtimeDirectory);

            var sourceDirectory = context.LlamaBuildDirectory.Combine(setting.BuildPath).Combine("bin")
                .Combine(context.MsvcBuildSettings.First().BuildConfiguration);

            context.Log.Information($"Moving {sourceDirectory.FullPath} to {runtimeDirectory.FullPath}");

            context.CopyDirectory(sourceDirectory, runtimeDirectory);
        }

        public override bool ShouldRun(BuildContext context)
        {
            return context.IsRunningOnWindows();
        }
    }
}
