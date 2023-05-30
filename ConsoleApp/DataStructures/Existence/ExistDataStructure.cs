using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Existence
{
    public abstract class ExistDataStructure
    {
        public int x { get; set; }
        public int ymin { get; set; }
        public int ymax { get; set; }
        protected ExistDataStructure(string str, int x, int ymin, int ymax)
        {
            this.x = x;
            this.ymin = ymin;
            this.ymax = ymax;
        }
        protected ExistDataStructure(SuffixArrayFinal str, int x, int ymin, int ymax)
        {
            this.x = x;
            this.ymin = ymin;
            this.ymax = ymax;
        }
        public abstract bool Matches(string pattern);
        public abstract bool MatchesFixedGap(string pattern1, string pattern2);
        public abstract bool MatchesVariableGap(string pattern1, string pattern2);
    }
}
