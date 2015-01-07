// 
// ARestServicePlugin.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2012-2015 Henning Rauch
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

#region

using NoSQL.GraphDB.Service;
using NoSQL.GraphDB.Service.REST;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml;
using NoSQL.GraphDB.Helper;
using NoSQL.GraphDB.Log;
using Framework.Serialization;

#endregion

namespace NoSQL.GraphDB.Plugin
{
    /// <summary>
    /// Generelle ServicePlugin Klasse. Zukünftige ServicePlugins erben einfach von dieser Klasse, überschreiben die abstrakten Methoden und sind bereits fixfertig
    /// in den Core integrierbar. So muss Code!
    /// </summary>
    public abstract class ARESTServicePlugin : IService
    {
        public abstract string Version { get; }
        public abstract double RESTInterfaceVersion { get; }
        public abstract string PluginName { get; }
        public abstract string Description { get; }
        public abstract string Manufacturer { get; }
        public abstract Type RESTServiceInterfaceType { get; }

        protected DateTime _startTime;
        protected Boolean _isRunning;
        protected readonly Dictionary<String, String> _metaData = new Dictionary<string, string>();
        public IRESTService _service;
        protected List<ServiceHost> _hosts = new List<ServiceHost>();
        protected List<Uri> _uris = new List<Uri>();
        protected String _uriPattern;
        protected IPAddress _address;
        protected UInt16 _port;

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
            _service.Shutdown();
            _hosts.ForEach(h => h.Close());
            return true;
        }

        public bool TryStart()
        {
            try
            {
                if (!_isRunning)
                {
                    _hosts.ForEach(h => h.Open());

                    _isRunning = true;
                    _startTime = DateTime.Now;
                    Logger.LogInfo(Description + " v" + Version + Environment.NewLine + "   -> Service is started at " + String.Join(" | ", _uris));
                }
                else
                {
                    Logger.LogInfo(Description + " v" + Version + Environment.NewLine + "   -> Service is already started at " + String.Join(" | ", _uris));
                }
            }
            catch (Exception e)
            {
                Logger.LogError(String.Format("Could not start service \"{0}\".{1}{2}", PluginName, Environment.NewLine, e.Message));

                return false;
            }
            return true;
        }

        /// <summary>
        /// Called when the service plugin was restarted.
        /// </summary>
        public virtual void OnServiceRestart() { }

        public void Save(SerializationWriter writer)
        {
            writer.Write(_uriPattern);
            writer.Write(_address.ToString());
            writer.Write(_port);

            _service.Save(writer);
        }

        public void Load(SerializationReader reader, Fallen8 fallen8)
        {
            _uriPattern = reader.ReadString();
            _address = IPAddress.Parse(reader.ReadString());
            _port = reader.ReadUInt16();

            _service = LoadServiceFromSerialization(reader, fallen8);
            _service.Load(reader, fallen8);

            StartService();
        }

        public abstract IRESTService LoadServiceFromSerialization(SerializationReader reader, Fallen8 fallen8);

        public Type PluginCategory
        {
            get { return typeof(IService); }
        }

        public void InitializeParams(NoSQL.GraphDB.Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            _uriPattern = (string)GetConfigParamOrDefault(parameter, "URIPattern", PluginName) + "/" + RESTInterfaceVersion.ToString("0.0", CultureInfo.InvariantCulture);
            _address = IPAddress.Parse(GetConfigParamOrDefault(parameter, "IPAddress", IPAddress.Loopback).ToString());
            _port = (ushort)Convert.ChangeType(GetConfigParamOrDefault(parameter, "Port", 9923), typeof(ushort));
        }

        protected object GetConfigParamOrDefault(IDictionary<string, object> parameters, String param, object defaultParam)
        {
            bool paramAvailable = parameters != null && parameters.ContainsKey(param);
            if (paramAvailable)
            {
                LogConfigParam(param, paramAvailable, parameters[param]);
                return parameters[param];
            }
            LogConfigParam(param, paramAvailable, defaultParam);
            return defaultParam;
        }

        protected void LogConfigParam(String paramName, bool paramAvailable, object param)
        {
            Logger.LogInfo(String.Format("[{0}:config] {1}{2}: {3}", PluginName, paramName, paramAvailable ? "" : " (n/a, using default)", param));
        }

        public void Initialize(Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            InitializeParams(fallen8, parameter);
            _service = CreateService(fallen8, parameter);
            StartService();
        }

        public abstract IRESTService CreateService(Fallen8 fallen8, IDictionary<string, object> parameter);

        protected void StartService()
        {
            Dictionary<string, string> configs = ConfigHelper.GetPluginSpecificConfig(PluginName);

            // wenn ein zweiter Port vorhanden ist, kann eine zweite Instanz gestartet werden
            int instances = configs.ContainsKey("2ndPort") ? 2 : 1;
            string[] prefixes = new string[] { "", "2nd" };
            // TODO: Dynamisch Anzahl und Art der Prefixes herausfinden; momentan sind die Werte fix (max 2 Prefixes/Instanzen, einmal "" und einmal "2nd")

            try
            {
                for (int instance = 1; instance <= instances; instance++)
                {
                    String authMethod = ConfigHelper.GetInstanceParam(configs, "AuthenticationMethod", prefixes, instance);

                    ServiceSecurity serviceSecurity = new ServiceSecurity(authMethod);
                    Uri uri = new Uri(serviceSecurity.HTTP_S + "://" + _address + ":" + _port + "/" + _uriPattern + "/REST");
                    if (!uri.IsWellFormedOriginalString())
                    {
                        throw new Exception("The URI Pattern is not well formed!");
                    }
                    _uris.Add(uri);

                    ServiceHost host = new ServiceHost(_service, uri)
                    {
                        CloseTimeout = new TimeSpan(0, 0, 0, 0, 50)

                    };
                    _hosts.Add(host);

                    var binding = new WebHttpBinding
                    {
                        MaxReceivedMessageSize = 268435456,
                        SendTimeout = new TimeSpan(1, 0, 0),
                        ReceiveTimeout = new TimeSpan(1, 0, 0),
                        // für Security: Anhand des Binding-Namens wird eruiert, welche ConfigSection & Prefix für diese ServiceBinding-Instanz genutzt werden soll
                        Name = PluginName + "." + prefixes[instance - 1]
                    };
                    binding.Security.Mode = serviceSecurity.BindingSecurityMode;
                    binding.Security.Transport.ClientCredentialType = serviceSecurity.BindingClientCredentialType;

                    var readerQuotas = new XmlDictionaryReaderQuotas
                    {
                        MaxDepth = 2147483647,
                        MaxStringContentLength = 2147483647,
                        MaxBytesPerRead = 2147483647,
                        MaxNameTableCharCount = 2147483647,
                        MaxArrayLength = 2147483647
                    };
                    binding.ReaderQuotas = readerQuotas;

                    var se = host.AddServiceEndpoint(RESTServiceInterfaceType, binding, uri);
                    var webBehav = new WebHttpBehavior
                    {
                        FaultExceptionEnabled = true,
                        HelpEnabled = true
                    };
                    se.Behaviors.Add(webBehav);

                    // this adds a additional instanceId header to every response
                    se.Behaviors.Add(new FaultTolerantServiceBehavior());

                    ((ServiceBehaviorAttribute)host.Description.Behaviors[typeof(ServiceBehaviorAttribute)]).InstanceContextMode = InstanceContextMode.Single;
                }
            }
            catch (Exception)
            {
                _hosts.ForEach(h => h.Abort());
                throw;
            }
        }

        public void Dispose()
        {
            TryStop();
            _service.Dispose();
        }
    }
}