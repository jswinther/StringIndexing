using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Count
{
    internal class Fixed_ESA_Runtime : CountFixed
    {
        public Fixed_ESA_Runtime(SuffixArrayFinal str, int gap) : base(str, gap)
        {
        }

        public Fixed_ESA_Runtime(string str, int gap) : base(str, gap)
        {
        }

        public override int Matches(string pattern1, string pattern2)
        {
            int count = 0;
            var occs1 = SA.SinglePattern(pattern1);
            var occs2 = new HashSet<int>(SA.SinglePattern(pattern2));

            foreach (var occ1 in occs1)
            {
                if (occs2.Contains(occ1 + pattern1.Length + gap))
                    count++;
            }
            return count;
        }
    }
}
