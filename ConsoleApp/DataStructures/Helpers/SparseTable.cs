namespace ConsoleApp.DataStructures.Helpers
{
    internal class SparseTable : RangeMinimumQuery
    {
        private int[,] table;
        private int[] logTable;

        public int this[int a, int b] => RMQ(a, b);

        public SparseTable(int[] arr) : base(arr)
        {
            int n = arr.Length;
            int logN = (int)Math.Log(n, 2) + 1;
            table = new int[n, logN];
            logTable = new int[n + 1];

            // Initialize the table with the base case values
            for (int i = 0; i < n; i++)
            {
                table[i, 0] = arr[i];
            }

            // Compute the values for the rest of the table
            for (int j = 1; j < logN; j++)
            {
                for (int i = 0; i + (1 << j) <= n; i++)
                {
                    table[i, j] = Math.Min(table[i, j - 1], table[i + (1 << j - 1), j - 1]);
                }
            }

            // Precompute the logarithm table for faster queries
            for (int i = 2; i <= n; i++)
            {
                logTable[i] = logTable[i / 2] + 1;
            }
        }

        public override int RMQ(int left, int right)
        {
            if (left > right) return 0;
            int length = right - left + 1;
            int k = logTable[length];
            return Math.Min(table[left, k], table[right - (1 << k) + 1, k]);
        }
    }

}
