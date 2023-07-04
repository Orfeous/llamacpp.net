using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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


    }

    public LanguageModelOptionsViewModel LanguageModelOptionsViewModel { get; set; } = new LanguageModelOptionsViewModel();

    public ILanguageModelFactory Factory { get; set; }

    public IModelRepository ModelRepository { get; set; }

    [RelayCommand]
    public Task Create(RepositoryModel model)
    {
        var languageModel = this.Factory.CreateModel(model.Name, LanguageModelOptionsViewModel.ToOptions());

        this.Loading = true;
        this.ChatViewModel = new ChatViewModel(model.Name, languageModel);

        this.Loading = false;

        return Task.CompletedTask;
    }

    public bool Loading { get; set; }

    [ObservableProperty] private ChatViewModel _chatViewModel;
}

public partial class MessageViewModel : ViewModelBase
{
    public MessageViewModel(string sender, string message)
    {
        this.Sender = sender;
        this.Message = message;
    }

    public MessageViewModel()
    {
        this.Sender = string.Empty;
        this.Message = string.Empty;
    }

    [ObservableProperty] private string _message;
    [ObservableProperty] private string _sender;
}