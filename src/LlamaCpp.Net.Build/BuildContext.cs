using System.Collections.Generic;
using System.Linq;
using Cake.Common;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Configuration;

namespace LlamaCpp.Net.Build
{
    public class BuildContext : FrostingContext
    {
        public string LlamaRepositoryName = "llama.cpp";
        public string LlamaRepositoryUrl = "https://github.com/ggerganov/llama.cpp";

        public BuildContext(ICakeContext context)
            : base(context)
        {
            // paths
            RepositoryRoot = GetRepositoryRoot(context);
            LibPath = RepositoryRoot.Combine("lib");
            TmpDir = RepositoryRoot.Combine("tmp");
            RuntimeDirectory = RepositoryRoot.Combine("runtimes");
            LlamaCppNetDirectory = RepositoryRoot.Combine("src").Combine("LlamaCpp.Net");
            LlamaCppNetTestDirectory = RepositoryRoot.Combine("src").Combine("LlamaCpp.Net.Test");
            SolutionPath = RepositoryRoot.CombineWithFilePath("LlamaCpp.Net.sln");
            ModelDirectory = RepositoryRoot.Combine("models");
            // arguments
            LlamaCppCommitSha = context.Argument("llama-cpp-commit-sha", "7e4ea5beff567f53be92f75f9089e6f11fa5dabd");
            MsvcGenerator = context.Argument("msvc-generator", "Visual Studio 17 2022");
            BuildConfiguration = context.Argument("build-configuration", "Release");
            OpenClVersion = context.Argument("opencl-version", "2023.04.17");
            // settings
            BuildSettings = GetBuildSettings();
            this.GitData = new GitData(this);
        }

        public GitData GitData { get; set; }

        private static List<BuildSettings> GetBuildSettings()
        {
            var list = new List<BuildSettings>
            {
                new MsvcBuildSettings
                {
                    Platform = "X64",
                    BuildConfiguration = "Release",
                    Avx512Support = Avx512Support.Avx512 | Avx512Support.Vbmi | Avx512Support.Vnni,
                    BlasType =
                        BlasType.CuBlas,
                    EnableKQuants = false,
                },
                new MsvcBuildSettings
                {
                    Platform = "X64",
                    BuildConfiguration = "Release",
                    Avx512Support = Avx512Support.Avx512 | Avx512Support.Vbmi | Avx512Support.Vnni,
                    BlasType =
                        BlasType.CuBlas,
                    EnableKQuants = true,
                },
            };


            return list;
        }

        public DirectoryPath LlamaCppNetTestDirectory { get; set; }

        public FilePath SolutionPath { get; set; }

        public DirectoryPath LlamaCppNetDirectory { get; set; }

        public DirectoryPath RepositoryRoot { get; init; }

        public List<BuildSettings> BuildSettings { get; init; }

        public List<MsvcBuildSettings> MsvcBuildSettings => BuildSettings.OfType<MsvcBuildSettings>().ToList();
        public string MsvcGenerator { get; init; }

        public DirectoryPath LibPath { get; init; }

        public DirectoryPath TmpDir { get; init; }
        public string LlamaCppCommitSha { get; init; }
        public DirectoryPath LlamaSourceDirectory => LibPath.Combine(LlamaRepositoryName);
        public DirectoryPath LlamaBuildDirectory => TmpDir.Combine("llama.cpp").Combine("build");
        public string BuildConfiguration { get; init; }
        public DirectoryPath RuntimeDirectory { get; init; }
        public DirectoryPath ModelDirectory { get; set; }
        public string OpenClVersion { get; set; }

        private static DirectoryPath GetRepositoryRoot(ICakeContext context)
        {
            var directoryPath = context.Environment.WorkingDirectory;

            while (!context.DirectoryExists(directoryPath.Combine(".git")))
            {
                directoryPath = directoryPath.GetParent();
            }

            return directoryPath;
        }

        public DirectoryPath GetOutputDirectory(BuildSettings setting)
        {
            return LlamaBuildDirectory.Combine(setting.BuildPath);
        }
    }
}
