// 
// Fallen8PluginFactory.cs
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
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace Fallen8.API.Plugin
{
    /// <summary>
    /// Fallen8 plugin factory.
    /// </summary>
    public static class Fallen8PluginFactory
    {
        /// <summary>
        /// Tries to find a plugin.
        /// </summary>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
        /// Result.
        /// </param>
        /// <param name='name'>
        /// The unique name of the pluginN.
        /// </param>
        /// <typeparam name='T'>
        /// The interface type of the plugin.
        /// </typeparam>
        public static Boolean TryFindPlugin<T> (out T result, String name)
        {
            foreach (var aPluginTypeOfT in GetAllTypes<T>()) {
                var aPluginInstance = Activate (aPluginTypeOfT);
                if (aPluginInstance != null) {
                    if (aPluginInstance.PluginName == name) {
                        result = (T)aPluginInstance;
                        return true;
                    }
                }
            }
            
            result = default(T);
            return false;
        }
        
        /// <summary>
        /// Tries to get available plugin descriptions.
        /// </summary>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
        /// Result.
        /// </param>
        /// <typeparam name='T'>
        /// The interface type of the plugin.
        /// </typeparam>
        public static Boolean TryGetAvailablePlugins<T> (out IEnumerable<String> result)
        {
            var descriptions = (from aPluginTypeOfT in GetAllTypes<T>() select Activate(aPluginTypeOfT) 
                                    into aPluginInstance where aPluginInstance != null select aPluginInstance.Description)
                                    .ToList();

            result = descriptions;
            
            return descriptions.Any();
        }
        
        #region private helper
        
        /// <summary>
        /// Gets all types.
        /// </summary>
        /// <returns>
        /// The all types.
        /// </returns>
        /// <typeparam name='T'>
        /// The type of the plugin.
        /// </typeparam>
        private static IEnumerable<Type> GetAllTypes<T> ()
        {
            var result = new List<Type> ();
            
            var files = Directory.EnumerateFiles (Environment.CurrentDirectory, "*.dll")
                .Union (Directory.EnumerateFiles (Environment.CurrentDirectory, "*.exe"));

            foreach (var file in files)
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

                    if (!IsInterfaceOf<IFallen8Plugin>(aType))
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

                    result.Add(aType);
                }
            }

            return result;
            
        }
        
        /// <summary>
        /// Determines whether a type is interface of the specified type.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is interface of the specified type; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='type'>
        /// Type.
        /// </param>
        /// <typeparam name='T'>
        /// The interface type.
        /// </typeparam>
        private static Boolean IsInterfaceOf<T> (Type type)
        {
            return typeof(T).IsAssignableFrom (type);
        }
        
        /// <summary>
        /// Activate the specified currentPluginType.
        /// </summary>
        /// <param name='currentPluginType'>
        /// Current plugin type.
        /// </param>
        private static IFallen8Plugin Activate (Type currentPluginType)
        {
            Object instance;
                        
            try {
                instance = Activator.CreateInstance (currentPluginType);

            } catch (TypeLoadException) {
                return null;                   
            }
            
            return instance as IFallen8Plugin;
        }
        
        #endregion
    }
}

