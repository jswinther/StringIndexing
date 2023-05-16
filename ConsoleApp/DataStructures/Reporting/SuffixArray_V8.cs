using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    internal class SuffixArray_V8
    {
        private SuffixArrayFinal SA;

        private Dictionary<(int, int), IntervalNode> Tree;
        private Dictionary<(int, int), IntervalNode> Leaves;
        private IntervalNode Root;

        private Dictionary<(int, int), Dictionary<(int, int), Dictionary<int, int>>> HashedTree = new();
        
        public double MinSize { get; set; }
        public double MaxSize { get; set; }
        public SuffixArray_V8(string str)
        {
            SA = new SuffixArrayFinal(str);
            MinSize = Math.Pow(SA.n, 0.50);
            //MaxSize = Math.Pow(SA.n, 0.66);
            SA.BuildChildTable();
            SA.GetAllLcpIntervals((int)MinSize, out Tree, out Leaves, out Root);
            List<(IntervalNode, System.Collections.Generic.HashSet<int>)> intermediate = new();
            foreach (var intervalNode in Tree.Values.Skip(1))
            {
                var h = new System.Collections.Generic.HashSet<int>(SA.GetOccurrencesForInterval(intervalNode.Interval));
                intermediate.Add((intervalNode, h));
            }

            foreach ((var interval1, var hashset1) in intermediate)
            {
                HashedTree.Add(interval1.Interval, new());
                foreach ((var interval2, var hashset2) in intermediate)
                {
                    HashedTree[interval1.Interval].Add(interval2.Interval, new());
                    var reference = HashedTree[interval1.Interval][interval2.Interval];
                    foreach (int occ1 in hashset1)
                    {
                        foreach (var occ2 in hashset2.Select(s => s - occ1).Where(o2 => o2 > 0 && o2 < SA.n))
                        {
                            reference.TryAdd(occ2 - occ1, 0);
                            reference[occ2 - occ1]++;

                        }
                    }
                }
            }
        }

        public int Matches(string pattern)
        {
            var a = SA.ExactStringMatchingWithESA(pattern);
            return a.j + 1 - a.i;
        }

        public int Matches(string pattern1, int x, string pattern2)
        {
            var occs = HashedTree[SA.ExactStringMatchingWithESA(pattern1)][SA.ExactStringMatchingWithESA(pattern2)][x];
            return occs;
        }

        public int Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            var occs = HashedTree[SA.ExactStringMatchingWithESA(pattern1)][SA.ExactStringMatchingWithESA(pattern2)];
            return occs.Where(key => y_min <= key.Key && key.Key <= y_max).Select(key => key.Value).Sum();
        }
    }
}
