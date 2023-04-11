using Avalonia.Controls;
using KlawiaturaGAvalonia.ViewModels;

namespace KlawiaturaGAvalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}