//
// IHelloService.cs
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
using System.ServiceModel;
using System.ComponentModel;
using System.ServiceModel.Web;
using NoSQL.GraphDB.Service.REST;

namespace Service
{
    [ServiceContract (Namespace = "Fallen-8-Training", Name = "Fallen-8 training service")]
    public interface IHelloService : IRESTService
    {
        /// <summary>
        /// Say hello to someone
        /// </summary>
        /// <param name="who">The one you like to say hello to</param>
        /// <returns>Hello to someone</returns>
        [OperationContract (Name = "Hello")]
        [Description ("[F8Training] Hello: Says hello to someone.")]
        [WebGet (UriTemplate = "/hello/{who}", ResponseFormat = WebMessageFormat.Json)]
        String Hello (String who);
    }
}

