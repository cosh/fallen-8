// 
// EdgePropertyModel.cs
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

using System.Linq;
using System.Collections.Generic;
using Fallen8.API.Helper;
using Fallen8.API.Error;

namespace Fallen8.API.Model
{
    /// <summary>
    /// Edge property model.
    /// </summary>
    public sealed class EdgePropertyModel : AThreadSafeElement
    {
        #region Data
        
        /// <summary>
        /// The source vertex.
        /// </summary>
        public readonly VertexModel SourceVertex;
        
        /// <summary>
        /// The edges.
        /// </summary>
        private List<EdgeModel> _edges;
        
        #endregion
        
        #region Constructer
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EdgePropertyModel"/> class.
        /// </summary>
        /// <param name='sourceVertex'>
        /// Source vertex.
        /// </param>
        /// <param name='edges'>
        /// Edges.
        /// </param>
        public EdgePropertyModel (VertexModel sourceVertex, List<EdgeModel> edges)
        {
            SourceVertex = sourceVertex;
            _edges = edges;   
        }
    
        #endregion
       
        #region internal methods

        /// <summary>
        /// Trims the edges and makes them distinct.
        /// </summary>
        internal void TrimEdges()
        {
            if (WriteResource()) {
                
                _edges = _edges.Distinct().ToList();
                
                FinishWriteResource();
                
                return;
            }
            
            throw new CollisionException();
        }

        /// <summary>
        /// Adds an edge.
        /// </summary>
        /// <param name='outEdge'>
        /// Out edge.
        /// </param>
        internal void AddEdge(EdgeModel outEdge)
        {
            if (outEdge == null) return;
   
            if (WriteResource()) {
                       
                if (_edges == null)
                {
                    _edges = new List<EdgeModel> { outEdge };
                }
                else
                {
                    _edges.Add(outEdge);
                }
                
                FinishWriteResource();
                
                return;
            }
            
            throw new CollisionException();
            
        }

        /// <summary>
        /// Adds the edges.
        /// </summary>
        /// <param name='edges'>
        /// Edges.
        /// </param>
        internal void AddEdges(IEnumerable<EdgeModel> edges)
        {
            if (edges == null) return;
   
            if (WriteResource()) {
                if (_edges == null)
                {
                    _edges = new List<EdgeModel>(edges);
                }
                else
                {
                    _edges.AddRange(edges);
                }
                
                FinishWriteResource();
                
                return;
            }
            
            throw new CollisionException();
        }

        #endregion
        
        #region public methods
        
        /// <summary>
        /// Gets the edges.
        /// </summary>
        /// <returns>
        /// The edges.
        /// </returns>
        public IEnumerable<EdgeModel> GetEdges()
        {
            if (ReadResource()) {
                
                foreach (var aEdge in _edges) {
                    yield return aEdge;
                }
                
                FinishReadResource();
                
                yield break;
            }
            
            throw new CollisionException();
        }
        
        #endregion
    }
}

