using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    internal class HomemadeRangeTree
    {
        // LCP intervals
        private readonly int[] x;
        // Position in text T
        private readonly int[] y;
        //private readonly int[][] z;
        private readonly int lgn;
        private int w = 0;
        private readonly int n;
        private readonly int m;
        //private readonly RangeNode[] Nodes;
        private RangeNode root;

        public class RangeNode
        {
            public RangeNode? Parent { get; set; }
            public RangeNode? Left { get; set; }
            public RangeNode? Right { get; set; }
            public (int, int) X_interval { get; set; }
            public List<RangePoint> Points { get; set; }
            public class RangePoint : IComparable<RangePoint>
            {
                public int? LeftBridgeIndex { get; set; }
                public int? RightBridgeIndex { get; set; }
                public int Value { get; set; }

                public int CompareTo(RangePoint? other)
                {
                    return Value.CompareTo(other.Value);
                }
            }            
        }

        /*public (int, int) this[int i]
        {
            get { return (x[i], y[i]); }
            set { (x[i], y[i]) = value; }
        }*/

        /// <summary>
        /// X is the LCP interval index
        /// Y is the actual position in the string
        /// </summary>
        /// <param name="suffixArray"></param>
        public HomemadeRangeTree(int[] suffixArray, int[] isa)
        {
            n = suffixArray.Length;
            lgn = (int)Math.Round(Math.Log2(n));
            m = (int)Math.Round(Math.Pow(2, lgn + 1) - 1);
            root = new RangeNode();
            root.Parent = null;
            root.X_interval = (0, n);
            root.Points = new();
            x = new int[n];
            y = new int[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = i;
                y[i] = suffixArray[i];
                root.Points[i].Value = i;
            }

            Queue<RangeNode> traverser = new();
            traverser.Enqueue(root);
            while (traverser.Count > 0)
            {
                var node = traverser.Dequeue();
                
                if (Size(node) > 2)
                {
                    node.Left = new RangeNode();
                    node.Left.Parent = node;
                    node.Right = new RangeNode();
                    node.Right.Parent = node;
                    (int l, int r) = node.X_interval;
                    var leftInterval = (l, (l + r) / 2);
                    var rightInterval = ((l + r) / 2, r);
                    node.Left.X_interval = leftInterval;
                    node.Right.X_interval = rightInterval;
                    var leftPoints = node.Points.Where(p => Math.Abs(isa[p.Value]) <= leftInterval.Item1 && Math.Abs(isa[p.Value]) >= leftInterval.Item2);
                    var rightPoints = node.Points.Where(p => Math.Abs(isa[p.Value]) <= rightInterval.Item1 && Math.Abs(isa[p.Value]) >= rightInterval.Item2);
                    node.Left.Points = leftPoints.ToList();
                    node.Right.Points = rightPoints.ToList();

                    traverser.Enqueue(node.Left);
                    traverser.Enqueue(node.Right);
                }
            }
            
        }

        public int Size(RangeNode node) => node.X_interval.Item2 - node.X_interval.Item1 + 1;

        
    }
}
