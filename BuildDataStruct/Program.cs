using ConsoleApp.DataStructures;

namespace BuildDataStruct
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var sa = SuffixArrayFinal.CreateSuffixArray("banana");
            sa.BuildChildTable();
            Console.WriteLine();
        }
    }
}