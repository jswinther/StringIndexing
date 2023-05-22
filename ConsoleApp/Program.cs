using ConsoleApp.DataStructures;
using ConsoleApp.DataStructures.Count;
using ConsoleApp.DataStructures.Existence;
using ConsoleApp.DataStructures.Reporting;
using Gma.DataStructures.StringSearch;
using System;
using System.Diagnostics;
using System.Reflection;
using static ConsoleApp.DataStructures.AlgoSuffixTreeProblem;
using static ConsoleApp.Program;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp
{
    internal class Program
    {
        public delegate ReportDataStructure BuildReportDataStructure(string str);
        public delegate ExistDataStructure BuildExistDataStructure(string str, int x, int ymin, int ymax);
        public delegate CountDataStructure BuildCountDataStructure(string str, int x, int ymin, int ymax);

        public static ReportDataStructure BuildSuffixArray_V1(string str)
        {
            return new SA_R_V1(str);
        }

        public static ReportDataStructure BuildSuffixArray_V2(string str)
        {
            return new SA_R_V2(str);
        }

        public static ReportDataStructure BuildSuffixArray_V3(string str)
        {
            return new SA_R_V3(str);
        }

        public static ReportDataStructure BuildSuffixArray_V4_1(string str)
        {
            return new SA_R_V4_1(str);
        }

        public static ReportDataStructure BuildSuffixArray_V5(string str)
        {
            return new SA_R_V5(str);
        }

        public static ReportDataStructure BuildSuffixArray_V4_2(string str)
        {
            return new SA_R_V4_2(str);
        }

        public static ReportDataStructure BuildSuffixArray_V4_3(string str)
        {
            return new SA_R_V4_3(str);
        }

        public static CountDataStructure BuildSuffixArray_V8(string str, int x, int ymin, int ymax)
        {
            return new SA_C_V2(str, x, ymin, ymax);
        }

        public static ReportDataStructure BuildSuffixTree(string str)
        {
            return SuffixTree.Create(str);
        }

        public static ReportDataStructure BuildPrecomputed(string str)
        {
            return new PreCompSubs(str);
        }

        public static ReportDataStructure BuildUkkonen(string str)
        {
            
            return new UkkonenWrapper(str);
        }

        public static ReportDataStructure BuildSuffixOther(string str)
        {
            return new SuffixTreeOther(str);
        }

        public static ReportDataStructure BuildBaratgabor(string str)
        {
            return new BaratgaborSuffixTree(str);
        }

        public static ExistDataStructure BuildSA_E_V1(string str, int x, int ymin, int ymax)
        {
            return new SA_E_V1(str, x, ymin, ymax);
        }

        public static ExistDataStructure BuildSA_E_V2(string str, int x, int ymin, int ymax)
        {
            return new SA_E_V2(str, x, ymin, ymax);
        }
        public static ExistDataStructure BuildSA_E_V3(string str, int x, int ymin, int ymax)
        {
            return new SA_E_V3(str, x, ymin, ymax);
        }
        public static ExistDataStructure BuildSA_E_V4(string str, int x, int ymin, int ymax)
        {
            return new SA_E_V4(str, x, ymin, ymax);
        }

        public static Benchmark[] BenchReportDataStructure(BuildReportDataStructure matcher, string name, string str, params Query[] queries)
        {
            Benchmark[] benchmarks = new Benchmark[queries.Length];
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            // Construction
            var patternMatcher = matcher.Invoke(str);
            stopwatch.Stop();
            for (int i = 0; i < benchmarks.Length; i++)
            {
                benchmarks[i] = new Benchmark();
                benchmarks[i].ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
                benchmarks[i].DataStructureName = patternMatcher.GetType().Name;
                benchmarks[i].DataName = name;
                benchmarks[i].QueryName = queries[i].QueryName;
            }
            
            int repetitions = 5;
            // Query Time
            for (int i = 0; i < queries.Length; i++) // For each query
            {
                // Single Pattern
                
                try
                {
                    for (int j = 0; j < repetitions; j++) // for each repetition
                    {
                        stopwatch = Stopwatch.StartNew();
                        var occs = patternMatcher.Matches(queries[i].P1);
                        stopwatch.Stop();
                        benchmarks[i].SinglePatternMatchesQueryOccs = occs.Count();
                        benchmarks[i].SinglePatternMatchesQuery += stopwatch.ElapsedMilliseconds;
                    }
                    benchmarks[i].SinglePatternMatchesQuery /= repetitions;
                }
                catch (Exception)
                {
                    benchmarks[i].SinglePatternMatchesQuery = -1;
                }
                
                
                // Double Pattern + Fixed Gap
                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.Matches(queries[i].P1, queries[i].X, queries[i].P2);
                    stopwatch.Stop();
                    benchmarks[i].DoublePatternFixedMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmarks[i].DoublePatternFixedMatchesQueryOccs = occs.Count();
                }
                catch (NotImplementedException)
                {
                    benchmarks[i].DoublePatternFixedMatchesQuery = -1;
                }
                
                // Double Pattern + Variable Gap
                
                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.Matches(queries[i].P1, queries[i].Y.Min, queries[i].Y.Max, queries[i].P2);
                    stopwatch.Stop();
                    benchmarks[i].DoublePatternVariableMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmarks[i].DoublePatternVariableMatchesQueryOccs = occs.Count();
                }
                catch (NotImplementedException)
                {
                    benchmarks[i].DoublePatternVariableMatchesQuery = -1;
                }
                
            }
            stopwatch.Stop();
            //benchmark.QueryTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            
            
            return benchmarks;
        }

        public static Benchmark BenchCountDataStructure(BuildCountDataStructure matcher, string name, string str, params Query[] queries)
        {
            Benchmark benchmark = new Benchmark();
            Stopwatch stopwatch = Stopwatch.StartNew();
            var q = queries[0];
            // Construction
            var patternMatcher = matcher.Invoke(str, q.X, q.Y.Min, q.Y.Max);
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
                    benchmark.SinglePatternMatchesQueryOccs = occs;

                }
                catch (Exception)
                {
                    benchmark.SinglePatternMatchesQuery = -1;
                }

                /*
                // Double Pattern + Fixed Gap
                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.MatchesFixed(query.P1, query.P2);
                    stopwatch.Stop();
                    benchmark.DoublePatternFixedMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmark.DoublePatternFixedMatchesQueryOccs = occs;
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
                    var occs = patternMatcher.MatchesVariable(query.P1, query.P2);
                    stopwatch.Stop();
                    benchmark.DoublePatternVariableMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmark.DoublePatternVariableMatchesQueryOccs = occs;
                }
                catch (Exception)
                {
                    benchmark.DoublePatternVariableMatchesQuery = -1;
                }

            }
            stopwatch.Stop();
            benchmark.QueryTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            benchmark.DataStructureName = patternMatcher.GetType().Name;
            benchmark.DataName = name;

            return benchmark;
        }

        public static Benchmark BenchExistDataStructure(BuildExistDataStructure matcher, string name, string str, int x, int ymin, int ymax, params (string p1, string p2)[] queries)
        {
            Benchmark benchmark = new Benchmark();
            Stopwatch stopwatch = Stopwatch.StartNew();
            // Construction
            var patternMatcher = matcher.Invoke(str, x, ymin, ymax);
            stopwatch.Stop();
            benchmark.ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            // Query Time
            foreach (var query in queries)
            {
                // Single Pattern

                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.Matches(query.p1);
                    stopwatch.Stop();
                    benchmark.SinglePatternMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmark.SinglePatternMatchesQueryOccs = occs;

                }
                catch (Exception)
                {
                    benchmark.SinglePatternMatchesQuery = -1;
                }

                
                // Double Pattern + Fixed Gap
                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.MatchesFixedGap(query.p1, query.p2);
                    stopwatch.Stop();
                    benchmark.DoublePatternFixedMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmark.DoublePatternFixedMatchesQueryOccs = occs;
                }
                catch (Exception)
                {
                    benchmark.DoublePatternFixedMatchesQuery = -1;
                }
                
                // Double Pattern + Variable Gap

                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.MatchesVariableGap(query.p1, query.p2);
                    stopwatch.Stop();
                    benchmark.DoublePatternVariableMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmark.DoublePatternVariableMatchesQueryOccs = occs;
                }
                catch (Exception)
                {
                    benchmark.DoublePatternVariableMatchesQuery = -1;
                }

            }
            stopwatch.Stop();
            benchmark.QueryTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            benchmark.DataStructureName = patternMatcher.GetType().Name;
            benchmark.DataName = name;

            return benchmark;
        }

        internal class Benchmark
        {
            public string DataStructureName { get; set; }
            public string DataName { get; set; }

            public string QueryName { get; set; }
            public long ConstructionTimeMilliseconds { get; set; }
            public long QueryTimeMilliseconds { get; set; }
            public long SinglePatternMatchesQuery { get; internal set; }
            public long DoublePatternFixedMatchesQuery { get; internal set; }
            public long DoublePatternVariableMatchesQuery { get; internal set; }
            public object SinglePatternMatchesQueryOccs { get; internal set; }
            public object DoublePatternFixedMatchesQueryOccs { get; internal set; }
            public object DoublePatternVariableMatchesQueryOccs { get; internal set; }
        }

        static void Main(string[] args)
        {

            (string, string)[] testData = new (string, string)[]
            {
                //DummyData.Dummy,
                //DummyData.DNA("TEST"),
                //DummyData.DNA("DNA_512"),
                //DummyData.DNA("DNA_8192"),
                //DummyData.DNA("DNA_16384"),
                //DummyData.DNA("DNA_262144"),
                //DummyData.DNA("DNA_524288"),
                //DummyData.DNA("DNA_1048576"),
                DummyData.DNA("DNA_2097152"),
                //DummyData.DNA("DNA_4194304"),
                //DummyData.DNA("DNA_1048576"),
                //DummyData.PCC("realDNA_1048576"),
                //DummyData.PCC("proteins_1048576"),
                //DummyData.ENG("english_1048576"),
                //DummyData.PCC("english_8388608"),
                //DummyData.DNA("DNA_16777216"),
                //DummyData.DNA("DNA_33554432")
            };

            BuildReportDataStructure[] reportingDataStructures = new BuildReportDataStructure[]
            {
                //BuildSuffixArray_V1,
                BuildSuffixArray_V2,
                //BuildSuffixArray_V3,
                //BuildSuffixArray_V4_1,
                //BuildSuffixArray_V5,
                //BuildSuffixArray_V4_2,
                //BuildSuffixArray_V4_3,
                
            };

            BuildCountDataStructure[] countingDataStructures = new BuildCountDataStructure[]
            {
                BuildSuffixArray_V8  // ALTID BAD, IKKE KØR PÅ ANDET END 512

            };

            BuildExistDataStructure[] existenceDataStructures = new BuildExistDataStructure[]
            {
                //BuildSA_E_V1,   // ALTID BAD, IKKE KØR PÅ ANDET END 512
                //BuildSA_E_V2,
                //BuildSA_E_V3,
                //BuildSA_E_V4
            };


            var table = new ConsoleTable("Data Structure & Data", "Construction Time MS", "Pattern Type", "Single Pattern Query Time MS", "Double Pattern Fixed Query Time MS", "Double Pattern Variable Query Time MS");

            string p1 = "a";
            Random random = new Random();
            int x = 1;
            string p2 = "a";
            Query query = new Query(p1, x, p2);
            query.Y = (1, 45);

            

            foreach (var sequence in testData)
            {
                SuffixArray_Scanner suffixArray_Scanner = new SuffixArray_Scanner(sequence);
                if (suffixArray_Scanner.botPattern.EndsWith('|'))
                {
                    //suffixArray_Scanner.botPattern = suffixArray_Scanner.botPattern.Remove(suffixArray_Scanner.botPattern.Length -1);
                }
                Query query1 = new Query(suffixArray_Scanner.topPattern, x, p2, "Top");
                Random random1 = new Random();
                Query query2 = new Query(suffixArray_Scanner.midPatterns.GetRandom(), x, p2, "Mid");
                Query query3 = new Query(suffixArray_Scanner.botPattern, x, p2, "Bot");
                Query[] queries = new Query[3];
                query1.Y = query.Y; query2.Y = query.Y; query3.Y = query.Y;
                queries[0] = query1;
                queries[1] = query2;
                queries[2] = query3;
                //Console.WriteLine(query3.P1);


                foreach (var dataStructure in reportingDataStructures)
                {
                    var b = BenchReportDataStructure(dataStructure, sequence.Item1, sequence.Item2, queries);
                    foreach (var bench in b)
                    {
                        table.AddRow($"{bench.DataStructureName} {bench.DataName}", $"{bench.ConstructionTimeMilliseconds}", $"{bench.QueryName}", $"{bench.SinglePatternMatchesQuery}ms, Occs: {bench.SinglePatternMatchesQueryOccs}", $"{bench.DoublePatternFixedMatchesQuery}ms, Occs: {bench.DoublePatternFixedMatchesQueryOccs}", $"{bench.DoublePatternVariableMatchesQuery}ms, Occs: {bench.DoublePatternVariableMatchesQueryOccs}");
                    }
                    
                    //b = BenchReportDataStructure(dataStructure, sequence.Item1, sequence.Item2, query2);
                    //table.AddRow($"{b.DataStructureName} {b.DataName}", $"{b.ConstructionTimeMilliseconds}", $"Middle", $"{b.SinglePatternMatchesQuery}ms, Occs: {b.SinglePatternMatchesQueryOccs}", $"{b.DoublePatternFixedMatchesQuery}ms, Occs: {b.DoublePatternFixedMatchesQueryOccs}", $"{b.DoublePatternVariableMatchesQuery}ms, Occs: {b.DoublePatternVariableMatchesQueryOccs}");
                    //b = BenchReportDataStructure(dataStructure, sequence.Item1, sequence.Item2, query3);
                    //table.AddRow($"{b.DataStructureName} {b.DataName}", $"{b.ConstructionTimeMilliseconds}", $"Bot", $"{b.SinglePatternMatchesQuery}ms, Occs: {b.SinglePatternMatchesQueryOccs}", $"{b.DoublePatternFixedMatchesQuery}ms, Occs: {b.DoublePatternFixedMatchesQueryOccs}", $"{b.DoublePatternVariableMatchesQuery}ms, Occs: {b.DoublePatternVariableMatchesQueryOccs}");
                }

                foreach (var dataStructure in countingDataStructures)
                {
                    var b = BenchCountDataStructure(dataStructure, sequence.Item1, sequence.Item2, query1);
                    table.AddRow($"{b.DataStructureName} {b.DataName}", $"{b.ConstructionTimeMilliseconds}", $"Top", $"{b.SinglePatternMatchesQuery}ms, Occs: {b.SinglePatternMatchesQueryOccs}", $"{b.DoublePatternFixedMatchesQuery}ms, Occs: {b.DoublePatternFixedMatchesQueryOccs}", $"{b.DoublePatternVariableMatchesQuery}ms, Occs: {b.DoublePatternVariableMatchesQueryOccs}");
                    b = BenchCountDataStructure(dataStructure, sequence.Item1, sequence.Item2, query2);
                    table.AddRow($"{b.DataStructureName} {b.DataName}", $"{b.ConstructionTimeMilliseconds}", $"Middle", $"{b.SinglePatternMatchesQuery}ms, Occs: {b.SinglePatternMatchesQueryOccs}", $"{b.DoublePatternFixedMatchesQuery}ms, Occs: {b.DoublePatternFixedMatchesQueryOccs}", $"{b.DoublePatternVariableMatchesQuery}ms, Occs: {b.DoublePatternVariableMatchesQueryOccs}");
                    b = BenchCountDataStructure(dataStructure, sequence.Item1, sequence.Item2, query3);
                    table.AddRow($"{b.DataStructureName} {b.DataName}", $"{b.ConstructionTimeMilliseconds}", $"Bot", $"{b.SinglePatternMatchesQuery}ms, Occs: {b.SinglePatternMatchesQueryOccs}", $"{b.DoublePatternFixedMatchesQuery}ms, Occs: {b.DoublePatternFixedMatchesQueryOccs}", $"{b.DoublePatternVariableMatchesQuery}ms, Occs: {b.DoublePatternVariableMatchesQueryOccs}");
                }

                foreach (var dataStructure in existenceDataStructures)
                {
                    var b = BenchExistDataStructure(dataStructure, sequence.Item1, sequence.Item2, x, query.Y.Min, query.Y.Max, (suffixArray_Scanner.topPattern, p2));
                    table.AddRow($"{b.DataStructureName} {b.DataName}", $"{b.ConstructionTimeMilliseconds}", $"Top", $"{b.SinglePatternMatchesQuery}ms, Occs: {b.SinglePatternMatchesQueryOccs}", $"{b.DoublePatternFixedMatchesQuery}ms, Occs: {b.DoublePatternFixedMatchesQueryOccs}", $"{b.DoublePatternVariableMatchesQuery}ms, Occs: {b.DoublePatternVariableMatchesQueryOccs}");
                    b = BenchExistDataStructure(dataStructure, sequence.Item1, sequence.Item2, x, query.Y.Min, query.Y.Max, (suffixArray_Scanner.midPatterns.GetRandom(), p2));
                    table.AddRow($"{b.DataStructureName} {b.DataName}", $"{b.ConstructionTimeMilliseconds}", $"Middle", $"{b.SinglePatternMatchesQuery}ms, Occs: {b.SinglePatternMatchesQueryOccs}", $"{b.DoublePatternFixedMatchesQuery}ms, Occs: {b.DoublePatternFixedMatchesQueryOccs}", $"{b.DoublePatternVariableMatchesQuery}ms, Occs: {b.DoublePatternVariableMatchesQueryOccs}");
                    b = BenchExistDataStructure(dataStructure, sequence.Item1, sequence.Item2, x, query.Y.Min, query.Y.Max, (suffixArray_Scanner.botPattern, p2));
                    table.AddRow($"{b.DataStructureName} {b.DataName}", $"{b.ConstructionTimeMilliseconds}", $"Bot", $"{b.SinglePatternMatchesQuery}ms, Occs: {b.SinglePatternMatchesQueryOccs}", $"{b.DoublePatternFixedMatchesQuery}ms, Occs: {b.DoublePatternFixedMatchesQueryOccs}", $"{b.DoublePatternVariableMatchesQuery}ms, Occs: {b.DoublePatternVariableMatchesQueryOccs}");
                }
            }
            table.Options.NumberAlignment = Alignment.Right;
            table.Write();
            var workdir = Assembly.GetAssembly(typeof(Program)).Location; ;
            var dir = Directory.GetParent(workdir).Parent.Parent.Parent.FullName;
            var date = DateTime.Now.ToString("yyyyMMddTHHmmss");
            File.WriteAllText($"{dir}\\consoleoutput{date}.txt", table.ToString());
        }



            

        

        public static T[] KWayMerge<T>(T[][] arrays) where T : IComparable<T>
        {
            // Create a SortedSet to store the current minimum element from each array
            PriorityQueue<int, T> minHeap = new PriorityQueue<int, T>();
            int[] counters = new int[arrays.Length];
            // Initialize the heap with the first element from each input array
            for (int i = 0; i < arrays.Length; i++)
            {
                if (arrays[i].Length > 0)
                {
                    minHeap.Enqueue(i, arrays[i][counters[i]++]);
                }
            }

            // Calculate the total number of elements in all the input arrays
            int totalElements = arrays.Sum(a => a.Length);

            // Initialize the output array with the correct length
            T[] result = new T[totalElements];
            int index = 0;

            // Merge the arrays using the k-way merge algorithm
            while (minHeap.TryDequeue(out int arrayIndex, out T value) && index < totalElements)
            {
                result[index++] = value;
                if (arrays[arrayIndex].Length > counters[arrayIndex])
                {
                    minHeap.Enqueue(arrayIndex, arrays[arrayIndex][counters[arrayIndex]++]);
                }
            }
            return result;
        }
    }
}

