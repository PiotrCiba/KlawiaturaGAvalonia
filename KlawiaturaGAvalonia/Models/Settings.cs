namespace KlawiaturaGAvalonia.Models
{
    public class Settings
    {
        //population settings
        public int popSize { get; set; } = 50;
        public int childNumber { get; set; } = 0;
        public int carryoverType { get; set; } = 0;
        public double carryVar { get; set; } = 10;
        public bool culling { get; set; } = false;
        public double cullingRate { get; set; } = 10;

        //re-population settings
        public int currSel { get; set; } = 0;
        public double SelPressure { get; set; } = 1.5;
        public int currX { get; set; } = 0;
        public int currMut { get; set; } = 0;
        public double mutChance { get; set; } = 0.05;
        public int mutSeverity { get; set; } = 5;

        //GA overall settings
        public bool fullMemory { get; set; } = true;
        public bool isanimated { get; set; } = false;
        public bool currStopMode { get; set; } = true;
        public int gensToRun { get; set; } = 250;
        public double epsToStopAt { get; set; } = 0.01;
        
    }
}
