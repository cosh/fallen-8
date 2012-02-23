// 
//  Fallen8RESTServicePlugin.cs
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
using Framework.Serialization;
using System.ServiceModel;
using System.Net;
using System.ServiceModel.Description;
using System.Xml;

namespace Fallen8.API.Service.REST
{
	/// <summary>
	/// Fallen-8 REST service.
	/// </summary>
	public sealed class Fallen8RESTServicePlugin : IFallen8Service
	{
		#region data
		
		 /// <summary>
        ///   The starting time of the service
        /// </summary>
        private DateTime _startTime;

        /// <summary>
        ///   Is running?
        /// </summary>
        private Boolean _isRunning;

        /// <summary>
        ///   MetaData for this service
        /// </summary>
        private readonly Dictionary<String, String> _metaData = new Dictionary<string, string>();

        /// <summary>
        ///   The actual service
        /// </summary>
        private Fallen8RESTService _service;

        /// <summary>
        ///   The host that runs the service
        /// </summary>
        private ServiceHost _host;

        /// <summary>
        ///   Service description
        /// </summary>
        private String _description = "The Fallen-8 plugin that starts the REST service";
		
		#endregion
		
		#region Constructor
		
		public Fallen8RESTServicePlugin ()
		{
		}
		
		#endregion

		#region IFallen8Service implementation
		public DateTime StartTime
        {
            get { return _startTime; }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public IDictionary<string, string> Metadata
        {
            get { return _metaData; }
        }

        public bool TryStop()
        {
            _host.Close();

            return true;
        }
		#endregion

		#region IFallen8Serializable implementation
		public void Save (SerializationWriter writer)
		{
			throw new NotImplementedException ();
		}

		public void Open (SerializationReader reader, Fallen8 fallen8)
		{
			throw new NotImplementedException ();
		}
		#endregion

		#region IFallen8Plugin implementation
		public void Initialize (Fallen8 fallen8, IDictionary<string, object> parameter)
		{
			String uriPattern = "Fallen8";
            if (parameter != null && parameter.ContainsKey("URIPattern"))
                uriPattern = (String) Convert.ChangeType(parameter["URIPattern"], typeof (String));

            var address = IPAddress.Any;
            if (parameter != null && parameter.ContainsKey("IPAddress"))
                address = (IPAddress) Convert.ChangeType(parameter["IPAddress"], typeof (IPAddress));

            ushort port = 2357;
            if (parameter != null && parameter.ContainsKey("Port"))
                port = (ushort) Convert.ChangeType(parameter["Port"], typeof (ushort));

            StartService(fallen8, address, port, uriPattern);
		}

		public string PluginName {
			get 
			{
				return "Fallen-8_REST_Service";
			}
		}

		public Type PluginCategory {
			get 
			{
				return typeof(IFallen8Service);
			}
		}

		public string Description {
			get 
			{
				return "A simple REST service that talks JSON";
			}
		}

		public string Manufacturer {
			get 
			{
				return "Henning Rauch";
			}
		}
		#endregion

		#region IDisposable implementation
		public void Dispose ()
		{
			TryStop();
            _service.Dispose();
		}
		#endregion

		public void StartService (Fallen8 fallen8, IPAddress ipAddress, ushort port, string uriPattern)
		{
			var uri = new Uri("http://" + ipAddress + ":" + port + "/" + uriPattern);

            if (!uri.IsWellFormedOriginalString())
                throw new Exception("The URI Pattern is not well formed!");

            _service = new Fallen8RESTService(fallen8);

            _host = new ServiceHost(_service, uri);
			
           	var restServiceAddress = "REST";
			
            try
            {
                var binding = new WebHttpBinding
                                  {
                                      MaxBufferSize = 268435456,
                                      MaxReceivedMessageSize = 268435456,
                                      SendTimeout = new TimeSpan(1, 0, 0),
                                      ReceiveTimeout = new TimeSpan(1, 0, 0)
                                  };

                var readerQuotas = new XmlDictionaryReaderQuotas
                                       {
                                           MaxDepth = 2147483647,
                                           MaxStringContentLength = 2147483647,
                                           MaxBytesPerRead = 2147483647,
                                           MaxNameTableCharCount = 2147483647,
                                           MaxArrayLength = 2147483647
                                       };

                binding.ReaderQuotas = readerQuotas;
				
                var se = _host.AddServiceEndpoint(typeof (IFallen8RESTService), binding, restServiceAddress);
                var webBehav = new WebHttpBehavior
                                   {
                                       HelpEnabled = true
                                   };
                se.Behaviors.Add(webBehav);

                ((ServiceBehaviorAttribute) _host.Description.Behaviors[typeof (ServiceBehaviorAttribute)]).
                    InstanceContextMode = InstanceContextMode.Single;

                _host.Open();
            }
            catch (CommunicationException)
            {
                _host.Abort();
                throw;
            }

            _isRunning = true;
            _startTime = DateTime.Now;
            _description += Environment.NewLine + "   -> Service is started at " + uri + "/" + restServiceAddress;
			
		}
	}
}

