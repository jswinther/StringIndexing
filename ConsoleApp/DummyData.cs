using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp.Program;

namespace ConsoleApp
{
    public class DummyData
    {
        public static IEnumerable<(string, GetData)> GetData(DS[] dataSets)
        {
            List<(string, GetData)> data = new List<(string, GetData)> ();
            foreach (var item in dataSets)
            {
                switch (item) 
                {
                    case DS._512:
                        data.Add(("DNA_512", DNA));
                        break;
                    case DS._8192:
                        data.Add(("DNA_8192", DNA));
                        break;
                    case DS._16384:
                        data.Add(("DNA_16384", DNA));
                        break;
                    case DS._262144:
                        data.Add(("DNA_262144", DNA));
                        break;
                    case DS._524288:
                        data.Add(("DNA_524288", DNA));
                        break;
                    case DS._1048576:
                        data.Add(("DNA_1048576", DNA));
                        data.Add(("proteins_1048576", PCC));
                        data.Add(("realDNA_1048576", PCC));
                        data.Add(("english_1048576", ENG));
                        break;
                    case DS._2097152:
                        data.Add(("DNA_2097152", DNA));
                        data.Add(("proteins_2097152", PCC));
                        data.Add(("realDNA_2097152", PCC));
                        data.Add(("english_2097152", ENG));
                        break;
                    case DS._4194304:
                        data.Add(("DNA_4194304", DNA));
                        data.Add(("proteins_4194304", PCC));
                        data.Add(("realDNA_4194304", PCC));
                        data.Add(("english_4194304", ENG));
                        break;
                    case DS._8388608:
                        data.Add(("DNA_8388608", DNA));
                        data.Add(("proteins_8388608", PCC));
                        data.Add(("realDNA_8388608", PCC));
                        data.Add(("english_8388608", ENG));
                        break;
                    case DS._16777216:
                        data.Add(("DNA_16777216", DNA));
                        data.Add(("proteins_16777216", PCC));
                        data.Add(("realDNA_16777216", PCC));
                        data.Add(("english_16777216", ENG));
                        break;
                    case DS._33554432:
                        data.Add(("DNA_33554432", DNA));
                        data.Add(("proteins_33554432", PCC));
                        data.Add(("realDNA_33554432", PCC));
                        data.Add(("english_33554432", ENG));
                        break;
                }
            }
            return data;
        }









        public static readonly string Dummy = "banana";
        
      

        public static string DNA(string filename)
        {
            var workdir = Assembly.GetAssembly(typeof(Program)).Location; ;
            var dir = Directory.GetParent(workdir).Parent.Parent.Parent.FullName;
            var file = $"{dir}\\Data\\{filename}.txt";
            var lines = string.Concat(File.ReadAllLines(file).Skip(2));
            return lines;
        }

        public static string PCC(string filename)
        {
            var workdir = Assembly.GetAssembly(typeof(Program)).Location; ;
            var dir = Directory.GetParent(workdir).Parent.Parent.Parent.FullName;
            var file = $"{dir}\\Data\\{filename}.txt";
            var lines = string.Concat(File.ReadAllLines(file)).ToLower();
            return lines;
        }

        public static string ENG(string filename)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var workdir = Assembly.GetAssembly(typeof(Program)).Location; ;
            var dir = Directory.GetParent(workdir).Parent.Parent.Parent.FullName;
            var file = $"{dir}\\Data\\{filename}.txt";
            var byteLines = System.Text.Encoding.GetEncoding(1252).GetBytes((string.Concat(File.ReadAllLines(file)).ToLower()));
            var utf8Text = System.Text.Encoding.UTF8.GetString(byteLines);
            return utf8Text;
        }

        


    }
}
