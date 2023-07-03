using ConsoleApp.Data.Obsolete;
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
    public class Fixed_PartialHash : ReportFixed
    {
        private Dictionary<(int, int), IntervalNode> Tree;
        private Dictionary<(int, int), HashSet<int>> SortedTree;

        public Fixed_PartialHash(SuffixArrayFinal str) : base(str)
        {
            BuildDataStructure();
        }
        public Fixed_PartialHash(string str) : base(str)
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
                SortedTree.Add(i, new HashSet<int>(SA.GetOccurrencesForInterval(i)));
            }
        }

        public override IEnumerable<int> Matches(string pattern1, int x, string pattern2)
        {
            List<int> occs = new();
            var occs2 = ReportHashedOccurrences(pattern2);
            foreach (var occ1 in SA.SinglePattern(pattern1))
            {
                if (occs2.Contains(occ1 + pattern1.Length + x))
                    occs.Add(occ1);
            }
            return occs;
        }

        public override HashSet<int> ReportHashedOccurrences(string pattern)
        {
            var interval = SA.ExactStringMatchingWithESA(pattern);
            if (SortedTree.ContainsKey(interval)) return SortedTree[interval];
            else return new HashSet<int>(SA.GetOccurrencesForInterval(interval));
        }
    }
}
