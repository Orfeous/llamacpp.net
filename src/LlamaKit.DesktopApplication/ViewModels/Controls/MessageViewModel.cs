using CommunityToolkit.Mvvm.ComponentModel;
using FluentIcons.Common;
using LlamaKit.DesktopApplication.ViewModels.Abstractions;

namespace LlamaKit.DesktopApplication.ViewModels.Controls;

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

    public Symbol Icon
    {
        get
        {
            if (Sender == "You")
            {
                return Symbol.Person;
            }

            else
            {
                return Symbol.Chat;
            }
        }
    }
}