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

        public static IEnumerable<int> GetViewBetween(this int[] array, int x, int y)
        {
            (int s, int e) = array.BinarySearchOnRange(x, y);
            if (s < 0) return Enumerable.Empty<int>();
            return array.Take(new Range(s, e + 1));
        }

        public static bool ContainsElementInRange(this int[] array, int x, int y)
        {
            return (array.BinarySearchOnRange(x, y) != (-1, -1));
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
