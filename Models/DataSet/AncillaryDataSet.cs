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
 * 10/05/2011      RC                     Added rounded ranges for displaying
 * 10/24/2011      RC                     Changed from receiving a DataTable to a DataRow.
 * 11/28/2011      RC                     Added decoding Prti01Sentence.
 * 11/29/2011      RC                     Added decoding Prti02Sentence.
 * 12/08/2011      RC          1.09       Fixed bug with setting ID for PRTI01/02 constructor.
 * 12/19/2011      RC          1.10       Added variable for bad Temp.  
 *                                         In SysTempRounded, if system temp is bad, do not display anything.
 * 01/18/2012      RC          1.14       Added Encode() to convert to byte array.
 *                                         Removed "private set".
 * 01/24/2012      RC          1.14       Removed Rounded properties to methods to reduce memory footprint.
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
            #region Variables

            /// <summary>
            /// Number of elements within this data set
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 13;

            /// <summary>
            /// Value when the system temperature is bad.
            /// </summary>
            public const int BAD_SYS_TEMP = -99999;

            #endregion

            #region Properties

            /// <summary>
            /// Range of the first Bin from the transducer in meters.
            /// (Blank)
            /// </summary>
            public float FirstBinRange { get; set; }

            /// <summary>
            /// Size of a Bin in meters.
            /// </summary>
            public float BinSize { get; set; }

            /// <summary>
            /// First Profile Ping Time in seconds.
            /// </summary>
            public float FirstPingTime { get; set; }

            /// <summary>
            /// Last Profile Ping Time in seconds.
            /// </summary>
            public float LastPingTime { get; set; }

            /// <summary>
            /// Heading in degrees.
            /// </summary>
            public float Heading { get; set; }

            /// <summary>
            /// Pitch in degrees.
            /// </summary>
            public float Pitch { get; set; }

            /// <summary>
            /// Roll in degrees.
            /// </summary>
            public float Roll { get; set; }

            /// <summary>
            /// Water Temperature in degrees.  Used in Speed of Sound.
            /// </summary>
            public float WaterTemp { get; set; }

            /// <summary>
            /// System (board) temperature in degrees.
            /// </summary>
            public float SystemTemp { get; set; }

            /// <summary>
            /// Salinity in parts per thousand (ppt).
            /// Used in Speed of Sound.
            /// </summary>
            public float Salinity { get; set; }

            /// <summary>
            /// Pressure in Pascal.
            /// </summary>
            public float Pressure { get; set; }

            /// <summary>
            /// Depth of the transducer into the water in meters.
            /// Used in Speed of Sound.
            /// </summary>
            public float TransducerDepth { get; set; }

            /// <summary>
            /// Speed of Sound in meters/sec.
            /// </summary>
            public float SpeedOfSound { get; set; }

            #endregion

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
                Decode(ancData);
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
                Decode(ancData);
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
            private void Decode(byte[] data)
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
            /// Generate a byte array representing the
            /// dataset.  The byte array is in the binary format.
            /// The format can be found in the RTI ADCP User Guide.
            /// It contains a header and payload.  This byte array 
            /// will be combined with the other dataset byte arrays
            /// to form an ensemble.
            /// </summary>
            /// <returns>Byte array of the ensemble.</returns>
            public byte[] Encode()
            {
                int index = 0;

                // Get the length
                byte[] payload = new byte[NUM_DATA_ELEMENTS * Ensemble.BYTES_IN_FLOAT];
                System.Buffer.BlockCopy(Converters.FloatToByteArray(FirstBinRange), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(BinSize), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(FirstPingTime), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(LastPingTime), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Heading), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Pitch), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Roll), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(WaterTemp), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(SystemTemp), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Salinity), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Pressure), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(TransducerDepth), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(SpeedOfSound), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                // Generate header for the dataset
                byte[] header = this.GenerateHeader(NumElements);

                // Create the array to hold the dataset
                byte[] result = new byte[payload.Length + header.Length];

                // Copy the header to the array
                System.Buffer.BlockCopy(header, 0, result, 0, header.Length);

                // Copy the Nmea data to the array
                System.Buffer.BlockCopy(payload, 0, result, header.Length, payload.Length);

                return result;
            }

            /// <summary>
            /// Move pass the base data of the Ancillary.  Then based
            /// off the index, move to the correct location.
            /// </summary>
            /// <param name="index">PlaybackIndex of the order of the data.</param>
            /// <returns>PlaybackIndex for the given xAxis.  Start of the data</returns>
            private int GenerateIndex(int index)
            {
                return GetBaseDataSize(NameLength) + (index * Ensemble.BYTES_IN_FLOAT);
            }

            /// <summary>
            /// Generate a index within a payload byte array.
            /// </summary>
            /// <param name="index">Element number within the payload.</param>
            /// <returns>Start location within the payload.</returns>
            private int GeneratePayloadIndex(int index)
            {
                return index * Ensemble.BYTES_IN_FLOAT;
            }

            /// <summary>
            /// Get all the information about the Ancillary data.
            /// </summary>
            /// <param name="dataRow">DataTable containing the Ancillary data type.</param>
            private void Decode(DataRow dataRow)
            {
                // Go through the result settings the settings
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