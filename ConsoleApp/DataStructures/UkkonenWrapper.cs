using Gma.DataStructures.StringSearch;
using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            IEnumerable<WordPosition<int>> occs1 = suffixTree.RetrieveSubstrings(pattern1);
            IEnumerable<WordPosition<int>> occs2 = suffixTree.RetrieveSubstrings(pattern2);
            List<(int, int)> occs = new List<(int, int)>();
            foreach (var item1 in occs1)
            {
                foreach (var item2 in occs2)
                {
                    if (item2.CharPosition == (item1.CharPosition + pattern1.Length + x))
                    {
                        occs.Add((item1.CharPosition, item2.CharPosition));
                    }
                }

            }
            return occs;
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            throw new NotImplementedException();
        }
        
    }
}
