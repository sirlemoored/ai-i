using System;

namespace si_1
{
    public class Program
    {
        static void Main(string[] args)
        {
            Environment e = new Environment(10, "trivial_0.ttp");
            e.LoadData();
            e.CreateDistanceMatrix();
            e.PrintDistanceMatrix();
            //e.InitializePopulation();
            for (int i = 0; i < 100; i++)
            {
                Individual ind = new Individual(e.getNumberOfNodes());
                Console.WriteLine("Osobnik " + (String.Format("{0, -4}", i + 1)) + "Koszt: " + String.Format("{0, 10}", e.CalculateCostF(ind)) + " trasa: " + ind.PrintRoute());
            }
            
            Console.Read();
        }
    }
}
