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
 * 12/08/2011      RC          1.09       Initial coding
 * 02/25/2013      RC          2.18       Removed Orientation.
 *                                         Added JSON encoding and Decoding.
 * 02/06/2014      RC          2.21.3     Added constructor that takes a PRTI03 sentence.  Added Q value.
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
        /// Data set containing all the Water Mass Instrument Velocity data.
        /// </summary>
        [JsonConverter(typeof(InstrumentWaterMassDataSetSerializer))]
        public class InstrumentWaterMassDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// Number of elements within this data set
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 5;

            #endregion

            #region Properties

            /// <summary>
            /// Water Mass X velocity in meters per second.
            /// </summary>
            public float VelocityX { get; set; }

            /// <summary>
            /// Water Mass Y velocity in meters per second.
            /// </summary>
            public float VelocityY { get; set; }

            /// <summary>
            /// Water Mass Z velocity in meters per second.
            /// </summary>
            public float VelocityZ { get; set; }

            /// <summary>
            /// Water Mass Q velocity in meters per second.
            /// </summary>
            public float VelocityQ { get; set; }

            /// <summary>
            /// Depth layer the Water Mass Velocity was taken in meters.
            /// </summary>
            public float WaterMassDepthLayer { get; set; }

            #endregion

            /// <summary>
            /// Create an Instrument Water Mass Velocity data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public InstrumentWaterMassDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                VelocityX = Ensemble.EMPTY_VELOCITY;
                VelocityY = Ensemble.EMPTY_VELOCITY;
                VelocityZ = Ensemble.EMPTY_VELOCITY;
                VelocityQ = Ensemble.EMPTY_VELOCITY;
                WaterMassDepthLayer = 0;
            }

            /// <summary>
            /// Create an empty Instrument Water Mass Velocity data set.
            /// </summary>
            public InstrumentWaterMassDataSet() :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.WaterMassInstrumentID)
            {
                // Initialize data
                VelocityX = Ensemble.EMPTY_VELOCITY;
                VelocityY = Ensemble.EMPTY_VELOCITY;
                VelocityZ = Ensemble.EMPTY_VELOCITY;
                VelocityQ = Ensemble.EMPTY_VELOCITY;
                WaterMassDepthLayer = 0;
            }

            ///// <summary>
            ///// Create an Instrument Water Mass Velocity data set.  Include all the information to
            ///// create the data set.
            ///// </summary>
            ///// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            ///// <param name="numBins">Number of Bin</param>
            ///// <param name="numBeams">Number of beams</param>
            ///// <param name="imag"></param>
            ///// <param name="nameLength">Length of name</param>
            ///// <param name="name">Name of data type</param>
            ///// <param name="x">Water Mass East Velocity in m/s.</param>
            ///// <param name="y">Water Mass North Velocity in m/s.</param>
            ///// <param name="z">Water Mass Vertical Velocity in m/s.</param>
            ///// <param name="depthLayer">Depth layer of the Water Mass measurement in meters.</param>
            ///// <param name="orientation">Orientation of the beams.  Default Down.</param>
            //public InstrumentWaterMassDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, float x, float y, float z, float depthLayer) :
            //    base(valueType, numBins, numBeams, imag, nameLength, name)
            //{
            //    // Initialize data
            //    VelocityX = x;
            //    VelocityY = y;
            //    VelocityZ = z;
            //    WaterMassDepthLayer = depthLayer;
            //}

            /// <summary>
            /// Create an Insturment Water Mass Velocity data set.  Include all the information
            /// to create the data set from the NMEA sentence.
            /// </summary>
            /// <param name="sentence">NMEA sentence containing the data.</param>
            public InstrumentWaterMassDataSet(Prti01Sentence sentence) :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.WaterMassInstrumentID)
            {
                // Initialize data
                VelocityX = Convert.ToSingle(sentence.WaterMassVelX.ToMetersPerSecond().Value);
                VelocityY = Convert.ToSingle(sentence.WaterMassVelY.ToMetersPerSecond().Value);
                VelocityZ = Convert.ToSingle(sentence.WaterMassVelZ.ToMetersPerSecond().Value);
                VelocityQ = Ensemble.EMPTY_VELOCITY;
                WaterMassDepthLayer = Convert.ToSingle(sentence.WaterMassDepth.ToMeters().Value);
            }

            /// <summary>
            /// Create an Insturment Water Mass Velocity data set.  Include all the information
            /// to create the data set from the NMEA sentence.
            /// </summary>
            /// <param name="sentence">NMEA sentence containing the data.</param>
            public InstrumentWaterMassDataSet(Prti03Sentence sentence) :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.WaterMassInstrumentID)
            {
                // Initialize data
                VelocityX = Convert.ToSingle(sentence.WaterMassVelX.ToMetersPerSecond().Value);
                VelocityY = Convert.ToSingle(sentence.WaterMassVelY.ToMetersPerSecond().Value);
                VelocityZ = Convert.ToSingle(sentence.WaterMassVelZ.ToMetersPerSecond().Value);
                VelocityQ = Convert.ToSingle(sentence.WaterMassVelQ.ToMetersPerSecond().Value);
                WaterMassDepthLayer = Convert.ToSingle(sentence.WaterMassDepth.ToMeters().Value);
            }

            /// <summary>
            /// Create an Instrument Water Mass Velocity data set.  Intended for JSON  deserialize.  This method
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
            /// <param name="VelocityX">Water Mass X Velocity in m/s.</param>
            /// <param name="VelocityY">Water Mass Y Velocity in m/s.</param>
            /// <param name="VelocityZ">Water Mass Z Velocity in m/s.</param>
            /// <param name="VelocityQ">Water Mass Q Velocity in m/s.</param>
            /// <param name="WaterMassDepthLayer">Depth layer of the Water Mass measurement in meters.</param>
            [JsonConstructor]
            public InstrumentWaterMassDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name, float VelocityX, float VelocityY, float VelocityZ, float VelocityQ, float WaterMassDepthLayer) :
                base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                // Initialize data
                this.VelocityX = VelocityX;
                this.VelocityY = VelocityY;
                this.VelocityZ = VelocityZ;
                this.VelocityQ = VelocityQ;
                this.WaterMassDepthLayer = WaterMassDepthLayer;
            }

            /// <summary>
            /// Override the ToString to return all the velocity data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("X : " + VelocityX.ToString() + "\n");
                builder.Append("Y : " + VelocityY.ToString() + "\n");
                builder.Append("Z : " + VelocityZ.ToString() + "\n");
                builder.Append("Q : " + VelocityQ.ToString() + "\n");
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
        public class InstrumentWaterMassDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.InstrumentWaterMassData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as InstrumentWaterMassDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // Velocity X
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VELX);
                writer.WriteValue(data.VelocityX);

                // Velocity Y
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VELY);
                writer.WriteValue(data.VelocityY);

                // Velocity Z
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VELZ);
                writer.WriteValue(data.VelocityZ);

                // Velocity Q
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VELQ);
                writer.WriteValue(data.VelocityQ);

                // Water Mass Depth Layer
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WATERMASSDEPTHLAYER);
                writer.WriteValue(data.WaterMassDepthLayer);

                // End the object
                writer.WriteEndObject();
            }

            /// <summary>
            /// Read the JSON object and convert to the object.  This will allow the serializer to
            /// automatically convert the object.  No special instructions need to be done and all
            /// the properties found in the JSON string need to be used.
            /// 
            /// DataSet.InstrumentWaterMassDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.InstrumentWaterMassDataSet}(encodedEns)
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
                    var data = new InstrumentWaterMassDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.WaterMassInstrumentID);

                    // Velocity X
                    data.VelocityX = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_VELX];

                    // Velocity Y
                    data.VelocityY = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_VELY];

                    // Velocity Z
                    data.VelocityZ = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_VELZ];

                    // Velocity Q
                    data.VelocityQ = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_VELQ];

                    // Water Mass Depth Layer
                    data.WaterMassDepthLayer = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_WATERMASSDEPTHLAYER];

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
                return typeof(InstrumentWaterMassDataSet).IsAssignableFrom(objectType);
            }
        }

    }
}