using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Existence
{
    public class SA_E_V4 : ExistDataStructure
    {
        int x;
        SuffixArrayFinal SA;
        Dictionary<(int, int), IntervalNode> Tree;
        Dictionary<(int, int), IntervalNode> Leaves;
        private IntervalNode Root;

        Dictionary<(int, int), List<HashSet<(int, int)>>> Nodes = new Dictionary<(int, int), List<HashSet<(int, int)>>>();

        public SA_E_V4(string str, int fixedGap, int minGap, int maxGap) : base(str, fixedGap, minGap, maxGap)
        {
            this.x = x;
            SA = new SuffixArrayFinal(str);
            SA.BuildChildTable();
            SA.GetAllLcpIntervals((int)Math.Sqrt(SA.n), out Tree, out Leaves, out Root);
            var hashed = Leaves.Values.Select(s => new { interval = s.Interval, occs = new HashSet<int>(SA.GetOccurrencesForInterval(s.Interval)) }).ToArray();

            foreach (var item in hashed)
            {
                var hs1 = item.occs;
                Nodes.Add(item.interval, new List<HashSet<(int, int)>>());
                var no = Nodes[item.interval];
                for (int i = 0; i < Tree[item.interval].DistanceToRoot + 1; i++)
                {
                    no.Add(new HashSet<(int, int)>());
                    foreach (var item1 in hashed)
                    {
                        var hs2 = item1.occs;
                        if (hs1.Any(o1 => hs2.Contains(o1 + x + i)))
                        {
                            no[i].Add(item1.interval);
                            var parent = Tree[item1.interval].Parent;
                            while (no[i].Add(parent.Interval))
                            {
                                parent = parent.Parent;
                                if (parent == null) break;
                            }
                        }
                    }                    
                }
            }
            ComputeLeafIntervals();

        }


        public void ComputeLeafIntervals()
        {
            var Leaves = this.Leaves.Keys.ToArray();
            for (int i = 0; i < Leaves.Length; i++)
            {
                (int, int) leafInterval = Leaves[i];
                var leaf = Tree[leafInterval];
                var parentInterval = leaf.Parent;
                while (Tree.ContainsKey(parentInterval.Interval))
                {
                    var parent = Tree[parentInterval.Interval];
                    if (parent.LeftMostLeaf > i) Tree[parentInterval.Interval].LeftMostLeaf = i;
                    if (parent.RightMostLeaf < i) Tree[parentInterval.Interval].RightMostLeaf = i;
                    if (parent.Parent == null) break;
                    parentInterval = parent.Parent;
                }
            }
        }

        public override bool Matches(string pattern)
        {
            throw new NotImplementedException();
        }

        public override bool MatchesFixedGap(string p1, string p2)
        {
            var pattern1Interval = SA.ExactStringMatchingWithESA(p1);
            var pattern2Interval = SA.ExactStringMatchingWithESA(p2);
            var val = Tree[pattern1Interval];
            foreach (var leaf in Leaves.Keys.Take(new Range(val.LeftMostLeaf, val.RightMostLeaf + 1)))
            {
                var n = Nodes[leaf];
                var hs = n[val.DistanceToRoot];
                if (hs.Contains(pattern2Interval))
                { return true; }
            }
            return false;
        }

        public override bool MatchesVariableGap(string pattern1, string pattern2)
        {
            throw new NotImplementedException();
        }

    


    }
}
