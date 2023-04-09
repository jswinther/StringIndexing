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
            List<(int, int)> occs = new List<(int, int)>();
            var occs1 = GetOccurrencesForPattern(pattern1);
            var occs2 = GetOccurrencesForPattern(pattern2);
             
            Array.Sort(occs1);
            Array.Sort(occs2);

            int k = 0;
            for (int i = 0; i < occs1.Length; i++)
            {
                int occ1 = occs1[i];

                int min = occ1 + pattern1.Length + y_min;
                int max = occ1 + pattern1.Length + y_max;

                for (int j = k; j < occs2.Length; j++)
                {
                    int occ2 = occs2[j];
                    if (min <= occ2 && occ2 <= max)
                    {
                        occs.Add((occ1, occs2[j] - occ1 + pattern2.Length));
                        break;
                    }
                    else
                    {
                        k++;
                    }
                }
            }
            return occs;
        }

        #endregion
    }
}
