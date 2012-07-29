#region Usings

using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Index.Spatial;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for ISpatialIndexUnitTest and is intended
    ///to contain all ISpatialIndexUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ISpatialIndexUnitTest
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


        internal virtual ISpatialIndex CreateISpatialIndex()
        {
            // TODO: Instantiate an appropriate concrete class.
            ISpatialIndex target = null;
            return target;
        }

        /// <summary>
        ///A test for Containment
        ///</summary>
        [TestMethod()]
        public void ContainmentUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            AGraphElement graphElement = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Containment(out result, graphElement);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Containment
        ///</summary>
        [TestMethod()]
        public void ContainmentUnitTest1()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            IGeometry geometry = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Containment(out result, geometry);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Distance
        ///</summary>
        [TestMethod()]
        public void DistanceUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            IGeometry geometry1 = null; // TODO: Initialize to an appropriate value
            IGeometry geometry2 = null; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = target.Distance(geometry1, geometry2);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Distance
        ///</summary>
        [TestMethod()]
        public void DistanceUnitTest1()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            AGraphElement graphElement1 = null; // TODO: Initialize to an appropriate value
            AGraphElement graphElement2 = null; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = target.Distance(graphElement1, graphElement2);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Enclosure
        ///</summary>
        [TestMethod()]
        public void EnclosureUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            AGraphElement graphElement = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Enclosure(out result, graphElement);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Enclosure
        ///</summary>
        [TestMethod()]
        public void EnclosureUnitTest1()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            IGeometry geometry = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Enclosure(out result, geometry);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetAllNeighbors
        ///</summary>
        [TestMethod()]
        public void GetAllNeighborsUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            IGeometry geometry = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.GetAllNeighbors(out result, geometry);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetAllNeighbors
        ///</summary>
        [TestMethod()]
        public void GetAllNeighborsUnitTest1()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            AGraphElement graphElement = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.GetAllNeighbors(out result, graphElement);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetNextNeighbors
        ///</summary>
        [TestMethod()]
        public void GetNextNeighborsUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            IGeometry geometry = null; // TODO: Initialize to an appropriate value
            int countOfNextNeighbors = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.GetNextNeighbors(out result, geometry, countOfNextNeighbors);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetNextNeighbors
        ///</summary>
        [TestMethod()]
        public void GetNextNeighborsUnitTest1()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            AGraphElement graphElement = null; // TODO: Initialize to an appropriate value
            int countOfNextNeighbors = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.GetNextNeighbors(out result, graphElement, countOfNextNeighbors);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Overlap
        ///</summary>
        [TestMethod()]
        public void OverlapUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            AGraphElement graphElement = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Overlap(out result, graphElement);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Overlap
        ///</summary>
        [TestMethod()]
        public void OverlapUnitTest1()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            IGeometry geometry = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Overlap(out result, geometry);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SearchDistance
        ///</summary>
        [TestMethod()]
        public void SearchDistanceUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            float distance = 0F; // TODO: Initialize to an appropriate value
            IGeometry geometry = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.SearchDistance(out result, distance, geometry);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SearchDistance
        ///</summary>
        [TestMethod()]
        public void SearchDistanceUnitTest1()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            float distance = 0F; // TODO: Initialize to an appropriate value
            AGraphElement graphElement = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.SearchDistance(out result, distance, graphElement);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SearchPoint
        ///</summary>
        [TestMethod()]
        public void SearchPointUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            IPoint point = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.SearchPoint(out result, point);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SearchRegion
        ///</summary>
        [TestMethod()]
        public void SearchRegionUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialIndex target = CreateISpatialIndex(); // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> result = null; // TODO: Initialize to an appropriate value
            ReadOnlyCollection<AGraphElement> resultExpected = null; // TODO: Initialize to an appropriate value
            IMBR minimalBoundedRechtangle = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.SearchRegion(out result, minimalBoundedRechtangle);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }
    }
}
