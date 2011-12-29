// 
// IFallen8.cs
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
using System.Linq;
using System.Text;
using Fallen8.Model;
using Fallen8.API.Expression;
using Fallen8.API.Index;
using Fallen8.API.Helper;

namespace Fallen8.API
{
	/// <summary>
	/// Fallen-8 interface.
	/// </summary>
    public interface IFallen8
    {
		/// <summary>
		/// Gets the graph.
		/// </summary>
		/// <value>
		/// The graph.
		/// </value>
        IGraphModel Graph { get; }
		
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
		Int64 CreateVertex(VertexModelDefinition vertexDefinition);
		
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
		Int64 CreateEdge(Int64 sourceVertexId, Int64 edgePropertyId, EdgeModelDefinition edgeDefinition);
		
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
		Boolean TryAddProperty(Int64 graphElementId, Int64 propertyId, IComparable property);
		
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
		Boolean TryAddStringProperty(Int64 graphElementId, String propertyName, Object schemalessProperty);
		
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
		Boolean TryRemoveProperty(Int64 graphElementId, Int64 propertyId);
		
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
		Boolean TryRemoveStringProperty(Int64 graphElementId, String propertyName);
		
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
		Boolean TryRemoveGraphElement(Int64 graphElementId);
		
		#endregion
		
		#region search
		
		/// <summary>
		/// Search for graph elements by a specified propertyId, literal and binary operation.
		/// </summary>
		/// <returns>
		/// <c>true</c> if something was found; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='result'>
		/// The resulting graph elements.
		/// </param>
		/// <param name='propertyId'>
		/// Property identifier.
		/// </param>
		/// <param name='literal'>
		/// Literal.
		/// </param>
		/// <param name='binOp'>
		/// Binary operator.
		/// </param>
		Boolean Search(out IEnumerable<IGraphElementModel> result, Int64 propertyId, IComparable literal, BinaryOperator binOp = BinaryOperator.Equals);
		
		/// <summary>
		/// Search for graph elements by a specified propertyName, literal and binary operation.
		/// </summary>
		/// <returns>
		/// <c>true</c> if something was found; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='result'>
		/// The resulting graph elements.
		/// </param>
		/// <param name='propertyName'>
		/// Property name.
		/// </param>
		/// <param name='literal'>
		/// Literal.
		/// </param>
		/// <param name='binOp'>
		/// Binary operator.
		/// </param>
		Boolean Search(out IEnumerable<IGraphElementModel> result, String propertyName, IComparable literal, BinaryOperator binOp = BinaryOperator.Equals);
		
		/// <summary>
		/// Search for graph elements by a specified property range.
		/// </summary>
		/// <returns>
		/// <c>true</c> if something was found; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='result'>
		/// The resulting graph elements.
		/// </param>
		/// <param name='propertyId'>
		/// Property identifier.
		/// </param>
		/// <param name='leftLimit'>
		/// Left limit.
		/// </param>
		/// <param name='rightLimit'>
		/// Right limit.
		/// </param>
		/// <param name='includeLeft'>
		/// Include left.
		/// </param>
		/// <param name='includeRight'>
		/// Include right.
		/// </param>
		Boolean SearchInRange(out IEnumerable<IGraphElementModel> result, Int64 propertyId, IComparable leftLimit, IComparable rightLimit,Boolean includeLeft = true, Boolean includeRight = true);
		
		/// <summary>
		/// Fulltext search for graph elements by a specified query string using an index.
		/// </summary>
		/// <returns>
		/// <c>true</c> if something was found; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='result'>
		/// The resulting fulltext result.
		/// </param>
		/// <param name='indexId'>
		/// Index identifier.
		/// </param>
		/// <param name='searchQuery'>
		/// Search query.
		/// </param>
		Boolean SearchFulltext(out FulltextSearchResult result, Int64 indexId, String searchQuery);
		
		/// <summary>
		/// Spatial search for graph elements by a specified geometry and distance using an spatial index.
		/// </summary>
		/// <returns>
		/// <c>true</c> if something was found; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='result'>
		/// The resulting graph elements.
		/// </param>
		/// <param name='indexId'>
		/// Index identifier.
		/// </param>
		/// <param name='geometry'>
		/// Geometry.
		/// </param>
		/// <param name='distance'>
		/// Distance.
		/// </param>
		Boolean SearchSpatial(out IEnumerable<IGraphElementModel> result, Int64 indexId, IGeometry geometry, Double? distance);
		
		#endregion
    }
}
