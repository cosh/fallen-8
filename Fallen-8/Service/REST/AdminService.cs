// 
//  AdminService.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using NoSQL.GraphDB.Algorithms.Path;
using NoSQL.GraphDB.Helper;
using NoSQL.GraphDB.Index;
using NoSQL.GraphDB.Plugin;
using NoSQL.GraphDB.Service.REST.Ressource;
using NoSQL.GraphDB.Service.REST.Result;
using NoSQL.GraphDB.Service.REST.Specification;

#endregion

namespace NoSQL.GraphDB.Service.REST
{
    /// <summary>
    ///   Fallen-8 AdminService REST service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    public sealed class AdminService : IAdminService, IDisposable
    {
        #region Data

        /// <summary>
        ///   The internal Fallen-8 instance
        /// </summary>
        private readonly NoSQL.GraphDB.Fallen8 _fallen8;

        /// <summary>
        ///   The ressources.
        /// </summary>
        private Dictionary<String, MemoryStream> _ressources;

        /// <summary>
        ///   The html befor the code injection
        /// </summary>
        private String _frontEndPre;

        /// <summary>
        ///   The html after the code injection
        /// </summary>
        private String _frontEndPost;

        /// <summary>
        /// The Fallen-8 save path
        /// </summary>
        private String _savePath;

        /// <summary>
        /// The Fallen-8 save file
        /// </summary>
        private String _saveFile;

        /// <summary>
        /// The optimal number of partitions
        /// </summary>
        private UInt32 _optimalNumberOfPartitions;

        #endregion

        #region Constructor

        /// <summary>
        ///   Initializes a new instance of the AdminService class.
        /// </summary>
        /// <param name='fallen8'> Fallen-8. </param>
        public AdminService(Fallen8 fallen8)
        {
            _fallen8 = fallen8;
            LoadFrontend();

            _saveFile = "Temp.f8s";
            _savePath = Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + _saveFile;

            _optimalNumberOfPartitions = Convert.ToUInt32(Environment.ProcessorCount * 3 / 2);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //do nothing atm
        }

        #endregion

        #region IAdminService implementation
        
        public void Trim()
        {
            _fallen8.Trim();
        }

        public StatusREST Status()
        {
			var freeBytesOfMemory = GetFreeMemory();
			var totalBytesOfMemoryUsed = GetTotalMemory() - freeBytesOfMemory;

            var vertexCount = _fallen8.VertexCount;
            var edgeCount = _fallen8.EdgeCount;

            IEnumerable<String> availableIndices;
            PluginFactory.TryGetAvailablePlugins<IIndex>(out availableIndices);

            IEnumerable<String> availablePathAlgos;
            PluginFactory.TryGetAvailablePlugins<IShortestPathAlgorithm>(out availablePathAlgos);

            IEnumerable<String> availableServices;
            PluginFactory.TryGetAvailablePlugins<IService>(out availableServices);

            return new StatusREST
                       {
                           AvailableIndexPlugins = new List<String>(availableIndices),
                           AvailablePathPlugins = new List<String>(availablePathAlgos),
                           AvailableServicePlugins = new List<String>(availableServices),
                           EdgeCount = edgeCount,
                           VertexCount = vertexCount,
                           UsedMemory = totalBytesOfMemoryUsed,
                           FreeMemory = freeBytesOfMemory
                       };
        }

        public Stream GetFrontend()
        {
            if (WebOperationContext.Current != null)
            {
                var baseUri = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri;

                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

                var sb = new StringBuilder();

                sb.Append(_frontEndPre);
                sb.Append(Environment.NewLine);
                sb.AppendLine("var baseUri = \"" + baseUri + "\";" + Environment.NewLine);
                sb.Append(_frontEndPost);

                return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
            }

            return new MemoryStream(Encoding.UTF8.GetBytes("Sorry, no frontend available."));
        }

        public void ReloadFrontend()
        {
            LoadFrontend();
        }

        public Stream GetFrontendRessources(String ressourceName)
        {
            MemoryStream ressourceStream;
            if (_ressources.TryGetValue(ressourceName, out ressourceStream))
            {
                var result = new MemoryStream();
                var buffer = new byte[32768];
                int read;
                while ((read = ressourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    result.Write(buffer, 0, read);
                }
                ressourceStream.Position = 0;
                result.Position = 0;

                if (WebOperationContext.Current != null)
                {
                    var extension = ressourceName.Split('.').Last();

                    switch (extension)
                    {
                        case "html":
                        case "htm":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                            break;
                        case "png":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "image/png";
                            break;
                        case "css":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "text/css";
                            break;
                        case "gif":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "image/gif";
                            break;
                        case "ico":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "image/ico";
                            break;
                        case "swf":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-shockwave-flash";
                            break;
                        case "js":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "text/javascript";
                            break;
                        default:
                            throw new ApplicationException(String.Format("File type {0} not supported", extension));
                    }
                }

                return result;
            }

            return null;
        }
        
        public void Load(string startServices)
        {
            _fallen8.Load(FindLatestFallen8(), Convert.ToBoolean(startServices));
        }

        public void Save()
        {
            _fallen8.Save(_savePath, _optimalNumberOfPartitions);
        }

		public void TabulaRasa ()
		{
			_fallen8.TabulaRasa();
		}

		public uint VertexCount ()
		{
			return _fallen8.VertexCount;
		}

		public uint EdgeCount ()
		{
			return _fallen8.EdgeCount;
		}

		public UInt64 FreeMem ()
		{
			return GetFreeMemory();
		}
		
		public bool CreateService (PluginSpecification definition)
		{
			IService service;
            return _fallen8.ServiceFactory.TryAddService(out service, definition.PluginType, definition.UniqueId, ServiceHelper.CreatePluginOptions(definition.PluginOptions));
		}

		public bool DeleteService (ServiceDeleteSpecificaton definition)
		{
			return _fallen8.ServiceFactory.Services.Remove(definition.ServiceId);
		}

		#endregion

        #region private helper

        /// <summary>
        /// Get the free memory of the system
        /// </summary>
        /// <returns>Free memory in bytes</returns>
		private UInt64 GetFreeMemory ()
		{
			#if __MonoCS__
    			//mono specific code
				var pc = new PerformanceCounter("Mono Memory", "Total Physical Memory");
            	var totalMemory = (ulong)pc.RawValue;

				Process.GetCurrentProcess().Refresh();
            	var usedMemory = (ulong)Process.GetCurrentProcess().WorkingSet64;

				return totalMemory - usedMemory;
			#else
                var freeMem = new PerformanceCounter("Memory", "Available Bytes");
            	return Convert.ToUInt64(freeMem.NextValue());
			#endif
		}

        /// <summary>
        /// Gets the total memory of the system
        /// </summary>
        /// <returns>Total memory in bytes</returns>
		private UInt64 GetTotalMemory ()
		{
			#if __MonoCS__
    			//mono specific code
				var pc = new PerformanceCounter("Mono Memory", "Total Physical Memory");
            	return (ulong)pc.RawValue;
			#else
            var computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();	
				return computerInfo.TotalPhysicalMemory;
			#endif
		}

        /// <summary>
        /// Searches for the latest fallen-8
        /// </summary>
        /// <returns></returns>
        private string FindLatestFallen8()
        {
            var versions = System.IO.Directory.EnumerateFiles(Environment.CurrentDirectory,
                                               _saveFile + Constants.VersionSeparator + "*")
                                               .ToList();

            if (versions.Count > 0)
            {
                var fileToPathMapper = versions
                    .Select(path => path.Split(System.IO.Path.DirectorySeparatorChar))
                    .Where(_ => !_.Last().Contains(Constants.GraphElementsSaveString))
                    .Where(_ => !_.Last().Contains(Constants.IndexSaveString))
                    .Where(_ => !_.Last().Contains(Constants.ServiceSaveString))
                    .ToDictionary(key => key.Last(), value => value.Aggregate((a, b) => a + System.IO.Path.DirectorySeparatorChar + b));

                var latestRevision = fileToPathMapper
                    .Select(file => file.Key.Split(Constants.VersionSeparator)[1])
                    .Select(revisionString => DateTime.FromBinary(Convert.ToInt64(revisionString)))
                    .OrderByDescending(revision => revision)
                    .First()
                    .ToBinary()
                    .ToString(CultureInfo.InvariantCulture);

                return fileToPathMapper.First(_ => _.Key.Contains(latestRevision)).Value;
            }
            return _savePath;
        }

        /// <summary>
        ///   Load the frontend
        /// </summary>
        private void LoadFrontend()
        {
            if (_ressources != null)
            {
                foreach (var memoryStream in _ressources)
                {
                    memoryStream.Value.Dispose();
                }
            }

            _ressources = FindRessources();
            _frontEndPre = Frontend.Pre;
            _frontEndPost = Frontend.Post;
        }

        /// <summary>
        ///   Find all ressources
        /// </summary>
        /// <returns> Ressources </returns>
        private static Dictionary<string, MemoryStream> FindRessources()
        {
            var ressourceDirectory = Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + "Service" +
                                     System.IO.Path.DirectorySeparatorChar + "REST" +
                                     System.IO.Path.DirectorySeparatorChar + "Ressource" +
                                     System.IO.Path.DirectorySeparatorChar;

            return Directory.EnumerateFiles(ressourceDirectory)
                .ToDictionary(
                    key => key.Split(System.IO.Path.DirectorySeparatorChar).Last(),
                    CreateMemoryStreamFromFile);
        }

        /// <summary>
        ///   Creates a memory stream from a file
        /// </summary>
        /// <param name="value"> The path of the file </param>
        /// <returns> MemoryStream </returns>
        private static MemoryStream CreateMemoryStreamFromFile(string value)
        {
            MemoryStream result;

            using (var file = File.OpenRead(value))
            {
                var reader = new BinaryReader(file);
                result = new MemoryStream(reader.ReadBytes((Int32) file.Length));
            }

            return result;
        }

        #endregion
    }
}