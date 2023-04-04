using C5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    // Inspired by this paper https://www.uni-ulm.de/fileadmin/website_uni_ulm/iui.inst.190/Mitarbeiter/ohlebusch/PAPERS/HCMB8.pdf
    internal class SuffixArray_V3_Ger : PatternMatcher
    {
        public int n { get; }
        public string[] Suffixes { get; private set; }
        public (int Up, int Down, int Next)[] Children { get; }
        private SuffixArray_V2 SuffixArray_V2 { get; set; }
        private Dictionary<(int, int), SortedSet<int>> sorted = new();

        public SuffixArray_V3_Ger(string S) : base(S)
        {
            
            SuffixArray_V2 = new SuffixArray_V2 (S);
            n = SuffixArray_V2.S.Length;
            
            Children = new (int Up, int Down, int Next)[n];
            BuildChildTable();

            System.Collections.Generic.HashSet<(int, int)> hashSet = new();
            Queue<(int, int)> intervals = new Queue<(int, int)> ();
            intervals.Enqueue((0, n - 1));
            // First add child intervals for the interval [0..n]
            var Initinterval = intervals.Dequeue();
            hashSet.Add((Initinterval.Item1, Initinterval.Item2));
            foreach (var item in GetChildIntervalsInit(Initinterval.Item1, Initinterval.Item2))
            {
                if (item != (-1, -1) && item.Item2 - item.Item1 > 0)
                {

                    if (!hashSet.Contains(item)) intervals.Enqueue(item);
                    hashSet.Add(item);

                }
            }
            while (intervals.Count > 0)
            {
                var interval = intervals.Dequeue();
                if (interval.Item1 == interval.Item2) hashSet.Add((interval.Item1, interval.Item2));
                else
                {
                    hashSet.Add(interval);
                    foreach (var item in GetChildIntervals(interval.Item1, interval.Item2))
                    {
                        if (item != (-1, -1) && item.Item2 - item.Item1 > 0)
                        {
                            
                            if (!hashSet.Contains(item)) intervals.Enqueue(item);
                            hashSet.Add(item);

                        }
                    }
                }
                
            }
            Dictionary<(int, int), SortedSet<int>> dic = new();
            foreach (var interval in hashSet)
            {
                var originalPlacesOfSuffixes = SA.Take(new Range(new Index(interval.Item1 == -1 ? 0 : interval.Item1), new Index(interval.Item2 + 1)));
                dic.Add(interval, new SortedSet<int>(originalPlacesOfSuffixes));
            }
            sorted = dic;
        }

        public int[] LCP { get => SuffixArray_V2.Lcp1; }
        public int[] SA { get => SuffixArray_V2.Sa; }
        public string S { get => SuffixArray_V2.S; }
        #region PatternMatcher
        public override IEnumerable<int> Matches(string pattern)
        {
            var interval = SuffixArray_V2.ExactStringMatchingWithESA(pattern);
            return sorted[interval];
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
       


        #region Base Construction

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


        //Only works on interval [0..n]
        private List<(int, int)> GetChildIntervalsInit(int i, int j)
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


            //6.7

            private List<(int, int)> GetChildIntervals(int i, int j)
        {
            if (j < i) return new List<(int, int)>();
            List<(int, int)> intervals = new List<(int, int)>();
            int i1 = 0;
            if (i != -1 && i < Children[j+1].Up && Children[j+1].Up <= j)
            {
                i1 = Children[j+1].Up;
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
            while (i1 != -1 && Children[i1].Next != -1)
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
