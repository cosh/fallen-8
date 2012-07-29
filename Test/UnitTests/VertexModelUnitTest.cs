#region Usings

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for VertexModelUnitTest and is intended
    ///to contain all VertexModelUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VertexModelUnitTest
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
        ///A test for VertexModel Constructor
        ///</summary>
        [TestMethod()]
        public void VertexModelConstructorUnitTest()
        {
            Assert.Inconclusive("TODO");

            int id = 0; // TODO: Initialize to an appropriate value
            uint creationDate = 0; // TODO: Initialize to an appropriate value
            PropertyContainer[] properties = null; // TODO: Initialize to an appropriate value
            var target = new VertexModel(id, creationDate, properties);
        }

        /// <summary>
        ///A test for GetAllNeighbors
        ///</summary>
        [TestMethod()]
        public void GetAllNeighborsUnitTest()
        {
            Assert.Inconclusive("TODO");

            int id = 0; // TODO: Initialize to an appropriate value
            uint creationDate = 0; // TODO: Initialize to an appropriate value
            PropertyContainer[] properties = null; // TODO: Initialize to an appropriate value
            var target = new VertexModel(id, creationDate, properties); // TODO: Initialize to an appropriate value
            List<VertexModel> expected = null; // TODO: Initialize to an appropriate value
            List<VertexModel> actual;
            actual = target.GetAllNeighbors();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetHashCode
        ///</summary>
        [TestMethod()]
        public void GetHashCodeUnitTest()
        {
            Assert.Inconclusive("TODO");

            int id = 0; // TODO: Initialize to an appropriate value
            uint creationDate = 0; // TODO: Initialize to an appropriate value
            PropertyContainer[] properties = null; // TODO: Initialize to an appropriate value
            var target = new VertexModel(id, creationDate, properties); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.GetHashCode();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetInDegree
        ///</summary>
        [TestMethod()]
        public void GetInDegreeUnitTest()
        {
            Assert.Inconclusive("TODO");

            int id = 0; // TODO: Initialize to an appropriate value
            uint creationDate = 0; // TODO: Initialize to an appropriate value
            PropertyContainer[] properties = null; // TODO: Initialize to an appropriate value
            var target = new VertexModel(id, creationDate, properties); // TODO: Initialize to an appropriate value
            uint expected = 0; // TODO: Initialize to an appropriate value
            uint actual;
            actual = target.GetInDegree();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetIncomingEdgeIds
        ///</summary>
        [TestMethod()]
        public void GetIncomingEdgeIdsUnitTest()
        {
            Assert.Inconclusive("TODO");

            int id = 0; // TODO: Initialize to an appropriate value
            uint creationDate = 0; // TODO: Initialize to an appropriate value
            PropertyContainer[] properties = null; // TODO: Initialize to an appropriate value
            var target = new VertexModel(id, creationDate, properties); // TODO: Initialize to an appropriate value
            List<ushort> expected = null; // TODO: Initialize to an appropriate value
            List<ushort> actual;
            actual = target.GetIncomingEdgeIds();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetOutDegree
        ///</summary>
        [TestMethod()]
        public void GetOutDegreeUnitTest()
        {
            Assert.Inconclusive("TODO");

            int id = 0; // TODO: Initialize to an appropriate value
            uint creationDate = 0; // TODO: Initialize to an appropriate value
            PropertyContainer[] properties = null; // TODO: Initialize to an appropriate value
            var target = new VertexModel(id, creationDate, properties); // TODO: Initialize to an appropriate value
            uint expected = 0; // TODO: Initialize to an appropriate value
            uint actual;
            actual = target.GetOutDegree();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetOutgoingEdgeIds
        ///</summary>
        [TestMethod()]
        public void GetOutgoingEdgeIdsUnitTest()
        {
            Assert.Inconclusive("TODO");

            int id = 0; // TODO: Initialize to an appropriate value
            uint creationDate = 0; // TODO: Initialize to an appropriate value
            PropertyContainer[] properties = null; // TODO: Initialize to an appropriate value
            var target = new VertexModel(id, creationDate, properties); // TODO: Initialize to an appropriate value
            List<ushort> expected = null; // TODO: Initialize to an appropriate value
            List<ushort> actual;
            actual = target.GetOutgoingEdgeIds();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryGetInEdge
        ///</summary>
        [TestMethod()]
        public void TryGetInEdgeUnitTest()
        {
            Assert.Inconclusive("TODO");

            int id = 0; // TODO: Initialize to an appropriate value
            uint creationDate = 0; // TODO: Initialize to an appropriate value
            PropertyContainer[] properties = null; // TODO: Initialize to an appropriate value
            var target = new VertexModel(id, creationDate, properties); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<EdgeModel> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<EdgeModel> resultExpected = null; // TODO: Initialize to an appropriate value
            ushort edgePropertyId = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryGetInEdge(out result, edgePropertyId);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryGetOutEdge
        ///</summary>
        [TestMethod()]
        public void TryGetOutEdgeUnitTest()
        {
            Assert.Inconclusive("TODO");

            int id = 0; // TODO: Initialize to an appropriate value
            uint creationDate = 0; // TODO: Initialize to an appropriate value
            PropertyContainer[] properties = null; // TODO: Initialize to an appropriate value
            var target = new VertexModel(id, creationDate, properties); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<EdgeModel> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<EdgeModel> resultExpected = null; // TODO: Initialize to an appropriate value
            ushort edgePropertyId = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryGetOutEdge(out result, edgePropertyId);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        [TestMethod()]
        public void op_EqualityUnitTest()
        {
            Assert.Inconclusive("TODO");

            VertexModel a = null; // TODO: Initialize to an appropriate value
            VertexModel b = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = (a == b);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod()]
        public void op_InequalityUnitTest()
        {
            Assert.Inconclusive("TODO");

            VertexModel a = null; // TODO: Initialize to an appropriate value
            VertexModel b = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = (a != b);
            Assert.AreEqual(expected, actual);
        }
    }
}
