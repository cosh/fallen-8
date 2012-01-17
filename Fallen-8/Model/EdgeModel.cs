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
using System.Collections.Generic;
using System.Linq;
using Fallen8.API.Error;

namespace Fallen8.Model
{
    /// <summary>
    /// Edge model.
    /// </summary>
    public sealed class EdgeModel : AGraphElement, IEdgeModel
    {
        #region Data
        
        /// <summary>
        /// The target vertex.
        /// </summary>
        private readonly VertexModel _targetVertex;
        
        /// <summary>
        /// The edge property model.
        /// </summary>
        private readonly EdgePropertyModel _edgePropertyModel;
        
        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Fallen8.Model.EdgeModel"/> class.
        /// </summary>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        /// <param name='creationDate'>
        /// Creation date.
        /// </param>
        /// <param name='targetVertex'>
        /// Target vertex.
        /// </param>
        /// <param name='sourceEdgeProperty'>
        /// Source edge property.
        /// </param>
        /// <param name='properties'>
        /// Properties.
        /// </param>
        public EdgeModel (Int64 id, DateTime creationDate, VertexModel targetVertex, EdgePropertyModel sourceEdgeProperty, Dictionary<Int64, Object> properties) : base (id, creationDate, properties)
        {
            _targetVertex = targetVertex;
            _edgePropertyModel = sourceEdgeProperty;
        }
        
        #endregion
        
        #region IEdgeModel implementation
        public IVertexModel TargetVertex {
            get {
                return _targetVertex;
            }
        }

        public IEdgePropertyModel SourceEdgeProperty {
            get {
                return _edgePropertyModel;
            }
        }
        #endregion

        #region Equals Overrides

        public override Boolean Equals (Object obj)
        {
            // If parameter is null return false.
            if (obj == null) {
                return false;
            }

            // If parameter cannot be cast to IEdgeModel return false.
            var p = obj as EdgeModel;

            return p != null && Equals (p);
        }

        public Boolean Equals (EdgeModel p)
        {
            // If parameter is null return false:
            if ((object)p == null) {
                return false;
            }

            return _targetVertex.Id == p.TargetVertex.Id
                   && (_edgePropertyModel.SourceVertex.Id == p.SourceEdgeProperty.SourceVertex.Id)
                   && (base.PropertiesEqual(base._properties, p._properties));
        }

        public static Boolean operator == (EdgeModel a, EdgeModel b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals (a, b)) {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) {
                return false;
            }

            // Return true if the fields match:
            return a.Equals (b);
        }

        public static Boolean operator != (EdgeModel a, EdgeModel b)
        {
            return !(a == b);
        }

        public override int GetHashCode ()
        {
            return _targetVertex.GetHashCode () ^ _edgePropertyModel.SourceVertex.GetHashCode ();
        }

        #endregion

        #region IGraphElementModel implementation
        public long Id {
            get {
                return base._id;
            }
        }

        public DateTime CreationDate {
            get {
                return base._creationDate;
            }
        }

        public DateTime ModificationDate {
            get {
                return base._modificationDate;
            }
        }
        
        public IEnumerable<PropertyContainer> GetAllProperties ()
        {
            if (ReadResource ()) {
                
                List<PropertyContainer> result = new List <PropertyContainer> ();
                
                if (base._properties != null && base._properties.Count > 0) {
                    result.AddRange (base._properties.Select (_ => new PropertyContainer (_.Key, _.Value)));
                }
                FinishReadResource ();
                
                return result;
            }
            
            throw new CollisionException ();
        }

        public Boolean TryGetProperty<TResult>(out TResult result, Int64 propertyId)
        {
            if (ReadResource())
            {

                Boolean foundsth = false;
                Object rawResult;
                if (base._properties != null && base._properties.Count > 0 && base._properties.TryGetValue(propertyId, out rawResult))
                {
                    result = (TResult)rawResult;
                    foundsth = true;
                }
                else
                {
                    result = default(TResult);
                }
                FinishReadResource();

                return foundsth;
            }

            throw new CollisionException();
        }

        #endregion
     
    }
}
