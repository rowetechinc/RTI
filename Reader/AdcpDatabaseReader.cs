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
 * 05/10/2012      RC          2.11       Initial coding  
 * 05/23/2012      RC          2.11       Added GetAllEnsembles() to get all the ensembles for a project at once.
 *                                         Changed how PopulateCache() query for the data.
 * 05/29/2012      RC          2.11       Added GetEnsembles() to read in a set of ensembles.
 * 06/12/2012      RC          2.11       Added GetFirstEnsemble().
 * 
 */
namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;

    /// <summary>
    /// Cache the data from the database.  Reading in data from the database is slow.
    /// This will cache a number of ensembles so reading can be done from the cache and
    /// the IO processing of reading the database can be done in the background.
    /// </summary>
    public class AdcpDatabaseReader
    {

        #region Variables

        /// <summary>
        /// Cache to hold the data.  This will reduce
        /// the time to playback an ensemble.  The time
        /// to query the database is high and the cache
        /// allows the query to be done in the future.
        /// </summary>
        private Cache<long, DataSet.Ensemble> _cache;

        /// <summary>
        /// Flag that caching is in progress.
        /// This will prevent caching from being
        /// duplicated at the same time.
        /// </summary>
        private volatile bool _cacheInProgress;

        /// <summary>
        /// Size of the cache.
        /// </summary>
        private const int CACHE_SIZE = 100;

        /// <summary>
        /// Parse data retrieved from the sqlite database.
        /// </summary>
        private AdcpDatabaseCodec _adcpDbCodec;

        /// <summary>
        /// Keep track of the previous
        /// index to determine if a jump occurs.
        /// A jump is when the index falls outside
        /// the cached area.
        /// </summary>
        private long _prevIndex;

        /// <summary>
        /// Check if the project has changed.
        /// If the project has changed, then we have to 
        /// clear the cache.
        /// </summary>
        private Project _prevProject;

        #endregion

        /// <summary>
        /// Initialize values
        /// </summary>
        public AdcpDatabaseReader()
        {
            // Setup the codec
            _adcpDbCodec = new AdcpDatabaseCodec();

            // Setup the cache
            _cacheInProgress = false;
            _cache = new Cache<long, DataSet.Ensemble>(CACHE_SIZE);
            _prevIndex = 0;
            _prevProject = null;
        }

        /// <summary>
        /// Shutdown the object.
        /// </summary>
        public void Shutdown()
        {
            // Shutdown the codec
            _adcpDbCodec.Shutdown();
        }

        /// <summary>
        /// Playback the given ensemble.
        /// </summary>
        /// <param name="project">Project to get the data.</param>
        /// <param name="index">Index for the ensemble.</param>
        /// <returns>Ensemble read from the database.</returns>
        public DataSet.Ensemble GetEnsemble(Project project, long index)
        {
            // Verify the index is valid
            // The index starts at 1 so anything less than 0 is bad
            if (project != null && index > 0)
            {
                // Check if the cache needs to be cleared for a new project
                if (_prevProject == null || project != _prevProject)
                {
                    _prevProject = project;
                    ClearCache();
                }

                // Get the ensemble from the
                // cache or database.
                return ReadEnsemble(project, index);
            }

            // Return an empty ensemble
            return null;
        }

        /// <summary>
        /// Get a fixed number of ensembles starting at index from the project.
        /// This will open the database for the given project.   It will then read
        /// the ensemble starting at index.
        /// </summary>
        /// <param name="project">Project to read data.</param>
        /// <param name="index">Start location.</param>
        /// <param name="size">Number of ensembles to read.</param>
        /// <returns>Cache with the read ensembles.</returns>
        public Cache<long, DataSet.Ensemble> GetEnsembles(Project project, long index, uint size)
        {
            Cache<long, DataSet.Ensemble> cache = new Cache<long, DataSet.Ensemble>(size);

            // Verify the project is good
            if (project != null)
            {
                cache = _adcpDbCodec.QueryForDataSet(project, index, size);
            }

            return cache;
        }

        /// <summary>
        /// Return the first ensemble found.  This will get the total number
        /// of ensembles to have a timeout.  It will then start to look for
        /// first ensemble.  The first ensemble found will be returned.
        /// </summary>
        /// <param name="project">Project to get the ensemble.</param>
        /// <returns>First ensemble in the project.</returns>
        public DataSet.Ensemble GetFirstEnsemble(Project project)
        {
            DataSet.Ensemble ensemble = null;

            // Get the total number ensembles
            // Then return the first ensemble found
            int total = GetNumberOfEnsembles(project);
            for (int x = 0; x < total; x++)
            {
                ensemble = GetEnsemble(project, x);
                if (ensemble != null)
                {
                    return ensemble;
                }
            }

                return ensemble;
        }

        /// <summary>
        /// Get all the ensembles for the given project.  This will
        /// get ensemble from the database and store to a cache.
        /// </summary>
        /// <param name="project">Project to get the ensembles.</param>
        /// <returns>Cache of all the ensembles.</returns>
        public Cache<long, DataSet.Ensemble> GetAllEnsembles(Project project)
        {
            uint size = (uint)GetNumberOfEnsembles(project);
            Cache<long, DataSet.Ensemble> cache = new Cache<long, DataSet.Ensemble>(size);

            // Verify the project is good
            if (project != null)
            {
                cache = _adcpDbCodec.QueryForDataSet(project, 0, size);
            }

            return cache;
        }

        /// <summary>
        /// Get the number of ensembles in the given
        /// project.
        /// </summary>
        /// <param name="project">Project to determine the number of ensembles.</param>
        public int GetNumberOfEnsembles(Project project)
        {
            return AdcpDatabaseCodec.GetNumberOfEnsembles(project);
        }

        #region Read Ensemble

        /// <summary>
        /// Check if the ensemble is in the cache.
        /// If it is not in the cache, then query
        /// the database for the ensemble.  Then
        /// update the cache with the next
        /// </summary>
        /// <param name="project">Project to get the data.</param>
        /// <param name="index"></param>
        /// <returns>Ensemble read from the database or cache.</returns>
        private DataSet.Ensemble ReadEnsemble(Project project, long index)
        {
            // Check if the cache has not been populated
            if (_cache.Count() <= 0)
            {
                PopulateCache(project, index);
            }

            // Store the index
            _prevIndex = index;

            // Use a background worker to get
            // the ensemble.
            if (project != null)
            {
                // Try to get the data from the cache, if not in cache, query for data
                DataSet.Ensemble data = _cache.Get(index);
                if (data == null)
                {
                    // Query for ensemble data
                    data = _adcpDbCodec.QueryForDataSet(project, index);

                    //// Check if a jump occured
                    //// A jump is when the index is outside
                    //// the cached area
                    //if (index > (CACHE_SIZE / 2))
                    //{
                    //    if (index > _prevIndex + (CACHE_SIZE / 2) ||
                    //        index < _prevIndex - (CACHE_SIZE / 2))
                    //    {
                    //        // Jump occured
                    //        //PopulateCache(index);
                    //    }
                    //}
                }


                // Update the cache
                UpdateCache(project, index);

                // Disturbute the dataset to all subscribers
                if (data != null)
                {
                    // Create a clone so the ensemble in the
                    //cache is not modified
                    return data.Clone();
                }
            }

            return null;
        }

        #endregion

        #region Cache

        /// <summary>
        /// The cache did not have a value, 
        /// so update the cache with the last index plus the next 
        /// number of values up the maximum size of the cache.
        /// The cache will start be caching half the size of the cache.  It
        /// will then be a moving cache with the first half of the cache
        /// with previous data and the second half of the cache with 
        /// new data.
        /// </summary>
        /// <param name="project">Project to get the data.</param>
        /// <param name="startIndex">Start Index used to know where to start cacheing.</param>
        private void PopulateCache(Project project, long startIndex)
        {
            if (!_cacheInProgress)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                    _cacheInProgress = true;

                    // Set the cache for the project
                    _cache = _adcpDbCodec.QueryForDataSet(project, startIndex, CACHE_SIZE);

                    _cacheInProgress = false;
                };
                worker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Update the cache with a new entry
        /// based off the current position.
        /// The cache is moving with half the cache
        /// holding the previous ensembles from current location
        /// and the other half holding future ensembles from 
        /// the current location.
        /// </summary>
        /// <param name="project">Project to get the data.</param>
        /// <param name="index">Current position of playback.</param>
        private void UpdateCache(Project project, long index)
        {
            BackgroundWorker workerCache = new BackgroundWorker();
            workerCache.DoWork += delegate(object sCache, DoWorkEventArgs argsCache)
            {
                // Keep adding new data to the cache if it is good
                CacheData(project, index + CACHE_SIZE / 2);
            };
            workerCache.RunWorkerAsync();
        }

        /// <summary>
        /// Get the data from the database.
        /// Then add it to the cache.
        /// The cache will evict old data.
        /// </summary>
        /// <param name="project">Project to get the data.</param>
        /// <param name="index">Index of the database.</param>
        private void CacheData(Project project, long index)
        {
            // Get the data fromm the database.
            DataSet.Ensemble data = _adcpDbCodec.QueryForDataSet(project, index);

            if (data != null)
            {
                // Add to cache
                _cache.Add(index, data);

                // Add the data to the average manager
                //_avgMgr.AddEnsemble(data);
            }
        }

        /// <summary>
        /// Clear the cache.
        /// </summary>
        private void ClearCache()
        {
            // Clear the cache
            _cache.Clear();
        }

        #endregion

    }
}
