using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    internal class PrecomputedSubstrings
    {
        Dictionary<string, HashSet<int>> Substrings = new();

        public PrecomputedSubstrings(string text)
        {
            for (int i = 1; i <= text.Length; i++)
            {
                for (int j = 0; j <= text.Length - i; j++)
                {
                    var t = text.Substring(j, i);
                    if (!Substrings.ContainsKey(t))
                    {
                        Substrings[t] = new HashSet<int>();
                    }
                    Substrings[t].Add(j);
                }
            }

        }

        public IEnumerable<int> Report(Query query)
        {
            string p1 = query.P1;
            int x = query.X;
            string p2 = query.P2;
            List<int> occs = new List<int>();
            if (Substrings.ContainsKey(p1))
            {
                var p1occs = Substrings[p1];
                if (Substrings.ContainsKey(p2))
                {
                    var p2occs = Substrings[p2];
                    foreach (var p1occ in p1occs)
                    {
                        if (p2occs.Contains(p1occ + p1.Length + x))
                        {
                            occs.Add(p1occ);
                        }
                    }
                }
            }
            return occs;
        }

        public void PrintHaram()
        {
            foreach (var key in Substrings.Keys)
            {
                Console.WriteLine(key);
                foreach (var index in Substrings[key])
                {
                    Console.Write($"{index}\t");
                }

                Console.WriteLine("\n");
            }
        }
    }
}
