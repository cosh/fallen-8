// 
// Fallen8.cs
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
using Fallen8.Model;
using Fallen8.API.Index;
using Fallen8.API.Helper;
using Fallen8.API.Expression;
using System.Collections.Generic;

namespace Fallen8.API
{
    /// <summary>
    /// Fallen8.
    /// </summary>
	public sealed class Fallen8 : IFallen8
	{
        #region Data
        
        /// <summary>
        /// The model.
        /// </summary>
        private readonly IGraphModel _model;
        
        #endregion
        
        #region IFallen8 implementation
        public IGraphModel Graph {
            get {
                return _model;
            }
        }

        public IFallen8IndexFactory IndexProvider {
            get {
                throw new NotImplementedException ();
            }
        }
        #endregion

        #region IFallen8Write implementation
        public IVertexModel CreateVertex (VertexModelDefinition vertexDefinition)
        {
            throw new NotImplementedException ();
        }

        public IEdgeModel CreateEdge (long sourceVertexId, long edgePropertyId, EdgeModelDefinition edgeDefinition)
        {
            throw new NotImplementedException ();
        }

        public bool TryAddProperty (long graphElementId, long propertyId, IComparable property)
        {
            throw new NotImplementedException ();
        }

        public bool TryAddStringProperty (long graphElementId, string propertyName, object schemalessProperty)
        {
            throw new NotImplementedException ();
        }

        public bool TryRemoveProperty (long graphElementId, long propertyId)
        {
            throw new NotImplementedException ();
        }

        public bool TryRemoveStringProperty (long graphElementId, string propertyName)
        {
            throw new NotImplementedException ();
        }

        public bool TryRemoveGraphElement (long graphElementId)
        {
            throw new NotImplementedException ();
        }
        #endregion

        #region IFallen8Read implementation
        public bool Search (out IEnumerable<IGraphElementModel> result, long propertyId, IComparable literal, BinaryOperator binOp)
        {
            throw new NotImplementedException ();
        }

        public bool SearchInIndex (out IEnumerable<IGraphElementModel> result, long indexId, IComparable literal, BinaryOperator binOp)
        {
            throw new NotImplementedException ();
        }

        public bool SearchInRange (out IEnumerable<IGraphElementModel> result, long indexId, IComparable leftLimit, IComparable rightLimit, bool includeLeft, bool includeRight)
        {
            throw new NotImplementedException ();
        }

        public bool SearchFulltext (out FulltextSearchResult result, long indexId, string searchQuery)
        {
            throw new NotImplementedException ();
        }

        public bool SearchSpatial (out IEnumerable<IGraphElementModel> result, long indexId, IGeometry geometry)
        {
            throw new NotImplementedException ();
        }
        #endregion
	}
}

