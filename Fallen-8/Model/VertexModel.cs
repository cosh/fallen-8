// 
// VertexModel.cs
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
    /// Vertex model.
    /// </summary>
    public sealed class VertexModel : AGraphElement, IVertexModel
    {
        #region Data
        
        /// <summary>
        /// The out edges.
        /// </summary>
        private IDictionary<long, IEdgePropertyModel> _outEdges;
        
        /// <summary>
        /// The in edges.
        /// </summary>
        private IDictionary<long, IEnumerable<IEdgeModel>> _inEdges;
        
        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Fallen8.Model.VertexModel"/> class.
        /// </summary>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        /// <param name='creationDate'>
        /// Creation date.
        /// </param>
        public VertexModel (Int64 id, DateTime creationDate) : base (id, creationDate)
        {
        }
        
        #endregion
        
        #region IVertexModel implementation
        public IDictionary<long, IEdgePropertyModel> OutgoingEdges {
            get {
                return _outEdges;
            }
        }

        public IDictionary<long, IEnumerable<IEdgeModel>> IncomingEdges {
            get {
                return _inEdges;
            }
        }
        #endregion
  
        #region Equals Overrides

        public override Boolean Equals (Object obj)
        {
            // If parameter is null return false.
            if (obj == null) {
                return false;
            }

            // If parameter cannot be cast to VertexModel return false.
            var p = obj as VertexModel;

            return p != null && Equals (p);
        }

        public Boolean Equals (VertexModel p)
        {
            // If parameter is null return false:
            if ((object)p == null) {
                return false;
            }

            return base._id == p.Id;
        }

        public static Boolean operator == (VertexModel a, VertexModel b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals (a, b)) {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) {
                return false;
            }

            // Return true if the fields match:
            return a.Equals (b);
        }

        public static Boolean operator != (VertexModel a, VertexModel b)
        {
            return !(a == b);
        }

        public override int GetHashCode ()
        {
            return base._id.GetHashCode ();
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
    }
}
