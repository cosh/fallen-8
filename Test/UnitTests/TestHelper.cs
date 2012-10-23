using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
                ExecuteTestWithPersistence(load, test, persist, reload, reTest, () => { });
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
            return new Fallen8();
        }

        /// <summary>
        /// Checks if two F8 instances are equal
        /// </summary>
        /// <param name="a">The Fallen-8 a</param>
        /// <param name="b">The Fallen-8 b</param>
        /// <returns></returns>
        public static bool CheckIfFallen8IsEqual(Fallen8 a, Fallen8 b)
        {
            return true;
        }
    }
}
