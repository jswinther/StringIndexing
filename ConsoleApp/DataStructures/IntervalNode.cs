namespace ConsoleApp.DataStructures
{
    internal partial class SuffixArrayFinal
    {
        public class IntervalNode
        {
            public (int start, int end) Interval { get; set; }
            public int LeftMostLeaf { get; set; } = int.MaxValue;
            public int RightMostLeaf { get; set; } = int.MinValue;
            public (int, int) Parent { get; set; }
            public List<(int, int)> Children { get; set; } = new();
            public int DistanceToRoot { get; set; }
            public (int Start, int End) TopNodesInterval { get; set; }

            public IntervalNode((int start, int end) interval, (int start, int end) parent, int distanceToRoot)
            {
                Interval = interval;
                Parent = parent;
                DistanceToRoot = distanceToRoot;
            }

            public bool IsLeaf { get => Children.Count == 0; }
            public int[] SortedOccurrences { get; set; }
        }
    }
}
