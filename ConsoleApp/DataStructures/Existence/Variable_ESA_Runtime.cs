using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Existence
{
    internal class Variable_ESA_Runtime : ExistVariable
    {
        public Variable_ESA_Runtime(SuffixArrayFinal str, int minGap, int maxGap) : base(str, minGap, maxGap)
        {
        }

        public Variable_ESA_Runtime(string str, int minGap, int maxGap) : base(str, minGap, maxGap)
        {
        }

        public override bool Matches(string pattern1, string pattern2)
        {
            var occs1 = SA.SinglePattern(pattern1);
            var occs2 = SA.SinglePattern(pattern2);
            occs2.Sort();

            foreach (var occ1 in occs1)
            {
                int min = occ1 + minGap + pattern1.Length;
                int max = occ1 + maxGap + pattern1.Length;
                (var a, var b) = occs2.BinarySearchOnRange(min, max);
                if (a != -1 && b != -1) return true;
            }
            return false;
        }
    }
}
