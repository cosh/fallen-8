// 
//  IFallen8RESTService.cs
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
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;

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
		Fallen8RESTProperties GetAllVertexProperties(String vertexIdentifier);
        
		/// <summary>
        /// Returns all edge properties
        /// </summary>
        /// <param name="edgeIdentifier">The edge identifier</param>
        /// <returns>PropertyId -> PropertyValue</returns>
        [OperationContract(Name = "EdgeProperties")]
        [WebGet(UriTemplate = "/Edges/{edgeIdentifier}/Properties", ResponseFormat = WebMessageFormat.Json)]
        Fallen8RESTProperties GetAllEdgeProperties(String edgeIdentifier);
		
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
        [OperationContract(Name = "AvailableInEdges")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/AvailableInEdges", ResponseFormat = WebMessageFormat.Json)]
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
		
		#region frontend
		
		/// <summary>
		/// Gets the frontend.
		/// </summary>
		/// <returns>
		/// The frontend.
		/// </returns>
		[OperationContract(Name = "Frontend")]
        [WebGet(UriTemplate = "/Frontend")]
        Stream GetFrontend();

        /// <summary>
        /// Reload the frontend.
        /// </summary>
        [OperationContract(Name = "ReloadFrontend")]
        [WebGet(UriTemplate = "/ReloadFrontend")]
        void ReloadFrontend();
		
		/// <summary>
        /// Gets the frontend ressources.
        /// </summary>
        /// <returns>
        /// The frontend ressources.
        /// </returns>
        /// <param name='ressourceName'>
        /// Ressource name.
        /// </param>
		[OperationContract(Name = "FrontendRessource")]
        [WebGet(UriTemplate = "/Frontend/Ressource/{ressourceName}")]
		Stream GetFrontendRessources(String ressourceName);
		
		#endregion

        #region scan

        /// <summary>
        /// Full graph scan for graph elements
        /// </summary>
        /// <param name="propertyId"> The property identifier </param>
        /// <param name="definition"> The scan specification </param>
        /// <returns> The matching identifier </returns>
        [OperationContract(Name = "GraphScan")]
        [WebInvoke(UriTemplate = "/GraphScan?propertyId={propertyId}", Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<Int32> GraphScan(String propertyId, ScanSpecification definition);

        /// <summary>
        /// Index scan for graph elements
        /// </summary>
        /// <param name="indexId"> The index identifier </param>
        /// <param name="definition"> The scan specification </param>
        /// <returns> The matching identifier </returns>
        [OperationContract(Name = "IndexScan")]
        [WebInvoke(UriTemplate = "/IndexScan?indexId={indexId}", Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<Int32> IndexScan(String indexId, ScanSpecification definition);

        /// <summary>
        /// Scan for graph elements by a specified property range.
        /// </summary>
        /// <param name="indexId"> The index identifier </param>
        /// <param name="definition"> The scan specification </param>
        /// <returns> The matching identifier </returns>
        [OperationContract(Name = "RangeIndexScan")]
        [WebInvoke(UriTemplate = "/RangeIndexScan?indexId={indexId}", Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<Int32> RangeIndexScan(String indexId, RangeScanSpecification definition);

        #endregion

        #region persistence
        #endregion
    }
}

