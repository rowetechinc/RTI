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
 * 01/09/2014      RC          2.21.3     Initial coding
 * 07/21/2014      RC          2.23.0     Fixed checking if the voltage existed in Decode().
 * 07/24/2014      RC          2.23.0     Added and empty constructor.
 * 04/03/2015      RC          3.0.3      Fixed bug with Encode().
 * 09/25/2015      RC          3.1.1      Added missing variables.
 * 10/07/2015      RC          3.2.0      Check for missing JSON values in ReadJson.
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
        /// Dataset containing all the System Setup data.
        /// </summary>
        [JsonConverter(typeof(SystemSetupDataSetSerializer))]
        public class SystemSetupDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// Number of elements in this data set.
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 21;

            #endregion

            #region Properties

            /// <summary>
            /// Bottom Track Samples per second.
            /// </summary>
            public float BtSamplesPerSecond { get; set; }

            /// <summary>
            /// Bottom Track System Frequency in hertz.
            /// </summary>
            public float BtSystemFreqHz { get; set; }

            /// <summary>
            /// Bottom Track CPCE.
            /// </summary>
            public float BtCPCE { get; set; }

            /// <summary>
            /// Bottom Track Number of Coded Elements.
            /// </summary>
            public float BtNCE { get; set; }

            /// <summary>
            /// Bottom Track Repeat N.
            /// Number of Repeated Codes.
            /// </summary>
            public float BtRepeatN { get; set; }

            /// <summary>
            /// Water Profile Samples per second.
            /// </summary>
            public float WpSamplesPerSecond { get; set; }

            /// <summary>
            /// Water Profile System Frequency in hertz.
            /// </summary>
            public float WpSystemFreqHz { get; set; }

            /// <summary>
            /// Water Profile CPCE.
            /// Cycles per Code Element.
            /// </summary>
            public float WpCPCE { get; set; }

            /// <summary>
            /// Water Profile Number of coded elements.
            /// Number of code elements in code.
            /// </summary>
            public float WpNCE { get; set; }

            /// <summary>
            /// Water Profile Repeat N.
            /// Number of repeated codes.
            /// </summary>
            public float WpRepeatN { get; set; }

            /// <summary>
            /// Water Profile Lag Samples.
            /// Samples in lag.
            /// </summary>
            public float WpLagSamples { get; set; }

            /// <summary>
            /// Voltage.
            /// VI Power.
            /// </summary>
            public float Voltage { get; set; }

            /// <summary>
            /// Transmit Voltage.
            /// </summary>
            public float XmtVoltage { get; set; }

            /// <summary>
            /// BT Broadband.
            /// </summary>
            public float BtBroadband { get; set; }

            /// <summary>
            /// BT Lag Length.
            /// </summary>
            public float BtLagLength { get; set; }

            /// <summary>
            /// BT Narrowband.
            /// </summary>
            public float BtNarrowband { get; set; }

            /// <summary>
            /// Bottom Track Beam MUX.
            /// </summary>
            public float BtBeamMux { get; set; }

            /// <summary>
            /// WP Broadband.
            /// </summary>
            public float WpBroadband { get; set; }

            /// <summary>
            /// WP Lag Length.
            /// </summary>
            public float WpLagLength { get; set; }

            /// <summary>
            /// WP Bandwidth.
            /// </summary>
            public float WpTransmitBandwidth { get; set; }

            /// <summary>
            /// WP Bandwidth.
            /// </summary>
            public float WpReceiveBandwidth { get; set; }

            #endregion

            #region Contructors

            /// <summary>
            /// Create a System Setup data set.  This will create an empty dataset.
            /// </summary>
            public SystemSetupDataSet() :
                base(DataSet.Ensemble.DATATYPE_FLOAT,                       // Type of data stored (Float or Int)
                            NUM_DATA_ELEMENTS,                              // Number of data elements
                            1,                                              // Number of beams
                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                            DataSet.Ensemble.SystemSetupID)
            {
                // Initialize arrays
                Init();
            }

            /// <summary>
            /// Create a System Setup data set.  This will create an empty dataset.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public SystemSetupDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize arrays
                Init();
            }

            /// <summary>
            /// Create a System Setup data set.  Includes all the information
            /// about the current System Setup data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="ssData">Byte array containing System Setup data</param>
            public SystemSetupDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] ssData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize arrays
                Init();

                // Decode the information
                Decode(ssData);
            }

            /// <summary>
            /// Create an System Setup data set.  Intended for JSON  deserialize.  This method
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
            /// <param name="BtSamplesPerSecond">BT Samples Per Second.</param>
            /// <param name="BtSystemFreqHz">BT System Frequency Hertz.</param>
            /// <param name="BtCPCE">BT CPCE.</param>
            /// <param name="BtNCE">BT NCE.</param>
            /// <param name="BtRepeatN">BT Repeat N.</param>
            /// <param name="WpSamplesPerSecond">WP Samples Per Second.</param>
            /// <param name="WpSystemFreqHz">WP System Frequency Hertz.</param>
            /// <param name="WpCPCE">WP CPCE.</param>
            /// <param name="WpNCE">WP NCE.</param>
            /// <param name="WpRepeatN">WP Repeat N.</param>
            /// <param name="WpLagSamples">WP Lag Samples.</param>
            /// <param name="Voltage">Voltage</param>
            [JsonConstructor]
            public SystemSetupDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name,
                float BtSamplesPerSecond, float BtSystemFreqHz, float BtCPCE, float BtNCE, float BtRepeatN, 
                float WpSamplesPerSecond, float WpSystemFreqHz, float WpCPCE, float WpNCE, float WpRepeatN, float WpLagSamples,
                float Voltage)
                : base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                this.BtSamplesPerSecond = BtSamplesPerSecond;
                this.BtSystemFreqHz = BtSystemFreqHz;
                this.BtCPCE = BtCPCE;
                this.BtNCE = BtNCE;
                this.BtRepeatN = BtRepeatN;

                this.WpSamplesPerSecond = WpSamplesPerSecond;
                this.WpSystemFreqHz = WpSystemFreqHz;
                this.WpCPCE = WpCPCE;
                this.WpNCE = WpNCE;
                this.WpRepeatN = WpRepeatN;
                this.WpLagSamples = WpLagSamples;

                this.Voltage = Voltage;
            }

            #endregion

            #region Init

            /// <summary>
            /// Initialize all the arrays for the System Setup dataset.
            /// This will create the arrays based off the number of beams
            /// given.
            /// </summary>
            public void Init()
            {

                this.BtSamplesPerSecond = 0.0f;
                this.BtSystemFreqHz = 0.0f;
                this.BtCPCE = 0.0f;
                this.BtNCE = 0.0f;
                this.BtRepeatN = 0.0f;

                this.WpSamplesPerSecond = 0.0f;
                this.WpSystemFreqHz = 0.0f;
                this.WpCPCE = 0.0f;
                this.WpNCE = 0.0f;
                this.WpRepeatN = 0.0f;
                this.WpLagSamples = 0.0f;

                this.Voltage = 0.0f;
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

                BtSamplesPerSecond = MathHelper.ByteArrayToFloat(data, GenerateIndex(0));
                BtSystemFreqHz = MathHelper.ByteArrayToFloat(data, GenerateIndex(1));
                BtCPCE = MathHelper.ByteArrayToFloat(data, GenerateIndex(2));
                BtNCE = MathHelper.ByteArrayToFloat(data, GenerateIndex(3));
                BtRepeatN = MathHelper.ByteArrayToFloat(data, GenerateIndex(4));

                WpSamplesPerSecond = MathHelper.ByteArrayToFloat(data, GenerateIndex(5));
                WpSystemFreqHz = MathHelper.ByteArrayToFloat(data, GenerateIndex(6));
                WpCPCE = MathHelper.ByteArrayToFloat(data, GenerateIndex(7));
                WpNCE = MathHelper.ByteArrayToFloat(data, GenerateIndex(8));
                WpRepeatN = MathHelper.ByteArrayToFloat(data, GenerateIndex(9));
                WpLagSamples = MathHelper.ByteArrayToFloat(data, GenerateIndex(10));

                if (NumElements >= 11)
                {
                    Voltage = MathHelper.ByteArrayToFloat(data, GenerateIndex(11));
                }

                if (NumElements >= 21)
                {
                    XmtVoltage = MathHelper.ByteArrayToFloat(data, GenerateIndex(12));
                    BtBroadband = MathHelper.ByteArrayToFloat(data, GenerateIndex(13));
                    BtLagLength = MathHelper.ByteArrayToFloat(data, GenerateIndex(14));
                    BtNarrowband = MathHelper.ByteArrayToFloat(data, GenerateIndex(15));
                    BtBeamMux = MathHelper.ByteArrayToFloat(data, GenerateIndex(16));
                    WpBroadband = MathHelper.ByteArrayToFloat(data, GenerateIndex(17));
                    WpLagLength = MathHelper.ByteArrayToFloat(data, GenerateIndex(18));
                    WpTransmitBandwidth = MathHelper.ByteArrayToFloat(data, GenerateIndex(19));
                    WpReceiveBandwidth = MathHelper.ByteArrayToFloat(data, GenerateIndex(20));

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
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(BtSamplesPerSecond), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(BtSystemFreqHz), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(BtCPCE), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(BtNCE), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(BtRepeatN), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(WpSamplesPerSecond), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(WpSystemFreqHz), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(WpCPCE), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(WpNCE), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(WpRepeatN), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(WpLagSamples), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Voltage), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(XmtVoltage), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(BtBroadband), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(BtLagLength), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(BtNarrowband), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(BtBeamMux), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(WpBroadband), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(WpLagLength), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(WpTransmitBandwidth), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(WpReceiveBandwidth), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

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

                sb.Append("BT SamplesPerSecond: " + BtSamplesPerSecond);
                sb.Append(" BT SystemFreqHz: " + BtSystemFreqHz);
                sb.Append(" BT CPCE: " + BtCPCE);
                sb.Append(" BT NCE: " + BtNCE);
                sb.Append(" BT RepeatN: " + BtRepeatN);
                sb.AppendLine();
                sb.Append("WP SamplesPerSecond: " + WpSamplesPerSecond);
                sb.Append(" WP SystemFreqHz: " + WpSystemFreqHz);
                sb.Append(" WP CPCE: " + WpCPCE);
                sb.Append(" WP NCE: " + WpNCE);
                sb.Append(" WP RepeatN: " + WpRepeatN);
                sb.Append(" WP LagSamples: " + WpLagSamples);

                sb.Append(" Voltage: " + Voltage);

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
        public class SystemSetupDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.SystemSetupDataSet).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as SystemSetupDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // BT SamplesPerSecond
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_BT_SAMPLESPERSECOND);
                writer.WriteValue(data.BtSamplesPerSecond);

                // BT SystemFreqHz
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_BT_SYSTEMFREQHZ);
                writer.WriteValue(data.BtSystemFreqHz);

                // BT CPCE
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_BT_CPCE);
                writer.WriteValue(data.BtCPCE);

                // BT NCE
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_BT_NCE);
                writer.WriteValue(data.BtNCE);

                // BT RepeatN
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_BT_REPEATN);
                writer.WriteValue(data.BtRepeatN);

                // WP SamplesPerSecond
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_WP_SAMPLESPERSECOND);
                writer.WriteValue(data.WpSamplesPerSecond);

                // WP SystemFreqHz
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_WP_SYSTEMFREQHZ);
                writer.WriteValue(data.WpSystemFreqHz);

                // WP CPCE
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_WP_CPCE);
                writer.WriteValue(data.WpCPCE);

                // WP NCE
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_WP_NCE);
                writer.WriteValue(data.WpNCE);

                // WP RepeatN
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_WP_REPEATN);
                writer.WriteValue(data.WpRepeatN);

                // WP LagSamples
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_WP_LAGSAMPLES);
                writer.WriteValue(data.WpLagSamples);

                // Voltage
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_VOLTAGE);
                writer.WriteValue(data.Voltage);

                // XmtVoltage
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_XMTVOLTAGE);
                writer.WriteValue(data.XmtVoltage);

                // BtBroadband
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_BTBROADBAND);
                writer.WriteValue(data.BtBroadband);

                // BtLagLength
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_BTLAGLENGTH);
                writer.WriteValue(data.BtLagLength);

                // BtNarrowband
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_BTNARROWBAND);
                writer.WriteValue(data.BtNarrowband);

                // BtBeamMux
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_BTBEAMMUX);
                writer.WriteValue(data.BtBeamMux);

                // WpBroadband
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_WPBROADBAND);
                writer.WriteValue(data.WpBroadband);

                // WpLagLength
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_WPLAGLENGTH);
                writer.WriteValue(data.WpLagLength);

                // WpBandWidth
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_WPBANDWIDTH);
                writer.WriteValue(data.WpTransmitBandwidth);

                // WpBandWidth1
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SS_WPBANDWIDTH1);
                writer.WriteValue(data.WpReceiveBandwidth);

                // End the object
                writer.WriteEndObject();
            }

            /// <summary>
            /// Read the JSON object and convert to the object.  This will allow the serializer to
            /// automatically convert the object.  No special instructions need to be done and all
            /// the properties found in the JSON string need to be used.
            /// 
            /// DataSet.SystemSetupDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.SystemSetupDataSet}(encodedEns)
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
                    var data = new SystemSetupDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.SystemSetupID);

                    // BT SamplesPerSecond
                    data.BtSamplesPerSecond = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_BT_SAMPLESPERSECOND];

                    // BT SystemFreqHz
                    data.BtSystemFreqHz = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_BT_SYSTEMFREQHZ];

                    // BT CPCE
                    data.BtCPCE = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_BT_CPCE];

                    // BT NCE
                    data.BtNCE = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_BT_NCE];

                    // BT RepeatN
                    data.BtRepeatN = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_BT_REPEATN];

                    // WP SamplesPerSecond
                    data.WpSamplesPerSecond = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WP_SAMPLESPERSECOND];

                    // WP SystemFreqHz
                    data.WpSystemFreqHz = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WP_SYSTEMFREQHZ];

                    // WP CPCE
                    data.WpCPCE = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WP_CPCE];

                    // WP NCE
                    data.WpNCE = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WP_NCE];

                    // WP RepeatN
                    data.WpRepeatN = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WP_REPEATN];

                    // WP LagSamples
                    data.WpLagSamples = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WP_LAGSAMPLES];

                    // Voltage
                    data.Voltage = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_VOLTAGE];

                    if (jsonObject[DataSet.BaseDataSet.JSON_STR_SS_XMTVOLTAGE] != null)
                    {
                        // XmtVoltage
                        data.XmtVoltage = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_XMTVOLTAGE];
                    }

                    if (jsonObject[DataSet.BaseDataSet.JSON_STR_SS_BTBROADBAND] != null)
                    {
                        // BtBroadband
                        data.BtBroadband = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_BTBROADBAND];
                    }

                    if (jsonObject[DataSet.BaseDataSet.JSON_STR_SS_BTLAGLENGTH] != null)
                    {
                        // BtLagLength
                        data.BtLagLength = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_BTLAGLENGTH];
                    }

                    if (jsonObject[DataSet.BaseDataSet.JSON_STR_SS_BTNARROWBAND] != null)
                    {
                        // BtNarrowband
                        data.BtNarrowband = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_BTNARROWBAND];
                    }

                    if (jsonObject[DataSet.BaseDataSet.JSON_STR_SS_BTBEAMMUX] != null)
                    {
                        // BtBeamMux
                        data.BtBeamMux = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_BTBEAMMUX];
                    }

                    if (jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WPBROADBAND] != null)
                    {
                        // WpBroadband
                        data.WpBroadband = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WPBROADBAND];
                    }

                    if (jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WPLAGLENGTH] != null)
                    {
                        // WpLagLength
                        data.WpLagLength = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WPLAGLENGTH];
                    }

                    if (jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WPBANDWIDTH] != null)
                    {
                        // WpBandWidth
                        data.WpTransmitBandwidth = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WPBANDWIDTH];
                    }

                    if (jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WPBANDWIDTH1] != null)
                    {
                        // WpBandWidth1
                        data.WpReceiveBandwidth = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SS_WPBANDWIDTH1];
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
                return typeof(SystemSetupDataSet).IsAssignableFrom(objectType);
            }

        }

    }
}
