using Cake.Common;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using LibGit2Sharp;
using LlamaCpp.Net.Build.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace LlamaCpp.Net.Build;

public class BuildContext : FrostingContext
{
    public BuildContext(ICakeContext context)
        : base(context)
    {
        // paths
        RepositoryRoot = GetRepositoryRoot(context);

        TmpDir = RepositoryRoot.Combine("tmp");
        RuntimeDirectory = RepositoryRoot.Combine("runtimes");

        LlamaCppNetDirectory = RepositoryRoot.Combine("src").Combine("LlamaCpp.Net");
        LlamaCppNetTestDirectory = RepositoryRoot.Combine("src").Combine("LlamaCpp.Net.Test");

        SolutionPath = RepositoryRoot.CombineWithFilePath("LlamaCpp.Net.sln");
        ModelDirectory = RepositoryRoot.Combine("models");
        // arguments
        MsvcGenerator = context.Argument("msvc-generator", "Visual Studio 17 2022");
        BuildConfiguration = context.Argument("build-configuration", "Release");
        OpenClVersion = context.Argument("opencl-version", "2023.04.17");
        // settings
        BuildSettings = GetBuildSettings();
        // "llama.cpp", "https://github.com/ggerganov/llama.cpp",
        LlamaDependency = new DependencyInfo
        {
            Name = "llama.cpp",
            RepositoryUrl = "https://github.com/ggerganov/llama.cpp",
            DesiredCommit = context.Argument("llama-cpp-commit-sha", "7e4ea5beff567f53be92f75f9089e6f11fa5dabd"),
            SourcePath = RepositoryRoot.Combine("lib").Combine("llama.cpp"),
            BuildPath = TmpDir.Combine("llama.cpp").Combine("build")
        };

        OpenBlasDependency = new DependencyInfo
        {
            Name = "OpenBlas",
            RepositoryUrl = "https://github.com/xianyi/OpenBLAS",
            DesiredCommit = context.Argument("openblas-commit-sha", "9487046fe02614fe307845ccf7931667037c4038"),
            SourcePath = RepositoryRoot.Combine("lib").Combine("OpenBlas"),
            BuildPath = TmpDir.Combine("OpenBlas").Combine("build")
        };

        ClBlastDependency = new DependencyInfo
        {
            Name = "ClBlast",
            RepositoryUrl = "https://github.com/CNugteren/CLBlast",
            DesiredCommit = context.Argument("clblast-commit-sha", "28a61c53a69ad598cd3ed8992fb6be88643f3c4b"),
            SourcePath = RepositoryRoot.Combine("lib").Combine("ClBlast"),
            BuildPath = TmpDir.Combine("ClBlast").Combine("build")
        };

        OpenClDependency = new DependencyInfo
        {
            Name = "OpenCl",
            RepositoryUrl = "https://github.com/KhronosGroup/OpenCL-SDK",
            DesiredCommit = "ae7fcae82fe0b7bcc272e43fc324181b2d544eea",
            SourcePath = RepositoryRoot.Combine("lib").Combine("OpenCl"),
            BuildPath = TmpDir.Combine("OpenCl").Combine("build")
        };

        BlisDependency = new DependencyInfo
        {
            Name = "Blis",
            RepositoryUrl = "https://github.com/flame/blis",
            SourcePath = RepositoryRoot.Combine("lib").Combine("Blis"),
            BuildPath = TmpDir.Combine("Blis").Combine("build"),
            DesiredCommit = "6b894c30b9bb2c2518848d74e4c8d96844f77f24"
        };
        var repo = new Repository(RepositoryRoot.FullPath);

        Authors = repo.Commits.Select(commit => commit.Author.Name).Distinct();
    }

    public DependencyInfo BlisDependency { get; set; }

    public IEnumerable<string> Authors { get; set; }

    public DependencyInfo OpenClDependency { get; }

    public DependencyInfo ClBlastDependency { get; }

    public DependencyInfo OpenBlasDependency { get; }

    public DependencyInfo LlamaDependency { get; }


    public DirectoryPath LlamaCppNetTestDirectory { get; }

    public FilePath SolutionPath { get; set; }

    public DirectoryPath LlamaCppNetDirectory { get; }

    public DirectoryPath RepositoryRoot { get; }

    public List<BuildSettings> BuildSettings { get; }

    public IEnumerable<MsvcBuildSettings> MsvcBuildSettings => BuildSettings.OfType<MsvcBuildSettings>().ToList();

    public IEnumerable<ClangBuildSettings> ClangBuildSettings =>
        BuildSettings.OfType<ClangBuildSettings>().ToList();

    public string MsvcGenerator { get; }

    public DirectoryPath LibPath => RepositoryRoot.Combine("lib");

    public DirectoryPath TmpDir { get; }
    public string BuildConfiguration { get; }
    public DirectoryPath RuntimeDirectory { get; }
    public DirectoryPath ModelDirectory { get; set; }
    public string OpenClVersion { get; }
    public string NugetVersion { get; set; } = "1.0.0";

    private static List<BuildSettings> GetBuildSettings()
    {
        var list = new List<BuildSettings>
        {
            new MsvcBuildSettings
            {

                Triplet = "x86_64-pc-windows-msvc",
                BuildConfiguration = "Release",
                Avx512Support = Avx512Support.Avx512 | Avx512Support.Vbmi | Avx512Support.Vnni,
                BlasType =
                    BlasType.CuBlas,
                EnableKQuants = false
            }
        };


        return list;
    }

    private static DirectoryPath GetRepositoryRoot(ICakeContext context)
    {
        var directoryPath = context.Environment.WorkingDirectory;

        while (!context.DirectoryExists(directoryPath.Combine(".git")))
        {
            directoryPath = directoryPath.GetParent();
        }

        return directoryPath;
    }


}