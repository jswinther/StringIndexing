using ConsoleApp.DataStructures.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    internal class Fixed_ST_Runtime : IReportFixed
    {
        private WrapSuffixTree reportFixed;

        public Fixed_ST_Runtime(WrapSuffixTree reportFixed)
        {
            this.reportFixed = reportFixed;
        }

        public IEnumerable<int> Matches(string pattern1, int x, string pattern2)
        {
            List<int> result = new List<int>();
            var occs2 = reportFixed.suffixTree.RetrieveSubstrings(pattern2).Select(s => s.CharPosition).ToHashSet();
            foreach (var occ1 in reportFixed.suffixTree.RetrieveSubstrings(pattern1))
            {
                if (occs2.Contains(occ1.CharPosition + x + pattern1.Length))
                {
                    result.Add(occ1.CharPosition);
                }
            }
            return result;
        }

        public HashSet<int> ReportHashedOccurrences(string pattern)
        {
            throw new NotImplementedException();
        }
    }
}
