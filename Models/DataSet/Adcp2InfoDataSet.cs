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
 * 09/25/2015      RC          3.1.1     Initial coding
 * 
 */

namespace RTI
{
    namespace DataSet
    {

        using System;
        using System.Text;
        using Newtonsoft.Json;
        using Newtonsoft.Json.Linq;

        /// <summary>
        /// Dataset containing all the ADCP 2 Info data.
        /// </summary>
        [JsonConverter(typeof(Adcp2InfoDataSetSerializer))]
        public class Adcp2InfoDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// Number of elements in this data set.
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 4;

            #endregion

            #region Properties

            /// <summary>
            /// VI Power.
            /// </summary>
            public float ViPwr { get; set; }

            /// <summary>
            /// VI NF.
            /// </summary>
            public float ViNf { get; set; }

            /// <summary>
            /// VI NFL.
            /// </summary>
            public float ViNfl { get; set; }

            /// <summary>
            /// VI Sleep.
            /// </summary>
            public float ViSleep { get; set; }

            #endregion

            #region Contructors

            /// <summary>
            /// Create a ADCP 2 Info data set.  This will create an empty dataset.
            /// </summary>
            public Adcp2InfoDataSet() :
                base(DataSet.Ensemble.DATATYPE_FLOAT,                       // Type of data stored (Float or Int)
                            NUM_DATA_ELEMENTS,                              // Number of data elements
                            1,                                              // Number of beams
                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                            DataSet.Ensemble.Adcp2InfoID)
            {
                // Initialize arrays
                Init();
            }

            /// <summary>
            /// Create a ADCP 2 Info data set.  This will create an empty dataset.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public Adcp2InfoDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize arrays
                Init();
            }

            /// <summary>
            /// Create a ADCP 2 Info data set.  Includes all the information
            /// about the current System Setup data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="ssData">Byte array containing System Setup data</param>
            public Adcp2InfoDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] ssData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize arrays
                Init();

                // Decode the information
                Decode(ssData);
            }

            /// <summary>
            /// Create an ADCP 2 Info data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.SystemSetupDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 97ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.SystemSetupDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.SystemSetupDataSet}(json); 
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
            /// <param name="ViPwr">BT Samples Per Second.</param>
            /// <param name="ViNf">BT System Frequency Hertz.</param>
            /// <param name="ViNfl">BT CPCE.</param>
            /// <param name="ViSleep">BT NCE.</param>
            /// 
            [JsonConstructor]
            public Adcp2InfoDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name,
                float ViPwr, float ViNf, float ViNfl, float ViSleep)
                : base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                this.ViPwr = ViPwr;
                this.ViNf = ViNf;
                this.ViNfl = ViNfl;
                this.ViSleep = ViSleep;
            }

            #endregion

            #region Init

            /// <summary>
            /// Initialize all the arrays for the ADCP 2 Info dataset.
            /// This will create the arrays based off the number of beams
            /// given.
            /// </summary>
            public void Init()
            {

                this.ViPwr = 0.0f;
                this.ViNf = 0.0f;
                this.ViNfl = 0.0f;
                this.ViSleep = 0.0f;
            }

            #endregion

            #region Decode / Encode

            /// <summary>
            /// Move pass the Base data of the System Setup.  Then based
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
            /// Get all the information about the System Setup data.
            /// </summary>
            /// <param name="data">Byte array containing the System Setup data type.</param>
            private void Decode(byte[] data)
            {

                ViPwr = MathHelper.ByteArrayToFloat(data, GenerateIndex(0));
                ViNf = MathHelper.ByteArrayToFloat(data, GenerateIndex(1));
                ViNfl = MathHelper.ByteArrayToFloat(data, GenerateIndex(2));
                ViSleep = MathHelper.ByteArrayToFloat(data, GenerateIndex(3));
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
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ViPwr), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ViNf), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ViNfl), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ViSleep), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                // Generate header for the dataset
                byte[] header = this.GenerateHeader(NUM_DATA_ELEMENTS);

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
            /// Override the ToString to return all the System Setup data as a string.
            /// </summary>
            /// <returns>String of this object.</returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("ViPwr: " + ViPwr);
                sb.Append(" ViNf: " + ViNf);
                sb.Append(" ViNfl: " + ViNfl);
                sb.Append(" ViSleep: " + ViSleep);

                return sb.ToString();
            }

            #endregion

        }

        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.SystemSetupDataSet).
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
        public class Adcp2InfoDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.Adcp2InfoDataSet).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as Adcp2InfoDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // ViPwr
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_ADCP2INFO_VIPWR);
                writer.WriteValue(data.ViPwr);

                // ViNf
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_ADCP2INFO_VINF);
                writer.WriteValue(data.ViNf);

                // ViNfl
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_ADCP2INFO_VINFL);
                writer.WriteValue(data.ViNfl);

                // ViSleep
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_ADCP2INFO_VISLEEP);
                writer.WriteValue(data.ViSleep);

                // End the object
                writer.WriteEndObject();
            }

            /// <summary>
            /// Read the JSON object and convert to the object.  This will allow the serializer to
            /// automatically convert the object.  No special instructions need to be done and all
            /// the properties found in the JSON string need to be used.
            /// 
            /// DataSet.Adcp2InfoDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.Adcp2InfoDataSet}(encodedEns)
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
                    var data = new Adcp2InfoDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.Adcp2InfoID);

                    // ViPwr
                    data.ViPwr = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_ADCP2INFO_VIPWR];

                    // ViNf
                    data.ViNf = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_ADCP2INFO_VINF];

                    // ViNfl
                    data.ViNfl = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_ADCP2INFO_VINFL];

                    // ViSleep
                    data.ViSleep = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_ADCP2INFO_VISLEEP];

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
                return typeof(Adcp2InfoDataSet).IsAssignableFrom(objectType);
            }

        }

    }
}
