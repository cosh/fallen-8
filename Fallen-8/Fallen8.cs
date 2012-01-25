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
using System.Linq;
using Fallen8.API.Index.Fulltext;
using Fallen8.API.Index.Spatial;
using Fallen8.API.Model;
using Fallen8.API.Index;
using Fallen8.API.Helper;
using Fallen8.API.Expression;
using System.Collections.Generic;
using System.Threading;
using Fallen8.API.Error;
using System.Collections.ObjectModel;
using Fallen8.API.Index.Range;
using System.IO;

namespace Fallen8.API
{
    /// <summary>
    /// Fallen8.
    /// </summary>
    public sealed class Fallen8 : AThreadSafeElement, IFallen8Read, IFallen8Write
	{
        #region Data
        
        /// <summary>
        /// The graph elements
        /// </summary>
        private List<AGraphElement> _graphElements;

        /// <summary>
        /// The index factory.
        /// </summary>
        public readonly IFallen8IndexFactory IndexFactory;
        
        /// <summary>
        /// The current identifier.
        /// </summary>
        private Int32 _currentId = -1;
        
        /// <summary>
        /// Binary operator delegate.
        /// </summary>
        private delegate Boolean BinaryOperatorDelegate(IComparable property, IComparable literal);

        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the Fallen8 class.
        /// </summary>
        public Fallen8 ()
        {
            IndexFactory = new Fallen8IndexFactory();
            _graphElements = new List<AGraphElement>(5000000);
            IndexFactory.Indices.Clear();
        }
        
        #endregion
        
        #region IFallen8Write implementation
  
        public Boolean TryOpen (Stream streamRepresentation)
        {
            throw new NotImplementedException ();
        }
        
        public void TabulaRasa()
        {
            if (WriteResource())
            {
                _currentId = -1;
                _graphElements = new List<AGraphElement>(5000000);
                FinishWriteResource();

                return;
            }

            throw new CollisionException();
        }

        public VertexModel CreateVertex(DateTime creationDate, IDictionary<Int32, Object> properties = null, IDictionary<Int32, List<EdgeModelDefinition>> edges = null)
        {
            if (WriteResource())
            {
                //create the new vertex
                VertexModel newVertex = new VertexModel(Interlocked.Increment(ref _currentId), creationDate, properties);

                _graphElements.Add(newVertex);

                if (edges != null && edges.Count > 0)
                {
                    var outEdges = edges.ToDictionary(aEdge => aEdge.Key,
                                                      aEdge => CreateEdgeProperty(aEdge.Key, aEdge.Value, newVertex));

                    newVertex.SetOutEdges(outEdges);
                }
                FinishWriteResource();

                return newVertex;
            }

            throw new CollisionException();
        }

        public EdgeModel CreateEdge(Int32 sourceVertexId, Int32 edgePropertyId, EdgeModelDefinition edgeDefinition)
        {
            if (WriteResource())
            {
                EdgeModel outgoingEdge;

                try
                {
                    //get the related vertices
                    var sourceVertex = (VertexModel)_graphElements[sourceVertexId];
                    var targetVertex = (VertexModel)_graphElements[edgeDefinition.TargetVertexId];

                    EdgePropertyModel epm;
                    if (sourceVertex.TryGetOutEdge(out epm, edgePropertyId))
                    {
                        outgoingEdge = new EdgeModel(Interlocked.Increment(ref _currentId), edgeDefinition.CreationDate, targetVertex, epm, edgeDefinition.Properties);
                        epm.AddEdge(outgoingEdge);
                    }
                    else
                    {
                        //build the necessary edge contruct
                        epm = new EdgePropertyModel(sourceVertex, null);
                        outgoingEdge = new EdgeModel(Interlocked.Increment(ref _currentId), edgeDefinition.CreationDate, targetVertex, epm, edgeDefinition.Properties);
                        epm.AddEdge(outgoingEdge);

                        sourceVertex.AddOutEdge(edgePropertyId, outgoingEdge);
                    }

                    //add the edge to the graph elements
                    _graphElements.Add(outgoingEdge);

                    //link the vertices
                    targetVertex.AddIncomingEdge(edgePropertyId, outgoingEdge);
                }
                finally
                {
                    FinishWriteResource();
                }

                return outgoingEdge;
            }

            throw new CollisionException();
        }

        public bool TryAddProperty(Int32 graphElementId, Int32 propertyId, Object property)
        {
            if (ReadResource())
            {
                var graphElement = _graphElements.ElementAtOrDefault(graphElementId);

                FinishReadResource();

                return graphElement != null && graphElement.TryAddProperty(propertyId, property);
            }

            throw new CollisionException();
        }

        public bool TryRemoveProperty(Int32 graphElementId, Int32 propertyId)
        {
            if (ReadResource())
            {
                var graphElement = _graphElements.ElementAtOrDefault(graphElementId);
                    
                FinishReadResource();

                return graphElement != null && graphElement.TryRemoveProperty(propertyId);
            }

            throw new CollisionException();
        }

        public bool TryRemoveGraphElement(Int32 graphElementId)
        {
            //different actions for vertices, and edges
            throw new NotImplementedException();
        }
        #endregion

        #region IFallen8Read implementation
        public bool Search(out List<AGraphElement> result, Int32 propertyId, IComparable literal, BinaryOperator binOp)
        {
            #region binary operation

            switch (binOp)
            {
                case BinaryOperator.Equals:
                    result = FindElements(BinaryEqualsMethod, literal, propertyId);
                    break;

                case BinaryOperator.Greater:
                    result = FindElements(BinaryGreaterMethod, literal, propertyId);
                    break;

                case BinaryOperator.GreaterOrEquals:
                    result = FindElements(BinaryGreaterOrEqualMethod, literal, propertyId);
                    break;

                case BinaryOperator.LowerOrEquals:
                    result = FindElements(BinaryLowerOrEqualMethod, literal, propertyId);
                    break;

                case BinaryOperator.Lower:
                    result = FindElements(BinaryLowerMethod, literal, propertyId);
                    break;

                case BinaryOperator.NotEquals:
                    result = FindElements(BinaryNotEqualsMethod, literal, propertyId);
                    break;

                default:
                    result = new List<AGraphElement>();

                    break;
            }

            #endregion

            return result.Count > 0;
        }

        public bool SearchInIndex(out List<AGraphElement> result, String indexId, IComparable literal, BinaryOperator binOp)
        {
            IIndex index;
            if (!IndexFactory.TryGetIndex(out index, indexId))
            {

                result = null;
                return false;
            }

            #region binary operation

            switch (binOp)
            {
                case BinaryOperator.Equals:

                    IEnumerable<AGraphElement> equalsResult;
                    index.TryGetValue(out equalsResult, literal);
                    result = new List<AGraphElement>(equalsResult);
                    break;

                case BinaryOperator.Greater:
                    result = FindElementsIndex(BinaryGreaterMethod, literal, index);
                    break;

                case BinaryOperator.GreaterOrEquals:
                    result = FindElementsIndex(BinaryGreaterOrEqualMethod, literal, index);
                    break;

                case BinaryOperator.LowerOrEquals:
                    result = FindElementsIndex(BinaryLowerOrEqualMethod, literal, index);
                    break;

                case BinaryOperator.Lower:
                    result = FindElementsIndex(BinaryLowerMethod, literal, index);
                    break;

                case BinaryOperator.NotEquals:
                    result = FindElementsIndex(BinaryNotEqualsMethod, literal, index);
                    break;

                default:
                    result = new List<AGraphElement>();

                    break;
            }

            #endregion

            return result.Count > 0;
        }

        public bool SearchInRange (out List<AGraphElement> result, String indexId, IComparable leftLimit, IComparable rightLimit, bool includeLeft, bool includeRight)
        {
            IIndex index;
            if (!IndexFactory.TryGetIndex (out index, indexId)) {

                result = null;
                return false;
            }
            
            IRangeIndex rangeIndex = index as IRangeIndex;
            if (rangeIndex != null) {
                
                IEnumerable<AGraphElement> graphElementResult;
                if (rangeIndex.Between (out graphElementResult, leftLimit, rightLimit, includeLeft, includeRight)) {
                    
                    result = graphElementResult.ToList ();
                    
                    return true;
                }
            }
            
            result = null;
            return false;
        }

        public bool SearchFulltext (out FulltextSearchResult result, String indexId, string searchQuery)
        {
            IIndex index;
            if (!IndexFactory.TryGetIndex (out index, indexId)) {

                result = null;
                return false;
            }
            
            IFulltextIndex fulltextIndex = index as IFulltextIndex;
            if (fulltextIndex != null) {
                
                if (fulltextIndex.TryQuery(out result, searchQuery)) {
                    
                    return true;
                }
            }
            
            result = null;
            return false;
        }

        public bool SearchSpatial (out List<AGraphElement> result, String indexId, IGeometry geometry)
        {
            IIndex index;
            if (!IndexFactory.TryGetIndex (out index, indexId)) {

                result = null;
                return false;
            }
            
            ISpatialIndex spatialIndex = index as ISpatialIndex;
            if (spatialIndex != null) {
                
                IEnumerable<AGraphElement> graphElementResult;
                if (spatialIndex.TryGetValues (out graphElementResult, geometry)) {
                    
                    result = graphElementResult.ToList ();
                    
                    return true;
                }
            }
            
            result = null;
            return false;
        }
        
        public bool TrySaveAs (out Stream savedFallen8)
        {
            throw new NotImplementedException ();
        }

        #endregion

        #region public methods

        /// <summary>
        /// Gets a vertex by its identifier.
        /// </summary>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
        /// The vertex.
        /// </param>
        /// <param name='id'>
        /// System wide unique identifier.
        /// </param>
        public Boolean TryGetVertex(out VertexModel result, Int32 id)
        {
            if (ReadResource())
            {
                var graphElement  = _graphElements.ElementAtOrDefault(id);

                FinishReadResource();

                if (graphElement != null)
                {
                    result = graphElement as VertexModel;

                    return result != null;
                }

                result = null;
                return false;
            }

            throw new CollisionException();

        }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <returns>
        /// The vertices.
        /// </returns>
        public ReadOnlyCollection<VertexModel> GetVertices()
        {
            if (ReadResource())
            {
                ReadOnlyCollection<VertexModel> vertices;

                try
                {
                    vertices = new ReadOnlyCollection<VertexModel>(_graphElements.OfType<VertexModel>().ToList());
                }
                finally
                {
                    FinishReadResource();
                }

                return vertices;
            }

            throw new CollisionException();
        }

        /// <summary>
        /// Gets an edge by its identifier.
        /// </summary>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
        /// The edge.
        /// </param>
        /// <param name='id'>
        /// System wide unique identifier.
        /// </param>
        public Boolean TryGetEdge(out EdgeModel result, Int32 id)
        {
            if (ReadResource())
            {
                if (ReadResource())
                {
                    var graphElement = _graphElements.ElementAtOrDefault(id);

                    FinishReadResource();

                    if (graphElement != null)
                    {
                        result = graphElement as EdgeModel;

                        return result != null;
                    }

                    result = null;
                    return false;
                }

                throw new CollisionException();
            }

            throw new CollisionException();
        }

        /// <summary>
        /// Gets the edges.
        /// </summary>
        /// <returns>
        /// The edges.
        /// </returns>
        public ReadOnlyCollection<EdgeModel> GetEdges()
        {
            if (ReadResource())
            {
                ReadOnlyCollection<EdgeModel> edges;

                try
                {
                    edges = new ReadOnlyCollection<EdgeModel>(_graphElements.OfType<EdgeModel>().ToList());
                }
                finally
                {
                    FinishReadResource();
                }

                return edges;
            }

            throw new CollisionException();
        }

        #endregion

        #region private helper methods

        /// <summary>
        /// Finds the elements.
        /// </summary>
        /// <returns>
        /// The elements.
        /// </returns>
        /// <param name='finder'>
        /// Finder.
        /// </param>
        /// <param name='literal'>
        /// Literal.
        /// </param>
        /// <param name='propertyId'>
        /// Property identifier.
        /// </param>
        private List<AGraphElement> FindElements(BinaryOperatorDelegate finder, IComparable literal, Int32 propertyId)
        {
            if (ReadResource())
            {
                List<AGraphElement> result;

                result = _graphElements.AsParallel().Where(aGraphElement =>
                                                               {
                                                                   Object property;
                                                                   return
                                                                       aGraphElement.TryGetProperty(out property,
                                                                                                    propertyId) &&
                                                                       finder(property as IComparable, literal);
                                                               }).ToList();
                FinishReadResource();

                return result;
            }

            throw new CollisionException();
   
        }
        
        /// <summary>
        /// Finds elements via an index.
        /// </summary>
        /// <returns>
        /// The elements.
        /// </returns>
        /// <param name='finder'>
        /// Finder delegate.
        /// </param>
        /// <param name='literal'>
        /// Literal.
        /// </param>
        /// <param name='index'>
        /// Index.
        /// </param>
        private static List<AGraphElement> FindElementsIndex(BinaryOperatorDelegate finder, IComparable literal, IIndex index)
        {
            return index.GetKeyValues ()
                .AsParallel ()
                .Where (aIndexElement => finder (aIndexElement.Key, literal))
                .Select (_ => _.Value)
                .SelectMany(__ => __)
                .Distinct()
                .ToList();       
        }
        
        /// <summary>
        /// Method for binary comparism
        /// </summary>
        /// <returns>
        /// <c>true</c> for equality; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='property'>
        /// Property.
        /// </param>
        /// <param name='literal'>
        /// Literal.
        /// </param>
        private static Boolean BinaryEqualsMethod (IComparable property, IComparable literal)
        {
            return property.Equals (literal);
        }
        
        /// <summary>
        /// Method for binary comparism
        /// </summary>
        /// <returns>
        /// <c>true</c> for inequality; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='property'>
        /// Property.
        /// </param>
        /// <param name='literal'>
        /// Literal.
        /// </param>
        private static Boolean BinaryNotEqualsMethod (IComparable property, IComparable literal)
        {
            return !property.Equals (literal);
        }
        
        /// <summary>
        /// Method for binary comparism
        /// </summary>
        /// <returns>
        /// <c>true</c> for greater property; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='property'>
        /// Property.
        /// </param>
        /// <param name='literal'>
        /// Literal.
        /// </param>
        private static Boolean BinaryGreaterMethod (IComparable property, IComparable literal)
        {
            return property.CompareTo (literal) > 0;
        }
        
        /// <summary>
        /// Method for binary comparism
        /// </summary>
        /// <returns>
        /// <c>true</c> for lower property; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='property'>
        /// Property.
        /// </param>
        /// <param name='literal'>
        /// Literal.
        /// </param>
        private static Boolean BinaryLowerMethod (IComparable property, IComparable literal)
        {
            return property.CompareTo (literal) < 0;
        }
        
        /// <summary>
        /// Method for binary comparism
        /// </summary>
        /// <returns>
        /// <c>true</c> for lower or equal property; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='property'>
        /// Property.
        /// </param>
        /// <param name='literal'>
        /// Literal.
        /// </param>
        private static Boolean BinaryLowerOrEqualMethod (IComparable property, IComparable literal)
        {
            return property.CompareTo (literal) <= 0;
        }
        
        /// <summary>
        /// Method for binary comparism
        /// </summary>
        /// <returns>
        /// <c>true</c> for greater or equal property; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='property'>
        /// Property.
        /// </param>
        /// <param name='literal'>
        /// Literal.
        /// </param>
        private static Boolean BinaryGreaterOrEqualMethod (IComparable property, IComparable literal)
        {
            return property.CompareTo (literal) >= 0;
        }
        
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
        private EdgePropertyModel CreateEdgeProperty(Int32 edgePropertyId, IEnumerable<EdgeModelDefinition> edgeDefinitions, VertexModel sourceVertex)
        {
            var edges = new List<EdgeModel> ();
            
            var result = new EdgePropertyModel (sourceVertex, edges);
            
            foreach (var aEdgeDefinition in edgeDefinitions) {
                
                var targetVertex = (VertexModel)_graphElements [aEdgeDefinition.TargetVertexId];
                
                var outgoingEdge = new EdgeModel (Interlocked.Increment (ref _currentId), aEdgeDefinition.CreationDate, targetVertex, result, aEdgeDefinition.Properties);
                
                //add the new edge to the store
                _graphElements.Add(outgoingEdge);

                targetVertex.AddIncomingEdge (edgePropertyId, outgoingEdge);
                
                edges.Add (outgoingEdge);
            }
            
            return result;
        }
        
        #endregion
    }
}

