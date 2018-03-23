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
 * 03/30/2012      RC          2.07       Moved Converters.cs methods to MathHelper.cs.
 * 01/04/2013      RC          2.17       Created a constructor that take no data.
 * 02/21/2013      RC          2.18       For PRTI messages, made LastPingTime the PRTI start time.
 * 02/25/2013      RC          2.18       Added JSON encoding and Decoding.
 * 02/26/2013      RC          2.18       Fixed FirstPingTime for PRTI sentences.
 * 02/06/2014      RC          2.21.3     Added constructor that takes a PRTI03 sentence.
 * 03/26/2014      RC          2.21.4     Added a simpler constructor and added DecodePd0Ensemble().
 * 07/28/2014      RC          2.23.0     Fixed a bug setting the ElementMulitplier and NumElements.
 * 02/07/2018      RC          3.4.5      Added IsUpwardFacing() to know the direction the ADCP is facing.
 * 
 */

using System;
using System.Data;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the Ancillary data.
        /// </summary>
        [JsonConverter(typeof(AncillaryDataSetSerializer))]
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
            /// Water Temperature in degrees farenheit.  Used in Speed of Sound.
            /// </summary>
            public float WaterTemp { get; set; }

            /// <summary>
            /// System (board) temperature in degrees farenheit.
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
            public AncillaryDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Init the values
                Init();
            }

            /// <summary>
            /// Create a Ancillary data set.  Includes all the information
            /// about the current Ancillary data.
            /// </summary>
            public AncillaryDataSet() :
                base(DataSet.Ensemble.DATATYPE_FLOAT,                       // Type of data stored (Float or Int)
                            NUM_DATA_ELEMENTS,                              // Number of Elements
                            DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM,     // Element Multiplier
                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                            DataSet.Ensemble.AncillaryID)                   // Dataset ID
            {
                // Init the values
                Init();
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
            /// <param name="ancData">Byte array containing Ancillary data</param>
            public AncillaryDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] ancData) :
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
                // Init the values
                Init();

                WaterTemp = Convert.ToSingle(sentence.Temperature / 100.0);     // Sentence stores temp at 1/100 degree Celcius.
                LastPingTime = sentence.StartTime / 100;                        // Convert the Start Time for hundreds of seconds to seconds.
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
                // Init the values
                Init();

                WaterTemp = Convert.ToSingle(sentence.Temperature / 100.0);   // Sentence stores temp at 1/100 degree Celcius.
                LastPingTime = sentence.StartTime / 100;                        // Convert the Start Time for hundreds of seconds to seconds.
            }

            /// <summary>
            /// Create a Ancillary data set.  Include all the information
            /// about the current Ancillary data from the sentence.
            /// This will set the temperature.
            /// </summary>
            /// <param name="sentence">Sentence containing data.</param>
            public AncillaryDataSet(Prti03Sentence sentence) :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.AncillaryID)
            {
                // Init the values
                Init();

                WaterTemp = Convert.ToSingle(sentence.Temperature / 100.0);     // Sentence stores temp at 1/100 degree Celcius.
                LastPingTime = sentence.StartTime / 100;                        // Convert the Start Time for hundreds of seconds to seconds.
            }

            /// <summary>
            /// Create an Ancillary data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.AncillaryDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 97ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.AncillaryDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.AncillaryDataSet}(json); 
            /// 
            /// To use this method for JSON you must have all the parameters match all the properties in this object.
            /// 
            /// </summary>
            /// <param name="ValueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="NumElements">Number of Bin</param>
            /// <param name="ElementsMultiplier">Number of beams</param>
            /// <param name="Imag"></param>
            /// <param name="NameLength">Length of name</param>
            /// <param name="Name">Name of data type</param>
            /// <param name="FirstBinRange">First Bin Range.</param>
            /// <param name="BinSize">Bin Size in meters.</param>
            /// <param name="FirstPingTime">Time of first ping in seconds</param>
            /// <param name="LastPingTime">Time of last ping in seconds.</param>
            /// <param name="Heading">Heading in degrees.</param>
            /// <param name="Pitch">Pitch in degrees.</param>
            /// <param name="Roll">Roll in degrees.</param>
            /// <param name="WaterTemp">Water Temperature in degrees farenheit.</param>
            /// <param name="SystemTemp">System Temperature in degrees farenheit.</param>
            /// <param name="Salinity">Salinity of the water in PPM.</param>
            /// <param name="Pressure">Pressure read by the pressure sensor in Pascals.</param>
            /// <param name="TransducerDepth">Depth of the transducer in meters.</param>
            /// <param name="SpeedOfSound">Speed of Sound measured in m/s.</param>
            [JsonConstructor]
            private AncillaryDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name,
                        float FirstBinRange, float BinSize, float FirstPingTime, float LastPingTime,
                        float Heading, float Pitch, float Roll, float WaterTemp, float SystemTemp,
                        float Salinity, float Pressure, float TransducerDepth, float SpeedOfSound) :
                base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                // Initialize data
                this.FirstBinRange = FirstBinRange;
                this.BinSize = BinSize;
                this.FirstPingTime = FirstPingTime;
                this.LastPingTime = LastPingTime;
                this.Heading = Heading;
                this.Pitch = Pitch;
                this.Roll = Roll;
                this.WaterTemp = WaterTemp;
                this.SystemTemp = SystemTemp;
                this.Salinity = Salinity;
                this.Pressure = Pressure;
                this.TransducerDepth = TransducerDepth;
                this.SpeedOfSound = SpeedOfSound;
            }

            /// <summary>
            /// Add additional data to the dataset.
            /// This will add the heading, pitch and roll
            /// from the NMEA sentence.
            /// </summary>
            /// <param name="sentence">Sentence containing data.</param>
            public void AddAncillaryData(Prti30Sentence sentence)
            {
                Heading = sentence.Heading;
                Pitch = sentence.Pitch;
                Roll = sentence.Roll;
            }

            /// <summary>
            /// Add additional data to the dataset.
            /// This will add the heading, pitch and roll
            /// from the NMEA sentence.
            /// </summary>
            /// <param name="sentence">Sentence containing data.</param>
            public void AddAncillaryData(Prti31Sentence sentence)
            {
                Heading = sentence.Heading;
                Pitch = sentence.Pitch;
                Roll = sentence.Roll;
            }

            /// <summary>
            /// Initialize the value.
            /// </summary>
            private void Init()
            {
                FirstBinRange = 0.0f;
                BinSize = 0.0f;
                FirstPingTime = 0.0f;
                LastPingTime = 0.0f;
                Heading = 0.0f;
                Pitch = 0.0f;
                Roll = 0.0f;
                WaterTemp = 0.0f;
                SystemTemp = 0.0f;
                Salinity = 0.0f;
                Pressure = 0.0f;
                TransducerDepth = 0.0f;
                SpeedOfSound = 0.0f;
            }

            /// <summary>
            /// Get all the information about the Ancillary data.
            /// </summary>
            /// <param name="data">Byte array containing the Ancillary data type.</param>
            private void Decode(byte[] data)
            {
                FirstBinRange = MathHelper.ByteArrayToFloat(data, GenerateIndex(0));
                BinSize = MathHelper.ByteArrayToFloat(data, GenerateIndex(1));
                FirstPingTime = MathHelper.ByteArrayToFloat(data, GenerateIndex(2));
                LastPingTime = MathHelper.ByteArrayToFloat(data, GenerateIndex(3));
                Heading = MathHelper.ByteArrayToFloat(data, GenerateIndex(4));
                Pitch = MathHelper.ByteArrayToFloat(data, GenerateIndex(5));
                Roll = MathHelper.ByteArrayToFloat(data, GenerateIndex(6));
                WaterTemp = MathHelper.ByteArrayToFloat(data, GenerateIndex(7));
                SystemTemp = MathHelper.ByteArrayToFloat(data, GenerateIndex(8));
                Salinity = MathHelper.ByteArrayToFloat(data, GenerateIndex(9));
                Pressure = MathHelper.ByteArrayToFloat(data, GenerateIndex(10));
                TransducerDepth = MathHelper.ByteArrayToFloat(data, GenerateIndex(11));
                SpeedOfSound = MathHelper.ByteArrayToFloat(data, GenerateIndex(12));
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
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(FirstBinRange), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(BinSize), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(FirstPingTime), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(LastPingTime), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Heading), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Pitch), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Roll), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(WaterTemp), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SystemTemp), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Salinity), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Pressure), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(TransducerDepth), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SpeedOfSound), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                // Generate header for the dataset
                byte[] header = this.GenerateHeader(NUM_DATA_ELEMENTS);

                // Create the array to hold the dataset
                byte[] result = new byte[payload.Length + header.Length];

                // Copy the header to the array
                System.Buffer.BlockCopy(header, 0, result, 0, header.Length);

                // Copy the payload data to the array
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
            /// Check if the ADCP is upward facing or downward facing.
            /// 
            /// The ADCP is Upward facing if the Roll is around 0 degrees.
            /// The ADCP is Downward facing if the Roll is around 180 degrees.
            /// </summary>
            /// <returns></returns>
            public bool IsUpwardFacing()
            {
                float roll = Math.Abs(Roll);

                if(roll > 0 && roll < 30)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Get the depth for a specific bin.
            /// Assume that the first bin is bin 0.
            /// </summary>
            /// <param name="binNum">Bin number to get the depth.</param>
            /// <returns>Depth for a specific bin.</returns>
            public float GetBinToDepth(int binNum)
            {
                if(binNum >= 0)
                {
                    return (this.FirstBinRange + (binNum * this.BinSize));
                }

                return 0;
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

            #region PD0 Ensemble

            /// <summary>
            /// Convert the PD0 Fixed Leader and Variable Leader data type to the RTI Ancillary data set.
            /// </summary>
            /// <param name="fl">PD0 Fixed Leader.</param>
            /// <param name="vl">PD0 Variable Leader.</param>
            public void DecodePd0Ensemble(Pd0FixedLeader fl, Pd0VariableLeader vl)
            {
                // Get the time
                TimeSpan ts = new TimeSpan((int)vl.RtcY2kDay, (int)vl.RtcY2kHour, (int)vl.RtcY2kMinute, (int)vl.RtcY2kSecond);

                // Initialize data
                this.FirstBinRange = fl.Bin1Distance / 100.0f; ;
                this.BinSize = fl.DepthCellLength / 100.0f;
                this.FirstPingTime = (float)ts.TotalSeconds;
                this.LastPingTime = FirstPingTime;
                this.Heading = vl.Heading;
                this.Pitch = vl.Pitch;
                this.Roll = vl.Roll;
                this.WaterTemp = vl.Temperature;
                this.SystemTemp = 0.0f;
                this.Salinity = vl.Salinity;
                this.Pressure = vl.Pressure / 0.0001f;
                this.TransducerDepth = vl.DepthOfTransducer / 10.0f;
                this.SpeedOfSound = 0.0f;
            }

            #endregion
        }

        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.AncillaryData).
        /// 
        /// 50ms for this method.
        /// 100ms for calling SerializeObject default.
        /// 
        /// Use this method whenever possible to convert to JSON.
        /// 
        /// http://james.newtonking.com/projects/json/help/
        /// http://james.newtonking.com/projects/json/help/index.html?topic=html/ReadingWritingJSON.htm
        /// http://blog.maskalik.com/asp-net/json-net-implement-custom-serialization
        /// </summary>
        public class AncillaryDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.AncillaryData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as AncillaryDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // FirstBinRange
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_FIRSTBINRANGE);
                writer.WriteValue(data.FirstBinRange);

                // BinSize
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BINSIZE);
                writer.WriteValue(data.BinSize);

                // FirstPingTime
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_FIRSTPINGTIME);
                writer.WriteValue(data.FirstPingTime);

                // LastPingTime
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_LASTPINGTIME);
                writer.WriteValue(data.LastPingTime);

                // Heading
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_HEADING);
                writer.WriteValue(data.Heading);

                // Pitch
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PITCH);
                writer.WriteValue(data.Pitch);

                // Roll
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_ROLL);
                writer.WriteValue(data.Roll);

                // WaterTemp
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WATERTEMP);
                writer.WriteValue(data.WaterTemp);

                // SystemTemp
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SYSTEMP);
                writer.WriteValue(data.SystemTemp);

                // Salinity
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SALINITY);
                writer.WriteValue(data.Salinity);

                // Pressure
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PRESSURE);
                writer.WriteValue(data.Pressure);

                // TransducerDepth
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_TRANSDUCERDEPTH);
                writer.WriteValue(data.TransducerDepth);

                // SpeedOfSound
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SPEEDOFSOUND);
                writer.WriteValue(data.SpeedOfSound);

                // End the object
                writer.WriteEndObject();
            }

            /// <summary>
            /// Read the JSON object and convert to the object.  This will allow the serializer to
            /// automatically convert the object.  No special instructions need to be done and all
            /// the properties found in the JSON string need to be used.
            /// 
            /// DataSet.AncillaryDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.AncillaryDataSet}(encodedEns)
            /// 
            /// </summary>
            /// <param name="reader">NOT USED. JSON reader.</param>
            /// <param name="objectType">NOT USED> Type of object.</param>
            /// <param name="existingValue">NOT USED.</param>
            /// <param name="serializer">Serialize the object.</param>
            /// <returns>Serialized object.</returns>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.Null)
                {
                    // Load the object
                    JObject jsonObject = JObject.Load(reader);

                    // Decode the data
                    int NumElements = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_NUMELEMENTS];
                    int ElementsMultiplier = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_ELEMENTSMULTIPLIER];

                    // Create the object
                    var data = new AncillaryDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.AncillaryID);

                    // FirstBinRange
                    data.FirstBinRange = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_FIRSTBINRANGE];

                    // BinSize
                    data.BinSize = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_BINSIZE];

                    // FirstPingTime
                    data.FirstPingTime = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_FIRSTPINGTIME];

                    // LastPingTime
                    data.LastPingTime = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_LASTPINGTIME];

                    // Heading
                    data.Heading = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_HEADING];

                    // Pitch
                    data.Pitch = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PITCH];

                    // Roll
                    data.Roll = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_ROLL];

                    // WaterTemp
                    data.WaterTemp = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_WATERTEMP];

                    // SystemTemp
                    data.SystemTemp = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SYSTEMP];

                    // Salinity
                    data.Salinity = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SALINITY];

                    // Pressure
                    data.Pressure = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PRESSURE];

                    // TransducerDepth
                    data.TransducerDepth = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_TRANSDUCERDEPTH];

                    // SpeedOfSound
                    data.SpeedOfSound = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SPEEDOFSOUND];

                    return data;
                }

                return null;
            }

            /// <summary>
            /// Check if the given object is the correct type.
            /// </summary>
            /// <param name="objectType">Object to convert.</param>
            /// <returns>TRUE = object given is the correct type.</returns>
            public override bool CanConvert(Type objectType)
            {
                return typeof(AncillaryDataSet).IsAssignableFrom(objectType);
            }
        }
    }
}