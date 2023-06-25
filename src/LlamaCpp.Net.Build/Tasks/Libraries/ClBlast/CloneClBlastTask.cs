using Cake.Common.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Libraries.Git;

namespace LlamaCpp.Net.Build.Tasks.Libraries.ClBlast;

[TaskName("Git.CloneClBlast")]
public class CloneClBlastTask : CloneTask
{
    public override void Run(BuildContext context)
    {
        context.EnsureDirectoryExists(context.LibPath);

        CloneAndCheckout(context, context.ClBlastDependency);
    }


    public override bool ShouldRun(BuildContext context)
    {
        return ShouldRun(context, context.ClBlastDependency);
    }
}