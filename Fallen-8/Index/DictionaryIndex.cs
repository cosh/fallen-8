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
using System.Linq;
using System.Collections.Generic;
using Fallen8.Model;
using Fallen8.API.Plugin;
using System.Collections.Concurrent;
using Fallen8.API.Helper;
using Fallen8.API.Error;

namespace Fallen8.API.Index
{
    /// <summary>
    /// Dictionary index.
    /// </summary>
    public sealed class DictionaryIndex : AThreadSafeElement, IIndex
    {
        #region Data
        
        /// <summary>
        /// The index dictionary.
        /// </summary>
        private Dictionary<IComparable, HashSet<IGraphElementModel>> _idx;

        /// <summary>
        /// The description of the plugin
        /// </summary>
        private String _description = "A very conservative directory index";
        
        #endregion
  
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Fallen8.API.Index.DictionaryIndex"/> class.
        /// </summary>
        public DictionaryIndex ()
        {
        }
        
        #endregion
        
        #region IIndex implementation
        public long CountOfKeys ()
        {
            if (ReadResource ()) {
                
                var result = _idx.Keys.Count;
                
                FinishReadResource ();
                
                return result;
            }
            
            throw new CollisionException ();
        }

        public long CountOfValues ()
        {
            if (ReadResource ()) {
                
                var result = _idx.Values.SelectMany (_ => _).Count ();
                
                FinishReadResource ();
                
                return result;
            }
            
            throw new CollisionException ();
        }

        public void AddOrUpdate (IComparable key, IGraphElementModel graphElement)
        {
            if (WriteResource ()) {
                
                HashSet<IGraphElementModel> values;
                if (_idx.TryGetValue (key, out values)) {
                
                    values.Add (graphElement);
                
                } else {
                    
                    values = new HashSet<IGraphElementModel> ();
                    values.Add (graphElement);
                    _idx.Add (key, values);
                }
                
                FinishWriteResource ();
            }
            
            
            throw new CollisionException ();
        }

        public bool TryRemoveKey (IComparable key)
        {
            if (WriteResource ()) {
                var removedSomething = _idx.Remove (key);
                
                FinishWriteResource ();
                
                return removedSomething;
            }
            
            throw new CollisionException ();
        }

        public void RemoveValue (IGraphElementModel graphElement)
        {
            if (WriteResource ()) {
                
                foreach (var aKV in _idx) {
                    aKV.Value.Remove (graphElement);
                }
                
                FinishWriteResource ();
            }
            
            throw new CollisionException ();
        }
        
        public void Wipe ()
        {
            if (WriteResource ()) {
                
                _idx.Clear ();
                
                FinishWriteResource ();
            }
            
            throw new CollisionException ();
        }

        public IEnumerable<IComparable> GetKeys ()
        {
            if (ReadResource ()) {
                
                var result = new List<IComparable> (_idx.Keys);
                
                FinishReadResource ();
                
                return result;
            }
            
            throw new CollisionException ();
        }


        public IEnumerable<KeyValuePair<IComparable, IEnumerable<IGraphElementModel>>> GetKeyValues ()
        {
            if (ReadResource ()) {
                
                foreach (var aKV in _idx) 
                    yield return new KeyValuePair<IComparable, IEnumerable<IGraphElementModel>>(aKV.Key, new List<IGraphElementModel>(aKV.Value));
                
                FinishReadResource ();
                
                yield break;
            }
            
            throw new CollisionException ();
        }

        public bool GetValue (out IEnumerable<IGraphElementModel> result, IComparable key)
        {
            if (ReadResource ()) {
                
                HashSet<IGraphElementModel> graphElements;
                
                var foundSth = _idx.TryGetValue (key, out graphElements);
                
                if (foundSth) {
                    result = graphElements;
                } else {
                    result = null;
                }
                
                FinishReadResource ();
                
                return foundSth;
            }
            
            throw new CollisionException ();
        }
        #endregion

        #region IFallen8Plugin implementation
        public IFallen8Plugin Initialize (IFallen8 fallen8, IDictionary<string, object> parameter)
        {
            _idx = new Dictionary<IComparable, HashSet<IGraphElementModel>> ();
            
            return this;
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
    }
}

