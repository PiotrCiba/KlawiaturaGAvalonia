using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Threading;
using KlawiaturaGAvalonia.Models;
using KlawiaturaGAvalonia.Views;
using OxyPlot;
using OxyPlot.Series;
using Path = System.IO.Path;

namespace KlawiaturaGAvalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public Window? _window { get; set; }

    //default constructor
    public MainWindowViewModel()
    {
    }

    //Colour fields
    public string PrimaryColour { get; set; } = "White";

    //Keys are put in order: 1, 1.5, 2, 3, 4, 5
    public string[] PointKey { get; set; } = { "White", "White", "White", "White", "White", "LightGray" };
    public string SecKeyColour { get; set; } = "LightGray";
    public string BorderColour { get; set; } = "Gray";

    //Virtual keyboard layout visualisation contents
    public int KbFontSize { get; set; } = 19;

    public string[][] CurrentLayout { get; set; } =
    {
        new[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "[", "]" },
        new[] { "A", "S", "D", "F", "G", "H", "J", "K", "L", ";", "'" },
        new[] { "Z", "X", "C", "V", "B", "N", "M", ",", ".", "?" }
    };

    public string[] Layouts { get; set; } = { "QWERTY", "Dvorak", "ARENSITO", "Colemak", "Workman", "<Selected>" };
    private int _selectedlayout;

    public int SelectedLayout
    {
        get { return _selectedlayout; }
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
    public int EvalFontSize { get; set; } = 18;

    //Settings contents
    public int SetFontSize { get; set; } = 12;
    public Settings Settings { get; set; } = new Settings();
    public string[] ChildNumbers { get; set; } = { "1", "2" };
    public string[] CarryModes { get; set; } = { "no carry", "Elityzm (%)" };
    public string[] SelectionAlgorithms { get; set; } = { "Turniej", "Ruletka", "Rank" };
    public string[] CrossoverAlgorithms { get; set; } = { "OX", "CX", "ERX", "AEX" };
    public string[] MutationAlgorithms { get; set; } = { "Pair Swap", "Partial Scramble", "Inversion Mutation" };

    //Results contents
    private Progress<int>? Progress { get; set; }
    private int _currentProgressValue;

    public int CurrentProgressValue
    {
        get { return _currentProgressValue; }
        set
        {
            _currentProgressValue = value;
            OnPropertyChanged();
        }
    }

    private List<Summary> _summaries = new List<Summary>();

    public List<Summary> GenerationSummaries
    {
        get { return _summaries; }
        set
        {
            if (_summaries != value)
            {
                _summaries = value;
                OnPropertyChanged();
            }

        }
    }

    private List<Chromosom[]> _allgen = new List<Chromosom[]>();

    public List<Chromosom[]> AllGenerations
    {
        get { return _allgen; }
        set
        {
            if (_allgen != value)
            {
                _allgen = value;
                OnPropertyChanged();
            }
        }
    }

    private List<Chromosom> _currgen;

    public List<Chromosom> CurrentGeneration
    {
        get { return _currgen; }
        set
        {
            if (_currgen != value)
            {
                _currgen = value;
                OnPropertyChanged();
            }
        }
    }

    private int _currSelGen;

    public int CurrSelGeneration
    {
        get { return _currSelGen; }
        set
        {
            if (_currSelGen != value)
            {
                _currSelGen = value;
                OnGenSelectionChanged();
                OnPropertyChanged();
            }
        }
    }

    private int _currSelChrom;

    public int CurrSelChromosome
    {
        get { return _currSelChrom; }
        set
        {
            if (_currSelChrom != value)
            {
                _currSelChrom = value;
                OnChromosomeSelectionChanged();
                OnPropertyChanged();
            }
        }
    }

    //Button commands
    private bool ShowingCosts { get; set; }

    public void ShowCost()
    {
        if (ShowingCosts)
        {
            ShowingCosts = false;
            CurrentLayout = new[]
            {
                new[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "[", "]" },
                new[] { "A", "S", "D", "F", "G", "H", "J", "K", "L", ";", "'" },
                new[] { "Z", "X", "C", "V", "B", "N", "M", ",", ".", "?" }
            };
            PointKey = new[] { "White", "White", "White", "White", "White", "LightGray" };
        }
        else
        {
            ShowingCosts = true;
            CurrentLayout = new[]
            {
                new[] { "4", "2", "2", "3", "4", "4", "3", "2", "2", "4", "5", "5" },
                new[] { "1.5", "1", "1", "1", "3", "3", "1", "1", "1", "1.5", "3" },
                new[] { "4", "4", "3", "2", "4", "4", "2", "3", "4", "4" }
            };
            PointKey = new[] { "#FF9ECB51", "#FFB3D46F", "#FFC7DE91", "#FFEF8487", "#FFDD2F39", "#FFB0252C" };
        }

        OnPropertyChanged(nameof(CurrentLayout));
        OnPropertyChanged(nameof(PointKey));
    }

    public void ChangeLayoutSelection()
    {
        if (ShowingCosts)
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

        if (SelectedLayout >= 0)
        {
            SelectedLayoutName = Layouts[SelectedLayout];
            CurrentLayoutFitness = Fitness.Fn(Settings.FitSet,CurrentLayout);
            CurrentImprovementOverQwerty = CurrentLayoutFitness / QwertyFitness;
            OnPropertyChanged(nameof(CurrentLayout));
            OnPropertyChanged(nameof(SelectedLayoutName));
            OnPropertyChanged(nameof(CurrentLayoutFitness));
            OnPropertyChanged(nameof(CurrentImprovementOverQwerty));
        }
    }

    public async void OnStartButtonClick()
    {
        //Re-setting the progressbar

        //setting up the progress updates
        Progress = new Progress<int>(value => CurrentProgressValue = value);

        //disabling GUI controls
        //this.IsEnabled = false;

        //calling GA asympotically, to get unpdates on the progressbar
        (List<Summary>, List<Chromosom[]>) output = await StartTask();

        //enabling GUI controls
        //this.IsEnabled = true;

        //clearing datagrid sources

        //sending Start returns to both DataGrids
        GenerationSummaries = output.Item1;
        AllGenerations = new List<Chromosom[]>(output.Item2);
        CurrentGeneration = AllGenerations.Last().ToList();

        //for ShowGraph, let's try something like this...
        datapoints = new Collection<DPoint>();
        foreach (var s in GenerationSummaries)
            datapoints.Add(new DPoint { gen = s.IdPokolenia, fit = s.BestFitness });
        OnPropertyChanged(nameof(datapoints));

        OnPropertyChanged(nameof(GenerationSummaries));
        OnPropertyChanged(nameof(AllGenerations));
        OnPropertyChanged(nameof(CurrSelGeneration));
        OnPropertyChanged(nameof(CurrentGeneration));
    }

    private async Task<(List<Summary>, List<Chromosom[]>)> StartTask()
    {
        //calling Start method to execute GA
        return await Task.Run(() =>
            GeneticAlgorithm.Start(Settings, Progress ?? throw new InvalidOperationException()));
    }

    public void OnGenSelectionChanged()
    {
        if (CurrSelGeneration >= 0)
        {
            CurrentGeneration = AllGenerations[CurrSelGeneration].ToList();
            OnPropertyChanged(nameof(CurrentGeneration)) ;
            OnChromosomeSelectionChanged();
        }
    }

    public void OnChromosomeSelectionChanged()
    {
        if (CurrSelChromosome >= 0)
        {
            SelectedLayoutName = "Gen " + CurrSelGeneration + ", Child " + CurrSelChromosome;
            CurrentLayout = GeneticAlgorithm.StringToLayout(CurrentGeneration[CurrSelChromosome].layout);
            CurrentLayoutFitness = Fitness.Fn(Settings.FitSet, CurrentLayout);
            CurrentImprovementOverQwerty = CurrentLayoutFitness / QwertyFitness;
            OnPropertyChanged(nameof(CurrentLayout));
            OnPropertyChanged(nameof(SelectedLayoutName));
            OnPropertyChanged(nameof(CurrentLayoutFitness));
            OnPropertyChanged(nameof(CurrentImprovementOverQwerty));
        }
    }

    //Save button behaviour

    public async void SaveBtn()
    {
        var svdlg = new SaveFileDialog();
        if (Avalonia.Application.Current != null && Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            svdlg.InitialFileName = "result-"+DateTime.Now.ToShortDateString()+".csv";
            svdlg.Directory = "C:/";
            svdlg.Filters = new List<FileDialogFilter>()
            {
                new() { Name = "Częściowy zapis wyników (.csv)", Extensions = new List<string> { "csv" } },
                new() { Name = "Pełen zapis wyników (.txt)", Extensions = new List<string> { "txt" } },
                new() { Name = "Skrypt AutoHotkey z aktualnym układem (.ahk)", Extensions = new List<string> { "ahk" } }
            };
            var file = await svdlg.ShowAsync(desktop.MainWindow);
            Debug.WriteLine(file);
            if (file != null)
            {
                var extension = Path.GetExtension(file);
                switch (extension)
                {
                    case ".csv":
                        WriteSummariesToCsv(file);
                        break;
                    case ".txt":
                        WriteResultsToTxt(file);
                        break;
                    case ".ahk":
                        CreateAutoHotkeyScript(file);
                        break;
                }
            }
        }
    }

    private void WriteSummariesToCsv(string path)
    {
        using (var sw = new StreamWriter(path))
        {
            sw.WriteLine("gen;bestFn;AvgFn");
            foreach (var s in GenerationSummaries)
                sw.WriteLine("{0};{1};{2}", s.IdPokolenia, s.BestFitness, s.AvgFitness);
        }
    }

    private void WriteResultsToTxt(string path)
    {
        using (var sw = new StreamWriter(path))
        {
            sw.WriteLine("KlawiaturaGAvalonia Results for run {0} ", DateTime.Now);
            sw.WriteLine("\n===================================================================");
            sw.WriteLine("\n\tSETTINGS:");
            sw.WriteLine("\n1) POPULATION:");
            sw.Write(" size: \t\t\t{0}\n children per parent: \t{1}\n", Settings.Main.popSize,Settings.Repop.childNumber+1);
            sw.WriteLine("\n2) CARRY-OVER:");
            sw.Write(" carry-over mode: \t{0}\n carry-over ratio (%): \t{1}\n ", CarryModes[Settings.Repop.carryoverType], Settings.Repop.carryVar);
            sw.Write("culling: \t\t{0}\n culling ratio (%): \t{1}\n",Settings.Repop.culling,Settings.Repop.cullingRate);
            sw.WriteLine("\n3) SELECTION, CROSSOVER:");
            sw.Write("selection algorithm: \t{0}\n selection pressure \t{1}\n crossover operator: \t{2}\n", 
                SelectionAlgorithms[Settings.Repop.currSel], Settings.Repop.SelPressure, CrossoverAlgorithms[Settings.Repop.currX]);
            sw.WriteLine("\n4) MUTATION:");
            sw.Write(" mutation type: \t{0}\n mutation chance: \t{1}\n mutation severity: \t{2}\n",
                MutationAlgorithms[Settings.Repop.currMut],Settings.Repop.mutChance,Settings.Repop.mutSeverity);
            sw.WriteLine("\n5) BEHAVIOUR:");
            sw.Write(" stop after {0}: \t{1}\n FullMemory: \t\t{2}\n", (Settings.Main.currStopMode)?"gen ":"eps < ",
                (Settings.Main.currStopMode)?Settings.Main.gensToRun:Settings.Main.epsToStopAt,Settings.Main.fullMemory);
            sw.WriteLine("===================================================================");
            sw.WriteLine("\nRESULTS:");
            sw.WriteLine("GEN\t|\tBEST\t|\tAVG"+(Settings.Main.fullMemory?"\t|\tBestExample":""));
            var len = GenerationSummaries.Count;
            for (int i = 0; i < len; i++)
            {
                sw.Write("{0}\t|\t{1}\t|\t{2}",GenerationSummaries[i].IdPokolenia,
                    Math.Round(GenerationSummaries[i].BestFitness,5),Math.Round(GenerationSummaries[i].AvgFitness,5));
                if(Settings.Main.fullMemory)
                    sw.Write("\t|\t"+AllGenerations[i][0].layout);
                sw.WriteLine();
            }
            sw.WriteLine("===================================================================");
        }
    }

    private void CreateAutoHotkeyScript(string path)
    {
        using (var sw = new StreamWriter(path))
        {
            string[][] qwerty =
            {
                new[] { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "leftbracket", "rightbracket" },
                new[] { "a", "s", "d", "f", "g", "h", "j", "k", "l", "semicolon", "'" },
                new[] { "z", "x", "c", "v", "b", "n", "m", "comma", "period", "slash" }
            };
            sw.Write("#NoEnv\nSendMode Input\nSetWorkingDir %A_ScriptDir%\n\n");
            sw.WriteLine("; Define key mappings");
            for (int i = 0; i < 3; i++)
            {
                var rowlen = qwerty[i].Length;
                for (int k = 0; k < rowlen; k++)
                {
                    sw.WriteLine("{0}::{1}",qwerty[i][k],AutoHotkeyPrep(CurrentLayout[i][k]));
                }
            }
            sw.WriteLine("\n; Disable Caps Lock key\nCapsLock::Return");
        }
    }

    private string AutoHotkeyPrep(string input)
    {
        if (input.All(x => char.IsLetter(x)))
        {
            return input.ToLower();
        }
        switch (input)
        {
            case "[":
                return "leftbracket";
            case "]":
                return "rightbracket";
            case ";":
                return "semicolon";
            case "'":
                return "'";
            case ",":
                return "comma";
            case ".":
                return "period";
            case "?":
                return "slash";
        }

        return "<AhPrep error: string was" + input +">";
    }
    
    //ToDo: if the different fitness functions are implemented, add an option to spit out a txt import file for patorjk.com

    //graph

    //todo: have a working graph display, that shows how the fitness score of each generation changes
    public Collection<DPoint> datapoints { get; set; } = new Collection<DPoint>();
    public void DisplayGraph()
    {
        if (GenerationSummaries.Count>0)
        {
            FitnessGraph window2 = new FitnessGraph(GenerationSummaries);
            window2.Show();
        };
    }

    public class DPoint
    {
        public int gen { get; set; }
        public double fit { get; set; }
    }
}

