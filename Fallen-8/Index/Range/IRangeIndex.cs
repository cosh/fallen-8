// 
// IRangeIndex.cs
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
using System.Collections.ObjectModel;
using Fallen8.API.Model;

namespace Fallen8.API.Index.Range
{
	/// <summary>
	/// Fallen8 range index.
	/// </summary>
    public interface IRangeIndex : IIndex
	{
		/// <summary>
		/// Searches for graph elements lower than the the key.
		/// </summary>
		/// <returns>
		/// <c>true</c> if something was found; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='result'>
		/// Result.
		/// </param>
		/// <param name='key'>
		/// Key.
		/// </param>
		/// <param name='includeKey'>
		/// Include the key.
		/// </param>
        Boolean LowerThan(out ReadOnlyCollection<AGraphElement> result, IComparable key, bool includeKey = true);
		
		/// <summary>
		/// Searches for graph elements greater than the the key.
		/// </summary>
		/// <returns>
		/// <c>true</c> if something was found; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='result'>
		/// Result.
		/// </param>
		/// <param name='key'>
		/// Key.
		/// </param>
		/// <param name='includeKey'>
		/// Include the key.
		/// </param>
        Boolean GreaterThan(out ReadOnlyCollection<AGraphElement> result, IComparable key, bool includeKey = true);
		
		/// <summary>
		/// Searches for graph elements between a specified range
		/// </summary>
		/// <returns>
		/// <c>true</c> if something was found; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='result'>
		/// Result.
		/// </param>
		/// <param name='leftLimit'>
		/// Left limit.
		/// </param>
		/// <param name='rightLimit'>
		/// Right limit.
		/// </param>
		/// <param name='includeleftLimit'>
		/// Include the left limit.
		/// </param>
		/// <param name='includerightLimit'>
		/// Include the right limit.
		/// </param>
        Boolean Between(out ReadOnlyCollection<AGraphElement> result, IComparable leftLimit, IComparable rightLimit, bool includeleftLimit = true, bool includerightLimit = true);
	}
}

