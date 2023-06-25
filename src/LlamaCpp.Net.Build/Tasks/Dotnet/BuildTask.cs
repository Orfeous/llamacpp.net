using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks.Dotnet
{
    [IsDependentOn(typeof(RestoreTask))]
    public sealed class BuildTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            var version = context.Version;
            var settings = new DotNetBuildSettings
            {
                Configuration = context.BuildConfiguration,
                NoRestore = true,
                NoIncremental = true,
                MSBuildSettings = new DotNetMSBuildSettings()
                    .AppendVersionArguments(version)
            };


            context.DotNetBuild(context.LlamaCppNetDirectory.FullPath, settings);
        }
    }
}