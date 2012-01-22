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
using System.Linq;
using System.Collections.Generic;
using Fallen8.API.Model;
using Fallen8.API.Plugin;
using Fallen8.API.Helper;
using Fallen8.API.Error;

namespace Fallen8.API.Index
{
    /// <summary>
    /// Single value index.
    /// </summary>
    public sealed class SingleValueIndex : AThreadSafeElement, IIndex
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
            if (ReadResource ())
            {

                var result = _idx.Count;
                
                FinishReadResource ();
                
                return result;
            }
            
            throw new CollisionException ();
        }

        public long CountOfValues ()
        {
            if (ReadResource ()) {
                
                var result = _idx.Count;
                
                FinishReadResource ();
                
                return result;
            }
            
            throw new CollisionException ();
        }

        public void AddOrUpdate(IComparable key, AGraphElement graphElement)
        {
            if (WriteResource ())
            {

                _idx[key] = graphElement;

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
                    if (ReferenceEquals(aKv.Value, graphElement))
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
                    yield return new KeyValuePair<IComparable, IEnumerable<AGraphElement>>(aKv.Key, new List<AGraphElement>{aKv.Value});
                
                FinishReadResource ();
                
                yield break;
            }
            
            throw new CollisionException ();
        }

        public bool GetValue(out IEnumerable<AGraphElement> result, IComparable key)
        {
            if (ReadResource ())
            {

                AGraphElement element;
                var foundSth = _idx.TryGetValue(key, out element);

                result = foundSth ? new List<AGraphElement> {element} : null;
                
                FinishReadResource ();
                
                return foundSth;
            }
            
            throw new CollisionException ();
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
        public bool GetValue(out AGraphElement result, IComparable key)
        {
            if (ReadResource())
            {
                var foundSth = _idx.TryGetValue(key, out result);

                FinishReadResource();

                return foundSth;
            }

            throw new CollisionException();
        }

        #endregion
    }
}

