using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.DataStructures.Reporting;

namespace ConsoleApp.DataStructures
{
    internal class PreCompSubs : ReportDataStructure
    {
        Dictionary<string, HashSet<int>> Substrings = new();


        public PreCompSubs(string text): base(text)
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

        public IEnumerable<(int, int)> Report(Query query)
        {
            string p1 = query.P1;
            int x = query.X;
            string p2 = query.P2;
            List<(int, int)> occs = new();
            if (Substrings.ContainsKey(p1))
            {
                var p1occs = Substrings[p1];
                if (Substrings.ContainsKey(p2))
                {
                    var p2occs = Substrings[p2];
                    if (p1occs.Count < p2occs.Count)
                    {
                        foreach (var p1occ in p1occs)
                        {
                            if (p2occs.Contains(p1occ + p1.Length + x))
                            {
                                occs.Add((p1occ, p1occ + p1.Length + x));
                            }
                        }
                    }
                    else
                    {
                        foreach (var p2occ in p2occs)
                        {
                            if (p1occs.Contains(p2occ - p1.Length - x))
                            {
                                occs.Add((p1.Length - x - p2occ, p2occ + p2.Length));
                            }
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

        public override IEnumerable<int> Matches(string pattern)
        {
            return Substrings[pattern];
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            return Report(new Query(pattern1, x, pattern2));
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            throw new NotImplementedException();
        }
    }
}
