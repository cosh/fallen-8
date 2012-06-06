// 
//  ServiceHelper.cs
//  
//  Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
//  
//  Copyright (c) 2012 Henning Rauch
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Jint;
using NoSQL.GraphDB.Algorithms.Path;
using NoSQL.GraphDB.Helper;
using NoSQL.GraphDB.Index;
using NoSQL.GraphDB.Index.Fulltext;
using NoSQL.GraphDB.Index.Spatial;
using NoSQL.GraphDB.Log;
using NoSQL.GraphDB.Model;
using NoSQL.GraphDB.Plugin;
using NoSQL.GraphDB.Service.REST.Ressource;
using NoSQL.GraphDB.Service.REST.Result;
using NoSQL.GraphDB.Service.REST.Specification;

#endregion

namespace NoSQL.GraphDB.Service.REST
{
    /// <summary>
    ///   Static service helper class
    /// </summary>
    public static class ServiceHelper
    {
        /// <summary>
        /// Creates the plugin options.
        /// </summary>
        /// <returns>
        /// The plugin options.
        /// </returns>
        /// <param name='options'>
        /// Options.
        /// </param>
        internal static IDictionary<string, object> CreatePluginOptions(Dictionary<string, PropertySpecification> options)
        {
            return options.ToDictionary(key => key.Key, value => CreateObject(value.Value));
        }

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <returns>
        /// The object.
        /// </returns>
        /// <param name='key'>
        /// Key.
        /// </param>
        internal static object CreateObject(PropertySpecification key)
        {
            return Convert.ChangeType(
                key.Property,
                Type.GetType(key.FullQualifiedTypeName, true, true));
        }
    }
}