#region Usings

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Index.Spatial;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for IGeometryUnitTest and is intended
    ///to contain all IGeometryUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IGeometryUnitTest
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


        internal virtual IGeometry CreateIGeometry()
        {
            // TODO: Instantiate an appropriate concrete class.
            IGeometry target = null;
            return target;
        }

        /// <summary>
        ///A test for GeometryToMBR
        ///</summary>
        [TestMethod()]
        public void GeometryToMBRUnitTest()
        {
            Assert.Inconclusive("TODO");


            IGeometry target = CreateIGeometry(); // TODO: Initialize to an appropriate value
            IMBR expected = null; // TODO: Initialize to an appropriate value
            IMBR actual;
            actual = target.GeometryToMBR();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Dimensions
        ///</summary>
        [TestMethod()]
        public void DimensionsUnitTest()
        {
            Assert.Inconclusive("TODO");

            IGeometry target = CreateIGeometry(); // TODO: Initialize to an appropriate value
            List<IDimension> actual;
            actual = target.Dimensions;
        }
    }
}
