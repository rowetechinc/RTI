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
 * 03/13/2014      RC          2.21.4     Initial coding
 * 
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Test the PD0 Header.
    /// </summary>
    [TestFixture]
    public class Pd0HeaderTest
    {

        /// <summary>
        /// Test Adding A Data Type.
        /// Fixed size data types.
        /// </summary>
        [Test]
        public void TestAddDataTypeFixed()
        {
            Pd0Header header = new Pd0Header();
            Pd0VariableLeader vl = new Pd0VariableLeader();
            Pd0FixedLeader fl = new Pd0FixedLeader();
            fl.NumberOfCells = 5;
            header.AddDataType(vl);
            header.AddDataType(fl);

            byte[] data = header.Encode();

            // Size
            int size = Pd0VariableLeader.DATATYPE_SIZE;
            size += Pd0FixedLeader.DATATYPE_SIZE;
            size += 2;      // Spare
            //size += 2;      // Checksum
            size += header.GetDataTypeSize();


            Assert.AreEqual(2, header.NumberOfDataTypes(), "Number of Data Types is incorrect.");
            Assert.AreEqual(5, header.NumberOfDepthCells, "Number of depth cells is incorrect.");
            Assert.AreEqual(size, MathHelper.LsbMsbShort(data[2], data[3]), "Number of Bytes is incorrect.");
            Assert.AreEqual(2, data[5], "Number of Data Types is incorrect.");
            Assert.AreEqual(6 + (2 * 2), data.Length, "Array Size is incorrect.");

            int vlOffset = header.GetDataTypeSize();
            Assert.AreEqual(vlOffset, MathHelper.LsbMsbShort(data[6], data[7]), "Variable Leader offset is incorrect.");

            int flOffset = header.GetDataTypeSize() + Pd0VariableLeader.DATATYPE_SIZE;
            Assert.AreEqual(flOffset, MathHelper.LsbMsbShort(data[8], data[9]), "Fixed Leader offset is incorrect.");
        }

        /// <summary>
        /// Test Adding A Data Type.
        /// Velocity data type added.
        /// </summary>
        [Test]
        public void TestAddDataTypeVel()
        {
            Pd0Header header = new Pd0Header();
            Pd0VariableLeader vl = new Pd0VariableLeader();
            Pd0FixedLeader fl = new Pd0FixedLeader();

            #region Velocity

            Pd0Velocity vel = new Pd0Velocity();
            // 2 Byte Header
            // 8 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] data = new byte[42];

            data[0] = Pd0Velocity.ID_LSB;
            data[1] = Pd0Velocity.ID_MSB;
            data[2] = 0xE8;                                   // DS0 Beam 0 LSB
            data[3] = 0x00;                                   // DS0 Beam 0 MSB
            data[4] = 0xBD;                                   // DS0 Beam 1 LSB
            data[5] = 0xFE;                                   // DS0 Beam 1 MSB
            data[6] = 0xC8;                                   // DS0 Beam 2 LSB
            data[7] = 0x01;                                   // DS0 Beam 2 MSB
            data[8] = 0x72;                                   // DS0 Beam 3 LSB
            data[9] = 0xFD;                                   // DS0 Beam 3 MSB

            data[(4 * 8) + 2 + 0] = 0xD0;                     // DS4 Beam 0 LSB
            data[(4 * 8) + 2 + 1] = 0x04;                     // DS4 Beam 0 MSB
            data[(4 * 8) + 2 + 2] = 0xD5;                     // DS4 Beam 1 LSB
            data[(4 * 8) + 2 + 3] = 0xFA;                     // DS4 Beam 1 MSB
            data[(4 * 8) + 2 + 4] = 0xB0;                     // DS4 Beam 2 LSB
            data[(4 * 8) + 2 + 5] = 0x05;                     // DS4 Beam 2 MSB
            data[(4 * 8) + 2 + 6] = 0x8A;                     // DS4 Beam 3 LSB
            data[(4 * 8) + 2 + 7] = 0xF9;                     // DS4 Beam 3 MSB

            vel.Decode(data);

            #endregion

            fl.NumberOfCells = 5;
            header.AddDataType(vl);
            header.AddDataType(fl);
            header.AddDataType(vel);

            byte[] hdrData = header.Encode();

            // Size
            int size = Pd0VariableLeader.DATATYPE_SIZE;         // Variable Leader size
            size += Pd0FixedLeader.DATATYPE_SIZE;               // Fixed Leader size
            size += 2;                                          // Spare size
            size += header.GetDataTypeSize();                   // Header size
            size += vel.GetDataTypeSize();                      // Velocity size


            Assert.AreEqual(3, header.NumberOfDataTypes(), "Number of Data Types is incorrect.");
            Assert.AreEqual(5, header.NumberOfDepthCells, "Number of depth cells is incorrect.");
            Assert.AreEqual(size, MathHelper.LsbMsbShort(hdrData[2], hdrData[3]), "Number of Bytes is incorrect.");
            Assert.AreEqual(3, hdrData[5], "Number of Data Types is incorrect.");
            Assert.AreEqual(6 + (3 * 2), hdrData.Length, "Array Size is incorrect.");

            int vlOffset = header.GetDataTypeSize();
            Assert.AreEqual(vlOffset, MathHelper.LsbMsbShort(hdrData[6], hdrData[7]), "Variable Leader offset is incorrect.");

            int flOffset = header.GetDataTypeSize() + Pd0VariableLeader.DATATYPE_SIZE;
            Assert.AreEqual(flOffset, MathHelper.LsbMsbShort(hdrData[8], hdrData[9]), "Fixed Leader offset is incorrect.");

            int velOffset = header.GetDataTypeSize() + Pd0VariableLeader.DATATYPE_SIZE + Pd0FixedLeader.DATATYPE_SIZE;
            Assert.AreEqual(velOffset, MathHelper.LsbMsbShort(hdrData[10], hdrData[11]), "Velocity offset is incorrect.");
        }

        /// <summary>
        /// Test Adding A Data Type.
        /// Correlation data type added.
        /// </summary>
        [Test]
        public void TestAddDataTypeCorr()
        {
            Pd0Header header = new Pd0Header();
            Pd0VariableLeader vl = new Pd0VariableLeader();
            Pd0FixedLeader fl = new Pd0FixedLeader();

            #region Velocity

            Pd0Velocity vel = new Pd0Velocity();
            // 2 Byte Header
            // 8 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] data = new byte[42];

            data[0] = Pd0Velocity.ID_LSB;
            data[1] = Pd0Velocity.ID_MSB;
            data[2] = 0xE8;                                   // DS0 Beam 0 LSB
            data[3] = 0x00;                                   // DS0 Beam 0 MSB
            data[4] = 0xBD;                                   // DS0 Beam 1 LSB
            data[5] = 0xFE;                                   // DS0 Beam 1 MSB
            data[6] = 0xC8;                                   // DS0 Beam 2 LSB
            data[7] = 0x01;                                   // DS0 Beam 2 MSB
            data[8] = 0x72;                                   // DS0 Beam 3 LSB
            data[9] = 0xFD;                                   // DS0 Beam 3 MSB

            data[(4 * 8) + 2 + 0] = 0xD0;                     // DS4 Beam 0 LSB
            data[(4 * 8) + 2 + 1] = 0x04;                     // DS4 Beam 0 MSB
            data[(4 * 8) + 2 + 2] = 0xD5;                     // DS4 Beam 1 LSB
            data[(4 * 8) + 2 + 3] = 0xFA;                     // DS4 Beam 1 MSB
            data[(4 * 8) + 2 + 4] = 0xB0;                     // DS4 Beam 2 LSB
            data[(4 * 8) + 2 + 5] = 0x05;                     // DS4 Beam 2 MSB
            data[(4 * 8) + 2 + 6] = 0x8A;                     // DS4 Beam 3 LSB
            data[(4 * 8) + 2 + 7] = 0xF9;                     // DS4 Beam 3 MSB

            vel.Decode(data);

            #endregion

            #region Correlation

            Pd0Correlation corr = new Pd0Correlation();

            // 2 Byte Header
            // 4 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] corrData = new byte[22];

            corrData[0] = Pd0Velocity.ID_LSB;
            corrData[1] = Pd0Velocity.ID_MSB;
            corrData[2] = 0xE8;                                   // DS0 Beam 0 
            corrData[3] = 0x7B;                                   // DS0 Beam 1 
            corrData[4] = 0x7A;                                   // DS0 Beam 2 
            corrData[5] = 0xDF;                                   // DS0 Beam 3

            corrData[(4 * 4) + 2 + 0] = 0x6F;                     // DS4 Beam 0 
            corrData[(4 * 4) + 2 + 1] = 0xDE;                     // DS4 Beam 1 
            corrData[(4 * 4) + 2 + 2] = 0x15;                     // DS4 Beam 2 
            corrData[(4 * 4) + 2 + 3] = 0x22;                     // DS4 Beam 3 


            corr.Decode(corrData);

            #endregion

            fl.NumberOfCells = 5;
            header.AddDataType(vl);
            header.AddDataType(fl);
            header.AddDataType(vel);
            header.AddDataType(corr);

            byte[] hdrData = header.Encode();

            // Size
            int size = Pd0VariableLeader.DATATYPE_SIZE;         // Variable Leader size
            size += Pd0FixedLeader.DATATYPE_SIZE;               // Fixed Leader size
            size += 2;                                          // Spare size
            size += header.GetDataTypeSize();                   // Header size
            size += vel.GetDataTypeSize();                      // Velocity size
            size += corr.GetDataTypeSize();                     // Correlation size


            Assert.AreEqual(4, header.NumberOfDataTypes(), "Number of Data Types is incorrect.");
            Assert.AreEqual(5, header.NumberOfDepthCells, "Number of depth cells is incorrect.");
            Assert.AreEqual(size, MathHelper.LsbMsbShort(hdrData[2], hdrData[3]), "Number of Bytes is incorrect.");
            Assert.AreEqual(4, hdrData[5], "Number of Data Types is incorrect.");
            Assert.AreEqual(6 + (4 * 2), hdrData.Length, "Array Size is incorrect.");

            int vlOffset = header.GetDataTypeSize();
            Assert.AreEqual(vlOffset, MathHelper.LsbMsbShort(hdrData[6], hdrData[7]), "Variable Leader offset is incorrect.");

            int flOffset = header.GetDataTypeSize() + Pd0VariableLeader.DATATYPE_SIZE;
            Assert.AreEqual(flOffset, MathHelper.LsbMsbShort(hdrData[8], hdrData[9]), "Fixed Leader offset is incorrect.");

            int velOffset = header.GetDataTypeSize() + Pd0VariableLeader.DATATYPE_SIZE + Pd0FixedLeader.DATATYPE_SIZE;
            Assert.AreEqual(velOffset, MathHelper.LsbMsbShort(hdrData[10], hdrData[11]), "Velocity offset is incorrect.");

            int corrOffset = header.GetDataTypeSize() + Pd0VariableLeader.DATATYPE_SIZE + Pd0FixedLeader.DATATYPE_SIZE + vel.GetDataTypeSize() ;
            Assert.AreEqual(corrOffset, MathHelper.LsbMsbShort(hdrData[12], hdrData[13]), "Correlation offset is incorrect.");
        }

    }
}
