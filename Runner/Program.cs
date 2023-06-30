using ConsoleApp;
using ConsoleApp.DataStructures;
using ConsoleApp.DataStructures.Reporting;
using System.Diagnostics;
using System.Text.Json;

namespace Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string ds = args[0];
            string dn = args[1];
            string p1 = args[2];
            int min = int.Parse(args[3]);
            int max = int.Parse(args[4]);
            string p2 = args[5];

            ReportFixed rf = null;
            ReportVariable rv = null;
            var sw = Stopwatch.StartNew();
            var suffixA = JsonSerializer.Deserialize<SuffixArrayFinal>(File.ReadAllText($"{Helper.TryGetSolutionDirectoryInfo()}\\{dn}.json"));
            switch (ds)
            {
                case "F_SA_Runtime":
                    rf = new Fixed_SA_Runtime(suffixA);
                    break;

                case "F_ESA_Runtime":
                    rf = new Fixed_ESA_Runtime(suffixA);
                    break;
                case "F_ESA_Hashed":
                    break;
                case "F_ESA_PH":
                    break;
                case "V_SA_Runtime":
                    break;
                case "V_ESA_Runtime":
                    break;
                case "V_ESA_Sorted":
                    break;
                case "V_ESA_PS1":
                    break;
                case "V_ESA_PS2":
                    break;


                default:
                    break;
            }

           
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            if (rf != null)
            {
                sw.Restart();
                rf.Matches(p1, min, p2);
                sw.Stop();
            }
            else
            {
                sw.Restart();
                rv.Matches(p1, min, max, p2);
                sw.Stop();
            }
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}