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
 * 10/31/2014      RC          3.0.2      Initial coding
 * 12/19/2014      RC          3.0.2      Added NumBeams.
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
        /// Dataset containing all the Range Tracking for surface tracking data.
        /// </summary>
        [JsonConverter(typeof(RangeTrackingDataSetSerializer))]
        public class RangeTrackingDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// Number of elements in this data set.
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 13;

            #endregion

            #region Properties

            /// <summary>
            /// SNR values for each beam.
            /// </summary>
            public float[] SNR { get; set; }

            /// <summary>
            /// Range values for each beam.
            /// </summary>
            public float[] Range { get; set; }

            /// <summary>
            /// Number of pings averaged together for each beam.
            /// </summary>
            public float[] Pings { get; set; }

            /// <summary>
            /// Number of beams in the data.
            /// If the number of beams is 1, then the arrays will only contain
            /// 1 value for each property.
            /// </summary>
            public float NumBeams { get; set; }

            #endregion

            /// <summary>
            /// Create a Range Tracking data set.  This will create an empty dataset.
            /// </summary>
            public RangeTrackingDataSet(int numBeams) :
                base(DataSet.Ensemble.DATATYPE_FLOAT,                       // Type of data stored (Float or Int)
                            NUM_DATA_ELEMENTS,                              // Number of data elements
                            numBeams,                                       // Number of beams
                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                            DataSet.Ensemble.RangeTrackingID)
            {
                // Initialize arrays
                Init(numBeams);
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
            public RangeTrackingDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize arrays
                Init(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM);
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
            /// <param name="rtData">Byte array containing Range Tracking data</param>
            public RangeTrackingDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] rtData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize arrays
                Init(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM);

                // Decode the information
                Decode(rtData);
            }

            /// <summary>
            /// Create an Range Tracking data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.RangeTrackingDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 97ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.RangeTrackingDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.RangeTrackingDataSet}(json); 
            /// 
            /// To use this method for JSON you must have all the parameters match all the properties in this object.
            /// 
            /// </summary>
            /// <param name="ValueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="NumElements">Number of Elements in the dataset.</param>
            /// <param name="ElementsMultiplier">Items per Elements.</param>
            /// <param name="Imag"></param>
            /// <param name="NameLength">Length of name</param>
            /// <param name="NumBeams">Number of beams.</param>
            /// <param name="Name">Name of data type</param>
            /// <param name="SNR">Singal to Noise Ratio.</param>
            /// <param name="Range">Range.</param>
            /// <param name="Pings">Number of pings.</param>
            [JsonConstructor]
            public RangeTrackingDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name,
                int NumBeams, float[] SNR, float[] Range, float[] Pings)
                : base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                this.NumBeams = NumBeams;
                this.SNR = SNR;
                this.Range = Range;
                this.Pings = Pings;

            }

            #region Init

            /// <summary>
            /// Initialize all the arrays for the Range Tracking dataset.
            /// This will create the arrays based off the number of beams
            /// given.
            /// </summary>
            /// <param name="numBeams">Number of beams.</param>
            public void Init(int numBeams)
            {
                this.NumBeams = numBeams;
                SNR = new float[numBeams];
                Range = new float[numBeams];
                Pings = new float[numBeams];
            }

            #endregion

            #region Decode / Encode

            /// <summary>
            /// Move pass the Base data of the Range Tracking.  Then based
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
            /// Get all the information about the Range Tracking data.
            /// </summary>
            /// <param name="data">Byte array containing the Range Tracking data type.</param>
            private void Decode(byte[] data)
            {
                
                NumBeams = MathHelper.ByteArrayToFloat(data, GenerateIndex(0));

                if (NumBeams == 4.0f)
                {
                    SNR[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(1));
                    SNR[1] = MathHelper.ByteArrayToFloat(data, GenerateIndex(2));
                    SNR[2] = MathHelper.ByteArrayToFloat(data, GenerateIndex(3));
                    SNR[3] = MathHelper.ByteArrayToFloat(data, GenerateIndex(4));

                    Range[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(5));
                    Range[1] = MathHelper.ByteArrayToFloat(data, GenerateIndex(6));
                    Range[2] = MathHelper.ByteArrayToFloat(data, GenerateIndex(7));
                    Range[3] = MathHelper.ByteArrayToFloat(data, GenerateIndex(8));

                    Pings[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(9));
                    Pings[1] = MathHelper.ByteArrayToFloat(data, GenerateIndex(10));
                    Pings[2] = MathHelper.ByteArrayToFloat(data, GenerateIndex(11));
                    Pings[3] = MathHelper.ByteArrayToFloat(data, GenerateIndex(12));
                }
                else
                {
                    SNR[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(1));

                    Range[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(2));

                    Pings[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(3));
                }
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

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(NumBeams), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SNR[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SNR[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SNR[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SNR[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Range[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Range[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Range[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Range[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Pings[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Pings[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Pings[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Pings[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

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
            /// Override the ToString to return all the Profile Engineering data as a string.
            /// </summary>
            /// <returns>String of this object.</returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                int i = 0;

                sb.Append("SNR: ");
                for (i = 0; i < 4; i++)
                    sb.Append(SNR[i].ToString("0.000") + " ");
                sb.AppendLine();

                sb.Append("Range: ");
                for (i = 0; i < 4; i++)
                    sb.Append(Range[i].ToString("0.000") + " ");
                sb.AppendLine();

                sb.Append("Pings: ");
                for (i = 0; i < 4; i++)
                    sb.Append(Pings[i].ToString("0.000") + " ");
                sb.AppendLine();

                return sb.ToString();
            }

            #endregion
        }

        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.RangeTrackingData).
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
        public class RangeTrackingDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.RangeTrackingData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as RangeTrackingDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // Number of Beams
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_NUMBEAMS);
                writer.WriteValue(data.NumBeams);

                // SNR
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_RT_SNR);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.SNR.Length; beam++)
                {
                    writer.WriteValue(data.SNR[beam]);
                }
                writer.WriteEndArray();

                // Range
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_RT_RANGE);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.Range.Length; beam++)
                {
                    writer.WriteValue(data.Range[beam]);
                }
                writer.WriteEndArray();

                // Pings
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_RT_PINGS);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.Pings.Length; beam++)
                {
                    writer.WriteValue(data.Pings[beam]);
                }
                writer.WriteEndArray();

                // End the object
                writer.WriteEndObject();
            }

            /// <summary>
            /// Read the JSON object and convert to the object.  This will allow the serializer to
            /// automatically convert the object.  No special instructions need to be done and all
            /// the properties found in the JSON string need to be used.
            /// 
            /// DataSet.RangeTrackingDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.RangeTrackingDataSet}(encodedEns)
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
                    var data = new RangeTrackingDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.RangeTrackingID);

                    // Number of beams
                    data.NumBeams = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_NUMBEAMS];

                    // SNR
                    JArray jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_RT_SNR];
                    data.SNR = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.SNR[x] = (float)jArray[x];
                    }

                    // Range
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_RT_RANGE];
                    data.Range = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.Range[x] = (float)jArray[x];
                    }

                    // Pings
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_RT_PINGS];
                    data.Pings = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.Pings[x] = (float)jArray[x];
                    }

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
                return typeof(RangeTrackingDataSet).IsAssignableFrom(objectType);
            }

        }
    }
}
