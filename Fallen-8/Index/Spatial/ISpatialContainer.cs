// 
// ISpatialContainer.cs
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

using NoSQL.GraphDB.Index.Spatial.Implementation.SpatialContainer;

#endregion

namespace NoSQL.GraphDB.Index.Spatial
{
    /// <summary>
    /// The container for spatial objects 
    /// </summary>
    public interface ISpatialContainer
    {
        /// <summary>
        /// check whether the container parameter-container includes 
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        bool Inclusion(ISpatialContainer container);
        /// <summary>
        /// check whether the containers overlap
        /// </summary>
        /// <param name="container">
        /// container
        /// </param>
        /// <returns>
        /// result
        /// </returns>
        bool Intersection(ISpatialContainer container);
        /// <summary>
        /// check whether the containers border
        /// </summary>
        /// <param name="container">
        /// container
        /// </param>
        /// <returns>
        /// result
        /// </returns>
        bool Adjacency(ISpatialContainer container);
        /// <summary>
        /// check whether the objects have the same geometric shapes
        /// </summary>
        /// <param name="container">
        /// container
        /// </param>
        /// <returns>
        /// result
        /// </returns>
        bool EqualTo(ISpatialContainer container);
        /// <summary>
        /// The type of container: 
        /// point container for point objects or
        /// spatial container for spatial objects
        /// </summary>
        TypeOfContainer Container { get; }
        /// <summary>
        /// Lower point of minimal bounded rectangel
        /// </summary>
        float[] LowerPoint { get; }
        /// <summary>
        /// Upper point of minimal bunded rectangel
        /// </summary>
        float[] UpperPoint { get; }
    }
}
