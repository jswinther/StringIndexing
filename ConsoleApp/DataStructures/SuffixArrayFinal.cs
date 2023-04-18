using C5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    internal class SuffixArrayFinal
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
        public (int Up, int Down, int Next)[] Children;

        public SuffixArrayFinal(string str) 
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
                    if (Lcp1[i] <= Lcp1[stack.Peek()] && Lcp1[stack.Peek()] != Lcp1[lastIndex])
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
            if (i != -1 && i < Children[j + 1].Up && Children[j + 1].Up <= j)
            {
                i1 = Children[j + 1].Up;
            }
            else if (i != -1)
            {
                i1 = Children[i].Down;
            }
            i = i == -1 ? 0 : i;
            intervals.Add((i, i1 - 1));
            while (i1 != -1 && Children[i1].Next != -1)
            {
                var i2 = Children[i1].Next;
                i1 = i1 == -1 ? 0 : i1;
                intervals.Add((i1, i2 - 1));
                i1 = i2;
            }
            i1 = i1 == -1 ? 0 : i1;
            intervals.Add((i1, j));
            return intervals;
        }


        //Only works on interval [0..n]

        public (int, int) GetIntervalInit(int i, int j, char c, int ci)
        {
            if (j < i) return (-1, -1);
            int i1 = Children[i].Next;

            if (S[Sa[i] + ci] == c && S[Sa[i1 - 1] + ci] == c)
            {
                return (i, i1 - 1);
            }
            //intervals.Add((i, i1 - 1));
            int i2 = -1;
            while (Children[i1].Next != -1)
            {
                i2 = Children[i1].Next;
                if (S[Sa[i1] + ci] == c && S[Sa[i2 - 1] + ci] == c)
                {
                    return (i1, i2 - 1);
                }
                //intervals.Add((i1, i2 - 1));
                i1 = i2;
            }
            //intervals.Add((i1, j));
            if (S[Sa[i1] + ci] == c)
            {
                return (i1, j);
            }
            else return (-1, -1);

        }


        public (int, int) GetInterval(int i, int j, char c, int ci)
        {
            int i1;
            if (i < Children[j + 1].Up && Children[j + 1].Up <= j)
            {
                i1 = Children[j + 1].Up;

            }
            else
            {
                i1 = Children[i].Down;
            }
            if (S[Sa[i] + ci] == c && S[Sa[i1 - 1] + ci] == c)
            {
                return (i, i1 - 1);
            }
            //intervals.Add((i, i1 - 1));
            int i2 = -1;
            while (Children[i1].Next != -1)
            {
                i2 = Children[i1].Next;
                if (S[Sa[i1] + ci] == c && S[Sa[i2 - 1] + ci] == c)
                {
                    return (i1, i2 - 1);
                }
                //intervals.Add((i1, i2 - 1));
                i1 = i2;
            }
            //intervals.Add((i1, j));
            if (S[Sa[i1] + ci] == c && S[Sa[j] + ci] == c)
            {
                return (i1, j);
            }
            else return (-1, -1);
        }

        public (int i, int j) ExactStringMatchingWithESA(string pattern)
        {
            int c = 0;
            bool queryFound = true;
            //(int i, int j) = GetInterval(0, N - 1, pattern[c], c);
            (int i, int j) = GetIntervalInit(0, n - 1, pattern[c], c);
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

        public IEnumerable<(int, int)> ReportOccurences(string pattern1, int y_min, int y_max, string pattern2, IEnumerable<int> occurrencesP1, SortedSet<int> occurencesP2)
        {
            List<(int, int)> occs = new();
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

        public class IntervalNode
        {
            public (int start, int end) Interval { get; set; }
            public int Size { get => Interval.end + 1 - Interval.start; }
            public int LeftMostLeaf { get; set; } = int.MaxValue;
            public int RightMostLeaf { get; set; } = int.MinValue;
            public IntervalNode Parent { get; set; }
            public List<IntervalNode> Children { get; set; } = new();
            public int DistanceToRoot { get; set; }

            public IntervalNode((int start, int end) interval, IntervalNode parent, int distanceToRoot)
            {
                Interval = interval;
                Parent = parent;
                DistanceToRoot = distanceToRoot;
            }

            public bool IsLeaf { get => Children.Count == 0; }
            public int[] SortedOccurrences { get; set; }
        }

        public System.Collections.Generic.HashSet<(int, int)> _leaves { get; } = new();
        public Dictionary<(int, int), IntervalNode> _nodes { get; } = new();
        public void GetAllLcpIntervals(int minSize)
        {
            System.Collections.Generic.HashSet<(int, int)> hashSet = new();
            Queue<(int, int)> intervals = new Queue<(int, int)>();
            intervals.Enqueue((0, n - 1));
            // First add child intervals for the interval [0..n]
            var Initinterval = intervals.Dequeue();
            hashSet.Add((Initinterval.Item1, Initinterval.Item2));
            _nodes.Add(Initinterval, new IntervalNode(Initinterval, null, 0));
            var currNode = _nodes[Initinterval];
            foreach (var item in GetChildIntervalsInit(Initinterval.Item1, Initinterval.Item2))
            {
                if (item != (-1, -1) && item.Item2 - item.Item1 >= minSize - 1)
                {
                    if (!hashSet.Contains(item)) intervals.Enqueue(item);
                    hashSet.Add(item);
                    var child = new IntervalNode(item, currNode, currNode.DistanceToRoot + 1);
                    currNode.Children.Add(child);
                    _nodes.Add(item, child);
                }
            }
            while (intervals.Count > 0)
            {
                var interval = intervals.Dequeue();
                hashSet.Add(interval);
                _nodes.TryGetValue(interval, out currNode);
                if (interval.Item1 == interval.Item2)
                {
                    _leaves.Add(interval);
                }
                else
                {
                    foreach (var item in GetChildIntervals(interval.Item1, interval.Item2))
                    {
                        if (item != (-1, -1) && item.Item2 - item.Item1 >= minSize - 1)
                        {

                            intervals.Enqueue(item);
                            var child = new IntervalNode(item, currNode, currNode.DistanceToRoot + 1);
                            currNode.Children.Add(child);
                            _nodes.Add(item, child);
                        }
                    }

                    if (currNode.Children.Count == 0)
                    {
                        _leaves.Add(currNode.Interval);
                    }
                }
                
            }
        }

        




        //Only works on interval [0..n]
        public List<(int, int)> GetChildIntervalsInit(int i, int j)
        {
            if (j < i) return new List<(int, int)>();
            List<(int, int)> intervals = new List<(int, int)>();
            int i1 = Children[i].Next;
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

        // expects two sorted arrays
        public IEnumerable<(int, int)> FindFirstOccurrenceForEachPattern1Occurrence(string pattern1, int y_min, int y_max, string pattern2, int[] occs1, int[] occs2)
        {
            List<(int, int)> occs = new List<(int, int)>();
            int k = 0;
            for (int i = 0; i < occs1.Length; i++)
            {
                int occ1 = occs1[i];

                int min = occ1 + pattern1.Length + y_min;
                int max = occ1 + pattern1.Length + y_max;

                for (int j = k; j < occs2.Length; j++)
                {
                    int occ2 = occs2[j];
                    if (min <= occ2 && occ2 <= max)
                    {
                        occs.Add((occ1, occs2[j] - occ1 + pattern2.Length));
                        break;
                    }
                    else
                    {
                        k++;
                    }
                }
            }
            return occs;
        }

        public int[] GetOccurrencesForPattern(string pattern)
        {
            (int start, int end) = ExactStringMatchingWithESA(pattern);
            return GetOccurrencesForInterval(start, end);
        }

        public int[] GetOccurrencesForInterval((int start, int end) interval)
        {
            return GetOccurrencesForInterval(interval.start, interval.end);
        }

        public int[] GetOccurrencesForInterval(int start, int end)
        {
            int[] occurrences = new int[end + 1 - start];
            for (int i = 0; i < occurrences.Length; i++)
            {
                occurrences[i] = Sa[i + start];
            }
            return occurrences;
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
}
