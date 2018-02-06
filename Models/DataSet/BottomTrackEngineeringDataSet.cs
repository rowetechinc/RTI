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
 * 12/31/2013      RC          2.21.2     Initial coding
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
        /// Dataset containing all the Bottom Track Engineering data.
        /// </summary>
        [JsonConverter(typeof(BottomTrackEngineeringDataSetSerializer))]
        public class BottomTrackEngineeringDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// Number of elements in this data set.
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 30;

            #endregion

            #region Properties

            /// <summary>
            /// Samples per second.
            /// </summary>
            public float SamplesPerSecond { get; set; }

            /// <summary>
            /// System Frequency in hertz.
            /// </summary>
            public float SystemFreqHz { get; set; }

            /// <summary>
            /// LagSamples.
            /// Samples in lag.
            /// </summary>
            public float LagSamples { get; set; }

            /// <summary>
            /// CPCE.
            /// Cycles per Code Element.
            /// </summary>
            public float CPCE { get; set; }

            /// <summary>
            /// NCE.
            /// Number of code elements in code.
            /// </summary>
            public float NCE { get; set; }

            /// <summary>
            /// Repeat N.
            /// Number of repeated codes.
            /// </summary>
            public float RepeatN { get; set; }

            /// <summary>
            /// Amb Hertz.
            /// </summary>
            public float[] AmbHz { get; set; }

            /// <summary>
            /// Amb Velocity.
            /// </summary>
            public float[] AmbVel { get; set; }

            /// <summary>
            /// Amb amplitude.
            /// </summary>
            public float[] AmbAmp { get; set; }

            /// <summary>
            /// Amb Correlation.
            /// </summary>
            public float[] AmbCor { get; set; }

            /// <summary>
            /// Amb Signal to Noise Ratio.
            /// </summary>
            public float[] AmbSNR { get; set; }

            /// <summary>
            /// Lag Used.
            /// </summary>
            public float[] LagUsed { get; set; }

            #endregion

            #region Contructors

            /// <summary>
            /// Create a Bottom Track Engineering data set.  This will create an empty dataset.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public BottomTrackEngineeringDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize arrays
                Init(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM);
            }

            /// <summary>
            /// Create a Profile Engineering data set.  Includes all the information
            /// about the current Profile Engineering data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="bteData">Byte array containing Bottom Track Engineering data</param>
            public BottomTrackEngineeringDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] bteData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize arrays
                Init(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM);

                // Decode the information
                Decode(bteData);
            }

            /// <summary>
            /// Create an Bottom Track Engineering data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.BottomTrackEngineeringDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 97ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.BottomTrackEngineeringDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.BottomTrackEngineeringDataSet}(json); 
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
            /// <param name="SamplesPerSecond">Samples per Second.</param>
            /// <param name="SystemFreqHz">System Frequency in Hertz.</param>
            /// <param name="LagSamples">Lag Samples.</param>
            /// <param name="CPCE">CPCE.</param>
            /// <param name="NCE">NCE.</param>
            /// <param name="RepeatN">Repeat N.</param>
            /// <param name="AmbHz">Pre Ping Velocity.</param>
            /// <param name="AmbVel">Pre Ping Correlation.</param>
            /// <param name="AmbAmp">Pre Ping Amplitude.</param>
            /// <param name="AmbCor">Samples Per Second.</param>
            /// <param name="AmbSNR">System Frequency Hertz.</param>
            /// <param name="LagUsed">Lag Samples.</param>
            [JsonConstructor]
            public BottomTrackEngineeringDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name,
                float SamplesPerSecond, float SystemFreqHz, float LagSamples, float CPCE, float NCE, float RepeatN,
                float[] AmbHz, float[] AmbVel, float[] AmbAmp, float[] AmbCor, float[] AmbSNR, float[] LagUsed)
                : base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                this.SamplesPerSecond = SamplesPerSecond;
                this.SystemFreqHz = SystemFreqHz;
                this.LagSamples = LagSamples;
                this.CPCE = CPCE;
                this.NCE = NCE;
                this.RepeatN = RepeatN;

                this.AmbHz = AmbHz;
                this.AmbVel = AmbVel;
                this.AmbAmp = AmbAmp;
                this.AmbCor = AmbCor;
                this.AmbSNR = AmbSNR;
                this.LagUsed = LagUsed;
            }

            #endregion

            #region Init

            /// <summary>
            /// Initialize all the arrays for the Bottom Track Engineering dataset.
            /// This will create the arrays based off the number of beams
            /// given.
            /// </summary>
            /// <param name="numBeams">Number of beams.</param>
            public void Init(int numBeams)
            {
                SamplesPerSecond = 0.0f;
                SystemFreqHz = 0.0f;
                LagSamples = 0.0f;
                CPCE = 0.0f;
                NCE = 0.0f;
                RepeatN = 0.0f;

                AmbHz = new float[numBeams];
                AmbVel = new float[numBeams];
                AmbAmp = new float[numBeams];
                AmbCor = new float[numBeams];
                AmbSNR = new float[numBeams];
                LagUsed = new float[numBeams];
            }

            #endregion

            #region Decode / Encode

            /// <summary>
            /// Move pass the Base data of the Bottom Track Engineering.  Then based
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
            /// Get all the information about the Bottom Track Engineering data.
            /// </summary>
            /// <param name="data">Byte array containing the Bottom Track Engineering data type.</param>
            private void Decode(byte[] data)
            {

                SamplesPerSecond = MathHelper.ByteArrayToFloat(data, GenerateIndex(0));
                SystemFreqHz = MathHelper.ByteArrayToFloat(data, GenerateIndex(1));
                LagSamples = MathHelper.ByteArrayToFloat(data, GenerateIndex(2));
                CPCE = MathHelper.ByteArrayToFloat(data, GenerateIndex(3));
                NCE = MathHelper.ByteArrayToFloat(data, GenerateIndex(4));
                RepeatN = MathHelper.ByteArrayToFloat(data, GenerateIndex(5));


                AmbHz[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(6));
                AmbHz[1] = MathHelper.ByteArrayToFloat(data, GenerateIndex(7));
                AmbHz[2] = MathHelper.ByteArrayToFloat(data, GenerateIndex(8));
                AmbHz[3] = MathHelper.ByteArrayToFloat(data, GenerateIndex(9));

                AmbVel[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(10));
                AmbVel[1] = MathHelper.ByteArrayToFloat(data, GenerateIndex(11));
                AmbVel[2] = MathHelper.ByteArrayToFloat(data, GenerateIndex(12));
                AmbVel[3] = MathHelper.ByteArrayToFloat(data, GenerateIndex(13));

                AmbAmp[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(14));
                AmbAmp[1] = MathHelper.ByteArrayToFloat(data, GenerateIndex(15));
                AmbAmp[2] = MathHelper.ByteArrayToFloat(data, GenerateIndex(16));
                AmbAmp[3] = MathHelper.ByteArrayToFloat(data, GenerateIndex(17));

                AmbCor[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(18));
                AmbCor[1] = MathHelper.ByteArrayToFloat(data, GenerateIndex(19));
                AmbCor[2] = MathHelper.ByteArrayToFloat(data, GenerateIndex(20));
                AmbCor[3] = MathHelper.ByteArrayToFloat(data, GenerateIndex(21));

                AmbSNR[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(22));
                AmbSNR[1] = MathHelper.ByteArrayToFloat(data, GenerateIndex(23));
                AmbSNR[2] = MathHelper.ByteArrayToFloat(data, GenerateIndex(24));
                AmbSNR[3] = MathHelper.ByteArrayToFloat(data, GenerateIndex(25));

                LagUsed[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(26));
                LagUsed[1] = MathHelper.ByteArrayToFloat(data, GenerateIndex(27));
                LagUsed[2] = MathHelper.ByteArrayToFloat(data, GenerateIndex(28));
                LagUsed[3] = MathHelper.ByteArrayToFloat(data, GenerateIndex(29));

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
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SamplesPerSecond), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SystemFreqHz), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(LagSamples), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(CPCE), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(NCE), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(RepeatN), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbHz[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbHz[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbHz[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbHz[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbVel[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbVel[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbVel[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbVel[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbAmp[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbAmp[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbAmp[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbAmp[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbCor[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbCor[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbCor[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbCor[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbSNR[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbSNR[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbSNR[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(AmbSNR[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(LagUsed[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(LagUsed[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(LagUsed[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(LagUsed[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

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
            /// Override the ToString to return all the Bottom Track Engineering data as a string.
            /// </summary>
            /// <returns>String of this object.</returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                int i = 0;

                sb.Append("SamplesPerSecond: " + SamplesPerSecond);
                sb.Append(" SystemFreqHz: " + SystemFreqHz);
                sb.Append(" LagSamples: " + LagSamples);
                sb.Append(" CPCE: " + CPCE);
                sb.Append(" NCE: " + NCE);
                sb.Append("RepeatN: " + RepeatN);
                sb.AppendLine();

                sb.Append("Amb Hz: ");
                for (i = 0; i < 4; i++)
                    sb.Append(AmbHz[i].ToString("0.000") + " ");
                sb.AppendLine();

                sb.Append("Amb Vel: ");
                for (i = 0; i < 4; i++)
                    sb.Append(AmbVel[i].ToString("0.000") + " ");
                sb.AppendLine();

                sb.Append("Amb Amp: ");
                for (i = 0; i < 4; i++)
                    sb.Append(AmbAmp[i].ToString("0.000") + " ");
                sb.AppendLine();

                sb.Append("Amb Corr: ");
                for (i = 0; i < 4; i++)
                    sb.Append(AmbCor[i].ToString("0.000") + " ");
                sb.AppendLine();

                sb.Append("Amb SNR: ");
                for (i = 0; i < 4; i++)
                    sb.Append(AmbSNR[i].ToString("0.000") + " ");
                sb.AppendLine();

                sb.Append("Lag Used: ");
                for (i = 0; i < 4; i++)
                    sb.Append(LagUsed[i].ToString("0.000") + " ");
                sb.AppendLine();

                return sb.ToString();
            }

            #endregion

        }

        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.BottomTrackEngineeringData).
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
        public class BottomTrackEngineeringDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.BottomTrackEngineeringData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as BottomTrackEngineeringDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // SamplesPerSecond
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BTE_SAMPLESPERSECOND);
                writer.WriteValue(data.SamplesPerSecond);

                // SystemFreqHz
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BTE_SYSTEMFREQHZ);
                writer.WriteValue(data.SystemFreqHz);

                // LagSamples
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BTE_LAGSAMPLES);
                writer.WriteValue(data.LagSamples);

                // CPCE
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BTE_CPCE);
                writer.WriteValue(data.CPCE);

                // NCE
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BTE_NCE);
                writer.WriteValue(data.NCE);

                // RepeatN
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BTE_REPEATN);
                writer.WriteValue(data.RepeatN);

                // AmbHz
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BTE_AMBHZ);
                writer.WriteStartArray();
                for (int beam = 0; beam < 4; beam++)
                {
                    writer.WriteValue(data.AmbHz[beam]);
                }
                writer.WriteEndArray();

                // AmbVel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BTE_AMBVEL);
                writer.WriteStartArray();
                for (int beam = 0; beam < 4; beam++)
                {
                    writer.WriteValue(data.AmbVel[beam]);
                }
                writer.WriteEndArray();

                // AmbAmp
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BTE_AMBAMP);
                writer.WriteStartArray();
                for (int beam = 0; beam < 4; beam++)
                {
                    writer.WriteValue(data.AmbAmp[beam]);
                }
                writer.WriteEndArray();

                // AmbCor
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BTE_AMBCOR);
                writer.WriteStartArray();
                for (int beam = 0; beam < 4; beam++)
                {
                    writer.WriteValue(data.AmbCor[beam]);
                }
                writer.WriteEndArray();

                // AmbSNR
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BTE_AMBSNR);
                writer.WriteStartArray();
                for (int beam = 0; beam < 4; beam++)
                {
                    writer.WriteValue(data.AmbSNR[beam]);
                }
                writer.WriteEndArray();

                // LagUsed
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BTE_LAGUSED);
                writer.WriteStartArray();
                for (int beam = 0; beam < 4; beam++)
                {
                    writer.WriteValue(data.LagUsed[beam]);
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
            /// DataSet.BottomTrackEngineeringDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.BottomTrackEngineeringDataSet}(encodedEns)
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
                    var data = new BottomTrackEngineeringDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.BottomTrackEngineeringID);

                    // SamplesPerSecond
                    data.SamplesPerSecond = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_BTE_SAMPLESPERSECOND];

                    // SystemFreqHz
                    data.SystemFreqHz = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_BTE_SYSTEMFREQHZ];

                    // LagSamples
                    data.LagSamples = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_BTE_LAGSAMPLES];

                    // CPCE
                    data.CPCE = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_BTE_CPCE];

                    // NCE
                    data.NCE = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_BTE_NCE];

                    // RepeatN
                    data.RepeatN = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_BTE_REPEATN];

                    // AmbHz
                    JArray jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BTE_AMBHZ];
                    data.AmbHz = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.AmbHz[x] = (float)jArray[x];
                    }

                    // AmbVel
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BTE_AMBVEL];
                    data.AmbVel = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.AmbVel[x] = (float)jArray[x];
                    }

                    // AmbAmp
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BTE_AMBAMP];
                    data.AmbAmp = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.AmbAmp[x] = (float)jArray[x];
                    }

                    // AmbCor
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BTE_AMBCOR];
                    data.AmbCor = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.AmbCor[x] = (float)jArray[x];
                    }

                    // AmbSNR
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BTE_AMBSNR];
                    data.AmbSNR = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.AmbSNR[x] = (float)jArray[x];
                    }

                    // LagUsed
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BTE_LAGUSED];
                    data.LagUsed = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.LagUsed[x] = (float)jArray[x];
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
                return typeof(BottomTrackEngineeringDataSet).IsAssignableFrom(objectType);
            }

        }

    }
}
