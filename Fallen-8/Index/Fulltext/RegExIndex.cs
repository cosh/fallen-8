// 
// RegExIndex.cs
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
using System.Text.RegularExpressions;
using Framework.Serialization;
using NoSQL.GraphDB.Error;
using NoSQL.GraphDB.Helper;
using NoSQL.GraphDB.Log;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Index.Fulltext
{
	/// <summary>
	/// Regular expression index
	/// </summary>
    public sealed class RegExIndex : AThreadSafeElement, IFulltextIndex
	{
        #region Data

        /// <summary>
        /// The index dictionary.
        /// </summary>
        private Dictionary<String, List<AGraphElement>> _idx;

        /// <summary>
        /// The description of the plugin
        /// </summary>
        private String _description = "A very very simple fulltext index using regular expressions";

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the RegExIndex class.
        /// </summary>
        public RegExIndex()
        {
        }
        
        #endregion

        #region IFulltextIndex Members

        public bool TryQuery(out FulltextSearchResult result, string query)
        {
            var regexpression = new Regex(query, RegexOptions.IgnoreCase);
            var foundSth = false;
            result = null;

            if (ReadResource())
            {
                try
                {
					var matchingGraphElements = new Dictionary<AGraphElement, List<string>>();
					var currentScore = 0;
					var maximumScore = 0;
                    const char whitespace = ' ';

                    foreach (var aKV in _idx)
                    {
                        var matches = regexpression.Matches(aKV.Key);

                        if (matches.Count > 0)
                        {
                            if (!foundSth)
                            {
                                result = new FulltextSearchResult();
                                foundSth = true;
                            }

                            var localHighlights = new List<String>(matches.Count);
                            foreach (Match match in matches)
                            {
                                var currentPosition = -1;
                                var lastPosition = -1;

                                for (var i = 0; i < match.Index; i++)
                                {
                                    if (aKV.Key[i] == whitespace)
                                    {
                                        currentPosition = i;
                                    }

                                    if (currentPosition > lastPosition)
                                    {
                                        lastPosition = currentPosition;
                                    }
                                }

                                var firstWhitespacePrev = lastPosition;

                                lastPosition = -1;

                                for (var i = match.Index + match.Length; i < aKV.Key.Length; i++)
                                {
                                    if (aKV.Key[i] == whitespace)
                                    {
                                        lastPosition = i;
                                        break;
                                    }
                                }

                                var firstWhitespaceAfter = lastPosition;

                                if (firstWhitespacePrev == -1 && firstWhitespaceAfter == -1)
                                {
                                    localHighlights.Add(aKV.Key);
                                    continue;
                                }

                                if (firstWhitespacePrev == -1)
                                {
                                    localHighlights.Add(aKV.Key.Substring(0, firstWhitespaceAfter));
                                    continue;
                                }

                                if (firstWhitespaceAfter == -1)
                                {
                                    localHighlights.Add(aKV.Key.Substring(firstWhitespacePrev + 1));
                                    continue;
                                }

                                localHighlights.Add(aKV.Key.Substring(firstWhitespacePrev + 1, firstWhitespaceAfter - firstWhitespacePrev - 1));
                            }

                            for (var i = 0; i < aKV.Value.Count; i++)
                            {
								List<string> globalHighlights;
								if (matchingGraphElements.TryGetValue(aKV.Value[i], out globalHighlights)) 
								{
									globalHighlights.AddRange(localHighlights);
									currentScore = globalHighlights.Count;
								}
								else 
								{
									matchingGraphElements.Add(aKV.Value[i], localHighlights);
									currentScore = localHighlights.Count;
								}

								maximumScore = currentScore > maximumScore 
										? currentScore
										: maximumScore;
                            }
                        }
                    }

					if (foundSth) 
					{
						//create the result
						result = new FulltextSearchResult 
						{ 
							MaximumScore = maximumScore,
							Elements = matchingGraphElements
								.Select(aKV => new FulltextSearchResultElement(aKV.Key, aKV.Value.Count, aKV.Value))
								.ToList()
						};
					}

                    return foundSth;
                }
                finally
                {
                    FinishReadResource();
                }
            }

            throw new CollisionException();
        }

	    #endregion

        #region IIndex Members

        public int CountOfKeys()
        {
            if (ReadResource())
            {
                var keyCount = _idx.Keys.Count;

                FinishReadResource();

                return keyCount;
            }

            throw new CollisionException();
        }

        public int CountOfValues()
        {
            if (ReadResource())
            {
                var valueCount = _idx.Values.SelectMany(_ => _).Count();

                FinishReadResource();

                return valueCount;
            }

            throw new CollisionException();
        }

        public void AddOrUpdate(object keyObject, AGraphElement graphElement)
        {
            String key;
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

        public bool TryRemoveKey(object keyObject)
        {
            String key;
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
                var toBeRemovedKeys = new List<String>();

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

        public void Wipe()
        {
            if (WriteResource())
            {
                _idx.Clear();

                FinishWriteResource();

                return;
            }

            throw new CollisionException();
        }

        public IEnumerable<object> GetKeys()
        {
            if (ReadResource())
            {
                var keys = new List<IComparable>(_idx.Keys);

                FinishReadResource();

                return keys;
            }

            throw new CollisionException();
        }

        public IEnumerable<KeyValuePair<Object, ReadOnlyCollection<AGraphElement>>> GetKeyValues()
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

        public bool TryGetValue(out ReadOnlyCollection<AGraphElement> result, object keyObject)
        {
            String key;
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

        #region IPlugin Members

        public string PluginName
        {
            get { return "RegExIndex"; }
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

        public void Initialize(Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            _idx = new Dictionary<String, List<AGraphElement>>();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _idx.Clear();
            _idx = null;
        }

        #endregion

        #region IFallen8Serializable Members

        public void Save(SerializationWriter writer)
        {
            if (ReadResource())
            {
                writer.WriteOptimized(0);//parameter
                writer.WriteOptimized(_idx.Count);
                foreach (var aKV in _idx)
                {
                    writer.Write(aKV.Key);
                    writer.WriteOptimized(aKV.Value.Count);
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

        public void Load(SerializationReader reader, Fallen8 fallen8)
        {
            if (WriteResource())
            {
                reader.ReadOptimizedInt32();//parameter

                var keyCount = reader.ReadOptimizedInt32();

                _idx = new Dictionary<String, List<AGraphElement>>(keyCount);

                for (var i = 0; i < keyCount; i++)
                {
                    var key = reader.ReadString();
                    var value = new List<AGraphElement>();
                    var valueCount = reader.ReadOptimizedInt32();
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
                            Logger.LogError(String.Format("Error while deserializing the index. Could not find the graph element \"{0}\"", graphElementId));
                        }
                    }
                    _idx.Add(key, value);
                }

                FinishWriteResource();

                return;
            }

            throw new CollisionException();
        }

        #endregion

		#region private helper

	    /// <summary>
	    /// Generates the highlight.
	    /// </summary>
	    /// <returns>
	    /// The highlight.
	    /// </returns>
	    /// <param name='value'>
	    /// Value.
	    /// </param>
	    /// <param name="baseString">The base string </param>
	    private static String GenerateHighlight (Match value, String baseString)
		{
            //die linken und rechten Nachbarn auch noch mit ausgeben (über whitespaces)
	        const char whitespace = ' ';
		    var firstWhitespacePrev = baseString.LastIndexOf(whitespace, 0, value.Index);
            var firstWhitespaceAfter = baseString.IndexOf(whitespace, value.Index + value.Length + 1);

	        if (firstWhitespacePrev == -1 && firstWhitespaceAfter == -1)
	        {
	            return baseString;
	        }

	        if (firstWhitespacePrev == -1)
	        {
                return baseString.Substring(0, firstWhitespaceAfter);	            
	        }

            if (firstWhitespaceAfter == -1)
            {
                return baseString.Substring(firstWhitespacePrev);
            }

		    return baseString.Substring(firstWhitespacePrev, firstWhitespaceAfter);
		}

		#endregion
    }
}

