using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Samplers.Abstractions;

namespace LlamaKit.Abstractions;

public interface ILanguageModelFactory
{
    public ILanguageModel CreateModel(string filePath, LanguageModelOptions? options = null, Action<ISamplingPipelineBuilder>? builder = null
    );

    Task<ILanguageModel> CreateModelAsync(string modelName, LanguageModelOptions options);
}