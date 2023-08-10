using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LlamaCpp.Net.Examples.ConsoleChat
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 1) { Console.WriteLine("Please provide the model's full path as an argument"); Environment.Exit(1); }
            if (string.IsNullOrWhiteSpace(args[0])) { Console.WriteLine("Please provide the model's full path as an argument"); Environment.Exit(1); }
            var _modelPath = args[0];

            _modelPath = "D:\\LLM\\wizardLM-7B.ggmlv3.q4_0.bin";

            var modelOptions = LanguageModelOptions.Default;
            var host = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ILanguageModel>(s =>
                        new LanguageModel(_modelPath, modelOptions, s.GetRequiredService<ILogger<LanguageModel>>())
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

                await foreach (var text in model.InferAsync(prompt, inferenceOptions))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(text);
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }
        }
    }
}