// 
// VertexModel.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fallen8.Model
{
    public sealed class VertexModel : IVertexModel
    {
        #region IVertexModel Members

        public IDictionary<long, IEdgePropertyModel> Edges
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IEdgeModel> GetIncomingEdges(long typeId, long edgeId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGraphElementModel Members

        public IDictionary<long, IComparable> Properties
        {
            get { throw new NotImplementedException(); }
        }

        public IDictionary<string, object> SchemalessProperties
        {
            get { throw new NotImplementedException(); }
        }

        public long Id
        {
            get { throw new NotImplementedException(); }
        }

        public long TypeID
        {
            get { throw new NotImplementedException(); }
        }

        public string TypeName
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEquatable<IVertexModel> Members

        public bool Equals(IVertexModel other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
