using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    /// <summary>
    /// Formerly known as V3
    /// </summary>
    internal class Variable_ESA_Sorted : ReportVariable
    {
        private Dictionary<(int, int), IntervalNode> Tree;
        private Dictionary<(int, int), IntervalNode> Leaves;
        private IntervalNode Root;
        private Dictionary<(int, int), int[]> Sorted = new();
        public Variable_ESA_Sorted(SuffixArrayFinal str) : base(str)
        {
            SA.GetAllLcpIntervals(1, out Tree, out Leaves, out Root);
            BuildDataStructure();
        }
        public Variable_ESA_Sorted(string str) : base(str)
        {
            SA.GetAllLcpIntervals(1, out Tree, out Leaves, out Root);
            BuildDataStructure();
        }
        private void BuildDataStructure()
        {
            var keys = Tree.Keys.ToList();

            for (int i = 1; i < keys.Count; i++)
            {
                var interval = keys[i];
                Sorted.Add(interval, SA.GetOccurrencesForInterval(interval).Sort());
            }
        }
        public override IEnumerable<int> Matches(string pattern1, int minGap, int maxGap, string pattern2)
        {
            List<int> occs = new();
            var occurrencesP1 = ReportSortedOccurrences(pattern1);
            var occurrencesP2 = ReportSortedOccurrences(pattern2);
            foreach (var occ1 in occurrencesP1)
            {
                int min = occ1 + minGap + pattern1.Length;
                int max = occ1 + maxGap + pattern1.Length;
                occs.AddRange(occurrencesP2.GetViewBetween(min, max));
            }
            return occs;
        
        }

        public override int[] ReportSortedOccurrences(string pattern)
        {
            var interval = SA.ExactStringMatchingWithESA(pattern);
            if (interval == (-1, -1)) return new int[] { };
            return Sorted[interval];
        }
    }
}
