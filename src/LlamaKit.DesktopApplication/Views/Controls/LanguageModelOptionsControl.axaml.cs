using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LlamaKit.DesktopApplication;

public partial class LanguageModelOptionsControl : UserControl
{
    public LanguageModelOptionsControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}