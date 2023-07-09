using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;

namespace LlamaKit.Abstractions;

public interface ILanguageModelFactory
{
    public ILanguageModel CreateModel(string filePath, LanguageModelOptions? options = null
    );

    Task<ILanguageModel> CreateModelAsync(string modelName, LanguageModelOptions options);
}