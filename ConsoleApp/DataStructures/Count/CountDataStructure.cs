using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Count
{
    public abstract class CountDataStructure
    {
        protected CountDataStructure(string str)
        {
        }
        public abstract int Matches(string pattern);
        public abstract int Matches(string pattern1, int x, string pattern2);
        public abstract int Matches(string pattern1, int y_min, int y_max, string pattern2);
    }
}
