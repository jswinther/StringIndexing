using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.InitialSolutions
{
    internal class PreCompSubs : PatternMatcher
    {
        Dictionary<string, HashSet<int>> Substrings = new();

        public PreCompSubs(string text) : base(text)
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

        public override IEnumerable<int> Matches(string pattern)
        {
            return Substrings[pattern];
        }

        public override IEnumerable<(int, int)> Matches(string p1, int x, string p2)
        {
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

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            throw new NotImplementedException();
        }
    }
}
