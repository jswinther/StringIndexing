using Shrulik.NKDBush;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    internal class Variable_ESA_2D : ReportVariable
    {
        private KDBush<double[]> KDTree = null;
        public Variable_ESA_2D(SuffixArrayFinal str) : base(str)
        {
            Build();
        }

        public Variable_ESA_2D(string str) : base(str)
        {
            Build();
        }

        private void Build()
        {
            double[][] points = new double[(int)SA.n - 1][];
            for (int i = 0; i < SA.n - 1; i++)
            {
                points[i] = new double[2];
                points[i][0] = i;
                points[i][1] = SA.m_sa[i];
                //KdTree.Insert(new Coordinate(i, SA.m_sa[i]), new Node());
            }
            KDTree = new KDBush<double[]>(points, nodeSize: 32);
        }

        public override IEnumerable<(int,int)> Matches(string pattern1, int minGap, int maxGap, string pattern2)
        {
            List<(int,int)> occs = new();
            //List<KdNode<Node>> occs = new();
            var occs1 = SA.SinglePattern(pattern1);
            var int2 = SA.ExactStringMatchingWithESA(pattern2);

            foreach (var occ1 in occs1)
            {
                int min = occ1 + minGap + pattern1.Length;
                int max = occ1 + maxGap + pattern1.Length;
                //Envelope envelope = new Envelope(int2.i, int2.j, min, max);
                //var o = KdTree.Query(envelope);
                var o = KDTree.Range(int2.i, min, int2.j, max);
                occs.AddRange(o.Select(s => (s, s)));
            }
            return occs;
        }

        public override int[] ReportSortedOccurrences(string pattern)
        {
            return SA.SinglePattern(pattern).Sort();
        }
    }
}
