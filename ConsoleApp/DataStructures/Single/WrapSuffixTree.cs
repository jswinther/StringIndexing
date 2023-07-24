using ConsoleApp.DataStructures.Obsolete.TrieNet.Ukkonen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Single
{
    internal class WrapSuffixTree : IReportSinglePattern
    {
        public CharUkkonenTrie<int> suffixTree;
        public WrapSuffixTree(string str)
        {
            str += "|";
            suffixTree = new CharUkkonenTrie<int>(0);
            suffixTree.Add(str, 0);
        }

        public IEnumerable<int> SinglePatternMatching(string pattern, out double mt, out double rt)
        {
            var sw = Stopwatch.StartNew();
            var p = suffixTree.SearchNode(pattern);
            sw.Stop();
            mt = sw.Elapsed.TotalNanoseconds;
            sw = Stopwatch.StartNew();
            var occs = new List<int>(p.GetData().Select(x => x.CharPosition));
            sw.Stop();
            rt = sw.Elapsed.TotalNanoseconds;
            return occs;
        }
    }
}
