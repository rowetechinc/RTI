/*
 * Copyright 2011, Rowe Technology Inc. 
 * All rights reserved.
 * http://www.rowetechinc.com
 * https://github.com/rowetechinc
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *  1. Redistributions of source code must retain the above copyright notice, this list of
 *      conditions and the following disclaimer.
 *      
 *  2. Redistributions in binary form must reproduce the above copyright notice, this list
 *      of conditions and the following disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 *      
 *  THIS SOFTWARE IS PROVIDED BY Rowe Technology Inc. ''AS IS'' AND ANY EXPRESS OR IMPLIED 
 *  WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 *  FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
 *  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 *  CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 *  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 *  ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 *  ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *  
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Rowe Technology Inc.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 12/16/2011      RC          1.10       Initial coding.
 * 12/19/2011      RC          1.10       Check if node exist before adding.
 * 01/27/2012      RC          1.14       Added a clear method, to clear the cache.
 * 05/29/2012      RC          2.11       Added GetFirst().
 * 
 */

using System.Collections.Generic;
using System;
using System.Threading;
using System.Diagnostics;


namespace RTI
{
    /// <summary>
    /// Coded found at: http://lrucache.codeplex.com
    /// Simple Cache object.  This will have a dictionary containing the
    /// data.  The TKey is the key used in the dictionary.  The TValue is the
    /// data being added to the dictionary.  There is also a queue
    /// that will be use to evict data.  A max size for the cache will be
    /// given in the constructor.  As data is added to cache, data is also removed
    /// from the cache based off the last item in the queue.
    /// 
    /// A more complex cache can be used System.Runtime.Caching.
    /// This one contains time stamps or change monitors for eviction.
    /// </summary>
    /// <typeparam name="TKey">Key for the Cache dictionary.</typeparam>
    /// <typeparam name="TValue">Object stored in the Cache dictionary.</typeparam>
    public class Cache<TKey, TValue> where TValue : class
    {
        /// <summary>
        /// Maximum size of the dictionary and queue.
        /// </summary>
        private uint _maxCacheSize;

        /// <summary>
        /// Dictionary to store the object based off a key.  This is
        /// used to retrieve the data quickly.
        /// </summary>
        private readonly Dictionary<TKey, TValue> _cachedNodesDictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        /// List used to determine which nodes needs to be evicted
        /// based off the location in the list.  When evictions need to be
        /// made, the items at the beginning of the list will be removed.
        /// </summary>
        private readonly List<TKey> _keyList = new List<TKey>();

        /// <summary>
        /// Cache using a dictionary and list.  The cache is used
        /// to store the values.  The list is used to keep the
        /// dictionary to a fixed size.  
        /// </summary>
        /// <param name="maxCacheSize">Size of the cache.</param>
        public Cache(uint maxCacheSize = (uint)100)
        {
            _maxCacheSize = maxCacheSize;
        }

        /// <summary>
        /// Add an item to the cache.  This will
        /// add an item to the dictionary and the queue.
        /// If the key/value already exist, do not replace.
        /// It will then check if the dictionary has it
        /// its maximum size, if it has, it will remove
        /// an item.
        /// </summary>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        public void Add(TKey key, TValue value)
        {
            // If the key already contains a value, return
            TValue node;
            if (this._cachedNodesDictionary.TryGetValue(key, out node))
            {
                //this.Delete(node);
                return;
            }

            // Add to the dictionary
            _cachedNodesDictionary.Add(key, value);

            // Add to the list
            _keyList.Add(key);

            // If max size, remove item from queue
            if (_keyList.Count > _maxCacheSize)
            {
                Remove();
            }
        }

        /// <summary>
        /// Try to retrieve the value from the cache.
        /// If the value does not exist, it will return null.
        /// </summary>
        /// <param name="key">Key of the value in the cache.</param>
        /// <returns>Value from the cache or null.</returns>
        public TValue Get(TKey key)
        {
            TValue data = null;

            //if (this.cachedNodesDictionary.TryGetValue(key, out data))
            //{
            //    return data;
            //}
            //else
            //{
            //    //Trace.WriteLine(string.Format("Cache miss for key: {0}", key.ToString()));
            //    return data;
            //}
            _cachedNodesDictionary.TryGetValue(key, out data);
            return data;

        }

        /// <summary>
        /// Number of items in the cache.
        /// </summary>
        /// <returns>Number of items in the cache.</returns>
        public int Count()
        {
            return _keyList.Count;
        }

        /// <summary>
        /// Get the key for the given index.
        /// If the index is out of range, an
        /// exception will be thrown.
        /// </summary>
        /// <param name="index">Index of item.</param>
        /// <returns>Key within the </returns>
        public TKey IndexKey(int index)
        {
            return _keyList[index];
        }

        /// <summary>
        /// Get the value for the given index.
        /// This will use the index to get the 
        /// key and get the value from the dictionary.
        /// </summary>
        /// <param name="index">Index within the list.</param>
        /// <returns>Value within the dictionary for the given index.c89.5 </returns>
        public TValue IndexValue(int index)
        {
            TKey key = _keyList[index];
            return Get(key);
        }

        /// <summary>
        /// Remove a value from the dictionary.
        /// This is usually called when the dictionary
        /// has hit its maximum size and a value needs
        /// to be removed.
        /// </summary>
        private void Remove( )
        {
            // Remove the item from the dictionary
            _cachedNodesDictionary.Remove(_keyList[0]);

            // Remove the item from the list
            _keyList.RemoveAt(0);
        }

        /// <summary>
        /// Clear the cache.  This will
        /// clear the dictionary and the list.
        /// </summary>
        public void Clear()
        {
            _cachedNodesDictionary.Clear();
            _keyList.Clear();
        }

        /// <summary>
        /// Return the first value in the dictionary.  
        /// If the dictionary is empty, then return null.
        /// </summary>
        /// <returns>Return the first value in the cache.</returns>
        public TValue GetFirst()
        {
            foreach (TValue value in _cachedNodesDictionary.Values)
            {
                return value;
            }

            return null;
        }

    }
}