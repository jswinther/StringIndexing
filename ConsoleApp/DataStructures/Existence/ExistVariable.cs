using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Existence
{
    public abstract class ExistVariable
    {
        protected SuffixArrayFinal SA;
        protected int minGap;
        protected int maxGap;
        protected ExistVariable(SuffixArrayFinal str, int minGap, int maxGap)
        {
            SA = str;
            this.minGap = minGap;
            this.maxGap = maxGap;
        }
        protected ExistVariable(string str, int minGap, int maxGap)
        {
            SA = SuffixArrayFinal.CreateSuffixArray(str);
            this.minGap = minGap;
            this.maxGap = maxGap;
        }
        public abstract bool Matches(string pattern1, string pattern2);
    }
}
