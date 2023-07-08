using LlamaKit.Repositories;

namespace LlamaKit.DesktopApplication.ViewModels;

public class LoadLanguageModelCommand
{
    public string Name { get; set; }
    public string Path { get; set; }
    public PresetModel Preset { get; set; }

    public LoadLanguageModelCommand()
    {
    }
}