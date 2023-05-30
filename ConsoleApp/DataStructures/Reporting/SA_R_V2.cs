using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp.DataStructures.Reporting
{
    internal class SA_R_V2 : ReportDataStructure
    {
        SuffixArrayFinal SA;
        public SA_R_V2(string str) : base(str)
        {
            SA = new SuffixArrayFinal(str);
            
        }
        public SA_R_V2(SuffixArrayFinal str) : base(str)
        {
            SA = str;

        }

        public override IEnumerable<int> Matches(string pattern)
        {
            var occs = SA.GetOccurrencesForPattern(pattern);
            occs.Sort();
            return occs;
        }

        public override IEnumerable<int> Matches(string pattern1, int x, string pattern2)
        {
            /*
            var int1 = SA.ExactStringMatchingWithESA(pattern1);
            var int2 = SA.ExactStringMatchingWithESA(pattern2);
            List<int> ints = new(int2.j - int2.i + 1);
            for (int i = int1.i; i <= int1.j; i++)
            {
                if (i + pattern1.Length + x < SA.n.Value)
                {
                    ints.Add(SA[i + pattern1.Length + x]);
                }
            }


            return ints.ToArray().Sort().GetViewBetween(int2.i, int2.j).Select(s => (s, s));
            */
            List<int> occs = new List<int>();
            var occs1 = SA.GetOccurrencesForPattern(pattern1);

            
            var occs2 = new HashSet<int>(SA.GetOccurrencesForPattern(pattern2));

            foreach (var occ1 in occs1)
            {
                if (occs2.Contains(occ1 + pattern1.Length + x))
                    occs.Add(occ1);
            }
            return occs;

        }

        public override IEnumerable<int> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            List<int> occs = new List<int>();
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
            var occs2 = SA.GetOccurrencesForPattern(pattern2);
            occs2.Sort();
            foreach (var occ1 in occs1)
            {
                int min = occ1 + y_min + pattern1.Length;
                int max = occ1 + y_max + pattern1.Length;
                foreach (var occ2 in occs2.GetViewBetween(min, max))
                {
                    occs.Add(occ1);
                }
            }
            return occs;
        }

        
    }
}
