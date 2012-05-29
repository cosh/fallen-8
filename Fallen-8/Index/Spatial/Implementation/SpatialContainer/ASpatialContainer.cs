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

namespace NoSQL.GraphDB.Index.Spatial.Implementation.SpatialContainer
{
    /// <summary>
    /// container for spatial data
    /// </summary>
    public abstract class ASpatialContainer : IMBR, IRTreeContainer 
    {
        public float Area;
        public TypeOfContainer Container { get { return TypeOfContainer.MBRContainer; } }
        #region Inclusion of MBR and Point
        virtual public bool Inclusion(ISpatialContainer container)
        {
            if (container is ASpatialContainer)
            {
                var currentLower = ((ASpatialContainer)container).Lower;
                var currentUpper = ((ASpatialContainer)container).Upper;


                for (int i = 0; i < this.Lower.Length; i++)
                {
                    if (this.Lower[i] > currentLower[i] || this.Upper[i] < currentUpper[i])
                        return false;
                }

                return true;
            }
            
            return IsPointInclusion((APointContainer)container);
        }

        public bool Inclusion(IMBR container)
        {
            var currentLower = container.Lower;
            var currentUpper = container.Upper;

            for (int i = 0; i < this.Lower.Length; i++)
            {

                if (this.Lower[i] > currentLower[i] || this.Upper[i] < currentUpper[i])
                    return false;
            }

            return true;
        }

        #endregion
        #region Point Inclusion
        private bool IsPointInclusion(APointContainer aPointContainer)
        {
            var currentEnumerator = aPointContainer.Coordinates;

            for (int i = 0; i < this.Lower.Length; i++)
            {
                if (this.Lower[i] > currentEnumerator[i] || this.Upper[i] < currentEnumerator[i])
                    return false;
            }

            return true;
        }
        #endregion
        #region Intersection of internal space
        private bool InternalIntersection(ASpatialContainer spatialContainer)
        {
            var currentLower = spatialContainer.Lower;
            var currentUpper = spatialContainer.Upper;
            for (int i = 0; i < this.Lower.Length; i++)
            {
                if (this.Lower[i] >= currentUpper[i] || this.Upper[i] <= currentLower[i])
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
                var currentLower = ((ASpatialContainer) container).Lower;
                var currentUpper = ((ASpatialContainer) container).Upper;
                for (int i = 0; i < this.Lower.Length; i++)
                {
                    if (this.Lower[i] > currentUpper[i] || this.Upper[i] < currentLower[i])
                        return false;
                }

                return true;
            }
            return this.Inclusion(container);
        }

        #endregion
        #region Adjacency
        virtual public bool Adjacency(ISpatialContainer container)
        {
            if (container is APointContainer)
            {
                var currentPointContainer = (APointContainer)container;

                for (int i = 0; i < this.Lower.Length; i++)
                {
                    if (this.Lower[i] != currentPointContainer.Coordinates[i]
                        || this.Upper[i] != currentPointContainer.Coordinates[i])
                        return false;
                }


                return true;
            }
            if (this.Intersection(container) && !this.InternalIntersection((ASpatialContainer)container))
                return true;
            
            return false;
        }

        #endregion
        #region Equal
        virtual public bool EqualTo(ISpatialContainer myContainer)
        {
            if (myContainer is APointContainer)
            {

                var currentPoint = ((APointContainer)myContainer).Coordinates;

                for (int i = 0; i < this.Lower.Length; i++)
                {
                    if (this.Lower[i] != currentPoint[i] || this.Upper[i] != currentPoint[i])
                        return false;
                }

                return true;
            }
            
            var currentLower = ((ASpatialContainer)myContainer).Lower;
            var currentUpper = ((ASpatialContainer)myContainer).Upper;

            for (int i = 0; i < this.Lower.Length; i++)
            {
                if (currentLower[i] != this.Lower[i] || currentUpper[i] != this.Upper[i])
                    return false;
            }

            return true;
        }
        #endregion
        #region Point get,set
        
        virtual public float[] LowerPoint
        {
            get { return Lower; }
            set { Lower = value; }


        }

        virtual public float[] UpperPoint
        {
            get { return Upper; }
            set { Upper = value; }

        }
        #endregion


        public float GetArea()
        {
            var currentArea = 1.0f;
            for (int i = 0; i < Upper.Length; i++)
            {
                currentArea *= Upper[i] - Lower[i];

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
