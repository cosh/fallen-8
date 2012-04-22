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
using System;
using System.Linq;
using System.Text;


namespace Fallen8.API.Index.Spatial.Implementation.Metric
{
    /// <summary>
    /// Metric for geo data with lantidude and longiutude
    /// </summary>
    public class GeoMetric : IMetric
    {
        /// <summary>
        /// the readius of earth
        /// </summary>
        public Double RadiusOfEarth { get; private set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="radiusOfEarth">
        /// radius
        /// </param>
        public GeoMetric(Double radiusOfEarth)
        {
            RadiusOfEarth = radiusOfEarth;
        }
        public Double Distance(IMBP point1, IMBP point2)
        {
            Double currentDistance = 0.0;

            if (point1.Coordinates.Count() != 2 && point2.Coordinates.Count() != 2)
                throw new Exception("The points are not in geo space");

            var currentRPointStart = point1.Coordinates;
            var currentRPointEnd = point2.Coordinates;

            var longiutudeStart = currentRPointStart[0];
            var lontidueStart = currentRPointStart[1];
            var longitudeEnd = currentRPointEnd[0];
            var lontidudeEnd = currentRPointEnd[1];

            currentDistance = RadiusOfEarth * Math.Atan(
                Math.Sqrt(
                    Math.Pow(Math.Cos(lontidudeEnd) * Math.Sin(longiutudeStart - longitudeEnd), 2) +
                    Math.Pow(Math.Cos(lontidueStart) * Math.Sin(lontidudeEnd) -
                            Math.Sin(lontidueStart) * Math.Cos(lontidudeEnd) *
                            Math.Cos(longiutudeStart - longitudeEnd), 2)) /
                (Math.Sin(lontidueStart) * Math.Sin(lontidudeEnd) +
                Math.Cos(lontidudeEnd) * Math.Cos(lontidueStart) * Math.Cos(longiutudeStart - longitudeEnd)));

            return currentDistance;
        }


        public Double[] TransformationOfDistance(double distance, int countOfAxis)
        {
            if (countOfAxis != 2)
                throw new Exception("The points are not in geo space");
            throw new NotImplementedException();
        }
    }

}
