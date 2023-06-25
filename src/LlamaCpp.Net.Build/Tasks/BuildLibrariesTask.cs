using Cake.Core;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks;

[TaskDescription("Builds the libraries")]
[IsDependentOn(typeof(Libraries.OpenBlas.BuildOpenBlasTask))]
[IsDependentOn(typeof(Libraries.Llama.BuildLlamaTask))]
[IsDependentOn(typeof(Libraries.OpenCl.BuildOpenClTask))]
[IsDependentOn(typeof(Libraries.ClBlast.BuildClBlastTask))]
public class BuildLibrariesTask : FrostingTask
{
    public override void Run(ICakeContext context)
    {
    }
}