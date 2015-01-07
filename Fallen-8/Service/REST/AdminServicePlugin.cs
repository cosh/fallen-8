// 
//  AdminServicePlugin.cs
//  
//  Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
//       Norbert Schuler <norbert.schuler@b-s-s.de>
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

using Framework.Serialization;
using NoSQL.GraphDB.Plugin;
using System.Collections.Generic;

#endregion

namespace NoSQL.GraphDB.Service.REST
{
    /// <summary>
    ///   Fallen-8 Admin REST service.
    /// </summary>
    public sealed class AdminServicePlugin : ARESTServicePlugin
    {
        public static string PLUGIN_NAME = "Fallen-8_Admin_Service";

        public override string Version
        {
            get { return "2.3.0"; }
        }

        public override double RESTInterfaceVersion
        {
            get { return 1.0; }
        }

        public override string PluginName
        {
            get { return PLUGIN_NAME; }
        }

        public override string Description
        {
            get { return "Fallen-8 Admin Service Plugin"; }
        }

        public override string Manufacturer
        {
            get { return "Henning Rauch"; }
        }

        public override System.Type RESTServiceInterfaceType
        {
            get { return typeof(IAdminService); }
        }

        public override IRESTService LoadServiceFromSerialization(SerializationReader reader, Fallen8 fallen8)
        {
            return new AdminService(fallen8);
        }

        public override IRESTService CreateService(Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            return new AdminService(fallen8);
        }
    }
}