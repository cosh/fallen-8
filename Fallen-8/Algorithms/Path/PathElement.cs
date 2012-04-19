// 
// PathElement.cs
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
using Fallen8.API.Model;

namespace Fallen8.API.Algorithms.Path
{
    /// <summary>
    ///   The element of a path
    /// </summary>
    public class PathElement
    {
        #region Data

        private VertexModel _sourceVertex;

        /// <summary>
        ///   The source vertex
        /// </summary>
        public VertexModel SourceVertex
        {
            get
            {
                return _sourceVertex ??
                       (_sourceVertex = Direction == Direction.IncomingEdge ? Edge.TargetVertex : Edge.SourceVertex);
            }
        }

        private VertexModel _targetVertex;

        /// <summary>
        ///   The target vertex
        /// </summary>
        public VertexModel TargetVertex
        {
            get
            {
                return _targetVertex ??
                       (_targetVertex = Direction == Direction.IncomingEdge ? Edge.SourceVertex : Edge.TargetVertex);
            }
        }

        /// <summary>
        ///   The edge.
        /// </summary>
        public EdgeModel Edge { get; private set; }

        /// <summary>
        ///   The edge property identifier.
        /// </summary>
        public UInt16 EdgePropertyId { get; private set; }

        /// <summary>
        ///   Direction.
        /// </summary>
        public Direction Direction { get; set; }

        /// <summary>
        ///   The weight of this path element
        /// </summary>
        public Double Weight { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        ///   Creates a new path element
        /// </summary>
        /// <param name="edge"> The edge. </param>
        /// <param name="edgePropertyId"> The edge property identifier. </param>
        /// <param name="direction"> The direction. </param>
        /// <param name="weight"> The weight. </param>
        public PathElement(EdgeModel edge, UInt16 edgePropertyId, Direction direction, Double weight = 0.0)
        {
            Edge = edge;
            EdgePropertyId = edgePropertyId;
            Direction = direction;
            Weight = weight;
            _sourceVertex = null;
            _targetVertex = null;
        }

        #endregion

        #region public methods

        /// <summary>
        ///   Calculates the weight of this path element
        /// </summary>
        /// <param name="vertexCost"> The vertex cost delegate. </param>
        /// <param name="edgeCost"> The edge cost delegate </param>
        public void CalculateWeight(PathDelegates.VertexCost vertexCost, PathDelegates.EdgeCost edgeCost)
        {
            Weight = 0;

            if (vertexCost != null)
            {
                Weight = vertexCost(TargetVertex);
            }

            if (edgeCost != null)
            {
                Weight += edgeCost(Edge);
            }
        }

        #endregion

        #region Equals Overrides

        public override Boolean Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to PathElement return false.
            var p = obj as PathElement;

            return p != null && Equals(p);
        }

        public Boolean Equals(PathElement p)
        {
            // If parameter is null return false:
            if ((object) p == null)
            {
                return false;
            }

            return ReferenceEquals(this.Edge, p.Edge) && this.Direction == p.Direction;
        }

        public static Boolean operator ==(PathElement a, PathElement b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(PathElement a, PathElement b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.Edge.GetHashCode();
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("{0}{1}{2}", SourceVertex.Id, Direction == Direction.IncomingEdge ? "<-" : "->",
                                 TargetVertex.Id);
        }

        #endregion
    }
}