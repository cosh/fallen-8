// 
// IGraphElementModel.cs
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

namespace Fallen8.Model
{
	/// <summary>
	/// Graph element model interface.
	/// </summary>
    public interface IGraphElementModel
    {
        /// <summary>
        /// Gets the system wide unique identifier.
        /// </summary>
        /// <value>
        /// The system wide unique identifier.
        /// </value>
        Int64 Id { get; }
		
		/// <summary>
		/// Gets the creation date.
		/// </summary>
		/// <value>
		/// The creation date.
		/// </value>
		DateTime CreationDate { get; }
		
		/// <summary>
		/// Gets the modification date.
		/// </summary>
		/// <value>
		/// The modification date.
		/// </value>
		DateTime ModificationDate { get; }
        
        /// <summary>
        /// Gets all properties.
        /// </summary>
        /// <returns>
        /// All properties.
        /// </returns>
        IEnumerable<PropertyContainer> GetAllProperties();
        
        /// <summary>
        /// Tries the get property.
        /// </summary>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='result'>
        /// Result.
        /// </param>
        /// <param name='propertyId'>
        /// Property identifier.
        /// </param>
        Boolean TryGetProperty(out Object result, Int64 propertyId);
    }
}
