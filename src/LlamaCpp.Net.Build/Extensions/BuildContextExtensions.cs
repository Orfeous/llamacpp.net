using Cake.Common.IO;
using Cake.Core.Diagnostics;

namespace LlamaCpp.Net.Build.Extensions;

public static class BuildContextExtensions
{
    public static void CleanDependencyTmpPath(this BuildContext context, DependencyInfo dependency)
    {
        var contextTmpDir = dependency.BuildPath;
        context.Log.Information($"Cleaning {contextTmpDir.FullPath}");

        if (context.DirectoryExists(contextTmpDir))
        {
            context.DeleteDirectory(contextTmpDir, new DeleteDirectorySettings { Force = true, Recursive = true });
        }

        context.EnsureDirectoryExists(contextTmpDir);

    }


}