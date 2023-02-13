using ConsoleApp.DataStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp
{
    internal class Runner
    {
        private Problem problem;

        public Runner(Problem problem)
        {
            this.problem = problem;
        }

        public delegate Benchmark BenchmarkDataStructure();
        public void Run(params BenchmarkDataStructure[] benchmarks)
        {
            foreach (var bench in benchmarks)
            {
                bench.Invoke();
            }
        }

        public Benchmark TrieBenchmark()
        {
            Benchmark benchmark = new Benchmark();
            Stopwatch stopwatch = Stopwatch.StartNew();
            Trie trie = new Trie(problem.Text);
            trie.GetStrings(problem.Query.P1);
            stopwatch.Stop();
            benchmark.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            DisplayBenchmark(benchmark);
            return benchmark;
        }

        public Benchmark PrecomputedBenchmark()
        {
            Benchmark benchmark = new Benchmark();
            Stopwatch stopwatch = Stopwatch.StartNew();
            var trie = new PrecomputedSubstrings(problem.Text);
            trie.Report(problem.Query);
            stopwatch.Stop();
            benchmark.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            DisplayBenchmark(benchmark);
            return benchmark;
        }

        public void DisplayBenchmark(Benchmark benchmark)
        {
            Console.WriteLine(benchmark.ElapsedMilliseconds.ToString());
        }
    }
}
