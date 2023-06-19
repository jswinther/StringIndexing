using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    /// <summary>
    /// Formerly known as V3
    /// </summary>
    internal class Variable_ESA_PartiallySorted_V1 : ReportVariable
    {
        private Dictionary<(int, int), IntervalNode> Tree;
        Dictionary<(int, int), IntervalNode> Leaves1;
        public (int, int)[] Leaves { get; private set; }
        public IntervalNode[] Nodes { get; private set; }
        public List<(int, int)> TopNodes { get; private set; }
        public int Height { get; set; }
        private IntervalNode Root;
        private Dictionary<(int, int), int[]> Sorted;
        public Variable_ESA_PartiallySorted_V1(SuffixArrayFinal str) : base(str)
        {
            BuildDataStructure();
        }
        public Variable_ESA_PartiallySorted_V1(string str) : base(str)
        {
            BuildDataStructure();
        }
        private void BuildDataStructure()
        {
            SA.GetAllLcpIntervals(1, out Tree, out Leaves1, out Root);
            Leaves = Leaves1.Keys.ToArray();
            UpdateDeepestLeaf();
            Sorted = new();
            var Nodes = Tree.Values.ToArray();




            foreach (var intervalToBeSorted in Nodes.Where(n => n.DistanceToRoot >= 0.25 * n.DeepestLeaf || n.DistanceToRoot == 1 && n.IsLeaf))
            {
                var occs = SA.GetOccurrencesForInterval(intervalToBeSorted.Interval);
                occs.Sort();
                Sorted.Add(intervalToBeSorted.Interval, occs);
            }
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

        public void UpdateDeepestLeaf()
        {
            for (int i = 0; i < Leaves.Length; i++)
            {
                (int, int) leafInterval = Leaves[i];
                var leaf = Tree[leafInterval];
                var parentInterval = leaf.Parent;
                leaf.DeepestLeaf = leaf.DistanceToRoot;
                while (Tree.ContainsKey(parentInterval.Interval))
                {
                    var parent = Tree[parentInterval.Interval];
                    if (parent.DeepestLeaf < leaf.DistanceToRoot) parent.DeepestLeaf = leaf.DistanceToRoot;
                    if (parent.Parent == null) break;
                    parentInterval = parent.Parent;
                }
            }
            List<IntervalNode> nodes = new List<IntervalNode>();

            Queue<IntervalNode> intervalNodes = new Queue<IntervalNode>();
            foreach (var node in Root.Children) intervalNodes.Enqueue(node);
            while (intervalNodes.Count > 0)
            {
                IntervalNode node = intervalNodes.Dequeue();
                if (node.DistanceToRoot == 1 && node.IsLeaf) nodes.Add(node);
                if (node.DistanceToRoot > 0.25 * node.DeepestLeaf) nodes.Add(node);
                else
                {
                    foreach (var child in node.Children)
                    {
                        intervalNodes.Enqueue(child);
                    }
                }
            }
            TopNodes = nodes.Select(s => s.Interval).ToList();
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
            if (Sorted.ContainsKey(interval)) return Sorted[interval];
            if (Tree.ContainsKey(interval) && Tree[interval].LeftMostLeaf < int.MaxValue)
            {
                var intervalNode = Tree[interval];
                int start = intervalNode.LeftMostLeaf;
                int end = intervalNode.RightMostLeaf;
                int[][] arr = new int[end - start + 1][];
                for (int i = start; i < end + 1; i++)
                {
                    arr[i - start] = Sorted[TopNodes[i]];
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
