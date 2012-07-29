#region Usings

using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Service;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for ServiceFactoryIntegrationTest and is intended
    ///to contain all ServiceFactoryIntegrationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ServiceFactoryIntegrationTest
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
        ///A test for ServiceFactory Constructor
        ///</summary>
        [TestMethod()]
        public void ServiceFactoryConstructorIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new ServiceFactory(fallen8);
        }

        /// <summary>
        ///A test for GetAvailableServicePlugins
        ///</summary>
        [TestMethod()]
        public void GetAvailableServicePluginsIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new ServiceFactory(fallen8); // TODO: Initialize to an appropriate value
            IEnumerable<string> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<string> actual;
            actual = target.GetAvailableServicePlugins();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ShutdownAllServices
        ///</summary>
        [TestMethod()]
        public void ShutdownAllServicesIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new ServiceFactory(fallen8); // TODO: Initialize to an appropriate value
            target.ShutdownAllServices();
        }

        /// <summary>
        ///A test for StartAdminService
        ///</summary>
        [TestMethod()]
        public void StartAdminServiceIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new ServiceFactory(fallen8); // TODO: Initialize to an appropriate value
            IPAddress iPAddress = null; // TODO: Initialize to an appropriate value
            ushort port = 0; // TODO: Initialize to an appropriate value
            target.StartAdminService(iPAddress, port);
        }

        /// <summary>
        ///A test for StartAllServices
        ///</summary>
        [TestMethod()]
        public void StartAllServicesIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new ServiceFactory(fallen8); // TODO: Initialize to an appropriate value
            target.StartAllServices();
        }

        /// <summary>
        ///A test for StartGraphService
        ///</summary>
        [TestMethod()]
        public void StartGraphServiceIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new ServiceFactory(fallen8); // TODO: Initialize to an appropriate value
            IPAddress iPAddress = null; // TODO: Initialize to an appropriate value
            ushort port = 0; // TODO: Initialize to an appropriate value
            target.StartGraphService(iPAddress, port);
        }

        /// <summary>
        ///A test for TryAddService
        ///</summary>
        [TestMethod()]
        public void TryAddServiceIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            Fallen8 fallen8 = null; // TODO: Initialize to an appropriate value
            var target = new ServiceFactory(fallen8); // TODO: Initialize to an appropriate value
            IService service = null; // TODO: Initialize to an appropriate value
            IService serviceExpected = null; // TODO: Initialize to an appropriate value
            string servicePluginName = string.Empty; // TODO: Initialize to an appropriate value
            string serviceName = string.Empty; // TODO: Initialize to an appropriate value
            IDictionary<string, object> parameter = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryAddService(out service, servicePluginName, serviceName, parameter);
            Assert.AreEqual(serviceExpected, service);
            Assert.AreEqual(expected, actual);
        }
    }
}
