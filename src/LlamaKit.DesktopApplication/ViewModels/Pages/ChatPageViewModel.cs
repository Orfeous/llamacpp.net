using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CsvHelper;
using CsvHelper.Configuration;
using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using LlamaKit.Abstractions;
using LlamaKit.DesktopApplication.ViewModels.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace LlamaKit.DesktopApplication.ViewModels.Pages;

public partial class ChatPageViewModel : PageViewModel, IRecipient<LoadLanguageModelCommand>
{
    public ChatPageViewModel()
    {
        this.Messages = new ObservableCollection<Controls.MessageViewModel>();

        this.Messages.CollectionChanged += (sender, args) =>
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Messages)));
        };

        this.Factory = App.Current.Services.GetRequiredService<ILanguageModelFactory>();

        WeakReferenceMessenger.Default.Register(this);
    }

    private ILanguageModelFactory Factory { get; }


    public ObservableCollection<Controls.MessageViewModel> Messages { get; set; }


    private ILanguageModel? LanguageModel { get; set; }

    [ObservableProperty] private string _modelName;

    [ObservableProperty] private string _messageText;

    [ObservableProperty]
    private InferenceOptionsViewModel _inferenceOptions = new InferenceOptionsViewModel();

    [RelayCommand]
    private async Task InsertNewLine()
    {
        this.MessageText += "\n";
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task Send(string message)
    {
        this.Messages.Add(new Controls.MessageViewModel("You", message));

        this.MessageText = string.Empty;

        this.IsLoading = true;


        var responseMessage = new Controls.MessageViewModel(
            ModelName,
            "...");

        this.Messages.Add(responseMessage);

        await foreach (var part in this.LanguageModel.InferAsync(message, InferenceOptions.ToInferenceOptions()))
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
    [ObservableProperty] private bool togglePane;

    [ObservableProperty] public bool allowInput = false;


    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task Export()
    {
        if (this.IsLoading)
        {
            return;
        }



        var fileName = Path.Join(AppDomain.CurrentDomain.BaseDirectory, $"{ModelName}-{DateTime.Now:yy-MM-dd}.csv");
        // use csvhelper
        await using var sw = new StreamWriter(fileName);

        await using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<MessageViewModelMap>();

        if (File.Exists(fileName))
        {
            csv.WriteHeader<Controls.MessageViewModel>();

            await csv.NextRecordAsync();
        }


        var messages = this.Messages.ToList();

        await csv.WriteRecordsAsync(messages);
    }

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


        this.ModelName = Path.GetFileNameWithoutExtension(modelName).Split(".")[0];



        AllowInput = false;
        this.LanguageModel = await this.Factory.CreateModelAsync(modelName, LanguageModelOptions.Default);

        AllowInput = true;
    }
}

internal sealed class MessageViewModelMap : ClassMap<MessageViewModel>
{
    public MessageViewModelMap()
    {
        Map(m => m.Sender).Index(0);
        Map(m => m.Icon).Index(1);
        Map(m => m.Message).Index(2);
    }

}