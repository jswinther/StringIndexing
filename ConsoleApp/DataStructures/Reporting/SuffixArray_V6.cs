using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp.DataStructures.Reporting.SuffixArray_V2;
using static ConsoleApp.DataStructures.SuffixArrayFinal;

namespace ConsoleApp.DataStructures.Reporting
{
    internal class SuffixArray_V6 : PatternMatcher
    {
        SuffixArrayFinal SA;
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

        public SuffixArray_V6(string str) : base(str)
        {
            SA = new SuffixArrayFinal(str);
            // Populates _nodes and _leaves
            MinIntervalSize = (int)Math.Floor(Math.Sqrt(SA.n));
            MaxIntervalSize = (int)Math.Floor(Math.Pow(SA.n,(0.667)));
            SA.BuildChildTable();
            SA.GetAllLcpIntervals(MinIntervalSize, out Tree, out Leaves1, out Root);
            Leaves = Leaves1.Keys.ToArray();
            //UpdateDeepestLeaf();

            BuildDataStructure();
    

            

        }

        private void BuildDataStructure()
        {
            SortedTree = new();
            //Height = Tree.Last().Value.DistanceToRoot/2;
            Nodes = Tree.Values.ToArray();
           

            
           
            foreach (var intervalToBeSorted in Nodes.Where(n => n.Size <= MaxIntervalSize))
            {
                var occs = SA.GetOccurrencesForInterval(intervalToBeSorted.Interval);
                occs.Sort();
                SortedTree.Add(intervalToBeSorted.Interval, occs);
            }
            /*HashSet<IntervalNode> parents = new();
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
            }*/
            
        }

        private int[] UnsortedOccurrencesForPattern(string pattern)
        {
            return SA.GetOccurrencesForPattern(pattern);
        }

        private int[] SortedOccurrencesForPattern(string pattern)
        {
            var interval = SA.ExactStringMatchingWithESA(pattern);
            var intervalSize = (interval.j + 1 - interval.i);
            if (SortedTree.ContainsKey(interval)) return SortedTree[interval];
            else if (intervalSize < MinIntervalSize)
            {
                var occs = SA.GetOccurrencesForInterval(interval);
                Array.Sort(occs);
                return occs;
            }
            else
            {
                var occs = SA.GetOccurrencesForInterval(interval);
                occs.Sort();
                return occs;
            }
        }

        #region Pattern Matching
        public override IEnumerable<int> Matches(string pattern)
        {
            return SortedOccurrencesForPattern(pattern);
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            List<(int, int)> occs = new List<(int, int)>();
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
            var occs2 = new HashSet<int>(SA.GetOccurrencesForPattern(pattern2));

            foreach (var occ1 in occs1)
            {
                if (occs2.Contains(occ1 + pattern1.Length + x))
                    occs.Add((occ1, occ1 + pattern2.Length + pattern2.Length + x));
            }
            return occs;
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            List<(int, int)> occs = new();
            var occs1 = UnsortedOccurrencesForPattern(pattern1);
            var occs2 = SortedOccurrencesForPattern(pattern2);
            foreach (var occ1 in occs1)
            {
                int min = occ1 + y_min + pattern1.Length;
                int max = occ1 + y_max + pattern1.Length;
                foreach (var occ2 in occs2.GetViewBetween(min, max))
                {
                    occs.Add((occ1, occ2 - occ1 + pattern2.Length));
                }
            }
            return occs;
        }
        #endregion

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
            List<IntervalNode> nodes = new List<IntervalNode> ();
            
            Queue<IntervalNode> intervalNodes = new Queue<IntervalNode>();
            var root = Tree.Values.First();
            foreach (var node in root.Children) intervalNodes.Enqueue(node);
            while (intervalNodes.Count > 0)
            {
                IntervalNode node = intervalNodes.Dequeue();
                if (node.DistanceToRoot == 1 && node.IsLeaf) nodes.Add(node);
                if (node.DistanceToRoot > 0.25 * node.DeepestLeaf) nodes.Add(node);
                else {
                    foreach (var child in node.Children)
                    {
                        intervalNodes.Enqueue(child);
                    }
                }
            }

            var non = nodes.OrderBy(s => s.Interval.start).Select(s => s.Interval);
            TopNodes = nodes.Select(s => s.Interval).ToList();
            var sum = nodes.Sum(s => s.Size);
        }


      




        #region Query Methods


     



        public int[] GetSortedLeavesForInterval((int, int) interval)
        {
            if (!SortedTree.ContainsKey(interval))
            {
                var occs = SA.GetOccurrencesForInterval(interval);
                occs.Sort();
                return occs;
            }
            var node = Tree[interval];
            if (node.IsLeaf) return node.SortedOccurrences;
            var childIntervals = Leaves.Take(new Range(node.LeftMostLeaf, node.RightMostLeaf + 1)).ToList();
            var arrayOfSortedLeafOccurrences = childIntervals.Select(ci => SortedTree[ci]).ToArray();
            int[] sortedLeaves = MergeKSortedArrays(arrayOfSortedLeafOccurrences);
            var nonSortedIntervals = FindNonSortedIntervals(childIntervals, interval);
            var occurrencesOfNonSortedIntervalsSorted = nonSortedIntervals
                .SelectMany(SA.GetOccurrencesForInterval)
                .ToArray();

            occurrencesOfNonSortedIntervalsSorted.Sort();

            IEnumerable<int> sortedOccurrences = SortTwoSortedArrays(sortedLeaves, occurrencesOfNonSortedIntervalsSorted);
            return sortedOccurrences.ToArray();
        }

        private IEnumerable<int> SortTwoSortedArrays(int[] A, int[] B)
        {
            List<int> sorted = new(A.Length + B.Length);
            int i = 0, j = 0, n = A.Length, m = B.Length;
            while (i < n || j < m)
            {
                if (i >= n)
                {
                    sorted.Add(B[j]);
                    j++;
                }
                else if (j >= m)
                {
                    sorted.Add(A[i]);
                    i++;
                }
                else
                {
                    if (A[i] < B[j])
                    {
                        sorted.Add(A[i]);
                        i++;
                    }
                    else
                    {
                        sorted.Add(B[j]);
                        j++;
                    }
                }
            }
            return sorted;

        }

        private int[] MergeKSortedArrays(int[][] arrayOfSortedLeafOccurrences)
        {

            //return arrayOfSortedLeafOccurrences.SelectMany(s => s).OrderBy(key => key).ToArray();
            return Program.KWayMerge(arrayOfSortedLeafOccurrences);

        }

        public List<(int, int)> FindNonSortedIntervals(List<(int, int)> preSortedLeafNodes, (int, int) interval)
        {
            (int min, int max) = interval;
            List<(int, int)> nonSortedIntervals = new();
            // From min in the original interval to the smallest interval start.
            (int, int) firstInterval = (min, preSortedLeafNodes[0].Item1 - 1);
            nonSortedIntervals.Add(firstInterval);
            for (int i = 1; i < preSortedLeafNodes.Count; i++)
            {
                if (preSortedLeafNodes[i].Item1 - preSortedLeafNodes[i - 1].Item2 > 1)
                {
                    (int, int) tempInterval = (preSortedLeafNodes[i - 1].Item2 + 1, preSortedLeafNodes[i].Item1 - 1);
                    nonSortedIntervals.Add(tempInterval);
                }
            }
            (int, int) lastInterval = (preSortedLeafNodes[preSortedLeafNodes.Count - 1].Item2 + 1, max);
            nonSortedIntervals.Add(lastInterval);

            return nonSortedIntervals;
        }








        #endregion




    }


}
