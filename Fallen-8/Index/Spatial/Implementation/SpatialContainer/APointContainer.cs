// 
// APointContainer.cs
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
    /// Container for point data
    /// </summary>
    abstract public class APointContainer : IRTreeContainer, IMBP
    {
        /// <summary>
        /// type of container
        /// </summary>
        public TypeOfContainer Container { get { return TypeOfContainer.POINTCONTAINER; } }
        /// <summary>
        /// coordination of container
        /// </summary>
        virtual public Double[] Coordinates { get; protected set; }

        #region Inclusion of MBR and Point
        virtual public bool Inclusion(ISpatialContainer container)
        {
            return this.EqualTo(container);
        }
        #endregion
        #region Intersection of MBR and Point
        virtual public bool Intersection(ISpatialContainer container)
        {
            #region MBR
            if (container is ASpatialContainer)
            {
                var currentContainer = (ASpatialContainer)container;
                for (int i = 0; i < this.Coordinates.Length; i++)
                {
                    if (currentContainer.LowerPoint[i] > this.Coordinates[i] || currentContainer.UpperPoint[i] < this.Coordinates[i])
                        return false;
                }

                return true;
            }
            #endregion
            #region Point
            else
            {
                return this.EqualTo(container);
            }

            #endregion

        }
        #endregion
        #region Equal
        virtual public bool EqualTo(ISpatialContainer container)
        {
            if (container is APointContainer)
            {

                var currentPoint = ((APointContainer)container).Coordinates;

                for (int i = 0; i < this.Coordinates.Length; i++)
                {
                    if (this.Coordinates[i] != currentPoint[i])
                        return false;
                }

                return true;
            }
            else
            {
                var currentLower = (container as ASpatialContainer).LowerPoint;
                var currentUpper = (container as ASpatialContainer).UpperPoint;

                for (int i = 0; i < this.Coordinates.Length; i++)
                {
                    if (currentLower[i] != this.Coordinates[i] || currentUpper[i] != this.Coordinates[i])
                        return false;
                }
                return true;
            }

        }
        #endregion
        #region Adjacency
        virtual public bool Adjacency(ISpatialContainer container)
        {
            #region Point
            if (container is APointContainer)
            {
                return this.EqualTo(container);
            }
            #endregion
            #region MBR
            else
            {
                var currentLower = ((ASpatialContainer)container).LowerPoint;
                var currentUpper = ((ASpatialContainer)container).UpperPoint;
                for (int i = 0; i < this.Coordinates.Length; i++)
                {
                    if (currentLower[i] != this.Coordinates[i] || currentUpper[i] != this.Coordinates[i])
                        return false;
                }

                return true;
            }

        }
            #endregion
        #endregion


        public ARTreeContainer Parent
        {
            get;
            set;
        }

    }
}
