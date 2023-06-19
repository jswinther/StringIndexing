using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    /// <summary>
    /// Formerly known as V1
    /// </summary>
    internal class Variable_SA_Runtime : ReportVariable
    {
        public Variable_SA_Runtime(SuffixArrayFinal str) : base(str)
        {
        }

        public Variable_SA_Runtime(string str) : base(str)
        {
        }

        public override IEnumerable<int> Matches(string pattern1, int minGap, int maxGap, string pattern2)
        {
            List<int> occs = new();
            var occurrencesP1 = SA.BinaryMatches(pattern1);
            var occurrencesP2 = ReportSortedOccurrences(pattern2);
            foreach (var occ1 in occurrencesP1)
            {
                int min = occ1 + minGap + pattern1.Length;
                int max = occ1 + maxGap + pattern1.Length;
                occs.AddRange(occurrencesP2.GetViewBetween(min, max));
            }
            return occs;
        
        }

        public override int[] ReportSortedOccurrences(string pattern)
        {
            return SA.BinaryMatches(pattern).ToArray().Sort();
        }
    }
}
