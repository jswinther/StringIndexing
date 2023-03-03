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
            return new PreCompSubs(str);
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
            foreach (var query in queries)
            {
                // Single Pattern
                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.Matches(query.P1).Count();
                    stopwatch.Stop();
                    benchmark.SinglePatternMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmark.SinglePatternMatchesQueryOccs = occs;

                }
                catch (NotImplementedException)
                {
                    benchmark.SinglePatternMatchesQuery = -1;
                }

                // Double Pattern + Fixed Gap
                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.Matches(query.P1, query.X, query.P2).Count();
                    stopwatch.Stop();
                    benchmark.DoublePatternFixedMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmark.DoublePatternFixedMatchesQueryOccs = occs;
                }
                catch (NotImplementedException)
                {
                    benchmark.DoublePatternFixedMatchesQuery = -1;
                }
                
                // Double Pattern + Variable Gap
                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.Matches(query.P1, query.Y.Min, query.Y.Max, query.P2).Count();
                    stopwatch.Stop();
                    benchmark.DoublePatternVariableMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmark.DoublePatternVariableMatchesQueryOccs = occs;
                }
                catch (NotImplementedException)
                {
                    benchmark.DoublePatternVariableMatchesQuery = -1;
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
            public long SinglePatternMatchesQuery { get; internal set; }
            public long DoublePatternFixedMatchesQuery { get; internal set; }
            public long DoublePatternVariableMatchesQuery { get; internal set; }
            public int SinglePatternMatchesQueryOccs { get; internal set; }
            public int DoublePatternFixedMatchesQueryOccs { get; internal set; }
            public int DoublePatternVariableMatchesQueryOccs { get; internal set; }
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
                BuildSuffixArray,
                //BuildUkkonen
            };
            
            var table = new ConsoleTable("Data Structure & Data", "Construction Time MS", "Single Pattern Query Time MS", "Double Pattern Fixed Query Time MS", "Double Pattern Variable Query Time MS");

            string p1 = "t";
            Random random = new Random();
            int x = random.Next(5, 10);
            string p2 = "a";
            Query query = new Query(p1, x, p2);
            query.Y = (5, 10);

            foreach (var dataStructure in dataStructures)
            {
                foreach (var sequence in dnaSequences)
                {
                    var b = BenchDataStructure(dataStructure, sequence, query);
                    table.AddRow($"{b.DataStructureName} {b.DataName}", $"{b.ConstructionTimeMilliseconds}", $"{b.SinglePatternMatchesQuery}ms, Occs: {b.SinglePatternMatchesQueryOccs}", $"{b.DoublePatternFixedMatchesQuery}ms, Occs: {b.DoublePatternFixedMatchesQueryOccs}", $"{b.DoublePatternVariableMatchesQuery}ms, Occs: {b.DoublePatternVariableMatchesQueryOccs}");
                }
            }
            table.Options.NumberAlignment = Alignment.Right;
            table.Write();

        }
    }
}

