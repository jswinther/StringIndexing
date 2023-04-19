using C5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    internal abstract class BaseSuffixArray : PatternMatcher
    {

        public const int EOC = int.MaxValue;
        public int[] m_sa;
        public int[] m_isa;
        public int[] m_lcp;
        public C5.HashDictionary<char, int> m_chainHeadsDict = new HashDictionary<char, int>(new CharComparer());
        public List<Chain> m_chainStack = new List<Chain>();
        public ArrayList<Chain> m_subChains = new ArrayList<Chain>();
        public int m_nextRank = 1;
        public string m_str;
        public int n;
        protected BaseSuffixArray(string str) : base(str)
        {
            m_str = str;
            m_str += "|";
            n = m_str.Length;
            m_sa = new int[n];
            m_isa = new int[n];


            FormInitialChains();
            BuildSuffixArray();
            BuildLcpArray();
        }

        public int this[int index]
        {
            get { return Sa[index]; }
        }

        
       


       

   

        

        public int Length
        {
            get { return Sa.Length; }
        }

        public int[] Sa { get => m_sa; set => m_sa = value; }
        public int[] Lcp1 { get => m_lcp; set => m_lcp = value; }
        public string S { get => m_str; }

        public void ComputeLCP()
        {
            int n = m_str.Length;
            Lcp1 = new int[n];
            int[] rank = new int[n];

            // Compute rank array
            for (int i = 0; i < n; i++)
            {
                rank[Sa[i]] = i;
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

                int j = Sa[rank[i] + 1];

                while (i + k < n && j + k < n && m_str[i + k] == m_str[j + k])
                {
                    k++;
                }

                Lcp1[rank[i]] = k;

                if (k > 0)
                {
                    k--;
                }
            }
        }

        public int Lcp(string s1, string s2)
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

       

        private void BuildLcpArray()
        {
            Lcp1 = new int[Sa.Length + 1];
            Lcp1[0] = Lcp1[Sa.Length] = 0;

            for (int i = 1; i < Sa.Length; i++)
            {
                Lcp1[i] = CalcLcp(Sa[i - 1], Sa[i]);
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

        private int CalcLcp(int i, int j)
        {
            int lcp;
            int maxIndex = m_str.Length - Math.Max(i, j);       // Out of bounds prevention
            for (lcp = 0; (lcp < maxIndex) && (m_str[i + lcp] == m_str[j + lcp]); lcp++) ;
            return lcp;
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







  

        private void FormInitialChains()
        {
            // Link all suffixes that have the same first character
            FindInitialChains();
            SortAndPushSubchains();
        }

        private void RankSuffix(int index)
        {
            // We use the ISA to hold both ranks and chain links, so we differentiate by setting
            // the sign.
            m_isa[index] = -m_nextRank;
            Sa[m_nextRank - 1] = index;
            m_nextRank++;
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

        private void SortAndPushSubchains()
        {
            m_subChains.Sort();
            for (int i = m_subChains.Count - 1; i >= 0; i--)
            {
                m_chainStack.Add(m_subChains[i]);
            }
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
            return HomemadeCompare(head, other.head);
            //return m_str.Substring(head, length).CompareTo(m_str.Substring(other.head, other.length));
        }

        private int HomemadeCompare(int x, int y)
        {
            int i = x, j = y;
            while (i < m_str.Length && j < m_str.Length)
            {
                if (m_str[i] != m_str[j])
                {
                    return m_str[i].CompareTo(m_str[j]);
                }
                i++;
                j++;
            }
            return i - j;
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
