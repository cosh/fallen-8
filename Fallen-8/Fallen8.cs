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
using System.Threading;

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
        private readonly GraphModel _model;
        
        /// <summary>
        /// The index factory.
        /// </summary>
        private readonly Fallen8IndexFactory _indexFactory;
        
        /// <summary>
        /// The current identifier.
        /// </summary>
        private Int64 _currentId;
        
        #endregion
        
        #region IFallen8 implementation
        public IGraphModel Graph {
            get {
                return _model;
            }
        }

        public IFallen8IndexFactory IndexProvider {
            get {
                return _indexFactory;
            }
        }
        #endregion

        #region IFallen8Write implementation
        public IVertexModel CreateVertex (VertexModelDefinition vertexDefinition)
        {
            //create the new vertex
            VertexModel newVertex = new VertexModel (Interlocked.Increment (ref _currentId), vertexDefinition.CreationDate, vertexDefinition.Properties);
            
            _model.Graphelements.TryAdd(newVertex.Id, newVertex);
            
            if (vertexDefinition.Edges != null && vertexDefinition.Edges.Count > 0) {
                
                Dictionary<long, EdgePropertyModel> outEdges = new Dictionary<long, EdgePropertyModel> ();
                
                foreach (var aEdge in vertexDefinition.Edges) {
                    outEdges.Add (aEdge.Key, CreateEdgeProperty (aEdge.Key, aEdge.Value, newVertex));
                }
                
                newVertex.AddOutEdges (outEdges);
            }
            
            return newVertex;
        }

        public IEdgeModel CreateEdge (long sourceVertexId, long edgePropertyId, EdgeModelDefinition edgeDefinition)
        {
            //get the related vertices
            var sourceVertex = (VertexModel)_model.Graphelements [sourceVertexId];
            var targetVertex = (VertexModel)_model.Graphelements [edgeDefinition.TargetVertexId];
            
            //build the necessary edge contruct
            var edgeProperty = new EdgePropertyModel (sourceVertex, null);
            var outgoingEdge = new EdgeModel (Interlocked.Increment (ref _currentId), edgeDefinition.CreationDate, targetVertex, edgeProperty, edgeDefinition.Properties);
            edgeProperty.AddEdge (outgoingEdge);
            
            //link the vertices
            sourceVertex.AddOutEdge (edgePropertyId, outgoingEdge);
            targetVertex.AddIncomingEdge (edgePropertyId, outgoingEdge);
            
            return outgoingEdge;
        }

        public bool TryAddProperty (long graphElementId, long propertyId, IComparable property)
        {
            throw new NotImplementedException ();
        }

        public bool TryRemoveProperty (long graphElementId, long propertyId)
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
  
        #region private helper methods
        
        /// <summary>
        /// Creates the edge property.
        /// </summary>
        /// <returns>
        /// The edge property.
        /// </returns>
        /// <param name='edgePropertyId'>
        /// Edge property identifier.
        /// </param>
        /// <param name='edgeDefinitions'>
        /// Edge definitions.
        /// </param>
        /// <param name='sourceVertex'>
        /// New vertex.
        /// </param>
        private EdgePropertyModel CreateEdgeProperty (Int64 edgePropertyId, List<EdgeModelDefinition> edgeDefinitions, VertexModel sourceVertex)
        {
            List<IEdgeModel> edges = new List<IEdgeModel> ();
            
            EdgePropertyModel result = new EdgePropertyModel (sourceVertex, edges);
            
            foreach (var aEdgeDefinition in edgeDefinitions) {
                
                var targetVertex = (VertexModel)_model.Graphelements [aEdgeDefinition.TargetVertexId];
                
                var outgoingEdge = new EdgeModel (Interlocked.Increment (ref _currentId), aEdgeDefinition.CreationDate, targetVertex, result, aEdgeDefinition.Properties);
                
                targetVertex.AddIncomingEdge (edgePropertyId, outgoingEdge);
                
                edges.Add (outgoingEdge);
            }
            
            return result;
        }
        
        #endregion
	}
}

