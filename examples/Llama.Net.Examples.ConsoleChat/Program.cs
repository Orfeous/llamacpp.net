using LlamaCpp.Net.Configuration;
using LlamaCpp.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

namespace Llama.Net.Examples.ConsoleChat
{
    internal class Program
    {
        private static readonly string _modelDirectory = @"d:\Development\llamacpp.net\models";
        //private static readonly string _modelDirectory = Path.Combine(AppContext.BaseDirectory, "models");
        private static readonly string _modelName = "wizardLM-7B.ggmlv3.q4_0.bin";
        private static readonly string _modelPath = Path.Combine(_modelDirectory, _modelName);

        static void Main(string[] args)
        {
            if (!File.Exists(_modelPath)) { throw new FileNotFoundException($"Model not found at: '{_modelPath}'"); }
            var model = new LanguageModel(_modelPath, new Logger<LanguageModel>(new NullLoggerFactory()), LanguageModelOptions.Default);

            string? prompt = "";
            while (prompt != "/stop")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\nUser: ");
                prompt = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                if (!string.IsNullOrWhiteSpace(prompt))
                {
                    foreach (var text in model.Infer(prompt))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(text);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

            }
        }
    }
}