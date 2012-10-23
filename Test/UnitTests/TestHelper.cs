using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NoSQL.GraphDB.Test
{
    /// <summary>
    /// A static testhelper class
    /// </summary>
    public static class TestHelper
    {
        public delegate T Load<out T>();
        public delegate void Test<in T>(T toBeTestedElement);
        public delegate void Persist<in T>(T toBePersistedElement);
        public delegate T Reload<out T>();
        public delegate void Clean();

        
        public static void ExecuteTestWithPersistence<T>(Load<T> load, Test<T> test, Persist<T> persist, Reload<T> reload, Clean clean)
        {
            try
            {
                var toBeTestedElement = load();
                test(toBeTestedElement);
                persist(toBeTestedElement);
                toBeTestedElement = reload();
                test(toBeTestedElement);
            }
            finally
            {
                if (clean != null)
                {
                    clean();
                }
            }
        }

        public static void ExecuteTestWithPersistence<T>(Load<T> load, Test<T> test, Persist<T> persist, Reload<T> reload, String saveGameName)
        {
            try
            {
                ExecuteTestWithPersistence(load, test, persist, reload, () => { });
            }
            finally
            {
                Cleanup(saveGameName);
            }
        }

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
    }
}
