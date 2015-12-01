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
 * 10/04/2011      RC                     Added CorrelationBinList to display text.
 *                                         Added method to populate the list.
 * 10/05/2011      RC                     Round number when displaying.
 * 10/25/2011      RC                     Added new constructor that takes no data and made CreateBinList public.
 * 12/07/2011      RC          1.08       Remove BinList     
 * 01/19/2012      RC          1.14       Added Encode() to create a byte array of data.
 *                                         Removed "private set".
 *                                         Rename Decode methods to Decode().
 * 03/30/2012      RC          2.07       Moved Converters.cs methods to MathHelper.cs.
 * 02/25/2013      RC          2.18       Removed Orientation.
 *                                         Added JSON encoding and Decoding.
 * 05/01/2013      RC          2.19       Added ability to handle single beam data in JSON.
 * 03/25/2014      RC          2.21.4     Added a simpler constructor and added DecodePd0Ensemble().
 * 05/07/2014      RC          2.21.4     In DecodePd0Ensemble(), changed the conversion to match WH correlation values.
 * 07/24/2014      RC          2.23.0     Fixed bug in DecodePd0Ensemble() if numCodeRepeats is 0 or N is 0.
 * 
 */

using System.Data;
using System;
using System.ComponentModel;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the Correlation data.
        /// </summary>
        [JsonConverter(typeof(CorrelationDataSetSerializer))]
        public class CorrelationDataSet : BaseDataSet
        {
            #region Properties

            /// <summary>
            /// Store all the Correlation data for the ADCP.
            /// [bins,beams]
            /// </summary>
            public float[,] CorrelationData { get; set; }

            #endregion

            /// <summary>
            /// Create an Correlation data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public CorrelationDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                CorrelationData = new float[NumElements, ElementsMultiplier];
            }

            /// <summary>
            /// Create an Correlation data set.
            /// </summary>
            /// <param name="numBins">Number of Bin.</param>
            /// <param name="numBeams">Number of beams.  Default uses DEFAULT_NUM_BEAMS_BEAM.</param>
            public CorrelationDataSet(int numBins, int numBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM) :
                base(DataSet.Ensemble.DATATYPE_FLOAT,                   // Type of data stored (Float or Int)
                        numBins,                                        // Number of bins
                        numBeams,                                       // Number of beams
                        DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                        DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                        DataSet.Ensemble.CorrelationID)                 // Dataset ID
            {
                // Initialize data
                CorrelationData = new float[NumElements, ElementsMultiplier];
            }

            /// <summary>
            /// Create an Correlation data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="correlationData">Byte array containing Correlation data</param>
            public CorrelationDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] correlationData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                CorrelationData = new float[NumElements, ElementsMultiplier];

                // Decode the byte array for Correlation data
                Decode(correlationData);
            }

            /// <summary>
            /// Create an Correlation data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.CorrelationDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 162ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.CorrelationDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.CorrelationDataSet}(json); 
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
            /// <param name="CorrelationData">2D Array containing Correlation data. [Bin, Beam]</param>
            [JsonConstructor]
            private CorrelationDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name, float[,] CorrelationData) :
                base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                // Initialize data
                this.CorrelationData = CorrelationData;
            }

            /// <summary>
            /// Get all the Correlation ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin Bin Beams.
            /// </summary>
            /// <param name="dataType">Byte array containing the Correlation data type.</param>
            private void Decode(byte[] dataType)
            {
                int index = 0;
                for (int beam = 0; beam < ElementsMultiplier; beam++)
                {
                    for (int bin = 0; bin < NumElements; bin++)
                    {
                        index = GetBinBeamIndex(NameLength, NumElements, beam, bin);
                        CorrelationData[bin, beam] = MathHelper.ByteArrayToFloat(dataType, index);
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
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(CorrelationData[bin, beam]), 0, result, index, Ensemble.BYTES_IN_FLOAT);
                    }
                }

                return result;
            }

            /// <summary>
            /// Override the ToString to return all the Correlation data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string s = "";
                for (int bin = 0; bin < NumElements; bin++)
                {
                    s += "C Bin: " + bin + "\t";
                    for (int beam = 0; beam < ElementsMultiplier; beam++)
                    {
                        s += "\t" + beam + ": " + CorrelationData[bin, beam];
                    }
                    s += "\n";
                }

                return s;
            }

            #region PD0 Ensemble

            /// <summary>
            /// Convert the Pd0 Correlation data type to the RTI Correlation data set.
            /// </summary>
            /// <param name="corr">PD0 Correlation.</param>
            /// <param name="numRepeats">Number of code repeats from the fixed leader.</param>
            public void DecodePd0Ensemble(Pd0Correlation corr, float numRepeats)
            {
                if (corr.Correlation != null)
                {
                    CorrelationData = new float[corr.Correlation.GetLength(0), corr.Correlation.GetLength(1)];

                    // PD0 is 0.5 dB per count

                    for (int bin = 0; bin < corr.Correlation.GetLength(0); bin++)
                    {
                        for (int beam = 0; beam < corr.Correlation.GetLength(1); beam++)
                        {
                            // PD0 beam order 3,2,0,1
                            int newBeam = 0;
                            switch (beam)
                            {
                                case 3:
                                    newBeam = 0;
                                    break;
                                case 2:
                                    newBeam = 1;
                                    break;
                                case 0:
                                    newBeam = 2;
                                    break;
                                case 1:
                                    newBeam = 3;
                                    break;
                            }

                            //CorrelationData[bin, beam] = corr.Correlation[bin, newBeam] / 255.0f;

                            // Check if numRepeats = 0    
                            if (numRepeats == 0) { numRepeats = 1; }

                            // Constant used in WH to normalize the value based off the number of repeats
                            // Then normalize the value to 128 being the max
                            float n = ((numRepeats - 1.0f) / numRepeats);

                            // Check if n = 0    
                            if (n == 0) { n = 1.0f; }

                            CorrelationData[bin, beam] = (corr.Correlation[bin, newBeam] / 128.0f) * n ;
                        }
                    }
                }
            }

            #endregion

        }

        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.CorrelationData).
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
        public class CorrelationDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.CorrelationData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as CorrelationDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");


                // Write the float[,] array data
                // This will be an array of arrays
                // Each array element will contain an array with the 4 beam's value
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_CORRELATIONDATA);
                writer.WriteStartArray();
                for (int bin = 0; bin < data.NumElements; bin++)
                {
                    // Write an array of float values for each beam's value
                    writer.WriteStartArray();

                    for (int beam = 0; beam < data.ElementsMultiplier; beam++)
                    {
                        writer.WriteValue(data.CorrelationData[bin, beam]);
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
            /// DataSet.CorrelationDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.CorrelationDataSet}(encodedEns)
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
                    var data = new CorrelationDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.CorrelationID);
                    data.CorrelationData = new float[NumElements, ElementsMultiplier];

                    // Decode the 2D array 
                    JArray jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_CORRELATIONDATA];
                    if (jArray.Count <= NumElements)                                                            // Verify size
                    {
                        for (int bin = 0; bin < jArray.Count; bin++)
                        {
                            JArray arrayData = (JArray)jArray[bin];
                            if (arrayData.Count <= ElementsMultiplier)                                          // Verify size
                            {
                                for (int beam = 0; beam < arrayData.Count; beam++)
                                {
                                    data.CorrelationData[bin, beam] = (float)arrayData[beam];
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
                return typeof(CorrelationDataSet).IsAssignableFrom(objectType);
            }
        }
    }
}