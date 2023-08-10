using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using LlamaCpp.Net.Build.Configuration;
using LlamaCpp.Net.Build.Tasks.Libraries;
using NuGet.Packaging;
using NuGet.Versioning;
using System;
using System.IO;
using System.Linq;

namespace LlamaCpp.Net.Build.Tasks.Packaging
{
    [TaskName("PackageNuget")]
    [IsDependentOn(typeof(CmakeBuildTask))]
    public class PackageNugetTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            var nugetTemp = context.TmpDir.Combine("nuget");

            context.EnsureDirectoryExists(nugetTemp);

            context.CleanDirectory(nugetTemp);

            PackageRuntimes(context, nugetTemp);
        }

        private static void PackageRuntimes(BuildContext context, DirectoryPath nugetTemp)
        {
            foreach (var groups in context.BuildSettings.GroupBy(settings => settings.PackageName()))
            {
                var id = groups.Key;


                var setting = groups.First();
                var package = new PackageBuilder
                {
                    Id = id,
                    Title = "LlamaCpp.Net ",
                    Properties =
                    {
                        [nameof(setting.Avx512Support)] = setting.Avx512Support.ToString(),
                        [nameof(setting.EnableKQuants)] = setting.EnableKQuants.ToString(),
                        [nameof(setting.BlasType)] = setting.BlasType.ToString()
                    },
                    Version = NuGetVersion.Parse(context.NugetVersion),
                    Owners = { "LlamaCpp.Net" },
                    TargetFrameworks = { new NuGet.Frameworks.NuGetFramework("netstandard", new Version(2, 1)) }
                };
                foreach (var author in context.Authors)
                {
                    package.Authors.Add(author);
                }


                var description =
                    "This is the runtime package for LlamaCpp.Net. It contains the native binaries for the platform \n\n" +
                    "This package uses " + setting.BlasType + " for BLAS operations.\n" +
                    "AVX512 support is " + ((setting.Avx512Support == Avx512Support.None) ? "enabled" : "disabled") +
                    "\n";
                if (setting.EnableKQuants)
                {
                    description +=
                        "This package uses K-Quants for quantization and may not be compatible with all models.\n";
                }


                description += "\n\n" + "This package is generated based on the llama.cpp commit : " +
                               context.LlamaDependency.DesiredCommit;

                package.Description = description;

                foreach (var s in groups)
                {
                    context.Log.Information($"Packing {id}");


                    var buildPath = context.LlamaDependency.GetOutputDirectory(setting)
                        .Combine("bin")
                        .Combine(setting.BuildConfiguration);

                    foreach (var filePath in context.GetFiles(buildPath.FullPath + "/**/*.dll"))
                    {
                        context.Log.Information($"Adding {filePath.FullPath}");

                        // read to memo

                        var stream = File.OpenRead(filePath.FullPath);

                        var memory = new MemoryStream();
                        stream.CopyTo(memory);
                        memory.Position = 0;

                        var targetPath = "";
                        if (s is MsvcBuildSettings settings)
                        {
                            targetPath = $"runtimes/{settings.Triplet}/native/{filePath.GetFilename().FullPath}";

                        }

                        else
                        {
                            targetPath = $"runtimes/native/{filePath.GetFilename().FullPath}";

                        }

                        var packageFile = new PhysicalPackageFile(memory)
                        {
                            SourcePath = filePath.FullPath,
                            TargetPath = targetPath
                        };


                        package.Files.Add(packageFile);
                    }
                }

                var nuspecPath = nugetTemp.Combine($"{id}.nupkg");

                using var file = File.Create(nuspecPath.FullPath);

                package.Save(file);
            }
        }

        public override bool ShouldRun(BuildContext context)
        {
            return false;
        }
    }
}
