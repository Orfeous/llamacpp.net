using Cake.Common;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Configuration;

namespace LlamaCpp.Net.Build.Tasks.Libraries.Abstractions
{
    public abstract class BaseCmakeBuildTask : FrostingTask<BuildContext>
    {

        protected void Run(BuildContext context, BuildSettings setting, DependencyInfo dependency)
        {
            var processParameterBuilder = new ProcessArgumentBuilder();

            processParameterBuilder.Append("--build");
            processParameterBuilder.AppendQuoted(dependency.GetOutputDirectory(setting).FullPath);
            processParameterBuilder.Append("--parallel");
            processParameterBuilder.Append("--config ");
            processParameterBuilder.AppendQuoted(setting.BuildConfiguration);


            var process = context.StartProcess("cmake",
                new ProcessSettings
                {
                    WorkingDirectory = dependency.BuildPath,
                    Arguments = processParameterBuilder.Render()
                });
            if (process != 0)
            {
                throw new CakeException("Cmake build failed");
            }
        }
    }
}
