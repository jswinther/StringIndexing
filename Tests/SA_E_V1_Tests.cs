using ConsoleApp;
using ConsoleApp.DataStructures.Existence;

namespace Tests
{
    [TestClass]
    public class SA_E_V1_Tests
    {
        static SA_E_V1 SA;
        [TestMethod]
        public void A_DNA_512()
        {
           SA = new SA_E_V1(DummyData.DNA("DNA_512"), 5, 1, 45);
        }

        [TestMethod]
        public void A_Query_512()
        {
            Assert.IsTrue(SA.FixedExists("a", "a"));
        }

        [TestMethod]
        public void B_DNA_262144()
        {
            SA = new SA_E_V1(DummyData.DNA("DNA_262144"), 5, 1, 45);
        }

        [TestMethod]
        public void B_Query_262144()
        {
            Assert.IsTrue(SA.FixedExists("a", "a"));
        }
    }
}