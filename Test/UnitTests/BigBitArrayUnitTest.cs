#region Usings

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Helper;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for BigBitArrayUnitTest and is intended
    ///to contain all BigBitArrayUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BigBitArrayUnitTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for BigBitArray Constructor
        ///</summary>
        [TestMethod()]
        public void BigBitArrayConstructorUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new BigBitArray();
        }

        /// <summary>
        ///A test for Clear
        ///</summary>
        [TestMethod()]
        public void ClearUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new BigBitArray(); // TODO: Initialize to an appropriate value
            target.Clear();
        }

        /// <summary>
        ///A test for GetValue
        ///</summary>
        [TestMethod()]
        public void GetValueUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new BigBitArray(); // TODO: Initialize to an appropriate value
            int index = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.GetValue(index);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SetValue
        ///</summary>
        [TestMethod()]
        public void SetValueUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new BigBitArray(); // TODO: Initialize to an appropriate value
            int index = 0; // TODO: Initialize to an appropriate value
            bool value = false; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.SetValue(index, value);
            Assert.AreEqual(expected, actual);
        }
    }
}
