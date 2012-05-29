// 
// PathDelegates.cs
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
using NoSQL.GraphDB.Model;

namespace NoSQL.GraphDB.Algorithms.Path
{
    /// <summary>
    /// Path delegates
    /// </summary>
    public static class PathDelegates
    {
        /// <summary>
        /// Filter for edge properties
        /// </summary>
        /// <param name="edgePropertyId">The edge property identifier.</param>
        /// <param name="direction">The direction of the edge.</param>
        /// <returns>False will filter the edge property</returns>
        public delegate bool EdgePropertyFilter(UInt16 edgePropertyId, Direction direction);

        /// <summary>
        /// Filter for edges
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <param name="direction">The direction of the edge.</param>
        /// <returns>False will filter the edge</returns>
        public delegate bool EdgeFilter(EdgeModel edge, Direction direction);
       
        /// <summary>
        /// Sets the cost of for the edge
        /// </summary>
        /// <param name="edge">The edge</param>
        /// <returns>The order key.</returns>
        public delegate double EdgeCost(EdgeModel edge);

        /// <summary>
        /// Sets the cost of for the vertex
        /// </summary>
        /// <param name="vertex">The vertex</param>
        public delegate double VertexCost(VertexModel vertex);
    }
}
