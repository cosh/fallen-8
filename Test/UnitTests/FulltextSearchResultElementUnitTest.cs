#region Usings

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Index.Fulltext;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for FulltextSearchResultElementUnitTest and is intended
    ///to contain all FulltextSearchResultElementUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FulltextSearchResultElementUnitTest
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
        ///A test for FulltextSearchResultElement Constructor
        ///</summary>
        [TestMethod()]
        public void FulltextSearchResultElementConstructorUnitTest()
        {
            Assert.Inconclusive("TODO");

            AGraphElement graphElement = null; // TODO: Initialize to an appropriate value
            double score = 0F; // TODO: Initialize to an appropriate value
            IEnumerable<string> highlights = null; // TODO: Initialize to an appropriate value
            var target = new FulltextSearchResultElement(graphElement, score, highlights);
        }

        /// <summary>
        ///A test for AddHighlight
        ///</summary>
        [TestMethod()]
        public void AddHighlightUnitTest()
        {
            Assert.Inconclusive("TODO");

            AGraphElement graphElement = null; // TODO: Initialize to an appropriate value
            double score = 0F; // TODO: Initialize to an appropriate value
            IEnumerable<string> highlights = null; // TODO: Initialize to an appropriate value
            var target = new FulltextSearchResultElement(graphElement, score, highlights); // TODO: Initialize to an appropriate value
            string highlight = string.Empty; // TODO: Initialize to an appropriate value
            target.AddHighlight(highlight);
        }
    }
}
