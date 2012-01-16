/*
 * Copyright © 2011 
 * Rowe Technology Inc.
 * All rights reserved.
 * http://www.rowetechinc.com
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification is NOT permitted.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Comments
 * -----------------------------------------------------------------
 * 12/06/2011      RC          Initial coding
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
    /// data being added to the dictionary.  There is also a doublely linked
    /// list that will be use to evict data.  A max size for the cache will be
    /// given in the constructor.  As data is added to cache, data is also removed
    /// from the cache based off the last item in the doublely linked list.  If a 
    /// piece of data is retrieved from the cache, it is moved to the front of the
    /// cache.
    /// 
    /// A more complex cache can be used System.Runtime.Caching.
    /// This one contains time stamps or change monitors for eviction.
    /// </summary>
    /// <typeparam name="TKey">Key for the Cache dictionary.</typeparam>
    /// <typeparam name="TValue">Object stored in the Cache dictionary.</typeparam>
    public class LruCache<TKey, TValue> where TValue : class
    {
        /// <summary>
        /// Dictionary to store the object based off a key.  This is
        /// used to retrieve the data quickly.
        /// </summary>
        private readonly Dictionary<TKey, NodeInfo> cachedNodesDictionary = new Dictionary<TKey, NodeInfo>();
        
        /// <summary>
        /// List used to determine which nodes needs to be evicted
        /// based off the location in the list.  When evictions need to be
        /// made, the items at the end of the list will be removed.
        /// </summary>
        private readonly LinkedList<NodeInfo> lruLinkedList = new LinkedList<NodeInfo>();

        /// <summary>
        /// Maximum size of the cache.
        /// </summary>
        private readonly uint maxSize;

        /// <summary>
        /// Timeout value for all data.
        /// </summary>
        private readonly TimeSpan timeOut;
        
        /// <summary>
        /// Lock the data.
        /// </summary>
        private static readonly ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
        
        /// <summary>
        /// Timer that when goes up, looks for
        /// data to evict from the cache.
        /// </summary>
        private Timer cleanupTimer;

        /// <summary>
        /// Construct a cache with a given size timeout span and time to refresh the cache values
        /// for timeouts.
        /// </summary>
        /// <param name="itemExpiryTimeout">Timeout value for an object in cache.</param>
        /// <param name="maxCacheSize">Maximum size of the cache (Default 100)</param>
        /// <param name="memoryRefreshInterval">Time to check for data to evict from cache (Default 1000 (1 sec))</param>
        public LruCache(TimeSpan itemExpiryTimeout, uint maxCacheSize = (uint)100, uint memoryRefreshInterval = (uint)1000)
        {
            this.timeOut = itemExpiryTimeout;
            this.maxSize = maxCacheSize;
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            TimerCallback tcb = this.RemoveExpiredElements;
            this.cleanupTimer = new Timer(tcb, autoEvent, 0, memoryRefreshInterval);
        }

        /// <summary>
        /// Add data to the cache.
        /// Verify the size of the
        /// cache does not exceed the max before adding the data.
        /// If data already exist with the given key, then
        /// do not add the data again.
        /// </summary>
        /// <param name="key">Key to the object.</param>
        /// <param name="cacheObject">Object to store.</param>
        public void AddObject(TKey key, TValue cacheObject)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            //Trace.WriteLine(string.Format("Adding a cache object with key: {0}", key.ToString()));
            rwl.EnterWriteLock();
            try
            {
                NodeInfo node;
                if (this.cachedNodesDictionary.TryGetValue(key, out node))
                {
                    //this.Delete(node);
                    return;
                }

                this.ShrinkToSize(this.maxSize - 1);
                this.CreateNodeandAddtoList(key, cacheObject);
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }

        /// <summary>
        /// Get the object from the cache.  Use
        /// the key to find the data.  Return null
        /// if the data does not exist in the cache.
        /// If the accesscount of a node is greater than 20, 
        /// move it further up in the list to ensure it cannot
        /// be evicted.
        /// </summary>
        /// <param name="key">Key for the data.</param>
        /// <returns>Data, NULL if the data does not exist in cache.</returns>
        public TValue GetObject(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            TValue data = null;
            NodeInfo node;
            rwl.EnterReadLock();
            try
            {
                if (this.cachedNodesDictionary.TryGetValue(key, out node))
                {
                    if (node != null && !node.IsExpired())
                    {
                        //Trace.WriteLine(string.Format("Cache hit for key: {0}", key.ToString()));
                        node.AccessCount++;
                        data = node.Value;

                        if (node.AccessCount > 20)
                        {
                            ThreadPool.QueueUserWorkItem(this.AddBeforeFirstNode, key);
                        }
                    }
                }
                else
                {
                    //Trace.WriteLine(string.Format("Cache miss for key: {0}", key.ToString()));
                }

                return data;
            }
            finally
            {
                rwl.ExitReadLock();
            }
        }

        /// <summary>
        /// Return the number of elements in the
        /// cache.
        /// </summary>
        /// <returns>Number of elements in the cache.</returns>
        public int Count()
        {
            return this.cachedNodesDictionary.Count;
        }

        /// <summary>
        /// Remove any expired items from the dictionary and list.
        /// This will check if any items are expired.  If they are, 
        /// it will remove them.  This method is called from a timer.
        /// </summary>
        /// <param name="stateInfo">Timer callback parameter.</param>
        private void RemoveExpiredElements(object stateInfo)
        {
            rwl.EnterWriteLock();
            try
            {
                while (this.lruLinkedList.Last != null)
                {
                    NodeInfo node = this.lruLinkedList.Last.Value;
                    if (node != null && node.IsExpired())
                    {
                        this.Delete(node);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }

        /// <summary>
        /// Add an object to the list.
        /// </summary>
        /// <param name="userKey">Key for the object.</param>
        /// <param name="cacheObject">Object to cache.</param>
        private void CreateNodeandAddtoList(TKey userKey, TValue cacheObject)
        {
            NodeInfo node = new NodeInfo(userKey, cacheObject, (this.timeOut > DateTime.MaxValue.Subtract(DateTime.UtcNow) ? DateTime.MaxValue : DateTime.UtcNow.Add(this.timeOut)));

            node.LLNode = this.lruLinkedList.AddFirst(node);
            this.cachedNodesDictionary[userKey] = node;
        }

        /// <summary>
        /// Move the given key value up in the list to prevent
        /// a premature eviction.  This object has been requested
        /// a large amount of time so ensure it continues to live
        /// in the cache.
        /// </summary>
        /// <param name="stateinfo">Key of the object.</param>
        private void AddBeforeFirstNode(object stateinfo)
        {
            rwl.EnterWriteLock();
            try
            {
                TKey key = (TKey)stateinfo;
                NodeInfo nodeInfo;
                if (this.cachedNodesDictionary.TryGetValue(key, out nodeInfo))
                {
                    if (nodeInfo != null && !nodeInfo.IsExpired() && nodeInfo.AccessCount > 20)
                    {
                        if (nodeInfo.LLNode != this.lruLinkedList.First)
                        {
                            this.lruLinkedList.Remove(nodeInfo.LLNode);
                            nodeInfo.LLNode = this.lruLinkedList.AddBefore(this.lruLinkedList.First, nodeInfo);
                            nodeInfo.AccessCount = 0;
                        }
                    }
                }
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }

        /// <summary>
        /// Ensure the cache size does not exceed
        /// the maximum limit.  This will reduce the
        /// cache size if it exceeds the max value.
        /// </summary>
        /// <param name="desiredSize">Size of the cache.</param>
        private void ShrinkToSize(uint desiredSize)
        {
            while (this.cachedNodesDictionary.Count > desiredSize)
            {
                this.RemoveLeastValuableNode();
            }
        }

        /// <summary>
        /// Remove the least valuable node.  This will
        /// be the node at the end of the list.
        /// </summary>
        private void RemoveLeastValuableNode()
        {
            if (this.lruLinkedList.Last != null)
            {
                NodeInfo node = this.lruLinkedList.Last.Value;
                this.Delete(node);
            }
        }

        /// <summary>
        /// Remove a node from the linkedlist and the dictionary.
        /// </summary>
        /// <param name="node">Node to remove.</param>
        private void Delete(NodeInfo node)
        {
            //Trace.WriteLine(string.Format("Evicting object from cache for key: {0}", node.Key.ToString()));
            this.lruLinkedList.Remove(node.LLNode);
            this.cachedNodesDictionary.Remove(node.Key);
        }
        
        /// <summary>
        /// Class to describe a node.
        /// This class represents data stored in the LinkedList Node and Dictionary.
        /// </summary>
        private class NodeInfo
        {
            /// <summary>
            /// Timeout value to look for nodes that need to be
            /// evicted based off time.
            /// </summary>
            private readonly DateTime timeOutTime;

            /// <summary>
            /// Constructor a node based off key, the value and a
            /// timeout value.
            /// </summary>
            /// <param name="key">Key.</param>
            /// <param name="value">Object.</param>
            /// <param name="timeouttime">Timeout value.</param>
            internal NodeInfo(TKey key, TValue value, DateTime timeouttime)
            {
                this.Key = key;
                this.Value = value;
                this.timeOutTime = timeouttime;
            }

            /// <summary>
            /// Key for the object.
            /// </summary>
            internal TKey Key { get; private set; }

            /// <summary>
            /// Object to store in cache.
            /// </summary>
            internal TValue Value { get; private set; }

            /// <summary>
            /// Number of times the data has been accessed.
            /// </summary>
            internal int AccessCount { get; set; }

            /// <summary>
            /// Node for the linked list to check for evictions.
            /// </summary>
            internal LinkedListNode<NodeInfo> LLNode { get; set; }

            /// <summary>
            /// Check if this object has expired.
            /// It check the current time and the timemout time.
            /// </summary>
            /// <returns>True if time has expired.</returns>
            internal bool IsExpired()
            {
                return DateTime.UtcNow >= this.timeOutTime;
            }
        }
    }
}