using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace LlamaKit.DesktopApplication.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    public ChatViewModel()
    {
        this.Messages = new ObservableCollection<MessageViewModel>();


    }
    public ChatViewModel(string modelName, ILanguageModel languageModel)
    {
        this.LanguageModel = languageModel;
        this.ModelName = modelName;

        this.Messages = new ObservableCollection<MessageViewModel>();

        this.Messages.CollectionChanged += (sender, args) =>
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Messages)));
        };
    }

    public ObservableCollection<MessageViewModel> Messages { get; set; }


    private ILanguageModel LanguageModel { get; set; }

    [ObservableProperty] public string modelName;

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
}