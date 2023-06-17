using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks
{
    [TaskName("Clean")]
    [TaskDescription("Cleans the build directories")]
    public sealed class CleanTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            CleanDirectory(context, context.TmpDir);
            CleanDirectory(context, context.RuntimeDirectory);
        }

        private void CleanDirectory(BuildContext context, DirectoryPath contextTmpDir)
        {
            context.Log.Information($"Cleaning {contextTmpDir.FullPath}");

            context.EnsureDirectoryDoesNotExist(context.TmpDir);
            context.EnsureDirectoryExists(context.TmpDir);
        }
    }
}
