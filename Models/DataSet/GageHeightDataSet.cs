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
 * 03/09/2015      RC          3.0.3      Initial coding
 * 
 * 
 * 
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Dataset containing all the Gage Height data for surface tracking data for horizontal systems.
        /// </summary>
        [JsonConverter(typeof(GageHeightDataSetSerializer))]
        public class GageHeightDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// Number of elements in this data set.
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 14;

            #endregion

            #region Properties

            /// <summary>
            /// Status of the data.
            /// </summary>
            public Status Status { get; set; }

            /// <summary>
            /// Average Range in meters.
            /// </summary>
            public float AvgRange { get; set; }

            /// <summary>
            /// Standard Deviation in meters.
            /// </summary>
            public float StdDev { get; set; }

            /// <summary>
            /// Average signal to noise ratio in dB.
            /// </summary>
            public float AvgSN { get; set; }

            /// <summary>
            /// N
            /// </summary>
            public float N { get; set; }

            /// <summary>
            /// Salinity in PPT.
            /// </summary>
            public float Salinity { get; set; }

            /// <summary>
            /// Pressure in Pascal.
            /// </summary>
            public float Pressure { get; set; }

            /// <summary>
            /// Depth in meters.
            /// </summary>
            public float Depth { get; set; }

            /// <summary>
            /// Water temperature in degrees.
            /// </summary>
            public float WaterTemp { get; set; }

            /// <summary>
            /// System temperature in degrees.
            /// </summary>
            public float SystemTemp { get; set; }

            /// <summary>
            /// Speed of Sound in meters per second.
            /// </summary>
            public float SoS { get; set; }

            /// <summary>
            /// Heading in degrees.
            /// </summary>
            public float Heading { get; set; }

            /// <summary>
            /// Pitch in degrees.
            /// </summary>
            public float Pitch { get; set; }

            /// <summary>
            /// ARoll in degrees.
            /// </summary>
            public float Roll { get; set; }

            #endregion


            /// <summary>
            /// Create a Gage Height data set.  This will create an empty dataset.
            /// </summary>
            public GageHeightDataSet() :
                base(DataSet.Ensemble.DATATYPE_FLOAT,                       // Type of data stored (Float or Int)
                            NUM_DATA_ELEMENTS,                              // Number of data elements
                            1,                                              // Number of beams
                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                            DataSet.Ensemble.GageHeightID)
            {
                // Initialize arrays
                Init();
            }

            /// <summary>
            /// Create a Range Tracking data set.  This will create an empty dataset.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public GageHeightDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize arrays
                Init();
            }

            /// <summary>
            /// Create a Range Tracking data set.  Includes all the information
            /// about the current Profile Engineering data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="ghData">Byte array containing Range Tracking data</param>
            public GageHeightDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] ghData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize arrays
                Init();

                // Decode the information
                Decode(ghData);
            }

            /// <summary>
            /// Create an Gage Height data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.GageHeightDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 97ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.GageHeightDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.GageHeightDataSet}(json); 
            /// 
            /// To use this method for JSON you must have all the parameters match all the properties in this object.
            /// 
            /// </summary>
            /// <param name="ValueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="NumElements">Number of Elements in the dataset.</param>
            /// <param name="ElementsMultiplier">Items per Elements.</param>
            /// <param name="Imag"></param>
            /// <param name="NameLength">Length of name</param>
            /// <param name="Name">Name of data type</param>
            /// <param name="Status">Status.</param>
            /// <param name="AvgRange">Average Range.</param>
            /// <param name="StdDev">Standard Deivation.</param>
            /// <param name="AvgSN">Average Signal to Noise.</param>
            /// <param name="N">N.</param>
            /// <param name="Salinity">Salinity.</param>
            /// <param name="Pressure">Pressure.</param>
            /// <param name="Depth">Depth of the water.</param>
            /// <param name="WaterTemp">Water Temperature.</param>
            /// <param name="SystemTemp">System Temperature.</param>
            /// <param name="SoS">Speed of Sound.</param>
            /// <param name="Heading">Heading.</param>
            /// <param name="Pitch">Pitch.</param>
            /// <param name="Roll">Roll.</param>
            [JsonConstructor]
            public GageHeightDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name,
                Status Status, float AvgRange, float StdDev, float AvgSN, float N, float Salinity, float Pressure, float Depth,
                float WaterTemp, float SystemTemp, float SoS, float Heading, float Pitch, float Roll)
                : base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                this.Status = Status;
                this.AvgRange = AvgRange;
                this.StdDev = StdDev;
                this.AvgSN = AvgSN;
                this.N = N;
                this.Salinity = Salinity;
                this.Pressure = Pressure;
                this.Depth = Depth;
                this.WaterTemp = WaterTemp;
                this.SystemTemp = SystemTemp;
                this.SoS = SoS;
                this.Heading = Heading;
                this.Pitch = Pitch;
                this.Roll = Roll;
            }

            #region Init

            /// <summary>
            /// Initialize all the values for the Gage Height dataset.
            /// </summary>
            public void Init()
            {
                this.Status = new Status(0);
                this.AvgRange = 0.0f;
                this.StdDev = 0.0f;
                this.AvgSN = 0.0f;
                this.N = 0.0f;
                this.Salinity = 0.0f;
                this.Pressure = 0.0f;
                this.Depth = 0.0f;
                this.WaterTemp = 0.0f;
                this.SystemTemp = 0.0f;
                this.SoS = 0.0f;
                this.Heading = 0.0f;
                this.Pitch = 0.0f;
                this.Roll = 0.0f;
            }

            #endregion

            #region Decode / Encode

            /// <summary>
            /// Move pass the Base data of the Gage Height.  Then based
            /// off the data, move to the correct location.
            /// </summary>
            /// <param name="index">Index of the order of the data.</param>
            /// <returns>Index for the given data.  Start of the data</returns>
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
            /// Get all the information about the Gage Height data.
            /// </summary>
            /// <param name="data">Byte array containing the Gage Height data type.</param>
            private void Decode(byte[] data)
            {

                Status = new Status((int)MathHelper.ByteArrayToFloat(data, GenerateIndex(0)));
                AvgRange = MathHelper.ByteArrayToFloat(data, GenerateIndex(1));
                StdDev = MathHelper.ByteArrayToFloat(data, GenerateIndex(2));
                AvgSN = MathHelper.ByteArrayToFloat(data, GenerateIndex(3));
                N = MathHelper.ByteArrayToFloat(data, GenerateIndex(4));
                Salinity = MathHelper.ByteArrayToFloat(data, GenerateIndex(5));
                Pressure = MathHelper.ByteArrayToFloat(data, GenerateIndex(6));
                Depth = MathHelper.ByteArrayToFloat(data, GenerateIndex(7));
                WaterTemp = MathHelper.ByteArrayToFloat(data, GenerateIndex(8));
                SystemTemp = MathHelper.ByteArrayToFloat(data, GenerateIndex(9));
                SoS = MathHelper.ByteArrayToFloat(data, GenerateIndex(10));
                Heading = MathHelper.ByteArrayToFloat(data, GenerateIndex(11));
                Pitch = MathHelper.ByteArrayToFloat(data, GenerateIndex(12));
                Roll = MathHelper.ByteArrayToFloat(data, GenerateIndex(13));
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

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Status.Value), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AvgRange), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(StdDev), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AvgSN), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(N), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Salinity), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Pressure), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Depth), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(WaterTemp), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SystemTemp), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SoS), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Heading), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Pitch), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Roll), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

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

            #endregion

            #region Override

            /// <summary>
            /// Override the ToString to return all the Gage Height data as a string.
            /// </summary>
            /// <returns>String of this object.</returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Status: " + Status);
                sb.Append(" AvgRange: " + AvgRange);
                sb.Append(" StdDev: " + StdDev);
                sb.Append(" AvgSN: " + AvgSN);
                sb.Append(" N: " + N);
                sb.Append(" Salinity: " + Salinity);
                sb.Append(" Pressure: " + Pressure);
                sb.Append(" Depth: " + Depth);
                sb.Append(" WaterTemp: " + WaterTemp);
                sb.Append(" SystemTemp: " + SystemTemp);
                sb.Append(" SoS: " + SoS);
                sb.Append(" Heading: " + Heading);
                sb.Append(" Pitch: " + Pitch);
                sb.Append(" Roll: " + Roll);

                return sb.ToString();
            }

            #endregion
        }

        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.GageHeightData).
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
        public class GageHeightDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.GageHeightData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as GageHeightDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // Status
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_STATUS);
                writer.WriteValue(data.Status);

                // AvgRange
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_AVGRANGE);
                writer.WriteValue(data.AvgRange);

                // StdDev
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_SD);
                writer.WriteValue(data.StdDev);

                // AvgSN
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_AVGSN);
                writer.WriteValue(data.AvgSN);

                // N
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_N);
                writer.WriteValue(data.N);

                // Salinity
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_SALINITY);
                writer.WriteValue(data.Salinity);

                // Pressure
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_PRESSURE);
                writer.WriteValue(data.Pressure);

                // Depth
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_DEPTH);
                writer.WriteValue(data.Depth);

                // WaterTemp
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_WATERTEMP);
                writer.WriteValue(data.WaterTemp);

                // SystemTemp
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_SYSTEMTEMP);
                writer.WriteValue(data.SystemTemp);

                // SoS
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_SOS);
                writer.WriteValue(data.SoS);

                // Heading
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_HEADING);
                writer.WriteValue(data.Heading);

                // Pitch
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_PITCH);
                writer.WriteValue(data.Pitch);

                // Roll
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_GH_ROLL);
                writer.WriteValue(data.Roll);

                // End the object
                writer.WriteEndObject();
            }

            /// <summary>
            /// Read the JSON object and convert to the object.  This will allow the serializer to
            /// automatically convert the object.  No special instructions need to be done and all
            /// the properties found in the JSON string need to be used.
            /// 
            /// DataSet.GageHeightDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.GageHeightDataSet}(encodedEns)
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
                    var data = new GageHeightDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.GageHeightID);

                    // Status
                    data.Status = new Status((int)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_STATUS]);

                    // AvgRange
                    data.AvgRange = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_AVGRANGE];

                    // StdDev
                    data.StdDev = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_SD];

                    // AvgSN
                    data.AvgSN = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_AVGSN];

                    // N
                    data.N = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_N];

                    // Salinity
                    data.Salinity = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_SALINITY];

                    // Pressure
                    data.Pressure = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_PRESSURE];

                    // Depth
                    data.Depth = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_DEPTH];

                    // WaterTemp
                    data.WaterTemp = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_WATERTEMP];

                    // SystemTemp
                    data.SystemTemp = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_SYSTEMTEMP];

                    // SoS
                    data.SoS = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_SOS];

                    // Heading
                    data.Heading = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_HEADING];

                    // Pitch
                    data.Pitch = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_PITCH];

                    // Roll
                    data.Roll = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_GH_ROLL];

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
                return typeof(ProfileEngineeringDataSet).IsAssignableFrom(objectType);
            }

        }

    }
}
