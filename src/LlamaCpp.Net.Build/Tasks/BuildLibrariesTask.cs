using System.Collections.Generic;
using Cake.Common;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using Path = System.IO.Path;

namespace LlamaCpp.Net.Build.Tasks;

[TaskName("Build.Libraries")]
[IsDependentOn(typeof(BuildAllTask))]
public class BuildLibrariesTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {

    }
}

public enum Platform
{
    Windows,
    Linux,
    MacOs
}

[TaskName("Build-All")]
public sealed class BuildAllTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var platforms = new List<Platform> { Platform.Windows, Platform.Linux };

        var architectures = new List<string> { "x64", "x86" };

        BuildPlatform(context, Platform.Linux, "x64");
        /*
        BuildPlatform(context, Platform.Windows, "x86");
        BuildPlatform(context, Platform.Windows, "x64", true);
        */





    }

    private void BuildPlatform(BuildContext context, Platform targetPlatform, string arch, bool cublas = false,
        bool clblast = false)
    {
        context.Log.Information($"Building binaries for {arch} with cublas: {cublas}, and clblast: {clblast}");

        var platform = arch;
        if (targetPlatform == Platform.Windows)
        {
            platform = GetMSBuildPlatform(arch);

        }
    


        if (string.IsNullOrEmpty(platform))
        {
            context.Log.Information($"Unknown architecture {arch}");
            return;
        }

        var options = new ProcessArgumentBuilder();
        options.Append("-S");
        options.Append(".");

        options.Append("-DBUILD_SHARED_LIBS=1");

        var identifier = GetIdentifier(targetPlatform, arch);

        if (cublas)
        {
            options.Append("-DLLAMA_CUBLAS=1");
            identifier += "-cublas";
        }

        if (clblast)
        {
            options.Append("-DLLAMA_CLBLAST=1");
            identifier += "-clblast";
        }


        if (targetPlatform == Platform.Linux)
        {
            options = BuildLinuxArguments(context, options, arch, cublas, clblast, identifier);
        }
        else
        {
            options = BuildWindowsArguments(context, options, arch, cublas, clblast, identifier);
        }

        var buildDirectory = context.TmpDir.Combine("build").Combine(identifier);

        options.Append("-B");
        options.AppendQuoted(buildDirectory.FullPath);
       

        if (context.DirectoryExists(buildDirectory.FullPath))
        {
            context.Log.Information($"Deleting old build files for {buildDirectory}");
            context.DeleteDirectory(buildDirectory.FullPath,
                new EnsureDirectoryDoesNotExistSettings { Recursive = true });
        }

        context.CreateDirectory(buildDirectory);

        // Call CMake to generate the makefiles
        context.Log.Information($"Running 'cmake {options.Render()}'");

        context.StartProcess("cmake", new ProcessSettings
        {
            Arguments = options.Render(),
            RedirectStandardOutput = true,
            WorkingDirectory = context.LibPath.FullPath
        });

        var outputPath = context.TmpDir.Combine("output").Combine(identifier);

        context.CreateDirectory(outputPath);
        context.CleanDirectory(outputPath);

        context.StartProcess("cmake", new ProcessSettings
        {
            Arguments = $"--build {buildDirectory.FullPath} --config Release",
            RedirectStandardOutput = true
        });
    }

    private static string GetIdentifier(Platform targetPlatform, string arch)
    {
        var identifier = "";
        if (targetPlatform == Platform.Windows)
        {
            identifier = "win";
        }
        else if (targetPlatform == Platform.Linux)
        {
            identifier = "linux";
        }
        else if (targetPlatform == Platform.MacOs)
        {
            identifier = "macos";
        }

        identifier += "-";
        identifier += arch;
        return identifier;
    }

    private ProcessArgumentBuilder BuildLinuxArguments(BuildContext context, ProcessArgumentBuilder options,
        string arch, bool cublas,
        bool clblast, string identifier)
    {
        options.Append("-DCMAKE_SYSTEM_NAME=Linux");
        options.Append("-DCMAKE_SYSTEM_PROCESSOR=x86_64");

        if (arch == "arm64")
        {
            options.Append("-DCMAKE_C_COMPILER=aarch64-linux-gnu-gcc");
            options.Append("-DCMAKE_CXX_COMPILER=aarch64-linux-gnu-g++");
            options.Append("-DCMAKE_SYSTEM_PROCESSOR=aarch64");
        }
        else if (arch == "arm")
        {
            options.Append("-DCMAKE_C_COMPILER=arm-linux-gnueabihf-gcc");
            options.Append("-DCMAKE_CXX_COMPILER=arm-linux-gnueabihf-g++");
            options.Append("-DCMAKE_SYSTEM_PROCESSOR=arm");
        }

        var runtimePath = new DirectoryPath(Path.Combine(".", "Whisper.net.Runtime"));
        if (cublas)
        {
            runtimePath = new DirectoryPath(runtimePath.FullPath + ".Cublas");
        }

        if (clblast)
        {
            runtimePath = new DirectoryPath(runtimePath.FullPath + ".Clblast");
        }

        return options;
    }

    private ProcessArgumentBuilder BuildWindowsArguments(BuildContext context, ProcessArgumentBuilder options,
        string arch, bool cublas,
        bool clblast, string identifier)
    {
        var platform = GetMSBuildPlatform(arch);
        if (string.IsNullOrEmpty(platform))
        {
            context.Log.Information($"Unknown architecture {arch}");
        }



        var runtimePath = new DirectoryPath(Path.Combine(".", "Whisper.net.Runtime"));
        if (cublas)
        {
            runtimePath = new DirectoryPath(runtimePath.FullPath + ".Cublas");
        }

        if (clblast)
        {
            runtimePath = new DirectoryPath(runtimePath.FullPath + ".Clblast");
        }

        return options;
    }

    private string GetMSBuildPlatform(string arch)
    {
        var platforms = new Dictionary<string, string>
        {
            { "x64", "x64" },
            { "x86", "Win32" },
            { "arm64", "ARM64" },
            { "arm", "ARM" }
        };

        if (platforms.ContainsKey(arch))
        {
            return platforms[arch];
        }

        return null;
    }
}