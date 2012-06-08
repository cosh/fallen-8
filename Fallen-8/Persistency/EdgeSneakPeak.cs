// 
// EdgeSneakPeak.cs
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
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Persistency
{
    /// <summary>
    /// Edge sneak peak.
    /// </summary>
    public struct EdgeSneakPeak
    {
        /// <summary>
        /// The identifier of the edge.
        /// </summary>
        public Int32 Id;
        
        /// <summary>
        /// The creation date.
        /// </summary>
        public UInt32 CreationDate;
        
        /// <summary>
        /// The modification date.
        /// </summary>
        public UInt32 ModificationDate;
        
        /// <summary>
        /// The properties.
        /// </summary>
        public PropertyContainer[] Properties;
        
        /// <summary>
        /// The source vertex identifier.
        /// </summary>
        public Int32 SourceVertexId;
        
        /// <summary>
        /// The target vertex identifier.
        /// </summary>
        public Int32 TargetVertexId;
    }
}

