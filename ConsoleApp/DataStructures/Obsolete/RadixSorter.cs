using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Obsolete
{
    internal class RadixSorter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array">array to be sorted</param>
        /// <param name="maxVal">the length of the string</param>
        /// <returns></returns>
        public static int[] Sort(int[] array, int maxVal)
        {
            for (int exponent = 1; maxVal / exponent > 0; exponent *= 10)
                CountingSort(array, exponent);
            return array;
        }

        public static void CountingSort(int[] array, int exponent)
        {
            int size = array.Length;
            var outputArr = new int[size];
            var occurences = new int[10];
            for (int i = 0; i < 10; i++)
                occurences[i] = 0;
            for (int i = 0; i < size; i++)
                occurences[array[i] / exponent % 10]++;
            for (int i = 1; i < 10; i++)
                occurences[i] += occurences[i - 1];
            for (int i = size - 1; i >= 0; i--)
            {
                outputArr[occurences[array[i] / exponent % 10] - 1] = array[i];
                occurences[array[i] / exponent % 10]--;
            }
            for (int i = 0; i < size; i++)
                array[i] = outputArr[i];
        }


    }
}
