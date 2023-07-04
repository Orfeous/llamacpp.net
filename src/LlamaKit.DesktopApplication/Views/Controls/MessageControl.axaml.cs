using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LlamaKit.DesktopApplication;

public partial class MessageControl : UserControl
{
    public MessageControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}