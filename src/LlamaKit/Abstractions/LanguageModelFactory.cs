using LlamaCpp.Net;
using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using Microsoft.Extensions.Logging;

namespace LlamaKit.Abstractions;

public class LanguageModelFactory : ILanguageModelFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<LanguageModelFactory> _logger;

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
            return new LanguageModel(filePath, new Logger<LanguageModel>(_loggerFactory), options);
        }

        else
        {
            _logger.LogError($"Model file {filePath} not found");
            throw new FileNotFoundException($"Model file {filePath} not found");
        }
    }

    public Task<ILanguageModel> CreateModelAsync(string filePath, LanguageModelOptions options)
    {
        return Task.Run(() => CreateModel(filePath, options));
    }
}