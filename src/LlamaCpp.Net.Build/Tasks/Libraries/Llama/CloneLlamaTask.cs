using Cake.Common.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Libraries.Abstractions;

namespace LlamaCpp.Net.Build.Tasks.Libraries.Llama
{
    [TaskDescription("Downloads the LlamaCpp repository and checks out the correct commit")]
    public class CloneLlamaTask : CloneTask
    {
        public override void Run(BuildContext context)
        {
            context.EnsureDirectoryExists(context.LibPath);

            CloneAndCheckout(context, context.LlamaDependency);
        }


        public override bool ShouldRun(BuildContext context)
        {
            return ShouldRun(context, context.LlamaDependency);
        }
    }
}