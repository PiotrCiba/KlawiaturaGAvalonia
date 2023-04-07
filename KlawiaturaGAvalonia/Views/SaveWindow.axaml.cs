using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace KlawiaturaGAvalonia.Views;

public partial class SaveWindow : Window
{
    public SaveWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}