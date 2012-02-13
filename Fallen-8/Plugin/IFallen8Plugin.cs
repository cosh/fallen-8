// 
// IFallen8Plugin.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2011 Henning Rauch
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
using System.Collections.Generic;
using Framework.Serialization;

namespace Fallen8.API.Plugin
{
	public interface IFallen8Plugin : IDisposable
	{
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        String PluginName { get; }

        /// <summary>
        /// Gets or sets the type of the plugin.
        /// </summary>
        /// <value>
        /// The type of the plugin.
        /// </value>
        Type PluginType { get; }

        /// <summary>
        /// Gets or sets the plugin category.
        /// </summary>
        /// <value>
        /// The plugin category.
        /// </value>
        Type PluginCategory { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        String Description { get; }

        /// <summary>
        /// Gets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        String Manufacturer { get; }
		
		/// <summary>
		/// Tries to inititialize the plugin.
		/// </summary>
        /// <param name='fallen8'>
        /// A fallen-8 session.
        /// </param>
		/// <param name='parameter'>
		/// Parameter.
		/// </param>
        /// <returns>
        /// The initialized plugin
        /// </returns>
        void Initialize(Fallen8 fallen8, IDictionary<String, Object> parameter);
        
        /// <summary>
        /// Save the plugin.
        /// </summary>
        /// <param name='writer'>
        /// Writer.
        /// </param>
        void Save(SerializationWriter writer);
        
        /// <summary>
        /// Open the plugin.
        /// </summary>
        /// <param name='reader'>
        /// Reader.
        /// </param>
        void Open(SerializationReader reader, Fallen8 fallen8);
	}
}

