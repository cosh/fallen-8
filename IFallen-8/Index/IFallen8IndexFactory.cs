// 
// IFallen8IndexFactory.cs
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
using Fallen8.API.Helper;
using Fallen8.API.Plugin;

namespace Fallen8.API.Index
{
	/// <summary>
	/// Index factory.
	/// </summary>
	public interface IFallen8IndexFactory
	{
		/// <summary>
		/// Gets the indices.
		/// </summary>
		/// <value>
		/// The indices.
		/// </value>
		IDictionary<String, IIndex> Indices { get; }
		
		/// <summary>
		/// Gets the available index plugins.
		/// </summary>
		/// <returns>
		/// The available index plugins.
		/// </returns>
		IEnumerable<PluginDescription> GetAvailableIndexPlugins();
		
		/// <summary>
		/// Tries to create an index.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the index was created; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='index'>
		/// The created index.
		/// </param>
        /// <param name='indexName'>
        /// Index name.
        /// </param> 
		/// <param name='indexTypeName'>
		/// Index type.
		/// </param>
		/// <param name='parameter'>
		/// Parameter for the index.
		/// </param>
		Boolean TryCreateIndex(out IIndex index, String indexName, String indexTypeName, IDictionary<String, Object> parameter);
		
		/// <summary>
		/// Tries to delete the index.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the index was deleted; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='indexName'>
		/// Index name.
		/// </param>
		Boolean TryDeleteIndex(String indexName);
		
		/// <summary>
		/// Tries the index of the get.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the index was found; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='index'>
		/// Index.
		/// </param>
		/// <param name='indexName'>
		/// Index name.
		/// </param>
		Boolean TryGetIndex(out IIndex index, String indexName);
	}
}

