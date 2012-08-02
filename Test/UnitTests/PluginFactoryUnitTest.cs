#region Usings

using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Plugin;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for PluginFactoryUnitTest and is intended
    ///to contain all PluginFactoryUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PluginFactoryUnitTest
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
        ///A test for Assimilate
        ///</summary>
        [TestMethod()]
        public void AssimilateUnitTest()
        {
            Assert.Inconclusive("TODO");

            Stream dllStream = null; // TODO: Initialize to an appropriate value
            string path = string.Empty; // TODO: Initialize to an appropriate value
            PluginFactory.Assimilate(dllStream, path);
        }

        /// <summary>
        ///A test for TryFindPlugin
        ///</summary>
        public void TryFindPluginUnitTestHelper<T>()
        {
            Assert.Inconclusive("TODO");
        }

        [TestMethod()]
        public void TryFindPluginUnitTest()
        {
            Assert.Inconclusive("TODO");

            TryFindPluginUnitTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for TryGetAvailablePlugins
        ///</summary>
        public void TryGetAvailablePluginsUnitTestHelper<T>()
        {
            Assert.Inconclusive("TODO");

            IEnumerable<string> result = null; // TODO: Initialize to an appropriate value
            IEnumerable<string> resultExpected = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = PluginFactory.TryGetAvailablePlugins<T>(out result);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TryGetAvailablePluginsUnitTest()
        {
            Assert.Inconclusive("TODO");

            TryGetAvailablePluginsUnitTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for TryGetAvailablePluginsWithDescriptions
        ///</summary>
        public void TryGetAvailablePluginsWithDescriptionsUnitTestHelper<T>()
        {
            Dictionary<string, string> result = null; // TODO: Initialize to an appropriate value
            Dictionary<string, string> resultExpected = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = PluginFactory.TryGetAvailablePluginsWithDescriptions<T>(out result);
            Assert.AreEqual(resultExpected, result);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TryGetAvailablePluginsWithDescriptionsUnitTest()
        {
            Assert.Inconclusive("TODO");

            TryGetAvailablePluginsWithDescriptionsUnitTestHelper<GenericParameterHelper>();
        }
    }
}
