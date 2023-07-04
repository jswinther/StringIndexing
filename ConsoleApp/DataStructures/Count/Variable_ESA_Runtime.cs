using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Count
{
    internal class Variable_ESA_Runtime : CountVariable
    {
        public Variable_ESA_Runtime(SuffixArrayFinal str, int minGap, int maxGap) : base(str, minGap, maxGap)
        {
        }

        public Variable_ESA_Runtime(string str, int minGap, int maxGap) : base(str, minGap, maxGap)
        {
        }

        public override int Matches(string pattern1, string pattern2)
        {
            int count = 0;
            var occs1 = SA.SinglePattern(pattern1);
            var occs2 = SA.SinglePattern(pattern2);
            occs2.Sort();

            foreach (var occ1 in occs1)
            {
                int min = occ1 + minGap + pattern1.Length;
                int max = occ1 + maxGap + pattern1.Length;
                (var a, var b) = occs2.BinarySearchOnRange(min, max);
                count += b - a + 1;
            }
            return count;
        }
    }
}
