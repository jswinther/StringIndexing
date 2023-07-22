using ConsoleApp.DataStructures.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    internal class Variable_ST_Runtime : IReportVariable
    {
        private WrapSuffixTree reportFixed;

        public Variable_ST_Runtime(WrapSuffixTree reportFixed)
        {
            this.reportFixed = reportFixed;
        }

        public IEnumerable<(int, int)> Matches(string pattern1, int minGap, int maxGap, string pattern2)
        {
            List<(int, int)> result = new List<(int, int)>();
            var occs2 = reportFixed.suffixTree.RetrieveSubstrings(pattern2).Select(s => s.CharPosition).ToArray().Sort();
            foreach (var occ1 in reportFixed.suffixTree.RetrieveSubstrings(pattern1))
            {
                int min = occ1.CharPosition + minGap + pattern1.Length;
                int max = occ1.CharPosition + maxGap + pattern1.Length;
                foreach (var occ2 in occs2.GetViewBetween(min, max))
                {
                    result.Add((occ1.CharPosition, occ2 + pattern2.Length));
                }
            }
            return result;
        }

        public int[] ReportSortedOccurrences(string pattern)
        {
            throw new NotImplementedException();
        }
    }
}
