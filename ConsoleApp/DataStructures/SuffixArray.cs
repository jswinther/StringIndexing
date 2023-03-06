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
    internal class SuffixArray : PatternMatcher
    {
        public SuffixArray(string str) : base(str)
        {
            m_str = str;
            m_sa = new int[str.Length];
            m_isa = new int[m_str.Length];

            FormInitialChains();
            BuildSuffixArray();
            BuildLcpArray();
        }

        public override IEnumerable<int> Matches(string pattern)
        {
            int[] suffixArray = m_sa;
            int[] lcpArray = m_lcp;
            int start = 0;
            int end = m_str.Length - 1;
            List<int> matchingIndices = new List<int>();
            while (start <= end)
            {
                int mid = (start + end) / 2;
                string suffix = m_str.Substring(suffixArray[mid]);
                int lcp = lcpArray[mid];
                if (suffix.StartsWith(pattern))
                {
                    if (lcp >= pattern.Length)
                    {
                        matchingIndices.Add(suffixArray[mid]);
                        int left = mid - 1;
                        while (left >= start && lcpArray[left] >= pattern.Length)
                        {
                            matchingIndices.Add(suffixArray[left]);
                            left--;
                        }
                        int right = mid + 1;
                        while (right <= end && lcpArray[right] >= pattern.Length)
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
                else if (pattern.CompareTo(suffix) < 0)
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

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            int start = 0;
            int end = str.Length - 1;
            SortedSet<(int, int)> matchingIndices = new();
            while (start <= end)
            {
                int mid = (start + end) / 2;
                string suffix = str.Substring(m_sa[mid]);
                if (mid + x + pattern2.Length < m_sa.Length)
                {
                    int lcp = m_lcp[mid];
                    if (suffix.StartsWith(pattern1))
                    {
                        if (lcp >= pattern1.Length)
                        {
                            matchingIndices.Add((m_sa[mid], m_sa[mid] + x + pattern2.Length));
                            int left = mid - 1;
                            while (left >= start && m_lcp[left] >= pattern1.Length)
                            {
                                matchingIndices.Add((m_sa[left], m_sa[left] + x + pattern2.Length));
                                left--;
                            }
                            int right = mid + 1;
                            while (right <= end && m_lcp[right] >= pattern1.Length)
                            {
                                matchingIndices.Add((m_sa[right], m_sa[right] + x + pattern2.Length));
                                right++;
                            }
                            return matchingIndices;
                        }
                        else
                        {
                            start = mid + 1;
                        }
                    }
                    else if (pattern1.CompareTo(suffix) < 0)
                    {
                        end = mid - 1;
                    }
                    else
                    {
                        start = mid + 1;
                    }
                }
                
            }
            return matchingIndices;
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            var p1occs = Matches(pattern1);
            var p2occs = PrivateSortedSetMatches(pattern2);
            int ymin = y_min;
            int ymax = y_max;
            List<(int, int)> items = new();
            foreach (var o1 in p1occs)
            {
                int min = o1 + ymin + pattern1.Length - 1;
                int max = o1 + ymax + pattern1.Length + 1;
                foreach (var item in p2occs.GetViewBetween(min, max))
                {
                    items.Add((o1, item));
                }

            }
            return items;
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

        public int[] Lcp
        {
            get { return m_lcp; }
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

        private void BuildLcpArray()
        {
            m_lcp = new int[m_sa.Length + 1];
            m_lcp[0] = m_lcp[m_sa.Length] = 0;

            for (int i = 1; i < m_sa.Length; i++)
            {
                m_lcp[i] = CalcLcp(m_sa[i - 1], m_sa[i]);
            }
        }

        private int CalcLcp(int i, int j)
        {
            int lcp;
            int maxIndex = m_str.Length - Math.Max(i, j);       // Out of bounds prevention
            for (lcp = 0; (lcp < maxIndex) && (m_str[i + lcp] == m_str[j + lcp]); lcp++) ;
            return lcp;
        }
    }

    #region HelperClasses
    [Serializable]
    internal class Chain : IComparable<Chain>
    {
        public int head;
        public int length;
        private string m_str;

        public Chain(string str)
        {
            m_str = str;
        }

        public int CompareTo(Chain other)
        {
            return m_str.Substring(head, length).CompareTo(m_str.Substring(other.head, other.length));
        }

        public override string ToString()
        {
            return m_str.Substring(head, length);
        }
    }

    [Serializable]
    internal class CharComparer : System.Collections.Generic.EqualityComparer<char>
    {
        public override bool Equals(char x, char y)
        {
            return x.Equals(y);
        }

        public override int GetHashCode(char obj)
        {
            return obj.GetHashCode();
        }
    }

    [Serializable]
    internal struct SuffixRank
    {
        public int head;
        public int rank;
    }

    [Serializable]
    internal class SuffixRankComparer : IComparer<SuffixRank>
    {
        public bool Equals(SuffixRank x, SuffixRank y)
        {
            return x.rank.Equals(y.rank);
        }

        public int Compare(SuffixRank x, SuffixRank y)
        {
            return x.rank.CompareTo(y.rank);
        }
    }
    #endregion
}
