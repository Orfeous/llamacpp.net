using Cake.Common.IO;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks.Libraries.Blis;

[TaskName("Git.OpenCl")]
public class CloneOpenClTask : CloneTask
{
    public override void Run(BuildContext context)
    {
        context.EnsureDirectoryExists(context.LibPath);

        CloneAndCheckout(context, context.OpenClDependency);
    }

    public override bool ShouldRun(BuildContext context)
    {
        return ShouldRun(context, context.OpenClDependency);
    }
}