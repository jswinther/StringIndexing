using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp.DataStructures.SuffixArrayFinal;

namespace ConsoleApp.DataStructures.Existence
{
    public class SA_E_V0 : ExistDataStructure
    {
        private SuffixArrayFinal SA;
        public SA_E_V0(string str, int fixedGap, int minGap, int maxGap) : base(str, fixedGap, minGap, maxGap)
        {
            SA = new SuffixArrayFinal(str);
            
        }

        public SA_E_V0(SuffixArrayFinal str, int x, int ymin, int ymax) : base(str, x, ymin, ymax)
        {
            SA = str;
        }

        public override bool Matches(string pattern)
        {
            (var a, var b) = SA.ExactStringMatchingWithESA(pattern);
            return ((a, b) != (-1, -1));
        }

        public override bool MatchesFixedGap(string pattern1, string pattern2)
        {
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
            var occs2 = new HashSet<int>(SA.GetOccurrencesForPattern(pattern2));

            foreach (var occ1 in occs1)
            {
                if (occs2.Contains(occ1 + pattern1.Length + x))
                    return true;
            }
            return false;
        }

        public override bool MatchesVariableGap(string pattern1, string pattern2)
        {
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
            var occs2 = SA.GetOccurrencesForPattern(pattern2);
            occs2.Sort();

            foreach (var occ1 in occs1)
            {
                int min = occ1 + ymin + pattern1.Length;
                int max = occ1 + ymax + pattern1.Length;
                (var a, var b) = occs2.BinarySearchOnRange(min, max);
                if ((a, b) != (-1, -1)) return true;
            }
            return false;

        }
    }
}
