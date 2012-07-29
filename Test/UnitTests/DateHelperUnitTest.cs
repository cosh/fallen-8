#region Usings

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Helper;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for DateHelperUnitTest and is intended
    ///to contain all DateHelperUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DateHelperUnitTest
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
        ///A test for ConvertDateTime
        ///</summary>
        [TestMethod()]
        public void ConvertDateTimeUnitTest()
        {
            Assert.Inconclusive("TODO");

            var date = new DateTime(); // TODO: Initialize to an appropriate value
            uint expected = 0; // TODO: Initialize to an appropriate value
            uint actual;
            actual = DateHelper.ConvertDateTime(date);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetDateTimeFromUnixTimeStamp
        ///</summary>
        [TestMethod()]
        public void GetDateTimeFromUnixTimeStampUnitTest()
        {
            Assert.Inconclusive("TODO");

            uint secondsFromNineTeenSeventy = 0; // TODO: Initialize to an appropriate value
            var expected = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime actual;
            actual = DateHelper.GetDateTimeFromUnixTimeStamp(secondsFromNineTeenSeventy);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetModificationDate
        ///</summary>
        [TestMethod()]
        public void GetModificationDateUnitTest()
        {
            Assert.Inconclusive("TODO");

            uint creationDate = 0; // TODO: Initialize to an appropriate value
            uint expected = 0; // TODO: Initialize to an appropriate value
            uint actual;
            actual = DateHelper.GetModificationDate(creationDate);
            Assert.AreEqual(expected, actual);
        }
    }
}
