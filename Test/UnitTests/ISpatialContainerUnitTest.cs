#region Usings

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Index.Spatial;
using NoSQL.GraphDB.Index.Spatial.Implementation.SpatialContainer;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for ISpatialContainerUnitTest and is intended
    ///to contain all ISpatialContainerUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ISpatialContainerUnitTest
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


        internal virtual ISpatialContainer CreateISpatialContainer()
        {
            // TODO: Instantiate an appropriate concrete class.
            ISpatialContainer target = null;
            return target;
        }

        /// <summary>
        ///A test for Adjacency
        ///</summary>
        [TestMethod()]
        public void AdjacencyUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialContainer target = CreateISpatialContainer(); // TODO: Initialize to an appropriate value
            ISpatialContainer container = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Adjacency(container);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for EqualTo
        ///</summary>
        [TestMethod()]
        public void EqualToUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialContainer target = CreateISpatialContainer(); // TODO: Initialize to an appropriate value
            ISpatialContainer container = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.EqualTo(container);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Inclusion
        ///</summary>
        [TestMethod()]
        public void InclusionUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialContainer target = CreateISpatialContainer(); // TODO: Initialize to an appropriate value
            ISpatialContainer container = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Inclusion(container);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Intersection
        ///</summary>
        [TestMethod()]
        public void IntersectionUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialContainer target = CreateISpatialContainer(); // TODO: Initialize to an appropriate value
            ISpatialContainer container = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Intersection(container);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Container
        ///</summary>
        [TestMethod()]
        public void ContainerUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialContainer target = CreateISpatialContainer(); // TODO: Initialize to an appropriate value
            TypeOfContainer actual;
            actual = target.Container;
        }

        /// <summary>
        ///A test for LowerPoint
        ///</summary>
        [TestMethod()]
        public void LowerPointUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialContainer target = CreateISpatialContainer(); // TODO: Initialize to an appropriate value
            float[] actual;
            actual = target.LowerPoint;
        }

        /// <summary>
        ///A test for UpperPoint
        ///</summary>
        [TestMethod()]
        public void UpperPointUnitTest()
        {
            Assert.Inconclusive("TODO");

            ISpatialContainer target = CreateISpatialContainer(); // TODO: Initialize to an appropriate value
            float[] actual;
            actual = target.UpperPoint;
        }
    }
}
