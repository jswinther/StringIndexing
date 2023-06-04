namespace ConsoleApp
{
    public partial class Program
    {
        public class Benchmark
        {
            public string DataStructureName { get; set; }
            public string DataName { get; set; }

            public string QueryName { get; set; }
            public long ConstructionTimeMilliseconds { get; set; }
            public long QueryTimeMilliseconds { get; set; }
            public long SinglePatternMatchesQuery { get; internal set; }
            public long DoublePatternFixedMatchesQuery { get; internal set; }
            public long DoublePatternVariableMatchesQuery { get; internal set; }
            public object SinglePatternMatchesQueryOccs { get; internal set; }
            public object DoublePatternFixedMatchesQueryOccs { get; internal set; }
            public object DoublePatternVariableMatchesQueryOccs { get; internal set; }
        }
    }
}

