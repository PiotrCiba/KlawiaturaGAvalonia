﻿using System;
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
            Chromosom[] output = new Chromosom[2];
            
            //preparing the probabilities for each candidate
            double[] probabilities = new double[candidates.Length];
            int len = candidates.Length;
            double total = 0;
            for (int i = 0; i < len; i++)
            {
                total += 1.0 / candidates[i].fitness;
            }

            for (int i = 0; i < len; i++)
            {
                double probability = (1.0 / candidates[i].fitness) / total;
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
                int value = indexesToRoll[k];
                indexesToRoll[k] = indexesToRoll[n];
                indexesToRoll[n] = value;
            }

            total = 0;
            foreach (var i in indexesToRoll)
            {
                total += probabilities[i];
            }

            //spinning the roulette 2 times
            HashSet<int> set = new HashSet<int>();
            double spin;
            double sum;
            while (set.Count < 2)
            {
                spin = rng.NextDouble() * total;
                sum = 0;
                for (int k = 0; k < indexesToRoll.Count; k++)
                {
                    sum += probabilities[indexesToRoll[k]];
                    if (spin < sum)
                    {
                        set.Add(indexesToRoll[k]);
                    }
                }
            }

            int[] chosenIdexes = set.ToArray();

            output[0] = candidates[chosenIdexes[0]];
            output[1] = candidates[chosenIdexes[1]];

            return output;
        }

        private static Chromosom[] RankSelectionAlgorithm(Chromosom[] candidates, double selPress)
        {
            var temp = candidates.OrderBy(o=>o.fitness).ToArray();
            Chromosom[] output = new Chromosom[2];

            //preparing the probabilities for each candidate
            double[] probabilities = new double[candidates.Length];
            int len = candidates.Length;
            
            for (int i = 0; i < len; i++)
            {
                double probability = (selPress - (2 * selPress - 2) * i / (len - 1))/len;
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
                int value = indexesToRoll[k];
                indexesToRoll[k] = indexesToRoll[n];
                indexesToRoll[n] = value;
            }

            double total = 0;
            foreach (var i in indexesToRoll)
            {
                total += probabilities[i];
            }

            //spinning the roulette 2 times
            HashSet<int> set = new HashSet<int>();
            double spin;
            double sum;
            while (set.Count < 2)
            {
                spin = rng.NextDouble() * total;
                sum = 0;
                for (int k = 0; k < indexesToRoll.Count; k++)
                {
                    sum += probabilities[indexesToRoll[k]];
                    if (spin < sum)
                    {
                        set.Add(indexesToRoll[k]);
                    }
                }
            }

            int[] chosenIdexes = set.ToArray();

            output[0] = temp[chosenIdexes[0]];
            output[1] = temp[chosenIdexes[1]];

            return output;
        }
    }
}
