﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp.DataStructures
{
    internal class SuffixArray_Scanner
    {

        /*
         * Test info:
         * - 1 top node
         * - 1 bottom node
         * - Deepest leaf
         * - 3-4 "mid" nodes
         * - Term for hvor balanced træet er
         */
        SuffixArrayFinal SA;
        public Dictionary<(int, int), IntervalNode> Tree;
        public Dictionary<(int, int), IntervalNode> SortedTree;
        Dictionary<(int, int), IntervalNode> Leaves1;
        public (int, int)[] Leaves { get; private set; }
        public IntervalNode[] Nodes { get; private set; }
        public List<(int, int)> TopNodes { get; private set; }
        public int Height { get; set; }
        private IntervalNode Root;
        public string topPattern = null;
        public string botPattern = null;
        public string[] midPatterns = new string[5];
        public SuffixArray_Scanner((string, string) args, SuffixArrayFinal sa)
        {
            (string name, string str) = args;
            SA = sa;
            SA.GetAllLcpIntervals(1, out Tree, out Leaves1, out Root);

            Queue<IntervalNode> findTestNodes = new Queue<IntervalNode>();
            foreach (var child in Root.Children)
            {
                findTestNodes.Enqueue(child);
            }

            //HashSet<(int, int)> top = new HashSet<(int, int)>();
            Random r = new Random();
            double probRoll = 0.0;
            double minSize = Math.Log(SA.n.Value);
            while (findTestNodes.Count > 0)
            {
                var n = findTestNodes.Dequeue();
                if (n.DistanceToRoot < 5 && n.DistanceToRoot > 0)
                {
                    probRoll = n.Size / (double)Tree.Count * 0.2;
                    if (r.NextDouble() <= probRoll || topPattern == null)
                    {
                        var patLength = SA.GetLcp(n.Interval.start, n.Interval.end);
                        topPattern = string.Concat(SA.m_str.Take(new Range(SA.m_sa[n.Interval.start], SA.m_sa[n.Interval.start] + patLength)));
                    }

                }
                else if (n.Size <= minSize)
                {
                    probRoll = n.Size / (0.5 * Tree.Count);
                    if (r.NextDouble() <= probRoll || botPattern == null)
                    {
                        var patLength = SA.GetLcp(n.Interval.start, n.Interval.end);
                        botPattern = string.Concat(SA.m_str.Take(new Range(SA.m_sa[n.Interval.start], SA.m_sa[n.Interval.start] + patLength)));
                    }
                }

                foreach (var item in n.Children)
                {
                    findTestNodes.Enqueue(item);
                }
            }

     
            var midNodes = Tree.Values.Skip(1).Where(n => n.Size > Math.Sqrt(SA.n.Value));
            for (int i = 0; i < 5; i++)
            {
                int index = r.Next(0, midNodes.Count());
                var node = midNodes.ElementAt(index);
                var patLength = SA.GetLcp(node.Interval.start, node.Interval.end);
                midPatterns[i] = string.Concat(SA.m_str.Take(new Range(SA.m_sa[node.Interval.start], SA.m_sa[node.Interval.start] + patLength)));
            }


        }
    }
}
