#region Usings

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Helper;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for Fallen8PoolUnitTest and is intended
    ///to contain all Fallen8PoolUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Fallen8PoolUnitTest
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
        ///A test for Fallen8Pool Constructor
        ///</summary>
        [TestMethod()]
        public void Fallen8PoolConstructorUnitTest()
        {
            Assert.Inconclusive("TODO");

            uint minValue = 0; // TODO: Initialize to an appropriate value
            uint maxValue = 0; // TODO: Initialize to an appropriate value
            var target = new Fallen8Pool(minValue, maxValue);
        }

        /// <summary>
        ///A test for TryGetFallen8
        ///</summary>
        [TestMethod()]
        public void TryGetFallen8UnitTest()
        {
            Assert.Inconclusive("TODO");

            uint minValue = 0; // TODO: Initialize to an appropriate value
            uint maxValue = 0; // TODO: Initialize to an appropriate value
            var target = new Fallen8Pool(minValue, maxValue); // TODO: Initialize to an appropriate value
            Fallen8 result = null; // TODO: Initialize to an appropriate value
            Fallen8 resultExpected = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryGetFallen8(out result);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for RecycleFallen8
        ///</summary>
        [TestMethod()]
        public void RecycleFallen8UnitTest()
        {
            Assert.Inconclusive("TODO");

            uint minValue = 0; // TODO: Initialize to an appropriate value
            uint maxValue = 0; // TODO: Initialize to an appropriate value
            var target = new Fallen8Pool(minValue, maxValue); // TODO: Initialize to an appropriate value
            Fallen8 instance = null; // TODO: Initialize to an appropriate value
            target.RecycleFallen8(instance);
        }
    }
}
