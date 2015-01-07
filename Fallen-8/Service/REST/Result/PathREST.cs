// 
//  PathREST.cs
//  
//  Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
//  
//  Copyright (c) 2012-2015 Henning Rauch
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

using System.Collections.Generic;
using System.Runtime.Serialization;
using NoSQL.GraphDB.Algorithms.Path;

#endregion

namespace NoSQL.GraphDB.Service.REST.Result
{
	/// <summary>
    /// The REST pendant to Path
    /// </summary>
    [DataContract]
    public sealed class PathREST
    {
		#region data

        /// <summary>
        /// The path elements
        /// </summary>
        [DataMember(IsRequired = true)]
		public List<PathElementREST> PathElements;
		
        /// <summary>
        /// The weight of the path
        /// </summary>
        [DataMember(IsRequired = true)]
		public double TotalWeight;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new PathREST instance
        /// </summary>
        public PathREST(Path toBeTransferredResult)
        {
            var toBeTransferredPathElements = toBeTransferredResult.GetPathElements();

            PathElements = new List<PathElementREST>(toBeTransferredPathElements.Count);

            for (var i = 0; i < toBeTransferredPathElements.Count; i++)
            {
                PathElements.Add(new PathElementREST(toBeTransferredPathElements[i]));
            }

            TotalWeight = toBeTransferredResult.Weight;
        }

        #endregion
    }
}

