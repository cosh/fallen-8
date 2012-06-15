// 
// BigBitArray.cs
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
using System.Collections;

#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#endregion

namespace NoSQL.GraphDB.Helper
{
    /// <summary>
    /// A big bit array which is accessable from Int32.MinValue to Int32.MaxValue
    /// </summary>
    public sealed class BigBitArray 
    {
        #region data

        /// <summary>
        /// The actual data structure
        /// </summary>
        private readonly BitArray[] _data;

        /// <summary>
        /// The extends per shard
        /// </summary>
        private readonly int[] _extends;

        /// <summary>
        /// The size of a single shard
        /// </summary>
        private const Int32 ShardSize = 134217728;

        /// <summary>
        /// The size of a single shard
        /// </summary>
        private const Int32 NumberOfShards = 32;

        /// <summary>
        /// The size of a single shard
        /// </summary>
        private const Int32 ShardSizeInitial = 10000;

        /// <summary>
        /// The size of a single shard
        /// </summary>
        private const Int32 ExtendSize = 1500000;
       
        #endregion

        #region constructor

        /// <summary>
        /// Creates a new big bit array
        /// </summary>
        public BigBitArray()
        {
            _extends = new int[NumberOfShards];
            _data = new BitArray[NumberOfShards];
            for (var i = 0; i < NumberOfShards; i++)
            {
                _data[i] = new BitArray(ShardSizeInitial, false);
                _extends[i] = 1;
            }
        }
       
        #endregion

        #region public methods
        
		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name='index'>
		/// Index.
		/// </param>
		/// <param name='value'>
		/// Value.
		/// </param>
        public void SetValue(int index, Boolean value)
        {
            UInt16 shardIndex = 0;
            for (var i = 0; i < NumberOfShards; i++)
            {
                var lowerLimit = Int32.MinValue + i*ShardSize;
                var upperLimit = lowerLimit + ShardSize;
                if (lowerLimit <= index && index < upperLimit)
                {
                    break;
                }
                shardIndex++;
            }
            var shard = _data[shardIndex];
            var positionInShard = index%ShardSize ;
            if (positionInShard < 0)
            {
                //because we count backwards
                positionInShard += ShardSize;
            }

            if (positionInShard >= shard.Length)
            {
                var newSize = shard.Length + ExtendSize * _extends[shardIndex];
                if (newSize>ShardSize)
                {
                    newSize = ShardSize;
                }
                else
                {
                    _extends[shardIndex] *= 3;
                }


                var newShard = new BitArray(newSize, false);
				newShard.Or(shard);
                _data[shardIndex] = newShard;
                shard = newShard;
            }

            shard[positionInShard] = value;
        }

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <returns>
		/// The value.
		/// </returns>
		/// <param name='index'>
		/// If set to <c>true</c> index.
		/// </param>
        public bool GetValue(int index)
        {
            UInt16 shardIndex = 0;
            for (var i = 0; i < NumberOfShards; i++)
            {
                var lowerLimit = Int32.MinValue + i * ShardSize;
                var upperLimit = lowerLimit + ShardSize;
                if (lowerLimit <= index && index < upperLimit)
                {
                    break;
                }
                shardIndex++;
            }
            var shard = _data[shardIndex];
            var positionInShard = index % ShardSize;
            if (positionInShard < 0)
            {
                //because we count backwards
                positionInShard += ShardSize;
            }
            
            if (positionInShard < shard.Length)
            {
                return shard[positionInShard];
            }

            return false;
        }

		/// <summary>
		/// Clear this instance.
		/// </summary>
        public void Clear()
        {
            foreach (var shard in _data)
            {
				shard.SetAll(false);
            }
        }

        #endregion
    }
}