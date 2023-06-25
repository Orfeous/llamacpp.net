using Cake.Core;
using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Enviroment;

namespace LlamaCpp.Net.Build.Tasks;

[TaskDescription("Sets up the development environment")]
[IsDependentOn(typeof(BuildLibrariesTask))]
[IsDependentOn(typeof(MoveToRuntimeDirectoryTask))]
public class SetupDevelopmentEnvironmentTask : FrostingTask
{
    public override void Run(ICakeContext context)
    {
    }
}