#region Usings

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Algorithms.Path;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for PathHelperUnitTest and is intended
    ///to contain all PathHelperUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PathHelperUnitTest
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
        ///A test for GetValidEdges
        ///</summary>
        [TestMethod()]
        public void GetValidEdgesUnitTest()
        {
            Assert.Inconclusive("TODO");

            VertexModel vertex = null; // TODO: Initialize to an appropriate value
            var direction = new Direction(); // TODO: Initialize to an appropriate value
            PathDelegates.EdgePropertyFilter edgepropertyFilter = null; // TODO: Initialize to an appropriate value
            PathDelegates.EdgeFilter edgeFilter = null; // TODO: Initialize to an appropriate value
            PathDelegates.VertexFilter vertexFilter = null; // TODO: Initialize to an appropriate value
            List<Tuple<ushort, IEnumerable<EdgeModel>>> expected = null; // TODO: Initialize to an appropriate value
            List<Tuple<ushort, IEnumerable<EdgeModel>>> actual;
            actual = PathHelper.GetValidEdges(vertex, direction, edgepropertyFilter, edgeFilter, vertexFilter);
            Assert.AreEqual(expected, actual);
        }
    }
}
