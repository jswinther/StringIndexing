using ConsoleApp;
using ConsoleApp.DataStructures;
using System.Text;

namespace StringTests
{
    [TestClass]
    public class SuffixArrayTests
    {
        private SuffixArrayWrapper SA;
        private Problem problem;
        public SuffixArrayTests()
        {
            string text = DummyData.DNA_512;
            //string text = "banana$";
            string p1 = "t";
            //string p1 = "a";
            Random random = new Random();
            int x = random.Next();
            string p2 = "g";
            //string p2 = "a";
            Query query = new Query(p1, x, p2);
            query.Y = (3, 10);
            problem = new Problem(text, query);
        }

        [TestMethod]
        public void Construction_512()
        {
            SA = new SuffixArrayWrapper(problem.Text);
        }

        [TestMethod]
        public void Query_512()
        {
            SA = new SuffixArrayWrapper(problem.Text);
            SA.GetOccurrencesWithSortedSet(problem.Query);
        }
    }
}