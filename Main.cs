using System;
using System.Collections.Generic;

namespace si_1
{
    public class Program
    {
        static void Main(string[] args)
        {
            Environment e = new Environment(100, "easy_0.ttp", 0.1f, 0.6f, 0.1f, 2f);
            e.LoadData();
            e.CreateDistanceMatrix();
            e.PrintNodes();
            Console.Read();
            e.InitializePopulation();
            for (int i = 0; i < 1000; i++)
            {
                e.AssessPopulationFitness();
                //e.SelectRoulette();
                e.SelectTournament();
                e.CrossoverPopulation();
                e.MutatePopulation();
                Console.WriteLine("gen " + i);
            }
            e.ExportIndividual(e.getBest());
            Console.WriteLine(e.getBest()._costG);
            Console.WriteLine(e.getBest()._costF);
            Console.Read();
        }
    }
}
