using Cake.Common.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Libraries.Git;

namespace LlamaCpp.Net.Build.Tasks.Libraries.OpenCl;

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