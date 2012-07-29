#region Usings

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Helper;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for IndexHelperUnitTest and is intended
    ///to contain all IndexHelperUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IndexHelperUnitTest
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
        ///A test for CheckObject
        ///</summary>
        public void CheckObjectUnitTestHelper<T>()
            where T : class
        {
            T result = null; // TODO: Initialize to an appropriate value
            T resultExpected = null; // TODO: Initialize to an appropriate value
            object obj = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = IndexHelper.CheckObject<T>(out result, obj);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CheckObjectUnitTest()
        {
            Assert.Inconclusive("TODO");

            CheckObjectUnitTestHelper<GenericParameterHelper>();
        }
    }
}
