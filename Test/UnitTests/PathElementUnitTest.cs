#region Usings

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Algorithms.Path;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for PathElementUnitTest and is intended
    ///to contain all PathElementUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PathElementUnitTest
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
        ///A test for PathElement Constructor
        ///</summary>
        [TestMethod()]
        public void PathElementConstructorUnitTest()
        {
            Assert.Inconclusive("TODO");

            EdgeModel edge = null; // TODO: Initialize to an appropriate value
            ushort edgePropertyId = 0; // TODO: Initialize to an appropriate value
            var direction = new Direction(); // TODO: Initialize to an appropriate value
            double weight = 0F; // TODO: Initialize to an appropriate value
            var target = new PathElement(edge, edgePropertyId, direction, weight);
        }

        /// <summary>
        ///A test for CalculateWeight
        ///</summary>
        [TestMethod()]
        public void CalculateWeightUnitTest()
        {
            Assert.Inconclusive("TODO");

            EdgeModel edge = null; // TODO: Initialize to an appropriate value
            ushort edgePropertyId = 0; // TODO: Initialize to an appropriate value
            var direction = new Direction(); // TODO: Initialize to an appropriate value
            double weight = 0F; // TODO: Initialize to an appropriate value
            var target = new PathElement(edge, edgePropertyId, direction, weight); // TODO: Initialize to an appropriate value
            PathDelegates.VertexCost vertexCost = null; // TODO: Initialize to an appropriate value
            PathDelegates.EdgeCost edgeCost = null; // TODO: Initialize to an appropriate value
            target.CalculateWeight(vertexCost, edgeCost);
        }

        /// <summary>
        ///A test for TargetVertex
        ///</summary>
        [TestMethod()]
        public void TargetVertexUnitTest()
        {
            Assert.Inconclusive("TODO");

            EdgeModel edge = null; // TODO: Initialize to an appropriate value
            ushort edgePropertyId = 0; // TODO: Initialize to an appropriate value
            var direction = new Direction(); // TODO: Initialize to an appropriate value
            double weight = 0F; // TODO: Initialize to an appropriate value
            var target = new PathElement(edge, edgePropertyId, direction, weight); // TODO: Initialize to an appropriate value
            VertexModel actual;
            actual = target.TargetVertex;
        }

        /// <summary>
        ///A test for SourceVertex
        ///</summary>
        [TestMethod()]
        public void SourceVertexUnitTest()
        {
            Assert.Inconclusive("TODO");

            EdgeModel edge = null; // TODO: Initialize to an appropriate value
            ushort edgePropertyId = 0; // TODO: Initialize to an appropriate value
            var direction = new Direction(); // TODO: Initialize to an appropriate value
            double weight = 0F; // TODO: Initialize to an appropriate value
            var target = new PathElement(edge, edgePropertyId, direction, weight); // TODO: Initialize to an appropriate value
            VertexModel actual;
            actual = target.SourceVertex;
        }
    }
}
