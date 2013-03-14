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
        /// Data set containing all the Water Mass Earth Velocity data.
        /// </summary>
        [JsonConverter(typeof(EarthWaterMassDataSetSerializer))]
        public class EarthWaterMassDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// Number of elements within this data set
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 4;

            #endregion

            #region Properties

            /// <summary>
            /// Water Mass East velocity in meters per second.
            /// </summary>
            public float VelocityEast { get; set; }

            /// <summary>
            /// Water Mass North velocity in meters per second.
            /// </summary>
            public float VelocityNorth { get; set; }

            /// <summary>
            /// Water Mass Vertical velocity in meters per second.
            /// </summary>
            public float VelocityVertical { get; set; }

            /// <summary>
            /// Depth layer the Water Mass Velocity was taken in meters.
            /// </summary>
            public float WaterMassDepthLayer { get; set; }

            #endregion

            /// <summary>
            /// Create an Earth Water Mass Velocity data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public EarthWaterMassDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                VelocityEast = Ensemble.EMPTY_VELOCITY;
                VelocityNorth = Ensemble.EMPTY_VELOCITY;
                VelocityVertical = Ensemble.EMPTY_VELOCITY;
                WaterMassDepthLayer = 0;
            }

            /// <summary>
            /// Create an Earth Water Mass Velocity data set.  Include all the information
            /// to create the data set from the NMEA sentence.
            /// </summary>
            /// <param name="sentence">NMEA sentence containing the data.</param>
            public EarthWaterMassDataSet(Prti02Sentence sentence) :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.WaterMassEarthID)
            {
                // Initialize data
                VelocityEast = Convert.ToSingle(sentence.WaterMassVelEast.ToMetersPerSecond().Value);
                VelocityNorth = Convert.ToSingle(sentence.WaterMassVelNorth.ToMetersPerSecond().Value);
                VelocityVertical = Convert.ToSingle(sentence.WaterMassVelUp.ToMetersPerSecond().Value);
                WaterMassDepthLayer = Convert.ToSingle(sentence.WaterMassDepth.ToMeters().Value);
            }

            /// <summary>
            /// Create an Earth Water Mass Velocity data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.EarthWaterMassDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 162ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.EarthWaterMassDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.EarthWaterMassDataSet}(json); 
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
            /// <param name="VelocityEast">Water Mass East Velocity in m/s.</param>
            /// <param name="VelocityNorth">Water Mass North Velocity in m/s.</param>
            /// <param name="VelocityVertical">Water Mass Vertical Velocity in m/s.</param>
            /// <param name="WaterMassDepthLayer">Depth layer of the Water Mass measurement in meters.</param>
            [JsonConstructor]
            public EarthWaterMassDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name, float VelocityEast, float VelocityNorth, float VelocityVertical, float WaterMassDepthLayer) :
                base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                // Initialize data
                this.VelocityEast = VelocityEast;
                this.VelocityNorth = VelocityNorth;
                this.VelocityVertical = VelocityVertical;
                this.WaterMassDepthLayer = WaterMassDepthLayer;
            }

            /// <summary>
            /// Override the ToString to return all the velocity data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("East : " + VelocityEast.ToString() + "\n");
                builder.Append("North : " + VelocityNorth.ToString() + "\n");
                builder.Append("Vertical : " + VelocityVertical.ToString() + "\n");
                builder.Append("Depth Layer : " + WaterMassDepthLayer.ToString() + "\n");
                

                return builder.ToString();
            }
        }


        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthWaterMassData).
        /// 
        /// 9ms for this method.
        /// 15ms for calling SerializeObject default.
        /// 
        /// Use this method whenever possible to convert to JSON.
        /// 
        /// http://james.newtonking.com/projects/json/help/
        /// http://james.newtonking.com/projects/json/help/index.html?topic=html/ReadingWritingJSON.htm
        /// http://blog.maskalik.com/asp-net/json-net-implement-custom-serialization
        /// </summary>
        public class EarthWaterMassDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthWaterMassData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as EarthWaterMassDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // Velocity East
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VELEAST);
                writer.WriteValue(data.VelocityEast);

                // Velocity North
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VELNORTH);
                writer.WriteValue(data.VelocityNorth);

                // Velocity Vertical
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VELVERTICAL);
                writer.WriteValue(data.VelocityVertical);

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
            /// DataSet.EarthWaterMassDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.EarthWaterMassDataSet}(encodedEns)
            /// 
            /// </summary>
            /// <param name="reader">NOT USED. JSON reader.</param>
            /// <param name="objectType">NOT USED> Type of object.</param>
            /// <param name="existingValue">NOT USED.</param>
            /// <param name="serializer">Serialize the object.</param>
            /// <returns>Serialized object.</returns>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                // Check if anything is written
                if (reader.TokenType != JsonToken.Null)
                {
                    // Load the object
                    JObject jsonObject = JObject.Load(reader);

                    // Decode the data
                    int NumElements = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_NUMELEMENTS];
                    int ElementsMultiplier = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_ELEMENTSMULTIPLIER];

                    // Create the object
                    var data = new EarthWaterMassDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.WaterMassEarthID);

                    // Velocity East
                    data.VelocityEast = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_VELEAST];

                    // Velocity North
                    data.VelocityNorth = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_VELNORTH];

                    // Velocity Vertical
                    data.VelocityVertical = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_VELVERTICAL];

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
                return typeof(EarthWaterMassDataSet).IsAssignableFrom(objectType);
            }
        }
    }
}