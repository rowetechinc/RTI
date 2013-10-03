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
 * 9/30/2011       RC                     Initial coding
 * 10/11/2011      RC                     Buffer data before writing
 *                                         Write all data in 1 transaction
 * 10/17/2011      RC                     Validate Ensemble And Ancillary before writing
 * 10/18/2011      RC                     Properly Shutdown object
 * 10/19/2011      RC                     Changed property from IsLive to IsRecording.
 *                                         Observer removed, now data received through event.
 * 11/04/2011      RC                     Check if project is selected in ProcessDataThread().
 * 11/07/2011      RC                     Clear the buffer to the file on shutdown.
 * 12/12/2011      RC          1.09       Removed IsProjectSelected().
 *                                         Removed IsRecording().  Now checked in RecorderManager.
 *                                         Removed Eventhandler to receive data to record.  Now called directly with AddIncomingData().
 *                                         Changed writing ElementMultipler for NumBeams to NumBeams.
 *                                         Write nothing for NmeaData if Nmea dataset not available.
 * 12/15/2011      RC          1.09       Fixed bug in WriteBeamToDatabase() where NumElements instead of NumBeams was being used.
 * 12/21/2011      RC          1.10       System.Data.SQLite 1.0.77 takes DateTime instead of DateTime.ToString().
 * 12/28/2011      RC          1.10       Use Ensemble's row id instead of number for foreign key for tblBottomTrack and tblBeam.
 * 12/29/2011      RC          1.11       Adding log and removing RecorderManager.
 * 01/13/2012      RC          1.12       Merged Ensemble table and Bottom Track table in database.
 * 01/20/2012      RC          1.14       Added writing Firmware version and subsystem to ensemble table.
 * 01/24/2012      RC          1.14       Used variables to create query strings.
 *                                         Fixed bug with SysSerialNumber being written to the database.
 * 01/26/2012      RC          1.14       Store Subsystem as a byte.
 * 01/30/2012      RC          1.14       Changed subsystem in firmware to subsystem index.
 * 02/23/2012      RC          2.03       Changed Status in Ensemble DataSet and Bottom Track DataSet to a Status object.
 * 04/25/2012      RC          2.10       Fix bug, if bottom track does not exist, add 0 as paramater values.
 * 09/14/2012      RC          2.15       Get and Set Adcp Commands and Options to the project database.
 * 09/18/2012      RC          2.15       In WriteEnsembleDataToDatabase(), added SubsystemConfig to the database.
 * 10/02/2012      RC          2.15       Added UpdateAdcpConfiguration() to write the AdcpConfiguration to the database.
 * 10/22/2012      RC          2.15       When writing the ensemble data, try to get the ADCP Configurtation to also write to the project.
 * 12/28/2012      RC          2.17       Make AdcpConfiguration::AdcpSubsystemConfigExist() take only 1 argument.
 * 03/06/2013      RC          2.18       Changed the format of the database.
 * 06/28/2013      RC          2.19       Replaced Shutdown() with IDisposable.
 * 07/26/2013      RC          2.19.3     Added EnsembleWriteEvent event to send when an ensemble has been written to the database.
 * 08/09/2013      RC          2.19.4     Added AppConfiguration.
 * 08/19/2013      RC          2.19.4     Put PublishEnsembleWrite() in AddIncomingData() to know when an ensemble is received to write.
 * 
 */


using System;
using System.Collections;
using System.Threading;
using System.Data.SQLite;
using System.Data.Common;
using System.Diagnostics;
using log4net;
using System.Text;
using RTI.Commands;
using System.Data;
using System.Collections.Generic;

namespace RTI
{

    /// <summary>
    /// Tables are created in Project.cs
    /// The tables are created when a project is created.
    /// 
    /// This will write data to the database when it becomes available.
    /// Data is received by subscribing to CurrentDataSetManager.Instance.ReceiveRecordDataset.
    /// To set a new project set the property SelectedProject
    /// </summary>
    public class AdcpDatabaseWriter: IDisposable
    {
        #region Variables

        /// <summary>
        /// Logger for logging error messages.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Max number of items in the
        /// queue before writing to the database.
        /// </summary>
        public const int MAX_QUEUE_SIZE = 30;

        /// <summary>
        /// Queue to hold all incoming data until written
        /// to the database file.
        /// </summary>
        private Queue _datasetQueue;

        /// <summary>
        /// Thread to process the data.
        /// </summary>
        private Thread _processDataThread;

        /// <summary>
        /// Flag used to kill the thread when
        /// shutting down.
        /// </summary>
        private bool _continue;

        /// <summary>
        /// Used to make the thread sleep and wakeup.
        /// </summary>
        private EventWaitHandle _eventWaitData;

        #region Command and Options flags and locks

        /// <summary>
        /// Flag if the AdcpConfiguration need to be updated.
        /// </summary>
        private bool _isAdcpConfigurationNeedUpdate;

        /// <summary>
        /// Lock for the _isAdcpConfigurationNeedUpdate flag.
        /// </summary>
        private object _lockIsAdcpConfigurationNeedUpdate;


        #endregion

        #region App Configuration

        /// <summary>
        /// Flag if the AppConfiguration need to be updated.
        /// </summary>
        private bool _isAppConfigurationNeedUpdate;

        /// <summary>
        /// Lock for the _isAppConfigurationNeedUpdate flag.
        /// </summary>
        private object _lockIsAppConfigurationNeedUpdate;

        /// <summary>
        /// String to hold the latest application configuration JSON string.
        /// </summary>
        private string _appConfigurationJsonStr;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Set the new Selected project and reset the database
        /// connection to the new project.
        /// </summary>
        private Project _selectedProject;
        /// <summary>
        /// Set the new Selected project and reset the database
        /// connection to the new project.
        /// </summary>
        public Project SelectedProject
        {
            private get { return _selectedProject; }
            set
            {
                // Reset database connection
                ResetDatabaseConnection();

                _selectedProject = value;
            }
        }

        #endregion

        /// <summary>
        /// Constructor 
        /// 
        /// Create queue to hold incoming ensemble data.
        /// Subscribe to receive the latest ensemble data.
        /// Start the thread to write the latest ensemble to 
        /// the project database.
        /// </summary>
        public AdcpDatabaseWriter()
        {
            // Initialize data
            _selectedProject = null;
            //IsRecording = true;

            // Create a queue to hold incoming data
            _datasetQueue = new Queue();

            // Commands and options flags and locks
            _isAdcpConfigurationNeedUpdate = false;
            _lockIsAdcpConfigurationNeedUpdate = new object();
            _isAppConfigurationNeedUpdate = false;
            _lockIsAppConfigurationNeedUpdate = new object();

            // Initialize the thread
            _continue = true;
            _eventWaitData = new EventWaitHandle(false, EventResetMode.AutoReset);
            _processDataThread = new Thread(ProcessDataThread);
            _processDataThread.Start();
        }

        /// <summary>
        /// Record the data to the database.  This
        /// will take the data and add it to the queue.
        /// When the queue becomes full, it will flush it
        /// to the database file.
        /// </summary>
        /// <param name="adcpData">Data to write.</param>
        public void AddIncomingData(DataSet.Ensemble adcpData)
        {
            // Set the latest data
            _datasetQueue.Enqueue(adcpData);

            if (_datasetQueue.Count > MAX_QUEUE_SIZE)
            {
                // Wake up the thread to process data
                _eventWaitData.Set();
            }

            // Publish the Ensemble ID that was written to the database
            // This ID is the ensemble number
            PublishEnsembleWrite(adcpData.EnsembleData.EnsembleNumber);
        }

        #region AdcpConfiguration

        /// <summary>
        /// Update the database with the latest AdcpConfiguration.
        /// </summary>
        /// <param name="config">AdcpConfiguration to update.</param>
        public void UpdateAdcpConfiguration(AdcpConfiguration config)
        {
            // Update the selected project with the latest configuration
            // Then wake up the thread to update the commands to the database
            SelectedProject.Configuration = config;

            // Update the flag
            lock (_lockIsAdcpConfigurationNeedUpdate)
            {
                _isAdcpConfigurationNeedUpdate = true;
            }

            // Wake up the thread to process data
            _eventWaitData.Set();
        }
        
        ///// <summary>
        ///// Update the database with the latest AdcpCommands.
        ///// This will set the AdcpConfiguration's AdcpCommand.
        ///// </summary>
        ///// <param name="commands">AdcpCommands to update.</param>
        //public void UpdateAdcpCommands(AdcpCommands commands)
        //{
        //    // Set the new Commands to the AdcpConfiguration
        //    SelectedProject.Configuration.Commands = commands;

        //    // Update the database.
        //    UpdateAdcpConfiguration(SelectedProject.Configuration);
        //}

        ///// <summary>
        ///// Update the database with the latest DeploymentOptions.
        ///// This will set the AdcpConfiguration's DeploymentOptions.
        ///// </summary>
        ///// <param name="options">DeploymentOptions to update.</param>
        //public void UpdateDeploymentOptions(DeploymentOptions options)
        //{
        //    // Set the new Commands to the AdcpConfiguration
        //    SelectedProject.Configuration.DeploymentOptions = options;

        //    // Update the database.
        //    UpdateAdcpConfiguration(SelectedProject.Configuration);
        //}

        ///// <summary>
        ///// Find the AdcpSubsystemConfig in the dictionary.  Set the commands to the correct
        ///// AdcpSubsystemConfig then write the AdcpConfiguration to the database.
        ///// </summary>
        ///// <param name="asConfig">AdcpSubsystemConfig to match and get the commands.</param>
        //public void UpdateAdcpSubsystemConfigurationCommands(AdcpSubsystemConfig asConfig)
        //{
        //    // Find the SubsystemConfiguration and update the commands
        //    foreach (AdcpSubsystemConfig asc in SelectedProject.Configuration.SubsystemConfigDict.Values)
        //    {
        //        if (asc == asConfig)
        //        {
        //            asc.Commands = asConfig.Commands;
        //        }
        //    }

        //    // Update the database.
        //    UpdateAdcpConfiguration(SelectedProject.Configuration);
        //}

        /// <summary>
        /// Check if the AdcpConfiguration needs to be updated in the database.
        /// If it needs to be updated, the query will be run against the database.
        /// The flag will then be reset.
        /// 
        /// The flag is locked because of a multithreaded environment.
        /// </summary>
        /// <param name="cnn">Database connection.</param>
        /// <param name="selectedProject">Selected Project to get the lateset AdcpConfiguration.</param>
        private void CheckCommandsAndOptions(SQLiteConnection cnn, Project selectedProject)
        {
            // Check if the ADCP Commands needs to be updated
            if (_isAdcpConfigurationNeedUpdate)
            {
                // Send the query to the database
                UpdateAdcpConfiguration(cnn, selectedProject);

                // Lock the flag and reset the value
                lock (_lockIsAdcpConfigurationNeedUpdate)
                {
                    _isAdcpConfigurationNeedUpdate = false;
                }
            }
        }


        /// <summary>
        /// Update the row with the latest ADCP Configuration.
        /// </summary>
        /// <param name="cnn">Connection to the database.</param>
        /// <param name="project">Project to set configuration to.</param>
        private void UpdateAdcpConfiguration(SQLiteConnection cnn, Project project)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(project.Configuration);                                    // Serialize object to JSON
            string jsonCmd = String.Format("{0} = '{1}'", DbCommon.COL_CMD_ADCP_CONFIGURATION, json);
            string query = String.Format("UPDATE {0} SET {1} WHERE ID=1;", DbCommon.TBL_ENS_OPTIONS, jsonCmd);                 // Create query string

            try
            {
                using (DbCommand cmd = cnn.CreateCommand())
                {
                    Debug.WriteLine(query);
                    cmd.CommandText = query;                // Set query string
                    cmd.ExecuteNonQuery();                  // Execute the query
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error updating AdcpConfiguration in project: {0}", query), e);
            }
        }

        #endregion

        #region App Configuration

        /// <summary>
        /// Store the config temporarly.  Then set the flag
        /// that the application configuration needs to be updated.
        /// Then wakeup the write thread to do the write.
        /// </summary>
        /// <param name="config">JSON string to write to the AppConfiguration column.</param>
        public void UpdateAppConfiguration(string config)
        {
            // Store the latest config
            // Then wake up the thread to update the config to the database
            _appConfigurationJsonStr = config;

            // Update the flag
            lock (_lockIsAppConfigurationNeedUpdate)
            {
                _isAppConfigurationNeedUpdate = true;
            }

            // Wake up the thread to process data
            _eventWaitData.Set();
        }

        /// <summary>
        /// Check if the AdcpConfiguration needs to be updated in the database.
        /// If it needs to be updated, the query will be run against the database.
        /// The flag will then be reset.
        /// 
        /// The flag is locked because of a multithreaded environment.
        /// </summary>
        /// <param name="cnn">Database connection.</param>
        /// <param name="selectedProject">Selected Project to get the lateset AdcpConfiguration.</param>
        private void CheckAppConfiguration(SQLiteConnection cnn, Project selectedProject)
        {
            // Check if the ADCP Commands needs to be updated
            if (_isAppConfigurationNeedUpdate)
            {
                // Send the query to the database
                UpdateAppConfiguration(cnn, selectedProject);

                // Lock the flag and reset the value
                lock (_lockIsAppConfigurationNeedUpdate)
                {
                    _isAppConfigurationNeedUpdate = false;          // Reset flag
                    _appConfigurationJsonStr = "";                  // Reset string
                }
            }
        }


        /// <summary>
        /// Update the row with the latest Application Configuration.
        /// </summary>
        /// <param name="cnn">Connection to the database.</param>
        /// <param name="project">Project to set configuration to.</param>
        private void UpdateAppConfiguration(SQLiteConnection cnn, Project project)
        {
            string jsonCmd = String.Format("{0} = '{1}'", DbCommon.COL_CMD_APP_CONFIGURATION, _appConfigurationJsonStr);        // Set the column name and JSON string
            string query = String.Format("UPDATE {0} SET {1} WHERE ID=1;", DbCommon.TBL_ENS_OPTIONS, jsonCmd);                  // Create query string

            using (DbCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = query;                // Set query string
                cmd.ExecuteNonQuery();                  // Execute the query
            }
        }

        #endregion

        #region Process Thread

        /// <summary>
        /// Decode the ADCP data received using a seperate
        /// thread.  This thread will also pass data to all
        /// _observers.
        /// </summary>
        private void ProcessDataThread()
        {
            while (_continue)
            {
                // Block until awoken when data is received
                _eventWaitData.WaitOne();

                // If wakeup was called to kill thread
                if (!_continue)
                {
                    return;
                }

                // Get the current number of ensembles
                // in the queue
                //int count = _datasetQueue.Count;
                //Stopwatch watch = new Stopwatch();
                //watch.Start();
                if (_selectedProject != null)
                {
                    // Used to determine if a new Subsystem Configuration was found
                    AdcpSubsystemConfig asConfig = null;

                    try
                    {
                        // Open a connection to the database
                        SQLiteConnection cnn = DbCommon.OpenProjectDB(_selectedProject);

                        // Start a transaction
                        using (DbTransaction dbTrans = cnn.BeginTransaction())
                        {
                            // Write all data to the database
                            //for (int x = 0; x < count; x++)
                            while (_datasetQueue.Count > 0)
                            {
                                // Remove dataset from the queue and write to the database
                                DataSet.Ensemble ensemble = (DataSet.Ensemble)_datasetQueue.Dequeue();
                                WriteEnsembleData(ensemble, cnn);

                                // Set the serial number if it has not been set already to the project
                                if (_selectedProject.SerialNumber.IsEmpty())
                                {
                                    _selectedProject.SerialNumber = ensemble.EnsembleData.SysSerialNumber;
                                }

                                // Add the ADCP Configuration to the project
                                // Check if the configuration has already been added to the project.
                                // If it has not been added to the project add it now.
                                // Later it will be written to the project.
                                // CheckSubsystemCompatibility() is used because of the change to the SubsystemCode vs SubsystemIndex in Firmware 2.13
                                if (ensemble.IsEnsembleAvail)
                                {
                                    SubsystemConfiguration ssConfig = ensemble.EnsembleData.SubsystemConfig;
                                    if (ssConfig != null)
                                    {
                                        Subsystem ss = ensemble.EnsembleData.GetSubSystem();
                                        CheckSubsystemCompatibility(ensemble, ref ss, ref ssConfig);
                                        if (!_selectedProject.Configuration.AdcpSubsystemConfigExist(ssConfig))
                                        {
                                            _selectedProject.Configuration.AddConfiguration(ss, out asConfig);
                                        }
                                    }
                                }
                            }

                            // Check if Commands and Options need to be updated
                            CheckCommandsAndOptions(cnn, _selectedProject);

                            // Check if Application Configurations nees to be updated
                            CheckAppConfiguration(cnn, _selectedProject);

                            // Commit the transaction
                            dbTrans.Commit();
                        }
                        // Close the connection to the database
                        cnn.Close();
                    }
                    catch (SQLiteException ex)
                    {
                        log.Error("Error adding Ensemble data to database.", ex);
                    }
                    catch (NullReferenceException ex)
                    {
                        log.Error("Selected Mission does not exist. ", ex);
                    }

                    //watch.Stop();
                    //long result = watch.ElapsedMilliseconds;
                    //Debug.WriteLine("Writer1 Result: {0}", result);

                    // If there was a new Configuration found
                    // Update the project file with the latest configuration
                    if (asConfig != null)
                    {
                        UpdateAdcpConfiguration(_selectedProject.Configuration);
                    }
                }
            }
        }

        /// <summary>
        /// If the queue is not filled or
        /// there is still data in the queue after
        /// all the data has been read, flush
        /// the queue of the remaining data.
        /// </summary>
        public void Flush()
        {
            // Wake up the thread to process data
            _eventWaitData.Set();
        }

        /// <summary>
        /// Before setting new selected project
        /// write remaining data to the database file.
        /// This will start the thread to process
        /// the remaining data in the thread.
        /// </summary>
        private void ResetDatabaseConnection()
        {
            // Write the remaining data in the queue
            Flush();
        }

        /// <summary>
        /// Shutdown this object.
        /// 
        /// Stop the thread.
        /// </summary>
        public void Dispose()
        {
            // Write the remaining data in the queue
            while (_datasetQueue.Count > 0 || _isAdcpConfigurationNeedUpdate)
            {
                Flush();
            }

            // Stop the thread
            _continue = false;

            // Wake up the thread to kill thread
            _eventWaitData.Set();
        }

        /// <summary>
        /// Firmware 2.12 and older:
        /// In Firmware version 2.12 and previous versions, the Firmware data in EnsembleDataSet set contained
        /// the SubsystemIndex.  Now it contains the SubsystemCode.  This will check for a SubsystemCode of 0.  0
        /// is not a possible SubsystemCode and is a sign of older firmware.  If this is older firmware, then change
        /// the SubsystemCode to the correct value based off the Index and the serial number.
        /// 
        /// 
        /// </summary>
        /// <param name="ensemble">Ensemble to the Subsystem and Serial number.</param>
        /// <param name="ss">Subsystem if the value needs to change.</param>
        /// <param name="ssConfig">SubsystemConfiguration if the value needs to change.</param>
        private void CheckSubsystemCompatibility(DataSet.Ensemble ensemble, ref Subsystem ss, ref SubsystemConfiguration ssConfig)
        {
            // In firmware version greater than 2.12, a code of 0 is not possible
            // It used to be that the code was the index from the serial number.
            // So now get the code from the serial number.
            // This can only work if the older system has 1 configuration.
            if (ss.Code == 0)
            {
                if (ensemble.IsEnsembleAvail)
                {
                    if (ensemble.EnsembleData.SysSerialNumber.SubsystemsString().Length > 0)
                    {
                        ss.Code = Convert.ToByte(ensemble.EnsembleData.SysSerialNumber.SubsystemsString()[0]);
                        ss.Index = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Write the dataset to the database.  This
        /// will check what data types exist in the 
        /// dataset and then write the data to the 
        /// database using a single transaction.
        /// </summary>
        /// <param name="dataset">Ensemble containing data to write.</param>
        /// <param name="cnn">Connection to the database.</param>
        private void WriteEnsembleData(DataSet.Ensemble dataset, SQLiteConnection cnn)
        {
            // Check if the project is set
            if (_selectedProject != null && dataset != null)
            {
                int ensId = WriteEnsembleDataToDatabase(dataset, cnn);       // Write data

                // Publish the Ensemble ID that was written to the database
                // This ID is also the last count of the number of ensembles in the database
                //PublishEnsembleWrite(ensId);
            }
        }

        #endregion

        #region Write Ensembles to Database

        /// <summary>
        /// Write the Ensemble dataset to the project database.
        /// </summary>
        /// <param name="ensemble">Dataset to write to the database.</param>
        /// <param name="cnn">Connection to the database.</param>
        /// <returns>Returns the ID for the row where data was added.</returns>
        private int WriteEnsembleDataToDatabase(DataSet.Ensemble ensemble, SQLiteConnection cnn)
        {
            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            int ensId = 0;
            //if (Validator.ValidateEnsembleDataSet(ensemble))
            //{
            try
            {
                using (DbCommand cmd = cnn.CreateCommand())
                {
                    // Create the statement
                    StringBuilder builder = new StringBuilder();
                    builder.Append("INSERT INTO tblEnsemble(");
                    builder.Append(string.Format("{0},", DbCommon.COL_ENS_ENS_NUM));
                    builder.Append(string.Format("{0},", DbCommon.COL_ENS_DATETIME));
                    builder.Append(string.Format("{0},", DbCommon.COL_ENSEMBLE_DS));
                    builder.Append(string.Format("{0},", DbCommon.COL_ANCILLARY_DS));
                    builder.Append(string.Format("{0},", DbCommon.COL_AMPLITUDE_DS));
                    builder.Append(string.Format("{0},", DbCommon.COL_CORRELATION_DS));
                    builder.Append(string.Format("{0},", DbCommon.COL_BEAMVELOCITY_DS));
                    builder.Append(string.Format("{0},", DbCommon.COL_EARTHVELOCITY_DS));
                    builder.Append(string.Format("{0},", DbCommon.COL_INSTRUMENTVELOCITY_DS));
                    builder.Append(string.Format("{0},", DbCommon.COL_BOTTOMTRACK_DS));
                    builder.Append(string.Format("{0},", DbCommon.COL_GOODBEAM_DS));
                    builder.Append(string.Format("{0},", DbCommon.COL_GOODEARTH_DS));
                    builder.Append(string.Format("{0},", DbCommon.COL_NMEA_DS));
                    builder.Append(string.Format("{0},", DbCommon.COL_EARTHWATERMASS_DS));
                    builder.Append(string.Format("{0}", DbCommon.COL_INSTRUMENTWATERMASS_DS));

                    builder.Append(") ");
                    builder.Append("VALUES(");
                    builder.Append("@ensNum, ");
                    builder.Append("@dateTime, ");
                    builder.Append("@ensembleDS, ");
                    builder.Append("@ancillaryDS, ");
                    builder.Append("@amplitudeDS, ");
                    builder.Append("@correlationDS, ");
                    builder.Append("@beamVelDS, ");
                    builder.Append("@earthVelDS, ");
                    builder.Append("@instrVelDS, ");
                    builder.Append("@bottomTrackDS, ");
                    builder.Append("@goodBeamDS, ");
                    builder.Append("@goodEarthDS, ");
                    builder.Append("@nmeaDS, ");
                    builder.Append("@earthWaterMassDS, ");
                    builder.Append("@instrumentWaterMassDS ");
                    builder.Append("); ");
                    builder.Append("SELECT last_insert_rowid();");

                    cmd.CommandText = builder.ToString();

                    // Ensemble data
                    if (ensemble.IsEnsembleAvail)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@ensNum", System.Data.DbType.Int32) { Value = ensemble.EnsembleData.EnsembleNumber });
                        cmd.Parameters.Add(new SQLiteParameter("@dateTime", System.Data.DbType.DateTime) { Value = ensemble.EnsembleData.EnsDateTime });
                        cmd.Parameters.Add(new SQLiteParameter("@ensembleDS", System.Data.DbType.String) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EnsembleData) });

                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@ensNum", System.Data.DbType.Int32) { Value = 1 });
                        cmd.Parameters.Add(new SQLiteParameter("@dateTime", System.Data.DbType.DateTime) { Value = DateTime.Now });
                        cmd.Parameters.Add(new SQLiteParameter("@ensembleDS", System.Data.DbType.String) { Value = DBNull.Value });
                    }

                    // Ancillary Data
                    if (ensemble.IsAncillaryAvail)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@ancillaryDS", System.Data.DbType.String) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.AncillaryData) });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@ancillaryDS", System.Data.DbType.String) { Value = DBNull.Value });
                    }

                    // Amplitude data
                    if (ensemble.IsAmplitudeAvail)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@amplitudeDS", System.Data.DbType.String) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.AmplitudeData) });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@amplitudeDS", System.Data.DbType.String) { Value = DBNull.Value });
                    }

                    // Correlation
                    if (ensemble.IsCorrelationAvail)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@correlationDS", System.Data.DbType.String) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.CorrelationData) });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@correlationDS", System.Data.DbType.String) { Value = DBNull.Value });
                    }

                    // Beam Velocity
                    if (ensemble.IsBeamVelocityAvail)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@beamVelDS", System.Data.DbType.String) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.BeamVelocityData) });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@beamVelDS", System.Data.DbType.String) { Value = DBNull.Value });
                    }

                    // Earth Velocity
                    if (ensemble.IsEarthVelocityAvail)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@earthVelDS", System.Data.DbType.String) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthVelocityData) });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@earthVelDS", System.Data.DbType.String) { Value = DBNull.Value });
                    }

                    // Instrument Velocity
                    if (ensemble.IsInstrumentVelocityAvail)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@instrVelDS", System.Data.DbType.String) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.InstrumentVelocityData) });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@instrVelDS", System.Data.DbType.String) { Value = DBNull.Value });
                    }

                    // Bottom Track
                    if (ensemble.IsBottomTrackAvail)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@bottomTrackDS", System.Data.DbType.String) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.BottomTrackData) });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@bottomTrackDS", System.Data.DbType.String) { Value = DBNull.Value });
                    }

                    // Good Beam
                    if (ensemble.IsGoodBeamAvail)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@goodBeamDS", System.Data.DbType.String) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.GoodBeamData) });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@goodBeamDS", System.Data.DbType.String) { Value = DBNull.Value });
                    }

                    // Good Earth
                    if (ensemble.IsGoodEarthAvail)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@goodEarthDS", System.Data.DbType.String) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.GoodEarthData) });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@goodEarthDS", System.Data.DbType.String) { Value = DBNull.Value });
                    }

                    // Nmea
                    if (ensemble.IsNmeaAvail)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@nmeaDS", System.Data.DbType.String) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.NmeaData) });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@nmeaDS", System.Data.DbType.String) { Value = DBNull.Value });
                    }

                    // Earth Water Mass Velocity
                    if (ensemble.IsEarthWaterMassAvail)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@earthWaterMassDS", System.Data.DbType.String) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthWaterMassData) });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@earthWaterMassDS", System.Data.DbType.String) { Value = DBNull.Value });
                    }

                    // Instrument Water Mass Velocity
                    if (ensemble.IsInstrumentWaterMassAvail)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@instrumentWaterMassDS", System.Data.DbType.String) { Value = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.InstrumentWaterMassData) });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@instrumentWaterMassDS", System.Data.DbType.String) { Value = DBNull.Value });
                    }

                    // Run the query and get the result for the last row
                    object cmdExecuteScalar = cmd.ExecuteScalar();
                    ensId = Convert.ToInt32(cmdExecuteScalar);
                }
            }
            catch (SQLiteException e)
            {
                log.Error("Error adding Ensemble data to database.", e);
                return ensId;
            }
            //}

            //watch.Stop();
            //long result = watch.ElapsedMilliseconds;
            //Debug.WriteLine("Wrtier 1 Result: {0}", result);
            return ensId;
        }

        #endregion

        #region Update Ensemble GPS

        /// <summary>
        /// Update the ensemble in the database with the latest GPS data.
        /// This will replace any NmeaDataset in the Ensemble stored in the
        /// database.  It will use the ensemble number to find the ensemble
        /// to update.
        /// </summary>
        /// <param name="nmea">Nmea Dataset.</param>
        /// <param name="ensNum">Ensemble number.</param>
        public void UpdateNmeaDataSet(DataSet.NmeaDataSet nmea, int ensNum)
        {
            // Open a connection to the database
            SQLiteConnection cnn = DbCommon.OpenProjectDB(_selectedProject);

            // Write the data
            UpdateEnsembleToDatabaseGps(nmea, ensNum, cnn);

            // Close the connection
            cnn.Close();
        }

        /// <summary>
        /// Write the NMEA dataset to the ensemble in the database.
        /// This will replace any previous ensemble GPS data with the same ensemble number.
        /// </summary>
        /// <param name="nmea">NMEA dataset.</param>
        /// <param name="ensNum">Ensemble number.</param>
        /// <param name="cnn">Database connection.</param>
        private void UpdateEnsembleToDatabaseGps(DataSet.NmeaDataSet nmea, int ensNum, SQLiteConnection cnn)
        {
            try
            {
                using (DbCommand cmd = cnn.CreateCommand())
                {
                    // Create the statement
                    cmd.CommandText = string.Format("UPDATE {0} SET {1}='{2}' WHERE {3}={4};",          // SQL Query
                                    DbCommon.TBL_ENS_ENSEMBLE,                                          // Ensemble Table
                                    DbCommon.COL_NMEA_DS,                                               // NMEA DataSet Column
                                    Newtonsoft.Json.JsonConvert.SerializeObject(nmea),                  // NMEA data as JSON
                                    DbCommon.COL_ENS_ENS_NUM,                                           // Ensemble Number Column
                                    ensNum);                                                            // Ensemble Number

                    // Run the command
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SQLiteException e)
            {
                log.Error("Error updating GPS Ensemble data to database.", e);
            }
        }

        #endregion

        #region Events

        #region Ensemble Write Event

        /// <summary>
        /// Event To subscribe to. 
        /// </summary>
        /// <param name="count">Number of ensembles in the database.</param>
        public delegate void EnsembleWriteEventHandler(long count);

        /// <summary>
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// writer.EnsembleWriteEvent += new writer.EnsembleWriteEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// writer.EnsembleWriteEvent -= (method to call)
        /// </summary>
        public event EnsembleWriteEventHandler EnsembleWriteEvent;

        /// <summary>
        /// Verify there is a subscriber before calling the
        /// subscribers with the new event.
        /// </summary>
        private void PublishEnsembleWrite(long count)
        {
            if (EnsembleWriteEvent != null)
            {
                EnsembleWriteEvent(count);
            }
        }

        #endregion

        #endregion
    }
}