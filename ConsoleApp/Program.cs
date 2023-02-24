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
                //DummyData.DNA_512,
                DummyData.DNA_262144
            };

            foreach (var dna in dnas)
            {
                string text = dna;
                //string text = "mississippi";
                string p1 = text.Substring(0, 10);
                //string p1 = "ss";
                Random random = new Random();
                int x = 0;
                string p2 = text.Substring(15, 10);
                //string p2 = "a";
                Query query = new Query(p1, x, p2);
                query.Y = (5, 15);
                Problem problem = new Problem(text, query);
                Runner runner = new Runner(problem);

            runner.Run(
                runner.SuffixArrayBenchmark, runner.SuffixArrayBenchmarkSortedSet, runner.BaratgaborBenchmark, runner.SuffixOtherBenchmark
            );
            }
            //BaratgaborSuffixTree SF = new BaratgaborSuffixTree();
            //SF.AddString("banana");
            //Console.Write(SF.PrintTree());

            
            

            
        }
    }
}

