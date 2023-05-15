using C5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp.DataStructures.SuffixArrayFinal;

namespace ConsoleApp.DataStructures.Reporting
{
    // Inspired by this paper https://www.uni-ulm.de/fileadmin/website_uni_ulm/iui.inst.190/Mitarbeiter/ohlebusch/PAPERS/HCMB8.pdf
    internal class SuffixArray_V3 : PatternMatcher
    {

        private Dictionary<(int, int), int[]> sorted = new();
        Dictionary<(int, int), IntervalNode> Tree;
        Dictionary<(int, int), IntervalNode> Leaves;
        SuffixArrayFinal SA;
        public SuffixArray_V3(string S) : base(S)
        {
            SA = new SuffixArrayFinal(S);
            SA.BuildChildTable();
            SA.GetAllLcpIntervals(2, out Tree, out Leaves);
            var keys = Tree.Keys.ToList();

            for (int i = 1; i < keys.Count; i++)
            {
                var interval = keys[i];
                var originalPlacesOfSuffixes = SA.GetOccurrencesForInterval(interval);
                originalPlacesOfSuffixes.Sort();
                sorted.Add(interval, originalPlacesOfSuffixes);
            }
           
        }


        #region PatternMatcher
        public override IEnumerable<int> Matches(string pattern)
        {
            return ArrayOfPattern(pattern);
        }

        private int[] ArrayOfPattern(string pattern)
        {
            var interval = SA.ExactStringMatchingWithESA(pattern);
            if (interval == (-1, -1)) return new int[] { };
            if (interval.j - interval.i == 0) return new int[] { SA[interval.i] };
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
            var pattern1Occurrences = ArrayOfPattern(pattern1);
            var pattern2Occurrences = ArrayOfPattern(pattern2);
           
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
