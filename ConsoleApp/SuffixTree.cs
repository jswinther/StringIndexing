using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class SuffixTree
    {
        private readonly string T;

        Dictionary<string, SuffixTreeNode> Keys = new Dictionary<string, SuffixTreeNode>();

        public SuffixTree(string t)
        {
            T = t;
            SortedSet<string> suffixes = new();

            for (int i = t.Length - 1; i >= 0; i--)
            {
                suffixes.Add(t.Substring(i, t.Length - i) + "$");
            }

            foreach (var suffix in suffixes)
            {
                InsertSuffix(suffix);
            }
        }

        private void InsertSuffix(string suffix)
        {
            string subSuffix = suffix[0].ToString();
            int iterator = 1;
            while (Keys.ContainsKey(subSuffix))
            {
                if (iterator == suffix.Length) return;
                subSuffix += suffix[iterator];
                iterator++;
            }
            if (subSuffix.Length == 1)
            {
                Keys.Add(suffix, new SuffixTreeNode(suffix));
            } 
        }

        public IEnumerable<int> ReportOccurrences(string P)
        {
            return null;
        }

        private class SuffixTreeNode
        {
            private string suffix;
            public SuffixTreeNode(string suffix)
            {
                this.suffix = suffix;
            }
        }

        public void Run() 
        {
            
        }
    }
}
