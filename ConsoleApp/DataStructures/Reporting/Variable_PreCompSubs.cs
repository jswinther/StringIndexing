using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    internal class Variable_PreCompSubs : IReportVariable
    {
        Dictionary<string, int[]> Substrings = new();
        public Variable_PreCompSubs(string str) 
        {
            Dictionary<string, List<int>> ss = new();
            for (int i = 1; i <= str.Length; i++)
            {
                for (int j = 0; j <= str.Length - i; j++)
                {
                    var s = str.Substring(j, i);
                    if (!Substrings.ContainsKey(s))
                    {
                        ss[s] = new List<int>();
                    }
                    ss[s].Add(j);
                }
            }
            foreach (var item in ss)
            {
                Substrings.Add(item.Key, item.Value.ToArray().Sort());
            }
        }

        public IEnumerable<(int, int)> Matches(string pattern1, int minGap, int maxGap, string pattern2)
        {
            List<(int, int)> occs = new();
            var occs1 = ReportSortedOccurrences(pattern1);
            var occs2 = ReportSortedOccurrences(pattern2);
            foreach (var occ1 in occs1)
            {
                int min = occ1 + minGap + pattern1.Length;
                int max = occ1 + maxGap + pattern1.Length;
                foreach (var occ2 in occs2.GetViewBetween(min, max))
                {
                    occs.Add((occ1, occ2 + pattern2.Length));
                }
            }
            return occs;
        }

        public int[] ReportSortedOccurrences(string pattern)
        {
            if (Substrings.ContainsKey(pattern)) return Substrings[pattern];
            else return new int[] { };
        }
    }
}
