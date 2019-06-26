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
 * 09/01/2011      RC                     Initial coding
 * 10/18/2011      RC                     Fixed bug. If duplicate ensembles had different Bin sizes.
 * 10/19/2011      RC                     Removed IsLive and added PausePlayback() and ResumePlayback().
 *                                         Fixed bug if app stopped and still playing back, stop the thread to get proper shutdown.
 *                                         Removed threading and made only a codec.  Threading in PlaybackProject.cs.
 *                                         Renamed to AdcpDatabaseCodec.cs
 * 10/20/2011      RC                     Added QueryForEnsembleDict and QueryForDataSet(UniqueId)
 * 10/24/2011      RC                     Got datarow and passed to all.
 * 11/07/2011      RC                     Check if project is null when getting number of ensembles.
 * 12/07/2011      RC          1.08       Remove Creating BinList in AddBeamData().  
 * 12/09/2011      RC          1.09       Added NmeaData to Ensemble table.  Now NMEA data is past to constructor.
 * 12/28/2011      RC          1.10       Remove passing paramater as Ref.
 *                                         Use Ensemble's row id instead of number for foreign key for tblBottomTrack and tblBeam.
 * 01/13/2012      RC          1.12       Merged Ensemble table and Bottom Track table in database.
 * 01/24/2012      RC          1.14       Put try/catch in ParseDataTables() if error parsing.
 * 02/29/2012      RC          2.04       Return 0 in GetNumberOfEnsembles() if no project is given.
 * 05/23/2012      RC          2.11       Added QueryForDataSet() that returns a cache of data.
 *                                         Made GetNumberOfEnsembles() a static method.
 * 03/06/2013      RC          2.18       Added logger and changed the format of the database.
 * 03/08/2013      RC          2.18       For each method call, created a new one with the SQLiteConnection to prevent opening and closing connection for each call.
 *                                         Added GetProjectVersion() to get the project version.
 * 06/28/2013      RC          2.19       Replaced Shutdown() with IDisposable.
 * 01/28/2014      RC          2.21.3     Read in ProfileEngineering, BottomTrackEngineering and SystemSetup datasets.
 * 01/30/2014      RC          2.21.3     Read in GPS1, GPS2, NMEA1 and NMEA2 data.
 * 01/31/2014      RC          2.21.3     Check if the column exist when parsing the data in ParseDataTables().
 * 10/31/2014      RC          3.0.2      Added Range Tracking DataSet.
 * 06/26/2019      RC          3.4.12     Added ShipVelocity and ShipWaterMass to ParseDataTables().
 * 
 */


using System.Diagnostics;
using System.Data;
using System;
using System.Collections.Generic;
using log4net;
using System.Data.SQLite;

namespace RTI
{
    /// <summary>
    /// This class will read and write to an
    /// ADCP Database file.
    /// </summary>
    public class AdcpDatabaseCodec
    {
        #region Variables

        /// <summary>
        /// Logger for logging error messages.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        /// <summary>
        /// Constructor
        /// 
        /// </summary>
        public AdcpDatabaseCodec()
        {

        }

        #region Utilities

        /// <summary>
        /// Query the tblEnsemble for the number of rows it contains.
        /// Return the value.
        /// </summary>
        /// <param name="project">Project to check.</param>
        /// <returns>Number of rows in the table.</returns>
        public static int GetNumberOfEnsembles(Project project)
        {
            if (project != null)
            {
                string query = String.Format("SELECT COUNT(*) FROM {0};", DbCommon.TBL_ENS_ENSEMBLE);
                return DbCommon.RunQueryOnProjectDb(project, query);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Query the tblOptions for the revision of the
        /// project file.
        /// </summary>
        /// <param name="project">Project to check.</param>
        /// <returns>Number of rows in the table.</returns>
        public static string GetProjectVersion(Project project)
        {
            if (project != null)
            {
                string query = String.Format("SELECT {0} FROM {1} WHERE ID=1;", DbCommon.COL_CMD_REV, DbCommon.TBL_ENS_OPTIONS);
                return Convert.ToString(DbCommon.RunQueryOnProjectDbObj(project, query));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Update the tblOptions for the revision of the
        /// project file.
        /// </summary>
        /// <param name="project">Project to check.</param>
        /// <param name="revision">Revision of the project.</param>
        /// <returns>Number of rows in the table.</returns>
        public static void UpdateProjectVersion(Project project, string revision)
        {
            if (project != null)
            {
                string query = String.Format("UPDATE tblOptions SET {0} = {1} WHERE ID=1;", DbCommon.COL_CMD_REV, revision);
                DbCommon.RunStatmentOnProjectDb(project, query);
            }
        }

        /// <summary>
        /// Retrieve the dataset based off the index and project.  Limit it
        /// to the first dataset found with the correct index.  The index is
        /// auto incremented so there should only be 1 ensemble per index.
        /// </summary>
        /// <param name="project">Project containing the ensemble.</param>
        /// <param name="index">Row ID</param>
        /// <returns>Dataset based off the Row ID given.</returns>
        public DataSet.Ensemble QueryForDataSet(Project project, long index)
        {
            // Query for the ensemble
            string queryEns = String.Format("SELECT * FROM {0} WHERE ID={1} LIMIT 1;", DbCommon.TBL_ENS_ENSEMBLE, index.ToString());
            DataTable data = DbCommon.GetDataTableFromProjectDb(project, queryEns);
            if (data.Rows.Count > 0)
            {
                DataSet.Ensemble dataset = ParseDataTables(project, data.Rows[0]);
                return dataset;
            }

            return null;
        }

        /// <summary>
        /// Retrieve the dataset based off the index and project.  Limit it
        /// to the first dataset found with the correct index.  The index is
        /// auto incremented so there should only be 1 ensemble per index.
        /// </summary>
        /// <param name="cnn">Sqlite Database Connection.</param>
        /// <param name="project">Project containing the ensemble.</param>
        /// <param name="index">Row ID</param>
        /// <returns>Dataset based off the Row ID given.</returns>
        public DataSet.Ensemble QueryForDataSet(SQLiteConnection cnn, Project project, long index)
        {
            // Query for the ensemble
            string queryEns = String.Format("SELECT * FROM {0} WHERE ID={1} LIMIT 1;", DbCommon.TBL_ENS_ENSEMBLE, index.ToString());
            DataTable data = DbCommon.GetDataTableFromProjectDb(cnn, project, queryEns);
            if (data.Rows.Count > 0)
            {
                DataSet.Ensemble dataset = ParseDataTables(project, data.Rows[0]);
                return dataset;
            }

            return null;
        }

        /// <summary>
        /// Get a list of ensembles from the database.  This will query for a list of
        /// rows from the database starting from the index and getting the size given.
        /// Then parse and add the data to the list.
        /// </summary>
        /// <param name="project">Project containing the ensemble.</param>
        /// <param name="index">Row ID</param>
        /// <param name="size">Number of ensembles to get from the database.</param>
        /// <returns>List of dataset based off the Row ID given and size.</returns>
        public Cache<long, DataSet.Ensemble> QueryForDataSet(Project project, long index, uint size)
        {
            Cache<long, DataSet.Ensemble> cache = new Cache<long, DataSet.Ensemble>(size);

            // Query for the ensemble
            string queryEns = String.Format("SELECT * FROM {0} WHERE ID>={1} LIMIT {2};", DbCommon.TBL_ENS_ENSEMBLE, index.ToString(), size.ToString());
            DataTable data = DbCommon.GetDataTableFromProjectDb(project, queryEns);
            foreach (DataRow row in data.Rows)
            {
                int id = 0;
                try { id = Convert.ToInt32(row[DbCommon.COL_ENS_ID]); }
                catch (Exception) { }

                DataSet.Ensemble dataset = ParseDataTables(project, row);
                cache.Add(id, dataset);
            }

            return cache;
        }

        /// <summary>
        /// Convert the data from database to dataset.
        /// </summary>
        /// <param name="project">Project containing the datasets</param>
        /// <param name="ensembleDataRow">DataTable returned from a query.</param>
        /// <returns>DataSet of ensemble.</returns>
        public DataSet.Ensemble ParseDataTables(Project project, DataRow ensembleDataRow)
        {
            // Create an ensemble
            DataSet.Ensemble ensemble = new DataSet.Ensemble();

            try
            {
                try
                {
                    // Ensemble
                    if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_ENSEMBLE_DS))
                    {
                        ensemble.EnsembleData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.EnsembleDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_ENSEMBLE_DS]));
                    }
                    if (ensemble.EnsembleData != null)
                    {
                        ensemble.IsEnsembleAvail = true;
                    }
                }
                catch (Exception e) { log.Error("Error parsing the Ensemble data from the database.", e); }

                try { 
                // Amplitude
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_AMPLITUDE_DS))
                {
                    ensemble.AmplitudeData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.AmplitudeDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_AMPLITUDE_DS]));
                }
                if (ensemble.AmplitudeData != null)
                {
                    ensemble.IsAmplitudeAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the Amplitude data from the database.", e); }

                try { 
                // Correlation
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_CORRELATION_DS))
                {
                    ensemble.CorrelationData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.CorrelationDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_CORRELATION_DS]));
                }
                if (ensemble.CorrelationData != null)
                {
                    ensemble.IsCorrelationAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the Correlation data from the database.", e); }

                try { 
                // Ancillary
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_ANCILLARY_DS))
                {
                    ensemble.AncillaryData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.AncillaryDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_ANCILLARY_DS]));
                }
                if (ensemble.AncillaryData != null)
                {
                    ensemble.IsAncillaryAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the Ancillary data from the database.", e); }

                try { 
                // Beam Velocity
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_BEAMVELOCITY_DS))
                {
                    ensemble.BeamVelocityData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.BeamVelocityDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_BEAMVELOCITY_DS]));
                }
                if (ensemble.BeamVelocityData != null)
                {
                    ensemble.IsBeamVelocityAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the Beam Velocity data from the database.", e); }

                try { 
                // Instrument Velocity
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_INSTRUMENTVELOCITY_DS))
                {
                    ensemble.InstrumentVelocityData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.InstrumentVelocityDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_INSTRUMENTVELOCITY_DS]));
                }
                if (ensemble.InstrumentVelocityData != null)
                {
                    ensemble.IsInstrumentVelocityAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the Instrument Velocity data from the database.", e); }

                try
                {
                    // Earth Velocity
                    if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_EARTHVELOCITY_DS))
                    {
                        ensemble.EarthVelocityData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.EarthVelocityDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_EARTHVELOCITY_DS]));
                    }
                    if (ensemble.EarthVelocityData != null)
                    {
                        ensemble.IsEarthVelocityAvail = true;
                    }
                }
                catch (Exception e) { log.Error("Error parsing the Earth Velocity data from the database.", e); }

                try
                {
                    // Ship Velocity
                    if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_SHIPVELOCITY_DS))
                    {
                        ensemble.ShipVelocityData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.ShipVelocityDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_SHIPVELOCITY_DS]));
                    }
                    if (ensemble.ShipVelocityData != null)
                    {
                        ensemble.IsShipVelocityAvail = true;
                    }
                }
                catch (Exception e) { log.Error("Error parsing the Ship Velocity data from the database.", e); }

                try { 
                // Good Beam
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_GOODBEAM_DS))
                {
                    ensemble.GoodBeamData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.GoodBeamDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_GOODBEAM_DS]));
                }
                if (ensemble.GoodBeamData != null)
                {
                    ensemble.IsGoodBeamAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the Good Beam data from the database.", e); }

                try { 
                // Good Earth
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_GOODEARTH_DS))
                {
                    ensemble.GoodEarthData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.GoodEarthDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_GOODEARTH_DS]));
                }
                if (ensemble.GoodEarthData != null)
                {
                    ensemble.IsGoodEarthAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the Good Earth data from the database.", e); }

                try { 
                // Bottom Track
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_BOTTOMTRACK_DS))
                {
                    ensemble.BottomTrackData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.BottomTrackDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_BOTTOMTRACK_DS]));
                }
                if (ensemble.BottomTrackData != null)
                {
                    ensemble.IsBottomTrackAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the Bottom Track data from the database.", e); }

                try { 
                // NMEA
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_NMEA_DS))
                {
                    ensemble.NmeaData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.NmeaDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_NMEA_DS]));
                }
                if (ensemble.NmeaData != null)
                {
                    ensemble.IsNmeaAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the NMEA data from the database.", e); }

                try { 
                // Earth Water Mass Velocity
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_EARTHWATERMASS_DS))
                {
                    ensemble.EarthWaterMassData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.EarthWaterMassDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_EARTHWATERMASS_DS]));
                }
                if (ensemble.EarthWaterMassData != null)
                {
                    ensemble.IsEarthWaterMassAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the Earth Water Mass Velocity data from the database.", e); }

                try { 
                // Instrument Water Mass Velocity
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_INSTRUMENTWATERMASS_DS))
                {
                    ensemble.InstrumentWaterMassData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.InstrumentWaterMassDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_INSTRUMENTWATERMASS_DS]));
                }
                if (ensemble.InstrumentWaterMassData != null)
                {
                    ensemble.IsInstrumentWaterMassAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the Instrument Water Mass Velocity data from the database.", e); }

                try
                {
                    // Ship Water Mass Velocity
                    if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_SHIPWATERMASS_DS))
                    {
                        ensemble.ShipWaterMassData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.ShipWaterMassDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_SHIPWATERMASS_DS]));
                    }
                    if (ensemble.ShipWaterMassData != null)
                    {
                        ensemble.IsShipWaterMassAvail = true;
                    }
                }
                catch (Exception e) { log.Error("Error parsing the Ship Water Mass Velocity data from the database.", e); }

                try { 
                // Profile Engineering
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_PROFILEENGINEERING_DS))
                {
                    ensemble.ProfileEngineeringData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.ProfileEngineeringDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_PROFILEENGINEERING_DS]));
                }
                if (ensemble.ProfileEngineeringData != null)
                {
                    ensemble.IsProfileEngineeringAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the Profile Engineering data from the database.", e); }

                try { 
                // Bottom Track Engineering
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_BOTTOMTRACKENGINEERING_DS))
                {
                    ensemble.BottomTrackEngineeringData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.BottomTrackEngineeringDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_BOTTOMTRACKENGINEERING_DS]));
                }
                if (ensemble.BottomTrackEngineeringData != null)
                {
                    ensemble.IsBottomTrackEngineeringAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the Bottom Track Engineering data from the database.", e); }

                try { 
                // System Setup
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_SYSTEMSETUP_DS))
                {
                    ensemble.SystemSetupData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.SystemSetupDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_SYSTEMSETUP_DS]));
                }
                if (ensemble.SystemSetupData != null)
                {
                    ensemble.IsSystemSetupAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the System Setup data from the database.", e); }

                try { 
                // Range Tracking
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_RANGETRACKING_DS))
                {
                    ensemble.RangeTrackingData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.RangeTrackingDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_RANGETRACKING_DS]));
                }
                if (ensemble.RangeTrackingData != null)
                {
                    ensemble.IsRangeTrackingAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the Ranging Tracking data from the database.", e); }

                try { 
                // ADCP GPS
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_ADCPGPS))
                {
                    ensemble.AdcpGpsData = Convert.ToString(ensembleDataRow[DbCommon.COL_ADCPGPS]);
                }
                if (!string.IsNullOrEmpty(ensemble.AdcpGpsData))
                {
                    ensemble.IsAdcpGpsDataAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the ADCP GPS data from the database.", e); }

                try { 
                // GPS 1
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_GPS1))
                {
                    ensemble.Gps1Data = Convert.ToString(ensembleDataRow[DbCommon.COL_GPS1]);
                }
                if (!string.IsNullOrEmpty(ensemble.Gps1Data))
                {
                    ensemble.IsGps1DataAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the GPS 1 data from the database.", e); }

                try { 
                // GPS 2
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_GPS2))
                {
                    ensemble.Gps2Data = Convert.ToString(ensembleDataRow[DbCommon.COL_GPS2]);
                }
                if (!string.IsNullOrEmpty(ensemble.Gps2Data))
                {
                    ensemble.IsGps2DataAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the GPS 2 data from the database.", e); }

                try { 
                // NMEA 1
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_NMEA1))
                {
                    ensemble.Nmea1Data = Convert.ToString(ensembleDataRow[DbCommon.COL_NMEA1]);
                }
                if (!string.IsNullOrEmpty(ensemble.Nmea1Data))
                {
                    ensemble.IsNmea1DataAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the NMEA 1 data from the database.", e); }

                try { 
                // NMEA 2
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_NMEA2))
                {
                    ensemble.Nmea2Data = Convert.ToString(ensembleDataRow[DbCommon.COL_NMEA2]);
                }
                if (!string.IsNullOrEmpty(ensemble.Nmea2Data))
                {
                    ensemble.IsNmea2DataAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the NMEA 2 data from the database.", e); }

                try { 
                // DVL
                if (ensembleDataRow.Table.Columns.Contains(DbCommon.COL_DVL_DS))
                {
                    ensemble.DvlData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.DvlDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_DVL_DS]));
                }
                if (ensemble.DvlData != null)
                {
                    ensemble.IsDvlDataAvail = true;
                }
                }
                catch (Exception e) { log.Error("Error parsing the DVL data from the database.", e); }

            }
            catch (Exception e)
            {
                // If there was a error parsing
                // Return what could be parsed.
                log.Error("Error parsing the data from the database.", e);
                return ensemble;
            }

            return ensemble;
        }

        #endregion


    }
}