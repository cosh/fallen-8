// 
// AGraphElement.cs
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
using System.Threading;
using Fallen8.API.Helper;
using Fallen8.API.Error;

namespace Fallen8.Model
{
    /// <summary>
    /// A graph element.
    /// </summary>
    public abstract class AGraphElement : AThreadSafeElement
    {
        #region Data
        
        /// <summary>
        /// The identifier of this graph element.
        /// </summary>
        public readonly Int32 Id;
        
        /// <summary>
        /// The creation date.
        /// </summary>
        public readonly DateTime CreationDate;
        
        /// <summary>
        /// The modification date.
        /// </summary>
        public DateTime ModificationDate;
        
        /// <summary>
        /// The properties.
        /// </summary>
        protected IDictionary<Int32, object> _properties;
  
        #endregion
        
        #region constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Fallen8.Model.AGraphElement"/> class.
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
        protected AGraphElement(Int32 id, DateTime creationDate, Dictionary<Int32, Object> properties)
        {
            Id = id;
            CreationDate = creationDate;
            ModificationDate = creationDate;
            _properties = properties;
        }
        
        #endregion
        
        #region protected helpers
        
        /// <summary>
        /// Compares the properties of a graph element
        /// </summary>
        /// <returns>
        /// <c>true</c> if the properties are equal; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='propertiesA'>
        /// Properties a.
        /// </param>
        /// <param name='propertiesB'>
        /// Properties b.
        /// </param>
        protected Boolean PropertiesEqual(IDictionary<Int32, object> propertiesA, IDictionary<Int32, object> propertiesB)
        {
            if (propertiesA == null && propertiesB == null) {
                return true;
            }
            
            if (propertiesA != null && propertiesB == null) {
                return false;
            }
            
            if (propertiesA == null && propertiesB != null) {
                return false;
            }
            
            if (propertiesA.Count != propertiesB.Count) {
                return false;
            }
            
            foreach (var aPropertyInA in propertiesA) {
                Object valueOfB;
                if (propertiesB.TryGetValue (aPropertyInA.Key, out valueOfB)) {
                    if (!aPropertyInA.Value.Equals (valueOfB)) {
                        return false;
                    }
                } else {
                    return false;
                }
            }
            
            return true;
        }
        
        #endregion
  
        #region public methods

        /// <summary>
        /// Gets all properties.
        /// </summary>
        /// <returns>
        /// All properties.
        /// </returns>
        public IEnumerable<PropertyContainer> GetAllProperties()
        {
            if (ReadResource())
            {

                List<PropertyContainer> result = new List<PropertyContainer>();

                if (_properties != null && _properties.Count > 0)
                {
                    result.AddRange(_properties.Select(_ => new PropertyContainer(_.Key, _.Value)));
                }
                FinishReadResource();

                return result;
            }

            throw new CollisionException();
        }

        /// <summary>
        /// Tries the get property.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property</typeparam>
        /// <param name="result">Result.</param>
        /// <param name="propertyId">Property identifier.</param>
        /// <returns><c>true</c> if something was found; otherwise, <c>false</c>.</returns>
        public Boolean TryGetProperty<TProperty>(out TProperty result, Int32 propertyId)
        {
            if (ReadResource())
            {

                Boolean foundsth = false;
                Object rawResult;
                if (_properties != null && _properties.Count > 0 && _properties.TryGetValue(propertyId, out rawResult))
                {
                    result = (TProperty)rawResult;
                    foundsth = true;
                }
                else
                {
                    result = default(TProperty);
                }
                FinishReadResource();

                return foundsth;
            }

            throw new CollisionException();
        }

        /// <summary>
        /// Tries to add a property.
        /// </summary>
        /// <returns>
        /// <c>true</c> if it was an update; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='propertyId'>
        /// If set to <c>true</c> property identifier.
        /// </param>
        /// <param name='property'>
        /// If set to <c>true</c> property.
        /// </param>
        /// <exception cref='CollisionException'>
        /// Is thrown when the collision exception.
        /// </exception>
        public bool TryAddProperty(Int32 propertyId, object property)
        {
            if (WriteResource ()) {
                
                Boolean foundProperty = _properties.ContainsKey (propertyId);
                
                if (foundProperty) {
                    _properties [propertyId] = property;
                } else {
                    _properties.Add (propertyId, property);
                }

                //set the modificationdate
                ModificationDate = DateTime.Now;

                FinishWriteResource ();
                return foundProperty;
            }
            
            throw new CollisionException ();
        }
        
        /// <summary>
        /// Tries to remove a property.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the property was removed; otherwise, <c>false</c> if there was no such property.
        /// </returns>
        /// <param name='propertyId'>
        /// If set to <c>true</c> property identifier.
        /// </param>
        /// <exception cref='CollisionException'>
        /// Is thrown when the collision exception.
        /// </exception>
        public bool TryRemoveProperty(Int32 propertyId)
        {
            if (WriteResource ()) {
                
                Boolean removedSomething = _properties.Remove (propertyId);

                if (removedSomething)
                {
                    //set the modificationdate
                    ModificationDate = DateTime.Now;
                }

                FinishWriteResource ();
                return removedSomething;
            }
            
            throw new CollisionException ();
            
        }
        
        #endregion
    }
}

