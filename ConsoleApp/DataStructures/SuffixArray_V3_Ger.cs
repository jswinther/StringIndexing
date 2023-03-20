using C5;
using System;
using System.Collections;
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
        public int[] SA { get; private set; }
        public int[] LCP { get; private set; }
        public string BWT { get; }
        public int[] ISA { get; }
        public string[] Suffixes { get; private set; }
        public (int Up, int Down, int Next)[] Children { get; }

        public SuffixArray_V3_Ger(string S) : base(S)
        {
            S += "|";
            n = S.Length;
            this.S = S;
            SA = new int[n];
            ISA = new int[n];
            Suffixes = new string[n];
            Children = new (int Up, int Down, int Next)[n];
            FormInitialChains();
            BuildSuffixArray();
            BuildLcpArray();
            BuildChildTable();
        }

    

        #region Building the LCP Intervals and Other Stuff

        private class CompareSuffixes : IComparer<string?>
        {
            public int Compare(string? x, string? y)
            {
                return CompareStrings(x, y);
            }

            public int CompareStrings(string x, string y)
            {
                int min = Math.Min(x.Length, y.Length);
                int i = 0;
                while (i < min)
                {
                    if (x[i] < y[i])
                        return 1;
                    if (x[i] > y[i])
                        return -1;
                    i++;
                }
                if (x.Length > y.Length)
                    return -1;
                if (y.Length > x.Length)
                    return 1;
                return 0;
            }
        }

        #endregion


        #region PatternMatcher
        public override IEnumerable<int> Matches(string pattern)
        {
            var a = ExactStringMatchingWithESA(pattern);
            if (a == (-1, -1)) return Enumerable.Empty<int>();
            List<int> occs = new List<int>();
            for (int i = a.i; i <= a.j; i++)
            {
                occs.Add(SA[i]);
            }
            //int substringOccurrence = FindIndexOfFirstOccurrence(pattern);
            //if (substringOccurrence < 0) return Enumerable.Empty<int>();
            //return AddOccurrences(substringOccurrence, pattern);
            return occs;
        }

        private IEnumerable<int> AddOccurrences(int substringOccurrence, string pattern)
        {
            yield return SA[substringOccurrence];

            // Check all suffixes that come after the first occurrence of the substring
            for (int i = substringOccurrence + 1; i < n && LCP[i - 1] >= pattern.Length; i++)
            {
                yield return SA[i];
            }

            // Check all suffixes that come before the first occurrence of the substring
            for (int i = substringOccurrence - 1; i >= 0 && LCP[i] >= pattern.Length; i--)
            {
                yield return SA[i];
            }
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            int substringOccurrence = FindIndexOfFirstOccurrence(pattern2);
            if (substringOccurrence < 0) return Enumerable.Empty<(int, int)>();
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
            int substringOccurrence = FindIndexOfFirstOccurrence(pattern2);
            if (substringOccurrence < 0) return Enumerable.Empty<(int, int)>();
            List<(int, int)> occs = new List<(int, int)>();
            var pattern1Occurrences = Matches(pattern1);
            var pattern2Occurrences = new SortedSet<int>(Matches(pattern2));
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
        #endregion


        #region Base Construction
        private const int EOC = int.MaxValue;
        private C5.HashDictionary<char, int> m_chainHeadsDict = new HashDictionary<char, int>(new CharComparer());
        private List<Chain> m_chainStack = new List<Chain>();
        private ArrayList<Chain> m_subChains = new ArrayList<Chain>();
        private int m_nextRank = 1;
        private void FormInitialChains()
        {
            // Link all suffixes that have the same first character
            FindInitialChains();
            SortAndPushSubchains();
        }

        private void FindInitialChains()
        {
            // Scan the string left to right, keeping rightmost occurences of characters as the chain heads
            for (int i = 0; i < S.Length; i++)
            {
                if (m_chainHeadsDict.Contains(S[i]))
                {
                    ISA[i] = m_chainHeadsDict[S[i]];
                }
                else
                {
                    ISA[i] = EOC;
                }
                m_chainHeadsDict[S[i]] = i;
            }

            // Prepare chains to be pushed to stack
            foreach (int headIndex in m_chainHeadsDict.Values)
            {
                Chain newChain = new Chain(S);
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

                if (ISA[chain.head] == EOC)
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
            char sym = S[chain.head + chain.length];
            if (m_chainHeadsDict.Contains(sym))
            {
                // Continuation of an existing chain, this is the leftmost
                // occurence currently known (others may come up later)
                ISA[m_chainHeadsDict[sym]] = chain.head;
                ISA[chain.head] = EOC;
            }
            else
            {
                // This is the beginning of a new subchain
                ISA[chain.head] = EOC;
                Chain newChain = new Chain(S);
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
                int nextIndex = ISA[chain.head];
                if (chain.head + chain.length > S.Length - 1)
                {
                    // If this substring reaches end of string it cannot be extended.
                    // At this point it's the first in lexicographic order so it's safe
                    // to just go ahead and rank it.
                    RankSuffix(chain.head);
                }
                else if (ISA[chain.head + chain.length] < 0)
                {
                    SuffixRank sr = new SuffixRank();
                    sr.head = chain.head;
                    sr.rank = -ISA[chain.head + chain.length];
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
            ISA[index] = -m_nextRank;
            SA[m_nextRank - 1] = index;
            Suffixes[m_nextRank - 1] = S.Substring(index);
            m_nextRank++;
        }

        public void ComputeLCP()
        {
            int n = S.Length;
            LCP = new int[n];
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

                LCP[rank[i]] = k;

                if (k > 0)
                {
                    k--;
                }
            }
        }

        private void BuildLcpArray()
        {
            LCP = new int[SA.Length];
            LCP[0] = LCP[SA.Length - 1] = 0;

            for (int i = 1; i < SA.Length; i++)
            {
                LCP[i] = CalcLcp(SA[i - 1], SA[i]);
            }
        }

        private int CalcLcp(int i, int j)
        {
            int lcp;
            int maxIndex = S.Length - Math.Max(i, j);       // Out of bounds prevention
            for (lcp = 0; (lcp < maxIndex) && (S[i + lcp] == S[j + lcp]); lcp++) ;
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
                while (LCP[i] < LCP[stack.Peek()])
                {
                    lastIndex = stack.Pop();
                    if ((LCP[i] <= LCP[stack.Peek()]) && (LCP[stack.Peek()] != LCP[lastIndex]))
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
                while (LCP[i] < LCP[stack2.Peek()])
                {
                    stack2.Pop();
                }

                if (LCP[i] == LCP[stack2.Peek()])
                {
                    lastIndex = stack2.Pop();
                    Children[lastIndex].Next = i;
                }
                stack2.Push(i);
            }
        }

        //6.7

        private List<(int, int)> GetChildIntervals(int i, int j)
        {
            List<(int, int)> intervals = new List<(int, int)>();
            int i1;
            if (i < Children[j + 1].Up && Children[j + 1].Up <= j)
            {
                i1 = Children[j + 1].Up;
            }
            else
            {
                i1 = Children[(j + 1)].Down;
            }
            intervals.Add((i, i1 - 1));
            while (Children[i1].Next != -1)
            {
                var i2 = Children[i1].Next;
                intervals.Add((i1, i2 - 1));
                i1 = i2;
            }
            intervals.Add((i1, j));
            return intervals;
        }

        


        private (int, int) GetInterval(int i, int j, char c, int ci)
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
            if (S[SA[i] + ci] == c)
            {
                return (i, i1 - 1);
            }
            //intervals.Add((i, i1 - 1));
            while (Children[i1].Next != -1)
            {
                var i2 = Children[i1].Next;
                if (S[SA[i1] + ci] == c)
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
            (int i, int j) = GetInterval(0, n - 1, pattern[c], c);
            while (i != -1 && j != -1 && c < pattern.Length && queryFound)
            {
                int idx = SA[i];
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
                    queryFound = ComparePrefix(pattern, S.Substring(SA[i])) == 0;
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
            var sufi = S.Substring(SA[i]);
            var sufj = S.Substring(SA[j]);
            int k = 0;
            while (sufi[k] == sufj[k])
            {
                ++k;
            }
            return k;
        }

        public void Traverse()
        {
            Stack<Interval> stack = new();
            Interval lastInterval = null;
            stack.Push(new(0, 0, int.MaxValue, null));
            for (int i = 1; i < n; i++)
            {
                int leftBoundary = i - 1;
                while (LCP[i] < stack.Peek().LCP)
                {
                    stack.Peek().RB = i - 1;
                    lastInterval = stack.Pop();
                    Process(lastInterval);
                    if (LCP[i] <= stack.Peek().LCP)
                    {
                        stack.Peek().Children.Add(lastInterval);
                        lastInterval = null;
                    }
                }
                if (LCP[i] <= stack.Peek().LCP)
                {
                    if (lastInterval == null) stack.Push(new(LCP[i], leftBoundary, int.MaxValue, null));
                    else
                    {
                        stack.Push(new Interval(LCP[i], leftBoundary, int.MaxValue, lastInterval));
                        lastInterval = null;
                    }
                }
            }

        }

        private void Process(Interval interval)
        {

        }

        private class Interval
        {
            public int LCP { get; set; }
            public int LB { get; set; }
            public int RB { get; set; }
            public List<Interval> Children { get; set; }

            public Interval(int lCP, int lB, int rB, params Interval[] children)
            {
                LCP = lCP;
                LB = lB;
                RB = rB;
                if (children != null)
                {
                    Children = new List<Interval>(children);
                }
                else
                {
                    Children = new List<Interval>();
                }
            }
        }

        public void ConstructChildren()
        {

        }
        #endregion









    }
}
