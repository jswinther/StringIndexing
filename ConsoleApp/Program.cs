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
            var a = new SuffixArrayKarkkainan("banana$");
            global::System.Console.WriteLine(string.Join(',', a.SuffixArray));







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
                string p1 = "tat";
                Random random = new Random();
                int x = random.Next(text.Length);
                string p2 = "ctc";
                Query query = new Query(p1, x, p2);
                Problem problem = new Problem(text, query);
                Runner runner = new Runner(problem);

            runner.Run(
                runner.SuffixArrayBenchmark, runner.SuffixOtherBenchmark
            );
            }

            
            

            
        }
    }
}

