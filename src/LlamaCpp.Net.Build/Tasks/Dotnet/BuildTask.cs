using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks.Dotnet
{

    [IsDependentOn(typeof(RestoreTask))]
    public sealed class BuildTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.DotNetBuild(context.LlamaCppNetDirectory.FullPath,
                new DotNetBuildSettings
                {
                    Configuration = context.BuildConfiguration,
                    NoRestore = true,
                    NoIncremental = true
                });
        }
    }
}
