using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Count
{
    public abstract class CountFixed
    {
        protected SuffixArrayFinal SA;
        protected int gap;
        protected CountFixed(SuffixArrayFinal str, int gap)
        {
            SA = str;
            this.gap = gap;
        }
        protected CountFixed(string str, int gap)
        {
            SA = SuffixArrayFinal.CreateSuffixArray(str);
            this.gap = gap;
        }
        public abstract int Matches(string pattern1, string pattern2);
    }
}
