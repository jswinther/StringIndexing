namespace ConsoleApp.Data.Obsolete
{
    internal abstract class RangeMinimumQuery
    {
        protected int[] arr;
        protected int N;

        protected RangeMinimumQuery(int[] arr)
        {
            this.arr = arr;
            N = arr.Length;
        }

        public abstract int RMQ(int startIndex, int endIndex);
    }
}
