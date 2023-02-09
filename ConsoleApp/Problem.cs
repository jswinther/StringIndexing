using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Problem
    {
        public string Text { get; set; }
        public Query Query { get; set; }
        public Problem() { }

        public Problem(string text, Query query)
        {
            Text = text;
            Query = query;
        }
    }
}
