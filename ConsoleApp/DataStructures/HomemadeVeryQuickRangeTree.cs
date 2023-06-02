using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    internal class HomemadeVeryQuickRangeTree
    {
        private readonly int[] x;
        private readonly int[] y;
        private (int, int)[] points;

        public (int, int)[] Points
        {
            get { return points; }
            set { points = value; }
        }

        public (int, int) this[int i]
        {
            get { return (x[i], y[i]); }
            set { (x[i], y[i]) = value; }
        }




    }
}
