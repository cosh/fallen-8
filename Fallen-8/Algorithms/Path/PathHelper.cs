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
using System.Collections.ObjectModel;
using Fallen8.API.Model;
namespace Fallen8.API.Algorithms.Path
{
    /// <summary>
    /// A static helper class for path algorithms
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Gets all relevant path elements from a vertex
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="edgepropertyFilter">The edge property filter.</param>
        /// <param name="edgeFilter">The edge filter</param>
        /// <param name="adjacentVertexFilter">The adjacent vertex filter</param>
        /// <param name="visitedEdges">The already visited edges</param>
        /// <returns>Enumerable of path elements</returns>
        public static IEnumerable<PathElement> GetAllRelevantPathElements(VertexModel vertex, PathDelegates.EdgePropertyFilter edgepropertyFilter, PathDelegates.EdgeFilter edgeFilter, PathDelegates.AdjacentVertexFilter adjacentVertexFilter, HashSet<PathElement> visitedEdges)
        {
            #region data

            ReadOnlyCollection<EdgeModel> edges;
            EdgeModel edge;
            ReadOnlyCollection<EdgeContainer> edgePropertys;
            EdgeContainer edgeProperty;
            var result = new List<PathElement>();

            #endregion

            #region Outgoing edges

            edgePropertys = vertex.GetOutgoingEdges();
            if (edgePropertys != null)
            {
                for (var i = 0; i < edgePropertys.Count; i++)
                {
                    edgeProperty = edgePropertys[i];

                    if (edgepropertyFilter != null &&
                        !edgepropertyFilter(edgeProperty.EdgePropertyId, Direction.OutgoingEdge))
                    {
                        continue;
                    }

                    for (var j = 0; j < edgeProperty.Edges.Count; j++)
                    {
                        edge = edgeProperty.Edges[j];

                        if (edgeFilter != null && !edgeFilter(edge, edgeProperty.EdgePropertyId, Direction.OutgoingEdge))
                        {
                            continue;
                        }

                        if (adjacentVertexFilter != null && !adjacentVertexFilter(edge.TargetVertex))
                        {
                            continue;
                        }

                        var pathElement = new PathElement(edge, edgeProperty.EdgePropertyId, Direction.OutgoingEdge);

                        if (!visitedEdges.Contains(pathElement))
                        {
                            result.Add(pathElement);
                        }
                    }
                }    
            }
            
            #endregion

            #region Incoming edges

            edgePropertys = vertex.GetIncomingEdges();
            if (edgePropertys != null)
            {
                for (var i = 0; i < edgePropertys.Count; i++)
                {
                    edgeProperty = edgePropertys[i];

                    if (edgepropertyFilter != null &&
                        !edgepropertyFilter(edgeProperty.EdgePropertyId, Direction.IncomingEdge))
                    {
                        continue;
                    }

                    for (var j = 0; j < edgeProperty.Edges.Count; j++)
                    {
                        edge = edgeProperty.Edges[j];

                        if (edgeFilter != null && !edgeFilter(edge, edgeProperty.EdgePropertyId, Direction.IncomingEdge))
                        {
                            continue;
                        }

                        if (adjacentVertexFilter != null && !adjacentVertexFilter(edge.SourceVertex))
                        {
                            continue;
                        }

                        var pathElement = new PathElement(edge, edgeProperty.EdgePropertyId, Direction.IncomingEdge);

                        if (!visitedEdges.Contains(pathElement))
                        {
                            result.Add(pathElement);
                        }
                    }
                }                
            }
            
            #endregion

            return result;
        }
    }
}
