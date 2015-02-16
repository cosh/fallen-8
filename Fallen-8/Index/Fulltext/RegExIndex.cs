// 
// RegExIndex.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2011-2015 Henning Rauch
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
                    var matchingGraphElements = new Dictionary<AGraphElement, NestedHighLightAndCounter>();
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

                            var localHighlights = new HashSet<String>();
                            var countOfLocalHighlights = 0;
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
                                    countOfLocalHighlights++;
                                    continue;
                                }

                                if (firstWhitespacePrev == -1)
                                {
                                    localHighlights.Add(aKV.Key.Substring(0, firstWhitespaceAfter));
                                    countOfLocalHighlights++;
                                    continue;
                                }

                                if (firstWhitespaceAfter == -1)
                                {
                                    localHighlights.Add(aKV.Key.Substring(firstWhitespacePrev + 1));
                                    countOfLocalHighlights++;
                                    continue;
                                }

                                localHighlights.Add(aKV.Key.Substring(firstWhitespacePrev + 1, firstWhitespaceAfter - firstWhitespacePrev - 1));
                                countOfLocalHighlights++;
                            }

                            for (var i = 0; i < aKV.Value.Count; i++)
                            {
                                NestedHighLightAndCounter globalHighlights;
								if (matchingGraphElements.TryGetValue(aKV.Value[i], out globalHighlights)) 
								{
									globalHighlights.Highlights.UnionWith(localHighlights);
									currentScore = globalHighlights.NumberOfHighlights + countOfLocalHighlights;
								}
								else
								{
								    matchingGraphElements.Add(aKV.Value[i],
								                              new NestedHighLightAndCounter
								                                  {
								                                      Highlights = new HashSet<string>(localHighlights),
								                                      NumberOfHighlights = countOfLocalHighlights
								                                  });
                                    currentScore = countOfLocalHighlights;
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
								.Select(aKV => new FulltextSearchResultElement(aKV.Key, aKV.Value.NumberOfHighlights, aKV.Value.Highlights))
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

            throw new CollisionException(this);
        }

	    #endregion

        #region public methods

        /// <summary>
        /// A method to query the regex index
        /// </summary>
        /// <param name="result">The number of matching graph elements (distinct)</param>
        /// <param name="query">The query</param>
        /// <param name="filter">The filter that should be applied</param>
        /// <returns>True if something has been found, otherwise false</returns>
        public bool TryQuery(out IEnumerable<AGraphElement> result, string query, Func<Regex, String, Boolean> filter)
        {
            var regexpression = new Regex(query, RegexOptions.IgnoreCase);
            result = null;

            if (ReadResource())
            {
                try
                {
                    var matchingGraphElements = new HashSet<AGraphElement>();

                    foreach (var aKV in _idx)
                    {
                        if (filter(regexpression, aKV.Key))
                        {
                            matchingGraphElements.UnionWith(aKV.Value);
                        }
                    }


                    return matchingGraphElements.Count > 0;
                }
                finally
                {
                    FinishReadResource();
                }
            }

            throw new CollisionException(this);
        }

        #endregion

        #region IIndex Members

        public int CountOfKeys()
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

        public int CountOfValues()
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

        public void AddOrUpdate(object keyObject, AGraphElement graphElement)
        {
            String key;
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

        public bool TryRemoveKey(object keyObject)
        {
            String key;
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
                }
                finally
                {
                    FinishWriteResource();
                }

                return;
            }

            throw new CollisionException(this);
        }

        public void Wipe()
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

        public IEnumerable<object> GetKeys()
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

            throw new CollisionException(this);
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
                try
                {
                    writer.Write(0);//parameter
                    writer.Write(_idx.Count);
                    foreach (var aKV in _idx)
                    {
                        writer.Write(aKV.Key);
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

                    _idx = new Dictionary<String, List<AGraphElement>>(keyCount);

                    for (var i = 0; i < keyCount; i++)
                    {
                        var key = reader.ReadString();
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
                                Logger.LogError(String.Format("Error while deserializing the index. Could not find the graph element \"{0}\"", graphElementId));
                            }
                        }
                        _idx.Add(key, value);
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

        #region helper class

        /// <summary>
        /// Private nested class used to carry some highlightning information
        /// </summary>
        class NestedHighLightAndCounter
        {
            /// <summary>
            /// The highlights
            /// </summary>
            public HashSet<String> Highlights { get; set; }
            
            /// <summary>
            /// The number of highlights
            /// </summary>
            public Int32 NumberOfHighlights { get; set; }
        } 

        #endregion
    }
}

