using Cake.Common;
using Cake.Core.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Configuration;

namespace LlamaCpp.Net.Build.Tasks.Libraries.ClBlast.Cmake;

[TaskName("Cmake.ClBlast.Cmake.Configure")]
[IsDependentOn(typeof(CloneClBlastTask))]
public class ConfigureTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        foreach (var setting in context.MsvcBuildSettings)
        {
            Run(context, setting, context.ClBlastDependency);
        }
    }

    private void Run(BuildContext context, MsvcBuildSettings setting, DependencyInfo dependency)
    {
        var options = new CmakeOptions();

        options.Generator = context.MsvcGenerator;
        options.SourcePath = dependency.SourcePath.FullPath;
        options.BuildPath = dependency.GetOutputDirectory(setting).FullPath;


        var arguments = options.Render();

        context.StartProcess("cmake", new ProcessSettings { Arguments = arguments });
    }

    public override bool ShouldRun(BuildContext context)
    {
        return true;
    }
}