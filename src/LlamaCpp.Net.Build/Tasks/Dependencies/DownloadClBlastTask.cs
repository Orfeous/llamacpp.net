using System.Linq;
using Cake.Common.IO;
using Cake.Common.Net;
using Cake.Frosting;
using LlamaCpp.Net.Build.Configuration;

namespace LlamaCpp.Net.Build.Tasks.Dependencies
{
    [TaskName("Dependencies.ClBlast")]
    public class DownloadClBlastTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            var openClVersion = context.OpenClVersion;
            var url =
                $"https://github.com/KhronosGroup/OpenCL-SDK/releases/download/v{openClVersion}/OpenCL-SDK-v{openClVersion}-Source.zip";

            var outputDirectory = context.TmpDir.Combine("OpenCl").Combine(openClVersion);
            context.EnsureDirectoryExists(outputDirectory);
            var outputPath = outputDirectory
                .CombineWithFilePath("opencl.zip");
            context.DownloadFile(url, outputPath.FullPath, new DownloadFileSettings()
            {

            });

            context.Unzip(outputPath.FullPath, context.LibPath.Combine("OpenCl").FullPath);
        }

        public override bool ShouldRun(BuildContext context) => context.BuildSettings.Any(settings => settings.BlasType == BlasType.CLBlast);
    }

}
