using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp.DataStructures.SuffixArray_V2;

namespace ConsoleApp.DataStructures
{
    internal class SuffixArray_V4 : SuffixArray_V2
    {
        public Dictionary<(int, int), IntervalNode> Tree { get => _nodes; }
        public HashSet<(int, int)> Leaves { get => _leaves; }

        public SuffixArray_V4(string str) : base(str)
        {
            // Populates _nodes and _leaves
            int logn = (int)Math.Floor(Math.Log2(n));
            int minIntervalSize = logn;
            GetAllLcpIntervals(minIntervalSize);
            var leaf = Leaves.FirstOrDefault();
            var dist = Tree.DistanceToRoot(leaf);
            var siblings = Tree.SiblingIntervals(leaf);
            var parent = Tree[leaf].Parent;
            var grandParent = Tree[parent].Parent;
            var children = Tree.GetLeafNodesForInterval(grandParent);
            
        }

        public override IEnumerable<int> Matches(string pattern)
        {
            var _int = ExactStringMatchingWithESA(pattern);
            var occs = Tree.GetSortedLeavesForInterval(_int);


            return base.Matches(pattern);
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            return base.Matches(pattern1, x, pattern2);
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            return base.Matches(pattern1, y_min, y_max, pattern2);
        }


        public void GetAllLcpIntervals(int minSize)
        {
            HashSet<(int, int)> hashSet = new();
            Queue<(int, int)> intervals = new Queue<(int, int)>();
            intervals.Enqueue((0, n - 1));
            // First add child intervals for the interval [0..n]
            var Initinterval = intervals.Dequeue();
            hashSet.Add((Initinterval.Item1, Initinterval.Item2));
            _nodes.Add(Initinterval, new IntervalNode(Initinterval, (-1, -1)));
            var currNode = _nodes[Initinterval];
            foreach (var item in GetChildIntervalsInit(Initinterval.Item1, Initinterval.Item2))
            {
                if (item != (-1, -1) && item.Item2 - item.Item1 >= minSize - 1)
                {

                    if (!hashSet.Contains(item)) intervals.Enqueue(item);
                    hashSet.Add(item);
                    currNode.Children.Add(item);
                    _nodes.Add(item, new IntervalNode(item, currNode.Interval));
                }
            }
            while (intervals.Count > 0)
            {
                var interval = intervals.Dequeue();
                if (interval.Item1 == interval.Item2) hashSet.Add((interval.Item1, interval.Item2));
                else
                {
                    hashSet.Add(interval);
                    _nodes.TryGetValue(interval, out currNode);
                    foreach (var item in GetChildIntervals(interval.Item1, interval.Item2))
                    {
                        if (item != (-1, -1) && item.Item2 - item.Item1 >= minSize - 1)
                        {

                            if (!hashSet.Contains(item))
                            {
                                intervals.Enqueue(item);
                                currNode.Children.Add(item);
                                _nodes.Add(item, new IntervalNode(item, currNode.Interval));
                            }
                            hashSet.Add(item);

                        }
                    }

                    if (currNode.Children.Count == 0)
                    {
                        _leaves.Add(currNode.Interval);
                        var originalPlacesOfSuffixes = Sa.Take(new Range(new Index(interval.Item1 == -1 ? 0 : interval.Item1), new Index(interval.Item2 + 1)));
                        currNode.SortedOccurrences = new SortedSet<int>(originalPlacesOfSuffixes);
                    }
                }
            }
        }
    }

    internal static class ExtensionMethodsForSuffixArray_V4
    {
        public static int DistanceToRoot(this Dictionary<(int, int), IntervalNode> tree, (int, int) interval)
        {
            int counter = 0;
            var tempInterval = interval;
            while (tree.TryGetValue(tempInterval, out IntervalNode node))
            {
                // We are at root
                if (node.Parent == (-1, -1)) return counter;
                tempInterval = node.Parent;
                ++counter;
            }
            return -1;
        }

        public static HashSet<(int, int)> SiblingIntervals(this Dictionary<(int, int), IntervalNode> tree, (int, int) interval)
        {
            if (!tree.TryGetValue(interval, out IntervalNode node)) return new();
            if (node.Parent == (-1, -1)) return new();
            var parentNode = tree[node.Parent];
            return parentNode.Children;
        }

        public static SortedSet<int> GetSortedLeavesForInterval(this Dictionary<(int, int), IntervalNode> tree, (int, int) interval)
        {
            var childIntervals = GetLeafNodesForInterval(tree, interval);
            var mergedOccs = childIntervals.SelectMany(s => tree[s].SortedOccurrences);
            var sortedOccs = new SortedSet<int>(mergedOccs);


            return sortedOccs;
        }

        public static HashSet<(int, int)> GetLeafNodesForInterval(this Dictionary<(int, int), IntervalNode> tree, (int, int) interval)
        {
            Queue<(int, int)> intervals = new();
            HashSet<(int, int)> leaves = new();
            intervals.Enqueue(interval);
            while (intervals.TryDequeue(out (int, int) res))
            {
                var childIntervals = tree[res].Children;
                foreach (var item in childIntervals)
                {
                    if (tree[item].IsLeaf) leaves.Add(item);
                    else intervals.Enqueue(item);
                }
            }
            return leaves;
        }
    }
}
