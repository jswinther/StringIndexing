using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Benchmark
    {
        public long ElapsedMilliseconds { get; set; }
        public long ConstructionTimeMilliseconds { get; set; }
        public long QueryTimeMilliseconds { get; set; }
    }
}
