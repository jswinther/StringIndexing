using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Count
{
    internal class SA_C_V2_Fenwick : CountDataStructure
    {
        SuffixArrayFinal SA;
        public SA_C_V2_Fenwick(string str, int x, int ymin, int ymax) : base(str, x, ymin, ymax)
        {
            SA = new SuffixArrayFinal(str);
        }

        public override int Matches(string pattern)
        {
            throw new NotImplementedException();
        }

        public override int MatchesFixed(string pattern1, string pattern2)
        {
            throw new NotImplementedException();
        }

        public override int MatchesVariable(string pattern1, string pattern2)
        {
            throw new NotImplementedException();
        }
    }
}
