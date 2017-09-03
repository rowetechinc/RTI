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
 * 09/02/2017      RC          3.4.3      Made the spacing for the PD6 data match the documentation.
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

        /// <summary>
        /// Set the values for this object.
        /// Typically used to convert data to PD6 or PD13.
        /// </summary>
        /// <param name="pitch">Pitch in degrees.</param>
        /// <param name="roll">Roll in degrees.</param>
        /// <param name="heading">Heading in degrees.</param>
        public SA(float heading, float pitch, float roll)
        {
            this.Pitch = pitch;
            this.Roll = roll;
            this.Heading = heading;
        }

        /// <summary>
        /// Output the data back into the orignal format.
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ID);
            sb.Append(",");

            // Pitch
            if (this.Pitch >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(this.Pitch)).ToString("0.00")));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(this.Pitch)).ToString("0.00")));
            }
            sb.Append(",");

            // Roll
            if (this.Roll >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(this.Roll)).ToString("0.00")));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(this.Roll)).ToString("0.00")));
            }
            sb.Append(",");

            // Heading
            sb.Append(String.Format("{0, 5}", Heading.ToString("0.00")));

            // <CR><LF>
            sb.Append("\r\n");

            return sb.ToString();
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

        /// <summary>
        /// Create a TS object.
        /// </summary>
        /// <param name="dt">Date and Time</param>
        /// <param name="hundrethSec">Hundredth of a second.</param>
        /// <param name="salinity">Salinity in PPT</param>
        /// <param name="temp">Temperature in degrees C.</param>
        /// <param name="xdcr_depth">Transducer depth in meters.</param>
        /// <param name="sos">Spedd of Sound in m/s.</param>
        /// <param name="bit">BIT Status.</param>
        /// <param name="leak">Lead Detection.</param>
        public TS(DateTime dt, byte hundrethSec, float salinity, float temp, float xdcr_depth, float sos, int bit, RTI.DataSet.DvlDataSet.LeakDetectionOptions leak)
        {
            this.DateAndTime = dt;
            this.HundredthSec = hundrethSec;
            this.Salinity = salinity;
            this.Temperature = temp;
            this.DepthOfTransducer = xdcr_depth;
            this.SpeedOfSound = sos;
            this.BIT = bit;
            this.LeakDetection = leak;
        }

        /// <summary>
        /// Output the data back into the orignal format.
        /// </summary>
        /// <returns></returns>
        /// <param name="isLeakDetect">Flag if Leak Detect value should be included.</param>
        public string Encode(bool isLeakDetect = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ID);
            sb.Append(",");

            // Date and Time
            sb.Append(this.DateAndTime.ToString("yyMMddHHmmss"));
            sb.Append(this.HundredthSec.ToString("00"));
            sb.Append(",");

            // Salinity
            sb.Append(String.Format("{0, 4}", this.Salinity.ToString("0.0")));
            sb.Append(",");

            // Temperature
            if (this.Temperature >= 0)
            {
                sb.Append(String.Format("{0, 5}", "+" + ((int)Math.Round(this.Temperature)).ToString("0.0")));
            }
            else
            {
                sb.Append(String.Format("{0, 5}", ((int)Math.Round(this.Temperature)).ToString("0.0")));
            }
            sb.Append(",");

            // Depth of XDCR
            sb.Append(String.Format("{0, 6}", this.DepthOfTransducer.ToString("0.0")));
            sb.Append(",");

            // Speed of Sound
            sb.Append(String.Format("{0, 6}", this.SpeedOfSound.ToString("0.0")));
            sb.Append(",");

            // BIT
            sb.Append(String.Format("{0, 3}", this.BIT));
            
            if(isLeakDetect)
            {
                sb.Append(",");
                // Leak Detector
                switch (this.LeakDetection)
                {
                    case RTI.DataSet.DvlDataSet.LeakDetectionOptions.OK:
                        sb.Append(LEAKDETECT_OK);
                        break;
                    case RTI.DataSet.DvlDataSet.LeakDetectionOptions.WaterDetected:
                        sb.Append(LEAKDETECT_WATER);
                        break;
                    case RTI.DataSet.DvlDataSet.LeakDetectionOptions.NotInstalled:
                    default:
                        sb.Append(LEAKDETECT_NOT_INSTALLED);
                        break;
                }
            }

            // <CR><LF>
            sb.Append("\r\n");

            return sb.ToString();
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

        /// <summary>
        /// Create RA object.
        /// </summary>
        /// <param name="pressure">Pressure in kPa.</param>
        /// <param name="rangeB0">Range to Bottom Beam 0 in decimeters.</param>
        /// <param name="rangeB1">Range to Bottom Beam 1 in decimeters.</param>
        /// <param name="rangeB2">Range to Bottom Beam 2 in decimeters.</param>
        /// <param name="rangeB3">Range to Bottom Beam 3 in decimeters.</param>
        public RA(float pressure, float rangeB0, float rangeB1, float rangeB2, float rangeB3)
        {
            this.Pressure = pressure;
            this.RangeToBottomB0 = rangeB0;
            this.RangeToBottomB1 = rangeB1;
            this.RangeToBottomB2 = rangeB2;
            this.RangeToBottomB3 = rangeB3;
        }

        /// <summary>
        /// Output the data back into the orignal format.
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ID);
            sb.Append(",");

            // Pressure
            sb.Append(((int)Math.Round(this.Pressure)).ToString());
            sb.Append(",");

            // Range Beam 0
            sb.Append(((int)Math.Round(this.RangeToBottomB0)).ToString());
            sb.Append(",");

            // Range Beam 1
            sb.Append(((int)Math.Round(this.RangeToBottomB1)).ToString());
            sb.Append(",");
            
            // Range Beam 2
            sb.Append(((int)Math.Round(this.RangeToBottomB2)).ToString());
            sb.Append(",");

            // Range Beam 3
            sb.Append(((int)Math.Round(this.RangeToBottomB3)).ToString());

            // <CR><LF>
            sb.Append("\r\n");


            return sb.ToString();
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

        /// <summary>
        /// Create the WI object.
        /// </summary>
        /// <param name="x">X Water Mass Velocity in mm/s.</param>
        /// <param name="y">Y Water Mass Velocity in mm/s.</param>
        /// <param name="z">Z Water Mass Velocity in mm/s.</param>
        /// <param name="q">Error Water Mass Velocity in mm/s.</param>
        /// <param name="isGood">Flag if data is good.</param>
        public WI(float x, float y, float z, float q, bool isGood)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Q = q;
            this.IsGood = isGood;
        }

        /// <summary>
        /// Output the data back into the orignal format.
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ID);
            sb.Append(",");

            // X
            float x = this.X;
            if (x == DataSet.Ensemble.BAD_VELOCITY)
            {
                x = PD0.BAD_VELOCITY;
            }
            if (x >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+"+((int)Math.Round(x)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(x)).ToString()));
            }
            sb.Append(",");

            // Y
            float y = this.Y;
            if (y == DataSet.Ensemble.BAD_VELOCITY)
            {
                y = PD0.BAD_VELOCITY;
            }
            if (y >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(y)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(y)).ToString()));
            }
            sb.Append(",");

            // Z
            float z = this.Z;
            if (z == DataSet.Ensemble.BAD_VELOCITY)
            {
                z = PD0.BAD_VELOCITY;
            }
            if (z >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(z)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(z)).ToString()));
            }
            sb.Append(",");

            // Q
            float q = this.Q;
            if (q == DataSet.Ensemble.BAD_VELOCITY)
            {
                q = PD0.BAD_VELOCITY;
            }
            if (q >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(q)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(q)).ToString()));
            }
            sb.Append(",");

            // Is Good
            if (IsGood)
            {
                sb.Append("A");
            }
            else
            {
                sb.Append("V");
            }

            // <CR><LF>
            sb.Append("\r\n");


            return sb.ToString();
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

        /// <summary>
        /// Create the WS object.
        /// </summary>
        /// <param name="t">+/- Transverse velocity data in mm/s.</param>
        /// <param name="l">+/- Longitudinal velocity data in mm/s.</param>
        /// <param name="n">+/- Z-axis velocity data in mm/s.</param>
        /// <param name="isGood">Flag if data is good.</param>
        public WS(float t, float l, float n, bool isGood)
        {
            this.T = t;
            this.L = l;
            this.N = n;
            this.IsGood = isGood;
        }

        /// <summary>
        /// Output the data back into the orignal format.
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ID);
            sb.Append(",");

            // Transverse
            float t = this.T;
            if (t == DataSet.Ensemble.BAD_VELOCITY)
            {
                t = PD0.BAD_VELOCITY;
            }
            if (t >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(t)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(t)).ToString()));
            }
            sb.Append(",");

            // Longitudinal
            float l = this.L;
            if (l == DataSet.Ensemble.BAD_VELOCITY)
            {
                l = PD0.BAD_VELOCITY;
            }
            if (l >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(l)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(l)).ToString()));
            }
            sb.Append(",");

            // Vertical
            float n = this.N;
            if (n == DataSet.Ensemble.BAD_VELOCITY)
            {
                n = PD0.BAD_VELOCITY;
            }
            if (n >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(n)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(n)).ToString()));
            }
            sb.Append(",");

            // Is Good
            if (IsGood)
            {
                sb.Append("A");
            }
            else
            {
                sb.Append("V");
            }

            // <CR><LF>
            sb.Append("\r\n");


            return sb.ToString();
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

        /// <summary>
        /// Create the WE object.
        /// </summary>
        /// <param name="east">East Water Mass Velocity in mm/s.</param>
        /// <param name="north">North Water Mass Velocity in mm/s.</param>
        /// <param name="vert">Vertical Water Mass Velocity in mm/s.</param>
        /// <param name="isGood">Flag if data is good.</param>
        public WE(float east, float north, float vert, bool isGood)
        {
            this.E = east;
            this.N = north;
            this.U = vert;
            this.IsGood = isGood;
        }

        /// <summary>
        /// Output the data back into the orignal format.
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ID);
            sb.Append(", ");

            // East
            float east = this.E;
            if (east == DataSet.Ensemble.BAD_VELOCITY)
            {
                east = PD0.BAD_VELOCITY;
            }
            if (east >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(east)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(east)).ToString()));
            }
            sb.Append(",");

            // North
            float north = this.N;
            if (north == DataSet.Ensemble.BAD_VELOCITY)
            {
                north = PD0.BAD_VELOCITY;
            }
            if (north >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(north)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(north)).ToString()));
            }
            sb.Append(",");

            // Z
            float vertical = this.U;
            if (vertical == DataSet.Ensemble.BAD_VELOCITY)
            {
                vertical = PD0.BAD_VELOCITY;
            }
            if (vertical >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(vertical)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(vertical)).ToString()));
            }
            sb.Append(",");

            // Is Good
            if (IsGood)
            {
                sb.Append("A");
            }
            else
            {
                sb.Append("V");
            }

            // <CR><LF>
            sb.Append("\r\n");


            return sb.ToString();
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

        /// <summary>
        /// Create the WD object.
        /// </summary>
        /// <param name="east">East Water Mass Velocity in mm/s.</param>
        /// <param name="north">North Water Mass Velocity in mm/s.</param>
        /// <param name="vert">Vertical Water Mass Velocity in mm/s.</param>
        /// <param name="rangeToWmCenter">Range to the WM center in meter.</param>
        /// <param name="time">Time since last good-velocity estimate in seconds.</param>
        public WD(float east, float north, float vert, float rangeToWmCenter, float time)
        {
            this.E = east;
            this.N = north;
            this.U = vert;
            this.RangeToWmCenter = rangeToWmCenter;
            this.Time = time;
        }

        /// <summary>
        /// Output the data back into the orignal format.
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ID);
            sb.Append(", ");

            // East
            float east = this.E;
            if (east == DataSet.Ensemble.BAD_VELOCITY)
            {
                east = PD0.BAD_VELOCITY;
            }
            if (east >= 0)
            {
                sb.Append(String.Format("{0, 12}", "+" + ((int)Math.Round(east)).ToString("0.00")));
            }
            else
            {
                sb.Append(String.Format("{0, 12}", ((int)Math.Round(east)).ToString("0.00")));
            }
            sb.Append(",");

            // North
            float north = this.N;
            if (north == DataSet.Ensemble.BAD_VELOCITY)
            {
                north = PD0.BAD_VELOCITY;
            }
            if (north >= 0)
            {
                sb.Append(String.Format("{0, 12}", "+" + ((int)Math.Round(north)).ToString("0.00")));
            }
            else
            {
                sb.Append(String.Format("{0, 12}", ((int)Math.Round(north)).ToString("0.00")));
            }
            sb.Append(",");

            // Z
            float vertical = this.U;
            if (vertical == DataSet.Ensemble.BAD_VELOCITY)
            {
                vertical = PD0.BAD_VELOCITY;
            }
            if (vertical >= 0)
            {
                sb.Append(String.Format("{0, 12}", "+" + ((int)Math.Round(vertical)).ToString("0.00")));
            }
            else
            {
                sb.Append(String.Format("{0, 12}", ((int)Math.Round(vertical)).ToString("0.00")));
            }
            sb.Append(",");

            // Range to Center of WM
            sb.Append(String.Format("{0, 7}", ((int)Math.Round(this.RangeToWmCenter)).ToString("0.00")));
            sb.Append(",");

            // Time
            sb.Append(String.Format("{0, 6}", ((int)Math.Round(this.Time)).ToString("0.00")));

            // <CR><LF>
            sb.Append("\r\n");


            return sb.ToString();
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

        /// <summary>
        /// Create the WI object.
        /// </summary>
        /// <param name="x">X Water Mass Velocity in mm/s.</param>
        /// <param name="y">Y Water Mass Velocity in mm/s.</param>
        /// <param name="z">Z Water Mass Velocity in mm/s.</param>
        /// <param name="q">Error Water Mass Velocity in mm/s.</param>
        /// <param name="isGood">Flag if data is good.</param>
        public BI(float x, float y, float z, float q, bool isGood)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Q = q;
            this.IsGood = isGood;
        }

        /// <summary>
        /// Output the data back into the orignal format.
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ID);
            sb.Append(", ");

            // X
            float x = this.X;
            if (x == DataSet.Ensemble.BAD_VELOCITY)
            {
                x = PD0.BAD_VELOCITY;
            }
            if (x >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(x)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(x)).ToString()));
            }
            sb.Append(",");

            // Y
            float y = this.Y;
            if (y == DataSet.Ensemble.BAD_VELOCITY)
            {
                y = PD0.BAD_VELOCITY;
            }
            if (y >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(y)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(y)).ToString()));
            }
            sb.Append(",");

            // Z
            float z = this.Z;
            if (z == DataSet.Ensemble.BAD_VELOCITY)
            {
                z = PD0.BAD_VELOCITY;
            }
            if (z >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(z)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(z)).ToString()));
            }
            sb.Append(",");

            // Q
            float q = this.Q;
            if (q == DataSet.Ensemble.BAD_VELOCITY)
            {
                q = PD0.BAD_VELOCITY;
            }
            if (q >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(q)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(q)).ToString()));
            }
            sb.Append(",");

            // Is Good
            if (IsGood)
            {
                sb.Append("A");
            }
            else
            {
                sb.Append("V");
            }

            // <CR><LF>
            sb.Append("\r\n");


            return sb.ToString();
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

        /// <summary>
        /// Create the BS object.
        /// </summary>
        /// <param name="t">+/- Transverse velocity data in mm/s.</param>
        /// <param name="l">+/- Longitudinal velocity data in mm/s.</param>
        /// <param name="n">+/- Z-axis velocity data in mm/s.</param>
        /// <param name="isGood">Flag if data is good.</param>
        public BS(float t, float l, float n, bool isGood)
        {
            this.T = t;
            this.L = l;
            this.N = n;
            this.IsGood = isGood;
        }

        /// <summary>
        /// Output the data back into the orignal format.
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ID);
            sb.Append(", ");

            // Transverse
            float t = this.T;
            if (t == DataSet.Ensemble.BAD_VELOCITY)
            {
                t = PD0.BAD_VELOCITY;
            }
            if (t >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(t)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(t)).ToString()));
            }
            sb.Append(",");

            // Longitudinal
            float l = this.L;
            if (l == DataSet.Ensemble.BAD_VELOCITY)
            {
                l = PD0.BAD_VELOCITY;
            }
            if (l >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(l)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(l)).ToString()));
            }
            sb.Append(",");

            // Vertical
            float n = this.N;
            if (n == DataSet.Ensemble.BAD_VELOCITY)
            {
                n = PD0.BAD_VELOCITY;
            }
            if (n >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(n)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(n)).ToString()));
            }
            sb.Append(",");

            // Is Good
            if (IsGood)
            {
                sb.Append("A");
            }
            else
            {
                sb.Append("V");
            }

            // <CR><LF>
            sb.Append("\r\n");


            return sb.ToString();
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

        /// <summary>
        /// Create the BE object.
        /// </summary>
        /// <param name="east">East Water Mass Velocity in mm/s.</param>
        /// <param name="north">North Water Mass Velocity in mm/s.</param>
        /// <param name="vert">Vertical Water Mass Velocity in mm/s.</param>
        /// <param name="isGood">Flag if data is good.</param>
        public BE(float east, float north, float vert, bool isGood)
        {
            this.E = east;
            this.N = north;
            this.U = vert;
            this.IsGood = isGood;
        }

        /// <summary>
        /// Output the data back into the orignal format.
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ID);
            sb.Append(", ");

            // East
            float east = this.E;
            if (east == DataSet.Ensemble.BAD_VELOCITY)
            {
                east = PD0.BAD_VELOCITY;
            }
            if (east >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(east)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(east)).ToString()));
            }
            sb.Append(",");

            // North
            float north = this.N;
            if (north == DataSet.Ensemble.BAD_VELOCITY)
            {
                north = PD0.BAD_VELOCITY;
            }
            if (north >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(north)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(north)).ToString()));
            }
            sb.Append(",");

            // Z
            float vertical = this.U;
            if (vertical == DataSet.Ensemble.BAD_VELOCITY)
            {
                vertical = PD0.BAD_VELOCITY;
            }
            if (vertical >= 0)
            {
                sb.Append(String.Format("{0, 6}", "+" + ((int)Math.Round(vertical)).ToString()));
            }
            else
            {
                sb.Append(String.Format("{0, 6}", ((int)Math.Round(vertical)).ToString()));
            }
            sb.Append(",");

            // Is Good
            if (IsGood)
            {
                sb.Append("A");
            }
            else
            {
                sb.Append("V");
            }

            // <CR><LF>
            sb.Append("\r\n");


            return sb.ToString();
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

        /// <summary>
        /// Create the BD object.
        /// </summary>
        /// <param name="east">East Water Mass Velocity in mm/s.</param>
        /// <param name="north">North Water Mass Velocity in mm/s.</param>
        /// <param name="vert">Vertical Water Mass Velocity in mm/s.</param>
        /// <param name="rangeToBottom">Range to Bottom in meters.</param>
        /// <param name="time">Time since last good-velocity estimate in seconds.</param>
        public BD(float east, float north, float vert, float rangeToBottom, float time)
        {
            this.E = east;
            this.N = north;
            this.U = vert;
            this.RangeToBottom = rangeToBottom;
            this.Time = time;
        }

        /// <summary>
        /// Output the data back into the orignal format.
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ID);
            sb.Append(", ");

            // East
            float east = this.E;
            if (east == DataSet.Ensemble.BAD_VELOCITY)
            {
                east = PD0.BAD_VELOCITY;
            }
            if (east >= 0)
            {
                sb.Append(String.Format("{0, 12}", "+" + ((int)Math.Round(east)).ToString("0.00")));
            }
            else
            {
                sb.Append(String.Format("{0, 12}", ((int)Math.Round(east)).ToString("0.00")));
            }
            sb.Append(",");

            // North
            float north = this.N;
            if (north == DataSet.Ensemble.BAD_VELOCITY)
            {
                north = PD0.BAD_VELOCITY;
            }
            if (north >= 0)
            {
                sb.Append(String.Format("{0, 12}", "+" + ((int)Math.Round(north)).ToString("0.00")));
            }
            else
            {
                sb.Append(String.Format("{0, 12}", ((int)Math.Round(north)).ToString("0.00")));
            }
            sb.Append(",");

            // Z
            float vertical = this.U;
            if (vertical == DataSet.Ensemble.BAD_VELOCITY)
            {
                vertical = PD0.BAD_VELOCITY;
            }
            if (vertical >= 0)
            {
                sb.Append(String.Format("{0, 12}", "+" + ((int)Math.Round(vertical)).ToString("0.00")));
            }
            else
            {
                sb.Append(String.Format("{0, 12}", ((int)Math.Round(vertical)).ToString("0.00")));
            }
            sb.Append(",");

            float range = this.RangeToBottom;
            sb.Append(String.Format("{0, 7}", ((int)Math.Round(range)).ToString("0.00")));
            sb.Append(", ");

            float time = this.Time;
            sb.Append(String.Format("{0, 6}", ((int)Math.Round(time)).ToString("0.00")));

            // <CR><LF>
            sb.Append("\r\n");


            return sb.ToString();
        }
    }
}
