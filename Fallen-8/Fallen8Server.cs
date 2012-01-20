// 
// Fallen8Server.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2012 Henning Rauch
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
using Fallen8.API.Plugin;
using Fallen8.API.Service;

namespace Fallen8.API
{
    /// <summary>
    /// Fallen8 server.
    /// </summary>
    public sealed class Fallen8Server
    {
        #region Data
        
        /// <summary>
        /// The services.
        /// </summary>
        public readonly List<IFallen8Service> Services; 
        
        /// <summary>
        /// The Fallen-8.
        /// </summary>
        public readonly Fallen8 Fallen8;
        
        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the Fallen8Server class.
        /// </summary>
        public Fallen8Server ()
        {
            Services = new List<IFallen8Service>();
            Fallen8 = new Fallen8();
        }
        
        #endregion
        
        #region public methods

        /// <summary>
        /// Shutdown this Fallen-8 server.
        /// </summary>
        public void Shutdown ()
        {
            foreach (var aService in Services)
            {
                aService.TryStop ();
            }
        }

        /// <summary>
        /// Gets the available service plugins.
        /// </summary>
        /// <returns>
        /// The available service plugins.
        /// </returns>
        public IEnumerable<String> GetAvailableServicePlugins ()
        {
            IEnumerable<String> result;
            
            Fallen8PluginFactory.TryGetAvailablePlugins<IFallen8Service> (out result);
            
            return result;
        }

        /// <summary>
        /// Tries to start a service.
        /// </summary>
        /// <returns>
        /// True for success.
        /// </returns>
        /// <param name='service'>
        /// The launched service.
        /// </param>
        /// <param name='servicePluginName'>
        /// The name of this service.
        /// </param>
        /// <param name='parameter'>
        /// The parameters of this service.
        /// </param>
        public bool TryStartService(out IFallen8Service service, string servicePluginName, IDictionary<string, object> parameter)
        {
            IFallen8Service serviceSchema;
            if (Fallen8PluginFactory.TryFindPlugin<IFallen8Service>(out serviceSchema, servicePluginName))
            {
                try
                {
                    service = (IFallen8Service)serviceSchema.Initialize(Fallen8, parameter);

                    Services.Add(service);

                    return true;
                }
                catch (Exception)
                {
                    service = null;
                    return false;
                }
            }

            service = null;
            return false;
        }

        #endregion
    }
}

