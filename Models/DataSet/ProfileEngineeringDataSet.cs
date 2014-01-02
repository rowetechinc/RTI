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
 * 12/30/2013      RC          2.21.2     Initial coding
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
        /// Dataset containing all the Profile Engineering data.
        /// </summary>
        [JsonConverter(typeof(ProfileEngineeringDataSetSerializer))]
        public class ProfileEngineeringDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// Number of elements in this data set.
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 23;

            #endregion

            #region Properties

            /// <summary>
            /// Pre Ping velocity in meters per second.
            /// </summary>
            public float[] PrePingVel { get; set; }

            /// <summary>
            /// Pre Ping Correlation.
            /// </summary>
            public float[] PrePingCor { get; set; }

            /// <summary>
            /// Pre Ping amplitude.
            /// </summary>
            public float[] PrePingAmp { get; set; }

            /// <summary>
            /// Samples per second.
            /// </summary>
            public float SamplesPerSecond { get; set; }

            /// <summary>
            /// System Frequency in hertz.
            /// </summary>
            public float SystemFreqHz { get; set; }

            /// <summary>
            /// LagSamples
            /// </summary>
            public float LagSamples { get; set; }

            /// <summary>
            /// CPCE.
            /// </summary>
            public float CPCE { get; set; }

            /// <summary>
            /// NCE.
            /// </summary>
            public float NCE { get; set; }

            /// <summary>
            /// Repeat N.
            /// </summary>
            public float RepeatN { get; set; }

            /// <summary>
            /// Pre Ping Gap.
            /// </summary>
            public float PrePingGap { get; set; }

            /// <summary>
            /// Pre Ping NCE.
            /// </summary>
            public float PrePingNCE { get; set; }

            /// <summary>
            /// Pre Ping Repeat N.
            /// </summary>
            public float PrePingRepeatN { get; set; }

            /// <summary>
            /// Pre Ping Lag Samples.
            /// </summary>
            public float PrePingLagSamples { get; set; }

            /// <summary>
            /// TR High Gain
            /// </summary>
            public float TRHighGain { get; set; }

            #endregion

            #region Contructors

            /// <summary>
            /// Create a Profile Engineering data set.  This will create an empty dataset.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public ProfileEngineeringDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
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
            /// <param name="peData">Byte array containing Profile Engineering data</param>
            public ProfileEngineeringDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] peData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize arrays
                Init(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM);

                // Decode the information
                Decode(peData);
            }

            /// <summary>
            /// Create an Profile Engineering data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.ProfileEngineeringDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 97ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.ProfileEngineeringDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.ProfileEngineeringDataSet}(json); 
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
            /// <param name="PrePingVel">Pre Ping Velocity.</param>
            /// <param name="PrePingCor">Pre Ping Correlation.</param>
            /// <param name="PrePingAmp">Pre Ping Amplitude.</param>
            /// <param name="SamplesPerSecond">Samples Per Second.</param>
            /// <param name="SystemFreqHz">System Frequency Hertz.</param>
            /// <param name="LagSamples">Lag Samples.</param>
            /// <param name="CPCE">CPCE.</param>
            /// <param name="NCE">NCE.</param>
            /// <param name="RepeatN">Repeat N.</param>
            /// <param name="PrePingGap">Pre Ping Gap.</param>
            /// <param name="PrePingNCE">Pre Ping NCE.</param>
            /// <param name="PrePingRepeatN">Pre Ping Repeat N.</param>
            /// <param name="PrePingLagSamples">Pre Ping Lag Samples.</param>
            /// <param name="TRHighGain">TR High Gain.</param>
            [JsonConstructor]
            public ProfileEngineeringDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name,
                float[] PrePingVel, float[] PrePingCor, float[] PrePingAmp, float SamplesPerSecond, float SystemFreqHz, float LagSamples,
                float CPCE, float NCE, float RepeatN, float PrePingGap, float PrePingNCE, float PrePingRepeatN, float PrePingLagSamples,
                float TRHighGain)
                : base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                this.PrePingVel = PrePingVel;
                this.PrePingCor = PrePingCor;
                this.PrePingAmp = PrePingAmp;

                this.SamplesPerSecond = SamplesPerSecond;
                this.SystemFreqHz = SystemFreqHz;
                this.LagSamples = LagSamples;
                this.CPCE = CPCE;
                this.NCE = NCE;
                this.RepeatN = RepeatN;
                this.PrePingGap = PrePingGap;
                this.PrePingNCE = PrePingNCE;
                this.PrePingRepeatN = PrePingRepeatN;
                this.PrePingLagSamples = PrePingLagSamples;
                this.TRHighGain = TRHighGain;
            }

            #endregion

            #region Init

            /// <summary>
            /// Initialize all the arrays for the Profie Engineering dataset.
            /// This will create the arrays based off the number of beams
            /// given.
            /// </summary>
            /// <param name="numBeams">Number of beams.</param>
            public void Init(int numBeams)
            {
                PrePingVel = new float[numBeams];
                PrePingCor = new float[numBeams];
                PrePingAmp = new float[numBeams];

                SamplesPerSecond = 0.0f;
                SystemFreqHz = 0.0f;
                LagSamples = 0.0f;
                CPCE = 0.0f;
                NCE = 0.0f;
                RepeatN = 0.0f;
                PrePingGap = 0.0f;
                PrePingNCE = 0.0f;
                PrePingRepeatN = 0.0f;
                PrePingLagSamples = 0.0f;
                TRHighGain = 0.0f;
            }

            #endregion

            #region Decode / Encode

            /// <summary>
            /// Move pass the Base data of the Profile Engineering.  Then based
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
            /// Get all the information about the Profile Engineering data.
            /// </summary>
            /// <param name="data">Byte array containing the Profile Engineering data type.</param>
            private void Decode(byte[] data)
            {

                PrePingVel[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(0));
                PrePingVel[1] = MathHelper.ByteArrayToFloat(data, GenerateIndex(1));
                PrePingVel[2] = MathHelper.ByteArrayToFloat(data, GenerateIndex(2));
                PrePingVel[3] = MathHelper.ByteArrayToFloat(data, GenerateIndex(3));

                PrePingCor[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(4));
                PrePingCor[1] = MathHelper.ByteArrayToFloat(data, GenerateIndex(5));
                PrePingCor[2] = MathHelper.ByteArrayToFloat(data, GenerateIndex(6));
                PrePingCor[3] = MathHelper.ByteArrayToFloat(data, GenerateIndex(7));

                PrePingAmp[0] = MathHelper.ByteArrayToFloat(data, GenerateIndex(8));
                PrePingAmp[1] = MathHelper.ByteArrayToFloat(data, GenerateIndex(9));
                PrePingAmp[2] = MathHelper.ByteArrayToFloat(data, GenerateIndex(10));
                PrePingAmp[3] = MathHelper.ByteArrayToFloat(data, GenerateIndex(11));

                SamplesPerSecond = MathHelper.ByteArrayToFloat(data, GenerateIndex(12));
                SystemFreqHz = MathHelper.ByteArrayToFloat(data, GenerateIndex(13));
                LagSamples = MathHelper.ByteArrayToFloat(data, GenerateIndex(14));
                CPCE = MathHelper.ByteArrayToFloat(data, GenerateIndex(15));
                NCE = MathHelper.ByteArrayToFloat(data, GenerateIndex(16));
                RepeatN = MathHelper.ByteArrayToFloat(data, GenerateIndex(17));
                PrePingGap = MathHelper.ByteArrayToFloat(data, GenerateIndex(18));
                PrePingNCE = MathHelper.ByteArrayToFloat(data, GenerateIndex(19));
                PrePingRepeatN = MathHelper.ByteArrayToFloat(data, GenerateIndex(20));
                PrePingLagSamples = MathHelper.ByteArrayToFloat(data, GenerateIndex(21));
                TRHighGain = MathHelper.ByteArrayToFloat(data, GenerateIndex(22));

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
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingVel[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingVel[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingVel[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingVel[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingCor[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingCor[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingCor[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingCor[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingAmp[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingAmp[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingAmp[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingAmp[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SamplesPerSecond), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SystemFreqHz), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(LagSamples), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(CPCE), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(NCE), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(RepeatN), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingGap), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingNCE), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingRepeatN), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PrePingLagSamples), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(TRHighGain), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

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

                sb.Append("Pre Ping Vel: ");
                for (i = 0; i < 4; i++)
                    sb.Append(PrePingVel[i].ToString("0.000") + " ");
                sb.AppendLine();

                sb.Append("Pre Ping Cor: ");
                for (i = 0; i < 4; i++)
                    sb.Append(PrePingCor[i].ToString("0.000") + " ");
                sb.AppendLine();

                sb.Append("Pre Ping Amp: ");
                for (i = 0; i < 4; i++)
                    sb.Append(PrePingAmp[i].ToString("0.000") + " ");
                sb.AppendLine();

                sb.Append("SamplesPerSecond: " + SamplesPerSecond);
                sb.Append(" SystemFreqHz: " + SystemFreqHz);
                sb.Append(" LagSamples: " + LagSamples);
                sb.Append(" CPCE: " + CPCE);
                sb.Append(" NCE: " + NCE);
                sb.AppendLine();
                sb.Append("RepeatN: " + RepeatN);
                sb.Append(" PrePingGap: " + PrePingGap);
                sb.Append(" PrePingNCE: " + PrePingNCE);
                sb.Append(" PrePingRepeatN: " + PrePingRepeatN);
                sb.Append(" PrePingLagSamples: " + PrePingLagSamples);
                sb.Append(" TRHighGain: " + TRHighGain);

                return sb.ToString();
            }

            #endregion

        }

        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.ProfileEngineeringData).
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
        public class ProfileEngineeringDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.ProfileEngineeringData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as ProfileEngineeringDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // PrePingVel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_PREPINGVEL);
                writer.WriteStartArray();
                for (int beam = 0; beam < 4; beam++)
                {
                    writer.WriteValue(data.PrePingVel[beam]);
                }
                writer.WriteEndArray();

                // PrePingCor
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_PREPINGCOR);
                writer.WriteStartArray();
                for (int beam = 0; beam < 4; beam++)
                {
                    writer.WriteValue(data.PrePingCor[beam]);
                }
                writer.WriteEndArray();

                // PrePingAmp
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_PREPINGAMP);
                writer.WriteStartArray();
                for (int beam = 0; beam < 4; beam++)
                {
                    writer.WriteValue(data.PrePingAmp[beam]);
                }
                writer.WriteEndArray();

                // SamplesPerSecond
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_SAMPLESPERSECOND);
                writer.WriteValue(data.SamplesPerSecond);

                // SystemFreqHz
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_SYSTEMFREQHZ);
                writer.WriteValue(data.SystemFreqHz);

                // LagSamples
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_LAGSAMPLES);
                writer.WriteValue(data.LagSamples);

                // CPCE
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_CPCE);
                writer.WriteValue(data.CPCE);

                // NCE
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_NCE);
                writer.WriteValue(data.NCE);

                // RepeatN
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_REPEATN);
                writer.WriteValue(data.RepeatN);

                // PrePingGap
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_PREPINGGAP);
                writer.WriteValue(data.PrePingGap);

                // PrePingNCE
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_PREPINGNCE);
                writer.WriteValue(data.PrePingNCE);

                // PrePingRepeatN
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_PREPINGREPEATN);
                writer.WriteValue(data.PrePingRepeatN);

                // PrePingLagSamples
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_PREPINGLAGSAMPLES);
                writer.WriteValue(data.PrePingLagSamples);

                // TRHighGain
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PE_PREPINGTRHIGHGAIN);
                writer.WriteValue(data.TRHighGain);

                // End the object
                writer.WriteEndObject();
            }

            /// <summary>
            /// Read the JSON object and convert to the object.  This will allow the serializer to
            /// automatically convert the object.  No special instructions need to be done and all
            /// the properties found in the JSON string need to be used.
            /// 
            /// DataSet.ProfileEngineeringDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.ProfileEngineeringDataSet}(encodedEns)
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
                    var data = new ProfileEngineeringDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.ProfileEngineeringID);

                    // PrePingVel
                    JArray jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_PREPINGVEL];
                    data.PrePingVel = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.PrePingVel[x] = (float)jArray[x];
                    }

                    // PrePingCor
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_PREPINGCOR];
                    data.PrePingCor = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.PrePingCor[x] = (float)jArray[x];
                    }

                    // PrePingAmp
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_PREPINGAMP];
                    data.PrePingAmp = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.PrePingAmp[x] = (float)jArray[x];
                    }

                    // SamplesPerSecond
                    data.SamplesPerSecond = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_SAMPLESPERSECOND];

                    // SystemFreqHz
                    data.SystemFreqHz = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_SYSTEMFREQHZ];

                    // LagSamples
                    data.LagSamples = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_LAGSAMPLES];

                    // CPCE
                    data.CPCE = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_CPCE];

                    // NCE
                    data.NCE = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_NCE];

                    // RepeatN
                    data.RepeatN = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_REPEATN];

                    // PrePingGap
                    data.PrePingGap = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_PREPINGGAP];

                    // PrePingNCE
                    data.PrePingNCE = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_PREPINGNCE];

                    // PrePingRepeatN
                    data.PrePingRepeatN = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_PREPINGREPEATN];

                    // PrePingLagSamples
                    data.PrePingLagSamples = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_PREPINGLAGSAMPLES];

                    // TRHighGain
                    data.TRHighGain = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PE_PREPINGTRHIGHGAIN];

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
