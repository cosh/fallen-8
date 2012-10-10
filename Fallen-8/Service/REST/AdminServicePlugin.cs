// 
//  AdminServicePlugin.cs
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
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml;
using Framework.Serialization;
using NoSQL.GraphDB.Log;
using System.Configuration;

#endregion

namespace NoSQL.GraphDB.Service.REST
{
    /// <summary>
    ///   Fallen-8 Admin REST service.
    /// </summary>
    public sealed class AdminServicePlugin : IService
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
        ///   The actual service
        /// </summary>
        internal IAdminService Service;

        /// <summary>
        ///   The host that runs the service
        /// </summary>
        private ServiceHost _host;

        /// <summary>
        ///   Service description
        /// </summary>
        private const String _description = "The Fallen-8 plugin that starts the admin service";

        /// <summary>
        /// The URI of the service
        /// </summary>
        private Uri _uri;

        /// <summary>
        /// REST service address
        /// </summary>
        private String _restServiceAddress;

        #endregion

        #region Constructor

        #endregion

        #region IService implementation

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
            get { return null; }
        }

        public bool TryStop()
        {
            _host.Close();

            return true;
        }

        public bool TryStart()
        {
            try
            {
                if (!_isRunning)
                {
                    _host.Open();

                    _isRunning = true;
                    _startTime = DateTime.Now;
                    Logger.LogInfo(_description + Environment.NewLine + "   -> Service is started at " + _uri + "/" + _restServiceAddress);
                }
                else
                {
                    Logger.LogInfo(_description + Environment.NewLine + "   -> Service is already started at " + _uri + "/" + _restServiceAddress);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(String.Format("Could not start service \"{0}\".{1}{2}", PluginName, Environment.NewLine, e.Message));

                return false;
            }

            return true;
        }

        #endregion

        #region IFallen8Serializable implementation

        public void Save(SerializationWriter writer)
        {
        }

        public void Load(SerializationReader reader, Fallen8 fallen8)
        {
            StartService(fallen8);
        }

        #endregion

        #region IPlugin implementation

        public void Initialize(Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            StartService(fallen8);
        }

        public string PluginName
        {
            get { return "Fallen-8_Admin_Service"; }
        }

        public Type PluginCategory
        {
            get { return typeof (IService); }
        }

        public string Description
        {
            get { return _description; }
        }

        public string Manufacturer
        {
            get { return "Henning Rauch"; }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            TryStop();
            Service.Dispose();
        }

        #endregion

        #region private helper

        /// <summary>
        ///   Starts the actual service
        /// </summary>
        /// <param name="fallen8"> Fallen-8. </param>
        private void StartService(Fallen8 fallen8)
        {
            #region configuration
            var configs = new Dictionary<String, String>();
            foreach (String key in ConfigurationManager.AppSettings)
            {
                var value = ConfigurationManager.AppSettings[key];

                configs.Add(key, value);
            }
            
            String adminIpAddress;
            configs.TryGetValue("AdminIPAddress", out adminIpAddress);
            
            UInt16 adminPort = 0;
            String adminPortString;
            if(configs.TryGetValue("AdminPort", out adminPortString))
            {
                adminPort = Convert.ToUInt16(adminPortString);
            }
            
            String adminUriPattern;
            configs.TryGetValue("AdminUriPattern", out adminUriPattern);

            String restServiceAddress;
            configs.TryGetValue("RESTServicePattern", out restServiceAddress);

            #endregion

            var address = String.IsNullOrWhiteSpace(adminIpAddress) ? IPAddress.Any.ToString() : adminIpAddress;
            var port = adminPort;
            var uriPattern = String.IsNullOrWhiteSpace(adminUriPattern) ? "Admin" : adminUriPattern;

            _uri = new Uri("http://" + address + ":" + port + "/" + uriPattern);

            if (!_uri.IsWellFormedOriginalString())
                throw new Exception("The URI pattern is not well formed!");

            Service = new AdminService(fallen8);

            _host = new ServiceHost(Service, _uri)
                        {
                            CloseTimeout = new TimeSpan(0, 0, 0, 0, 50)
                        };

            _restServiceAddress = String.IsNullOrWhiteSpace(restServiceAddress) ? "REST" : restServiceAddress;            

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

                var se = _host.AddServiceEndpoint(typeof (IAdminService), binding, _restServiceAddress);
                var webBehav = new WebHttpBehavior
                                   {
                                       HelpEnabled = true
                                   };
                se.Behaviors.Add(webBehav);

                ((ServiceBehaviorAttribute) _host.Description.Behaviors[typeof (ServiceBehaviorAttribute)]).
                    InstanceContextMode = InstanceContextMode.Single;

            }
            catch (Exception)
            {
                _host.Abort();
                throw;
            }
        }

        #endregion
    }
}