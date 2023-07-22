using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Single
{
    public class WrapPrecomp : IReportSinglePattern
    {
        public Dictionary<string, LinkedList<int>> D = new Dictionary<string, LinkedList<int>>();
        public WrapPrecomp(string text) 
        {
            text += "|";
            for (int i = 1; i <= text.Length; i++)
            {
                for (int j = 0; j <= text.Length - i; j++)
                {
                    var s = text.Substring(j, i);
                    if (!D.ContainsKey(s)) D[s] = new LinkedList<int>();
                    D[s].AddLast(j);
                }
            }
        }

        public IEnumerable<int> SinglePatternMatching(string pattern, out double mt, out double rt)
        {
            var sw = Stopwatch.StartNew();
            var t = D.ContainsKey(pattern);
            sw.Stop();
            mt = sw.Elapsed.TotalNanoseconds;
            sw = Stopwatch.StartNew();
            D.TryGetValue(pattern, out var occs);
            sw.Stop();
            rt = sw.Elapsed.TotalNanoseconds;
            return occs;
        }
    }
}
