// 
// ASpatialContainer.cs
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

namespace Fallen8.API.Index.Spatial.Implementation.SpatialContainer
{
    /// <summary>
    /// container for spatial data
    /// </summary>
    public abstract class ASpatialContainer : IRTreeContainer, IMBR
    {
        public Double Area;
        public TypeOfContainer Container { get { return TypeOfContainer.MBRCONTAINER; } }
        #region Inclusion of MBR and Point
        virtual public bool Inclusion(ISpatialContainer container)
        {
            if (container is ASpatialContainer)
            {
                var currentLower = ((ASpatialContainer)container).LowerPoint;
                var currentUpper = ((ASpatialContainer)container).UpperPoint;


                for (int i = 0; i < this.LowerPoint.Length; i++)
                {
                    if (this.LowerPoint[i] > currentLower[i] || this.UpperPoint[i] < currentUpper[i])
                        return false;
                }

                return true;
            }
            else
            {
                {
                    return IsPointInclusion((APointContainer)container);
                }
            }
        }
        public bool Inclusion(IMBR container)
        {
            var currentLower = container.LowerPoint;
            var currentUpper = container.UpperPoint;

            for (int i = 0; i < this.LowerPoint.Length; i++)
            {

                if (this.LowerPoint[i] > currentLower[i] || this.UpperPoint[i] < currentUpper[i])
                    return false;
            }

            return true;
        }

        #endregion
        #region Point Inclusion
        private bool IsPointInclusion(APointContainer aPointContainer)
        {
            var currentEnumerator = aPointContainer.Coordinates;

            for (int i = 0; i < this.LowerPoint.Length; i++)
            {
                if (this.LowerPoint[i] > currentEnumerator[i] || this.UpperPoint[i] < currentEnumerator[i])
                    return false;
            }

            return true;
        }
        #endregion
        #region Intersection of internal space
        private bool InternalIntersection(ASpatialContainer spatialContainer)
        {
            var currentLower = spatialContainer.LowerPoint;
            var currentUpper = spatialContainer.UpperPoint;
            for (int i = 0; i < this.LowerPoint.Length; i++)
            {
                if (this.LowerPoint[i] >= currentUpper[i] || this.UpperPoint[i] <= currentLower[i])
                    return false;
            }

            return true;
        }
        #endregion
        #region Intersection of MBR and Point
        virtual public bool Intersection(ISpatialContainer container)
        {
            if (container is ASpatialContainer)
            {
                var currentLower = ((ASpatialContainer)container).LowerPoint;
                var currentUpper = ((ASpatialContainer)container).UpperPoint;
                for (int i = 0; i < this.LowerPoint.Length; i++)
                {
                    if (this.LowerPoint[i] > currentUpper[i] || this.UpperPoint[i] < currentLower[i])
                        return false;
                }

                return true;
            }
            else
            {
                {
                    return this.Inclusion(container);
                }
            }
        }
        #endregion
        #region Adjacency
        virtual public bool Adjacency(ISpatialContainer container)
        {
            if (container is APointContainer)
            {
                var currentPointContainer = (APointContainer)container;

                for (int i = 0; i < this.LowerPoint.Length; i++)
                {
                    if (this.LowerPoint[i] != currentPointContainer.Coordinates[i]
                        || this.UpperPoint[i] != currentPointContainer.Coordinates[i])
                        return false;
                }


                return true;
            }
            else
            {
                if (this.Intersection(container) && !this.InternalIntersection((ASpatialContainer)container))
                    return true;
                else
                    return false;
            }

        }
        #endregion
        #region Equal
        virtual public bool EqualTo(ISpatialContainer myContainer)
        {
            if (myContainer is APointContainer)
            {

                var currentPoint = ((APointContainer)myContainer).Coordinates;

                for (int i = 0; i < this.LowerPoint.Length; i++)
                {
                    if (this.LowerPoint[i] != currentPoint[i] || this.UpperPoint[i] != currentPoint[i])
                        return false;
                }

                return true;
            }
            else
            {
                var currentLower = ((ASpatialContainer)myContainer).LowerPoint;
                var currentUpper = ((ASpatialContainer)myContainer).UpperPoint;


                for (int i = 0; i < this.LowerPoint.Length; i++)
                {
                    if (currentLower[i] != this.LowerPoint[i] || currentUpper[i] != this.UpperPoint[i])
                        return false;
                }

                return true;
            }

        }
        #endregion
        #region Point get,set
        virtual public Double[] LowerPoint
        {
            get;
            set;

        }

        virtual public Double[] UpperPoint
        {
            get;
            set;
        }
        #endregion


        public Double GetArea()
        {
            var currentArea = 1.0;
            for (int i = 0; i < UpperPoint.Length; i++)
            {
                currentArea *= UpperPoint[i] - LowerPoint[i];

            }
            return currentArea;
        }


        public ARTreeContainer Parent
        {
            get;
            set;
        }


    }

}
