using LlamaCpp.Net;
using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using LlamaKit.Abstractions;
using Microsoft.Extensions.Logging;

namespace LlamaKit;

public class LanguageModelFactory : ILanguageModelFactory
{
    private readonly ILogger<LanguageModelFactory> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public LanguageModelFactory(ILogger<LanguageModelFactory> logger,
        ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public ILanguageModel CreateModel(string filePath,
        LanguageModelOptions? options = null)
    {
        if (File.Exists(filePath))
        {
            _logger.LogInformation($"Creating model for {filePath}");
            return new LanguageModel(filePath, options, new Logger<LanguageModel>(_loggerFactory));
        }

        _logger.LogError($"Model file {filePath} not found");
        throw new FileNotFoundException($"Model file {filePath} not found");
    }

    public Task<ILanguageModel> CreateModelAsync(string filePath, LanguageModelOptions options)
    {
        return Task.Run(() => CreateModel(filePath, options));
    }
}