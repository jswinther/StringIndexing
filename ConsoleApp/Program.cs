using System;
using System.Diagnostics;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var s1 = new SuffixTree(DummyData.DNA);

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            s1.Run();
            stopwatch.Stop();
            Console.WriteLine();
        }
    }
}

