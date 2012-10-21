#region Usings

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Helper;
using System.Threading;

#endregion

namespace NoSQL.GraphDB.Test
{

    [TestClass()]
    public class BigListUnitTest
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


        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BigList_SetValue_BadIndex_UnitTest()
        {
            var bigList = new BigList<Object>();

            bigList.SetValue(-1, new object());
        }

        [TestMethod()]
        public void BigList_SetValue_UnitTest()
        {
            var bigList = new BigList<Object>();

            var object0 = new object();
            bigList.SetValue(0, object0);
            var object0Actual = bigList.GetElement(0);
            Assert.IsTrue(ReferenceEquals(object0, object0Actual));

            var object10 = new object();
            bigList.SetValue(10, object10);
            var object10Actual = bigList.GetElement(10);
            Assert.IsTrue(ReferenceEquals(object10, object10Actual));

            var object250000000 = new object();
            bigList.SetValue(250000000, object250000000);
            var object250000000Actual = bigList.GetElement(250000000);
            Assert.IsTrue(ReferenceEquals(object250000000, object250000000Actual));
        }
    }
}
