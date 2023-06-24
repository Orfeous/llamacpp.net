using Cake.Common.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Libraries.Git;

namespace LlamaCpp.Net.Build.Tasks.Libraries.OpenBlas;

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