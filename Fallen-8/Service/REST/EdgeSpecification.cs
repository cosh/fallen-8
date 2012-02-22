// 
//  EdgeSpecification.cs
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
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Fallen8.API.Service.REST
{
	/// <summary>
    ///   The edge specification
    /// </summary>
    [DataContract]
    public sealed class EdgeSpecification
    {
        /// <summary>
        ///   The creation date
        /// </summary>
        [DataMember]
        public UInt32 CreationDate { get; set; }
		
		// <summary>
        ///   The source vertex
        /// </summary>
        [DataMember]
        public Int32 SourceVertex { get; set; }
		
		// <summary>
        ///   The target vertex
        /// </summary>
        [DataMember]
        public Int32 TargetVertex { get; set; }
		
        /// <summary>
        ///   The properties of the vertex
        /// </summary>
        [DataMember]
        public Dictionary<UInt16, PropertySpecification> Properties { get; set; }
    }
}

