using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    /// <summary>
    /// Formerly known as V4_2
    /// </summary>
    public class Variable_PartialSort : ReportVariable
    {
        private Dictionary<(int, int), IntervalNode> Tree;
        private Dictionary<(int, int), int[]> SortedTree; 
        public Variable_PartialSort(SuffixArrayFinal str) : base(str)
        {
            BuildDataStructure();
        }
        public Variable_PartialSort(string str) : base(str)
        {
            BuildDataStructure();
        }
        private void BuildDataStructure()
        {
            var minSize = Math.Sqrt(SA.n.Value);
            SA.GetAllLcpIntervals(minSize, out Tree, out var Leaves, out var Root);
            SortedTree = new();
            foreach (var i in Tree.Values.Select(node => node.Interval))
            {
                SortedTree.Add(i, SA.GetOccurrencesForInterval(i).Sort());
            }
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int minGap, int maxGap, string pattern2)
        {
            List<(int, int)> occs = new();
            var occs1 = SA.SinglePattern(pattern1);
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


        public override int[] ReportSortedOccurrences(string pattern)
        {
            var interval = SA.ExactStringMatchingWithESA(pattern);
            if (SortedTree.ContainsKey(interval)) return SortedTree[interval];
            else return SA.GetOccurrencesForInterval(interval).Sort();
        }
    }
}
