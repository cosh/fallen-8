// 
// AThreadSafeElement.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
//       Ilya Loginov <isloginov@gmail.com>
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

#region Usings

using System;
using System.Threading;

#endregion

namespace NoSQL.GraphDB.Helper
{
    /// <summary>
    /// A thread safe element.
    /// </summary>
    public abstract class AThreadSafeElement
    {
        /// <summary>
        /// The using resource.
        /// 0 for false, 1 for true.
        /// </summary>
        private volatile Int32 _usingResource;

        /// <summary>
        /// Reads the resource.
        /// Blocks if reading is currently not allowed
        /// </summary>
        /// <returns>
        /// <c>true</c> if reading is allowed; otherwise, <c>false</c>.
        /// </returns>
        protected bool ReadResource()
        {
            for (var i = 0; i < int.MaxValue; i++) {
                while ((_usingResource & 0xfff00000) != 0)
                    Thread.Yield();

                if ((Interlocked.Increment(ref _usingResource) & 0xfff00000) == 0) {
                    return true;
                }

                Interlocked.Decrement(ref _usingResource);
            }

            return false;
        }

        /// <summary>
        /// Reading this resource is finished.
        /// </summary>
        protected void FinishReadResource ()
        {
            //Release the lock
            Interlocked.Decrement (ref _usingResource);
        }

        /// <summary>
        /// Writes the resource.
        /// Blocks if another thread reads or writes this resource
        /// </summary>
        /// <returns>
        /// <c>true</c> if writing is allowed; otherwise, <c>false</c>.
        /// </returns>
        protected bool WriteResource()
        {
            for (var i = 0; i < int.MaxValue; i++) {
                while ((_usingResource & 0xfff00000) != 0)
                    Thread.Yield();

                if ((Interlocked.Add(ref _usingResource, 0x100000) & 0xfff00000) == 0x100000) {
                    while ((_usingResource & 0x000fffff) != 0)
                        Thread.Yield();

                    return true;
                }

                Interlocked.Add(ref _usingResource, - 0x100000);
            }

            return false;
        }

        /// <summary>
        /// Writing this resource is finished
        /// </summary>
        protected void FinishWriteResource ()
        {
            //Release the lock
            Interlocked.Add(ref _usingResource, - 0x100000);
        }
    }
}

