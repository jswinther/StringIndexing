using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Existence
{
    // Bruteforce
    internal class SA_E_V3 : ExistDataStructure
    {
        int x;
        SuffixArrayFinal SA;
        Dictionary<(int, int), IntervalNode> Tree;
        Dictionary<(int, int), IntervalNode> Leaves;
        private IntervalNode Root;
        Dictionary<(int, int), HashSet<(int, int)>> Exists = new Dictionary<(int, int), HashSet<(int, int)>>();
        Dictionary<(int, int), HashSet<int>> HashedNodes = new Dictionary<(int, int), HashSet<int>>(); 
        public SA_E_V3(string str, int fixedGap, int minGap, int maxGap) : base(str, fixedGap, minGap, maxGap)
        {
            this.x = x;
            SA = new SuffixArrayFinal(str);

            SA.BuildChildTable();
            SA.GetAllLcpIntervals((int)Math.Sqrt(SA.n), out Tree, out Leaves, out Root);
            
            foreach (var item in Tree.OrderBy(s => s.Value.Size).Take((int)Math.Sqrt(SA.n)))
            {
                HashedNodes.Add(item.Key, new HashSet<int>(SA.GetOccurrencesForInterval(item.Key)));
            }

            
            foreach ((var key1, var occs1) in HashedNodes)
            {
                var int1 = Tree[key1];
                Exists.Add(int1.Interval, new HashSet<(int, int)>());
                foreach ((var int2, var occs2) in HashedNodes)
                {
                    if (occs1.Any(occ1 => occs2.Contains(occ1 + int1.DistanceToRoot + x)))
                    {
                        Exists[key1].Add(int2);
                    }
                }
            }
        }

        public bool IsIntervalSizeLessThan((int, int) interval, out HashSet<int> occs)
        {
            var b = interval.Item2 - interval.Item1 < (int)Math.Sqrt(SA.n);
            if (b)
            {
                occs = new HashSet<int>(SA.GetOccurrencesForInterval(interval));
            }
            else
            {
                occs = new HashSet<int>();
            }
            return b;
        }

        public override bool Matches(string pattern)
        {
            (var a, var b) = SA.ExactStringMatchingWithESA(pattern);
            return ((a, b) != (-1, -1));
        }

        public override bool MatchesFixedGap(string p1, string p2)
        {
            var pattern1Interval = SA.ExactStringMatchingWithESA(p1);
            var pattern2Interval = SA.ExactStringMatchingWithESA(p2);
            HashSet<int> n2hash = new();
            HashSet<int> n1hash = new();
            if (IsIntervalSizeLessThan(pattern1Interval, out n1hash))
            {
                IsIntervalSizeLessThan(pattern2Interval, out n2hash);
                if (HashedNodes.TryGetValue(pattern1Interval, out var n1))
                {
                    n2hash.Any(s2 => n1.Contains(s2 - x - p1.Length));
                }
                else if (HashedNodes.TryGetValue(pattern2Interval, out var n2))
                {
                    n1hash.Any(s1 => n2.Contains(s1 + x + p1.Length));
                }
                else
                {
                    n1hash.Any(s1 => n2hash.Contains(s1 + x + p1.Length));
                }
            }
            if (Exists.TryGetValue(pattern1Interval, out var result))
            {
                return result.Contains(pattern2Interval);
            }
            else
            {
                var occs2 = new HashSet<int>(SA.GetOccurrencesForInterval(pattern2Interval));
                return SA.GetOccurrencesForInterval(pattern1Interval).Any(occ1 => occs2.Contains(occ1 + x + p1.Length));
            }
        }

        public override bool MatchesVariableGap(string pattern1, string pattern2)
        {
            throw new NotImplementedException();
        }

     
    }

}
