#region Usings

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Service.REST;
using NoSQL.GraphDB.Service.REST.Result;
using NoSQL.GraphDB.Service.REST.Specification;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for IAdminServiceIntegrationTest and is intended
    ///to contain all IAdminServiceIntegrationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IAdminServiceIntegrationTest
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


        internal virtual IAdminService CreateIAdminService()
        {
            // TODO: Instantiate an appropriate concrete class.
            IAdminService target = null;
            return target;
        }

        /// <summary>
        ///A test for CreateService
        ///</summary>
        [TestMethod()]
        public void CreateServiceIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IAdminService target = CreateIAdminService(); // TODO: Initialize to an appropriate value
            PluginSpecification definition = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.CreateService(definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DeleteService
        ///</summary>
        [TestMethod()]
        public void DeleteServiceIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IAdminService target = CreateIAdminService(); // TODO: Initialize to an appropriate value
            ServiceDeleteSpecificaton definition = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.DeleteService(definition);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for EdgeCount
        ///</summary>
        [TestMethod()]
        public void EdgeCountIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IAdminService target = CreateIAdminService(); // TODO: Initialize to an appropriate value
            uint expected = 0; // TODO: Initialize to an appropriate value
            uint actual;
            actual = target.EdgeCount();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FreeMem
        ///</summary>
        [TestMethod()]
        public void FreeMemIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IAdminService target = CreateIAdminService(); // TODO: Initialize to an appropriate value
            ulong expected = 0; // TODO: Initialize to an appropriate value
            ulong actual;
            actual = target.FreeMem();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IAdminService target = CreateIAdminService(); // TODO: Initialize to an appropriate value
            string startServices = string.Empty; // TODO: Initialize to an appropriate value
            target.Load(startServices);
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IAdminService target = CreateIAdminService(); // TODO: Initialize to an appropriate value
            target.Save();
        }

        /// <summary>
        ///A test for Status
        ///</summary>
        [TestMethod()]
        public void StatusIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IAdminService target = CreateIAdminService(); // TODO: Initialize to an appropriate value
            StatusREST expected = null; // TODO: Initialize to an appropriate value
            StatusREST actual;
            actual = target.Status();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TabulaRasa
        ///</summary>
        [TestMethod()]
        public void TabulaRasaIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IAdminService target = CreateIAdminService(); // TODO: Initialize to an appropriate value
            target.TabulaRasa();
        }

        /// <summary>
        ///A test for Trim
        ///</summary>
        [TestMethod()]
        public void TrimIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IAdminService target = CreateIAdminService(); // TODO: Initialize to an appropriate value
            target.Trim();
        }

        /// <summary>
        ///A test for UploadPlugin
        ///</summary>
        [TestMethod()]
        public void UploadPluginIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IAdminService target = CreateIAdminService(); // TODO: Initialize to an appropriate value
            Stream dllStream = null; // TODO: Initialize to an appropriate value
            target.UploadPlugin(dllStream);
        }

        /// <summary>
        ///A test for VertexCount
        ///</summary>
        [TestMethod()]
        public void VertexCountIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IAdminService target = CreateIAdminService(); // TODO: Initialize to an appropriate value
            uint expected = 0; // TODO: Initialize to an appropriate value
            uint actual;
            actual = target.VertexCount();
            Assert.AreEqual(expected, actual);
        }
    }
}
