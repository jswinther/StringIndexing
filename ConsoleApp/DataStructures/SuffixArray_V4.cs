﻿using System;
using System.Collections;
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
            int logn = (int)Math.Floor(Math.Sqrt(n));
            int minIntervalSize = logn;
            GetAllLcpIntervals(minIntervalSize);
           
        }

        #region Pattern Matching
        public override IEnumerable<int> Matches(string pattern)
        {
            return base.Matches(pattern);
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            return base.Matches(pattern1, x, pattern2);
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            var occs1 = Matches(pattern1);


            var occs2 = GetSortedLeavesForInterval(ExactStringMatchingWithESA(pattern2));
            return ReportOccurences(pattern1, y_min, y_max, pattern2, occs1, occs2);
        }
        #endregion




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
                        var originalPlacesOfSuffixes = GetOccurrencesForInterval(interval.Item1, interval.Item2);
                        Array.Sort(originalPlacesOfSuffixes);
                        currNode.SortedOccurrences = originalPlacesOfSuffixes;
                    }
                }
            }
        }




        #region Query Methods


        public int DistanceToRoot((int, int) interval)
        {
            int counter = 0;
            var tempInterval = interval;
            while (Tree.TryGetValue(tempInterval, out IntervalNode node))
            {
                // We are at root
                if (node.Parent == (-1, -1)) return counter;
                tempInterval = node.Parent;
                ++counter;
            }
            return -1;
        }

        public HashSet<(int, int)> SiblingIntervals((int, int) interval)
        {
            if (!Tree.TryGetValue(interval, out IntervalNode node)) return new();
            if (node.Parent == (-1, -1)) return new();
            var parentNode = Tree[node.Parent];
            return parentNode.Children;
        }

        public int[] GetSortedLeavesForInterval((int, int) interval)
        {


            var childIntervals = GetLeafNodesForInterval(interval);
            var arrayOfSortedLeafOccurrences = childIntervals.Select(ci => Tree[ci].SortedOccurrences).ToArray();
            int[] sortedLeaves = MergeKSortedArrays(arrayOfSortedLeafOccurrences);
            var nonSortedIntervals = FindNonSortedIntervals(childIntervals, interval);
            var occurrencesOfNonSortedIntervalsSorted = nonSortedIntervals
                .SelectMany(GetOccurrencesForInterval)
                .ToArray();

            Array.Sort(occurrencesOfNonSortedIntervalsSorted);

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
                    } else
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

        public List<(int, int)> GetLeafNodesForInterval((int, int) interval)
        {
            Queue<(int, int)> intervals = new();
            List<(int, int)> leaves = new();
            intervals.Enqueue(interval);
            while (intervals.TryDequeue(out (int, int) res))
            {
                var childIntervals = Tree[res].Children;
                foreach (var item in childIntervals)
                {
                    if (Tree[item].IsLeaf) leaves.Add(item);
                    else intervals.Enqueue(item);
                }
            }
            return leaves;
        }


        #endregion




    }

    
}
