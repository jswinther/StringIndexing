using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp.DataStructures.Count
{
    internal class SA_C_V1 : CountDataStructure
    {

        SuffixArrayFinal SA;
        public SA_C_V1(string str, int x, int ymin, int ymax) : base(str, x, ymin, ymax)
        {
            SA = new SuffixArrayFinal(str);
        }

        public override int Matches(string pattern)
        {
            (var a, var b) = SA.ExactStringMatchingWithESA(pattern);
            return b - a + 1;
        }

        public override int MatchesFixed(string pattern1, string pattern2)
        {
            int count = 0;
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
            var occs2 = new HashSet<int>(SA.GetOccurrencesForPattern(pattern2));
            
            foreach (var occ1 in occs1)
            {
                if (occs2.Contains(occ1 + pattern1.Length + x)) 
                    count++;
            }
            return count;
        }

        public override int MatchesVariable(string pattern1, string pattern2)
        {
            int count = 0;
            var occs1 = SA.GetOccurrencesForPattern(pattern1);
            var occs2 = SA.GetOccurrencesForPattern(pattern2);
            occs2.Sort();
            
            foreach (var occ1 in occs1)
            {
                int min = occ1 + y_min + pattern1.Length;
                int max = occ1 + y_max + pattern1.Length;
                (var a, var b) = occs2.BinarySearchOnRange(min, max);
                count += b - a + 1;
            }
            return count;
            
        }

        public static int SizeOfInterval((int, int) interval)
        {
            return interval.Item2 + 1 - interval.Item1;
        }
    }
}
