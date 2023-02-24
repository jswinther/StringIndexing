using ConsoleApp.DataStructures;
using System;
using System.Diagnostics;
using static ConsoleApp.DataStructures.AlgoSuffixTreeProblem;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp
{
    internal class Program
    {
       
        

        static void Main(string[] args)
        {
            string[] dnas = new string[] {
                DummyData.DNA_65536
            };

            foreach (var dna in dnas)
            {
                string text = dna;
                string p1 = "gca";
                Random random = new Random();
                int x = 1;
                string p2 = "tag";
                Query query = new Query(p1, x, p2);
                query.Y = (3, 10);
                Problem problem = new Problem(text, query);
                Runner runner = new Runner(problem);

            runner.Run(
                runner.SuffixArrayBenchmark, runner.BaratgaborBenchmark,
                runner.SuffixArrayBenchmark, runner.SuffixArrayBenchmarkSortedSet
            );
            }
            //BaratgaborSuffixTree SF = new BaratgaborSuffixTree();
            //SF.AddString("banana");
            //Console.Write(SF.PrintTree());

            
            

            
        }
    }
}

