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

        /// <summary>
        /// Shutdown method.
        /// </summary>
        public void Shutdown()
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
            DataTable data = DbCommon.GetDataTableFromProjectDb(project, queryEns);
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
                // Ensemble
                ensemble.EnsembleData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.EnsembleDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_ENSEMBLE_DS]));
                if (ensemble.EnsembleData != null)
                {
                    ensemble.IsEnsembleAvail = true;
                }

                // Amplitude
                ensemble.AmplitudeData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.AmplitudeDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_AMPLITUDE_DS]));
                if (ensemble.AmplitudeData != null)
                {
                    ensemble.IsAmplitudeAvail = true;
                }

                // Correlation
                ensemble.CorrelationData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.CorrelationDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_CORRELATION_DS]));
                if (ensemble.CorrelationData != null)
                {
                    ensemble.IsCorrelationAvail = true;
                }

                // Ancillary
                ensemble.AncillaryData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.AncillaryDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_ANCILLARY_DS]));
                if (ensemble.AncillaryData != null)
                {
                    ensemble.IsAncillaryAvail = true;
                }

                // Beam Velocity
                ensemble.BeamVelocityData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.BeamVelocityDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_BEAMVELOCITY_DS]));
                if (ensemble.BeamVelocityData != null)
                {
                    ensemble.IsBeamVelocityAvail = true;
                }

                // Instrument Velocity
                ensemble.InstrumentVelocityData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.InstrumentVelocityDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_INSTRUMENTVELOCITY_DS]));
                if (ensemble.InstrumentVelocityData != null)
                {
                    ensemble.IsInstrumentVelocityAvail = true;
                }

                // Earth Velocity
                ensemble.EarthVelocityData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.EarthVelocityDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_EARTHVELOCITY_DS]));
                if (ensemble.EarthVelocityData != null)
                {
                    ensemble.IsEarthVelocityAvail = true;
                }

                // Good Beam
                ensemble.GoodBeamData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.GoodBeamDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_GOODBEAM_DS]));
                if (ensemble.GoodBeamData != null)
                {
                    ensemble.IsGoodBeamAvail = true;
                }

                // Good Earth
                ensemble.GoodEarthData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.GoodEarthDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_GOODEARTH_DS]));
                if (ensemble.GoodEarthData != null)
                {
                    ensemble.IsGoodEarthAvail = true;
                }

                // Bottom Track
                ensemble.BottomTrackData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.BottomTrackDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_BOTTOMTRACK_DS]));
                if (ensemble.BottomTrackData != null)
                {
                    ensemble.IsBottomTrackAvail = true;
                }

                // NMEA
                ensemble.NmeaData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.NmeaDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_NMEA_DS]));
                if (ensemble.NmeaData != null)
                {
                    ensemble.IsNmeaAvail = true;
                }

                // Earth Water Mass Velocity
                ensemble.EarthWaterMassData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.EarthWaterMassDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_EARTHWATERMASS_DS]));
                if (ensemble.EarthWaterMassData != null)
                {
                    ensemble.IsEarthWaterMassAvail = true;
                }

                // Instrument Water Mass Velocity
                ensemble.InstrumentWaterMassData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.InstrumentWaterMassDataSet>(Convert.ToString(ensembleDataRow[DbCommon.COL_INSTRUMENTWATERMASS_DS]));
                if (ensemble.InstrumentWaterMassData != null)
                {
                    ensemble.IsInstrumentWaterMassAvail = true;
                }

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