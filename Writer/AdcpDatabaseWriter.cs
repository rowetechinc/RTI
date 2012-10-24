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
    public class AdcpDatabaseWriter
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
        }

        #region Update AdcpConfiguration

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

        /// <summary>
        /// Update the database with the latest AdcpCommands.
        /// This will set the AdcpConfiguration's AdcpCommand.
        /// </summary>
        /// <param name="commands">AdcpCommands to update.</param>
        public void UpdateAdcpCommands(AdcpCommands commands)
        {
            // Set the new Commands to the AdcpConfiguration
            SelectedProject.Configuration.Commands = commands;

            // Update the database.
            UpdateAdcpConfiguration(SelectedProject.Configuration);
        }

        /// <summary>
        /// Update the database with the latest DeploymentOptions.
        /// This will set the AdcpConfiguration's DeploymentOptions.
        /// </summary>
        /// <param name="options">DeploymentOptions to update.</param>
        public void UpdateDeploymentOptions(DeploymentOptions options)
        {
            // Set the new Commands to the AdcpConfiguration
            SelectedProject.Configuration.DeploymentOptions = options;

            // Update the database.
            UpdateAdcpConfiguration(SelectedProject.Configuration);
        }

        /// <summary>
        /// Find the AdcpSubsystemConfig in the dictionary.  Set the commands to the correct
        /// AdcpSubsystemConfig then write the AdcpConfiguration to the database.
        /// </summary>
        /// <param name="asConfig">AdcpSubsystemConfig to match and get the commands.</param>
        public void UpdateAdcpSubsystemConfigurationCommands(AdcpSubsystemConfig asConfig)
        {
            // Find the SubsystemConfiguration and update the commands
            foreach (AdcpSubsystemConfig asc in SelectedProject.Configuration.SubsystemConfigDict.Values)
            {
                if (asc==asConfig)
                {
                    asc.Commands = asConfig.Commands;
                }
            }

            // Update the database.
            UpdateAdcpConfiguration(SelectedProject.Configuration);
        }

        #endregion

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
                                if (_selectedProject.SerialNumber == SerialNumber.Empty)
                                {
                                    _selectedProject.SerialNumber = ensemble.EnsembleData.SysSerialNumber;
                                }

                                // Add the ADCP Configuration to the project
                                // Check if the configuration has already been added to the project.
                                // If it has not been added to the project add it now.
                                // Later it will be written to the project.
                                // CheckSubsystemCompatibility() is used because of the change to the SubsystemCode vs SubsystemIndex in Firmware 2.13
                                SubsystemConfiguration ssConfig = ensemble.EnsembleData.SubsystemConfig;
                                Subsystem ss = ensemble.EnsembleData.GetSubSystem();
                                CheckSubsystemCompatibility(ensemble, ref ss, ref ssConfig);
                                if (!_selectedProject.Configuration.AdcpSubsystemConfigExist(ss, ssConfig))
                                {
                                    _selectedProject.Configuration.AddConfiguration(ss, out asConfig);
                                }
                            }

                            // Check if Commands and Options need to be updated
                            CheckCommandsAndOptions(cnn, _selectedProject);

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
        public void Shutdown()
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
                // Check if ensemble data is available
                // Ensemble data must be available to write any other data
                // Everything keys off the ensemble number
                if (dataset.IsEnsembleAvail)
                {
                   int ensId = WriteEnsembleDataToDatabase(dataset, cnn);       // Ensemble data !!!THIS MUST BE WRITTEN FIRST FOR FOREIGN KEY

                    // Check if Beam velocity data is available
                    if (dataset.IsBeamVelocityAvail)
                    {
                        WriteBeamToDatabase(dataset, cnn, ensId);
                    }
                }
            }
        }

        #region Write Ensembles to Database

        /// <summary>
        /// Write the Ensemble dataset to the project database.
        /// </summary>
        /// <param name="dataset">Dataset to write to the database.</param>
        /// <param name="cnn">Connection to the database.</param>
        /// <returns>Returns the ID for the row where data was added.</returns>
        private int WriteEnsembleDataToDatabase(DataSet.Ensemble dataset, SQLiteConnection cnn)
        {
            int ensId = 0;
            if (Validator.ValidateEnsembleDataSet(dataset))
            {
                try
                {
                    using (DbCommand cmd = cnn.CreateCommand())
                    {
                        // Create the statement
                        StringBuilder builder = new StringBuilder();
                        builder.Append("INSERT INTO tblEnsemble("); 
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_ENS_NUM));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_NUM_BIN));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_NUM_BEAM));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_DES_PING_COUNT));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_ACT_PING_COUNT));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_STATUS));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_YEAR));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_MONTH));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_DAY));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_HOUR));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_MINUTE));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_SECOND));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_HUN_SECOND));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_DATETIME));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_FIRST_BIN_RANGE));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_BIN_SIZE));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_FIRST_PING_TIME));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_LAST_PING_TIME));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_HEADING));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_PITCH));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_ROLL));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_TEMP_WATER));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_TEMP_SYS));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_SALINITY));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_PRESSURE));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_XDCR_DEPTH));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_SOS));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_DB_TIME));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_IS_BEAM_VEL_AVAIL));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_IS_INSTR_VEL_AVAIL));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_IS_EARTH_VEL_AVAIL));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_IS_AMP_AVAIL));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_IS_CORR_AVAIL));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_IS_GB_AVAIL));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_IS_GE_AVAIL));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_IS_ANCIL_AVAIL));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_IS_BT_AVAIL));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_IS_NMEA_AVAIL));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_NMEA_DATA));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_SYS_SERIAL));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_FIRMWARE_MAJOR));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_FIRMWARE_MINOR));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_FIRMWARE_REVISION));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_SUBSYSTEM_CODE));
                        builder.Append(string.Format("{0},", DbCommon.COL_ENS_SUBSYS_CONFIG));

                        builder.Append(string.Format("{0},", DbCommon.COL_BT_FIRST_PING_TIME));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_LAST_PING_TIME));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_HEADING));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_PITCH));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_ROLL));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_TEMP_WATER));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_TEMP_SYS));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_SALINITY));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_PRESSURE));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_SOS));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_STATUS));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_ACTUAL_PING_COUNT));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_RANGE_B0));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_RANGE_B1));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_RANGE_B2));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_RANGE_B3));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_SNR_B0));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_SNR_B1));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_SNR_B2));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_SNR_B3));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_AMP_B0));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_AMP_B1));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_AMP_B2));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_AMP_B3));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_CORR_B0));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_CORR_B1));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_CORR_B2));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_CORR_B3));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_BEAM_VEL_B0));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_BEAM_VEL_B1));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_BEAM_VEL_B2));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_BEAM_VEL_B3));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_BEAM_GOOD_B0));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_BEAM_GOOD_B1));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_BEAM_GOOD_B2));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_BEAM_GOOD_B3));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_INSTR_VEL_B0));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_INSTR_VEL_B1));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_INSTR_VEL_B2));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_INSTR_VEL_B3));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_INSTR_GOOD_B0));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_INSTR_GOOD_B1));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_INSTR_GOOD_B2));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_INSTR_GOOD_B3));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_EARTH_VEL_B0));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_EARTH_VEL_B1));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_EARTH_VEL_B2));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_EARTH_VEL_B3));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_EARTH_GOOD_B0));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_EARTH_GOOD_B1));
                        builder.Append(string.Format("{0},", DbCommon.COL_BT_EARTH_GOOD_B2));
                        builder.Append(string.Format("{0}", DbCommon.COL_BT_EARTH_GOOD_B3));
                        builder.Append(") ");
                        builder.Append("VALUES(");
                        builder.Append("@ensembleNum, @numBins, @numBeams, @desiredPingCount, @actualPingCount, @status, ");
                        builder.Append("@year, @month, @day, @hour, @minute, @second, @hundSec, @dateTime, ");
                        builder.Append("@firstBinRange, @binSize, @profileFirstPingTime, @profileLastPingTime, @heading, @pitch, @roll, ");
                        builder.Append("@waterTemp, @sysTemp, @salinity, @pressure, @Depth, @sos, @dbTime, ");
                        builder.Append("@isBeamVelAvail, @isInstrVelAvail, @isEarthVelAvail, @isAmpAvail, @isCorrAvail, @isGoodBeamAvail, @isGoodEarthAvail, @isAncillaryAvail, @isBottomTrackAvail, @isNmeaAvail, ");
                        builder.Append("@nmeaData, @sysSerialNum, @firmwareMajor, @firmwareMinor, @firmwareRevision, @subsystem, @subConfig, ");
                        
                        builder.Append("@btFirstPingTime, @btLastPingTime, @btHeading, @btPitch, @btRoll, @btWaterTemp, @btSysTemp, ");
                        builder.Append("@btSalinity, @btPressure, @btSos, @btStatus, @btActualPingCount, ");
                        builder.Append("@btRangeB0, @btRangeB1, @btRangeB2, @btRangeB3, ");
                        builder.Append("@btSnrB0, @btSnrB1, @btSnrB2, @btSnrB3, ");
                        builder.Append("@btAmpB0, @btAmpB1, @btAmpB2, @btAmpB3, ");
                        builder.Append("@btCorrB0, @btCorrB1, @btCorrB2, @btCorrB3, ");
                        builder.Append("@btBeamVelB0, @btBeamVelB1, @btBeamVelB2, @btBeamVelB3, ");
                        builder.Append("@btBeamGoodB0, @btBeamGoodB1, @btBeamGoodB2, @btBeamGoodB3, ");
                        builder.Append("@btInstrVelB0, @btInstrVelB1, @btInstrVelB2, @btInstrVelB3, ");
                        builder.Append("@btInstrGoodB0, @btInstrGoodB1, @btInstrGoodB2, @btInstrGoodB3, ");
                        builder.Append("@btEarthVelB0, @btEarthVelB1, @btEarthVelB2, @btEarthVelB3, ");
                        builder.Append("@btEarthGoodB0, @btEarthGoodB1, @btEarthGoodB2, @btEarthGoodB3");
                        builder.Append("); ");
                        builder.Append("SELECT last_insert_rowid();");

                        cmd.CommandText = builder.ToString();

                        //// Add all the parameters
                        cmd.Parameters.Add(new SQLiteParameter("@ensembleNum", System.Data.DbType.Int32) { Value = dataset.EnsembleData.EnsembleNumber });
                        cmd.Parameters.Add(new SQLiteParameter("@numBins", System.Data.DbType.Int32) { Value = dataset.EnsembleData.NumBins });
                        cmd.Parameters.Add(new SQLiteParameter("@numBeams", System.Data.DbType.Int32) { Value = dataset.EnsembleData.NumBeams });
                        cmd.Parameters.Add(new SQLiteParameter("@desiredPingCount", System.Data.DbType.Int32) { Value = dataset.EnsembleData.DesiredPingCount });
                        cmd.Parameters.Add(new SQLiteParameter("@actualPingCount", System.Data.DbType.Int32) { Value = dataset.EnsembleData.ActualPingCount });
                        cmd.Parameters.Add(new SQLiteParameter("@status", System.Data.DbType.Int32) { Value = dataset.EnsembleData.Status.Value });
                        cmd.Parameters.Add(new SQLiteParameter("@year", System.Data.DbType.Int32) { Value = dataset.EnsembleData.Year });
                        cmd.Parameters.Add(new SQLiteParameter("@month", System.Data.DbType.Int32) { Value = dataset.EnsembleData.Month });
                        cmd.Parameters.Add(new SQLiteParameter("@day", System.Data.DbType.Int32) { Value = dataset.EnsembleData.Day });
                        cmd.Parameters.Add(new SQLiteParameter("@hour", System.Data.DbType.Int32) { Value = dataset.EnsembleData.Hour });
                        cmd.Parameters.Add(new SQLiteParameter("@minute", System.Data.DbType.Int32) { Value = dataset.EnsembleData.Minute });
                        cmd.Parameters.Add(new SQLiteParameter("@second", System.Data.DbType.Int32) { Value = dataset.EnsembleData.Second });
                        cmd.Parameters.Add(new SQLiteParameter("@hundSec", System.Data.DbType.Int32) { Value = dataset.EnsembleData.HSec });
                        cmd.Parameters.Add(new SQLiteParameter("@dateTime", System.Data.DbType.DateTime) { Value = dataset.EnsembleData.EnsDateTime });
                        cmd.Parameters.Add(new SQLiteParameter("@firstBinRange", System.Data.DbType.Single) { Value = dataset.AncillaryData.FirstBinRange });
                        cmd.Parameters.Add(new SQLiteParameter("@binSize", System.Data.DbType.Single) { Value = dataset.AncillaryData.BinSize });
                        cmd.Parameters.Add(new SQLiteParameter("@profileFirstPingTime", System.Data.DbType.Single) { Value = dataset.AncillaryData.FirstPingTime });
                        cmd.Parameters.Add(new SQLiteParameter("@profileLastPingTime", System.Data.DbType.Single) { Value = dataset.AncillaryData.LastPingTime });
                        cmd.Parameters.Add(new SQLiteParameter("@heading", System.Data.DbType.Single) { Value = dataset.AncillaryData.Heading });
                        cmd.Parameters.Add(new SQLiteParameter("@pitch", System.Data.DbType.Single) { Value = dataset.AncillaryData.Pitch });
                        cmd.Parameters.Add(new SQLiteParameter("@roll", System.Data.DbType.Single) { Value = dataset.AncillaryData.Roll });
                        cmd.Parameters.Add(new SQLiteParameter("@waterTemp", System.Data.DbType.Single) { Value = dataset.AncillaryData.WaterTemp });
                        cmd.Parameters.Add(new SQLiteParameter("@sysTemp", System.Data.DbType.Single) { Value = dataset.AncillaryData.SystemTemp });
                        cmd.Parameters.Add(new SQLiteParameter("@salinity", System.Data.DbType.Single) { Value = dataset.AncillaryData.Salinity });
                        cmd.Parameters.Add(new SQLiteParameter("@pressure", System.Data.DbType.Single) { Value = dataset.AncillaryData.Pressure });
                        cmd.Parameters.Add(new SQLiteParameter("@Depth", System.Data.DbType.Single) { Value = dataset.AncillaryData.TransducerDepth });
                        cmd.Parameters.Add(new SQLiteParameter("@sos", System.Data.DbType.Single) { Value = dataset.AncillaryData.SpeedOfSound });
                        cmd.Parameters.Add(new SQLiteParameter("@dbTime", System.Data.DbType.DateTime) { Value = DateTime.Now });
                        cmd.Parameters.Add(new SQLiteParameter("@isBeamVelAvail", System.Data.DbType.Boolean) { Value = DbCommon.ConvertBool(dataset.IsBeamVelocityAvail) });
                        cmd.Parameters.Add(new SQLiteParameter("@isInstrVelAvail", System.Data.DbType.Boolean) { Value = DbCommon.ConvertBool(dataset.IsInstrVelocityAvail) });
                        cmd.Parameters.Add(new SQLiteParameter("@isEarthVelAvail", System.Data.DbType.Boolean) { Value = DbCommon.ConvertBool(dataset.IsEarthVelocityAvail) });
                        cmd.Parameters.Add(new SQLiteParameter("@isAmpAvail", System.Data.DbType.Boolean) { Value = DbCommon.ConvertBool(dataset.IsAmplitudeAvail) });
                        cmd.Parameters.Add(new SQLiteParameter("@isCorrAvail", System.Data.DbType.Boolean) { Value = DbCommon.ConvertBool(dataset.IsCorrelationAvail) });
                        cmd.Parameters.Add(new SQLiteParameter("@isGoodBeamAvail", System.Data.DbType.Boolean) { Value = DbCommon.ConvertBool(dataset.IsGoodBeamAvail) });
                        cmd.Parameters.Add(new SQLiteParameter("@isGoodEarthAvail", System.Data.DbType.Boolean) { Value = DbCommon.ConvertBool(dataset.IsGoodEarthAvail) });
                        cmd.Parameters.Add(new SQLiteParameter("@isAncillaryAvail", System.Data.DbType.Boolean) { Value = DbCommon.ConvertBool(dataset.IsAncillaryAvail) });
                        cmd.Parameters.Add(new SQLiteParameter("@isBottomTrackAvail", System.Data.DbType.Boolean) { Value = DbCommon.ConvertBool(dataset.IsBottomTrackAvail) });
                        cmd.Parameters.Add(new SQLiteParameter("@isNmeaAvail", System.Data.DbType.Boolean) { Value = DbCommon.ConvertBool(dataset.IsNmeaAvail) });
                        // NMEA data below
                        cmd.Parameters.Add(new SQLiteParameter("@sysSerialNum", System.Data.DbType.String) { Value = dataset.EnsembleData.SysSerialNumber.ToString() });
                        cmd.Parameters.Add(new SQLiteParameter("@firmwareMajor", System.Data.DbType.UInt16) { Value = dataset.EnsembleData.SysFirmware.FirmwareMajor });
                        cmd.Parameters.Add(new SQLiteParameter("@firmwareMinor", System.Data.DbType.UInt16) { Value = dataset.EnsembleData.SysFirmware.FirmwareMinor });
                        cmd.Parameters.Add(new SQLiteParameter("@firmwareRevision", System.Data.DbType.UInt16) { Value = dataset.EnsembleData.SysFirmware.FirmwareRevision });
                        cmd.Parameters.Add(new SQLiteParameter("@subsystem", System.Data.DbType.UInt16) { Value = dataset.EnsembleData.SysFirmware.SubsystemCode });
                        cmd.Parameters.Add(new SQLiteParameter("@subConfig", System.Data.DbType.UInt16) { Value = dataset.EnsembleData.SubsystemConfig.CommandSetup });

                        // Bottom Track parameters
                        if (dataset.IsBottomTrackAvail)
                        {
                            cmd.Parameters.Add(new SQLiteParameter("@btFirstPingTime", System.Data.DbType.Single) { Value = dataset.BottomTrackData.FirstPingTime });
                            cmd.Parameters.Add(new SQLiteParameter("@btLastPingTime", System.Data.DbType.Single) { Value = dataset.BottomTrackData.LastPingTime });
                            cmd.Parameters.Add(new SQLiteParameter("@btHeading", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Heading });
                            cmd.Parameters.Add(new SQLiteParameter("@btPitch", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Pitch });
                            cmd.Parameters.Add(new SQLiteParameter("@btRoll", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Roll });
                            cmd.Parameters.Add(new SQLiteParameter("@btWaterTemp", System.Data.DbType.Single) { Value = dataset.BottomTrackData.WaterTemp });
                            cmd.Parameters.Add(new SQLiteParameter("@btSysTemp", System.Data.DbType.Single) { Value = dataset.BottomTrackData.SystemTemp });
                            cmd.Parameters.Add(new SQLiteParameter("@btSalinity", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Salinity });
                            cmd.Parameters.Add(new SQLiteParameter("@btPressure", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Pressure });
                            cmd.Parameters.Add(new SQLiteParameter("@btSos", System.Data.DbType.Single) { Value = dataset.BottomTrackData.SpeedOfSound });
                            cmd.Parameters.Add(new SQLiteParameter("@btStatus", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Status.Value });
                            cmd.Parameters.Add(new SQLiteParameter("@btActualPingCount", System.Data.DbType.Single) { Value = dataset.BottomTrackData.ActualPingCount });
                            cmd.Parameters.Add(new SQLiteParameter("@btRangeB0", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Range[0] });
                            cmd.Parameters.Add(new SQLiteParameter("@btRangeB1", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Range[1] });
                            cmd.Parameters.Add(new SQLiteParameter("@btRangeB2", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Range[2] });
                            cmd.Parameters.Add(new SQLiteParameter("@btRangeB3", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Range[3] });
                            cmd.Parameters.Add(new SQLiteParameter("@btSnrB0", System.Data.DbType.Single) { Value = dataset.BottomTrackData.SNR[0] });
                            cmd.Parameters.Add(new SQLiteParameter("@btSnrB1", System.Data.DbType.Single) { Value = dataset.BottomTrackData.SNR[1] });
                            cmd.Parameters.Add(new SQLiteParameter("@btSnrB2", System.Data.DbType.Single) { Value = dataset.BottomTrackData.SNR[2] });
                            cmd.Parameters.Add(new SQLiteParameter("@btSnrB3", System.Data.DbType.Single) { Value = dataset.BottomTrackData.SNR[3] });
                            cmd.Parameters.Add(new SQLiteParameter("@btAmpB0", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Amplitude[0] });
                            cmd.Parameters.Add(new SQLiteParameter("@btAmpB1", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Amplitude[1] });
                            cmd.Parameters.Add(new SQLiteParameter("@btAmpB2", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Amplitude[2] });
                            cmd.Parameters.Add(new SQLiteParameter("@btAmpB3", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Amplitude[3] });
                            cmd.Parameters.Add(new SQLiteParameter("@btCorrB0", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Correlation[0] });
                            cmd.Parameters.Add(new SQLiteParameter("@btCorrB1", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Correlation[1] });
                            cmd.Parameters.Add(new SQLiteParameter("@btCorrB2", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Correlation[2] });
                            cmd.Parameters.Add(new SQLiteParameter("@btCorrB3", System.Data.DbType.Single) { Value = dataset.BottomTrackData.Correlation[3] });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamVelB0", System.Data.DbType.Single) { Value = dataset.BottomTrackData.BeamVelocity[0] });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamVelB1", System.Data.DbType.Single) { Value = dataset.BottomTrackData.BeamVelocity[1] });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamVelB2", System.Data.DbType.Single) { Value = dataset.BottomTrackData.BeamVelocity[2] });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamVelB3", System.Data.DbType.Single) { Value = dataset.BottomTrackData.BeamVelocity[3] });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamGoodB0", System.Data.DbType.Single) { Value = dataset.BottomTrackData.BeamGood[0] });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamGoodB1", System.Data.DbType.Single) { Value = dataset.BottomTrackData.BeamGood[1] });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamGoodB2", System.Data.DbType.Single) { Value = dataset.BottomTrackData.BeamGood[2] });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamGoodB3", System.Data.DbType.Single) { Value = dataset.BottomTrackData.BeamGood[3] });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrVelB0", System.Data.DbType.Single) { Value = dataset.BottomTrackData.InstrumentVelocity[0] });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrVelB1", System.Data.DbType.Single) { Value = dataset.BottomTrackData.InstrumentVelocity[1] });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrVelB2", System.Data.DbType.Single) { Value = dataset.BottomTrackData.InstrumentVelocity[2] });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrVelB3", System.Data.DbType.Single) { Value = dataset.BottomTrackData.InstrumentVelocity[3] });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrGoodB0", System.Data.DbType.Single) { Value = dataset.BottomTrackData.InstrumentGood[0] });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrGoodB1", System.Data.DbType.Single) { Value = dataset.BottomTrackData.InstrumentGood[1] });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrGoodB2", System.Data.DbType.Single) { Value = dataset.BottomTrackData.InstrumentGood[2] });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrGoodB3", System.Data.DbType.Single) { Value = dataset.BottomTrackData.InstrumentGood[3] });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthVelB0", System.Data.DbType.Single) { Value = dataset.BottomTrackData.EarthVelocity[0] });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthVelB1", System.Data.DbType.Single) { Value = dataset.BottomTrackData.EarthVelocity[1] });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthVelB2", System.Data.DbType.Single) { Value = dataset.BottomTrackData.EarthVelocity[2] });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthVelB3", System.Data.DbType.Single) { Value = dataset.BottomTrackData.EarthVelocity[3] });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthGoodB0", System.Data.DbType.Single) { Value = dataset.BottomTrackData.EarthGood[0] });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthGoodB1", System.Data.DbType.Single) { Value = dataset.BottomTrackData.EarthGood[1] });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthGoodB2", System.Data.DbType.Single) { Value = dataset.BottomTrackData.EarthGood[2] });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthGoodB3", System.Data.DbType.Single) { Value = dataset.BottomTrackData.EarthGood[3] });
                        }
                        else
                        {
                            cmd.Parameters.Add(new SQLiteParameter("@btFirstPingTime", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btLastPingTime", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btHeading", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btPitch", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btRoll", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btWaterTemp", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btSysTemp", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btSalinity", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btPressure", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btSos", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btStatus", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btActualPingCount", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btRangeB0", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btRangeB1", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btRangeB2", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btRangeB3", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btSnrB0", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btSnrB1", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btSnrB2", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btSnrB3", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btAmpB0", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btAmpB1", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btAmpB2", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btAmpB3", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btCorrB0", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btCorrB1", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btCorrB2", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btCorrB3", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamVelB0", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamVelB1", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamVelB2", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamVelB3", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamGoodB0", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamGoodB1", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamGoodB2", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btBeamGoodB3", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrVelB0", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrVelB1", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrVelB2", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrVelB3", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrGoodB0", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrGoodB1", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrGoodB2", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btInstrGoodB3", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthVelB0", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthVelB1", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthVelB2", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthVelB3", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthGoodB0", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthGoodB1", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthGoodB2", System.Data.DbType.Single) { Value = 0 });
                            cmd.Parameters.Add(new SQLiteParameter("@btEarthGoodB3", System.Data.DbType.Single) { Value = 0 });
                        }

                        if (dataset.IsNmeaAvail)
                        {
                            cmd.Parameters.Add(new SQLiteParameter("@nmeaData", System.Data.DbType.String) { Value = dataset.NmeaData.ToString() });
                        }
                        else
                        {
                            cmd.Parameters.Add(new SQLiteParameter("@nmeaData", System.Data.DbType.String) { Value = "" });
                        }

                        //cmd.ExecuteNonQuery();
                        ensId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                catch (SQLiteException e)
                {
                    log.Error("Error adding Ensemble data to database.", e);
                    return ensId;
                }
            }

            return ensId;
        }

        /// <summary>
        /// Write the Beam data to the project database.
        /// </summary>
        /// <param name="dataset">Dataset to write to the database.</param>
        /// <param name="cnn">Connection to the database.</param>
        /// <param name="ensembleRowId">Row ID withing the ensemble.</param>
        private void WriteBeamToDatabase1(DataSet.Ensemble dataset, SQLiteConnection cnn, int ensembleRowId)
        {
            try
            {
                for (int bin = 0; bin < dataset.EnsembleData.NumBins; bin++)
                {
                    for (int beam = 0; beam < dataset.EnsembleData.NumBeams; beam++)
                    {
                        using (DbCommand cmd = cnn.CreateCommand())
                        {
                            // Create the statement
                            StringBuilder builder = new StringBuilder();
                            builder.Append("INSERT INTO tblBeam(");
                            builder.Append(string.Format("{0},", DbCommon.COL_BV_ENS_ID));
                            builder.Append(string.Format("{0},", DbCommon.COL_BV_BIN_NUM));
                            builder.Append(string.Format("{0},", DbCommon.COL_BV_BEAM_NUM));
                            builder.Append(string.Format("{0},", DbCommon.COL_BV_VEL_BEAM));
                            builder.Append(string.Format("{0},", DbCommon.COL_BV_VEL_EARTH));
                            builder.Append(string.Format("{0},", DbCommon.COL_BV_VEL_INSTR));
                            builder.Append(string.Format("{0},", DbCommon.COL_BV_AMP));
                            builder.Append(string.Format("{0},", DbCommon.COL_BV_CORR));
                            builder.Append(string.Format("{0},", DbCommon.COL_BV_GOOD_BEAM));
                            builder.Append(string.Format("{0},", DbCommon.COL_BV_GOOD_EARTH));
                            builder.Append(string.Format("{0}", DbCommon.COL_BV_ORIENT));
                            builder.Append(") ");
                            builder.Append("VALUES(");
                            builder.Append("@ensembleId, @binNum, @beamNum,");
                            builder.Append("@beamVel, @earthVel, @instrVel,");
                            builder.Append("@amp, @corr, @goodBeam, @goodEarth, @orient");
                            builder.Append(");");
                            cmd.CommandText = builder.ToString();

                            // Add all the parameters
                            cmd.Parameters.Add(new SQLiteParameter("@ensembleId", System.Data.DbType.UInt32) { Value = ensembleRowId });
                            cmd.Parameters.Add(new SQLiteParameter("@binNum", System.Data.DbType.UInt32) { Value = bin });
                            cmd.Parameters.Add(new SQLiteParameter("@beamNum", System.Data.DbType.UInt16) { Value = beam });
                            cmd.Parameters.Add(new SQLiteParameter("@beamVel", System.Data.DbType.Single) { Value = dataset.BeamVelocityData.BeamVelocityData[bin, beam] });
                            cmd.Parameters.Add(new SQLiteParameter("@earthVel", System.Data.DbType.Single) { Value = dataset.EarthVelocityData.EarthVelocityData[bin, beam] });
                            cmd.Parameters.Add(new SQLiteParameter("@instrVel", System.Data.DbType.Single) { Value = dataset.InstrVelocityData.InstrumentVelocityData[bin, beam] });
                            cmd.Parameters.Add(new SQLiteParameter("@amp", System.Data.DbType.Single) { Value = dataset.AmplitudeData.AmplitudeData[bin, beam] });
                            cmd.Parameters.Add(new SQLiteParameter("@corr", System.Data.DbType.Single) { Value = dataset.CorrelationData.CorrelationData[bin, beam] });
                            cmd.Parameters.Add(new SQLiteParameter("@goodBeam", System.Data.DbType.UInt16) { Value = dataset.GoodBeamData.GoodBeamData[bin, beam] });
                            cmd.Parameters.Add(new SQLiteParameter("@goodEarth", System.Data.DbType.UInt16) { Value = dataset.GoodEarthData.GoodEarthData[bin, beam] });
                            cmd.Parameters.Add(new SQLiteParameter("@orient", System.Data.DbType.UInt16) { Value = dataset.BeamVelocityData.Orientation });

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SQLiteException e)
            {
                log.Error("Error adding Beam BeamVelocity data to database.", e);
            }
        }

        /// <summary>
        /// Write the Beam data to the project database.
        /// </summary>
        /// <param name="dataset">Dataset to write to the database.</param>
        /// <param name="cnn">Connection to the database.</param>
        /// <param name="ensembleRowId">Row ID withing the ensemble.</param>
        private void WriteBeamToDatabase(DataSet.Ensemble dataset, SQLiteConnection cnn, int ensembleRowId)
        {
            try
            {
                using (DbTransaction dbTrans = cnn.BeginTransaction())
                {
                    for (int bin = 0; bin < dataset.EnsembleData.NumBins; bin++)
                    {
                        for (int beam = 0; beam < dataset.EnsembleData.NumBeams; beam++)
                        {
                            using (DbCommand cmd = cnn.CreateCommand())
                            {
                                // Create the statement
                                // Create the statement
                                StringBuilder builder = new StringBuilder();
                                builder.Append("INSERT INTO tblBeam(");
                                builder.Append(string.Format("{0},", DbCommon.COL_BV_ENS_ID));
                                builder.Append(string.Format("{0},", DbCommon.COL_BV_BIN_NUM));
                                builder.Append(string.Format("{0},", DbCommon.COL_BV_BEAM_NUM));
                                builder.Append(string.Format("{0},", DbCommon.COL_BV_VEL_BEAM));
                                builder.Append(string.Format("{0},", DbCommon.COL_BV_VEL_EARTH));
                                builder.Append(string.Format("{0},", DbCommon.COL_BV_VEL_INSTR));
                                builder.Append(string.Format("{0},", DbCommon.COL_BV_AMP));
                                builder.Append(string.Format("{0},", DbCommon.COL_BV_CORR));
                                builder.Append(string.Format("{0},", DbCommon.COL_BV_GOOD_BEAM));
                                builder.Append(string.Format("{0},", DbCommon.COL_BV_GOOD_EARTH));
                                builder.Append(string.Format("{0}", DbCommon.COL_BV_ORIENT));
                                builder.Append(") ");
                                builder.Append("VALUES(");
                                builder.Append("@ensembleId, @binNum, @beamNum,");
                                builder.Append("@beamVel, @earthVel, @instrVel,");
                                builder.Append("@amp, @corr, @goodBeam, @goodEarth, @orient");
                                builder.Append(");");
                                cmd.CommandText = builder.ToString();

                                // Add all the parameters
                                cmd.Parameters.Add(new SQLiteParameter("@ensembleId", System.Data.DbType.UInt32) { Value = ensembleRowId });
                                cmd.Parameters.Add(new SQLiteParameter("@binNum", System.Data.DbType.UInt32) { Value = bin });
                                cmd.Parameters.Add(new SQLiteParameter("@beamNum", System.Data.DbType.UInt16) { Value = beam });
                                cmd.Parameters.Add(new SQLiteParameter("@beamVel", System.Data.DbType.Single) { Value = dataset.BeamVelocityData.BeamVelocityData[bin, beam] });
                                cmd.Parameters.Add(new SQLiteParameter("@earthVel", System.Data.DbType.Single) { Value = dataset.EarthVelocityData.EarthVelocityData[bin, beam] });
                                cmd.Parameters.Add(new SQLiteParameter("@instrVel", System.Data.DbType.Single) { Value = dataset.InstrVelocityData.InstrumentVelocityData[bin, beam] });
                                cmd.Parameters.Add(new SQLiteParameter("@amp", System.Data.DbType.Single) { Value = dataset.AmplitudeData.AmplitudeData[bin, beam] });
                                cmd.Parameters.Add(new SQLiteParameter("@corr", System.Data.DbType.Single) { Value = dataset.CorrelationData.CorrelationData[bin, beam] });
                                cmd.Parameters.Add(new SQLiteParameter("@goodBeam", System.Data.DbType.UInt16) { Value = dataset.GoodBeamData.GoodBeamData[bin, beam] });
                                cmd.Parameters.Add(new SQLiteParameter("@goodEarth", System.Data.DbType.UInt16) { Value = dataset.GoodEarthData.GoodEarthData[bin, beam] });
                                cmd.Parameters.Add(new SQLiteParameter("@orient", System.Data.DbType.UInt16) { Value = dataset.BeamVelocityData.Orientation });

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    dbTrans.Commit();
                }
            }
            catch (SQLiteException e)
            {
                log.Error("Error adding Beam BeamVelocity data to database.", e);
            }
        }

        #endregion

        #region Set AdcpConfiguration To Database

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
            string query = String.Format("UPDATE {0} SET {1} WHERE ID=1;", DbCommon.TBL_ENS_CMDS, jsonCmd);                 // Create query string

            using (DbCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = query;                // Set query string
                cmd.ExecuteNonQuery();                  // Execute the query
            }
        }

        #endregion
    }
}