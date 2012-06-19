// 
// EdgeContainer.cs
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

#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace NoSQL.GraphDB.Model
{
    /// <summary>
    /// edge container.
    /// </summary>
    public struct EdgeContainer
    {
        #region Data
        
        /// <summary>
        /// Gets or sets the edge property identifier.
        /// </summary>
        /// <value>
        /// The edge property identifier.
        /// </value>
        public UInt16 EdgePropertyId { get; private set; }
        
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public List<EdgeModel> Edges { get; private set; }
        
        #endregion

        #region constructor

        public EdgeContainer(UInt16 edgePropertyId, List<EdgeModel> edges) : this()
        {
            EdgePropertyId = edgePropertyId;
            Edges = edges;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return EdgePropertyId + ": |E|=" + Edges.Count;
        }

        #endregion
    }
}

