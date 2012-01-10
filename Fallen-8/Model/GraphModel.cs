// 
// GraphModel.cs
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
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Fallen8.API.Error;

namespace Fallen8.Model
{
    /// <summary>
    /// Graph model.
    /// </summary>
    public sealed class GraphModel : AGraphElement, IGraphModel
    {
        #region Data
        
        /// <summary>
        /// The graph elements.
        /// </summary>
        private readonly ConcurrentDictionary<long, IGraphElementModel> _graphElements;
        
        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Fallen8.Model.GraphModel"/> class.
        /// </summary>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        /// <param name='creationDate'>
        /// Creation date.
        /// </param>
        /// <param name='properties'>
        /// Properties.
        /// </param>
        public GraphModel (Int64 id, DateTime creationDate, Dictionary<Int64, Object> properties) : base(id, creationDate, properties)
        {
            _graphElements = new ConcurrentDictionary<long, IGraphElementModel>();   
        }
        
        #endregion
        
        #region IGraphModel implementation
        
        public IVertexModel GetVertex (long id)
        {
            return GetElement (id) as IVertexModel;
        }

        public IEnumerable<IVertexModel> GetVertices ()
        {
            return _graphElements.Where (aGraphElementKV => aGraphElementKV.Value is IVertexModel).Select (aVertexKV => (IVertexModel)aVertexKV.Value);
        }

        public IEdgeModel GetEdge (long id)
        {
            return GetElement (id) as IEdgeModel;
        }

        public IEnumerable<IEdgeModel> GetEdges ()
        {
            return _graphElements.Where (aGraphElementKV => aGraphElementKV.Value is IEdgeModel).Select (aEdgeKV => (IEdgeModel)aEdgeKV.Value);
        }

        public ConcurrentDictionary<long, IGraphElementModel> Graphelements
        {
            get {
                return _graphElements;
            }
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
        
        public Boolean TryGetProperty (out Object result, Int64 propertyId)
        {
            if (ReadResource ()) {
                
                Boolean foundsth = false;
                
                if (base._properties != null && base._properties.Count > 0 && base._properties.TryGetValue (propertyId, out result)) {
                    foundsth = true;    
                }
                else
                {
                    result = null;
                }
                FinishReadResource ();
                
                return foundsth;
            }
            
            throw new CollisionException ();
        }
        
        #endregion
  
        #region private methods
        
        /// <summary>
        /// Gets an element.
        /// </summary>
        /// <returns>
        /// The element or null if there is no such element.
        /// </returns>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        private IGraphElementModel GetElement (long id)
        {
            IGraphElementModel result;
            
            _graphElements.TryGetValue (id, out result);
            
            return result;
        }
        
        #endregion
    }
}

