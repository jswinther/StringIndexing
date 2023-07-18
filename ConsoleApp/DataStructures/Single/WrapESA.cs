using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public IEnumerable<int> SinglePatternMatching(string pattern, out double mt, out double rt)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var interval = SA.ExactStringMatchingWithESA(pattern);
            sw.Stop();
            mt = sw.Elapsed.TotalNanoseconds;
            sw = Stopwatch.StartNew();
            var occs = SA.GetOccurrencesForInterval(interval);
            sw.Stop();
            rt = sw.Elapsed.TotalNanoseconds;
            return occs;
        }
    }
}
