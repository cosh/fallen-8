#region Usings

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Helper;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for StatisticsUnitTest and is intended
    ///to contain all StatisticsUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StatisticsUnitTest
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
        ///A test for Average
        ///</summary>
        [TestMethod()]
        public void AverageUnitTest()
        {
            Assert.Inconclusive("TODO");

            List<double> numbers = null; // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            actual = Statistics.Average(numbers);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Median
        ///</summary>
        [TestMethod()]
        public void MedianUnitTest()
        {
            Assert.Inconclusive("TODO");

            List<double> numbers = null; // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            actual = Statistics.Median(numbers);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for StandardDeviation
        ///</summary>
        [TestMethod()]
        public void StandardDeviationUnitTest()
        {
            Assert.Inconclusive("TODO");

            List<double> numbers = null; // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            actual = Statistics.StandardDeviation(numbers);
            Assert.AreEqual(expected, actual);
        }
    }
}
