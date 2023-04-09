﻿using ConsoleApp.DataStructures;
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
            return new SuffixArray_V3(str);
        }

        public static PatternMatcher BuildSuffixArray_V4(string str)
        {
            return new SuffixArray_V4(str);
        }

        public static PatternMatcher BuildSuffixArray_V5(string str)
        {
            return new SuffixArray_V5(str);
        }

        public static PatternMatcher BuildSuffixArray_V6(string str)
        {
            return new SuffixArray_V6_Merge(str);
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
                
                /*
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
                */
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
                //DummyData.DNA("TEST"),
                //DummyData.DNA("DNA_512"),
                DummyData.DNA("DNA_262144"),
                DummyData.DNA("DNA_524288"),
                DummyData.DNA("DNA_1048576"),
                //DummyData.DNA("DNA_2097152"),
                //DummyData.DNA("DNA_4194304"),
                //DummyData.DNA("DNA_8388608"),
                //DummyData.DNA("DNA_16777216"),
                //DummyData.DNA("DNA_33554432")
            };

            BuildDataStructure[] dataStructures = new BuildDataStructure[]
            {
                //BuildUkkonen,
                //BuildSuffixArray_V1,
                //BuildSuffixArray_V2,
                //BuildSuffixTree,
                BuildSuffixArray_V3,
                BuildSuffixArray_V4,
                BuildSuffixArray_V5,
                //BuildSuffixArray_V6,
                
                //BuildPrecomputed,
            };

            
            
            var table = new ConsoleTable("Data Structure & Data", "Construction Time MS", "Single Pattern Query Time MS", "Double Pattern Fixed Query Time MS", "Double Pattern Variable Query Time MS");

            string p1 = "a";
            Random random = new Random();
            int x = 1;
            string p2 = "a";
            Query query = new Query(p1, x, p2);
            query.Y = (1, 45);

            foreach (var sequence in dnaSequences)
            {
                foreach (var dataStructure in dataStructures)
                {
                    var b = BenchDataStructure(dataStructure, sequence, query);
                    table.AddRow($"{b.DataStructureName} {b.DataName}", $"{b.ConstructionTimeMilliseconds}", $"{b.SinglePatternMatchesQuery}ms, Occs: {b.SinglePatternMatchesQueryOccs}", $"{b.DoublePatternFixedMatchesQuery}ms, Occs: {b.DoublePatternFixedMatchesQueryOccs}", $"{b.DoublePatternVariableMatchesQuery}ms, Occs: {b.DoublePatternVariableMatchesQueryOccs}");
                }
            }
            table.Options.NumberAlignment = Alignment.Right;
            table.Write();

        }

        public static T[] KWayMerge<T>(T[][] arrays) where T : IComparable<T>
        {
            // Create a SortedSet to store the current minimum element from each array
            SortedSet<(T value, int arrayIndex)> minHeap = new SortedSet<(T, int)>();

            // Initialize the heap with the first element from each input array
            for (int i = 0; i < arrays.Length; i++)
            {
                if (arrays[i].Length > 0)
                {
                    minHeap.Add((arrays[i][0], i));
                }
            }

            // Calculate the total number of elements in all the input arrays
            int totalElements = arrays.Sum(a => a.Length);

            // Initialize the output array with the correct length
            T[] result = new T[totalElements];
            int index = 0;

            // Merge the arrays using the k-way merge algorithm
            while (minHeap.Count > 0)
            {
                // Extract the minimum element from the heap and add it to the output array
                (T value, int arrayIndex) = minHeap.Min;
                minHeap.Remove((value, arrayIndex));
                result[index++] = value;

                // If the array from which the minimum element was extracted is not empty, add the next element from that array to the heap
                if (arrays[arrayIndex].Length > 1)
                {
                    T[] nextElements = new T[arrays[arrayIndex].Length - 1];
                    Array.Copy(arrays[arrayIndex], 1, nextElements, 0, nextElements.Length);
                    minHeap.Add((nextElements[0], arrayIndex));
                    arrays[arrayIndex] = nextElements;
                }
                else if (arrays[arrayIndex].Length == 1)
                {
                    arrays[arrayIndex] = Array.Empty<T>(); // Mark the current array as empty so that we don't process it again
                }
            }

            return result;
        }
    }
}

