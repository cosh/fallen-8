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
using System.Linq;
using System.Collections.Generic;
using Fallen8.API.Plugin;
using Fallen8.API.Service;

namespace Fallen8.API
{
    /// <summary>
    /// Fallen8 server.
    /// </summary>
    public sealed class Fallen8Server : IFallen8Server
    {
        #region Data
        
        /// <summary>
        /// The services.
        /// </summary>
        private List<IFallen8Service> _services; 
        
        /// <summary>
        /// The fallen8.
        /// </summary>
        private Fallen8 _fallen8;
        
        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Fallen8.API.Fallen8Server"/> class.
        /// </summary>
        public Fallen8Server ()
        {
            _services = new List<IFallen8Service> ();
            _fallen8 = new Fallen8 ();
        }
        
        #endregion
        
        #region IFallen8Server implementation
        
        public IFallen8 Fallen8 {
            get { return _fallen8;}}
        
        public void Shutdown ()
        {
            foreach (var aService in _services) {
                aService.TryStop ();
            }
        }

        public IEnumerable<String> GetAvailableServicePlugins ()
        {
            IEnumerable<String> result;
            
            Fallen8PluginFactory.TryGetAvailablePlugins<IFallen8Service> (out result);
            
            return result;
        }

        public bool TryStartService (out IFallen8Service service, string servicePluginName, IDictionary<string, object> parameter)
        {
            if (Fallen8PluginFactory.TryFindPlugin<IFallen8Service> (out service, servicePluginName)) {
             
                try {
                    service.Initialize (_fallen8, parameter);
                    
                    _services.Add (service);
                    
                    return true;
                } catch (Exception) {
                    return false;
                }
            }
            return false;   
        }

        public IEnumerable<IFallen8Service> Services {
            get {
                return _services;
            }
        }
        #endregion
    }
}

