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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fallen8.API.Index.Spatial.Implementation.RTree;
using Fallen8.API.Index.Spatial.Implementation.SpatialContainer;

namespace Fallen8.API.Index.Spatial.Implementation.Metric
{
    /// <summary>
    /// Metric for n-dimensinal real space
    /// </summary>
   public class EuclidianMetric:IMetric
    {
     public  Double Distance(IMBP point1, IMBP point2)
       {
           Double currentDistance=0.0;
           
           if (point1.Coordinates.Count()==point2.Coordinates.Count()&&point1.Coordinates.Count()!=0)
                throw new Exception("The points are in different space or space not exist ");

           var currentRPointStart = point1.Coordinates.GetEnumerator();
           var currentRPointEnd = point2.Coordinates.GetEnumerator();

           do
           {
               currentDistance = (currentRPointStart.Current - currentRPointEnd.Current) *
                   (currentRPointStart.Current - currentRPointEnd.Current);
           }
           while (currentRPointEnd.MoveNext()&&currentRPointStart.MoveNext());

           currentDistance = Math.Sqrt(currentDistance);

           return currentDistance; 
       }
    }
}
