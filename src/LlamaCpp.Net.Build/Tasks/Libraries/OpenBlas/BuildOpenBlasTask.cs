using Cake.Frosting;
using LlamaCpp.Net.Build.Configuration;
using LlamaCpp.Net.Build.Tasks.Libraries.Abstractions;

namespace LlamaCpp.Net.Build.Tasks.Libraries.OpenBlas;

[IsDependentOn(typeof(CloneOpenBlasTask))]
public class BuildOpenBlasTask : BaseCmakeBuildTask
{
    public override void Run(BuildContext context)
    {
        foreach (var setting in context.MsvcBuildSettings)
        {
            Run(context, setting, context.OpenBlasDependency);
        }
    }

    public override bool ShouldRun(BuildContext context)
    {
        return true;
    }

    protected override void Configure(CmakeOptions options, BuildSettings buildSettings)
    {

    }
}