// 
//  Fallen8RESTService.cs
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

namespace Fallen8.API.Service.REST
{
	/// <summary>
	/// Fallen-8 REST service.
	/// </summary>
	public sealed class Fallen8RESTService : IFallen8RESTService, IDisposable
	{
		#region Data

        /// <summary>
        ///   The internal Fallen-8 instance
        /// </summary>
        private readonly Fallen8 _fallen8;
		
		#endregion
		
		#region Constructor
		
		/// <summary>
		/// Initializes a new instance of the Fallen8RESTService class.
		/// </summary>
		/// <param name='fallen8'>
		/// Fallen-8.
		/// </param>
		public Fallen8RESTService(Fallen8 fallen8)
		{
			_fallen8 = fallen8;
		}
		
		#endregion
		
		#region IDisposable Members

        public void Dispose()
        {
			//do nothing atm
        }

        #endregion

		#region IFallen8RESTService implementation
		public int AddVertex (VertexSpecification definition)
		{
			throw new NotImplementedException ();
		}

		public int AddEdge (EdgeSpecification definition)
		{
			throw new NotImplementedException ();
		}

		public Dictionary<ushort, string> GetAllVertexProperties (string vertexIdentifier)
		{
			throw new NotImplementedException ();
		}

		public Dictionary<ushort, string> GetAllEdgeProperties (string edgeIdentifier)
		{
			throw new NotImplementedException ();
		}

		public List<ushort> GetAllAvailableOutEdgesOnVertex (string vertexIdentifier)
		{
			throw new NotImplementedException ();
		}

		public List<ushort> GetAllAvailableIncEdgesOnVertex (string vertexIdentifier)
		{
			throw new NotImplementedException ();
		}

		public List<int> GetOutgoingEdges (string vertexIdentifier, string edgePropertyIdentifier)
		{
			throw new NotImplementedException ();
		}

		public List<int> GetIncomingEdges (string vertexIdentifier, string edgePropertyIdentifier)
		{
			throw new NotImplementedException ();
		}

		public List<int> SearchVertices (string typeName, string propertyId, string propertyValue)
		{
			throw new NotImplementedException ();
		}

		public long Trim ()
		{
			throw new NotImplementedException ();
		}

		public Fallen8Status Status ()
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

