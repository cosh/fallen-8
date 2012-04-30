// 
// BigList.cs
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
using System.Threading.Tasks;

namespace Fallen8.API.Helper
{
    /// <summary>
    /// A big list
    /// </summary>
    public sealed class BigList<T> 
        where T: class 
    {
        #region data

        /// <summary>
        /// The actual data structure
        /// </summary>
        private readonly T[][] _data;

        /// <summary>
        /// The extends per shard
        /// </summary>
        private readonly int[] _extends;

        /// <summary>
        /// The size of a single shard
        /// </summary>
        private const Int32 ShardSize = 134217728;
        //private const Int32 ShardSize = 268435456;

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
       
        /// <summary>
        /// The delegate to find elements in the big list
        /// </summary>
        /// <param name="objectOfT">The to be analyzed object of T</param>
        /// <returns>True or false</returns>
        public delegate Boolean ElementSeeker(T objectOfT);

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new big list
        /// </summary>
        public BigList()
        {
            _extends = new int[NumberOfShards];
            _data = new T[NumberOfShards][];
            for (int i = 0; i < NumberOfShards; i++)
            {
                _data[i] = new T[ShardSizeInitial];
                _extends[i] = 1;
            }
            
           SetValue(1, null);
        }

        /// <summary>
        /// Creates a new big list
        /// </summary>
        public BigList(BigList<T> elements)
            : this()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region public methods
        
        public void SetValue(int index, T item)
        {
            UInt16 shardIndex = 0;
            for (int i = 0; i < NumberOfShards; i++)
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


                var newShard = new T[newSize];
                Array.Copy(shard, newShard, shard.Length);
                _data[shardIndex] = newShard;
                shard = newShard;
            }

            shard[positionInShard] = item;
        }

        public void SetDefault(int index)
        {
            UInt16 shardIndex = 0;
            for (int i = 0; i < NumberOfShards; i++)
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

            if (positionInShard >= shard.Length)
            {
                var newSize = shard.Length + ExtendSize * _extends[shardIndex];
                if (newSize > ShardSize)
                {
                    newSize = ShardSize;
                }
                else
                {
                    _extends[shardIndex] *= 3;
                }


                var newShard = new T[newSize];
                Array.Copy(shard, newShard, shard.Length);
                _data[shardIndex] = newShard;
                shard = newShard;
            }

            shard[positionInShard] = null;
        }

        public bool TryGetElementOrDefault<TResult>(out TResult result, int index)
            where TResult : class 
        {
            UInt16 shardIndex = 0;
            for (int i = 0; i < NumberOfShards; i++)
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
                result = shard[positionInShard] as TResult;
                return result != null;
            }

            result = null;
            return false;
        }

        public void Clear()
        {
            foreach (var shard in _data)
            {
                Array.Clear(shard, 0, shard.Length);
            }
        }

        public List<TResult> GetAllOfType<TResult>() where TResult : class 
        {
            var lockObject = new object();
            var result = new List<TResult>();

            Parallel.ForEach(
                _data,
                () => new List<TResult>(),
                delegate(T[] shard, ParallelLoopState state, long arg3, List<TResult> arg4)
                {
                    for (int i = 0; i < shard.Length; i++)
                    {
                        var value = shard[i] as TResult;

                        if (value != null)
                        {
                            arg4.Add(value);
                        }
                    }

                    return arg4;
                },
                delegate(List<TResult> local)
                {
                    lock (lockObject)
                    {
                        result.AddRange(local);
                    }
                });

            return result;
        }

        public List<T> FindElements(ElementSeeker delgate)
        {
            var lockObject = new object();
            var result = new List<T>();

            Parallel.ForEach(
                _data,
                () => new List<T>(),
                delegate(T[] shard, ParallelLoopState state, long arg3, List<T> arg4)
                {
                    for (int i = 0; i < shard.Length; i++)
                    {
                        if (shard[i] != null && delgate(shard[i]))
                        {
                            arg4.Add(shard[i]);
                        }
                    }

                    return arg4;
                },
                delegate(List<T> local)
                {
                    lock (lockObject)
                    {
                        result.AddRange(local);
                    }
                });

            return result;
        }

        public uint GetCountOf<TInteresting>()
        {
            var lockObject = new object();
            UInt32 count = 0;

            Parallel.ForEach(
                _data,
                () => new uint(),
                delegate(T[] shard, ParallelLoopState state, long arg3, uint arg4)
                    {
                        for (int i = 0; i < shard.Length; i++)
                        {
                            if (shard[i] != null && shard[i] is TInteresting)
                            {
                                arg4++;
                            }
                        }

                        return arg4;
                    }, 
                delegate(UInt32 localSum)
                {
                    lock (lockObject)
                    {
                        count += localSum;
                    }
                });

            return count;
        }

        #endregion

        
    }
}