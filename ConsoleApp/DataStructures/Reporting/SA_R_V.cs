using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp.DataStructures.SuffixArrayFinal;

namespace ConsoleApp.DataStructures.Reporting
{
    internal class SA_R_V : ReportDataStructure
    {
        SuffixArrayFinal SA;
        Dictionary<(int, int), IntervalNode> Tree;
        Dictionary<(int, int), IntervalNode> Leaves;
        IntervalNode Root;
        public SA_R_V(string str) : base(str)
        {
            SA = new(str);
            
            SA.GetAllLcpIntervals(0, out Tree, out Leaves, out Root);
            int x = 5;
            foreach (((int start, int end), IntervalNode leafNode) in Leaves)
            {
                int depth = leafNode.DistanceToRoot;
                int left = start - depth - x;
                int right = start + depth + x;
                if (left >= 0)
                {
                    leafNode.MatchingIntervals.Add((left, left));
                }
                if (right < Leaves.Count)
                {
                    leafNode.MatchingIntervals.Add((right, right));
                }
            }

            var a = ComputeMatchingIntervals(Tree.Values.First());
        }

        private HashSet<(int, int)> ComputeMatchingIntervals(IntervalNode node)
        {
            if (node.MatchingIntervals.Count > 0)
            {
                return node.MatchingIntervals;
            }
            else
            {
                HashSet<(int, int)> values = new HashSet<(int, int)>();
                foreach (var child in node.Children)
                {
                    values.UnionWith(ComputeMatchingIntervals(child));
                }
                node.MatchingIntervals = values;
                return values;
            }
        }

        public override IEnumerable<int> Matches(string pattern)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<int> Matches(string pattern1, int x, string pattern2)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<int> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            throw new NotImplementedException();
        }
    }
}
