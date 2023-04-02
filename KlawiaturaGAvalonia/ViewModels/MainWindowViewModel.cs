namespace KlawiaturaGAvalonia.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    //Colour fields
    public string PrimaryColour { get; set; } = "White";
    public string PrimKeyColour { get; set; } = "White";
    public string SecKeyColour { get; set; } = "LightGray";
    public string BorderColour { get; set; } = "Black";
    
    //Virtual keyboard layout visualisation contents
    public int KbFontSize { get; set; } = 20;
    
    //Evaluation section contents
    public int EvalFontSize { get; set; } = 20;
    
    //Settings contents
    
    //Results contents

}