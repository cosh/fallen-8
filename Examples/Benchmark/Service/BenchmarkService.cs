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
using System.Diagnostics;

namespace Service
{
    public class BenchmarkService  : IBenchmarkService, IDisposable
    {
        #region Data

        /// <summary>
        ///   The internal Fallen-8 instance
        /// </summary>
        private readonly Fallen8 _fallen8;

        /// <summary>
        /// The intro provider.
        /// </summary>
        private readonly BenchmarkProvider _benchmarkProvider;

        #endregion

        #region Contrcutor

        public BenchmarkService (Fallen8 fallen8)
        {
            this._fallen8 = fallen8;
            this._benchmarkProvider = new BenchmarkProvider (this._fallen8);
        }

        #endregion

        #region IBenchmarkService implementation

        public string CreateGraph (string nodeCount, string edgeCount)
        {
            var sw = Stopwatch.StartNew ();

            _benchmarkProvider.CreateGraph (Convert.ToInt32 (nodeCount), Convert.ToInt32 (edgeCount));

            sw.Stop ();

            #if __MonoCS__
            //mono specific code
            #else 
            GC.Collect();
            GC.Collect();
            GC.WaitForFullGCApproach();
            #endif

            return String.Format ("It took {0}ms to create a Fallen-8 graph with {1} nodes and {2} edges per node.", sw.Elapsed.TotalMilliseconds, nodeCount, edgeCount);
        }

        public string Benchmark (string iterations)
        {
            return _benchmarkProvider.Benchmark (Convert.ToInt32 (iterations));
        }

        #endregion

        #region IRESTService implementation

        public void Shutdown ()
        {
            //nothing to do
        }

        #endregion

        #region IFallen8Serializable implementation

        public void Save (Framework.Serialization.SerializationWriter writer)
        {
            //nothing to do
        }

        public void Load (Framework.Serialization.SerializationReader reader, NoSQL.GraphDB.Fallen8 fallen8)
        {
            //nothing to do
        }

        #endregion

        #region IDisposable implementation

        public void Dispose ()
        {
            //nothing to do
        }

        #endregion
    }
}

