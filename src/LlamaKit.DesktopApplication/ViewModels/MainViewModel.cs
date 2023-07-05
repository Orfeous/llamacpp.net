using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LlamaKit.Abstractions;
using LlamaKit.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LlamaKit.DesktopApplication.ViewModels;

public partial class MainViewModel : ViewModelBase
{

    [ObservableProperty] ObservableCollection<RepositoryModel> models = new ObservableCollection<RepositoryModel>();

    public MainViewModel()
    {
        this.ModelRepository = App.Current.Services.GetRequiredService<IModelRepository>();
        this.Factory = App.Current.Services.GetRequiredService<ILanguageModelFactory>();


        var models = this.ModelRepository.ToList();

        foreach (var model in models)
        {
            this.Models.Add(model);
        }


        _currentPageViewModel = _pageViewModels[nameof(Pages.ChatPageViewModel)];

    }

    // page view models

    private readonly Dictionary<string, PageViewModel> _pageViewModels = new Dictionary<string, PageViewModel>()
    {
        { nameof(Pages.ChatPageViewModel), new Pages.ChatPageViewModel() }

    };


    public LanguageModelOptionsViewModel LanguageModelOptionsViewModel { get; set; } = new LanguageModelOptionsViewModel();

    public ILanguageModelFactory Factory { get; set; }

    public IModelRepository ModelRepository { get; set; }

    [RelayCommand]
    private Task Create(RepositoryModel model)
    {
        WeakReferenceMessenger.Default.Send(new LoadLanguageModelCommand(model));


        return Task.CompletedTask;
    }

    public bool Loading { get; set; }

    [ObservableProperty]
    private PageViewModel _currentPageViewModel;


    [RelayCommand]
    private Task NavigateTo(string pageName)
    {
        this.CurrentPageViewModel = _pageViewModels[pageName];

        return Task.CompletedTask;
    }
}

public class LoadLanguageModelCommand
{

    public LoadLanguageModelCommand(RepositoryModel model)
    {
        Model = model;
    }

    public RepositoryModel Model { get; set; }
}

/// <summary>
/// An abstract class for enabling page navigation.
/// </summary>
public abstract class PageViewModel : ViewModelBase
{
}
