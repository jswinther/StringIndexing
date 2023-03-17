using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    // Inspired by this paper https://www.uni-ulm.de/fileadmin/website_uni_ulm/iui.inst.190/Mitarbeiter/ohlebusch/PAPERS/HCMB8.pdf
    internal class SuffixArray_V3_Ger : PatternMatcher
    {
        public string S { get; }
        public int n { get; }
        public int[] SA { get; }
        public int[] LCP { get; }
        public string BWT { get; }
        public int[] ISA { get; }

        public SuffixArray_V3_Ger(string S) : base(S)
        {
            n = S.Length;
            if (!S.EndsWith("$")) S += "$";
            this.S = S;
            SA = SuffixArray();
            LCP = LongestCommonPrefix();
            BWT = BurrowsWheelersTransformation();
            ISA = InverseSuffixArray();
        }

        #region Building the LCP Intervals and Other Stuff



        #endregion


        #region PatternMatcher
        public override IEnumerable<int> Matches(string pattern)
        {
            int substringOccurrence = FindIndexOfFirstOccurrence(pattern);
            if (substringOccurrence < 0) return Enumerable.Empty<int>();
            LinkedList<int> list = new LinkedList<int>();
            list.AddLast(substringOccurrence);
            AddOccurrencesToTheLeftOfIndex(substringOccurrence, pattern.Length, list);
            AddOccurrencesToTheRightOfIndex(substringOccurrence, pattern.Length, list);
            return list;
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Helper Methods
        private int FindIndexOfFirstOccurrence(string pattern)
        {
            int lo = 0;
            int hi = SA.Length - 1;
            while (lo <= hi)
            {
                int mid = lo + (hi - lo) / 2;
                // We cannot match if the pattern is longer than the remaining suffix
                if (pattern.Length > (SA.Length - mid)) return -1;
                string suffix = S.Substring(SA[mid]);
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

        private int ComparePrefix(string pattern, string suffix)
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

        private void AddOccurrencesToTheLeftOfIndex(int idx, int lcp, LinkedList<int> occs)
        {
            // Add an index i, if the index to the right has an lcp value higher than or equal to lcp
            for (int i = idx - 1; i >= 0 && LCP[i + 1] >= lcp; i--)
            {
                occs.AddLast(SA[i]);
            }
        }

        private void AddOccurrencesToTheRightOfIndex(int idx, int lcp, LinkedList<int> occs)
        {
            // Add an index i, if the index to the right has an lcp value higher than or equal to lcp
            for (int i = idx + 1; i < n && LCP[i] >= lcp; i++)
            {
                occs.AddLast(SA[i]);
            }
        }
        #endregion


        #region Base Construction
        private int[] SuffixArray()
        {
            var SA = new Tuple<int, int>[n];

            // Step 1: Populate SA with indices and temporary values
            for (int i = 0; i < n; i++)
            {
                SA[i] = Tuple.Create(i, 0);
            }

            // Step 2-5: Sort suffixes using temporary values for each prefix length
            for (int k = 1; k < n; k <<= 1)
            {
                Comparison<Tuple<int, int>> compare = (a, b) =>
                {
                    if (a.Item2 != b.Item2)
                    {
                        return a.Item2.CompareTo(b.Item2);
                    }
                    return a.Item1 + k < n && b.Item1 + k < n ?
                        SA[a.Item1 + k].Item2.CompareTo(SA[b.Item1 + k].Item2) :
                        (b.Item1 + k < n ? -1 : 1);
                };

                Array.Sort(SA, compare);

                int rank = 0;
                int[] temp = new int[n];

                for (int i = 0; i < n; i++)
                {
                    if (i > 0 && compare(SA[i - 1], SA[i]) != 0)
                    {
                        rank++;
                    }
                    temp[SA[i].Item1] = rank;
                }

                for (int i = 0; i < n; i++)
                {
                    SA[i] = Tuple.Create(i, temp[i]);
                }
            }

            // Step 6: Return suffix array
            return SA.Select(x => x.Item1).ToArray();
        }

        private int[] LongestCommonPrefix()
        {
            var m_lcp = new int[n];
            int[] rank = new int[n];

            // Compute rank array
            for (int i = 0; i < n; i++)
            {
                rank[SA[i]] = i;
            }

            int k = 0;

            // Compute LCP array
            for (int i = 0; i < n; i++)
            {
                if (rank[i] == n - 1)
                {
                    k = 0;
                    continue;
                }

                int j = SA[rank[i] + 1];

                while (i + k < n && j + k < n && S[i + k] == S[j + k])
                {
                    k++;
                }

                m_lcp[rank[i]] = k;

                if (k > 0)
                {
                    k--;
                }
            }
            return m_lcp;
        }

        public string BurrowsWheelersTransformation()
        {
            var bwt = new char[n];

            // Step 1: Initialize empty string
            bwt[0] = S[n - 1];

            // Step 2-3: Append characters from sorted suffixes
            for (int i = 0; i < n - 1; i++)
            {
                bwt[i + 1] = SA[i] == 0 ? S[n - 1] : S[SA[i] - 1];
            }

            // Step 4: Return BWT as a string
            return new string(bwt);
        }

        public int[] InverseSuffixArray()
        {
            var ISA = new int[n];

            for (int i = 0; i < n; i++)
            {
                ISA[SA[i]] = i;
            }

            return ISA;
        }
        #endregion









    }
}
