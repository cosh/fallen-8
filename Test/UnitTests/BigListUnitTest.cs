#region Usings

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Helper;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for BigListUnitTest and is intended
    ///to contain all BigListUnitTest Unit Tests
    ///</summary>
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


        /// <summary>
        ///A test for BigList`1 Constructor
        ///</summary>
        public void BigListConstructorUnitTestHelper<T>()
            where T : class
        {
            var target = new BigList<T>();
        }

        [TestMethod()]
        public void BigListConstructorUnitTest()
        {
            Assert.Inconclusive("TODO");

            BigListConstructorUnitTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for Clear
        ///</summary>
        public void ClearUnitTestHelper<T>()
            where T : class
        {
            var target = new BigList<T>(); // TODO: Initialize to an appropriate value
            target.Clear();
        }

        [TestMethod()]
        public void ClearUnitTest()
        {
            Assert.Inconclusive("TODO");

            ClearUnitTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for FindElements
        ///</summary>
        public void FindElementsUnitTestHelper<T>()
            where T : class
        {
            var target = new BigList<T>(); // TODO: Initialize to an appropriate value
            BigList<T>.ElementSeeker delgate = null; // TODO: Initialize to an appropriate value
            List<T> expected = null; // TODO: Initialize to an appropriate value
            List<T> actual;
            actual = target.FindElements(delgate);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void FindElementsUnitTest()
        {
            Assert.Inconclusive("TODO");

            FindElementsUnitTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for GetAllOfType
        ///</summary>
        public void GetAllOfTypeUnitTestHelper<T, TResult>()
            where T : class
            where TResult : class
        {
            var target = new BigList<T>(); // TODO: Initialize to an appropriate value
            List<TResult> expected = null; // TODO: Initialize to an appropriate value
            List<TResult> actual;
            actual = target.GetAllOfType<TResult>();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetAllOfTypeUnitTest()
        {
            Assert.Inconclusive("TODO");

            GetAllOfTypeUnitTestHelper<GenericParameterHelper, GenericParameterHelper>();
        }

        /// <summary>
        ///A test for GetCountOf
        ///</summary>
        public void GetCountOfUnitTestHelper<T, TInteresting>()
            where T : class
        {
            var target = new BigList<T>(); // TODO: Initialize to an appropriate value
            uint expected = 0; // TODO: Initialize to an appropriate value
            uint actual;
            actual = target.GetCountOf<TInteresting>();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetCountOfUnitTest()
        {
            Assert.Inconclusive("TODO");

            GetCountOfUnitTestHelper<GenericParameterHelper, GenericParameterHelper>();
        }

        /// <summary>
        ///A test for SetDefault
        ///</summary>
        public void SetDefaultUnitTestHelper<T>()
            where T : class
        {
            var target = new BigList<T>(); // TODO: Initialize to an appropriate value
            int index = 0; // TODO: Initialize to an appropriate value
            target.SetDefault(index);
        }

        [TestMethod()]
        public void SetDefaultUnitTest()
        {
            Assert.Inconclusive("TODO");

            SetDefaultUnitTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for SetValue
        ///</summary>
        public void SetValueUnitTestHelper<T>()
            where T : class
        {
            var target = new BigList<T>(); // TODO: Initialize to an appropriate value
            int index = 0; // TODO: Initialize to an appropriate value
            T item = null; // TODO: Initialize to an appropriate value
            target.SetValue(index, item);
        }

        [TestMethod()]
        public void SetValueUnitTest()
        {
            Assert.Inconclusive("TODO");

            SetValueUnitTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for TryGetElementOrDefault
        ///</summary>
        public void TryGetElementOrDefaultUnitTestHelper<T, TResult>()
            where T : class
            where TResult : class
        {
            var target = new BigList<T>(); // TODO: Initialize to an appropriate value
            TResult result = null; // TODO: Initialize to an appropriate value
            TResult resultExpected = null; // TODO: Initialize to an appropriate value
            int index = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryGetElementOrDefault<TResult>(out result, index);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TryGetElementOrDefaultUnitTest()
        {
            Assert.Inconclusive("TODO");

            TryGetElementOrDefaultUnitTestHelper<GenericParameterHelper, GenericParameterHelper>();
        }
    }
}
