using Cake.Common.IO;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks.Libraries.Blis
{
    [TaskName("Git.CloneLlamaCpp")]
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