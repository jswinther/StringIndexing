using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class DummyData
    {
        public static IEnumerable<(string, string)> GetData(DS[] dataSets)
        {
            List<(string, string)> data = new List<(string, string)> ();
            foreach (var item in dataSets)
            {
                switch (item) 
                {
                    case DS._512:
                        data.Add(DNA("DNA_512"));
                        break;
                    case DS._8192:
                        data.Add(DNA("DNA_8192"));
                        break;
                    case DS._16384:
                        data.Add(DNA("DNA_16384"));
                        break;
                    case DS._262144:
                        data.Add(DNA("DNA_262144"));
                        break;
                    case DS._524288:
                        data.Add(DNA("DNA_524288"));
                        break;
                    case DS._1048576:
                        data.Add(DNA("DNA_1048576"));
                        data.Add(PCC("proteins_1048576"));
                        data.Add(PCC("realDNA_1048576"));
                        data.Add(ENG("english_1048576"));
                        break;
                    case DS._2097152:
                        data.Add(DNA("DNA_2097152"));
                        data.Add(PCC("proteins_2097152"));
                        data.Add(PCC("realDNA_2097152"));
                        data.Add(ENG("english_2097152"));
                        break;
                    case DS._4194304:
                        data.Add(DNA("DNA_4194304"));
                        data.Add(PCC("proteins_4194304"));
                        data.Add(PCC("realDNA_4194304"));
                        data.Add(ENG("english_4194304"));
                        break;
                    case DS._8388608:
                        data.Add(DNA("DNA_8388608"));
                        data.Add(PCC("proteins_8388608"));
                        data.Add(PCC("realDNA_8388608"));
                        data.Add(ENG("english_8388608"));
                        break;
                    case DS._16777216:
                        data.Add(DNA("DNA_16777216"));
                        data.Add(PCC("proteins_16777216"));
                        data.Add(PCC("realDNA_16777216"));
                        data.Add(ENG("english_16777216"));
                        break;
                    case DS._33554432:
                        data.Add(DNA("DNA_33554432"));
                        data.Add(PCC("proteins_33554432"));
                        data.Add(PCC("realDNA_33554432"));
                        data.Add(ENG("english_33554432"));
                        break;
                }
            }
            return data;
        }









        public static readonly string Dummy = "banana";
        
      

        public static (string, string) DNA(string filename)
        {
            var workdir = Assembly.GetAssembly(typeof(Program)).Location; ;
            var dir = Directory.GetParent(workdir).Parent.Parent.Parent.FullName;
            var file = $"{dir}\\Data\\{filename}.txt";
            var lines = string.Concat(File.ReadAllLines(file).Skip(2));
            return (filename, lines);
        }

        public static (string, string) PCC(string filename)
        {
            var workdir = Assembly.GetAssembly(typeof(Program)).Location; ;
            var dir = Directory.GetParent(workdir).Parent.Parent.Parent.FullName;
            var file = $"{dir}\\Data\\{filename}.txt";
            var lines = string.Concat(File.ReadAllLines(file)).ToLower();
            return (filename,lines);
        }

        public static (string, string) ENG(string filename)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var workdir = Assembly.GetAssembly(typeof(Program)).Location; ;
            var dir = Directory.GetParent(workdir).Parent.Parent.Parent.FullName;
            var file = $"{dir}\\Data\\{filename}.txt";
            var byteLines = System.Text.Encoding.GetEncoding(1252).GetBytes((string.Concat(File.ReadAllLines(file)).ToLower()));
            var utf8Text = System.Text.Encoding.UTF8.GetString(byteLines);
            return (filename, utf8Text);
        }

        


    }
}
