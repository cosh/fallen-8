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
		/// The vertex identifier.
		/// </returns>
		/// <param name='vertexDefinition'>
		/// Vertex definition.
		/// </param>
		Int64 CreateVertex (VertexModelDefinition vertexDefinition);
		
		/// <summary>
		/// Creates an edge.
		/// </summary>
		/// <returns>
		/// The edge identifier.
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
		Int64 CreateEdge (Int64 sourceVertexId, Int64 edgePropertyId, EdgeModelDefinition edgeDefinition);
		
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
		Boolean TryAddProperty (Int64 graphElementId, Int64 propertyId, IComparable property);
		
		/// <summary>
		/// Tries to add a string property.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the property was added; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='graphElementId'>
		/// Graph element identifier.
		/// </param>
		/// <param name='propertyName'>
		/// Property name.
		/// </param>
		/// <param name='schemalessProperty'>
		/// The to be added schemaless property.
		/// </param>
		Boolean TryAddStringProperty (Int64 graphElementId, String propertyName, Object schemalessProperty);
		
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
		Boolean TryRemoveProperty (Int64 graphElementId, Int64 propertyId);
		
		/// <summary>
		/// Tries to remove a string property.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the property was removed; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='graphElementId'>
		/// Graph element identifier.
		/// </param>
		/// <param name='propertyName'>
		/// Property name.
		/// </param>
		Boolean TryRemoveStringProperty (Int64 graphElementId, String propertyName);
		
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
		Boolean TryRemoveGraphElement (Int64 graphElementId);
		
		#endregion
	}
}

