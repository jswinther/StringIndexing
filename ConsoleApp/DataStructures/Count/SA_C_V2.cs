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

        public SA_C_V2(string str, int x, int ymin, int ymax) : base(str, x, ymin, ymax)
        {
            SA = new SuffixArrayFinal(str);
            SA.BuildChildTable();
            SA.GetAllLcpIntervals(1, out Tree, out Leaves, out Root);
            variableLeafPairs = new int[Leaves.Count][];
            for (int lcpPos = 0; lcpPos < Leaves.Count; lcpPos++)
            {
                List<int> ints = new List<int>();
                var suffPos = SA[lcpPos];

                for (int realOffset = ymin; realOffset <= ymax; realOffset++)
                {
                    if (realOffset + suffPos >= SA.m_isa.Length) break;
                    ints.Add(Math.Abs(SA.m_isa[suffPos + realOffset]));
                    
                }
                variableLeafPairs[lcpPos] = ints.ToArray().Sort();
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
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
            var occs2 = new HashSet<int>(SA.GetOccurrencesForPattern(pattern2));

            foreach (var occ1 in occs1)
            {
                if (occs2.Contains(occ1 + pattern1.Length + x))
                    count++;
            }
            return count;
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
