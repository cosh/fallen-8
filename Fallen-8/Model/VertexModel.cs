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
using Fallen8.API.Error;

namespace Fallen8.API.Model
{
    /// <summary>
    /// Vertex model.
    /// </summary>
    public sealed class VertexModel : AGraphElement
    {
        #region Data
        
        /// <summary>
        /// The out edges.
        /// </summary>
        private Dictionary<Int32, EdgePropertyModel> _outEdges;
        
        /// <summary>
        /// The in edges.
        /// </summary>
        private Dictionary<Int32, List<EdgeModel>> _inEdges;
        
        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexModel"/> class.
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
        public VertexModel(Int32 id, DateTime creationDate, IDictionary<Int32, Object> properties)
            : base(id, creationDate, properties)
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
        internal void AddOutEdge(Int32 edgePropertyId, EdgeModel outEdge)
        {
            //Interlocked.CompareExchange(ref _outEdges, new Dictionary<int, EdgePropertyModel>(), null);

            if (WriteResource())
            {
                if (_outEdges == null)
                {
                    _outEdges = new Dictionary<int, EdgePropertyModel>();
                }

                EdgePropertyModel edgeProperty;
                if (_outEdges.TryGetValue(edgePropertyId, out edgeProperty))
                {
                    edgeProperty.AddEdge(outEdge);
                }
                else
                {
                    _outEdges.Add(edgePropertyId, outEdge.SourceEdgeProperty);
                }
                FinishWriteResource();

                return;
            }


            throw new CollisionException();
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
        internal void SetOutEdges(Dictionary<Int32, EdgePropertyModel> outEdges)
        {
            if (WriteResource())
            {
                _outEdges = outEdges;
                FinishWriteResource();

                return;
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
        internal void AddIncomingEdge(Int32 edgePropertyId, EdgeModel incomingEdge)
        {
            //Interlocked.CompareExchange(ref _inEdges, new Dictionary<int, List<EdgeModel>>(), null);

            if (WriteResource())
            {
                if (_inEdges == null)
                {
                    _inEdges = new Dictionary<int, List<EdgeModel>>();
                }

                List<EdgeModel> inEdges;
                if (_inEdges.TryGetValue(edgePropertyId, out inEdges))
                {
                    inEdges.Add(incomingEdge);
                }
                else
                {
                    _inEdges.Add(edgePropertyId, new List<EdgeModel> {incomingEdge});
                }
                FinishWriteResource();

                return;
            }

            throw new CollisionException();
        }

        #endregion

        #region IVertexModel implementation

        /// <summary>
        /// Gets all neighbors.
        /// </summary>
        /// <returns>
        /// The neighbors.
        /// </returns>
        public List<VertexModel> GetAllNeighbors()
        {
            if (ReadResource())
            {
                var neighbors = new List<VertexModel>();

                if (_outEdges != null && _outEdges.Count > 0)
                {
                    foreach (var aOutEdge in _outEdges)
                    {
                        neighbors.AddRange(aOutEdge.Value.GetEdges().Select(_ => _.TargetVertex));
                    }
                }

                if (_inEdges != null && _inEdges.Count > 0)
                {
                    foreach (var aInEdge in _inEdges)
                    {
                        neighbors.AddRange(aInEdge.Value.Select(_ => _.SourceEdgeProperty.SourceVertex));
                    }

                }
                FinishReadResource();

                return neighbors;
            }

            throw new CollisionException();
        }

        /// <summary>
        /// Gets the incoming edge identifiers.
        /// </summary>
        /// <returns>
        /// The incoming edge identifiers.
        /// </returns>
        public List<Int32> GetIncomingEdgeIds()
        {
            if (ReadResource())
            {
                var inEdges = new List<Int32>();

                if (_inEdges != null)
                {
                    inEdges.AddRange(_inEdges.Select(_ => _.Key));
                }
                FinishReadResource();

                return inEdges;
            }

            throw new CollisionException ();
        }

        /// <summary>
        /// Gets the outgoing edge identifiers.
        /// </summary>
        /// <returns>
        /// The outgoing edge identifiers.
        /// </returns>
        public List<Int32> GetOutgoingEdgeIds()
        {
            if (ReadResource())
            {
                var outEdges = new List<Int32>();

                if (_outEdges != null)
                {
                    outEdges.AddRange(_outEdges.Select(_ => _.Key));
                }
                FinishReadResource();

                return outEdges;
            }

            throw new CollisionException();
        }

        /// <summary>
        /// Tries to get an out edge.
        /// </summary>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
        /// Result.
        /// </param>
        /// <param name='edgePropertyId'>
        /// Edge property identifier.
        /// </param>
        public Boolean TryGetOutEdge(out EdgePropertyModel result, Int32 edgePropertyId)
        {
            if (ReadResource())
            {
                var foundSth = false;

                if (_outEdges != null && _outEdges.Count > 0)
                {
                    foundSth = _outEdges.TryGetValue(edgePropertyId, out result);
                }
                else
                {
                    result = null;
                }

                FinishReadResource();

                return foundSth;
            }

            throw new CollisionException ();
        }

        /// <summary>
        /// Tries to get in edges.
        /// </summary>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
        /// Result.
        /// </param>
        /// <param name='edgePropertyId'>
        /// Edge property identifier.
        /// </param>
        public Boolean TryGetInEdges(out List<EdgeModel> result, Int32 edgePropertyId)
        {
            if (ReadResource())
            {
                if (_inEdges != null && _inEdges.Count > 0)
                {
                    return _inEdges.TryGetValue(edgePropertyId, out result);
                }
                FinishReadResource();

                result = null;
                return false;
            }

            throw new CollisionException ();
        }

        #endregion

        #region Equals Overrides

        public override Boolean Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to IEdgeModel return false.
            var p = obj as VertexModel;

            return p != null && Equals(p);
        }

        public Boolean Equals(VertexModel p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return this.Id == p.Id;
        }

        public static Boolean operator ==(VertexModel a, VertexModel b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(VertexModel a, VertexModel b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        #endregion
    }
}
