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
    /// Formerly known as V4_3
    /// </summary>
    public class Fixed_ESA_PartiallyHashed_V3 : ReportFixed
    {
        public Dictionary<(int, int), IntervalNode> Tree;
        public Dictionary<(int, int), HashSet<int>> SortedTree;
        Dictionary<(int, int), IntervalNode> Leaves1;
        public (int, int)[] Leaves { get; private set; }
        public IntervalNode[] Nodes { get; private set; }
        public List<(int, int)> TopNodes { get; private set; }
        public int Height { get; set; }
        private IntervalNode Root;
        public double MinSize { get; set; }
        public double MaxSize { get; set; }
        public Fixed_ESA_PartiallyHashed_V3(SuffixArrayFinal str) : base(str)
        {
            BuildDataStructure();
        }
        public Fixed_ESA_PartiallyHashed_V3(string str) : base(str)
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
                SortedTree.Add(intervalToBeSorted.Interval, new HashSet<int>(occs));
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
            if (SortedTree.ContainsKey(interval)) return SortedTree[interval];
            if (Tree.ContainsKey(interval) && Tree[interval].LeftMostLeaf < int.MaxValue)
            {
                var intervalNode = Tree[interval];
                int start = intervalNode.LeftMostLeaf;
                int end = intervalNode.RightMostLeaf;

                for (int i = start; i < end + 1; i++)
                {
                    result.UnionWith(SortedTree[TopNodes[i]]);
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
