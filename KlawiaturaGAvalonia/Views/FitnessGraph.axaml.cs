using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using KlawiaturaGAvalonia.Models;
using KlawiaturaGAvalonia.ViewModels;
using OxyPlot;
using OxyPlot.Avalonia;

namespace KlawiaturaGAvalonia.Views;

public partial class FitnessGraph : Window
{
    public FitnessGraph()
    {
        InitializeComponent();
        DataContext = new FitnessWindowViewModel();
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public FitnessGraph(List<Summary> input)
    {
        DataContext = new FitnessWindowViewModel(input);
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