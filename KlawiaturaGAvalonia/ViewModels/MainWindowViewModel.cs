using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KlawiaturaAG;

namespace KlawiaturaGAvalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    //default constructor
    public MainWindowViewModel()
    {
        ChangeLayoutSelection();
    }
    //Colour fields
    public string PrimaryColour { get; set; } = "White";

    //Keys are put in order: 1, 1.5, 2, 3, 4, 5
    public string[] PointKey { get; set; } = { "White", "White", "White", "White", "White", "LightGray" };
    public string SecKeyColour { get; set; } = "LightGray";
    public string BorderColour { get; set; } = "Gray";

    //Virtual keyboard layout visualisation contents
    public int KbFontSize { get; set; } = 20;
    public string[][] CurrentLayout { get; set; } =
    {
        new [] {"Q","W","E","R","T","Y","U","I","O","P","[","]"},
        new [] {"A","S","D","F","G","H","J","K","L",";","'"}, 
        new [] {"Z","X","C","V","B","N","M",",",".","?"}
    };

    public string[] Layouts { get; set; } = { "QWERTY", "Dvorak", "ARENSITO", "Colemak", "Workman", "<Selected>" };
    private int _selectedlayout;
    public int SelectedLayout
    {
        get { return _selectedlayout;}
        set
        {
            _selectedlayout = value;
            ChangeLayoutSelection();
        }
    }
    public string SelectedLayoutName { get; set; } = "QWERTY";
    public double CurrentLayoutFitness { get; set; }
    public const double QwertyFitness = 243.5024299999992;
    public double CurrentImprovementOverQwerty { get; set; }
    public int EvalFontSize { get; set; } = 20;

    //Settings contents
    public int SetFontSize { get; set; } = 12;
    public Settings Settings { get; set; } = new Settings();
    public string[] ChildNumbers { get; set; } = { "1", "2" };
    public string[] CarryModes { get; set; } = { "no carry", "Elityzm (%)" };
    public string[] SelectionAlgorithms { get; set; } = { "Turniej", "Ruletka", "Rank" };
    public string[] CrossoverAlgorithms { get; set; } = { "OX", "CX", "ERX", "AEX"};
    public string[] MutationAlgorithms { get; set; } = { "Pair Swap", "Partial Scramble", "Inversion Mutation" };

    //Results contents

    public List<Summary> GenerationSummaries { get; set; } = new List<Summary>();
    public List<Chromosom[]> AllGenerations { get; set; } = new List<Chromosom[]>();
    public int currSelGeneration { get; set; } = 0;

    //Button commands
    private bool showingCosts { get; set; }

    public void ShowCost()
    {
        if (showingCosts)
        {
            showingCosts = false;
            CurrentLayout = new []{
                new [] {"Q","W","E","R","T","Y","U","I","O","P","[","]"},
                new [] {"A","S","D","F","G","H","J","K","L",";","'"}, 
                new [] {"Z","X","C","V","B","N","M",",",".","?"}
            };
            PointKey = new []{ "White", "White", "White", "White", "White", "LightGray" };
        }else
        {
            showingCosts = true;
            CurrentLayout = new []{
                new [] {"4","2","2","3","4","4","3","2","2","4","5","5"},
                new [] {"1.5","1","1","1","3","3","1","1","1","1.5","3"}, 
                new [] {"4","4","3","2","4","4","2","3","4","4"}
            };
            PointKey = new []{ "#FF9ECB51", "#FFB3D46F", "#FFC7DE91", "#FFEF8487", "#FFDD2F39", "#FFB0252C" };
        }
        OnPropertyChanged(nameof(CurrentLayout));
        OnPropertyChanged(nameof(PointKey));
    }

    public void ChangeLayoutSelection()
    {
        if (showingCosts)
            ShowCost();
        
        switch (SelectedLayout)
        {
            //Layout is saved in a string[] of {2, 12, 11, 10}
            case 0: //QWERTY layout
                CurrentLayout = new[]
                {
                    new[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "[", "]" },
                    new[] { "A", "S", "D", "F", "G", "H", "J", "K", "L", ";", "'" },
                    new[] { "Z", "X", "C", "V", "B", "N", "M", ",", ".", "?" }
                };
                break;
            case 1: //Dvorak layout
                CurrentLayout = new[]
                {
                    new[] { "'", ",", ".", "P", "Y", "F", "G", "C", "R", "L", "?", "=" },
                    new[] { "A", "O", "E", "U", "I", "D", "H", "T", "N", "S", "-" },
                    new[] { ";", "Q", "J", "K", "X", "B", "M", "W", "V", "Z" }
                };
                break;
            case 2: //ARENSITO layout
                CurrentLayout = new[]
                {
                    new[] { "Q", "L", ".", "P", "'", ";", "F", "U", "D", "K", "[", "]" },
                    new[] { "A", "R", "E", "N", "B", "G", "S", "I", "T", "O", "-" },
                    new[] { "Z", "W", ",", "H", "J", "V", "B", "Y", "M", "X" }
                };
                break;
            case 3: //Colemak layout
                CurrentLayout = new[]
                {
                    new[] { "Q", "W", "F", "P", "G", "J", "L", "U", "Y", ";", "[", "]" },
                    new[] { "A", "R", "S", "T", "D", "H", "N", "E", "I", "O", "'" },
                    new[] { "Z", "X", "C", "V", "B", "K", "M", ",", ".", "?" }
                };
                break;
            case 4: //Workman layout
                CurrentLayout = new[]
                {
                    new[] { "Q", "D", "R", "W", "B", "J", "F", "U", "P", ";", "[", "]" },
                    new[] { "A", "S", "H", "T", "G", "Y", "N", "E", "O", "I", "'" },
                    new[] { "Z", "X", "M", "C", "V", "K", "L", ",", ".", "?" }
                };
                break;
        }

        SelectedLayoutName = Layouts[SelectedLayout];
        CurrentLayoutFitness = GeneticAlgorithm.Fn(CurrentLayout);
        CurrentImprovementOverQwerty = CurrentLayoutFitness / QwertyFitness;
        OnPropertyChanged(nameof(CurrentLayout));
        OnPropertyChanged(nameof(SelectedLayoutName));
        OnPropertyChanged(nameof(CurrentLayoutFitness));
        OnPropertyChanged(nameof(CurrentImprovementOverQwerty));
    }
    //internal methods

}