namespace ConsoleApp.DataStructures
{
    public class IntervalNode
    {
        public (int start, int end) Interval { get; set; }
        public int Size { get => Interval.end + 1 - Interval.start; }
        public int LeftMostLeaf { get; set; } = int.MaxValue;
        public int RightMostLeaf { get; set; } = int.MinValue;
        public IntervalNode Parent { get; set; }
        public List<IntervalNode> Children { get; set; } = new();
        public int DistanceToRoot { get; set; }
        public int DeepestLeaf { get; set; } = int.MinValue;
        public System.Collections.Generic.HashSet<(int, int)> MatchingIntervals { get; set; } = new();

        public bool Merged { get; set; }

        public IntervalNode((int start, int end) interval, IntervalNode parent, int distanceToRoot)
        {
            Interval = interval;
            Parent = parent;
            DistanceToRoot = distanceToRoot;
            Merged = false;
        }

        public bool IsLeaf { get => Children.Count == 0; }
        public int[] SortedOccurrences { get; set; }
    }
}
