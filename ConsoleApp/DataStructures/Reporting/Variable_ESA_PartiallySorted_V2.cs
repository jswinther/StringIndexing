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
    internal class Variable_ESA_PartiallySorted_V2 : ReportVariable
    {
        public Dictionary<(int, int), IntervalNode> Tree;
        public Dictionary<(int, int), int[]> SortedTree;
        Dictionary<(int, int), IntervalNode> Leaves1;
        public (int, int)[] Leaves { get; private set; }
        public IntervalNode[] Nodes { get; private set; }
        public List<(int, int)> TopNodes { get; private set; }
        public int Height { get; set; }

        public int MinIntervalSize { get; set; }
        public int MaxIntervalSize { get; set; }

        private IntervalNode Root;
        public Variable_ESA_PartiallySorted_V2(SuffixArrayFinal str) : base(str)
        {
            BuildDataStructure();
        }
        public Variable_ESA_PartiallySorted_V2(string str) : base(str)
        {
            BuildDataStructure();
        }
        private void BuildDataStructure()
        {
            MinIntervalSize = (int)Math.Floor(Math.Sqrt(SA.n.Value));
            //MaxIntervalSize = (int)Math.Floor(Math.Pow(SA.n.Value, (0.667)));
            SA.GetAllLcpIntervals(MinIntervalSize, out Tree, out Leaves1, out Root);
            Leaves = Leaves1.Keys.ToArray();


            SortedTree = new();
            Nodes = Tree.Values.ToArray();
            foreach (var intervalToBeSorted in Nodes)
            {
                var occs = SA.GetOccurrencesForInterval(intervalToBeSorted.Interval);
                occs.Sort();
                SortedTree.Add(intervalToBeSorted.Interval, occs);
            }

        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int minGap, int maxGap, string pattern2)
        {
            List<(int, int)> occs = new();
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
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
            var intervalSize = (interval.j + 1 - interval.i);
            if (SortedTree.ContainsKey(interval)) return SortedTree[interval];
            else return SA.GetOccurrencesForInterval(interval).Sort();
        }
    }
}
