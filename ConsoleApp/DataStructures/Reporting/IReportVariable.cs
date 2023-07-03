namespace ConsoleApp.DataStructures.Reporting
{
    public interface IReportVariable
    {
        IEnumerable<(int, int)> Matches(string pattern1, int minGap, int maxGap, string pattern2);
        int[] ReportSortedOccurrences(string pattern);
    }
}