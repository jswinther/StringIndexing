using C5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp.DataStructures
{
    [Serializable]
    internal class SuffixArray_V2 : PatternMatcher
    {
        public SuffixArray_V2(string str) : base(str)
        {
            m_str = str;
            m_sa = new int[str.Length];
            m_isa = new int[m_str.Length];

            FormInitialChains();
            BuildSuffixArray();
            ComputeLCP();
        }

        public override IEnumerable<int> Matches(string pattern)
        {
            List<int> occurrences = new List<int>();

            // Construct the suffix array for the text
            int n = m_str.Length;
            int[] suffixArray = m_sa;

            // Find the first occurrence of the substring in the text
            int substringIndex = BinarySearch(pattern, m_str, suffixArray);

            // If the substring is not found in the text, return an empty list
            if (substringIndex == -1)
            {
                return occurrences;
            }

            // Add the index of the first occurrence of the substring to the list of occurrences
            occurrences.Add(suffixArray[substringIndex]);

            // Check all suffixes that come after the first occurrence of the substring
            for (int i = substringIndex + 1; i < n && m_lcp[i] >= pattern.Length; i++)
            {
                occurrences.Add(suffixArray[i]);
            }

            // Check all suffixes that come before the first occurrence of the substring
            for (int i = substringIndex - 1; i >= 0 && m_lcp[i] >= pattern.Length; i--)
            {
                occurrences.Add(suffixArray[i]);
            }

            return occurrences;
        }

        static int Lcp(string s1, string s2)
        {
            int n = Math.Min(s1.Length, s2.Length);
            for (int i = 0; i < n; i++)
            {
                if (s1[i] != s2[i])
                {
                    return i;
                }
            }
            return n;
        }

        static int BinarySearch(string pattern, string text, int[] suffixArray)
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
            int n = m_str.Length;
            int[] suffixArray = m_sa;

            // Find the first occurrence of the substring in the text
            int substringIndex = BinarySearch(pattern2, m_str, suffixArray);

            // If the substring is not found in the text, return an empty list
            if (substringIndex == -1)
            {
                return occs;
            }

            // Add the index of the first occurrence of the substring to the list of occurrences
            occurencesP2.Add(suffixArray[substringIndex]);

            // Check all suffixes that come after the first occurrence of the substring
            for (int i = substringIndex + 1; i < n && m_lcp[i] >= pattern2.Length; i++)
            {
                occurencesP2.Add(suffixArray[i]);
            }

            // Check all suffixes that come before the first occurrence of the substring
            for (int i = substringIndex - 1; i >= 0 && m_lcp[i] >= pattern2.Length; i--)
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
            int n = m_str.Length;
            int[] suffixArray = m_sa;

            // Find the first occurrence of the substring in the text
            int substringIndex = BinarySearch(pattern2, m_str, suffixArray);

            // If the substring is not found in the text, return an empty list
            if (substringIndex == -1)
            {
                return occs;
            }

            // Add the index of the first occurrence of the substring to the list of occurrences
            occurencesP2.Add(suffixArray[substringIndex]);

            // Check all suffixes that come after the first occurrence of the substring
            for (int i = substringIndex + 1; i < n && m_lcp[i] >= pattern2.Length; i++)
            {
                occurencesP2.Add(suffixArray[i]);
            }

            // Check all suffixes that come before the first occurrence of the substring
            for (int i = substringIndex - 1; i >= 0 && m_lcp[i] >= pattern2.Length; i--)
            {
                occurencesP2.Add(suffixArray[i]);
            }

            foreach (var occ1 in occurrencesP1)
            {
                int min = occ1 + y_min + pattern1.Length;
                int max = occ1 + y_max + pattern1.Length;
                foreach (var occ2 in occurencesP2.GetViewBetween(min, max))
                {
                    occs.Add((occ1, occ2 - occ1 + pattern2.Length));
                }
            }

            
            return occs;
        }

        private SortedSet<int> PrivateSortedSetMatches(string substring)
        {
            var suffixArray = m_sa;
            var lcpArray = m_lcp;
            int start = 0;
            var text = str;
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

      

       


       

   

        private const int EOC = int.MaxValue;
        private int[] m_sa;
        private int[] m_isa;
        private int[] m_lcp;
        private C5.HashDictionary<char, int> m_chainHeadsDict = new HashDictionary<char, int>(new CharComparer());
        private List<Chain> m_chainStack = new List<Chain>();
        private ArrayList<Chain> m_subChains = new ArrayList<Chain>();
        private int m_nextRank = 1;
        private string m_str;

        public int Length
        {
            get { return m_sa.Length; }
        }

        public int this[int index]
        {
            get { return m_sa[index]; }
        }

  




    

        /// 
        /// <summary>Find the index of a substring </summary>
        /// <param name="substr">Substring to look for</param>
        /// <returns>First index in the original string. -1 if not found</returns>
        public int IndexOf(string substr)
        {
            int l = 0;
            int r = m_sa.Length;
            int m = -1;

            if ((substr == null) || (substr.Length == 0))
            {
                return -1;
            }

            // Binary search for substring
            while (r > l)
            {
                m = (l + r) / 2;
                if (m_str.Substring(m_sa[m]).CompareTo(substr) < 0)
                {
                    l = m + 1;
                }
                else
                {
                    r = m;
                }
            }
            if ((l == r) && (l < m_str.Length) && (m_str.Substring(m_sa[l]).StartsWith(substr)))
            {
                return m_sa[l];
            }
            else
            {
                return -1;
            }
        }

  

        private void FormInitialChains()
        {
            // Link all suffixes that have the same first character
            FindInitialChains();
            SortAndPushSubchains();
        }

        private void FindInitialChains()
        {
            // Scan the string left to right, keeping rightmost occurences of characters as the chain heads
            for (int i = 0; i < m_str.Length; i++)
            {
                if (m_chainHeadsDict.Contains(m_str[i]))
                {
                    m_isa[i] = m_chainHeadsDict[m_str[i]];
                }
                else
                {
                    m_isa[i] = EOC;
                }
                m_chainHeadsDict[m_str[i]] = i;
            }

            // Prepare chains to be pushed to stack
            foreach (int headIndex in m_chainHeadsDict.Values)
            {
                Chain newChain = new Chain(m_str);
                newChain.head = headIndex;
                newChain.length = 1;
                m_subChains.Add(newChain);
            }
        }

        private void SortAndPushSubchains()
        {
            m_subChains.Sort();
            for (int i = m_subChains.Count - 1; i >= 0; i--)
            {
                m_chainStack.Add(m_subChains[i]);
            }
        }

        private void BuildSuffixArray()
        {
            while (m_chainStack.Count > 0)
            {
                // Pop chain
                Chain chain = m_chainStack[m_chainStack.Count - 1];
                m_chainStack.RemoveAt(m_chainStack.Count - 1);

                if (m_isa[chain.head] == EOC)
                {
                    // Singleton (A chain that contain only 1 suffix)
                    RankSuffix(chain.head);
                }
                else
                {
                    //RefineChains(chain);
                    RefineChainWithInductionSorting(chain);
                }
            }
        }

        private void RefineChains(Chain chain)
        {
            m_chainHeadsDict.Clear();
            m_subChains.Clear();
            while (chain.head != EOC)
            {
                int nextIndex = m_isa[chain.head];
                if (chain.head + chain.length > m_str.Length - 1)
                {
                    RankSuffix(chain.head);
                }
                else
                {
                    ExtendChain(chain);
                }
                chain.head = nextIndex;
            }
            // Keep stack sorted
            SortAndPushSubchains();
        }

        private void ExtendChain(Chain chain)
        {
            char sym = m_str[chain.head + chain.length];
            if (m_chainHeadsDict.Contains(sym))
            {
                // Continuation of an existing chain, this is the leftmost
                // occurence currently known (others may come up later)
                m_isa[m_chainHeadsDict[sym]] = chain.head;
                m_isa[chain.head] = EOC;
            }
            else
            {
                // This is the beginning of a new subchain
                m_isa[chain.head] = EOC;
                Chain newChain = new Chain(m_str);
                newChain.head = chain.head;
                newChain.length = chain.length + 1;
                m_subChains.Add(newChain);
            }
            // Save index in case we find a continuation of this chain
            m_chainHeadsDict[sym] = chain.head;
        }

        private void RefineChainWithInductionSorting(Chain chain)
        {
            // TODO - refactor/beautify some
            ArrayList<SuffixRank> notedSuffixes = new ArrayList<SuffixRank>();
            m_chainHeadsDict.Clear();
            m_subChains.Clear();

            while (chain.head != EOC)
            {
                int nextIndex = m_isa[chain.head];
                if (chain.head + chain.length > m_str.Length - 1)
                {
                    // If this substring reaches end of string it cannot be extended.
                    // At this point it's the first in lexicographic order so it's safe
                    // to just go ahead and rank it.
                    RankSuffix(chain.head);
                }
                else if (m_isa[chain.head + chain.length] < 0)
                {
                    SuffixRank sr = new SuffixRank();
                    sr.head = chain.head;
                    sr.rank = -m_isa[chain.head + chain.length];
                    notedSuffixes.Add(sr);
                }
                else
                {
                    ExtendChain(chain);
                }
                chain.head = nextIndex;
            }
            // Keep stack sorted
            SortAndPushSubchains();
            SortAndRankNotedSuffixes(notedSuffixes);
        }

        private void SortAndRankNotedSuffixes(ArrayList<SuffixRank> notedSuffixes)
        {
            notedSuffixes.Sort(new SuffixRankComparer());
            // Rank sorted noted suffixes 
            for (int i = 0; i < notedSuffixes.Count; ++i)
            {
                RankSuffix(notedSuffixes[i].head);
            }
        }

        private void RankSuffix(int index)
        {
            // We use the ISA to hold both ranks and chain links, so we differentiate by setting
            // the sign.
            m_isa[index] = -m_nextRank;
            m_sa[m_nextRank - 1] = index;
            m_nextRank++;
        }

        public void ComputeLCP()
        {
            int n = m_str.Length;
            m_lcp = new int[n];
            int[] rank = new int[n];

            // Compute rank array
            for (int i = 0; i < n; i++)
            {
                rank[m_sa[i]] = i;
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

                int j = m_sa[rank[i] + 1];

                while (i + k < n && j + k < n && m_str[i + k] == m_str[j + k])
                {
                    k++;
                }

                m_lcp[rank[i]] = k;

                if (k > 0)
                {
                    k--;
                }
            }
        }

        
    }

    
}
