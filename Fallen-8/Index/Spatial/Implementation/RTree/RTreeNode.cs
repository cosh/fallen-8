// 
// RTreeNode.cs
//  
// Author:
//       Andriy Kupershmidt <kuper133@googlemail.com>
// 
// Copyright (c) 2011-2015 Henning Rauch
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

using System.Collections.Generic;
using NoSQL.GraphDB.Index.Spatial.Implementation.SpatialContainer;

#endregion

namespace NoSQL.GraphDB.Index.Spatial.Implementation.RTree
{
    /// <summary>
    /// internal node of r-tree
    /// </summary>
    /// <summary>
    /// internal node of r-tree
    /// </summary>
    public sealed class RTreeNode : ARTreeContainer
    {
        public RTreeNode()
        {
            Children = new List<ARTreeContainer>();
        }
        public RTreeNode(IMBR mbr, IEnumerable<ARTreeContainer> children = null, ARTreeContainer parent = null)
        {
            if (parent != null)
                Parent = parent;
            Lower = mbr.Lower;
            Upper = mbr.Upper;
            Children = children != null ? new List<ARTreeContainer>(children) : new List<ARTreeContainer>();
        }

        public RTreeNode(float[] clower,
                        float[] cupper, IEnumerable<ARTreeContainer> children = null, ARTreeContainer parent = null)
        {
            if (parent != null)
                Parent = parent;
            Lower = clower;
            Upper = cupper;
            Children = children != null ? new List<ARTreeContainer>(children) : new List<ARTreeContainer>();
        }
        override public bool IsLeaf { get { return false; } }
        public List<ARTreeContainer> Children;


        public override void Dispose()
        {
            Children.Clear();
            Children = null;
            Parent = null;
            Lower = null;
            Upper = null;
        }

    }
}

