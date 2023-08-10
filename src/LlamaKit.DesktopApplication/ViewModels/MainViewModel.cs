using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LlamaKit.DesktopApplication.ViewModels.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LlamaKit.DesktopApplication.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {

        _currentPageViewModel = _pageViewModels[nameof(Pages.ChatPageViewModel)];
    }

    [ObservableProperty]
    private Controls.ModelSelectorViewModel _modelSelectorViewModel;

    // page view models

    private readonly Dictionary<string, PageViewModel> _pageViewModels = new Dictionary<string, PageViewModel>()
    {
        { nameof(Pages.ChatPageViewModel), new Pages.ChatPageViewModel() }
    };




    [ObservableProperty] private PageViewModel _currentPageViewModel;


    [RelayCommand]
    private Task NavigateTo(string pageName)
    {
        this.CurrentPageViewModel = _pageViewModels[pageName];

        return Task.CompletedTask;
    }
}