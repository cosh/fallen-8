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

namespace Fallen8.Model
{
    public sealed class GraphModel : AGraphElement, IGraphModel
    {
        #region Data
        
        private readonly IDictionary<long, IGraphElementModel> _graphElements;
        
        #endregion
        
        #region IGraphModel implementation
        
        public IVertexModel GetVertex (long id)
        {
            return GetElement (id) as IVertexModel;
        }

        public IEnumerable<IVertexModel> GetVertices ()
        {
            return _graphElements.Where (aGraphElementKV => aGraphElementKV.Value is IVertexModel).Select (aVertexKV => (IVertexModel)aVertexKV.Value);
        }

        public IEdgeModel GetEdge (long id)
        {
            return GetElement (id) as IEdgeModel;
        }

        public IEnumerable<IEdgeModel> GetEdges ()
        {
            return _graphElements.Where (aGraphElementKV => aGraphElementKV.Value is IEdgeModel).Select (aEdgeKV => (IEdgeModel)aEdgeKV.Value);
        }

        public IDictionary<long, IGraphElementModel> Graphelements {
            get {
                return _graphElements;
            }
        }
        #endregion

        #region IGraphElementModel implementation
        public long Id {
            get {
                return base._id;
            }
        }

        public DateTime CreationDate {
            get {
                return base._creationDate;
            }
        }

        public DateTime ModificationDate {
            get {
                return base._modificationDate;
            }
        }

        public IDictionary<long, object> Properties {
            get {
                return base._properties;
            }
        }
        #endregion
  
        #region private methods
        
        private IGraphElementModel GetElement (long id)
        {
            IGraphElementModel result;
            
            _graphElements.TryGetValue (id, out result);
            
            return result;
        }
        
        #endregion
    }
}

