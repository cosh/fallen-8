// 
// DictionaryIndex.cs
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Fallen8.API.Model;
using Framework.Serialization;

namespace Fallen8.API.Index
{
    /// <summary>
    /// Dictionary index.
    /// </summary>
    public sealed class DictionaryIndex : IIndex
    {
        #region Data
        
        /// <summary>
        /// The index dictionary.
        /// </summary>
        private Dictionary<IComparable, List<AGraphElement>> _idx;

        /// <summary>
        /// The description of the plugin
        /// </summary>
        private String _description = "A very conservative dictionary index";

        /// <summary>
        /// The lock
        /// </summary>
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        #endregion
  
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the DictionaryIndex class.
        /// </summary>
        public DictionaryIndex ()
        {
        }
        
        #endregion
        
        #region IIndex implementation
        
        public Int32 CountOfKeys()
        {
            _lock.EnterReadLock();
            try
            {
                return _idx.Keys.Count;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public Int32 CountOfValues()
        {
            _lock.EnterReadLock();
            try
            {
                return _idx.Values.SelectMany(_ => _).Count();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void AddOrUpdate(IComparable key, AGraphElement graphElement)
        {
            _lock.EnterWriteLock();
            try
            {
                List<AGraphElement> values;
                if (_idx.TryGetValue(key, out values))
                {
                    values.Add(graphElement);
                }
                else
                {
                    values = new List<AGraphElement> { graphElement };
                    _idx.Add(key, values);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool TryRemoveKey (IComparable key)
        {
            _lock.EnterWriteLock();
            try
            {
                return _idx.Remove(key);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void RemoveValue(AGraphElement graphElement)
        {
            _lock.EnterWriteLock();
            try
            {
                var toBeRemovedKeys = new List<IComparable>();

                foreach (var aKv in _idx)
                {
                    aKv.Value.Remove(graphElement);
                    if (aKv.Value.Count == 0)
                    {
                        toBeRemovedKeys.Add(aKv.Key);
                    }
                }

                toBeRemovedKeys.ForEach(_ => _idx.Remove(_));
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        
        public void Wipe ()
        {
            _lock.EnterWriteLock();
            try
            {
                _idx.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IEnumerable<IComparable> GetKeys ()
        {
            _lock.EnterReadLock();
            try
            {
                return new List<IComparable>(_idx.Keys);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }


        public IEnumerable<KeyValuePair<IComparable, ReadOnlyCollection<AGraphElement>>> GetKeyValues()
        {
            _lock.EnterReadLock();
            try
            {
                foreach (var aKv in _idx)
                    yield return new KeyValuePair<IComparable, ReadOnlyCollection<AGraphElement>>(aKv.Key, new ReadOnlyCollection<AGraphElement>(aKv.Value));
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public bool TryGetValue(out ReadOnlyCollection<AGraphElement> result, IComparable key)
        {
            _lock.EnterReadLock();
            try
            {
                List<AGraphElement> graphElements;
                var foundSth = _idx.TryGetValue(key, out graphElements);

                result = foundSth ? new ReadOnlyCollection<AGraphElement>(graphElements) : null;

                return foundSth;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        #endregion

        #region IFallen8Plugin implementation

        public void Save(SerializationWriter writer)
        {
            _lock.EnterReadLock();
            try
            {
                writer.WriteOptimized(0);//parameter
                writer.WriteOptimized(_idx.Count);
                foreach (var aKV in _idx)
                {
                    writer.WriteObject(aKV.Key);
                    writer.WriteOptimized(aKV.Value.Count);
                    foreach (var aItem in aKV.Value)
                    {
                        writer.WriteOptimized(aItem.Id);
                    }
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void Open(SerializationReader reader, Fallen8 fallen8)
        {
            _lock.EnterWriteLock();
            try
            {
                reader.ReadOptimizedInt32();//parameter

                var keyCount = reader.ReadOptimizedInt32();

                _idx = new Dictionary<IComparable, List<AGraphElement>>(keyCount);

                for (int i = 0; i < keyCount; i++)
                {
                    var key = reader.ReadObject();
                    var value = new List<AGraphElement>();
                    var valueCount = reader.ReadOptimizedInt32();
                    for (int j = 0; j < valueCount; j++)
                    {
                        var graphElementId = reader.ReadOptimizedInt32();
                        AGraphElement graphElement;
                        fallen8.TryGetGraphElement(out graphElement, graphElementId);
                        if (graphElement != null)
                        {
                            value.Add(graphElement);
                        }
                    }
                    _idx.Add((IComparable)key, value);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Initialize (Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            _idx = new Dictionary<IComparable, List<AGraphElement>>();
        }

        public string PluginName
        {
            get { return "DictionaryIndex"; }
        }

        public Type PluginType
        {
            get { return typeof(DictionaryIndex); }
        }

        public Type PluginCategory
        {
            get { return typeof(IIndex); }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string Manufacturer
        {
            get { return "Henning Rauch"; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _idx.Clear();
            _idx = null;
            _lock.Dispose();
            _lock = null;
        }

        #endregion
    }
}

