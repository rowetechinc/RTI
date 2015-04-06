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
 * 02/24/2014      RC          2.21.4     Initial coding
 * 
 * 
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Diagnostics;

    /// <summary>
    /// The header for the PD0 Data Type.
    /// </summary>
    public class Pd0Header : Pd0DataType
    {
        #region Variable

        /// <summary>
        /// LSB for the ID for the PD0 data type.
        /// </summary>
        public const byte ID_LSB = 0x7F;

        /// <summary>
        /// MSB for the ID for the PD0 data type.
        /// </summary>
        public const byte ID_MSB = 0x7F;

        /// <summary>
        /// Minimum number of bytes for the header.
        /// These bytes will hold the number of bytes and
        /// the number of data types and the ID.
        /// </summary>
        public const int HEADER_MIN_BYTE = 6;

        /// <summary>
        /// Number of bytes for a short value.
        /// Signed Short   (short)  -32768   to    32767
        /// Unsigned short (ushort) 0        to    65535
        /// </summary>
        public const int SHORT_NUM_BYTE = 2;

        #endregion

        #region Properties

        /// <summary>
        /// Number of bytes in the ensemble.
        /// This field contains the number of bytes from the start of the
        /// current ensemble up to, but not including, the 2-byte checksum
        /// (Figure 17, page 149).
        /// </summary>
        public int NumberOfBytes { get; set; }

        /// <summary>
        /// Number of depth cells in the ensemble.
        /// </summary>
        public int NumberOfDepthCells
        {
            get
            {
                return GetFixedLeader().NumberOfCells;
            }
        }

        /// <summary>
        /// List of all the data types in the ensemble.
        /// 
        /// This field contains the number of data types selected for collection.
        /// By default, fixed/variable leader, velocity, correlation
        /// magnitude, echo intensity, and percent good are selected for
        /// collection. This field will therefore have a value of six (4 data
        /// types + 2 for the Fixed/Variable Leader data).
        /// 
        /// This field contains the internal memory address offset where
        /// the Workhorse will store information for data type #1 (with this
        /// firmware, always the Fixed Leader). Adding “1” to this offset
        /// number gives the absolute Binary Byte number in the ensemble
        /// where Data Type #1 begins (the first byte of the ensemble is
        /// Binary Byte #1).
        /// 
        /// </summary>
        public Dictionary<Pd0ID.Pd0Types, Pd0DataType> DataTypes { get; set; }

        #endregion

        /// <summary>
        /// Initialize the data type.
        /// </summary>
        public Pd0Header()
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.Header)
        {
            // Initialize values
            NumberOfBytes = 6 + 2;                                          // 6 bytes for header fixed values and 2 bytes for ensemble spare
            DataTypes = new Dictionary<Pd0ID.Pd0Types, Pd0DataType>();
        }

        #region Encode

        /// <summary>
        /// Convert the object to a byte array to have
        /// binary data.
        /// </summary>
        /// <returns>Binary Array for binary format.</returns>
        public override byte[] Encode()
        {
            // Number of bytes
            byte numByteLsb;
            byte numByteMsb;
            MathHelper.LsbMsb(NumberOfBytes, out numByteLsb, out numByteMsb);

            // Create the array
            byte[] data = new byte[GetDataTypeSize()];
            data[0] = ID_LSB;                       // Header ID LSB
            data[1] = ID_MSB;                       // Header ID MSB
            data[2] = numByteLsb;                   // Number of Bytes LSB
            data[3] = numByteMsb;                   // Number of Bytes MSB
            data[4] = 0x0f;                         // Spare
            data[5] = (byte)DataTypes.Count;        // Number of Data Types
            
            // Start location is based off the number of data types
            // Each data type will consume 2 bytes for the offset
            // The header requires 6 bytes
            int start = (DataTypes.Count * SHORT_NUM_BYTE) + HEADER_MIN_BYTE;
            int x = HEADER_MIN_BYTE;

            // Add all the offset for each data type
            foreach(var dt in DataTypes)
            {
                // Get the next offset location
                // and convert to 2 bytes
                // Then add it to the array
                byte lsb;
                byte msb;
                MathHelper.LsbMsb(start, out lsb, out msb);
                data[x++] = lsb;
                data[x++] = msb;

                // Set the offset value
                dt.Value.Offset = (ushort)start;

                // Add in the size plus the last location
                start += GetDataTypeSize(dt.Value);
            }

            return data;
        }

        #endregion

        #region Decode

        /// <summary>
        /// Decode the given data into an object.
        /// </summary>
        /// <param name="data">Data to decode.</param>
        public override void Decode(byte[] data)
        {
            if (data.Length > HEADER_MIN_BYTE)
            {
                // Ensure the correct data type is given
                if (data[0] == ID_LSB && data[1] == ID_MSB)
                {
                    // Number of bytes
                    NumberOfBytes = MathHelper.LsbMsbInt(data[2], data[3]);

                    //DataTypes.Clear();                                  // Clear anything currently stored
                    int numDT = data[5];                                // Number of Data types
                    int dtOffset = HEADER_MIN_BYTE;                  // Start of the offsets

                    // Determine the size of each data type.
                    // This will start at the end of the header
                    //int prevOffset = dtOffset + (numDT * SHORT_NUM_BYTE);            

                    // Collect each offset
                    for (int x = 0; x < numDT; x++)
                    {
                        // Get the offset and add it to the list
                        byte lsb = data[dtOffset];
                        byte msb = data[dtOffset + 1];
                        ushort offset = MathHelper.LsbMsbUShort(lsb, msb);

                        if (data.Length > offset)
                        {
                            // Determine the data type
                            byte lsbID = data[offset];
                            byte msbID = data[offset + 1];
                            Pd0ID id = Pd0ID.GetType(lsbID, msbID);

                            // Create and add the data type to the ensemble
                            switch(id.Type)
                            {
                                case Pd0ID.Pd0Types.FixedLeader:
                                    // Copy the buffer
                                    byte[] flBuffer = new byte[Pd0FixedLeader.DATATYPE_SIZE];
                                    Buffer.BlockCopy(data, offset, flBuffer, 0, flBuffer.Length);

                                    if (DataTypes.ContainsKey(Pd0ID.Pd0Types.FixedLeader))
                                    {
                                        GetFixedLeader().Decode(flBuffer);
                                    }
                                    else
                                    {
                                        AddDataType(new Pd0FixedLeader(flBuffer, offset));
                                    }

                                    break;
                                case Pd0ID.Pd0Types.VariableLeader:
                                    // Copy the buffer
                                    byte[] vlBuffer = new byte[Pd0VariableLeader.DATATYPE_SIZE];
                                    Buffer.BlockCopy(data, offset, vlBuffer, 0, vlBuffer.Length);

                                    if (DataTypes.ContainsKey(Pd0ID.Pd0Types.VariableLeader))
                                    {
                                        GetVariableLeader().Decode(vlBuffer);   
                                    }
                                    else
                                    {
                                        AddDataType(new Pd0VariableLeader(vlBuffer, offset));
                                    }

                                    break;
                                case Pd0ID.Pd0Types.BottomTrack:
                                    // Copy the buffer
                                    byte[] btBuffer = new byte[Pd0BottomTrack.DATATYPE_SIZE];
                                    Buffer.BlockCopy(data, offset, btBuffer, 0, btBuffer.Length);

                                    // Decode the data
                                    if (DataTypes.ContainsKey(Pd0ID.Pd0Types.BottomTrack))
                                    {
                                        GetBottomTrack().Decode(btBuffer);
                                    }
                                    else
                                    {
                                        AddDataType(new Pd0BottomTrack(btBuffer, offset));
                                    }
                                    break;
                                case Pd0ID.Pd0Types.Velocity:
                                    // Copy the buffer
                                    byte[] velBuffer = new byte[Pd0Velocity.GetVelocitySize(GetFixedLeader().NumberOfCells)];
                                    Buffer.BlockCopy(data, offset, velBuffer, 0, velBuffer.Length);

                                    // Decode the data
                                    if (DataTypes.ContainsKey(Pd0ID.Pd0Types.Velocity))
                                    {
                                        GetVelocity().Decode(velBuffer);
                                    }
                                    else
                                    {
                                        AddDataType(new Pd0Velocity(velBuffer, offset));
                                    }
                                    break;
                                case Pd0ID.Pd0Types.Correlation:
                                    // Copy the buffer
                                    byte[] corrBuffer = new byte[Pd0Correlation.GetCorrelationSize(GetFixedLeader().NumberOfCells)];
                                    Buffer.BlockCopy(data, offset, corrBuffer, 0, corrBuffer.Length);

                                    // Decode the data
                                    if (DataTypes.ContainsKey(Pd0ID.Pd0Types.Correlation))
                                    {
                                        GetCorrelation().Decode(corrBuffer);
                                    }
                                    else
                                    {
                                        AddDataType(new Pd0Correlation(corrBuffer, offset));
                                    }
                                    break;
                                case Pd0ID.Pd0Types.EchoIntensity:
                                    // Copy the buffer
                                    byte[] eiBuffer = new byte[Pd0EchoIntensity.GetEchoIntensitySize(GetFixedLeader().NumberOfCells)];
                                    Buffer.BlockCopy(data, offset, eiBuffer, 0, eiBuffer.Length);

                                    // Decode the data
                                    if (DataTypes.ContainsKey(Pd0ID.Pd0Types.EchoIntensity))
                                    {
                                        GetEchoIntensity().Decode(eiBuffer);
                                    }
                                    else
                                    {
                                        AddDataType(new Pd0EchoIntensity(eiBuffer, offset));
                                    }
                                    break;
                                case Pd0ID.Pd0Types.PercentGood:
                                    // Copy the buffer
                                    byte[] pgBuffer = new byte[Pd0PercentGood.GetPercentGoodSize(GetFixedLeader().NumberOfCells)];
                                    Buffer.BlockCopy(data, offset, pgBuffer, 0, pgBuffer.Length);

                                    // Decode the data
                                    if (DataTypes.ContainsKey(Pd0ID.Pd0Types.PercentGood))
                                    {
                                        GetPercentGood().Decode(pgBuffer);
                                    }
                                    else
                                    {
                                        AddDataType(new Pd0PercentGood(pgBuffer, offset));
                                    }
                                    break;
                                default:
                                    Debug.WriteLine(string.Format("Unknown PD0 ID {0}", id.Type));
                                    break;
                            }
                        }

                        // Move the offset to the next value
                        dtOffset += 2;
                    }
                }

            }
        }

        #endregion

        #region Add DataType

        /// <summary>
        /// Add the data type to the ensemble.
        /// </summary>
        /// <param name="dataType">Data type.</param>
        public void AddDataType(Pd0DataType dataType)
        {
            // Increase the number of bytes
            // Add 2 for the offset added to the header
            NumberOfBytes += GetDataTypeSize(dataType) + 2;

            // Add the offset to the list
            DataTypes.Add(dataType.ID.Type, dataType);
        }

        /// <summary>
        /// Get the data type size.  Some of the data types have a fixed size.
        /// The others depend on the number of depth cells.
        /// </summary>
        /// <param name="dataType">Data type.</param>
        /// <returns>Number of bytes for each data types.</returns>
        public int GetDataTypeSize(Pd0DataType dataType)
        {
            switch(dataType.ID.Type)
            {
                case Pd0ID.Pd0Types.Header:
                    return GetDataTypeSize();                       // Sized based of number of type types
                case Pd0ID.Pd0Types.FixedLeader:
                    return Pd0FixedLeader.DATATYPE_SIZE;            // Fixed size
                case Pd0ID.Pd0Types.VariableLeader:      
                    return Pd0VariableLeader.DATATYPE_SIZE;         // Fixed size
                case Pd0ID.Pd0Types.BottomTrack:
                    return Pd0BottomTrack.DATATYPE_SIZE;            // Fixed size
                case Pd0ID.Pd0Types.Velocity:
                    return GetVelocitySize();                       // 2 Bytes for every beam, 4 beams for every depth cell
                case Pd0ID.Pd0Types.Correlation:
                case Pd0ID.Pd0Types.EchoIntensity:
                case Pd0ID.Pd0Types.PercentGood:
                    return GetCorrEchoPerGdSize();                  // 1 Byte for every beam, 4 beams for every depth cell
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Get the data type size for the Header data type.
        /// </summary>
        /// <returns>Number of bytes in the header.</returns>
        public override int GetDataTypeSize()
        {
            // Fixed size of 6 bytes for the ID and other bytes
            int size = HEADER_MIN_BYTE;

            // Count all the data types
            int count = DataTypes.Count;

            // Each data type requires 2 bytes
            return size + (count * SHORT_NUM_BYTE);
        }

        /// <summary>
        /// Determine the size header based off the
        /// number of data types.
        /// </summary>
        /// <param name="numDataTypes">Number of data types.</param>
        /// <returns>Number of bytes in the header.</returns>
        public static int GetHeaderSize(int numDataTypes)
        {
            // Fixed size of 6 bytes for the ID and other bytes
            int size = HEADER_MIN_BYTE;

            // Each data type requires 2 bytes
            return size + (numDataTypes * SHORT_NUM_BYTE);
        }

        /// <summary>
        /// Number of data types.
        /// </summary>
        /// <returns>Number of Data Types.</returns>
        public int NumberOfDataTypes()
        {
            return DataTypes.Count;
        }

        /// <summary>
        /// Get the data type size for the Velcoity data type.
        /// 
        /// 2 = ID
        /// Number of DepthCells * 4 = 4 Beams for each depth cell
        /// 2 * = 2 Bytes per beam
        /// </summary>
        /// <returns>Number of bytes for the data type.</returns>
        private int GetVelocitySize()
        {
            return 2 + (SHORT_NUM_BYTE * (NumberOfDepthCells * 4));
        }

        /// <summary>
        /// Get the data type size for the Correlation Maginitude,
        /// or Echo Intensity or Percent Good data type.
        /// 
        /// 2 = ID
        /// Number of DepthCells * 4 = 4 Beams for each depth cell
        /// </summary>
        /// <returns>Number of bytes for the data type.</returns>
        private int GetCorrEchoPerGdSize()
        {
            return 2 + (NumberOfDepthCells * 4);
        }

        #endregion

        #region Get Data Type

        /// <summary>
        /// Get the Fixed Leader from the header.
        /// </summary>
        /// <returns>Fixed Leader.</returns>
        public Pd0FixedLeader GetFixedLeader()
        {
            return (Pd0FixedLeader)DataTypes[Pd0ID.Pd0Types.FixedLeader];
        }

        /// <summary>
        /// Get the Variable Leader from the header.
        /// </summary>
        /// <returns>Variable Leader.</returns>
        public Pd0VariableLeader GetVariableLeader()
        {
            return (Pd0VariableLeader)DataTypes[Pd0ID.Pd0Types.VariableLeader];
        }

        /// <summary>
        /// Get the Velocity from the header.
        /// </summary>
        /// <returns>Velocity.</returns>
        public Pd0Velocity GetVelocity()
        {
            return (Pd0Velocity)DataTypes[Pd0ID.Pd0Types.Velocity];
        }

        /// <summary>
        /// Get the Percent Good from the header.
        /// </summary>
        /// <returns>Percent Good.</returns>
        public Pd0PercentGood GetPercentGood()
        {
            return (Pd0PercentGood)DataTypes[Pd0ID.Pd0Types.PercentGood];
        }

        /// <summary>
        /// Get the Correlation from the header.
        /// </summary>
        /// <returns>Correlation.</returns>
        public Pd0Correlation GetCorrelation()
        {
            return (Pd0Correlation)DataTypes[Pd0ID.Pd0Types.Correlation];
        }

        /// <summary>
        /// Get the Echo Intensity from the header.
        /// </summary>
        /// <returns>Echo Intensity.</returns>
        public Pd0EchoIntensity GetEchoIntensity()
        {
            return (Pd0EchoIntensity)DataTypes[Pd0ID.Pd0Types.EchoIntensity];
        }

        /// <summary>
        /// Get the BottomTrack from the header.
        /// </summary>
        /// <returns>BottomTrack.</returns>
        public Pd0BottomTrack GetBottomTrack()
        {
            return (Pd0BottomTrack)DataTypes[Pd0ID.Pd0Types.BottomTrack];
        }

        #endregion
    }
}
