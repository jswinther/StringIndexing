﻿using ConsoleApp.Data.Obsolete;
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
            (string, BuildFixedReportDataStructure)[] fixedReportDataStructures;
            (string, BuildVariableReportDataStructure)[] variableReportDataStructures;
            (string, BuildFixedExistDataStructure)[] fixedExistDataStructures;
            (string, BuildVariableExistDataStructure)[] variableExistDataStructures;
            (string, BuildFixedCountDataStructure)[] fixedCountingDataStructures;
            (string, BuildVariableCountDataStructure)[] variableCountingDataStructures;
            string sln;
            string resultsDir;
            SetupDirectories(out sln, out resultsDir);

            var tests = new string[]
            {
                "DNA_256",
                "DNA_512",
                "DNA_1024",
                "DNA_2048",
                "DNA_4096",
                "DNA_8192",
                "DNA_16384",
                "DNA_32768",
                "DNA_65536",
                "DNA_131072",
                "DNA_262144",
                "DNA_524288",
                "DNA_1048576",
                "DNA_2097152",
                "DNA_4194304",
                "DNA_8388608",
                "DNA_16777216",
                "DNA_33554432",

                "english_256",
                "english_512",
                "english_1024",
                "english_2048",
                "english_4096",
                "english_8192",
                "english_16384",
                "english_32768",
                "english_65536",
                "english_131072",
                "english_262144",
                "english_524288",
                "english_1048576",
                "english_2097152",
                "english_4194304",
                "english_8388608",
                "english_16777216",
                "english_33554432",

                "proteins_256",
                "proteins_512",
                "proteins_1024",
                "proteins_2048",
                "proteins_4096",
                "proteins_8192",
                "proteins_16384",
                "proteins_32768",
                "proteins_65536",
                "proteins_131072",
                "proteins_262144",
                "proteins_524288",
                "proteins_1048576",
                "proteins_2097152",
                "proteins_4194304",
                "proteins_8388608",
                "proteins_16777216",
                "proteins_33554432",

                "realDNA_256",
                "realDNA_512",
                "realDNA_1024",
                "realDNA_2048",
                "realDNA_4096",
                "realDNA_8192",
                "realDNA_16384",
                "realDNA_32768",
                "realDNA_65536",
                "realDNA_131072",
                "realDNA_262144",
                "realDNA_524288",
                "realDNA_1048576",
                "realDNA_2097152",
                "realDNA_4194304",
                "realDNA_8388608",
                "realDNA_16777216",
                "realDNA_33554432",
            };

            fixedReportDataStructures = new (string, BuildFixedReportDataStructure)[]
            {
                //("Fixed_Report_SA_Runtime", Fixed_SA_Runtime_Build),
                ("Fixed_Report_ESA_Runtime", Fixed_ESA_Runtime_Build),
                ("Fixed_Report_ESA_Hashed", Fixed_ESA_Hashed_Build),
                ("Fixed_Report_ESA_PartialHash", Fixed_ESA_PartialHash_Build),
            };

            variableReportDataStructures = new (string, BuildVariableReportDataStructure)[]
            {
                //("Variable_Report_SA_Runtime", Variable_SA_Runtime_Build),
                ("Variable_Report_ESA_Runtime", Variable_ESA_Runtime_Build),
                ("Variable_Report_ESA_Sorted", Variable_ESA_Sorted_Build),
                ("Variable_Report_PartialSort", Variable_ESA_PartialSort_Build),
                ("Variable_Report_PartialSort_TopNodes", Variable_ESA_PartialSort_TopNodes_Build),
                ("Variable_Report_ESA_KdTrees", Variable_ESA_2D_Build)
            };

            fixedCountingDataStructures = new (string, BuildFixedCountDataStructure)[]
            {
                ("Fixed_Count_ESA_Runtime", Build_Count_Fixed_ESA_Runtime),
                
            };

            variableCountingDataStructures = new (string, BuildVariableCountDataStructure)[]
            {
                ("Variable_Count_ESA_Runtime", Build_Count_Variable_ESA_Runtime),
            };

            fixedExistDataStructures = new (string, BuildFixedExistDataStructure)[]
            {
                ("Fixed_Exist_ESA_Runtime", Build_Exist_Fixed_ESA_Runtime),
                ("Fixed_Exist_ESA_PartiallyHashed",Fixed_Exist_ESA_PartiallyHashed)
            };

            variableExistDataStructures = new (string, BuildVariableExistDataStructure)[]
            {
                ("Variable_Exist_ESA_Runtime", Build_Exist_Variable_ESA_Runtime),
            };

            

            foreach (var item in fixedReportDataStructures)
            {
                File.WriteAllText($"{resultsDir}\\{item.Item1}.csv", "data,construction,topSingle,topFixed,midSingle,midFixed,bottomSingle,bottomFixed\n");
            }

            foreach (var item in variableReportDataStructures)
            {
                File.WriteAllText($"{resultsDir}\\{item.Item1}.csv", "data,construction,topSingle,topVariable,midSingle,midVariable,bottomSingle,bottomVariable\n");
            }

            foreach (var item in fixedCountingDataStructures)
            {
                File.WriteAllText($"{resultsDir}\\{item.Item1}.csv", "data,construction,topFixed,midFixed,bottomFixed\n");
            }

            foreach (var item in variableCountingDataStructures)
            {
                File.WriteAllText($"{resultsDir}\\{item.Item1}.csv", "data,construction,topVariable,midVariable,bottomVariable\n");
            }

            foreach (var item in fixedExistDataStructures)
            {
                File.WriteAllText($"{resultsDir}\\{item.Item1}.csv", "data,construction,topFixed,midFixed,bottomFixed\n");
            }

            foreach (var item in variableExistDataStructures)
            {
                File.WriteAllText($"{resultsDir}\\{item.Item1}.csv", "data,construction,topVariable,midVariable,bottomVariable\n");
            }

            InitConsoleTable();
            


            string p1 = "a";
            Random random = new Random();
            int x = 5;
            string p2 = "a";
            Query query = new Query(p1, x, p2);

            int reps = 10;

            foreach (var textName in tests)
            {
                var json = File.ReadAllText($"{Helper.TryGetSolutionDirectoryInfo()}\\{textName}.json");
                var suffixA = JsonSerializer.Deserialize<SuffixArrayFinal>(json);
                suffixA.BuildChildTable();
                query.Y = (5, 25);

                Query query1 = new Query("a", 5, "a");
                Query query2 = new Query("a", 5, "a");
                Query query3 = new Query("a", 5, "a");
                /*
                using (SuffixArray_Scanner suffixArray_Scanner = new SuffixArray_Scanner(suffixA))
                {
                    query1 = new Query(suffixArray_Scanner.topPattern, x, p2, "Top");
                    query2 = new Query(suffixArray_Scanner.midPatterns.GetRandom(), x, p2, "Mid");
                    query3 = new Query(suffixArray_Scanner.botPattern, x, p2, "Bot");
                    
                }
                */
                Stopwatch stopwatch;
                query1.Y = query.Y; query2.Y = query.Y; query3.Y = query.Y;

                foreach ((var name, var dataStructure) in fixedReportDataStructures)
                {
                    stopwatch = Stopwatch.StartNew();
                    IReportFixed reportFixed = dataStructure.Invoke(suffixA);
                    stopwatch.Stop();
                    var constructionTime = stopwatch.ElapsedMilliseconds;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.ReportHashedOccurrences(query1.P1);
                    }
                    stopwatch.Stop();
                    var topQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    /*
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.ReportHashedOccurrences(query2.P1);
                    }
                    stopwatch.Stop();
                    var midQueryTime = stopwatch.ElapsedMilliseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.ReportHashedOccurrences(query3.P1);
                    }
                    stopwatch.Stop();
                    var bottomQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    */
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.Matches(query1.P1, query1.X, query1.P2);
                    }
                    stopwatch.Stop();
                    var topFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    /*
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.Matches(query2.P1, query2.X, query2.P2);
                    }
                    stopwatch.Stop();
                    var midFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.Matches(query3.P1, query3.X, query3.P2);
                    }
                    stopwatch.Stop();
                    var bottomFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    */
                    
                    File.AppendAllText($"{resultsDir}\\{name}.csv", 
                        $"{textName},{constructionTime},{topQueryTime},{topFixedQueryTime}\n");
                    Console.WriteLine($"{name} {textName},{constructionTime},{topQueryTime},{topFixedQueryTime}");
                }

                foreach ((var name, var dataStructure) in variableReportDataStructures)
                {
                    
                    stopwatch = Stopwatch.StartNew();
                    IReportVariable reportVariable = dataStructure.Invoke(suffixA);
                    stopwatch.Stop();
                    var constructionTime = stopwatch.ElapsedMilliseconds;
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportVariable.ReportSortedOccurrences(query1.P1);
                    }
                    stopwatch.Stop();
                    var topQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    /*
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportVariable.ReportSortedOccurrences(query2.P1);
                    }
                    stopwatch.Stop();
                    var midQueryTime = stopwatch.ElapsedMilliseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportVariable.ReportSortedOccurrences(query3.P1);
                    }
                    stopwatch.Stop();
                    var bottomQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    */
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportVariable.Matches(query1.P1, query1.Y.Min, query1.Y.Max, query1.P2);
                    }
                    stopwatch.Stop();
                    var topFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    /*
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportVariable.Matches(query2.P1, query2.Y.Min, query2.Y.Max, query2.P2);
                    }
                    stopwatch.Stop();
                    var midFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportVariable.Matches(query3.P1, query3.Y.Min, query3.Y.Max, query3.P2);
                    }
                    stopwatch.Stop();
                    var bottomFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    */
                    File.AppendAllText($"{resultsDir}\\{name}.csv",
                        $"{textName},{constructionTime},{topQueryTime},{topFixedQueryTime}\n");
                    Console.WriteLine($"{name} {textName}\t{constructionTime}\t{topQueryTime}\t{topFixedQueryTime}");
                }

                foreach ((var name, var dataStructure) in fixedCountingDataStructures)
                {
                    
                    stopwatch = Stopwatch.StartNew();
                    CountFixed countFixed = dataStructure.Invoke(suffixA, x);
                    stopwatch.Stop();
                    var constructionTime = stopwatch.ElapsedMilliseconds;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(query1.P1, query1.P2);
                    }
                    stopwatch.Stop();
                    var topFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    /*
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(query2.P1, query2.P2);
                    }
                    stopwatch.Stop();
                    var midFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(query3.P1,  query3.P2);
                    }
                    stopwatch.Stop();
                    var bottomFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    */
                    File.AppendAllText($"{resultsDir}\\{name}.csv",
                        $"{textName},{constructionTime},{topFixedQueryTime}\n");
                    Console.WriteLine($"{name} {textName}\t{constructionTime},{topFixedQueryTime}");
                }

                foreach ((var name, var dataStructure) in variableCountingDataStructures)
                {
                    
                    stopwatch = Stopwatch.StartNew();
                    CountVariable countFixed = dataStructure.Invoke(suffixA, query.Y.Min, query.Y.Max);
                    stopwatch.Stop();
                    var constructionTime = stopwatch.ElapsedMilliseconds;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(query1.P1, query1.P2);
                    }
                    stopwatch.Stop();
                    var topFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    /*
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(query2.P1, query2.P2);
                    }
                    stopwatch.Stop();
                    var midFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(query3.P1, query3.P2);
                    }
                    stopwatch.Stop();
                    var bottomFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    */
                    File.AppendAllText($"{resultsDir}\\{name}.csv",
                        $"{textName},{constructionTime},{topFixedQueryTime}\n");
                    Console.WriteLine($"{name} {textName}\t{constructionTime},{topFixedQueryTime}");
                }

                foreach ((var name, var dataStructure) in fixedExistDataStructures)
                {
                    
                    stopwatch = Stopwatch.StartNew();
                    ExistFixed countFixed = dataStructure.Invoke(suffixA, x);
                    stopwatch.Stop();
                    var constructionTime = stopwatch.ElapsedMilliseconds;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(query1.P1, query1.P2);
                    }
                    stopwatch.Stop();
                    var topFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    /*
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(query2.P1, query2.P2);
                    }
                    stopwatch.Stop();
                    var midFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(query3.P1, query3.P2);
                    }
                    stopwatch.Stop();
                    var bottomFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    */
                    File.AppendAllText($"{resultsDir}\\{name}.csv",
                        $"{textName},{constructionTime},{topFixedQueryTime}\n");
                    Console.WriteLine($"{name} {textName}\t{constructionTime},{topFixedQueryTime}");
                }

                foreach ((var name, var dataStructure) in variableExistDataStructures)
                {
                    
                    stopwatch = Stopwatch.StartNew();
                    ExistVariable countFixed = dataStructure.Invoke(suffixA, query.Y.Min, query.Y.Max);
                    stopwatch.Stop();
                    var constructionTime = stopwatch.ElapsedMilliseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(query1.P1, query1.P2);
                    }
                    stopwatch.Stop();
                    var topFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    /*
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(query2.P1, query2.P2);
                    }
                    stopwatch.Stop();
                    var midFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(query3.P1, query3.P2);
                    }
                    stopwatch.Stop();
                    var bottomFixedQueryTime = stopwatch.ElapsedMilliseconds / reps;
                    */
                    File.AppendAllText($"{resultsDir}\\{name}.csv",
                        $"{textName},{constructionTime},{topFixedQueryTime}\n");
                    Console.WriteLine($"{name} {textName}\t{constructionTime},{topFixedQueryTime}");
                }

            }
            table.Write();
        }

        private static ExistFixed Fixed_Exist_ESA_PartiallyHashed(SuffixArrayFinal str, int x)
        {
            return new Fixed_Exist_ESA_PartiallyHashed(str, x);
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

        

        private static void InitConsoleTable()
        {
            table = new ConsoleTable("Data Structure & Data", "Construction Time MS", "Pattern Type", "Single Pattern Query Time MS", "Double Pattern Fixed Query Time MS", "Double Pattern Variable Query Time MS");
            table.Options.NumberAlignment = Alignment.Right;
        }

        static ConsoleTable table;







        //public delegate ReportDataStructure BuildReportDataStructure(SuffixArrayFinal str);
        public delegate IReportFixed BuildFixedReportDataStructure(SuffixArrayFinal str);
        public delegate IReportVariable BuildVariableReportDataStructure(SuffixArrayFinal str);
        public delegate ExistFixed BuildFixedExistDataStructure(SuffixArrayFinal str, int x);
        public delegate ExistVariable BuildVariableExistDataStructure(SuffixArrayFinal str, int ymin, int ymax);
        public delegate CountFixed BuildFixedCountDataStructure(SuffixArrayFinal str, int x);
        public delegate CountVariable BuildVariableCountDataStructure(SuffixArrayFinal str, int ymin, int ymax);
        public delegate string GetData(string str);






        public static ReportFixed Fixed_SA_Runtime_Build(SuffixArrayFinal str)
        {
            return new Fixed_SA_Runtime(str);
        }

        public static ReportFixed Fixed_ESA_Runtime_Build(SuffixArrayFinal str)
        {
            return new DataStructures.Reporting.Fixed_ESA_Runtime(str);
        }

        public static ReportFixed Fixed_ESA_Hashed_Build(SuffixArrayFinal str)
        {
            return new Fixed_ESA_Hashed(str);
        }

        public static ReportFixed Fixed_ESA_PartiallyHashed_V1_Build(SuffixArrayFinal str)
        {
            return new Fixed_ESA_PartiallyHashed_Obsolete(str);
        }

        public static ReportFixed Fixed_ESA_PartialHash_Build(SuffixArrayFinal str)
        {
            return new Fixed_PartialHash(str);
        }

        public static ReportVariable Variable_SA_Runtime_Build(SuffixArrayFinal str)
        {
            return new Variable_SA_Runtime(str);
        }

        public static ReportVariable Variable_ESA_Runtime_Build(SuffixArrayFinal str)
        {
            return new DataStructures.Reporting.Variable_ESA_Runtime(str);
        }

        public static ReportVariable Variable_ESA_Sorted_Build(SuffixArrayFinal str)
        {
            return new Variable_ESA_Sorted(str);
        }

        public static ReportVariable Variable_ESA_PartiallySorted_V1_Build(SuffixArrayFinal str)
        {
            return new Variable_ESA_PartiallySorted_Obsolete(str);
        }

        public static ReportVariable Variable_ESA_PartialSort_Build(SuffixArrayFinal str)
        {
            return new Variable_PartialSort(str);
        }

        public static ReportVariable Variable_ESA_PartialSort_TopNodes_Build(SuffixArrayFinal str)
        {
            return new Variable_PartialSort_TopNodes(str);
        }

        public static ReportVariable Variable_ESA_2D_Build(SuffixArrayFinal str)
        {
            return new Variable_ESA_2D(str);
        }

        

        public static CountFixed Build_Count_Fixed_ESA_Runtime(SuffixArrayFinal str, int x)
        {
            return new DataStructures.Count.Fixed_ESA_Runtime(str, x);
        }

        public static CountVariable Build_Count_Variable_ESA_Runtime(SuffixArrayFinal str, int ymin, int ymax)
        {
            return new DataStructures.Count.Variable_ESA_Runtime(str, ymin, ymax);
        }


        public static ExistFixed Build_Exist_Fixed_ESA_Runtime(SuffixArrayFinal str, int x)
        {
            return new DataStructures.Existence.Fixed_ESA_Runtime(str, x);
        }

        public static ExistVariable Build_Exist_Variable_ESA_Runtime(SuffixArrayFinal str, int ymin, int ymax)
        {
            return new DataStructures.Existence.Variable_ESA_Runtime(str, ymin, ymax);
        }





    }
}

