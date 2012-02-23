// 
//  IFallen8RESTService.cs
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
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Fallen8.API.Service.REST
{
	/// <summary>
	/// The Fallen-8 REST service.
	/// </summary>
    [ServiceContract(Namespace = "Fallen-8", Name = "Fallen-8RESTService")]
	public interface IFallen8RESTService
	{
		#region Import
       
		/// <summary>
        ///   Adds a vertex to the Fallen-8
        /// </summary>
        /// <param name="definition"> The vertex specification </param>
        /// <returns> The new vertex id </returns>
        [OperationContract(Name = "AddVertex")]
        [WebInvoke(UriTemplate = "/AddVertex", Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Int32 AddVertex(VertexSpecification definition);

        /// <summary>
        ///   Adds an edge to the Fallen-8
        /// </summary>
        /// <param name="definition"> The edge specification </param>
        /// <returns> The new edge id </returns>
        [OperationContract(Name = "AddEdge")]
        [WebInvoke(UriTemplate = "/AddEdge", Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Int32 AddEdge(EdgeSpecification definition);

		#endregion
		
		#region Selection
		
		/// <summary>
        /// Returns all vertex properties
        /// </summary>
        /// <param name="vertexIdentifier">The vertex identifier</param>
        /// <returns>PropertyName -> PropertyValue</returns>
        [OperationContract(Name = "VertexProperties")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/Properties", ResponseFormat = WebMessageFormat.Json)]
        Dictionary<UInt16, String> GetAllVertexProperties(String vertexIdentifier);
        
		/// <summary>
        /// Returns all edge properties
        /// </summary>
        /// <param name="edgeIdentifier">The edge identifier</param>
        /// <returns>PropertyId -> PropertyValue</returns>
        [OperationContract(Name = "VertexProperties")]
        [WebGet(UriTemplate = "/Edges/{edgeIdentifier}/Properties", ResponseFormat = WebMessageFormat.Json)]
        Dictionary<UInt16, String> GetAllEdgeProperties(String edgeIdentifier);
		
        /// <summary>
        /// Returns all available outgoing edges for a given vertex
        /// </summary>
        /// <param name="vertexIdentifier">The vertex identifier</param>
        /// <returns>List of available incoming edge property ids</returns>
        [OperationContract(Name = "AvailableOutEdges")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/AvailableOutEdges", ResponseFormat = WebMessageFormat.Json)]
        List<UInt16> GetAllAvailableOutEdgesOnVertex(String vertexIdentifier);
		
		/// <summary>
        /// Returns all available incoming edges for a given vertex
        /// </summary>
        /// <param name="vertexIdentifier">The vertex identifier</param>
        /// <returns>List of available incoming edge property ids</returns>
        [OperationContract(Name = "AvailableIncEdges")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/AvailableOutEdges", ResponseFormat = WebMessageFormat.Json)]
        List<UInt16> GetAllAvailableIncEdgesOnVertex(String vertexIdentifier);
		
        /// <summary>
        /// Returns all outgoing edges for a given edge property
        /// </summary>
        /// <param name="vertexIdentifier">The vertex identifier</param>
        /// <param name="edgePropertyIdentifier">The edge property identifier</param>
        /// <returns>List of edge ids</returns>
        [OperationContract(Name = "OutEdges")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/OutEdges/{edgePropertyIdentifier}", ResponseFormat = WebMessageFormat.Json)]
        List<Int32> GetOutgoingEdges(String vertexIdentifier, String edgePropertyIdentifier);

        /// <summary>
        /// Returns all incoming edges for a given edge property
        /// </summary>
        /// <param name="vertexIdentifier">The vertex identifier</param>
        /// <param name="edgePropertyIdentifier">The edge property identifier</param>
        /// <returns>List of edge ids</returns>
        [OperationContract(Name = "IncEdges")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/IncEdges/{edgePropertyIdentifier}", ResponseFormat = WebMessageFormat.Json)]
        List<Int32> GetIncomingEdges(String vertexIdentifier, String edgePropertyIdentifier);

		#endregion
		
        #region misc

        /// <summary>
        /// Trims the database
        /// </summary>
        [OperationContract(Name = "Trim")]
        [WebGet(UriTemplate = "/Trim", ResponseFormat = WebMessageFormat.Json)]
        void Trim();
		
		/// <summary>
        /// Status of the database
        /// </summary>
        /// <returns>The status</returns>
        [OperationContract(Name = "Status")]
        [WebGet(UriTemplate = "/Status", ResponseFormat = WebMessageFormat.Json)]
        Fallen8Status Status();

        #endregion
	}
}

