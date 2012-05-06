// 
// IMetric.cs
//  
// Author:
//       Andriy Kupershmidt <kuper133@googlemail.com>
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

namespace Fallen8.API.Index.Spatial
{
    /// <summary>
    ///IMetric is the metric for n-dimensional real space.
    /// </summary>
    public interface IMetric
    {
        /// <summary>    
        /// It is the function for calculation of distance between two points
        /// </summary>
        /// <param name="myPoint1">
        /// first point from real space
        /// </param>
        /// <param name="myPoint2">
        /// second point from real space
        /// </param>
        /// <returns>
        /// distance between two point-objects
        /// </returns>
        float Distance(IMBP myPoint1, IMBP myPoint2);
        /// <summary>
        /// transformation for all axis to find minimal bounded rechtangle
        /// </summary>
        /// <param name="distance">
        /// distance
        /// </param>
        /// <param name="mbr">
        /// minimal bounded rectangel
        /// </param>
        /// <returns>
        /// distance for all axis
        /// </returns>
        float[] TransformationOfDistance(float distance, IMBR mbr);
    }
}
