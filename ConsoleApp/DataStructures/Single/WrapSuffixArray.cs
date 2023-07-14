using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Single
{
    internal class WrapSuffixArray : IReportSinglePattern
    {
        private SuffixArrayFinal SA;
        public WrapSuffixArray(string text)
        {
            SA = SuffixArrayFinal.CreateSuffixArray(text);
        }

        public IEnumerable<int> SinglePatternMatching(string pattern)
        {
            return SA.BinaryMatches(pattern);
        }
    }
}
