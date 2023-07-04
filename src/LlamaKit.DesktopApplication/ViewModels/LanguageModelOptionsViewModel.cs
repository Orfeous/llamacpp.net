using CommunityToolkit.Mvvm.ComponentModel;
using LlamaCpp.Net.Configuration;

namespace LlamaKit.DesktopApplication.ViewModels;

public partial class LanguageModelOptionsViewModel : ViewModelBase
{
    public LanguageModelOptionsViewModel()
    {
        this.PromptPrefix = "USER:";
        this.PromptSuffix = "\n\nASSISTANT:";
        this.InitialPrompt =
            "A chat between a curious user and an artificial intelligence assistant. The assistant gives helpful, detailed, and polite answers to the user's questions. USER: hello, who are you? ASSISTANT:\r\n\r\n";


    }


    public LanguageModelOptions ToOptions()
    {
        var options = LanguageModelOptions.Default with
        {
            InitialPrompt = this.InitialPrompt,
        };

        if (!string.IsNullOrWhiteSpace(this.PromptPrefix))
        {
            options = options with
            {
                PromptPrefix = this.PromptPrefix
            };
        }

        if (!string.IsNullOrWhiteSpace(this.PromptSuffix))
        {
            options = options with
            {
                PromptSuffix = this.PromptSuffix
            };
        }

        return options;
    }

    [ObservableProperty]
    public string? initialPrompt;

    [ObservableProperty]
    public string? promptPrefix;

    [ObservableProperty]
    public string? promptSuffix;
}