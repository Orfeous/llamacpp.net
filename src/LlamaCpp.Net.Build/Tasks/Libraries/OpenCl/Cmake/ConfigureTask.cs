using Cake.Common;
using Cake.Core.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Configuration;

namespace LlamaCpp.Net.Build.Tasks.Libraries.OpenCl.Cmake;

[TaskName("Cmake.OpenCl.Cmake.Configure")]
[IsDependentOn(typeof(CloneOpenClTask))]
public class ConfigureTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        foreach (var setting in context.MsvcBuildSettings)
        {
            Run(context, setting, context.OpenClDependency);
        }


    }

    private void Run(BuildContext context, MsvcBuildSettings setting, DependencyInfo dependency)
    {
        var options = new CmakeOptions();

        options.Generator = context.MsvcGenerator;
        options.SourcePath = dependency.SourcePath.FullPath;
        options.BuildPath = dependency.GetOutputDirectory(setting).FullPath;

        options.Options.Add("BUILD_DOCS", false);
        options.Options.Add("BUILD_EXAMPLES", false);
        options.Options.Add("BUILD_TESTING", false);
        options.Options.Add("OPENCL_SDK_BUILD_SAMPLES", false);
        options.Options.Add("OPENCL_SDK_TEST_SAMPLES", false);

        var arguments = options.Render();

        context.StartProcess("cmake", new ProcessSettings { Arguments = arguments });
    }

    public override bool ShouldRun(BuildContext context)
    {
        return true;
    }
}