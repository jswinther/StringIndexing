using ConsoleApp.DataStructures;
using Gma.DataStructures.StringSearch;
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
            var occs = suffixArray.GetOccurrencesWithList(problem.Query);
            stopwatch.Stop();
            benchmark.QueryTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("List Benchmark Suffix Array");
            DisplayBenchmark(benchmark);
            Console.WriteLine("Number of occurrences: " + occs.Count());
            return benchmark;
        }

        public Benchmark SuffixArrayBenchmarkSortedSet()
        {
            Benchmark benchmark = new Benchmark();
            Stopwatch stopwatch = Stopwatch.StartNew();
            var suffixArray = new SuffixArrayKarkkainan(problem.Text);
            stopwatch.Stop();
            benchmark.ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            stopwatch = Stopwatch.StartNew();
            var occs = suffixArray.GetOccurrencesWithSortedSet(problem.Query);
            stopwatch.Stop();
            benchmark.QueryTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("SortedSet Benchmark Suffix Array");
            DisplayBenchmark(benchmark);
            Console.WriteLine("Number of occurrences: " + occs.Count());
            return benchmark;
        }

        public Benchmark UkkonenBenchmark()
        {
            Benchmark benchmark = new Benchmark();
            Stopwatch stopwatch = Stopwatch.StartNew();
            var suffixTree = new CharUkkonenTrie<int>(0);
            suffixTree.Add(problem.Text,0);
            stopwatch.Stop();
            benchmark.ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            stopwatch = Stopwatch.StartNew();
            IEnumerable<WordPosition<int>> occs1 = suffixTree.RetrieveSubstrings(problem.Query.P1);
            IEnumerable<WordPosition<int>> occs2 = suffixTree.RetrieveSubstrings(problem.Query.P2);
            List<(int, int)> occs = new List<(int, int)>();
            stopwatch.Stop();
            benchmark.QueryTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            stopwatch = Stopwatch.StartNew();
            foreach (var item1 in occs1)
            {
                foreach (var item2 in occs2)
                {
                    if (item2.CharPosition == (item1.CharPosition + problem.Query.P1.Length + problem.Query.X))
                    {
                        occs.Add((item1.CharPosition, item2.CharPosition));
                    }
                }
                    
            }
            stopwatch.Stop();
            benchmark.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Ukkonen Benchmark Suffix Tree");
            DisplayBenchmark(benchmark);
            foreach (var item in occs)
            {
                Console.WriteLine(item);
            }
            return benchmark;
        }
            


        public Benchmark SuffixOtherBenchmark()
        {
            Benchmark benchmark = new Benchmark();
            Stopwatch stopwatch = Stopwatch.StartNew();
            var su = new SuffixTreeOther(problem.Text);
            stopwatch.Stop();
            benchmark.ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            stopwatch = Stopwatch.StartNew();
            //su.Visualize();
            var occs = su.ReportAllOccurrences(problem.Query);
            stopwatch.Stop();
            benchmark.QueryTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            DisplayBenchmark(benchmark);
            foreach (var occ in occs)
            {
                global::System.Console.WriteLine(occ);
            }
            return benchmark;
        }

        public Benchmark BaratgaborBenchmark()
        {
            Benchmark benchmark = new Benchmark();
            Stopwatch stopwatch = Stopwatch.StartNew();
            var suffixTree = new BaratgaborSuffixTree();
            suffixTree.AddString(problem.Text);
            stopwatch.Stop();
            benchmark.ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            stopwatch = Stopwatch.StartNew();
            //var occs = suffixTree.ReportAllOccurrences(problem.Query);
            stopwatch.Stop();
            benchmark.QueryTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            DisplayBenchmark(benchmark);
            /*foreach (var occ in occs)
            {
                global::System.Console.WriteLine(occ);
            }*/
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
            Console.WriteLine($"Elapsed Time: {benchmark.ElapsedMilliseconds}ms");
        }
    }
}
