using Cake.Common.IO;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks.Libraries.Blis;

[TaskName("Git.CloneOpenBlas")]
public class CloneOpenBlasTask : CloneTask
{
    public override void Run(BuildContext context)
    {
        context.EnsureDirectoryExists(context.LibPath);

        CloneAndCheckout(context, context.OpenBlasDependency);
    }

    public override bool ShouldRun(BuildContext context)
    {
        return ShouldRun(context, context.OpenBlasDependency);
    }
}