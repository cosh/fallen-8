// 
// Path.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2012 Henning Rauch
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
using System.Collections;
using System.Collections.Generic;
using Fallen8.API.Model;

namespace Fallen8.API.Algorithms.Path
{
    /// <summary>
    /// The Path.
    /// </summary>
    public struct Path : IEnumerable<VertexModel>
    {
        #region Properties

        private PathElement[] _pathElements;

        private UInt16 _idx;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new path
        /// </summary>
        /// <param name="maximumLength">Maximum length.</param>
        public Path(UInt16 maximumLength = 6)
        {
            _pathElements = new PathElement[maximumLength];
            _idx = 0;
        }

        /// <summary>
        /// Create a new path
        /// </summary>
        /// <param name="maxDepth">Maximum length</param>
        /// <param name="path">The path where the to copy from</param>
        /// <param name="aPathElement">A new path element</param>
        public Path(ushort maxDepth, Path path, PathElement aPathElement)
        {
            _pathElements = new PathElement[maxDepth];
            Array.Copy(path._pathElements, this._pathElements, path._pathElements.Length);
            _idx = Convert.ToUInt16(_pathElements.Length); //works fine because the new PathElement that is going to be added
            _pathElements[_idx] = aPathElement;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Returns the elements of the path
        /// </summary>
        /// <returns>Path elements.</returns>
        public PathElement[] GetPathElements()
        {
            return _pathElements;
        }

        /// <summary>
        /// Gets the length of the path
        /// </summary>
        /// <returns>Path length</returns>
        public Int32 GetLength()
        {
            return _pathElements == null ? 0 : _idx + 1;
        }

        /// <summary>
        /// Adds a path element
        /// </summary>
        /// <param name="pathElement">PathElement.</param>
        /// <returns>True for successfull add, otherwise false</returns>
        public Boolean AddPathElement(PathElement pathElement)
        {
            if (_idx < (_pathElements.Length - 1))
            {
                _pathElements[_idx] = pathElement;
                _idx++;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the last vertex of the path.
        /// </summary>
        /// <returns>Vertex.</returns>
        public VertexModel GetLastVertex()
        {
            return _pathElements[_idx].GetTargetVertex();
        }

        #endregion

        #region IEnumerable<VertexModel> Members

        public IEnumerator<VertexModel> GetEnumerator()
        {
            if (_pathElements != null)
            {
                for (var i = 0; i < _idx + 1; i++)
                {
                    var pathElement = _pathElements[i];
                    if (pathElement.Direction == Direction.IncomingEdge)
                    {
                        yield return pathElement.Edge.SourceVertex;
                    }
                    else
                    {
                        yield return pathElement.Edge.TargetVertex;
                    }
                }
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
