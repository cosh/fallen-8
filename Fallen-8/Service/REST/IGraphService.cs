// 
//  IGraphService.cs
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
using System.ComponentModel;

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using NoSQL.GraphDB.Service.REST.Result;
using NoSQL.GraphDB.Service.REST.Specification;

#endregion

namespace NoSQL.GraphDB.Service.REST
{
    /// <summary>
    ///   The Fallen-8 graph service.
    /// </summary>
    [ServiceContract(Namespace = "Fallen-8", Name = "Fallen-8 graph service")]
    public interface IGraphService
    {
        #region Create/Add/Delete GRAPHELEMENT

        /// <summary>
        ///   Adds a vertex to the Fallen-8
        /// </summary>
        /// <param name="definition"> The vertex specification </param>
        /// <returns> The new vertex id </returns>
        [OperationContract(Name = "CreateVertex")]
		[Description("Adds a vertex to the Fallen-8.")]
        [WebInvoke(UriTemplate = "/Vertex/Create", Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Int32 AddVertex(VertexSpecification definition);

        /// <summary>
        ///   Adds an edge to the Fallen-8
        /// </summary>
        /// <param name="definition"> The edge specification </param>
        /// <returns> The new edge id </returns>
        [OperationContract(Name = "CreateEdge")]
		[Description("Adds an edge to the Fallen-8.")]
        [WebInvoke(UriTemplate = "/Edge/Create", Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Int32 AddEdge(EdgeSpecification definition);

        /// <summary>
        ///   Returns all graph element properties
        /// </summary>
        /// <param name="graphElementIdentifier"> The graph element identifier </param>
        /// <returns> PropertyName -> PropertyValue </returns>
        [OperationContract(Name = "GraphElementProperties")]
        [Description("Returns all graph element properties.")]
        [WebGet(UriTemplate = "/GraphElements/{graphElementIdentifier}/Properties", ResponseFormat = WebMessageFormat.Json)]
        PropertiesREST GetAllGraphelementProperties(String graphElementIdentifier);

		/// <summary>
        /// Tries to add a property to a graph element
        /// </summary>
        /// <param name="graphElementIdentifier"> The graph element identifier </param>
        /// <param name="propertyId"> The property identifier </param>
        /// <param name="definition"> The property specification </param>
        /// <returns> True for success, otherwise false </returns>
        [OperationContract(Name = "TryAddProperty")]
		[Description("Tries to add a property to a graph element.")]
        [WebInvoke(UriTemplate = "/GraphElements/{graphElementIdentifier}/TryAddProperty?propertyId={propertyId}", Method = "POST", 
		           RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean TryAddProperty(string graphElementIdentifier, string propertyId, PropertySpecification definition);

		/// <summary>
        /// Tries to delete a property from a graph element
        /// </summary>
        /// <param name="graphElementIdentifier"> The graph element identifier </param>
        /// <param name="propertyId"> The property identifier </param>
        /// <returns> True for success, otherwise false </returns>
        [OperationContract(Name = "TryDeleteProperty")]
		[Description("Tries to delete a property from a graph element.")]
        [WebInvoke(UriTemplate = "/GraphElements/{graphElementIdentifier}/TryDeleteProperty?propertyId={propertyId}", Method = "DELETE",
		           ResponseFormat = WebMessageFormat.Json)]
        Boolean TryRemoveProperty(string graphElementIdentifier, string propertyId);

		/// <summary>
        /// Tries to delete a graph element
        /// </summary>
        /// <param name="graphElementIdentifier"> The graph element identifier </param>
        /// <returns> True for success, otherwise false </returns>
        [OperationContract(Name = "TryDeleteGraphElement")]
		[Description("Tries to delete a graph element.")]
        [WebInvoke(UriTemplate = "/GraphElements/{graphElementIdentifier}/TryDelete", Method = "DELETE",
		           ResponseFormat = WebMessageFormat.Json)]
        Boolean TryRemoveGraphElement(string graphElementIdentifier);

        #endregion

		#region Create/Add/Delete INDEX

		/// <summary>
        /// Creates an index 
		/// </summary>
        /// <param name="definition"> The index specification </param>
        /// <returns> True for success otherwise false </returns>
        [OperationContract(Name = "CreateIndex")]
		[Description("Creates an index.")]
        [WebInvoke(
			UriTemplate = "/Index/Create", 
			Method = "POST", 
			RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
		bool CreateIndex(PluginSpecification definition);

		/// <summary>
        /// Updates an index 
		/// </summary>
        /// <param name="definition"> The update specification </param>
        /// <returns> True for success otherwise false </returns>
        [OperationContract(Name = "AddToIndex")]
		[Description("Updates an index.")]
        [WebInvoke(
			UriTemplate = "/Index/AddTo", 
			Method = "POST", 
			RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
		bool AddToIndex(IndexAddToSpecification definition);

		/// <summary>
        /// Deletes an index 
		/// </summary>
        /// <param name="definition"> The index delete specification </param>
        /// <returns> True for success otherwise false </returns>
        [OperationContract(Name = "DeleteIndex")]
		[Description("Deletes an index.")]
        [WebInvoke(
			UriTemplate = "/Index/Delete", 
			Method = "DELETE")]
		bool DeleteIndex(IndexDeleteSpecificaton definition);

		/// <summary>
        /// Deletes a key from an index 
		/// </summary>
        /// <param name="definition"> The index delete specification </param>
        /// <returns> True for success otherwise false </returns>
        [OperationContract(Name = "DeleteKeyFromIndex")]
		[Description("Deletes a key from an index.")]
        [WebInvoke(
			UriTemplate = "/Index/DeleteKey", 
			Method = "DELETE")]
		bool RemoveKeyFromIndex (IndexRemoveKeyFromIndexSpecification definition);

		/// <summary>
        /// Deletes a graph element from an index 
		/// </summary>
        /// <param name="definition"> The index delete specification </param>
        /// <returns> True for success otherwise false </returns>
        [OperationContract(Name = "RemoveGraphElementFromIndex")]
		[Description("Deletes a graph element from an index.")]
        [WebInvoke(
			UriTemplate = "/Index/DeleteGraphElement", 
			Method = "DELETE")]
		bool RemoveGraphElementFromIndex (IndexRemoveGraphelementFromIndexSpecification definition);

		#endregion

        #region Read

        /// <summary>
        ///  Returns the source vertex of the edge
        /// </summary>
        /// <param name="edgeIdentifier"> The edge identifier </param>
        /// <returns> source vertex id </returns>
        [OperationContract(Name = "EdgeSourceVertex")]
        [Description("Returns the source vertex of the edge.")]
        [WebGet(UriTemplate = "/Edges/{edgeIdentifier}/Source", ResponseFormat = WebMessageFormat.Json)]
        Int32 GetSourceVertexForEdge(String edgeIdentifier);

        /// <summary>
        ///  Returns the target vertex of the edge
        /// </summary>
        /// <param name="edgeIdentifier"> The edge identifier </param>
        /// <returns> target vertex id </returns>
        [OperationContract(Name = "EdgeTargetVertex")]
        [Description("Returns the target vertex of the edge.")]
        [WebGet(UriTemplate = "/Edges/{edgeIdentifier}/Target", ResponseFormat = WebMessageFormat.Json)]
        Int32 GetTargetVertexForEdge(String edgeIdentifier);

        /// <summary>
        ///   Returns all available outgoing edges for a given vertex
        /// </summary>
        /// <param name="vertexIdentifier"> The vertex identifier </param>
        /// <returns> List of available incoming edge property ids </returns>
        [OperationContract(Name = "AvailableOutEdges")]
		[Description("Returns all available outgoing edges for a given vertex.")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/AvailableOutEdges", ResponseFormat = WebMessageFormat.Json)]
        List<UInt16> GetAllAvailableOutEdgesOnVertex(String vertexIdentifier);

        /// <summary>
        ///   Returns all available incoming edges for a given vertex
        /// </summary>
        /// <param name="vertexIdentifier"> The vertex identifier </param>
        /// <returns> List of available incoming edge property ids </returns>
        [OperationContract(Name = "AvailableInEdges")]
		[Description("Returns all available incoming edges for a given vertex.")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/AvailableInEdges", ResponseFormat = WebMessageFormat.Json)]
        List<UInt16> GetAllAvailableIncEdgesOnVertex(String vertexIdentifier);

        /// <summary>
        ///   Returns all outgoing edges for a given edge property
        /// </summary>
        /// <param name="vertexIdentifier"> The vertex identifier </param>
        /// <param name="edgePropertyIdentifier"> The edge property identifier </param>
        /// <returns> List of edge ids </returns>
        [OperationContract(Name = "OutEdges")]
		[Description("Returns all outgoing edges for a given edge property.")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/OutEdges/{edgePropertyIdentifier}",
            ResponseFormat = WebMessageFormat.Json)]
        List<Int32> GetOutgoingEdges(String vertexIdentifier, String edgePropertyIdentifier);

        /// <summary>
        ///   Returns all incoming edges for a given edge property
        /// </summary>
        /// <param name="vertexIdentifier"> The vertex identifier </param>
        /// <param name="edgePropertyIdentifier"> The edge property identifier </param>
        /// <returns> List of edge ids </returns>
        [OperationContract(Name = "IncEdges")]
		[Description("Returns all incoming edges for a given edge property.")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/IncEdges/{edgePropertyIdentifier}",
            ResponseFormat = WebMessageFormat.Json)]
        List<Int32> GetIncomingEdges(String vertexIdentifier, String edgePropertyIdentifier);

		/// <summary>
        ///   Returns the in-degree of the vertex
        /// </summary>
        /// <param name="vertexIdentifier"> The vertex identifier </param>
        /// <returns> In-degree </returns>
        [OperationContract(Name = "InDegree")]
		[Description("Returns the in-degree of the vertex.")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/InDegree",
            ResponseFormat = WebMessageFormat.Json)]
        UInt32 GetInDegree(String vertexIdentifier);

		/// <summary>
        ///   Returns the out-degree of the vertex
        /// </summary>
        /// <param name="vertexIdentifier"> The vertex identifier </param>
        /// <returns> In-degree </returns>
        [OperationContract(Name = "OutDegree")]
		[Description("Returns the out-degree of the vertex.")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/OutDegree",
            ResponseFormat = WebMessageFormat.Json)]
        UInt32 GetOutDegree(String vertexIdentifier);

		/// <summary>
        ///   Returns the degree of an incoming edge
        /// </summary>
        /// <param name="vertexIdentifier"> The vertex identifier </param>
        /// <param name="edgePropertyIdentifier"> The edge property identifier </param>
        /// <returns> Degree of an incoming edge </returns>
        [OperationContract(Name = "IncEdgesDegree")]
		[Description("Returns the degree of an incoming edge.")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/IncEdges/{edgePropertyIdentifier}/Degree",
            ResponseFormat = WebMessageFormat.Json)]
        UInt32 GetInEdgeDegree(String vertexIdentifier, String edgePropertyIdentifier);

		/// <summary>
        ///   Returns the degree of an outgoing edge
        /// </summary>
        /// <param name="vertexIdentifier"> The vertex identifier </param>
        /// <param name="edgePropertyIdentifier"> The edge property identifier </param>
        /// <returns> Degree of an incoming edge </returns>
        [OperationContract(Name = "OutEdgesDegree")]
		[Description("Returns the degree of an outgoing edge.")]
        [WebGet(UriTemplate = "/Vertices/{vertexIdentifier}/OutEdges/{edgePropertyIdentifier}/Degree",
            ResponseFormat = WebMessageFormat.Json)]
        UInt32 GetOutEdgeDegree(String vertexIdentifier, String edgePropertyIdentifier);

        #endregion

        #region scan

        /// <summary>
        ///   Full graph scan for graph elements
        /// </summary>
        /// <param name="propertyId"> The property identifier </param>
        /// <param name="definition"> The scan specification </param>
        /// <returns> The matching identifier </returns>
        [OperationContract(Name = "GraphScan")]
		[Description("Full graph scan for graph elements.")]
        [WebInvoke(UriTemplate = "/Scan/Graph?propertyId={propertyId}", Method = "POST",
		           RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<Int32> GraphScan(String propertyId, ScanSpecification definition);

        /// <summary>
        ///   Index scan for graph elements
        /// </summary>
        /// <param name="definition"> The scan specification </param>
        /// <returns> The matching identifier </returns>
        [OperationContract(Name = "IndexScan")]
		[Description("Index scan for graph elements.")]
        [WebInvoke(UriTemplate = "/Scan/Index", Method = "POST", 
		           RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<Int32> IndexScan(IndexScanSpecification definition);

        /// <summary>
        ///   Scan for graph elements by a specified property range.
        /// </summary>
        /// <param name="definition"> The scan specification </param>
        /// <returns> The matching identifier </returns>
        [OperationContract(Name = "RangeIndexScan")]
		[Description("Scan for graph elements by a specified property range.")]
        [WebInvoke(UriTemplate = "/Scan/Index/Range", Method = "POST",
		           RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<Int32> RangeIndexScan(RangeIndexScanSpecification definition);

		/// <summary>
        ///   Fulltext scan for graph elements.
        /// </summary>
        /// <param name="definition"> The scan specification </param>
        /// <returns> The matching identifier </returns>
        [OperationContract(Name = "FulltextIndexScan")]
		[Description("Fulltext scan for graph elements.")]
        [WebInvoke(UriTemplate = "/Scan/Index/Fulltext", Method = "POST",
		           RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        FulltextSearchResultREST FulltextIndexScan(FulltextIndexScanSpecification definition);

		/// <summary>
        ///   Spatial index scan for graph elements. Finds all objects in a certain distance to a given graph element
        /// </summary>
        /// <param name="definition"> The search distance specification </param>
        /// <returns> The matching identifier </returns>
        [OperationContract(Name = "SpatialIndexScan")]
		[Description("Spatial index scan for graph elements. Finds all objects in a certain distance to a given graph element.")]
        [WebInvoke(UriTemplate = "/Scan/Index/Spatial/SearchDistance", Method = "POST",
		           RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<Int32> SpatialIndexScanSearchDistance(SearchDistanceSpecification definition);

        #endregion

        #region path

        /// <summary>
        /// Path traverser
        /// </summary>
        [OperationContract(Name = "Paths")]
		[Description("Path traverser.")]
        [WebInvoke(
            UriTemplate = "/Scan/Paths?from={from}&to={to}",
            Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<PathREST> GetPaths(String from, String to, PathSpecification definition);


        /// <summary>
        ///   Path traverser starting at a given vertex.
        /// </summary>
        /// <param name="vertexIdentifier"> The vertex identifier </param>
        /// <param name="to">The destination</param>
        /// <param name="definition">The definition of the path traversal</param>
        /// <returns> PropertyName -> PropertyValue </returns>
        [OperationContract(Name = "PathFromVertex")]
		[Description("Path traverser starting at a given vertex.")]
        [WebInvoke(
			UriTemplate = "/Vertices/{vertexIdentifier}/Paths?to={to}", 
			Method = "POST", RequestFormat = WebMessageFormat.Json, 
			ResponseFormat = WebMessageFormat.Json)]
        List<PathREST> GetPathsByVertex(String vertexIdentifier, String to, PathSpecification definition);

        #endregion
    }
}