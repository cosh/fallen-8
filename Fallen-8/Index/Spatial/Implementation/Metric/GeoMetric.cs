// 
// GeoMetric.cs
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

#region Usings

using System;

#endregion

namespace NoSQL.GraphDB.Index.Spatial.Implementation.Metric
{
    /// <summary>
    /// Metric for geo data with lantidude and longiutude
    /// </summary>
    public sealed class GeoMetric : IMetric
    {
        /// <summary>
        /// the readius of earth
        /// </summary>
        public float RadiusOfEarth { get; private set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="radiusOfEarth">
        /// radius
        /// </param>
        public GeoMetric(float radiusOfEarth)
        {
            RadiusOfEarth = radiusOfEarth;
        }
        public float Distance(IMBP point1, IMBP point2)
        {
            if (point1.Coordinates.Length != 2 && point2.Coordinates.Length != 2)
                throw new Exception("The points are not in geo space");

            var currentRPointStart = point1.Coordinates;
            var currentRPointEnd = point2.Coordinates;


            var longitudeStart = currentRPointStart[1] * Math.PI / 180;
            var latitudueStart = currentRPointStart[0] * Math.PI / 180;
            var longitudeEnd = currentRPointEnd[1] * Math.PI / 180;
            var latitudeEnd = currentRPointEnd[0] * Math.PI / 180;

            var cl1 = Math.Cos(latitudueStart);
            var cl2 = Math.Cos(latitudeEnd);
            var sl1 = Math.Sin(latitudueStart);
            var sl2 = Math.Sin(latitudeEnd);
            var delta = longitudeEnd - longitudeStart;
            var cdelta = Math.Cos(delta);
            var sdelta = Math.Sin(delta);

            
            var y = Math.Sqrt(Math.Pow(cl2 * sdelta, 2) + Math.Pow(cl1 * sl2 - sl1 * cl2 * cdelta, 2));
            var x = sl1 * sl2 + cl1 * cl2 * cdelta;
            var angelDifference = Math.Atan2(y, x);

            float currentDistance = (float)angelDifference * RadiusOfEarth;


            return currentDistance;
        }


        public float[] TransformationOfDistance(float distance, IMBR mbr)
        {
            {
                if (mbr.Lower.Length != 2)
                    throw new Exception("The points are not in geo space");

                var result = new float[2];
                var latidtude = mbr.Lower[0] * Math.PI / 180;
                result[0] = 180 * distance / ((float)Math.PI * RadiusOfEarth);
                var dist = (float)(180 * distance / (Math.PI * RadiusOfEarth * Math.Cos(latidtude)));
                if (dist > 360)
                {

                    result[1] = 360;
                }
                else
                {
                    result[1] = dist;
                }
                return result;
            }
        }
    }

}
