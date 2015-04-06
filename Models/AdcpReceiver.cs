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
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 04/23/2012      RC          0.01       Initial coding
 *       
 * 
 */


using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
namespace RTI
{

    /// <summary>
    /// ADCP Receiver data received when the ENGRR command is sent.
    /// 
    /// The data consists of 2 values to make up 1 sample.
    /// A real and imaginary value.  The values will come as
    /// pairs.
    /// 
    /// Each value will consist of 4 bytes.  The first 3 bytes 
    /// are the value and the last byte is the information about
    /// the value.
    /// 
    /// The information about the value will give: Gain, Channel Number, Sync, and board ID.
    /// 
    /// REAL
    /// Bit:   31 - 8        7         6 - 5           4 - 2               1         0
    /// Desc:    Value      Spare       Gain          Channel #           Sync      Type
    /// Size:   24 bits     1 bit      2 bits          3 bits             1 bit     1 bit
    ///                                11 = 0  dB      0 to 7                     1 = Real data
    ///                                10 = 18 dB      0 - 3 = XDCR 1
    ///                                01 = 36 dB      4 - 7 = XDCR 2
    ///                                00 = 54 dB
    ///                                
    /// Imaginary
    /// Bit:   31 - 8        7 - 3          2           1         0
    /// Desc:    Value      Board ID       Spare    Sync Trig    Type
    /// Size:   24 bits      5 bits        1 bit      1 bits     1 bit
    ///                                                          0 = Imaginary data
    /// 
    /// 
    /// </summary>
    public class AdcpReceiver
    {
        #region Data Classes

        /// <summary>
        /// Bad value.
        /// </summary>
        public const int BAD_VALUE = 16777216;

        #region Real Data

        /// <summary>
        /// Real numbers.
        /// </summary>
        public class RealData
        {
            #region Properties

            /// <summary>
            /// Real value.
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// Dynamic gain in dB.
            /// 11 = 0dB
            /// 10 = 18dB
            /// 01 = 36dB
            /// 00 = 54dB
            /// </summary>
            public int DynamicGain { get; set; }

            /// <summary>
            /// Channel Number.
            /// 0 to 7.
            /// 0-3 -  Are for first bank of receivers.  
            /// 4-7 -  Are for second bank of receivers if it is a dual frequency.
            /// </summary>
            public int ChannelNumber { get; set; }

            /// <summary>
            /// Sync Bit.
            /// </summary>
            public int SyncBit { get; set; }

            #endregion

            /// <summary>
            /// Initialize the values.
            /// </summary>
            public RealData()
            {
                // Initialize the value
                Value = 0;
                DynamicGain = 0;
                ChannelNumber = 0;
                SyncBit = 0;
            }

            /// <summary>
            /// Take a byte array to decode the data.
            /// </summary>
            /// <param name="data">Data to decode.</param>
            public RealData(byte[] data)
            {
                // Initialize the value
                Value = 0;
                DynamicGain = 0;
                ChannelNumber = 0;
                SyncBit = 0;

                // Decode the data
                Decode(data);
            }

            /// <summary>
            /// Decode the byte array data to
            /// all the values.
            /// </summary>
            /// <param name="data">Data to decode.</param>
            private void Decode(byte[] data)
            {
                int Val = 0;
                byte config_bits = 0;

                if (data.Length >= 4)
                {
                    // Data is four bytes long and sent in the following order: LSB2 MSB2 LSB1 MSB1
                    Val = (int)data[0] << 8;
                    Val += (int)data[1] << 16;
                    Val += (int)data[2];
                    config_bits = data[3];
                }

                // Verify the value
                if (Val > ((1 << 23) - 1))
                {
                    Val -= BAD_VALUE;
                }

                // Set the value
                Value = Val;

                // Decode config data
                DecodeConfigBits(config_bits);
            }

            /// <summary>
            /// Decode the configuration bits.
            /// This will get the dynamic gain, channel number
            /// and sync bit.
            /// </summary>
            /// <param name="configBits">Config Bits.</param>
            private void DecodeConfigBits(byte configBits)
            {
                DynamicGain = (configBits >> 5) & 0x0003;       // 2 Bits
                ChannelNumber = (configBits >> 2) & 0x007;      // 3 Bits
                SyncBit = (configBits >> 1) & 0x0001;           // 1 Bit
            }

            /// <summary>
            /// Output the value information as a string.
            /// </summary>
            /// <returns>String of the value.</returns>
            public override string ToString()
            {
                return string.Format("Real: {0}  DynGain: {1}  ChannelNum: {2}  SyncBit: {3}", Value, DynamicGain, ChannelNumber, SyncBit);
            }
        }

        #endregion

        #region Imaginary Data

        /// <summary>
        /// Imaginary numbers.
        /// </summary>
        public class ImaginaryData
        {
            #region Properties

            /// <summary>
            /// Real value.
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// Board ID.
            /// Lower 5 bits of I2C device address.
            /// </summary>
            public int BoardId { get; set; }

            /// <summary>
            /// Sync tigger input (rx_trig).
            /// </summary>
            public int SyncTriggerBit { get; set; }

            #endregion

            /// <summary>
            /// Initialize the values.
            /// </summary>
            public ImaginaryData()
            {
                // Initialize the value
                Value = 0;
                BoardId = 0;
                SyncTriggerBit = 0;
            }

            /// <summary>
            /// Take a byte array to decode the data.
            /// </summary>
            /// <param name="data">Data to decode.</param>
            public ImaginaryData(byte[] data)
            {
                // Initialize the value
                Value = 0;
                BoardId = 0;
                SyncTriggerBit = 0;

                // Decode the data
                Decode(data);
            }

            /// <summary>
            /// Decode the byte array data to
            /// all the values.
            /// </summary>
            /// <param name="data">Data to decode.</param>
            private void Decode(byte[] data)
            {
                int Val = 0;
                byte config_bits = 0;

                if (data.Length >= 4)
                {
                    // Data is four bytes long and sent in the following order: LSB2 MSB2 LSB1 MSB1
                    Val = (int)data[0] << 8;
                    Val += (int)data[1] << 16;
                    Val += (int)data[2];
                    config_bits = data[3];
                }

                // Verify the value
                if (Val > ((1 << 23) - 1))
                {
                    Val -= BAD_VALUE;
                }

                // Set the value
                Value = Val;

                // Decode config data
                DecodeConfigBits(config_bits);
            }

            /// <summary>
            /// Decode the configuration bits.
            /// This will get the dynamic gain, channel number
            /// and sync bit.
            /// </summary>
            /// <param name="configBits">Config Bits.</param>
            private void DecodeConfigBits(byte configBits)
            {
                BoardId = (configBits >> 3) & 0x001F;          // 5 Bits
                SyncTriggerBit = (configBits >> 1) & 0x0001;   // 1 Bit
            }

            /// <summary>
            /// Output the value information as a string.
            /// </summary>
            /// <returns>String of the value.</returns>
            public override string ToString()
            {
                return string.Format("Imaginary: {0}   BrdId: {1}  SyncTrigBit: {2}", Value, BoardId, SyncTriggerBit);
            }
        }

        #endregion

        #region Sample

        /// <summary>
        /// Sample containing a Real and Imaginary data.
        /// </summary>
        public struct Sample
        {
            /// <summary>
            /// Real Data sample.
            /// </summary>
            public RealData RealDataSample { get; set; }

            /// <summary>
            /// Imaginary Data sample.
            /// </summary>
            public ImaginaryData ImaginaryDataSample { get; set; }
        }

        #endregion

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public AdcpReceiver()
        {

        }

        /// <summary>
        /// Decode a file of all the samples.  A sample is made up of a
        /// group of Real and Imaginary data samples.  This will create a list
        /// of all samples found in the file.
        /// </summary>
        /// <param name="file">File to decode.</param>
        /// <returns>List of all the samples found in the list.</returns>
        public List<Sample> Decode(string file)
        {
            List<Sample> samples = new List<Sample>();
            List<byte> buffer = new List<byte>();

            // Placeholder for real data while waiting for imaginary data
            RealData realData = null;

            // Open file to read
            using (FileStream strm = File.OpenRead(file))
            {
                for (int x = 0; x < strm.Length; x++)
                {
                    // Read in a byte
                    int strmReadByte = strm.ReadByte();

                    // Verify the byte is good
                    if (strmReadByte >= 0)
                    {
                        // Add it to the list
                        buffer.Add((byte)strmReadByte);

                        // If the list has 4 bytes
                        // verify we have a good sample
                        // if we do not have a good sample,
                        // drop the first byte.  If we good have a 
                        // good sample, remove the sample from the list
                        // and decode the sample
                        if (buffer.Count == 4)
                        {
                            // Create an array for the data
                            byte[] data = buffer.ToArray();

                            if (ValidRealData(buffer))
                            {
                                realData = new RealData(data);
                                buffer.Clear();
                            }
                            else if (ValidImaginaryData(buffer))
                            {
                                ImaginaryData imgData = new ImaginaryData(data);
                                
                                // If a Real data has already been set
                                // Create a sample and add it to the list
                                //if (realData != null && imgData.BoardId <= 1)
                                if (realData != null)
                                {
                                    samples.Add(new Sample() { RealDataSample = realData, ImaginaryDataSample = imgData });
                                }

                                // Reset values
                                buffer.Clear();
                                realData = null;
                            }
                            else
                            {
                                buffer.RemoveAt(0);
                                realData = null;
                            }
                        }
                    }
                }
            }

            return samples;
        }

        /// <summary>
        /// Check if the least significant bit starts
        /// with a 1.  This designates the start of a sample.
        /// Raw data sample.
        /// </summary>
        /// <param name="data">Data to check.</param>
        /// <returns>True = Valid sample value. / False = Invalid Sample value.</returns>
        private bool ValidRealData(List<byte> data)
        {
            if(data.Count >= 4)
            {
                return (0x01 & ((int)data[3])) == 1;
            }

            return false;
        }

        /// <summary>
        /// Check if the least significant bit starts
        /// with a 0.  This designates the
        /// continuation of a sample.
        /// Imaginary Data Sample.
        /// </summary>
        /// <param name="data">Data to check.</param>
        /// <returns>True = Valid sample value. / False = Invalid Sample value.</returns>
        private bool ValidImaginaryData(List<byte> data)
        {
            if (data.Count >= 4)
            {
                return (0x01 & ((int)data[3])) == 0;
            }

            return false;
        }

    }
}
