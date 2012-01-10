// 
// Fallen8IndexFactory.cs
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
using Fallen8.API.Helper;
using Fallen8.API.Plugin;
using System.Threading;

namespace Fallen8.API.Index
{
    /// <summary>
    /// Fallen8 index factory.
    /// </summary>
    public sealed class Fallen8IndexFactory : IFallen8IndexFactory
    {
        #region Data
        
        /// <summary>
        /// The created indices.
        /// </summary>
        private readonly IDictionary<String, IIndex> _indices;
        
        #endregion
        
        #region constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Fallen8.API.Index.Fallen8IndexFactory"/> class.
        /// </summary>
        public Fallen8IndexFactory ()
        {
            _indices = new Dictionary<String, IIndex> ();
        }
        
        #endregion
        
        #region IFallen8IndexFactory implementation
        public IEnumerable<String> GetAvailableIndexPlugins ()
        {
            IEnumerable<String> result;
            
            Fallen8PluginFactory.TryGetAvailablePlugins<IIndex> (out result);
            
            return result;
        }

        public bool TryCreateIndex (out IIndex index, string indexName, string indexTypeName, IDictionary<string, object> parameter)
        {
            if (Fallen8PluginFactory.TryFindPlugin<IIndex> (out index, indexTypeName)) {
             
                try {
                    index.Initialize (null, parameter);
                    
                    _indices.Add (indexName, index);
                    
                    return true;
                } catch (Exception) {
                    return false;
                }
            }
            return false;   
        }

        public bool TryDeleteIndex (string indexName)
        {
            return _indices.Remove (indexName);
        }

        public bool TryGetIndex (out IIndex index, string indexName)
        {
            return _indices.TryGetValue (indexName, out index);
        }

        public IDictionary<string, IIndex> Indices {
            get {
                return _indices;
            }
        }
        #endregion   
    }
}

