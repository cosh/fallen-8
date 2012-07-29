#region Usings

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for IWriteIntegrationTest and is intended
    ///to contain all IWriteIntegrationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IWriteIntegrationTest
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


        internal virtual IWrite CreateIWrite()
        {
            // TODO: Instantiate an appropriate concrete class.
            IWrite target = null;
            return target;
        }

        /// <summary>
        ///A test for TryRemoveProperty
        ///</summary>
        [TestMethod()]
        public void TryRemovePropertyIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IWrite target = CreateIWrite(); // TODO: Initialize to an appropriate value
            int graphElementId = 0; // TODO: Initialize to an appropriate value
            ushort propertyId = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryRemoveProperty(graphElementId, propertyId);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryRemoveGraphElement
        ///</summary>
        [TestMethod()]
        public void TryRemoveGraphElementIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IWrite target = CreateIWrite(); // TODO: Initialize to an appropriate value
            int graphElementId = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryRemoveGraphElement(graphElementId);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryAddProperty
        ///</summary>
        [TestMethod()]
        public void TryAddPropertyIntegrationTest()
        {
            Assert.Inconclusive("TODO.");
            
            IWrite target = CreateIWrite(); // TODO: Initialize to an appropriate value
            int graphElementId = 0; // TODO: Initialize to an appropriate value
            ushort propertyId = 0; // TODO: Initialize to an appropriate value
            object property = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryAddProperty(graphElementId, propertyId, property);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Trim
        ///</summary>
        [TestMethod()]
        public void TrimIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IWrite target = CreateIWrite(); // TODO: Initialize to an appropriate value
            target.Trim();
        }

        /// <summary>
        ///A test for TabulaRasa
        ///</summary>
        [TestMethod()]
        public void TabulaRasaIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IWrite target = CreateIWrite(); // TODO: Initialize to an appropriate value
            target.TabulaRasa();
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IWrite target = CreateIWrite(); // TODO: Initialize to an appropriate value
            string path = string.Empty; // TODO: Initialize to an appropriate value
            bool startServices = false; // TODO: Initialize to an appropriate value
            target.Load(path, startServices);
        }

        /// <summary>
        ///A test for CreateVertex
        ///</summary>
        [TestMethod()]
        public void CreateVertexIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IWrite target = CreateIWrite(); // TODO: Initialize to an appropriate value
            uint creationDate = 0; // TODO: Initialize to an appropriate value
            PropertyContainer[] properties = null; // TODO: Initialize to an appropriate value
            VertexModel expected = null; // TODO: Initialize to an appropriate value
            VertexModel actual;
            actual = target.CreateVertex(creationDate, properties);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreateEdge
        ///</summary>
        [TestMethod()]
        public void CreateEdgeIntegrationTest()
        {
            Assert.Inconclusive("TODO.");

            IWrite target = CreateIWrite(); // TODO: Initialize to an appropriate value
            int sourceVertexId = 0; // TODO: Initialize to an appropriate value
            ushort edgePropertyId = 0; // TODO: Initialize to an appropriate value
            int targetVertexId = 0; // TODO: Initialize to an appropriate value
            uint creationDate = 0; // TODO: Initialize to an appropriate value
            PropertyContainer[] properties = null; // TODO: Initialize to an appropriate value
            EdgeModel expected = null; // TODO: Initialize to an appropriate value
            EdgeModel actual;
            actual = target.CreateEdge(sourceVertexId, edgePropertyId, targetVertexId, creationDate, properties);
            Assert.AreEqual(expected, actual);
        }
    }
}
