using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    internal class Variable_Report_BottomUp_Hash : ReportVariable
    {
        private Dictionary<(int, int), int[]> sorted = new Dictionary<(int, int), int[]>();
        public Variable_Report_BottomUp_Hash(SuffixArrayFinal str) : base(str)
        {
            if (str.m_ct_down == null) str.BuildChildTable();
            str.GetAllLcpIntervals(Math.Log2((double)str.n), out var tree, out var leaves, out var root);
            var sqrtn = Math.Sqrt((double)str.n);
            foreach (var t in tree.Keys.Skip(1))
            {
                if (t.Size() < sqrtn)
                {
                    sorted[t] = str.GetOccurrencesForInterval(t).Sort();
                }
            }
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int minGap, int maxGap, string pattern2)
        {
            List<(int, int)> occs = new();
            var occurrencesP1 = SA.SinglePattern(pattern1);
            var occurrencesP2 = ReportSortedOccurrences(pattern2);
            foreach (var occ1 in occurrencesP1)
            {
                int min = occ1 + minGap + pattern1.Length;
                int max = occ1 + maxGap + pattern1.Length;
                foreach (var occ2 in occurrencesP2.GetViewBetween(min, max))
                {
                    occs.Add((occ1, occ2 + pattern2.Length));
                }
            }
            return occs;
        }

        public override int[] ReportSortedOccurrences(string pattern)
        {
            var interval = SA.ExactStringMatchingWithESA(pattern);
            if (sorted.ContainsKey(interval)) return sorted[interval];
            else return SA.GetOccurrencesForInterval(interval).Sort();
        }
    }
}
