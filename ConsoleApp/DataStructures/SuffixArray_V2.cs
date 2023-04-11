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
    internal class SuffixArray_V2 : BaseSuffixArray
    {
        
        public (int Up, int Down, int Next)[] Children { get; }
        public SuffixArray_V2(string str) : base(str)
        {
            
            Children = new (int Up, int Down, int Next)[n];
            BuildChildTable();
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
            
            if (S[Sa[i] + ci] == c && S[Sa[i1-1] + ci] == c)
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
            if (i < Children[j+1].Up && Children[j+1].Up <= j)
            {
                i1 = Children[j+1].Up;

            }
            else
            {
                i1 = Children[i].Down;
            }
            if (S[Sa[i] + ci] == c && S[Sa[i1-1] + ci] == c)
            { 
                return (i, i1 - 1);
            }
            //intervals.Add((i, i1 - 1));
            int i2 = -1;
            while (Children[i1].Next != -1)
            {
                i2 = Children[i1].Next;
                if (S[Sa[i1] + ci] == c && S[Sa[i2-1] + ci] == c)
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
            return GetOccurrencesForPattern(pattern);
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
            var occurrencesP1 = Matches(pattern1);
            SortedSet<int> occurrencesP2 = new(Matches(pattern2));
            return ReportOccurences(pattern1, y_min, y_max, pattern2, occurrencesP1, occurrencesP2);
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

        public IEnumerable<(int, int)> ReportOccurences(string pattern1, int y_min, int y_max, string pattern2, IEnumerable<int> occurrencesP1, int[] occurencesP2)
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
            public int LeftMostLeaf { get; set; } = int.MaxValue;
            public int RightMostLeaf { get; set; } = int.MinValue;
            public (int, int) Parent { get; set; }
            public List<(int, int)> Children { get; set; } = new();

            public IntervalNode((int start, int end) interval, (int start, int end) parent)
            {
                Interval = interval;
                Parent = parent;
            }

            public bool IsLeaf { get => Children.Count == 0; }
            public int[] SortedOccurrences { get; set; }
        }

        public System.Collections.Generic.HashSet<(int, int)> _leaves { get; } = new();
        public Dictionary<(int, int), IntervalNode> _nodes { get; } = new();

        public void GetAllLcpIntervals()
        {
            System.Collections.Generic.HashSet<(int, int)> hashSet = new();
            Queue<(int, int)> intervals = new Queue<(int, int)>();
            intervals.Enqueue((0, n - 1));
            // First add child intervals for the interval [0..n]
            var Initinterval = intervals.Dequeue();
            hashSet.Add((Initinterval.Item1, Initinterval.Item2));
            _nodes.Add(Initinterval, new IntervalNode(Initinterval, (-1, -1)));
            var currNode = _nodes[Initinterval];
            foreach (var item in GetChildIntervalsInit(Initinterval.Item1, Initinterval.Item2))
            {
                if (item != (-1, -1) && item.Item2 - item.Item1 > 0)
                {

                    if (!hashSet.Contains(item)) intervals.Enqueue(item);
                    hashSet.Add(item);
                    currNode.Children.Add(item);
                    _nodes.Add(item, new IntervalNode(item, currNode.Interval));
                }
            }
            while (intervals.Count > 0)
            {
                var interval = intervals.Dequeue();
                if (interval.Item1 == interval.Item2) hashSet.Add((interval.Item1, interval.Item2));
                else
                {
                    hashSet.Add(interval);
                    _nodes.TryGetValue(interval, out currNode);
                    foreach (var item in GetChildIntervals(interval.Item1, interval.Item2))
                    {
                        if (item != (-1, -1) && item.Item2 - item.Item1 > 0)
                        {

                            if (!hashSet.Contains(item))
                            {
                                intervals.Enqueue(item);
                                currNode.Children.Add(item);
                                _nodes.Add(item, new IntervalNode(item, currNode.Interval));
                            }
                            hashSet.Add(item);

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

        


    }

    
}
