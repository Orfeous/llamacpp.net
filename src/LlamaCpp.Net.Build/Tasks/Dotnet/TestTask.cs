using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Common.Tools.DotNet.Test;
using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Dependencies;

namespace LlamaCpp.Net.Build.Tasks.Dotnet
{
    [TaskName("Dotnet.Test")]
    [IsDependentOn(typeof(DownloadTestModelTask))]
    public sealed class TestTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.DotNetRestore(context.LlamaCppNetTestDirectory.FullPath, new DotNetRestoreSettings());
            context.DotNetBuild(context.LlamaCppNetTestDirectory.FullPath,
                new DotNetBuildSettings
                {
                    Configuration = context.BuildConfiguration, NoRestore = true, NoIncremental = true
                });
            context.DotNetTest(context.LlamaCppNetTestDirectory.FullPath,
                new DotNetTestSettings
                {
                    Configuration = context.BuildConfiguration, NoRestore = true, NoBuild = true
                });
        }
    }
}
