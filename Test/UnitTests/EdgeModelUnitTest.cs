#region Usings

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for EdgeModelUnitTest and is intended
    ///to contain all EdgeModelUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EdgeModelUnitTest
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
        ///A test for EdgeModel Constructor
        ///</summary>
        [TestMethod()]
        public void EdgeModelConstructorUnitTest()
        {
            Assert.Inconclusive("TODO");

            int id = 0; // TODO: Initialize to an appropriate value
            uint creationDate = 0; // TODO: Initialize to an appropriate value
            VertexModel targetVertex = null; // TODO: Initialize to an appropriate value
            VertexModel sourceVertex = null; // TODO: Initialize to an appropriate value
            PropertyContainer[] properties = null; // TODO: Initialize to an appropriate value
            var target = new EdgeModel(id, creationDate, targetVertex, sourceVertex, properties);
        }
    }
}
