#region Usings

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Index;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for IndexFactoryUnitTest and is intended
    ///to contain all IndexFactoryUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IndexFactoryUnitTest
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
        ///A test for IndexFactory Constructor
        ///</summary>
        [TestMethod()]
        public void IndexFactoryConstructorUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new IndexFactory();
        }

        /// <summary>
        ///A test for GetAvailableIndexPlugins
        ///</summary>
        [TestMethod()]
        public void GetAvailableIndexPluginsUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new IndexFactory(); // TODO: Initialize to an appropriate value
            IEnumerable<string> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<string> actual;
            actual = target.GetAvailableIndexPlugins();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DeleteAllIndices
        ///</summary>
        [TestMethod()]
        public void DeleteAllIndicesUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new IndexFactory(); // TODO: Initialize to an appropriate value
            target.DeleteAllIndices();
        }

        /// <summary>
        ///A test for TryCreateIndex
        ///</summary>
        [TestMethod()]
        public void TryCreateIndexUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new IndexFactory(); // TODO: Initialize to an appropriate value
            IIndex index = null; // TODO: Initialize to an appropriate value
            IIndex indexExpected = null; // TODO: Initialize to an appropriate value
            string indexName = string.Empty; // TODO: Initialize to an appropriate value
            string indexTypeName = string.Empty; // TODO: Initialize to an appropriate value
            IDictionary<string, object> parameter = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryCreateIndex(out index, indexName, indexTypeName, parameter);
            Assert.AreEqual(indexExpected, index);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryDeleteIndex
        ///</summary>
        [TestMethod()]
        public void TryDeleteIndexUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new IndexFactory(); // TODO: Initialize to an appropriate value
            string indexName = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryDeleteIndex(indexName);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryGetIndex
        ///</summary>
        [TestMethod()]
        public void TryGetIndexUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new IndexFactory(); // TODO: Initialize to an appropriate value
            IIndex index = null; // TODO: Initialize to an appropriate value
            IIndex indexExpected = null; // TODO: Initialize to an appropriate value
            string indexName = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryGetIndex(out index, indexName);
            Assert.AreEqual(indexExpected, index);
            Assert.AreEqual(expected, actual);
        }
    }
}
