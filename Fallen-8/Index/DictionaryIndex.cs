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
using Fallen8.API.Model;
using Fallen8.API.Plugin;
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
        private Dictionary<IComparable, List<AGraphElement>> _idx;

        /// <summary>
        /// The description of the plugin
        /// </summary>
        private String _description = "A very conservative directory index";
        
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

        public void AddOrUpdate(IComparable key, AGraphElement graphElement)
        {
            if (WriteResource ()) {

                List<AGraphElement> values;
                if (_idx.TryGetValue (key, out values)) {
                
                    values.Add (graphElement);
                
                } else {

                    values = new List<AGraphElement> {graphElement};
                    _idx.Add (key, values);
                }
                
                FinishWriteResource ();

                return;
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

        public void RemoveValue(AGraphElement graphElement)
        {
            if (WriteResource ()) {

                var toBeRemovedKeys = new List<IComparable>();

                foreach (var aKv in _idx) 
                {
                    aKv.Value.Remove (graphElement);
                    if (aKv.Value.Count == 0)
                    {
                        toBeRemovedKeys.Add(aKv.Key);
                    }
                }

                toBeRemovedKeys.ForEach(_ => _idx.Remove(_));
                
                FinishWriteResource ();

                return;
            }
            
            throw new CollisionException ();
        }
        
        public void Wipe ()
        {
            if (WriteResource ()) {
                
                _idx.Clear ();
                
                FinishWriteResource ();

                return;
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


        public IEnumerable<KeyValuePair<IComparable, IEnumerable<AGraphElement>>> GetKeyValues()
        {
            if (ReadResource ()) {
                
                foreach (var aKv in _idx)
                    yield return new KeyValuePair<IComparable, IEnumerable<AGraphElement>>(aKv.Key, new List<AGraphElement>(aKv.Value));
                
                FinishReadResource ();
                
                yield break;
            }
            
            throw new CollisionException ();
        }

        public bool GetValue(out IEnumerable<AGraphElement> result, IComparable key)
        {
            if (ReadResource ()) {

                List<AGraphElement> graphElements;
                
                var foundSth = _idx.TryGetValue (key, out graphElements);
                
                result = foundSth ? graphElements : null;
                
                FinishReadResource ();
                
                return foundSth;
            }
            
            throw new CollisionException ();
        }
        #endregion

        #region IFallen8Plugin implementation
        public IFallen8Plugin Initialize (Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            _idx = new Dictionary<IComparable, List<AGraphElement>>();
            
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

