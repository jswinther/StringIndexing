namespace ConsoleApp.DataStructures
{
    public class SuffixArrayFinal
    {
        public static int? k = 0;
        public int? n { get; set; }
        public int[]? m_sa { get; set; }
        public int[]? m_isa { get; set; }
        public int[]? m_lcp { get; set; }
        public string? m_str { get; set; }
        public int[]? m_ct_up { get; set; }
        public int[]? m_ct_down { get; set; }
        public int[]? m_ct_next { get; set; }
        public Dictionary<char, int>? m_chainHeadsDict = new(new CharComparer());
        public List<Chain>? m_chainStack = new();
        public List<Chain>? m_subChains = new();
        public int m_nextRank = 1;

        public SuffixArrayFinal()
        {
            
        }

        public static SuffixArrayFinal CreateSuffixArray(string str)
        {
            return new SuffixArrayFinal(str);
        }

        private SuffixArrayFinal(string str)
        {
            m_str = str + "|";
            n = m_str.Length;
            m_sa = new int[n.Value];
            m_isa = new int[n.Value];
            m_ct_up = new int[n.Value];
            m_ct_down = new int[n.Value];
            m_ct_next = new int[n.Value];
            
            do
            {
                n /= 10;
                ++k;
            } while (n != 0);
            n = m_str.Length;
            FormInitialChains();
            BuildSuffixArray();
            BuildLcpArray();
            //BuildChildTable();
        }

        #region Construction
        private void BuildLcpArray()
        {
            m_lcp = new int[m_sa.Length + 1];
            m_lcp[0] = m_lcp[m_sa.Length] = 0;

            for (int i = 1; i < m_sa.Length; i++)
            {
                m_lcp[i] = CalcLcp(m_sa[i - 1], m_sa[i]);
            }
        }

        private void BuildSuffixArray()
        {
            while (m_chainStack.Count > 0)
            {
                // Pop chain
                Chain chain = m_chainStack[m_chainStack.Count - 1];
                m_chainStack.RemoveAt(m_chainStack.Count - 1);

                if (m_isa[chain.head] == int.MaxValue)
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

        public void CalculateSubTreeSize(IntervalNode root)
        {
            root.SubtreeSize = Update(root);
        }

        private int CalcLcp(int i, int j)
        {
            int lcp;
            int maxIndex = m_str.Length - Math.Max(i, j);       // Out of bounds prevention
            for (lcp = 0; lcp < maxIndex && m_str[i + lcp] == m_str[j + lcp]; lcp++) ;
            return lcp;
        }

        private void ExtendChain(Chain chain)
        {
            char sym = m_str[chain.head + chain.length];
            if (m_chainHeadsDict.ContainsKey(sym))
            {
                // Continuation of an existing chain, this is the leftmost
                // occurence currently known (others may come up later)
                m_isa[m_chainHeadsDict[sym]] = chain.head;
                m_isa[chain.head] = int.MaxValue;
            }
            else
            {
                // This is the beginning of a new subchain
                m_isa[chain.head] = int.MaxValue;
                Chain newChain = new Chain(m_str);
                newChain.head = chain.head;
                newChain.length = chain.length + 1;
                m_subChains.Add(newChain);
            }
            // m_save index in case we find a continuation of this chain
            m_chainHeadsDict[sym] = chain.head;
        }

        private void FindInitialChains()
        {
            // Scan the string left to right, keeping rightmost occurences of characters as the chain heads
            for (int i = 0; i < m_str.Length; i++)
            {
                if (m_chainHeadsDict.ContainsKey(m_str[i]))
                {
                    m_isa[i] = m_chainHeadsDict[m_str[i]];
                }
                else
                {
                    m_isa[i] = int.MaxValue;
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
            m_ct_up = new int[n.Value];
            m_ct_down = new int[n.Value];
            m_ct_next = new int[n.Value];
            Stack<int> stack = new Stack<int>();
            int lastIndex = -1;
            stack.Push(0);
            for (int i = 1; i < n; i++)
            {
                m_ct_next[i] = -1;
                m_ct_up[i] = -1;
                m_ct_down[i] = -1;
                while (m_lcp[i] < m_lcp[stack.Peek()])
                {
                    lastIndex = stack.Pop();
                    if (m_lcp[i] <= m_lcp[stack.Peek()] && m_lcp[stack.Peek()] != m_lcp[lastIndex])
                    {
                        m_ct_down[stack.Peek()] = lastIndex;
                    }
                }

                if (lastIndex != -1)
                {
                    m_ct_up[i] = lastIndex;
                    lastIndex = -1;
                }
                stack.Push(i);
            }
            Stack<int> stack2 = new Stack<int>();
            stack2.Push(0);
            for (int i = 1; i < n; i++)
            {
                while (m_lcp[i] < m_lcp[stack2.Peek()])
                {
                    stack2.Pop();
                }

                if (m_lcp[i] == m_lcp[stack2.Peek()])
                {
                    lastIndex = stack2.Pop();
                    m_ct_next[lastIndex] = i;
                }
                stack2.Push(i);
            }
        }

        //6.7

        private List<(int, int)> GetChildIntervals(int i, int j)
        {
            if (j < i) return new List<(int, int)>();
            if (j + 1 == n) return new List<(int, int)>();
            List<(int, int)> intervals = new List<(int, int)>();
            int i1 = 0;
            if (i != -1 && i < m_ct_up[j + 1] && m_ct_up[j + 1] <= j)
            {
                i1 = m_ct_up[j + 1];
            }
            else if (i != -1)
            {
                i1 = m_ct_down[i];
            }
            i = i == -1 ? 0 : i;
            intervals.Add((i, i1 - 1));
            while (i1 != -1 && m_ct_next[i1] != -1)
            {
                var i2 = m_ct_next[i1];
                i1 = i1 == -1 ? 0 : i1;
                intervals.Add((i1, i2 - 1));
                i1 = i2;
            }
            i1 = i1 == -1 ? 0 : i1;
            intervals.Add((i1, j));
            return intervals;
        }


        //Only works on interval [0..n]

        private (int, int) GetIntervalInit(int i, int j, char c, int ci)
        {
            if (j < i) return (-1, -1);
            int i1 = m_ct_next[i];

            if (m_str[m_sa[i] + ci] == c && m_str[m_sa[i1 - 1] + ci] == c)
            {
                return (i, i1 - 1);
            }
            //intervals.Add((i, i1 - 1));
            int i2 = -1;
            while (m_ct_next[i1] != -1)
            {
                i2 = m_ct_next[i1];
                if (m_str[m_sa[i1] + ci] == c && m_str[m_sa[i2 - 1] + ci] == c)
                {
                    return (i1, i2 - 1);
                }
                //intervals.Add((i1, i2 - 1));
                i1 = i2;
            }
            //intervals.Add((i1, j));
            if (m_str[m_sa[i1] + ci] == c)
            {
                return (i1, j);
            }
            else return (-1, -1);

        }


        private (int, int) GetInterval(int i, int j, char c, int ci)
        {
            int i1;
            if (i < m_ct_up[j + 1] && m_ct_up[j + 1] <= j)
            {
                i1 = m_ct_up[j + 1];

            }
            else
            {
                i1 = m_ct_down[i];
            }
            if (m_str[m_sa[i] + ci] == c && m_str[m_sa[i1 - 1] + ci] == c)
            {
                return (i, i1 - 1);
            }
            //intervals.Add((i, i1 - 1));
            int i2 = -1;
            while (m_ct_next[i1] != -1)
            {
                i2 = m_ct_next[i1];
                if (m_str[m_sa[i1] + ci] == c && m_str[m_sa[i2 - 1] + ci] == c)
                {
                    return (i1, i2 - 1);
                }
                //intervals.Add((i1, i2 - 1));
                i1 = i2;
            }
            //intervals.Add((i1, j));
            if (m_str[m_sa[i1] + ci] == c && m_str[m_sa[j] + ci] == c)
            {
                return (i1, j);
            }
            else return (-1, -1);
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
            m_sa[m_nextRank - 1] = index;
            m_nextRank++;
        }

        private void RefineChains(Chain chain)
        {
            m_chainHeadsDict.Clear();
            m_subChains.Clear();
            while (chain.head != int.MaxValue)
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
            List<SuffixRank> notedSuffixes = new List<SuffixRank>();
            m_chainHeadsDict.Clear();
            m_subChains.Clear();

            while (chain.head != int.MaxValue)
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

        private void SortAndRankNotedSuffixes(List<SuffixRank> notedSuffixes)
        {
            notedSuffixes.Sort(new SuffixRankComparer());
            // Rank sorted noted suffixes 
            for (int i = 0; i < notedSuffixes.Count; ++i)
            {
                RankSuffix(notedSuffixes[i].head);
            }
        }
        #endregion

        #region public
        public IEnumerable<int> BinaryMatches(string pattern)
        {
            List<int> occurrences = new List<int>();
            // Find the first occurrence of the substring in the text
            (int i, int j) = BinarySearch(pattern);
            if (i == -1 && j == -1) return Enumerable.Empty<int>();
            return m_sa[i..(j + 1)];
        }

        public (int, int) BinarySearch(string pattern)
        {
            int lo = 0;
            int hi = n.Value;
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
                    int i, j;
                    for (j = mid + 1; j < n && m_lcp[j] >= pattern.Length; j++) ;
                    for (i = mid - 1; i >= 0 && m_lcp[i + 1] >= pattern.Length; i--)
                        return (i, j);
                }
            }
            return (-1, -1);
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


        public (int i, int j) ExactStringMatchingWithESA(string pattern)
        {
            
            int c = 0;
            bool queryFound = true;
            //(int i, int j) = GetInterval(0, N - 1, pattern[c], c);
            (int i, int j) = GetIntervalInit(0, n.Value - 1, pattern[c], c);
            while (i != -1 && j != -1 && c < pattern.Length && queryFound)
            {
                int idx = m_sa[i];
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
                    queryFound = ComparePrefix(pattern, m_str.Substring(idx)) == 0;
                    if (queryFound) break;
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

        public void GetAllLcpIntervals(double minSize, out Dictionary<(int, int), IntervalNode> _nodes, out Dictionary<(int, int), IntervalNode> _leaves, out IntervalNode root)
        {

            _nodes = new();
            _leaves = new();
            HashSet<(int, int)> hashSet = new();
            Queue<(int, int)> intervals = new Queue<(int, int)>();
            intervals.Enqueue((0, n.Value - 1));
            // First add child intervals for the interval [0..n]
            var Initinterval = intervals.Dequeue();
            hashSet.Add((Initinterval.Item1, Initinterval.Item2));
            root = new IntervalNode(Initinterval, null, 0);
            _nodes.Add(Initinterval, root);
            var currNode = _nodes[Initinterval];
            foreach (var item in GetChildIntervalsInit(Initinterval.Item1, Initinterval.Item2))
            {
                if (item != (-1, -1) && item.Item2 - item.Item1 >= minSize - 1)
                {
                    if (!hashSet.Contains(item)) intervals.Enqueue(item);
                    hashSet.Add(item);
                    var child = new IntervalNode(item, currNode, currNode.DistanceToRoot + 1);
                    currNode.Children.Add(child);
                    var occs = GetOccurrencesForInterval(item);
                    currNode.Min = occs.Min();
                    currNode.Max = occs.Max();
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
                    _leaves.Add(interval, currNode);
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
                        _leaves.Add(currNode.Interval, currNode);
                    }
                }

            }
            
        }

        public int[] SinglePattern(string pattern)
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
            return m_sa[start..(end + 1)];
        }

        /*
        public int GetLcp(int i, int j)
        {
            var sufi = m_str.Substring(m_sa[i]);
            var sufj = m_str.Substring(m_sa[j]);
            int minLength = Math.Min(sufi.Length, sufj.Length);
            int k = 0;
            while (k < minLength && sufi[k] == sufj[k])
            {
                ++k;
            }
            return k;
        }
        */

        public int GetLcp(int i, int j)
        {
            if (i < m_ct_up[j + 1] && m_ct_up[j + 1] <= j)
            {
                return m_lcp[m_ct_up[j + 1]];
            }
            else return m_lcp[m_ct_down[i]];
        }

        //Only works on interval [0..n]
        public List<(int, int)> GetChildIntervalsInit(int i, int j)
        {
            if (j < i) return new List<(int, int)>();
            List<(int, int)> intervals = new List<(int, int)>();
            int i1 = m_ct_next[i];
            intervals.Add((i, i1 - 1));
            while (m_ct_next[i1] != -1)
            {
                var i2 = m_ct_next[i1];
                intervals.Add((i1, i2 - 1));
                i1 = i2;

            }
            intervals.Add((i1, j));
            return intervals;

        }

        public int Update(IntervalNode node)
        {
            if (node.Children.Count == 0) return 1;
            int sum = 0;
            foreach (var child in node.Children)
            {
                child.SubtreeSize = Update(child);
                sum += child.SubtreeSize;
            }
            return sum + 1;
        }
        #endregion
    }

    #region HelperClasses
    [Serializable]
    public class Chain : IComparable<Chain>
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
