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
        private int n;
        public (int Up, int Down, int Next)[] Children { get; }
        public string S { get => m_str; }
        public int[] Lcp1 { get => m_lcp; }
        public int[] Sa { get => m_sa; }
        public int MyProperty { get; set; }
        public int N { get => n; }
        public SuffixArray_V2(string str) : base(str)
        {
            
            m_str = str;
            m_str += "|";
            n = m_str.Length;
            m_sa = new int[n];
            m_isa = new int[n];
            
            FormInitialChains();
            BuildSuffixArray();
            BuildLcpArray();
            Children = new (int Up, int Down, int Next)[n];
            BuildChildTable();
        }

        private void BuildLcpArray()
        {
            m_lcp = new int[Sa.Length];
            Lcp1[0] = 0;

            for (int i = 1; i < Sa.Length; i++)
            {
                Lcp1[i] = CalcLcp(Sa[i - 1], Sa[i]);
            }
        }

        private int CalcLcp(int i, int j)
        {
            int lcp;
            int maxIndex = m_str.Length - Math.Max(i, j);       // Out of bounds prevention
            for (lcp = 0; (lcp < maxIndex) && (m_str[i + lcp] == m_str[j + lcp]); lcp++) ;
            return lcp;
        }

        // 6.2, 6.5

        public void BuildChildTable()
        {
            Stack<int> stack = new Stack<int>();
            int lastIndex = -1;
            stack.Push(0);
            for (int i = 1; i < n; i++)
            {
                Children[i].Next = -1;
                Children[i].Up = -1;
                Children[i].Down = -1;
                while (Lcp1[i] < Lcp1[stack.Peek()])
                {
                    lastIndex = stack.Pop();
                    if ((Lcp1[i] <= Lcp1[stack.Peek()]) && (Lcp1[stack.Peek()] != Lcp1[lastIndex]))
                    {
                        Children[stack.Peek()].Down = lastIndex;
                    }
                }

                if (lastIndex != -1)
                {
                    Children[i].Up = lastIndex;
                    lastIndex = -1;
                }
                stack.Push(i);
            }
            Stack<int> stack2 = new Stack<int>();
            stack2.Push(0);
            for (int i = 1; i < n; i++)
            {
                while (Lcp1[i] < Lcp1[stack2.Peek()])
                {
                    stack2.Pop();
                }

                if (Lcp1[i] == Lcp1[stack2.Peek()])
                {
                    lastIndex = stack2.Pop();
                    Children[lastIndex].Next = i;
                }
                stack2.Push(i);
            }
        }

        //6.7

        public List<(int, int)> GetChildIntervals(int i, int j)
        {
            if (j < i) return new List<(int, int)>();
            List<(int, int)> intervals = new List<(int, int)>();
            int i1 = 0;
            if (i != -1 && i < Children[j].Up && Children[j].Up <= j)
            {
                i1 = Children[i].Next;
            }
            else if (i != -1)
            {
                i1 = Children[i].Down;
            }
            intervals.Add((i, i1 - 1));
            while (i1 != -1 && Children[i1].Next != -1)
            {
                var i2 = Children[i1].Next;
                intervals.Add((i1, i2 - 1));
                i1 = i2; 
            }
            intervals.Add((i1, j));
            return intervals;
        }




        public (int, int) GetInterval(int i, int j, char c, int ci)
        {
            int i1;
            if (i < Children[j].Up && Children[j].Up <= j)
            {
                i1 = Children[i].Next;

            }
            else
            {
                i1 = Children[i].Down;
            }
            if (S[Sa[i] + ci] == c)
            {
                return (i, i1 - 1);
            }
            //intervals.Add((i, i1 - 1));
            while (i1 != -1 && Children[i1].Next != -1)
            {
                var i2 = Children[i1].Next;
                if (S[Sa[i1] + ci] == c)
                {
                    return (i1, i2 - 1);
                }
                //intervals.Add((i1, i2 - 1));
                i1 = i2;
            }
            //intervals.Add((i1, j));
            return (i1, j);
        }

        public (int i, int j) ExactStringMatchingWithESA(string pattern)
        {
            int c = 0;
            bool queryFound = true;
            (int i, int j) = GetInterval(0, N - 1, pattern[c], c);
            while (i != -1 && j != -1 && c < pattern.Length && queryFound)
            {
                int idx = Sa[i];
                if (i != j)
                {
                    int l = GetLcp(i, j);
                    int min = Math.Min(l, pattern.Length);
                    queryFound = (i, j) != (-1, -1);
                    c = min;
                    if (c == pattern.Length) return (i, j);
                    (i, j) = GetInterval(i, j, pattern[c], c);
                }
                else
                {
                    queryFound = ComparePrefix(pattern, S.Substring(idx)) == 0;
                }
            }
            if (queryFound)
            {
                return (i, j);
            }
            else
            {
                return (-1, -1);
            }
        }

        public int GetLcp(int i, int j)
        {
            var sufi = S.Substring(Sa[i]);
            var sufj = S.Substring(Sa[j]);
            int k = 0;
            while (sufi[k] == sufj[k])
            {
                ++k;
            }
            return k;
        }

        public override IEnumerable<int> Matches(string pattern)
        {
            (int start, int end) = ExactStringMatchingWithESA(pattern);
            if ((start, end) == (-1, -1)) { return Enumerable.Empty<int>(); }
            if (start == -1) start = 0;
            if (end == -1) end = 0;
            return Sa.Take(new Range(start, end));
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            var occurrencesP1 = Matches(pattern1);
            System.Collections.Generic.HashSet<int> occurencesP2 = new(Matches(pattern2));
            List<(int, int)> occs = new List<(int, int)>();
            foreach (var occ1 in occurrencesP1)
            {
                if (occurencesP2.Contains(occ1 + pattern1.Length + x))
                {
                    occs.Add((occ1, occ1 + pattern1.Length + x));
                }
            }

            return occs;
        }

        public (int, int) GetInterval(string pattern)
        {
            int N = m_lcp.Length;
            int logn = (int)Math.Ceiling(Math.Log(N, 2));
            int[,] st = new int[N, logn];
            int left = 1, right = N;
            int maxLength = 0, maxIntervalStart = 0;

            // Initialize the sparse table with the LCPArray values for intervals of length 1
            for (int i = 0; i < N; i++)
            {
                st[i, 0] = m_lcp[i];
            }

            // Compute the sparse table using dynamic programming
            for (int j = 1; j < logn; j++)
            {
                for (int i = 0; i < N; i++)
                {
                    if (i + (1 << j) > N) break; // Out of range
                    st[i, j] = Math.Min(st[i, j - 1], st[i + (1 << (j - 1)), j - 1]);
                }
            }


            while (left <= right)
            {
                int mid = (left + right) / 2;
                bool found = false;

                // Check if there exists an interval of length mid with a maximum value of at least mid
                for (int i = 0; i < N && !found; i++)
                {
                    int j = i + mid - 1;
                    if (j < N)
                    {
                        int k = (int)Math.Log(j - i + 1, 2);
                        int lcp = Math.Min(st[i + 1, k], st[j - (1 << k) + 1, k]);
                        if (lcp >= mid)
                        {
                            found = true;
                            maxLength = mid;
                            maxIntervalStart = m_sa[i];
                        }
                    }
                }

                if (found)
                {
                    // Look for a longer interval
                    left = mid + 1;
                }
                else
                {
                    // Look for a shorter interval
                    right = mid - 1;
                }
            }
            return (maxIntervalStart, maxIntervalStart + maxLength);
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


            

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            List<(int, int)> occs = new();
            var occurrencesP1 = Matches(pattern1);

            SortedSet<int> occurencesP2 = new(Matches(pattern2));
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

        private void CrackheadEnergy(string p)
        {
            int index = BinarySearch(p);


        }

        








        private int[] m_sa;
        private int[] m_isa;
        private int[] m_lcp;
        private const int EOC = int.MaxValue;
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

        public IEnumerable<(int lcp, int l, int r)> BottomUpTraversal()
        {
            int n = m_sa.Length;
            Stack<(int, int)> stack = new();
            stack.Push((0, 0));
            for (int R = 1; R < n; R++)
            {
                int next_L = R - 1;
                while (stack.Count > 0 && m_lcp[R] < stack.Peek().Item1)
                {
                    (int d, int L) = stack.Pop();
                    yield return (d, L, R);
                    next_L = L;
                }
                if (m_lcp[R] > stack.Peek().Item1)
                    stack.Push((m_lcp[R], next_L));
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
