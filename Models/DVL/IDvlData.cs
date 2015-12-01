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
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 06/18/2014      RC          2.22.1     Initial coding
 * 06/23/2014      RC          2.22.1     Added Sentence property and ToByteArray() in IDvlData.
 * 07/02/2014      RC          2.23.0     Changed Convert() to TryParse() and initialized the values.
 * 09/16/2014      RC          3.0.1      Fixed bug with TS not setting the Date and Time.
 * 10/09/2014      RC          3.0.2      Added Leak Detection to TS.
 * 08/11/2015      RC          3.0.5      Set the NUM_ELEM as const.
 * 
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


        

    /// <summary>
    /// DVL data interface.
    /// All DVL data can use this
    /// interface so they can be used
    /// in the same constructors.
    /// </summary>
    public class IDvlData
    {
        /// <summary>
        /// Sentence for this object.
        /// </summary>
        public string Sentence;

        /// <summary>
        /// Converts the packet into an array of bytes.
        /// </summary>
        /// <returns>Byte array of the sentence for this object.</returns>
        /// <remarks></remarks>
        public byte[] ToByteArray()
        {
            return Encoding.ASCII.GetBytes(Sentence + "\r\n");
        }
    }

    /// <summary>
    /// System Attitude Data.
    /// </summary>
    public class SA : IDvlData
    {
        /// <summary>
        /// System Attitude data ID.
        /// </summary>
        public const string ID = ":SA";

        /// <summary>
        /// Number of elements.
        /// Includes the ID.
        /// </summary>
        public const int NUM_ELEM = 4;

        /// <summary>
        /// Pitch in degrees.
        /// </summary>
        public float Pitch;

        /// <summary>
        /// Roll in degrees.
        /// </summary>
        public float Roll;

        /// <summary>
        /// Heading in degrees.
        /// </summary>
        public float Heading;

        /// <summary>
        /// Decode the sentence given.
        /// </summary>
        /// <param name="sent">Sentence to decode</param>
        public SA(string sent)
        {
            // Set the sentence
            Sentence = sent;

            // Decode the sentence
            string[] result = sent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length >= NUM_ELEM && result[0].Contains(ID))
            {
                // Remove the plus sign
                result[1].Replace("+", String.Empty);
                result[2].Replace("+", String.Empty);
                result[3].Replace("+", String.Empty);

                // Trim the results and convert to a float
                Pitch = 0.0f;
                Roll = 0.0f;
                Heading = 0.0f;
                Single.TryParse(result[1].Trim(), out Pitch);
                Single.TryParse(result[2].Trim(), out Roll);
                Single.TryParse(result[3].Trim(), out Heading);
            }
        }
    }

    /// <summary>
    /// Timing and scaling data.
    /// </summary>
    public class TS : IDvlData
    {
        #region Leak Detection

        /// <summary>
        /// O
        /// Leak Detect OK value.
        /// </summary>
        public const string LEAKDETECT_OK = "O";

        /// <summary>
        /// W
        /// Leak Detect Water Detected value.
        /// </summary>
        public const string LEAKDETECT_WATER = "W";

        /// <summary>
        /// X
        /// Leak Detect not installed value.
        /// </summary>
        public const string LEAKDETECT_NOT_INSTALLED = "X";

        #endregion

        /// <summary>
        /// Timing and scaling data ID.
        /// </summary>
        public const string ID = ":TS";

        /// <summary>
        /// Number of elements.
        /// Includes the ID.
        /// </summary>
        public const int NUM_ELEM = 7;

        /// <summary>
        /// Date and time.
        /// </summary>
        public DateTime DateAndTime;

        /// <summary>
        /// Store the hundredth of a second.
        /// </summary>
        public byte HundredthSec;

        /// <summary>
        /// Salinity in parts per thousand (ppt).
        /// </summary>
        public float Salinity;

        /// <summary>
        /// Temperature in C.
        /// </summary>
        public float Temperature;

        /// <summary>
        /// Depth of transducer face in meters.
        /// </summary>
        public float DepthOfTransducer;

        /// <summary>
        /// Speed of sound in meters per second (m/s).
        /// </summary>
        public float SpeedOfSound;

        /// <summary>
        /// Built-in Test (BIT) result code.
        /// </summary>
        public int BIT;

        /// <summary>
        /// Leak Detection results.
        /// O = OK.
        /// W = Water
        /// X = Not Installed.
        /// </summary>
        public RTI.DataSet.DvlDataSet.LeakDetectionOptions LeakDetection;

        /// <summary>
        /// Decode the sentence given.
        /// </summary>
        /// <param name="sent">Sentence to decode</param>
        public TS(string sent)
        {
            // Set the sentence
            Sentence = sent;

            // Decode the sentence
            string[] result = sent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length >= NUM_ELEM && result[0].Contains(ID))
            {
                // Remove the plus sign
                result[1].Replace("+", String.Empty);
                result[2].Replace("+", String.Empty);
                result[3].Replace("+", String.Empty);
                result[4].Replace("+", String.Empty);
                result[5].Replace("+", String.Empty);
                result[6].Replace("+", String.Empty);

                // Trim the results and convert to a float
                DateAndTime = DateTime.Now;
                Salinity = 0.0f;
                Temperature = 0.0f;
                DepthOfTransducer = 0.0f;
                SpeedOfSound = 0.0f;
                BIT = 0;

                // Pattern for the date and time
                string pattern = "yyMMddHHmmss";

                // DateTime cannot hold the hundredth of a second
                string dt = result[1].Trim();
                if(dt.Length >= 14)
                {
                    // Store the hundredth of second
                    HundredthSec = Convert.ToByte(dt.Substring(12, 2));

                    // Remove the hundredth of second from the string
                    dt = dt.Substring(0, 12);
                }

                DateTime.TryParseExact(dt, pattern, null, System.Globalization.DateTimeStyles.None, out DateAndTime);
                Single.TryParse(result[2].Trim(), out Salinity);
                Single.TryParse(result[3].Trim(), out Temperature);
                Single.TryParse(result[4].Trim(), out DepthOfTransducer);
                Single.TryParse(result[5].Trim(), out SpeedOfSound);
                Int32.TryParse(result[6].Trim(), out BIT);

                // Leak detection added
                if(result.Length == 8)
                {
                    switch(result[7].Trim())
                    {
                        case LEAKDETECT_OK:
                            LeakDetection = RTI.DataSet.DvlDataSet.LeakDetectionOptions.OK;
                            break;
                        case LEAKDETECT_WATER:
                            LeakDetection = RTI.DataSet.DvlDataSet.LeakDetectionOptions.WaterDetected;
                            break;
                        case LEAKDETECT_NOT_INSTALLED:
                        default:
                            LeakDetection = RTI.DataSet.DvlDataSet.LeakDetectionOptions.NotInstalled;
                            break;
                    }
                    
                }
            }
        }
    }

    /// <summary>
    /// Pressure and Range to Bottom Data.
    /// </summary>
    public class RA : IDvlData
    {
        /// <summary>
        /// Pressure and Range to Bottom data ID.
        /// </summary>
        public const string ID = ":RA";

        /// <summary>
        /// Number of elements.
        /// Includes the ID.
        /// </summary>
        public const int NUM_ELEM = 6;

        /// <summary>
        /// Pressure in kPa.
        /// </summary>
        public float Pressure;

        /// <summary>
        /// Range to Bottom in deci-meters Beam 0.
        /// </summary>
        public float RangeToBottomB0;

        /// <summary>
        /// Range to Bottom in deci-meters Beam 1.
        /// </summary>
        public float RangeToBottomB1;

        /// <summary>
        /// Range to Bottom in deci-meters Beam 2.
        /// </summary>
        public float RangeToBottomB2;

        /// <summary>
        /// Range to Bottom in deci-meters Beam 3.
        /// </summary>
        public float RangeToBottomB3;

        /// <summary>
        /// Decode the sentence given.
        /// </summary>
        /// <param name="sent">Sentence to decode</param>
        public RA(string sent)
        {
            // Set the sentence
            Sentence = sent;

            // Decode the sentence
            string[] result = sent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length >= NUM_ELEM && result[0].Contains(ID))
            {
                // Remove the plus sign
                result[1].Replace("+", String.Empty);
                result[2].Replace("+", String.Empty);
                result[3].Replace("+", String.Empty);
                result[4].Replace("+", String.Empty);
                result[5].Replace("+", String.Empty);

                // Trim the results and convert to a float
                Pressure = 0.0f;
                RangeToBottomB0 = 0.0f;
                RangeToBottomB1 = 0.0f;
                RangeToBottomB2 = 0.0f;
                RangeToBottomB3 = 0.0f;
                Single.TryParse(result[1].Trim(), out Pressure);
                Single.TryParse(result[2].Trim(), out RangeToBottomB0);
                Single.TryParse(result[3].Trim(), out RangeToBottomB1);
                Single.TryParse(result[4].Trim(), out RangeToBottomB2);
                Single.TryParse(result[5].Trim(), out RangeToBottomB3);
            }
        }
    }

    /// <summary>
    /// WaterMass, instrument-referenced velocity data. 
    /// </summary>
    public class WI : IDvlData
    {
        /// <summary>
        /// WaterMass, instrument-referenced velocity data ID.
        /// </summary>
        public const string ID = ":WI";

        /// <summary>
        /// Number of elements.
        /// Includes the ID.
        /// </summary>
        public const int NUM_ELEM = 6;

        /// <summary>
        /// +/- X-axis velocity data in mm/s.
        /// (+ = Bm0 B1 XDCR movement relative to water mass).
        /// </summary>
        public float X;

        /// <summary>
        /// +/- Y-axis velocity data in mm/s.
        /// (+ = Bm3 B2 XDCR movement relative to water mass).
        /// </summary>
        public float Y;

        /// <summary>
        /// +/- Z-axis velocity data in mm/s.
        /// (+ = transducer movement away from water mass.).
        /// </summary>
        public float Z;

        /// <summary>
        /// Error velocity data in mm/s.
        /// </summary>
        public float Q;

        /// <summary>
        /// Status of velocity data.  
        /// (A = good, V = bad)
        /// </summary>
        public bool IsGood;

        /// <summary>
        /// Decode the sentence given.
        /// </summary>
        /// <param name="sent">Sentence to decode</param>
        public WI(string sent)
        {
            // Decode the sentence
            string[] result = sent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length >= NUM_ELEM && result[0].Contains(ID))
            {
                // Remove the plus sign
                result[1].Replace("+", String.Empty);
                result[2].Replace("+", String.Empty);
                result[3].Replace("+", String.Empty);
                result[4].Replace("+", String.Empty);

                // Trim the results and convert to a float
                X = PD0.BAD_VELOCITY;
                Y = PD0.BAD_VELOCITY;
                Z = PD0.BAD_VELOCITY;
                Q = PD0.BAD_VELOCITY;
                Single.TryParse(result[1].Trim(), out X);
                Single.TryParse(result[2].Trim(), out Y);
                Single.TryParse(result[3].Trim(), out Z);
                Single.TryParse(result[4].Trim(), out Q);

                // Check for Bad Velocity
                if (X == PD0.BAD_VELOCITY)
                {
                    X = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (Y == PD0.BAD_VELOCITY)
                {
                    Y = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (Z == PD0.BAD_VELOCITY)
                {
                    Z = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (Q == PD0.BAD_VELOCITY)
                {
                    Q = DataSet.Ensemble.BAD_VELOCITY;
                }


                if (result[5].Contains('A'))
                {
                    IsGood = true;
                }
                else
                {
                    IsGood = false;
                }
            }
        }
    }

    /// <summary>
    /// WaterMass, ship-referenced velocity data. 
    /// </summary>
    public class WS : IDvlData
    {
        /// <summary>
        /// WaterMass, ship-referenced velocity data ID.
        /// </summary>
        public const string ID = ":WS";

        /// <summary>
        /// Number of elements.
        /// Includes the ID.
        /// </summary>
        public const int NUM_ELEM = 5;

        /// <summary>
        /// +/- Transverse velocity data in mm/s.
        /// (+ = Port Stbd XDCR movement relative to water mass).
        /// </summary>
        public float T;

        /// <summary>
        /// +/- Longitudinal velocity data in mm/s.
        /// (+ = Aft Fwd XDCR movement relative to water mass).
        /// </summary>
        public float L;

        /// <summary>
        /// +/- Z-axis velocity data in mm/s.
        /// (+ = transducer movement away from water mass.).
        /// </summary>
        public float N;

        /// <summary>
        /// Status of velocity data.  
        /// (A = good, V = bad)
        /// </summary>
        public bool IsGood;

        /// <summary>
        /// Decode the sentence given.
        /// </summary>
        /// <param name="sent">Sentence to decode</param>
        public WS(string sent)
        {
            // Set the sentence
            Sentence = sent;

            // Decode the sentence
            string[] result = sent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length >= NUM_ELEM && result[0].Contains(ID))
            {
                // Remove the plus sign
                result[1].Replace("+", String.Empty);
                result[2].Replace("+", String.Empty);
                result[3].Replace("+", String.Empty);

                // Trim the results and convert to a float
                T = PD0.BAD_VELOCITY;
                L = PD0.BAD_VELOCITY;
                N = PD0.BAD_VELOCITY;
                Single.TryParse(result[1].Trim(), out T);
                Single.TryParse(result[2].Trim(), out L);
                Single.TryParse(result[3].Trim(), out N);

                // Check for Bad Velocity
                if (T == PD0.BAD_VELOCITY)
                {
                    T = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (L == PD0.BAD_VELOCITY)
                {
                    L = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (N == PD0.BAD_VELOCITY)
                {
                    N = DataSet.Ensemble.BAD_VELOCITY;
                }

                if (result[4].Contains('A'))
                {
                    IsGood = true;
                }
                else
                {
                    IsGood = false;
                }
            }
        }
    }

    /// <summary>
    /// WaterMass, Earth-referenced velocity data. 
    /// </summary>
    public class WE : IDvlData
    {
        /// <summary>
        /// WaterMass, earth-referenced velocity data ID.
        /// </summary>
        public const string ID = ":WE";

        /// <summary>
        /// Number of elements.
        /// Includes the ID.
        /// </summary>
        public const int NUM_ELEM = 5;

        /// <summary>
        /// +/- East (u-axis) velocity data in mm/s.
        /// (+ = ADCP movement to east).
        /// </summary>
        public float E;

        /// <summary>
        /// +/- North (v-axis) velocity data in mm/s.
        /// (+ = ADCP movement to north).
        /// </summary>
        public float N;

        /// <summary>
        /// +/- Upward (w-axis) velocity data in mm/s.
        /// (+ = ADCP movement to surface).
        /// </summary>
        public float U;

        /// <summary>
        /// Status of velocity data.  
        /// (A = good, V = bad)
        /// </summary>
        public bool IsGood;

        /// <summary>
        /// Decode the sentence given.
        /// </summary>
        /// <param name="sent">Sentence to decode</param>
        public WE(string sent)
        {
            // Set the sentence
            Sentence = sent;

            // Decode the sentence
            string[] result = sent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length >= NUM_ELEM && result[0].Contains(ID))
            {
                // Remove the plus sign
                result[1].Replace("+", String.Empty);
                result[2].Replace("+", String.Empty);
                result[3].Replace("+", String.Empty);

                // Trim the results and convert to a float
                E = PD0.BAD_VELOCITY;
                N = PD0.BAD_VELOCITY;
                U = PD0.BAD_VELOCITY;
                Single.TryParse(result[1].Trim(), out E);
                Single.TryParse(result[2].Trim(), out N);
                Single.TryParse(result[3].Trim(), out U);

                // Check for Bad Velocity
                if (E == PD0.BAD_VELOCITY)
                {
                    E = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (N == PD0.BAD_VELOCITY)
                {
                    N = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (U == PD0.BAD_VELOCITY)
                {
                    U = DataSet.Ensemble.BAD_VELOCITY;
                }

                if (result[4].Contains('A'))
                {
                    IsGood = true;
                }
                else
                {
                    IsGood = false;
                }
            }
        }
    }

    /// <summary>
    /// WaterMass, Earth-referenced distance data. 
    /// </summary>
    public class WD : IDvlData
    {
        /// <summary>
        /// WaterMass, earth-referenced distance data ID.
        /// </summary>
        public const string ID = ":WD";

        /// <summary>
        /// Number of elements.
        /// Includes the ID.
        /// </summary>
        public const int NUM_ELEM = 6;

        /// <summary>
        /// + East (u-axis) distance data in meters.
        /// </summary>
        public float E;

        /// <summary>
        /// + North (v-axis) distance data in meters.
        /// </summary>
        public float N;

        /// <summary>
        /// + Upward (w-axis) distance data in meters.
        /// </summary>
        public float U;

        /// <summary>
        /// Range to water-mass center in meters.
        /// </summary>
        public float RangeToWmCenter;

        /// <summary>
        /// Time since last good-velocity estimate in seconds.
        /// </summary>
        public float Time;

        /// <summary>
        /// Decode the sentence given.
        /// </summary>
        /// <param name="sent">Sentence to decode</param>
        public WD(string sent)
        {
            // Set the sentence
            Sentence = sent;

            // Decode the sentence
            string[] result = sent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length >= NUM_ELEM && result[0].Contains(ID))
            {
                // Remove the plus sign
                result[1].Replace("+", String.Empty);
                result[2].Replace("+", String.Empty);
                result[3].Replace("+", String.Empty);
                result[4].Replace("+", String.Empty);
                result[5].Replace("+", String.Empty);

                // Trim the results and convert to a float
                E = PD0.BAD_VELOCITY;
                N = PD0.BAD_VELOCITY;
                U = PD0.BAD_VELOCITY;
                RangeToWmCenter = 0.0f;
                Time = 0.0f;
                Single.TryParse(result[1].Trim(), out E);
                Single.TryParse(result[2].Trim(), out N);
                Single.TryParse(result[3].Trim(), out U);
                Single.TryParse(result[4].Trim(), out RangeToWmCenter);
                Single.TryParse(result[5].Trim(), out Time);

                // Check for Bad Velocity
                if (E == PD0.BAD_VELOCITY)
                {
                    E = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (N == PD0.BAD_VELOCITY)
                {
                    N = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (U == PD0.BAD_VELOCITY)
                {
                    U = DataSet.Ensemble.BAD_VELOCITY;
                }

            }
        }
    }

    /// <summary>
    /// Bottom Track, instrument-referenced velocity data. 
    /// </summary>
    public class BI : IDvlData
    {
        /// <summary>
        /// Bottom Track, instrument-referenced velocity data ID.
        /// </summary>
        public const string ID = ":BI";

        /// <summary>
        /// Number of elements.
        /// Includes the ID.
        /// </summary>
        public const int NUM_ELEM = 6;

        /// <summary>
        /// +/- X-axis velocity data in mm/s.
        /// (+ = Bm0 B1 XDCR movement relative to water mass).
        /// </summary>
        public float X;

        /// <summary>
        /// +/- Y-axis velocity data in mm/s.
        /// (+ = Bm3 B2 XDCR movement relative to water mass).
        /// </summary>
        public float Y;

        /// <summary>
        /// +/- Z-axis velocity data in mm/s.
        /// (+ = transducer movement away from water mass.).
        /// </summary>
        public float Z;

        /// <summary>
        /// Error velocity data in mm/s.
        /// </summary>
        public float Q;

        /// <summary>
        /// Status of velocity data.  
        /// (A = good, V = bad)
        /// </summary>
        public bool IsGood;

        /// <summary>
        /// Decode the sentence given.
        /// </summary>
        /// <param name="sent">Sentence to decode</param>
        public BI(string sent)
        {
            // Set the sentence
            Sentence = sent;

            // Decode the sentence
            string[] result = sent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length >= NUM_ELEM && result[0].Contains(ID))
            {
                // Remove the plus sign
                result[1].Replace("+", String.Empty);
                result[2].Replace("+", String.Empty);
                result[3].Replace("+", String.Empty);
                result[4].Replace("+", String.Empty);

                // Trim the results and convert to a float
                X = PD0.BAD_VELOCITY;
                Y = PD0.BAD_VELOCITY;
                Z = PD0.BAD_VELOCITY;
                Q = PD0.BAD_VELOCITY;
                Single.TryParse(result[1].Trim(), out X);
                Single.TryParse(result[2].Trim(), out Y);
                Single.TryParse(result[3].Trim(), out Z);
                Single.TryParse(result[4].Trim(), out Q);

                // Check for Bad Velocity
                if (X == PD0.BAD_VELOCITY)
                {
                    X = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (Y == PD0.BAD_VELOCITY)
                {
                    Y = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (Z == PD0.BAD_VELOCITY)
                {
                    Z = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (Q == PD0.BAD_VELOCITY)
                {
                    Q = DataSet.Ensemble.BAD_VELOCITY;
                }

                if (result[5].Contains('A'))
                {
                    IsGood = true;
                }
                else
                {
                    IsGood = false;
                }
            }
        }
    }

    /// <summary>
    /// Bottom Track, ship-referenced velocity data. 
    /// </summary>
    public class BS : IDvlData
    {
        /// <summary>
        /// Bottom Track, ship-referenced velocity data ID.
        /// </summary>
        public const string ID = ":BS";

        /// <summary>
        /// Number of elements.
        /// Includes the ID.
        /// </summary>
        public const int NUM_ELEM = 5;

        /// <summary>
        /// +/- Transverse velocity data in mm/s.
        /// (+ = Port Stbd XDCR movement relative to water mass).
        /// </summary>
        public float T;

        /// <summary>
        /// +/- Longitudinal velocity data in mm/s.
        /// (+ = Aft Fwd XDCR movement relative to water mass).
        /// </summary>
        public float L;

        /// <summary>
        /// +/- Z-axis velocity data in mm/s.
        /// (+ = transducer movement away from water mass.).
        /// </summary>
        public float N;

        /// <summary>
        /// Status of velocity data.  
        /// (A = good, V = bad)
        /// </summary>
        public bool IsGood;

        /// <summary>
        /// Decode the sentence given.
        /// </summary>
        /// <param name="sent">Sentence to decode</param>
        public BS(string sent)
        {
            // Set the sentence
            Sentence = sent;

            // Decode the sentence
            string[] result = sent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length >= NUM_ELEM && result[0].Contains(ID))
            {
                // Remove the plus sign
                result[1].Replace("+", String.Empty);
                result[2].Replace("+", String.Empty);
                result[3].Replace("+", String.Empty);

                // Trim the results and convert to a float
                T = PD0.BAD_VELOCITY;
                L = PD0.BAD_VELOCITY;
                N = PD0.BAD_VELOCITY;
                Single.TryParse(result[1].Trim(), out T);
                Single.TryParse(result[2].Trim(), out L);
                Single.TryParse(result[3].Trim(), out N);

                // Check for Bad Velocity
                if (T == PD0.BAD_VELOCITY)
                {
                    T = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (L == PD0.BAD_VELOCITY)
                {
                    L = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (N == PD0.BAD_VELOCITY)
                {
                    N = DataSet.Ensemble.BAD_VELOCITY;
                }

                if (result[4].Contains('A'))
                {
                    IsGood = true;
                }
                else
                {
                    IsGood = false;
                }
            }
        }
    }

    /// <summary>
    /// Bottom Track, Earth-referenced velocity data. 
    /// </summary>
    public class BE : IDvlData
    {
        /// <summary>
        /// Bottom Track, earth-referenced velocity data ID.
        /// </summary>
        public const string ID = ":BE";

        /// <summary>
        /// Number of elements.
        /// Includes the ID.
        /// </summary>
        public const int NUM_ELEM = 5;

        /// <summary>
        /// +/- East (u-axis) velocity data in mm/s.
        /// (+ = ADCP movement to east).
        /// </summary>
        public float E;

        /// <summary>
        /// +/- North (v-axis) velocity data in mm/s.
        /// (+ = ADCP movement to north).
        /// </summary>
        public float N;

        /// <summary>
        /// +/- Upward (w-axis) velocity data in mm/s.
        /// (+ = ADCP movement to surface).
        /// </summary>
        public float U;

        /// <summary>
        /// Status of velocity data.  
        /// (A = good, V = bad)
        /// </summary>
        public bool IsGood;

        /// <summary>
        /// Decode the sentence given.
        /// </summary>
        /// <param name="sent">Sentence to decode</param>
        public BE(string sent)
        {
            // Set the sentence
            Sentence = sent;

            // Decode the sentence
            string[] result = sent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length >= NUM_ELEM && result[0].Contains(ID))
            {
                // Remove the plus sign
                result[1].Replace("+", String.Empty);
                result[2].Replace("+", String.Empty);
                result[3].Replace("+", String.Empty);

                // Trim the results and convert to a float
                E = PD0.BAD_VELOCITY;
                N = PD0.BAD_VELOCITY;
                U = PD0.BAD_VELOCITY;
                Single.TryParse(result[1].Trim(), out E);
                Single.TryParse(result[2].Trim(), out N);
                Single.TryParse(result[3].Trim(), out U);

                // Check for Bad Velocity
                if (E == PD0.BAD_VELOCITY)
                {
                    E = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (N == PD0.BAD_VELOCITY)
                {
                    N = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (U == PD0.BAD_VELOCITY)
                {
                    U = DataSet.Ensemble.BAD_VELOCITY;
                }

                if (result[4].Contains('A'))
                {
                    IsGood = true;
                }
                else
                {
                    IsGood = false;
                }
            }
        }
    }

    /// <summary>
    /// Bottom Track, Earth-referenced velocity data. 
    /// </summary>
    public class BD : IDvlData
    {
        /// <summary>
        /// Bottom Track, earth-referenced distance data ID.
        /// </summary>
        public const string ID = ":BD";

        /// <summary>
        /// Number of elements.
        /// Includes the ID.
        /// </summary>
        public const int NUM_ELEM = 6;

        /// <summary>
        /// +/- East (u-axis) distance data in mm/s.
        /// (+ = ADCP movement to east).
        /// </summary>
        public float E;

        /// <summary>
        /// +/- North (v-axis) distance data in mm/s.
        /// (+ = ADCP movement to north).
        /// </summary>
        public float N;

        /// <summary>
        /// +/- Upward (w-axis) distance data in mm/s.
        /// (+ = ADCP movement to surface).
        /// </summary>
        public float U;

        /// <summary>
        /// Range to bottom in meters.
        /// </summary>
        public float RangeToBottom;

        /// <summary>
        /// Time since last good-velocity estimate in seconds.
        /// </summary>
        public float Time;

        /// <summary>
        /// Decode the sentence given.
        /// </summary>
        /// <param name="sent">Sentence to decode</param>
        public BD(string sent)
        {
            // Set the sentence
            Sentence = sent;

            // Decode the sentence
            string[] result = sent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length >= NUM_ELEM && result[0].Contains(ID))
            {
                // Remove the plus sign
                result[1].Replace("+", String.Empty);
                result[2].Replace("+", String.Empty);
                result[3].Replace("+", String.Empty);
                result[4].Replace("+", String.Empty);
                result[5].Replace("+", String.Empty);

                // Trim the results and convert to a float
                E = PD0.BAD_VELOCITY;
                N = PD0.BAD_VELOCITY;
                U = PD0.BAD_VELOCITY;
                RangeToBottom = 0.0f;
                Time = 0.0f;
                Single.TryParse(result[1].Trim(), out E);
                Single.TryParse(result[2].Trim(), out N);
                Single.TryParse(result[3].Trim(), out U);
                Single.TryParse(result[4].Trim(), out RangeToBottom);
                Single.TryParse(result[5].Trim(), out Time);

                // Check for Bad Velocity
                if (E == PD0.BAD_VELOCITY)
                {
                    E = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (N == PD0.BAD_VELOCITY)
                {
                    N = DataSet.Ensemble.BAD_VELOCITY;
                }
                if (U == PD0.BAD_VELOCITY)
                {
                    U = DataSet.Ensemble.BAD_VELOCITY;
                }
            }
        }
    }
}
