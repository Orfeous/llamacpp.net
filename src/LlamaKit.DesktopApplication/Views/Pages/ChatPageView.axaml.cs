using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LlamaKit.DesktopApplication.Views.Pages;

public partial class ChatPageView : UserControl
{
    public ChatPageView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}