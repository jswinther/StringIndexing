using ConsoleApp.Data.Obsolete;
using ConsoleApp.DataStructures;
using ConsoleApp.DataStructures.Count;
using ConsoleApp.DataStructures.Existence;
using ConsoleApp.DataStructures.Reporting;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using static ConsoleApp.Data.Obsolete.AlgoSuffixTreeProblem;
using static ConsoleApp.Program;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<(string, GetData)> testData;
            (string, BuildReportDataStructure)[] reportingDataStructures;
            (string, BuildCountDataStructure)[] countingDataStructures;
            (string, BuildExistDataStructure)[] existenceDataStructures;
            string sln;
            string resultsDir;
            SetupDirectories(out sln, out resultsDir);
            GetTestSetup(sln, resultsDir, out testData, out reportingDataStructures, out countingDataStructures, out existenceDataStructures);
            InitConsoleTable();
            


            string p1 = "a";
            Random random = new Random();
            int x = 1;
            string p2 = "a";
            Query query = new Query(p1, x, p2);

            foreach (var test in testData)
            {
                var textName = test.Item1;
                var suffixA = JsonSerializer.Deserialize<SuffixArrayFinal>(File.ReadAllText($"{Helper.TryGetSolutionDirectoryInfo()}\\{textName}.json"));
                var sequence = test.Item2.Invoke(textName);
                query.Y = (1, (int)Math.Sqrt(sequence.Length));
                for (int i = 0; i < 10; i++)
                {
                    SuffixArray_Scanner suffixArray_Scanner = new SuffixArray_Scanner((textName, sequence), suffixA);
                    Query query1 = new Query(suffixArray_Scanner.topPattern, x, p2, "Top");
                    Random random1 = new Random();
                    Query query2 = new Query(suffixArray_Scanner.midPatterns.GetRandom(), x, p2, "Mid");
                    Query query3 = new Query(suffixArray_Scanner.botPattern, x, p2, "Bot");
                    Query[] queries = new Query[3];
                    query1.Y = query.Y; query2.Y = query.Y; query3.Y = query.Y;
                    queries[0] = query1;
                    queries[1] = query2;
                    queries[2] = query3;
                    suffixArray_Scanner = null;

                    foreach ((var name, var dataStructure) in reportingDataStructures)
                    {
                        Console.WriteLine($"Running + {name + '_' + i} on {textName}");
                        var b = BenchReportDataStructure(name + '_' + i, dataStructure, textName, suffixA, queries);
                        foreach (var bench in b)
                        {
                            table.AddRow($"{bench.DataStructureName} {bench.DataName}", $"{bench.ConstructionTimeMilliseconds}", $"{bench.QueryName}", $"{bench.SinglePatternMatchesQuery}ms, Occs: {bench.SinglePatternMatchesQueryOccs}", $"{bench.DoublePatternFixedMatchesQuery}ms, Occs: {bench.DoublePatternFixedMatchesQueryOccs}", $"{bench.DoublePatternVariableMatchesQuery}ms, Occs: {bench.DoublePatternVariableMatchesQueryOccs}");
                            Console.WriteLine($"{bench.DataStructureName}, {bench.ConstructionTimeMilliseconds}, {bench.QueryName}, {bench.SinglePatternMatchesQuery}, {bench.DoublePatternFixedMatchesQuery}, {bench.DoublePatternVariableMatchesQuery}");
                        }
                        var qs = b.Select(s => (s.SinglePatternMatchesQuery, s.DoublePatternFixedMatchesQuery, s.DoublePatternVariableMatchesQuery)).ToArray();
                        var bb = b[0];
                        File.AppendAllText($"{resultsDir}\\{name + '_' + i}.csv", $"{textName},{bb.ConstructionTimeMilliseconds},{qs[0].SinglePatternMatchesQuery},{qs[0].DoublePatternFixedMatchesQuery},{qs[0].DoublePatternVariableMatchesQuery},{qs[1].SinglePatternMatchesQuery},{qs[1].DoublePatternFixedMatchesQuery},{qs[1].DoublePatternVariableMatchesQuery},{qs[2].SinglePatternMatchesQuery},{qs[2].DoublePatternFixedMatchesQuery},{qs[2].DoublePatternVariableMatchesQuery}\n");
                    }

                    foreach ((var name, var dataStructure) in countingDataStructures)
                    {
                        Console.WriteLine($"Running + {name + '_' + i} on {textName}");
                        var b = BenchCountDataStructure(name, dataStructure, textName, suffixA, query.X, query.Y.Min, query.Y.Max, queries);
                        foreach (var bench in b)
                        {
                            table.AddRow($"{bench.DataStructureName} {bench.DataName}", $"{bench.ConstructionTimeMilliseconds}", $"{bench.QueryName}", $"{bench.SinglePatternMatchesQuery}ms, Occs: {bench.SinglePatternMatchesQueryOccs}", $"{bench.DoublePatternFixedMatchesQuery}ms, Occs: {bench.DoublePatternFixedMatchesQueryOccs}", $"{bench.DoublePatternVariableMatchesQuery}ms, Occs: {bench.DoublePatternVariableMatchesQueryOccs}");
                            Console.WriteLine($"{bench.DataStructureName}, {bench.ConstructionTimeMilliseconds}, {bench.QueryName}, {bench.SinglePatternMatchesQuery}, {bench.DoublePatternFixedMatchesQuery}, {bench.DoublePatternVariableMatchesQuery}");
                        }
                        var qs = b.Select(s => (s.SinglePatternMatchesQuery, s.DoublePatternFixedMatchesQuery, s.DoublePatternVariableMatchesQuery)).ToArray();
                        var bb = b[0];
                        File.AppendAllText($"{resultsDir}\\{name + '_' + i}.csv", $"{textName},{bb.ConstructionTimeMilliseconds},{qs[0].SinglePatternMatchesQuery},{qs[0].DoublePatternFixedMatchesQuery},{qs[0].DoublePatternVariableMatchesQuery},{qs[1].SinglePatternMatchesQuery},{qs[1].DoublePatternFixedMatchesQuery},{qs[1].DoublePatternVariableMatchesQuery},{qs[2].SinglePatternMatchesQuery},{qs[2].DoublePatternFixedMatchesQuery},{qs[2].DoublePatternVariableMatchesQuery}\n");
                    }

                    foreach ((var name, var dataStructure) in existenceDataStructures)
                    {
                        Console.WriteLine($"Running + {name + '_' + i} on {textName}");
                        var b = BenchExistDataStructure(name, dataStructure, textName, suffixA, query.X, query.Y.Min, query.Y.Max, queries);
                        foreach (var bench in b)
                        {
                            table.AddRow($"{bench.DataStructureName} {bench.DataName}", $"{bench.ConstructionTimeMilliseconds}", $"{bench.QueryName}", $"{bench.SinglePatternMatchesQuery}ms, Occs: {bench.SinglePatternMatchesQueryOccs}", $"{bench.DoublePatternFixedMatchesQuery}ms, Occs: {bench.DoublePatternFixedMatchesQueryOccs}", $"{bench.DoublePatternVariableMatchesQuery}ms, Occs: {bench.DoublePatternVariableMatchesQueryOccs}");
                            Console.WriteLine($"{bench.DataStructureName}, {bench.ConstructionTimeMilliseconds}, {bench.QueryName}, {bench.SinglePatternMatchesQuery}, {bench.DoublePatternFixedMatchesQuery}, {bench.DoublePatternVariableMatchesQuery}");
                        }
                        var qs = b.Select(s => (s.SinglePatternMatchesQuery, s.DoublePatternFixedMatchesQuery, s.DoublePatternVariableMatchesQuery)).ToArray();
                        var bb = b[0];
                        File.AppendAllText($"{resultsDir}\\{name + '_' + i}.csv", $"{textName},{bb.ConstructionTimeMilliseconds},{qs[0].SinglePatternMatchesQuery},{qs[0].DoublePatternFixedMatchesQuery},{qs[0].DoublePatternVariableMatchesQuery},{qs[1].SinglePatternMatchesQuery},{qs[1].DoublePatternFixedMatchesQuery},{qs[1].DoublePatternVariableMatchesQuery},{qs[2].SinglePatternMatchesQuery},{qs[2].DoublePatternFixedMatchesQuery},{qs[2].DoublePatternVariableMatchesQuery}\n");
                    }
                }

                
            }
            table.Write();
        }

        

        public static Benchmark[] BenchReportDataStructure(string dsname, BuildReportDataStructure matcher, string name, SuffixArrayFinal str, params Query[] queries)
        {
            Benchmark[] benchmarks = new Benchmark[queries.Length];
            Stopwatch stopwatch = Stopwatch.StartNew();
            // Construction
            ReportDataStructure patternMatcher = null;
            patternMatcher = matcher.Invoke(str);
            stopwatch.Stop();
            for (int i = 0; i < benchmarks.Length; i++)
            {
                benchmarks[i] = new Benchmark();
                benchmarks[i].ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
                benchmarks[i].DataStructureName = dsname;
                benchmarks[i].DataName = name;
                benchmarks[i].QueryName = queries[i].QueryName;
            }

            // Query Time
            for (int i = 0; i < queries.Length; i++) // For each query
            {
                // Single Pattern
                stopwatch = Stopwatch.StartNew();
                patternMatcher.Matches(queries[i].P1);
                stopwatch.Stop();
                benchmarks[i].SinglePatternMatchesQuery = stopwatch.ElapsedMilliseconds;

                // Double Pattern + Fixed Gap
                stopwatch = Stopwatch.StartNew();
                patternMatcher.Matches(queries[i].P1, queries[i].X, queries[i].P2);
                stopwatch.Stop();
                benchmarks[i].DoublePatternFixedMatchesQuery = stopwatch.ElapsedMilliseconds;
                //benchmarks[i].DoublePatternFixedMatchesQueryOccs = occs.Count();

                // Double Pattern + Variable Gap
                stopwatch = Stopwatch.StartNew();
                patternMatcher.Matches(queries[i].P1, queries[i].Y.Min, queries[i].Y.Max, queries[i].P2);
                stopwatch.Stop();
                benchmarks[i].DoublePatternVariableMatchesQuery = stopwatch.ElapsedMilliseconds;
                //benchmarks[i].DoublePatternVariableMatchesQueryOccs = occs.Count();

            }
            return benchmarks;
        }

        public static Benchmark[] BenchCountDataStructure(string dsname, BuildCountDataStructure matcher, string name, SuffixArrayFinal str, int x, int ymin, int ymax, params Query[] queries)
        {
            Benchmark[] benchmarks = new Benchmark[queries.Length];
            Stopwatch stopwatch = Stopwatch.StartNew();
            // Construction
            CountDataStructure patternMatcher = null;

            patternMatcher = matcher.Invoke(str, x, ymin, ymax);

            stopwatch.Stop();
            for (int i = 0; i < benchmarks.Length; i++)
            {
                benchmarks[i] = new Benchmark();
                benchmarks[i].ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
                benchmarks[i].DataStructureName = dsname;
                benchmarks[i].DataName = name;
                benchmarks[i].QueryName = queries[i].QueryName;
            }

            // Query Time
            for (int i = 0; i < queries.Length; i++) // For each query
            {
                // Single Pattern

                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.Matches(queries[i].P1);
                    stopwatch.Stop();
                    benchmarks[i].SinglePatternMatchesQueryOccs = occs;
                    benchmarks[i].SinglePatternMatchesQuery = stopwatch.ElapsedMilliseconds;
                }
                catch (Exception e)
                {
                    File.AppendAllText("exceptions", e.StackTrace);
                    benchmarks[i].SinglePatternMatchesQuery = -1;
                }


                // Double Pattern + Fixed Gap
                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.MatchesFixed(queries[i].P1, queries[i].P2);
                    stopwatch.Stop();
                    benchmarks[i].DoublePatternFixedMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmarks[i].DoublePatternFixedMatchesQueryOccs = occs;
                }
                catch (Exception e)
                {
                    File.AppendAllText("exceptions", e.StackTrace);
                    benchmarks[i].DoublePatternFixedMatchesQuery = -1;
                }

                // Double Pattern + Variable Gap

                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.MatchesVariable(queries[i].P1, queries[i].P2);
                    stopwatch.Stop();
                    benchmarks[i].DoublePatternVariableMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmarks[i].DoublePatternVariableMatchesQueryOccs = occs;
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

        public static Benchmark[] BenchExistDataStructure(string dsname, BuildExistDataStructure matcher, string name, SuffixArrayFinal str, int x, int ymin, int ymax, params Query[] queries)
        {
            Benchmark[] benchmarks = new Benchmark[queries.Length];
            Stopwatch stopwatch = Stopwatch.StartNew();
            // Construction
            ExistDataStructure patternMatcher = null;

            patternMatcher = matcher.Invoke(str, x, ymin, ymax);

            stopwatch.Stop();
            for (int i = 0; i < benchmarks.Length; i++)
            {
                benchmarks[i] = new Benchmark();
                benchmarks[i].ConstructionTimeMilliseconds = stopwatch.ElapsedMilliseconds;
                benchmarks[i].DataStructureName = dsname;
                benchmarks[i].DataName = name;
                benchmarks[i].QueryName = queries[i].QueryName;
            }

            // Query Time
            for (int i = 0; i < queries.Length; i++) // For each query
            {
                // Single Pattern

                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.Matches(queries[i].P1);
                    stopwatch.Stop();
                    benchmarks[i].SinglePatternMatchesQueryOccs = occs;
                    benchmarks[i].SinglePatternMatchesQuery = stopwatch.ElapsedMilliseconds;
                }
                catch (Exception e)
                {
                    File.AppendAllText("exceptions", e.StackTrace);
                    benchmarks[i].SinglePatternMatchesQuery = -1;
                }


                // Double Pattern + Fixed Gap
                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.MatchesFixedGap(queries[i].P1, queries[i].P2);
                    stopwatch.Stop();
                    benchmarks[i].DoublePatternFixedMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmarks[i].DoublePatternFixedMatchesQueryOccs = occs;

                }
                catch (Exception e)
                {
                    File.AppendAllText("exceptions", e.StackTrace);
                    benchmarks[i].DoublePatternFixedMatchesQuery = -1;
                }

                // Double Pattern + Variable Gap

                try
                {
                    stopwatch = Stopwatch.StartNew();
                    var occs = patternMatcher.MatchesVariableGap(queries[i].P1, queries[i].P2);
                    stopwatch.Stop();
                    benchmarks[i].DoublePatternVariableMatchesQuery = stopwatch.ElapsedMilliseconds;
                    benchmarks[i].DoublePatternVariableMatchesQueryOccs = occs;
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


        private static void SetupDirectories(out string slnDir, out string resultDir)
        {
            slnDir = Helper.TryGetSolutionDirectoryInfo().ToString();
            Directory.CreateDirectory($"{slnDir}\\StringIndexingGapResults");
            var files = Directory.GetFileSystemEntries($"{slnDir}\\StringIndexingGapResults", "Results_*");
            int last = 0;
            if (files.Length > 0)
            {
                last = files.Select(s => Convert.ToInt32(s.Split('_')[1])).Max() + 1;
            }
            resultDir = Directory.CreateDirectory($"{slnDir}\\StringIndexingGapResults\\Results_{last}").ToString();
        }

        private static void GetTestSetup(string slnDir, string resultsDir, out IEnumerable<(string, GetData)> testData, out (string, BuildReportDataStructure)[] reportingDataStructures, out (string, BuildCountDataStructure)[] countingDataStructures, out (string, BuildExistDataStructure)[] existenceDataStructures)
        {
            testData = DummyData.GetData(new DS[] {
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

            reportingDataStructures = new (string, BuildReportDataStructure)[]
            {
                ("SA_R_V1", BuildSuffixArray_V1),
                ("SA_R_V2", BuildSuffixArray_V2),
                /*
                ("SA_R_V3", BuildSuffixArray_V3),
                ("SA_R_V4_1", BuildSuffixArray_V4_1),
                */
                ("SA_R_V4_2", BuildSuffixArray_V4_2),
                ("SA_R_V4_3", BuildSuffixArray_V4_3),
                ("SA_R_V5", BuildSuffixArray_V5)
            };
            countingDataStructures = new (string, BuildCountDataStructure)[]
            {
                ("SA_C_V1", BuildSA_C_V1),
                //("SA_C_V2", BuildSA_C_V2)
            };
            existenceDataStructures = new (string, BuildExistDataStructure)[]
            {
                ("SA_E_V0", BuildSA_E_V0),
                /*
                ("SA_E_V1", BuildSA_E_V1),
                ("SA_E_V2", BuildSA_E_V2),
                ("SA_E_V3", BuildSA_E_V3),
                ("SA_E_V4", BuildSA_E_V4)
                */
            };

            for (int i = 0; i < 10; i++)
            {
                foreach (var item in reportingDataStructures)
                {
                    File.WriteAllText($"{resultsDir}\\{item.Item1}_{i}.csv", "data,construction,topSingle,topFixed,topVariable,midSingle,midFixed,midVariable,bottomSingle,bottomFixed,bottomVariable\n");
                }

                foreach (var item in countingDataStructures)
                {
                    File.WriteAllText($"{resultsDir}\\{item.Item1}_{i}.csv", "data,construction,topSingle,topFixed,topVariable,midSingle,midFixed,midVariable,bottomSingle,bottomFixed,bottomVariable\n");
                }

                foreach (var item in existenceDataStructures)
                {
                    File.WriteAllText($"{resultsDir}\\{item.Item1}_{i}.csv", "data,construction,topSingle,topFixed,topVariable,midSingle,midFixed,midVariable,bottomSingle,bottomFixed,bottomVariable\n");
                }
            }

            
        }

        private static void InitConsoleTable()
        {
            table = new ConsoleTable("Data Structure & Data", "Construction Time MS", "Pattern Type", "Single Pattern Query Time MS", "Double Pattern Fixed Query Time MS", "Double Pattern Variable Query Time MS");
            table.Options.NumberAlignment = Alignment.Right;
        }

        static ConsoleTable table;







        public delegate ReportDataStructure BuildReportDataStructure(SuffixArrayFinal str);
        public delegate ExistDataStructure BuildExistDataStructure(SuffixArrayFinal str, int x, int ymin, int ymax);
        public delegate CountDataStructure BuildCountDataStructure(SuffixArrayFinal str, int x, int ymin, int ymax);
        public delegate string GetData(string str);

        public static ReportDataStructure BuildSuffixArray_V1(SuffixArrayFinal str)
        {
            return new SA_R_V1(str);
        }

        public static ReportDataStructure BuildSuffixArray_V2(SuffixArrayFinal str)
        {
            return new SA_R_V2(str);
        }

        public static ReportDataStructure BuildSuffixArray_V3(SuffixArrayFinal str)
        {
            return new SA_R_V3(str);
        }

        public static ReportDataStructure BuildSuffixArray_V4_1(SuffixArrayFinal str)
        {
            return new SA_R_V4_1(str);
        }

        public static ReportDataStructure BuildSuffixArray_V5(SuffixArrayFinal str)
        {
            return new SA_R_V5(str);
        }

        public static ReportDataStructure BuildSuffixArray_V4_2(SuffixArrayFinal str)
        {
            return new SA_R_V4_2(str);
        }

        public static ReportDataStructure BuildSuffixArray_V4_3(SuffixArrayFinal str)
        {
            return new SA_R_V4_3(str);
        }
        public static CountDataStructure BuildSA_C_V1(SuffixArrayFinal str, int x, int ymin, int ymax)
        {
            return new SA_C_V1(str, x, ymin, ymax);
        }
        public static CountDataStructure BuildSA_C_V2(SuffixArrayFinal str, int x, int ymin, int ymax)
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

        public static ExistDataStructure BuildSA_E_V0(SuffixArrayFinal str, int x, int ymin, int ymax)
        {
            return new SA_E_V0(str, x, ymin, ymax);
        }

        public static ExistDataStructure BuildSA_E_V1(SuffixArrayFinal str, int x, int ymin, int ymax)
        {
            return new SA_E_V1(str, x, ymin, ymax);
        }

        public static ExistDataStructure BuildSA_E_V2(SuffixArrayFinal str, int x, int ymin, int ymax)
        {
            return new SA_E_V2(str, x, ymin, ymax);
        }
        public static ExistDataStructure BuildSA_E_V3(SuffixArrayFinal str, int x, int ymin, int ymax)
        {
            return new SA_E_V3(str, x, ymin, ymax);
        }
        public static ExistDataStructure BuildSA_E_V4(SuffixArrayFinal str, int x, int ymin, int ymax)
        {
            return new SA_E_V4(str, x, ymin, ymax);
        }
    }
}

