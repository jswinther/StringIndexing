using ConsoleApp;
using ConsoleApp.DataStructures.Existence;

namespace Tests
{
    [TestClass]
    public class SA_E_V2_Tests
    {
        static SA_E_V2 SA;
        [TestMethod]
        public void A_DNA_512()
        {
            SA = new SA_E_V2(DummyData.DNA("DNA_512"), 5, 1, 45);
        }

        [TestMethod]
        public void A_Query_512()
        {
            Assert.IsTrue(SA.FixedExists("d", "dS"));
        }


        [TestMethod]
        public void B_DNA_262144()
        {
            SA = new SA_E_V2(DummyData.DNA("DNA_262144"), 5, 1, 45);
        }

        [TestMethod]
        public void B_Query_262144()
        {
            Assert.IsTrue(SA.FixedExists("a", "a"));
        }

        [TestMethod]
        public void C_DNA_524288()
        {
            SA = new SA_E_V2(DummyData.DNA("DNA_524288"), 5, 1, 45);
        }

        [TestMethod]
        public void C_Query_524288()
        {
            Assert.IsTrue(SA.FixedExists("a", "a"));
        }

        [TestMethod]
        public void D_DNA_1048576()
        {
            SA = new SA_E_V2(DummyData.DNA("DNA_1048576"), 5, 1, 45);
        }

        [TestMethod]
        public void D_Query_1048576()
        {
            Assert.IsTrue(SA.FixedExists("acat", "gtag"));
        }

        [TestMethod]
        public void E_DNA_4194304()
        {
            SA = new SA_E_V2(DummyData.DNA("DNA_4194304"), 5, 1, 45);
        }

        [TestMethod]
        public void E_Query_4194304()
        {
            Assert.IsTrue(SA.FixedExists("acat", "gtag"));
        }

        [TestMethod]
        public void F_DNA_8388608()
        {
            SA = new SA_E_V2(DummyData.DNA("DNA_8388608"), 5, 1, 45);
        }

        [TestMethod]
        public void F_Query_8388608()
        {
            Assert.IsTrue(SA.FixedExists("acat", "gtag"));
        }

        [TestMethod]
        public void G_DNA_16777216()
        {
            SA = new SA_E_V2(DummyData.DNA("DNA_16777216"), 5, 1, 45);
        }

        [TestMethod]
        public void G_Query_16777216()
        {
            Assert.IsTrue(SA.FixedExists("acat", "gtag"));

        }
    }
}