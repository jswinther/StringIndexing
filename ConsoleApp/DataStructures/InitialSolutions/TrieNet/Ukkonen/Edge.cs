using System;

namespace ConsoleApp.DataStructures.InitialSolutions.TrieNet.Ukkonen
{
    internal class Edge<K, T> where K : IComparable<K>
    {
        public Edge(ReadOnlyMemory<K> label, Node<K, T> target)
        {
            Label = label;
            Target = target;
        }

        public ReadOnlyMemory<K> Label { get; set; }

        public Node<K, T> Target { get; private set; }
    }
}