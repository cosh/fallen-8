// 
// ServiceSecurity.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2012-2015 Henning Rauch
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
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace NoSQL.GraphDB.Service.REST
{
    /// <summary>
    /// This helper class wraps the security settings of a service plugin.
    /// Remark: Maybe refactor this to single values for each service if desired.
    /// </summary>
    public class ServiceSecurity
    {
        public WebHttpSecurityMode BindingSecurityMode { get; set; }
        public HttpClientCredentialType BindingClientCredentialType { get; set; }
        public string HTTP_S { get; set; }
        public bool SSL { get; set; }

        public ServiceSecurity(string authMethod)
        {
            authMethod = authMethod == null ? "" : authMethod; // prevent Nullpointers

            SSL = authMethod.Contains("SSL");
            bool basic = authMethod.Contains("Basic");
            bool windows = authMethod.Contains("Windows");

            BindingSecurityMode = WebHttpSecurityMode.None;
            BindingClientCredentialType = HttpClientCredentialType.None;
            HTTP_S = "http";

            if (basic)
            {
                BindingSecurityMode = WebHttpSecurityMode.TransportCredentialOnly;
                BindingClientCredentialType = HttpClientCredentialType.Basic;
            }
            if (windows)
            {
                BindingSecurityMode = WebHttpSecurityMode.TransportCredentialOnly;
                BindingClientCredentialType = HttpClientCredentialType.Windows;
            }
            if (SSL)
            {
                BindingSecurityMode = WebHttpSecurityMode.Transport;
                HTTP_S = "https";
            }
        }
    }
}
