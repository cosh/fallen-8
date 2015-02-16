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

#region Usings

using System;
using System.Collections.ObjectModel;
using NoSQL.GraphDB.Error;
using NoSQL.GraphDB.Helper;

#endregion

namespace NoSQL.GraphDB.Model
{
    /// <summary>
    ///   A graph element.
    /// </summary>
    public abstract class AGraphElement : AThreadSafeElement
    {
        #region Data

        /// <summary>
        ///   The identifier of this graph element.
        /// </summary>
        public Int32 Id;

        /// <summary>
        ///   The creation date.
        /// </summary>
        public readonly UInt32 CreationDate;

        /// <summary>
        ///   The modification date.
        /// </summary>
        public UInt32 ModificationDate;

        /// <summary>
        ///   The properties.
        /// </summary>
        private PropertyContainer[] _properties;

        #endregion

        #region constructor

        /// <summary>
        ///   Initializes a new instance of the <see cref="AGraphElement" /> class.
        /// </summary>
        /// <param name='id'> Identifier. </param>
        /// <param name='creationDate'> Creation date. </param>
        /// <param name='properties'> Properties. </param>
        protected AGraphElement(Int32 id, UInt32 creationDate, PropertyContainer[] properties)
        {
            Id = id;
            CreationDate = creationDate;
            ModificationDate = 0;
            _properties = properties;
        }

        #endregion

        #region public methods

        /// <summary>
        ///  Gets the creation date
        /// </summary>
        /// <returns> Creation date </returns>
        public DateTime GetCreationDate()
        {
            if (ReadResource())
            {
                try
                {
                    var creationDate = DateHelper.GetDateTimeFromUnixTimeStamp(CreationDate);
                    return creationDate;
                }
                finally
                {
                    FinishReadResource();
                }

            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///  Gets the modification date
        /// </summary>
        /// <returns> Modification date </returns>
        public DateTime GetModificationDate()
        {
            if (ReadResource())
            {
                try
                {
                    var modificationDate = DateHelper.GetDateTimeFromUnixTimeStamp(CreationDate + ModificationDate);
                    return modificationDate;
                }
                finally
                {
                    FinishReadResource();
                }

            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///   Returns the count of properties
        /// </summary>
        /// <returns> Count of Properties </returns>
        public Int32 GetPropertyCount()
        {
            if (ReadResource())
            {
                try
                {
                    var count = _properties.Length;
                    return count;
                }
                finally
                {
                    FinishReadResource();
                }

            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///   Gets all properties.
        /// </summary>
        /// <returns> All properties. </returns>
        public ReadOnlyCollection<PropertyContainer> GetAllProperties()
        {
            if (ReadResource())
            {
                try
                {
                    var result = _properties != null
                        ? new ReadOnlyCollection<PropertyContainer>(_properties)
                        : new ReadOnlyCollection<PropertyContainer>(new PropertyContainer[0]);

                    return result;
                }
                finally
                {
                    FinishReadResource();
                }

            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///   Tries the get property.
        /// </summary>
        /// <typeparam name="TProperty"> Type of the property </typeparam>
        /// <param name="result"> Result. </param>
        /// <param name="propertyId"> Property identifier. </param>
        /// <returns> <c>true</c> if something was found; otherwise, <c>false</c> . </returns>
        public Boolean TryGetProperty<TProperty>(out TProperty result, UInt16 propertyId)
        {
            if (ReadResource())
            {
                try
                {
                    if (_properties != null)
                    {
                        for (var i = 0; i < _properties.Length; i++)
                        {
                            var aPropContainer = _properties[i];
                            if (aPropContainer.Value != null && aPropContainer.PropertyId == propertyId)
                            {
                                result = (TProperty)aPropContainer.Value;
                                return true;
                            }
                        }
                    }

                    result = default(TProperty);

                    return false;
                }
                finally
                {
                    FinishReadResource();
                }

            }

            throw new CollisionException(this);
        }

        #endregion

        #region internal methods

        /// <summary>
        ///   Trims the graph element
        /// </summary>
        internal virtual void Trim()
        {
            //do nothing
        }

        /// <summary>
        ///   Tries to add a property.
        /// </summary>
        /// <returns> <c>true</c> if it was an update; otherwise, <c>false</c> . </returns>
        /// <param name='propertyId'> If set to <c>true</c> property identifier. </param>
        /// <param name='property'> If set to <c>true</c> property. </param>
        /// <exception cref='CollisionException'>Is thrown when the collision exception.</exception>
        internal bool TryAddProperty(UInt16 propertyId, object property)
        {
            if (WriteResource())
            {
                try
                {
                    var foundProperty = false;
                    var idx = 0;

                    if (_properties != null)
                    {
                        for (var i = 0; i < _properties.Length; i++)
                        {
                            if (_properties[i].PropertyId == propertyId)
                            {
                                foundProperty = true;
                                idx = i;
                                break;
                            }
                        }

                        if (!foundProperty)
                        {
                            //resize
                            var newProperties = new PropertyContainer[_properties.Length + 1];
                            Array.Copy(_properties, newProperties, _properties.Length);
                            newProperties[_properties.Length] = new PropertyContainer { PropertyId = propertyId, Value = property };

                            _properties = newProperties;
                        }
                        else
                        {
                            _properties[idx] = new PropertyContainer { PropertyId = propertyId, Value = property };
                        }
                    }
                    else
                    {
                        _properties = new PropertyContainer[0];
                        _properties[0] = new PropertyContainer { PropertyId = propertyId, Value = property };
                    }

                    //set the modificationdate
                    ModificationDate = DateHelper.GetModificationDate(CreationDate);

                    return foundProperty;
                }
                finally
                {
                    FinishWriteResource();
                }

            }

            throw new CollisionException(this);
        }

        /// <summary>
        ///   Tries to remove a property.
        /// </summary>
        /// <returns> <c>true</c> if the property was removed; otherwise, <c>false</c> if there was no such property. </returns>
        /// <param name='propertyId'> If set to <c>true</c> property identifier. </param>
        /// <exception cref='CollisionException'>Is thrown when the collision exception.</exception>
        internal bool TryRemoveProperty(UInt16 propertyId)
        {
            if (WriteResource())
            {
                try
                {
                    var removedSomething = false;

                    if (_properties != null)
                    {
                        var toBeRemovedIdx = 0;

                        for (var i = 0; i < _properties.Length; i++)
                        {
                            if (_properties[i].PropertyId == propertyId)
                            {
                                toBeRemovedIdx = i;
                                removedSomething = true;
                                break;
                            }
                        }

                        if (removedSomething)
                        {
                            //resize
                            var newProperties = new PropertyContainer[_properties.Length - 1];
                            if (newProperties.Length != 0)
                            {
                                //everything until the to be removed item
                                Array.Copy(_properties, newProperties, toBeRemovedIdx);

                                if (toBeRemovedIdx > newProperties.Length)
                                {
                                    //everything after the removed item
                                    Array.Copy(_properties, toBeRemovedIdx + 1, newProperties, toBeRemovedIdx,
                                               _properties.Length - toBeRemovedIdx);
                                }

                                _properties = newProperties;
                            }

                            //set the modificationdate
                            ModificationDate = DateHelper.GetModificationDate(CreationDate);
                        }
                    }
                    return removedSomething;
                }
                finally
                {
                    FinishWriteResource();
                }

            }

            throw new CollisionException(this);
        }

        /// <summary>
        /// Sets the id of the element
        /// </summary>
        /// <param name="newId">The new id</param>
        internal void SetId(Int32 newId)
        {
            if (WriteResource())
            {
                Id = newId;
                FinishWriteResource();

                return;
            }

            throw new CollisionException(this);
        }

        #endregion
    }
}