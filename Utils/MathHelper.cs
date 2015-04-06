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
 * 01/05/2012      RC          1.11       Initial coding
 * 01/16/2012      RC          1.14       Added Calculate Magnitude and direction.
 * 02/14/2012      RC          2.03       Fixed standard deviation equation.
 * 02/24/2012      RC          2.03       Added DegreeToRadian.
 * 03/29/2012      RC          2.07       Added the methods from Converters.cs
 * 04/10/2012      RC          2.08       Changed ByteArrayToInt() to ByteArrayToInt8() and ByteArrayToInt32().
 *                                         Added MB_TO_BYTES.
 * 06/04/2012      RC          2.11       Added ParseDouble().
 * 06/22/2012      RC          2.12       Added PercentError() and PercentDifference() methods.
 * 07/06/2012      RC          2.12       Added ByteArrayToDouble() and ByteArrayToFloat64().
 * 07/19/2012      RC          2.12       When parsing the byte arrays, verify the byte arrays given are the correct size.
 * 07/30/2012      RC          2.13       Added MemorySizeString() to display file memory sizes with the highest scale factor.
 * 12/21/2012      RC          2.17       Removed the abolute value from PercentError per Steve Maier request.  He wants negative percent error.
 * 06/13/2013      RC          2.19       Added HUNSEC_TO_MILLISEC.
 * 07/31/2013      RC          2.19.3     Added BoolToTrueFalseStr() and BoolToOnOffStr().
 * 11/18/2013      RC          2.21.0     Added TimeSpanPrettyFormat() to give the time span as a string with plurization.
 * 12/05/2013      RC          2.21.0     Added seconds to TimeSpanPrettyFormat().
 * 02/24/2014      RC          2.21.4     Added MsbLsb().
 * 07/23/2014      RC          2.23.0     Added functions to set and get byte arrays for strings.
 * 07/28/2014      RC          2.23.0     Added a try/catch block in SubArray().
 * 10/17/2014      RC          3.0.2      Added milliseconds in TimeSpanPrettyFormat().
 * 10/31/2014      RC          3.0.2      Added DoubleToByteArray().
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using log4net;


namespace RTI
{
    /// <summary>
    /// Include additional math equations.
    /// </summary>
    public class MathHelper
    {
        #region Variables

        /// <summary>
        /// Logger for logging error messages.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Value to convert megabytes to bytes.
        /// </summary>
        public const int MB_TO_BYTES = 1048576;

        /// <summary>
        /// Multiply HUNSEC * HUNSEC_TO_MILLISEC to convert
        /// hundredth of a second to milliseconds.
        /// 
        /// Divide MILLISECOND / HUNSEC_TO_MILLISEC to convert
        /// millisecond to Hundredth of a second.
        /// 
        /// </summary>
        public const double HUNSEC_TO_MILLISEC = 10.0;

        #endregion

        #region Standard Deviation

        /// <summary>
        /// Calculate the population standard deviation for the 4 values given.
        /// It is assumed that every value is used to calculate the standard deviation.
        /// If we were only given a small sample of points, then the sample standard deviation should be used.
        /// </summary>
        /// <param name="v1">Value 1.</param>
        /// <param name="v2">Value 2.</param>
        /// <param name="v3">Value 3.</param>
        /// <param name="v4">Value 4.</param>
        /// <param name="avg">Output the calculate average.</param>
        /// <returns>Standard deviation of 4 values.</returns>
        public static double StandardDev(double v1, double v2, double v3, double v4, out double avg)
        {
            avg = 0;
            int count = 0;

            // Omit any bad values (0 Bad)
            if (v1 != 0)
            {
                avg += v1;
                count++;
            }
            if (v2 != 0)
            {
                avg += v2;
                count++;
            }
            if (v3 != 0)
            {
                avg += v3;
                count++;
            }
            if (v4 != 0)
            {
                avg += v4;
                count++;
            }

            // Calculate the average
            // If nothing to average return 0
            if (count > 0)
            {
                avg /= count;
            }
            else
            {
                avg = 0;
                return 0;
            }

            double variance = 0;
            if (v1 != 0)
            {
                variance += Math.Pow(v1 - avg, 2);
            }
            if (v2 != 0)
            {
                variance += Math.Pow(v2 - avg, 2);
            }
            if (v3 != 0)
            {
                variance += Math.Pow(v3 - avg, 2);
            }
            if (v4 != 0)
            {
                variance += Math.Pow(v4 - avg, 2);
            }

            // Calculate the variance
            variance /= count;

            // Return the standard deviation
            return Math.Sqrt(variance);
        }

        /// <summary>
        /// Calculate the population standard deviation for the list of values given.
        /// It is assumed that every value is used to calculate the standard deviation.
        /// If we were only given a small sample of points, then the sample standard deviation should be used.
        /// </summary>
        /// <param name="values">Values to calculate.</param>
        /// <param name="BAD_VALUE">Bad value to omit.</param>
        /// <param name="avg">Average of all the values in the list excluding the bad values.</param>
        /// <returns>Population standard deviation of the values excluding the bad values.</returns>
        public static double StandardDev(List<double> values, double BAD_VALUE, out double avg)
        {
            avg = 0;
            int count = 0;
            double variance = 0;

            // Calculate average
            foreach (double value in values)
            {
                // Omit any bad values
                if (value != BAD_VALUE)
                {
                    avg += value;
                    count++;
                }
            }

            // Calculate the average
            // If nothing to average return 0
            if (count > 0)
            {
                avg /= count;
            }
            else
            {
                avg = 0;
                return 0;
            }

            // Calculate variance
            foreach (double value in values)
            {
                // Omit any bad values
                if (value != BAD_VALUE)
                {
                    variance += Math.Pow(value - avg, 2);
                }
            }

            
            // Calculate the variance
            variance /= count;

            // Return the standard deviation
            return Math.Sqrt(variance);
        }

        #endregion

        #region Vectors

        /// <summary>
        /// Calculate the Magnitude given the North, East and Vertical velocity.
        /// </summary>
        /// <param name="east">East Velocity.</param>
        /// <param name="north">North Velocity.</param>
        /// <param name="vertical">Vertical Velocity.</param>
        /// <returns>Magnitude of the velocities given.</returns>
        public static double CalculateMagnitude(double east, double north, double vertical)
        {
            return Math.Sqrt((east * east) + (north * north) + (vertical * vertical));
        }

        /// <summary>
        /// Calculate the Direction of the velocities given.
        /// Value will be returned in degrees.  Give the Y axis as the first parameter.
        /// </summary>
        /// <param name="y">Y axis velocity value.</param>
        /// <param name="x">X axis velocity value.</param>
        /// <returns>Direction of the velocity return in degrees.</returns>
        public static double CalculateDirection(double y, double x)
        {
            return (Math.Atan2(y, x)) * (180.0 / Math.PI);
        }

        /// <summary>
        /// Convert the given angle to radians.
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns>Angle in radians.</returns>
        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        #endregion

        #region Byte Array Math

        /// <summary>
        /// Struct to hold the bytes and the integer or float.
        /// Conversion will be done by populating the values.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct TestUnion
        {
            [FieldOffset(0)]
            public byte Byte1;
            [FieldOffset(1)]
            public byte Byte2;
            [FieldOffset(2)]
            public byte Byte3;
            [FieldOffset(3)]
            public byte Byte4;
            [FieldOffset(4)]
            public byte Byte5;
            [FieldOffset(5)]
            public byte Byte6;
            [FieldOffset(6)]
            public byte Byte7;
            [FieldOffset(7)]
            public byte Byte8;
            [FieldOffset(0)]
            public float Float;
            [FieldOffset(0)]
            public int Int;
            [FieldOffset(0)]
            public double Double;
        }


        /// <summary>
        /// Convert the bytes to a 32 bit (4 bytes) float.
        /// 
        /// Check if the data is in big endian form.
        /// If the system is in Little Endian, then
        /// the byte converter will not convert properly
        /// and the bytes will have to be reversed.
        /// Big Endian      = Most Significant ------ Least Significant     (ARM, 68k, ...)     (As Displayed on a calculator)
        /// Little Endian   = Least Significant ------ Most Significant     (x86)               (Reverse order on a calculator) (Windows)
        /// 
        /// If Bad return 0.
        /// </summary>
        /// <param name="data">4 Byte of data for a float.</param>
        /// <param name="index">Index in the array to start.</param>
        /// <param name="isBigEndian">Flag used to determine if the given byte array is in Big Endian.</param>
        /// <returns>Float value from the byte array.</returns>
        public static float ByteArrayToFloat(byte[] data, int index, bool isBigEndian = false)
        {
            // Number of bytes the data should at least contain
            int expectedLen = 4;

            // Ensure the data exist and there is enough data
            if (data == null || data.Length < expectedLen || index + expectedLen > data.Length)
            {
                return 0;
            }

            // Check if the system is Little Endian
            // and the data was given as Big Endian
            if (BitConverter.IsLittleEndian && isBigEndian)
            {
                // Then reverse the values to be in Little Endian
                TestUnion result = new TestUnion();
                result.Byte4 = data[index++];
                result.Byte3 = data[index++];
                result.Byte2 = data[index++];
                result.Byte1 = data[index];

                return result.Float;
            }
            // We are on a Little Endian System
            // and the data is given as Little Endian
            else
            {
                return BitConverter.ToSingle(data, index);
            }
        }

        /// <summary>
        /// Convert the bytes to a 64 bit (8 bytes) Float64.
        /// 
        /// Check if the data is in big endian form.
        /// If the system is in Little Endian, then
        /// the byte converter will not convert properly
        /// and the bytes will have to be reversed.
        /// Big Endian      = Most Significant ------ Least Significant     (ARM, 68k, ...)     (As Displayed on a calculator)
        /// Little Endian   = Least Significant ------ Most Significant     (x86)               (Reverse order on a calculator) (Windows)
        /// 
        /// If Bad return 0.
        /// </summary>
        /// <param name="data">8 Byte of data for a Float64 (Double).</param>
        /// <param name="index">Index in the array to start.</param>
        /// <param name="isBigEndian">Flag used to determine if the given byte array is in Big Endian.</param>
        /// <returns>Float value from the byte array.</returns>
        public static double ByteArrayToFloat64(byte[] data, int index, bool isBigEndian = false)
        {
            // Number of bytes the data should at least contain
            int expectedLen = 8;

            // Ensure the data exist and there is enough data
            if (data == null || data.Length < expectedLen || index + expectedLen > data.Length)
            {
                return 0;
            }

            // Check if the system is Little Endian
            // and the data was given as Big Endian
            if (BitConverter.IsLittleEndian && isBigEndian)
            {
                // Then reverse the values to be in Little Endian
                TestUnion result = new TestUnion();
                result.Byte8 = data[index++];
                result.Byte7 = data[index++];
                result.Byte6 = data[index++];
                result.Byte5 = data[index++];
                result.Byte4 = data[index++];
                result.Byte3 = data[index++];
                result.Byte2 = data[index++];
                result.Byte1 = data[index];

                return result.Double;
            }
            // We are on a Little Endian System
            // and the data is given as Little Endian
            else
            {
                return BitConverter.ToDouble(data, index);
            }
        }

        /// <summary>
        /// Convert the bytes to a 64 bit (8 bytes) Double.
        /// 
        /// Check if the data is in big endian form.
        /// If the system is in Little Endian, then
        /// the byte converter will not convert properly
        /// and the bytes will have to be reversed.
        /// Big Endian      = Most Significant ------ Least Significant     (ARM, 68k, ...)     (As Displayed on a calculator)
        /// Little Endian   = Least Significant ------ Most Significant     (x86)               (Reverse order on a calculator) (Windows)
        /// 
        /// If Bad return 0.
        /// </summary>
        /// <param name="data">8 Byte of data for a Double.</param>
        /// <param name="index">Index in the array to start.</param>
        /// <param name="isBigEndian">Flag used to determine if the given byte array is in Big Endian.</param>
        /// <returns>Float value from the byte array.</returns>
        public static double ByteArrayToDouble(byte[] data, int index, bool isBigEndian = false)
        {
            return ByteArrayToFloat64(data, index, isBigEndian);
        }

        /// <summary>
        /// Convert the bytes to a 32 bit (4 bytes) Unsigned integer.
        /// 
        /// Check if the data is in big endian form.
        /// If the system is in Little Endian, then
        /// the byte converter will not convert properly
        /// and the bytes will have to be reversed.
        /// Big Endian      = Most Significant ------ Least Significant     (ARM, 68k, ...)     (As Displayed on a calculator)
        /// Little Endian   = Least Significant ------ Most Significant     (x86)               (Reverse order on a calculator)
        /// 
        /// If Bad return 0.
        /// </summary>
        /// <param name="data">Data to convert.</param>
        /// <param name="index">Location of the data.</param>
        /// <param name="isBigEndian">Flag used to determine if the given byte array is in Big Endian.</param>
        /// <returns>Unsigned integer.</returns>
        public static UInt32 ByteArrayToUInt32(byte[] data, int index, bool isBigEndian = false)
        {
            // Number of bytes the data should at least contain
            int expectedLen = 4;

            // Ensure the data exist and there is enough data
            if (data == null || data.Length < expectedLen || index + expectedLen > data.Length)
            {
                return 0;
            }

            // Check if the system is Little Endian
            // and the data was given as Big Endian
            if (BitConverter.IsLittleEndian && isBigEndian)
            {
                // Then reverse the values to be in Little Endian
                return (UInt32)((data[index] << 24) | data[index + 1] << 16 | (data[index + 2] << 8) | data[index + 3]);
            }
            // We are on a Little Endian System
            // and the data is given as Little Endian
            else
            {
                return (UInt32)((data[index + 3] << 24) | data[index + 2] << 16 | (data[index + 1] << 8) | data[index]);
            }
        }

        /// <summary>
        /// Convert the bytes to a 32 bit (4 bytes) Unsigned integer.
        /// 
        /// Check if the data is in big endian form.
        /// If the system is in Little Endian, then
        /// the byte converter will not convert properly
        /// and the bytes will have to be reversed.
        /// Big Endian      = Most Significant ------ Least Significant     (ARM, 68k, ...)     (As Displayed on a calculator)
        /// Little Endian   = Least Significant ------ Most Significant     (x86)               (Reverse order on a calculator)
        /// 
        /// If Bad return 0.
        /// </summary>
        /// <param name="data">Data to convert.</param>
        /// <param name="index">Location of the data.</param>
        /// <param name="isBigEndian">Flag used to determine if the given byte array is in Big Endian.</param>
        /// <returns>Unsigned integer.</returns>
        public static int ByteArrayToInt32(byte[] data, int index, bool isBigEndian = false)
        {
            // Number of bytes the data should at least contain
            int expectedLen = 4;

            // Ensure the data exist and there is enough data
            if (data == null || data.Length < expectedLen || index + expectedLen > data.Length)
            {
                return 0;
            }

            // Check if the system is Little Endian
            // and the data was given as Big Endian
            if (BitConverter.IsLittleEndian && isBigEndian)
            {
                // Then reverse the values to be in Little Endian
                return (int)((data[index] << 24) | data[index + 1] << 16 | (data[index + 2] << 8) | data[index + 3]);
            }
            // We are on a Little Endian System
            // and the data is given as Little Endian
            else
            {
                return (int)((data[index + 3] << 24) | data[index + 2] << 16 | (data[index + 1] << 8) | data[index]);
            }
        }

        /// <summary>
        /// Convert the bytes to a 16 bit (2 bytes) Unsigned integer.
        /// Shift values to create value.
        /// 
        /// Check if the data is in big endian form.
        /// If the system is in Little Endian, then
        /// the byte converter will not convert properly
        /// and the bytes will have to be reversed.
        /// Big Endian      = Most Significant ------ Least Significant     (ARM, 68k, ...)     (As Displayed on a calculator)
        /// Little Endian   = Least Significant ------ Most Significant     (x86)               (Reverse order on a calculator)
        /// 
        /// If Bad return 0.
        /// </summary>
        /// <param name="data">Data received.</param>
        /// <param name="index">Index in the array to start.</param>
        /// <param name="isBigEndian">Flag used to determine if the given byte array is in Big Endian.</param>
        /// <returns>UInt16 from the index location.</returns>
        public static UInt16 ByteArrayToUInt16(byte[] data, int index, bool isBigEndian = false)
        {
            // Number of bytes the data should at least contain
            int expectedLen = 2;

            // Ensure the data exist and there is enough data
            if (data == null || data.Length < expectedLen || index + expectedLen > data.Length)
            {
                return 0;
            }

            // Check if the system is Little Endian
            // and the data was given as Big Endian
            if (BitConverter.IsLittleEndian && isBigEndian)
            {
                // Then reverse the values to be in Little Endian
                return (UInt16)((data[index] << 8) | data[index+1]);


            }
            // We are on a Little Endian System
            // and the data is given as Little Endian
            else
            {
                return (UInt16)((data[index+1] << 8) | data[index]);
            }

        }

        /// <summary>
        /// Convert the single byte given in the data at the given
        /// index into a integer value.
        /// 
        /// If Bad return 0.
        /// </summary>
        /// <param name="data">Data received.</param>
        /// <param name="index">Index in the array to start.</param>
        /// <returns>UInt16 from the index location.</returns>
        public static int ByteArrayToInt8(byte[] data, int index)
        {
            // Ensure there is a byte to work with
            // starting from the index
            if (data == null || index > data.Length)
            {
                return 0;
            }

            return (int)data[index];
        }

        /// <summary>
        /// Convert the single byte given in the data at the given
        /// index into a boolean value.
        /// 
        /// Bad Index return False.
        /// Anything greater than 0 return true;  0x01 - 0xFF
        /// Only 0x00 is false.
        /// </summary>
        /// <param name="data">Data received.</param>
        /// <param name="index">Index in the array to start.</param>
        /// <returns>Boolean from the index location.</returns>
        public static bool ByteArrayToBoolean(byte[] data, int index)
        {
            if (data == null || index > data.Length)
            {
                return false;
            }

            return BitConverter.ToBoolean(data, index);
        }

        /// <summary>
        /// Convert the given float to a byte array.
        /// The return value is a array containing the bytes.
        /// The byte array will be returned in Little Endian
        /// unless the inBigEndian is set to true.
        /// 
        /// Check if the data is in big endian form.
        /// If the system is in Little Endian, then
        /// the byte converter will not convert properly
        /// and the bytes will have to be reversed.
        /// Big Endian      = Most Significant ------ Least Significant     (ARM, 68k, ...)     (As Displayed on a calculator)
        /// Little Endian   = Least Significant ------ Most Significant     (x86)               (Reverse order on a calculator)
        /// 
        /// </summary>
        /// <param name="value">Float value to convert.</param>
        /// <param name="inBigEndian">Set to True if you want to results returned in Big Endian format.</param>
        /// <returns>Byte array which contains the bytes of the conversion.</returns>
        public static byte[] FloatToByteArray(float value, bool inBigEndian = false)
        {
            // Check for a bad value
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                byte[] tmp = new byte[4];
                tmp[0] = 0;
                tmp[1] = 0;
                tmp[2] = 0;
                tmp[3] = 0;
                return tmp;
            }

            TestUnion result = new TestUnion();
            result.Float = value;

            byte[] resultArray = new byte[4];
            resultArray[0] = result.Byte1;
            resultArray[1] = result.Byte2;
            resultArray[2] = result.Byte3;
            resultArray[3] = result.Byte4;

            // Reverse the order if data needs to be in Big Endian
            // and the system is in Little Endian
            if (BitConverter.IsLittleEndian && inBigEndian)
            {
                Array.Reverse(resultArray);
            }

            return resultArray;
        }

        /// <summary>
        /// Convert the given double to a byte array.
        /// The return value is a array containing the bytes.
        /// The byte array will be returned in Little Endian
        /// unless the inBigEndian is set to true.
        /// 
        /// Check if the data is in big endian form.
        /// If the system is in Little Endian, then
        /// the byte converter will not convert properly
        /// and the bytes will have to be reversed.
        /// Big Endian      = Most Significant ------ Least Significant     (ARM, 68k, ...)     (As Displayed on a calculator)
        /// Little Endian   = Least Significant ------ Most Significant     (x86)               (Reverse order on a calculator)
        /// 
        /// </summary>
        /// <param name="value">Double value to convert.</param>
        /// <param name="inBigEndian">Set to True if you want to results returned in Big Endian format.</param>
        /// <returns>Byte array which contains the bytes of the conversion.</returns>
        public static byte[] DoubleToByteArray(double value, bool inBigEndian = false)
        {
            // Check for a bad value
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                byte[] tmp = new byte[8];
                tmp[0] = 0;
                tmp[1] = 0;
                tmp[2] = 0;
                tmp[3] = 0;
                tmp[4] = 0;
                tmp[5] = 0;
                tmp[6] = 0;
                tmp[7] = 0;
                return tmp;
            }

            TestUnion result = new TestUnion();
            result.Double = value;

            byte[] resultArray = new byte[8];
            resultArray[0] = result.Byte1;
            resultArray[1] = result.Byte2;
            resultArray[2] = result.Byte3;
            resultArray[3] = result.Byte4;
            resultArray[4] = result.Byte5;
            resultArray[5] = result.Byte6;
            resultArray[6] = result.Byte7;
            resultArray[7] = result.Byte8;

            // Reverse the order if data needs to be in Big Endian
            // and the system is in Little Endian
            if (BitConverter.IsLittleEndian && inBigEndian)
            {
                Array.Reverse(resultArray);
            }

            return resultArray;
        }

        /// <summary>
        /// Convert the given UInt32 to a byte array.
        /// The return value is a array containing the bytes.
        /// The byte array will be returned in Little Endian
        /// unless the inBigEndian is set to true.
        /// 
        /// Check if the data is in big endian form.
        /// If the system is in Little Endian, then
        /// the byte converter will not convert properly
        /// and the bytes will have to be reversed.
        /// Big Endian      = Most Significant ------ Least Significant     (ARM, 68k, ...)     (As Displayed on a calculator)
        /// Little Endian   = Least Significant ------ Most Significant     (x86)               (Reverse order on a calculator)
        /// 
        /// </summary>
        /// <param name="value">Float value to convert.</param>
        /// <param name="inBigEndian">Set to True if you want to results returned in Big Endian format.</param>
        /// <returns>Byte array which contains the bytes of the conversion.</returns>
        public static byte[] UInt32ToByteArray(UInt32 value,  bool inBigEndian = false)
        {
            byte[] temp = BitConverter.GetBytes(value);

            // If little endian and want in Big Endian, the bytes are in the
            // wrong order and reverse
            if (BitConverter.IsLittleEndian && inBigEndian)
            {
                Array.Reverse(temp);
            }

            return temp;
        }

        /// <summary>
        /// Convert the given UInt16 to a byte array.
        /// The return value is a array containing the bytes.
        /// The byte array will be returned in Little Endian
        /// unless the inBigEndian is set to true.
        /// 
        /// Check if the data is in big endian form.
        /// If the system is in Little Endian, then
        /// the byte converter will not convert properly
        /// and the bytes will have to be reversed.
        /// Big Endian      = Most Significant ------ Least Significant     (ARM, 68k, ...)     (As Displayed on a calculator)
        /// Little Endian   = Least Significant ------ Most Significant     (x86)               (Reverse order on a calculator)
        /// 
        /// </summary>
        /// <param name="value">Float value to convert.</param>
        /// <param name="inBigEndian">Set to True if you want to results returned in Big Endian format.</param>
        /// <returns>Byte array which contains the bytes of the conversion.</returns>
        public static byte[] UInt16ToByteArray(UInt16 value, bool inBigEndian = false)
        {
            byte[] temp = BitConverter.GetBytes(value);

            // If little endian and want in Big Endian, the bytes are in the
            // wrong order and reverse
            if (BitConverter.IsLittleEndian && inBigEndian)
            {
                Array.Reverse(temp);
            }

            return temp;
        }

        /// <summary>
        /// Convert the given Int to a byte array.
        /// The return value is a array containing the bytes.
        /// The byte array will be returned in Little Endian
        /// unless the inBigEndian is set to true.
        /// 
        /// Check if the data is in big endian form.
        /// If the system is in Little Endian, then
        /// the byte converter will not convert properly
        /// and the bytes will have to be reversed.
        /// Big Endian      = Most Significant ------ Least Significant     (ARM, 68k, ...)     (As Displayed on a calculator)
        /// Little Endian   = Least Significant ------ Most Significant     (x86)               (Reverse order on a calculator)
        /// 
        /// </summary>
        /// <param name="value">Float value to convert.</param>
        /// <param name="inBigEndian">Set to True if you want to results returned in Big Endian format.</param>
        /// <returns>Byte array which contains the bytes of the conversion.</returns>
        public static byte[] Int32ToByteArray(int value, bool inBigEndian = false)
        {
            TestUnion result = new TestUnion();
            result.Int = value;

            byte[] resultArray = new byte[4];
            resultArray[0] = result.Byte1;
            resultArray[1] = result.Byte2;
            resultArray[2] = result.Byte3;
            resultArray[3] = result.Byte4;

            // Reverse the order if data needs to be in Big Endian
            // and the system is in Little Endian
            if (BitConverter.IsLittleEndian && inBigEndian)
            {
                Array.Reverse(resultArray);
            }

            return resultArray;
        }

        /// <summary>
        /// Convert an UInt8 to byte array.
        /// Create an array with a single element.
        /// If the value given is greater then a byte,
        /// then 0 will be returned.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted to byte array.</returns>
        public static byte[] UInt8ToByteArray(int value)
        {
            // Check if the value is valid
            if (value < 0 || value > 255)
            {
                value = 0;
            }

            byte[] temp = new byte[1];
            temp[0] = (byte)value;

            return temp;
        }

        /// <summary>
        /// Convert an Boolean to byte array.
        /// The byte array must then be in Big Endian,
        /// so the byte array may need to be reversed.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted to byte array.</returns>
        public static byte[] BooleanToByteArray(bool value)
        {
            byte[] temp = new byte[1];

            if (value)
            {
                temp[0] = 1;
            }
            else
            {
                temp[0] = 0;
            }

            return temp;
        }

        /// <summary>
        /// Convert the given byte array to a string.
        /// </summary>
        /// <param name="packet">Ensemble containing data.</param>
        /// <param name="index">Location of first byte in Ensemble to convert.</param>
        /// <param name="len">Length of the string in bytes.</param>
        /// <param name="inBigEndian">Is the packet given in Big Endian form.</param>
        /// <returns>String value for the given byte array.</returns>
        public static string ByteArrayToString(byte[] packet, int len, int index, bool inBigEndian = false)
        {
            // Get the data from the packet
            byte[] value = SubArray<byte>(packet, index, len);

            // Reverse the order if data needs to be in Big Endian
            // and the system is in Little Endian
            if (BitConverter.IsLittleEndian && inBigEndian)
            {
                Array.Reverse(value);
            }

            string s = "";
            int i;
            for (i = 0; i < value.Length; i++)
            {
                s += (char)value[i];
            }
            return s;
        }

        /// <summary>
        /// Get the subarray of for the given array
        /// based off the index and length.  This
        /// can take any type of array.
        /// </summary>
        /// <typeparam name="T">Type of data within array.</typeparam>
        /// <param name="data">Array containing data.</param>
        /// <param name="index">Start point.</param>
        /// <param name="length">Number of bytes to copy to subarray.</param>
        /// <returns></returns>
        public static T[] SubArray<T>(T[] data, int index, int length)
        {
            // Create a new buffer
            T[] result = new T[length];

            try
            {
                // Copy the buffer
                System.Buffer.BlockCopy(data, index, result, 0, length);
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error getting subarray. Data Length: {0}, Data Index: {1}, Result Length: {2}", data.Length, index, length), e);
            }
            
            
            return result;
        }

        #endregion

        #region Double

        /// <summary>
        /// Parse a string to a double.
        /// Convert the value from other locale types.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <returns>Value parsed.</returns>
        public static double ParseDouble(string s)
        {
            if (s == null)
                return double.NaN;
            s = s.Replace(',', '.');
            double result;
            if (double.TryParse(s, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out result))
                return result;
            return double.NaN;
        }

        #endregion

        #region Percent Error

        /// <summary>
        /// Precent Error.
        /// Applied when comparing an experimental quantity, E, with a theoretical
        /// quantity, T, which is considered the "correct" value.  The percent error
        /// is the absolute value of the difference divided by the "correct" value 
        /// times 100.
        /// </summary>
        /// <param name="correctValue">The correct value.</param>
        /// <param name="experimentalValue">Experimental Value.</param>
        /// <returns>Percent error as a percent.</returns>
        public static double PercentError(double correctValue, double experimentalValue)
        {
            // Check for bad values
            if (correctValue == 0)
            {
                return 0;
            }

            //return Math.Abs((correctValue - experimentalValue) / correctValue) * 100;
            return ((correctValue - experimentalValue) / correctValue) * 100;
        }

        /// <summary>
        /// Applied when comparing two experimental quantities, E1 and E2, neither
        /// of which can be considered the "correct" value.  The percent difference
        /// is the absolute value of the difference over the mean times 100.
        /// </summary>
        /// <param name="exp1Value">Experimental Value 1.</param>
        /// <param name="exp2Value">Experimental Value 2.</param>
        /// <returns>Percent Difference as a percent.</returns>
        public static double PercentDifference(double exp1Value, double exp2Value)
        {
            // Check for bad values
            if (0.5 * (exp1Value + exp2Value) == 0)
            {
                return 0;
            }

            return ((Math.Abs(exp1Value - exp2Value)) / (0.5 * (exp1Value + exp2Value))) * 100;
        }

        #endregion

        #region Memory Size

        /// <summary>
        /// Display a pretty file size.  This will determine the 
        /// highest factor is can display for the size.  It will then
        /// display the value with the highest scale factor.
        /// http://sharpertutorials.com/pretty-format-bytes-kb-mb-gb/
        /// </summary>
        /// <param name="bytes">Bytes to display.</param>
        /// <returns>String with the size given has the highest scale factor.</returns>
        public static string MemorySizeString(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                {
                    return string.Format("{0:##.##} {1}", Decimal.Divide(bytes, max), order);
                }

                max /= scale;
            }
            return "0 Bytes";
        }

        #endregion

        #region Bool

        /// <summary>
        /// Convert the bool to a string of "True" or "False".
        /// </summary>
        /// <param name="val">Value to convert.</param>
        /// <returns>TRUE or False string.</returns>
        public static string BoolToTrueFalseStr(bool val)
        {
            if (val)
            {
                return "True";
            }

            return "False";
        }

        /// <summary>
        /// Convert the bool to a string of "On" or "Off".
        /// </summary>
        /// <param name="val">Value to convert.</param>
        /// <returns>On or Off string.</returns>
        public static string BoolToOnOffStr(bool val)
        {
            if (val)
            {
                return "On";
            }

            return "Off";
        }

        #endregion

        #region DateTime

        /// <summary>
        /// Give the time span with plurization.
        /// </summary>
        /// <param name="span">Time span between 2 date and times.</param>
        /// <returns>String of the time span.</returns>
        public static string TimeSpanPrettyFormat(TimeSpan span)
        {
            if (span == TimeSpan.Zero) return "0 minutes";

            var sb = new StringBuilder();
            if (span.Days > 0)
                sb.AppendFormat("{0} day{1} ", span.Days, span.Days > 1 ? "s" : String.Empty);
            if (span.Hours > 0)
                sb.AppendFormat("{0} hour{1} ", span.Hours, span.Hours > 1 ? "s" : String.Empty);
            if (span.Minutes > 0)
                sb.AppendFormat("{0} minute{1} ", span.Minutes, span.Minutes > 1 ? "s" : String.Empty);
            if (span.Seconds > 0)
                sb.AppendFormat("{0} second{1} ", span.Seconds, span.Seconds > 1 ? "s" : String.Empty);
            if (span.Milliseconds > 0)
                sb.AppendFormat("{0} millisecond{1} ", span.Milliseconds, span.Milliseconds > 1 ? "s" : String.Empty);
            return sb.ToString();
        }

        /// <summary>
        /// Get the time span between the two date times.  
        /// dt1 - dt2
        /// </summary>
        /// <param name="dt1">Date Time 1.</param>
        /// <param name="dt2">Date Time 2.</param>
        /// <returns>String of the time span.</returns>
        public static string TimeSpanPrettyFormat(DateTime dt1, DateTime dt2)
        {
            return TimeSpanPrettyFormat(dt1 - dt2);
        }

        /// <summary>
        /// Give the time value with plurization.
        /// </summary>
        /// <param name="tv">Time value.</param>
        /// <returns>String of the time span.</returns>
        public static string TimeValuePrettyFormat(RTI.Commands.TimeValue tv)
        {
            //if (span == TimeSpan.Zero) return "0 minutes";

            var sb = new StringBuilder();
            if (tv.Hour > 0)
                sb.AppendFormat("{0} hour{1} ", tv.Hour, tv.Hour > 1 ? "s" : String.Empty);
            if (tv.Minute > 0)
                sb.AppendFormat("{0} minute{1} ", tv.Minute, tv.Minute > 1 ? "s" : String.Empty);
            if (tv.Second > 0)
                sb.AppendFormat("{0} second{1} ", tv.Second, tv.Second > 1 ? "s" : String.Empty);
            if (tv.HunSec > 0)
                sb.AppendFormat("{0} millisecond{1} ", tv.HunSec * 0.1, tv.HunSec * 0.1 > 1 ? "s" : String.Empty);
            return sb.ToString();
        }

        #endregion

        #region MSB LSB

        /// <summary>
        /// Get the Most Significant Bit and Least Significant Bit for the given value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="lsb">Least Significant Bit.</param>
        /// <param name="msb">Most Significant Bit.</param>
        public static void LsbMsb(int value, out byte lsb, out byte msb)
        {
            lsb = (byte)(value & 0xFFu);
            msb = (byte)((value >> 8) & 0xFFu);
        }

        /// <summary>
        /// Get the Most Significant Bit and Least Significant Bit for the given value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="lsb">Least Significant Bit.</param>
        /// <param name="msb">Most Significant Bit.</param>
        public static void LsbMsbShort(short value, out byte lsb, out byte msb)
        {
            lsb = (byte)(value & 0xFFu);
            msb = (byte)(value >> 8);
        }

        /// <summary>
        /// Get the Most Significant Bit and Least Significant Bit for the given value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="lsb">Least Significant Bit.</param>
        /// <param name="msb">Most Significant Bit.</param>
        public static void LsbMsbUShort(ushort value, out byte lsb, out byte msb)
        {
            lsb = (byte)(value & 0xFFu);
            msb = (byte)(value >> 8);
        }

        /// <summary>
        /// Get the Most Significant Bit and Least Significant Bit for the given value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="lsb">Least Significant Bit.</param>
        /// <param name="msb">Most Significant Bit.</param>
        public static void LsbMsb(double value, out byte lsb, out byte msb)
        {
            lsb = 0;
            msb = 0;
            byte[] data = BitConverter.GetBytes(value);
            if (data.Length > 2)
            {
                lsb = data[6];
                msb = data[7];
            }
        }

        /// <summary>
        /// Get the Most Significant Bit and Least Significant Bit for the given value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="lsb">Least Significant Bit.</param>
        /// <param name="msb">Most Significant Bit.</param>
        public static void LsbMsb(float value, out byte lsb, out byte msb)
        {
            lsb = 0;
            msb = 0;
            byte[] data = BitConverter.GetBytes(value);
            if (data.Length > 2)
            {
                lsb = data[6];
                msb = data[7];
            }
        }

        /// <summary>
        /// Convert the LSB and MSB to an integer.
        /// </summary>
        /// <param name="lsb">Least Significant Bit.</param>
        /// <param name="msb">Most Significant Bit.</param>
        /// <returns>Integer from the LSB and MSB.</returns>
        public static int LsbMsbInt(byte lsb, byte msb)
        {
            return (msb << 8) + lsb;
        }

        /// <summary>
        /// Convert the LSB and MSB to an integer.
        /// </summary>
        /// <param name="lsb">Least Significant Bit.</param>
        /// <param name="msb">Most Significant Bit.</param>
        /// <returns>Integer from the LSB and MSB.</returns>
        public static uint LsbMsbUInt(byte lsb, byte msb)
        {
            return (uint)((msb << 8) + lsb);
        }

        /// <summary>
        /// Convert the LSB and MSB to an short.
        /// </summary>
        /// <param name="lsb">Least Significant Bit.</param>
        /// <param name="msb">Most Significant Bit.</param>
        /// <returns>Integer from the LSB and MSB.</returns>
        public static short LsbMsbShort(byte lsb, byte msb)
        {
            return (short)((msb << 8) + lsb);
        }

        /// <summary>
        /// Convert the LSB and MSB to an short.
        /// </summary>
        /// <param name="lsb">Least Significant Bit.</param>
        /// <param name="msb">Most Significant Bit.</param>
        /// <returns>Integer from the LSB and MSB.</returns>
        public static ushort LsbMsbUShort(byte lsb, byte msb)
        {
            return (ushort)((msb << 8) + lsb);
        }

        /// <summary>
        /// Convert the LSB and MSB to an uint.
        /// </summary>
        /// <param name="data">Data to get the bytes.</param>
        /// <param name="offset">Location in the data to start.</param>
        /// <returns>Unsigned Integer from the LSB and MSB.</returns>
        public static uint LsbMsbUInt(byte[] data, int offset)
        {
            //return (uint)(  (data[offset + 3] << 24) +
            //                (data[offset + 2] << 16) +
            //                (data[offset + 1] << 8) +
            //                (data[offset]));
            return BitConverter.ToUInt32(data, offset);
        }

        #endregion

        #region Bits

        /// <summary>
        /// Zero the bit in the given value.
        /// </summary>
        /// <param name="value">Value that contains the bit.</param>
        /// <param name="position">Position of the bit in the value.</param>
        /// <returns>New value with the bit set to zero.</returns>
        public static int ZeroBitInt(int value, int position)
        {
            return value & ~(1 << position);
        }

        /// <summary>
        /// Zero the bit in the given value.
        /// </summary>
        /// <param name="value">Value that contains the bit.</param>
        /// <param name="position">Position of the bit in the value.</param>
        /// <returns>New value with the bit set to zero.</returns>
        public static short ZeroBitShort(short value, int position)
        {
            return (short)(value & ~(1 << position));
        }

        /// <summary>
        /// Zero the bit in the given value.
        /// </summary>
        /// <param name="value">Value that contains the bit.</param>
        /// <param name="position">Position of the bit in the value.</param>
        /// <returns>New value with the bit set to zero.</returns>
        public static ushort ZeroBitUShort(ushort value, int position)
        {
            return (ushort)(value & ~(1 << position));
        }

        /// <summary>
        /// Zero the bit in the given value.
        /// </summary>
        /// <param name="value">Value that contains the bit.</param>
        /// <param name="position">Position of the bit in the value.</param>
        /// <returns>New value with the bit set to zero.</returns>
        public static byte ZeroBitByte(byte value, byte position)
        {
            return (byte)(value & ~(1 << position));
        }

        /// <summary>
        /// Set the bit in the given value.
        /// </summary>
        /// <param name="value">Value that contains the bit.</param>
        /// <param name="position">Position of the bit in the value.</param>
        /// <returns>New value with the bit set to one.</returns>
        public static int SetBitInt(int value, int position)
        {
            return value |= 1 << position;
        }

        /// <summary>
        /// Set the bit in the given value.
        /// </summary>
        /// <param name="value">Value that contains the bit.</param>
        /// <param name="position">Position of the bit in the value.</param>
        /// <returns>New value with the bit set to one.</returns>
        public static short SetBitShort(short value, int position)
        {
            return value |= (short)(1 << position);
        }

        /// <summary>
        /// Set the bit in the given value.
        /// </summary>
        /// <param name="value">Value that contains the bit.</param>
        /// <param name="position">Position of the bit in the value.</param>
        /// <returns>New value with the bit set to one.</returns>
        public static ushort SetBitUShort(ushort value, int position)
        {
            return value |= (ushort)(1 << position);
        }

        /// <summary>
        /// Set the bit in the given value.
        /// </summary>
        /// <param name="value">Value that contains the bit.</param>
        /// <param name="position">Position of the bit in the value.</param>
        /// <returns>New value with the bit set to one.</returns>
        public static byte SetBitByte(byte value, int position)
        {
            return value |= (byte)(1 << position);
        }


        /// <summary>
        /// Check if the bit in the given position is set for the given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="position">Position within the value.</param>
        /// <returns>TRUE = Bit is 1 / False = Bit is 0</returns>
        public static bool IsBitSet(byte value, int position)
        {
            return (value & (1 << position)) != 0;
        }

        /// <summary>
        /// Check if the bit in the given position is set for the given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="position">Position within the value.</param>
        /// <returns>TRUE = Bit is 1 / False = Bit is 0</returns>
        public static bool IsBitSet(int value, int position)
        {
            return (value & (1 << position)) != 0;
        }

        #endregion

        #region Strings

        /// <summary>
        /// Convert the given string to a byte array.
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <returns>Byte array of the string.</returns>
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Convert the byte array to a string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <returns>String of the given byte array.</returns>
        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        #endregion
    }
}
