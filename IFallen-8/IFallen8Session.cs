// 
// IFallen8Session.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2011 Henning Rauch
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fallen8.Model;
using System.Data;
using Fallen8.API.Service;

namespace Fallen8.API
{
	/// <summary>
	/// Fallen-8 session interface.
	/// </summary>
    public interface IFallen8Session
    {
		/// <summary>
		/// Gets the fallen-8 server.
		/// </summary>
		/// <value>
		/// The fallen-8 server.
		/// </value>
        IFallen8Server Server { get; }
        
		/// <summary>
		/// Begin a transaction with the specified isolation level.
		/// </summary>
		/// <param name='level'>
		/// Isolation level.
		/// </param>
        IFallen8Transaction Begin(IsolationLevel level = IsolationLevel.Chaos);
		
		/// <summary>
		/// Disconnect this session instance.
		/// </summary>
        void Disconnect();
    }
}
