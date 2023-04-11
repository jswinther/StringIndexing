using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    internal class SuffixArray_V6_Merge : SuffixArray_V2
    {
        public SuffixArray_V6_Merge(string str) : base(str)
        {
        }

        #region Pattern Matching

        public override IEnumerable<int> Matches(string pattern)
        {
            return GetOccurrencesForPattern(pattern);
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            return base.Matches(pattern1, x, pattern2);
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {

            var occs1 = GetOccurrencesForPattern(pattern1);
            var occs2 = GetOccurrencesForPattern(pattern2);

            Array.Sort(occs1);
            Array.Sort(occs2);
            return FindFirstOccurrenceForEachPattern1Occurrence(pattern1, y_min, y_max, pattern2, occs1, occs2);
        }

        #endregion
    }
}
