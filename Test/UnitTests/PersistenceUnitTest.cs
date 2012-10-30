#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoSQL.GraphDB.Index;
using NoSQL.GraphDB.Model;
using System.Threading;

#endregion

namespace NoSQL.GraphDB.Test
{
    
    [TestClass()]
    public class PersistenceUnitTest
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
        public void Persistence_Basic_UnitTest()
        {
            var saveGamePath = System.IO.Path.Combine(Environment.CurrentDirectory, "test.fs8");

            TestHelper.ExecuteTestWithPersistence(
                () => new Fallen8(),
                element =>
                    {
                        Assert.IsTrue(element != null);
                        Assert.IsTrue(element.EdgeCount == 0);
                        Assert.IsTrue(element.VertexCount == 0);
                    },
                element => element.Save(saveGamePath),
                () =>
                    {
                        var reloadedF8 = new Fallen8();
                        reloadedF8.Load(saveGamePath);
                        return reloadedF8;
                    },
                (reference, reloaded) => Assert.IsTrue(TestHelper.CheckIfFallen8IsEqual(reference, reloaded)),
                saveGamePath);
        }

        [TestMethod()]
        public void Persistence_RandomGraph_UnitTest()
        {
            var saveGamePath = System.IO.Path.Combine(Environment.CurrentDirectory, "randomGraph.fs8");

            for (int i = 0; i < 100; i++)
            {

                TestHelper.ExecuteTestWithPersistence(
                    () => TestHelper.CreateRandomGraph(5000, 10),
                    element =>
                    {
                        Assert.IsTrue(element != null);
                        Assert.IsTrue(element.EdgeCount > 0);
                        Assert.IsTrue(element.VertexCount > 0);
                    },
                    element => element.Save(saveGamePath),
                    () =>
                    {
                        var reloadedF8 = new Fallen8();
                        reloadedF8.Load(saveGamePath);
                        return reloadedF8;
                    },
                    (reference, reloaded) => Assert.IsTrue(TestHelper.CheckIfFallen8IsEqual(reference, reloaded)),
                    saveGamePath);
            }

            Thread.Sleep(2000);
        }
    }
}
