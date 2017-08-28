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
 * 08/25/2017      RC          3.4.2      Initial coding
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
        /// Data set containing all the EarthVelocity data.
        /// </summary>
        [JsonConverter(typeof(ShipVelocityDataSetSerializer))]
        public class ShipVelocityDataSet : BaseDataSet
        {
            #region Properties

            /// <summary>
            /// Store all the Ship velocity data for the ADCP.
            /// [bin,beam]
            /// </summary>
            public float[,] ShipVelocityData { get; set; }

            #endregion

            /// <summary>
            /// Create an Ship Velocity data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public ShipVelocityDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                ShipVelocityData = new float[NumElements, ElementsMultiplier];
            }

            /// <summary>
            /// Create an Ship Velocity data set.
            /// </summary>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams.  Default uses DEFAULT_NUM_BEAMS_BEAM.</param>
            public ShipVelocityDataSet(int numBins, int numBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM) :
                base(DataSet.Ensemble.DATATYPE_FLOAT,                   // Type of data stored (Float or Int)
                        numBins,                                        // Number of bins
                        numBeams,                                       // Number of beams
                        DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                        DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                        DataSet.Ensemble.ShipVelocityID)               // Dataset ID
            {
                // Initialize data
                ShipVelocityData = new float[NumElements, ElementsMultiplier];
            }

            /// <summary>
            /// Create an Ship Velocity data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="velocityData">Byte array containing ShipVelocity velocity data</param>
            public ShipVelocityDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] velocityData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                ShipVelocityData = new float[NumElements, ElementsMultiplier];

                // Decode the byte array for velocity data
                Decode(velocityData);
            }

            /// <summary>
            /// Create an Earth Velocity data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.ShipVelocityDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 65ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.ShipVelocityDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.ShipVelocityDataSet}(json); 
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
            /// <param name="ShipVelocityData">2D Array containing Earth velocity data. [Bin, Beam]</param>
            [JsonConstructor]
            private ShipVelocityDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name, float[,] ShipVelocityData) :
                base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                // Initialize data
                this.ShipVelocityData = ShipVelocityData;
            }

            /// <summary>
            /// Get all the Earth Velocity ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin x Beams.
            /// </summary>
            /// <param name="dataType">Byte array containing the Earth Velocity data type.</param>
            private void Decode(byte[] dataType)
            {
                int index = 0;

                for (int beam = 0; beam < ElementsMultiplier; beam++)
                {
                    for (int bin = 0; bin < NumElements; bin++)
                    {
                        index = GetBinBeamIndex(NameLength, NumElements, beam, bin);
                        ShipVelocityData[bin, beam] = MathHelper.ByteArrayToFloat(dataType, index);
                    }
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
                // Calculate the payload size
                int payloadSize = (NumElements * ElementsMultiplier * Ensemble.BYTES_IN_FLOAT);

                // The size of the array is the header of the dataset
                // and the binxbeams value with each value being a float.
                byte[] result = new byte[GetBaseDataSize(NameLength) + payloadSize];

                // Add the header to the byte array
                byte[] header = GenerateHeader(NumElements);
                System.Buffer.BlockCopy(header, 0, result, 0, header.Length);

                // Add the payload to the results
                int index = 0;
                for (int beam = 0; beam < ElementsMultiplier; beam++)
                {
                    for (int bin = 0; bin < NumElements; bin++)
                    {
                        // Get the index for the next element and add to the array
                        index = GetBinBeamIndex(NameLength, NumElements, beam, bin);
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ShipVelocityData[bin, beam]), 0, result, index, Ensemble.BYTES_IN_FLOAT);
                    }
                }

                return result;
            }

            /// <summary>
            /// Override the ToString to return all the velocity data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string s = "";
                for (int bin = 0; bin < NumElements; bin++)
                {
                    s += "E Bin: " + bin + "\t";
                    for (int beam = 0; beam < ElementsMultiplier; beam++)
                    {
                        s += "\t" + beam + ": " + ShipVelocityData[bin, beam];
                    }
                    s += "\n";
                }

                return s;
            }

            #region PD0 Ensemble

            /// <summary>
            /// Convert the Pd0 Velocity data type to the RTI Ship Velocity data set.
            /// </summary>
            /// <param name="vel">PD0 Velocity.</param>
            public void DecodePd0Ensemble(Pd0Velocity vel)
            {
                if (vel.Velocities != null)
                {
                    ShipVelocityData = new float[vel.Velocities.GetLength(0), vel.Velocities.GetLength(1)];

                    for (int bin = 0; bin < vel.Velocities.GetLength(0); bin++)
                    {
                        for (int beam = 0; beam < vel.Velocities.GetLength(1); beam++)
                        {
                            // beam order 3,2,0,1; XYZ order 1,0,-2,3, ENU order 0,1,2,3

                            // Check for bad velocity
                            if (vel.Velocities[bin, beam] != PD0.BAD_VELOCITY)
                            {
                                ShipVelocityData[bin, beam] = vel.Velocities[bin, beam] / 1000.0f;   // m/s to mm/s 
                            }
                            else
                            {
                                // Bad velocity
                                ShipVelocityData[bin, beam] = DataSet.Ensemble.BAD_VELOCITY;
                            }
                        }
                    }
                }
            }

            #endregion

            #region Utilties

            /// <summary>
            /// Return whether any of the Earth Velocity values are bad.
            /// If one is bad, they are all bad.  This will check the given bin.
            /// If we do not allow 3 Beam solution and Q is bad, then all is bad.
            /// </summary>
            /// <param name="bin">Bin to check.</param>
            /// <param name="allow3BeamSolution">Allow a 3 Beam solution. Default = true</param>
            /// <returns>TRUE = All values good / False = One or more of the values are bad.</returns>
            public bool IsBinGood(int bin, bool allow3BeamSolution = true)
            {
                if (ElementsMultiplier > DataSet.Ensemble.BEAM_Q_INDEX)
                {
                    // If the Q is bad and we do not allow 3 Beam solution, then all is bad.
                    if (ShipVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] == DataSet.Ensemble.BAD_VELOCITY && !allow3BeamSolution)
                    {
                        return false;
                    }
                }

                bool b0 = false;
                bool b1 = false;
                bool b2 = false;
                if (ElementsMultiplier > DataSet.Ensemble.BEAM_EAST_INDEX)
                {
                    b0 = ShipVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] == DataSet.Ensemble.BAD_VELOCITY;
                }
                if (ElementsMultiplier > DataSet.Ensemble.BEAM_NORTH_INDEX)
                {
                    b1 = ShipVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] == DataSet.Ensemble.BAD_VELOCITY;
                }
                if (ElementsMultiplier > DataSet.Ensemble.BEAM_VERTICAL_INDEX)
                {
                    b2 = ShipVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] == DataSet.Ensemble.BAD_VELOCITY;
                }

                // Check if any of the beams are bad
                if (b0 || b1 || b2 )
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Calculate the magnitude of the velocity.  Use Earth form East, North and Vertical velocity to 
            /// calculate the value.
            /// </summary>
            /// <param name="bin">Bin to get the Velocity Magnitude.</param>
            /// <returns>Magnitude of Velocity. If any velocities were bad, DataSet.AdcpDataSet.BAD_VELOCITY is returned.</returns>
            public double GetVelocityMagnitude(int bin)
            {
                try
                {
                    // Ensure the velocities are good
                    if (ShipVelocityData.GetLength(1) >= 3)
                    {
                        if ((ShipVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] != Ensemble.BAD_VELOCITY) &&
                            (ShipVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] != Ensemble.BAD_VELOCITY) &&
                            (ShipVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] != Ensemble.BAD_VELOCITY))
                        {
                            return (MathHelper.CalculateMagnitude(ShipVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX], ShipVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX], ShipVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX]));
                        }
                    }

                    return DataSet.Ensemble.BAD_VELOCITY;
                }
                catch (System.IndexOutOfRangeException)
                {
                    // When the display is changing from one dataset to another, 
                    // the number of bins could change and then it could select out of range.
                    return DataSet.Ensemble.BAD_VELOCITY;
                }
            }

            /// <summary>
            /// Get the direction for the velocity.  Use the parameter
            /// to determine if the Y axis is North or East.  
            /// </summary>
            /// <param name="isYNorth">Set flag if you want Y axis to be North or East.</param>
            /// <param name="bin">Bin to get the Velocity Direction.</param>
            /// <returns>Direction of the velocity in degrees.  If any velocities were bad, return -1.</returns>
            public double GetVelocityDirection(bool isYNorth, int bin)
            {
                try
                {
                    // Ensure the velocities are good
                    if (ShipVelocityData.GetLength(1) >= 3)
                    {
                        if ((ShipVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] != Ensemble.BAD_VELOCITY) &&
                            (ShipVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] != Ensemble.BAD_VELOCITY) &&
                            (ShipVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] != Ensemble.BAD_VELOCITY))
                        {
                            if (isYNorth)
                            {
                                //return (Math.Atan2(EarthVelocityData[Bin, 1], EarthVelocityData[Bin, 0])) * (180 / Math.PI);
                                return MathHelper.CalculateDirection(ShipVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX], ShipVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX]);
                            }
                            else
                            {
                                //return (Math.Atan2(EarthVelocityData[Bin, 0], EarthVelocityData[Bin, 1])) * (180 / Math.PI);
                                return MathHelper.CalculateDirection(ShipVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX], ShipVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX]);
                            }
                        }
                    }

                    return -1;
                }
                catch (System.IndexOutOfRangeException)
                {
                    // When the display is changing from one dataset to another, 
                    // the number of bins could change and then it could select out of range.
                    return -1;
                }
            }

            #endregion
        }

        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.ShipVelocityData).
        /// 
        /// 65ms for this method.
        /// 100ms for calling SerializeObject default.
        /// 
        /// Use this method whenever possible to convert to JSON.
        /// 
        /// http://james.newtonking.com/projects/json/help/
        /// http://james.newtonking.com/projects/json/help/index.html?topic=html/ReadingWritingJSON.htm
        /// http://blog.maskalik.com/asp-net/json-net-implement-custom-serialization
        /// </summary>
        public class ShipVelocityDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.ShipVelocityData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as ShipVelocityDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // Write the float[,] array data
                // This will be an array of arrays
                // Each array element will contain an array with the 4 beam's value
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_EARTHVELOCITYDATA);
                writer.WriteStartArray();
                for (int bin = 0; bin < data.ShipVelocityData.GetLength(0); bin++)
                {
                    // Write an array of float values for each beam's value
                    writer.WriteStartArray();

                    for (int beam = 0; beam < data.ShipVelocityData.GetLength(1); beam++)
                    {
                        writer.WriteValue(data.ShipVelocityData[bin, beam]);
                    }

                    writer.WriteEndArray();
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
            /// DataSet.ShipVelocityDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.ShipVelocityDataSet}(encodedEns)
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
                    var data = new ShipVelocityDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.ShipVelocityID);
                    data.ShipVelocityData = new float[NumElements, ElementsMultiplier];

                    // Decode the 2D array 
                    JArray jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_EARTHVELOCITYDATA];
                    if (jArray.Count <= NumElements)                                                            // Verify size
                    {
                        for (int bin = 0; bin < jArray.Count; bin++)
                        {
                            JArray arrayData = (JArray)jArray[bin];
                            if (arrayData.Count <= ElementsMultiplier)                                          // Verify size
                            {
                                for (int beam = 0; beam < arrayData.Count; beam++)
                                {
                                    data.ShipVelocityData[bin, beam] = (float)arrayData[beam];
                                }
                            }
                        }
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
                return typeof(ShipVelocityDataSet).IsAssignableFrom(objectType);
            }
        }
    }
}