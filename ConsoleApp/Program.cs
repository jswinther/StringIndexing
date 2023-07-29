using ConsoleApp.DataStructures;
using ConsoleApp.DataStructures.Count;
using ConsoleApp.DataStructures.Existence;
using ConsoleApp.DataStructures.Reporting;
using ConsoleApp.DataStructures.Single;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using static ConsoleApp.Program;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp
{
    public class Program
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
                "english_256",
                "realDNA_256",
                "DNA_256",
                "proteins_256",
                "english_512",
                "DNA_512",
                "realDNA_512",
                "proteins_512",
                "DNA_1024",
                "english_1024",
                "realDNA_1024",
                "proteins_1024",
                "english_2048",
                "DNA_2048",
                "realDNA_2048",
                "proteins_2048",
                "english_4096",
                "DNA_4096",
                "realDNA_4096",
                "proteins_4096",
                "english_8192",
                "realDNA_8192",
                "DNA_8192",
                "proteins_8192",
                "proteins_16384",
                "realDNA_16384",
                "DNA_16384",
                "english_16384",
                "DNA_32768",
                "english_32768",
                "realDNA_32768",
                "proteins_32768",
                
                "english_65536",
                "realDNA_65536",
                "DNA_65536",
                "proteins_65536",
                "DNA_131072",
                "proteins_131072",
                "realDNA_131072",
                "english_131072",
                "proteins_262144",
                "realDNA_262144",
                "DNA_262144",
                "english_262144",
                "proteins_524288",
                "realDNA_524288",
                "DNA_524288",
                "english_524288",
                
                "proteins_1048576",
                "realDNA_1048576",
                "DNA_1048576",
                "english_1048576",
                "proteins_2097152",
                "realDNA_2097152",
                "DNA_2097152",
                "english_2097152",
                "proteins_4194304",
                "DNA_4194304",
                "realDNA_4194304",
                "english_4194304",
                /*
                "proteins_8388608",
                "DNA_8388608",
                "realDNA_8388608",
                "english_8388608",
                
                "proteins_16777216",
                "DNA_16777216",
                "realDNA_16777216",
                "english_16777216",
                "proteins_33554432",
                "realDNA_33554432",
                "english_33554432",
                "DNA_33554432",
                */
            };

            var singlePatternReportingDataStructures = new (string, BuildSinglePatternReporting)[]
            {
                ("Precomputed Substrings", BuildPrecomputedSubstrings),
                ("Suffix Tree", BuildSuffixTree),
                ("Suffix Array", BuildSuffixArray),
                ("Enhanced Suffix Array", BuildEnhancedSuffixArray)
            };

            fixedReportDataStructures = new (string, BuildFixedReportDataStructure)[]
            {
                
                ("Fixed_Report_SA_Runtime", Fixed_SA_Runtime_Build),
                ("Fixed_Report_ESA_Runtime", Fixed_ESA_Runtime_Build),
                ("Fixed_Report_ESA_Hashed", Fixed_ESA_Hashed_Build),
                ("Fixed_Report_ESA_PartialHash", Fixed_ESA_PartialHash_Build),
                
            };

            variableReportDataStructures = new (string, BuildVariableReportDataStructure)[]
            {
                
                
                ("Variable_Report_SA_Runtime", Variable_SA_Runtime_Build),
                ("Variable_Report_ESA_Runtime", Variable_ESA_Runtime_Build),
                ("Variable_Report_ESA_Sorted", Variable_ESA_Sorted_Build),
                ("Variable_Report_PartialSort", Variable_ESA_PartialSort_Build),
                ("Variable_Report_PartialSort_TopNodes", Variable_ESA_PartialSort_TopNodes_Build),
                ("Variable_Report_ESA_KdTrees", Variable_ESA_2D_Build),
                ("Variable_Report_ESA_BottomUp", Variable_ESA_BottomUp_Build),

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
            };

            variableExistDataStructures = new (string, BuildVariableExistDataStructure)[]
            {
                ("Variable_Exist_ESA_Runtime", Build_Exist_Variable_ESA_Runtime),
            };

            File.WriteAllText($"{resultsDir}\\Fixed_Report_SuffixTree_Hash.csv", "data,length,construction,dptop,dpmid,dpbot\n");
            File.WriteAllText($"{resultsDir}\\Variable_Report_SuffixTree_1DRP.csv", "data,length,construction,dptop,dpmid,dpbot\n");
            File.WriteAllText($"{resultsDir}\\Fixed_Report_PreComp_Hash.csv", "data,length,construction,dptop,dpmid,dpbot\n");
            File.WriteAllText($"{resultsDir}\\Variable_Report_PreComp_1DRP.csv", "data,length,construction,dptop,dpmid,dpbot\n");

            foreach (var item in fixedReportDataStructures)
            {
                File.WriteAllText($"{resultsDir}\\{item.Item1}.csv", "data,length,construction,sptop,spmid,spbot,dptop,dpmid,dpbot\n");
            }

            foreach (var item in variableReportDataStructures)
            {
                File.WriteAllText($"{resultsDir}\\{item.Item1}.csv", "data,length,construction,sptop,spmid,spbot,dptop,dpmid,dpbot\n");
            }

            foreach (var item in fixedCountingDataStructures)
            {
                File.WriteAllText($"{resultsDir}\\{item.Item1}.csv", "data,length,construction,dptop,dpmid,dpbot\n");
            }

            foreach (var item in variableCountingDataStructures)
            {
                File.WriteAllText($"{resultsDir}\\{item.Item1}.csv", "data,length,construction,dptop,dpmid,dpbot\n");
            }

            foreach (var item in fixedExistDataStructures)
            {
                File.WriteAllText($"{resultsDir}\\{item.Item1}.csv", "data,length,construction,dptop,dpmid,dpbot\n");
            }

            foreach (var item in variableExistDataStructures)
            {
                File.WriteAllText($"{resultsDir}\\{item.Item1}.csv", "data,length,construction,dptop,dpmid,dpbot\n");
            }

            foreach (var item in singlePatternReportingDataStructures)
            {
                File.WriteAllText($"{resultsDir}\\{item.Item1}.csv", "data,length,construction,topMatching,midMatching,botMatching,topReporting,midReporting,botReporting\n");
            }

            InitConsoleTable();
            


            string p1 = "a";
            Random random = new Random();
            int x = 5;
            string p2 = "a";
            Query query = new Query(p1, x, p2);

            double reps = 10;


            foreach (var textName in tests)
            {
                var json = File.ReadAllText($"{Helper.TryGetSolutionDirectoryInfo()}\\jsonFiles\\{textName}.json");
                var suffixA = JsonSerializer.Deserialize<SuffixArrayFinal>(json);
                suffixA.BuildChildTable();
                query.Y = (5, 25);

                SuffixArray_Scanner suffixArray_Scanner = new SuffixArray_Scanner((textName, ""), suffixA);

                //Query query1 = new Query(suffixArray_Scanner.topPattern, x, p2, "Top");
                //Query query2 = new Query(suffixArray_Scanner.midPatterns.GetRandom(), x, p2, "Mid");
                //Query query3 = new Query(suffixArray_Scanner.botPattern, x, p2, "Bot");
                    
                Stopwatch stopwatch;
                //query1.Y = query.Y; query2.Y = query.Y; query3.Y = query.Y;

                  
                
                foreach ((var name, var dataStructure) in singlePatternReportingDataStructures)
                {
                    if (name.Contains("Precomputed") && suffixA.n > 2050) continue;
                    var dataset = DummyData.Read(textName);
                    stopwatch = Stopwatch.StartNew();
                    IReportSinglePattern reportFixed = dataStructure.Invoke(dataset);
                    stopwatch.Stop();
                    var constructionTime = stopwatch.Elapsed.TotalNanoseconds;
                    
                    long matchingTime = 0;
                    long reportingTime = 0;
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.SinglePatternMatching(suffixArray_Scanner.topPattern[i], out var mt, out var rt);
                        matchingTime += (long)mt;
                        reportingTime += (long)rt;
                    }
                    var topMatchingTime = matchingTime / reps;
                    var topReportingTime = reportingTime / reps;

                    matchingTime = 0;
                    reportingTime = 0;
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.SinglePatternMatching(suffixArray_Scanner.midPatterns[i], out var mt, out var rt);
                        matchingTime += (long)mt;
                        reportingTime += (long)rt;
                    }
                    var midMatchingTime = matchingTime / reps;
                    var midReportingTime = reportingTime / reps;

                    matchingTime = 0;
                    reportingTime = 0;
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.SinglePatternMatching(suffixArray_Scanner.botPattern[i], out var mt, out var rt);
                        matchingTime += (long)mt;
                        reportingTime += (long)rt;
                    }
                    var botMatchingTime = matchingTime / reps;
                    var botReportingTime = reportingTime / reps;

                    var d = textName.Split('_');
                    string s = string.Format($"{d[0]},{d[1]},{(long)constructionTime},{(long)topMatchingTime},{(long)midMatchingTime},{(long)botMatchingTime},{(long)topReportingTime},{(long)midReportingTime},{(long)botReportingTime}\n");
                    File.AppendAllText($"{resultsDir}\\{name}.csv", s);
                    Console.WriteLine(name + " " + s);

                    if (name.Contains("Precomp") && suffixA.n < 2050)
                    {
                        stopwatch = Stopwatch.StartNew();
                        Variable_PreCompSubs vps = new Variable_PreCompSubs((WrapPrecomp)reportFixed);
                        stopwatch.Stop();
                        constructionTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        stopwatch = Stopwatch.StartNew();
                        for (int i = 0; i < reps; i++)
                        {
                            vps.Matches(suffixArray_Scanner.topPattern[i], query.Y.Min, query.Y.Max, suffixArray_Scanner.topPattern.GetRandom());
                        }
                        stopwatch.Stop();
                        var topFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        stopwatch = Stopwatch.StartNew();
                        for (int i = 0; i < reps; i++)
                        {
                            vps.Matches(suffixArray_Scanner.midPatterns[i], query.Y.Min, query.Y.Max, suffixArray_Scanner.topPattern.GetRandom());
                        }
                        stopwatch.Stop();
                        var midFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        stopwatch = Stopwatch.StartNew();
                        for (int i = 0; i < reps; i++)
                        {
                            vps.Matches(suffixArray_Scanner.botPattern[i], query.Y.Min, query.Y.Max, suffixArray_Scanner.topPattern.GetRandom());
                        }
                        stopwatch.Stop();
                        var bottomFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        d = textName.Split('_');

                        s = string.Format("{0},{1},{2},{3},{4},{5}\n",
                            d[0], d[1], (long)constructionTime, (long)topFixedQueryTime, (long)midFixedQueryTime, (long)bottomFixedQueryTime);
                        File.AppendAllText($"{resultsDir}\\Variable_Report_PreComp_1DRP.csv", s);
                        Console.WriteLine(s);

                        stopwatch = Stopwatch.StartNew();
                        Fixed_PreCompSubs fps = new Fixed_PreCompSubs((WrapPrecomp)reportFixed);
                        stopwatch.Stop();
                        constructionTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        stopwatch = Stopwatch.StartNew();
                        for (int i = 0; i < reps; i++)
                        {
                            fps.Matches(suffixArray_Scanner.topPattern[i], x, suffixArray_Scanner.topPattern.GetRandom());
                        }
                        stopwatch.Stop();
                        topFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        stopwatch = Stopwatch.StartNew();
                        for (int i = 0; i < reps; i++)
                        {
                            fps.Matches(suffixArray_Scanner.midPatterns[i], x, suffixArray_Scanner.topPattern.GetRandom());
                        }
                        stopwatch.Stop();
                        midFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        stopwatch = Stopwatch.StartNew();
                        for (int i = 0; i < reps; i++)
                        {
                            fps.Matches(suffixArray_Scanner.botPattern[i], x, suffixArray_Scanner.topPattern.GetRandom());
                        }
                        stopwatch.Stop();
                        bottomFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        d = textName.Split('_');

                        s = string.Format("{0},{1},{2},{3},{4},{5}\n",
                            d[0], d[1], (long)constructionTime, (long)topFixedQueryTime, (long)midFixedQueryTime, (long)bottomFixedQueryTime);
                        File.AppendAllText($"{resultsDir}\\Fixed_Report_PreComp_Hash.csv", s);
                        Console.WriteLine(s);
                    }

                    if (name.Contains("Suffix Tree"))
                    {
                        stopwatch = Stopwatch.StartNew();
                        Variable_ST_Runtime vps = new Variable_ST_Runtime((WrapSuffixTree)reportFixed);
                        stopwatch.Stop();
                        constructionTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        stopwatch = Stopwatch.StartNew();
                        for (int i = 0; i < reps; i++)
                        {
                            vps.Matches(suffixArray_Scanner.topPattern[i], query.Y.Min, query.Y.Max, suffixArray_Scanner.topPattern.GetRandom());
                        }
                        stopwatch.Stop();
                        var topFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        stopwatch = Stopwatch.StartNew();
                        for (int i = 0; i < reps; i++)
                        {
                            vps.Matches(suffixArray_Scanner.midPatterns[i], query.Y.Min, query.Y.Max, suffixArray_Scanner.topPattern.GetRandom());
                        }
                        stopwatch.Stop();
                        var midFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        stopwatch = Stopwatch.StartNew();
                        for (int i = 0; i < reps; i++)
                        {
                            vps.Matches(suffixArray_Scanner.botPattern[i], query.Y.Min, query.Y.Max, suffixArray_Scanner.topPattern.GetRandom());
                        }
                        stopwatch.Stop();
                        var bottomFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        d = textName.Split('_');

                        s = string.Format("{0},{1},{2},{3},{4},{5}\n",
                            d[0], d[1], (long)constructionTime, (long)topFixedQueryTime, (long)midFixedQueryTime, (long)bottomFixedQueryTime);
                        File.AppendAllText($"{resultsDir}\\Variable_Report_SuffixTree_1DRP.csv", s);
                        Console.WriteLine(s);

                        stopwatch = Stopwatch.StartNew();
                        Fixed_ST_Runtime fps = new Fixed_ST_Runtime((WrapSuffixTree)reportFixed);
                        stopwatch.Stop();
                        constructionTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        stopwatch = Stopwatch.StartNew();
                        for (int i = 0; i < reps; i++)
                        {
                            fps.Matches(suffixArray_Scanner.topPattern[i], x, suffixArray_Scanner.topPattern.GetRandom());
                        }
                        stopwatch.Stop();
                        topFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        stopwatch = Stopwatch.StartNew();
                        for (int i = 0; i < reps; i++)
                        {
                            fps.Matches(suffixArray_Scanner.midPatterns[i], x, suffixArray_Scanner.topPattern.GetRandom());
                        }
                        stopwatch.Stop();
                        midFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        stopwatch = Stopwatch.StartNew();
                        for (int i = 0; i < reps; i++)
                        {
                            fps.Matches(suffixArray_Scanner.botPattern[i], x, suffixArray_Scanner.topPattern.GetRandom());
                        }
                        stopwatch.Stop();
                        bottomFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                        d = textName.Split('_');

                        s = string.Format("{0},{1},{2},{3},{4},{5}\n",
                            d[0], d[1], (long)constructionTime, (long)topFixedQueryTime, (long)midFixedQueryTime, (long)bottomFixedQueryTime);
                        File.AppendAllText($"{resultsDir}\\Fixed_Report_SuffixTree_Hash.csv", s);
                        Console.WriteLine(s);
                    }

                }
                

                
                foreach ((var name, var dataStructure) in fixedReportDataStructures)
                {
                    
                    stopwatch = Stopwatch.StartNew();
                    IReportFixed reportFixed = dataStructure.Invoke(suffixA);
                    stopwatch.Stop();
                    var constructionTime = stopwatch.Elapsed.TotalNanoseconds;
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.ReportHashedOccurrences(suffixArray_Scanner.topPattern[i]);
                    }
                    stopwatch.Stop();
                    var topQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.ReportHashedOccurrences(suffixArray_Scanner.midPatterns[i]);
                    }
                    stopwatch.Stop();
                    var midQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.ReportHashedOccurrences(suffixArray_Scanner.botPattern[i]);
                    }
                    stopwatch.Stop();
                    var bottomQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.Matches(suffixArray_Scanner.topPattern[i], x, suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var topFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.Matches(suffixArray_Scanner.midPatterns[i], x, suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var midFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportFixed.Matches(suffixArray_Scanner.botPattern[i], x, suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var bottomFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                   
                    var d = textName.Split('_');
                    string s = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}\n",
                        d[0], d[1], (long)constructionTime, (long)topQueryTime, (long)midQueryTime, (long)bottomQueryTime,
                        (long)topFixedQueryTime, (long)midFixedQueryTime, (long)bottomFixedQueryTime);
                    File.AppendAllText($"{resultsDir}\\{name}.csv", s);
                    Console.WriteLine(s);
                }
                

                foreach ((var name, var dataStructure) in variableReportDataStructures)
                {
                    
                    stopwatch = Stopwatch.StartNew();
                    IReportVariable reportVariable = dataStructure.Invoke(suffixA);
                    stopwatch.Stop();
                    var constructionTime = stopwatch.Elapsed.TotalNanoseconds;
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportVariable.ReportSortedOccurrences(suffixArray_Scanner.topPattern[i]);
                    }
                    stopwatch.Stop();
                    var topQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportVariable.ReportSortedOccurrences(suffixArray_Scanner.midPatterns[i]);
                    }
                    stopwatch.Stop();
                    var midQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportVariable.ReportSortedOccurrences(suffixArray_Scanner.botPattern[i]);
                    }
                    stopwatch.Stop();
                    var bottomQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportVariable.Matches(suffixArray_Scanner.topPattern[i], query.Y.Min, query.Y.Max, suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var topFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportVariable.Matches(suffixArray_Scanner.midPatterns[i], query.Y.Min, query.Y.Max, suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var midFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        reportVariable.Matches(suffixArray_Scanner.botPattern[i], query.Y.Min, query.Y.Max, suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var bottomFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    var d = textName.Split('_');

                    string s = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}\n",
                        d[0], d[1], (long)constructionTime, (long)topQueryTime, (long)midQueryTime, (long)bottomQueryTime,
                        (long)topFixedQueryTime, (long)midFixedQueryTime, (long)bottomFixedQueryTime);
                    File.AppendAllText($"{resultsDir}\\{name}.csv", s);
                    Console.WriteLine(s);
                }

                
                foreach ((var name, var dataStructure) in fixedCountingDataStructures)
                {
                    
                    stopwatch = Stopwatch.StartNew();
                    CountFixed countFixed = dataStructure.Invoke(suffixA, x);
                    stopwatch.Stop();
                    var constructionTime = stopwatch.Elapsed.TotalNanoseconds;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(suffixArray_Scanner.topPattern[i], suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var topFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(suffixArray_Scanner.midPatterns[i], suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var midFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(suffixArray_Scanner.botPattern[i],  suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var bottomFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    var d = textName.Split('_');

                    string s = string.Format("{0},{1},{2},{3},{4},{5}\n",
                        d[0], d[1], (long)constructionTime, (long)topFixedQueryTime, (long)midFixedQueryTime, (long)bottomFixedQueryTime);
                    File.AppendAllText($"{resultsDir}\\{name}.csv", s);
                    Console.WriteLine(s);
                }

                foreach ((var name, var dataStructure) in variableCountingDataStructures)
                {
                    
                    stopwatch = Stopwatch.StartNew();
                    CountVariable countFixed = dataStructure.Invoke(suffixA, query.Y.Min, query.Y.Max);
                    stopwatch.Stop();
                    var constructionTime = stopwatch.Elapsed.TotalNanoseconds;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(suffixArray_Scanner.topPattern[i], suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var topFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(suffixArray_Scanner.midPatterns[i], suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var midFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(suffixArray_Scanner.botPattern[i], suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var bottomFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    var d = textName.Split('_');

                    string s = string.Format("{0},{1},{2},{3},{4},{5}\n",
                        d[0], d[1], (long)constructionTime, (long)topFixedQueryTime, (long)midFixedQueryTime, (long)bottomFixedQueryTime);
                    File.AppendAllText($"{resultsDir}\\{name}.csv", s);
                    Console.WriteLine(s);
                }

                foreach ((var name, var dataStructure) in fixedExistDataStructures)
                {
                    
                    stopwatch = Stopwatch.StartNew();
                    ExistFixed countFixed = dataStructure.Invoke(suffixA, x);
                    stopwatch.Stop();
                    var constructionTime = stopwatch.Elapsed.TotalNanoseconds;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(suffixArray_Scanner.topPattern[i], suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var topFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(suffixArray_Scanner.midPatterns[i], suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var midFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(suffixArray_Scanner.botPattern[i], suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var bottomFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    var d = textName.Split('_');

                    string s = string.Format("{0},{1},{2},{3},{4},{5}\n",
                        d[0], d[1], (long)constructionTime, (long)topFixedQueryTime, (long)midFixedQueryTime, (long)bottomFixedQueryTime);
                    File.AppendAllText($"{resultsDir}\\{name}.csv", s);
                    Console.WriteLine(s);
                }

                foreach ((var name, var dataStructure) in variableExistDataStructures)
                {
                    
                    stopwatch = Stopwatch.StartNew();
                    ExistVariable countFixed = dataStructure.Invoke(suffixA, query.Y.Min, query.Y.Max);
                    stopwatch.Stop();
                    var constructionTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(suffixArray_Scanner.topPattern[i], suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var topFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(suffixArray_Scanner.midPatterns[i], suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var midFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;

                    stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < reps; i++)
                    {
                        countFixed.Matches(suffixArray_Scanner.botPattern[i], suffixArray_Scanner.topPattern.GetRandom());
                    }
                    stopwatch.Stop();
                    var bottomFixedQueryTime = stopwatch.Elapsed.TotalNanoseconds / reps;
                    
                    var d = textName.Split('_');

                    string s = string.Format("{0},{1},{2},{3},{4},{5}\n",
                        d[0], d[1], (long)constructionTime, (long)topFixedQueryTime, (long)midFixedQueryTime, (long)bottomFixedQueryTime);
                    File.AppendAllText($"{resultsDir}\\{name}.csv", s);
                    Console.WriteLine(s);
                }

                




            }
            table.Write();
        }

        private static IReportVariable Variable_ESA_BottomUp_Build(SuffixArrayFinal str)
        {
            return new Variable_Report_BottomUp_Hash(str);
        }

        private static IReportSinglePattern BuildEnhancedSuffixArray(string str)
        {
            return new WrapESA(str);
        }

        private static IReportSinglePattern BuildSuffixArray(string str)
        {
            return new WrapSuffixArray(str);
        }

        private static IReportSinglePattern BuildSuffixTree(string str)
        {
            return new WrapSuffixTree(str);
        }

        private static IReportSinglePattern BuildPrecomputedSubstrings(string str)
        {
            return new WrapPrecomp(str);
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
        public delegate IReportSinglePattern BuildSinglePatternReporting(string str);
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

