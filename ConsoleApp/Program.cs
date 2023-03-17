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
        public static PatternMatcher BuildSuffixArray_V1(string str)
        {
            return new SuffixArray_V1(str);
        }

        public static PatternMatcher BuildSuffixArray_V2(string str)
        {
            return new SuffixArray_V2(str);
        }

        public static PatternMatcher BuildSuffixArray_V3(string str)
        {
            return new SuffixArray_V3_Ger(str);
        }

        public static PatternMatcher BuildSuffixTree(string str)
        {
            return SuffixTree.Create(str);
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
                    var occs = patternMatcher.Matches(query.P1);
                    stopwatch.Stop();
                    benchmark.SinglePatternMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmark.SinglePatternMatchesQueryOccs = occs.Count();

                }
                catch (NotImplementedException)
                {
                    benchmark.SinglePatternMatchesQuery = -1;
                }

                // Double Pattern + Fixed Gap
                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.Matches(query.P1, query.X, query.P2);
                    stopwatch.Stop();
                    benchmark.DoublePatternFixedMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmark.DoublePatternFixedMatchesQueryOccs = occs.Count();
                }
                catch (NotImplementedException)
                {
                    benchmark.DoublePatternFixedMatchesQuery = -1;
                }
                
                // Double Pattern + Variable Gap
                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.Matches(query.P1, query.Y.Min, query.Y.Max, query.P2);
                    stopwatch.Stop();
                    benchmark.DoublePatternVariableMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmark.DoublePatternVariableMatchesQueryOccs = occs.Count();
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
                //DummyData.Dummy,
                DummyData.DNA("TEST"),
                //DummyData.DNA("DNA_512"),
                //DummyData.DNA("DNA_262144"),
                //DummyData.DNA("DNA_524288"),
                //DummyData.DNA("DNA_1048576"),
                //DummyData.DNA("DNA_2097152"),
                //DummyData.DNA("DNA_4194304"),
                //DummyData.DNA("DNA_8388608"),
                //DummyData.DNA("DNA_16777216"),
                //DummyData.DNA("DNA_33554432")
            };

            BuildDataStructure[] dataStructures = new BuildDataStructure[]
            {
                //BuildUkkonen,
                //BuildSuffixArray_V2,
                //BuildSuffixArray_V1,
                BuildSuffixTree,
                BuildSuffixArray_V3,
                //BuildPrecomputed,
            };
            
            var table = new ConsoleTable("Data Structure & Data", "Construction Time MS", "Single Pattern Query Time MS", "Double Pattern Fixed Query Time MS", "Double Pattern Variable Query Time MS");

            string p1 = "a";
            Random random = new Random();
            int x = 1;
            string p2 = "a";
            Query query = new Query(p1, x, p2);
            query.Y = (1, 45);

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

