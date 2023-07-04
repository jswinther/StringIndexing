using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Existence
{
    public abstract class ExistFixed
    {
        protected SuffixArrayFinal SA;
        protected int gap;
        protected ExistFixed(SuffixArrayFinal str, int gap)
        {
            SA = str;
            this.gap = gap;
        }
        protected ExistFixed(string str, int gap)
        {
            SA = SuffixArrayFinal.CreateSuffixArray(str);
            this.gap = gap;
        }
        public abstract bool Matches(string pattern1, string pattern2);
    }
}
