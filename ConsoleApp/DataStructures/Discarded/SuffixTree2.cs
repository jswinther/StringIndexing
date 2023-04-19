using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Discarded
{
    internal class SuffixTree2
    {
        private class Node
        {
            public Dictionary<char, Node> children = new Dictionary<char, Node>();
            public int start;
            public int end;
            public int lcpIntervalStart;
            public int lcpIntervalEnd;
        }

        private readonly string text;
        private readonly Node root;

        public SuffixTree2(string text)
        {
            this.text = text;
            root = new Node();
            for (int i = 0; i < text.Length; i++)
            {
                InsertSuffix(i, root);
            }
        }

        private void InsertSuffix(int i, Node node)
        {
            if (i >= text.Length)
            {
                return;
            }

            if (node.children.TryGetValue(text[i], out Node child))
            {
                int j = child.start;
                while (i < text.Length && j <= child.end && text[i] == text[j])
                {
                    i++;
                    j++;
                }
                if (j <= child.end)
                {
                    SplitChild(child, j);
                }
                else
                {
                    InsertSuffix(i, child);
                }
            }
            else
            {
                node.children[text[i]] = new Node { start = i, end = text.Length - 1, lcpIntervalStart = i };
            }
        }

        private void SplitChild(Node node, int i)
        {
            Node splitNode = new Node { start = node.start, end = i - 1, lcpIntervalStart = node.lcpIntervalStart, lcpIntervalEnd = node.lcpIntervalStart + i - node.start - 1 };
            node.start = i;
            Node child = node.children[text[i]];
            node.children[text[i]] = splitNode;
            splitNode.children[text[child.start]] = child;
            child.start = i;
            splitNode.children[text[child.start]] = child;
            child.lcpIntervalStart = i;
        }

        public List<(int, int)> GetLongestCommonPrefixIntervals(string pattern)
        {
            List<(int, int)> intervals = new List<(int, int)>();
            int i = 0;
            Node node = root;
            int j = node.start;
            while (i < pattern.Length)
            {
                if (!node.children.TryGetValue(pattern[i], out node))
                {
                    return intervals;
                }

                int k = node.start;
                while (i < pattern.Length && k <= node.end && pattern[i] == text[k])
                {
                    i++;
                    k++;
                }

                if (i == pattern.Length)
                {
                    intervals.Add((node.lcpIntervalStart, node.lcpIntervalEnd));
                    return intervals;
                }

                if (k > node.end)
                {
                    intervals.Add((node.lcpIntervalStart, node.lcpIntervalEnd));
                    j = k;
                }
                else
                {
                    intervals.Add((node.lcpIntervalStart, node.lcpIntervalStart + k - node.start - 1));
                }
            }
            return intervals;
        }
    }
}
