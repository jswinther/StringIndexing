using Gma.DataStructures.StringSearch;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    internal class UkkonenWrapper : PatternMatcher
    {
        CharUkkonenTrie<int> suffixTree;
        public UkkonenWrapper(string str) : base(str)
        {
            suffixTree = new CharUkkonenTrie<int>(0);
            suffixTree.Add(str, 0);
        }

        public override IEnumerable<int> Matches(string pattern)
        {
            return suffixTree.RetrieveSubstrings(pattern).Select(x => x.CharPosition);
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            var occs1 = suffixTree.RetrieveSubstrings(pattern1).Select(s => s.CharPosition);
            var occs2 = suffixTree.RetrieveSubstrings(pattern2).Select(s => s.CharPosition).ToHashSet();

            List<(int, int)> occs = new List<(int, int)>();
            foreach (var item1 in occs1)
            {
                if (occs2.Contains(item1 + pattern1.Length + x))
                    occs.Add((item1, item1 + pattern1.Length + x + pattern2.Length));

            }
            return occs;
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            var occs1 = suffixTree.RetrieveSubstrings(pattern1).Select(s => s.CharPosition);
            var occs2 = new SortedSet<int>(suffixTree.RetrieveSubstrings(pattern2).Select(s => s.CharPosition));

            List<(int, int)> occs = new List<(int, int)>();
            foreach (var item1 in occs1)
            {
                foreach (var item2 in occs2.GetViewBetween(item1 + pattern1.Length + y_min, item1 + pattern1.Length + y_max))
                {
                    occs.Add((item1, item2 + pattern2.Length));
                }                  
            }
            return occs;
        }
        
    }
}
