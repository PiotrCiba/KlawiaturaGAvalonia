using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using Avalonia.Input;
using DynamicData;

namespace KlawiaturaGAvalonia.Models;

public class Fitness
{
    //settings field
    public FitnessSettings currentSettings;
    
    //classes needed to calculate the advanced fitness
    //private TypingStats FU;
    
    //częstotliwości znaków w języku Angielskim
    private Dictionary<string, double> charFreq = new() {
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
    private double[][] koszt = {
        new double[] { 4, 2, 2, 3, 4, 4, 3, 2, 2, 4, 5, 5 },
        new [] { 1.5, 1, 1, 1, 3, 3, 1, 1, 1, 1.5, 3 },
        new double[] { 4, 4, 3, 2, 4, 4, 2, 3, 4, 4 }
    };

    public Fitness()
    {
        currentSettings = new();
    }
    public Fitness(FitnessSettings fs)
    {
        currentSettings = fs;
    }
    public void Update(FitnessSettings fs)
    {
        currentSettings = fs;
    }
    
    public double Fn(string input)
    {
        double sum = 0;
        if (!currentSettings.AdvancedMode)   //simple Workman Fn
        {
            string[][] tempLayout;
            {
                //QWERTYUIOP[]ASDFGHJKL;'ZXCVBNM,.?
                string[] temp = new string[3];
                temp[0] = input.Substring(0, 12);
                temp[1] = input.Substring(12, 11);
                temp[2] = input.Substring(23, 10);
                string[][] output = new string[3][];
                for (int i = 0; i < 3; i++)
                    output[i] = temp[i].Select(x => new string(x, 1)).ToArray();
                tempLayout =  output;
            }
            for (int i = 0; i < tempLayout.Length; i++)
            {
                for (int k = 0; k < tempLayout[i].Length; k++)
                {
                    //sumowanie [koszt klawisza] * [częstotliwość znaku]
                    sum += koszt[i][k] * charFreq[tempLayout[i][k]];
                }
            }
        }
        else
        {
            /*
            public string[] first = {"brak", "naprzemiennie ręce", "ta sama ręka", "naprzemiennie palce", "ten sam palec" };
            public int firstindex = 0;
            public double firstVal = 0.33;
            public string[] second = { "brak", "rozkład palców", "użycie rzędów", "palce + rzędy" };
            public int secondindex = 0;
            public double secondVal = 0.33;
            public string[] third = { "brak", "odległość (km)" };
            public int thirdindex = 0;
            public double thirdVal = 0.33;
             */
            var stats = new TypingStats(input);
            IEnumerable<string> lines = File.ReadLines($"corpus/{currentSettings.filenames[currentSettings.selectedbookindex]}");
            foreach (var line in lines)
            {
                var uppercase = line.ToUpperInvariant();
                foreach (var x in uppercase)
                {
                    stats.NextLetterFinger(x);
                    stats.NextLetterRows(x);
                    stats.NextLetterDistance(x);
                }
            }

            double keystrokes = stats.numberRow + stats.topRow + stats.homeRow + stats.bottomRow;
            
            if (currentSettings.firstindex > 0)
            {
                switch (currentSettings.firstindex)
                {
                    case 1:     //naprzemienne ręce - punkty karne za procent użycia tej samej ręki
                        sum += stats.sameHandUsage / keystrokes * currentSettings.firstVal * 100;
                        break;
                    case 2:     //ta sama ręka - punkty karne za procent użycia naprzemiennych rąk
                        sum+= stats.alternatingHandUsage / keystrokes * currentSettings.firstVal * 100;
                        break;
                    case 3:     //naprzemienne palce - punkty karne z ten sam palec
                        sum+= stats.sameFingerUsage / keystrokes * currentSettings.firstVal * 100;
                        break;
                    case 4:     //ten sam palec - pkt karne za naprzemienne palce
                        sum += stats.alternatingFingerUsage / keystrokes * currentSettings.firstVal * 100;
                        break;
                }
            }

            if (currentSettings.secondindex > 0)
            {
                switch (currentSettings.secondindex)
                {
                    case 1:
                        sum += stats.FingerScore() * currentSettings.secondVal;
                        break;
                    case 2:
                        sum += stats.RowScore() * currentSettings.secondVal;
                        break;
                    case 3:
                        sum += stats.FingerScore() * currentSettings.secondVal +
                               stats.RowScore() * currentSettings.secondVal;
                        break;
                }
            }

            if (currentSettings.thirdindex > 0)
            {
                sum += stats.distance / 1000000 * currentSettings.thirdVal;
            }
        }
        //zwracanie sumy
        return sum;
    }
    
    /*
     * new[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "[", "]" },
       new[] { "A", "S", "D", "F", "G", "H", "J", "K", "L", ";", "'" },
       new[] { "Z", "X", "C", "V", "B", "N", "M", ",", ".", "?" }
     */

    private class TypingStats
    {
        private string _layout;
        private char prevKey;
        //these are for tracking finge usage
        private string[] _fingerFields = { "1!","2@","3#","4$5%","6^7&","8*","9(","0)-_=+:\"/" };
        public int[] _fingerStats = new int[8];
        //these are for tracking alternating/same usage of a finger/hand
        private int _prevFinger = -1;
        public int alternatingFingerUsage = 0;
        public int sameFingerUsage = 0;
        public int alternatingHandUsage = 0;
        public int sameHandUsage = 0;
        //these are for tracking the distance travelled
        public double distance = 0;
        //these are for counting rows used
        public int numberRow = 0;
        public int topRow = 0;
        public int homeRow = 0;
        public int bottomRow = 0;
        public TypingStats(string layout)
        {
            this._layout = layout;
            string[] temp = new string[3];
            temp[0] = layout.Substring(0, 12);
            temp[1] = layout.Substring(12, 11);
            temp[2] = layout.Substring(23, 10);
            foreach (var t in temp)
            {
                _fingerFields[0] += t[0];           //left little finger
                _fingerFields[1] += t[1];           //left ring finger
                _fingerFields[2] += t[2];           //left middle ginger
                _fingerFields[3] += t[3] + t[4];    //left index finger
                _fingerFields[4] += t[5] + t[6];    //right index finger
                _fingerFields[5] += t[7];           //right middle finer
                _fingerFields[6] += t[8];           //right ring finger
                for (int i = 9; i < t.Length; i++)  //right little finger
                {
                    _fingerFields[7] += t[i];
                }
            }
        }

        public void NextLetterFinger(char x)
        {
            int whichFingerField = 0;
            foreach (var finger in _fingerFields)
            {
                if (finger.Contains(x))
                {
                    whichFingerField = _fingerFields.IndexOf(finger);
                    _fingerStats[whichFingerField]++;
                    if (_prevFinger != -1)
                    {
                        //fingers
                        if (_prevFinger == whichFingerField)
                            sameFingerUsage++;
                        else
                            alternatingFingerUsage++;
                        //hands
                        int[] leftHand = { 0, 1, 2, 3 };
                        int prevstate = leftHand.Contains(_prevFinger) ? 1 : -1;
                        int nextstate = leftHand.Contains(whichFingerField) ? 1 : -1;
                        if (prevstate * nextstate > 0)
                        {
                            sameHandUsage++;
                        }
                        else
                        {
                            alternatingHandUsage++;
                        }
                    }
                    _prevFinger = whichFingerField;
                    break;
                }
            }
        }

        public void NextLetterDistance(char x)
        {
            int whichFingerField = -1;
            foreach (var finger in _fingerFields)
            {
                if (finger.Contains(x))
                {
                    whichFingerField = _fingerFields.IndexOf(finger);
                    break;
                }
            }

            if (whichFingerField>=0)
            {
                (int row, int col) = CharToPos(x);
                if (new[] { 3, 4 }.Contains(whichFingerField))
                {
                    var distance1 = Math.Sqrt(Math.Pow(Math.Abs(3 - col)*20, 2) + Math.Pow(Math.Abs(2 - row)*20, 2));
                    var distance2 = Math.Sqrt(Math.Pow(Math.Abs(6 - col)*20, 2) + Math.Pow(Math.Abs(2 - row)*20, 2));
                    distance += distance1 > distance2 ? distance1 : distance2;
                }else if (whichFingerField == 7)
                {
                    distance += Math.Sqrt(Math.Pow(Math.Abs(9 - col) * 20, 2) + Math.Pow(Math.Abs(2 - row) * 20, 2));
                }
                else
                {
                    distance += Math.Abs(2 - row) * 20;
                }
            }
        }

        public void NextLetterRows(char x)
        {
            (int row, _) = CharToPos(x);
            switch (row)
            {
                case 0:
                    numberRow++;
                    break;
                case 1:
                    topRow++;
                    break;
                case 2:
                    homeRow++;
                    break;
                case 3:
                    bottomRow++;
                    break;
            }
        }

        private (int r, int c) CharToPos(char x)
        {
            const string numrow = "1!2@3#4$5%6^7&8*9(0)-_=+";
            int pos = _layout.IndexOf(x);
            if (pos >= 0 && pos < _layout.Length)
            {
                if (pos >= 23)
                    return (3, pos - 23);
                if (pos < 12)
                    return (1, pos);
                return (2, pos - 12);
            }
            return numrow.Contains(x) ? (0, numrow.IndexOf(x)) : (-1, -1);
        }

        public double FingerScore()
        {
            double output = 0;
            double[] wzorzec = { 9, 10, 15, 16, 16, 15, 10, 9 };

            double keystrokes = _fingerStats.Sum();
            
            for (int i = 0; i < wzorzec.Length; i++)
            {
                var deviation = Math.Abs(_fingerStats[i]/keystrokes*100 - wzorzec[i]);
                output += deviation;
            }
            return output;
        }

        public double RowScore()
        {
            double output = 0;
            double[] wzorzec = { 21, 66, 13 };
            double keystrokes = numberRow + topRow + homeRow + bottomRow;
            output += Math.Abs(topRow / keystrokes * 100 - wzorzec[0]);
            output += Math.Abs(homeRow / keystrokes * 100 - wzorzec[1]);
            output += Math.Abs(bottomRow / keystrokes * 100 - wzorzec[2]);
            return output;
        }

        //return -1,0,1 depending on row used in relation to home row
        /*
        public int WhatRow(char nextLetter)
        {
            string[] temp = new string[4];
            temp[0] = "1!2@3#4$5%6^7&8*9(0)-_=+";
            temp[1] = _layout.Substring(0, 12);
            temp[2] = _layout.Substring(12, 11);
            temp[3] = _layout.Substring(23, 10);
            for(int i=0;i<temp.Length;i++)
            {
                if (temp[i].Contains(nextLetter))
                {
                    return i;
                }
            }
            return -1;
        }
        */
    }
}