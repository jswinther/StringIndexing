using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Trie
    {
        private string T;
        public Trie(string t)
        {
            T = t;
            SortedSet<string> suffixes = new();

            for (int i = t.Length - 1; i >= 0; i--)
            {
                suffixes.Add(t.Substring(i, t.Length - i) + "$");
            }

            foreach (var suffix in suffixes)
            {
                Insert(suffix);
            }
        }



        private TrieNode root = new TrieNode();

        private class TrieNode
        {
            // We could probably use something smaller than a char since we know that the alphabet never exceeds 26 or whatever the size of the DNA alphabet is.
            Dictionary<char, TrieNode> children = new Dictionary<char, TrieNode>();
            public bool ContainsKey(char key) => children.ContainsKey(key);
            public TrieNode this[char key]
            {
                get => children[key];
            }
            public TrieNode AddLetter(char letter)
            {
                children.Add(letter, new TrieNode());
                return children[letter];
            }

            public bool EndOfWord
            {
                get => children.Count == 0;
            }
        }

        

        public void Insert(string word)
        {
            TrieNode level = root;
            foreach (var letter in word)
            {
                if (level.ContainsKey(letter))
                {
                    level = level[letter];
                }
                else
                {
                    level = level.AddLetter(letter);
                }
            }
        }

        


    }


}