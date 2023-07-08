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
using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Configuration;
using LlamaKit.Abstractions;
using LlamaKit.Repositories;
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

        var inferenceOptions = this.InferenceOptions.ToInferenceOptions();
        inferenceOptions.PromptPrefix = Preset.PromptPrefix;
        inferenceOptions.PromptSuffix = Preset.PromptSuffix;
        await foreach (var part in this.LanguageModel.InferAsync(message, inferenceOptions))
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
        this.LoadLanguageModelCommand.ExecuteAsync(message);
    }

    [RelayCommand]
    private async Task LoadLanguageModel(LoadLanguageModelCommand model)
    {
        Messages.Clear();
        this.LanguageModel?.Dispose();

        this.Preset = model.Preset;

        this.ModelName = Path.GetFileNameWithoutExtension(model.Name).Split(".")[0];



        AllowInput = false;
        this.LanguageModel = await this.Factory.CreateModelAsync(model.Path, LanguageModelOptions.Default with
        {
            InitialPrompt = model.Preset.InitialPrompt,
        });


        AllowInput = true;
    }

    public PresetModel Preset { get; set; }
}
