using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks
{
    [TaskName("Clean.Library")]
    [TaskDescription("Cleans the build directories")]
    public sealed class CleanTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.CleanDependencyTmpPath(context.LlamaDependency);
            context.CleanDependencyTmpPath(context.OpenBlasDependency);
            context.CleanDependencyTmpPath(context.ClBlastDependency);

        }

        private static void CleanDirectory(BuildContext context, DirectoryPath contextTmpDir)
        {
            context.Log.Information($"Cleaning {contextTmpDir.FullPath}");

            if (context.DirectoryExists(contextTmpDir))
            {
                context.DeleteDirectory(contextTmpDir, new DeleteDirectorySettings { Force = true, Recursive = true });
            }

            context.EnsureDirectoryExists(contextTmpDir);

        }
    }

    public static class BuildContextExtensions
    {
        public static void CleanDependencyTmpPath(this BuildContext context, DependencyInfo dependency)
        {
            var contextTmpDir = dependency.BuildPath;
            context.Log.Information($"Cleaning {contextTmpDir.FullPath}");

            if (context.DirectoryExists(contextTmpDir))
            {
                context.DeleteDirectory(contextTmpDir, new DeleteDirectorySettings { Force = true, Recursive = true });
            }

            context.EnsureDirectoryExists(contextTmpDir);

        }


    }
}
