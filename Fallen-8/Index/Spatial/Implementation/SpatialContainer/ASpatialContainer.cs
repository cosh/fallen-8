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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fallen8.API.Index.Spatial.Implementation.SpatialContainer
{
    /// <summary>
    /// container for spatial data
    /// </summary>
    public abstract class ASpatialContainer:ISpatialContainer,IMBR
    {
        public TypeOfContainer Container { get { return TypeOfContainer.MBRCONTAINER; } }
        #region Inclusion of MBR and Point
        virtual public bool Inclusion(ISpatialContainer container)
        {
            if (container.Container == TypeOfContainer.MBRCONTAINER)
            {
                var currentContainer = container as ASpatialContainer;
                var currentLowerEnumerator = currentContainer.LowerPoint.GetEnumerator();
                var currentUpperEnumerator = currentContainer.UpperPoint.GetEnumerator();
                foreach (Double value in this.LowerPoint)
                {
                    if (value > currentLowerEnumerator.Current)
                        return false;
                    currentLowerEnumerator.MoveNext();
                }
                foreach (Double value in this.UpperPoint)
                {
                    if (value < currentUpperEnumerator.Current)
                        return false;
                    currentUpperEnumerator.MoveNext();
                }

                return true;
            }
            else
            {
                if (container.Container == TypeOfContainer.POINTCONTAINER)
                {
                    return IsPointInclusion(container as APointContainer);
                }
                else
                    return false;
            }
        }
        #endregion
        #region Point Inclusion
        private bool IsPointInclusion(APointContainer aPointContainer)
        {
            var currentEnumerator = aPointContainer.Coordinates.GetEnumerator();

            foreach (Double value in this.LowerPoint)
            {
                if (value > currentEnumerator.Current)
                    return false;
                currentEnumerator.MoveNext();
            }

            currentEnumerator.Reset();

            foreach (Double value in this.UpperPoint)
            {
                if (value < currentEnumerator.Current)
                    return false;
                currentEnumerator.MoveNext();
            }

            return true;
        }
        #endregion
        #region Intersection of internal space
        private bool InternalIntersection(ASpatialContainer spatialContainer)
        {
            var currentLowerEnumerator = spatialContainer.LowerPoint.GetEnumerator();
            var currentUpperEnumerator = spatialContainer.UpperPoint.GetEnumerator();

            foreach (Double value in this.LowerPoint)
            {
                if (value >= currentUpperEnumerator.Current)
                    return false;
                currentUpperEnumerator.MoveNext();
            }
            foreach (Double value in this.UpperPoint)
            {
                if (value <= currentLowerEnumerator.Current)
                    return false;
                currentLowerEnumerator.MoveNext();
            }
            return true;
        }
        #endregion
        #region Intersection of MBR and Point
        virtual public bool Intersection(ISpatialContainer container)
        {
            if (container.Container == TypeOfContainer.MBRCONTAINER)
            {
                var currentContainer = container as ASpatialContainer;
                var currentLowerEnumerator = currentContainer.LowerPoint.GetEnumerator();
                var currentUpperEnumerator = currentContainer.UpperPoint.GetEnumerator();

                foreach (Double value in this.LowerPoint)
                {
                    if (value > currentUpperEnumerator.Current)
                        return false;
                    currentUpperEnumerator.MoveNext();
                }
                foreach (Double value in this.UpperPoint)
                {
                    if (value < currentLowerEnumerator.Current)
                        return false;
                    currentLowerEnumerator.MoveNext();
                }
                return true;
            }
            else
                if (container.Container == TypeOfContainer.POINTCONTAINER)
                {
                    return this.Inclusion(container);
                }
            return false;
        }
        #endregion
        #region Adjacency
        virtual  public bool Adjacency(ISpatialContainer container)
        {
            if (container.Container == TypeOfContainer.POINTCONTAINER)
            {
                var currentPointContainer = container as APointContainer;
                var currentLowerEnumerator = this.LowerPoint.GetEnumerator();
                var currentUpperEnumerator = this.UpperPoint.GetEnumerator();

                foreach (Double value in currentPointContainer.Coordinates)
                {
                    if (currentLowerEnumerator.Current != value || currentUpperEnumerator.Current != value)
                        return false;

                    currentLowerEnumerator.MoveNext();
                    currentUpperEnumerator.MoveNext();
                }
                return true;
            }
            else
                if (container.Container == TypeOfContainer.MBRCONTAINER)
                {
                    if (this.Intersection(container) && !this.InternalIntersection(container as ASpatialContainer))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
        }
        #endregion
        #region Equal
        virtual public bool EqualTo(ISpatialContainer myContainer)
        {
            if (myContainer.Container == TypeOfContainer.POINTCONTAINER)
            {

                var currentPointEnumerator = (myContainer as APointContainer).Coordinates.GetEnumerator();

                foreach (double value in this.LowerPoint)
                {
                    if (value != currentPointEnumerator.Current)
                        return false;
                    currentPointEnumerator.MoveNext();
                }
                currentPointEnumerator.Reset();
                foreach (double value in this.UpperPoint)
                {
                    if (value != currentPointEnumerator.Current)
                        return false;
                    currentPointEnumerator.MoveNext();
                }

                return true;
            }
            else
                if (myContainer.Container == TypeOfContainer.MBRCONTAINER)
                {
                    var currentLowerEnumerator = (myContainer as ASpatialContainer).LowerPoint.GetEnumerator();
                    var currentUpperEnumerator = (myContainer as ASpatialContainer).UpperPoint.GetEnumerator();

                    foreach (Double value in this.LowerPoint)
                    {
                        if (currentLowerEnumerator.Current != value)
                            return false;

                        currentLowerEnumerator.MoveNext();
                    }

                    foreach (Double value in this.UpperPoint)
                    {
                        if (currentUpperEnumerator.Current != value)
                            return false;

                        currentUpperEnumerator.MoveNext();
                    }

                    return true;
                }
                else
                    return false;
        }
        #endregion
        #region Point get,set
        virtual public IEnumerable<double> LowerPoint
      {
          get;
          protected set;

      }

      virtual public IEnumerable<double> UpperPoint
      {
          get;
          protected set;
      }
        #endregion
    }

}
