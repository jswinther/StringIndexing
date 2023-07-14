using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Single
{
    internal class WrapPrecomp : IReportSinglePattern
    {
        private Dictionary<string, LinkedList<int>> D = new Dictionary<string, LinkedList<int>>();
        public WrapPrecomp(string text) 
        {
            for (int i = 1; i <= text.Length; i++)
            {
                for (int j = 0; j <= text.Length - i; j++)
                {
                    var s = text.Substring(j, i);
                    if (!D.ContainsKey(s)) D[s] = new LinkedList<int>();
                    D[s].AddLast(j);
                }
            }
        }

        public IEnumerable<int> SinglePatternMatching(string pattern)
        {
            if (D.ContainsKey(pattern)) return D[pattern];
            else return Enumerable.Empty<int>();
        }
    }
}
