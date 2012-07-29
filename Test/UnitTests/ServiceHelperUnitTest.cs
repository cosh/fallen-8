#region Usings

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Model;
using NoSQL.GraphDB.Service.REST;
using NoSQL.GraphDB.Service.REST.Specification;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    
    /// <summary>
    ///This is a test class for ServiceHelperUnitTest and is intended
    ///to contain all ServiceHelperUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ServiceHelperUnitTest
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
        ///A test for GenerateProperties
        ///</summary>
        [TestMethod()]
        public void GeneratePropertiesUnitTest()
        {
            Assert.Inconclusive("TODO");

            Dictionary<ushort, PropertySpecification> propertySpecification = null; // TODO: Initialize to an appropriate value
            PropertyContainer[] expected = null; // TODO: Initialize to an appropriate value
            PropertyContainer[] actual;
            actual = ServiceHelper.GenerateProperties(propertySpecification);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Transform
        ///</summary>
        [TestMethod()]
        public void TransformUnitTest()
        {
            Assert.Inconclusive("TODO");

            PropertySpecification definition = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ServiceHelper.Transform(definition);
            Assert.AreEqual(expected, actual);
        }
    }
}
