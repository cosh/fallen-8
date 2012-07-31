// 
//  PathElementREST.cs
//  
//  Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
//  
//  Copyright (c) 2012 Henning Rauch
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
using System.Runtime.Serialization;
using NoSQL.GraphDB.Algorithms.Path;

#endregion

namespace NoSQL.GraphDB.Service.REST.Result
{
	/// <summary>
    /// The REST pendant to PathElement
    /// </summary>
    [DataContract]
    public sealed class PathElementREST
    {
		#region data

        /// <summary>
        /// The source vertex identifier
        /// </summary>
        [DataMember(IsRequired = true)]
		public Int32 SourceVertexId;

        /// <summary>
        /// The target vertex identifier
        /// </summary>
        [DataMember(IsRequired = true)]
        public Int32 TargetVertexId;

        /// <summary>
        /// The edge identifier
        /// </summary>
        [DataMember(IsRequired = true)]
        public Int32 EdgeId;

        /// <summary>
        /// The edge property identifier
        /// </summary>
        [DataMember(IsRequired = true)]
        public UInt16 EdgePropertyId;

        /// <summary>
        /// The direction of the edge
        /// </summary>
        [DataMember(IsRequired = true)]
        public Direction Direction;

        /// <summary>
        /// The weight of the pathelement
        /// </summary>
        [DataMember(IsRequired = true)]
		public double Weight;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new PathElementREST instance
        /// </summary>
        public PathElementREST(PathElement toBeTransferredResult)
        {
            SourceVertexId = toBeTransferredResult.SourceVertex.Id;
            TargetVertexId = toBeTransferredResult.TargetVertex.Id;
            EdgeId = toBeTransferredResult.Edge.Id;
            EdgePropertyId = toBeTransferredResult.EdgePropertyId;
            Direction = toBeTransferredResult.Direction;
            Weight = toBeTransferredResult.Weight;
        }

        #endregion
    }
}

