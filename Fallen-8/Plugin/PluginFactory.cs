// 
// PluginFactory.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2012 Henning Rauch
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

#endregion

namespace NoSQL.GraphDB.Plugin
{
    /// <summary>
    ///   Fallen8 plugin factory.
    /// </summary>
    public static class PluginFactory
    {
        /// <summary>
        /// A TypeEvaluator delegate
        /// </summary>
        /// <param name="type">The to be evaluated type</param>
        /// <returns>True = OK otherwise not</returns>
        public delegate bool TypeEvaluator(Type type);

        /// <summary>
        ///   Tries to find a plugin.
        /// </summary>
        /// <returns> <c>true</c> if something was found; otherwise, <c>false</c> . </returns>
        /// <param name='result'> Result. </param>
        /// <param name='name'> The unique name of the pluginN. </param>
        /// <typeparam name='T'> The interface type of the plugin. </typeparam>
        public static Boolean TryFindPlugin<T>(out T result, String name) where T : IPlugin
        {
            foreach (var aPluginTypeOfT in GetAllTypes<T>())
            {
                var aPluginInstance = Activate<IPlugin>(aPluginTypeOfT);
                if (aPluginInstance != null)
                {
                    if (aPluginInstance.PluginName == name)
                    {
                        result = (T) aPluginInstance;
                        return true;
                    }
                }
            }

            result = default(T);
            return false;
        }

        /// <summary>
        ///   Tries to find a class.
        /// </summary>
        /// <returns> <c>true</c> if something was found; otherwise, <c>false</c> . </returns>
        /// <param name='result'> Result. </param>
        /// <param name='evaluator'> A type evaluator delegate </param>
        /// <typeparam name='T'> The interface type of the plugin. </typeparam>
        public static Boolean TryFind<T>(out T result, TypeEvaluator evaluator)
        {
            foreach (var aPluginTypeOfT in GetAllTypes<T>(false).Where(_ => evaluator(_)))
            {
                var aPluginInstance = Activator.CreateInstance(aPluginTypeOfT);
                if (aPluginInstance != null)
                {
                    result = (T)aPluginInstance;
                    return true;
                }
            }

            result = default(T);
            return false;
        }

        /// <summary>
        ///   Tries to get available plugin descriptions.
        /// </summary>
        /// <returns> <c>true</c> if something was found; otherwise, <c>false</c> . </returns>
        /// <param name='result'> Result. </param>
        /// <typeparam name='T'> The interface type of the plugin. </typeparam>
        public static Boolean TryGetAvailablePluginsWithDescriptions<T>(out Dictionary<String, String> result)
        {
            result = (from aPluginTypeOfT in GetAllTypes<T>()
                      select Activate<IPlugin>(aPluginTypeOfT)
                      into aPluginInstance
                      where aPluginInstance != null
                      select aPluginInstance).ToDictionary(key => key.PluginName, GenerateDescription);
            return result.Any();
        }

        /// <summary>
        ///   Tries to get available plugin descriptions.
        /// </summary>
        /// <returns> <c>true</c> if something was found; otherwise, <c>false</c> . </returns>
        /// <param name='result'> Result. </param>
        /// <typeparam name='T'> The interface type of the plugin. </typeparam>
        public static Boolean TryGetAvailablePlugins<T>(out IEnumerable<String> result)
        {
            result = (from aPluginTypeOfT in GetAllTypes<T>()
                      select Activate<IPlugin>(aPluginTypeOfT)
                      into aPluginInstance
                      where aPluginInstance != null
                      select aPluginInstance.PluginName);
            return result.Any();
        }

		/// <summary>
		/// Assimilate the specified dllStream.
		/// </summary>
		/// <param name='dllStream'>
		/// Dll stream.
		/// </param>
		/// <param name='path'>
		/// The path where the dll should be assimilated.
		/// </param>
		public static String Assimilate (Stream dllStream, String path = null)
		{
			var assimilationPath = path ?? Environment.CurrentDirectory + Path.DirectorySeparatorChar + Path.GetRandomFileName() + ".dll";

			using(var dllFileStream = File.Create(assimilationPath, 1024))
			{
                dllStream.CopyTo(dllFileStream);
			}

		    return assimilationPath;
		}

        /// <summary>
        /// Tries to find plugins
        /// </summary>
        /// <typeparam name="T">The interface of the plugins</typeparam>
        /// <param name="newTypeObjects">The resulting types</param>
        /// <param name="pathToNewAssembly">The path to the interesting assembly</param>
        /// <param name="checkForIPlugin">Check for IPlugin</param>
        /// <returns>True for success otherwise false</returns>
        public static bool TryFindPlugins<T>(out IEnumerable<T> newTypeObjects, string pathToNewAssembly, bool checkForIPlugin = true) where T:class 
        {
            var newTypes = new List<Type>(ProcessAFile<T>(pathToNewAssembly, checkForIPlugin));

            newTypeObjects = newTypes.Select(Activate<T>);

            return newTypes.Count > 0;
        }

        #region private helper

        /// <summary>
        ///   Generates the description for a plugin
        /// </summary>
        /// <param name="aPluginInstance"> A plugin instance </param>
        /// <returns> </returns>
        private static string GenerateDescription(IPlugin aPluginInstance)
        {
            var sb = new StringBuilder();

            sb.AppendLine(String.Format("NAME: {0}", aPluginInstance.PluginName));
            sb.AppendLine(String.Format("  *DESCRIPTION: {0}", aPluginInstance.Description));
            sb.AppendLine(String.Format("  *MANUFACTURER: {0}", aPluginInstance.Manufacturer));
            sb.AppendLine(String.Format("  *TYPE: {0}", aPluginInstance.GetType().FullName));
            sb.AppendLine(String.Format("  *CATEGORY: {0}", aPluginInstance.PluginCategory.FullName));

            return sb.ToString();
        }

        /// <summary>
        ///   Gets all types.
        /// </summary>
        /// <returns> The all types. </returns>
        /// <typeparam name='T'> The type of the plugin. </typeparam>
        private static IEnumerable<Type> GetAllTypes<T>(Boolean checkForIPlugin = true)
        {
            var result = new List<Type>();

            string currentAssemblyDirectoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var files = Directory.EnumerateFiles(currentAssemblyDirectoryName, "*.dll")
                .Union(Directory.EnumerateFiles(currentAssemblyDirectoryName, "*.exe"));

            foreach (var file in files)
            {
                result.AddRange(ProcessAFile<T>(file, checkForIPlugin));
            }

            return result;
        }

        /// <summary>
        ///   Determines whether a type is interface of the specified type.
        /// </summary>
        /// <returns> <c>true</c> if this instance is interface of the specified type; otherwise, <c>false</c> . </returns>
        /// <param name='type'> Type. </param>
        /// <typeparam name='T'> The interface type. </typeparam>
        private static Boolean IsInterfaceOf<T>(Type type)
        {
            return typeof (T).IsAssignableFrom(type);
        }

        /// <summary>
        ///   Activate the specified currentPluginType.
        /// </summary>
        /// <param name='currentPluginType'> Current plugin type. </param>
        private static T Activate<T>(Type currentPluginType) where T:class 
        {
            Object instance;

            try
            {
                instance = Activator.CreateInstance(currentPluginType);
            }
            catch (TypeLoadException)
            {
                return default(T);
            }

            return instance as T;
        }

        /// <summary>
        /// Processes a file
        /// </summary>
        /// <typeparam name="T">The interface type</typeparam>
        /// <param name="file">The interesting file</param>
        /// <param name="checkForIPlugin">Should there be a check for IPlugin</param>
        /// <returns>Enumerable of matching types</returns>
        private static IEnumerable<Type> ProcessAFile<T>(string file, bool checkForIPlugin)
        {
            var assembly = Assembly.LoadFrom(file);
            var types = assembly.GetTypes();

            foreach (var aType in types)
            {
                if (!aType.IsClass || aType.IsAbstract)
                {
                    continue;
                }

                if (!aType.IsPublic)
                {
                    continue;
                }

                if (checkForIPlugin && !IsInterfaceOf<IPlugin>(aType))
                {
                    continue;
                }

                if (!IsInterfaceOf<T>(aType))
                {
                    continue;
                }

                if (aType.GetConstructor(Type.EmptyTypes) == null)
                {
                    continue;
                }

                yield return aType;
            }
        }

        #endregion
    }
}