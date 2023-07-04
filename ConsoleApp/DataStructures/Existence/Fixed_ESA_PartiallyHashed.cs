using ConsoleApp.Data.Obsolete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Existence
{
    public class Fixed_Exist_ESA_PartiallyHashed : ExistFixed
    {
        private Dictionary<(int, int), HashSet<int>> _partiallyHashed = new();
        private Dictionary<((int, int), (int, int)), bool> Exists = new Dictionary<((int, int), (int, int)), bool>();
        private double CutOff;
        public Fixed_Exist_ESA_PartiallyHashed(SuffixArrayFinal str, int gap) : base(str, gap)
        {
            BuildDataStructure();
        }

        public Fixed_Exist_ESA_PartiallyHashed(string str, int gap) : base(str, gap)
        {
            BuildDataStructure();
        }

        private void BuildDataStructure()
        {
            CutOff = Math.Sqrt(SA.n.Value);
            SA.GetAllLcpIntervals(CutOff, out var Tree, out var Leaves, out var Root);
            var intervals = Tree.Keys.Skip(1).ToArray();
            foreach (var key in intervals)
            {
                _partiallyHashed.Add(key, new HashSet<int>(SA.GetOccurrencesForInterval(key)));
            }
            foreach (var int1 in intervals)
            {
                var occs1 = _partiallyHashed[int1];
                foreach (var int2 in intervals)
                {
                    var occs2 = _partiallyHashed[int2];
                    Exists.TryAdd((int1, int2), 
                        occs1.Any(occ1 => occs2.Contains(occ1 + gap + Tree[int1].DistanceToRoot)));
                }
            }
        }

        public override bool Matches(string pattern1, string pattern2)
        {
            var int1 = SA.ExactStringMatchingWithESA(pattern1);
            var int2 = SA.ExactStringMatchingWithESA(pattern2);
            HashSet<int> occs1;
            int[] occs2;
            if (Exists.TryGetValue((int1, int2), out var match))
            {
                return match;
            }
            if (!_partiallyHashed.TryGetValue(int1, out occs1)) 
                occs1 = new HashSet<int>(SA.GetOccurrencesForInterval(int1));
            occs2 = SA.GetOccurrencesForInterval(int2);
            foreach (var occ2 in occs2)
                if (occs1.Contains(occ2 - pattern1.Length - gap))
                    return true;
            return false;
        }
    }
}
