// 
// IFallen8Read.cs
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Fallen8.API.Algorithms.Path;
using Fallen8.API.Expression;
using Fallen8.API.Index.Fulltext;
using Fallen8.API.Index.Spatial;
using Fallen8.API.Model;

namespace Fallen8.API
{
    /// <summary>
    ///   Fallen8 read interface.
    /// </summary>
    public interface IFallen8Read
    {
        #region search

        /// <summary>
        ///   Scan for graph elements by a specified propertyId, literal and binary operation.
        /// </summary>
        /// <returns> <c>true</c> if something was found; otherwise, <c>false</c> . </returns>
        /// <param name='result'> The resulting graph elements. </param>
        /// <param name='propertyId'> Property identifier. </param>
        /// <param name='literal'> Literal. </param>
        /// <param name='binOp'> Binary operator. </param>
        Boolean GraphScan(out List<AGraphElement> result, UInt16 propertyId, IComparable literal,
                          BinaryOperator binOp = BinaryOperator.Equals);

        /// <summary>
        ///   Scan for graph elements by a specified index identifiert, a literal and a binary operation
        /// </summary>
        /// <returns> <c>true</c> if something was found; otherwise, <c>false</c> . </returns>
        /// <param name='result'> Result. </param>
        /// <param name='indexId'> Index identifier. </param>
        /// <param name='literal'> Literal. </param>
        /// <param name='binOp'> Binary operator. </param>
        Boolean IndexScan(out ReadOnlyCollection<AGraphElement> result, String indexId, IComparable literal,
                          BinaryOperator binOp = BinaryOperator.Equals);

        /// <summary>
        ///   Scan for graph elements by a specified property range.
        /// </summary>
        /// <returns> <c>true</c> if something was found; otherwise, <c>false</c> . </returns>
        /// <param name='result'> The resulting graph elements. </param>
        /// <param name='indexId'> Index identifier. </param>
        /// <param name='leftLimit'> Left limit. </param>
        /// <param name='rightLimit'> Right limit. </param>
        /// <param name='includeLeft'> Include left. </param>
        /// <param name='includeRight'> Include right. </param>
        Boolean RangeIndexScan(out ReadOnlyCollection<AGraphElement> result, String indexId, IComparable leftLimit,
                               IComparable rightLimit, Boolean includeLeft = true, Boolean includeRight = true);

        /// <summary>
        ///   Fulltext search for graph elements by a specified query string using an index.
        /// </summary>
        /// <returns> <c>true</c> if something was found; otherwise, <c>false</c> . </returns>
        /// <param name='result'> The resulting fulltext result. </param>
        /// <param name='indexId'> Index identifier. </param>
        /// <param name='searchQuery'> GraphScan query. </param>
        Boolean FulltextIndexScan(out FulltextSearchResult result, String indexId, String searchQuery);

        /// <summary>
        ///   Spatial search for graph elements by a specified geometry and distance using an spatial index.
        /// </summary>
        /// <returns> <c>true</c> if something was found; otherwise, <c>false</c> . </returns>
        /// <param name='result'> The resulting graph elements. </param>
        /// <param name='indexId'> Index identifier. </param>
        /// <param name='geometry'> Geometry. </param>
        Boolean SpatialIndexScan(out ReadOnlyCollection<AGraphElement> result, String indexId, IGeometry geometry);

        #endregion

        #region Persistence

        /// <summary>
        ///   Save Fallen-8 in the specified path.
        /// </summary>
        /// <param name='path'> Path. </param>
        /// <param name='savePartitions'> The number of save partitions. </param>
        void Save(String path, UInt32 savePartitions = 5);

        #endregion

        #region calculations

        /// <summary>
        ///   Calculates the shortest path using a plugin
        /// </summary>
        /// <param name="result"> The resulting path. </param>
        /// <param name="algorithmname"> The name of the algorithm. </param>
        /// <param name="sourceVertexId"> The source vertex identifier. </param>
        /// <param name="destinationVertexId"> The destination vertex identifier. </param>
        /// <param name="maxDepth"> The maximum depth. </param>
        /// <param name="maxPathWeight"> The maximum path weight. </param>
        /// <param name="maxResults"> The maximum number of results. </param>
        /// <param name="edgePropertyFilter"> The edge property filter. </param>
        /// <param name="edgeFilter"> The edge filter. </param>
        /// <param name="edgeCost"> The edge cost. </param>
        /// <param name="vertexCost"> The vertex cost. </param>
        /// <returns> True if the plugin was found, otherwise false. </returns>
        bool CalculateShortestPath(
            out List<Path> result,
            string algorithmname,
            Int32 sourceVertexId,
            Int32 destinationVertexId,
            Int32 maxDepth = 1,
            double maxPathWeight = Double.MaxValue,
            Int32 maxResults = 1,
            PathDelegates.EdgePropertyFilter edgePropertyFilter = null,
            PathDelegates.EdgeFilter edgeFilter = null,
            PathDelegates.EdgeCost edgeCost = null,
            PathDelegates.VertexCost vertexCost = null);

        #endregion
    }
}