#region Usings

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Index.Fulltext;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for RegExIndexUnitTest and is intended
    ///to contain all RegExIndexUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RegExIndexUnitTest
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
        ///A test for RegExIndex Constructor
        ///</summary>
        [TestMethod()]
        public void RegExIndexConstructorUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new RegExIndex();
        }

        /// <summary>
        ///A test for AddOrUpdate
        ///</summary>
        [TestMethod()]
        public void AddOrUpdateUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new RegExIndex(); // TODO: Initialize to an appropriate value
            object keyObject = null; // TODO: Initialize to an appropriate value
            AGraphElement graphElement = null; // TODO: Initialize to an appropriate value
            target.AddOrUpdate(keyObject, graphElement);
        }

        /// <summary>
        ///A test for CountOfKeys
        ///</summary>
        [TestMethod()]
        public void CountOfKeysUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new RegExIndex(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.CountOfKeys();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CountOfValues
        ///</summary>
        [TestMethod()]
        public void CountOfValuesUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new RegExIndex(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.CountOfValues();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetKeyValues
        ///</summary>
        [TestMethod()]
        public void GetKeyValuesUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new RegExIndex(); // TODO: Initialize to an appropriate value
            IEnumerable<KeyValuePair<object, ReadOnlyCollection<AGraphElement>>> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<KeyValuePair<object, ReadOnlyCollection<AGraphElement>>> actual;
            actual = target.GetKeyValues();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetKeys
        ///</summary>
        [TestMethod()]
        public void GetKeysUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new RegExIndex(); // TODO: Initialize to an appropriate value
            IEnumerable<object> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<object> actual;
            actual = target.GetKeys();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Initialize
        ///</summary>
        [TestMethod()]
        public void InitializeUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new RegExIndex(); // TODO: Initialize to an appropriate value
            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            IDictionary<string, object> parameter = null; // TODO: Initialize to an appropriate value
            target.Initialize(fallen8, parameter);
        }

        /// <summary>
        ///A test for RemoveValue
        ///</summary>
        [TestMethod()]
        public void RemoveValueUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new RegExIndex(); // TODO: Initialize to an appropriate value
            AGraphElement graphElement = null; // TODO: Initialize to an appropriate value
            target.RemoveValue(graphElement);
        }

        /// <summary>
        ///A test for TryGetValue
        ///</summary>
        [TestMethod()]
        public void TryGetValueUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new RegExIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            object keyObject = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryGetValue(out result, keyObject);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryQuery
        ///</summary>
        [TestMethod()]
        public void TryQueryUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new RegExIndex(); // TODO: Initialize to an appropriate value
            FulltextSearchResult result = null; // TODO: Initialize to an appropriate value
            FulltextSearchResult resultExpected = null; // TODO: Initialize to an appropriate value
            string query = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryQuery(out result, query);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryRemoveKey
        ///</summary>
        [TestMethod()]
        public void TryRemoveKeyUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new RegExIndex(); // TODO: Initialize to an appropriate value
            object keyObject = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryRemoveKey(keyObject);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Wipe
        ///</summary>
        [TestMethod()]
        public void WipeUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new RegExIndex(); // TODO: Initialize to an appropriate value
            target.Wipe();
        }
    }
}
