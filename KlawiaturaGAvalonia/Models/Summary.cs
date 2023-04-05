using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KlawiaturaAG
{
    public class Summary
    {
        public int IdPokolenia { get; set; }
        public double BestFitness { get; set; }
        public double AvgFitness { get; set; }

        public Summary(int idPokolenia, double bestFitness, double avgFitness)
        {
            IdPokolenia = idPokolenia;
            BestFitness = bestFitness;
            AvgFitness = avgFitness;
        }
    }
}
