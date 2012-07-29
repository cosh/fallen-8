// 
// BidirectionalLevelSynchronousSSSP.cs
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

using NoSQL.GraphDB.Helper;

#region Usings

using NoSQL.GraphDB.Model;
using System;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace NoSQL.GraphDB.Algorithms.Path
{
    /// <summary>
    ///   Bidirctional level synchronous SSSP algorithm
    /// </summary>
    public sealed class BidirectionalLevelSynchronousSSSP : IShortestPathAlgorithm
    {
        #region Data

        /// <summary>
        ///   The Fallen-8
        /// </summary>
        private Fallen8 _fallen8;

        #endregion

        #region IShortestPathAlgorithm Members

        public List<Path> Calculate(
            int sourceVertexId,
            int destinationVertexId,
            Int32 maxDepth = 1,
            Double maxPathWeight = Double.MaxValue,
            Int32 maxResults = 1,
            PathDelegates.EdgePropertyFilter edgePropertyFilter = null,
            PathDelegates.VertexFilter vertexFilter = null,
            PathDelegates.EdgeFilter edgeFilter = null,
            PathDelegates.EdgeCost edgeCost = null,
            PathDelegates.VertexCost vertexCost = null)
        {
            #region initial checks

            VertexModel sourceVertex;
            VertexModel targetVertex;
            if (!(_fallen8.TryGetVertex(out sourceVertex, sourceVertexId)
                  && _fallen8.TryGetVertex(out targetVertex, destinationVertexId)))
            {
                return null;
            }

            if (maxDepth == 0 || maxResults == 0 || maxResults <= 0)
            {
                return null;
            }

            if (ReferenceEquals(sourceVertex, targetVertex))
            {
                return null;
            }

            #endregion

            #region data

            var sourceVisitedVertices = new BigBitArray();
            sourceVisitedVertices.SetValue(sourceVertex.Id, true);

            var targetVisitedVertices = new BigBitArray();
            targetVisitedVertices.SetValue(targetVertex.Id, true);

            #endregion

            #region maxdepth = 1

            if (maxDepth == 1)
            {
                var depthOneFrontier = GetGlobalFrontier(new List<VertexModel> { sourceVertex }, sourceVisitedVertices, edgePropertyFilter, edgeFilter, vertexFilter);
            }

            #endregion

            #region maxdepth > 1

            //find the middle element  s-->m-->t
            Int32 sourceLevel = 0;
            Int32 targetLevel = 0;

            var sourceFrontiers = new List<Dictionary<VertexModel, VertexPredecessor>>();
            var targetFrontiers = new List<Dictionary<VertexModel, VertexPredecessor>>();
            Dictionary<VertexModel, VertexPredecessor> currentSourceFrontier = null;
            Dictionary<VertexModel, VertexPredecessor> currentTargetFrontier = null;
            IEnumerable<VertexModel> currentSourceVertices = new List<VertexModel> { sourceVertex };
            IEnumerable<VertexModel> currentTargetVertices = new List<VertexModel> { targetVertex };

            List<VertexModel> middleVertices = null;

            do
            {
                #region calculate frontier
                
                #region source --> target

                currentSourceFrontier = GetGlobalFrontier(currentSourceVertices, sourceVisitedVertices, edgePropertyFilter, edgeFilter, vertexFilter);
                sourceFrontiers.Add(currentSourceFrontier);
                currentSourceVertices = sourceFrontiers[sourceLevel].Keys;
                sourceLevel++;

                if (currentSourceFrontier.ContainsKey(targetVertex))
                {
                    if (middleVertices == null)
                    {
                        middleVertices = new List<VertexModel>{targetVertex};
                    }
                    else
                    {
                        middleVertices.Add(targetVertex);                        
                    }
                    break;
                }
                if (FindMiddleVertices(out middleVertices, currentSourceFrontier, currentTargetFrontier)) break;
                if ((sourceLevel + targetLevel) == maxDepth) break;

                #endregion

                #region target --> source

                currentTargetFrontier = GetGlobalFrontier(currentTargetVertices, targetVisitedVertices, edgePropertyFilter, edgeFilter, vertexFilter);
                targetFrontiers.Add(currentTargetFrontier);
                currentTargetVertices = targetFrontiers[targetLevel].Keys;
                targetLevel++;


                if (currentTargetFrontier.ContainsKey(sourceVertex))
                {
                    if (middleVertices == null)
                    {
                        middleVertices = new List<VertexModel> { sourceVertex };
                    }
                    else
                    {
                        middleVertices.Add(sourceVertex);
                    }
                    break;
                }
                if (FindMiddleVertices(out middleVertices, currentSourceFrontier, currentTargetFrontier)) break;
                if ((sourceLevel + targetLevel) == maxDepth) break;

                #endregion

                #endregion

            } while (true);

            return middleVertices != null 
                ? CreatePaths(middleVertices, sourceFrontiers, targetFrontiers, maxResults, sourceLevel, targetLevel) 
                : null;

            #endregion
        }

        #endregion

        #region private helper

        /// <summary>
        /// Finds the middle vertices of two given frontiers
        /// </summary>
        /// <param name="middleVertices">The result</param>
        /// <param name="currentSourceFrontier">The source frontier</param>
        /// <param name="currentTargetFrontier">The target frontier</param>
        /// <returns>True if there are middle vertices, otherwise false</returns>
        private static bool FindMiddleVertices(
            out List<VertexModel> middleVertices, 
            Dictionary<VertexModel, VertexPredecessor> currentSourceFrontier, 
            Dictionary<VertexModel, VertexPredecessor> currentTargetFrontier)
        {
            if (currentSourceFrontier == null || currentTargetFrontier == null)
            {
                middleVertices = null;
                return false;
            }

            middleVertices = currentSourceFrontier.Keys.Intersect(currentTargetFrontier.Keys).ToList();
            middleVertices = middleVertices.Count > 0 ? middleVertices : null;

            return middleVertices != null;
        }

        /// <summary>
        /// Creates the paths
        /// </summary>
        /// <param name="middleVertices">The middle vertices of the path</param>
        /// <param name="sourceFrontiers">The source frontier</param>
        /// <param name="targetFrontiers">The target frontier</param>
        /// <param name="maxResults">The maximum number of paths in result</param>
        /// <param name="sourceLevel">The source level</param>
        /// <param name="targetLevel">The target level</param>
        /// <returns>A list of paths</returns>
        private static List<Path> CreatePaths(
            List<VertexModel> middleVertices,
            List<Dictionary<VertexModel, VertexPredecessor>> sourceFrontiers,
            List<Dictionary<VertexModel, VertexPredecessor>> targetFrontiers,
            int maxResults,
            Int32 sourceLevel,
            Int32 targetLevel)
        {
            if (middleVertices != null && middleVertices.Count > 0)
            {
                var result = new List<Path>();

                #region middle --> source

                var middleToSourcePaths = new List<Path>();

                var previousSourceLevel = sourceLevel - 1;
                if (previousSourceLevel == 0)
                {
                    //source must be pred now
                    for (var i = 0; i < middleVertices.Count; i++)
                    {
                        var middleVertex = middleVertices[i];
                        var pred = sourceFrontiers[previousSourceLevel][middleVertex];

                        middleToSourcePaths.AddRange(
                            pred.Incoming.Select(
                                edgeLocation =>
                                new Path(new PathElement(edgeLocation.Edge, edgeLocation.EdgePropertyId,
                                                         Direction.IncomingEdge))));
                        middleToSourcePaths.AddRange(
                            pred.Outgoing.Select(
                                edgeLocation =>
                                new Path(new PathElement(edgeLocation.Edge, edgeLocation.EdgePropertyId,
                                                         Direction.OutgoingEdge))));
                    }
                }
                else
                {
                    //recursion
                    middleToSourcePaths = CreateToSourcePaths(middleVertices, sourceFrontiers, sourceLevel);
                }


                //they have to be in reverse order because we went backward
                middleToSourcePaths.ForEach(_ => _.ReversePath());

                #endregion

                #region middle --> target

                var previousTargetLevel = targetLevel - 1;
                switch (previousTargetLevel)
                {
                    case -1:
                        //the target vertex located in the middle vertices
                        //nothing to do
                        break;

                    case 0:
                        //target is direct pred
                        for (var i = 0; i < middleToSourcePaths.Count; i++)
                        {
                            var middlePath = middleToSourcePaths[i];
                            var pred = targetFrontiers[previousTargetLevel][middlePath.LastPathElement.TargetVertex];
                            result.AddRange(
                            pred.Incoming.Select(
                                edgeLocation =>
                                new Path(middlePath, new PathElement(edgeLocation.Edge, edgeLocation.EdgePropertyId,
                                                         Direction.OutgoingEdge))));
                            result.AddRange(
                                pred.Outgoing.Select(
                                    edgeLocation =>
                                    new Path(middlePath, new PathElement(edgeLocation.Edge, edgeLocation.EdgePropertyId,
                                                             Direction.IncomingEdge))));
                        }

                        break;

                    default:
                        //recursion
                        result = CreatePathsRecusive(middleToSourcePaths, targetFrontiers, targetLevel, Direction.OutgoingEdge, Direction.IncomingEdge);

                        break;

                }

                #endregion

                return result.Take(maxResults).ToList();
            }

            return null;
        }

        /// <summary>
        /// Creates the paths from the middle vertices to the source
        /// </summary>
        /// <param name="middleVertices">The middle vertices</param>
        /// <param name="sourceFrontiers">The source frontier</param>
        /// <param name="sourceLevel">The source level</param>
        /// <returns>The list of paths from the source to the middle vertices in reverse order</returns>
        private static List<Path> CreateToSourcePaths(List<VertexModel> middleVertices, List<Dictionary<VertexModel, VertexPredecessor>> sourceFrontiers, int sourceLevel)
        {
            var firstPaths = new List<Path>();
            //source must be pred now
            for (var i = 0; i < middleVertices.Count; i++)
            {
                var middleVertex = middleVertices[i];
                var pred = sourceFrontiers[sourceLevel][middleVertex];

                firstPaths.AddRange(
                    pred.Incoming.Select(
                        edgeLocation =>
                        new Path(new PathElement(edgeLocation.Edge, edgeLocation.EdgePropertyId,
                                                 Direction.IncomingEdge))));
                firstPaths.AddRange(
                    pred.Outgoing.Select(
                        edgeLocation =>
                        new Path(new PathElement(edgeLocation.Edge, edgeLocation.EdgePropertyId,
                                                 Direction.OutgoingEdge))));
            }

            if (sourceLevel == 0)
            {
                return firstPaths;
            }

            var newSourceLevel = sourceLevel - 1;

            return CreatePathsRecusive(firstPaths, sourceFrontiers, newSourceLevel, Direction.IncomingEdge, Direction.OutgoingEdge);
        }

        /// <summary>
        /// Creates paths in a recursive way
        /// </summary>
        /// <param name="currentPaths">The paths to start from</param>
        /// <param name="frontier">The frontier to walk on</param>
        /// <param name="level">The current level</param>
        /// <param name="incomingPredDirection">The direction of the incoming predecessors (depends on the frontier)</param>
        /// <param name="outgoingPredDirection">The direction of the outgoing predecessors (depends on the frontier)</param>
        /// <returns>List of paths</returns>
        private static List<Path> CreatePathsRecusive(List<Path> currentPaths, List<Dictionary<VertexModel, VertexPredecessor>> frontier, int level, Direction incomingPredDirection, Direction outgoingPredDirection)
        {
            var result = new List<Path>();

            switch (level)
            {
                case -1:
                    return currentPaths;

                case 0:
                    //target is direct pred
                    for (var i = 0; i < currentPaths.Count; i++)
                    {
                        var middlePath = currentPaths[i];
                        var pred = frontier[level][middlePath.LastPathElement.TargetVertex];
                        result.AddRange(
                        pred.Incoming.Select(
                            edgeLocation =>
                            new Path(middlePath, new PathElement(edgeLocation.Edge, edgeLocation.EdgePropertyId,
                                                     incomingPredDirection))));
                        result.AddRange(
                            pred.Outgoing.Select(
                                edgeLocation =>
                                new Path(middlePath, new PathElement(edgeLocation.Edge, edgeLocation.EdgePropertyId,
                                                         outgoingPredDirection))));
                    }
                    break;

                default:

                    var newMiddlePaths = new List<Path>();

                    for (var i = 0; i < currentPaths.Count; i++)
                    {
                        var middlePath = currentPaths[i];
                        var pred = frontier[level][middlePath.LastPathElement.TargetVertex];
                        newMiddlePaths.AddRange(
                        pred.Incoming.Select(
                            edgeLocation =>
                            new Path(middlePath, new PathElement(edgeLocation.Edge, edgeLocation.EdgePropertyId,
                                                     incomingPredDirection))));
                        newMiddlePaths.AddRange(
                            pred.Outgoing.Select(
                                edgeLocation =>
                                new Path(middlePath, new PathElement(edgeLocation.Edge, edgeLocation.EdgePropertyId,
                                                         outgoingPredDirection))));
                    }

                    var newPredLevel = level - 1;

                    result = CreatePathsRecusive(newMiddlePaths, frontier, newPredLevel, incomingPredDirection, outgoingPredDirection);

                    break;
            }

            return result;
        }

        
        /// <summary>
        /// Gets the global frontier corresponding to a certain level
        /// </summary>
        /// <param name="startingVertices">The starting vertices behind the frontier</param>
        /// <param name="visitedVertices">The visited vertices corresponding to the frontier</param>
        /// <param name="edgepropertyFilter">The edge property filter</param>
        /// <param name="edgeFilter">The edge filter</param>
        /// <param name="vertexFilter">The vertex filter</param>
        /// <returns>The frontier vertices and their predecessors</returns>
        private static Dictionary<VertexModel, VertexPredecessor> GetGlobalFrontier(IEnumerable<VertexModel> startingVertices, BigBitArray visitedVertices, 
            PathDelegates.EdgePropertyFilter edgepropertyFilter,
            PathDelegates.EdgeFilter edgeFilter,
            PathDelegates.VertexFilter vertexFilter)
        {
            var frontier = new Dictionary<VertexModel, VertexPredecessor>();

            foreach (var aKv in startingVertices)
            {
                foreach (var aFrontierElement in GetLocalFrontier(aKv, visitedVertices, edgepropertyFilter, edgeFilter, vertexFilter))
                {
                    VertexPredecessor pred;
                    if (frontier.TryGetValue(aFrontierElement.FrontierVertex, out pred))
                    {
                        switch (aFrontierElement.EdgeDirection)
                        {
                            case Direction.IncomingEdge:
                                pred.Incoming.Add(aFrontierElement.EdgeLocation);
                                break;

                            case Direction.OutgoingEdge:
                                pred.Outgoing.Add(aFrontierElement.EdgeLocation);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        pred = new VertexPredecessor();
                        switch (aFrontierElement.EdgeDirection)
                        {
                            case Direction.IncomingEdge:
                                pred.Incoming.Add(aFrontierElement.EdgeLocation);
                                break;

                            case Direction.OutgoingEdge:
                                pred.Outgoing.Add(aFrontierElement.EdgeLocation);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        frontier.Add(aFrontierElement.FrontierVertex, pred);
                    }
                }
            }

            return frontier;
        }

        /// <summary>
        /// Gets the frontier elements on an incoming edge
        /// </summary>
        /// <param name="vertex">The vertex behind the frontier</param>
        /// <param name="edgepropertyFilter">The edge property filter</param>
        /// <param name="edgeFilter">The edge filter</param>
        /// <param name="vertexFilter">The vertex filter</param>
        /// <param name="alreadyVisited">The vertices that have been visited already</param>
        /// <returns>A number of frontier elements</returns>
        private static IEnumerable<FrontierElement> GetValidIncomingEdges(
            VertexModel vertex,
            PathDelegates.EdgePropertyFilter edgepropertyFilter,
            PathDelegates.EdgeFilter edgeFilter,
            PathDelegates.VertexFilter vertexFilter,
            BigBitArray alreadyVisited)
        {
            var edgeProperties = vertex.GetIncomingEdges();
            var result = new List<FrontierElement>();

            if (edgeProperties != null)
            {
                foreach (var edgeContainer in edgeProperties)
                {
                    if (edgepropertyFilter != null && !edgepropertyFilter(edgeContainer.EdgePropertyId, Direction.IncomingEdge))
                    {
                        continue;
                    }

                    if (edgeFilter != null)
                    {
                        for (var i = 0; i < edgeContainer.Edges.Count; i++)
                        {
                            var aEdge = edgeContainer.Edges[i];
                            if (edgeFilter(aEdge, Direction.IncomingEdge))
                            {
                                if (alreadyVisited.SetValue(aEdge.SourceVertex.Id, true))
                                {
                                    if (vertexFilter != null)
                                    {
                                        if (vertexFilter(aEdge.SourceVertex))
                                        {
                                            result.Add(new FrontierElement
                                                           {
                                                               EdgeDirection = Direction.IncomingEdge,
                                                               EdgeLocation = new EdgeLocation
                                                                                  {
                                                                                      Edge = aEdge,
                                                                                      EdgePropertyId =
                                                                                          edgeContainer.EdgePropertyId
                                                                                  },
                                                               FrontierVertex = aEdge.SourceVertex
                                                           });
                                        }
                                    }
                                    else
                                    {
                                        result.Add(new FrontierElement
                                                       {
                                                           EdgeDirection = Direction.IncomingEdge,
                                                           EdgeLocation = new EdgeLocation
                                                                              {
                                                                                  Edge = aEdge,
                                                                                  EdgePropertyId =
                                                                                      edgeContainer.EdgePropertyId
                                                                              },
                                                           FrontierVertex = aEdge.SourceVertex
                                                       });
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (vertexFilter != null)
                        {
                            for (var i = 0; i < edgeContainer.Edges.Count; i++)
                            {
                                var aEdge = edgeContainer.Edges[i];

                                if (alreadyVisited.SetValue(aEdge.SourceVertex.Id, true))
                                {
                                    if (vertexFilter(aEdge.SourceVertex))
                                    {
                                        result.Add(new FrontierElement
                                                       {
                                                           EdgeDirection = Direction.IncomingEdge,
                                                           EdgeLocation = new EdgeLocation
                                                                              {
                                                                                  Edge = aEdge,
                                                                                  EdgePropertyId =
                                                                                      edgeContainer.EdgePropertyId
                                                                              },
                                                           FrontierVertex = aEdge.SourceVertex
                                                       });
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (var i = 0; i < edgeContainer.Edges.Count; i++)
                            {
                                var aEdge = edgeContainer.Edges[i];
                                if (alreadyVisited.SetValue(aEdge.SourceVertex.Id, true))
                                {
                                    result.Add(new FrontierElement
                                                   {
                                                       EdgeDirection = Direction.IncomingEdge,
                                                       EdgeLocation = new EdgeLocation
                                                                          {
                                                                              Edge = aEdge,
                                                                              EdgePropertyId =
                                                                                  edgeContainer.EdgePropertyId
                                                                          },
                                                       FrontierVertex = aEdge.SourceVertex
                                                   });
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the frontier elements on an outgoing edge
        /// </summary>
        /// <param name="vertex">The vertex behind the frontier</param>
        /// <param name="edgepropertyFilter">The edge property filter</param>
        /// <param name="edgeFilter">The edge filter</param>
        /// <param name="vertexFilter">The vertex filter</param>
        /// <param name="alreadyVisited">The vertices that have been visited already</param>
        /// <returns>A number of frontier elements</returns>
        private static IEnumerable<FrontierElement> GetValidOutgoingEdges(
            VertexModel vertex,
            PathDelegates.EdgePropertyFilter edgepropertyFilter,
            PathDelegates.EdgeFilter edgeFilter,
            PathDelegates.VertexFilter vertexFilter,
            BigBitArray alreadyVisited)
        {
            var edgeProperties = vertex.GetOutgoingEdges();
            var result = new List<FrontierElement>();

            if (edgeProperties != null)
            {
                foreach (var edgeContainer in edgeProperties)
                {
                    if (edgepropertyFilter != null && !edgepropertyFilter(edgeContainer.EdgePropertyId, Direction.OutgoingEdge))
                    {
                        continue;
                    }

                    if (edgeFilter != null)
                    {
                        for (var i = 0; i < edgeContainer.Edges.Count; i++)
                        {
                            var aEdge = edgeContainer.Edges[i];
                            if (edgeFilter(aEdge, Direction.OutgoingEdge))
                            {
                                if (alreadyVisited.SetValue(aEdge.TargetVertex.Id, true))
                                {
                                    if (vertexFilter != null)
                                    {
                                        if (vertexFilter(aEdge.TargetVertex))
                                        {
                                            result.Add(new FrontierElement
                                            {
                                                EdgeDirection = Direction.OutgoingEdge,
                                                EdgeLocation = new EdgeLocation
                                                {
                                                    Edge = aEdge,
                                                    EdgePropertyId =
                                                        edgeContainer.EdgePropertyId
                                                },
                                                FrontierVertex = aEdge.TargetVertex
                                            });
                                        }
                                    }
                                    else
                                    {
                                        result.Add(new FrontierElement
                                        {
                                            EdgeDirection = Direction.OutgoingEdge,
                                            EdgeLocation = new EdgeLocation
                                            {
                                                Edge = aEdge,
                                                EdgePropertyId =
                                                    edgeContainer.EdgePropertyId
                                            },
                                            FrontierVertex = aEdge.TargetVertex
                                        });
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (vertexFilter != null)
                        {
                            for (var i = 0; i < edgeContainer.Edges.Count; i++)
                            {
                                var aEdge = edgeContainer.Edges[i];

                                if (alreadyVisited.SetValue(aEdge.TargetVertex.Id, true))
                                {
                                    if (vertexFilter(aEdge.TargetVertex))
                                    {
                                        result.Add(new FrontierElement
                                        {
                                            EdgeDirection = Direction.OutgoingEdge,
                                            EdgeLocation = new EdgeLocation
                                            {
                                                Edge = aEdge,
                                                EdgePropertyId =
                                                    edgeContainer.EdgePropertyId
                                            },
                                            FrontierVertex = aEdge.TargetVertex
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (var i = 0; i < edgeContainer.Edges.Count; i++)
                            {
                                var aEdge = edgeContainer.Edges[i];
                                if (alreadyVisited.SetValue(aEdge.TargetVertex.Id, true))
                                {
                                    result.Add(new FrontierElement
                                    {
                                        EdgeDirection = Direction.OutgoingEdge,
                                        EdgeLocation = new EdgeLocation
                                        {
                                            Edge = aEdge,
                                            EdgePropertyId =
                                                edgeContainer.EdgePropertyId
                                        },
                                        FrontierVertex = aEdge.TargetVertex
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the local frontier corresponding to a vertex
        /// </summary>
        /// <param name="vertex">The vertex behind the local frontier</param>
        /// <param name="alreadyVisitedVertices">The vertices that have been visited already</param>
        /// <param name="edgepropertyFilter">The edge property filter</param>
        /// <param name="edgeFilter">The edge filter</param>
        /// <param name="vertexFilter">The vertex filter</param>
        /// <returns>The local frontier</returns>
        private static IEnumerable<FrontierElement> GetLocalFrontier(VertexModel vertex, BigBitArray alreadyVisitedVertices, 
            PathDelegates.EdgePropertyFilter edgepropertyFilter,
            PathDelegates.EdgeFilter edgeFilter,
            PathDelegates.VertexFilter vertexFilter)
        {
            var result = new List<FrontierElement>();

            result.AddRange(GetValidIncomingEdges(vertex, edgepropertyFilter, edgeFilter, vertexFilter, alreadyVisitedVertices));
            result.AddRange(GetValidOutgoingEdges(vertex, edgepropertyFilter, edgeFilter, vertexFilter, alreadyVisitedVertices));

            return result;
        }
        
        #endregion

        #region IPlugin Members

        public string PluginName
        {
            get { return "BLS"; }
        }

        public Type PluginCategory
        {
            get { return typeof (IShortestPathAlgorithm); }
        }

        public string Description
        {
            get
            {
                return "Bidirectional level synchronous single source shortest path algorithm.";
            }
        }

        public string Manufacturer
        {
            get { return "Henning Rauch"; }
        }

        public void Initialize(Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            _fallen8 = fallen8;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //nothing to do atm
        }

        #endregion
    }

    internal class VertexPredecessor
    {
        public readonly List<EdgeLocation> Incoming = new List<EdgeLocation>();
        public readonly List<EdgeLocation> Outgoing = new List<EdgeLocation>();
    }

    internal class EdgeLocation
    {
        public EdgeModel Edge;
        public UInt16 EdgePropertyId;
    }

    internal class FrontierElement
    {
        public VertexModel FrontierVertex;
        public EdgeLocation EdgeLocation;
        public Direction EdgeDirection;
    }
}
