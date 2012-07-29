#region Usings

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Index.Fulltext;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for IFulltextIndexUnitTest and is intended
    ///to contain all IFulltextIndexUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IFulltextIndexUnitTest
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


        internal virtual IFulltextIndex CreateIFulltextIndex()
        {
            // TODO: Instantiate an appropriate concrete class.
            IFulltextIndex target = null;
            return target;
        }

        /// <summary>
        ///A test for TryQuery
        ///</summary>
        [TestMethod()]
        public void TryQueryUnitTest()
        {
            Assert.Inconclusive("TODO");

            IFulltextIndex target = CreateIFulltextIndex(); // TODO: Initialize to an appropriate value
            FulltextSearchResult result = null; // TODO: Initialize to an appropriate value
            FulltextSearchResult resultExpected = null; // TODO: Initialize to an appropriate value
            string query = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryQuery(out result, query);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }
    }
}
