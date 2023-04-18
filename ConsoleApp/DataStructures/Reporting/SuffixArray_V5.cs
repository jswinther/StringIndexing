using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp.DataStructures.SuffixArrayFinal;

namespace ConsoleApp.DataStructures.Reporting
{
    internal class SuffixArray_V5 : PatternMatcher
    {
        SuffixArrayFinal SA;
        Dictionary<(int, int), IntervalNode> Tree;
        System.Collections.Generic.HashSet<(int, int)> Leaves;
        public SuffixArray_V5(string str) : base(str)
        {
            SA = new(str);
            SA.BuildChildTable();
            SA.GetAllLcpIntervals((int)Math.Log2(SA.n), out Tree, out Leaves);

            Tree.Values.Where(s => s.Size > Math.Sqrt(SA.n) && s.Children.All(e => e.Size < Math.Sqrt(SA.n)));

            var nodes = Tree.Values.ToArray();
            List<int[]> sortedLeaves = Leaves.AsParallel().Select(SA.GetOccurrencesForInterval).ToList();
            var t = Tree;
            sortedLeaves.AsParallel().ForAll(s => Array.Sort(s));
            for (int i = 1; i < sortedLeaves.Count; i++)
            {
                var node1 = sortedLeaves[i];

                for (int j = i + 1; j < sortedLeaves.Count; j++)
                {
                    var node2 = sortedLeaves[j];
                }
            }
        }

        public override IEnumerable<int> Matches(string pattern)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            throw new NotImplementedException();
        }
    }
}
