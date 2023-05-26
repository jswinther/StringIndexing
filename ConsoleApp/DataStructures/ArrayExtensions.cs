namespace ConsoleApp.DataStructures
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns the index of successor of min and predecessor of max
        /// </summary>
        /// <param name="array"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static (int, int) BinarySearchOnRange(this int[] array, int min, int max)
        {
            int minIndex = array.IndexOfSucessor(min);
            int maxIndex = array.IndexOfPredecessor(max);
            if (minIndex == -1 || array[minIndex] > max)
            {
                return (-1, -1);
            }
            return (minIndex, maxIndex);
        }

        public static (int, int) BinarySearchOnRange(this Span<int> array, int min, int max)
        {
            int minIndex = array.IndexOfSucessor(min);
            int maxIndex = array.IndexOfPredecessor(max);
            if (minIndex == -1 || array[minIndex] > max)
            {
                return (-1, -1);
            }
            return (minIndex, maxIndex);
        }

        private static Random Random = new Random();

        public static T GetRandom<T>(this IEnumerable<T> values)
        {
            return values.ElementAt(Random.Next(values.Count()));
        }

        public static IEnumerable<T> GetRandom<T>(this IEnumerable<T> values, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                yield return values.GetRandom();
            }
        }

        public static string GetSubstringOfPercentageOfString(this string s, double percent)
        {
            double maxStart = s.Length * (1 - percent);
            double maxLength = s.Length * percent;
            return s.Substring(Random.Next((int)maxStart), (int)maxLength);
        }

        public static IEnumerable<string> GetSubstringOfPercentageOfString(this string s, double percent, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                yield return s.GetSubstringOfPercentageOfString(percent);
            }
        }

        public static int IndexOfSucessor(this int[] array, int v)
        {
            int left = 0;
            int right = array.Length - 1;
            int result = -1;

            while (left <= right)
            {
                int mid = (left + right) / 2;

                if (array[mid] <= v)
                {
                    left = mid + 1;
                }
                else
                {
                    result = mid;
                    right = mid - 1;
                }
            }
            return result;
        }

        public static int IndexOfSucessor(this Span<int> array, int v)
        {
            int left = 0;
            int right = array.Length - 1;
            int result = -1;

            while (left <= right)
            {
                int mid = (left + right) / 2;

                if (array[mid] <= v)
                {
                    left = mid + 1;
                }
                else
                {
                    result = mid;
                    right = mid - 1;
                }
            }
            return result;
        }

        public static int IndexOfPredecessor(this int[] array, int v)
        {
            int left = 0;
            int right = array.Length - 1;
            int result = -1;

            while (left <= right)
            {
                int mid = (left + right) / 2;

                if (array[mid] >= v)
                {
                    right = mid - 1;
                }
                else
                {
                    result = mid;
                    left = mid + 1;
                }
            }

            return result;
        }

        public static int IndexOfPredecessor(this Span<int> array, int v)
        {
            int left = 0;
            int right = array.Length - 1;
            int result = -1;

            while (left <= right)
            {
                int mid = (left + right) / 2;

                if (array[mid] >= v)
                {
                    right = mid - 1;
                }
                else
                {
                    result = mid;
                    left = mid + 1;
                }
            }

            return result;
        }

        public static IEnumerable<int> GetViewBetween(this int[] array, int x, int y)
        {
            (int s, int e) = array.BinarySearchOnRange(x, y);
            if (s < 0) return Enumerable.Empty<int>();
            return array.Take(new Range(s, e + 1));
        }

        public static int[] Sort(this int[] a)
        {
            if (SuffixArrayFinal.k >= Math.Log2(a.Length))
            {
                Array.Sort(a);
                return a;
            }

            // our helper array 
            int[] t = new int[a.Length];

            // number of bits our group will be long 
            int r = 4; // try to set this also to 2, 8 or 16 to see if it is quicker or not 

            // number of bits of a C# int 
            int b = 32;

            // counting and prefix arrays
            // (note dimensions 2^r which is the number of all possible values of a r-bit number) 
            int[] count = new int[1 << r];
            int[] pref = new int[1 << r];

            // number of groups 
            int groups = (int)Math.Ceiling(b / (double)r);

            // the mask to identify groups 
            int mask = (1 << r) - 1;

            // the algorithm: 
            for (int c = 0, shift = 0; c < groups; c++, shift += r)
            {
                // reset count array 
                for (int j = 0; j < count.Length; j++)
                    count[j] = 0;

                // counting elements of the c-th group 
                for (int i = 0; i < a.Length; i++)
                    count[a[i] >> shift & mask]++;

                // calculating prefixes 
                pref[0] = 0;
                for (int i = 1; i < count.Length; i++)
                    pref[i] = pref[i - 1] + count[i - 1];

                // from a[] to t[] elements ordered by c-th group 
                for (int i = 0; i < a.Length; i++)
                    t[pref[a[i] >> shift & mask]++] = a[i];

                // a[]=t[] and start again until the last group 
                t.CopyTo(a, 0);
            }
            // a is sorted 
            return a;
        }

        public static bool ContainsElementInRange(this int[] array, int x, int y)
        {
            return array.BinarySearchOnRange(x, y) != (-1, -1);
        }

        public static int MinUnsorted(this int[] array)
        {
            return array.Min();
        }

        public static int MaxUnsorted(this int[] array)
        {
            return array.Max();
        }

        public static (int min, int max) MinMax(this int[] array)
        {
            return (array.MinUnsorted(), array.MaxUnsorted());
        }

        public static int MinSorted(this int[] array)
        {
            return array[0];
        }

        public static int MaxSorted(this int[] array)
        {
            return array[array.Length - 1];
        }
    }
}
