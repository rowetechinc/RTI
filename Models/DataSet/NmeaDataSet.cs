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
 *       
 * 
 */


using System.Text.RegularExpressions;
using System.Collections.Generic;
using DotSpatial.Positioning;
using System;
using System.Text;

namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the NMEA data.
        /// </summary>
        public class NmeaDataSet : BaseDataSet
        {
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

            /// <summary>
            /// Array of strings of all the NMEA strings within
            /// this dataset.
            /// </summary>
            public NmeaSentence[] NmeaStrings { get; private set; }

            /// <summary>
            /// Last GPS GPGGA message received in this
            /// dataset.
            /// Global Positioning System Fix Data.
            /// </summary>
            public GpggaSentence GPGGA { get; private set; }

            /// <summary>
            /// Last GPS GPVTG message received in this
            /// dataset.
            /// Track made good and ground speed.
            /// </summary>
            public GpvtgSentence GPVTG { get; private set; }

            /// <summary>
            /// Last GPS GPRMC message received in this 
            /// dataset.
            /// Recommended minimum specific GPS/Transit data
            /// </summary>
            public GprmcSentence GPRMC { get; private set; }

            /// <summary>
            /// Last GPS PGRMF message received in this
            /// dataset.
            /// Represents a Garmin $PGRMF sentence.
            /// </summary>
            public PgrmfSentence PGRMF { get; private set; }

            /// <summary>
            /// Last GPS GPGLL message received in this dataset.
            /// Geographic position, latitude / longitude.
            /// </summary>
            public GpgllSentence GPGLL { get; private set; }

            /// <summary>
            /// Last GPS GPGSV message received in this dataset.
            /// GPS Satellites in view.
            /// </summary>
            public GpgsvSentence GPGSV { get; private set; }

            /// <summary>
            /// Last GPS GPGSA message received in this dataset.
            /// GPS DOP and active satellites.
            /// </summary>
            public GpgsaSentence GPGSA { get; private set; }

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

                // Decode the byte array for NMEA data
                DecodeNmeaData(nmeaData);
            }

            /// <summary>
            /// Create an NMEA data set based off string given.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="nmeaData">String containing NMEA data</param>
            public NmeaDataSet(string nmeaData) :
                base(DataSet.Ensemble.DATATYPE_FLOAT, 0, DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.NMEAID)
            {
                // Initialize all values
                GPGGA = null;
                GPVTG = null;
                GPRMC = null;
                PGRMF = null;
                GPGLL = null;
                GPGSV = null;
                GPGSA = null;

                // Decode the byte array for NMEA data
                DecodeNmeaData(nmeaData);
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
                for (int x = 0; x < NmeaStrings.Length; x++)
                {
                    builder.Append(NmeaStrings[x].Sentence + "\r\n");
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
                for (int x = 0; x < NmeaStrings.Length; x++)
                {
                    builder.Append(NmeaStrings[x]);
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
                            list.AddLast(sentence);

                            // Set values from the NMEA data
                            SetValues(sentence);
                        }
                    }
                }

                // Store the Nmea data
                NmeaStrings = new NmeaSentence[list.Count];
                list.CopyTo(NmeaStrings, 0);
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
                for (int x = 0; x < NmeaStrings.Length; x++)
                {
                    builder.Append(NmeaStrings[x].Sentence);
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
    }
}