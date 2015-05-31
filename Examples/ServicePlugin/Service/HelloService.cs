//
// HelloService.cs
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

namespace Service
{
    public class HelloService : IHelloService
    {
        public HelloService ()
        {
        }

        #region IHelloService implementation

        public string Hello (string who)
        {
            return "hello " + who;
        }

        #endregion

        #region IRESTService implementation

        public void Shutdown ()
        {
            //do nothing here
        }

        #endregion

        #region IFallen8Serializable implementation

        public void Save (Framework.Serialization.SerializationWriter writer)
        {
            //do nothing here
        }

        public void Load (Framework.Serialization.SerializationReader reader, NoSQL.GraphDB.Fallen8 fallen8)
        {
            //do nothing here
        }

        #endregion

        #region IDisposable implementation

        public void Dispose ()
        {
            //do nothing here
        }

        #endregion
    }
}

