using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Existence
{
    // Bruteforce
    internal class SA_E_V4
    {
        int x;
        SuffixArrayFinal SA;
        Dictionary<(int, int), IntervalNode> Tree;
        Dictionary<(int, int), IntervalNode> Leaves;
        Dictionary<(int, int), HashSet<(int, int)>> Exists = new Dictionary<(int, int), HashSet<(int, int)>>();
        public SA_E_V4(string str, int x) 
        {
            this.x = x;
            SA = new SuffixArrayFinal(str);

            SA.BuildChildTable();
            SA.GetAllLcpIntervals((int)Math.Sqrt(SA.n), out Tree, out Leaves);

            var hashedNodes = Tree.Select(n => new { interval = n.Value, occs = new HashSet<int>(SA.GetOccurrencesForInterval(n.Key))});
            foreach (var item in hashedNodes)
            {
                var int1 = item.interval;
                var occs1 = item.occs;
                Exists.Add(int1.Interval, new HashSet<(int, int)>());
                foreach (var item1 in hashedNodes)
                {
                    var int2 = item1.interval;
                    var occs2 = item1.occs;
                    if (occs1.Any(occ1 => occs2.Contains(occ1 + int1.DistanceToRoot + x)))
                    {
                        Exists[int1.Interval].Add(int2.Interval);
                    }
                }
            }
        }

        public bool PatternExists(string p1, string p2)
        {
            var pattern1Interval = SA.ExactStringMatchingWithESA(p1);
            var pattern2Interval = SA.ExactStringMatchingWithESA(p2);
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
    }
}
