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

namespace Fallen8.API.Helper
{
	/// <summary>
	/// Edge model definition.
	/// </summary>
	public sealed class EdgeModelDefinition
	{
		/// <summary>
		/// Adds a property.
		/// </summary>
		/// <returns>
		/// The edge model definition.
		/// </returns>
		/// <param name='id'>
		/// Identifier.
		/// </param>
		/// <param name='property'>
		/// Property.
		/// </param>
		public EdgeModelDefinition AddProperty (Int64 id, IComparable property)
		{
			throw new NotImplementedException ();
		}
		
		/// <summary>
		/// Adds a string property.
		/// </summary>
		/// <returns>
		/// The edge model definition.
		/// </returns>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='property'>
		/// Property.
		/// </param>
		public EdgeModelDefinition AddStringProperty (String name, IComparable property)
		{
			throw new NotImplementedException ();
		}
		
		/// <summary>
		/// Sets the target vertex.
		/// </summary>
		/// <returns>
		/// The edge model definition
		/// </returns>
		/// <param name='targetVertexId'>
		/// Target vertex identifier.
		/// </param>
		public EdgeModelDefinition SetTargetVertex (Int64 targetVertexId)
		{
			throw new NotImplementedException ();
		}
	}
}