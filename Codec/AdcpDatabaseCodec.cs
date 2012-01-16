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
 * 
 */


using System.Diagnostics;
using System.Data;
using System;
using System.Collections.Generic;
namespace RTI
{
    /// <summary>
    /// This class will read and write to an
    /// ADCP Database file.
    /// </summary>
    public class AdcpDatabaseCodec
    {
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
        public int GetNumberOfEnsembles(Project project)
        {
            if (project != null)
            {
                string query = String.Format("SELECT COUNT(*) FROM {0};", DbCommon.TBL_ENS_ENSEMBLE);
                return DbCommon.RunQueryOnProjectDb(project, query);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Retrieve the dataset based off the index and project.
        /// </summary>
        /// <param name="project">Project containing the ensemble.</param>
        /// <param name="index">Row ID</param>
        /// <returns>Dataset based off the Row ID given.</returns>
        public DataSet.Ensemble QueryForDataSet(Project project, long index)
        {
            // Query for the ensemble
            string queryEns = String.Format("SELECT * FROM {0} WHERE ID={1};", DbCommon.TBL_ENS_ENSEMBLE, index.ToString());
            DataTable data = DbCommon.GetDataTableFromProjectDb(project, queryEns);
            if (data.Rows.Count > 0)
            {
                DataSet.Ensemble dataset = ParseDataTables(project, data.Rows[0]);
                return dataset;
            }

            return null;
        }

        /// <summary>
        /// Convert the data from database to dataset.
        /// </summary>
        /// <param name="project">Project containing the datasets</param>
        /// <param name="ensembleDataRow">DataTable returned from a query.</param>
        /// <returns>DataSet of ensemble.</returns>
        public DataSet.Ensemble ParseDataTables(Project project, DataRow ensembleDataRow)
        {
            // Need to get a couple ranges
            // Initialize all ranges
            bool IsBeamVelocityAvail = false;
            bool IsInstrVelocityAvail = false;
            bool IsEarthVelocityAvail = false;
            bool IsAmplitudeAvail = false;
            bool IsCorrelationAvail = false;
            bool IsGoodBeamAvail = false;
            bool IsGoodEarthAvail = false;
            bool IsAncillaryAvail = false;
            bool IsBottomTrackAvail = false;
            bool IsNmeaAvail = false;
            int numBeams = 0;
            int numBins = 0;
            int ensId = 0;

            // Number of bins and beams and which dataset exist
            // If more than 1 result is found, return the first one found
            IsBeamVelocityAvail = (bool)ensembleDataRow[DbCommon.COL_ENS_IS_BEAM_VEL_AVAIL];
            IsInstrVelocityAvail = (bool)ensembleDataRow[DbCommon.COL_ENS_IS_INSTR_VEL_AVAIL];
            IsEarthVelocityAvail = (bool)ensembleDataRow[DbCommon.COL_ENS_IS_EARTH_VEL_AVAIL];
            IsAmplitudeAvail = (bool)ensembleDataRow[DbCommon.COL_ENS_IS_AMP_AVAIL];
            IsCorrelationAvail = (bool)ensembleDataRow[DbCommon.COL_ENS_IS_CORR_AVAIL];
            IsGoodBeamAvail = (bool)ensembleDataRow[DbCommon.COL_ENS_IS_GB_AVAIL];
            IsGoodEarthAvail = (bool)ensembleDataRow[DbCommon.COL_ENS_IS_GE_AVAIL];
            IsAncillaryAvail = (bool)ensembleDataRow[DbCommon.COL_ENS_IS_ANCIL_AVAIL];
            IsBottomTrackAvail = (bool)ensembleDataRow[DbCommon.COL_ENS_IS_BT_AVAIL];
            IsNmeaAvail = (bool)ensembleDataRow[DbCommon.COL_ENS_IS_NMEA_AVAIL];
            numBeams = Convert.ToInt32(ensembleDataRow[DbCommon.COL_ENS_NUM_BEAM].ToString());
            numBins = Convert.ToInt32(ensembleDataRow[DbCommon.COL_ENS_NUM_BIN].ToString());
            ensId = Convert.ToInt32(ensembleDataRow[DbCommon.COL_ENS_ID]);

            // Get the Beam DataTable
            // Less then BinNum is used to ensure if duplicate 
            // ensemble numbers are used, if files are combined, 
            // there will not be an index out of range error
            string queryBeam = String.Format("SELECT * FROM {0} WHERE {1}={2} AND {3}<{4};", DbCommon.TBL_ENS_BEAM, DbCommon.COL_BV_ENS_ID, ensId, DbCommon.COL_BV_BIN_NUM, numBins);
            DataTable beamDT = DbCommon.GetDataTableFromProjectDb(project, queryBeam);

            // Get the Bottom Track DataTable
            //string queryBT = String.Format("SELECT * FROM {0} WHERE {1}={2};", DbCommon.TBL_ENS_BOTTOMTRACK, DbCommon.COL_BT_ENS_ID, ensId);
            //DataTable btDT = DbCommon.GetDataTableFromProjectDb(project, queryBT);

            DataSet.Ensemble dataset = new DataSet.Ensemble();
            dataset.AddEnsembleData(DataSet.Ensemble.DATATYPE_INT,
                                        DataSet.EnsembleDataSet.NUM_DATA_ELEMENTS,
                                        numBeams,
                                        DataSet.Ensemble.DEFAULT_IMAG,
                                        DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                        DataSet.Ensemble.EnsembleDataID,
                                        ensembleDataRow);

            // Ancillary data
            if (IsAncillaryAvail)
            {
                dataset.AddAncillaryData(DataSet.Ensemble.DATATYPE_FLOAT,
                            DataSet.AncillaryDataSet.NUM_DATA_ELEMENTS,
                            numBeams,
                            DataSet.Ensemble.DEFAULT_IMAG,
                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                            DataSet.Ensemble.AncillaryID,
                            ensembleDataRow);
            }

            // Bottom track data
            if (IsBottomTrackAvail)
            {
                dataset.AddBottomTrackData(DataSet.Ensemble.DATATYPE_FLOAT,
                            numBins,
                            numBeams,
                            DataSet.Ensemble.DEFAULT_IMAG,
                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                            DataSet.Ensemble.BottomTrackID,
                            ensembleDataRow);
            }

            // NMEA data
            if (IsNmeaAvail)
            {
                dataset.AddNmeaData(ensembleDataRow[DbCommon.COL_ENS_NMEA_DATA].ToString());
            }


            // Add all the Beam data at once
            if (IsAmplitudeAvail && IsBeamVelocityAvail && IsEarthVelocityAvail && IsInstrVelocityAvail && IsCorrelationAvail && IsGoodBeamAvail && IsGoodEarthAvail)
            {
                // Add all the beam data in one loop
                AddBeamData(beamDT, dataset, numBins, numBeams);
            }
            else
            {
                // Amplitude data
                if (IsAmplitudeAvail)
                {
                    dataset.AddAmplitudeData(DataSet.Ensemble.DATATYPE_FLOAT,
                                numBins,
                                numBeams,
                                DataSet.Ensemble.DEFAULT_IMAG,
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                DataSet.Ensemble.AmplitudeID,
                                beamDT);
                }

                // Beam Velocity data
                if (IsBeamVelocityAvail)
                {
                    dataset.AddBeamVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,
                                numBins,
                                numBeams,
                                DataSet.Ensemble.DEFAULT_IMAG,
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                DataSet.Ensemble.VelocityID,
                                beamDT);
                }

                // Instrument Velocity data
                if (IsInstrVelocityAvail)
                {
                    dataset.AddInstrVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,
                                numBins,
                                numBeams,
                                DataSet.Ensemble.DEFAULT_IMAG,
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                DataSet.Ensemble.InstrumentID,
                                beamDT);
                }

                // Earth Velocity data
                if (IsEarthVelocityAvail)
                {
                    dataset.AddEarthVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,
                                numBins,
                                numBeams,
                                DataSet.Ensemble.DEFAULT_IMAG,
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                DataSet.Ensemble.EarthID,
                                beamDT);
                }

                // Correlation data
                if (IsCorrelationAvail)
                {
                    dataset.AddCorrelationData(DataSet.Ensemble.DATATYPE_FLOAT,
                                numBins,
                                numBeams,
                                DataSet.Ensemble.DEFAULT_IMAG,
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                DataSet.Ensemble.CorrelationID,
                                beamDT);
                }

                // Good Beam data
                if (IsGoodBeamAvail)
                {
                    dataset.AddGoodBeamData(DataSet.Ensemble.DATATYPE_INT,
                                numBins,
                                numBeams,
                                DataSet.Ensemble.DEFAULT_IMAG,
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                DataSet.Ensemble.GoodBeamID,
                                beamDT);
                }

                // Good Earth data
                if (IsGoodEarthAvail)
                {
                    dataset.AddGoodEarthData(DataSet.Ensemble.DATATYPE_INT,
                                numBins,
                                numBeams,
                                DataSet.Ensemble.DEFAULT_IMAG,
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                DataSet.Ensemble.GoodEarthID,
                                beamDT);
                }
            }

            return dataset;
        }

        /// <summary>
        /// Add all the beam data at one time to the all
        /// the datasets that use beam data.  This will
        /// reduce the number of loops required to create
        /// a dataset.
        /// </summary>
        /// <param name="beamDT">Reference to datatable with beam data.</param>
        /// <param name="dataset">Reference to dataset to add data to.</param>
        /// <param name="numBins">Number of bins.</param>
        /// <param name="numBeams">Number of beams.</param>
        private void AddBeamData(DataTable beamDT, DataSet.Ensemble dataset, int numBins, int numBeams)
        {
            dataset.AddAmplitudeData(DataSet.Ensemble.DATATYPE_FLOAT,
                numBins,
                numBeams,
                DataSet.Ensemble.DEFAULT_IMAG,
                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                DataSet.Ensemble.AmplitudeID);

            dataset.AddCorrelationData(DataSet.Ensemble.DATATYPE_FLOAT,
                                numBins,
                                numBeams,
                                DataSet.Ensemble.DEFAULT_IMAG,
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                DataSet.Ensemble.CorrelationID);

            dataset.AddBeamVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,
                                numBins,
                                numBeams,
                                DataSet.Ensemble.DEFAULT_IMAG,
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                DataSet.Ensemble.VelocityID);

            dataset.AddEarthVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,
                                numBins,
                                numBeams,
                                DataSet.Ensemble.DEFAULT_IMAG,
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                DataSet.Ensemble.EarthID);

            dataset.AddInstrVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,
                                numBins,
                                numBeams,
                                DataSet.Ensemble.DEFAULT_IMAG,
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                DataSet.Ensemble.InstrumentID);

            dataset.AddGoodBeamData(DataSet.Ensemble.DATATYPE_INT,
                                numBins,
                                numBeams,
                                DataSet.Ensemble.DEFAULT_IMAG,
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                DataSet.Ensemble.GoodBeamID);

            dataset.AddGoodEarthData(DataSet.Ensemble.DATATYPE_INT,
                                numBins,
                                numBeams,
                                DataSet.Ensemble.DEFAULT_IMAG,
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,
                                DataSet.Ensemble.GoodEarthID);


            // Go through the result settings the settings
            // If more than 1 result is found, return the first one found
            foreach (DataRow r in beamDT.Rows)
            {
                // Get the Bin and beam for the row
                int bin = Convert.ToInt32(r[DbCommon.COL_BV_BIN_NUM].ToString());
                int beam = Convert.ToInt32(r[DbCommon.COL_BV_BEAM_NUM].ToString());

                // Set the Amplitude data
                float value = Convert.ToSingle(r[DbCommon.COL_BV_AMP].ToString());
                dataset.AmplitudeData.AmplitudeData[bin, beam] = value;

                // Set the Correlation data
                value = Convert.ToSingle(r[DbCommon.COL_BV_CORR].ToString());
                dataset.CorrelationData.CorrelationData[bin, beam] = value;

                // Set the Beam Velocity data
                value = Convert.ToSingle(r[DbCommon.COL_BV_VEL_BEAM].ToString());
                dataset.BeamVelocityData.BeamVelocityData[bin, beam] = value;

                // Set the Earth Velocity data
                value = Convert.ToSingle(r[DbCommon.COL_BV_VEL_EARTH].ToString());
                dataset.EarthVelocityData.EarthVelocityData[bin, beam] = value;

                // Set the Instrument Velocity data
                value = Convert.ToSingle(r[DbCommon.COL_BV_VEL_INSTR].ToString());
                dataset.InstrVelocityData.InstrumentVelocityData[bin, beam] = value;

                // Set the Good Beam data
                int iValue = Convert.ToInt32(r[DbCommon.COL_BV_GOOD_BEAM].ToString());
                dataset.GoodBeamData.GoodBeamData[bin, beam] = iValue;

                // Set the Good Earth data
                iValue = Convert.ToInt32(r[DbCommon.COL_BV_GOOD_EARTH].ToString());
                dataset.GoodEarthData.GoodEarthData[bin, beam] = iValue;
            }
        }

        #endregion
    }
}