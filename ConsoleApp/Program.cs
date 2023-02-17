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
                DummyData.DNA_512,
                DummyData.DNA_1024,
                DummyData.DNA_2048,
                DummyData.DNA_4096,
                DummyData.DNA_8192,
                DummyData.DNA_16384
            };

            foreach (var dna in dnas)
            {
                string text = dna;
                string p1 = dna.Substring(0, 10);
                Random random = new Random();
                int x = 9;
                string p2 = dna.Substring(19, 10);
                Query query = new Query(p1, x, p2);
                Problem problem = new Problem(text, query);
                Runner runner = new Runner(problem);

            runner.Run(
                runner.SuffixArrayBenchmark
            );
            }

            
            

            
        }
    }
}

