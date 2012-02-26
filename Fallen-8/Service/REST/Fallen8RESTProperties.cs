// 
//  Fallen8RESTProperties.cs
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
    ///   The Fallen-8 REST properties
    /// </summary>
    [DataContract]
    public sealed class Fallen8RESTProperties
    {
        /// <summary>
        ///   The creation date
        /// </summary>
        [DataMember]
        public DateTime CreationDate { get; set; }
		
		/// <summary>
        ///   The modification date
        /// </summary>
        [DataMember]
        public DateTime ModificationDate { get; set; }
		
        /// <summary>
        ///   The properties
        /// </summary>
        [DataMember]
        public Dictionary<UInt16, String> Properties { get; set; }
    }
}