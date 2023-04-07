using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    internal class SuffixArray_V5_Radix : SuffixArray_V2
    {
        public SuffixArray_V5_Radix(string str) : base(str)
        {
        }

        #region Pattern Matching

        public override IEnumerable<int> Matches(string pattern)
        {
            return base.Matches(pattern);
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            return base.Matches(pattern1, x, pattern2);
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            var occs1 = base.Matches(pattern1);
            var occs2 = base.Matches(pattern2);

            
            //var sortedOccs2 = RadixSorter.Sort(occs2.ToArray(), n);
            var sortedOccs2 = new SortedSet<int>(occs2);

            return new List<(int, int)>();
        }

        #endregion
    }
}
