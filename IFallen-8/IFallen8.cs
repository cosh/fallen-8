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
		
		Int64 CreateVertex();
		
		Int64 CreateEdge();
		
		#endregion
		
		#region update
		
		Boolean TryAddProperty(Int64 graphElementId, Int64 propertyId, IComparable property);

		Boolean TryAddSchemalessProperty(Int64 graphElementId, String propertyName, Object schemalessProperty);
		
		Boolean TryRemoveProperty(Int64 graphElementId, Int64 propertyId);
		
		Boolean TryRemoveSchemalessProperty(Int64 graphElementId, String propertyName);
		
		#endregion
		
		#region delete
		
		Boolean TryRemoveGraphElement(Int64 graphElementId);
		
		#endregion
		
		#region search
		
		IEnumerable<IGraphElementModel> Search(out IEnumerable<IGraphElementModel> result, Int64 propertyId, IComparable literal, BinaryOperator binOp = BinaryOperator.Equals);
		
		IEnumerable<IGraphElementModel> SearchSchemaless(out IEnumerable<IGraphElementModel> result, String propertyName, IComparable literal, BinaryOperator binOp = BinaryOperator.Equals);
	
		Boolean SearchInRange(out IEnumerable<IGraphElementModel> result, Int64 propertyId, IComparable leftLimit, IComparable rightLimit,Boolean includeLeft = true, Boolean includeRight = true);
		
		Boolean SearchFulltext(out FulltextSearchResult result, Int64 indexId, String searchQuery);
		
		Boolean SearchGeo(out IEnumerable<IGraphElementModel> result, Int64 indexId, IGeometry geometry, Double? distance);
		
		#endregion
    }
}
