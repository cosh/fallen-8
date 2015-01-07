// 
//  Fallen8Pool.cs
//  
//  Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
//  
//  Copyright (c) 2012-2015 Henning Rauch
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
using System.Collections.Concurrent;

#endregion

namespace NoSQL.GraphDB.Helper
{
    /// <summary>
    ///   Fallen-8 object pool.
    /// </summary>
    public sealed class Fallen8Pool
    {
        #region Data

        /// <summary>
        ///   The place where the Fallen-8 live.
        /// </summary>
        private readonly ConcurrentQueue<Fallen8> _instances;

        /// <summary>
        ///   The max count of instances.
        /// </summary>
        private readonly UInt32 _maxValue;

        /// <summary>
        ///   The min count of objects.
        /// </summary>
        private readonly UInt32 _minValue;

        #endregion

        #region Constructor

        /// <summary>
        ///   Initializes a new instance of the Fallen-8 object pool/> class.
        /// </summary>
        /// <param name='minValue'> The min count of instances. </param>
        /// <param name='maxValue'> The max count of instances. </param>
        public Fallen8Pool(UInt32 minValue = 1, UInt32 maxValue = 2)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentException("maxValue",
                                            String.Format("The maxvalue {0} is lower than the minvalue {1}.", maxValue,
                                                          minValue));
            }

            _minValue = minValue;
            _maxValue = maxValue;
            _instances = new ConcurrentQueue<Fallen8>();
            FillQueue();
        }

        #endregion

        #region public methods

        /// <summary>
        ///   Tries to get a pooled Fallen-8 instance.
        /// </summary>
        /// <returns> True for success. </returns>
        /// <param name='result'> The resulting Fallen-8. </param>
        public bool TryGetFallen8(out Fallen8 result)
        {
            if (_instances.TryDequeue(out result))
            {
                if (_instances.Count < _minValue)
                {
                    FillQueue();
                }

                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        ///   Recycles a Fallen-8 instance.
        /// </summary>
        /// <param name='instance'> Fallen-8 instance. </param>
        public void RecycleFallen8(Fallen8 instance)
        {
            instance.TabulaRasa();

            if (_instances.Count < _maxValue)
            {
                _instances.Enqueue(instance);
            }
        }

        #endregion

        #region private helper

        /// <summary>
        ///   Fills the queue.
        /// </summary>
        private void FillQueue()
        {
            var countOfNewInstances = _maxValue - _instances.Count;

            if (countOfNewInstances > _minValue)
            {
                //do not try to fill the pool entirely... maybe there are some recycled instances
                countOfNewInstances = (countOfNewInstances + _minValue)/2;
            }

            for (var i = 0; i < countOfNewInstances; i++)
            {
                _instances.Enqueue(new Fallen8());
            }
        }

        #endregion
    }
}