using Cake.Common.IO;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks.Libraries.Blis;

[TaskName("Git.Blis")]
public class CloneBlisTask : CloneTask
{
    public override void Run(BuildContext context)
    {
        context.EnsureDirectoryExists(context.LibPath);

        CloneAndCheckout(context, context.BlisDependency);
    }

    public override bool ShouldRun(BuildContext context)
    {
        return ShouldRun(context, context.BlisDependency);
    }
}