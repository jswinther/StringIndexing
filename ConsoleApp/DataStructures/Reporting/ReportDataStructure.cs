using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Reporting
{
    public abstract class ReportDataStructure
    {
        protected ReportDataStructure(string str)
        {
        }
        public abstract IEnumerable<int> Matches(string pattern);
        public abstract IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2);
        public abstract IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2);
    }
}
