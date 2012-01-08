// 
// AGraphElementDefinition.cs
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
using System.Collections.Generic;

namespace Fallen8.API.Helper
{
    /// <summary>
    /// An abstract graph element definition.
    /// </summary>
    public abstract class AGraphElementDefinition
    {
        #region Data
        
        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>
        /// The creation date.
        /// </value>
        public DateTime CreationDate { get; protected set; }
        
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public Dictionary<Int64, Object> Properties { get; protected set; }
        #endregion
            
        #region constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Fallen8.API.Helper.AGraphElementDefinition"/> class.
        /// </summary>
        /// <param name='creationDate'>
        /// Creation date.
        /// </param>
        protected AGraphElementDefinition (DateTime creationDate)
        {
            CreationDate = creationDate;
        }
        
        #endregion
        
        /// <summary>
        /// Adds the property internal.
        /// </summary>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        /// <param name='val'>
        /// Value.
        /// </param>
        protected void AddPropertyInternal (Int64 id, Object val)
        {
            if (Properties == null) {
                Properties = new Dictionary<long, Object> ();
            }
            
            Properties.Add (id, val);
        }
    }
}

