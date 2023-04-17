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
            return array.Take(new Range(s, e + 1));
        }

        public static bool ContainsElementInRange(this int[] array, int x, int y)
        {
            return (array.BinarySearchOnRange(x, y) != (-1, -1));
        }
        
    }
}
