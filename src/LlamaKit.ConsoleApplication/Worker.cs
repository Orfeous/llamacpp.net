using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using LlamaKit.Abstractions;
using Spectre.Console;

namespace LlamaKit.ConsoleApplication
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ILanguageModelFactory _factory;
        private readonly IModelRepository _modelRepository;
        private readonly IPresetRepository _presetRepository;

        public Worker(ILogger<Worker> logger, ILanguageModelFactory factory, IModelRepository modelRepository,
            IPresetRepository presetRepository)
        {
            _logger = logger;
            _factory = factory;
            _modelRepository = modelRepository;
            _presetRepository = presetRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Chat(stoppingToken);
        }

        private async Task Chat(CancellationToken stoppingToken)
        {
            var models = _modelRepository.ToList();

            var selectedModelName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a model")
                    .PageSize(10)
                    .AddChoices(models.Select(x => x.Name))
            );


            var selectedModel = models.FirstOrDefault(x => x.Name == selectedModelName);


            var languageModelOptions = new LanguageModelOptions
            {
                InitialPrompt = "You are a helpful assistant.\n",
                UseMemoryLock = true,
                ContextSize = 4196,
                GpuLayerCount = 20,
            };
            var inferenceOptions = new InferenceOptions
            {
                SamplingMethod = SamplingMethod.MirostatV2,
                MaxNumberOfTokens = -1,
                PenalizeNewLine = false,
                PromptPrefix = "USER: ",
                PromptSuffix = "\nASSISTANT: ",
                Antiprompts = new List<string>()
                {
                    "USER:",
                    "ASSISTANT:"
                }
            };

            var model = await _factory.CreateModelAsync(selectedModel.Path, languageModelOptions);

            Console.Clear();


            while (true)
            {
                var prompt = AnsiConsole.Prompt(
                    new TextPrompt<string>("USER: ")
                );

                await Prompt(prompt, stoppingToken, model, inferenceOptions);

                AnsiConsole.WriteLine();
            }
        }

        private static async Task Prompt(string prompt, CancellationToken stoppingToken, ILanguageModel model,
            InferenceOptions inferenceOptions)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(prompt);
            Console.WriteLine();

            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            await foreach (var text in model.InferAsync(prompt, inferenceOptions, stoppingToken))
            {
                Console.Write(text);
            }

            Console.ResetColor();
        }
    }
}