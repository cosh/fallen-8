// 
// IGraphModel.cs
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

namespace Fallen8.Model
{
	/// <summary>
	/// Graph model interface.
	/// </summary>
    public interface IGraphModel : IGraphElementModel
    {
		/// <summary>
		/// Gets the graph elements.
		/// </summary>
		/// <value>
		/// The graph elements.
		/// </value>
        IDictionary<Int64, IGraphElementModel> Graphelements { get; }
		
		#region vertices
		
		/// <summary>
		/// Gets a vertex by its identifier.
		/// </summary>
		/// <returns>
		/// The vertex.
		/// </returns>
		/// <param name='id'>
		/// System wide unique identifier.
		/// </param>
		IVertexModel GetVertex (Int64 id);
        
		/// <summary>
		/// Gets vertices by their identifier.
		/// </summary>
		/// <returns>
		/// The vertices.
		/// </returns>
		/// <param name='ids'>
		/// System wide unique identifiers.
		/// </param>
		IEnumerable<IVertexModel> GetVertices (IEnumerable<Int64> ids);
		
		/// <summary>
		/// Gets the vertices.
		/// </summary>
		/// <returns>
		/// The vertices.
		/// </returns>
		IEnumerable<IVertexModel> GetVertices ();
		
		#endregion
		
		#region edges
		
		/// <summary>
		/// Gets an edge by its identifier.
		/// </summary>
		/// <returns>
		/// The edge.
		/// </returns>
		/// <param name='id'>
		/// System wide unique identifier.
		/// </param>
		IEdgeModel GetEdge (Int64 id);
        
		/// <summary>
		/// Gets edges by their identifier.
		/// </summary>
		/// <returns>
		/// The edges.
		/// </returns>
		/// <param name='ids'>
		/// System wide unique identifiers.
		/// </param>
		IEnumerable<IEdgeModel> GetEdges (IEnumerable<Int64> ids);
		
		/// <summary>
		/// Gets the edges.
		/// </summary>
		/// <returns>
		/// The edges.
		/// </returns>
		IEnumerable<IEdgeModel> GetEdges ();
		
		#endregion
    }
}
