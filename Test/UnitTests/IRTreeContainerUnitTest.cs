#region Usings

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Index.Spatial;
using NoSQL.GraphDB.Index.Spatial.Implementation.SpatialContainer;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for IRTreeContainerUnitTest and is intended
    ///to contain all IRTreeContainerUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IRTreeContainerUnitTest
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


        internal virtual IRTreeContainer CreateIRTreeContainer()
        {
            // TODO: Instantiate an appropriate concrete class.
            IRTreeContainer target = null;
            return target;
        }

        /// <summary>
        ///A test for Parent
        ///</summary>
        [TestMethod()]
        public void ParentUnitTest()
        {
            Assert.Inconclusive("TODO");

            IRTreeContainer target = CreateIRTreeContainer(); // TODO: Initialize to an appropriate value
            ARTreeContainer expected = null; // TODO: Initialize to an appropriate value
            ARTreeContainer actual;
            target.Parent = expected;
            actual = target.Parent;
            Assert.AreEqual(expected, actual);
        }
    }
}
