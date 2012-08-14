// 
//  IAdminService.cs
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

#region Usings

using System;
using System.ComponentModel;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using NoSQL.GraphDB.Service.REST.Result;
using NoSQL.GraphDB.Service.REST.Specification;

#endregion

namespace NoSQL.GraphDB.Service.REST
{
    /// <summary>
    ///   The Fallen-8 admin service.
    /// </summary>
    [ServiceContract(Namespace = "Fallen-8", Name = "Fallen-8 admin service")]
    public interface IAdminService : IDisposable
    {
		#region services

		/// <summary>
        /// Starts a service 
		/// </summary>
        /// <param name="definition"> The service specification </param>
        /// <returns> True for success otherwise false </returns>
		[OperationContract(Name = "StartService")]
		[Description("Starts a service.")]
		[WebInvoke(
			UriTemplate = "/Service/Start", 
			Method = "POST", 
			RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
		bool CreateService(PluginSpecification definition);

		/// <summary>
        /// Deletes a service 
		/// </summary>
        /// <param name="definition"> The service delete specification </param>
        /// <returns> True for success otherwise false </returns>
        [OperationContract(Name = "DeleteService")]
		[Description("Deletes a service.")]
        [WebInvoke(UriTemplate = "/Service/Delete",
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
		bool DeleteService(ServiceDeleteSpecificaton definition);

		#endregion

        #region misc

        /// <summary>
        ///   Trims the database
        /// </summary>
        [OperationContract(Name = "Trim")]
		[Description("Trims the database.")]
        [WebGet(UriTemplate = "/Trim", ResponseFormat = WebMessageFormat.Json)]
        void Trim();

        /// <summary>
        ///   Status of the database
        /// </summary>
        /// <returns> The status </returns>
        [OperationContract(Name = "Status")]
		[Description("Status of the database.")]
        [WebGet(UriTemplate = "/Status", ResponseFormat = WebMessageFormat.Json)]
        StatusREST Status();

		/// <summary>
        /// Gets the number of vertices
        /// </summary>
        /// <returns> Number of vertices </returns>
        [OperationContract(Name = "VertexCount")]
		[Description("Gets the number of vertices.")]
        [WebGet(UriTemplate = "/Status/VertexCount", ResponseFormat = WebMessageFormat.Json)]
        UInt32 VertexCount();

		/// <summary>
        /// Gets the number of edges
        /// </summary>
        /// <returns> Number of edges </returns>
        [OperationContract(Name = "EdgeCount")]
		[Description("Gets the number of edges")]
        [WebGet(UriTemplate = "/Status/EdgeCount", ResponseFormat = WebMessageFormat.Json)]
        UInt32 EdgeCount();

		/// <summary>
        /// Gets the number of free bytes in RAM
        /// </summary>
        /// <returns> Number of free bytes </returns>
        [OperationContract(Name = "FreeMem")]
		[Description("Gets the number of free bytes in RAM.")]
        [WebGet(UriTemplate = "/Status/FreeMem", ResponseFormat = WebMessageFormat.Json)]
        UInt64 FreeMem();

        /// <summary>
        /// Put the database in its initial state (deletes all vertices and edges).
        /// </summary>
        [OperationContract(Name = "TabulaRasa")]
		[Description("Put the database in its initial state (deletes all vertices and edges).")]
        [WebGet(UriTemplate = "/TabulaRasa",
            ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        void TabulaRasa();


        #endregion

        #region persistence

        /// <summary>
        ///   Loads a Fallen-8
        /// </summary>
        /// <param name="startServices"> Start the services of the loaded save point? </param>
        [OperationContract(Name = "Load")]
		[Description("Loads a Fallen-8.")]
		[WebGet(UriTemplate = "/Load?startServices={startServices}")]
        void Load(string startServices);

        /// <summary>
        ///   Saves the Fallen-8
        /// </summary>
        [OperationContract(Name = "Save")]
		[Description("Saves the Fallen-8.")]
        [WebGet(UriTemplate = "/Save")]
        void Save();

        #endregion
    
		#region plugin

		/// <summary>
		/// Uploads a plugin.
		/// </summary>
		/// <param name='dllStream'>
		/// Dll stream.
		/// </param>
		[OperationContract(Name = "UploadPluginService")]
		[Description("Upload of a plugin.")]
        [WebInvoke(UriTemplate = "/Plugin/Upload",
            Method = "POST")]
		void UploadPlugin(Stream dllStream);

		#endregion
	}
}