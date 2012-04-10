// 
// PathHelper.cs
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
    ///   A static helper class for path algorithms
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        ///   Get the valid edges of a vertex
        /// </summary>
        /// <param name="vertex"> The vertex. </param>
        /// <param name="direction"> The direction. </param>
        /// <param name="edgepropertyFilter"> The edge property filter. </param>
        /// <param name="edgeFilter"> The edge filter. </param>
        /// <returns> Valid edges </returns>
        public static List<Tuple<UInt16, IEnumerable<EdgeModel>>> GetValidEdges(VertexModel vertex, Direction direction,
                                                                                PathDelegates.EdgePropertyFilter
                                                                                    edgepropertyFilter,
                                                                                PathDelegates.EdgeFilter edgeFilter)
        {
            var edgeProperties = direction == Direction.IncomingEdge
                                     ? vertex.GetIncomingEdges()
                                     : vertex.GetOutgoingEdges();
            var result = new List<Tuple<ushort, IEnumerable<EdgeModel>>>();

            if (edgeProperties != null)
            {
                foreach (var edgeContainer in edgeProperties)
                {
                    if (edgepropertyFilter != null && !edgepropertyFilter(edgeContainer.EdgePropertyId, direction))
                    {
                        continue;
                    }

                    if (edgeFilter != null)
                    {
                        var validEdges = new List<EdgeModel>();

                        foreach (var aEdge in edgeContainer.Edges)
                        {
                            if (edgeFilter(aEdge, direction))
                            {
                                validEdges.Add(aEdge);
                            }
                        }
                        result.Add(new Tuple<ushort, IEnumerable<EdgeModel>>(edgeContainer.EdgePropertyId, validEdges));
                    }
                    else
                    {
                        result.Add(new Tuple<ushort, IEnumerable<EdgeModel>>(edgeContainer.EdgePropertyId,
                                                                             edgeContainer.Edges));
                    }
                }
            }

            return result;
        }
    }
}