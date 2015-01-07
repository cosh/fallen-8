// 
//  ServiceHelper.cs
//  
//  Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
//  
//  Copyright (c) 2012-2015 Henning Rauch
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

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using NoSQL.GraphDB.Model;
using NoSQL.GraphDB.Service.REST.Specification;

#endregion

#region Usings

#endregion

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

		/// <summary>
        ///   Generates the properties.
        /// </summary>
        /// <returns> The properties. </returns>
        /// <param name='propertySpecification'> Property specification. </param>
        public static PropertyContainer[] GenerateProperties(
            Dictionary<UInt16, PropertySpecification> propertySpecification)
        {
            PropertyContainer[] properties = null;

            if (propertySpecification != null)
            {
                var propCounter = 0;
                properties = new PropertyContainer[propertySpecification.Count];

                foreach (var aPropertyDefinition in propertySpecification)
                {
                    properties[propCounter] = new PropertyContainer
                     {
                         PropertyId = aPropertyDefinition.Key,
                         Value = aPropertyDefinition.Value.FullQualifiedTypeName != null
                             ? Convert.ChangeType(aPropertyDefinition.Value.Property,
                                                Type.GetType(
                                                    aPropertyDefinition.Value.FullQualifiedTypeName,
                                                    true, true))
                            : aPropertyDefinition.Value.Property
                     };
                    propCounter++;
                }
            }

            return properties;
        }

		public static Object Transform(PropertySpecification definition)
		{
            return definition.FullQualifiedTypeName == null 
                ? definition.Property
                : Convert.ChangeType(definition.Property, Type.GetType(definition.FullQualifiedTypeName, true, true));
		}
    }
}