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
 * 03/12/2014      RC          3.4.8      Initial coding
 * 
 * 
 * 
 */

namespace RTI
{
    using DotSpatial.Positioning;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Echo Intensity Data Type.
    /// </summary>
    public class Pd0NmeaData : Pd0DataType
    {
        #region Variable

        /// <summary>
        /// Setup logger to report errors.
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// LSB for the ID for the PD0 NMEA data type.
        /// </summary>
        public const byte ID_LSB = 0x22;

        /// <summary>
        /// MSB for the ID for the PD0 NMEA data type.
        /// </summary>
        public const byte ID_MSB = 0x20;

        #region V2

        /// <summary>
        /// GGA ID LSB
        /// </summary>
        public const byte ID_NMEA_GGA_V2_LSB = 0x00;

        /// <summary>
        /// GGA ID MSB
        /// </summary>
        public const byte ID_NMEA_GGA_V2_MSB = 0x68;

        /// <summary>
        /// VTG ID LSB
        /// </summary>
        public const byte ID_NMEA_VTG_V2_LSB = 0x00;

        /// <summary>
        /// VTG ID MSB
        /// </summary>
        public const byte ID_NMEA_VTG_V2_MSB = 0x69;

        /// <summary>
        /// HBT ID LSB
        /// </summary>
        public const byte ID_NMEA_HBT_V2_LSB = 0x00;

        /// <summary>
        /// HBT ID MSB
        /// </summary>
        public const byte ID_NMEA_HBT_V2_MSB = 0x6A;

        /// <summary>
        /// HDT ID LSB
        /// </summary>
        public const byte ID_NMEA_HDT_V2_LSB = 0x00;

        /// <summary>
        /// HDT ID MSB
        /// </summary>
        public const byte ID_NMEA_HDT_V2_MSB = 0x6B;

        #endregion

        #region ADCP Integrated NMEA

        /// <summary>
        /// GGA ID LSB
        /// </summary>
        public const byte ID_NMEA_GGA_AIN_LSB = 0xCC;

        /// <summary>
        /// GGA ID MSB
        /// </summary>
        public const byte ID_NMEA_GGA_AIN_MSB = 0x00;

        /// <summary>
        /// VTG ID LSB
        /// </summary>
        public const byte ID_NMEA_VTG_AIN_LSB = 0xCD;

        /// <summary>
        /// VTG ID MSB
        /// </summary>
        public const byte ID_NMEA_VTG_AIN_MSB = 0x00;

        /// <summary>
        /// HBT ID LSB
        /// </summary>
        public const byte ID_NMEA_HBT_AIN_LSB = 0xCE;

        /// <summary>
        /// HBT ID MSB
        /// </summary>
        public const byte ID_NMEA_HBT_AIN_MSB = 0x00;

        /// <summary>
        /// HDT ID LSB
        /// </summary>
        public const byte ID_NMEA_HDT_AIN_LSB = 0xCF;

        /// <summary>
        /// HDT ID MSB
        /// </summary>
        public const byte ID_NMEA_HDT_AIN_MSB = 0x00;

        /// <summary>
        /// HDT ID LSB
        /// </summary>
        public const byte ID_NMEA_OTHER_AIN_LSB = 0xC8;

        /// <summary>
        /// HDT ID MSB
        /// </summary>
        public const byte ID_NMEA_OTHER_AIN_MSB = 0x00;

        #endregion

        #region GPS Structs

        /// <summary>
        /// GGA number of bytes.
        /// </summary>
        public const int BYTES_PER_GGA = 57;

        /// <summary>
        /// VTG number of bytes.
        /// </summary>
        public const int BYTES_PER_VTG = 28;

        /// <summary>
        /// VTG number of bytes.
        /// </summary>
        public const int BYTES_PER_HDT = 16;

        /// <summary>
        /// DBT number of bytes.
        /// </summary>
        public const int BYTES_PER_DBT = 22;

        #endregion

        /// <summary>
        /// Number of bytes for the header.
        /// The LSB and MSB.
        /// </summary>
        public const ushort BYTES_PER_HEADER = 14;

        /// <summary>
        /// Used to account for th end of the NMEA string's \n\r\0
        /// </summary>
        public const ushort BYTES_PER_NEWLINE_CR_NULL = 3;

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

        /// <summary>
        /// Last GPS GPHDT message received in this dataset.
        /// GPS Heading.
        /// </summary>
        [JsonIgnore]
        public GphdtSentence GPHDT { get; set; }

        #endregion

        /// <summary>
        /// Initialize the object.
        /// </summary>
        public Pd0NmeaData()
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.NmeaData)
        {

        }

        /// <summary>
        /// Initialize and decode the given data.
        /// </summary>
        /// <param name="data">PD0 Binary data.</param>
        /// <param name="offset">Offset in the binary data.</param>
        public Pd0NmeaData(byte[] data, ushort offset)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.BottomTrack)
        {
            // Initialize and decode the data
            this.Offset = offset;
            Decode(data);
        }

        /// <summary>
        /// Initialize the object.
        /// </summary>
        /// <param name="nmea">RTI NMEA data.</param>
        public Pd0NmeaData(DataSet.NmeaDataSet nmea)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.NmeaData)
        {
            DecodeRtiEnsemble(nmea);
        }
        /// <summary>
        /// Encode the data type to binary PD0 format.
        /// </summary>
        /// <returns>Binary PD0 format.</returns>
        public override byte[] Encode()
        {
            // Create the bytes
            List<byte> nmeaData = new List<byte>();
            double dTime = 0.0;

            foreach (string nmea in NmeaStrings)
            {
                if (nmea.Contains("GGA"))
                {
                    byte[] gga = ProcessGpsMsg(nmea, dTime, ID_NMEA_GGA_AIN_LSB, ID_NMEA_GGA_AIN_MSB);
                    nmeaData.AddRange(gga);
                }
                else if (nmea.Contains("HDT"))
                {
                    byte[] hdt = ProcessGpsMsg(nmea, dTime, ID_NMEA_HDT_AIN_LSB, ID_NMEA_HDT_AIN_MSB);
                    nmeaData.AddRange(hdt);
                }
                else if (nmea.Contains("VTG"))
                {
                    byte[] vtg = ProcessGpsMsg(nmea, dTime, ID_NMEA_HDT_AIN_LSB, ID_NMEA_VTG_AIN_MSB);
                    nmeaData.AddRange(vtg);
                }
                else if (nmea.Contains("HBT"))
                {
                    byte[] hbt = ProcessGpsMsg(nmea, dTime, ID_NMEA_HBT_AIN_LSB, ID_NMEA_HBT_AIN_MSB);
                    nmeaData.AddRange(hbt);
                }
                else
                {
                    byte[] other = ProcessGpsMsg(nmea, dTime, ID_NMEA_OTHER_AIN_LSB, ID_NMEA_OTHER_AIN_MSB);
                    nmeaData.AddRange(other);
                }
            }

            return nmeaData.ToArray();
        }

        /// <summary>
        /// Decode the given binary PD0 data in the object.
        /// </summary>
        /// <param name="data">Binary PD0 data.</param>
        public override void Decode(byte[] data)
        {
            // Message size is without the header
            // Or it is bytes data[4] and data[5].
            int msgSize = data.Length - BYTES_PER_HEADER;

            // Verify we have data passed the header
            if (msgSize > 0)
            {
                // Get the string from the message
                byte[] msg = new byte[msgSize];
                Buffer.BlockCopy(data, BYTES_PER_HEADER, msg, 0, msgSize);

                // Convert the msg to a string and add to dataset
                AddNmea(BitConverter.ToString(msg));
            }
        }

        /// <summary>
        /// Get the number of bytes in the data type.
        /// This is based off the number of NMEA strings in the data set.
        /// </summary>
        /// <returns>Number of bytes for the data type.</returns>
        public override int GetDataTypeSize()
        {
            // Each NMEA string has a 14 byte header
            int numBytes = 0;
            foreach(string nmea in NmeaStrings)
            {
                numBytes += Pd0NmeaData.GetDataTypeSize(nmea);
            }

            return numBytes;
        }

        /// <summary>
        /// Return the number of header offsets that need to be created
        /// for the NMEA data type.
        /// </summary>
        /// <returns>Number of NMEA strings to create a data type offset.</returns>
        public int GetNumOfHeaderOffsets()
        {
            return NmeaStrings.Count;
        }

        /// <summary>
        /// Get the data type size for each NMEA string.
        /// 
        /// Header + NMEA String + \n\r\0 
        /// 
        /// </summary>
        /// <param name="nmea">NMEA string including the $ and the checksum.</param>
        /// <returns>Number of bytes for the data type.</returns>
        public static int GetDataTypeSize(string nmea)
        {
            // Header + NMEA String + \n\r\0
            return BYTES_PER_HEADER + nmea.Trim().Length + BYTES_PER_NEWLINE_CR_NULL;
        }

        /// <summary>
        /// Give at least the first 6 bytes of the message.
        /// Data[4] and data[5] give the message size.  Then add 14 bytes for the header to get the total
        /// size of the data set.
        /// </summary>
        /// <param name="data">Data to get the NMEA Data size.</param>
        /// <returns>Size of the data set..</returns>
        public static int GetNmeaDataSize(byte[] data)
        {
            // Verify there is enough data
            if(data.Length < 6)
            {
                // Convert the hex values to int
                return MathHelper.ByteArrayToUInt16(data, 4) + BYTES_PER_HEADER;
            }

            return 0;

        }

        #region RTI Ensemble

        /// <summary>
        /// Convert the RTI Amplitude data set to the PD0 Echo Intensity data type.
        /// </summary>
        /// <param name="nmeaDS">RTI Amplitude data set.</param>
        public void DecodeRtiEnsemble(DataSet.NmeaDataSet nmeaDS)
        {
            NmeaStrings = nmeaDS.NmeaStrings;
            GPGGA = nmeaDS.GPGGA;
            GPGLL = nmeaDS.GPGLL;
            GPGSA = nmeaDS.GPGSA;
            GPGSV = nmeaDS.GPGSV;
            GPHDT = nmeaDS.GPHDT;
            GPRMC = nmeaDS.GPRMC;
            GPVTG = nmeaDS.GPVTG;
            PGRMF = nmeaDS.PGRMF;
        }


        #region GPS Message

        /// <summary>
        /// Process the GPS message to a byte array.
        /// </summary>
        /// <param name="gpsString">GPS String.</param>
        /// <param name="dTime">Delta Time.</param>
        /// <param name="subIdLsb">GPS String Subtype ID LSB.</param>
        /// <param name="subIdMsb">GPS String Subtype ID MSB</param>
        /// <returns>Byte array of the GPS message.</returns>
        private byte[] ProcessGpsMsg(string gpsString, double dTime, byte subIdLsb, byte subIdMsb)
        {
            // Add \r\n\u
            // Carriage Return 0x0d
            // New Line 0x0a
            // Null 0x00
            // First trim, then add the characters
            gpsString = gpsString.Trim();
            gpsString += "\r\n" + char.MinValue;        // char.MinValue is a null terminated string

            // Message Size
            ushort msgSize = (ushort)(BYTES_PER_HEADER + gpsString.Length);
            byte msgSizeLsb, msgSizeMsb;
            MathHelper.LsbMsbUShort(msgSize, out msgSizeLsb, out msgSizeMsb);

            // Delta Time
            byte[] deltaTime = BitConverter.GetBytes(dTime);
            if (deltaTime.Length < 8)
            {
                deltaTime = new byte[8];
            }

            // Create the array
            byte[] data = new byte[msgSize];

            data[0] = ID_LSB;                                   // General ID LSB 0x20
            data[1] = ID_MSB;                                   // General ID MSB 0x22
            data[2] = subIdLsb;                                 // Specific ID LSB
            data[3] = subIdMsb;                                 // Subtype ID MSB 
            data[4] = msgSizeLsb;                               // Message Size LSB
            data[5] = msgSizeMsb;                               // Message Size MSB
            data[6] = deltaTime[0];                             // Delta Time 
            data[7] = deltaTime[1];                             // Delta Time 
            data[8] = deltaTime[2];                             // Delta Time 
            data[9] = deltaTime[3];                             // Delta Time 
            data[10] = deltaTime[4];                            // Delta Time 
            data[11] = deltaTime[5];                            // Delta Time 
            data[12] = deltaTime[6];                            // Delta Time 
            data[13] = deltaTime[7];                            // Delta Time

            // Msg
            byte[] msg = Encoding.ASCII.GetBytes(gpsString);

            // Copy the message to the result
            Buffer.BlockCopy(msg, 0, data, 14, msg.Length);

            return data;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="gga"></param>
        ///// <returns></returns>
        //private byte[] ProcessGGA(string gga)
        //{
        //    // Add \r\n\u
        //    // Carriage Return 0x0d
        //    // New Line 0x0a
        //    // Null 0x00
        //    // First trim, then add the characters
        //    gga = gga.Trim();
        //    gga += "\r\n";

        //    // Message Size
        //    ushort msgSize = (ushort)(BYTES_PER_HEADER + gga.Length);
        //    byte msgSizeLsb, msgSizeMsb;
        //    MathHelper.LsbMsbUShort(msgSize, out msgSizeLsb, out msgSizeMsb);

        //    // Delta Time
        //    double dTime = 0.0;
        //    byte[] deltaTime = BitConverter.GetBytes(dTime);
        //    if (deltaTime.Length < 8)
        //    {
        //        deltaTime = new byte[8];
        //    }

        //    // Create the array
        //    byte[] data = new byte[msgSize];

        //    data[0] = ID_LSB;                                   // General ID LSB 0x20
        //    data[1] = ID_MSB;                                   // General ID MSB 0x22
        //    data[2] = ID_NMEA_GGA_V2_LSB;                       // Specific ID LSB 0x01
        //    data[3] = ID_NMEA_GGA_V2_MSB;                       // Specific ID MSB 0x04
        //    data[4] = msgSizeLsb;                               // Message Size LSB
        //    data[5] = msgSizeMsb;                               // Message Size MSB
        //    data[6] = deltaTime[0];                             // Delta Time 
        //    data[7] = deltaTime[1];                             // Delta Time 
        //    data[8] = deltaTime[2];                             // Delta Time 
        //    data[9] = deltaTime[3];                             // Delta Time 
        //    data[10] = deltaTime[4];                            // Delta Time 
        //    data[11] = deltaTime[5];                            // Delta Time 
        //    data[12] = deltaTime[6];                            // Delta Time 
        //    data[13] = deltaTime[7];                            // Delta Time

        //    // Msg
        //    byte[] msg = Encoding.ASCII.GetBytes(gga);

        //    // Copy the message to the result
        //    Buffer.BlockCopy(msg, 0, data, 14, msg.Length);

        //    return data;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="gga"></param>
        ///// <returns></returns>
        //private byte[] DecodeGGAStruct(GpggaSentence gga)
        //{

        //    // Message Size
        //    ushort msgSize = BYTES_PER_HEADER + BYTES_PER_GGA;
        //    byte msgSizeLsb, msgSizeMsb;
        //    MathHelper.LsbMsbUShort(msgSize, out msgSizeLsb, out msgSizeMsb);

        //    // Delta Time
        //    double dTime = 0.0;
        //    byte[] deltaTime = BitConverter.GetBytes(dTime);
        //    if(deltaTime.Length < 8)
        //    {
        //        deltaTime = new byte[8];
        //    }

        //    // Create the array
        //    byte[] data = new byte[msgSize];

        //    data[0] = ID_LSB;                                   // General ID LSB 0x20
        //    data[1] = ID_MSB;                                   // General ID MSB 0x22
        //    data[2] = ID_NMEA_GGA_V2_LSB;                       // Specific ID LSB 0x01
        //    data[3] = ID_NMEA_GGA_V2_MSB;                       // Specific ID MSB 0x04
        //    data[4] = msgSizeLsb;                               // Message Size LSB
        //    data[5] = msgSizeMsb;                               // Message Size MSB
        //    data[6] = deltaTime[0];                             // Delta Time 
        //    data[7] = deltaTime[1];                             // Delta Time 
        //    data[8] = deltaTime[2];                             // Delta Time 
        //    data[9] = deltaTime[3];                             // Delta Time 
        //    data[10] = deltaTime[4];                            // Delta Time 
        //    data[11] = deltaTime[5];                            // Delta Time 
        //    data[12] = deltaTime[6];                            // Delta Time 
        //    data[13] = deltaTime[7];                            // Delta Time 

        //    // Msg Body

        //    #region $GPGGA Header

        //    byte[] szHeader = Encoding.ASCII.GetBytes(gga.CommandWord);
        //    if (szHeader.Length != 7)
        //    {
        //        szHeader = new byte[7];
        //    }
        //    data[14] = szHeader[0];                             // Header
        //    data[15] = szHeader[1];                             // Header
        //    data[16] = szHeader[2];                             // Header
        //    data[17] = szHeader[3];                             // Header
        //    data[18] = szHeader[4];                             // Header
        //    data[19] = szHeader[5];                             // Header
        //    data[20] = szHeader[6];                             // Header

        //    #endregion

        //    #region HHMMSS.SS

        //    byte[] szUTC = new byte[10];
        //    if (gga.Words.Length > 2)
        //    {
        //        szUTC = Encoding.ASCII.GetBytes(gga.Words[1]);
        //        if (szUTC.Length != 10)
        //        {
        //            szUTC = new byte[10];
        //        }
        //    }

        //    data[21] = szUTC[0];                                // UTC time
        //    data[22] = szUTC[1];                                // UTC time
        //    data[23] = szUTC[2];                                // UTC time
        //    data[24] = szUTC[3];                                // UTC time
        //    data[25] = szUTC[4];                                // UTC time
        //    data[26] = szUTC[5];                                // UTC time
        //    data[27] = szUTC[6];                                // UTC time
        //    data[28] = szUTC[7];                                // UTC time
        //    data[29] = szUTC[8];                                // UTC time
        //    data[30] = szUTC[9];                                // UTC time

        //    #endregion

        //    #region Latitude

        //    byte[] dLatitude = new byte[8];
        //    dLatitude = MathHelper.DoubleToByteArray(gga.Position.Latitude.DecimalDegrees);
        //    if (dLatitude.Length != 8)
        //    {
        //        dLatitude = new byte[8];
        //    }

        //    byte tcNS = Convert.ToByte('N');
        //    if(gga.Position.Latitude.Hemisphere == LatitudeHemisphere.South)
        //    {
        //        tcNS = Convert.ToByte('S');
        //    }

        //    data[31] = dLatitude[0];                                // Latitude
        //    data[32] = dLatitude[1];                                // Latitude
        //    data[33] = dLatitude[2];                                // Latitude
        //    data[34] = dLatitude[3];                                // Latitude
        //    data[35] = dLatitude[4];                                // Latitude
        //    data[36] = dLatitude[5];                                // Latitude
        //    data[37] = dLatitude[6];                                // Latitude
        //    data[38] = dLatitude[7];                                // Latitude

        //    data[39] = tcNS;                                        // North or South

        //    #endregion

        //    #region Longitude

        //    byte[] dLongitude = new byte[8];
        //    dLongitude = MathHelper.DoubleToByteArray(gga.Position.Longitude.DecimalDegrees);
        //    if (dLongitude.Length != 8)
        //    {
        //        dLongitude = new byte[8];
        //    }

        //    byte tcEW = Convert.ToByte('E');
        //    if (gga.Position.Longitude.Hemisphere == LongitudeHemisphere.West)
        //    {
        //        tcNS = Convert.ToByte('W');
        //    }

        //    data[40] = dLongitude[0];                               // Latitude
        //    data[41] = dLongitude[1];                               // Latitude
        //    data[42] = dLongitude[2];                               // Latitude
        //    data[43] = dLongitude[3];                               // Latitude
        //    data[44] = dLongitude[4];                               // Latitude
        //    data[45] = dLongitude[5];                               // Latitude
        //    data[46] = dLongitude[6];                               // Latitude
        //    data[47] = dLongitude[7];                               // Latitude

        //    data[48] = tcNS;                                        // East or West

        //    #endregion

        //    #region Quality

        //    byte ucQuality = Convert.ToByte(gga.FixQuality);

        //    data[49] = ucQuality;                               // Quality

        //    #endregion

        //    #region Number of Satellites

        //    byte ucNmbSat = Convert.ToByte(gga.FixedSatelliteCount);

        //    data[50] = ucNmbSat;                               // Number of Satellites

        //    #endregion

        //    #region HDOP

        //    byte[] fHDOP = MathHelper.FloatToByteArray(gga.HorizontalDilutionOfPrecision.Value);
        //    if(fHDOP.Length != 4)
        //    {
        //        fHDOP = new byte[4];
        //    }

        //    data[51] = fHDOP[0];                                // HDOP
        //    data[52] = fHDOP[1];                                // HDOP
        //    data[53] = fHDOP[2];                                // HDOP
        //    data[54] = fHDOP[3];                                // HDOP

        //    #endregion

        //    #region Altitude

        //    byte[] fAltitude = MathHelper.FloatToByteArray((float)gga.Altitude.ToMeters().Value);       // Force to Meters
        //    if (fAltitude.Length != 4)
        //    {
        //        fAltitude = new byte[4];
        //    }

        //    byte tcAltUnit = Convert.ToByte('M');

        //    data[55] = fAltitude[0];                            // Altitude
        //    data[56] = fAltitude[1];                            // Altitude
        //    data[57] = fAltitude[2];                            // Altitude
        //    data[58] = fAltitude[3];                            // Altitude

        //    data[59] = tcAltUnit;                               // Altitude unit

        //    #endregion

        //    #region Geoid

        //    byte[] fGeoid = MathHelper.FloatToByteArray((float)gga.GeoidalSeparation.ToMeters().Value);       // Force to Meters
        //    if (fGeoid.Length != 4)
        //    {
        //        fGeoid = new byte[4];
        //    }

        //    byte tcGeoidUnit = Convert.ToByte('M');

        //    data[60] = fGeoid[0];                                   // Altitude
        //    data[61] = fGeoid[1];                                   // Altitude
        //    data[62] = fGeoid[2];                                   // Altitude
        //    data[63] = fGeoid[3];                                   // Altitude

        //    data[64] = tcGeoidUnit;                                 // Altitude unit

        //    #endregion

        //    #region Age of DGPS

        //    byte[] fAgeDGPS = MathHelper.FloatToByteArray((float)gga.DifferentialGpsAge.TotalSeconds);       // Force to Seconds
        //    if (fAgeDGPS.Length != 4)
        //    {
        //        fAgeDGPS = new byte[4];
        //    }

        //    data[65] = fAgeDGPS[0];                                     // Age of Differential GPS data
        //    data[66] = fAgeDGPS[1];                                     // Age of Differential GPS data
        //    data[67] = fAgeDGPS[2];                                     // Age of Differential GPS data
        //    data[68] = fAgeDGPS[3];                                     // Age of Differential GPS data

        //    #endregion

        //    #region Ref Station ID

        //    byte[] sRefStationId = MathHelper.UInt16ToByteArray((UInt16)gga.DifferentialGpsStationID);
        //    if (sRefStationId.Length != 2)
        //    {
        //        sRefStationId = new byte[2];
        //    }

        //    data[69] = sRefStationId[0];                                     // Differential GPS Station ID
        //    data[66] = sRefStationId[1];                                     // Differential GPS Station ID

        //    #endregion

        //    return data;
        //}

        #endregion

        #region Add NMEA
        
        /// <summary>
        /// Add another NMEA string to the dataset.
        /// </summary>
        /// <param name="nmeaString"></param>
        public void AddNmea(string nmeaString)
        {
            // Create a NMEA sentence and verify its valid
            NmeaSentence sentence = new NmeaSentence(nmeaString);
            if (sentence.IsValid)
            {
                // Add to the list
                NmeaStrings.Add(sentence.Sentence);

                // Set values from the NMEA data
                SetValues(sentence);
            }
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

            try
            {
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
                if (sentence.CommandWord.EndsWith("HDT", StringComparison.Ordinal))
                {
                    // Yes.  Convert it using the fast pre-parseed constructor
                    GPHDT = new GphdtSentence(sentence.Sentence);
                }
            }
            catch (Exception e)
            {
                log.Error("Error decoding a NMEA sentance.", e);
            }
        }

        #endregion

        #endregion


    }
}
