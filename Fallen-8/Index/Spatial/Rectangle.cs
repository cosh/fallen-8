// 
// Rectangle.cs
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

using System.Collections.Generic;
using Fallen8.API.Index.Spatial.Implementation.SpatialContainer;

namespace Fallen8.API.Index.Spatial
{
	/// <summary>
	/// Rectangle.
	/// </summary>
	public sealed class Rectangle : IGeometry
    {
        #region constructor

        public Rectangle(IPoint lowerLeft, IPoint upperRight)
        {
            LowerLeft = lowerLeft;
            UpperRight = upperRight;
        }

        #endregion

        #region

        /// <summary>
		/// Gets or sets the lower left corner.
		/// </summary>
		/// <value>
		/// The lower left corner.
		/// </value>
		public IPoint LowerLeft { get; private set; }
		
		/// <summary>
		/// Gets or sets the upper right corner.
		/// </summary>
		/// <value>
		/// The upper right corner.
		/// </value>
		public IPoint UpperRight { get; private set; }

        #endregion

        #region IGeometry

        public IMBR GeometryToMBR()
        {
            return new MBR(LowerLeft.PointToSpaceR(),UpperRight.PointToSpaceR());
        }

        public List<IDimension> Dimensions
        {
            get { return LowerLeft.Dimensions; }
        }

        #endregion

    }
}

