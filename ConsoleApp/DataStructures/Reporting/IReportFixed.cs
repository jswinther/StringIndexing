namespace ConsoleApp.DataStructures.Reporting
{
    public interface IReportFixed
    {
        IEnumerable<int> Matches(string pattern1, int x, string pattern2);
        HashSet<int> ReportHashedOccurrences(string pattern);
    }
}