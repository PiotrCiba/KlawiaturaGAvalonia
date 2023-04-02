namespace KlawiaturaGAvalonia.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    //Colour fields
    public string PrimaryColour { get; set; } = "White";
    public string PrimKeyColour { get; set; } = "White";
    public string SecKeyColour { get; set; } = "LightGray";
    public string BorderColour { get; set; } = "Black";
    
    //UI element contents
    public string KLabel { get; set; } = "Klawiatura";
    public int KbFontSize { get; set; } = 20;
    public string PButton { get; set; } = "Punktacja";
    
}