using System.Collections.Generic;
using System.Linq;

namespace KlawiaturaGAvalonia.Models
{
    public static class CarryModule
    {
        public static (Chromosom[], Chromosom[]) Select(List<Chromosom> pop, int type, double variable, bool isCulling, double cullingMargin)
        {
            (Chromosom[] carry, Chromosom[] parents) output;
            output.carry = new Chromosom[0];
            output.parents = new Chromosom[0];

            switch (type)
            {
                case 0:
                    output.parents = pop.ToArray();
                    output.carry = new Chromosom[0];
                    break;
                case 1:
                    output = Elityzm(pop,variable,isCulling,cullingMargin);
                    break;
            }

            return output;
        }

        private static (Chromosom[], Chromosom[]) Elityzm(List<Chromosom> pop, double topMargin, bool culling, double cullingVar)
        {
            (Chromosom[] carry, Chromosom[] parents) output;

            //take topmargin% off the top of the pop and carry it over
            output.carry = pop.OrderBy(p => p.fitness).Take((int)(topMargin / 100 * pop.Count)).ToArray();

            //if culling is on copy over , 
            if (culling)
                output.parents = pop.OrderBy(p => p.fitness).Take(pop.Count - (int)(cullingVar / pop.Count)).ToArray();
            else
                output.parents = pop.ToArray();

            return output;
        }
    }
}
