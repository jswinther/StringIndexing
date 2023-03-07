namespace ConsoleApp.DataStructures
{
    internal class SparseTable : RangeMinimumQuery
    {
        private int[,] lookup;
        public SparseTable(int[] arr) : base(arr)
        {
            int logArr = (int)Math.Ceiling(Math.Log2(N));
            lookup = new int[logArr, logArr];
            // Initialize M for the intervals with length 1
            for (int i = 0; i < N; i++)
                lookup[i, 0] = arr[i];

            // Compute values from smaller to bigger intervals
            for (int j = 1; (1 << j) <= N; j++)
            {

                // Compute minimum value for all intervals with
                // size 2^j
                for (int i = 0; (i + (1 << j) - 1) < N; i++)
                {

                    // For arr[2][10], we compare arr[lookup[0][7]]
                    // and arr[lookup[3][10]]
                    if (lookup[i, j - 1] <
                                lookup[i + (1 << (j - 1)), j - 1])
                        lookup[i, j] = lookup[i, j - 1];
                    else
                        lookup[i, j] =
                                lookup[i + (1 << (j - 1)), j - 1];
                }
            }
        }

        public override int RMQ(int startIndex, int endIndex)
        {
            // Find highest power of 2 that is smaller
            // than or equal to count of elements in given
            // range. For [2, 10], j = 3
            int j = (int)Math.Log(endIndex - startIndex + 1);

            // Compute minimum of last 2^j elements with first
            // 2^j elements in range.
            // For [2, 10], we compare arr[lookup[0][3]] and
            // arr[lookup[3][3]],
            if (lookup[startIndex, j] <= lookup[endIndex - (1 << j) + 1, j])
                return lookup[startIndex, j];

            else
                return lookup[endIndex - (1 << j) + 1, j];
        }
    }
}
