#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Algorithms.Path;
using NoSQL.GraphDB.Expression;
using NoSQL.GraphDB.Index.Fulltext;
using NoSQL.GraphDB.Index.Spatial;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for IReadIntegrationTest and is intended
    ///to contain all IReadIntegrationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IReadIntegrationTest
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


        internal virtual IRead CreateIRead()
        {
            // TODO: Instantiate an appropriate concrete class.
            IRead target = null;
            return target;
        }

        /// <summary>
        ///A test for TryGetVertex
        ///</summary>
        [TestMethod()]
        public void TryGetVertexIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IRead target = CreateIRead(); // TODO: Initialize to an appropriate value
            VertexModel result = null; // TODO: Initialize to an appropriate value
            VertexModel resultExpected = null; // TODO: Initialize to an appropriate value
            int id = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryGetVertex(out result, id);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryGetGraphElement
        ///</summary>
        [TestMethod()]
        public void TryGetGraphElementIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IRead target = CreateIRead(); // TODO: Initialize to an appropriate value
            AGraphElement result = null; // TODO: Initialize to an appropriate value
            AGraphElement resultExpected = null; // TODO: Initialize to an appropriate value
            int id = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryGetGraphElement(out result, id);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryGetEdge
        ///</summary>
        [TestMethod()]
        public void TryGetEdgeIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IRead target = CreateIRead(); // TODO: Initialize to an appropriate value
            EdgeModel result = null; // TODO: Initialize to an appropriate value
            EdgeModel resultExpected = null; // TODO: Initialize to an appropriate value
            int id = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryGetEdge(out result, id);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SpatialIndexScan
        ///</summary>
        [TestMethod()]
        public void SpatialIndexScanIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IRead target = CreateIRead(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            string indexId = string.Empty; // TODO: Initialize to an appropriate value
            IGeometry geometry = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.SpatialIndexScan(out result, indexId, geometry);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IRead target = CreateIRead(); // TODO: Initialize to an appropriate value
            string path = string.Empty; // TODO: Initialize to an appropriate value
            uint savePartitions = 0; // TODO: Initialize to an appropriate value
            target.Save(path, savePartitions);
        }

        /// <summary>
        ///A test for RangeIndexScan
        ///</summary>
        [TestMethod()]
        public void RangeIndexScanIntegrationTest()
        {
            Assert.Inconclusive("TODO.");
            
            IRead target = CreateIRead(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            string indexId = string.Empty; // TODO: Initialize to an appropriate value
            IComparable leftLimit = null; // TODO: Initialize to an appropriate value
            IComparable rightLimit = null; // TODO: Initialize to an appropriate value
            bool includeLeft = false; // TODO: Initialize to an appropriate value
            bool includeRight = false; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.RangeIndexScan(out result, indexId, leftLimit, rightLimit, includeLeft, includeRight);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GraphScan
        ///</summary>
        [TestMethod()]
        public void GraphScanIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IRead target = CreateIRead(); // TODO: Initialize to an appropriate value
            List<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            List<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            ushort propertyId = 0; // TODO: Initialize to an appropriate value
            IComparable literal = null; // TODO: Initialize to an appropriate value
            var binOp = new BinaryOperator(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.GraphScan(out result, propertyId, literal, binOp);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IndexScan
        ///</summary>
        [TestMethod()]
        public void IndexScanIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IRead target = CreateIRead(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            string indexId = string.Empty; // TODO: Initialize to an appropriate value
            IComparable literal = null; // TODO: Initialize to an appropriate value
            var binOp = new BinaryOperator(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IndexScan(out result, indexId, literal, binOp);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetVertices
        ///</summary>
        [TestMethod()]
        public void GetVerticesIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IRead target = CreateIRead(); // TODO: Initialize to an appropriate value
            List<VertexModel> expected = null; // TODO: Initialize to an appropriate value
            List<VertexModel> actual;
            actual = target.GetVertices();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetEdges
        ///</summary>
        [TestMethod()]
        public void GetEdgesIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IRead target = CreateIRead(); // TODO: Initialize to an appropriate value
            List<EdgeModel> expected = null; // TODO: Initialize to an appropriate value
            List<EdgeModel> actual;
            actual = target.GetEdges();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FulltextIndexScan
        ///</summary>
        [TestMethod()]
        public void FulltextIndexScanIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IRead target = CreateIRead(); // TODO: Initialize to an appropriate value
            FulltextSearchResult result = null; // TODO: Initialize to an appropriate value
            FulltextSearchResult resultExpected = null; // TODO: Initialize to an appropriate value
            string indexId = string.Empty; // TODO: Initialize to an appropriate value
            string searchQuery = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.FulltextIndexScan(out result, indexId, searchQuery);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CalculateShortestPath
        ///</summary>
        [TestMethod()]
        public void CalculateShortestPathIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IRead target = CreateIRead(); // TODO: Initialize to an appropriate value
            List<Path> result = null; // TODO: Initialize to an appropriate value
            List<Path> resultExpected = null; // TODO: Initialize to an appropriate value
            string algorithmname = string.Empty; // TODO: Initialize to an appropriate value
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
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.CalculateShortestPath(out result, algorithmname, sourceVertexId, destinationVertexId, maxDepth, maxPathWeight, maxResults, edgePropertyFilter, vertexFilter, edgeFilter, edgeCost, vertexCost);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }
    }
}
