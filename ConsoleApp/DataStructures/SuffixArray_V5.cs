﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp.DataStructures
{
    internal class SuffixArray_V5 : SuffixArray_V2
    {
        public SuffixArray_V5(string str) : base(str)
        {
        }

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


            //var sortedOccs2 = RadixSorter.Sort(occs2.ToArray(), n);
            //var sortedOccs2 = new SortedSet<int>(occs2);
            Array.Sort(occs2);
            //var set = new SortedSet<int>(occs2);

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
