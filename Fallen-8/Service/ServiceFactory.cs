// 
// ServiceFactory.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2011 Henning Rauch
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
using System.Linq;
using Fallen8.API.Error;
using Fallen8.API.Log;
using Fallen8.API.Plugin;
using Fallen8.API.Helper;

namespace Fallen8.API.Service
{
    /// <summary>
    ///   Service factory
    /// </summary>
    public sealed class ServiceFactory : AThreadSafeElement
    {
        #region Data

        /// <summary>
        /// The Fallen-8 instance
        /// </summary>
        private readonly Fallen8 _fallen8;

        /// <summary>
        ///   The created indices.
        /// </summary>
        public readonly IDictionary<String, IFallen8Service> Services;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new service factory
        /// </summary>
        /// <param name="fallen8">Fallen-8</param>
        public ServiceFactory(Fallen8 fallen8)
        {
            _fallen8 = fallen8;
            Services = new Dictionary<string, IFallen8Service>();
        }

        #endregion

        #region public methods

        /// <summary>
        ///   Gets the available service plugins.
        /// </summary>
        /// <returns> The available service plugins. </returns>
        public IEnumerable<String> GetAvailableServicePlugins()
        {
            Dictionary<String, string> result;

            Fallen8PluginFactory.TryGetAvailablePluginsWithDescriptions<IFallen8Service>(out result);

            return result.Select(_ => _.Value);
        }

        /// <summary>
        ///   Tries to start a service.
        /// </summary>
        /// <returns> True for success. </returns>
        /// <param name='service'> The launched service. </param>
        /// <param name='servicePluginName'> The name of the service plugin. </param>
        /// <param name="serviceName"> The name of the service instance </param>
        /// <param name='parameter'> The parameters of this service. </param>
        public bool TryStartService(out IFallen8Service service, string servicePluginName, string serviceName,
                                    IDictionary<string, object> parameter)
        {
            try
            {
                if (Fallen8PluginFactory.TryFindPlugin(out service, servicePluginName))
                {
                    if (WriteResource())
                    {
                        if (Services.ContainsKey(serviceName))
                        {
                            Logger.LogError(String.Format("There already exists a service with the name {0}",
                                                          serviceName));
                            service = null;

                            FinishWriteResource();
                            return false;
                        }

                        service.Initialize(_fallen8, parameter);
                        Services.Add(serviceName, service);

                        FinishWriteResource();
                        return true;
                    }
                    
                    throw new CollisionException();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(
                    String.Format("Fallen-8 was not able to start the {0} service plugin. Message: {1}",
                                  servicePluginName, e.Message));

                FinishWriteResource();

                service = null;
                return false;
            }

            service = null;
            return false;
        }

        /// <summary>
        /// Shuts down all the services
        /// </summary>
        public void ShutdownAllServices()
        {
            if (WriteResource())
            {
                foreach (var service in Services)
                {
                    service.Value.TryStop();
                }

                FinishWriteResource();
            }

            throw new CollisionException();
        }

        #endregion
    }
}