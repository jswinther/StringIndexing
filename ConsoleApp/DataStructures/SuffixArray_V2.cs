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

        

        public override IEnumerable<int> Matches(string pattern)
        {
            /* Den rigtige med GetInterval og Childtable!!!! */
            (int start, int end) = ExactStringMatchingWithESA(pattern);
            if ((start, end) == (-1, -1)) { return Enumerable.Empty<int>(); }
            if (start == -1) start = 0;
            if (end == -1) end = 0;
            return Sa.Take(new Range(start, end + 1));

            /*Den fake med precomputed intervaller 
            IntervalFinder = new IntervalFinder(pattern, S);
            (int start, int end) = IntervalFinder.GetInterval();
            return Sa.Take(new Range(start, end));*/
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

        


    }


    
}
