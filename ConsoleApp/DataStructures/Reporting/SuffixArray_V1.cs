using C5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp.DataStructures.Reporting
{

    [Serializable]
    internal class SuffixArray_V1 : ReportDataStructure
    {
        SuffixArrayFinal SA;
        public SuffixArray_V1(string str) : base(str)
        {
            SA = new SuffixArrayFinal(str);   
        }

        public override IEnumerable<int> Matches(string pattern)
        {
            List<int> occurrences = new List<int>();

            // Construct the suffix array for the text
            int n = SA.m_str.Length;
            int[] suffixArray = SA.Sa;

            // Find the first occurrence of the substring in the text
            int substringIndex = BinarySearch(pattern, SA.m_str, suffixArray);

            // If the substring is not found in the text, return an empty list
            if (substringIndex == -1)
            {
                return occurrences;
            }

            // Add the index of the first occurrence of the substring to the list of occurrences
            occurrences.Add(suffixArray[substringIndex]);

            // Check all suffixes that come after the first occurrence of the substring
            for (int i = substringIndex + 1; i < n && SA.Lcp1[i] >= pattern.Length; i++)
            {
                occurrences.Add(suffixArray[i]);
            }

            // Check all suffixes that come before the first occurrence of the substring
            for (int i = substringIndex - 1; i >= 0 && SA.Lcp1[i + 1] >= pattern.Length; i--)
            {
                occurrences.Add(suffixArray[i]);
            }

            return occurrences;
        }

        public int BinarySearch(string pattern, string text, int[] suffixArray)
        {
            int lo = 0;
            int hi = suffixArray.Length - 1;
            while (lo <= hi)
            {
                int mid = lo + (hi - lo) / 2;
                string suffix = text.Substring(suffixArray[mid]);
                int cmp = ComparePrefix(pattern, suffix);
                if (cmp < 0)
                {
                    hi = mid - 1;
                }
                else if (cmp > 0)
                {
                    lo = mid + 1;
                }
                else
                {
                    return mid;
                }
            }
            return -1;
        }

        static int ComparePrefix(string pattern, string suffix)
        {
            int n = Math.Min(pattern.Length, suffix.Length);
            for (int i = 0; i < n; i++)
            {
                if (pattern[i] < suffix[i])
                {
                    return -1;
                }
                else if (pattern[i] > suffix[i])
                {
                    return 1;
                }
            }
            return 0;
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            List<(int, int)> occs = new();
            var occurrencesP1 = Matches(pattern1);

            System.Collections.Generic.HashSet<int> occurencesP2 = new();

            // Construct the suffix array for the text
            int n = SA.m_str.Length;
            int[] suffixArray = SA.Sa;

            // Find the first occurrence of the substring in the text
            int substringIndex = BinarySearch(pattern2, SA.m_str, suffixArray);

            // If the substring is not found in the text, return an empty list
            if (substringIndex == -1)
            {
                return occs;
            }

            // Add the index of the first occurrence of the substring to the list of occurrences
            occurencesP2.Add(suffixArray[substringIndex]);

            // Check all suffixes that come after the first occurrence of the substring
            for (int i = substringIndex + 1; i < n && SA.Lcp1[i - 1] >= pattern2.Length; i++)
            {
                occurencesP2.Add(suffixArray[i]);
            }

            // Check all suffixes that come before the first occurrence of the substring
            for (int i = substringIndex - 1; i >= 0 && SA.Lcp1[i] >= pattern2.Length; i--)
            {
                occurencesP2.Add(suffixArray[i]);
            }

            foreach (var occ1 in occurrencesP1)
            {
                if (occurencesP2.Contains(occ1 + pattern1.Length + x))
                {
                    occs.Add((occ1, occ1 + pattern1.Length + x));
                }
            }

            return occs;
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            List<(int, int)> occs = new();
            var occurrencesP1 = Matches(pattern1);

            SortedSet<int> occurencesP2 = new();

            // Construct the suffix array for the text
            int n = SA.m_str.Length;
            int[] suffixArray = SA.Sa;

            // Find the first occurrence of the substring in the text
            int substringIndex = BinarySearch(pattern2, SA.m_str, suffixArray);

            // If the substring is not found in the text, return an empty list
            if (substringIndex == -1)
            {
                return occs;
            }

            // Add the index of the first occurrence of the substring to the list of occurrences
            occurencesP2.Add(suffixArray[substringIndex]);

            // Check all suffixes that come after the first occurrence of the substring
            for (int i = substringIndex + 1; i < n && SA.Lcp1[i - 1] >= pattern2.Length; i++)
            {
                occurencesP2.Add(suffixArray[i]);
            }

            // Check all suffixes that come before the first occurrence of the substring
            for (int i = substringIndex - 1; i >= 0 && SA.Lcp1[i] >= pattern2.Length; i--)
            {
                occurencesP2.Add(suffixArray[i]);
            }

            foreach (var occ1 in occurrencesP1)
            {
                int min = occ1 + y_min + pattern1.Length;
                int max = occ1 + y_max + pattern1.Length;
                var getkekd = occurencesP2.GetViewBetween(min, max);
                foreach (var occ2 in occurencesP2.GetViewBetween(min, max))
                {
                    occs.Add((occ1, occ2 - occ1 + pattern2.Length));
                }
            }


            return occs;
        }
    }


}
