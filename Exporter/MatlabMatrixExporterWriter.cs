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
 * 06/23/2018      RC          3.4.7      Initial coding
 * 09/14/2018      RC          3.4.10     Check if the data exist before trying to write it. 
 * 
 * 
 * 
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;

    /// <summary>
    /// Export the data to Matlab format.
    /// This will strip the RTI header from each dataset.
    /// </summary>
    public class MatlabMatrixExporterWriter : IExporterWriter
    {
        #region Variables

        // Setup logger
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Sub-file name to be used for each
        /// ensemble file.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// Folder path.
        /// </summary>
        private string _filePath;

        /// <summary>
        /// Export options.
        /// </summary>
        private ExportOptions _options;

        /// <summary>
        /// Store the ensemble data based off the subsystem.
        /// </summary>
        private Dictionary<SubsystemConfiguration, List<EnsMatrix>> _dictSubsystem;

        #endregion

        #region Class

        /// <summary>
        /// Ensemble matrix.
        /// </summary>
        private class EnsMatrix
        {
            /// <summary>
            /// Subsystem of the ensemble.
            /// </summary>
            public SubsystemConfiguration SubsystemConfig { get; set; }

            /// <summary>
            /// Serial number.
            /// </summary>
            public SerialNumber SerialNumber { get; set; }

            #region Matrix

            /// <summary>
            /// Ensemble data info.
            /// </summary>
            public EnsData EnsData { get; set; }

            /// <summary>
            /// Water Velocity data and vectors.
            /// </summary>
            public VelData VelData { get; set; }

            /// <summary>
            /// NMEA data.
            /// </summary>
            public NmeaData NmeaData { get; set; }

            /// <summary>
            /// Bottom Track velocity and range data.
            /// </summary>
            public BtData BtData { get; set; }

            /// <summary>
            /// Initialize the data.
            /// </summary>
            /// <param name="ensemble">Ensemble data.</param>
            public EnsMatrix(DataSet.Ensemble ensemble)
            {
                if (ensemble != null)
                {
                    if (ensemble.IsEnsembleAvail)
                    {
                        // Subsystem
                        SubsystemConfig = ensemble.EnsembleData.SubsystemConfig;
                        SerialNumber = ensemble.EnsembleData.SysSerialNumber;
                    }

                    // Ensemble data
                    EnsData = new EnsData(ensemble);

                    // Velocity data
                    VelData = new VelData(ensemble);

                    // Bottom Track data
                    BtData = new BtData(ensemble);

                    // NMEA data
                    NmeaData = new NmeaData(ensemble);
                }
            }

            #endregion

            public EnsMatrix()
            {

            }
        }

        /// <summary>
        /// Velocity data.
        /// </summary>
        public class VelData
        {
            /// <summary>
            /// Beam 0 velocity
            /// </summary>
            public float[] Beam0 { get; set; }

            /// <summary>
            /// Beam 1 velocity.
            /// </summary>
            public float[] Beam1 { get; set; }

            /// <summary>
            /// Beam 2 velocity.
            /// </summary>
            public float[] Beam2 { get; set; }

            /// <summary>
            /// Beam 3 velocity
            /// </summary>
            public float[] Beam3 { get; set; }

            /// <summary>
            /// Earth East velocity.
            /// </summary>
            public float[] EarthEast { get; set; }

            /// <summary>
            /// Earth North velocity.
            /// </summary>
            public float[] EarthNorth { get; set; }

            /// <summary>
            /// Earth Vertical velocity.
            /// </summary>
            public float[] EarthVert { get; set; }

            /// <summary>
            /// Earth Error velocity.
            /// </summary>
            public float[] EarthError { get; set; }

            /// <summary>
            /// Water magnitude.
            /// </summary>
            public float[] Magnitude { get; set; }

            /// <summary>
            /// Water Direction.
            /// </summary>
            public float[] Direction { get; set; }

            /// <summary>
            /// Initialize the values.
            /// </summary>
            /// <param name="ensemble">Ensemble data.</param>
            public VelData(DataSet.Ensemble ensemble)
            {
                // Earth Velocity
                if (ensemble.IsEarthVelocityAvail)
                {
                    int numBeams = ensemble.EarthVelocityData.ElementsMultiplier;
                    int numBins = ensemble.EarthVelocityData.NumElements;

                    // Init arrays
                    EarthEast = new float[numBins];
                    EarthNorth = new float[numBins];
                    EarthVert = new float[numBins];
                    EarthError = new float[numBins];

                    for (int bin = 0; bin < numBins; bin++)
                    {
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            if (beam == 0)
                            {
                                EarthEast[bin] = ensemble.EarthVelocityData.EarthVelocityData[bin, beam];
                            }
                            if (beam == 1)
                            {
                                EarthNorth[bin] = ensemble.EarthVelocityData.EarthVelocityData[bin, beam];
                            }
                            if (beam == 0)
                            {
                                EarthVert[bin] = ensemble.EarthVelocityData.EarthVelocityData[bin, beam];
                            }
                            if (beam == 0)
                            {
                                EarthError[bin] = ensemble.EarthVelocityData.EarthVelocityData[bin, beam];
                            }
                        }
                    }
                }

                // Beam Velocity
                if (ensemble.IsBeamVelocityAvail)
                {
                    int numBeams = ensemble.BeamVelocityData.ElementsMultiplier;
                    int numBins = ensemble.BeamVelocityData.NumElements;

                    // Init arrays
                    Beam0 = new float[numBins];
                    Beam1 = new float[numBins];
                    Beam2 = new float[numBins];
                    Beam3 = new float[numBins];

                    for (int bin = 0; bin < numBins; bin++)
                    {
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            if (beam == 0)
                            {
                                Beam0[bin] = ensemble.BeamVelocityData.BeamVelocityData[bin, beam];
                            }
                            if (beam == 1)
                            {
                                Beam1[bin] = ensemble.BeamVelocityData.BeamVelocityData[bin, beam];
                            }
                            if (beam == 0)
                            {
                                Beam2[bin] = ensemble.BeamVelocityData.BeamVelocityData[bin, beam];
                            }
                            if (beam == 0)
                            {
                                Beam3[bin] = ensemble.BeamVelocityData.BeamVelocityData[bin, beam];
                            }
                        }
                    }
                }

                // Velocity vector
                if (ensemble.IsEarthVelocityAvail)
                {
                    if (ensemble.EarthVelocityData.IsVelocityVectorAvail)
                    {
                        int numBins = ensemble.EarthVelocityData.VelocityVectors.Length;

                        // Init arrays
                        Magnitude = new float[numBins];
                        Direction = new float[numBins];

                        for (int bin = 0; bin < numBins; bin++)
                        {
                            Magnitude[bin] = (float)ensemble.EarthVelocityData.VelocityVectors[bin].Magnitude;
                            Direction[bin] = (float)ensemble.EarthVelocityData.VelocityVectors[bin].DirectionYNorth;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Encoded Velocity data.
        /// </summary>
        public class VelDataEncode
        {
            /// <summary>
            /// Beam 0 velocity
            /// </summary>
            public byte[] Beam0 { get; set; }

            /// <summary>
            /// Beam 0 Name.
            /// </summary>
            public const string BEAM_0_NAME = "BEAM_VEL_0\0";

            /// <summary>
            /// Beam 0 Name length.
            /// </summary>
            public const int BEAM_0_NAMELENGTH = 11;

            /// <summary>
            /// Beam 1 velocity.
            /// </summary>
            public byte[] Beam1 { get; set; }

            /// <summary>
            /// Beam 1 Name.
            /// </summary>
            public const string BEAM_1_NAME = "BEAM_VEL_1\0";

            /// <summary>
            /// Beam 1 Name length.
            /// </summary>
            public const int BEAM_1_NAMELENGTH = 11;

            /// <summary>
            /// Beam 2 velocity.
            /// </summary>
            public byte[] Beam2 { get; set; }

            /// <summary>
            /// Beam 2 Name.
            /// </summary>
            public const string BEAM_2_NAME = "BEAM_VEL_2\0";

            /// <summary>
            /// Beam 2 Name length.
            /// </summary>
            public const int BEAM_2_NAMELENGTH = 11;

            /// <summary>
            /// Beam 3 velocity
            /// </summary>
            public byte[] Beam3 { get; set; }

            /// <summary>
            /// Beam 3 Name.
            /// </summary>
            public const string BEAM_3_NAME = "BEAM_VEL_3\0";

            /// <summary>
            /// Beam 3 Name length.
            /// </summary>
            public const int BEAM_3_NAMELENGTH = 11;

            /// <summary>
            /// Earth East velocity.
            /// </summary>
            public byte[] EarthEast { get; set; }

            /// <summary>
            /// Earth Velocity U Name.
            /// </summary>
            public const string EARTH_VEL_U_NAME = "EARTH_VEL_U\0";

            /// <summary>
            /// Earth Velocity U Name length.
            /// </summary>
            public const int EARTH_VEL_U_NAMELENGTH = 12;

            /// <summary>
            /// Earth North velocity.
            /// </summary>
            public byte[] EarthNorth { get; set; }

            /// <summary>
            /// Earth Velocity V Name.
            /// </summary>
            public const string EARTH_VEL_V_NAME = "EARTH_VEL_V\0";

            /// <summary>
            /// Earth Velocity V Name length.
            /// </summary>
            public const int EARTH_VEL_V_NAMELENGTH = 12;

            /// <summary>
            /// Earth Vertical velocity.
            /// </summary>
            public byte[] EarthVert { get; set; }

            /// <summary>
            /// Earth Velocity Z Name.
            /// </summary>
            public const string EARTH_VEL_Z_NAME = "EARTH_VEL_Z\0";

            /// <summary>
            /// Earth Velocity Z Name length.
            /// </summary>
            public const int EARTH_VEL_Z_NAMELENGTH = 12;

            /// <summary>
            /// Earth Error velocity.
            /// </summary>
            public byte[] EarthError { get; set; }

            /// <summary>
            /// Earth Velocity Err Name.
            /// </summary>
            public const string EARTH_VEL_ERR_NAME = "EARTH_VEL_ERR\0";

            /// <summary>
            /// Earth Velocity Err Name length.
            /// </summary>
            public const int EARTH_VEL_ERR_NAMELENGTH = 14;

            /// <summary>
            /// Water magnitude.
            /// </summary>
            public byte[] Magnitude { get; set; }

            /// <summary>
            /// Velocity Magnitude Name.
            /// </summary>
            public const string VEL_MAG_NAME = "VEL_MAG\0";

            /// <summary>
            /// Velocity Magnitude Name length.
            /// </summary>
            public const int VEL_MAG_NAMELENGTH = 8;

            /// <summary>
            /// Water Direction.
            /// </summary>
            public byte[] Direction { get; set; }

            /// <summary>
            /// Velocity Direction Name.
            /// </summary>
            public const string VEL_DIR_NAME = "VEL_DIR\0";

            /// <summary>
            /// Velocity Direction Name length.
            /// </summary>
            public const int VEL_DIR_NAMELENGTH = 8;
        }

        /// <summary>
        /// Bottom Track data.
        /// </summary>
        public class BtData
        {
            /// <summary>
            /// Beam 0 velocity.
            /// </summary>
            public float Beam0 { get; set; }

            /// <summary>
            /// Beam 1 velocity.
            /// </summary>
            public float Beam1 { get; set; }

            /// <summary>
            /// Beam 2 velocity.
            /// </summary>
            public float Beam2 { get; set; }

            /// <summary>
            /// Beam 3 velocity.
            /// </summary>
            public float Beam3 { get; set; }

            /// <summary>
            /// Earth East velocity.
            /// </summary>
            public float EarthEast { get; set; }

            /// <summary>
            /// Earth North velocity.
            /// </summary>
            public float EarthNorth { get; set; }

            /// <summary>
            /// Earth Vertical velocity.
            /// </summary>
            public float EarthVertical { get; set; }

            /// <summary>
            /// Earth Error velocity.
            /// </summary>
            public float EarthError { get; set; }

            /// <summary>
            /// Beam 0 Range.
            /// </summary>
            public float Range0 { get; set; }

            /// <summary>
            /// Beam 1 Range.
            /// </summary>
            public float Range1 { get; set; }

            /// <summary>
            /// Beam 2 Range.
            /// </summary>
            public float Range2 { get; set; }

            /// <summary>
            /// Beam 3 Range.
            /// </summary>
            public float Range3 { get; set; }

            /// <summary>
            /// Status
            /// </summary>
            public float Status { get; set; }

            /// <summary>
            /// Initialize the value.
            /// </summary>
            /// <param name="ensemble">Ensemble data</param>
            public BtData(DataSet.Ensemble ensemble)
            {
                if (ensemble.IsBottomTrackAvail)
                {
                    Status = ensemble.BottomTrackData.Status.Value;

                    for(int beam = 0; beam < ensemble.BottomTrackData.NumBeams; beam++)
                    {
                        if(beam == 0)
                        {
                            Beam0 = ensemble.BottomTrackData.BeamVelocity[beam];
                            EarthEast = ensemble.BottomTrackData.EarthVelocity[beam];
                            Range0 = ensemble.BottomTrackData.Range[beam];
                        }

                        if (beam == 1)
                        {
                            Beam1 = ensemble.BottomTrackData.BeamVelocity[beam];
                            EarthNorth = ensemble.BottomTrackData.EarthVelocity[beam];
                            Range1 = ensemble.BottomTrackData.Range[beam];
                        }

                        if (beam == 2)
                        {
                            Beam2 = ensemble.BottomTrackData.BeamVelocity[beam];
                            EarthVertical = ensemble.BottomTrackData.EarthVelocity[beam];
                            Range2 = ensemble.BottomTrackData.Range[beam];
                        }

                        if (beam == 3)
                        {
                            Beam3 = ensemble.BottomTrackData.BeamVelocity[beam];
                            EarthError = ensemble.BottomTrackData.EarthVelocity[beam];
                            Range3 = ensemble.BottomTrackData.Range[beam];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Encoded Bottom Track data.
        /// </summary>
        public class BtDataEncode
        {
            /// <summary>
            /// Beam 0 velocity.
            /// </summary>
            public byte[] Beam0 { get; set; }

            /// <summary>
            /// Beam 0 Name.
            /// </summary>
            public const string BEAM_0_NAME = "BEAM0_BT\0";

            /// <summary>
            /// Beam 0 Name length.
            /// </summary>
            public const int BEAM_0_NAMELENGTH = 9;

            /// <summary>
            /// Beam 1 velocity.
            /// </summary>
            public byte[] Beam1 { get; set; }

            /// <summary>
            /// Beam 1 Name.
            /// </summary>
            public const string BEAM_1_NAME = "BEAM1_BT\0";

            /// <summary>
            /// Beam 1 Name length.
            /// </summary>
            public const int BEAM_1_NAMELENGTH = 9;

            /// <summary>
            /// Beam 2 velocity.
            /// </summary>
            public byte[] Beam2 { get; set; }

            /// <summary>
            /// Beam 2 Name.
            /// </summary>
            public const string BEAM_2_NAME = "BEAM2_BT\0";

            /// <summary>
            /// Beam 2 Name length.
            /// </summary>
            public const int BEAM_2_NAMELENGTH = 9;

            /// <summary>
            /// Beam 3 velocity.
            /// </summary>
            public byte[] Beam3 { get; set; }

            /// <summary>
            /// Beam 3 Name.
            /// </summary>
            public const string BEAM_3_NAME = "BEAM3_BT\0";

            /// <summary>
            /// Beam 3 Name length.
            /// </summary>
            public const int BEAM_3_NAMELENGTH = 9;

            /// <summary>
            /// Earth East velocity.
            /// </summary>
            public byte[] EarthEast { get; set; }

            /// <summary>
            /// Earth East Name.
            /// </summary>
            public const string EARTH_EAST_NAME = "U_BT\0";

            /// <summary>
            /// Earth East Name length.
            /// </summary>
            public const int EARTH_EAST_NAMELENGTH = 5;

            /// <summary>
            /// Earth North velocity.
            /// </summary>
            public byte[] EarthNorth { get; set; }

            /// <summary>
            /// Earth North Name.
            /// </summary>
            public const string EARTH_NORTH_NAME = "V_BT\0";

            /// <summary>
            /// Earth North Name length.
            /// </summary>
            public const int EARTH_NORTH_NAMELENGTH = 5;

            /// <summary>
            /// Earth Vertical velocity.
            /// </summary>
            public byte[] EarthVertical { get; set; }

            /// <summary>
            /// Earth Vertical Name.
            /// </summary>
            public const string EARTH_VERT_NAME = "Z_BT\0";

            /// <summary>
            /// Earth Vertical Name length.
            /// </summary>
            public const int EARTH_VERT_NAMELENGTH = 5;

            /// <summary>
            /// Earth Error velocity.
            /// </summary>
            public byte[] EarthError { get; set; }

            /// <summary>
            /// Earth Error Name.
            /// </summary>
            public const string EARTH_ERROR_NAME = "Err_BT\0";

            /// <summary>
            /// Earth Error Name length.
            /// </summary>
            public const int EARTH_ERROR_NAMELENGTH = 7;

            /// <summary>
            /// Beam 0 Range.
            /// </summary>
            public byte[] Range0 { get; set; }

            /// <summary>
            /// Range 0 Name.
            /// </summary>
            public const string RANGE_0_NAME = "Range0\0";

            /// <summary>
            /// Range 0 Name length.
            /// </summary>
            public const int RANGE_0_NAMELENGTH = 7;

            /// <summary>
            /// Beam 1 Range.
            /// </summary>
            public byte[] Range1 { get; set; }

            /// <summary>
            /// Range 1 Name.
            /// </summary>
            public const string RANGE_1_NAME = "Range1\0";

            /// <summary>
            /// Range 1 Name length.
            /// </summary>
            public const int RANGE_1_NAMELENGTH = 7;

            /// <summary>
            /// Beam 2 Range.
            /// </summary>
            public byte[] Range2 { get; set; }

            /// <summary>
            /// Range 2 Name.
            /// </summary>
            public const string RANGE_2_NAME = "Range2\0";

            /// <summary>
            /// Range 2 Name length.
            /// </summary>
            public const int RANGE_2_NAMELENGTH = 7;

            /// <summary>
            /// Beam 3 Range.
            /// </summary>
            public byte[] Range3 { get; set; }

            /// <summary>
            /// Range 3 Name.
            /// </summary>
            public const string RANGE_3_NAME = "Range3\0";

            /// <summary>
            /// Range 3 Name length.
            /// </summary>
            public const int RANGE_3_NAMELENGTH = 7;

            /// <summary>
            /// Status
            /// </summary>
            public byte[] Status { get; set; }

            /// <summary>
            /// BT Status Name.
            /// </summary>
            public const string BT_STATUS_NAME = "Status_BT\0";

            /// <summary>
            /// BT Status Name length.
            /// </summary>
            public const int BT_STATUS_NAMELENGTH = 10;
        }

        /// <summary>
        /// NMEA data.
        /// </summary>
        public class NmeaData
        {
            /// <summary>
            /// Latitude.
            /// </summary>
            public float Lat { get; set; }

            /// <summary>
            /// Longitude.
            /// </summary>
            public float Lon { get; set; }

            /// <summary>
            /// Heading.
            /// </summary>
            public float Heading { get; set; }

            /// <summary>
            /// Speed.
            /// </summary>
            public float Speed { get; set; }

            /// <summary>
            /// Initialize the values.
            /// </summary>
            /// <param name="ensemble"></param>
            public NmeaData(DataSet.Ensemble ensemble)
            {
                if (ensemble.IsNmeaAvail)
                {
                    // Latitude and Longitude
                    if (ensemble.NmeaData.IsGpggaAvail())
                    {
                        Lat = (float)ensemble.NmeaData.GPGGA.Position.Latitude.DecimalDegrees;
                        Lon = (float)ensemble.NmeaData.GPGGA.Position.Longitude.DecimalDegrees;
                    }

                    // Speed
                    if (ensemble.NmeaData.IsGpvtgAvail())
                    {
                        Speed = (float)ensemble.NmeaData.GPVTG.Speed.ToMetersPerSecond().Value;
                    }

                    // Heading
                    if (ensemble.NmeaData.IsGphdtAvail())
                    {
                        Heading = (float)ensemble.NmeaData.GPHDT.Heading.DecimalDegrees;
                    }
                }
            }
        }

        /// <summary>
        /// Encoded NMEA data.
        /// </summary>
        public class NmeaDataEncode
        {
            /// <summary>
            /// Number of elements in the data set.
            /// </summary>
            public const int NUM_ELEMENTS = 4;

            /// <summary>
            /// NMEA data encoded.
            /// </summary>
            public byte[] NmeaData { get; set; }

            /// <summary>
            /// NMEA Name.
            /// </summary>
            public const string NMEA_NAME = "NMEA\0";

            /// <summary>
            /// NMEA Name length.
            /// </summary>
            public const int NMEA_NAMELENGTH = 5;
        }

        /// <summary>
        /// Ensemble data.
        /// </summary>
        public class EnsData
        {
            /// <summary>
            /// Date and time year.
            /// </summary>
            public float Year { get; set; }

            /// <summary>
            /// Date and time month.
            /// </summary>
            public float Month { get; set; }

            /// <summary>
            /// Date and time day.
            /// </summary>
            public float Day { get; set; }

            /// <summary>
            /// Date and time Hour.
            /// </summary>
            public float Hour { get; set; }

            /// <summary>
            /// Date and time minute.
            /// </summary>
            public float Minute { get; set; }

            /// <summary>
            /// Date and time second.
            /// </summary>
            public float Second { get; set; }

            /// <summary>
            /// Date and time Hundredth of Second.
            /// </summary>
            public float HSecond { get; set; }

            /// <summary>
            /// Number of beams.
            /// </summary>
            public float NumBeams { get; set; }

            /// <summary>
            /// Number of bins.
            /// </summary>
            public float NumBins { get; set; }

            /// <summary>
            /// Heading.
            /// </summary>
            public float Heading { get; set; }

            /// <summary>
            /// Pitch.
            /// </summary>
            public float Pitch { get; set; }

            /// <summary>
            /// Roll.
            /// </summary>
            public float Roll { get; set; }

            /// <summary>
            /// Status string.
            /// </summary>
            public float Status { get; set; }

            /// <summary>
            /// Intiialize the data.
            /// </summary>
            /// <param name="ensemble"></param>
            public EnsData(DataSet.Ensemble ensemble)
            {
                // Date Time and Status
                if (ensemble.IsEnsembleAvail)
                {
                    Year = ensemble.EnsembleData.Year;
                    Month = ensemble.EnsembleData.Month;
                    Day = ensemble.EnsembleData.Day;
                    Hour = ensemble.EnsembleData.Hour;
                    Minute = ensemble.EnsembleData.Minute;
                    Second = ensemble.EnsembleData.Second;
                    HSecond = ensemble.EnsembleData.HSec;

                    NumBeams = ensemble.EnsembleData.NumBeams;
                    NumBins = ensemble.EnsembleData.NumBins;

                    Status = ensemble.EnsembleData.Status.Value;
                }

                // HPR
                if (ensemble.IsAncillaryAvail)
                {
                    Heading = ensemble.AncillaryData.Heading;
                    Pitch = ensemble.AncillaryData.Pitch;
                    Roll = ensemble.AncillaryData.Roll;
                }
            }
        }

        /// <summary>
        /// Encoded Ens data.
        /// </summary>
        public class EnsDataEncode
        {
            /// <summary>
            /// Number of elements in the data set.
            /// </summary>
            public const int NUM_ELEMENTS = 13;

            /// <summary>
            /// ENS data encoded.
            /// </summary>
            public byte[] EnsData { get; set; }

            /// <summary>
            /// ENS Name.
            /// </summary>
            public const string ENS_NAME = "ENS\0";

            /// <summary>
            /// ENS Name length.
            /// </summary>
            public const int ENS_NAMELENGTH = 4;
        }

        #endregion

        /// <summary>
        /// Initialize the object.
        /// </summary>
        public MatlabMatrixExporterWriter()
        {
            _fileName = "";

            string myDoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string DEFAULT_FOLDER_PATH = string.Format(@"{0}\RTI", myDoc);

            _filePath = DEFAULT_FOLDER_PATH;
            _options = new ExportOptions();

            _dictSubsystem = new Dictionary<SubsystemConfiguration, List<EnsMatrix>>();
        }

        
        /// <summary>
        /// Open the export to begin the writing process.
        /// The filename will be used to give the sub-filename.
        /// Files will be broken up by ensemble.
        /// </summary>
        /// <param name="filePath">File path to the write to.</param>
        /// <param name="fileName">Sub-Filename to use to for each file.</param>
        /// <param name="options">Export options.</param>
        public void  Open(string filePath, string fileName, ExportOptions options)
        {
 	        //Set the file path and name
            _filePath = filePath;
            _fileName = fileName;
            _options = options;
        }

        /// <summary>
        /// Each ensemble will have its own file.  So the filename will contain the subsystem and 
        /// ensemble number.
        /// </summary>
        /// <param name="ensemble">Ensemble to encode to matlab.</param>
        /// <param name="isMultipleFile">Set flag if you want individual files per ensemble or combine it all into one file.</param>
        public void Write(DataSet.Ensemble ensemble, bool isMultipleFile = false)
        {
            if (ensemble != null && ensemble.IsEnsembleAvail)
            {
                SubsystemConfiguration ss = ensemble.EnsembleData.SubsystemConfig;
                if(!_dictSubsystem.ContainsKey(ss))
                {
                    _dictSubsystem[ss] = new List<EnsMatrix>();
                }

                // Add the data to the ENS matrix
                _dictSubsystem[ss].Add(new EnsMatrix(ensemble));
            }
        }

        /// <summary>
        /// Do nothing
        /// </summary>
        /// <returns>Return the options.</returns>
        public ExportOptions Close()
        {
            List<BtData> btData = new List<BtData>();
            List<VelData> velData = new List<VelData>();
            List<NmeaData> nmeaData = new List<NmeaData>();
            List<EnsData> ensData = new List<EnsData>();

            // Write all the data to the files
            // Go through each subsystem
            foreach(var ss in _dictSubsystem)
            {
                // Store the serial number
                string serialNumber = String.Empty;

                // Go through each ensemble
                foreach (var ens in ss.Value)
                {
                    // Set the serial number if it is empty still
                    if (string.IsNullOrEmpty(serialNumber))
                    {
                        serialNumber = ens.SerialNumber.ToString(); ;
                    }

                    // Accumulate the data
                    btData.Add(ens.BtData);
                    velData.Add(ens.VelData);
                    nmeaData.Add(ens.NmeaData);
                    ensData.Add(ens.EnsData);
                }

                #region Write Vel Data

                // Convert data to byte[]
                VelDataEncode encodedVelData = EncodeVelData(velData);

                WriteFile(encodedVelData.Beam0, serialNumber, ss.Key, "Beam0");
                WriteFile(encodedVelData.Beam1, serialNumber, ss.Key, "Beam1");
                WriteFile(encodedVelData.Beam2, serialNumber, ss.Key, "Beam2");
                WriteFile(encodedVelData.Beam3, serialNumber, ss.Key, "Beam3");
                WriteFile(encodedVelData.EarthEast, serialNumber, ss.Key, "U");
                WriteFile(encodedVelData.EarthNorth, serialNumber, ss.Key, "V");
                WriteFile(encodedVelData.EarthVert, serialNumber, ss.Key, "Z");
                WriteFile(encodedVelData.EarthError, serialNumber, ss.Key, "Err");
                WriteFile(encodedVelData.Magnitude, serialNumber, ss.Key, "Mag");
                WriteFile(encodedVelData.Direction, serialNumber, ss.Key, "Dir");

                #endregion

                #region Write BT Data

                // Convert data to byte[]
                BtDataEncode encodedBtData = EncodeBtData(btData);

                // Write the data
                WriteFile(encodedBtData.Beam0, serialNumber, ss.Key, "BT_Beam0");
                WriteFile(encodedBtData.Beam1, serialNumber, ss.Key, "BT_Beam1");
                WriteFile(encodedBtData.Beam2, serialNumber, ss.Key, "BT_Beam2");
                WriteFile(encodedBtData.Beam3, serialNumber, ss.Key, "BT_Beam3");
                WriteFile(encodedBtData.EarthEast, serialNumber, ss.Key, "BT_U");
                WriteFile(encodedBtData.EarthNorth, serialNumber, ss.Key, "BT_V");
                WriteFile(encodedBtData.EarthVertical, serialNumber, ss.Key, "BT_Z");
                WriteFile(encodedBtData.EarthError, serialNumber, ss.Key, "BT_Err");
                WriteFile(encodedBtData.Range0, serialNumber, ss.Key, "BT_Range0");
                WriteFile(encodedBtData.Range1, serialNumber, ss.Key, "BT_Range1");
                WriteFile(encodedBtData.Range2, serialNumber, ss.Key, "BT_Range2");
                WriteFile(encodedBtData.Range3, serialNumber, ss.Key, "BT_Range3");
                WriteFile(encodedBtData.Status, serialNumber, ss.Key, "BT_Status");

                #endregion

                #region Write NMEA Data

                // Convert data to byte[]
                NmeaDataEncode encodedNmeaData = EncodeNmeaData(nmeaData);

                // Write the data
                WriteFile(encodedNmeaData.NmeaData, serialNumber, ss.Key, "NMEA");

                #endregion

                #region Write ENS Data

                // Convert data to byte[]
                EnsDataEncode encodedEnsData = EncodeEnsData(ensData);

                // Write the data
                WriteFile(encodedEnsData.EnsData, serialNumber, ss.Key, "ENS");

                #endregion
            }


            return _options;
        }


        /// <summary>
        /// Create the file name
        /// filepath_ProjectName_serialnumber_cepoIndex_subsystemCode_dataType.mat
        /// </summary>
        /// <param name="matrix">Data to write to the file.</param>
        /// <param name="serialNumber">Serial number of the ADCP.</param>
        /// <param name="subsystem">Subsystem number.</param>
        /// <param name="name">Name of the data type.</param>
        private void WriteFile(byte[] matrix, string serialNumber, SubsystemConfiguration subsystem, string name)
        {
            // Create a file name
            string filename = _filePath + _fileName;
            filename += "_" + serialNumber;
            filename += "_" + Convert.ToString(subsystem.CepoIndex) + "_" + subsystem.SubSystem.CodeToString() + "_";
            filename += name;

            // Get the extension
            filename += ".mat";

            try
            {
                File.WriteAllBytes(filename, matrix);
            }
            catch (Exception)
            {
                //log.Error(string.Format("Error writing matlab file {0}", filename), e);
            }
        }

        /// <summary>
        /// Encode the Velocity data to byte arrays to write to the files. 
        /// </summary>
        /// <param name="velData">Velocity data accumulated.</param>
        /// <returns>All the MATLAB format byte array data.</returns>
        public VelDataEncode EncodeVelData(List<VelData> velData)
        {
            if(velData.Count < 0)
            {
                return new VelDataEncode();
            }

            int numElements = velData.First().EarthEast.Length;     // Number of bins
            int elementsMultiplier = velData.Count;                 // Number of ensembles
            VelDataEncode encodedData = new VelDataEncode();

            // Calculate the payload size
            int payloadSize = (numElements * elementsMultiplier * DataSet.Ensemble.BYTES_IN_FLOAT);

            // The size of the array is the header of the dataset
            // and the binxbeams value with each value being a float.
            encodedData.Beam0 = new byte[DataSet.BaseDataSet.GetBaseDataSize(VelDataEncode.BEAM_0_NAMELENGTH) + payloadSize];
            encodedData.Beam1 = new byte[DataSet.BaseDataSet.GetBaseDataSize(VelDataEncode.BEAM_1_NAMELENGTH) + payloadSize];
            encodedData.Beam2 = new byte[DataSet.BaseDataSet.GetBaseDataSize(VelDataEncode.BEAM_2_NAMELENGTH) + payloadSize];
            encodedData.Beam3 = new byte[DataSet.BaseDataSet.GetBaseDataSize(VelDataEncode.BEAM_3_NAMELENGTH) + payloadSize];
            encodedData.EarthEast = new byte[DataSet.BaseDataSet.GetBaseDataSize(VelDataEncode.EARTH_VEL_U_NAMELENGTH) + payloadSize];
            encodedData.EarthNorth = new byte[DataSet.BaseDataSet.GetBaseDataSize(VelDataEncode.EARTH_VEL_V_NAMELENGTH) + payloadSize];
            encodedData.EarthVert = new byte[DataSet.BaseDataSet.GetBaseDataSize(VelDataEncode.EARTH_VEL_Z_NAMELENGTH) + payloadSize];
            encodedData.EarthError = new byte[DataSet.BaseDataSet.GetBaseDataSize(VelDataEncode.EARTH_VEL_ERR_NAMELENGTH) + payloadSize];
            encodedData.Magnitude = new byte[DataSet.BaseDataSet.GetBaseDataSize(VelDataEncode.VEL_MAG_NAMELENGTH) + payloadSize];
            encodedData.Direction = new byte[DataSet.BaseDataSet.GetBaseDataSize(VelDataEncode.VEL_DIR_NAMELENGTH) + payloadSize];

            #region Header
            // Add the header to the byte array
            byte[] headerBeam0 = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, VelDataEncode.BEAM_0_NAMELENGTH, VelDataEncode.BEAM_0_NAME, numElements);
            System.Buffer.BlockCopy(headerBeam0, 0, encodedData.Beam0, 0, headerBeam0.Length);

            byte[] headerBeam1 = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, VelDataEncode.BEAM_1_NAMELENGTH, VelDataEncode.BEAM_1_NAME, numElements);
            System.Buffer.BlockCopy(headerBeam1, 0, encodedData.Beam1, 0, headerBeam1.Length);

            byte[] headerBeam2 = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, VelDataEncode.BEAM_2_NAMELENGTH, VelDataEncode.BEAM_2_NAME, numElements);
            System.Buffer.BlockCopy(headerBeam2, 0, encodedData.Beam2, 0, headerBeam2.Length);

            byte[] headerBeam3 = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, VelDataEncode.BEAM_3_NAMELENGTH, VelDataEncode.BEAM_3_NAME, numElements);
            System.Buffer.BlockCopy(headerBeam3, 0, encodedData.Beam3, 0, headerBeam0.Length);

            byte[] headerEast = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, VelDataEncode.EARTH_VEL_U_NAMELENGTH, VelDataEncode.EARTH_VEL_U_NAME, numElements);
            System.Buffer.BlockCopy(headerEast, 0, encodedData.EarthEast, 0, headerEast.Length);

            byte[] headerNorth = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, VelDataEncode.EARTH_VEL_V_NAMELENGTH, VelDataEncode.EARTH_VEL_V_NAME, numElements);
            System.Buffer.BlockCopy(headerNorth, 0, encodedData.EarthNorth, 0, headerNorth.Length);

            byte[] headerVert = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, VelDataEncode.EARTH_VEL_Z_NAMELENGTH, VelDataEncode.EARTH_VEL_Z_NAME, numElements);
            System.Buffer.BlockCopy(headerVert, 0, encodedData.EarthVert, 0, headerVert.Length);

            byte[] headerErr = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, VelDataEncode.EARTH_VEL_ERR_NAMELENGTH, VelDataEncode.EARTH_VEL_ERR_NAME, numElements);
            System.Buffer.BlockCopy(headerErr, 0, encodedData.EarthError, 0, headerErr.Length);

            byte[] headerMag = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, VelDataEncode.VEL_MAG_NAMELENGTH, VelDataEncode.VEL_MAG_NAME, numElements);
            System.Buffer.BlockCopy(headerMag, 0, encodedData.Magnitude, 0, headerMag.Length);

            byte[] headerDir = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, VelDataEncode.VEL_DIR_NAMELENGTH, VelDataEncode.VEL_DIR_NAME, numElements);
            System.Buffer.BlockCopy(headerDir, 0, encodedData.Direction, 0, headerDir.Length);

            #endregion

            #region Payload

            // Add the payload to the result
            int index = 0;
            for (int ens = 0; ens < elementsMultiplier; ens++)
            {
                for (int bin = 0; bin < numElements; bin++)
                {
                    // Get the index for the next element and add to the array
                    if (velData[ens].Beam0 != null)
                    {
                        index = DataSet.BaseDataSet.GetBinBeamIndexStatic(VelDataEncode.BEAM_0_NAMELENGTH, numElements, ens, bin);
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(velData[ens].Beam0[bin]), 0, encodedData.Beam0, index, DataSet.Ensemble.BYTES_IN_FLOAT);
                    }

                    if (velData[ens].Beam1 != null)
                    {
                        index = DataSet.BaseDataSet.GetBinBeamIndexStatic(VelDataEncode.BEAM_1_NAMELENGTH, numElements, ens, bin);
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(velData[ens].Beam1[bin]), 0, encodedData.Beam1, index, DataSet.Ensemble.BYTES_IN_FLOAT);
                    }

                    if (velData[ens].Beam2 != null)
                    {
                        index = DataSet.BaseDataSet.GetBinBeamIndexStatic(VelDataEncode.BEAM_2_NAMELENGTH, numElements, ens, bin);
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(velData[ens].Beam2[bin]), 0, encodedData.Beam2, index, DataSet.Ensemble.BYTES_IN_FLOAT);
                    }

                    if (velData[ens].Beam3 != null)
                    {
                        index = DataSet.BaseDataSet.GetBinBeamIndexStatic(VelDataEncode.BEAM_3_NAMELENGTH, numElements, ens, bin);
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(velData[ens].Beam3[bin]), 0, encodedData.Beam3, index, DataSet.Ensemble.BYTES_IN_FLOAT);
                    }

                    if (velData[ens].EarthEast != null)
                    {
                        index = DataSet.BaseDataSet.GetBinBeamIndexStatic(VelDataEncode.EARTH_VEL_U_NAMELENGTH, numElements, ens, bin);
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(velData[ens].EarthEast[bin]), 0, encodedData.EarthEast, index, DataSet.Ensemble.BYTES_IN_FLOAT);
                    }

                    if (velData[ens].EarthNorth != null)
                    {
                        index = DataSet.BaseDataSet.GetBinBeamIndexStatic(VelDataEncode.EARTH_VEL_V_NAMELENGTH, numElements, ens, bin);
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(velData[ens].EarthNorth[bin]), 0, encodedData.EarthNorth, index, DataSet.Ensemble.BYTES_IN_FLOAT);
                    }

                    if (velData[ens].EarthVert != null)
                    {
                        index = DataSet.BaseDataSet.GetBinBeamIndexStatic(VelDataEncode.EARTH_VEL_Z_NAMELENGTH, numElements, ens, bin);
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(velData[ens].EarthVert[bin]), 0, encodedData.EarthVert, index, DataSet.Ensemble.BYTES_IN_FLOAT);
                    }

                    if (velData[ens].EarthError != null)
                    {
                        index = DataSet.BaseDataSet.GetBinBeamIndexStatic(VelDataEncode.EARTH_VEL_ERR_NAMELENGTH, numElements, ens, bin);
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(velData[ens].EarthError[bin]), 0, encodedData.EarthError, index, DataSet.Ensemble.BYTES_IN_FLOAT);
                    }

                    if (velData[ens].Magnitude != null)
                    {
                        index = DataSet.BaseDataSet.GetBinBeamIndexStatic(VelDataEncode.VEL_MAG_NAMELENGTH, numElements, ens, bin);
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(velData[ens].Magnitude[bin]), 0, encodedData.Magnitude, index, DataSet.Ensemble.BYTES_IN_FLOAT);
                    }

                    if (velData[ens].Direction != null)
                    {
                        index = DataSet.BaseDataSet.GetBinBeamIndexStatic(VelDataEncode.VEL_DIR_NAMELENGTH, numElements, ens, bin);
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(velData[ens].Direction[bin]), 0, encodedData.Direction, index, DataSet.Ensemble.BYTES_IN_FLOAT);
                    }
                }
            }
            #endregion

            return encodedData;
        }

        /// <summary>
        /// Encode the Bottom Track data to byte arrays to write to the files. 
        /// </summary>
        /// <param name="btData">Bottom Track data accumulated.</param>
        /// <returns>All the MATLAB format byte array data.</returns>
        public BtDataEncode EncodeBtData(List<BtData> btData)
        {
            int numElements = 1;
            int elementsMultiplier = btData.Count;
            BtDataEncode encodedData = new BtDataEncode();

            // Calculate the payload size
            int payloadSize = (numElements * elementsMultiplier * DataSet.Ensemble.BYTES_IN_FLOAT);

            // The size of the array is the header of the dataset
            // and the binxbeams value with each value being a float.
            encodedData.Beam0 = new byte[DataSet.BaseDataSet.GetBaseDataSize(BtDataEncode.BEAM_0_NAMELENGTH) + payloadSize];
            encodedData.Beam1 = new byte[DataSet.BaseDataSet.GetBaseDataSize(BtDataEncode.BEAM_1_NAMELENGTH) + payloadSize];
            encodedData.Beam2 = new byte[DataSet.BaseDataSet.GetBaseDataSize(BtDataEncode.BEAM_2_NAMELENGTH) + payloadSize];
            encodedData.Beam3 = new byte[DataSet.BaseDataSet.GetBaseDataSize(BtDataEncode.BEAM_3_NAMELENGTH) + payloadSize];
            encodedData.EarthEast = new byte[DataSet.BaseDataSet.GetBaseDataSize(BtDataEncode.EARTH_EAST_NAMELENGTH) + payloadSize];
            encodedData.EarthNorth = new byte[DataSet.BaseDataSet.GetBaseDataSize(BtDataEncode.EARTH_NORTH_NAMELENGTH) + payloadSize];
            encodedData.EarthVertical = new byte[DataSet.BaseDataSet.GetBaseDataSize(BtDataEncode.EARTH_VERT_NAMELENGTH) + payloadSize];
            encodedData.EarthError = new byte[DataSet.BaseDataSet.GetBaseDataSize(BtDataEncode.EARTH_ERROR_NAMELENGTH) + payloadSize];
            encodedData.Range0 = new byte[DataSet.BaseDataSet.GetBaseDataSize(BtDataEncode.RANGE_0_NAMELENGTH) + payloadSize];
            encodedData.Range1 = new byte[DataSet.BaseDataSet.GetBaseDataSize(BtDataEncode.RANGE_1_NAMELENGTH) + payloadSize];
            encodedData.Range2 = new byte[DataSet.BaseDataSet.GetBaseDataSize(BtDataEncode.RANGE_2_NAMELENGTH) + payloadSize];
            encodedData.Range3 = new byte[DataSet.BaseDataSet.GetBaseDataSize(BtDataEncode.RANGE_3_NAMELENGTH) + payloadSize];
            encodedData.Status = new byte[DataSet.BaseDataSet.GetBaseDataSize(BtDataEncode.BT_STATUS_NAMELENGTH) + payloadSize];

            #region Header
            // Add the header to the byte array
            byte[] headerBeam0 = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, BtDataEncode.BEAM_0_NAMELENGTH, BtDataEncode.BEAM_0_NAME, numElements);
            System.Buffer.BlockCopy(headerBeam0, 0, encodedData.Beam0, 0, headerBeam0.Length);

            byte[] headerBeam1 = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, BtDataEncode.BEAM_1_NAMELENGTH, BtDataEncode.BEAM_1_NAME, numElements);
            System.Buffer.BlockCopy(headerBeam1, 0, encodedData.Beam1, 0, headerBeam1.Length);

            byte[] headerBeam2 = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, BtDataEncode.BEAM_2_NAMELENGTH, BtDataEncode.BEAM_2_NAME, numElements);
            System.Buffer.BlockCopy(headerBeam2, 0, encodedData.Beam2, 0, headerBeam2.Length);

            byte[] headerBeam3 = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, BtDataEncode.BEAM_3_NAMELENGTH, BtDataEncode.BEAM_3_NAME, numElements);
            System.Buffer.BlockCopy(headerBeam3, 0, encodedData.Beam3, 0, headerBeam0.Length);

            byte[] headerEast = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, BtDataEncode.EARTH_EAST_NAMELENGTH, BtDataEncode.EARTH_EAST_NAME, numElements);
            System.Buffer.BlockCopy(headerEast, 0, encodedData.EarthEast, 0, headerEast.Length);

            byte[] headerNorth = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, BtDataEncode.EARTH_NORTH_NAMELENGTH, BtDataEncode.EARTH_NORTH_NAME, numElements);
            System.Buffer.BlockCopy(headerNorth, 0, encodedData.EarthNorth, 0, headerNorth.Length);

            byte[] headerVert = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, BtDataEncode.EARTH_VERT_NAMELENGTH, BtDataEncode.EARTH_VERT_NAME, numElements);
            System.Buffer.BlockCopy(headerVert, 0, encodedData.EarthVertical, 0, headerVert.Length);

            byte[] headerErr = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, BtDataEncode.EARTH_ERROR_NAMELENGTH, BtDataEncode.EARTH_ERROR_NAME, numElements);
            System.Buffer.BlockCopy(headerErr, 0, encodedData.EarthError, 0, headerErr.Length);

            byte[] headerRange0 = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, BtDataEncode.RANGE_0_NAMELENGTH, BtDataEncode.RANGE_0_NAME, numElements);
            System.Buffer.BlockCopy(headerRange0, 0, encodedData.Range0, 0, headerRange0.Length);

            byte[] headerRange1 = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, BtDataEncode.RANGE_1_NAMELENGTH, BtDataEncode.RANGE_1_NAME, numElements);
            System.Buffer.BlockCopy(headerRange1, 0, encodedData.Range1, 0, headerRange1.Length);

            byte[] headerRange2 = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, BtDataEncode.RANGE_2_NAMELENGTH, BtDataEncode.RANGE_2_NAME, numElements);
            System.Buffer.BlockCopy(headerRange2, 0, encodedData.Range2, 0, headerRange2.Length);

            byte[] headerRange3 = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, BtDataEncode.RANGE_3_NAMELENGTH, BtDataEncode.RANGE_3_NAME, numElements);
            System.Buffer.BlockCopy(headerRange3, 0, encodedData.Range3, 0, headerRange3.Length);

            byte[] headerStatus = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, BtDataEncode.BT_STATUS_NAMELENGTH, BtDataEncode.BT_STATUS_NAME, numElements);
            System.Buffer.BlockCopy(headerStatus, 0, encodedData.Status, 0, headerStatus.Length);
            #endregion

            #region Payload
            // Add the payload to the results
            const int ELEMENT_INDEX = 0;
            int index = 0;
            for (int ens = 0; ens < numElements; ens++)
            {
                // Get the index for the next element and add to the array
                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(BtDataEncode.BEAM_0_NAMELENGTH, numElements, ens, ELEMENT_INDEX); 
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(btData[ens].Beam0), 0, encodedData.Beam0, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(BtDataEncode.BEAM_1_NAMELENGTH, numElements, ens, ELEMENT_INDEX);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(btData[ens].Beam1), 0, encodedData.Beam1, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(BtDataEncode.BEAM_2_NAMELENGTH, numElements, ens, ELEMENT_INDEX);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(btData[ens].Beam2), 0, encodedData.Beam2, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(BtDataEncode.BEAM_3_NAMELENGTH, numElements, ens, ELEMENT_INDEX);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(btData[ens].Beam3), 0, encodedData.Beam3, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(BtDataEncode.EARTH_EAST_NAMELENGTH, numElements, ens, ELEMENT_INDEX);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(btData[ens].EarthEast), 0, encodedData.EarthEast, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(BtDataEncode.EARTH_NORTH_NAMELENGTH, numElements, ens, ELEMENT_INDEX);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(btData[ens].EarthNorth), 0, encodedData.EarthNorth, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(BtDataEncode.EARTH_VERT_NAMELENGTH, numElements, ens, ELEMENT_INDEX);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(btData[ens].EarthVertical), 0, encodedData.EarthVertical, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(BtDataEncode.EARTH_ERROR_NAMELENGTH, numElements, ens, ELEMENT_INDEX);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(btData[ens].EarthError), 0, encodedData.EarthError, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(BtDataEncode.RANGE_0_NAMELENGTH, numElements, ens, ELEMENT_INDEX);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(btData[ens].Range0), 0, encodedData.Range0, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(BtDataEncode.RANGE_1_NAMELENGTH, numElements, ens, ELEMENT_INDEX);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(btData[ens].Range1), 0, encodedData.Range1, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(BtDataEncode.RANGE_2_NAMELENGTH, numElements, ens, ELEMENT_INDEX);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(btData[ens].Range2), 0, encodedData.Range2, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(BtDataEncode.RANGE_3_NAMELENGTH, numElements, ens, ELEMENT_INDEX);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(btData[ens].Range3), 0, encodedData.Range3, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(BtDataEncode.BT_STATUS_NAMELENGTH, numElements, ens, ELEMENT_INDEX);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(btData[ens].Status), 0, encodedData.Status, index, DataSet.Ensemble.BYTES_IN_FLOAT);
            }
            #endregion

            return encodedData;
        }

        /// <summary>
        /// Encode the NMEA data to byte arrays to write to the files. 
        /// </summary>
        /// <param name="nmeaData">NMEA data accumulated.</param>
        /// <returns>All the MATLAB format byte array data.</returns>
        public NmeaDataEncode EncodeNmeaData(List<NmeaData> nmeaData)
        {
            int numElements = NmeaDataEncode.NUM_ELEMENTS;
            int elementsMultiplier = nmeaData.Count;
            NmeaDataEncode encodedData = new NmeaDataEncode();

            // Calculate the payload size
            int payloadSize = (numElements * elementsMultiplier * DataSet.Ensemble.BYTES_IN_FLOAT);

            // The size of the array is the header of the dataset
            // and the binxbeams value with each value being a float.
            encodedData.NmeaData = new byte[DataSet.BaseDataSet.GetBaseDataSize(NmeaDataEncode.NMEA_NAMELENGTH) + payloadSize];

            #region Header

            // Add the header to the byte array
            byte[] header = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, NmeaDataEncode.NMEA_NAMELENGTH, NmeaDataEncode.NMEA_NAME, numElements);
            System.Buffer.BlockCopy(header, 0, encodedData.NmeaData, 0, header.Length);

            #endregion

            #region Payload

            // Add the payload to the results
            int index = 0;
            for (int ens = 0; ens < elementsMultiplier; ens++)
            {
                // Get the index for the next element and add to the array
                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(NmeaDataEncode.NMEA_NAMELENGTH, numElements, ens, 0);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(nmeaData[ens].Lat), 0, encodedData.NmeaData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(NmeaDataEncode.NMEA_NAMELENGTH, numElements, ens, 1);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(nmeaData[ens].Lon), 0, encodedData.NmeaData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(NmeaDataEncode.NMEA_NAMELENGTH, numElements, ens, 2);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(nmeaData[ens].Heading), 0, encodedData.NmeaData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(NmeaDataEncode.NMEA_NAMELENGTH, numElements, ens, 3);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(nmeaData[ens].Speed), 0, encodedData.NmeaData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

            }

            #endregion

            return encodedData;
        }

        /// <summary>
        /// Encode the ENS data to byte arrays to write to the files. 
        /// </summary>
        /// <param name="ensData">ENS data accumulated.</param>
        /// <returns>All the MATLAB format byte array data.</returns>
        public EnsDataEncode EncodeEnsData(List<EnsData> ensData)
        {
            int numElements = EnsDataEncode.NUM_ELEMENTS;
            int elementsMultiplier = ensData.Count;
            EnsDataEncode encodedData = new EnsDataEncode();

            // Calculate the payload size
            int payloadSize = (numElements * elementsMultiplier * DataSet.Ensemble.BYTES_IN_FLOAT);

            // The size of the array is the header of the dataset
            // and the binxbeams value with each value being a float.
            encodedData.EnsData = new byte[DataSet.BaseDataSet.GetBaseDataSize(EnsDataEncode.ENS_NAMELENGTH) + payloadSize];

            #region Header

            // Add the header to the byte array
            byte[] header = DataSet.BaseDataSet.GenerateHeader(DataSet.Ensemble.DATATYPE_FLOAT, elementsMultiplier, 0, EnsDataEncode.ENS_NAMELENGTH, EnsDataEncode.ENS_NAME, numElements);
            System.Buffer.BlockCopy(header, 0, encodedData.EnsData, 0, header.Length);

            #endregion

            #region Payload

            // Add the payload to the results
            int index = 0;
            for (int ens = 0; ens < elementsMultiplier; ens++)
            {
                // Get the index for the next element and add to the array
                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(EnsDataEncode.ENS_NAMELENGTH, numElements, ens, 0);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ensData[ens].Year), 0, encodedData.EnsData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(EnsDataEncode.ENS_NAMELENGTH, numElements, ens, 1);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ensData[ens].Month), 0, encodedData.EnsData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(EnsDataEncode.ENS_NAMELENGTH, numElements, ens, 2);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ensData[ens].Day), 0, encodedData.EnsData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(EnsDataEncode.ENS_NAMELENGTH, numElements, ens, 3);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ensData[ens].Hour), 0, encodedData.EnsData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(EnsDataEncode.ENS_NAMELENGTH, numElements, ens, 4);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ensData[ens].Minute), 0, encodedData.EnsData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(EnsDataEncode.ENS_NAMELENGTH, numElements, ens, 5);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ensData[ens].Second), 0, encodedData.EnsData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(EnsDataEncode.ENS_NAMELENGTH, numElements, ens, 6);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ensData[ens].HSecond), 0, encodedData.EnsData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(EnsDataEncode.ENS_NAMELENGTH, numElements, ens, 7);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ensData[ens].NumBeams), 0, encodedData.EnsData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(EnsDataEncode.ENS_NAMELENGTH, numElements, ens, 8);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ensData[ens].NumBins), 0, encodedData.EnsData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(EnsDataEncode.ENS_NAMELENGTH, numElements, ens, 9);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ensData[ens].Heading), 0, encodedData.EnsData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(EnsDataEncode.ENS_NAMELENGTH, numElements, ens, 10);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ensData[ens].Pitch), 0, encodedData.EnsData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(EnsDataEncode.ENS_NAMELENGTH, numElements, ens, 11);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ensData[ens].Roll), 0, encodedData.EnsData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

                index = DataSet.BaseDataSet.GetBinBeamIndexStatic(EnsDataEncode.ENS_NAMELENGTH, numElements, ens, 12);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ensData[ens].Status), 0, encodedData.EnsData, index, DataSet.Ensemble.BYTES_IN_FLOAT);

            }

            #endregion

            return encodedData;
        }
    }
}
