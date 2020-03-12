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
 * 08/25/2017      RC          3.4.2      Initial coding.
 * 03/11/2020      RC          3.4.16     Added PD0 Missing Values and DecodePd0Ensemble()
 * 
 */

using System.Data;
using System;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the Water Mass Ship Velocity data.
        /// </summary>
        [JsonConverter(typeof(ShipWaterMassDataSetSerializer))]
        public class ShipWaterMassDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// Number of elements within this data set
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 4;

            #endregion

            #region Properties

            /// <summary>
            /// Water Mass X velocity in meters per second.
            /// </summary>
            public float VelocityTransverse { get; set; }

            /// <summary>
            /// Water Mass Y velocity in meters per second.
            /// </summary>
            public float VelocityLongitudinal { get; set; }

            /// <summary>
            /// Water Mass Z velocity in meters per second.
            /// </summary>
            public float VelocityNormal { get; set; }

            /// <summary>
            /// Water Mass Q velocity in meters per second.
            /// </summary>
            public float VelocityQ { get; set; }

            /// <summary>
            /// Depth layer the Water Mass Velocity was taken in meters.
            /// </summary>
            public float WaterMassDepthLayer { get; set; }

            #region PD0

            #region Reference Layers

            /// <summary>
            /// Bottom Track Reference Layer Mininum.
            /// 
            /// Stores the minimum layer size, the near boundary, and the far
            /// boundary of the BT water-reference layer (BL-command).
            /// 
            /// Scaling (minimum layer size): LSD = 1 dm; Range = 0-999dm
            /// 
            /// Scaling (near/far boundaries): LSD = 1 dm; Range = 0-9999dm
            /// </summary>
            public ushort BtRefLayerMin { get; set; }

            /// <summary>
            /// Bottom Track Reference Layer Near.
            /// 
            /// Stores the minimum layer size, the near boundary, and the far
            /// boundary of the BT water-reference layer (BL-command).
            /// 
            /// Scaling (minimum layer size): LSD = 1 dm; Range = 0-999dm
            /// 
            /// Scaling (near/far boundaries): LSD = 1 dm; Range = 0-9999dm
            /// </summary>
            public ushort BtRefLayerNear { get; set; }

            /// <summary>
            /// Bottom Track Reference Layer Far.
            /// 
            /// Stores the minimum layer size, the near boundary, and the far
            /// boundary of the BT water-reference layer (BL-command).
            /// 
            /// Scaling (minimum layer size): LSD = 1 dm; Range = 0-999dm
            /// 
            /// Scaling (near/far boundaries): LSD = 1 dm; Range = 0-9999dm
            /// </summary>
            public ushort BtRefLayerFar { get; set; }

            #endregion

            #region Reference Layers Correlation

            /// <summary>
            /// Bottom Track Reference Layer Correlation Beam 0.
            /// 
            /// Contains correlation magnitude data for the water reference
            /// layer for each beam. Reference layer correlation magnitudes
            /// have the same format and scale factor as water-profiling
            /// magnitudes (Table 5).
            /// </summary>
            public byte BtRefLayerCorrBeam0 { get; set; }

            /// <summary>
            /// Bottom Track Reference Layer Correlation Beam 1.
            /// 
            /// Contains correlation magnitude data for the water reference
            /// layer for each beam. Reference layer correlation magnitudes
            /// have the same format and scale factor as water-profiling
            /// magnitudes (Table 5).
            /// </summary>
            public byte BtRefLayerCorrBeam1 { get; set; }

            /// <summary>
            /// Bottom Track Reference Layer Correlation Beam 2.
            /// 
            /// Contains correlation magnitude data for the water reference
            /// layer for each beam. Reference layer correlation magnitudes
            /// have the same format and scale factor as water-profiling
            /// magnitudes (Table 5).
            /// </summary>
            public byte BtRefLayerCorrBeam2 { get; set; }

            /// <summary>
            /// Bottom Track Reference Layer Correlation Beam 3.
            /// 
            /// Contains correlation magnitude data for the water reference
            /// layer for each beam. Reference layer correlation magnitudes
            /// have the same format and scale factor as water-profiling
            /// magnitudes (Table 5).
            /// </summary>
            public byte BtRefLayerCorrBeam3 { get; set; }

            #endregion

            #region Reference Layer Echo Intensity

            /// <summary>
            /// Bottom Track Reference Layer Echo Intensity Beam 0.
            /// 
            /// Contains echo intensity data for the reference layer for each
            /// beam. Reference layer intensities have the same format and
            /// scale factor as water-profiling intensities.
            /// </summary>
            public byte BtRefLayerEchoIntensityBeam0 { get; set; }

            /// <summary>
            /// Bottom Track Reference Layer Echo Intensity Beam 1.
            /// 
            /// Contains echo intensity data for the reference layer for each
            /// beam. Reference layer intensities have the same format and
            /// scale factor as water-profiling intensities.
            /// </summary>
            public byte BtRefLayerEchoIntensityBeam1 { get; set; }

            /// <summary>
            /// Bottom Track Reference Layer Echo Intensity Beam 2.
            /// 
            /// Contains echo intensity data for the reference layer for each
            /// beam. Reference layer intensities have the same format and
            /// scale factor as water-profiling intensities.
            /// </summary>
            public byte BtRefLayerEchoIntensityBeam2 { get; set; }

            /// <summary>
            /// Bottom Track Reference Layer Echo Intensity Beam 3.
            /// 
            /// Contains echo intensity data for the reference layer for each
            /// beam. Reference layer intensities have the same format and
            /// scale factor as water-profiling intensities.
            /// </summary>
            public byte BtRefLayerEchoIntensityBeam3 { get; set; }

            #endregion

            #region Reference Layer Percent Good

            /// <summary>
            /// Bottom Track Reference Layer Percent Good Beam 0.
            /// 
            /// Contains percent-good data for the water reference layer for
            /// each beam. They indicate the reliability of reference layer
            /// data. It is the percentage of bottom-track pings that have
            /// passed a reference layer validity algorithm during an ensemble.
            /// 
            /// Scaling: LSD = 1 percent; Range = 0 to 100 percent
            /// </summary>
            public byte BtRefLayerPercentGoodBeam0 { get; set; }

            /// <summary>
            /// Bottom Track Reference Layer Percent Good Beam 1.
            /// 
            /// Contains percent-good data for the water reference layer for
            /// each beam. They indicate the reliability of reference layer
            /// data. It is the percentage of bottom-track pings that have
            /// passed a reference layer validity algorithm during an ensemble.
            /// 
            /// Scaling: LSD = 1 percent; Range = 0 to 100 percent
            /// </summary>
            public byte BtRefLayerPercentGoodBeam1 { get; set; }

            /// <summary>
            /// Bottom Track Reference Layer Percent Good Beam 2.
            /// 
            /// Contains percent-good data for the water reference layer for
            /// each beam. They indicate the reliability of reference layer
            /// data. It is the percentage of bottom-track pings that have
            /// passed a reference layer validity algorithm during an ensemble.
            /// 
            /// Scaling: LSD = 1 percent; Range = 0 to 100 percent
            /// </summary>
            public byte BtRefLayerPercentGoodBeam2 { get; set; }

            /// <summary>
            /// Bottom Track Reference Layer Percent Good Beam 3.
            /// 
            /// Contains percent-good data for the water reference layer for
            /// each beam. They indicate the reliability of reference layer
            /// data. It is the percentage of bottom-track pings that have
            /// passed a reference layer validity algorithm during an ensemble.
            /// 
            /// Scaling: LSD = 1 percent; Range = 0 to 100 percent
            /// </summary>
            public byte BtRefLayerPercentGoodBeam3 { get; set; }

            #endregion

            #endregion

            #endregion

            /// <summary>
            /// Create an Ship Water Mass Velocity data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public ShipWaterMassDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                VelocityTransverse = Ensemble.EMPTY_VELOCITY;
                VelocityLongitudinal = Ensemble.EMPTY_VELOCITY;
                VelocityNormal = Ensemble.EMPTY_VELOCITY;
                VelocityQ = Ensemble.EMPTY_VELOCITY;
                WaterMassDepthLayer = 0;

                BtRefLayerFar = 0;
                BtRefLayerMin = 0;
                BtRefLayerNear = 0;
                BtRefLayerCorrBeam0 = 0;
                BtRefLayerCorrBeam1 = 0;
                BtRefLayerCorrBeam2 = 0;
                BtRefLayerCorrBeam3 = 0;
                BtRefLayerEchoIntensityBeam0 = 0;
                BtRefLayerEchoIntensityBeam1 = 0;
                BtRefLayerEchoIntensityBeam2 = 0;
                BtRefLayerEchoIntensityBeam3 = 0;
                BtRefLayerEchoIntensityBeam0 = 0;
                BtRefLayerEchoIntensityBeam1 = 0;
                BtRefLayerEchoIntensityBeam2 = 0;
                BtRefLayerEchoIntensityBeam3 = 0;
            }

            /// <summary>
            /// Create an empty Instrument Water Mass Velocity data set.
            /// </summary>
            public ShipWaterMassDataSet() :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.WaterMassShipID)
            {
                // Initialize data
                VelocityTransverse = Ensemble.EMPTY_VELOCITY;
                VelocityLongitudinal = Ensemble.EMPTY_VELOCITY;
                VelocityNormal = Ensemble.EMPTY_VELOCITY;
                VelocityQ = Ensemble.EMPTY_VELOCITY;
                WaterMassDepthLayer = 0;

                BtRefLayerFar = 0;
                BtRefLayerMin = 0;
                BtRefLayerNear = 0;
                BtRefLayerCorrBeam0 = 0;
                BtRefLayerCorrBeam1 = 0;
                BtRefLayerCorrBeam2 = 0;
                BtRefLayerCorrBeam3 = 0;
                BtRefLayerEchoIntensityBeam0 = 0;
                BtRefLayerEchoIntensityBeam1 = 0;
                BtRefLayerEchoIntensityBeam2 = 0;
                BtRefLayerEchoIntensityBeam3 = 0;
                BtRefLayerEchoIntensityBeam0 = 0;
                BtRefLayerEchoIntensityBeam1 = 0;
                BtRefLayerEchoIntensityBeam2 = 0;
                BtRefLayerEchoIntensityBeam3 = 0;
            }

            /// <summary>
            /// Create an Ship Water Mass Velocity data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.InstrumentWaterMassDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 162ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.InstrumentWaterMassDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.InstrumentWaterMassDataSet}(json); 
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
            /// <param name="VelocityTrans">Water Mass Transverse Velocity in m/s.</param>
            /// <param name="VelocityLongitudinal">Water Mass Longitudinal Velocity in m/s.</param>
            /// <param name="VelocityNormal">Water Mass Normal Velocity in m/s.</param>
            /// <param name="WaterMassDepthLayer">Depth layer of the Water Mass measurement in meters.</param>
            [JsonConstructor]
            public ShipWaterMassDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name,
                                            float VelocityTrans, float VelocityLongitudinal, float VelocityNormal, float WaterMassDepthLayer) :
                base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                // Initialize data
                this.VelocityTransverse = VelocityTrans;
                this.VelocityLongitudinal = VelocityLongitudinal;
                this.VelocityNormal = VelocityNormal;
                this.WaterMassDepthLayer = WaterMassDepthLayer;

                VelocityQ = Ensemble.EMPTY_VELOCITY;

                BtRefLayerFar = 0;
                BtRefLayerMin = 0;
                BtRefLayerNear = 0;
                BtRefLayerCorrBeam0 = 0;
                BtRefLayerCorrBeam1 = 0;
                BtRefLayerCorrBeam2 = 0;
                BtRefLayerCorrBeam3 = 0;
                BtRefLayerEchoIntensityBeam0 = 0;
                BtRefLayerEchoIntensityBeam1 = 0;
                BtRefLayerEchoIntensityBeam2 = 0;
                BtRefLayerEchoIntensityBeam3 = 0;
                BtRefLayerEchoIntensityBeam0 = 0;
                BtRefLayerEchoIntensityBeam1 = 0;
                BtRefLayerEchoIntensityBeam2 = 0;
                BtRefLayerEchoIntensityBeam3 = 0;
            }

            /// <summary>
            /// Convert the PD0 Bottom Track data type to the RTI Bottom Track data set.
            /// </summary>
            /// <param name="bt">PD0 Bottom Track.</param>
            /// <param name="xform">Coordinate Transform.</param>
            /// <param name="vl">Variable Leader.</param>
            public void DecodePd0Ensemble(Pd0BottomTrack bt, PD0.CoordinateTransforms xform, Pd0VariableLeader vl)
            {
                #region Velocity Data

                this.WaterMassDepthLayer = ((bt.BtRefLayerNear + bt.BtRefLayerFar) / 2.0f) / 10.0f;  // Divide by 10 to convert DM to M

                // Set velocities and check for bad velocities
                if (bt.BtRefLayerVelocityBeam0 == PD0.BAD_VELOCITY)
                {
                    this.VelocityTransverse = Ensemble.EMPTY_VELOCITY;
                }
                else
                {
                    this.VelocityTransverse = bt.BtRefLayerVelocityBeam0 / 1000.0f;
                }

                if (bt.BtRefLayerVelocityBeam1 == PD0.BAD_VELOCITY)
                {
                    this.VelocityLongitudinal = Ensemble.EMPTY_VELOCITY;
                }
                else
                {
                    this.VelocityLongitudinal = bt.BtRefLayerVelocityBeam1 / 1000.0f;
                }

                if (bt.BtRefLayerVelocityBeam2 == PD0.BAD_VELOCITY)
                {
                    this.VelocityNormal = Ensemble.EMPTY_VELOCITY;
                }
                else
                {
                    this.VelocityNormal = bt.BtRefLayerVelocityBeam2 / 1000.0f;
                }

                if (bt.BtRefLayerVelocityBeam3 == PD0.BAD_VELOCITY)
                {
                    this.VelocityQ = Ensemble.EMPTY_VELOCITY;
                }
                else
                {
                    this.VelocityQ = bt.BtRefLayerVelocityBeam3 / 1000.0f;
                }

                #endregion

                BtRefLayerFar = bt.BtRefLayerFar;
                BtRefLayerMin = bt.BtRefLayerMin;
                BtRefLayerNear = bt.BtRefLayerNear;
                BtRefLayerCorrBeam0 = bt.BtRefLayerCorrBeam0;                           // Should divide by 255 to match RTB correlation range
                BtRefLayerCorrBeam1 = bt.BtRefLayerCorrBeam1;
                BtRefLayerCorrBeam2 = bt.BtRefLayerCorrBeam2;
                BtRefLayerCorrBeam3 = bt.BtRefLayerCorrBeam3;
                BtRefLayerEchoIntensityBeam0 = bt.BtRefLayerEchoIntensityBeam0;         // Should divide by 2 to match RTB amplitude db range
                BtRefLayerEchoIntensityBeam1 = bt.BtRefLayerEchoIntensityBeam1;
                BtRefLayerEchoIntensityBeam2 = bt.BtRefLayerEchoIntensityBeam2;
                BtRefLayerEchoIntensityBeam3 = bt.BtRefLayerEchoIntensityBeam3;
                BtRefLayerPercentGoodBeam0 = bt.BtRefLayerPercentGoodBeam0;
                BtRefLayerPercentGoodBeam1 = bt.BtRefLayerPercentGoodBeam1;
                BtRefLayerPercentGoodBeam2 = bt.BtRefLayerPercentGoodBeam2;
                BtRefLayerPercentGoodBeam3 = bt.BtRefLayerPercentGoodBeam3;

            }

            /// <summary>
            /// Override the ToString to return all the velocity data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("Transverse : " + VelocityTransverse.ToString() + "\n");
                builder.Append("Longitudinal : " + VelocityLongitudinal.ToString() + "\n");
                builder.Append("Normal : " + VelocityNormal.ToString() + "\n");
                builder.Append("Depth Layer : " + WaterMassDepthLayer.ToString());

                return builder.ToString();
            }
        }

        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.InstrumentWaterMassDat).
        /// 
        /// 6ms for this method.
        /// 12ms for calling SerializeObject default.
        /// 
        /// Use this method whenever possible to convert to JSON.
        /// 
        /// http://james.newtonking.com/projects/json/help/
        /// http://james.newtonking.com/projects/json/help/index.html?topic=html/ReadingWritingJSON.htm
        /// http://blog.maskalik.com/asp-net/json-net-implement-custom-serialization
        /// </summary>
        public class ShipWaterMassDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.ShipWaterMassData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as ShipWaterMassDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // Velocity Transverse
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VEL_TRANS);
                writer.WriteValue(data.VelocityTransverse);

                // Velocity Longitudinal
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VEL_LONG);
                writer.WriteValue(data.VelocityLongitudinal);

                // Velocity Normal
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VEL_NORM);
                writer.WriteValue(data.VelocityNormal);

                // Velocity Error
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VELQ);
                writer.WriteValue(data.VelocityQ);

                // Water Mass Depth Layer
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WATERMASSDEPTHLAYER);
                writer.WriteValue(data.WaterMassDepthLayer);

                // Water Mass Depth Layer Far
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_LAYER_FAR);
                writer.WriteValue(data.BtRefLayerFar);

                // Water Mass Depth Layer Min
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_LAYER_MIN);
                writer.WriteValue(data.BtRefLayerMin);

                // Water Mass Depth Layer Near
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_LAYER_NEAR);
                writer.WriteValue(data.BtRefLayerNear);

                // Water Mass Depth Layer Correlation Beam 0
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_CORR_0);
                writer.WriteValue(data.BtRefLayerCorrBeam0);

                // Water Mass Depth Layer Correlation Beam 1
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_CORR_1);
                writer.WriteValue(data.BtRefLayerCorrBeam1);

                // Water Mass Depth Layer Correlation Beam 2
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_CORR_2);
                writer.WriteValue(data.BtRefLayerCorrBeam2);

                // Water Mass Depth Layer Correlation Beam 3
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_CORR_3);
                writer.WriteValue(data.BtRefLayerCorrBeam3);

                // Water Mass Depth Layer Echo Intensity Beam 0
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_ECHO_0);
                writer.WriteValue(data.BtRefLayerEchoIntensityBeam0);

                // Water Mass Depth Layer Echo Intensity Beam 1
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_ECHO_1);
                writer.WriteValue(data.BtRefLayerEchoIntensityBeam1);

                // Water Mass Depth Layer Echo Intensity Beam 2
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_ECHO_2);
                writer.WriteValue(data.BtRefLayerEchoIntensityBeam2);

                // Water Mass Depth Layer Echo Intensity Beam 3
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_ECHO_3);
                writer.WriteValue(data.BtRefLayerEchoIntensityBeam3);

                // Water Mass Depth Layer Percent Good Beam 0
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_PG_0);
                writer.WriteValue(data.BtRefLayerPercentGoodBeam0);

                // Water Mass Depth Layer Percent Good Beam 1
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_PG_1);
                writer.WriteValue(data.BtRefLayerPercentGoodBeam1);

                // Water Mass Depth Layer Percent Good Beam 2
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_PG_2);
                writer.WriteValue(data.BtRefLayerPercentGoodBeam2);

                // Water Mass Depth Layer Percent Good Beam 3
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WM_PG_3);
                writer.WriteValue(data.BtRefLayerPercentGoodBeam3);
                // End the object
                writer.WriteEndObject();
            }

            /// <summary>
            /// Read the JSON object and convert to the object.  This will allow the serializer to
            /// automatically convert the object.  No special instructions need to be done and all
            /// the properties found in the JSON string need to be used.
            /// 
            /// DataSet.ShipWaterMassDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.ShipWaterMassDataSet}(encodedEns)
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
                    var data = new ShipWaterMassDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.WaterMassShipID);

                    // Velocity Transverse
                    data.VelocityTransverse = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_VEL_TRANS];

                    // Velocity Longitudinal
                    data.VelocityLongitudinal = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_VEL_LONG];

                    // Velocity Normal
                    data.VelocityNormal = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_VEL_NORM];

                    // Velocity Error
                    data.VelocityQ = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_VELQ];

                    // Water Mass Depth Layer
                    data.WaterMassDepthLayer = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_WATERMASSDEPTHLAYER];

                    // Water Mass Depth Layer Far
                    data.BtRefLayerFar = (ushort)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_LAYER_FAR];

                    // Water Mass Depth Layer Min
                    data.BtRefLayerMin = (ushort)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_LAYER_MIN];

                    // Water Mass Depth Layer Near
                    data.BtRefLayerNear = (ushort)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_LAYER_NEAR];

                    // Water Mass Correlation Beam 0
                    data.BtRefLayerCorrBeam0 = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_CORR_0];

                    // Water Mass Correlation Beam 1
                    data.BtRefLayerCorrBeam1 = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_CORR_1];

                    // Water Mass Correlation Beam 2
                    data.BtRefLayerCorrBeam2 = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_CORR_2];

                    // Water Mass Correlation Beam 3
                    data.BtRefLayerCorrBeam3 = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_CORR_3];

                    // Water Mass Percent Good Beam 0
                    data.BtRefLayerPercentGoodBeam0 = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_PG_0];

                    // Water Mass Percent Good Beam 1
                    data.BtRefLayerPercentGoodBeam1 = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_PG_1];

                    // Water Mass Percent Good Beam 2
                    data.BtRefLayerPercentGoodBeam2 = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_PG_2];

                    // Water Mass Percent Good Beam 3
                    data.BtRefLayerPercentGoodBeam3 = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_PG_3];

                    // Water Mass Echo Intensity Beam 0
                    data.BtRefLayerEchoIntensityBeam0 = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_ECHO_0];

                    // Water Mass Echo Intensity Beam 1
                    data.BtRefLayerEchoIntensityBeam1 = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_ECHO_1];

                    // Water Mass Echo Intensity Beam 2
                    data.BtRefLayerEchoIntensityBeam2 = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_ECHO_2];

                    // Water Mass Echo Intensity Beam 3
                    data.BtRefLayerEchoIntensityBeam3 = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_WM_ECHO_3];
                    
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
                return typeof(ShipWaterMassDataSet).IsAssignableFrom(objectType);
            }
        }

    }
}