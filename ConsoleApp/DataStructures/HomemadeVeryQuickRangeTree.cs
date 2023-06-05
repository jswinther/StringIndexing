using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    internal class HomemadeVeryQuickRangeTree
    {
        // LCP intervals
        private readonly int[] x;
        // Position in text T
        private readonly int[] y;
        // Sorted starting positions (y-values) for every range (x-ranges).
        private readonly int[][] z;
        private readonly int lgn;
        private int w = 0;
        private readonly int n;
        // Size of tree
        private readonly int m;
        private readonly RangeNode[] Nodes;

        private class RangeNode
        {
            public RangeNode Parent { get; set; }
            public RangeNode Left { get; set; }
            public RangeNode Right { get; set; }
            public RangePoint[] Points { get; set; }
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

        public (int, int) this[int i]
        {
            get { return (x[i], y[i]); }
            set { (x[i], y[i]) = value; }
        }

        /// <summary>
        /// X is the LCP interval index
        /// Y is the actual position in the string
        /// </summary>
        /// <param name="suffixArray"></param>
        public HomemadeVeryQuickRangeTree(int[] suffixArray)
        {
            n = suffixArray.Length;
            lgn = (int)Math.Round(Math.Log2(n));
            x = new int[n];
            y = new int[n];
            m = (int)Math.Round(Math.Pow(2, lgn + 1) - 1);
            z = new int[m][];
            Nodes = new RangeNode[m];
            for (int i = 0; i < n; i++)
            {
                x[i] = i;
                y[i] = suffixArray[i];
            }
            Queue<(int, int)> traverser = new();
            traverser.Enqueue((0, n - 1));
            while (traverser.Count > 0)
            {
                var node = traverser.Dequeue();
                Insert(node);
                if (Size(node) > 2)
                {
                    (int l, int r) = node;
                    var leftInterval = (l, (l + r) / 2);
                    var rightInterval = ((l + r) / 2, r);
                    traverser.Enqueue(leftInterval);
                    traverser.Enqueue(rightInterval);
                }
            }
            
        }

        public void Insert((int, int) lcpInterval)
        {
            if (lcpInterval.Item1 == lcpInterval.Item2) z[w++] = new int[] { y[lcpInterval.Item1] };
            else z[w++] = y[lcpInterval.Item1..lcpInterval.Item2].Sort();
        }

        public int[] Get((int, int) lcpInterval)
        {
            int lg = (int) Math.Round(Math.Log2(Size(lcpInterval)));
            int d = lgn - lg;
            int idx = (int)Math.Round(Math.Pow(2, d + 1) - 1);
            int start = (int)(lcpInterval.Item1 / Size(lcpInterval));
            return z[(idx + start)];
        }

        public int Size((int, int) lcpInterval) => lcpInterval.Item2 - lcpInterval.Item1 + 1;
    }
}
