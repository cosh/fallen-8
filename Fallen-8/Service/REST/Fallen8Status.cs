// 
//  Fallen8Status.cs
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

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NoSQL.GraphDB.Service.REST
{
    /// <summary>
    ///   The Fallen-8 status
    /// </summary>
    [DataContract]
    public sealed class Fallen8Status
    {
        /// <summary>
        ///   The available memory
        /// </summary>
        [DataMember]
        public UInt64 FreeMemory { get; set; }

        /// <summary>
        ///   The used memory
        /// </summary>
        [DataMember]
        public UInt64 UsedMemory { get; set; }

        /// <summary>
        ///   Vertex count
        /// </summary>
        [DataMember]
        public UInt32 VertexCount { get; set; }

        /// <summary>
        ///   Edge count
        /// </summary>
        [DataMember]
        public UInt32 EdgeCount { get; set; }

        /// <summary>
        ///   Available index plugins
        /// </summary>
        [DataMember]
        public List<String> AvailableIndexPlugins { get; set; }

        /// <summary>
        ///   Available path plugins
        /// </summary>
        [DataMember]
        public List<String> AvailablePathPlugins { get; set; }

        /// <summary>
        ///   Available index plugins
        /// </summary>
        [DataMember]
        public List<String> AvailableServicePlugins { get; set; }
    }
}