// 
// EdgeModelDefinition.cs
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
using Fallen8.API.Model;

namespace Fallen8.API.Helper
{
	/// <summary>
	/// Edge model definition.
	/// </summary>
	public struct EdgeModelDefinition
	{
        #region Data

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>
        /// The creation date.
        /// </value>
        public DateTime CreationDate;

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public readonly List<PropertyContainer> Properties;

        /// <summary>
        /// Gets or sets the target vertex identifier.
        /// </summary>
        /// <value>
        /// The target vertex identifier.
        /// </value>
        public readonly Int32 TargetVertexId;
        
        #endregion
        
        #region constructor

	    /// <summary>
	    /// Initializes a new instance of the EdgeModelDefinition class.
	    /// </summary>
	    /// <param name='targetVertex'>
	    /// Target vertex.
	    /// </param>
	    /// <param name='creationDate'>
	    /// Creation date.
	    /// </param>
	    /// <param name="properties">Properties</param>
	    public EdgeModelDefinition(Int32 targetVertex, DateTime creationDate, List<PropertyContainer> properties = null)
        {
            TargetVertexId = targetVertex;
            CreationDate = creationDate;
            Properties = properties;
        }
        
        #endregion
	}
}