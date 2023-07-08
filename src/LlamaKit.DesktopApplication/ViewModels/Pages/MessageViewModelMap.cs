using CsvHelper.Configuration;
using LlamaKit.DesktopApplication.ViewModels.Controls;

namespace LlamaKit.DesktopApplication.ViewModels.Pages;

internal sealed class MessageViewModelMap : ClassMap<MessageViewModel>
{
    public MessageViewModelMap()
    {
        Map(m => m.Sender).Index(0);
        Map(m => m.Icon).Index(1);
        Map(m => m.Message).Index(2);
    }

}