using Cake.Common.IO;
using LlamaCpp.Net.Build.Tasks.Libraries.Abstractions;

namespace LlamaCpp.Net.Build.Tasks.Libraries.OpenBlas;

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