using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Count
{
    public abstract class CountVariable
    {
        protected SuffixArrayFinal SA;
        protected int minGap;
        protected int maxGap;
        protected CountVariable(SuffixArrayFinal str, int minGap, int maxGap)
        {
            SA = str;
            this.minGap = minGap;
            this.maxGap = maxGap;
        }
        protected CountVariable(string str, int minGap, int maxGap)
        {
            SA = SuffixArrayFinal.CreateSuffixArray(str);
            this.minGap = minGap;
            this.maxGap = maxGap;
        }
        public abstract int Matches(string pattern1, string pattern2);
    }
}
