// 
// BidirectionalLevelSynchronousSSSP.cs
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
using NoSQL.GraphDB.Helper;

#region Usings

using NoSQL.GraphDB.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using NoSQL.GraphDB.Plugin;

#endregion

namespace NoSQL.GraphDB.Algorithms.Path
{
    /// <summary>
    ///   Bidirctional level synchronous SSSP algorithm
    /// </summary>
    public sealed class BidirectionalLevelSynchronousSSSP : IShortestPathAlgorithm
    {
        #region Data

        /// <summary>
        ///   The Fallen-8
        /// </summary>
        private Fallen8 _fallen8;

        #endregion

        #region IShortestPathAlgorithm Members

        public List<NoSQL.GraphDB.Algorithms.Path.Path> Calculate(
            int sourceVertexId,
            int destinationVertexId,
            Int32 maxDepth = 1,
            Double maxPathWeight = Double.MaxValue,
            Int32 maxResults = 1,
            PathDelegates.EdgePropertyFilter edgePropertyFilter = null,
            PathDelegates.VertexFilter vertexFilter = null,
            PathDelegates.EdgeFilter edgeFilter = null,
            PathDelegates.EdgeCost edgeCost = null,
            PathDelegates.VertexCost vertexCost = null)
        {
            #region initial checks

            VertexModel sourceVertex;
            VertexModel targetVertex;
            if (!(_fallen8.TryGetVertex(out sourceVertex, sourceVertexId) 
			      && _fallen8.TryGetVertex(out targetVertex, destinationVertexId)))
            {
                return null;
            }

            if (maxDepth == 0 || maxResults == 0 || maxResults <= 0)
            {
                return null;
            }

            #endregion

			var optimalNumberOfTasks = ParallelHelper.GetOptimalNumberOfTasks();

			//ein bitarray für jede seite
			//beim vergleich der grenzen --> && der beiden BItarrays

			return null;
        }

        #endregion

        #region private helper

        
        #endregion

        #region IPlugin Members

        public string PluginName
        {
            get { return "BLS"; }
        }

        public Type PluginCategory
        {
            get { return typeof (IShortestPathAlgorithm); }
        }

        public string Description
        {
            get
            {
                return "Bidirectional level synchronous single source shortest path algorithm.";
            }
        }

        public string Manufacturer
        {
            get { return "Henning Rauch"; }
        }

        public void Initialize(Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            _fallen8 = fallen8;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //nothing to do atm
        }

        #endregion
    }
}
