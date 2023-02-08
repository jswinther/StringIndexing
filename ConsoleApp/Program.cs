using System;
using System.Diagnostics;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var s1 = new PrecomputedSubstrings(DummyData.DNA1000);





            foreach (var occ in s1.Report("tat", 2, "ctc"))
            {
                global::System.Console.WriteLine(occ);
            }
        }
    }
}

