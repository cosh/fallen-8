#region Usings

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Algorithms.Path;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for BidirectionalLevelSynchronousSSSPUnitTest and is intended
    ///to contain all BidirectionalLevelSynchronousSSSPUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BidirectionalLevelSynchronousSSSPUnitTest
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
        ///A test for Calculate
        ///</summary>
        [TestMethod()]
        public void CalculateUnitTest()
        {
            Assert.Inconclusive("TODO");

            var target = new BidirectionalLevelSynchronousSSSP(); // TODO: Initialize to an appropriate value
            int sourceVertexId = 0; // TODO: Initialize to an appropriate value
            int destinationVertexId = 0; // TODO: Initialize to an appropriate value
            int maxDepth = 0; // TODO: Initialize to an appropriate value
            double maxPathWeight = 0F; // TODO: Initialize to an appropriate value
            int maxResults = 0; // TODO: Initialize to an appropriate value
            PathDelegates.EdgePropertyFilter edgePropertyFilter = null; // TODO: Initialize to an appropriate value
            PathDelegates.VertexFilter vertexFilter = null; // TODO: Initialize to an appropriate value
            PathDelegates.EdgeFilter edgeFilter = null; // TODO: Initialize to an appropriate value
            PathDelegates.EdgeCost edgeCost = null; // TODO: Initialize to an appropriate value
            PathDelegates.VertexCost vertexCost = null; // TODO: Initialize to an appropriate value
            List<Path> expected = null; // TODO: Initialize to an appropriate value
            List<Path> actual;
            actual = target.Calculate(sourceVertexId, destinationVertexId, maxDepth, maxPathWeight, maxResults, edgePropertyFilter, vertexFilter, edgeFilter, edgeCost, vertexCost);
            Assert.AreEqual(expected, actual);
        }
    }
}
