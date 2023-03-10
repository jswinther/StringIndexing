using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    internal class IntervalTree
    {
        SparseTable SparseTable;

        internal class Node
        {
            public int start, end, maxEnd;
            public Node left, right;
            public int v;

            public Node(int start, int end, int v)
            {
                this.start = start;
                this.end = end;
                this.maxEnd = end;
                this.left = null;
                this.right = null;
                this.v = v;
            }
        }

        private Node root;

        public IntervalTree(int[] arr)
        {
            SparseTable = new SparseTable(arr);
            this.root = buildIntervalTree(arr);
        }

        Node buildIntervalTree(int[] arr)
        {
            return buildIntervalTree(arr, 0, arr.Length - 1);
        }

        Node buildIntervalTree(int[] arr, int start, int end)
        {
            Node node = new Node(start, end, SparseTable.RMQ(start, end));
            if (start == end)
            {
                node.v = arr[start];
                return node;
            }
            else
            {
                int mid = (start + end) / 2;
                node.left = buildIntervalTree(arr, start, mid);
                node.right = buildIntervalTree(arr, mid + 1, end);
                node.v = Math.Min(node.left.v, node.right.v);
                return node;
            }
        }

        public void Insert(int start, int end, int v, int[] isa)
        {
            this.root = InsertHelper(this.root, start, end, v);
        }

        private Node InsertHelper(Node node, int start, int end, int v)
        {
            if (node == null)
            {
                var node1 = new Node(start, end, v);
                
                return node1;
            }
                

            if (start < node.start)
                node.left = InsertHelper(node.left, start, end, v);
            else
                node.right = InsertHelper(node.right, start, end, v);

            node.maxEnd = Math.Max(node.maxEnd, end);
            return node;
        }

        public List<Node> Query(int k, int i)
        {
            List<Node> result = new List<Node>();
            QueryHelper(this.root, k, i, result);
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
