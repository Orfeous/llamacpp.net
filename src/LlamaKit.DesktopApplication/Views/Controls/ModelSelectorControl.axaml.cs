using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LlamaKit.DesktopApplication.Views.Controls;

public partial class ModelSelectorControl : UserControl
{
    public ModelSelectorControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}