using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using KlawiaturaGAvalonia.ViewModels;
using KlawiaturaGAvalonia.Views;

namespace KlawiaturaGAvalonia;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                //DataContext = new MainWindowViewModel()
                //{
                //    _window = desktop.MainWindow,
                //},
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}