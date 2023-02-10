using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    //Class implementing the Suffix Tree Problem, using Ukonnen's algorithm for construction
    internal class AlgoSuffixTreeProblem
    {
        public class STNode {
            public String subst = ""; //The substring that this node represents (e.g. the string matched to get to this point)
            public List<int> children = new List<int>(); //List of all children (e.g. longer suffixes with differing next chars)
        }

        public class SuffixTree{
            public List<STNode> nodes = new List<STNode>();

            public SuffixTree(String T) {
                nodes.Add(new STNode());
                for (int i = 0; i < T.Length; i++)
                {
                    addSuffix(T.Substring(i));
                }
            }

            private void addSuffix(String sub){
                int n = 0; //Index of the node that we are looking at?
                int i = 0; //index of next character to be matched
                while (i < sub.Length)
                {
                    char currChar = sub[i]; //The current character being matched (inserted into suffix tree)
                    List<int> children = nodes[n].children;
                    int x = 0;
                    int n2; //Index of next node, once we cant continue matching.
                    while(true) {
                        if (x == children.Count) {
                            // no children, remainer of substring becomes the new node to insert
                            n2 = children.Count;
                            STNode temp = new STNode();
                            temp.subst = sub.Substring(i);
                            nodes.Add(temp);
                            children.Add(n2);
                            return;
                        }
                        n2 = children[x];
                        if (nodes[n2].subst[0] == currChar) break;
                        x++;
                    }
                }
            }

        }


        

        /*
        Dictionary<string, SuffixTreeNode> Keys = new Dictionary<string, SuffixTreeNode>();

        public AlgoSuffixTreeProblem(string t)
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

        }*/
    }
}
