using CommunityToolkit.Mvvm.ComponentModel;
using LlamaKit.Abstractions;
using LlamaKit.DesktopApplication.ViewModels.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;

namespace LlamaKit.DesktopApplication.ViewModels.Controls;

public partial class ModelSelectorViewModel : ViewModelBase
{
    [ObservableProperty]
    ObservableCollection<LanguageModelViewModel> models = new ObservableCollection<LanguageModelViewModel>();

    public ModelSelectorViewModel()
    {
        this.ModelRepository = App.Current.Services.GetRequiredService<IModelRepository>();
        this.PresetRepository = App.Current.Services.GetRequiredService<IPresetRepository>();


        var models = this.ModelRepository.ToList();

        foreach (var model in models)
        {
            var presets = this.PresetRepository.GetApplicablePresets(model.Name, model.Family, model.ModelSize);
            var m = new LanguageModelViewModel()
            {
                Name = model.Name,
                Path = model.Path,
                Size = model.Size,
                Type = model.Type,
                ModelSize = model.ModelSize,
                SelectedPreset = presets?.FirstOrDefault(),
                Family = model.Family,
                Presets = presets
            };


            this.Models.Add(m);
        }
    }

    public IPresetRepository PresetRepository { get; set; }

    public IModelRepository ModelRepository { get; set; }

    [ObservableProperty] private LanguageModelViewModel selectedItem;

}