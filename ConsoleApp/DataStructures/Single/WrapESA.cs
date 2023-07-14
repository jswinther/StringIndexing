using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Single
{
    internal class WrapESA : IReportSinglePattern
    {
        private SuffixArrayFinal SA;
        public WrapESA(string text)
        {
            SA = SuffixArrayFinal.CreateSuffixArray(text);
            SA.BuildChildTable();
        }

        public IEnumerable<int> SinglePatternMatching(string pattern)
        {
            return SA.SinglePattern(pattern);
        }
    }
}
