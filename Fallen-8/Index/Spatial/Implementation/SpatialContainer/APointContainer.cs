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

namespace NoSQL.GraphDB.Index.Spatial.Implementation.SpatialContainer
{
        /// <summary>
    /// Container for point data
    /// </summary>
    abstract public class APointContainer : IMBP, IRTreeContainer
    {
        /// <summary>
        /// type of container
        /// </summary>
        public TypeOfContainer Container { get { return TypeOfContainer.PointContainer; } }
      

        #region Inclusion of MBR and Point
        virtual public bool Inclusion(ISpatialContainer container)
        {
            return EqualTo(container);
        }
        #endregion
        #region Intersection of MBR and Point
        virtual public bool Intersection(ISpatialContainer container)
        {
            #region MBR
            if (container is ASpatialContainer)
            {
                var currentContainer = (ASpatialContainer)container;
                for (var i = 0; i < Coordinates.Length; i++)
                {
                    if (currentContainer.Lower[i] > Coordinates[i] || currentContainer.Upper[i] < Coordinates[i])
                        return false;
                }

                return true;
            }
            #endregion
            #region Point
            
            return EqualTo(container);

            #endregion
        }

            #endregion
        #region Equal
        virtual public bool EqualTo(ISpatialContainer container)
        {
            if (container is APointContainer)
            {

                var currentPoint = ((APointContainer)container).Coordinates;

                for (int i = 0; i < Coordinates.Length; i++)
                {
                    if (Coordinates[i] != currentPoint[i])
                        return false;
                }

                return true;
            }
            
            var currentLower = ((ASpatialContainer)container).Lower;
            var currentUpper = ((ASpatialContainer)container).Upper;

            for (int i = 0; i < Coordinates.Length; i++)
            {
                if (currentLower[i] != Coordinates[i] || currentUpper[i] != Coordinates[i])
                    return false;
            }
            return true;
        }
        #endregion
        #region Adjacency
        virtual public bool Adjacency(ISpatialContainer container)
        {
            #region Point
            if (container is APointContainer)
            {
                return EqualTo(container);
            }
            #endregion
            #region MBR
            
            var currentLower = ((ASpatialContainer)container).Lower;
            var currentUpper = ((ASpatialContainer)container).Upper;
            for (int i = 0; i < Coordinates.Length; i++)
            {
                if (currentLower[i] != Coordinates[i] || currentUpper[i] != Coordinates[i])
                    return false;
            }

            return true;
        }
            #endregion
        #endregion

        #region Point get,set
       
        virtual public float[] LowerPoint
        {
            get { return Coordinates; }


        }

        virtual public float[] UpperPoint
        {
            get { return Coordinates; }

        }
        #endregion
        public ARTreeContainer Parent
        {
            get;
            set;
        }




    }
    
}
