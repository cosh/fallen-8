// 
// VertexModel.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2011 Henning Rauch
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
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Fallen8.API.Error;

namespace Fallen8.Model
{
    /// <summary>
    /// Vertex model.
    /// </summary>
    public sealed class VertexModel : AGraphElement, IVertexModel
    {
        #region Data
        
        /// <summary>
        /// The out edges.
        /// </summary>
        private Dictionary<long, EdgePropertyModel> _outEdges;
        
        /// <summary>
        /// The in edges.
        /// </summary>
        private Dictionary<long, List<IEdgeModel>> _inEdges;
        
        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Fallen8.Model.VertexModel"/> class.
        /// </summary>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        /// <param name='creationDate'>
        /// Creation date.
        /// </param>
        /// <param name='properties'>
        /// Properties.
        /// </param>
        public VertexModel (Int64 id, DateTime creationDate, Dictionary<Int64, Object> properties) : base (id, creationDate, properties)
        {
        }
        
        #endregion
        
        #region public methods
  
        /// <summary>
        /// Adds the out edge.
        /// </summary>
        /// <param name='edgePropertyId'>
        /// Edge property identifier.
        /// </param>
        /// <param name='outEdge'>
        /// Out edge.
        /// </param>
        /// <exception cref='CollisionException'>
        /// Is thrown when the collision exception.
        /// </exception>
        public void AddOutEdge (long edgePropertyId, EdgeModel outEdge)
        {
            if (WriteResource ()) {
                
                if (_outEdges == null) {
                    _outEdges = new Dictionary<long, EdgePropertyModel> ();
                }
                    
                EdgePropertyModel edgeProperty;
                if (_outEdges.TryGetValue (edgePropertyId, out edgeProperty)) {
                    edgeProperty.AddEdge (outEdge);
                } else {
                    _outEdges.Add (edgePropertyId, new EdgePropertyModel (this, new List<IEdgeModel> {outEdge}));
                }
                       
            }
            
            FinishWriteResource ();
            
            throw new CollisionException ();
        }
        
        /// <summary>
        /// Adds the out edges.
        /// </summary>
        /// <param name='outEdges'>
        /// Out edges.
        /// </param>
        /// <exception cref='CollisionException'>
        /// Is thrown when the collision exception.
        /// </exception>
        public void AddOutEdges (Dictionary<long, EdgePropertyModel> outEdges)
        {
            if (WriteResource ()) {
                
                if (_outEdges == null) {
                    _outEdges = outEdges;
                } else {
                    if (outEdges != null && outEdges.Count > 0) {
                        foreach (var aOutEdge in outEdges) {
                            
                            EdgePropertyModel edgeProperty;
                            if (_outEdges.TryGetValue (aOutEdge.Key, out edgeProperty)) {
                                edgeProperty.AddEdges (aOutEdge.Value);
                            } else {
                                _outEdges.Add (aOutEdge.Key, aOutEdge.Value);
                            }
                        }
                    }
                }
            
                FinishWriteResource ();
            }
            
            throw new CollisionException ();
        }
        
        /// <summary>
        /// Adds the incoming edge.
        /// </summary>
        /// <param name='edgePropertyId'>
        /// Edge property identifier.
        /// </param>
        /// <param name='incomingEdge'>
        /// Incoming edge.
        /// </param>
        /// <exception cref='CollisionException'>
        /// Is thrown when the collision exception.
        /// </exception>
        public void AddIncomingEdge (Int64 edgePropertyId, EdgeModel incomingEdge)
        {
            if (WriteResource ()) {
                
                if (_inEdges == null) {
                    _inEdges = new Dictionary<long, List<IEdgeModel>> ();
                }
            
                List<IEdgeModel> inEdges;
                if (_inEdges.TryGetValue (edgePropertyId, out inEdges)) {
                
                    inEdges.Add (incomingEdge);
                } else {
                    _inEdges.Add (edgePropertyId, new List<IEdgeModel> {incomingEdge});
                }
                
                FinishWriteResource ();
            }
            
            throw new CollisionException ();
        }
        
        #endregion
        
        #region Equals Overrides

        public override Boolean Equals (Object obj)
        {
            // If parameter is null return false.
            if (obj == null) {
                return false;
            }

            // If parameter cannot be cast to VertexModel return false.
            var p = obj as VertexModel;

            return p != null && Equals (p);
        }

        public Boolean Equals (VertexModel p)
        {
            // If parameter is null return false:
            if ((object)p == null) {
                return false;
            }

            return base._id == p.Id;
        }

        public static Boolean operator == (VertexModel a, VertexModel b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals (a, b)) {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) {
                return false;
            }

            // Return true if the fields match:
            return a.Equals (b);
        }

        public static Boolean operator != (VertexModel a, VertexModel b)
        {
            return !(a == b);
        }

        public override int GetHashCode ()
        {
            return base._id.GetHashCode ();
        }
        
        #endregion
        
        #region IGraphElementModel implementation
        public long Id {
            get {
                return base._id;
            }
        }

        public DateTime CreationDate {
            get {
                return base._creationDate;
            }
        }

        public DateTime ModificationDate {
            get {
                return base._modificationDate;
            }
        }
        
        public IEnumerable<PropertyContainer> GetAllProperties ()
        {
            if (ReadResource ()) {
                
                List<PropertyContainer> result = new List <PropertyContainer> ();
                
                if (base._properties != null && base._properties.Count > 0) {
                    result.AddRange (base._properties.Select (_ => new PropertyContainer (_.Key, _.Value)));
                }
                FinishReadResource ();
                
                return result;
            }
            
            throw new CollisionException ();
        }
        
        public Boolean TryGetProperty (out Object result, Int64 propertyId)
        {
            if (ReadResource ()) {
                
                Boolean foundsth = false;
                
                if (base._properties != null && base._properties.Count > 0 && base._properties.TryGetValue (propertyId, out result)) {
                    foundsth = true;    
                }
                else
                {
                    result = null;
                }
                FinishReadResource ();
                
                return foundsth;
            }
            
            throw new CollisionException ();
        }

        #endregion
        
        #region IVertexModel implementation
        
        public IEnumerable<IVertexModel> GetAllNeighbors ()
        {
            if (ReadResource ()) {
                List<IVertexModel> neighbors = new List<IVertexModel> ();
                
                if (_outEdges != null && _outEdges.Count > 0) {
                    foreach (var aOutEdge in _outEdges) {
                        neighbors.AddRange (aOutEdge.Value.Select (_ => _.TargetVertex));
                    }
                }
                
                if (_inEdges != null && _inEdges.Count > 0) {
                    foreach (var aInEdge in _inEdges) {
                        neighbors.AddRange (aInEdge.Value.Select (_ => _.SourceEdgeProperty.SourceVertex));
                    }
                    
                }
                
                FinishReadResource ();
                
                return neighbors;
            }
            
            throw new CollisionException ();
        }
        
        public IEnumerable<Int64> GetIncomingEdgeIds ()
        {
            if (ReadResource ()) {
                List<Int64> inEdges = new List<Int64> ();
                
                if (_inEdges != null && _inEdges.Count > 0) {
                    inEdges.AddRange (_inEdges.Select (_ => _.Key));
                }
                
                FinishReadResource ();
                
                return inEdges;
            }
            
            throw new CollisionException ();
        }
        
        public IEnumerable<Int64> GetOutgoingEdgeIds ()
        {
            if (ReadResource ()) {
                List<Int64> outEdges = new List<Int64> ();
                
                if (_outEdges != null && _outEdges.Count > 0) {
                    outEdges.AddRange (_outEdges.Select (_ => _.Key));
                }
                
                FinishReadResource ();
                
                return outEdges;
            }
            
            throw new CollisionException ();
        }
        
        public Boolean TryGetOutEdge (out IEdgePropertyModel result, Int64 edgePropertyId)
        {
            if (ReadResource ()) {
                
                Boolean foundSth = false;
                
                if (_outEdges != null && _outEdges.Count > 0) {
                    
                    EdgePropertyModel edgeProperty;
                    foundSth = _outEdges.TryGetValue (edgePropertyId, out edgeProperty);
                    result = edgeProperty;
                } else {
                    result = null;
                }
                
                FinishReadResource ();
                
                return foundSth;
            }
            
            throw new CollisionException ();
        }
        
        public Boolean TryGetInEdges (out IEnumerable<IEdgeModel> result, Int64 edgePropertyId)
        {
            if (ReadResource ()) {
                
                Boolean foundSth = false;
                
                if (_inEdges != null && _inEdges.Count > 0) {
                    
                    List<IEdgeModel> inEdges;
                    foundSth = _inEdges.TryGetValue (edgePropertyId, out inEdges);
                    result = inEdges;
                } else {
                    result = null;
                }
                
                FinishReadResource ();
                
                return foundSth;
            }
            
            throw new CollisionException ();
        }
        
        #endregion
    }
}
