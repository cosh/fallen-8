// 
// VertexModel.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2011-2015 Henning Rauch
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

#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using NoSQL.GraphDB.Error;

#endregion

namespace NoSQL.GraphDB.Model
{
    /// <summary>
    ///   Vertex model.
    /// </summary>
    public sealed class VertexModel : AGraphElement
    {
        #region Data

        /// <summary>
        ///   The out edges.
        /// </summary>
        internal List<EdgeContainer> _outEdges;

        /// <summary>
        ///   The in edges.
        /// </summary>
        internal List<EdgeContainer> _inEdges;

        #endregion

        #region Constructor

        /// <summary>
        ///   Initializes a new instance of the <see cref="VertexModel" /> class.
        /// </summary>
        /// <param name='id'> Identifier. </param>
        /// <param name='creationDate'> Creation date. </param>
        /// <param name='properties'> Properties. </param>
        public VertexModel(Int32 id, UInt32 creationDate, PropertyContainer[] properties)
            : base(id, creationDate, properties)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the VertexModel class. For internal usage only
        /// </summary>
        /// <param name='id'> Identifier. </param>
        /// <param name='creationDate'> Creation date. </param>
        /// <param name='modificationDate'> Modification date. </param>
        /// <param name='properties'> Properties. </param>
        /// <param name='outEdges'> Out edges. </param>
        /// <param name='incEdges'> Inc edges. </param>
        internal VertexModel(Int32 id, UInt32 creationDate, UInt32 modificationDate, PropertyContainer[] properties,
                             List<EdgeContainer> outEdges, List<EdgeContainer> incEdges)
            : base(id, creationDate, properties)
        {
            _outEdges = outEdges;
            _inEdges = incEdges;
            ModificationDate = modificationDate;
        }

        #endregion

        #region internal methods

        /// <summary>
        ///   Adds the out edge.
        /// </summary>
        /// <param name='edgePropertyId'> Edge property identifier. </param>
        /// <param name='outEdge'> Out edge. </param>
        /// <exception cref='CollisionException'>Is thrown when the collision exception.</exception>
        internal void AddOutEdge(UInt16 edgePropertyId, EdgeModel outEdge)
        {
            if (WriteResource())
            {
				if (_outEdges == null)
				{
					_outEdges = new List<EdgeContainer>();
				}

				var foundSth = false;

				for (var i = 0; i < _outEdges.Count; i++)
				{
					var aOutEdge = _outEdges[i];
					if (aOutEdge.EdgePropertyId == edgePropertyId)
					{
						aOutEdge.Edges.Add(outEdge);

						foundSth = true;

						break;
					}
				}

				if (!foundSth)
				{
					_outEdges.Add(new EdgeContainer(edgePropertyId, new List<EdgeModel> {outEdge}));
				}

				FinishWriteResource();

				return;
			}


            throw new CollisionException(this);
        }

        /// <summary>
        ///   Adds the out edges.
        /// </summary>
        /// <param name='outEdges'> Out edges. </param>
        /// <exception cref='CollisionException'>Is thrown when the collision exception.</exception>
        internal void SetOutEdges(List<EdgeContainer> outEdges)
        {
            if (WriteResource())
            {
                try
                {
                    _outEdges = outEdges;
                }
                finally
                {
                    FinishWriteResource();
                }

                return;
            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///   Adds the incoming edge.
        /// </summary>
        /// <param name='edgePropertyId'> Edge property identifier. </param>
        /// <param name='incomingEdge'> Incoming edge. </param>
        /// <exception cref='CollisionException'>Is thrown when the collision exception.</exception>
        internal void AddIncomingEdge(UInt16 edgePropertyId, EdgeModel incomingEdge)
        {
            if (WriteResource())
            {
                try
                {
                    if (_inEdges == null)
                    {
                        _inEdges = new List<EdgeContainer>();
                    }

                    var foundSth = false;

                    for (var i = 0; i < _inEdges.Count; i++)
                    {
                        var aInEdge = _inEdges[i];
                        if (aInEdge.EdgePropertyId == edgePropertyId)
                        {
                            aInEdge.Edges.Add(incomingEdge);
                            foundSth = true;
                            break;
                        }
                    }

                    if (!foundSth)
                    {
					_inEdges.Add(new EdgeContainer(edgePropertyId, new List<EdgeModel> {incomingEdge}));
                    }

                }
                finally
                {
                    FinishWriteResource();
                }

                return;
            }

            throw new CollisionException(this);
		}

		/// <summary>
		///   Gets the incoming edges.
		/// </summary>
		/// <returns> The incoming edges. </returns>
		internal ReadOnlyCollection<EdgeContainer> GetIncomingEdges()
		{
			ReadOnlyCollection<EdgeContainer> result = null;

            if (ReadResource())
            {
                try
                {
                    if (_inEdges != null)
                    {
                        result = new ReadOnlyCollection<EdgeContainer>(_inEdges);
                    }

                    return result;
                }
                finally
                {
                    FinishReadResource();
                }

            }

            throw new CollisionException(this);
		}

		/// <summary>
		///   Gets the outgoing edges.
		/// </summary>
		/// <returns> The outgoing edges. </returns>
		internal ReadOnlyCollection<EdgeContainer> GetOutgoingEdges()
		{
			ReadOnlyCollection<EdgeContainer> result = null;

            if (ReadResource())
            {
                try
                {
                    if (_outEdges != null)
                    {
                        result = new ReadOnlyCollection<EdgeContainer>(_outEdges);
                    }
                    return result;
                }
                finally
                {
                    FinishReadResource();
                }

            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///   Removes an incoming edge
        /// </summary>
        /// <param name="edgePropertyId"> Edge property identifier. </param>
        /// <param name="toBeRemovedEdge"> The to be removed edge </param>
        internal void RemoveIncomingEdge(ushort edgePropertyId, EdgeModel toBeRemovedEdge)
        {
            if (WriteResource())
            {
                try
                {
                    if (_inEdges == null)
                    {
                        return;
                    }

                    for (var i = 0; i < _inEdges.Count; i++)
                    {
                        var aInEdge = _inEdges[i];
                        if (aInEdge.EdgePropertyId == edgePropertyId)
                        {
                            aInEdge.Edges.RemoveAll(_ => _.Id == toBeRemovedEdge.Id);
                            break;
                        }
                    }
                }
                finally
                {
                    FinishWriteResource();
                }

                return;
            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///   Removes an incoming edge
        /// </summary>
        /// <param name="toBeRemovedEdge"> The to be removed edge </param>
        /// <returns> The edge property identifier where the edge was deleted </returns>
        internal List<UInt16> RemoveIncomingEdge(EdgeModel toBeRemovedEdge)
        {
            if (WriteResource())
            {
                try
                {
                    var result = new List<UInt16>();

                    if (_inEdges == null)
                    {
                        return result;
                    }

                    for (var i = 0; i < _inEdges.Count; i++)
                    {
                        if (_inEdges[i].Edges.RemoveAll(_ => _.Id == toBeRemovedEdge.Id) > 0)
                        {
                            result.Add(_inEdges[i].EdgePropertyId);
                        }
                    }
                    return result;
                }
                finally
                {
                    FinishWriteResource();
                }

            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///   Remove outgoing edge
        /// </summary>
        /// <param name="edgePropertyId"> The edge property identifier. </param>
        /// <param name="toBeRemovedEdge"> The to be removed edge </param>
        internal void RemoveOutGoingEdge(ushort edgePropertyId, EdgeModel toBeRemovedEdge)
        {
            if (WriteResource())
            {
                try
                {
                    if (_outEdges == null)
                    {
                        return;
                    }

                    for (var i = 0; i < _outEdges.Count; i++)
                    {
                        var aOutEdge = _outEdges[i];
                        if (aOutEdge.EdgePropertyId == edgePropertyId)
                        {
                            aOutEdge.Edges.RemoveAll(_ => _.Id == toBeRemovedEdge.Id);
                            break;
                        }
                    }
                }
                finally
                {
                    FinishWriteResource();
                }

                return;
            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///   Removes an outgoing edge
        /// </summary>
        /// <param name="toBeRemovedEdge"> The to be removed edge </param>
        /// <returns> The edge property identifier where the edge was deleted </returns>
        internal List<UInt16> RemoveOutGoingEdge(EdgeModel toBeRemovedEdge)
        {
            if (WriteResource())
            {
                try
                {
                    var result = new List<UInt16>();

                    if (_outEdges == null)
                    {
                        return result;
                    }

                    for (var i = 0; i < _outEdges.Count; i++)
                    {
                        if (_outEdges[i].Edges.RemoveAll(_ => _.Id == toBeRemovedEdge.Id) > 0)
                        {
                            result.Add(_outEdges[i].EdgePropertyId);
                        }
                    }

                    return result;
                }
                finally
                {
                    FinishWriteResource();
                }

            }

            throw new CollisionException(this);
		}

		#endregion

		#region IVertexModel implementation

		public uint GetInDegree ()
		{
			if (ReadResource())
            {
                try
                {
                    UInt32 degree = 0;

                    if (_inEdges != null)
                    {
                        for (var i = 0; i < _inEdges.Count; i++)
                        {
                            degree += Convert.ToUInt32(_inEdges[i].Edges.Count);
                        }
                    }
                    return degree;
                }
                finally
                {
                    FinishReadResource();
                }

            }

            throw new CollisionException(this);
		}

		public uint GetOutDegree ()
		{
			if (ReadResource())
            {
                try
                {
                    UInt32 degree = 0;

                    if (_outEdges != null)
                    {
                        for (var i = 0; i < _outEdges.Count; i++)
                        {
                            degree += Convert.ToUInt32(_outEdges[i].Edges.Count);
                        }
                    }
                    return degree;
                }
                finally
                {
                    FinishReadResource();
                }

            }

            throw new CollisionException(this);
		}

        /// <summary>
        ///   Gets all neighbors.
        /// </summary>
        /// <returns> The neighbors. </returns>
        public List<VertexModel> GetAllNeighbors()
        {
            if (ReadResource())
            {
                try
                {
                    var neighbors = new List<VertexModel>();

                    if (_outEdges != null)
                    {
                        for (var i = 0; i < _outEdges.Count; i++)
                        {
                            neighbors.AddRange(_outEdges[i].Edges.Select(TargetVertexExtractor));
                        }
                    }

                    if (_inEdges != null)
                    {
                        for (var i = 0; i < _inEdges.Count; i++)
                        {
                            neighbors.AddRange(_inEdges[i].Edges.Select(SourceVertexExtractor));
                        }
                    }
                    return neighbors;
                }
                finally
                {
                    FinishReadResource();
                }
            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///   Gets the incoming edge identifiers.
        /// </summary>
        /// <returns> The incoming edge identifiers. </returns>
        public List<UInt16> GetIncomingEdgeIds()
        {
            if (ReadResource())
            {
                try
                {
                    var inEdges = new List<UInt16>();

                    if (_inEdges != null)
                    {
                        inEdges.AddRange(_inEdges.Select(_ => _.EdgePropertyId));
                    }
                    return inEdges;
                }
                finally
                {
                    FinishReadResource();
                }
            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///   Gets the outgoing edge identifiers.
        /// </summary>
        /// <returns> The outgoing edge identifiers. </returns>
        public List<UInt16> GetOutgoingEdgeIds()
        {
            if (ReadResource())
            {
                try
                {
                    var outEdges = new List<UInt16>();

                    if (_outEdges != null)
                    {
                        outEdges.AddRange(_outEdges.Select(_ => _.EdgePropertyId));
                    }
                    return outEdges;
                }
                finally
                {
                    FinishReadResource();
                }

            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///   Tries to get an out edge.
        /// </summary>
        /// <returns> <c>true</c> if something was found; otherwise, <c>false</c> . </returns>
        /// <param name='result'> Result. </param>
        /// <param name='edgePropertyId'> Edge property identifier. </param>
        public Boolean TryGetOutEdge(out ReadOnlyCollection<EdgeModel> result, UInt16 edgePropertyId)
        {
            if (ReadResource())
            {
                try
                {
                    var foundSth = false;
                    result = null;

                    if (_outEdges != null)
                    {
                        for (var i = 0; i < _outEdges.Count; i++)
                        {
                            var aOutEdge = _outEdges[i];
                            if (aOutEdge.EdgePropertyId == edgePropertyId)
                            {
                                result = aOutEdge.Edges.AsReadOnly();
                                foundSth = true;
                                break;
                            }
                        }
                    }

                    return foundSth;
                }
                finally
                {
                    FinishReadResource();
                }

            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///   Tries to get in edges.
        /// </summary>
        /// <returns> <c>true</c> if something was found; otherwise, <c>false</c> . </returns>
        /// <param name='result'> Result. </param>
        /// <param name='edgePropertyId'> Edge property identifier. </param>
        public Boolean TryGetInEdge(out ReadOnlyCollection<EdgeModel> result, UInt16 edgePropertyId)
        {
            if (ReadResource())
            {
                try
                {
                    result = null;
                    var foundSth = false;

                    if (_inEdges != null)
                    {
                        for (var i = 0; i < _inEdges.Count; i++)
                        {
                            var aInEdge = _inEdges[i];
                            if (aInEdge.EdgePropertyId == edgePropertyId)
                            {
                                result = aInEdge.Edges.AsReadOnly();
                                foundSth = true;
                                break;
                            }
                        }
                    }

                    return foundSth;
                }
                finally
                {
                    FinishReadResource();
                }

            }

            throw new CollisionException(this);
		}

		#endregion

		#region AGraphElement

        /// <summary>
        ///   The overide of the trim method
        /// </summary>
        internal override void Trim()
        {
            if (WriteResource())
            {
                try
                {
                    if (_outEdges != null)
                    {
                        _outEdges.TrimExcess();
                    }

                    if (_inEdges != null)
                    {
                        _inEdges.TrimExcess();
                    }
                }
                finally
                {
                    FinishWriteResource();
                }
                
                return;
            }

            throw new CollisionException(this);
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

			// If parameter cannot be cast to PathElement return false.
			var p = obj as VertexModel;

			return p != null && Equals(p);
		}

		public Boolean Equals(VertexModel p)
		{
			// If parameter is null return false:
			return (object)p != null && ReferenceEquals(this, p);
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

		#region misc overrides

		public override string ToString()
		{
			return Id.ToString(CultureInfo.InvariantCulture);
		}

		#endregion
		
		#region private helper
		
		/// <summary>
		/// Target vertex extractor.
		/// </summary>
		/// <returns>
		/// The target vertex.
		/// </returns>
		/// <param name='edge'>
		/// Edge.
		/// </param>
		private static VertexModel TargetVertexExtractor(EdgeModel edge)
		{
			return edge.TargetVertex;
		}
		
		/// <summary>
		/// Source vertex extractor.
		/// </summary>
		/// <returns>
		/// The source vertex.
		/// </returns>
		/// <param name='edge'>
		/// Edge.
		/// </param>
		private static VertexModel SourceVertexExtractor(EdgeModel edge)
		{
			return edge.SourceVertex;
		}
		
		#endregion
	}
}