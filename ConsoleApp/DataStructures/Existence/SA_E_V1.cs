using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp.DataStructures.SuffixArrayFinal;

namespace ConsoleApp.DataStructures.Existence
{
    public class SA_E_V1 : ExistDataStructure
    {
        private SuffixArrayFinal SA;
        private Dictionary<(int, int), IntervalNode> Tree;
        private Dictionary<(int, int), IntervalNode> Leaves;
        private IntervalNode Root;
        private int FixedGap { get; set; }
        private int MinGap { get; set; }
        private int MaxGap { get; set; }

        private Dictionary<((int, int), (int, int)), bool> Exists = new Dictionary<((int, int), (int, int)), bool>();


        public SA_E_V1(string str, int fixedGap, int minGap, int maxGap) : base(str, fixedGap, minGap, maxGap)
        {
            SA = new SuffixArrayFinal(str);
            FixedGap = fixedGap;
            MinGap = minGap;
            MaxGap = maxGap;
            SA.BuildChildTable();
            int minSizeForLcpIntervals = (int)Math.Sqrt(SA.n);
            SA.GetAllLcpIntervals(1, out Tree, out Leaves, out Root);

            var tree = Tree.Skip(1);
            foreach (var node in tree)
            {
                var n = node.Value;
                foreach (var node2 in tree)
                {
                    
                }
            }

            foreach (var int2 in Tree.Keys.Take(512))
            {
                var occs2 = new HashSet<int>(SA.GetOccurrencesForInterval(int2));
                
                foreach (var int1 in Tree.Keys.Take(512))
                {
                    var occs1 = SA.GetOccurrencesForInterval(int1);
                    Exists.Add((int1, int2), occs1.Any(occ1 => occs2.Contains(occ1 + FixedGap + Tree[int1].DistanceToRoot)));
                }
            }
        }





 

        public override bool Matches(string pattern)
        {
            throw new NotImplementedException();
        }

        public override bool MatchesFixedGap(string pattern1, string pattern2)
        {
            var int1 = SA.ExactStringMatchingWithESA(pattern1);
            var int2 = SA.ExactStringMatchingWithESA(pattern2);
            return Exists[(int1, int2)];
        }

        public override bool MatchesVariableGap(string pattern1, string pattern2)
        {
            throw new NotImplementedException();
        }
    }
}
