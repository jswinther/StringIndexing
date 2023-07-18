using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public IEnumerable<int> SinglePatternMatching(string pattern, out double mt, out double rt)
        {
            var sw = Stopwatch.StartNew();
            (int i, int j) = SA.BinarySearch(pattern);
            sw.Stop();
            mt = sw.Elapsed.TotalNanoseconds;
            sw = Stopwatch.StartNew();
            var occs = SA.m_sa[i..(j + 1)];
            sw.Stop();
            rt = sw.Elapsed.TotalNanoseconds;
            return occs;
        }
    }
}
