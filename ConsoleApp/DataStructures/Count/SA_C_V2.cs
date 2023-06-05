using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Count
{
    internal class SA_C_V2 : CountDataStructure
    {
        SuffixArrayFinal SA;
        private Dictionary<(int, int), IntervalNode> Tree;
        private Dictionary<(int, int), IntervalNode> Leaves;
        private IntervalNode Root;

        // key = start of LCP interval
        private int[][] variableLeafPairs;
        private int[] fixedLeafPairs;

        public SA_C_V2(string str, int x, int ymin, int ymax) : base(str, x, ymin, ymax)
        {
            SA = SuffixArrayFinal.CreateSuffixArray(str);
            
            //SA.GetAllLcpIntervals(1, out Tree, out Leaves, out Root);
            int n = SA.n.Value;
            variableLeafPairs = new int[n][];
            fixedLeafPairs = new int[n];
            for (int lcpPos = 0; lcpPos < n; lcpPos++)
            {
                List<int> variable = new List<int>();
                var suffPos = SA.m_sa[lcpPos];

                for (int realOffset = ymin; realOffset <= ymax; realOffset++)
                {
                    if (realOffset + suffPos >= SA.m_isa.Length) break;
                    variable.Add(Math.Abs(SA.m_isa[suffPos + realOffset]));
                    
                }
                variableLeafPairs[lcpPos] = variable.ToArray().Sort();
                if (suffPos + x < SA.m_isa.Length)
                    fixedLeafPairs[lcpPos] = Math.Abs(SA.m_isa[suffPos + x]);
            }
        }

        public SA_C_V2(SuffixArrayFinal str, int x, int ymin, int ymax) : base(str, x, ymin, ymax)
        {
            SA = str;

            //SA.GetAllLcpIntervals(1, out Tree, out Leaves, out Root);
            int n = SA.n.Value;
            variableLeafPairs = new int[n][];
            fixedLeafPairs = new int[n];
            for (int lcpPos = 0; lcpPos < n; lcpPos++)
            {
                List<int> variable = new List<int>();
                var suffPos = SA.m_sa[lcpPos];

                for (int realOffset = ymin; realOffset <= ymax; realOffset++)
                {
                    if (realOffset + suffPos >= SA.m_isa.Length) break;
                    variable.Add(Math.Abs(SA.m_isa[suffPos + realOffset]));

                }
                variableLeafPairs[lcpPos] = variable.ToArray().Sort();
                if (suffPos + x < SA.m_isa.Length)
                    fixedLeafPairs[lcpPos] = Math.Abs(SA.m_isa[suffPos + x]);
            }
        }

        public override int Matches(string pattern)
        {
            (var a, var b) = SA.ExactStringMatchingWithESA(pattern);
            return b - a + 1;
        }

        public override int MatchesFixed(string pattern1, string pattern2)
        {
            int count = 0;
            var int1 = SA.ExactStringMatchingWithESA(pattern1);
            var int2 = SA.ExactStringMatchingWithESA(pattern2);
            var occs = fixedLeafPairs[int1.i..(int1.j + 1)];
            return SizeOfInterval(occs.BinarySearchOnRange(int2.i, int2.j));

        }

        public override int MatchesVariable(string pattern1, string pattern2)
        {
            int count = 0;
            var int1 = SA.ExactStringMatchingWithESA(pattern1);
            var int2 = SA.ExactStringMatchingWithESA(pattern2);

            for (var i = int1.i; i < int1.j; i++)
            {
                count += SizeOfInterval(variableLeafPairs[i].BinarySearchOnRange(int2.i, int2.j));
            }
            return count;

        }

        public static int SizeOfInterval((int, int) interval)
        {
            return interval.Item2 + 1 - interval.Item1;
        }
    }
}
