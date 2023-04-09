namespace ConsoleApp.DataStructures
{
    public static class ArrayExtensions
    {
        public static IEnumerable<int> GetViewBetween(this int[] array, int x, int y)
        {
            List<int> occs = new();
            //var mid = Array.Find(array, element => x <= element && element <= y);
            
            int lo = 0;
            int hi = array.Length - 1;
            int mid = 0;
            while (lo <= hi)
            {
                mid = lo + (hi - lo) / 2;
           
                if (array[mid] > y)
                {
                    hi = mid - 1;
                }
                else if (array[mid] < x)
                {
                    lo = mid + 1;
                }
                else
                {
                    break;
                }
            }
            // TODO this is not sorted..
            int i = mid, j = mid - 1;
            while (i < array.Length && array[i] <= y)
            {
                occs.Add(array[i]);
                i++;
            }

            while (j > - 1 && array[j] >= x)
            {
                occs.Add(array[j]);
                j--;
            }
            return occs;
        }
    }
}
