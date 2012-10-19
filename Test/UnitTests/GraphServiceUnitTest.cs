#region Usings

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Service.REST;
using NoSQL.GraphDB.Service.REST.Result;
using NoSQL.GraphDB.Service.REST.Specification;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for GraphServiceUnitTest and is intended
    ///to contain all GraphServiceUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GraphServiceUnitTest
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
        ///A test for GraphService Constructor
        ///</summary>
        [TestMethod()]
        public void GraphServiceConstructorUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8);
        }

        /// <summary>
        ///A test for AddEdge
        ///</summary>
        [TestMethod()]
        public void AddEdgeUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            EdgeSpecification definition = null; // TODO: Initialize to an appropriate value
            Int64 expected = 0; // TODO: Initialize to an appropriate value
            Int64 actual;
            actual = target.AddEdge(definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for AddToIndex
        ///</summary>
        [TestMethod()]
        public void AddToIndexUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            IndexAddToSpecification definition = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.AddToIndex(definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for AddVertex
        ///</summary>
        [TestMethod()]
        public void AddVertexUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            VertexSpecification definition = null; // TODO: Initialize to an appropriate value
            Int64 expected = 0; // TODO: Initialize to an appropriate value
            Int64 actual;
            actual = target.AddVertex(definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreateIndex
        ///</summary>
        [TestMethod()]
        public void CreateIndexUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            PluginSpecification definition = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.CreateIndex(definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DeleteIndex
        ///</summary>
        [TestMethod()]
        public void DeleteIndexUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            IndexDeleteSpecificaton definition = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.DeleteIndex(definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FulltextIndexScan
        ///</summary>
        [TestMethod()]
        public void FulltextIndexScanUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            FulltextIndexScanSpecification definition = null; // TODO: Initialize to an appropriate value
            FulltextSearchResultREST expected = null; // TODO: Initialize to an appropriate value
            FulltextSearchResultREST actual;
            actual = target.FulltextIndexScan(definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetAllAvailableIncEdgesOnVertex
        ///</summary>
        [TestMethod()]
        public void GetAllAvailableIncEdgesOnVertexUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string vertexIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            List<ushort> expected = null; // TODO: Initialize to an appropriate value
            List<ushort> actual;
            actual = target.GetAllAvailableIncEdgesOnVertex(vertexIdentifier);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetAllAvailableOutEdgesOnVertex
        ///</summary>
        [TestMethod()]
        public void GetAllAvailableOutEdgesOnVertexUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string vertexIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            List<ushort> expected = null; // TODO: Initialize to an appropriate value
            List<ushort> actual;
            actual = target.GetAllAvailableOutEdgesOnVertex(vertexIdentifier);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetAllGraphelementProperties
        ///</summary>
        [TestMethod()]
        public void GetAllGraphelementPropertiesUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string graphElementIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            PropertiesREST expected = null; // TODO: Initialize to an appropriate value
            PropertiesREST actual;
            actual = target.GetAllGraphelementProperties(graphElementIdentifier);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetInDegree
        ///</summary>
        [TestMethod()]
        public void GetInDegreeUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string vertexIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            uint expected = 0; // TODO: Initialize to an appropriate value
            uint actual;
            actual = target.GetInDegree(vertexIdentifier);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetInEdgeDegree
        ///</summary>
        [TestMethod()]
        public void GetInEdgeDegreeUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string vertexIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            string edgePropertyIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            uint expected = 0; // TODO: Initialize to an appropriate value
            uint actual;
            actual = target.GetInEdgeDegree(vertexIdentifier, edgePropertyIdentifier);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetIncomingEdges
        ///</summary>
        [TestMethod()]
        public void GetIncomingEdgesUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string vertexIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            string edgePropertyIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            List<Int64> expected = null; // TODO: Initialize to an appropriate value
            List<Int64> actual;
            actual = target.GetIncomingEdges(vertexIdentifier, edgePropertyIdentifier);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetOutDegree
        ///</summary>
        [TestMethod()]
        public void GetOutDegreeUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string vertexIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            uint expected = 0; // TODO: Initialize to an appropriate value
            uint actual;
            actual = target.GetOutDegree(vertexIdentifier);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetOutEdgeDegree
        ///</summary>
        [TestMethod()]
        public void GetOutEdgeDegreeUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string vertexIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            string edgePropertyIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            uint expected = 0; // TODO: Initialize to an appropriate value
            uint actual;
            actual = target.GetOutEdgeDegree(vertexIdentifier, edgePropertyIdentifier);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetOutgoingEdges
        ///</summary>
        [TestMethod()]
        public void GetOutgoingEdgesUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string vertexIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            string edgePropertyIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            List<Int64> expected = null; // TODO: Initialize to an appropriate value
            List<Int64> actual;
            actual = target.GetOutgoingEdges(vertexIdentifier, edgePropertyIdentifier);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetPaths
        ///</summary>
        [TestMethod()]
        public void GetPathsUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string from = string.Empty; // TODO: Initialize to an appropriate value
            string to = string.Empty; // TODO: Initialize to an appropriate value
            PathSpecification definition = null; // TODO: Initialize to an appropriate value
            List<PathREST> expected = null; // TODO: Initialize to an appropriate value
            List<PathREST> actual;
            actual = target.GetPaths(from, to, definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetPathsByVertex
        ///</summary>
        [TestMethod()]
        public void GetPathsByVertexUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string from = string.Empty; // TODO: Initialize to an appropriate value
            string to = string.Empty; // TODO: Initialize to an appropriate value
            PathSpecification definition = null; // TODO: Initialize to an appropriate value
            List<PathREST> expected = null; // TODO: Initialize to an appropriate value
            List<PathREST> actual;
            actual = target.GetPathsByVertex(from, to, definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetSourceVertexForEdge
        ///</summary>
        [TestMethod()]
        public void GetSourceVertexForEdgeUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string edgeIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            Int64 expected = 0; // TODO: Initialize to an appropriate value
            Int64 actual;
            actual = target.GetSourceVertexForEdge(edgeIdentifier);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetTargetVertexForEdge
        ///</summary>
        [TestMethod()]
        public void GetTargetVertexForEdgeUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string edgeIdentifier = string.Empty; // TODO: Initialize to an appropriate value
            Int64 expected = 0; // TODO: Initialize to an appropriate value
            Int64 actual;
            actual = target.GetTargetVertexForEdge(edgeIdentifier);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GraphScan
        ///</summary>
        [TestMethod()]
        public void GraphScanUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string propertyIdString = string.Empty; // TODO: Initialize to an appropriate value
            ScanSpecification definition = null; // TODO: Initialize to an appropriate value
            IEnumerable<Int64> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<Int64> actual;
            actual = target.GraphScan(propertyIdString, definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IndexScan
        ///</summary>
        [TestMethod()]
        public void IndexScanUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            IndexScanSpecification definition = null; // TODO: Initialize to an appropriate value
            IEnumerable<Int64> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<Int64> actual;
            actual = target.IndexScan(definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for RangeIndexScan
        ///</summary>
        [TestMethod()]
        public void RangeIndexScanUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            RangeIndexScanSpecification definition = null; // TODO: Initialize to an appropriate value
            IEnumerable<Int64> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<Int64> actual;
            actual = target.RangeIndexScan(definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for RemoveGraphElementFromIndex
        ///</summary>
        [TestMethod()]
        public void RemoveGraphElementFromIndexUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            IndexRemoveGraphelementFromIndexSpecification definition = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.RemoveGraphElementFromIndex(definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for RemoveKeyFromIndex
        ///</summary>
        [TestMethod()]
        public void RemoveKeyFromIndexUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            IndexRemoveKeyFromIndexSpecification definition = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.RemoveKeyFromIndex(definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SpatialIndexScanSearchDistance
        ///</summary>
        [TestMethod()]
        public void SpatialIndexScanSearchDistanceUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            SearchDistanceSpecification definition = null; // TODO: Initialize to an appropriate value
            IEnumerable<Int64> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<Int64> actual;
            actual = target.SpatialIndexScanSearchDistance(definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryAddProperty
        ///</summary>
        [TestMethod()]
        public void TryAddPropertyUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string graphElementIdString = string.Empty; // TODO: Initialize to an appropriate value
            string propertyIdString = string.Empty; // TODO: Initialize to an appropriate value
            PropertySpecification definition = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryAddProperty(graphElementIdString, propertyIdString, definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryRemoveGraphElement
        ///</summary>
        [TestMethod()]
        public void TryRemoveGraphElementUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string graphElementIdString = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryRemoveGraphElement(graphElementIdString);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryRemoveProperty
        ///</summary>
        [TestMethod()]
        public void TryRemovePropertyUnitTest()
        {
            Assert.Inconclusive("TODO");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new GraphService(fallen8); // TODO: Initialize to an appropriate value
            string graphElementIdString = string.Empty; // TODO: Initialize to an appropriate value
            string propertyIdString = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryRemoveProperty(graphElementIdString, propertyIdString);
            Assert.AreEqual(expected, actual);
        }
    }
}
