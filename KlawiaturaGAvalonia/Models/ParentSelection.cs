using System;
using System.Collections.Generic;
using System.Linq;

namespace KlawiaturaGAvalonia.Models
{
    public static class ParentSelection
    {
        public static Chromosom[] Select(Chromosom[] candidates, int mode, double selectionPressure)
        {
            switch (mode)
            {
                case 0: //Turniej 
                    return TournamentSelectionAlgorithm(candidates);
                case 1: //Ruletka 
                    return RoulletteSelectionAlgorithm(candidates);
                case 2: //Rank Selection
                    return RankSelectionAlgorithm(candidates, selectionPressure);
                default:    //Turniej
                    return TournamentSelectionAlgorithm(candidates);
            }
        }

        private static Chromosom[] TournamentSelectionAlgorithm(Chromosom[] candidates)
        {
            Chromosom[] output = new Chromosom[2];
            //Randomizing 4 different indexes of candidates
            Random rnd= new Random();
            int maxIndex = candidates.Length;
            HashSet<int> set = new HashSet<int>();
            int number;
            while(set.Count < 4) {
                number = rnd.Next(maxIndex);
                set.Add(number);
            }
            int[] contestants = set.ToArray();

            //first parent
            if (candidates[contestants[0]].fitness < candidates[contestants[1]].fitness)
                output[0] = candidates[contestants[0]];
            else
                output[0] = candidates[contestants[1]];

            //second parent
            if (candidates[contestants[2]].fitness < candidates[contestants[3]].fitness)
                output[1] = candidates[contestants[2]];
            else
                output[1] = candidates[contestants[3]];

            return output;
        }
        private static Chromosom[] RoulletteSelectionAlgorithm(Chromosom[] candidates)
        {
            Chromosom[] candSorted = candidates.OrderBy(o => o.fitness).ToArray();
            //total fitness of pop
            int len = candSorted.Length;
            double[] inverseFitness = new double[len];
            double total = 0;
            for (int i = 0; i < len; i++)
            {
                inverseFitness[i] = 1.0 / candSorted[i].fitness;
                total += inverseFitness[i];
            }
            
            //set of selected chromosomes (there'll be only 2)
            HashSet<Chromosom> output = new HashSet<Chromosom>();

            //speen the wheel until you select 2 different Chromosomes
            Random rnd = new Random();
            while (output.Count < 2)
            {
                double roll = rnd.NextDouble()*total;
                int selectedIndex = 0;
                double tempSum = inverseFitness[selectedIndex];
                while (tempSum<roll)
                {
                    selectedIndex++;
                    tempSum += inverseFitness[selectedIndex];
                }

                if (selectedIndex < len)
                    output.Add(candSorted[selectedIndex]);
            }

            return output.ToArray();
        }

        private static Chromosom[] RankSelectionAlgorithm(Chromosom[] candidates, double selPress)
        {
            var temp = candidates.OrderBy(o=>o.fitness).ToArray();
            Chromosom[] output = new Chromosom[2];

            //preparing the probabilities for each candidate
            double[] probabilities = new double[candidates.Length];
            int len = candidates.Length;
            
            double c1 = selPress / len;
            double c2 = (2 * selPress - 2) / (len * len - len);
            
            for (int i = 0; i < len; i++)
            {
                double probability = c1 - c2 * (i + 1);
                probabilities[i] = probability;
            }

            //preparing the list of indexes based on how big the probability is
            List<int> indexesToRoll = new List<int>();

            for (int i = 0; i < len; i++)
            {
                int numFieldsToAdd = (int)(probabilities[i] * 100);
                for (int k = 0; k < numFieldsToAdd; k++)
                {
                    indexesToRoll.Add(i);
                }
            }

            //shuffling the list of indexes using Fisher-Yates algorithm
            Random rng = new Random();
            int n = indexesToRoll.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (indexesToRoll[k], indexesToRoll[n]) = (indexesToRoll[n], indexesToRoll[k]);
            }

            //spinning the roulette 2 times
            HashSet<Chromosom> set = new HashSet<Chromosom>();
            int spin;
            while (set.Count < 2)
            {
                spin = rng.Next(indexesToRoll.Count);
                set.Add(temp[indexesToRoll[spin]]);
            }

            return set.ToArray();
        }
    }
}
