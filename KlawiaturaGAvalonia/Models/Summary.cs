namespace KlawiaturaGAvalonia.Models
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
