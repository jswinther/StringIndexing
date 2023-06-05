using ConsoleApp;
using ConsoleApp.DataStructures;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace BuildDataStructures
{
    internal class Program
    {
        
        static void Main(string[] args)
        {

            CreateJSONFiles();
        }

        private static void CreateJSONFiles()
        {
            foreach (var set in DummyData.GetData(new DS[] { DS._512, DS._8192, DS._16384, DS._262144, DS._524288, DS._1048576, DS._2097152, DS._4194304, DS._8388608, DS._16777216, DS._33554432 }))
            {

                var name = set.Item1;
                var fileName = $"{ConsoleApp.Helper.TryGetSolutionDirectoryInfo()}\\{name}.json";
                var data = set.Item2.Invoke(name);
                SuffixArrayFinal saf = null;
                long elapsed = 0;
                for (int i = 0; i < 10; i++)
                {
                    var sw = Stopwatch.StartNew();
                    saf = SuffixArrayFinal.CreateSuffixArray(data);
                    sw.Stop();
                    elapsed += sw.ElapsedMilliseconds;
                }
                Console.WriteLine("Written " + name + $" in time {elapsed / 10}");
                File.AppendAllText("constructionTimes.txt", $"{name}, {elapsed / 10}\n");

                File.WriteAllText(fileName, "{");
                AddField(fileName, "n", saf.n);
                AddField(fileName, "m_sa", saf.m_sa);
                AddField(fileName, "m_isa", saf.m_isa);
                AddField(fileName, "m_lcp", saf.m_lcp);
                AddField(fileName, "m_str", saf.m_str);
                AddField(fileName, "m_ct_up", saf.m_ct_up);
                AddField(fileName, "m_ct_down", saf.m_ct_down);
                AddField(fileName, "m_ct_next", saf.m_ct_next);
                AddField(fileName, "m_chainHeadsDict", saf.m_chainHeadsDict);
                AddField(fileName, "m_chainStack", saf.m_chainStack);
                AddField(fileName, "m_subChains", saf.m_subChains);
                File.AppendAllText(fileName, "\"m_nextRank\":" + saf.m_nextRank + "}");



            }


            
        }

        private static void AddField(string filename, string fieldName, object fieldValue)
        {
            File.AppendAllText(filename, $"\"{fieldName}\": {JsonSerializer.Serialize(fieldValue)},");
        }
    }
}