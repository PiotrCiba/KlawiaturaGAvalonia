using System;
using System.Collections.Generic;
using System.Linq;

namespace KlawiaturaGAvalonia.Models
{
    public class GeneticAlgorithm
    {
        //Piotr Ciba

        public static (List<Summary>, List<Chromosom[]>) Start(Settings s, IProgress<int> progress)
        {
            //Czacza idzie tak: Rodzice > Fitness > CarryOver > Selekcja > Crossover > Mutacja > Dzieci > "Rodzice = Dzieci" > repeat

            //preparing the output variable;
            List<Summary> genSummaries = new List<Summary>();
            List<Chromosom[]> pokolenia = new List<Chromosom[]>();

            //Parent generation
            List<Chromosom> parents = new List<Chromosom>();

            //prepping Parents, random scramble
            for (int i = 0; i < s.Main.popSize; i++)
                parents.Add(new Chromosom());
            parents = ScrambleParentsLayouts(parents);

            //calculating the Parents' fitness
            foreach (var p in parents)
            {
                p.fitness = Fitness.Fn(s.FitSet,StringToLayout(p.layout));
            }

            //sorting Parents by fitness
            parents = parents.OrderBy(p => p.fitness).ToList();

            //adding the 0-th generation to Pokolenia
            pokolenia.Add(parents.ToArray());

            //do-while control variables breakcondition to exit, get to count generations (for generation number exit)
            bool breakCondition = false;
            int gensToMaintainEps = 100;
            int gen = 0;

            //creating the first Summary for GenSummaries
            var fitnesses = (from p in parents orderby p.fitness select p.fitness);
            Summary currGenSummary = new Summary(gen, fitnesses.First(), fitnesses.Average());

            genSummaries.Add(currGenSummary);

            //do-while for the GA's core
            do
            {
                gen++;  //increment gens for gen-based stop condition

                //prep children List
                List<Chromosom> children = new List<Chromosom>();

                //carryover-breedingPop from parents
                (Chromosom[] carry, Chromosom[] breeders) carryAndPop = CarryModule.Select(parents, s.Repop.carryoverType, s.Repop.carryVar, s.Repop.culling, s.Repop.cullingRate);

                //copy carry to children
                foreach (var c in carryAndPop.carry)
                {
                    children.Add(c);
                }

                //while children < parents
                while (children.Count < s.Main.popSize)
                {
                    //parentSelection, from breedingPop
                    Chromosom[] couple = ParentSelection.Select(carryAndPop.breeders, s.Repop.currSel, s.Repop.SelPressure);

                    //crossover operator, from selected parents
                    Chromosom[] childrenTemp = CrossoverModule.Select(couple, s.Repop.currX, s.Repop.childNumber);


                    //iterate over newly born chlidren
                    for (int i = 0; i <= s.Repop.childNumber; i++)
                    {
                        //mutationModule, may hit, may not hit
                        childrenTemp[i] = MutationModule.Select(childrenTemp[i], s.Repop.currMut, s.Repop.mutChance, s.Repop.mutSeverity);
                        //add new chlidren to children
                        children.Add(childrenTemp[i]);
                    }
                }

                //calculate fitness of all children
                foreach (var c in children)
                {
                    if (c.fitness == 0)
                        c.fitness = Fitness.Fn(s.FitSet,StringToLayout(c.layout));
                }
                //sort ascending
                children = (from c in children orderby c.fitness ascending select c).ToList();

                //if children overflow, cull the lowest fitness one
                while (children.Count > s.Main.popSize)
                {
                    children.Remove(children.Last());
                }

                //parents = children
                parents = children.OrderBy(o => o.fitness).ToList();

                //if fullmemory, add children to Pokolenia
                if (s.Main.fullMemory)
                {
                    pokolenia.Add(parents.ToArray());
                }
                //summary prep, add to GenSummaries
                fitnesses = (from p in parents select p.fitness);
                currGenSummary = new Summary(gen, fitnesses.First(), fitnesses.Average());
                genSummaries.Add(currGenSummary);

                int report = 0;

                if (s.Main.currStopMode)
                {
                    //update the progress for progressbar
                    report = (int)((double)gen / s.Main.gensToRun * 100);

                    //gens to run mode
                    if (gen >= s.Main.gensToRun)
                    {
                        breakCondition = true;
                    }
                }
                else
                {
                    //stop at set improvement
                    Summary[] temp = genSummaries.TakeLast(2).ToArray();
                    double currentEps = (temp[0].BestFitness - temp[1].BestFitness) / temp[1].BestFitness;
                    //calculate the report
                    report = (int)((s.Main.epsToStopAt / currentEps) * 100);
                    if (report < 0)
                        report = 0;

                    if (currentEps <= s.Main.epsToStopAt)
                    {
                        //the set improvement has to keep at it for ~10 rounds before stopping
                        if (gensToMaintainEps == 0)
                            breakCondition = true;
                        else
                            gensToMaintainEps--;
                    }
                    else
                    {
                        //if the low eps streak is broken, reset the counter
                        gensToMaintainEps = 100;
                    }

                }
                progress.Report(report);
            } while (!breakCondition);

            //return the results of the algorithm
            return (genSummaries, pokolenia);
        }

        /*                     0  1  2  3  4  5  6  7  8  9  10  11
                new double[] { 4, 2, 2, 3, 4, 5, 3, 2, 2, 4, 4, 5 },
                               12   13 14 15 16 17 18 19 20 21   22
                new double[] { 1.5, 1, 1, 1, 3, 3, 1, 1, 1, 1.5, 3 },
                               23 24 25 26 27 28 29 30 31 32
                new double[] { 4, 4, 3, 2, 5, 3, 2, 3, 4, 4 }
         */
        
        public static string LayoutToString(string[][] input)
        {
            string output="";
            for (int i = 0; i < 3; i++)
                output += string.Join("", input[i]);
            return output;
        }
        public static string[][] StringToLayout(string input)
        {
            //QWERTYUIOP[]ASDFGHJKL;'ZXCVBNM,.?
            string[] temp = new string[3];
            temp[0] = input.Substring(0, 12);
            temp[1] = input.Substring(12, 11);
            temp[2] = input.Substring(23, 10);
            string[][] output = new string[3][];
            for (int i = 0; i < 3; i++)
                output[i] = temp[i].Select(x => new string(x, 1)).ToArray();
            return output;
        }
        public static List<Chromosom> ScrambleParentsLayouts(List<Chromosom> input)
        {
            List<Chromosom> output = input;
            int len = input.Count;

            Random rand = new Random();

            for (int i = 0; i < len; i++)
            {
                char[] chars = output[i].layout.ToCharArray();
                for (int k = 0; k < chars.Length; k++)
                {
                    int randIndex = rand.Next(chars.Length);
                    (chars[k], chars[randIndex]) = (chars[randIndex], chars[k]);
                }
                output[i].layout = new string(chars);
            }

            return output;
        }
    }
}
