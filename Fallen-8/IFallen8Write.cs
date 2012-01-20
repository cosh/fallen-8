// 
// IFallen8Write.cs
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
using Fallen8.API.Helper;
using Fallen8.API.Model;
using System.Collections.Generic;

namespace Fallen8.API
{
	/// <summary>
	/// Fallen8 write interface.
	/// </summary>
	public interface IFallen8Write
	{
		
		#region create
		
		/// <summary>
		/// Creates a vertex.
		/// </summary>
		/// <returns>
		/// The vertex model.
		/// </returns>
        VertexModel CreateVertex(DateTime creationDate, Dictionary<Int32, Object> properties = null, Dictionary<Int32, List<EdgeModelDefinition>> edges = null);
		
		/// <summary>
		/// Creates an edge.
		/// </summary>
		/// <returns>
		/// The edge model.
		/// </returns>
		/// <param name='sourceVertexId'>
		/// Source vertex identifier.
		/// </param>
		/// <param name='edgePropertyId'>
		/// Edge property identifier.
		/// </param>
		/// <param name='edgeDefinition'>
		/// Edge definition.
		/// </param>
        EdgeModel CreateEdge(Int32 sourceVertexId, Int32 edgePropertyId, EdgeModelDefinition edgeDefinition);
		
		#endregion
		
		#region update
		
		/// <summary>
		/// Tries to add a property.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the property was added; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='graphElementId'>
		/// Graph element identifier.
		/// </param>
		/// <param name='propertyId'>
		/// Property identifier.
		/// </param>
		/// <param name='property'>
		/// The to be added property.
		/// </param>
        Boolean TryAddProperty(Int32 graphElementId, Int32 propertyId, Object property);
		
		/// <summary>
		/// Tries to remove a property.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the property was removed; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='graphElementId'>
		/// Graph element identifier.
		/// </param>
		/// <param name='propertyId'>
		/// Property identifier.
		/// </param>
        Boolean TryRemoveProperty(Int32 graphElementId, Int32 propertyId);
		
		#endregion
		
		#region delete
		
		/// <summary>
		/// Tries the remove graph element.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the graph element was removed; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='graphElementId'>
		/// Graph element identifier.
		/// </param>
        Boolean TryRemoveGraphElement(Int32 graphElementId);
		
		#endregion
	}
}

