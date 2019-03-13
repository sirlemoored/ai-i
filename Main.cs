using System;
using System.Collections.Generic;

namespace si_1
{
    public class Program
    {
        static void Main(string[] args)
        {


            Environment e = new Environment(100, "trivial_1.ttp");
            e.LoadData();
            e.CreateDistanceMatrix();
            e.InitializePopulation();
            e.PrintPopulation();
            Console.Read();
        }
    }
}
