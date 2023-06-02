using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.KdTree;

namespace ConsoleApp.DataStructures.Reporting
{
    internal class SA_R_V5 : ReportDataStructure
    {
        public class Node { }
        private readonly SuffixArrayFinal SA;
        private readonly KdTree<Node> KdTree = new();
        public SA_R_V5(string str) : base(str)
        {
            SA = new SuffixArrayFinal(str);
            BuildDataStructure();
        }

        public SA_R_V5(SuffixArrayFinal str) : base(str)
        {
            SA = str;
            BuildDataStructure();
        }

       

        private void BuildDataStructure()
        {
            for (int i = 0; i < SA.n; i++)
            {
                KdTree.Insert(new Coordinate(i, SA.m_sa[i]), new Node());
            }
        }

        public override IEnumerable<int> Matches(string pattern)
        {
            var occs = SA.GetOccurrencesForPattern(pattern);
            occs.Sort();
            return occs;
        }

        public override IEnumerable<int> Matches(string pattern1, int x, string pattern2)
        {
            List<int> occs = new List<int>();
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
            var occs2 = new HashSet<int>(SA.GetOccurrencesForPattern(pattern2));

            foreach (var occ1 in occs1)
            {
                if (occs2.Contains(occ1 + pattern1.Length + x))
                    occs.Add(occ1);
            }
            return occs;
        }

        public override IEnumerable<int> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            List<KdNode<Node>> occs = new();
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
            var int2 = SA.ExactStringMatchingWithESA(pattern2);
            
            foreach (var occ1 in occs1)
            {
                int min = occ1 + y_min + pattern1.Length;
                int max = occ1 + y_max + pattern1.Length;
                Envelope envelope = new Envelope(int2.i, int2.j, min, max);
                var o = KdTree.Query(envelope);
                occs.AddRange(o);
            }
            return occs.Select(s => (int)s.Y);
        }
    }
}
