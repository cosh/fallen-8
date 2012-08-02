#region Usings

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for AGraphElementUnitTest and is intended
    ///to contain all AGraphElementUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AGraphElementUnitTest
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


        internal virtual AGraphElement CreateAGraphElement()
        {
            // TODO: Instantiate an appropriate concrete class.
            AGraphElement target = new VertexModel(0, 0, new PropertyContainer[2]
                                                             {
                                                                 new PropertyContainer {PropertyId = 0, Value = 23},
                                                                 new PropertyContainer {PropertyId = 1, Value = "42"},
                                                             });
            return target;
        }

        /// <summary>
        ///A test for GetAllProperties
        ///</summary>
        [TestMethod()]
        public void GetAllPropertiesUnitTest()
        {
            var target = CreateAGraphElement(); // TODO: Initialize to an appropriate value
            var expected = new ReadOnlyCollection<PropertyContainer>(new List<PropertyContainer>(new PropertyContainer[2]
                                                             {
                                                                 new PropertyContainer {PropertyId = 0, Value = 23},
                                                                 new PropertyContainer {PropertyId = 1, Value = "42"},
                                                             }));
            var actual = target.GetAllProperties();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        /// <summary>
        ///A test for GetPropertyCount
        ///</summary>
        [TestMethod()]
        public void GetPropertyCountUnitTest()
        {
            var target = CreateAGraphElement();
            int expected = 2; 
            int actual;
            actual = target.GetPropertyCount();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TryGetPropertyUnitTest()
        {
            var target = CreateAGraphElement();
            int expected = 23;
            int actual;
            Assert.IsTrue(target.TryGetProperty(out actual, 0));
            Assert.AreEqual(expected, actual);
        }
    }
}
