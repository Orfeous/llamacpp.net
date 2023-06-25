using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Extensions;

namespace LlamaCpp.Net.Build.Tasks.Libraries
{
    [TaskDescription("Cleans the build directories")]
    public sealed class CleanTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.CleanDependencyTmpPath(context.LlamaDependency);
            context.CleanDependencyTmpPath(context.OpenBlasDependency);
            context.CleanDependencyTmpPath(context.ClBlastDependency);
            context.CleanDependencyTmpPath(context.BlisDependency);
            context.CleanDependencyTmpPath(context.OpenClDependency);


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
}
