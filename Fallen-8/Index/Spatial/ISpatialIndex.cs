// 
// ISpatialIndex.cs
//  
// Author:
//       Andriy Kupershmidt <kuper133@googlemail.com>
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
using System.Collections.ObjectModel;
using Fallen8.API.Model;

namespace Fallen8.API.Index.Spatial
{
    /// <summary>
    /// Fallen8 spatial index interface.
    /// </summary>
    public interface ISpatialIndex : IIndex
    {
        #region TryGetValues
        /// <summary>
        /// Tries to get the values.
        /// </summary>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
        /// Result.
        /// </param>
        /// <param name='geometry'>
        /// Geometry.
        /// </param>
        Boolean TryGetValues(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, Predicate<AGraphElement> predicat = null);
        /// <summary>
        /// Tries to get the values.
        /// </summary>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
        /// Result.
        /// </param>
        /// <param name='graphElement'>
        /// Element of graph.
        /// </param>
        Boolean TryGetValues(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicat = null);
        #endregion
        #region AddOrUpdate
        /// <summary>
        /// add or update neu element of grpah to spatial index
        /// </summary>
        /// <param name="key">
        /// geometry for neu element of graph
        /// </param>
        /// <param name="graphElement">
        /// neu element of graph
        /// </param>
        void AddOrUpdate(IGeometry key, AGraphElement graphElement);
        #endregion
        #region TryRemoveKey
        /// <summary>
        /// Tries to remove a key.
        /// </summary>
        /// <returns>
        /// <c>true</c> if something was removed; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='key'>
        /// Key.
        /// </param>
        Boolean TryRemoveKey(IGeometry key);
        #endregion
        #region Distance
        /// <summary>
        /// find distance between two geometric objects
        /// </summary>
        /// <param name="geometry1">
        /// geomtry 1
        /// </param>
        /// <param name="geometry2">
        /// geometry 2
        /// </param>
        /// <returns>
        /// value of distance
        /// </returns>
        Double Distance(IGeometry geometry1, IGeometry geometry2);
        /// <summary>
        /// find distance between two geometric elements of graph
        /// </summary>
        /// <param name="graphElement1">
        /// element og graph 1
        /// </param>
        /// <param name="graphElement2">
        /// element of graph 2
        /// </param>
        /// <returns>
        /// value of distance
        /// </returns>
        Double Distance(AGraphElement graphElement1, AGraphElement graphElement2);
        #endregion
        #region SearchRegion
        /// <summary>
        /// find all objects that have at least one point in this region
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="minimalBoundedRechtangle">
        /// region(minimal bounded rechtangle)
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean SearchRegion(out ReadOnlyCollection<AGraphElement> result, IMBR minimalBoundedRechtangle, Predicate<AGraphElement> predicat = null);
        #endregion
        #region Overlap
        /// <summary>
        /// find all objects that have at least one point in common with this geometry
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="geometry">
        /// geometric object
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean Overlap(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, Predicate<AGraphElement> predicat);
        /// <summary>
        /// find all objects that have at least one point in common with this element of graph
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="graphElement">
        /// element of graph
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean Overlap(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicat);
        #endregion
        #region Enclosure
        /// <summary>
        /// find all objects (if they exist), which this element of graph included.
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="graphElement">
        /// element of graph
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean Enclosure(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicat);
        /// <summary>
        /// find all objects (if they exist), which this geometry included.
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="geometry">
        /// geomtry
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean Enclosure(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, Predicate<AGraphElement> predicat);
        #endregion
        #region Containment
        /// <summary>
        /// find all objects, which this geometry contains. 
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="geometry">
        /// geometry
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean Containment(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, Predicate<AGraphElement> predicat);
        /// <summary>
        /// find all objects, which this element of graph contains. 
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="graphElement">
        /// element of graph
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean Containment(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicat);
        #endregion
        #region GetAllNeighbors
        /// <summary>
        /// find all neighbors for this element of graph
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="graphElement">
        /// element of graph
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean GetAllNeighbors(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicat);
        /// <summary>
        /// find all neighbors for this geometry
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="geometry">
        /// geometry
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean GetAllNeighbors(out ReadOnlyCollection<AGraphElement> result, IGeometry graphElement, Predicate<AGraphElement> predicat);
        #endregion
        #region GetNextNeighbors
        /// <summary>
        /// find k next neighbors for this element of graph
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="graphElement">
        /// element of graph
        /// </param>
        /// /// </param>
        /// <param name="countOfNextNeighbors">
        /// count of neighbors
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean GetNextNeighbors(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, int countOfNextNeighbors, Predicate<AGraphElement> predicat);
        /// <summary>
        /// find k next neighbors for this geometry
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="geometry">
        /// geometry
        /// </param>
        /// /// </param>
        /// <param name="countOfNextNeighbors">
        /// count of neighbors
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean GetNextNeighbors(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, int countOfNextNeighbors, Predicate<AGraphElement> predicat);
        #endregion
        #region SearchDistance
        /// <summary>
        /// find all object which distance less or equal d from this element of graph have
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="distance">
        /// value of distance for serching
        /// </param>
        /// <param name="graphElement">
        /// element of graph
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean SearchDistance(out ReadOnlyCollection<AGraphElement> result,
            Double distance,
            AGraphElement graphElement,
            Predicate<AGraphElement> predicat);
        /// <summary>
        /// find all object which distance less or equal d from this geometry have
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="distance">
        /// value of distance for serching
        /// </param>
        /// <param name="geometry">
        /// geometry
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean SerchDistance(out ReadOnlyCollection<AGraphElement> result,
            Double distance,
            IGeometry geometry,
            Predicate<AGraphElement> predicat);
        #endregion
        #region SearchPoint
        /// <summary>
        /// find all objects which this point have
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="point">
        /// point
        /// </param>
        /// <param name="predicat">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        Boolean SearchPoint(out ReadOnlyCollection<AGraphElement> result, IPoint point, Predicate<AGraphElement> predicat);
        #endregion
    }
}

