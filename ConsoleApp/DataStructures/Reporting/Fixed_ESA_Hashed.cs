using ConsoleApp.Data.Obsolete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    /// <summary>
    /// Formerly known as V3
    /// </summary>
    internal class Fixed_ESA_Hashed : ReportFixed
    {
        private Dictionary<(int, int), IntervalNode> Tree;
        private Dictionary<(int, int), IntervalNode> Leaves;
        private IntervalNode Root;
        private Dictionary<(int, int), HashSet<int>> Hashed = new();
        public Fixed_ESA_Hashed(SuffixArrayFinal str) : base(str)
        {
            SA.GetAllLcpIntervals(1, out Tree, out Leaves, out Root);
            BuildDataStructure();
        }
        public Fixed_ESA_Hashed(string str) : base(str)
        {
            SA.GetAllLcpIntervals(1, out Tree, out Leaves, out Root);
            BuildDataStructure();
        }
        private void BuildDataStructure()
        {
            var keys = Tree.Keys.ToList();

            for (int i = 1; i < keys.Count; i++)
            {
                var interval = keys[i];
                Hashed.Add(interval, new HashSet<int>(SA.GetOccurrencesForInterval(interval)));
            }
        }
        public override IEnumerable<int> Matches(string pattern1, int x, string pattern2)
        {
            List<int> occs = new();
            var occurrencesP1 = ReportHashedOccurrences(pattern1);
            var occurrencesP2 = ReportHashedOccurrences(pattern2);
            foreach (var occ1 in occurrencesP1)
            {
                if (occurrencesP2.Contains(occ1 + pattern1.Length + x))
                {
                    occs.Add(occ1);
                }
            }
            return occs;
        }

        public override HashSet<int> ReportHashedOccurrences(string pattern)
        {
            var interval = SA.ExactStringMatchingWithESA(pattern);
            if (interval == (-1, -1)) return new HashSet<int>();
            return Hashed[interval];
        }
    }
}
