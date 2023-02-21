using ConsoleApp.DataStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp.DataStructures.AlgoSuffixTreeProblem;

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

        public Benchmark SuffixArrayBenchmark()
        {
            Benchmark benchmark = new Benchmark();
            Stopwatch stopwatch = Stopwatch.StartNew();
            var suffixArray = new SuffixArrayKarkkainan(problem.Text);
            stopwatch.Stop();
            benchmark.ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            stopwatch = Stopwatch.StartNew();
            var occs = suffixArray.ReportAllOccurrences(problem.Query);
            stopwatch.Stop();
            benchmark.QueryTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            DisplayBenchmark(benchmark);
            foreach (var occ in occs)
            {
                global::System.Console.WriteLine(occ);
            }
            return benchmark;
        }

        public Benchmark SuffixOtherBenchmark()
        {
            Benchmark benchmark = new Benchmark();
            Stopwatch stopwatch = Stopwatch.StartNew();
            var su = new SuffixTreeOther(problem.Text);
            //su.Visualize();
            su.ReportAllOccurrences(problem.Query);
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

        public Benchmark AlgoSuffixTreeBenchmark()
        {
            Benchmark benchmark = new Benchmark();
            Stopwatch stopwatch = Stopwatch.StartNew();
            var algosuffixtree = new AlgoSuffixTree(problem.Text);
            algosuffixtree.print();
            stopwatch.Stop();
            benchmark.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            DisplayBenchmark(benchmark);
            return benchmark;
        }

        public void DisplayBenchmark(Benchmark benchmark)
        {
            Console.WriteLine($"Construction Time: {benchmark.ConstructionTimeMilliseconds}ms");
            Console.WriteLine($"Query Time: {benchmark.QueryTimeMilliseconds}ms");
        }
    }
}
