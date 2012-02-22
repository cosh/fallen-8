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
using System.Collections.Generic;
using Fallen8.API.Model;

namespace Fallen8.API.Algorithms.Path
{
    /// <summary>
    /// Path delegates
    /// </summary>
    public static class PathDelegates
    {
        /// <summary>
        /// Filter for edges
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <param name="edgePropertyId">The edge property identifier.</param>
        /// <param name="direction">The direction of the edge.</param>
        /// <returns>False will filter the edge</returns>
        public delegate bool EdgeFilter(EdgeModel edge, UInt32 edgePropertyId, Direction direction);

        /// <summary>
        /// Filter for the adjacent vertex
        /// </summary>
        /// <param name="vertex">The adjacent vertex.</param>
        /// <returns>False will filter the vertex</returns>
        public delegate bool AdjacentVertexFilter(VertexModel vertex);

        /// <summary>
        /// Sets the priority for edges
        /// </summary>
        /// <typeparam name="TOrderKey">The type of the order key.</typeparam>
        /// <param name="edge">The edge</param>
        /// <param name="edgePropertyId">The edge property identifier.</param>
        /// <param name="direction">The direction of the edge.</param>
        /// <param name="keyComparer">The comparer for the order key</param>
        /// <returns>The order key.</returns>
        public delegate TOrderKey EdgePriority<out TOrderKey>(EdgeModel edge, UInt32 edgePropertyId, Direction direction, IComparer<TOrderKey> keyComparer);
    }
}
