using Cake.Common.IO;
using LlamaCpp.Net.Build.Tasks.Libraries.Abstractions;

namespace LlamaCpp.Net.Build.Tasks.Libraries.ClBlast;

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