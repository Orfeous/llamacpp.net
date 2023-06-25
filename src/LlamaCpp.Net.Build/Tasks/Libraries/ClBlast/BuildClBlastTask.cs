using Cake.Frosting;
using LlamaCpp.Net.Build.Configuration;
using LlamaCpp.Net.Build.Tasks.Libraries.Abstractions;

namespace LlamaCpp.Net.Build.Tasks.Libraries.ClBlast;

[IsDependentOn(typeof(CloneClBlastTask))]
public class BuildClBlastTask : BaseCmakeBuildTask
{
    public override void Run(BuildContext context)
    {
        foreach (var setting in context.MsvcBuildSettings)
        {
            Run(context, setting, context.ClBlastDependency);
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