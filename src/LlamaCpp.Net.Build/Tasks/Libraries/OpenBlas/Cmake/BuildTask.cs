using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Libraries.Abstractions;

namespace LlamaCpp.Net.Build.Tasks.Libraries.OpenBlas.Cmake;

[TaskName("Cmake.OpenBlas.Cmake.Build")]
[IsDependentOn(typeof(ConfigureTask))]
public class BuildTask : BaseCmakeBuildTask
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
}