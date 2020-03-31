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
 * 03/06/2013      RC          2.18       Changed the format of the database.
 * 03/08/2013      RC          2.18       Removed cache and added an SQLiteConnection to improve performance.
 * 06/28/2013      RC          2.19       Replaced Shutdown() with IDisposable.
 * 12/09/2013      RC          2.21.0     Added GetLastEnsemble().
 * 09/02/2014      RC          3.0.1      In ReadEnsemble(), make the query take the connection so new connections will not need to be made with each call.
 * 
 */
namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using log4net;
    using System.Data.SQLite;
    using System.Data;

    /// <summary>
    /// Read the data from the Project SQLite database.  This will
    /// read the data from the database and then parse it to an 
    /// ensemble.
    /// </summary>
    public class AdcpDatabaseReader: IDisposable
    {

        #region Variables

        /// <summary>
        /// Parse data retrieved from the sqlite database.
        /// </summary>
        private AdcpDatabaseCodec _adcpDbCodec;

        /// <summary>
        /// Check if the project has changed.
        /// If the project has changed, then we have to 
        /// clear the cache.
        /// </summary>
        private Project _prevProject;

        /// <summary>
        /// Connection to the SQLite database.
        /// As the project changes, this will need to be
        /// opened and closed.
        /// </summary>
        private SQLiteConnection _cnn;

        #endregion

        /// <summary>
        /// Initialize values
        /// </summary>
        public AdcpDatabaseReader()
        {
            // Setup the codec
            _adcpDbCodec = new AdcpDatabaseCodec();

            // Initialize values
            _prevProject = null;
            _cnn = null;
        }

        /// <summary>
        /// Shutdown the object.
        /// </summary>
        public void Dispose()
        {
            // Close the project sqlite connection
            CloseConnection();

            // Shutdown the codec
            //_adcpDbCodec.Dispose();
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
                    // Set the project
                    _prevProject = project;

                    // Open connection to the database
                    OpenConnection(project);
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
        /// 
        /// The index starts with 1 in the database.
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
        /// of ensembles.  It will then start to look for
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
        /// Return the last ensemble found.  This will get the total number
        /// of ensembles.  It will then start to look for
        /// last ensemble.  The last ensemble found will be returned.
        /// </summary>
        /// <param name="project">Project to get the ensemble.</param>
        /// <returns>Last ensemble in the project.</returns>
        public DataSet.Ensemble GetLastEnsemble(Project project)
        {
            DataSet.Ensemble ensemble = null;

            // Get the total number ensembles
            // Then return the last ensemble found
            // This will move backwards until it finds a good ensemble
            int total = GetNumberOfEnsembles(project);
            for (int x = total; x > 0 ; x--)
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

        #region Get Burst Ensembles

        /// <summary>
        /// Get all the unique Burst Index in the project.  
        /// From this list you can then group all the ensembles
        /// with a burst index and burst ID.
        /// </summary>
        /// <param name="project">Project containing burst data.</param>
        /// <returns>DataTable contain the unique BurstID and BurstIndex</returns>
        public DataTable GetBurstIndexList(Project project)
        {
            // Create a list of burst index from project
            DataTable dt = new DataTable();

            // Use a background worker to get
            // the ensemble.
            if (project != null)
            {
                // Get the list
                // Connection is opened and closed with this command
                string query = String.Format("SELECT DISTINCT BurstID,BurstIndex FROM tblEnsemble ORDER BY DateTime;");
                dt = DbCommon.GetDataTableFromProjectDb(project, query);
            }

            return dt;
        }

        /// <summary>
        /// Get all the ensembles for the given BurstID and BurstIndex.
        /// </summary>
        /// <param name="project">Project containing the burst data.</param>
        /// <param name="burstID">Burst ID.</param>
        /// <param name="burstIndex">Burst Index</param>
        /// <returns>All ensembles in the given burst.</returns>
        public DataTable GetBurstEnsembles(Project project, int burstID, int burstIndex)
        {
            // Create a list of burst index from project
            DataTable dt = new DataTable();

            if (project != null && burstIndex != 0)
            {
                string query = String.Format("SELECT * FROM {0} WHERE BurstID={1} AND BurstIndex={2};", DbCommon.TBL_ENS_ENSEMBLE, burstID.ToString(), burstIndex.ToString());
                dt = DbCommon.GetDataTableFromProjectDb(project, query);
            }

            return dt;
        }

        /// <summary>
        /// Convert a datatable of an ensemble and convert
        /// it to an ensemble.
        /// </summary>
        /// <param name="dtEns">Datatable containing an ensemble.</param>
        /// <returns>Ensemble object.</returns>
        public DataSet.Ensemble DataTabletoEnsemble(DataRow dtEns)
        {
            return _adcpDbCodec.ParseDataTables(null, dtEns);
        }


        #endregion

        #region Read Ensemble

        /// <summary>
        /// Check if the ensemble is in the cache.
        /// If it is not in the cache, then query
        /// the database for the ensemble.  Then
        /// update the cache with the next
        /// </summary>
        /// <param name="project">Project to get the data.</param>
        /// <param name="index">Row ID of the data.</param>
        /// <returns>Ensemble read from the database or cache.</returns>
        private DataSet.Ensemble ReadEnsemble(Project project, long index)
        {
            // Use a background worker to get
            // the ensemble.
            if (project != null)
            {
                // Verify the connection is open
                if(_cnn == null && _cnn.State != System.Data.ConnectionState.Open)
                {
                    OpenConnection(project);
                }

                // Query for ensemble data
                DataSet.Ensemble ensemble = _adcpDbCodec.QueryForDataSet(_cnn, project, index);

                // Get the NMEA data and add it to the ensemble based off project settings

                // Disturbute the dataset to all subscribers
                if (ensemble != null)
                {
                    // Create the velocity vectors for the ensemble
                    DataSet.VelocityVectorHelper.CreateVelocityVector(ref ensemble);

                    // Create a clone so the ensemble in the
                    //cache is not modified
                    return ensemble.Clone();
                }
            }

            return null;
        }

        #endregion

        #region Open/Close Database Connection

        /// <summary>
        /// Open a connection to the SQLite database.
        /// This will check if the givn project is valid and
        /// is new.  If the project has changed, then open a 
        /// connection.  If the project is the same, then do
        /// not open a new connection.  It should already be
        /// open.
        /// </summary>
        /// <param name="project">Project to open.</param>
        private void OpenConnection(Project project)
        {
            if (project != null)
            {
                // Close the previous connection if it is open
                CloseConnection();

                // Open a new connection
                _cnn = DbCommon.OpenProjectDB(project);
            }

        }

        /// <summary>
        /// Close the connect to the SQLite database.
        /// </summary>
        private void CloseConnection()
        {
            // Close the previous connection if it is open
            if (_cnn != null)
            {
                _cnn.Close();
            }
        }

        #endregion
    }
}
