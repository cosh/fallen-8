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
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Fallen8.API.Helper;
using Fallen8.API.Error;

namespace Fallen8.Model
{
    /// <summary>
    /// Edge property model.
    /// </summary>
    public sealed class EdgePropertyModel : AThreadSafeElement, IEdgePropertyModel
    {
        #region Data
        
        /// <summary>
        /// The source vertex.
        /// </summary>
        private readonly IVertexModel _sourceVertex;
        
        /// <summary>
        /// The edges.
        /// </summary>
        private List<IEdgeModel> _edges;
        
        #endregion
        
        #region Constructer
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Fallen8.Model.EdgePropertyModel"/> class.
        /// </summary>
        /// <param name='sourceVertex'>
        /// Source vertex.
        /// </param>
        /// <param name='edges'>
        /// Edges.
        /// </param>
        public EdgePropertyModel (VertexModel sourceVertex, List<IEdgeModel> edges)
        {
            _sourceVertex = sourceVertex;
            _edges = edges;   
        }
    
        #endregion
        
        #region IEdgePropertyModel implementation
        public IVertexModel SourceVertex {
            get {
                return _sourceVertex;
            }
        }
        
        public void TrimEdges ()
        {
            if (WriteResource ()) {
                _edges = _edges.Distinct ().ToList ();
                
                FinishWriteResource ();
            }
            
            throw new CollisionException ();
        }
        
        #endregion

        #region IEnumerable[Fallen8.Model.IEdgeModel] implementation
        public IEnumerator<IEdgeModel> GetEnumerator ()
        {
            return _edges.GetEnumerator();
        }
        #endregion

        #region IEnumerable implementation
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return _edges.GetEnumerator();
        }
        #endregion
    }
}

