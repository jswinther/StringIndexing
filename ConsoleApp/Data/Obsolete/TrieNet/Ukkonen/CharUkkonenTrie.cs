using ConsoleApp.Data.Obsolete.TrieNet;
using System;
using System.Collections.Generic;

namespace ConsoleApp.Data.Obsolete.TrieNet.Ukkonen
{
    public class CharUkkonenTrie<T> : UkkonenTrie<char, T>, ISuffixTrie<T>
    {
        public CharUkkonenTrie() : base(0) { }

        public CharUkkonenTrie(int minSuffixLength) : base(minSuffixLength) { }

        public void Add(string key, T value)
        {
            Add(key.AsMemory(), value);
        }

        public IEnumerable<T> Retrieve(string query)
        {
            return Retrieve(query.AsSpan());
        }

        public IEnumerable<WordPosition<T>> RetrieveSubstrings(string query)
        {
            return RetrieveSubstrings(query.AsSpan());
        }
    }


}