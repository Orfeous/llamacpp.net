using CommunityToolkit.Mvvm.ComponentModel;

namespace LlamaKit.DesktopApplication.ViewModels;

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