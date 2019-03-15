using System;
using System.Collections.Generic;

namespace si_1
{
    public class Program
    {
        static void Main(string[] args)
        //                                 pop.                mut.  crss.  perc.  exp.  trn.   elitism
        //                                 size                prob. prob.  mut.         size   percentage
        { 
            Environment e = new Environment(100, "hard_0.ttp", 0.1f, 0.7f, 0.05f, 1.0f, 4 ,  0.0f, Environment.SELECTION_TOURNAMENT);
            e.LoadData();
            e.CreateDistanceMatrix();
            e.InitializePopulation();
            float bestCost = float.MinValue;
            for (int i = 0; i < 100; i++)
            {
                e.AssessPopulationFitness();
                e.Selection();
                e.PreserveElite();
                e.CrossoverPopulation();
                e.MutatePopulation();
                e.RestoreElite();
                
                Individual bestInd = e.getBest();
                if (bestInd.GetTotalCost() > bestCost)
                {
                    bestCost = bestInd.GetTotalCost();
                    Console.WriteLine("nowy najlepszy osobnik, generacja " + i + ", wynik: " + bestCost);
                }
                if (i % 2000 == 0)
                {
                    Console.WriteLine("\ngeneracja " + i + "\neksport najlepszego osobnika do .csv\n");
                    e.ExportIndividual(bestInd);
                }
            }
            Console.Read();
        }
    }
}
