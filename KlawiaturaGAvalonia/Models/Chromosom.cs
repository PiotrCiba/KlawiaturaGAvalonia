using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KlawiaturaAG
{
    public class Chromosom
    {
        public string layout { get; set; } = "QWERTYUIOP[]ASDFGHJKL;\'ZXCVBNM,.?";
        public double fitness { get; set; }
    }
}
