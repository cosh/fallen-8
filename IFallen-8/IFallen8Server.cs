// 
// IFallen8Server.cs
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
using System.Text;
using Fallen8.API.Service;
using Fallen8.API.Plugin;

namespace Fallen8.API
{
	/// <summary>
	/// Fallen-8 server interface.
	/// </summary>
    public interface IFallen8Server
    {
		/// <summary>
		/// Shutdown this Fallen-8 server.
		/// </summary>
        void Shutdown();
		
		/// <summary>
		/// Gets the services.
		/// </summary>
		/// <value>
		/// The services.
		/// </value>
        IEnumerable<IFallen8Service> Services { get; }
		
		/// <summary>
		/// Gets the available service plugins.
		/// </summary>
		/// <returns>
		/// The available service plugins.
		/// </returns>
		IEnumerable<PluginDescription> GetAvailableServicePlugins ();

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
        /// <param name='connector'>
        /// The the connection to fallen-8.
        /// </param>
        /// <param name='parameter'>
        /// The parameters of this service.
        /// </param>
        bool TryStartService(out IFallen8Service service, String servicePluginName, IFallen8Connector connector, IDictionary<String, Object> parameter);
    }
}