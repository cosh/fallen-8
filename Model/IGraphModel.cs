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
		
		/// <summary>
		/// Gets vertices by their vertex type identifier.
		/// </summary>
		/// <returns>
		/// The vertices by type.
		/// </returns>
		/// <param name='vertexTypeId'>
		/// Vertex type identifier.
		/// </param>
        IEnumerable<IVertexModel> GetVerticesByType(Int64 vertexTypeId);
        
		/// <summary>
		/// Gets edged by their edge type identifier.
		/// </summary>
		/// <returns>
		/// The edges by type.
		/// </returns>
		/// <param name='edgeTypeId'>
		/// Edge type identifier.
		/// </param>
		IEnumerable<IEdgeModel> GetEdgesByType(Int64 edgeTypeId);
        
		/// <summary>
		/// Gets graphs by their graph type identifier.
		/// </summary>
		/// <returns>
		/// The graphs by type.
		/// </returns>
		/// <param name='graphTypeId'>
		/// Graph type identifier.
		/// </param>
		IEnumerable<IGraphModel> GetGraphsByType(Int64 graphTypeId);
    }
}
