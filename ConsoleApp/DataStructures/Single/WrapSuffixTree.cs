using ConsoleApp.DataStructures.Obsolete.TrieNet.Ukkonen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Single
{
    internal class WrapSuffixTree : IReportSinglePattern
    {
        CharUkkonenTrie<int> suffixTree;
        public WrapSuffixTree(string str)
        {
            suffixTree = new CharUkkonenTrie<int>(0);
            suffixTree.Add(str, 0);
        }

        public IEnumerable<int> SinglePatternMatching(string pattern)
        {
            return new List<int>(suffixTree.RetrieveSubstrings(pattern).Select(x => x.CharPosition));
        }
    }
}
