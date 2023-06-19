using System;
using System.IO;
using System.Linq;
using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using LlamaCpp.Net.Build.Tasks.Cmake;
using NuGet.Packaging;
using NuGet.Versioning;

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

            foreach (var groups in context.BuildSettings.GroupBy(settings => settings.PackageName()))
            {
                var id = groups.Key;


                var setting = groups.First();
                var package = new PackageBuilder
                {
                    Id = id,
                    Description = "LlamaCpp.Net Runtime",
                    Title = "LlamaCpp.Net Runtime",
                    Properties =
                    {
                        [nameof(setting.Avx512Support)] = setting.Avx512Support.ToString(),
                        [nameof(setting.EnableKQuants)] = setting.EnableKQuants.ToString(),
                        [nameof(setting.BlasType)] = setting.BlasType.ToString()
                    },
                    Version = NuGetVersion.Parse("1.0.0"),
                    Authors = { "LlamaCpp.Net" },
                    Owners = { "LlamaCpp.Net" },
                    TargetFrameworks = { new NuGet.Frameworks.NuGetFramework("netstandard", new Version(2, 1)) }
                };
                foreach (var s in groups)
                {
                    context.Log.Information($"Packing {id}");


                    var buildPath = context.GetOutputDirectory(setting)
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

                        var packageFile = new PhysicalPackageFile(memory)
                        {
                            SourcePath = filePath.FullPath,
                            TargetPath = $"runtimes/{setting.Platform}/native/{filePath.GetFilename().FullPath}"
                        };


                        package.Files.Add(packageFile);
                    }
                }

                var nuspecPath = nugetTemp.Combine($"{id}.nupkg");

                using var file = File.Create(nuspecPath.FullPath);

                package.Save(file);
            }
        }
    }
}
