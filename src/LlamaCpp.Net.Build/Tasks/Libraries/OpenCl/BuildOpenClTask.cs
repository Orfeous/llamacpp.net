using Cake.Frosting;
using LlamaCpp.Net.Build.Configuration;
using LlamaCpp.Net.Build.Tasks.Libraries.Abstractions;

namespace LlamaCpp.Net.Build.Tasks.Libraries.OpenCl;

[IsDependentOn(typeof(CloneOpenClTask))]
public class BuildOpenClTask : BaseCmakeBuildTask
{
    public override void Run(BuildContext context)
    {
        foreach (var setting in context.MsvcBuildSettings)
        {
            Run(context, setting, context.OpenClDependency);
        }
    }


    public override bool ShouldRun(BuildContext context)
    {
        return true;
    }

    protected override void Configure(CmakeOptions options, BuildSettings buildSettings)
    {

        options.Options.Add("BUILD_DOCS", false);
        options.Options.Add("BUILD_EXAMPLES", false);
        options.Options.Add("BUILD_TESTING", false);
        options.Options.Add("OPENCL_SDK_BUILD_SAMPLES", false);
        options.Options.Add("OPENCL_SDK_TEST_SAMPLES", false);

    }
}