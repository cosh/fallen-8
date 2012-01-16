// 
// AThreadSafeElement.cs
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
using System;
using System.Threading;

namespace Fallen8.API.Helper
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
        private Int32 _usingResource = 0;
        
        protected AThreadSafeElement ()
        {
        }
        
         /// <summary>
        /// Reads the resource.
        /// Blocks if reading is currently not allowed
        /// </summary>
        /// <returns>
        /// <c>true</c> if reading is allowed; otherwise, <c>false</c>.
        /// </returns>
        protected bool ReadResource()
        {
            //>=0 indicates that the method is not in use.
            if (Interlocked.Increment(ref _usingResource) >= 0)
            {
                //Code to access a resource that is not thread safe would go here.
                return true;
            }

            //another thread writes something, so lets wait

            for (int i = 0; i < 1000; i++)
            {
                //usingResource was incremented in the if clause, so lets decrement it again
                Interlocked.Decrement(ref _usingResource);

                Thread.Sleep(1);

                if (Interlocked.Increment(ref _usingResource) >= 0)
                {
                    return true;
                }
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
            if (0 == Interlocked.CompareExchange(ref _usingResource, -1000, 0))
            {
                return true;
            }

            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(1);

                if (0 == Interlocked.CompareExchange(ref _usingResource, -1000, 0))
                {
                    return true;
                }
            }

            return false;
        }
  
        /// <summary>
        /// Writing this resource is finished
        /// </summary>
        protected void FinishWriteResource ()
        {
            //Release the lock
            Interlocked.Exchange(ref _usingResource, 0);
        }
    }
}

