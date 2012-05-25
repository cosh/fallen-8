// 
// EdgeModel.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
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
using System.Globalization;

namespace Fallen8.API.Model
{
    /// <summary>
    ///   Edge model.
    /// </summary>
    public sealed class EdgeModel : AGraphElement
    {
        #region Constructor

        /// <summary>
        ///   Initializes a new instance of the <see cref="EdgeModel" /> class.
        /// </summary>
        /// <param name='id'> Identifier. </param>
        /// <param name='creationDate'> Creation date. </param>
        /// <param name='targetVertex'> Target vertex. </param>
        /// <param name='sourceVertex'> Source vertex. </param>
        /// <param name='properties'> Properties. </param>
        public EdgeModel(Int32 id, UInt32 creationDate, VertexModel targetVertex, VertexModel sourceVertex,
                         PropertyContainer[] properties)
            : base(id, creationDate, properties)
        {
            TargetVertex = targetVertex;
            SourceVertex = sourceVertex;
        }

        /// <summary>
        ///   Initializes a new instance of the EdgeModel class.
        /// </summary>
        /// <param name='id'> Identifier. </param>
        /// <param name='creationDate'> Creation date. </param>
        /// <param name='modificationDate'> Modification date. </param>
        /// <param name='targetVertex'> Target vertex. </param>
        /// <param name='sourceVertex'> Source vertex. </param>
        /// <param name='properties'> Properties. </param>
        internal EdgeModel(Int32 id, UInt32 creationDate, UInt32 modificationDate, VertexModel targetVertex,
                           VertexModel sourceVertex, PropertyContainer[] properties)
            : base(id, creationDate, properties)
        {
            TargetVertex = targetVertex;
            SourceVertex = sourceVertex;
            ModificationDate = modificationDate;
        }

        #endregion

        #region IEdgeModel implementation

        /// <summary>
        ///   The target vertex.
        /// </summary>
        public readonly VertexModel TargetVertex;

        /// <summary>
        ///   The source vertex.
        /// </summary>
        public readonly VertexModel SourceVertex;

        #endregion

        #region overrides

        public override string ToString()
        {
            return Id.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}