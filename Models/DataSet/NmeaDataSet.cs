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
 * 12/08/2011      RC          1.09       Parse Nmea data.  Store data as NmeaSentence.
 * 12/09/2011      RC          1.09       Added many more Sentence types.
 *                                         Added method Encode().
 * 12/19/2011      RC          1.10       Fix bug checking if data is available == vs !=.
 *                                         Fix bug parsing a string of Nmea data.
 * 12/29/2011      RC          1.11       Changed from passing 4 parameters to only sentence for messages constructor.
 * 01/11/2012      RC          1.12       Fixed bug in SetNmeaStringArray() where if checkSumLoc was negative it would try to remove the string.
 * 02/20/2013      RC          2.18       Added an empty constructor.  Made the properties' Setter public for JSON coding.
 * 02/28/2013      RC          2.18       Added JSON encoding and Decoding.
 * 10/03/2013      RC          2.20.2     Fixed bug in constructor where the data type was not set correctly.  It is a byte type.
 *       
 * 
 */


using System.Text.RegularExpressions;
using System.Collections.Generic;
using DotSpatial.Positioning;
using System;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the NMEA data.
        /// </summary>
        [JsonConverter(typeof(NmeaDataSetSerializer))]
        public class NmeaDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// All NMEA sentences should end with this.
            /// </summary>
            public const string NMEA_END = "\r\n";

            /// <summary>
            /// Trim these value from the end of a 
            /// sentence.
            /// </summary>
            private char[] REMOVE_END = { '\r', '\n' };

            /// <summary>
            /// Size the checksum including
            /// the *.  The checksum contains
            /// 2 bytes.
            /// </summary>
            public const int NMEA_CHECKSUM_SIZE = 3;

            #endregion

            #region Properties

            /// <summary>
            /// List of strings of all the NMEA strings within
            /// this dataset.
            /// </summary>
            public List<string> NmeaStrings { get; set; }

            /// <summary>
            /// Last GPS GPGGA message received in this
            /// dataset.
            /// Global Positioning System Fix Data.
            /// </summary>
            [JsonIgnore]
            public GpggaSentence GPGGA { get; set; }

            /// <summary>
            /// Last GPS GPVTG message received in this
            /// dataset.
            /// Track made good and ground speed.
            /// </summary>
            [JsonIgnore]
            public GpvtgSentence GPVTG { get; set; }

            /// <summary>
            /// Last GPS GPRMC message received in this 
            /// dataset.
            /// Recommended minimum specific GPS/Transit data
            /// </summary>
            [JsonIgnore]
            public GprmcSentence GPRMC { get; set; }

            /// <summary>
            /// Last GPS PGRMF message received in this
            /// dataset.
            /// Represents a Garmin $PGRMF sentence.
            /// </summary>
            [JsonIgnore]
            public PgrmfSentence PGRMF { get; set; }

            /// <summary>
            /// Last GPS GPGLL message received in this dataset.
            /// Geographic position, latitude / longitude.
            /// </summary>
            [JsonIgnore]
            public GpgllSentence GPGLL { get; set; }

            /// <summary>
            /// Last GPS GPGSV message received in this dataset.
            /// GPS Satellites in view.
            /// </summary>
            [JsonIgnore]
            public GpgsvSentence GPGSV { get; set; }

            /// <summary>
            /// Last GPS GPGSA message received in this dataset.
            /// GPS DOP and active satellites.
            /// </summary>
            [JsonIgnore]
            public GpgsaSentence GPGSA { get; set; }

            #endregion

            /// <summary>
            /// Create an empty NMEA data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public NmeaDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize all values
                GPGGA = null;
                GPVTG = null;
                GPRMC = null;
                PGRMF = null;
                GPGLL = null;
                GPGSV = null;
                GPGSA = null;

                // Initialized list
                NmeaStrings = new List<string>();
            }

            /// <summary>
            /// Create an NMEA data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="nmeaData">Byte array containing NMEA data</param>
            public NmeaDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] nmeaData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize all values
                GPGGA = null;
                GPVTG = null;
                GPRMC = null;
                PGRMF = null;
                GPGLL = null;
                GPGSV = null;
                GPGSA = null;

                // Initialized list
                NmeaStrings = new List<string>();

                // Decode the byte array for NMEA data
                DecodeNmeaData(nmeaData);
            }

            /// <summary>
            /// Create an NMEA data set based off string given.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="nmeaData">String containing NMEA data.</param>
            public NmeaDataSet(string nmeaData) :
                base(DataSet.Ensemble.DATATYPE_BYTE, 0, DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.NmeaID)
            {
                // Initialize all values
                GPGGA = null;
                GPVTG = null;
                GPRMC = null;
                PGRMF = null;
                GPGLL = null;
                GPGSV = null;
                GPGSA = null;

                // Initialized list
                NmeaStrings = new List<string>();

                // Decode the byte array for NMEA data
                DecodeNmeaData(nmeaData);
            }

            /// <summary>
            /// Create an Nmea data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.GoodBeamDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 162ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.NmeaDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.NmeaDataSet}(json); 
            /// 
            /// To use this method for JSON you must have all the parameters match all the properties in this object.
            /// 
            /// </summary>
            /// <param name="NmeaStrings">List containing NMEA sentences.</param>
            [JsonConstructor]
            public NmeaDataSet(List<string> NmeaStrings) :
                base(DataSet.Ensemble.DATATYPE_BYTE, 0, DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.NmeaID)
            {
                // Initialize all values
                GPGGA = null;
                GPVTG = null;
                GPRMC = null;
                PGRMF = null;
                GPGLL = null;
                GPGSV = null;
                GPGSA = null;

                // Initialized list
                if (NmeaStrings != null)
                {
                    this.NmeaStrings = NmeaStrings;
                }
                else
                {
                    NmeaStrings = new List<string>();
                }

                // Decode the byte array for NMEA data
                DecodeNmeaData(NmeaStrings);
            }

            /// <summary>
            /// Return whether the GPGGA
            /// message is set.
            /// </summary>
            /// <returns>TRUE = GPGGA message exist.</returns>
            public bool IsGpggaAvail()
            {
                return GPGGA != null;
            }

            /// <summary>
            /// Return whether the GPVTG
            /// message is set.
            /// </summary>
            /// <returns>TRUE = GPVTG message exist.</returns>
            public bool IsGpvtgAvail()
            {
                return GPVTG != null;
            }

            /// <summary>
            /// Return whether the GPRMC 
            /// message is set.
            /// </summary>
            /// <returns>TRUE = GPRMC message exist.</returns>
            public bool IsGprmcAvail()
            {
                return GPRMC != null;
            }

            /// <summary>
            /// Return whether the PGRMF
            /// message is set.
            /// </summary>
            /// <returns>TRUE = PGRMF message exist.</returns>
            public bool IsPgrmfAvail()
            {
                return PGRMF != null;
            }

            /// <summary>
            /// Return whether the GPGLL
            /// message is set.
            /// </summary>
            /// <returns>TRUE = GPGLL message exist.</returns>
            public bool IsGpgllAvail()
            {
                return GPGLL != null;
            }

            /// <summary>
            /// Return whether the GPGSV
            /// message is set.
            /// </summary>
            /// <returns>TRUE = GPGSV message exist.</returns>
            public bool IsGpgsvAvail()
            {
                return GPGSV != null;
            }

            /// <summary>
            /// Return whether the GPGSA
            /// message is set.
            /// </summary>
            /// <returns>TRUE = GPGSA message exist.</returns>
            public bool IsGpgsaAvail()
            {
                return GPGSA != null;
            }

            /// <summary>
            /// Encode the data into binary format.
            /// This will include the header and the
            /// NMEA data in a byte array.
            /// </summary>
            /// <returns>NMEA data in byte array form.</returns>
            public byte[] Encode()
            {
                // Create a large string of all the NMEA data
                // Need to add the \r\n back to the end
                // of each sentence.
                StringBuilder builder = new StringBuilder();

                if (NmeaStrings != null)
                {
                    for (int x = 0; x < NmeaStrings.Count; x++)
                    {
                        builder.Append(NmeaStrings[x] + "\r\n");
                    }
                }
                string nmea = builder.ToString();

                // Get the length
                byte[] payload = System.Text.Encoding.ASCII.GetBytes(nmea);

                // Generate header for the dataset
                byte[] header = this.GenerateHeader(payload.Length);

                // Create the array to hold the dataset
                byte[] result = new byte[payload.Length + header.Length];

                // Copy the header to the array
                System.Buffer.BlockCopy(header, 0, result, 0, header.Length);

                // Copy the Nmea data to the array
                System.Buffer.BlockCopy(payload, 0, result, header.Length, payload.Length);

                return result;
            }

            /// <summary>
            /// If there is existing NMEA data, this will combine the 
            /// data.  This will take the existing data and add it to a
            /// buffer.  It will then add the new data to the buffer and
            /// parse the buffer.
            /// </summary>
            /// <param name="nmeaData">New data to combine to this dataset.</param>
            public void MergeNmeaData(string nmeaData)
            {
                StringBuilder builder = new StringBuilder();

                if (NmeaStrings != null)
                {
                    for (int x = 0; x < NmeaStrings.Count; x++)
                    {
                        builder.Append(NmeaStrings[x]);
                    }
                }

                // Add the incoming NMEA data to existing data
                builder.Append(nmeaData);

                // Parse the data
                SetNmeaStringArray(builder.ToString());
            }

            /// <summary>
            /// Find all the NMEA strings in the data.
            /// </summary>
            /// <param name="nmeaData">Byte array containing NMEA information.</param>
            private void DecodeNmeaData(byte[] nmeaData)
            {
                // Convert the byte array to string
                string nmeaStrings  = System.Text.ASCIIEncoding.ASCII.GetString(nmeaData);

                SetNmeaStringArray(nmeaStrings);
            }

            /// <summary>
            /// Decode the NMEA sentence from the array.  Then add
            /// it to the list of NMEA sentences.  Set value will try
            /// to update the NMEA data with the latest information.
            /// </summary>
            /// <param name="nmeaSentences">All the NMEA setences.</param>
            private void DecodeNmeaData(string[] nmeaSentences)
            {
                // Create a list of all the NMEA setences
                LinkedList<string> list = new LinkedList<string>();

                if (nmeaSentences != null)
                {
                    for (int x = 0; x < nmeaSentences.Length; x++)
                    {
                        // Create a NMEA sentence and verify its valid
                        NmeaSentence sentence = new NmeaSentence(nmeaSentences[x]);
                        if (sentence.IsValid)
                        {
                            // Add to the list
                            NmeaStrings.Add(sentence.Sentence);

                            // Set values from the NMEA data
                            SetValues(sentence);
                        }
                    }
                }

                // Copy the list to the NmeaStrings
                //NmeaStrings = new string[list.Count];
                //list.CopyTo(NmeaStrings, 0);
            }

            /// <summary>
            /// Decode the NMEA sentence from the array.  Then add
            /// it to the list of NMEA sentences.  Set value will try
            /// to update the NMEA data with the latest information.
            /// </summary>
            /// <param name="nmeaSentences">All the NMEA setences.</param>
            private void DecodeNmeaData(List<string> nmeaSentences)
            {
                // Create a list of all the NMEA setences
                LinkedList<string> list = new LinkedList<string>();

                if (nmeaSentences != null)
                {
                    for (int x = 0; x < nmeaSentences.Count; x++)
                    {
                        // Create a NMEA sentence and verify its valid
                        NmeaSentence sentence = new NmeaSentence(nmeaSentences[x]);
                        if (sentence.IsValid)
                        {
                            // Set values from the NMEA data
                            SetValues(sentence);
                        }
                    }
                }
            }

            /// <summary>
            /// Find all the NMEA strings in the data.
            /// </summary>
            /// <param name="nmeaData">String containing NMEA information.</param>
            private void DecodeNmeaData(string nmeaData)
            {
                SetNmeaStringArray(nmeaData);
            }

            /// <summary>
            /// Parse the string of all valid NMEA sentences.
            /// Store them to the array.
            /// </summary>
            /// <param name="nmeaStrings">String containing NMEA sentences.</param>
            private void SetNmeaStringArray(string nmeaStrings)
            {
                LinkedList<NmeaSentence> list = new LinkedList<NmeaSentence>();

                // Find the first $
                while(nmeaStrings.Contains("$") && nmeaStrings.Contains("*"))
                {
                    int start = nmeaStrings.IndexOf("$");
                    nmeaStrings = nmeaStrings.Substring(start);

                    // Check if a checksum exist in the data
                    // If so, being parsing
                    int checksumLoc = nmeaStrings.IndexOf("*");

                    string nmea = "";
                    // Find the start of the checksum
                    // Add NMEA_CHECKSUM_SIZE to include the * and checksum value
                    if (checksumLoc >= 0)
                    {
                        if (nmeaStrings.Length >= checksumLoc + NMEA_CHECKSUM_SIZE)
                        {
                            // Check if the checksum is good
                            if (!nmeaStrings.Substring(checksumLoc, NMEA_CHECKSUM_SIZE).Contains("$"))
                            {
                                // Get the NMEA string and remove it from the buffer.
                                nmea = nmeaStrings.Substring(0, checksumLoc + NMEA_CHECKSUM_SIZE);
                                nmeaStrings = nmeaStrings.Remove(0, nmea.Length);

                                // Remove any trailing new lines
                                nmea = nmea.TrimEnd(REMOVE_END);
                            }
                            else
                            {
                                // Bad Nmea string, $ within checksum
                                // Remove the bad string
                                nmeaStrings = nmeaStrings.Remove(0, checksumLoc);
                            }
                        }
                        else
                        {
                            nmeaStrings = nmeaStrings.Remove(0, checksumLoc);
                        }
                    }

                    if (nmea.Length > 0)
                    {
                        // Create a NMEA sentence and verify its valid
                        NmeaSentence sentence = new NmeaSentence(nmea);
                        if (sentence.IsValid)
                        {
                            // Add the nmea data to list
                            NmeaStrings.Add(sentence.Sentence);

                            // Set values from the NMEA data
                            SetValues(sentence);
                        }
                    }
                }

                // Store the Nmea data
                //NmeaStrings = new NmeaSentence[list.Count];
                //list.CopyTo(NmeaStrings, 0);
            }

            /// <summary>
            /// Set any possible values for the given NMEA data.
            /// It will continue to replace
            /// the values so the last value is used as the final value.
            /// </summary>
            /// <param name="sentence">NMEA sentence containing data.</param>
            private void SetValues(NmeaSentence sentence)
            {
                /*
                 * NMEA specification states that the first two letters of
                 * a sentence may change.  For example, for "$GPGSV" there may be variations such as
                 * "$__GSV" where the first two letters change.  As a result, we need only test the last three
                 * characters.
                 */

                if (sentence.CommandWord.EndsWith("GGA", StringComparison.Ordinal))
                {
                    // Yes.  Convert it using the fast pre-parseed constructor
                    GPGGA = new GpggaSentence(sentence.Sentence);

                    // Set the Lat and Lon and time
                }
                if (sentence.CommandWord.EndsWith("VTG", StringComparison.Ordinal))
                {
                    // Yes.  Convert it using the fast pre-parseed constructor
                    GPVTG = new GpvtgSentence(sentence.Sentence);
                }
                if (sentence.CommandWord.EndsWith("RMC", StringComparison.Ordinal))
                {
                    // Yes.  Convert it using the fast pre-parseed constructor
                    GPRMC = new GprmcSentence(sentence.Sentence);
                }
                if (sentence.CommandWord.EndsWith("RMF", StringComparison.Ordinal))
                {
                    // Yes.  Convert it using the fast pre-parseed constructor
                    PGRMF = new PgrmfSentence(sentence.Sentence);
                }
                if (sentence.CommandWord.EndsWith("GLL", StringComparison.Ordinal))
                {
                    // Yes.  Convert it using the fast pre-parseed constructor
                    GPGLL = new GpgllSentence(sentence.Sentence);
                }
                if (sentence.CommandWord.EndsWith("GSV", StringComparison.Ordinal))
                {
                    // Yes.  Convert it using the fast pre-parseed constructor
                    GPGSV = new GpgsvSentence(sentence.Sentence);
                }
                if (sentence.CommandWord.EndsWith("GSA", StringComparison.Ordinal))
                {
                    // Yes.  Convert it using the fast pre-parseed constructor
                    GPGSA = new GpgsaSentence(sentence.Sentence);
                }
            }

            /// <summary>
            /// Override the ToString to return all the NMEA data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();

                if (NmeaStrings != null)
                {
                    for (int x = 0; x < NmeaStrings.Count; x++)
                    {
                        builder.Append(NmeaStrings[x]);
                    }
                }

                return builder.ToString();
            }

            /// <summary>
            /// Determine whether there is a good
            /// GPS speed.
            /// If the VTG NMEA sentence does not exist, then
            /// no speed is given.  If VTG is given and the speed
            /// is good, then a good GPS Speed is available.
            /// </summary>
            /// <returns>TRUE = Good GPS Speed Available.</returns>
            public bool IsGpsSpeedGood()
            {
                if (IsGpvtgAvail())
                {
                    if (GPVTG.Speed.Value != DotSpatial.Positioning.Speed.Invalid.Value)
                    {
                        // GPS speed good
                        return true;
                    }
                }

                return false;
            }
        }

                /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.NmeaData).
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
        public class NmeaDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.NmeaData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as NmeaDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // Write the NMEA strings
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_NMEASTRINGS);
                if (data.NmeaStrings != null)
                {
                    writer.WriteStartArray();
                    for (int x = 0; x < data.NmeaStrings.Count; x++)
                    {
                        writer.WriteValue(data.NmeaStrings[x]);
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    writer.WriteNull();
                }

                // End the object
                writer.WriteEndObject();
            }

            /// <summary>
            /// Read the JSON object and convert to the object.  This will allow the serializer to
            /// automatically convert the object.  No special instructions need to be done and all
            /// the properties found in the JSON string need to be used.
            /// 
            /// DataSet.NmeaDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.NmeaDataSet}(encodedEns)
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
                    //int ValueType = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_VALUETYPE];
                    //int NumElements = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_NUMELEMENTS];
                    //int ElementsMultiplier = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_ELEMENTSMULTIPLIER];
                    //int Imag = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_IMAG];
                    //int NameLength = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_NAMELENGTH];
                    //string Name = (string)jsonObject[DataSet.BaseDataSet.JSON_STR_NAME];

                    // Create a list
                    List<string> NmeaStrings = new List<string>();

                    // Decode the NMEA string list
                    JArray jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_NMEASTRINGS];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the strings to the list
                        NmeaStrings.Add((string)jArray[x]);
                    }

                    // Return created object
                    return new NmeaDataSet(NmeaStrings);
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
                return typeof(NmeaDataSet).IsAssignableFrom(objectType);
            }
        }
    }
}