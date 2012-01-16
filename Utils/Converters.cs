/*
 * Copyright © 2011 
 * Rowe Technology Inc.
 * All rights reserved.
 * http://www.rowetechinc.com
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification is NOT permitted.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Vertion    Comments
 * -----------------------------------------------------------------
 * 12/09/2011      RC          1.09       Initial coding
 * 
 * 
 */

using System.Runtime.InteropServices;
using System;
namespace RTI
{
    /// <summary>
    /// Class of different converters.
    /// </summary>
    public class Converters
    {

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
            T[] result = new T[length];
            System.Buffer.BlockCopy(data, index, result, 0, length);
            return result;
        }

        #region Byte Array Conversions
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
            [FieldOffset(0)]
            public float Float;
            [FieldOffset(0)]
            public int Int;
        }

        /// <summary>
        /// Convert the given byte array to an Integer
        /// </summary>
        /// <param name="packet">Ensemble containing data.</param>
        /// <param name="index">Location of first byte in Ensemble to convert.</param>
        /// <returns>Integer value from given byte array at xAxis.</returns>
        public static int ByteArrayToInt(byte[] packet, int index)
        {
            TestUnion result = new TestUnion();
            result.Byte1 = packet[index++];
            result.Byte2 = packet[index++];
            result.Byte3 = packet[index++];
            result.Byte4 = packet[index];

            return result.Int;
        }

        /// <summary>
        /// Convert the given Int to a byte array.
        /// The return value is a array containing the bytes.
        /// </summary>
        /// <param name="value">Int value to convert.</param>
        /// <returns>Byte array which contains the bytes of the conversion.</returns>
        public static byte[] IntToByteArray(int value)
        {
            TestUnion result = new TestUnion();
            result.Int = value;

            byte[] resultArray = new byte[4];
            resultArray[0] = result.Byte1;
            resultArray[1] = result.Byte2;
            resultArray[2] = result.Byte3;
            resultArray[3] = result.Byte4;

            return resultArray;
        }

        /// <summary>
        /// Convert the given byte array to a string.
        /// </summary>
        /// <param name="packet">Ensemble containing data.</param>
        /// <param name="index">Location of first byte in Ensemble to convert.</param>
        /// <param name="len">Length of the string in bytes.</param>
        /// <returns>String value for the given byte array.</returns>
        public static string ByteArrayToString(byte[] packet, int len, int index)
        {
            string s = "";
            int i;
            for (i = 0; i < len; i++)
            {
                s += (char)packet[index++];
            }
            return s;
        }

        /// <summary>
        /// Convert the given byte array to a Float.
        /// </summary>
        /// <param name="packet">Ensemble containing data.</param>
        /// <param name="index">Location first byte in Ensemble to convert.</param>
        /// <returns>Float value for given byte array</returns>
        public static float ByteArrayToFloat(byte[] packet, int index)
        {
            TestUnion result = new TestUnion();
            result.Byte1 = packet[index++];
            result.Byte2 = packet[index++];
            result.Byte3 = packet[index++];
            result.Byte4 = packet[index];

            return result.Float;
        }

        /// <summary>
        /// Convert the given float to a byte array.
        /// The return value is a array containing the bytes.
        /// </summary>
        /// <param name="value">Float value to convert.</param>
        /// <returns>Byte array which contains the bytes of the conversion.</returns>
        public static byte[] FloatToByteArray(float value)
        {
            TestUnion result = new TestUnion();
            result.Float = value;

            byte[] resultArray = new byte[4];
            resultArray[0] = result.Byte1;
            resultArray[1] = result.Byte2;
            resultArray[2] = result.Byte3;
            resultArray[3] = result.Byte4;

            return resultArray;
        }

        #endregion
    }

}
