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
using System.Threading;
using Fallen8.API.Plugin;
using Framework.Serialization;

namespace Fallen8.API.Index
{
    /// <summary>
    ///   Fallen8 index factory.
    /// </summary>
    public sealed class Fallen8IndexFactory : IFallen8IndexFactory
    {
        #region Data

        /// <summary>
        ///   The created indices.
        /// </summary>
        private IDictionary<String, IIndex> _indices;

        #endregion

        #region constructor

        /// <summary>
        ///   Initializes a new instance of the Fallen8IndexFactory class.
        /// </summary>
        public Fallen8IndexFactory()
        {
            _indices = new Dictionary<String, IIndex>();
        }

        #endregion

        #region IFallen8IndexFactory implementation

        public IEnumerable<String> GetAvailableIndexPlugins()
        {
            IEnumerable<String> result;

            Fallen8PluginFactory.TryGetAvailablePlugins<IIndex>(out result);

            return result;
        }

        public bool TryCreateIndex(out IIndex index, string indexName, string indexTypeName,
                                   IDictionary<string, object> parameter)
        {
            if (Fallen8PluginFactory.TryFindPlugin(out index, indexTypeName))
            {
                try
                {
                    index.Initialize(null, parameter);

                    var newIndices = new Dictionary<string, IIndex>(_indices);
                    newIndices.Add(indexName, index);

                    Interlocked.Exchange(ref _indices, newIndices);

                    return true;
                }
                catch (Exception)
                {
                    index = null;
                    return false;
                }
            }
            index = null;
            return false;
        }

        public bool TryDeleteIndex(string indexName)
        {
            var newIndices = new Dictionary<string, IIndex>(_indices);

            var sthRemoved = newIndices.Remove(indexName);

            Interlocked.Exchange(ref _indices, newIndices);

            return sthRemoved;
        }

        public bool TryGetIndex(out IIndex index, string indexName)
        {
            return _indices.TryGetValue(indexName, out index);
        }

        public IDictionary<string, IIndex> Indices
        {
            get { return _indices; }
        }

        public void DeleteAllIndices()
        {
            var newIndices = new Dictionary<string, IIndex>();
            Interlocked.Exchange(ref _indices, newIndices);
        }

        #endregion

        #region internal methods

        /// <summary>
        ///   Opens and deserializes an index
        /// </summary>
        /// <param name="indexName"> The index name </param>
        /// <param name="indexPluginName"> The index plugin name </param>
        /// <param name="reader"> Serialization reader </param>
        /// <param name="fallen8"> Fallen-8 </param>
        internal void OpenIndex(string indexName, string indexPluginName, SerializationReader reader, Fallen8 fallen8)
        {
            IIndex index;
            if (Fallen8PluginFactory.TryFindPlugin(out index, indexPluginName))
            {
                index.Open(reader, fallen8);

                var newIndices = new Dictionary<string, IIndex>(_indices);
                newIndices.Add(indexName, index);

                Interlocked.Exchange(ref _indices, newIndices);
            }
        }

        #endregion
    }
}