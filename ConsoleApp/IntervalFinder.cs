using ConsoleApp.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class IntervalFinder
    {
        

        public IntervalFinder(string pattern1, string s)
        {
            // Check if pattern and string has already been searched
            var workdir = Assembly.GetAssembly(typeof(Program)).Location;
            var dir = Directory.GetParent(workdir).Parent.Parent.Parent.FullName;
            var file = $"{dir}\\Data\\DNA_{s.Length - 1}_{pattern1}.txt";
            if (File.Exists(file)) 
            {
                var lines = File.ReadAllLines(file);
                start = int.Parse(lines[0]);
                end = int.Parse(lines[1]);
            }
            else 
            {
                var sa = new SuffixArray_V1(s);
                (start, end) = sa.GetInterval(pattern1);
                using (var sw = new StreamWriter(file))
                {
                    sw.WriteLine(start == -1 ? 0 : start);
                    sw.WriteLine(end);
                }
            }
        }

        public (int start, int end) GetInterval()
        {
            return (start, end);
        }
        private int start;
        private int end;
        
    }
}
