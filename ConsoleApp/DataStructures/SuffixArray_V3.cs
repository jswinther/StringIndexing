using C5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    // Inspired by this paper https://www.uni-ulm.de/fileadmin/website_uni_ulm/iui.inst.190/Mitarbeiter/ohlebusch/PAPERS/HCMB8.pdf
    internal class SuffixArray_V3 : SuffixArray_V2
    {

        private Dictionary<(int, int), SortedSet<int>> sorted = new();

        public SuffixArray_V3(string S) : base(S)
        {
            Dictionary<(int, int), SortedSet<int>> dic = new();
           
            GetAllLcpIntervals();
            foreach (var interval in _nodes.Keys)
            {
                var originalPlacesOfSuffixes = Sa.Take(new Range(new Index(interval.Item1 == -1 ? 0 : interval.Item1), new Index(interval.Item2 + 1)));
                dic.Add(interval, new SortedSet<int>(originalPlacesOfSuffixes));
            }
            sorted = dic;
        }


        #region PatternMatcher
        public override IEnumerable<int> Matches(string pattern)
        {
            var interval = ExactStringMatchingWithESA(pattern);
            return sorted[interval];
        }

  

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            List<(int, int)> occs = new List<(int, int)>();
            var pattern1Occurrences = Matches(pattern1);
            var pattern2Occurrences = new System.Collections.Generic.HashSet<int>(Matches(pattern2));
            foreach (var occ1 in pattern1Occurrences)
            {
                if (pattern2Occurrences.Contains(occ1 + pattern1.Length + x))
                {
                    occs.Add((occ1, occ1 + pattern1.Length + x));
                }
            }
            return occs;
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            List<(int, int)> occs = new List<(int, int)>();
            var pattern1Occurrences = Matches(pattern1);
            var pattern2Occurrences = new SortedSet<int>(Matches(pattern2));
            foreach (var occ1 in pattern1Occurrences)
            {
                int min = occ1 + y_min + pattern1.Length;
                int max = occ1 + y_max + pattern1.Length;
                foreach (var occ2 in pattern2Occurrences.GetViewBetween(min, max))
                {
                    occs.Add((occ1, occ2 - occ1 + pattern2.Length));
                }
            }
            return occs;
        }
        #endregion

    }
}
