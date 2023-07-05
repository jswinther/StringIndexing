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
            var data = new string[] {
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
            foreach (var set in data)
            {

                var fileName = $"{Helper.TryGetSolutionDirectoryInfo()}\\jsonFiles\\{set}.json";
                SuffixArrayFinal saf = null;
                saf = SuffixArrayFinal.CreateSuffixArray(DummyData.Read(set));

                File.WriteAllText(fileName, "{");
                AddField(fileName, "n", saf.n);
                AddField(fileName, "m_sa", saf.m_sa);
                AddField(fileName, "m_isa", saf.m_isa);
                AddField(fileName, "m_lcp", saf.m_lcp);
                AddField(fileName, "m_str", saf.m_str);
                /*
                AddField(fileName, "m_ct_up", saf.m_ct_up);
                AddField(fileName, "m_ct_down", saf.m_ct_down);
                AddField(fileName, "m_ct_next", saf.m_ct_next);
                */
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