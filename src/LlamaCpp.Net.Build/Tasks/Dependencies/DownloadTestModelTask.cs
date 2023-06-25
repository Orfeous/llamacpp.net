using Cake.Common.IO;
using Cake.Common.Net;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace LlamaCpp.Net.Build.Tasks.Dependencies
{
    
    [IsDependeeOf(typeof(InstallDependenciesTask))]
    public class DownloadTestModelTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.ModelDirectory = context.RepositoryRoot.Combine("models");
            var url = "https://huggingface.co/TheBloke/wizardLM-7B-GGML/resolve/main/wizardLM-7B.ggmlv3.q4_0.bin";
            var fileName = "wizardLM-7B.ggmlv3.q4_0.bin";

            context.EnsureDirectoryExists(context.ModelDirectory);

            if (context.FileExists(context.ModelDirectory.CombineWithFilePath(fileName)))
            {
                context.Log.Information($"File {fileName} already exists, skipping download");
            }
            else
            {
                context.Log.Information($"Downloading {fileName} from {url}");
                context.DownloadFile(url, context.ModelDirectory.CombineWithFilePath(fileName),
                    new DownloadFileSettings());
            }
        }
    }
}
