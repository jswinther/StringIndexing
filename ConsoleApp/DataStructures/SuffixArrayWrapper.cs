using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    public class SuffixArrayWrapper
    {
        SuffixArrayEranMeir sa;

        public SuffixArrayWrapper(string s)
        {
            sa = new SuffixArrayEranMeir(s);
        }

        public List<int> FindSubstringOccurrences(string substring)
        {
            var suffixArray = sa;
            var lcpArray = sa.Lcp;
            int start = 0;
            var text = sa.Str;
            int end = text.Length - 1;
            List<int> matchingIndices = new List<int>();
            while (start <= end)
            {
                int mid = (start + end) / 2;
                string suffix = text.Substring(suffixArray[mid]);
                int lcp = lcpArray[mid];
                if (suffix.StartsWith(substring))
                {
                    if (lcp >= substring.Length - 1)
                    {
                        matchingIndices.Add(suffixArray[mid]);
                        start = mid + 1;
                    }
                    else
                    {
                        start = mid + 1;
                    }
                }
                else if (substring.CompareTo(suffix) < 0)
                {
                    end = mid - 1;
                }
                else
                {
                    start = mid + 1;
                }
            }
            return matchingIndices;
        }

        public SortedSet<int> FindSubstringOccurrencesSorted(string substring)
        {
            var suffixArray = sa;
            var lcpArray = sa.Lcp;
            int start = 0;
            var text = sa.Str;
            int end = text.Length - 1;
            SortedSet<int> matchingIndices = new SortedSet<int>();
            while (start <= end)
            {
                int mid = (start + end) / 2;
                string suffix = text.Substring(suffixArray[mid]);
                int lcp = lcpArray[mid];
                if (suffix.StartsWith(substring))
                {
                    if (lcp >= substring.Length - 1)
                    {
                        matchingIndices.Add(suffixArray[mid]);
                        int left = mid - 1;
                        while (left >= start && lcpArray[left] >= substring.Length - 1)
                        {
                            matchingIndices.Add(suffixArray[left]);
                            left--;
                        }
                        int right = mid + 1;
                        while (right <= end && lcpArray[right] >= substring.Length - 1)
                        {
                            matchingIndices.Add(suffixArray[right]);
                            right++;
                        }
                        return matchingIndices;
                    }
                    else
                    {
                        start = mid + 1;
                    }
                }
                else if (substring.CompareTo(suffix) < 0)
                {
                    end = mid - 1;
                }
                else
                {
                    start = mid + 1;
                }
            }
            return matchingIndices;
        }

        public IEnumerable<(int, int)> GetOccurrencesWithSortedSet(Query query)
        {
            var p1occs = FindSubstringOccurrences(query.P1);
            var p2occs = FindSubstringOccurrencesSorted(query.P2);
            int ymin = query.Y.Min;
            int ymax = query.Y.Max;
            List<(int, int)> items = new();
            foreach (var o1 in p1occs)
            {
                int min = o1 + ymin + query.P1.Length - 1;
                int max = o1 + ymax + query.P1.Length + 1;
                foreach (var item in p2occs.GetViewBetween(min, max))
                {
                    items.Add((o1, item));
                }

            }
            return items;
        }

        public List<int> ReportOccurrences(string p)
        {
            List<int> ints = new List<int>();
            int index = sa.IndexOf(p);
            if (index == -1) return ints;
            for (int u = index; u < sa.Str.Length; u++)
            {
                if (sa.Lcp[u] == sa.Lcp[index])
                {
                    ints.Add(sa[u]);
                }
            }
            return ints;
        }

        public SortedSet<int> ReportOccurrencesSortedSet(string p)
        {
            SortedSet<int> ints = new SortedSet<int>();
            int index = sa.IndexOf(p);
            if (index == -1) return ints;
            int u = index;
            while(sa.Lcp[u] == sa.Lcp[index])
            {
                ints.Add(sa[u]);
                ++u;
            }
            return ints;
        }
    }
}
