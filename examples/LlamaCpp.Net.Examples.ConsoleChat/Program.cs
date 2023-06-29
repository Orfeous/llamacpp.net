using LlamaCpp.Net;
using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Samplers.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LlamaCpp.Net.Examples.ConsoleChat
{
    internal class Program
    {
        private static readonly string _modelDirectory = @"D:\source\repos\llamacpp.net\models";

        //private static readonly string _modelDirectory = Path.Combine(AppContext.BaseDirectory, "models");
        private static readonly string _modelName = "wizardLM-7B.ggmlv3.q4_0.bin";
        private static readonly string _modelPath = Path.Combine(_modelDirectory, _modelName);

        static async Task Main(string[] args)
        {
            var modelOptions = LanguageModelOptions.Default with
            {
                PromptSuffix = "\n\n### Response:",
                GpuLayerCount = 10,
                ContextSize = 2048,
                Threads = 24,

                UseFp16Memory = false
            };
            var host = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ILanguageModel>(s =>
                        new LanguageModel(_modelPath, s.GetRequiredService<ILogger<LanguageModel>>(),
                            modelOptions, SamplingPipelinePreset.Default
                        )
                    );
                })
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                    logging.AddConsole();
                }).Build();

            var model = host.Services.GetRequiredService<ILanguageModel>();

            var inferenceOptions = InferenceOptions.Default;

            var prompt = "";
            while (prompt != "/stop")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\nUser: ");
                prompt = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                if (string.IsNullOrWhiteSpace(prompt))
                {
                    continue;
                }

                await foreach (var text in model.InferAsync(prompt, inferenceOptions with { SamplingMethod = SamplingMethod.Mirostat, MaxNumberOfTokens = 1000 }))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(text);
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }
        }
    }
}