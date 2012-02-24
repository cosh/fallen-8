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
    /// The element of a path
    /// </summary>
    public struct PathElement
    {
        #region Data

        /// <summary>
        /// The edge.
        /// </summary>
        public EdgeModel Edge { get; set; }

        /// <summary>
        /// The edge property identifier.
        /// </summary>
        public UInt16 EdgePropertyId { get; set; }

        /// <summary>
        /// Direction.
        /// </summary>
        public Direction Direction { get; set; }

        /// <summary>
        /// The weight of this path element
        /// </summary>
        public Double Weight { get; set; }

        #endregion

        #region public methods

        /// <summary>
        /// Gets the target vertex of this path element
        /// </summary>
        /// <returns>Vertex.</returns>
        public VertexModel GetTargetVertex()
        {
            return Direction == Direction.IncomingEdge ? Edge.SourceVertex : Edge.TargetVertex;
        }

        #endregion
    }
}
