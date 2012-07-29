#region Usings

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Index.Spatial;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for IMetricUnitTest and is intended
    ///to contain all IMetricUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IMetricUnitTest
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


        internal virtual IMetric CreateIMetric()
        {
            // TODO: Instantiate an appropriate concrete class.
            IMetric target = null;
            return target;
        }

        /// <summary>
        ///A test for Distance
        ///</summary>
        [TestMethod()]
        public void DistanceUnitTest()
        {
            Assert.Inconclusive("TODO");

            IMetric target = CreateIMetric(); // TODO: Initialize to an appropriate value
            IMBP myPoint1 = null; // TODO: Initialize to an appropriate value
            IMBP myPoint2 = null; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = target.Distance(myPoint1, myPoint2);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TransformationOfDistance
        ///</summary>
        [TestMethod()]
        public void TransformationOfDistanceUnitTest()
        {
            Assert.Inconclusive("TODO");

            IMetric target = CreateIMetric(); // TODO: Initialize to an appropriate value
            float distance = 0F; // TODO: Initialize to an appropriate value
            IMBR mbr = null; // TODO: Initialize to an appropriate value
            float[] expected = null; // TODO: Initialize to an appropriate value
            float[] actual;
            actual = target.TransformationOfDistance(distance, mbr);
            Assert.AreEqual(expected, actual);
        }
    }
}
