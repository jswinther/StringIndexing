using ConsoleApp.DataStructures;
using System;
using System.Diagnostics;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string text = DummyData.DNA10000;
            string p1 = "tat";
            Random random = new Random();
            int x = random.Next(text.Length);
            string p2 = "ctc";
            Query query = new Query(p1, x, p2);
            Problem problem = new Problem(text, query);
            Runner runner = new Runner(problem);

            runner.Run(
                runner.TrieBenchmark, 
                runner.TrieBenchmark,
                runner.TrieBenchmark,
                runner.PrecomputedBenchmark
            );
        }
    }
}

