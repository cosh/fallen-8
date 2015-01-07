// 
// FulltextSearchResult.cs
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

#endregion

namespace NoSQL.GraphDB.Index.Fulltext
{
	/// <summary>
	/// Fulltext search result.
	/// </summary>
	public sealed class FulltextSearchResult
    {
        #region data

        /// <summary>
		/// Gets or sets the maximum score.
		/// </summary>
		/// <value>
		/// The maximum score.
		/// </value>
		public Double MaximumScore { get; set; }
		
		/// <summary>
		/// Gets or sets the elements.
		/// </summary>
		/// <value>
		/// The elements.
		/// </value>
		public List<FulltextSearchResultElement> Elements { get; set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new FulltextSearchResult instance
        /// </summary>
        public FulltextSearchResult()
        {
            Elements = new List<FulltextSearchResultElement>();
        }

        #endregion

        #region public methods

        /// <summary>
        /// Adds an element
        /// </summary>
        /// <param name="element">Element</param>
        public void AddElement(FulltextSearchResultElement element)
        {
            Elements.Add(element);

            if (element.Score > MaximumScore)
            {
                MaximumScore = element.Score;
            }
        }

        #endregion


    }
}

