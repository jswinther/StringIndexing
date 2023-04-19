/* DEN HER ER FRICKED!! DO NOT USE! */

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
        public class STNode
        {
            public String subst = ""; //The substring that this node represents (e.g. the string matched to get to this point)
            public List<int> children = new List<int>(); //List of all children (e.g. longer suffixes with differing next chars)
        }

        public class AlgoSuffixTree
        {
            public List<STNode> nodes = new List<STNode>();

            public AlgoSuffixTree(string T)
            {
                nodes.Add(new STNode());
                for (int i = 0; i < T.Length; i++)
                {
                    addSuffix(T.Substring(i));
                }
            }

            private void addSuffix(String sub)
            {
                int n = 0; //Index of the node that we are looking at?
                int i = 0; //index of next character to be matched
                while (i < sub.Length)
                {
                    char currChar = sub[i]; //The current character being matched (inserted into suffix tree)
                    List<int> children = nodes[n].children;
                    int x = 0;
                    int n2; //Index of next node, once we cant continue matching.
                    while (true)
                    {
                        if (x == children.Count)
                        {
                            // no children, remainer of substring becomes the new node to insert
                            n2 = children.Count;
                            STNode temp = new STNode();
                            temp.subst = sub.Substring(i);
                            nodes.Add(temp);
                            children.Add(n2);
                            return;
                        }
                        n2 = children[x];
                        if (nodes[n2].subst[0].Equals(currChar)) break;
                        x++;
                    }
                    // This part finds the prefix of the remaining suffix in common with child node
                    String subst2 = nodes[n2].subst;
                    int j = 0;
                    while (j < subst2.Length)
                    {
                        if (sub[i + j] != subst2[j])
                        {
                            // Split node with index n2
                            int n3 = n2;
                            // new node to signify the part that is in common with child node
                            n2 = nodes.Count;
                            STNode temp = new STNode();
                            temp.subst = subst2.Substring(0, j);
                            temp.children.Add(n3);
                            nodes.Add(temp);
                            nodes[n3].subst = subst2.Substring(j); // cut away the part in common from the old node
                            nodes[n].children[x] = n2;
                            break; // now continue down the tree
                        }
                        j++;
                    }
                    i += j; // Skip the part that is in common
                    n = n2; // continues down the tree
                }
            }

            public void print()
            {
                if (nodes.Count == 0) {
                    Console.WriteLine("");
                    return;
                }
                print_format(0,"");
            }

            public void print_format(int n, String t) {
                List<int> children = nodes[n].children;
                if(children.Count == 0) {
                    Console.WriteLine("- " + nodes[n].subst);
                    return;
                }
                Console.WriteLine("? " + nodes[n].subst);
                for (int i = 0; i < children.Count -1; i++)
                {
                    int c = children[i];
                    Console.Write(t + "??");
                    print_format(c,t + "? ");
                }
                Console.Write(t + "??");
                print_format(children[children.Count - 1], t + " ");
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
