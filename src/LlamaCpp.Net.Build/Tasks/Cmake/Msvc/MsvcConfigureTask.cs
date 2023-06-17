using Cake.Common;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Configuration;
using LlamaCpp.Net.Build.Extensions;
using LlamaCpp.Net.Build.Tasks.Dependencies;
using LlamaCpp.Net.Build.Tasks.Git;

namespace LlamaCpp.Net.Build.Tasks.Cmake.Msvc
{
    [TaskName("Cmake.Msvc.Configure")]
    [IsDependentOn(typeof(CleanTask))]
    [IsDependentOn(typeof(CloneTask))]
    [IsDependentOn(typeof(DownloadClBlastTask))]
    public sealed class ConfigureTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            foreach (var setting in context.MsvcBuildSettings)
            {
                context.Log.Information($"Configuring {setting.BuildPath.FullPath}");

                Run(context, setting);
            }
        }

        private void Run(BuildContext context, MsvcBuildSettings setting)
        {
            var processParameterBuilder = new ProcessArgumentBuilder();

            processParameterBuilder.Append("-G ");
            processParameterBuilder.AppendQuoted(context.MsvcGenerator);

            processParameterBuilder.Append("-S ");
            processParameterBuilder.AppendQuoted(context.LlamaSourceDirectory.FullPath);
            processParameterBuilder.Append("-B ");
            processParameterBuilder.AppendQuoted(context.LlamaBuildDirectory.Combine(setting.BuildPath).FullPath);
            processParameterBuilder.AppendCmakeOption("BUILD_SHARED_LIBS", true);

            processParameterBuilder.AppendCmakeOption("LLAMA_ALL_WARNINGS", true);
            setting.Apply(processParameterBuilder);

            context.StartProcess("cmake", new ProcessSettings { Arguments = processParameterBuilder.Render() });
        }
        public override bool ShouldRun(BuildContext context)
        {
            return context.IsRunningOnWindows();
        }
    }
}
