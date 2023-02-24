using ConsoleApp.DataStructures;
using Gma.DataStructures.StringSearch;
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
                DummyData.DNA_8192
                //DummyData.DNA_262144
            };

            foreach (var dna in dnas)
            {
                string text = dna;
                //string text = "banana$";
                string p1 = "t";
                //string p1 = "a";
                Random random = new Random();
                int x = random.Next();
                string p2 = "g";
                //string p2 = "a";
                Query query = new Query(p1, x, p2);
                query.Y = (3, 10);
                Problem problem = new Problem(text, query);
                Runner runner = new Runner(problem);

            runner.Run(
                //runner.SuffixArrayBenchmark, runner.BaratgaborBenchmark,
                runner.UkkonenBenchmark
            );
            }

            
            

            
        }
    }
}

