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

#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#endregion

namespace NoSQL.GraphDB.Helper
{
    /// <summary>
    /// A big list
    /// </summary>
    public sealed class BigList<T>
    {
        #region data

        /// <summary>
        /// The actual data structure
        /// </summary>
        private readonly T[][] _data;

        /// <summary>
        /// The size of a single shard
        /// </summary>
        private const Int32 ShardSize = 200000000;

        /// <summary>
        /// The size of a single shard
        /// </summary>
        private const Int32 NumberOfShards = 1337;

        /// <summary>
        /// The initial size of a single shard
        /// </summary>
        private const Int32 ShardSizeInitial = 10000;

        /// <summary>
        /// The extension size
        /// </summary>
        private const Int32 ExtendSize = 1000000;

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
            _data = new T[NumberOfShards][];
        }

        #endregion

        #region public methods

        public void SetValue(Int32 index, T item)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            //find the right shard id
            var shardId = FindShardId(index);
            var shard = _data[shardId];

            //find the localSlot
            var localSlot = FindShardLocalSlot(shardId, index);

            shard = ExtendIfNeeded(shard, shardId, localSlot);

            shard[localSlot] = item;
        }

        public void SetDefault(Int32 index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            //find the right shard id
            var shardId = FindShardId(index);
            var shard = _data[shardId];

            //find the localSlot
            var localSlot = FindShardLocalSlot(shardId, index);

            shard = ExtendIfNeeded(shard, shardId, localSlot);

            shard[localSlot] = default(T);
        }

        public T GetElement(Int32 index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            //find the right shard id
            var shardId = FindShardId(index);
            var shard = _data[shardId];

            //find the localSlot
            var localSlot = FindShardLocalSlot(shardId, index);

            if (shard.Length > localSlot)
            {
                return shard[localSlot];
            }

            return default(T);
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
                    for (var i = 0; i < shard.Length; i++)
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
                    for (var i = 0; i < shard.Length; i++)
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
                    for (var i = 0; i < shard.Length; i++)
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

        #region private helper

        private T[] ExtendIfNeeded(T[] shard, long shardId, long localSlot)
        {
            if (localSlot > ShardSize)
            {
                throw new ArgumentOutOfRangeException("localSlot", "The localslot index cannot be greater than the shard size");
            }

            if (shard == null)
            {
                _data[shardId] = new T[ShardSizeInitial];
                shard = _data[shardId];
            }

            if (shard.Length > localSlot)
            {
                //no extension needed
                return shard;
            }

            //extend

            var currentLength = shard.Length;

            do
            {
                currentLength += ExtendSize;
            } while (localSlot > currentLength);

            if (currentLength > ShardSize)
            {
                currentLength = ShardSize;
            }

            var newShard = new T[currentLength];
            Array.Copy(shard, newShard, shard.Length);
            _data[shardId] = newShard;

            return newShard;
        }

        private static Int32 FindShardLocalSlot(Int32 shardId, Int32 index)
        {
            return index - (shardId * ShardSize);
        }

        private static Int32 FindShardId(Int32 index)
        {
            return index / ShardSize;
        }

        #endregion
    }
}