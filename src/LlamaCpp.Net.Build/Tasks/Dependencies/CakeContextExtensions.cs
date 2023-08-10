using Cake.Common;
using Cake.Core;
using Cake.Core.IO;

namespace LlamaCpp.Net.Build.Tasks.Dependencies;

public static class CakeContextExtensions
{
    public static void WingetInstall(this ICakeContext context, string packageName)
    {
        context.StartProcess("winget", new ProcessSettings { Arguments = "install " + packageName });
    }
}