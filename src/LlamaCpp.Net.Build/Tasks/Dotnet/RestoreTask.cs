using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks.Dotnet
{
    [TaskName("Dotnet.Restore")]
    public sealed class RestoreTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.DotNetRestore(context.LlamaCppNetDirectory.FullPath, new DotNetRestoreSettings());
        }
    }
}
