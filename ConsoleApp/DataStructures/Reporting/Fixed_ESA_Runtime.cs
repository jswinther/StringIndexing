using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    /// <summary>
    /// Formerly known as V2
    /// </summary>
    public class Fixed_ESA_Runtime : ReportFixed
    {
        public Fixed_ESA_Runtime(SuffixArrayFinal str) : base(str)
        {
        }
        public Fixed_ESA_Runtime(string str) : base(str)
        {
        }
        public override IEnumerable<int> Matches(string pattern1, int x, string pattern2)
        {
            List<int> occs = new();
            var occurrencesP1 = SA.SinglePattern(pattern1);
            var occurencesP2 = ReportHashedOccurrences(pattern2);
            foreach (var occ1 in occurrencesP1)
            {
                if (occurencesP2.Contains(occ1 + pattern1.Length + x))
                {
                    occs.Add(occ1);
                }
            }
            return occs;
        }

        public override HashSet<int> ReportHashedOccurrences(string pattern)
        {
            return new HashSet<int>(SA.SinglePattern(pattern));
        }
    }
}
