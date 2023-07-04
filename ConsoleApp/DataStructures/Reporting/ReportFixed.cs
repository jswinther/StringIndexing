using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    public abstract class ReportFixed : IReportFixed
    {
        protected SuffixArrayFinal SA;
        protected ReportFixed(string str)
        {
            SA = SuffixArrayFinal.CreateSuffixArray(str);
        }
        protected ReportFixed(SuffixArrayFinal str)
        {
            SA = str;
        }
        public abstract HashSet<int> ReportHashedOccurrences(string pattern);
        public abstract IEnumerable<int> Matches(string pattern1, int x, string pattern2);
    }
}
