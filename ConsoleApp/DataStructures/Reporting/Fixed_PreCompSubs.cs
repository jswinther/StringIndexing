using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp.DataStructures.Reporting
{
    public class Fixed_PreCompSubs : IReportFixed
    {
        Dictionary<string, HashSet<int>> Substrings = new();
        public Fixed_PreCompSubs(string str)
        {
            for (int i = 1; i <= str.Length; i++)
            {
                for (int j = 0; j <= str.Length - i; j++)
                {
                    var s = str.Substring(j, i);
                    if (!Substrings.ContainsKey(s))
                    {
                        Substrings[s] = new HashSet<int>();
                    }
                    Substrings[s].Add(j);
                }
            }
        }

        public IEnumerable<int> Matches(string pattern1, int x, string pattern2)
        {
            List<int> occs = new();
            var occs1 = ReportHashedOccurrences(pattern1);
            var occs2 = ReportHashedOccurrences(pattern2);
            if (occs1.Count < occs2.Count)
            {
                foreach (var p1occ in occs1)
                {
                    if (occs2.Contains(p1occ + pattern1.Length + x))
                    {
                        occs.Add(p1occ);
                    }
                }
            }
            else
            {
                foreach (var p2occ in occs2)
                {
                    if (occs1.Contains(p2occ - pattern1.Length - x))
                    {
                        occs.Add(p2occ);
                    }
                }
            }
            return occs;
        }

        public HashSet<int> ReportHashedOccurrences(string pattern)
        {
            if (Substrings.ContainsKey(pattern)) return Substrings[pattern];
            else return new HashSet<int>();
        }
    }
}
