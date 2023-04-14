using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp.DataStructures.Reporting
{
    internal class SuffixArray_V2 : PatternMatcher
    {
        SuffixArrayFinal SA;
        public SuffixArray_V2(string str) : base(str)
        {
            SA = new SuffixArrayFinal(str);
            SA.BuildChildTable();
        }

        public override IEnumerable<int> Matches(string pattern)
        {
            return SA.GetOccurrencesForPattern(pattern);
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            List<(int, int)> occs = new List<(int, int)>();
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
            var occs2 = new HashSet<int>(SA.GetOccurrencesForPattern(pattern2));

            foreach (var occ1 in occs1)
            {
                if (occs2.Contains(occ1 + pattern1.Length + x))
                    occs.Add((occ1, occ1 + pattern2.Length + pattern2.Length + x));
            }
            return occs;
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            List<(int, int)> occs = new List<(int, int)>();
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
            var occs2 = SA.GetOccurrencesForPattern(pattern2);
            Array.Sort(occs2);

            foreach (var occ1 in occs1)
            {
                int min = occ1 + y_min + pattern1.Length;
                int max = occ1 + y_max + pattern1.Length;
                foreach (var occ2 in occs2.GetViewBetween(min, max))
                {
                    occs.Add((occ1, occ2 - occ1 + pattern2.Length));
                }
            }
            return occs;
        }
    }
}
