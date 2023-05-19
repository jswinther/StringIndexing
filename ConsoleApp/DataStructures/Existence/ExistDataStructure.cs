using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Existence
{
    public abstract class ExistDataStructure
    {
        protected ExistDataStructure(string str, int x, int ymin, int ymax)
        {
        }
        public abstract bool Matches(string pattern);
        public abstract bool MatchesFixedGap(string pattern1, string pattern2);
        public abstract bool MatchesVariableGap(string pattern1, string pattern2);
    }
}
