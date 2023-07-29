using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public static class Helper
    {
        public static int[] KWayMerge(int[][] arrays)
        {
            PriorityQueue<int, int> minHeap = new PriorityQueue<int, int>();
            int[] counters = new int[arrays.Length];

            for (int i = 0; i < arrays.Length; i++)
            {
                if (arrays[i].Length > 0)
                {
                    minHeap.Enqueue(i, arrays[i][counters[i]++]);
                }
            }


            int totalElements = arrays.Sum(a => a.Length);
            int[] result = new int[totalElements];
            int index = 0;

            while (minHeap.TryDequeue(out int arrayIndex, out int value) && index < totalElements)
            {
                result[index++] = value;
                if (arrays[arrayIndex].Length > counters[arrayIndex])
                {
                    minHeap.Enqueue(arrayIndex, arrays[arrayIndex][counters[arrayIndex]++]);
                }
            }
            return result;
        }

        public static DirectoryInfo TryGetSolutionDirectoryInfo(string currentPath = null)
        {
            var directory = new DirectoryInfo(
                currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            return directory;
        }
    }
}
