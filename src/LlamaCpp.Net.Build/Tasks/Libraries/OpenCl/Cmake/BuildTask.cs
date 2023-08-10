using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Libraries.Abstractions;

namespace LlamaCpp.Net.Build.Tasks.Libraries.OpenCl.Cmake;

[TaskName("Cmake.OpenCl.Cmake.Build")]
[IsDependentOn(typeof(ConfigureTask))]
public class BuildTask : BaseCmakeBuildTask
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
}