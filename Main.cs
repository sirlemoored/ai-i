using System;

namespace si_1
{
    public class Entry
    {
        static void Main(string[] args)
        {
            Environment e = new Environment(10);
            e.LoadData();
            e.CreateDistanceMatrix();
            //e.InitializePopulation();
            for (int i = 0; i < 100; i++)
            {
                Individual ind = new Individual(439);
                Console.WriteLine(e.CalculateCostF(ind));
            }
            
            Console.Read();
        }
    }
}
