#region Usings

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
            AGraphElement target = null;
            return target;
        }

        /// <summary>
        ///A test for GetAllProperties
        ///</summary>
        [TestMethod()]
        public void GetAllPropertiesUnitTest()
        {
            Assert.Inconclusive("TODO");

            AGraphElement target = CreateAGraphElement(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<PropertyContainer> expected = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<PropertyContainer> actual;
            actual = target.GetAllProperties();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetPropertyCount
        ///</summary>
        [TestMethod()]
        public void GetPropertyCountUnitTest()
        {
            Assert.Inconclusive("TODO");

            AGraphElement target = CreateAGraphElement(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.GetPropertyCount();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryGetProperty
        ///</summary>
        public void TryGetPropertyUnitTestHelper<TProperty>()
        {
            Assert.Inconclusive("TODO");

            AGraphElement target = CreateAGraphElement(); // TODO: Initialize to an appropriate value
            TProperty result = default(TProperty); // TODO: Initialize to an appropriate value
            TProperty resultExpected = default(TProperty); // TODO: Initialize to an appropriate value
            ushort propertyId = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryGetProperty<TProperty>(out result, propertyId);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TryGetPropertyUnitTest()
        {
            Assert.Inconclusive("TODO");

            TryGetPropertyUnitTestHelper<GenericParameterHelper>();
        }
    }
}
