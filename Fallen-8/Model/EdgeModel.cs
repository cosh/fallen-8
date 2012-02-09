// 
// EdgeModel.cs
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
using Framework.Serialization;

namespace Fallen8.API.Model
{
    /// <summary>
    /// Edge model.
    /// </summary>
    public sealed class EdgeModel : AGraphElement
    {
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeModel"/> class.
        /// </summary>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        /// <param name='creationDate'>
        /// Creation date.
        /// </param>
        /// <param name='targetVertex'>
        /// Target vertex.
        /// </param>
        /// <param name='sourceEdgeProperty'>
        /// Source edge property.
        /// </param>
        /// <param name='properties'>
        /// Properties.
        /// </param>
        public EdgeModel(Int32 id, DateTime creationDate, VertexModel targetVertex, VertexModel sourceVertex, List<PropertyContainer> properties)
            : base(id, creationDate, properties)
        {
            TargetVertex = targetVertex;
            SourceVertex = sourceVertex;
        }
        
        #endregion
        
        #region IEdgeModel implementation

        /// <summary>
        /// The target vertex.
        /// </summary>
        public readonly VertexModel TargetVertex;

        /// <summary>
        /// The source vertex.
        /// </summary>
        public readonly VertexModel SourceVertex;
        
        #endregion

        #region Equals Overrides

        public override Boolean Equals (Object obj)
        {
            // If parameter is null return false.
            if (obj == null) {
                return false;
            }

            // If parameter cannot be cast to IEdgeModel return false.
            var p = obj as EdgeModel;

            return p != null && Equals (p);
        }

        public Boolean Equals (EdgeModel p)
        {
            // If parameter is null return false:
            if ((object)p == null) {
                return false;
            }

            return TargetVertex.Id == p.TargetVertex.Id
                   && (SourceVertex.Id == p.SourceVertex.Id);
        }

        public static Boolean operator == (EdgeModel a, EdgeModel b)
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

        public static Boolean operator != (EdgeModel a, EdgeModel b)
        {
            return !(a == b);
        }

        public override int GetHashCode ()
        {
            return TargetVertex.GetHashCode () ^ SourceVertex.GetHashCode ();
        }

        #endregion
        
        #region internal methods
        
        /// <summary>
        /// Writes the edge.
        /// </summary>
        /// <param name='writer'>
        /// Writer.
        /// </param>
        internal void Save (SerializationWriter writer)
        {
            writer.WriteOptimized(0);//0 for edge
            base.SaveGraphElement(writer);
            writer.WriteOptimized(SourceVertex.Id);
            writer.WriteOptimized(TargetVertex.Id);
        }
        
        #endregion
    }
}
