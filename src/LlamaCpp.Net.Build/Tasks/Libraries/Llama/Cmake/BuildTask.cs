using Cake.Common;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks.Libraries.Llama.Cmake
{



    [TaskName("Cmake.Msvc.Build")]
    [IsDependentOn(typeof(ConfigureTask))]
    public sealed class BuildTask : Abstractions.BaseCmakeBuildTask
    {
        public override void Run(BuildContext context)
        {
            foreach (var setting in context.MsvcBuildSettings)
            {
                Run(context, setting, context.LlamaDependency);
            }
        }


        public override bool ShouldRun(BuildContext context)
        {
            return context.IsRunningOnWindows();
        }
    }
}