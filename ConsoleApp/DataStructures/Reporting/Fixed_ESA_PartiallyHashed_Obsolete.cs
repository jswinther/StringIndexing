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
    /// Formerly known as V3
    /// </summary>
    internal class Fixed_ESA_PartiallyHashed_Obsolete : ReportFixed
    {
        private Dictionary<(int, int), IntervalNode> Tree;
        Dictionary<(int, int), IntervalNode> Leaves1;
        public (int, int)[] Leaves { get; private set; }
        public IntervalNode[] Nodes { get; private set; }
        public List<(int, int)> TopNodes { get; private set; }
        public int Height { get; set; }
        private IntervalNode Root;
        private Dictionary<(int, int), HashSet<int>> Hashed;
        public Fixed_ESA_PartiallyHashed_Obsolete(SuffixArrayFinal str) : base(str)
        {
            BuildDataStructure();
        }
        public Fixed_ESA_PartiallyHashed_Obsolete(string str) : base(str)
        {
            BuildDataStructure();
        }
        private void BuildDataStructure()
        {
            SA.GetAllLcpIntervals(1, out Tree, out Leaves1, out Root);
            Leaves = Leaves1.Keys.ToArray();
            UpdateDeepestLeaf();
            Hashed = new();
            Height = Tree.Last().Value.DistanceToRoot / 2;
            Nodes = Tree.Values.ToArray();
            foreach (var intervalToBeSorted in Nodes.Where(n => n.DistanceToRoot >= 0.25 * n.DeepestLeaf || n.DistanceToRoot == 1 && n.IsLeaf))
            {
                Hashed.Add(intervalToBeSorted.Interval, new HashSet<int>(SA.GetOccurrencesForInterval(intervalToBeSorted.Interval)));
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
        public override IEnumerable<int> Matches(string pattern1, int x, string pattern2)
        {
            List<int> occs = new();
            var occs1 = SA.SinglePattern(pattern1);
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
            HashSet<int> result = new HashSet<int>();
            var interval = SA.ExactStringMatchingWithESA(pattern);
            if (Hashed.ContainsKey(interval)) return Hashed[interval];
            if (Tree.ContainsKey(interval) && Tree[interval].LeftMostLeaf < int.MaxValue)
            {
                var intervalNode = Tree[interval];
                int start = intervalNode.LeftMostLeaf;
                int end = intervalNode.RightMostLeaf;

                for (int i = start; i < end + 1; i++)
                {
                    result.UnionWith(Hashed[TopNodes[i]]);
                }
                return result;
            }
            else
            {
                return new HashSet<int>(SA.GetOccurrencesForInterval(interval));
            }
        }
    }
}
