using Cake.Common.IO;
using LlamaCpp.Net.Build.Tasks.Libraries.Abstractions;

namespace LlamaCpp.Net.Build.Tasks.Libraries.OpenCl;

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