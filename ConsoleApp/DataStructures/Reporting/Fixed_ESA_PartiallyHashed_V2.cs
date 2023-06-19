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
    internal class Fixed_ESA_PartiallyHashed_V2 : ReportFixed
    {
        public Dictionary<(int, int), IntervalNode> Tree;
        public Dictionary<(int, int), HashSet<int>> SortedTree;
        Dictionary<(int, int), IntervalNode> Leaves1;
        public (int, int)[] Leaves { get; private set; }
        public IntervalNode[] Nodes { get; private set; }
        public List<(int, int)> TopNodes { get; private set; }
        public int Height { get; set; }

        public int MinIntervalSize { get; set; }
        public int MaxIntervalSize { get; set; }

        private IntervalNode Root;
        public Fixed_ESA_PartiallyHashed_V2(SuffixArrayFinal str) : base(str)
        {
            BuildDataStructure();
        }
        public Fixed_ESA_PartiallyHashed_V2(string str) : base(str)
        {
            BuildDataStructure();
        }
        private void BuildDataStructure()
        {
            MinIntervalSize = (int)Math.Floor(Math.Sqrt(SA.n.Value));
            MaxIntervalSize = (int)Math.Floor(Math.Pow(SA.n.Value, (0.667)));
            SA.GetAllLcpIntervals(MinIntervalSize, out Tree, out Leaves1, out Root);
            SortedTree = new();
            Nodes = Tree.Values.ToArray();
            foreach (var intervalToBeSorted in Nodes.Where(n => n.Size <= MaxIntervalSize))
            {
                SortedTree.Add(intervalToBeSorted.Interval, new HashSet<int>(SA.GetOccurrencesForInterval(intervalToBeSorted.Interval)));
            }
        }

        public override IEnumerable<int> Matches(string pattern1, int x, string pattern2)
        {
            List<int> occs = new();
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
            var occs2 = ReportHashedOccurrences(pattern2);
            foreach (var occ1 in occs1)
            {
                if (occs2.Contains(occ1 + pattern1.Length + x))
                    occs.Add(occ1);
            }
            return occs;
        }

        public override HashSet<int> ReportHashedOccurrences(string pattern)
        {
            var interval = SA.ExactStringMatchingWithESA(pattern);
            var intervalSize = (interval.j + 1 - interval.i);
            if (SortedTree.ContainsKey(interval)) return SortedTree[interval];
            else return new HashSet<int>(SA.GetOccurrencesForInterval(interval));
        }
    }
}
