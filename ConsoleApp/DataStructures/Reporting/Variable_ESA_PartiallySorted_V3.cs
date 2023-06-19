using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    /// <summary>
    /// Formerly known as V4_3
    /// </summary>
    internal class Variable_ESA_PartiallySorted_V3 : ReportVariable
    {
        public Dictionary<(int, int), IntervalNode> Tree;
        public Dictionary<(int, int), IntervalNode> SortedTree;
        Dictionary<(int, int), IntervalNode> Leaves1;
        public (int, int)[] Leaves { get; private set; }
        public IntervalNode[] Nodes { get; private set; }
        public List<(int, int)> TopNodes { get; private set; }
        public int Height { get; set; }
        private IntervalNode Root;
        public double MinSize { get; set; }
        public double MaxSize { get; set; }
        public Variable_ESA_PartiallySorted_V3(SuffixArrayFinal str) : base(str)
        {
            BuildDataStructure();
        }
        public Variable_ESA_PartiallySorted_V3(string str) : base(str)
        {
            BuildDataStructure();
        }
        private void BuildDataStructure()
        {
            MinSize = (int)Math.Floor(Math.Sqrt(SA.n.Value));
            MaxSize = Math.Pow(SA.n.Value, 0.67);


            SA.GetAllLcpIntervals((int)MinSize, out Tree, out Leaves1, out Root);
            Leaves = Leaves1.Keys.ToArray();
            SortedTree = new();
            Nodes = Tree.Values.Skip(1).ToArray();



            foreach (var intervalToBeSorted in Nodes.Where(n => n.Size <= MaxSize))
            {
                var occs = SA.GetOccurrencesForInterval(intervalToBeSorted.Interval);
                intervalToBeSorted.SortedOccurrences = occs;
                intervalToBeSorted.SortedOccurrences.Sort();
                SortedTree.Add(intervalToBeSorted.Interval, intervalToBeSorted);
            }
            Queue<IntervalNode> findTopNodes = new Queue<IntervalNode>();
            findTopNodes.Enqueue(Tree.Values.First());
            HashSet<(int, int)> top = new HashSet<(int, int)>();
            while (findTopNodes.Count > 0)
            {
                var n = findTopNodes.Dequeue();
                if (SortedTree.ContainsKey(n.Interval)) top.Add(n.Interval);
                else
                {
                    foreach (var item in n.Children)
                    {
                        findTopNodes.Enqueue(item);
                    }
                }
            }
            TopNodes = top.ToList();
            HashSet<IntervalNode> parents = new();
            for (int i = 0; i < TopNodes.Count; i++)
            {
                (int, int) leafInterval = TopNodes[i];
                var leaf = Tree[leafInterval];
                var parentInterval = leaf.Parent.Interval;
                while (Tree.ContainsKey(parentInterval))
                {
                    var parent = Tree[parentInterval];
                    parents.Add(parent);
                    if (parent.LeftMostLeaf > i) Tree[parentInterval].LeftMostLeaf = i;
                    if (parent.RightMostLeaf < i) Tree[parentInterval].RightMostLeaf = i;
                    if (parent.Parent == null) break;
                    parentInterval = parent.Parent.Interval;
                }
            }

        }

        public override IEnumerable<int> Matches(string pattern1, int minGap, int maxGap, string pattern2)
        {
            List<int> occs = new();
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
            var occs2 = ReportSortedOccurrences(pattern2);
            foreach (var occ1 in occs1)
            {
                int min = occ1 + minGap + pattern1.Length;
                int max = occ1 + maxGap + pattern1.Length;
                occs.AddRange(occs2.GetViewBetween(min, max));
            }
            return occs;
        }


        public override int[] ReportSortedOccurrences(string pattern)
        {
            var interval = SA.ExactStringMatchingWithESA(pattern);
            if (SortedTree.ContainsKey(interval)) return SortedTree[interval].SortedOccurrences;
            if (Tree.ContainsKey(interval) && Tree[interval].LeftMostLeaf < int.MaxValue)
            {

                var intervalNode = Tree[interval];
                int start = intervalNode.LeftMostLeaf;
                int end = intervalNode.RightMostLeaf;
                int[][] arr = new int[end - start + 1][];
                for (int i = start; i < end + 1; i++)
                {
                    arr[i - start] = SortedTree[TopNodes[i]].SortedOccurrences;
                }
                return Helper.KWayMerge(arr);
            }
            else
            {
                var occs = SA.GetOccurrencesForInterval(interval);
                occs.Sort();
                return occs;
            }
        }
    }
}
