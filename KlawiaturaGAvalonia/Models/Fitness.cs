using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Microsoft.Win32.SafeHandles;

namespace KlawiaturaGAvalonia.Models;

public static class Fitness
{
    //ToDo: an expanded Fn selection class, with a simple Workman, expanded Workman (from workmanlayout.org) and maybe the grading system from patorjk.com, with various literature to check against

    //częstotliwości znaków w języku Angielskim
    private static Dictionary<string, double> charFreq = new() {
        {"A", 8.4966}, {"B", 2.0720}, {"C", 4.5388}, {"D", 3.3844}, {"E", 11.1607},
        {"F", 1.8121}, {"G", 2.4705}, {"H", 3.0034}, {"I", 7.5448}, {"J", 0.1965},
        {"K", 1.1016}, {"L", 5.4893}, {"M", 3.0129}, {"N", 6.6544}, {"O", 7.1635},
        {"P", 3.1671}, {"Q", 0.1962}, {"R", 7.5809}, {"S", 5.7351}, {"T", 6.9509},
        {"U", 3.6308}, {"V", 1.0074}, {"W", 1.2899}, {"X", 0.2902}, {"Y", 1.7779},
        {"Z", 0.2722},
        {"[", 0.002}, {"]", 0.002}, {";", 0.0351}, {"-", 0.0252}, {"=", 0.0155},
        {"'", 0.054}, {",", 0.13688}, {".", 0.14511}, {"?", 0.0644}
    };

    //koszty przycisków wg. metody ewaluacji Worksmana w układzie {12, 11, 10}
    private static double[][] koszt = {
        new double[] { 4, 2, 2, 3, 4, 4, 3, 2, 2, 4, 5, 5 },
        new [] { 1.5, 1, 1, 1, 3, 3, 1, 1, 1, 1.5, 3 },
        new double[] { 4, 4, 3, 2, 4, 4, 2, 3, 4, 4 }
    };
    
    public static double Fn(FitnessSettings fs, string[][] input)
    {
        double sum = 0;
        if (!fs.AdvancedMode)
        {
            for (int i = 0; i < input.Length; i++)
            {
                for (int k = 0; k < input[i].Length; k++)
                {
                    //sumowanie [koszt klawisza] * [częstotliwość znaku]
                    sum += koszt[i][k] * charFreq[input[i][k]];
                }
            }
        }
        else
        {
            //todo advanced FN calculations
        }
        //zwracanie sumy
        return sum;
    }

    static (int r, int c) CharToPos(string layout, char x)
    {
        int pos = layout.IndexOf(x);
        if (pos >= 0 && pos < layout.Length)
        {
            if (pos >= 23)
                return (2,pos - 23);
            if (pos < 12)
                return (0, pos);
            return (1, pos - 12);
        }
        return (-1, -1);
    }

    class AlternatingHandUsage
    {
        private string _leftChars="";
        private bool leftright;
        private char previous;

        private int sameHand=0;
        private int diffHand=0;

        public AlternatingHandUsage(string layout)
        {
            string[] temp = new string[3];
            temp[0] = layout.Substring(0, 5);
            temp[1] = layout.Substring(12, 5);
            temp[2] = layout.Substring(23, 5);
            _leftChars = temp[0] + temp[1] + temp[2];
        }

        public void NextLetter(char x)
        {
            if (previous == null)
            {
                if (!_leftChars.Contains(x))
                    leftright = true;
                previous = x;
            }
            else
            {
                if (previous == x)
                    sameHand++;
                else
                {
                    if (_leftChars.Contains(x)){
                        if (leftright)
                            sameHand++;
                        else
                            diffHand++;
                    }else {
                        if (leftright)
                            diffHand++;
                        else
                            sameHand++;
                    }
                }
            }
        }
        
    }
}