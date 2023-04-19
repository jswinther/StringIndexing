using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Helpers
{
    internal class IntervalTree
    {
        SparseTable SparseTable;

        internal class Node
        {
            public int start, end, maxEnd;
            public Node left, right;
            public int v;
            public SortedSet<int> ints;

            public Node(int start, int end, int v, SortedSet<int> ints)
            {
                this.start = start;
                this.end = end;
                maxEnd = end;
                left = null;
                right = null;
                this.v = v;
                this.ints = ints;
            }
        }

        private Node root;



        public IntervalTree()
        {

        }



        public void Insert(int start, int end, int v, SortedSet<int> ints)
        {
            root = InsertHelper(root, start, end, v, ints);
        }

        private Node InsertHelper(Node node, int start, int end, int v, SortedSet<int> ints)
        {
            if (node == null)
            {
                var node1 = new Node(start, end, v, ints);

                return node1;
            }


            if (start < node.start)
                node.left = InsertHelper(node.left, start, end, v, ints);
            else
                node.right = InsertHelper(node.right, start, end, v, ints);

            node.maxEnd = Math.Max(node.maxEnd, end);
            return node;
        }

        public List<Node> Query(int k, int i)
        {
            List<Node> result = new List<Node>();
            QueryHelper(root, k, i, result);
            return result;
        }

        private void QueryHelper(Node node, int k, int i, List<Node> result)
        {
            if (node == null)
                return;

            if (node.start <= i && i <= node.end && node.v >= k)
                result.Add(node);

            if (node.left != null && node.left.maxEnd >= i)
                QueryHelper(node.left, k, i, result);

            if (node.right != null && node.right.start <= i)
                QueryHelper(node.right, k, i, result);
        }
    }

}
