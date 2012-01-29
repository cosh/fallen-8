// 
// SingleValueIndex.cs
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Fallen8.API.Model;

namespace Fallen8.API.Index
{
    /// <summary>
    /// Single value index.
    /// </summary>
    public sealed class SingleValueIndex : IIndex
    {
        #region Data
        
        /// <summary>
        /// The index dictionary.
        /// </summary>
        private Dictionary<IComparable, AGraphElement> _idx;

        /// <summary>
        /// The description of the plugin
        /// </summary>
        private String _description = "A very simple single value index.";

        /// <summary>
        /// The lock
        /// </summary>
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        
        #endregion
  
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the SingleValueIndex class.
        /// </summary>
        public SingleValueIndex()
        {
        }
        
        #endregion
        
        #region IIndex implementation
        public long CountOfKeys ()
        {
            _lock.EnterReadLock();
            try
            {
                return _idx.Count;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public long CountOfValues()
        {
            _lock.EnterReadLock();
            try
            {
                return _idx.Count;
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
                _idx[key] = graphElement;
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
                var toBeRemovedKeys = (from aKv in _idx where ReferenceEquals(aKv.Value, graphElement) select aKv.Key).ToList();

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
                    yield return new KeyValuePair<IComparable, ReadOnlyCollection<AGraphElement>>(aKv.Key, new ReadOnlyCollection<AGraphElement>(new List<AGraphElement> { aKv.Value }));
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
                AGraphElement element;
                var foundSth = _idx.TryGetValue(key, out element);

                result = foundSth ? new ReadOnlyCollection<AGraphElement>(new List<AGraphElement> { element }) : null;

                return foundSth;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        #endregion

        #region IFallen8Plugin implementation
        public void Initialize (Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            _idx = new Dictionary<IComparable, AGraphElement>();
        }

        public string PluginName
        {
            get { return "SingleValueIndex"; }
        }

        public Type PluginType
        {
            get { return typeof(SingleValueIndex); }
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

        #region public methods

        /// <summary>
        /// Gets a value from the index
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="key">Key</param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue(out AGraphElement result, IComparable key)
        {
            _lock.EnterReadLock();
            try
            {
                return _idx.TryGetValue(key, out result);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion
    }
}

