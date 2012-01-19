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
using Fallen8.Model;
using Fallen8.API.Index;
using Fallen8.API.Helper;
using Fallen8.API.Expression;
using System.Collections.Generic;
using System.Threading;
using Fallen8.API.Error;

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
        private readonly List<AGraphElement> _graphElements;

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
        /// Initializes a new instance of the <see cref="Fallen8.API.Fallen8"/> class.
        /// </summary>
        public Fallen8 ()
        {
            IndexFactory = new Fallen8IndexFactory();
            _graphElements = new List<AGraphElement>(5000000);
        }
        
        #endregion
        
        #region IFallen8Write implementation
        public VertexModel CreateVertex(DateTime creationDate, Dictionary<Int32, Object> properties = null, Dictionary<Int32, List<EdgeModelDefinition>> edges = null)
        {
            if (WriteResource())
            {
                //create the new vertex
                VertexModel newVertex = new VertexModel(Interlocked.Increment(ref _currentId), creationDate, properties);

                _graphElements.Add(newVertex);

                if (edges != null && edges.Count > 0)
                {
                    Dictionary<Int32, EdgePropertyModel> outEdges = new Dictionary<Int32, EdgePropertyModel>();

                    foreach (var aEdge in edges)
                    {
                        outEdges.Add(aEdge.Key, CreateEdgeProperty(aEdge.Key, aEdge.Value, newVertex));
                    }

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
                //get the related vertices
                var sourceVertex = (VertexModel)_graphElements[sourceVertexId];
                var targetVertex = (VertexModel)_graphElements[edgeDefinition.TargetVertexId];

                EdgeModel outgoingEdge;
                EdgePropertyModel epm;
                if (sourceVertex.TryGetOutEdge( out epm, edgePropertyId))
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

                FinishWriteResource();

                return outgoingEdge;
            }

            throw new CollisionException();
        }

        public bool TryAddProperty(Int32 graphElementId, Int32 propertyId, Object property)
        {
            if (ReadResource())
            {
                AGraphElement graphElement = _graphElements[graphElementId];

                FinishReadResource();

                if (graphElement != null)
                {
                    return graphElement.TryAddProperty(propertyId, property);
                }

                return false;
            }

            throw new CollisionException();
        }

        public bool TryRemoveProperty(Int32 graphElementId, Int32 propertyId)
        {
            if (ReadResource())
            {
                AGraphElement graphElement = _graphElements[graphElementId];

                FinishReadResource();

                if (graphElement != null)
                {
                    return graphElement.TryRemoveProperty(propertyId);
                }

                return false;
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
        public bool Search(out IEnumerable<AGraphElement> result, Int32 propertyId, IComparable literal, BinaryOperator binOp)
        {
            List<AGraphElement> graphElements = null;
            
            #region binary operation
            
            switch (binOp) {
            case BinaryOperator.Equals:
                    graphElements = new List<AGraphElement>(FindElements(BinaryEqualsMethod, literal, propertyId));
                break;
                
            case BinaryOperator.Greater:
                graphElements = new List<AGraphElement>(FindElements(BinaryGreaterMethod, literal, propertyId));
                break;
                
            case BinaryOperator.GreaterOrEquals:
                graphElements = new List<AGraphElement>(FindElements(BinaryGreaterOrEqualMethod, literal, propertyId));
                break;
            
            case BinaryOperator.LowerOrEquals:
                graphElements = new List<AGraphElement>(FindElements(BinaryLowerOrEqualMethod, literal, propertyId));
                break;
                
            case BinaryOperator.Lower:
                graphElements = new List<AGraphElement>(FindElements(BinaryLowerMethod, literal, propertyId));
                break;
            
            case BinaryOperator.NotEquals:
                graphElements = new List<AGraphElement>(FindElements(BinaryNotEqualsMethod, literal, propertyId));
                break;
                
            default:
                break;
            }
            
            #endregion
            
            result = graphElements;
            return graphElements.Count > 0;
        }

        public bool SearchInIndex(out IEnumerable<AGraphElement> result, String indexId, IComparable literal, BinaryOperator binOp)
        {
            IIndex index;
            if (!IndexFactory.TryGetIndex (out index, indexId)) {
                
                result = null;
                return false;
            }

            List<AGraphElement> graphElements = null;
            
            #region binary operation
            
            switch (binOp) {
            case BinaryOperator.Equals:
                
                return index.GetValue (out result, literal);
                
            case BinaryOperator.Greater:
                graphElements = new List<AGraphElement>(FindElementsIndex(BinaryGreaterMethod, literal, index));
                break;
                
            case BinaryOperator.GreaterOrEquals:
                graphElements = new List<AGraphElement>(FindElementsIndex(BinaryGreaterOrEqualMethod, literal, index));
                break;
            
            case BinaryOperator.LowerOrEquals:
                graphElements = new List<AGraphElement>(FindElementsIndex(BinaryLowerOrEqualMethod, literal, index));
                break;
                
            case BinaryOperator.Lower:
                graphElements = new List<AGraphElement>(FindElementsIndex(BinaryLowerMethod, literal, index));
                break;
            
            case BinaryOperator.NotEquals:
                graphElements = new List<AGraphElement>(FindElementsIndex(BinaryNotEqualsMethod, literal, index));
                break;
                
            default:
                break;
            }
            
            #endregion
            
            result = graphElements;
            return graphElements.Count > 0;
        }

        public bool SearchInRange(out IEnumerable<AGraphElement> result, String indexId, IComparable leftLimit, IComparable rightLimit, bool includeLeft, bool includeRight)
        {
            throw new NotImplementedException ();
        }

        public bool SearchFulltext (out FulltextSearchResult result, String indexId, string searchQuery)
        {
            throw new NotImplementedException ();
        }

        public bool SearchSpatial(out IEnumerable<AGraphElement> result, String indexId, IGeometry geometry)
        {
            throw new NotImplementedException ();
        }
        #endregion

        #region public methods

        /// <summary>
        /// Gets a vertex by its identifier.
        /// </summary>
        /// <returns>
        /// The vertex.
        /// </returns>
        /// <param name='id'>
        /// System wide unique identifier.
        /// </param>
        public VertexModel GetVertex(Int32 id)
        {
            return _graphElements[id] as VertexModel;
        }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <returns>
        /// The vertices.
        /// </returns>
        public IEnumerable<VertexModel> GetVertices()
        {
            return _graphElements.Where(aGraphElementKV => aGraphElementKV is VertexModel).Select(aVertexKV => (VertexModel)aVertexKV);
        }

        /// <summary>
        /// Gets an edge by its identifier.
        /// </summary>
        /// <returns>
        /// The edge.
        /// </returns>
        /// <param name='id'>
        /// System wide unique identifier.
        /// </param>
        public EdgeModel GetEdge(Int32 id)
        {
            return _graphElements[id] as EdgeModel;
        }

        /// <summary>
        /// Gets the edges.
        /// </summary>
        /// <returns>
        /// The edges.
        /// </returns>
        public IEnumerable<EdgeModel> GetEdges()
        {
            return _graphElements.Where(aGraphElementKV => aGraphElementKV is EdgeModel).Select(aEdgeKV => (EdgeModel)aEdgeKV);
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
        private IEnumerable<AGraphElement> FindElements(BinaryOperatorDelegate finder, IComparable literal, Int32 propertyId)
        {
            return _graphElements.AsParallel ().Where (aGraphElement => 
            {
                Object property; 
                if (aGraphElement.TryGetProperty (out property, propertyId)) {
                        
                    return finder (property as IComparable, literal);
                }
                    
                return false;
            });
                
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
        private IEnumerable<AGraphElement> FindElementsIndex(BinaryOperatorDelegate finder, IComparable literal, IIndex index)
        {
            return index.GetKeyValues ().AsParallel ().Where (aIndexElement => 
            {
                return finder (aIndexElement.Key, literal);
            }).Select (_ => _.Value).SelectMany(__ => __).Distinct();       
        }
        
        /// <summary>
        /// Method for binary comparism
        /// </summary>
        /// <c>true</c> for equality; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
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
        /// <c>true</c> for inequality; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
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
        /// <c>true</c> for greater property; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
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
        /// <c>true</c> for lower property; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
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
        /// <c>true</c> for lower or equal property; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
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
        /// <c>true</c> for greater or equal property; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
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
        private EdgePropertyModel CreateEdgeProperty(Int32 edgePropertyId, List<EdgeModelDefinition> edgeDefinitions, VertexModel sourceVertex)
        {
            List<EdgeModel> edges = new List<EdgeModel> ();
            
            EdgePropertyModel result = new EdgePropertyModel (sourceVertex, edges);
            
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

