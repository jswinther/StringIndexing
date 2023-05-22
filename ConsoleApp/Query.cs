using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class Query
    {
        public string P1 { get; set; }
        public int X { get; set; }
        public string P2 { get; set; }
        public (int Min, int Max) Y { get; set; }

        public string QueryName { get; set; }

        public Query(string p1, int x, string p2)
        {
            P1 = p1;
            X = x;
            P2 = p2;
        }

        public Query(string p1, int x, string p2, string queryName)
        {
            P1 = p1;
            X = x;
            P2 = p2;
            QueryName = queryName;

        }
    }
}
