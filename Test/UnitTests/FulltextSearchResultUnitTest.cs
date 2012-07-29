#region Usings

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Index.Fulltext;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for FulltextSearchResultUnitTest and is intended
    ///to contain all FulltextSearchResultUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FulltextSearchResultUnitTest
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
        ///A test for AddElement
        ///</summary>
        [TestMethod()]
        public void AddElementUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new FulltextSearchResult(); // TODO: Initialize to an appropriate value
            FulltextSearchResultElement element = null; // TODO: Initialize to an appropriate value
            target.AddElement(element);
        }

        /// <summary>
        ///A test for Elements
        ///</summary>
        [TestMethod()]
        public void ElementsUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new FulltextSearchResult(); // TODO: Initialize to an appropriate value
            List<FulltextSearchResultElement> expected = null; // TODO: Initialize to an appropriate value
            List<FulltextSearchResultElement> actual;
            target.Elements = expected;
            actual = target.Elements;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for MaximumScore
        ///</summary>
        [TestMethod()]
        public void MaximumScoreUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new FulltextSearchResult(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.MaximumScore = expected;
            actual = target.MaximumScore;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FulltextSearchResult Constructor
        ///</summary>
        [TestMethod()]
        public void FulltextSearchResultConstructorUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new FulltextSearchResult();
        }
    }
}
