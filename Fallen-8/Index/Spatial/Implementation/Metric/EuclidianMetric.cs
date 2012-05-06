// 
// EuclidianMetric.cs
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
using System;
namespace Fallen8.API.Index.Spatial.Implementation.Metric
{
    /// <summary>
    /// Metric for n-dimensinal real space
    /// </summary>
    public sealed class EuclidianMetric : IMetric
    {
        public float Distance(IMBP point1, IMBP point2)
        {
            float currentDistance = 0.0f;

            if (point1.Coordinates.Length != point2.Coordinates.Length && point1.Coordinates.Length != 0)
                throw new Exception("The points are in different space or space not exist ");

            var currentRPointStart = point1.Coordinates;
            var currentRPointEnd = point2.Coordinates;

            for (int i = 0; i < point1.Coordinates.Length; i++)
            {
                currentDistance += (currentRPointStart[i] - currentRPointEnd[i]) *
                       (currentRPointStart[i] - currentRPointEnd[i]);
            }


            currentDistance = (float)Math.Sqrt(currentDistance);

            return currentDistance;
        }




        public float[] TransformationOfDistance(float distance, IMBR mbr)
        {
            float[] transformation = new float[mbr.LowerPoint.Length];
            for (int i = 0; i < mbr.LowerPoint.Length; i++)
                transformation[i] = distance;
            return transformation;
        }
    }
}
