// 
//  Fallen8Status.cs
//  
//  Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
//  
//  Copyright (c) 2012 Henning Rauch
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, version 3 of the License.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fallen8.API.Service
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
        public Int64 FreeMemory { get; set; }
		
		/// <summary>
        ///   The used memory
        /// </summary>
        [DataMember]
        public Int64 UsedMemory { get; set; }
		
		/// <summary>
        /// Vertex count
        /// </summary>
        [DataMember]
        public UInt32 VertexCount { get; set; }
		
		/// <summary>
        /// Edge count
        /// </summary>
        [DataMember]
        public UInt32 EdgeCount { get; set; }
		
		/// <summary>
        /// Available index plugins
        /// </summary>
        [DataMember]
        public List<String> AvailableIndexPlugins { get; set; }
		
		/// <summary>
        /// Available path plugins
        /// </summary>
        [DataMember]
        public List<String> AvailablePathPlugins { get; set; }
		
		/// <summary>
        /// Available index plugins
        /// </summary>
        [DataMember]
        public List<String> AvailableServicePlugins { get; set; }
    }
}