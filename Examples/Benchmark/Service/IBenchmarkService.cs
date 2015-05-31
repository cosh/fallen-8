//
// IBenchmarkService.cs
//
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
//
// Copyright (c) 2015 Henning Rauch
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
using System.ComponentModel;
using System.ServiceModel;
using NoSQL.GraphDB.Service.REST;
using System.ServiceModel.Web;

namespace Service
{
    /// <summary>
    /// The benchmark service.
    /// </summary>
    [ServiceContract (Namespace = "Fallen-8", Name = "Fallen-8 benchmark service")]
    public interface IBenchmarkService : IRESTService
    {
        /// <summary>
        /// Create an equally distributed graph
        /// </summary>
        /// <param name="nodeCount">Node count</param>
        /// <param name="edgeCount">Edges per node</param>
        /// <returns>Some stats</returns>
        [OperationContract (Name = "CreateGraph")]
        [Description ("[F8Benchmark] CreateGraph: Creates an equally distributed graph.")]
        [WebGet (UriTemplate = "/CreateGraph?nodes={nodeCount}&edgesPerNode={edgeCount}", ResponseFormat = WebMessageFormat.Json)]
        String CreateGraph (String nodeCount, String edgeCount);

        /// <summary>
        /// Get the count of traversed edges per second
        /// </summary>
        /// <param name="iterations">Iterations</param>
        /// <returns>Some stats</returns>
        [OperationContract (Name = "BenchmarkGraph")]
        [Description ("[F8Benchmark] TPS: Gets the count of traversed edges per second.")]
        [WebGet (UriTemplate = "/algorithm/tps?iterations={iterations}", ResponseFormat = WebMessageFormat.Json)]
        String Benchmark (String iterations);
    }
}

