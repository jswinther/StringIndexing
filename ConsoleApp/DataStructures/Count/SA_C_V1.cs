using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Count
{
    internal class SA_C_V1 : CountDataStructure
    {
        private SuffixArrayFinal SA;

        private Dictionary<(int, int), IntervalNode> Tree;
        private Dictionary<(int, int), IntervalNode> Leaves;
        private IntervalNode Root;

        private Dictionary<(int, int), Dictionary<(int, int), Dictionary<int, int>>> HashedTree = new();

        public double MinSize { get; set; }
        public double MaxSize { get; set; }
        int x;
        int ymin;
        int ymax;
        public SA_C_V1(string str, int x, int ymin, int ymax) : base(str, x, ymin, ymax)
        {
            SA = new SuffixArrayFinal(str);
            this.x = x;
            this.ymin = ymin;
            this.ymax = ymax;
            MinSize = Math.Pow(SA.n, 0.50);
            //MaxSize = Math.Pow(SA.n, 0.66);
            SA.BuildChildTable();
            SA.GetAllLcpIntervals((int)MinSize, out Tree, out Leaves, out Root);
            List<(IntervalNode, HashSet<int>)> intermediate = new();
            foreach (var intervalNode in Tree.Values.Skip(1))
            {
                var h = new HashSet<int>(SA.GetOccurrencesForInterval(intervalNode.Interval));
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

        public override int Matches(string pattern)
        {
            var a = SA.ExactStringMatchingWithESA(pattern);
            return a.j + 1 - a.i;
        }

        public override int MatchesFixed(string pattern1, string pattern2)
        {
            var occs = HashedTree[SA.ExactStringMatchingWithESA(pattern1)][SA.ExactStringMatchingWithESA(pattern2)][x];
            return occs;
        }

        public override int MatchesVariable(string pattern1, string pattern2)
        {
            var occs = HashedTree[SA.ExactStringMatchingWithESA(pattern1)][SA.ExactStringMatchingWithESA(pattern2)];
            return occs.Where(key => ymin <= key.Key && key.Key <= ymax).Select(key => key.Value).Sum();
        }
    }
}
