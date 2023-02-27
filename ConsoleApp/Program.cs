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
        

        public delegate PatternMatcher BuildDataStructure(string str);
        public static PatternMatcher BuildSuffixArray(string str)
        {
            return new SuffixArray(str);
        }

        public static PatternMatcher BuildPrecomputed(string str)
        {
            return new PrecomputedSubstrings(str);
        }

        public static PatternMatcher BuildUkkonen(string str)
        {
            
            return new UkkonenWrapper(str);
        }

        public static PatternMatcher BuildSuffixOther(string str)
        {
            return new SuffixTreeOther(str);
        }

        public static PatternMatcher BuildBaratgabor(string str)
        {
            return new BaratgaborSuffixTree(str);
        }

        public static Benchmark BenchDataStructure(BuildDataStructure matcher, string str, params Query[] queries) 
        { 
            Benchmark benchmark = new Benchmark();
            Stopwatch stopwatch = Stopwatch.StartNew();
            // Construction
            var patternMatcher = matcher.Invoke(str);
            stopwatch.Stop();
            benchmark.ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            // Query Time
            stopwatch = Stopwatch.StartNew();
            foreach (var query in queries)
            {
                // Single Pattern
                try
                {
                    benchmark.SinglePatternMatches = patternMatcher.Matches(query.P1).Count();
                }
                catch (NotImplementedException)
                {

                }

                // Double Pattern + Fixed Gap
                try
                {
                    benchmark.DoublePatternFixedMatches = patternMatcher.Matches(query.P1, query.X, query.P2).Count();
                }
                catch (NotImplementedException)
                {

                }
                
                // Double Pattern + Variable Gap
                try
                {
                    benchmark.DoublePatternVariableMatches = patternMatcher.Matches(query.P1, query.Y.Min, query.Y.Max, query.P2).Count();
                }
                catch (NotImplementedException)
                {

                }
                
            }
            stopwatch.Stop();
            benchmark.QueryTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            benchmark.DataStructureName = patternMatcher.GetType().Name;
            benchmark.DataName = $"DNA_{str.Length}";
            
            return benchmark;
        }

        internal class Benchmark
        {
            public string DataStructureName { get; set; }
            public string DataName { get; set; }
            public long ConstructionTimeMilliseconds { get; set; }
            public long QueryTimeMilliseconds { get; set; }
            public int SinglePatternMatches { get; internal set; }
            public int DoublePatternFixedMatches { get; internal set; }
            public int DoublePatternVariableMatches { get; internal set; }

            public override string ToString()
            {
                return $"\nData Structure: {DataStructureName}\nDataset Size: {DataName}\nConstruction Time: {ConstructionTimeMilliseconds}ms\nQuery Time: {QueryTimeMilliseconds}ms\n" +
                    $"Single Matches: {SinglePatternMatches}\nDouble Matches Fixed: {DoublePatternFixedMatches}\nDouble Matches Variable: {DoublePatternVariableMatches}";
            }
        }

        static void Main(string[] args)
        {
            string[] dnaSequences = new string[] 
            {
                DummyData.DNA_512,
                DummyData.DNA_1024,
                DummyData.DNA_2048,
                DummyData.DNA_4096,
                DummyData.DNA_8192,
                DummyData.DNA_16384,
                DummyData.DNA_32768,
                DummyData.DNA_65536,
                DummyData.DNA_131072,
                DummyData.DNA_262144
            };

            BuildDataStructure[] dataStructures = new BuildDataStructure[]
            {
                BuildSuffixArray
        
            };

            string p1 = "t";
            Random random = new Random();
            int x = random.Next(5, 10);
            string p2 = "a";
            Query query = new Query(p1, x, p2);
            query.Y = (5, 10);

           
            var benchmarks = new List<Benchmark>();

            foreach (var sequence in dnaSequences)
            {
                foreach (var dataStructure in dataStructures)
                {
                    var benchmark = BenchDataStructure(dataStructure, sequence, query);
                    Console.WriteLine(benchmark);
                    benchmarks.Add(benchmark);
                }
            }

            
        }
    }
}

