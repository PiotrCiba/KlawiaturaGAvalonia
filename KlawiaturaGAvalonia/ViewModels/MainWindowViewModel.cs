using System;
using System.Collections.Generic;
using KlawiaturaAG;

namespace KlawiaturaGAvalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    //default constructor
    public MainWindowViewModel()
    {
        
        
    }

    //Colour fields
    public string PrimaryColour { get; set; } = "White";

    //Keys are put in order: 1, 1.5, 2, 3, 4, 5
    public string[] PointKey { get; set; } = { "White", "White", "White", "White", "White", "LightGray" };
    public string SecKeyColour { get; set; } = "LightGray";
    public string BorderColour { get; set; } = "Black";

    //Virtual keyboard layout visualisation contents
    public int KbFontSize { get; set; } = 20;
    public string[] CurrentLayout { get; set; } = { "QWERTYUIOP[]", "ASDFGHJKL;'", "ZXCVBNM,.?" };

    public string[] Layouts { get; set; } = { "QWERTY", "Dvorak", "ARENSITO", "Colemak", "Workman", "<Selected>" };
    public int SelectedLayout { get; set; } = 0;
    public double CurrentLayoutFitness { get; set; } = 0f;
    public double CurrentImprovementOverQwerty { get; set; } = 0f;
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
    

    //Window UI methods


    //internal methods

}