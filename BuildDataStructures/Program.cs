using ConsoleApp;
using ConsoleApp.DataStructures;
using System.Globalization;
using System.Text.Json;

namespace BuildDataStructures
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            foreach (var set in DummyData.GetData(new DS[] { DS._512, DS._8192, DS._16384, DS._262144, DS._524288, DS._1048576, DS._2097152, DS._4194304, DS._8388608, DS._16777216, DS._33554432}))
            {
                var name = set.Item1;
                var fileName = $"{name}.json";
                var data = set.Item2.Invoke(name);
                var saf = new SuffixArrayFinal(data);
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
                Console.WriteLine("Written " + name);
                /*
                var read = File.ReadAllText(fileName);
                var suffixArray = JsonSerializer.Deserialize<SuffixArrayFinal>(read);
                */
            } 
        }

        private static void AddField(string filename, string fieldName, object fieldValue)
        {
            File.AppendAllText(filename, $"\"{fieldName}\": {JsonSerializer.Serialize(fieldValue)},");
        }
    }
}