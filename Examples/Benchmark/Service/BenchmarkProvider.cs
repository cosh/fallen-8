//
// BenchmarkService.cs
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
using NoSQL.GraphDB;
using NoSQL.GraphDB.Model;
using System.Collections.Generic;
using NoSQL.GraphDB.Helper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;

namespace Service
{
    class BenchmarkProvider
    {
        #region Data

        private List<VertexModel> _toBeBenchenVertices = null;
        private int _numberOfToBeTestedVertices = 10000000;
        private Fallen8 _f8;

        #endregion

        #region Constructor

        public BenchmarkProvider (Fallen8 _fallen8)
        {
            this._f8 = _fallen8;
        }

        #endregion

        #region Puplic methods

        public void CreateGraph (int nodeCount, int edgeCount)
        {
            _f8.TabulaRasa ();
            var creationDate = DateHelper.ConvertDateTime (DateTime.Now);
            var vertexIDs = new List<Int32> ();
            var prng = new Random ();
            if (nodeCount < _numberOfToBeTestedVertices) {
                _numberOfToBeTestedVertices = nodeCount;
            }

            _toBeBenchenVertices = new List<VertexModel> (_numberOfToBeTestedVertices);

            for (var i = 0; i < nodeCount; i++) {
                vertexIDs.Add (_f8.CreateVertex (creationDate).Id);

            }

            if (edgeCount != 0) {
                foreach (var aVertexId in vertexIDs) {
                    var targetVertices = new HashSet<Int32> ();

                    do {
                        targetVertices.Add (vertexIDs [prng.Next (0, vertexIDs.Count)]);
                    } while (targetVertices.Count < edgeCount);

                    foreach (var aTargetVertex in targetVertices) {
                        _f8.CreateEdge (aVertexId, 0, aTargetVertex, creationDate);
                    }
                }

                _toBeBenchenVertices.AddRange (PickInterestingIDs (vertexIDs, prng)
                    .Select (aId => {
                    VertexModel v = null;

                    _f8.TryGetVertex (out v, aId);

                    return v;
                }));
            }
        }

        public string Benchmark (int myIterations = 1000)
        {
            if (_toBeBenchenVertices == null) {
                return "No vertices available";
            }

            List<VertexModel> vertices = _toBeBenchenVertices;
            var tps = new List<double> ();
            long edgeCount = 0;
            var sb = new StringBuilder ();

            Int32 range = ((vertices.Count / Environment.ProcessorCount) * 3) / 2;

            for (var i = 0; i < myIterations; i++) {
                var sw = Stopwatch.StartNew ();

                edgeCount = CountAllEdgesParallelPartitioner (vertices, range);

                sw.Stop ();

                tps.Add (edgeCount / sw.Elapsed.TotalSeconds);
            }

            sb.AppendLine (String.Format ("Traversed {0} edges ({1} in all iterations). Average: {2}TPS Median: {3}TPS StandardDeviation {4}TPS ", 
                edgeCount, (long)edgeCount * (long)myIterations, Statistics.Average (tps), Statistics.Median (tps), Statistics.StandardDeviation (tps)));

            return sb.ToString ();
        }

        #endregion

        #region private methods

        private IEnumerable<int> PickInterestingIDs (List<int> vertexIDs, Random prng)
        {
            for (int i = 0; i < _numberOfToBeTestedVertices; i++) {
                yield return vertexIDs [prng.Next (0, vertexIDs.Count)];
            }
        }

        private static long CountAllEdgesParallelPartitioner (List<VertexModel> vertices, Int32 vertexRange)
        {
            var lockObject = new object ();
            var edgeCount = 0L;
            var rangePartitioner = Partitioner.Create (0, vertices.Count, vertexRange);

            Parallel.ForEach (
                rangePartitioner,
                () => 0L,
                delegate(Tuple<int, int> range, ParallelLoopState loopstate, long initialValue) {
                    var localCount = initialValue;

                    for (var i = range.Item1; i < range.Item2; i++) {
                        ReadOnlyCollection<EdgeModel> outEdge;
                        if (vertices [i].TryGetOutEdge (out outEdge, 0)) {
                            for (int j = 0; j < outEdge.Count; j++) {
                                var vertex = outEdge [j].TargetVertex;
                                localCount++;
                            }
                        }
                    }

                    return localCount;
                },
                delegate(long localSum) {
                    lock (lockObject) {
                        edgeCount += localSum;
                    }
                });

            return edgeCount;
        }

        #endregion
    }

}

