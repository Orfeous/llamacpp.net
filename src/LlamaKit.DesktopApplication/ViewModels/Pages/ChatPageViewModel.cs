using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using LlamaKit.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LlamaKit.DesktopApplication.ViewModels.Pages;

public partial class ChatPageViewModel : PageViewModel, IRecipient<LoadLanguageModelCommand>
{
    public ChatPageViewModel()
    {
        this.Messages = new ObservableCollection<MessageViewModel>();

        this.Messages.CollectionChanged += (sender, args) =>
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Messages)));
        };

        this.Factory = App.Current.Services.GetRequiredService<ILanguageModelFactory>();

        WeakReferenceMessenger.Default.Register(this);
    }

    private ILanguageModelFactory Factory { get; }


    public ObservableCollection<MessageViewModel> Messages { get; set; }


    private ILanguageModel? LanguageModel { get; set; }

    [ObservableProperty] private string _modelName;

    [ObservableProperty] private string _messageText;


    [RelayCommand]
    private async Task InsertNewLine()
    {
        this.MessageText += "\n";
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task Send(string message)
    {
        this.Messages.Add(new MessageViewModel("You", message));

        this.MessageText = string.Empty;

        this.IsLoading = true;


        var responseMessage = new MessageViewModel(
            ModelName,
            "...");

        this.Messages.Add(responseMessage);

        await foreach (var part in this.LanguageModel.InferAsync(message, new InferenceOptions()
        {
            PenalizeNewLine = true,
            MaxNumberOfTokens = 3000,
            SamplingMethod = SamplingMethod.MirostatV2
        }))
        {
            if (responseMessage.Message == "...")
            {
                responseMessage.Message = string.Empty;
            }

            responseMessage.Message += part;
        }

        responseMessage.Message = responseMessage.Message.Trim();

        this.IsLoading = false;
    }

    public bool IsLoading { get; set; }

    [ObservableProperty]
    public bool allowInput = false;

    public void Receive(LoadLanguageModelCommand message)
    {
        if (LanguageModel != null)
        {
            this.LanguageModel?.Dispose();
        }

        this.ModelName = message.Model.Name;
        this.LoadLanguageModelCommand.ExecuteAsync(message.Model.Path);
    }

    [RelayCommand]
    private async Task LoadLanguageModel(string modelName)
    {
        this.LanguageModel?.Dispose();


        this.ModelName = modelName;

        AllowInput = false;
        this.LanguageModel = await this.Factory.CreateModelAsync(modelName, LanguageModelOptions.Default);

        AllowInput = true;
    }
}