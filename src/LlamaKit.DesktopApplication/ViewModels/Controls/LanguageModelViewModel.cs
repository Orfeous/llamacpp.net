using System.Collections.Generic;
using ByteSizeLib;
using CommunityToolkit.Mvvm.ComponentModel;
using LlamaKit.DesktopApplication.ViewModels.Abstractions;
using LlamaKit.Models;
using LlamaKit.Repositories;

namespace LlamaKit.DesktopApplication.ViewModels.Controls;

public partial class LanguageModelViewModel : ViewModelBase
{
    public LanguageModelViewModel()
    {

    }

    public string Name { get; init; }
    public string Path { get; init; }
    public ByteSize Size { get; init; }
    public SupportedModelTypes Type { get; set; }
    public int ModelSize { get; set; }
    public ModelFamily Family { get; set; }
    public IEnumerable<PresetModel> Presets { get; set; }
    [ObservableProperty] private PresetModel selectedPreset;
}