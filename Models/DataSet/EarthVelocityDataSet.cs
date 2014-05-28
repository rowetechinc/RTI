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
 * 09/01/2011      RC                     Initial coding
 * 10/04/2011      RC                     Added EarthVelocityBinList to display text.
 *                                         Added method to populate the list.
 * 10/05/2011      RC                     Round ranges to display and look for bad velocity
 * 10/25/2011      RC                     Added new constructor that takes no data and made CreateBinList public.
 * 11/03/2011      RC                     Added methods to get Magnitude, direction and XY points.
 * 11/15/2011      RC                     Added try/catch block in GetVelocityDirection() and GetVelocityMagnitude().  
 * 12/07/2011      RC          1.08       Remove BinList     
 * 12/09/2011      RC          1.09       Make orientation a parameter with a default value.
 * 12/28/2011      RC          1.10       Added GetVelocityVectors() to create array of vectores for velocity.
 * 01/19/2012      RC          1.14       Added Encode() to create a byte array of data.
 *                                         Removed "private set".
 *                                         Rename Decode methods to Decode().
 * 03/30/2012      RC          2.07       Moved Converters.cs methods to MathHelper.cs.
 * 06/20/2012      RC          2.12       Added IsBinGood().
 * 06/21/2012      RC          2.12       Add 3 beam solution option in IsBinGood().
 * 02/25/2013      RC          2.18       Removed Orientation.
 *                                         Added JSON encoding and Decoding.
 *                                         Changed VV to VelocityVectors.
 * 03/04/2013      RC          2.18       Create the VelocityVectors array in the constructor.
 * 05/01/2013      RC          2.19       Added ability to handle single beam data in JSON.
 * 03/25/2014      RC          2.21.4     Added a simpler constructor and added DecodePd0Ensemble().
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
        [JsonConverter(typeof(EarthVelocityDataSetSerializer))]
        public class EarthVelocityDataSet : BaseDataSet
        {
            #region Properties

            /// <summary>
            /// Store all the EarthVelocity velocity data for the ADCP.
            /// </summary>
            public float[,] EarthVelocityData { get; set; }

            /// <summary>
            /// Flag if the Velocity Vector Array is available.  This 
            /// array is created if screening is turned on and an array
            /// was created during the screening process.
            /// </summary>
            public bool IsVelocityVectorAvail { get; set; }

            /// <summary>
            /// Velocity Vector.  This is an array of VelocityVectors.
            /// The VelocityVector holds the magnitude and direciton for the 
            /// water velocity in a bin.  Each bin will have a VelocityVector.
            /// During screening this array can be created.  The screening
            /// will also remove the ship speed before creating the vector.
            /// </summary>
            public VelocityVector[] VelocityVectors { get; set; }

            #endregion

            /// <summary>
            /// Create an Earth Velocity data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public EarthVelocityDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                IsVelocityVectorAvail = false;
                EarthVelocityData = new float[NumElements, ElementsMultiplier];
                VelocityVectors = new VelocityVector[NumElements];
            }

            /// <summary>
            /// Create an Earth Velocity data set.
            /// </summary>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams.  Default uses DEFAULT_NUM_BEAMS_BEAM.</param>
            public EarthVelocityDataSet(int numBins, int numBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM) :
                base(DataSet.Ensemble.DATATYPE_FLOAT,                   // Type of data stored (Float or Int)
                        numBins,                                        // Number of bins
                        numBeams,                                       // Number of beams
                        DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                        DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                        DataSet.Ensemble.EarthVelocityID)               // Dataset ID
            {
                // Initialize data
                IsVelocityVectorAvail = false;
                EarthVelocityData = new float[NumElements, ElementsMultiplier];
                VelocityVectors = new VelocityVector[NumElements];
            }

            /// <summary>
            /// Create an Earth Velocity data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="velocityData">Byte array containing EarthVelocity velocity data</param>
            public EarthVelocityDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] velocityData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                IsVelocityVectorAvail = false;
                EarthVelocityData = new float[NumElements, ElementsMultiplier];
                VelocityVectors = new VelocityVector[NumElements];

                // Decode the byte array for velocity data
                Decode(velocityData);
            }

            /// <summary>
            /// Create an Earth Velocity data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.EarthVelocityDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 65ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.EarthVelocityDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.EarthVelocityDataSet}(json); 
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
            /// <param name="EarthVelocityData">2D Array containing Earth velocity data. [Bin, Beam]</param>
            /// <param name="IsVelocityVectorAvail">Flag if the Velocity Vector array is available.</param>
            /// <param name="VelocityVectors">Velcoity Vectors.</param>
            [JsonConstructor]
            private EarthVelocityDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name, float[,] EarthVelocityData, bool IsVelocityVectorAvail, VelocityVector[] VelocityVectors) :
                base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                // Initialize data
                this.EarthVelocityData = EarthVelocityData;
                this.IsVelocityVectorAvail = IsVelocityVectorAvail;
                this.VelocityVectors = VelocityVectors;
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
                        EarthVelocityData[bin, beam] = MathHelper.ByteArrayToFloat(dataType, index);
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
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(EarthVelocityData[bin, beam]), 0, result, index, Ensemble.BYTES_IN_FLOAT);
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
                        s += "\t" + beam + ": " + EarthVelocityData[bin, beam];
                    }
                    s += "\n";
                }

                return s;
            }

            #region PD0 Ensemble

            /// <summary>
            /// Convert the Pd0 Velocity data type to the RTI Earth Velocity data set.
            /// </summary>
            /// <param name="vel">PD0 Velocity.</param>
            public void DecodePd0Ensemble(Pd0Velocity vel)
            {
                if (vel.Velocities != null)
                {
                    EarthVelocityData = new float[vel.Velocities.GetLength(0), vel.Velocities.GetLength(1)];

                    for (int bin = 0; bin < vel.Velocities.GetLength(0); bin++)
                    {
                        for (int beam = 0; beam < vel.Velocities.GetLength(1); beam++)
                        {
                            // beam order 3,2,0,1; XYZ order 1,0,-2,3, ENU order 0,1,2,3

                            // Check for bad velocity
                            if (vel.Velocities[bin, beam] != PD0.BAD_VELOCITY)
                            {
                                EarthVelocityData[bin, beam] = vel.Velocities[bin, beam] / 1000.0f;   // m/s to mm/s 
                            }
                            else
                            {
                                // Bad velocity
                                EarthVelocityData[bin, beam] = DataSet.Ensemble.BAD_VELOCITY;
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
                // If the Q is bad and we do not allow 3 Beam solution, then all is bad.
                if (EarthVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] == DataSet.Ensemble.BAD_VELOCITY && !allow3BeamSolution)
                {
                    return false;
                }

                if (EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                    EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                    EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] == DataSet.Ensemble.BAD_VELOCITY )
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
                    if ((EarthVelocityData[bin, 0] != Ensemble.BAD_VELOCITY) &&
                        (EarthVelocityData[bin, 1] != Ensemble.BAD_VELOCITY) &&
                        (EarthVelocityData[bin, 2] != Ensemble.BAD_VELOCITY))
                    {
                        return (MathHelper.CalculateMagnitude(EarthVelocityData[bin, 0], EarthVelocityData[bin, 1], EarthVelocityData[bin, 2]));
                    }
                    else
                    {
                        return DataSet.Ensemble.BAD_VELOCITY;
                    }
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
                    if ((EarthVelocityData[bin, 0] != Ensemble.BAD_VELOCITY) &&
                        (EarthVelocityData[bin, 1] != Ensemble.BAD_VELOCITY) &&
                        (EarthVelocityData[bin, 2] != Ensemble.BAD_VELOCITY))
                    {
                        if (isYNorth)
                        {
                            //return (Math.Atan2(EarthVelocityData[Bin, 1], EarthVelocityData[Bin, 0])) * (180 / Math.PI);
                            return MathHelper.CalculateDirection(EarthVelocityData[bin, 1], EarthVelocityData[bin, 0]);
                        }
                        else
                        {
                            //return (Math.Atan2(EarthVelocityData[Bin, 0], EarthVelocityData[Bin, 1])) * (180 / Math.PI);
                            return MathHelper.CalculateDirection(EarthVelocityData[bin, 0], EarthVelocityData[bin, 1]);
                        }
                    }
                    else
                    {
                        return -1;
                    }
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
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthVelocityData).
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
        public class EarthVelocityDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthVelocityData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as EarthVelocityDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");


                // IsVelocityVectorAvail Property
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_ISVELOCITYVECTORAVAIL);
                writer.WriteValue(data.IsVelocityVectorAvail);

                // VelocityVector Property
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VELOCITYVECTORS);
                if (data.VelocityVectors != null)
                {
                    writer.WriteStartArray();
                    for (int x = 0; x < data.VelocityVectors.Length; x++)
                    {
                        // Write the velocity vector
                        if (data.VelocityVectors[x] != null)
                        {
                            writer.WriteStartObject();
                            writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VV_MAG);                   // Magnitude
                            writer.WriteValue(data.VelocityVectors[x].Magnitude);
                            writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VV_XNORTH);                // Direction X North
                            writer.WriteValue(data.VelocityVectors[x].DirectionXNorth);
                            writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_VV_YNORTH);                // Direction Y North
                            writer.WriteValue(data.VelocityVectors[x].DirectionYNorth);
                            writer.WriteEndObject();
                        }
                        else
                        {
                            // Write null if the value is null
                            writer.WriteNull();
                        }
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    writer.WriteNull();
                }

                // Write the float[,] array data
                // This will be an array of arrays
                // Each array element will contain an array with the 4 beam's value
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_EARTHVELOCITYDATA);
                writer.WriteStartArray();
                for (int bin = 0; bin < data.EarthVelocityData.GetLength(0); bin++)
                {
                    // Write an array of float values for each beam's value
                    writer.WriteStartArray();

                    for (int beam = 0; beam < data.EarthVelocityData.GetLength(1); beam++)
                    {
                        writer.WriteValue(data.EarthVelocityData[bin, beam]);
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
            /// DataSet.EarthVelocityDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.EarthVelocityDataSet}(encodedEns)
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
                    var data = new EarthVelocityDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.EarthVelocityID);
                    data.EarthVelocityData = new float[NumElements, ElementsMultiplier];

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
                                    data.EarthVelocityData[bin, beam] = (float)arrayData[beam];
                                }
                            }
                        }
                    }

                    // Decode VelocityVectors
                    data.IsVelocityVectorAvail = (bool)jsonObject[DataSet.BaseDataSet.JSON_STR_ISVELOCITYVECTORAVAIL];
                    JArray vvArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_VELOCITYVECTORS];
                    if (vvArray != null)
                    {
                        data.VelocityVectors = new VelocityVector[vvArray.Count];
                        for (int beam = 0; beam < vvArray.Count; beam++)
                        {
                            //VelocityVector vv = vvArray[beam].ToObject<VelocityVector>();
                            data.VelocityVectors[beam] = vvArray[beam].ToObject<VelocityVector>();
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
                return typeof(EarthVelocityDataSet).IsAssignableFrom(objectType);
            }
        }
    }
}