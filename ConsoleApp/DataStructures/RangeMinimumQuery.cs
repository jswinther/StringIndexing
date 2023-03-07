namespace ConsoleApp.DataStructures
{
    internal abstract class RangeMinimumQuery
    {
        protected int[] arr;
        protected int N;

        protected RangeMinimumQuery(int[] arr)
        {
            this.arr = arr;
            this.N = arr.Length;
        }

        public abstract int RMQ(int startIndex, int endIndex);
    }
}
