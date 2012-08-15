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
using NoSQL.GraphDB.Algorithms.Path;
using NoSQL.GraphDB.Helper;
using NoSQL.GraphDB.Index;
using NoSQL.GraphDB.Plugin;
using NoSQL.GraphDB.Service.REST.Result;
using NoSQL.GraphDB.Service.REST.Specification;
using System.Reflection;

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
        private readonly Fallen8 _fallen8;

        /// <summary>
        /// The Fallen-8 save path
        /// </summary>
        private readonly String _savePath;

        /// <summary>
        /// The Fallen-8 save file
        /// </summary>
        private readonly String _saveFile;

        /// <summary>
        /// The optimal number of partitions
        /// </summary>
        private readonly UInt32 _optimalNumberOfPartitions;

        #endregion

        #region Constructor

        /// <summary>
        ///   Initializes a new instance of the AdminService class.
        /// </summary>
        /// <param name='fallen8'> Fallen-8. </param>
        public AdminService(Fallen8 fallen8)
        {
            _fallen8 = fallen8;

            _saveFile = "Temp.f8s";

            string currentAssemblyDirectoryName = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _savePath = currentAssemblyDirectoryName + System.IO.Path.DirectorySeparatorChar + _saveFile;

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

		public void UploadPlugin (Stream dllStream)
		{
			PluginFactory.Assimilate(dllStream);
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
                var freeMemPerfCounter = new PerformanceCounter("Memory", "Available Bytes");
                var freeMem = freeMemPerfCounter.NextValue();
            	return Convert.ToUInt64(freeMem);
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
            string currentAssemblyDirectoryName = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var versions = Directory.EnumerateFiles(currentAssemblyDirectoryName,
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
            return null;
        }

        #endregion
    }
}