//
// BenchmarkServicePlugin.cs
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
using NoSQL.GraphDB.Plugin;
using NoSQL.GraphDB.Service.REST;
using Framework.Serialization;
using NoSQL.GraphDB;
using System.Collections.Generic;

namespace Service
{
    public sealed class BenchmarkServicePlugin : ARESTServicePlugin
    {
        public const string PLUGIN_NAME = "Fallen-8_Benchmark_Service";
        public const string MANUFACTURER = "Henning Rauch";
        public const string PLUGIN_DESCRIPTION = "Fallen-8 Benchmark Service Plugin";

        #region IFallen8Serializable implementation

        public override IRESTService LoadServiceFromSerialization (SerializationReader reader, Fallen8 fallen8)
        {
            return new BenchmarkService (fallen8);
        }

        #endregion

        #region IPlugin implementation

        public override IRESTService CreateService (Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            return new BenchmarkService (fallen8);
        }

        public override string Version {
            get { return "1.0.0"; }
        }

        public override double RESTInterfaceVersion {
            get { return 1.0; }
        }

        public override string PluginName {
            get { return PLUGIN_NAME; }
        }

        public override string Description {
            get { return PLUGIN_DESCRIPTION; }
        }

        public override string Manufacturer {
            get { return MANUFACTURER; }
        }

        public override Type RESTServiceInterfaceType {
            get { return typeof(IBenchmarkService); }
        }

        #endregion
    }
}

