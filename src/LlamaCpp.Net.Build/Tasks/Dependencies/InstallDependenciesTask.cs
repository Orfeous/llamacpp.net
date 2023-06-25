using Cake.Common;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks.Dependencies
{
    
    public class InstallDependenciesTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Log.Information("Checking dependencies...");

            var cmakeExists = CanResolveTool(context, "cmake", "cmake.exe");

            var gitExists = CanResolveTool(context, "git", "git.exe");

            var cudaToolkitExists = CanResolveTool(context, "nvcc", "nvcc.exe");


            if (context.IsRunningOnWindows())
            {
                if (!cmakeExists)
                {
                    context.WingetInstall("Kitware.CMake");
                }

                if (!gitExists)
                {
                    context.WingetInstall("Git.Git");
                }
            }
            if (context.IsRunningOnLinux())
            {

                var sb = new System.Text.StringBuilder();

                sb.AppendLine("Please install the following packages:");
                sb.AppendLine("cmake");
                sb.AppendLine("git");
                sb.AppendLine("build-essential");
                sb.AppendLine();

                sb.AppendLine("On Ubuntu:");
                sb.AppendLine("sudo apt install --no-install-recommends cmake git build-essential nvidia-cuda-toolkit");


            }

            if (!cudaToolkitExists)
            {
                context.Log.Error("Could not find nvcc.exe. Please install the CUDA toolkit.");
                context.Log.Error("https://developer.nvidia.com/cuda-toolkit");
            }
        }

        private static bool CanResolveTool(ICakeContext context, params string[] args)
        {
            foreach (var s in args)
            {
                var path = context.Tools.Resolve(s);

                if (path != null)
                {
                    context.Log.Information($"Found {s} at {path}");
                    return true;
                }
            }

            context.Log.Error($"Could not find any of the following: {string.Join(", ", args)}");

            return false;
        }
    }
}
