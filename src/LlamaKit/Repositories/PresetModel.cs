using LlamaKit.Models;

namespace LlamaKit.Repositories;

public class PresetModel
{
    public string Name { get; set; }

    public string DisplayName => string.IsNullOrWhiteSpace(Name) ? string.Join('-', Family, $"{Size}B") : Name;

    public ModelFamily Family { get; init; }

    public int Size { get; init; }
    public string InitialPrompt { get; set; }
    public string PromptPrefix { get; set; }
    public string PromptSuffix { get; set; }
}