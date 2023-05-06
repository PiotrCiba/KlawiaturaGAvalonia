namespace KlawiaturaGAvalonia.Models
{
    public class Settings
    {
        //fitness settings
        public FitnessSettings FitSet { get; set; } = new();

        //population/run settings - see Main class below
        public Main Main { get; set; } = new();
        
        //repopulation settings - see Repop class below
        public Repop Repop { get; set; } = new();
    }

    public class Main
    {
        //population size
        public int popSize { get; set; } = 50;
        
        //settings for when to run
        public bool fullMemory { get; set; } = true;
        public bool isanimated { get; set; } = false;
        public bool currStopMode { get; set; } = true;
        public int gensToRun { get; set; } = 250;
        public double epsToStopAt { get; set; } = 0.01;
    }

    public class Repop
    {
        //number of children per parent pair
        public int childNumber { get; set; } = 0;
        
        //Carry-over/culling settings
        public int carryoverType { get; set; } = 0;
        public double carryVar { get; set; } = 10;
        public bool culling { get; set; } = false;
        public double cullingRate { get; set; } = 10;
        
        //Selection settings
        public int currSel { get; set; } = 0;
        public double SelPressure { get; set; } = 1.5;
        
        //Crossover settings
        public int currX { get; set; } = 0;
        
        //Mutation settings
        public int currMut { get; set; } = 0;
        public double mutChance { get; set; } = 0.05;
        public int mutSeverity { get; set; } = 5;
    }

    public class FitnessSettings
    {
        //mode selection
        public bool AdvancedMode = false;
    }
}
