using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks.Dotnet;

[IsDependentOn(typeof(BuildTask))]
[IsDependentOn(typeof(TestTask))]
public sealed class PublishTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.EnsureDirectoryExists(context.TmpDir.Combine("nuget"));


        var settings = new DotNetPackSettings
        {
            Configuration = context.BuildConfiguration,
            NoBuild = true,
            NoRestore = true,

            MSBuildSettings = new DotNetMSBuildSettings()
                .AppendVersionArguments(context.Version),

            OutputDirectory = context.TmpDir.Combine("nuget"),


        };


        context.DotNetPack(context.LlamaCppNetDirectory.FullPath, settings);



    }
}