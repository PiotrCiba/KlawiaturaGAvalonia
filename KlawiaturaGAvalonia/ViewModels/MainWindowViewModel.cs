namespace KlawiaturaGAvalonia.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    //Colour fields
    public string PrimaryColour { get; set; } = "White";
    
    //Keys are put in order: 1, 1.5, 2, 3, 4, 5
    public string[] PointKey { get; set; } = {"White", "White", "White", "White", "White", "LightGray"};
    public string SecKeyColour { get; set; } = "LightGray";
    public string BorderColour { get; set; } = "Black";
    
    //Virtual keyboard layout visualisation contents
    public int KbFontSize { get; set; } = 20;
    private string[] CurrentLayout { get; set; } = { "QWERTYUIOP[]", "ASDFGHJKL;'", "ZXCVBNM,.?" };
    
    //Evaluation section contents
    public int EvalFontSize { get; set; } = 20;
    
    //Settings contents
    
    //Results contents

    //Window UI methods
    
    /*public void UpdateKeyboardLayout()
        {
            SolidColorBrush white = new SolidColorBrush(System.Windows.Media.Colors.White);
            SolidColorBrush lightGray = new SolidColorBrush(System.Windows.Media.Colors.LightGray);
            

            //First row update
            //Key_Q.Content = CurrentLayout[0][0];
            
            Rect_Q.Fill = white;
            Key_W.Content = CurrentLayout[0][1];
            Rect_W.Fill = white;
            Key_E.Content = CurrentLayout[0][2];
            Rect_E.Fill = white;
            Key_R.Content = CurrentLayout[0][3];
            Rect_R.Fill = white;
            Key_T.Content = CurrentLayout[0][4];
            Rect_T.Fill = white;
            Key_Y.Content = CurrentLayout[0][5];
            Rect_Y.Fill = white;
            Key_U.Content = CurrentLayout[0][6];
            Rect_U.Fill = white;
            Key_I.Content = CurrentLayout[0][7];
            Rect_I.Fill = white;
            Key_O.Content = CurrentLayout[0][8];
            Rect_O.Fill = white;
            Key_P.Content = CurrentLayout[0][9];
            Rect_P.Fill = white;
            Key_10.Content = CurrentLayout[0][10];
            Rect_10.Fill = lightGray;
            Key_11.Content = CurrentLayout[0][11];
            Rect_11.Fill = lightGray;

            //Second row update
            Key_A.Content = CurrentLayout[1][0];
            Rect_A.Fill = white;
            Key_S.Content = CurrentLayout[1][1];
            Rect_S.Fill = white;
            Key_D.Content = CurrentLayout[1][2];
            Rect_D.Fill = white;
            Key_F.Content = CurrentLayout[1][3];
            Rect_F.Fill = white;
            Key_G.Content = CurrentLayout[1][4];
            Rect_G.Fill = white;
            Key_H.Content = CurrentLayout[1][5];
            Rect_H.Fill = white;
            Key_J.Content = CurrentLayout[1][6];
            Rect_J.Fill = white;
            Key_K.Content = CurrentLayout[1][7];
            Rect_K.Fill = white;
            Key_L.Content = CurrentLayout[1][8];
            Rect_L.Fill = white;
            Key_20.Content = CurrentLayout[1][9];
            Rect_20.Fill = white;
            Key_21.Content = CurrentLayout[1][10];
            Rect_21.Fill = white;

            //Third row update
            Key_Z.Content = CurrentLayout[2][0];
            Rect_Z.Fill = white;
            Key_X.Content = CurrentLayout[2][1];
            Rect_X.Fill = white;
            Key_C.Content = CurrentLayout[2][2];
            Rect_C.Fill = white;
            Key_V.Content = CurrentLayout[2][3];
            Rect_V.Fill = white;
            Key_B.Content = CurrentLayout[2][4];
            Rect_B.Fill = white;
            Key_N.Content = CurrentLayout[2][5];
            Rect_N.Fill = white;
            Key_M.Content = CurrentLayout[2][6];
            Rect_M.Fill = white;
            Key_30.Content = CurrentLayout[2][7];
            Rect_30.Fill = white;
            Key_31.Content = CurrentLayout[2][8];
            Rect_31.Fill = white;
            Key_32.Content = CurrentLayout[2][9];
            Rect_32.Fill = white;
        }
        */
}