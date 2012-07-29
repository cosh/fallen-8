#region Usings

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Algorithms.Path;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for PathUnitTest and is intended
    ///to contain all PathUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PathUnitTest
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
        ///A test for Path Constructor
        ///</summary>
        [TestMethod()]
        public void PathConstructorUnitTest()
        {
            Assert.Inconclusive("TODO");

            int maximumLength = 0; // TODO: Initialize to an appropriate value
            var target = new Path(maximumLength);
        }

        /// <summary>
        ///A test for Path Constructor
        ///</summary>
        [TestMethod()]
        public void PathConstructorUnitTest1()
        {
            Assert.Inconclusive("TODO");

            Path anotherPath = null; // TODO: Initialize to an appropriate value
            PathElement lastElement = null; // TODO: Initialize to an appropriate value
            var target = new Path(anotherPath, lastElement);
        }

        /// <summary>
        ///A test for Path Constructor
        ///</summary>
        [TestMethod()]
        public void PathConstructorUnitTest2()
        {
            Assert.Inconclusive("TODO");

            PathElement pathElement = null; // TODO: Initialize to an appropriate value
            var target = new Path(pathElement);
        }

        /// <summary>
        ///A test for Path Constructor
        ///</summary>
        [TestMethod()]
        public void PathConstructorUnitTest3()
        {
            Assert.Inconclusive("TODO");

            PathElement firstPathElement = null; // TODO: Initialize to an appropriate value
            PathElement secondPathElement = null; // TODO: Initialize to an appropriate value
            var target = new Path(firstPathElement, secondPathElement);
        }

        /// <summary>
        ///A test for AddPathElement
        ///</summary>
        [TestMethod()]
        public void AddPathElementUnitTest()
        {
            Assert.Inconclusive("TODO");

            PathElement pathElement = null; // TODO: Initialize to an appropriate value
            var target = new Path(pathElement); // TODO: Initialize to an appropriate value
            PathElement pathElement1 = null; // TODO: Initialize to an appropriate value
            target.AddPathElement(pathElement1);
        }

        /// <summary>
        ///A test for CalculateWeight
        ///</summary>
        [TestMethod()]
        public void CalculateWeightUnitTest()
        {
            Assert.Inconclusive("TODO");

            PathElement pathElement = null; // TODO: Initialize to an appropriate value
            var target = new Path(pathElement); // TODO: Initialize to an appropriate value
            PathDelegates.VertexCost vertexCost = null; // TODO: Initialize to an appropriate value
            PathDelegates.EdgeCost edgeCost = null; // TODO: Initialize to an appropriate value
            target.CalculateWeight(vertexCost, edgeCost);
        }

        /// <summary>
        ///A test for GetLastVertex
        ///</summary>
        [TestMethod()]
        public void GetLastVertexUnitTest()
        {
            Assert.Inconclusive("TODO");


            PathElement pathElement = null; // TODO: Initialize to an appropriate value
            var target = new Path(pathElement); // TODO: Initialize to an appropriate value
            VertexModel expected = null; // TODO: Initialize to an appropriate value
            VertexModel actual;
            actual = target.GetLastVertex();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetLength
        ///</summary>
        [TestMethod()]
        public void GetLengthUnitTest()
        {
            Assert.Inconclusive("TODO");

            PathElement pathElement = null; // TODO: Initialize to an appropriate value
            var target = new Path(pathElement); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.GetLength();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetPathElements
        ///</summary>
        [TestMethod()]
        public void GetPathElementsUnitTest()
        {
            Assert.Inconclusive("TODO");

            PathElement pathElement = null; // TODO: Initialize to an appropriate value
            var target = new Path(pathElement); // TODO: Initialize to an appropriate value
            List<PathElement> expected = null; // TODO: Initialize to an appropriate value
            List<PathElement> actual;
            actual = target.GetPathElements();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ReversePath
        ///</summary>
        [TestMethod()]
        public void ReversePathUnitTest()
        {
            Assert.Inconclusive("TODO");

            PathElement pathElement = null; // TODO: Initialize to an appropriate value
            var target = new Path(pathElement); // TODO: Initialize to an appropriate value
            target.ReversePath();
        }
    }
}
