// 
// RangeIndex.cs
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

namespace NoSQL.GraphDB.Index.Range
{
	/// <summary>
	/// Fallen8 range index.
	/// </summary>
    public sealed class RangeIndex : AThreadSafeElement, IRangeIndex
	{
		#region Data
        
        /// <summary>
        /// The index dictionary.
        /// </summary>
        private Dictionary<IComparable, List<AGraphElement>> _idx;

        /// <summary>
        /// The description of the plugin
        /// </summary>
        private String _description = "A very very simple range index";

        #endregion
  
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the RangeIndex class.
        /// </summary>
        public RangeIndex ()
        {
        }
        
        #endregion
        
		#region IDisposable implementation
		public void Dispose ()
		{
			_idx.Clear();
            _idx = null;
		}
		#endregion

		#region IPlugin implementation
		public void Initialize (Fallen8 fallen8, IDictionary<string, object> parameter)
		{
            _idx = new Dictionary<IComparable, List<AGraphElement>>();
		}

		public string PluginName {
			
            get { return "RangeIndex"; }
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

		#region IFallen8Serializable implementation
		public void Save (SerializationWriter writer)
		{
			if (ReadResource()) 
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
				
				FinishReadResource();
				
				return;
			}
			
			throw new CollisionException();
		}

		public void Load (SerializationReader reader, Fallen8 fallen8)
		{
			
			if (WriteResource()) 
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
                        var graphElementId = reader.ReadInt64();
                        AGraphElement graphElement;
                        if(fallen8.TryGetGraphElement(out graphElement, graphElementId))
                        {
                            value.Add(graphElement);
                        }
                        else
                        {
                            Logger.LogError(String.Format("Error while deserializing the index. Could not find the graph element \"{0}\"", graphElementId));
                        }
                    }
                    _idx.Add((IComparable)key, value);
                }
				
				FinishWriteResource();
				
				return;
			}
			
			throw new CollisionException();
		}
		#endregion

		#region IIndex implementation
        
        public Int32 CountOfKeys()
        {
			if (ReadResource()) 
			{
				var keyCount = _idx.Keys.Count;
				
				FinishReadResource();
				
				return keyCount;
			}
			
			throw new CollisionException();
        }

        public Int32 CountOfValues()
        {
			if (ReadResource()) 
			{
				var valueCount = _idx.Values.SelectMany(_ => _).Count();
				
				FinishReadResource();
				
				return valueCount;
			}
			
			throw new CollisionException();
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
				
				FinishWriteResource();
				
				return;
			}
			
			throw new CollisionException();
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
				var foundSth = _idx.Remove(key);
				
				FinishWriteResource();
				
				return foundSth;
			}
			
			throw new CollisionException();
        }

        public void RemoveValue(AGraphElement graphElement)
        {
			if (WriteResource()) 
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
				
				FinishWriteResource();
				
				return;
			}
			
			throw new CollisionException();
        }
        
        public void Wipe ()
        {
			if (WriteResource()) 
			{
                _idx.Clear();
				
				FinishWriteResource();
				
				return;
			}
			
			throw new CollisionException();
        }

        public IEnumerable<Object> GetKeys ()
        {
			if (ReadResource()) 
			{
				var keys = new List<IComparable>(_idx.Keys);
				
				FinishReadResource();
				
				return keys;
			}
			
			throw new CollisionException();
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
			
			throw new CollisionException();
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
				List<AGraphElement> graphElements;
                var foundSth = _idx.TryGetValue(key, out graphElements);

                result = foundSth ? new ReadOnlyCollection<AGraphElement>(graphElements) : null;
				
				FinishReadResource();
				
				return foundSth;
			}
			
			throw new CollisionException();
        }
        #endregion

		#region IRangeIndex implementation
		public bool LowerThan (out ReadOnlyCollection<AGraphElement> result, IComparable key, bool includeKey)
		{
			if (ReadResource()) 
			{
				var listOfMatchingGraphElements = _idx
					.AsParallel()
						.Where(aKV => includeKey 
						       ? aKV.Key.CompareTo(key) <= 0 
						       : aKV.Key.CompareTo(key) < 0)
						.Select(aRelevantKV => aRelevantKV.Value)
						.SelectMany(_ => _)
						.ToList();

                result = listOfMatchingGraphElements.Count > 0 
					? new ReadOnlyCollection<AGraphElement>(listOfMatchingGraphElements) 
					: null;
				
				FinishReadResource();
				
				return result != null;
			}
			
			throw new CollisionException();
		}

		public bool GreaterThan (out ReadOnlyCollection<AGraphElement> result, IComparable key, bool includeKey)
		{
			if (ReadResource()) 
			{
				var listOfMatchingGraphElements = _idx
					.AsParallel()
						.Where(aKV => includeKey 
						       ? aKV.Key.CompareTo(key) >= 0 
						       : aKV.Key.CompareTo(key) > 0)
						.Select(aRelevantKV => aRelevantKV.Value)
						.SelectMany(_ => _)
						.ToList();

                result = listOfMatchingGraphElements.Count > 0 
					? new ReadOnlyCollection<AGraphElement>(listOfMatchingGraphElements) 
					: null;
				
				FinishReadResource();
				
				return result != null;
			}
			
			throw new CollisionException();
		}

		public bool Between (out ReadOnlyCollection<AGraphElement> result, IComparable lowerLimit, IComparable upperLimit, bool includeLowerLimit, bool includeUpperLimit)
		{
			if (ReadResource()) 
			{
				var listOfMatchingGraphElements = _idx
					.AsParallel()
						.Where(aKV => 
						       (includeLowerLimit 
						       ? aKV.Key.CompareTo(lowerLimit) <= 0 
						       : aKV.Key.CompareTo(lowerLimit) < 0)
						       &&
						       (includeUpperLimit 
						       ? aKV.Key.CompareTo(upperLimit) >= 0 
						       : aKV.Key.CompareTo(upperLimit) > 0))
						.Select(aRelevantKV => aRelevantKV.Value)
						.SelectMany(_ => _)
						.ToList();

                result = listOfMatchingGraphElements.Count > 0 
					? new ReadOnlyCollection<AGraphElement>(listOfMatchingGraphElements) 
					: null;
				
				FinishReadResource();
				
				return result != null;
			}
			
			throw new CollisionException();
		}
		#endregion
	}
}

