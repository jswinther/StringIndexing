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

      
    }
}
