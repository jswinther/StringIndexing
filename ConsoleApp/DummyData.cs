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
