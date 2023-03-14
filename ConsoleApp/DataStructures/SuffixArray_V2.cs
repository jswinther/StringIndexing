using C5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp.DataStructures
{
    [Serializable]
    internal class SuffixArray_V2 : PatternMatcher
    {
        private const int EOC = int.MaxValue;
        private int[] m_sa;
        private int[] m_isa;
        private int[] m_lcp;
        private C5.HashDictionary<char, int> m_chainHeadsDict = new HashDictionary<char, int>(new CharComparer());
        private List<Chain> m_chainStack = new List<Chain>();
        private ArrayList<Chain> m_subChains = new ArrayList<Chain>();
        private int m_nextRank = 1;
        private string m_str;
        public IntervalTree IntervalTree;
    
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

            // Find the first occurrence of the substring in the text
            int substringIndex = BinarySearch(pattern);
            
            // If the substring is not found in the text, return an empty list
            if (substringIndex == -1)
            {
                return occurrences;
            }

            // Find all intervals of value pattern length
            

            // Add the index of the first occurrence of the substring to the list of occurrences
            occurrences.Add(m_sa[substringIndex]);

            // Check all suffixes that come after the first occurrence of the substring
            for (int i = substringIndex + 1; i < n && m_lcp[i - 1] >= pattern.Length; i++)
            {
                occurrences.Add(m_sa[i]);
            }

            // Check all suffixes that come before the first occurrence of the substring
            for (int i = substringIndex - 1; i >= 0 && m_lcp[i] >= pattern.Length; i--)
            {
                occurrences.Add(m_sa[i]);
            }

            return occurrences;
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            List<(int, int)> occs = new();
            var occurrencesP1 = Matches(pattern1);

            System.Collections.Generic.HashSet<int> occurencesP2 = new();

            // Construct the suffix array for the text
            int n = m_str.Length;

            // Find the first occurrence of the substring in the text
            int substringIndex = BinarySearch(pattern2);

            // If the substring is not found in the text, return an empty list
            if (substringIndex == -1)
            {
                return occs;
            }

            // Add the index of the first occurrence of the substring to the list of occurrences
            occurencesP2.Add(m_sa[substringIndex]);

            // Check all suffixes that come after the first occurrence of the substring
            for (int i = substringIndex + 1; i < n && m_lcp[i - 1] >= pattern2.Length; i++)
            {
                occurencesP2.Add(m_sa[i]);
            }

            // Check all suffixes that come before the first occurrence of the substring
            for (int i = substringIndex - 1; i >= 0 && m_lcp[i] >= pattern2.Length; i--)
            {
                occurencesP2.Add(m_sa[i]);
            }
            int[] occs2 = occurencesP2.ToArray();
            foreach (var occ1 in occurrencesP1)
            {
                if ( occurencesP2.Contains(occ1 + pattern1.Length + x))
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

            // Find the first occurrence of the substring in the text
            int substringIndex = BinarySearch(pattern2);
            
            // If the substring is not found in the text, return an empty list
            if (substringIndex == -1)
            {
                return occs;
            }

            occurencesP2 = IntervalTree.Query(pattern2.Length, substringIndex).MaxBy(s => s.end - s.start).ints;

            /*
            // Add the index of the first occurrence of the substring to the list of occurrences
            occurencesP2.Add(m_sa[substringIndex]);

            // Check all suffixes that come after the first occurrence of the substring
            for (int i = substringIndex + 1; i < n && m_lcp[i - 1] >= pattern2.Length; i++)
            {
                occurencesP2.Add(m_sa[i]);
            }

            // Check all suffixes that come before the first occurrence of the substring
            for (int i = substringIndex - 1; i >= 0 && m_lcp[i] >= pattern2.Length; i--)
            {
                occurencesP2.Add(m_sa[i]);
            }
            */

            foreach (var occ1 in occurrencesP1)
            {
                int min = occ1 + y_min + pattern1.Length;
                int max1 = occ1 + y_max + pattern1.Length;
                foreach (var occ2 in occurencesP2.GetViewBetween(min, max1))
                {
                    occs.Add((occ1, occ2 - occ1 + pattern2.Length));
                }
            }

            
            return occs;
        }


        int BinarySearch(string pattern)
        {
            int lo = 0;
            int hi = m_sa.Length - 1;
            while (lo <= hi)
            {
                int mid = lo + (hi - lo) / 2;
                string suffix = m_str.Substring(m_sa[mid]);
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

        int ComparePrefix(string pattern, string suffix)
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

        private void ComputeLCP()
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
