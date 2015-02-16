// 
// DictionaryIndex.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2012-2015 Henning Rauch
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

#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Framework.Serialization;
using NoSQL.GraphDB.Error;
using NoSQL.GraphDB.Helper;
using NoSQL.GraphDB.Log;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Index
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
        private String _description = "A very conservative dictionary index";

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
			if (ReadResource()) 
			{
                try
                {
                    var keyCount = _idx.Keys.Count;
                    return keyCount;
                }
                finally
                {
                    FinishReadResource();
                }
				
			}
			
			throw new CollisionException(this);
        }

        public Int32 CountOfValues()
        {
			if (ReadResource()) 
			{
                try
                {
                    var valueCount = _idx.Values.SelectMany(_ => _).Count();
                    return valueCount;
                }
                finally
                {
                    FinishReadResource();
                }				
			}
			
			throw new CollisionException(this);
        }

        public void AddOrUpdate(Object keyObject, AGraphElement graphElement)
        {
            IComparable key;
            if (!IndexHelper.CheckObject(out key, keyObject))
            {
                return;
            }

			if (WriteResource()) 
			{
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
                    FinishWriteResource();
                }
				
				return;
			}
			
			throw new CollisionException(this);
        }

        public bool TryRemoveKey (Object keyObject)
        {
            IComparable key;
            if (!IndexHelper.CheckObject(out key, keyObject))
            {
                return false;
            }

			if (WriteResource()) 
			{
                try
                {
                    var foundSth = _idx.Remove(key);
                    return foundSth;
                }
                finally
                {
                    FinishWriteResource();
                }
			}
			
			throw new CollisionException(this);
        }

        public void RemoveValue(AGraphElement graphElement)
        {
			if (WriteResource()) 
			{
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
                    FinishWriteResource();
                }
				
				return;
			}
			
			throw new CollisionException(this);
        }
        
        public void Wipe ()
        {
			if (WriteResource()) 
			{
                try
                {
                    _idx.Clear();
                }
                finally
                {
                    FinishWriteResource();
                }
				
				return;
			}
			
			throw new CollisionException(this);
        }

        public IEnumerable<Object> GetKeys ()
        {
			if (ReadResource()) 
			{
                try
                {
                    var keys = new List<IComparable>(_idx.Keys);
                    return keys;
                }
                finally
                {
                    FinishReadResource();
                }
            }
			
			throw new CollisionException(this);
        }


        public IEnumerable<KeyValuePair<object, ReadOnlyCollection<AGraphElement>>> GetKeyValues()
        {
			if (ReadResource()) 
			{
                try
                {
                    foreach (var aKv in _idx)
                        yield return new KeyValuePair<object, ReadOnlyCollection<AGraphElement>>(aKv.Key, new ReadOnlyCollection<AGraphElement>(aKv.Value));
                }
                finally
                {
                    FinishReadResource();
                }
				
				yield break;
			}
			
			throw new CollisionException(this);
        }

        public bool TryGetValue(out ReadOnlyCollection<AGraphElement> result, Object keyObject)
        {
            IComparable key;
            if (!IndexHelper.CheckObject(out key, keyObject))
            {
                result = null;
                return false;
            }

			if (ReadResource()) 
			{
                try
                {
                    List<AGraphElement> graphElements;
                    var foundSth = _idx.TryGetValue(key, out graphElements);

                    result = foundSth ? new ReadOnlyCollection<AGraphElement>(graphElements) : null;
                    return foundSth;
                }
                finally
                {
                    FinishReadResource();
                }
				
			}
			
			throw new CollisionException(this);
        }
        #endregion

        #region IFallen8Serializable

        public void Save(SerializationWriter writer)
        {
			if (ReadResource()) 
			{
                try
                {
                    writer.Write(0);//parameter
                    writer.Write(_idx.Count);
                    foreach (var aKV in _idx)
                    {
                        writer.WriteObject(aKV.Key);
                        writer.Write(aKV.Value.Count);
                        foreach (var aItem in aKV.Value)
                        {
                            writer.Write(aItem.Id);
                        }
                    }
                }
                finally
                {
                    FinishReadResource();
                }
				
				return;
			}
			
			throw new CollisionException(this);
        }

        public void Load(SerializationReader reader, Fallen8 fallen8)
        {
			if (WriteResource()) 
			{
                try
                {
                    reader.ReadInt32();//parameter

                    var keyCount = reader.ReadInt32();

                    _idx = new Dictionary<IComparable, List<AGraphElement>>(keyCount);

                    for (var i = 0; i < keyCount; i++)
                    {
                        var key = reader.ReadObject();
                        var value = new List<AGraphElement>();
                        var valueCount = reader.ReadInt32();
                        for (var j = 0; j < valueCount; j++)
                        {
                            var graphElementId = reader.ReadInt32();
                            AGraphElement graphElement;
                            if (fallen8.TryGetGraphElement(out graphElement, graphElementId))
                            {
                                value.Add(graphElement);
                            }
                            else
                            {
                                Logger.LogError(String.Format("[DictionaryIndex] Error while deserializing the index. Could not find the graph element \"{0}\"", graphElementId));
                            }
                        }
                        _idx.Add((IComparable)key, value);
                    }
                }
                finally
                {
                    FinishWriteResource();
                }
				
				return;
			}
			
			throw new CollisionException(this);
        }

        #endregion

        #region IPlugin implementation

        public void Initialize (Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            _idx = new Dictionary<IComparable, List<AGraphElement>>();
        }

        public string PluginName
        {
            get { return "DictionaryIndex"; }
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
    }
}

