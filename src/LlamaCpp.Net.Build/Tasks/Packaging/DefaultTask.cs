using System;
using System.IO;
using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using NuGet.Packaging;
using NuGet.Versioning;

namespace LlamaCpp.Net.Build.Tasks.Packaging
{
    [TaskName("PackageNuget")]
    public class PackageNugetTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            var nugetTemp = context.TmpDir.Combine("nuget");

            context.EnsureDirectoryExists(nugetTemp);

            context.CleanDirectory(nugetTemp);
            foreach (var setting in context.BuildSettings)
            {
                var id = $"LlamaCpp.Net.Runtime-{setting.GetName()}";
                context.Log.Information($"Packing {id}");


                var nuspecDirectoryPath = nugetTemp.Combine(id);

                context.EnsureDirectoryExists(nuspecDirectoryPath);


                var package = new PackageBuilder()
                {
                    Id = id,
                    Description = "LlamaCpp.Net Runtime",
                    Title = "LlamaCpp.Net Runtime",
                };
                // add setting metadata to properties

                package.Properties[nameof(setting.Avx512Support)] = setting.Avx512Support.ToString();
                package.Properties[nameof(setting.EnableKQuants)] = setting.EnableKQuants.ToString();
                package.Properties[nameof(setting.BlasType)] = setting.BlasType.ToString();


                package.TargetFrameworks.Add(new NuGet.Frameworks.NuGetFramework("netstandard", new Version(2, 1)));


                package.Version = NuGetVersion.Parse("1.0.0");
                package.Authors.Add("LlamaCpp.Net");
                package.Owners.Add("LlamaCpp.Net");
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

                var nuspecPath = nuspecDirectoryPath.Combine($"{id}.nupkg");

                using var file = File.Create(nuspecPath.FullPath);

                package.Save(file);
            }
        }
    }
}
