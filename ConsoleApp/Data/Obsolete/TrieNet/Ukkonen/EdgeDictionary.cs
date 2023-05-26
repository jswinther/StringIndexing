using System;
using System.Collections.Generic;

namespace ConsoleApp.Data.Obsolete.TrieNet.Ukkonen
{
    internal class EdgeDictionary<K, T> : Dictionary<K, Edge<K, T>> where K : IComparable<K>
    {
        //TODO Consider using sorted list based implementation to save memory
    }
}