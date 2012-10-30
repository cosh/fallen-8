using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NoSQL.GraphDB.Model;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NoSQL.GraphDB.Test
{
    /// <summary>
    /// A static testhelper class
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// The load action
        /// </summary>
        /// <typeparam name="T">Type of the to be tested element</typeparam>
        /// <returns>An instance of the to be loaded element</returns>
        public delegate T Load<out T>();

        /// <summary>
        /// The test action
        /// </summary>
        /// <typeparam name="T">The type of the to be tested element</typeparam>
        /// <param name="toBeTestedElement">The to be tested element</param>
        public delegate void Test<in T>(T toBeTestedElement);

        /// <summary>
        /// The retest action
        /// </summary>
        /// <typeparam name="T">The type of the to be tested elements</typeparam>
        /// <param name="reference">The reference element</param>
        /// <param name="reloaded">The reloaded element</param>
        public delegate void Retest<in T>(T reference, T reloaded);

        /// <summary>
        /// The persist action
        /// </summary>
        /// <typeparam name="T">The type of the element that should be persisted</typeparam>
        /// <param name="toBePersistedElement">The to be persisted element</param>
        public delegate void Persist<in T>(T toBePersistedElement);
        
        /// <summary>
        /// The reload action
        /// </summary>
        /// <typeparam name="T">The type of the element that should be reloaded</typeparam>
        /// <returns>The reloaded element</returns>
        public delegate T Reload<out T>();
        
        /// <summary>
        /// The action that cleans up the mess
        /// </summary>
        public delegate void Clean();

        /// <summary>
        /// Executes a test with persistence
        /// </summary>
        /// <typeparam name="T">The type of the thing that should be persisted</typeparam>
        /// <param name="load">The load action</param>
        /// <param name="test">The test action</param>
        /// <param name="persist">The persist action</param>
        /// <param name="reload">The reload action</param>
        /// <param name="reTest">The retest action</param>
        /// <param name="clean">The clean action</param>
        public static void ExecuteTestWithPersistence<T>(Load<T> load, Test<T> test, Persist<T> persist, Reload<T> reload, Retest<T> reTest, Clean clean)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                var reference = load();
                sw.Stop();
                Debug.WriteLine(String.Format("LOAD: {0}ms", sw.Elapsed.TotalMilliseconds));

                sw.Restart();
                test(reference);
                sw.Stop();
                Debug.WriteLine(String.Format("TEST: {0}ms", sw.Elapsed.TotalMilliseconds));

                sw.Restart();
                persist(reference);
                sw.Stop();
                Debug.WriteLine(String.Format("PERSIST: {0}ms", sw.Elapsed.TotalMilliseconds));
                Thread.Sleep(1000);

                sw.Restart();
                var reloadedElement = reload();
                sw.Stop();
                Debug.WriteLine(String.Format("RELOAD: {0}ms", sw.Elapsed.TotalMilliseconds));

                sw.Restart();
                reTest(reference, reloadedElement);
                sw.Stop();
                Debug.WriteLine(String.Format("RETEST: {0}ms", sw.Elapsed.TotalMilliseconds));
            }
            finally
            {
                if (clean != null)
                {
                    clean();
                }
            }
        }

        /// <summary>
        /// Executes a test with persistence
        /// </summary>
        /// <typeparam name="T">The type of the thing that should be persisted</typeparam>
        /// <param name="load">The load action</param>
        /// <param name="test">The test action</param>
        /// <param name="persist">The persist action</param>
        /// <param name="reload">The reload action</param>
        /// <param name="saveGameName">The savegame filename</param>
        public static void ExecuteTestWithPersistence<T>(Load<T> load, Test<T> test, Persist<T> persist, Reload<T> reload, Retest<T> reTest, String saveGameName)
        {
            try
            {
                Cleanup(saveGameName);
                ExecuteTestWithPersistence(load, test, persist, reload, reTest, () => { });
                Thread.Sleep(1000);
            }
            finally
            {
                Cleanup(saveGameName);
            }
        }

        /// <summary>
        /// Cleans up a save game
        /// </summary>
        /// <param name="pathToSaveGame">The path to the savegame</param>
        public static void Cleanup(String pathToSaveGame)
        {
            var fileName = Path.GetFileName(pathToSaveGame);
            var path = Path.GetDirectoryName(pathToSaveGame);

            var files = Directory.EnumerateFiles(path, fileName + "*").ToList();

            foreach (var aFile in files)
            {
                File.Delete(aFile);
            }
        }

        /// <summary>
        /// Creates a random graph
        /// </summary>
        /// <param name="vertexCount">The vertex count</param>
        /// <param name="edgesPerVertex">The max number of edges per vertex</param>
        /// <returns>The F8 graph</returns>
        public static Fallen8 CreateRandomGraph(int vertexCount, int edgesPerVertex)
        {
            var result = new Fallen8();

            var propertyCount = 5;
            var vertexIDs = new List<int>();
            var prng = new Random();

            var availableProperties = GenerateProperties();

            for (int i = 0; i < vertexCount; i++)
            {
                var creationDate = Convert.ToUInt32(prng.Next(0, Int32.MaxValue));
                var properties = AssembleSomeProperties(availableProperties, prng, propertyCount);

                vertexIDs.Add(result.CreateVertex(creationDate, properties).Id);
            }

            for (int i = 0; i < vertexIDs.Count; i++)
            {
                var sourceId = vertexIDs[i];
                var countOfEdges = prng.Next(0, edgesPerVertex);

                for (int j = 0; j < countOfEdges; j++)
                {
                    var edgePropertyId = Convert.ToUInt16(prng.Next(UInt16.MinValue, UInt16.MaxValue));
                    var creationDate = Convert.ToUInt32(prng.Next(0, Int32.MaxValue));
                    var properties = AssembleSomeProperties(availableProperties, prng, propertyCount);
                    var targetId = vertexIDs[prng.Next(0, vertexIDs.Count)];

                    result.CreateEdge(sourceId, edgePropertyId, targetId, creationDate, properties);
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if two F8 instances are equal
        /// </summary>
        /// <param name="a">The Fallen-8 a</param>
        /// <param name="b">The Fallen-8 b</param>
        /// <returns></returns>
        public static bool CheckIfFallen8IsEqual(Fallen8 a, Fallen8 b)
        {
            Assert.AreEqual(a.VertexCount, b.VertexCount);
            Assert.AreEqual(a.EdgeCount, b.EdgeCount);

            foreach (var aReference in a.GetVertices())
            {
                VertexModel pendant;
                Assert.IsTrue(b.TryGetVertex(out pendant, aReference.Id));
                CheckIfVerticesAreEqual(aReference, pendant);
            }

            foreach (var aReference in a.GetEdges())
            {
                EdgeModel pendant;
                Assert.IsTrue(b.TryGetEdge(out pendant, aReference.Id));
                CheckIfEdgesAreEqual(aReference, pendant);
            }

            return true;
        }

        #region private helper

        private static void CheckIfEdgesAreEqual(EdgeModel aReference, EdgeModel pendant)
        {
            CheckIfAGraphElementsAreEqual(aReference, pendant);
        }

        private static void CheckIfVerticesAreEqual(VertexModel aReference, VertexModel pendant)
        {
            CheckIfAGraphElementsAreEqual(aReference, pendant);
        }

        private static void CheckIfAGraphElementsAreEqual(AGraphElement aReference, AGraphElement pendant)
        {
            Assert.AreEqual(aReference.Id, pendant.Id);
            Assert.AreEqual(aReference.CreationDate, pendant.CreationDate);
            Assert.AreEqual(aReference.ModificationDate, pendant.ModificationDate);
        }

        private static PropertyContainer[] AssembleSomeProperties(List<KeyValuePair<UInt16, Object>> availableProperties, Random prng, int propertyCount)
        {
            var result = new PropertyContainer[propertyCount];

            for (int i = 0; i < propertyCount; i++)
            {
                var propertyKV = availableProperties[prng.Next(0, availableProperties.Count)];

                result[i] = new PropertyContainer { PropertyId = propertyKV.Key, Value = propertyKV.Value };
            }
            return result;
        }

        private static List<KeyValuePair<UInt16, Object>> GenerateProperties()
        {
            var result = new List<KeyValuePair<UInt16, Object>>();

            #region strings 0 .. 10

            result.Add(new KeyValuePair<UInt16, Object>(0, ""));
            result.Add(new KeyValuePair<UInt16, Object>(1, "a"));
            result.Add(new KeyValuePair<UInt16, Object>(2, "äaüdlwqädlc"));
            result.Add(new KeyValuePair<UInt16, Object>(3, "2342341dsadsyc23r"));
            result.Add(new KeyValuePair<UInt16, Object>(4, "2342341dsadsyc23rssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssdddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddsddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd"));
            result.Add(new KeyValuePair<UInt16, Object>(4, GenerateRandomString(100)));
            result.Add(new KeyValuePair<UInt16, Object>(5, GenerateRandomString(10000)));

            #endregion

            #region Int32 11 .. 20

            result.Add(new KeyValuePair<UInt16, Object>(11, 23));
            result.Add(new KeyValuePair<UInt16, Object>(12, 0));
            result.Add(new KeyValuePair<UInt16, Object>(13, Int32.MinValue));
            result.Add(new KeyValuePair<UInt16, Object>(14, Int32.MaxValue));

            #endregion

            return result;
        }

        private static String GenerateRandomString(int numberOfChars)
        {
            const int lower = 0x21;
            const int upper = 0x7e;

            var builder = new StringBuilder();

            //create random with the seed (make sure it's not int.MinValue)
            var rnd = new Random(DateTime.Now.Millisecond % int.MinValue);

            for (int i = 0; i < numberOfChars; i++)
            {
                builder.Append((char)rnd.Next(lower, upper));
            }

            return builder.ToString();
        }

        #endregion
    }
}
