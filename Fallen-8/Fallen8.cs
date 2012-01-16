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
        private Int64 _currentId = Int64.MinValue;
        
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
            _indexFactory = new Fallen8IndexFactory ();
            _model = new GraphModel (Interlocked.Increment (ref _currentId), DateTime.Now, null);
        }
        
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
        public IVertexModel CreateVertex(VertexModelDefinition vertexDefinition)
        {
            //create the new vertex
            VertexModel newVertex = new VertexModel(Interlocked.Increment(ref _currentId), vertexDefinition.CreationDate, vertexDefinition.Properties);

            _model.Graphelements[newVertex.Id] = newVertex;

            if (vertexDefinition.Edges != null && vertexDefinition.Edges.Count > 0)
            {

                Dictionary<long, EdgePropertyModel> outEdges = new Dictionary<long, EdgePropertyModel>();

                foreach (var aEdge in vertexDefinition.Edges)
                {
                    outEdges.Add(aEdge.Key, CreateEdgeProperty(aEdge.Key, aEdge.Value, newVertex));
                }

                newVertex.AddOutEdges(outEdges);
            }

            vertexDefinition = null;

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

            //add the edge to the graph elements
            _model.Graphelements[outgoingEdge.Id] = outgoingEdge;

            //link the vertices
            sourceVertex.AddOutEdge (edgePropertyId, outgoingEdge);
            targetVertex.AddIncomingEdge (edgePropertyId, outgoingEdge);

            edgeDefinition = null;

            return outgoingEdge;
        }

        public bool TryAddProperty (long graphElementId, long propertyId, Object property)
        {
            IGraphElementModel graphElement;
            if (_model.Graphelements.TryGetValue (graphElementId, out graphElement)) {
                return ((AGraphElement)graphElement).TryAddProperty (propertyId, property);   
            } else {
                return false;
            }
        }

        public bool TryRemoveProperty (long graphElementId, long propertyId)
        {
            IGraphElementModel graphElement;
            if (_model.Graphelements.TryGetValue (graphElementId, out graphElement)) {
                return ((AGraphElement)graphElement).TryRemoveProperty (propertyId);   
            } else {
                return false;
            }
        }

        public bool TryRemoveGraphElement (long graphElementId)
        {
            IGraphElementModel gelement;
            return _model.Graphelements.TryRemove (graphElementId, out gelement);
        }
        #endregion

        #region IFallen8Read implementation
        public bool Search (out IEnumerable<IGraphElementModel> result, long propertyId, IComparable literal, BinaryOperator binOp)
        {
            List<IGraphElementModel> graphElements = null;
            
            #region binary operation
            
            switch (binOp) {
            case BinaryOperator.Equals:
                graphElements = new List<IGraphElementModel> (FindElements (BinaryEqualsMethod, literal, propertyId));
                break;
                
            case BinaryOperator.Greater:
                graphElements = new List<IGraphElementModel> (FindElements (BinaryGreaterMethod, literal, propertyId));
                break;
                
            case BinaryOperator.GreaterOrEquals:
                graphElements = new List<IGraphElementModel> (FindElements (BinaryGreaterOrEqualMethod, literal, propertyId));
                break;
            
            case BinaryOperator.LowerOrEquals:
                graphElements = new List<IGraphElementModel> (FindElements (BinaryLowerOrEqualMethod, literal, propertyId));
                break;
                
            case BinaryOperator.Lower:
                graphElements = new List<IGraphElementModel> (FindElements (BinaryLowerMethod, literal, propertyId));
                break;
            
            case BinaryOperator.NotEquals:
                graphElements = new List<IGraphElementModel> (FindElements (BinaryNotEqualsMethod, literal, propertyId));
                break;
                
            default:
                break;
            }
            
            #endregion
            
            result = graphElements;
            return graphElements.Count > 0;
        }

        public bool SearchInIndex (out IEnumerable<IGraphElementModel> result, String indexId, IComparable literal, BinaryOperator binOp)
        {
            IIndex index;
            if (!IndexProvider.TryGetIndex (out index, indexId)) {
                
                result = null;
                return false;
            }
            
            List<IGraphElementModel> graphElements = null;
            
            #region binary operation
            
            switch (binOp) {
            case BinaryOperator.Equals:
                
                return index.GetValue (out result, literal);
                
            case BinaryOperator.Greater:
                graphElements = new List<IGraphElementModel> (FindElementsIndex (BinaryGreaterMethod, literal, index));
                break;
                
            case BinaryOperator.GreaterOrEquals:
                graphElements = new List<IGraphElementModel> (FindElementsIndex (BinaryGreaterOrEqualMethod, literal, index));
                break;
            
            case BinaryOperator.LowerOrEquals:
                graphElements = new List<IGraphElementModel> (FindElementsIndex (BinaryLowerOrEqualMethod, literal, index));
                break;
                
            case BinaryOperator.Lower:
                graphElements = new List<IGraphElementModel> (FindElementsIndex (BinaryLowerMethod, literal, index));
                break;
            
            case BinaryOperator.NotEquals:
                graphElements = new List<IGraphElementModel> (FindElementsIndex (BinaryNotEqualsMethod, literal, index));
                break;
                
            default:
                break;
            }
            
            #endregion
            
            result = graphElements;
            return graphElements.Count > 0;
        }

        public bool SearchInRange (out IEnumerable<IGraphElementModel> result, String indexId, IComparable leftLimit, IComparable rightLimit, bool includeLeft, bool includeRight)
        {
            throw new NotImplementedException ();
        }

        public bool SearchFulltext (out FulltextSearchResult result, String indexId, string searchQuery)
        {
            throw new NotImplementedException ();
        }

        public bool SearchSpatial (out IEnumerable<IGraphElementModel> result, String indexId, IGeometry geometry)
        {
            throw new NotImplementedException ();
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
        private IEnumerable<IGraphElementModel> FindElements (BinaryOperatorDelegate finder, IComparable literal, Int64 propertyId)
        {
            return _model.Graphelements.AsParallel ().Where (aGraphElement => 
            {
                Object property; 
                if (aGraphElement.Value.TryGetProperty (out property, propertyId)) {
                        
                    return finder (property as IComparable, literal);
                }
                    
                return false;
            }).Select (_ => _.Value);
                
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
        private IEnumerable<IGraphElementModel> FindElementsIndex (BinaryOperatorDelegate finder, IComparable literal, IIndex index)
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
        private EdgePropertyModel CreateEdgeProperty (Int64 edgePropertyId, List<EdgeModelDefinition> edgeDefinitions, VertexModel sourceVertex)
        {
            List<IEdgeModel> edges = new List<IEdgeModel> ();
            
            EdgePropertyModel result = new EdgePropertyModel (sourceVertex, edges);
            
            foreach (var aEdgeDefinition in edgeDefinitions) {
                
                var targetVertex = (VertexModel)_model.Graphelements [aEdgeDefinition.TargetVertexId];
                
                var outgoingEdge = new EdgeModel (Interlocked.Increment (ref _currentId), aEdgeDefinition.CreationDate, targetVertex, result, aEdgeDefinition.Properties);
                _model.Graphelements[outgoingEdge.Id] = outgoingEdge;

                targetVertex.AddIncomingEdge (edgePropertyId, outgoingEdge);
                
                edges.Add (outgoingEdge);
            }
            
            return result;
        }
        
        #endregion
	}
}

