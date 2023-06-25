using Cake.Common.IO;
using LlamaCpp.Net.Build.Tasks.Libraries.Abstractions;

namespace LlamaCpp.Net.Build.Tasks.Libraries.Blis;

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