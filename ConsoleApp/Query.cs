using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Query
    {
        public string P1 { get; set; }
        public int X { get; set; }
        public string P2 { get; set; }

        public Query(string p1, int x, string p2)
        {
            P1 = p1;
            X = x;
            P2 = p2;
        }
    }
}
