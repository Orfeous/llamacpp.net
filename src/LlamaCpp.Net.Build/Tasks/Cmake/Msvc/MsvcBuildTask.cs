using Cake.Common;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Configuration;

namespace LlamaCpp.Net.Build.Tasks.Cmake.Msvc
{
    [TaskName("Cmake.Msvc.Build")]
    [IsDependentOn(typeof(ConfigureTask))]
    public sealed class BuildTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            foreach (var setting in context.MsvcBuildSettings)
            {
                Run(context, setting);
            }
        }

        private void Run(BuildContext context, MsvcBuildSettings setting)
        {
            var processParameterBuilder = new ProcessArgumentBuilder();

            processParameterBuilder.Append("--build");
            processParameterBuilder.AppendQuoted(context.LlamaBuildDirectory.Combine(setting.BuildPath).FullPath);
            processParameterBuilder.Append("--parallel");
            processParameterBuilder.Append("--config ");
            processParameterBuilder.AppendQuoted(setting.BuildConfiguration);


            var process = context.StartProcess("cmake",
                new ProcessSettings
                {
                    WorkingDirectory = context.LlamaBuildDirectory,
                    Arguments = processParameterBuilder.Render()
                });
            if (process != 0)
            {
                throw new CakeException("Cmake build failed");
            }
        }

        public override bool ShouldRun(BuildContext context)
        {
            return context.IsRunningOnWindows();
        }
    }
}
