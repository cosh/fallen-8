// 
// GraphModel.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2012 Henning Rauch
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
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Fallen8.API.Error;

namespace Fallen8.Model
{
    /// <summary>
    /// Graph model.
    /// </summary>
    public sealed class GraphModel : AGraphElement
    {
        #region Data
        
        /// <summary>
        /// The graph elements.
        /// </summary>
        public readonly ConcurrentDictionary<long, AGraphElement> GraphElements;
        
        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Fallen8.Model.GraphModel"/> class.
        /// </summary>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        /// <param name='creationDate'>
        /// Creation date.
        /// </param>
        /// <param name='properties'>
        /// Properties.
        /// </param>
        public GraphModel (Int64 id, DateTime creationDate, Dictionary<Int64, Object> properties) : base(id, creationDate, properties)
        {
            GraphElements = new ConcurrentDictionary<long, AGraphElement>();   
        }
        
        #endregion
        
        #region IGraphModel implementation

        /// <summary>
        /// Gets a vertex by its identifier.
        /// </summary>
        /// <returns>
        /// The vertex.
        /// </returns>
        /// <param name='id'>
        /// System wide unique identifier.
        /// </param>
        public VertexModel GetVertex (long id)
        {
            return GetElement (id) as VertexModel;
        }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <returns>
        /// The vertices.
        /// </returns>
        public IEnumerable<VertexModel> GetVertices ()
        {
            return GraphElements.Where (aGraphElementKV => aGraphElementKV.Value is VertexModel).Select (aVertexKV => (VertexModel)aVertexKV.Value);
        }

        /// <summary>
        /// Gets an edge by its identifier.
        /// </summary>
        /// <returns>
        /// The edge.
        /// </returns>
        /// <param name='id'>
        /// System wide unique identifier.
        /// </param>
        public EdgeModel GetEdge (long id)
        {
            return GetElement (id) as EdgeModel;
        }

        /// <summary>
        /// Gets the edges.
        /// </summary>
        /// <returns>
        /// The edges.
        /// </returns>
        public IEnumerable<EdgeModel> GetEdges ()
        {
            return GraphElements.Where (aGraphElementKV => aGraphElementKV.Value is EdgeModel).Select (aEdgeKV => (EdgeModel)aEdgeKV.Value);
        }

        #endregion

        #region private methods
        
        /// <summary>
        /// Gets an element.
        /// </summary>
        /// <returns>
        /// The element or null if there is no such element.
        /// </returns>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        private AGraphElement GetElement (long id)
        {
            AGraphElement result;
            
            GraphElements.TryGetValue (id, out result);
            
            return result;
        }
        
        #endregion
    }
}

