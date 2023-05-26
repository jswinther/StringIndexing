using ConsoleApp.DataStructures;
using ConsoleApp.DataStructures.Count;
using ConsoleApp.DataStructures.Existence;
using ConsoleApp.DataStructures.Reporting;
using Gma.DataStructures.StringSearch;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
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
        public static readonly int TIMEOUT = 300;

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
        public static CountDataStructure BuildSA_C_V1(string str, int x, int ymin, int ymax)
        {
            return new SA_C_V1(str, x, ymin, ymax);
        }
        public static CountDataStructure BuildSA_C_V2(string str, int x, int ymin, int ymax)
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

        public static Benchmark[] BenchReportDataStructure(string dsname, BuildReportDataStructure matcher, string name, string str, params Query[] queries)       
        {
            Benchmark[] benchmarks = new Benchmark[queries.Length];
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            // Construction
            ReportDataStructure patternMatcher = null;
            var t = Task.Run(() =>
            {
                patternMatcher = matcher.Invoke(str);
            });
            if (!t.Wait(TimeSpan.FromSeconds(TIMEOUT)))
            {
                stopwatch.Stop();
                for (int i = 0; i < benchmarks.Length; i++)
                {
                    benchmarks[i] = new Benchmark();
                    benchmarks[i].ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
                    benchmarks[i].DataStructureName = dsname;
                    benchmarks[i].DataName = name;
                    benchmarks[i].QueryName = queries[i].QueryName;
                    benchmarks[i].SinglePatternMatchesQuery = -1;
                    benchmarks[i].DoublePatternFixedMatchesQuery = -1;
                    benchmarks[i].DoublePatternVariableMatchesQuery = -1;
                }
                return benchmarks;
            };
            stopwatch.Stop();
            for (int i = 0; i < benchmarks.Length; i++)
            {
                benchmarks[i] = new Benchmark();
                benchmarks[i].ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
                benchmarks[i].DataStructureName = dsname;
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
                    var task = Task.Run(() =>
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
                    });
                    
                    if (!task.Wait(TimeSpan.FromSeconds(TIMEOUT)))
                        throw new TimeoutException(benchmarks[i].DataStructureName + " " + benchmarks[i].DataName + " timed out");
                }
                catch (Exception e)
                {
                    File.AppendAllText("exceptions", e.StackTrace);
                    benchmarks[i].SinglePatternMatchesQuery = -1;
                }
                
                
                // Double Pattern + Fixed Gap
                try
                {
                    var task = Task.Run(() =>
                    {
                        stopwatch = Stopwatch.StartNew();
                        var occs = patternMatcher.Matches(queries[i].P1, queries[i].X, queries[i].P2);
                        stopwatch.Stop();
                        benchmarks[i].DoublePatternFixedMatchesQuery = stopwatch.ElapsedMilliseconds;
                        benchmarks[i].DoublePatternFixedMatchesQueryOccs = occs.Count();
                    });
                    
                    if (!task.Wait(TimeSpan.FromSeconds(TIMEOUT)))
                        throw new TimeoutException(benchmarks[i].DataStructureName + " " + benchmarks[i].DataName + " timed out");
                }
                catch (Exception e)
                {
                    File.AppendAllText("exceptions", e.StackTrace);
                    benchmarks[i].DoublePatternFixedMatchesQuery = -1;
                }
                
                // Double Pattern + Variable Gap
                
                try
                {
                    var task = Task.Run(() =>
                    {
                        stopwatch = Stopwatch.StartNew();
                        var occs = patternMatcher.Matches(queries[i].P1, queries[i].Y.Min, queries[i].Y.Max, queries[i].P2);
                        stopwatch.Stop();
                        benchmarks[i].DoublePatternVariableMatchesQuery = stopwatch.ElapsedMilliseconds;
                        benchmarks[i].DoublePatternVariableMatchesQueryOccs = occs.Count();
                    });

                    if (!task.Wait(TimeSpan.FromSeconds(TIMEOUT)))
                        throw new TimeoutException(benchmarks[i].DataStructureName + " " + benchmarks[i].DataName + " timed out");
                }
                catch (Exception e)
                {
                    File.AppendAllText("exceptions", e.StackTrace);
                    benchmarks[i].DoublePatternVariableMatchesQuery = -1;
                }
                
            }
            stopwatch.Stop();
            //benchmark.QueryTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            
            
            return benchmarks;
        }

        public static Benchmark[] BenchCountDataStructure(string dsname, BuildCountDataStructure matcher, string name, string str, int x, int ymin, int ymax, params Query[] queries)
        {
            Benchmark[] benchmarks = new Benchmark[queries.Length];

            Stopwatch stopwatch = Stopwatch.StartNew();
            // Construction
            CountDataStructure patternMatcher = null;
            var t = Task.Run(() =>
            {
                patternMatcher = matcher.Invoke(str, x, ymin, ymax);
            });
            if (!t.Wait(TimeSpan.FromSeconds(TIMEOUT)))
            {
                stopwatch.Stop();
                for (int i = 0; i < benchmarks.Length; i++)
                {
                    benchmarks[i] = new Benchmark();
                    benchmarks[i].ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
                    benchmarks[i].DataStructureName = dsname;
                    benchmarks[i].DataName = name;
                    benchmarks[i].QueryName = queries[i].QueryName;
                    benchmarks[i].SinglePatternMatchesQuery = -1;
                    benchmarks[i].DoublePatternFixedMatchesQuery = -1;
                    benchmarks[i].DoublePatternVariableMatchesQuery = -1;
                }
                return benchmarks;
            };
            stopwatch.Stop();
            for (int i = 0; i < benchmarks.Length; i++)
            {
                benchmarks[i] = new Benchmark();
                benchmarks[i].ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
                benchmarks[i].DataStructureName = dsname;
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
                    var task = Task.Run(() =>
                    {
                        for (int j = 0; j < repetitions; j++) // for each repetition
                        {
                            stopwatch = Stopwatch.StartNew();
                            var occs = patternMatcher.Matches(queries[i].P1);
                            stopwatch.Stop();
                            benchmarks[i].SinglePatternMatchesQueryOccs = occs;
                            benchmarks[i].SinglePatternMatchesQuery += stopwatch.ElapsedMilliseconds;
                        }
                        benchmarks[i].SinglePatternMatchesQuery /= repetitions;
                    });
                    

                    if (!task.Wait(TimeSpan.FromSeconds(TIMEOUT)))
                        throw new TimeoutException(benchmarks[i].DataStructureName + " " + benchmarks[i].DataName + " timed out");
                }
                catch (Exception e)
                {
                    File.AppendAllText("exceptions", e.StackTrace);
                    benchmarks[i].SinglePatternMatchesQuery = -1;
                }


                // Double Pattern + Fixed Gap
                try
                {
                    var task = Task.Run(() =>
                    {
                        stopwatch = Stopwatch.StartNew();
                        var occs = patternMatcher.MatchesFixed(queries[i].P1, queries[i].P2);
                        stopwatch.Stop();
                        benchmarks[i].DoublePatternFixedMatchesQuery = stopwatch.ElapsedMilliseconds;
                        benchmarks[i].DoublePatternFixedMatchesQueryOccs = occs;
                    });
                    if (!task.Wait(TimeSpan.FromSeconds(TIMEOUT)))
                        throw new TimeoutException(benchmarks[i].DataStructureName + " " + benchmarks[i].DataName + " timed out");
                }
                catch (Exception e)
                {
                    File.AppendAllText("exceptions", e.StackTrace);
                    benchmarks[i].DoublePatternFixedMatchesQuery = -1;
                }

                // Double Pattern + Variable Gap

                try
                {
                    var task = Task.Run(() =>
                    {
                        stopwatch = Stopwatch.StartNew();
                        var occs = patternMatcher.MatchesVariable(queries[i].P1, queries[i].P2);
                        stopwatch.Stop();
                        benchmarks[i].DoublePatternVariableMatchesQuery = stopwatch.ElapsedMilliseconds;
                        benchmarks[i].DoublePatternVariableMatchesQueryOccs = occs;
                    });
                    
                    if (!task.Wait(TimeSpan.FromSeconds(TIMEOUT)))
                        throw new TimeoutException(benchmarks[i].DataStructureName + " " + benchmarks[i].DataName + " timed out");
                }
                catch (Exception e)
                {
                    File.AppendAllText("exceptions", e.StackTrace);
                    benchmarks[i].DoublePatternVariableMatchesQuery = -1;
                }

            }
            stopwatch.Stop();
            //benchmark.QueryTimeMilliseconds = stopwatch.ElapsedMilliseconds;


            return benchmarks;
        }

        public static Benchmark[] BenchExistDataStructure(string dsname, BuildExistDataStructure matcher, string name, string str, int x, int ymin, int ymax, params Query[] queries)
        {
            Benchmark[] benchmarks = new Benchmark[queries.Length];

            Stopwatch stopwatch = Stopwatch.StartNew();
            // Construction
            ExistDataStructure patternMatcher = null;
            var t = Task.Run(() =>
            {
                patternMatcher = matcher.Invoke(str, x, ymin, ymax);
            });
            if (!t.Wait(TimeSpan.FromSeconds(TIMEOUT)))
            {
                stopwatch.Stop();
                for (int i = 0; i < benchmarks.Length; i++)
                {
                    benchmarks[i] = new Benchmark();
                    benchmarks[i].ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
                    benchmarks[i].DataStructureName = dsname;
                    benchmarks[i].DataName = name;
                    benchmarks[i].QueryName = queries[i].QueryName;
                    benchmarks[i].SinglePatternMatchesQuery = -1;
                    benchmarks[i].DoublePatternFixedMatchesQuery = -1;
                    benchmarks[i].DoublePatternVariableMatchesQuery = -1;
                }
                return benchmarks;
            };
            stopwatch.Stop();
            for (int i = 0; i < benchmarks.Length; i++)
            {
                benchmarks[i] = new Benchmark();
                benchmarks[i].ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
                benchmarks[i].DataStructureName = dsname;
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
                    var task = Task.Run(() =>
                    {
                        for (int j = 0; j < repetitions; j++) // for each repetition
                        {
                            stopwatch = Stopwatch.StartNew();
                            var occs = patternMatcher.Matches(queries[i].P1);
                            stopwatch.Stop();
                            benchmarks[i].SinglePatternMatchesQueryOccs = occs;
                            benchmarks[i].SinglePatternMatchesQuery += stopwatch.ElapsedMilliseconds;
                        }
                        benchmarks[i].SinglePatternMatchesQuery /= repetitions;
                    });
                    if (!task.Wait(TimeSpan.FromSeconds(TIMEOUT)))
                        throw new TimeoutException(benchmarks[i].DataStructureName + " " + benchmarks[i].DataName + " timed out");
                }
                catch (Exception e)
                {
                    File.AppendAllText("exceptions", e.StackTrace);
                    benchmarks[i].SinglePatternMatchesQuery = -1;
                }


                // Double Pattern + Fixed Gap
                try
                {
                    var task = Task.Run(() =>
                    {
                        stopwatch = Stopwatch.StartNew();
                        var occs = patternMatcher.MatchesFixedGap(queries[i].P1, queries[i].P2);
                        stopwatch.Stop();
                        benchmarks[i].DoublePatternFixedMatchesQuery = stopwatch.ElapsedMilliseconds;
                        benchmarks[i].DoublePatternFixedMatchesQueryOccs = occs;
                    });
                    if (!task.Wait(TimeSpan.FromSeconds(TIMEOUT)))
                        throw new TimeoutException(benchmarks[i].DataStructureName + " " + benchmarks[i].DataName + " timed out");

                }
                catch (Exception e)
                {
                    File.AppendAllText("exceptions", e.StackTrace);
                    benchmarks[i].DoublePatternFixedMatchesQuery = -1;
                }

                // Double Pattern + Variable Gap

                try
                {
                    var task = Task.Run(() =>
                    {
                        stopwatch = Stopwatch.StartNew();
                        var occs = patternMatcher.MatchesVariableGap(queries[i].P1, queries[i].P2);
                        stopwatch.Stop();
                        benchmarks[i].DoublePatternVariableMatchesQuery = stopwatch.ElapsedMilliseconds;
                        benchmarks[i].DoublePatternVariableMatchesQueryOccs = occs;
                    });
                    if (!task.Wait(TimeSpan.FromSeconds(TIMEOUT)))
                        throw new TimeoutException(benchmarks[i].DataStructureName + " " + benchmarks[i].DataName + " timed out");
                }
                catch (Exception e)
                {
                    File.AppendAllText("exceptions", e.StackTrace);
                    benchmarks[i].DoublePatternVariableMatchesQuery = -1;
                }

            }
            stopwatch.Stop();
            //benchmark.QueryTimeMilliseconds = stopwatch.ElapsedMilliseconds;


            return benchmarks;
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




            var testData = DummyData.GetData(new DS[]
            {
                DS._512,
                DS._8192,
                DS._16384,
                DS._262144,
                DS._524288,
                DS._1048576,
                DS._2097152,
                DS._4194304,
                DS._8388608,
                DS._16777216,
                DS._33554432,
            });
                
                
               
          

            var reportingDataStructures = new(string, BuildReportDataStructure)[]
            {
                ("SA_R_V1", BuildSuffixArray_V1),
                ("SA_R_V2", BuildSuffixArray_V2),
                ("SA_R_V3", BuildSuffixArray_V3),
                ("SA_R_V4_1", BuildSuffixArray_V4_1),
                ("SA_R_V4_2", BuildSuffixArray_V4_2),
                ("SA_R_V4_3", BuildSuffixArray_V4_3),
            };

            var countingDataStructures = new(string, BuildCountDataStructure)[]
            {
                //  // ALTID BAD, IKKE KØR PÅ ANDET END 512
                ("SA_C_V1", BuildSA_C_V1),
                ("SA_C_V2", BuildSA_C_V2)
            };

            var existenceDataStructures = new(string, BuildExistDataStructure)[]
            {
                ("SA_E_V1", BuildSA_E_V1),
                ("SA_E_V2", BuildSA_E_V2),
                ("SA_E_V3", BuildSA_E_V3),
                ("SA_E_V4", BuildSA_E_V4)
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
                query.Y =  (1, (int)Math.Sqrt(sequence.Item2.Length));
                //Console.WriteLine(query3.P1);


                foreach ((var name, var dataStructure) in reportingDataStructures)
                {
                    var b = BenchReportDataStructure(name, dataStructure, sequence.Item1, sequence.Item2, queries);
                    foreach (var bench in b)
                    {
                        table.AddRow($"{bench.DataStructureName} {bench.DataName}", $"{bench.ConstructionTimeMilliseconds}", $"{bench.QueryName}", $"{bench.SinglePatternMatchesQuery}ms, Occs: {bench.SinglePatternMatchesQueryOccs}", $"{bench.DoublePatternFixedMatchesQuery}ms, Occs: {bench.DoublePatternFixedMatchesQueryOccs}", $"{bench.DoublePatternVariableMatchesQuery}ms, Occs: {bench.DoublePatternVariableMatchesQueryOccs}");
                    }
                }

                foreach ((var name, var dataStructure) in countingDataStructures)
                {
                    var b = BenchCountDataStructure(name, dataStructure, sequence.Item1, sequence.Item2, query.X, query.Y.Min, query.Y.Max, queries);
                    foreach (var bench in b)
                    {
                        table.AddRow($"{bench.DataStructureName} {bench.DataName}", $"{bench.ConstructionTimeMilliseconds}", $"{bench.QueryName}", $"{bench.SinglePatternMatchesQuery}ms, Occs: {bench.SinglePatternMatchesQueryOccs}", $"{bench.DoublePatternFixedMatchesQuery}ms, Occs: {bench.DoublePatternFixedMatchesQueryOccs}", $"{bench.DoublePatternVariableMatchesQuery}ms, Occs: {bench.DoublePatternVariableMatchesQueryOccs}");
                    }
                }

                foreach ((var name, var dataStructure) in existenceDataStructures)
                {
                    var b = BenchExistDataStructure(name, dataStructure, sequence.Item1, sequence.Item2, query.X, query.Y.Min, query.Y.Max, queries);
                    foreach (var bench in b)
                    {
                        table.AddRow($"{bench.DataStructureName} {bench.DataName}", $"{bench.ConstructionTimeMilliseconds}", $"{bench.QueryName}", $"{bench.SinglePatternMatchesQuery}ms, Occs: {bench.SinglePatternMatchesQueryOccs}", $"{bench.DoublePatternFixedMatchesQuery}ms, Occs: {bench.DoublePatternFixedMatchesQueryOccs}", $"{bench.DoublePatternVariableMatchesQuery}ms, Occs: {bench.DoublePatternVariableMatchesQueryOccs}");
                    }
                }
            }
            table.Options.NumberAlignment = Alignment.Right;
            table.Write();
            var workdir = Assembly.GetAssembly(typeof(Program)).Location; ;
            var dir = Directory.GetParent(workdir).Parent.Parent.Parent.FullName;
            var date = DateTime.Now.ToString("yyyyMMddTHHmmss");
            File.WriteAllText($"{dir}\\TablePrints\\consoleoutput{date}.txt", table.ToString());
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

