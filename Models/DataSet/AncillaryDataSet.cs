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
 * 10/05/2011      RC                     Added rounded ranges for displaying
 * 10/24/2011      RC                     Changed from receiving a DataTable to a DataRow.
 * 11/28/2011      RC                     Added decoding Prti01Sentence.
 * 11/29/2011      RC                     Added decoding Prti02Sentence.
 * 12/08/2011      RC          1.09       Fixed bug with setting ID for PRTI01/02 constructor.
 * 12/19/2011      RC          1.10       Added variable for bad Temp.  
 *                                         In SysTempRounded, if system temp is bad, do not display anything.
 *       
 * 
 */

using System;
using System.Data;

namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the Ancillary data.
        /// </summary>
        public class AncillaryDataSet : BaseDataSet
        {
            /// <summary>
            /// Number of elements within this data set
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 13;

            /// <summary>
            /// Value when the system temperature is bad.
            /// </summary>
            public const int BAD_SYS_TEMP = -99999;

            /// <summary>
            /// Range of the first Bin from the transducer in meters.
            /// </summary>
            public float FirstBinRange { get; private set; }

            /// <summary>
            /// Round version of the range of the first Bin from the transducer in meters.
            /// </summary>
            public string FirstBinRangeRounded { get { return FirstBinRange.ToString("0.000"); } }

            /// <summary>
            /// Size of a Bin in meters.
            /// </summary>
            public float BinSize { get; private set; }

            /// <summary>
            /// Round version of Size of a Bin in meters.
            /// </summary>
            public string BinSizeRounded { get { return BinSize.ToString("0.000"); } }

            /// <summary>
            /// First Profile Ping Time in seconds.
            /// </summary>
            public float FirstPingTime { get; private set; }

            /// <summary>
            /// Round version of First Profile ping time in seconds.
            /// </summary>
            public string FirstPingTimeRounded { get { return FirstPingTime.ToString("0.00"); } }

            /// <summary>
            /// Last Profile Ping Time in seconds.
            /// </summary>
            public float LastPingTime { get; private set; }

            /// <summary>
            /// Round version of Last Profile ping time in seconds
            /// </summary>
            public string LastPingTimeRounded { get { return LastPingTime.ToString("0.00"); ; } }

            /// <summary>
            /// Heading in degrees.
            /// </summary>
            public float Heading { get; private set; }

            /// <summary>
            /// Round version of Heading in degrees.
            /// </summary>
            public string HeadingRounded { get { return Heading.ToString("0.000"); } }

            /// <summary>
            /// Pitch in degrees.
            /// </summary>
            public float Pitch { get; private set; }

            /// <summary>
            /// Round version of Pitch in degrees.
            /// </summary>
            public string PitchRounded { get { return Pitch.ToString("0.000"); } }

            /// <summary>
            /// Roll in degrees.
            /// </summary>
            public float Roll { get; private set; }

            /// <summary>
            /// Round version of Roll in degrees.
            /// </summary>
            public string RollRounded { get { return Roll.ToString("0.000"); } }

            /// <summary>
            /// Water Temperature in degrees.  Used in Speed of Sound.
            /// </summary>
            public float WaterTemp { get; private set; }

            /// <summary>
            /// Round version of the Water temperature in degrees. 
            /// </summary>
            public string WaterTempRounded { get { return WaterTemp.ToString("0.000"); } }

            /// <summary>
            /// System (board) temperature in degrees.
            /// </summary>
            public float SystemTemp { get; private set; }

            /// <summary>
            /// Round version of the system temperature in degrees.
            /// If the temperature is bad, do not display anything.
            /// </summary>
            public string SystemTempRounded 
            { 
                get 
                {
                    if (SystemTemp == BAD_SYS_TEMP)
                    {
                        return "";
                    }

                    return SystemTemp.ToString("0.000"); 
                } 
            }

            /// <summary>
            /// Salinity in parts per thousand (ppt).
            /// Used in Speed of Sound.
            /// </summary>
            public float Salinity { get; private set; }

            /// <summary>
            /// Round version of Salinity in parts per thousand.
            /// </summary>
            public string SalinityRounded { get { return Salinity.ToString("0.000"); } }

            /// <summary>
            /// Pressure in Pascal.
            /// </summary>
            public float Pressure { get; private set; }

            /// <summary>
            /// Round version of Pressure in pascal.
            /// </summary>
            public string PressureRounded { get { return Pressure.ToString("0.000"); } }

            /// <summary>
            /// Depth of the transducer into the water in meters.
            /// Used in Speed of Sound.
            /// </summary>
            public float TransducerDepth { get; private set; }

            /// <summary>
            /// Round version of Depth of the tranducer into the water in meters.
            /// </summary>
            public string TransducerDepthRounded { get { return TransducerDepth.ToString("0.000"); } }

            /// <summary>
            /// Speed of Sound in meters/sec.
            /// </summary>
            public float SpeedOfSound { get; private set; }

            /// <summary>
            /// Round version of the Speed of Sound in meters/sec.
            /// </summary>
            public string SpeedOfSoundRounded { get { return SpeedOfSound.ToString("0.000"); } }

            /// <summary>
            /// Create a Ancillary data set.  Includes all the information
            /// about the current Ancillary data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="ancData">Byte array containing Ancillary data</param>
            public AncillaryDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] ancData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Decode the information
                DecodeAncillaryData(ancData);
            }

            /// <summary>
            /// Create a Ancillary data set.  Includes all the information
            /// about the current Ancillary data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="ancData">DataTable containing Ancillary data</param>
            public AncillaryDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataRow ancData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Decode the information
                DecodeAncillaryData(ancData);
            }

            /// <summary>
            /// Create a Ancillary data set.  Include all the information
            /// about the current Ancillary data from the sentence.
            /// This will set the temperature.
            /// </summary>
            /// <param name="sentence">Sentence containing data.</param>
            public AncillaryDataSet(Prti01Sentence sentence) :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.AncillaryID)
            {
                WaterTemp = Convert.ToSingle(sentence.Temperature / 100.0);   // Sentence stores temp at 1/100 degree Celcius.
            }

            /// <summary>
            /// Create a Ancillary data set.  Include all the information
            /// about the current Ancillary data from the sentence.
            /// This will set the temperature.
            /// </summary>
            /// <param name="sentence">Sentence containing data.</param>
            public AncillaryDataSet(Prti02Sentence sentence) :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.AncillaryID)
            {
                WaterTemp = Convert.ToSingle(sentence.Temperature / 100.0);   // Sentence stores temp at 1/100 degree Celcius.
            }

            /// <summary>
            /// Get all the information about the Ancillary data.
            /// </summary>
            /// <param name="data">Byte array containing the Ancillary data type.</param>
            private void DecodeAncillaryData(byte[] data)
            {
                FirstBinRange = Converters.ByteArrayToFloat(data, GenerateIndex(0));
                BinSize = Converters.ByteArrayToFloat(data, GenerateIndex(1));
                FirstPingTime = Converters.ByteArrayToFloat(data, GenerateIndex(2));
                LastPingTime = Converters.ByteArrayToFloat(data, GenerateIndex(3));
                Heading = Converters.ByteArrayToFloat(data, GenerateIndex(4));
                Pitch = Converters.ByteArrayToFloat(data, GenerateIndex(5));
                Roll = Converters.ByteArrayToFloat(data, GenerateIndex(6));
                WaterTemp = Converters.ByteArrayToFloat(data, GenerateIndex(7));
                SystemTemp = Converters.ByteArrayToFloat(data, GenerateIndex(8));
                Salinity = Converters.ByteArrayToFloat(data, GenerateIndex(9));
                Pressure = Converters.ByteArrayToFloat(data, GenerateIndex(10));
                TransducerDepth = Converters.ByteArrayToFloat(data, GenerateIndex(11));
                SpeedOfSound = Converters.ByteArrayToFloat(data, GenerateIndex(12));
            }

            /// <summary>
            /// Move pass the base data of the Ancillary.  Then based
            /// off the xAxis, move to the correct location.
            /// </summary>
            /// <param name="index">PlaybackIndex of the order of the data.</param>
            /// <returns>PlaybackIndex for the given xAxis.  Start of the data</returns>
            private int GenerateIndex(int index)
            {
                return GetBaseDataSize(NameLength) + (index * Ensemble.BYTES_IN_FLOAT);
            }

            /// <summary>
            /// Get all the information about the Ancillary data.
            /// </summary>
            /// <param name="dataRow">DataTable containing the Ancillary data type.</param>
            private void DecodeAncillaryData(DataRow dataRow)
            {
                // Go through the result settings the settings
                // If more than 1 result is found, return the first one found
                //foreach (DataRow r in dataTable.Rows)
                //{
                FirstBinRange = Convert.ToSingle(dataRow[DbCommon.COL_ENS_FIRST_BIN_RANGE].ToString());
                BinSize = Convert.ToSingle(dataRow[DbCommon.COL_ENS_BIN_SIZE].ToString());
                FirstPingTime = Convert.ToSingle(dataRow[DbCommon.COL_ENS_FIRST_PING_TIME].ToString());
                LastPingTime = Convert.ToSingle(dataRow[DbCommon.COL_ENS_LAST_PING_TIME].ToString());
                Heading = Convert.ToSingle(dataRow[DbCommon.COL_ENS_HEADING].ToString());
                Pitch = Convert.ToSingle(dataRow[DbCommon.COL_ENS_PITCH].ToString());
                Roll = Convert.ToSingle(dataRow[DbCommon.COL_ENS_ROLL].ToString());
                WaterTemp = Convert.ToSingle(dataRow[DbCommon.COL_ENS_TEMP_WATER].ToString());
                SystemTemp = Convert.ToSingle(dataRow[DbCommon.COL_ENS_TEMP_SYS].ToString());
                Salinity = Convert.ToSingle(dataRow[DbCommon.COL_ENS_SALINITY].ToString());
                Pressure = Convert.ToSingle(dataRow[DbCommon.COL_ENS_PRESSURE].ToString());
                TransducerDepth = Convert.ToSingle(dataRow[DbCommon.COL_ENS_XDCR_DEPTH].ToString());
                SpeedOfSound = Convert.ToSingle(dataRow[DbCommon.COL_ENS_SOS].ToString());
                //}
            }


            /// <summary>
            /// Override the ToString to return all the Ancillary data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string s = "";
                s += "First Bin TransducerDepth: " + FirstBinRange + "\n"; 
                s += "Bin Size:        " + BinSize + "\n";
                s += "First Ping:      " + FirstPingTime + "s \n";
                s += "Last Ping:       " + LastPingTime + "s\n";
                s += "Heading:         " + Heading + "\n";
                s += "Pitch:           " + Pitch + "\n"; 
                s += "Roll:            " + Roll + "\n";
                s += "Speed of Sound:  " + SpeedOfSound + "\n";
                s += "Water Temp:      " + WaterTemp + "\n";
                s += "Salinity:        " + Salinity + "\n"; 
                s += "TransducerDepth: " + TransducerDepth + "\n";
                s += "Pressure:        " + Pressure + "\n";
                s += "System Temp:     " + SystemTemp + "\n";

                return s;
            }
        }
    }
}