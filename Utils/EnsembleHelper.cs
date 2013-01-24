﻿/*
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
 * 12/24/2012      RC          2.17       Initial coding
 * 01/17/2013      RC          2.17       Added BottomTrack.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Create an ensemble.  This can be used for testing and for
    /// creating blank ensembles.
    /// </summary>
    public class EnsembleHelper
    {

        /// <summary>
        /// Generate an ensemble with blank data.  Set the number of bins and beams.
        /// </summary>
        /// <param name="numBins">Number of bins.</param>
        /// <param name="numBeams">Number of beams.</param>
        /// <param name="createDataSets">Set flag if all the datasets should be created automatically.</param>
        /// <returns>Ensemble created.</returns>
        public static DataSet.Ensemble GenerateEnsemble(int numBins, int numBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, bool createDataSets = true)
        {
            // Create dataset
            DataSet.Ensemble ensemble = new DataSet.Ensemble();

            // Check if the datasets should be created automatically
            if (createDataSets)
            {
                AddEnsemble(ref ensemble, numBins, numBeams);

                AddAncillary(ref ensemble, numBins, numBeams);

                AddCorrelation(ref ensemble, numBins, numBeams);

                AddAmplitude(ref ensemble, numBins, numBeams);

                AddEarthVelocity(ref ensemble, numBins, numBeams);

                AddBottomTrack(ref ensemble);

                //AddWaterMassEarth(ref ensemble, numBins, numBeams);
            }
            return ensemble;
        }

        #region Ensemble

        /// <summary>
        /// Add Ensemble Dataset to the ensemble.
        /// </summary>
        /// <param name="ensemble">Ensemble to add the dataset to.</param>
        /// <param name="numBins">Number of bins.</param>
        /// <param name="numBeams">Number of beams.</param>
        public static void AddEnsemble(ref DataSet.Ensemble ensemble, int numBins, int numBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM)
        {
            ensemble.AddEnsembleData(DataSet.Ensemble.DATATYPE_INT,                     // Type of data stored (Float or Int)
                                            numBins,                                    // Number of bins
                                            numBeams,                                   // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,              // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,       // Default Image length
                                            DataSet.Ensemble.EnsembleDataID);           // Dataset ID
        }

        #endregion

        #region Ancillary

        /// <summary>
        /// Add Ancillary Dataset to the ensemble.
        /// </summary>
        /// <param name="ensemble">Ensemble to add the dataset to.</param>
        /// <param name="numBins">Number of bins.</param>
        /// <param name="numBeams">Number of beams.</param>
        public static void AddAncillary(ref DataSet.Ensemble ensemble, int numBins, int numBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM)
        {
            ensemble.AddAncillaryData(DataSet.Ensemble.DATATYPE_FLOAT,                  // Type of data stored (Float or Int)
                                            numBins,                                    // Number of bins
                                            numBeams,                                   // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,              // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,       // Default Image length
                                            DataSet.Ensemble.AncillaryID);              // Dataset ID
        }

        #endregion

        #region Correlation

        /// <summary>
        /// Add a blank Correlation data set to
        /// the given ensemble.  This will use the number
        /// of bins and beams given to create the Correlation array.  
        /// The array will be empty.
        /// </summary>
        /// <param name="ensemble">Ensemble to add the dataset.</param>
        /// <param name="numBins">Number of bins.</param>
        /// <param name="numBeams">Number of beams.</param>
        public static void AddCorrelation(ref DataSet.Ensemble ensemble, int numBins, int numBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM)
        {
            // Check for null
            if (ensemble == null)
            {
                return;
            }

            // Add a blank Correlation Data set
            ensemble.AddCorrelationData(DataSet.Ensemble.DATATYPE_FLOAT,                // Type of data stored (Float or Int)
                                            numBins,                                    // Number of bins
                                            numBeams,                                   // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,              // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,       // Default Image length
                                            DataSet.Ensemble.CorrelationID);            // Dataset ID
        }

        #endregion

        #region Amplitude

        /// <summary>
        /// Add a blank Amplitude data set to
        /// the given ensemble.  This will use the number
        /// of bins and beams given to create the Amplitude array.  
        /// The array will be empty.
        /// </summary>
        /// <param name="ensemble">Ensemble to add the dataset.</param>
        /// <param name="numBins">Number of bins.</param>
        /// <param name="numBeams">Number of beams.</param>
        public static void AddAmplitude(ref DataSet.Ensemble ensemble, int numBins, int numBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM)
        {
            // Check for null
            if (ensemble == null)
            {
                return;
            }

            // Add a blank Correlation Data set
            ensemble.AddAmplitudeData(DataSet.Ensemble.DATATYPE_FLOAT,                  // Type of data stored (Float or Int)
                                            numBins,                                    // Number of bins
                                            numBeams,                                   // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,              // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,       // Default Image length
                                            DataSet.Ensemble.CorrelationID);            // Dataset ID
        }

        #endregion

        #region Earth Velocity

        /// <summary>
        /// Add a blank Earth velocity data set to
        /// the given ensemble.  This will use the number
        /// of bins and beams given to create the Earth
        /// Velocity array.  The array will be empty.
        /// </summary>
        /// <param name="ensemble">Ensemble to add the dataset.</param>
        /// <param name="numBins">Number of bins.</param>
        /// <param name="numBeams">Number of beams.</param>
        public static void AddEarthVelocity(ref DataSet.Ensemble ensemble, int numBins, int numBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM)
        {
            // Check for null
            if (ensemble == null)
            {
                return;
            }

            // Add a blank Earth Velocity Data set
            ensemble.AddEarthVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,              // Type of data stored (Float or Int)
                                            numBins,                                    // Number of bins
                                            numBeams,                                   // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,              // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,       // Default Image length
                                            DataSet.Ensemble.EarthID);                  // Dataset ID
        }

        #endregion

        #region Bottom Track

        /// <summary>
        /// Add the Bottom Track Dataset to the ensemble.  This will take an ensemble then add
        /// and empty Bottom Track Dataset to the ensemble.
        /// </summary>
        /// <param name="ensemble">Ensemble to add the Bottom Track Dataset.</param>
        public static void AddBottomTrack(ref DataSet.Ensemble ensemble)
        {
            ensemble.AddBottomTrackData(DataSet.Ensemble.DATATYPE_FLOAT,        // Type of data stored (Float or Int)
                                DataSet.BottomTrackDataSet.NUM_DATA_ELEMENTS,   // Number of bins (No bins for this dataset)
                                1,                                              // Number of beams
                                DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                DataSet.Ensemble.BottomTrackID);                // Dataset ID
        }

        #endregion

        #region Water Mass Earth

        /// <summary>
        /// Add Water Mass Earth data set.
        /// </summary>
        /// <param name="ensemble">Ensemble to add the dataset.</param>
        /// <param name="numBins">Number of bins in the dataset.</param>
        /// <param name="numBeams">Number of beam sin the dataset.</param>
        public static void AddWaterMassEarth(ref DataSet.Ensemble ensemble, int numBins, int numBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM)
        {
            //ensemble.AddEarthWaterMassData(DataSet.Ensemble.DATATYPE_FLOAT, 
            //                                DataSet.EarthWaterMassDataSet.NUM_DATA_ELEMENTS,        // Num elements (Bins)
            //                                DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM,                // Num Beams
            //                                DataSet.Ensemble.DEFAULT_IMAG,                          // Image
            //                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,                   // Name length
            //                                DataSet.Ensemble.WaterMassInstrumentID,                 // Name (Dataset ID)
            //                                0.3f,                                                   // Water Mass East Velocity in m/s.
            //                                -0.2f,                                                  // Water Mass North Velocity in m/s.
            //                                0.4f,                                                   // Water Mass Vertical Velocity in m/s.
            //                                0.1f);                                                  // Depth layer of the Water Mass measurement in meters.
        }

        #endregion

        #region Set Velocities Bad

        /// <summary>
        /// Set all velocities and Good Ping data bad
        /// for the given bin.  This will also verify the
        /// bin given is within range of the number of bins
        /// for the ensemble.  If the bin given is too large,
        /// nothing will be changed.
        /// </summary>
        /// <param name="ensemble">Ensemble to modify.</param>
        /// <param name="bin">Bin to modify.</param>
        public static void SetVelocitiesBad(ref DataSet.Ensemble ensemble, int bin)
        {
            

            // Beam Velocities
            if (ensemble.IsBeamVelocityAvail && bin < ensemble.BeamVelocityData.BeamVelocityData.GetLength(0))
            {
                ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_0_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_1_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_2_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_3_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            }
            if (ensemble.IsGoodBeamAvail && bin < ensemble.GoodBeamData.GoodBeamData.GetLength(0))
            {
                ensemble.GoodBeamData.GoodBeamData[bin, DataSet.Ensemble.BEAM_0_INDEX] = 0;
                ensemble.GoodBeamData.GoodBeamData[bin, DataSet.Ensemble.BEAM_1_INDEX] = 0;
                ensemble.GoodBeamData.GoodBeamData[bin, DataSet.Ensemble.BEAM_2_INDEX] = 0;
                ensemble.GoodBeamData.GoodBeamData[bin, DataSet.Ensemble.BEAM_3_INDEX] = 0;
            }

            // Earth Velocities
            if (ensemble.IsEarthVelocityAvail && bin < ensemble.EarthVelocityData.EarthVelocityData.GetLength(0))
            {
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            }
            if (ensemble.IsGoodEarthAvail && bin < ensemble.GoodEarthData.GoodEarthData.GetLength(0))
            {
                ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = 0;
                ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = 0;
                ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0;
                ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = 0;
            }

            // Earth Velocities
            if (ensemble.IsInstrVelocityAvail && bin < ensemble.InstrVelocityData.InstrumentVelocityData.GetLength(0))
            {
                ensemble.InstrVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_X_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                ensemble.InstrVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Y_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                ensemble.InstrVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Z_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                ensemble.InstrVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            }
        }

        #endregion

    }
}
