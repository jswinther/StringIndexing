using System;
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
        public List<string> topPattern = new();
        public List<string> botPattern = new();
        public List<string> midPatterns = new();
        public SuffixArray_Scanner((string, string) args, SuffixArrayFinal sa)
        {
            (string name, string str) = args;
            SA = sa;
            SA.GetAllLcpIntervals(1, out Tree, out Leaves1, out Root);

            Queue<IntervalNode> findTestNodes = new Queue<IntervalNode>();
            foreach (var child in Root.Children)
            {
                if (!child.IsLeaf)
                {
                    findTestNodes.Enqueue(child);
                }
            }

            //HashSet<(int, int)> top = new HashSet<(int, int)>();
            Random r = new Random();
            double probRoll = 0.0;
            double minSize = Math.Log(SA.n.Value);
            int top_id = 0;
            int bot_id = 0;

            int avg_leaf_dist = (int)Math.Round(Leaves1.Values.Average(s => s.DistanceToRoot));
            while (findTestNodes.Count > 0)
            {
                var n = findTestNodes.Dequeue();
                if (n.DistanceToRoot < 5 && n.DistanceToRoot > 0 && topPattern.Count < 10)
                {
                    probRoll = 0.33;
                    //1 / (topPattern.Count + 1);
                    if (r.NextDouble() <= probRoll)
                    {
                        if (n.Interval.start == n.Interval.end)
                        {
                            topPattern.Add(SA.m_str[SA.m_sa[n.Interval.start]..(SA.n.Value)]);
                        } else
                        {
                            var patLength = SA.GetLcp(n.Interval.start, n.Interval.end);
                            topPattern.Add(SA.m_str[SA.m_sa[n.Interval.start]..(SA.m_sa[n.Interval.start] + patLength)]);
                        }
                        
                        //string.Concat(SA.m_str.Take(new Range(SA.m_sa[n.Interval.start], SA.m_sa[n.Interval.start] + (patLength -1))));
                        top_id++;
                    }

                }
                else if (n.DistanceToRoot >= avg_leaf_dist && botPattern.Count < 10)
                {
                    probRoll = 0.33;
                        //1 / (botPattern.Count +1);
                    if (r.NextDouble() <= probRoll)
                    {
                        if (n.Interval.start == n.Interval.end)
                        {
                            botPattern.Add(SA.m_str[SA.m_sa[n.Interval.start]..(SA.n.Value)]);
                        }
                        else
                        {
                            var patLength = SA.GetLcp(n.Interval.start, n.Interval.end);
                            botPattern.Add(SA.m_str[SA.m_sa[n.Interval.start]..(SA.m_sa[n.Interval.start] + patLength)]);
                        }
                        bot_id++;
                    }
                }

                foreach (var item in n.Children)
                {
                    findTestNodes.Enqueue(item);
                }
            }

            while (topPattern.Count < 10)
            {
                topPattern.Add(topPattern.GetRandom());
            }

            while (botPattern.Count < 10)
            {
                botPattern.Add(botPattern.GetRandom());
            }


            var midNodes = Tree.Values.Skip(1);
            //.Where(n => n.Size > Math.Sqrt(SA.n.Value));
            for (int i = 0; i < 10; i++)
            {
                int index = r.Next(0, midNodes.Count());
                var node = midNodes.ElementAt(index);
                if (node.Interval.start == node.Interval.end)
                {
                    midPatterns.Add(SA.m_str[SA.m_sa[node.Interval.start]..(SA.n.Value)]);
                }
                else
                {
                    var patLength = SA.GetLcp(node.Interval.start, node.Interval.end);
                    midPatterns.Add(SA.m_str[SA.m_sa[node.Interval.start]..(SA.m_sa[node.Interval.start] + patLength)]);
                }
            }


        }
    }
}
