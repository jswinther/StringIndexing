using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    public abstract class ReportVariable : IReportVariable
    {
        protected SuffixArrayFinal SA;
        protected ReportVariable(SuffixArrayFinal str)
        {
            SA = str;
        }
        protected ReportVariable(string str)
        {
            SA = SuffixArrayFinal.CreateSuffixArray(str);
        }
        public abstract int[] ReportSortedOccurrences(string pattern);
        public abstract IEnumerable<(int, int)> Matches(string pattern1, int minGap, int maxGap, string pattern2);
    }
}
