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
            Configure(context, setting, dependency);

            Build(context, setting, dependency);
        }

        private void Configure(BuildContext context, BuildSettings setting, DependencyInfo dependency)
        {
            var options = new CmakeOptions();

            options.Generator = context.MsvcGenerator;
            options.SourcePath = dependency.SourcePath.FullPath;
            options.BuildPath = dependency.GetOutputDirectory(setting).FullPath;

            Configure(options, setting);

            var arguments = options.Render();

            context.StartProcess("cmake", new ProcessSettings { Arguments = arguments });
        }

        protected abstract void Configure(CmakeOptions options, BuildSettings buildSettings);

        private static void Build(BuildContext context, BuildSettings setting, DependencyInfo dependency)
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
