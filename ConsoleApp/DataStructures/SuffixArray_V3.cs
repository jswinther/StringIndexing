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

        private Dictionary<(int, int), int[]> sorted = new();

        public SuffixArray_V3(string S) : base(S)
        {
            GetAllLcpIntervals();
            var keys = _nodes.Keys.ToList();

            Stack<IntervalNode> nodes = new Stack<IntervalNode>();
            System.Collections.Generic.HashSet<(int, int)> visited = new System.Collections.Generic.HashSet<(int, int)>();
            nodes.Push(_nodes[keys[0]]);
            foreach (var leaf in _leaves)
            {
                var occs = GetOccurrencesForInterval(leaf);
                Array.Sort(occs);
                sorted.Add(leaf, occs);
            }

            while (nodes.Count > 0)
            {
                var top = nodes.Peek();

                if (!visited.Contains(top.Interval))
                {
                    foreach (var item in top.Children.Where(child => !_leaves.Contains(child)))
                    {
                        nodes.Push(_nodes[item]);
                    }
                    visited.Add(top.Interval);
                }
                else
                {
                    sorted.Add(top.Interval, Program.KWayMerge(top.Children.Select(s => sorted[s]).ToArray()));
                    nodes.Pop();
                }
            }

            /*
            for (int i = 1; i < keys.Count; i++)
            {
                var interval = keys[i];
                var originalPlacesOfSuffixes = GetOccurrencesForInterval(interval);
                Array.Sort(originalPlacesOfSuffixes);
                sorted.Add(interval, originalPlacesOfSuffixes);
            }
            */
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
            var pattern1Occurrences = sorted[ExactStringMatchingWithESA(pattern1)];
            var pattern2Occurrences = sorted[ExactStringMatchingWithESA(pattern2)];
            return FindFirstOccurrenceForEachPattern1Occurrence(pattern1, y_min, y_max, pattern2, pattern1Occurrences, pattern2Occurrences);
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
