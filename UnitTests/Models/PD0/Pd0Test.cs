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
    public class Pd0Test
    {

        #region Decode


        /// <summary>
        /// Test decoding.
        /// </summary>
        [Test]
        public void TestDecode()
        {
            PD0 pd0 = new PD0();
            pd0.FixedLeader.NumberOfCells = 5;

            byte[] data = pd0.Encode();

            // Size
            int size = Pd0VariableLeader.DATATYPE_SIZE;
            size += Pd0FixedLeader.DATATYPE_SIZE;
            size += 2;      // Spare
            //size += 2;      // Checksum
            size += pd0.Header.GetDataTypeSize();


            Assert.AreEqual(2, pd0.Header.NumberOfDataTypes(), "Number of Data Types is incorrect.");
            Assert.AreEqual(5, pd0.Header.NumberOfDepthCells, "Number of depth cells is incorrect.");
            Assert.AreEqual(size, MathHelper.LsbMsbShort(data[2], data[3]), "Number of Bytes is incorrect.");
            Assert.AreEqual(2, data[5], "Number of Data Types is incorrect.");
            Assert.AreEqual(size + 2, data.Length, "Array Size is incorrect.");     // Add 2 for the checksum

            int flOffset = pd0.Header.GetDataTypeSize();
            Assert.AreEqual(flOffset, MathHelper.LsbMsbShort(data[6], data[7]), "Fixed Leader offset is incorrect.");

            int vlOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE;
            Assert.AreEqual(vlOffset, MathHelper.LsbMsbShort(data[8], data[9]), "Variable Leader offset is incorrect.");


        }

        /// <summary>
        /// Test decoding with velocity data.
        /// </summary>
        [Test]
        public void TestDecodeVelocity()
        {
            PD0 pd0 = new PD0();
            pd0.FixedLeader.NumberOfCells = 5;

            #region Velocity

            Pd0Velocity vel = new Pd0Velocity();
            // 2 Byte Header
            // 8 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] velData = new byte[42];

            velData[0] = Pd0Velocity.ID_LSB;
            velData[1] = Pd0Velocity.ID_MSB;
            velData[2] = 0xE8;                                   // DS0 Beam 0 LSB
            velData[3] = 0x00;                                   // DS0 Beam 0 MSB
            velData[4] = 0xBD;                                   // DS0 Beam 1 LSB
            velData[5] = 0xFE;                                   // DS0 Beam 1 MSB
            velData[6] = 0xC8;                                   // DS0 Beam 2 LSB
            velData[7] = 0x01;                                   // DS0 Beam 2 MSB
            velData[8] = 0x72;                                   // DS0 Beam 3 LSB
            velData[9] = 0xFD;                                   // DS0 Beam 3 MSB

            velData[(4 * 8) + 2 + 0] = 0xD0;                     // DS4 Beam 0 LSB
            velData[(4 * 8) + 2 + 1] = 0x04;                     // DS4 Beam 0 MSB
            velData[(4 * 8) + 2 + 2] = 0xD5;                     // DS4 Beam 1 LSB
            velData[(4 * 8) + 2 + 3] = 0xFA;                     // DS4 Beam 1 MSB
            velData[(4 * 8) + 2 + 4] = 0xB0;                     // DS4 Beam 2 LSB
            velData[(4 * 8) + 2 + 5] = 0x05;                     // DS4 Beam 2 MSB
            velData[(4 * 8) + 2 + 6] = 0x8A;                     // DS4 Beam 3 LSB
            velData[(4 * 8) + 2 + 7] = 0xF9;                     // DS4 Beam 3 MSB

            vel.Decode(velData);

            #endregion

            pd0.AddDataType(vel);                       // Add Velocity

            byte[] data = pd0.Encode();

            // Size
            int size = Pd0VariableLeader.DATATYPE_SIZE;
            size += Pd0FixedLeader.DATATYPE_SIZE;
            size += 2;      // Spare
            //size += 2;      // Checksum
            size += pd0.Header.GetDataTypeSize();
            size += pd0.Velocity.GetDataTypeSize();


            Assert.AreEqual(3, pd0.Header.NumberOfDataTypes(), "Number of Data Types is incorrect.");
            Assert.AreEqual(5, pd0.Header.NumberOfDepthCells, "Number of depth cells is incorrect.");
            Assert.AreEqual(size, MathHelper.LsbMsbShort(data[2], data[3]), "Number of Bytes is incorrect.");
            Assert.AreEqual(3, data[5], "Number of Data Types is incorrect.");
            Assert.AreEqual(size + 2, data.Length, "Array Size is incorrect.");     // Add 2 for the checksum

            int flOffset = pd0.Header.GetDataTypeSize();
            Assert.AreEqual(flOffset, MathHelper.LsbMsbShort(data[6], data[7]), "Fixed Leader offset is incorrect.");

            int vlOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE;
            Assert.AreEqual(vlOffset, MathHelper.LsbMsbShort(data[8], data[9]), "Variable Leader offset is incorrect.");

            int velOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE;
            Assert.AreEqual(velOffset, MathHelper.LsbMsbShort(data[10], data[11]), "Velocity offset is incorrect.");
        }

        /// <summary>
        /// Test decoding with velocity and percent good data.
        /// </summary>
        [Test]
        public void TestDecodeVelocityPg()
        {
            PD0 pd0 = new PD0();
            pd0.FixedLeader.NumberOfCells = 5;

            #region Velocity

            Pd0Velocity vel = new Pd0Velocity();
            // 2 Byte Header
            // 8 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] velData = new byte[42];

            velData[0] = Pd0Velocity.ID_LSB;
            velData[1] = Pd0Velocity.ID_MSB;
            velData[2] = 0xE8;                                   // DS0 Beam 0 LSB
            velData[3] = 0x00;                                   // DS0 Beam 0 MSB
            velData[4] = 0xBD;                                   // DS0 Beam 1 LSB
            velData[5] = 0xFE;                                   // DS0 Beam 1 MSB
            velData[6] = 0xC8;                                   // DS0 Beam 2 LSB
            velData[7] = 0x01;                                   // DS0 Beam 2 MSB
            velData[8] = 0x72;                                   // DS0 Beam 3 LSB
            velData[9] = 0xFD;                                   // DS0 Beam 3 MSB

            velData[(4 * 8) + 2 + 0] = 0xD0;                     // DS4 Beam 0 LSB
            velData[(4 * 8) + 2 + 1] = 0x04;                     // DS4 Beam 0 MSB
            velData[(4 * 8) + 2 + 2] = 0xD5;                     // DS4 Beam 1 LSB
            velData[(4 * 8) + 2 + 3] = 0xFA;                     // DS4 Beam 1 MSB
            velData[(4 * 8) + 2 + 4] = 0xB0;                     // DS4 Beam 2 LSB
            velData[(4 * 8) + 2 + 5] = 0x05;                     // DS4 Beam 2 MSB
            velData[(4 * 8) + 2 + 6] = 0x8A;                     // DS4 Beam 3 LSB
            velData[(4 * 8) + 2 + 7] = 0xF9;                     // DS4 Beam 3 MSB

            vel.Decode(velData);

            #endregion
            pd0.AddDataType(vel);                       // Add Velocity

            #region Percent Good

            Pd0PercentGood pg = new Pd0PercentGood();

            // 2 Byte Header
            // 4 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] pgData = new byte[22];

            pgData[0] = Pd0PercentGood.ID_LSB;
            pgData[1] = Pd0PercentGood.ID_MSB;
            pgData[2] = 0xE8;                                   // DS0 Beam 0 
            pgData[3] = 0x7B;                                   // DS0 Beam 1 
            pgData[4] = 0x7A;                                   // DS0 Beam 2 
            pgData[5] = 0xDF;                                   // DS0 Beam 3

            pgData[(4 * 4) + 2 + 0] = 0x6F;                     // DS4 Beam 0 
            pgData[(4 * 4) + 2 + 1] = 0xDE;                     // DS4 Beam 1 
            pgData[(4 * 4) + 2 + 2] = 0x15;                     // DS4 Beam 2 
            pgData[(4 * 4) + 2 + 3] = 0x22;                     // DS4 Beam 3 


            pg.Decode(pgData);

            #endregion
            pd0.AddDataType(pg);                        // Add Percent Good

            byte[] data = pd0.Encode();

            // Size
            int size = Pd0VariableLeader.DATATYPE_SIZE;
            size += Pd0FixedLeader.DATATYPE_SIZE;
            size += 2;      // Spare
            //size += 2;      // Checksum
            size += pd0.Header.GetDataTypeSize();
            size += pd0.Velocity.GetDataTypeSize();
            size += pd0.PercentGood.GetDataTypeSize();


            Assert.AreEqual(4, pd0.Header.NumberOfDataTypes(), "Number of Data Types is incorrect.");
            Assert.AreEqual(5, pd0.Header.NumberOfDepthCells, "Number of depth cells is incorrect.");
            Assert.AreEqual(size, MathHelper.LsbMsbShort(data[2], data[3]), "Number of Bytes is incorrect.");
            Assert.AreEqual(4, data[5], "Number of Data Types is incorrect.");
            Assert.AreEqual(size + 2, data.Length, "Array Size is incorrect.");     // Add 2 for the checksum

            int flOffset = pd0.Header.GetDataTypeSize();
            Assert.AreEqual(flOffset, MathHelper.LsbMsbShort(data[6], data[7]), "Fixed Leader offset is incorrect.");

            int vlOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE;
            Assert.AreEqual(vlOffset, MathHelper.LsbMsbShort(data[8], data[9]), "Variable Leader offset is incorrect.");

            int velOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE;
            Assert.AreEqual(velOffset, MathHelper.LsbMsbShort(data[10], data[11]), "Velocity offset is incorrect.");

            int pgOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE + pd0.Velocity.GetDataTypeSize();
            Assert.AreEqual(pgOffset, MathHelper.LsbMsbShort(data[12], data[13]), "Percent Good offset is incorrect.");
        }

        /// <summary>
        /// Test decoding with velocity and percent good and Correlation data.
        /// </summary>
        [Test]
        public void TestDecodeVelocityPgCorr()
        {
            PD0 pd0 = new PD0();
            pd0.FixedLeader.NumberOfCells = 5;

            #region Velocity

            Pd0Velocity vel = new Pd0Velocity();
            // 2 Byte Header
            // 8 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] velData = new byte[42];

            velData[0] = Pd0Velocity.ID_LSB;
            velData[1] = Pd0Velocity.ID_MSB;
            velData[2] = 0xE8;                                   // DS0 Beam 0 LSB
            velData[3] = 0x00;                                   // DS0 Beam 0 MSB
            velData[4] = 0xBD;                                   // DS0 Beam 1 LSB
            velData[5] = 0xFE;                                   // DS0 Beam 1 MSB
            velData[6] = 0xC8;                                   // DS0 Beam 2 LSB
            velData[7] = 0x01;                                   // DS0 Beam 2 MSB
            velData[8] = 0x72;                                   // DS0 Beam 3 LSB
            velData[9] = 0xFD;                                   // DS0 Beam 3 MSB

            velData[(4 * 8) + 2 + 0] = 0xD0;                     // DS4 Beam 0 LSB
            velData[(4 * 8) + 2 + 1] = 0x04;                     // DS4 Beam 0 MSB
            velData[(4 * 8) + 2 + 2] = 0xD5;                     // DS4 Beam 1 LSB
            velData[(4 * 8) + 2 + 3] = 0xFA;                     // DS4 Beam 1 MSB
            velData[(4 * 8) + 2 + 4] = 0xB0;                     // DS4 Beam 2 LSB
            velData[(4 * 8) + 2 + 5] = 0x05;                     // DS4 Beam 2 MSB
            velData[(4 * 8) + 2 + 6] = 0x8A;                     // DS4 Beam 3 LSB
            velData[(4 * 8) + 2 + 7] = 0xF9;                     // DS4 Beam 3 MSB

            vel.Decode(velData);

            #endregion
            pd0.AddDataType(vel);                       // Add Velocity

            #region Percent Good

            Pd0PercentGood pg = new Pd0PercentGood();

            // 2 Byte Header
            // 4 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] pgData = new byte[22];

            pgData[0] = Pd0PercentGood.ID_LSB;
            pgData[1] = Pd0PercentGood.ID_MSB;
            pgData[2] = 0xE8;                                   // DS0 Beam 0 
            pgData[3] = 0x7B;                                   // DS0 Beam 1 
            pgData[4] = 0x7A;                                   // DS0 Beam 2 
            pgData[5] = 0xDF;                                   // DS0 Beam 3

            pgData[(4 * 4) + 2 + 0] = 0x6F;                     // DS4 Beam 0 
            pgData[(4 * 4) + 2 + 1] = 0xDE;                     // DS4 Beam 1 
            pgData[(4 * 4) + 2 + 2] = 0x15;                     // DS4 Beam 2 
            pgData[(4 * 4) + 2 + 3] = 0x22;                     // DS4 Beam 3 


            pg.Decode(pgData);

            #endregion
            pd0.AddDataType(pg);                        // Add Percent Good

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
            pd0.AddDataType(corr);                      // Add Correlation


            byte[] data = pd0.Encode();

            // Size
            int size = Pd0VariableLeader.DATATYPE_SIZE;
            size += Pd0FixedLeader.DATATYPE_SIZE;
            size += 2;      // Spare
            //size += 2;      // Checksum
            size += pd0.Header.GetDataTypeSize();
            size += pd0.Velocity.GetDataTypeSize();
            size += pd0.PercentGood.GetDataTypeSize();
            size += pd0.Correlation.GetDataTypeSize();


            Assert.AreEqual(5, pd0.Header.NumberOfDataTypes(), "Number of Data Types is incorrect.");
            Assert.AreEqual(5, pd0.Header.NumberOfDepthCells, "Number of depth cells is incorrect.");
            Assert.AreEqual(size, MathHelper.LsbMsbShort(data[2], data[3]), "Number of Bytes is incorrect.");
            Assert.AreEqual(5, data[5], "Number of Data Types is incorrect.");
            Assert.AreEqual(size + 2, data.Length, "Array Size is incorrect.");     // Add 2 for the checksum

            int flOffset = pd0.Header.GetDataTypeSize();
            Assert.AreEqual(flOffset, MathHelper.LsbMsbShort(data[6], data[7]), "Fixed Leader offset is incorrect.");

            int vlOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE;
            Assert.AreEqual(vlOffset, MathHelper.LsbMsbShort(data[8], data[9]), "Variable Leader offset is incorrect.");

            int velOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE;
            Assert.AreEqual(velOffset, MathHelper.LsbMsbShort(data[10], data[11]), "Velocity offset is incorrect.");

            int pgOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE + pd0.Velocity.GetDataTypeSize();
            Assert.AreEqual(pgOffset, MathHelper.LsbMsbShort(data[12], data[13]), "Percent Good offset is incorrect.");

            int corrOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE + pd0.Velocity.GetDataTypeSize() + pd0.Correlation.GetDataTypeSize();
            Assert.AreEqual(corrOffset, MathHelper.LsbMsbShort(data[14], data[15]), "Correlation offset is incorrect.");
        }

        /// <summary>
        /// Test decoding with velocity and percent good and Correlation and Echo Intensity data.
        /// </summary>
        [Test]
        public void TestDecodeVelocityPgCorrEi()
        {
            PD0 pd0 = new PD0();
            pd0.FixedLeader.NumberOfCells = 5;

            #region Velocity

            Pd0Velocity vel = new Pd0Velocity();
            // 2 Byte Header
            // 8 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] velData = new byte[42];

            velData[0] = Pd0Velocity.ID_LSB;
            velData[1] = Pd0Velocity.ID_MSB;
            velData[2] = 0xE8;                                   // DS0 Beam 0 LSB
            velData[3] = 0x00;                                   // DS0 Beam 0 MSB
            velData[4] = 0xBD;                                   // DS0 Beam 1 LSB
            velData[5] = 0xFE;                                   // DS0 Beam 1 MSB
            velData[6] = 0xC8;                                   // DS0 Beam 2 LSB
            velData[7] = 0x01;                                   // DS0 Beam 2 MSB
            velData[8] = 0x72;                                   // DS0 Beam 3 LSB
            velData[9] = 0xFD;                                   // DS0 Beam 3 MSB

            velData[(4 * 8) + 2 + 0] = 0xD0;                     // DS4 Beam 0 LSB
            velData[(4 * 8) + 2 + 1] = 0x04;                     // DS4 Beam 0 MSB
            velData[(4 * 8) + 2 + 2] = 0xD5;                     // DS4 Beam 1 LSB
            velData[(4 * 8) + 2 + 3] = 0xFA;                     // DS4 Beam 1 MSB
            velData[(4 * 8) + 2 + 4] = 0xB0;                     // DS4 Beam 2 LSB
            velData[(4 * 8) + 2 + 5] = 0x05;                     // DS4 Beam 2 MSB
            velData[(4 * 8) + 2 + 6] = 0x8A;                     // DS4 Beam 3 LSB
            velData[(4 * 8) + 2 + 7] = 0xF9;                     // DS4 Beam 3 MSB

            vel.Decode(velData);

            #endregion
            pd0.AddDataType(vel);                       // Add Velocity

            #region Percent Good

            Pd0PercentGood pg = new Pd0PercentGood();

            // 2 Byte Header
            // 4 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] pgData = new byte[22];

            pgData[0] = Pd0PercentGood.ID_LSB;
            pgData[1] = Pd0PercentGood.ID_MSB;
            pgData[2] = 0xE8;                                   // DS0 Beam 0 
            pgData[3] = 0x7B;                                   // DS0 Beam 1 
            pgData[4] = 0x7A;                                   // DS0 Beam 2 
            pgData[5] = 0xDF;                                   // DS0 Beam 3

            pgData[(4 * 4) + 2 + 0] = 0x6F;                     // DS4 Beam 0 
            pgData[(4 * 4) + 2 + 1] = 0xDE;                     // DS4 Beam 1 
            pgData[(4 * 4) + 2 + 2] = 0x15;                     // DS4 Beam 2 
            pgData[(4 * 4) + 2 + 3] = 0x22;                     // DS4 Beam 3 


            pg.Decode(pgData);

            #endregion
            pd0.AddDataType(pg);                        // Add Percent Good

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
            pd0.AddDataType(corr);                      // Add Correlation

            #region Echo Intensity

            Pd0EchoIntensity ei = new Pd0EchoIntensity();

            // 2 Byte Header
            // 4 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] eiData = new byte[22];

            eiData[0] = Pd0Velocity.ID_LSB;
            eiData[1] = Pd0Velocity.ID_MSB;
            eiData[2] = 0xE8;                                   // DS0 Beam 0 
            eiData[3] = 0x7B;                                   // DS0 Beam 1 
            eiData[4] = 0x7A;                                   // DS0 Beam 2 
            eiData[5] = 0xDF;                                   // DS0 Beam 3

            eiData[(4 * 4) + 2 + 0] = 0x6F;                     // DS4 Beam 0 
            eiData[(4 * 4) + 2 + 1] = 0xDE;                     // DS4 Beam 1 
            eiData[(4 * 4) + 2 + 2] = 0x15;                     // DS4 Beam 2 
            eiData[(4 * 4) + 2 + 3] = 0x22;                     // DS4 Beam 3 


            ei.Decode(eiData);

            #endregion
            pd0.AddDataType(ei);                        // Add Echo Intensity

            byte[] data = pd0.Encode();

            // Size
            int size = Pd0VariableLeader.DATATYPE_SIZE;
            size += Pd0FixedLeader.DATATYPE_SIZE;
            size += 2;      // Spare
            //size += 2;      // Checksum
            size += pd0.Header.GetDataTypeSize();
            size += pd0.Velocity.GetDataTypeSize();
            size += pd0.PercentGood.GetDataTypeSize();
            size += pd0.Correlation.GetDataTypeSize();
            size += pd0.EchoIntensity.GetDataTypeSize();


            Assert.AreEqual(6, pd0.Header.NumberOfDataTypes(), "Number of Data Types is incorrect.");
            Assert.AreEqual(5, pd0.Header.NumberOfDepthCells, "Number of depth cells is incorrect.");
            Assert.AreEqual(size, MathHelper.LsbMsbShort(data[2], data[3]), "Number of Bytes is incorrect.");
            Assert.AreEqual(6, data[5], "Number of Data Types is incorrect.");
            Assert.AreEqual(size + 2, data.Length, "Array Size is incorrect.");     // Add 2 for the checksum

            int flOffset = pd0.Header.GetDataTypeSize();
            Assert.AreEqual(flOffset, MathHelper.LsbMsbShort(data[6], data[7]), "Fixed Leader offset is incorrect.");

            int vlOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE;
            Assert.AreEqual(vlOffset, MathHelper.LsbMsbShort(data[8], data[9]), "Variable Leader offset is incorrect.");

            int velOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE;
            Assert.AreEqual(velOffset, MathHelper.LsbMsbShort(data[10], data[11]), "Velocity offset is incorrect.");

            int pgOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE + pd0.Velocity.GetDataTypeSize();
            Assert.AreEqual(pgOffset, MathHelper.LsbMsbShort(data[12], data[13]), "Percent Good offset is incorrect.");

            int corrOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE + pd0.Velocity.GetDataTypeSize() + pd0.Correlation.GetDataTypeSize();
            Assert.AreEqual(corrOffset, MathHelper.LsbMsbShort(data[14], data[15]), "Correlation offset is incorrect.");

            int eiOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE + pd0.Velocity.GetDataTypeSize() + pd0.Correlation.GetDataTypeSize() + pd0.EchoIntensity.GetDataTypeSize();
            Assert.AreEqual(eiOffset, MathHelper.LsbMsbShort(data[16], data[17]), "Echo Intensity offset is incorrect.");
        }

        /// <summary>
        /// Test decoding with velocity and percent good and Correlation and Echo Intensity and Bottom Track data.
        /// </summary>
        [Test]
        public void TestDecodeVelocityPgCorrEiBt()
        {
            PD0 pd0 = new PD0();
            pd0.FixedLeader.NumberOfCells = 5;

            #region Velocity

            Pd0Velocity vel = new Pd0Velocity();
            // 2 Byte Header
            // 8 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] velData = new byte[42];

            velData[0] = Pd0Velocity.ID_LSB;
            velData[1] = Pd0Velocity.ID_MSB;
            velData[2] = 0xE8;                                   // DS0 Beam 0 LSB
            velData[3] = 0x00;                                   // DS0 Beam 0 MSB
            velData[4] = 0xBD;                                   // DS0 Beam 1 LSB
            velData[5] = 0xFE;                                   // DS0 Beam 1 MSB
            velData[6] = 0xC8;                                   // DS0 Beam 2 LSB
            velData[7] = 0x01;                                   // DS0 Beam 2 MSB
            velData[8] = 0x72;                                   // DS0 Beam 3 LSB
            velData[9] = 0xFD;                                   // DS0 Beam 3 MSB

            velData[(4 * 8) + 2 + 0] = 0xD0;                     // DS4 Beam 0 LSB
            velData[(4 * 8) + 2 + 1] = 0x04;                     // DS4 Beam 0 MSB
            velData[(4 * 8) + 2 + 2] = 0xD5;                     // DS4 Beam 1 LSB
            velData[(4 * 8) + 2 + 3] = 0xFA;                     // DS4 Beam 1 MSB
            velData[(4 * 8) + 2 + 4] = 0xB0;                     // DS4 Beam 2 LSB
            velData[(4 * 8) + 2 + 5] = 0x05;                     // DS4 Beam 2 MSB
            velData[(4 * 8) + 2 + 6] = 0x8A;                     // DS4 Beam 3 LSB
            velData[(4 * 8) + 2 + 7] = 0xF9;                     // DS4 Beam 3 MSB

            vel.Decode(velData);

            #endregion
            pd0.AddDataType(vel);                       // Add Velocity

            #region Percent Good

            Pd0PercentGood pg = new Pd0PercentGood();

            // 2 Byte Header
            // 4 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] pgData = new byte[22];

            pgData[0] = Pd0PercentGood.ID_LSB;
            pgData[1] = Pd0PercentGood.ID_MSB;
            pgData[2] = 0xE8;                                   // DS0 Beam 0 
            pgData[3] = 0x7B;                                   // DS0 Beam 1 
            pgData[4] = 0x7A;                                   // DS0 Beam 2 
            pgData[5] = 0xDF;                                   // DS0 Beam 3

            pgData[(4 * 4) + 2 + 0] = 0x6F;                     // DS4 Beam 0 
            pgData[(4 * 4) + 2 + 1] = 0xDE;                     // DS4 Beam 1 
            pgData[(4 * 4) + 2 + 2] = 0x15;                     // DS4 Beam 2 
            pgData[(4 * 4) + 2 + 3] = 0x22;                     // DS4 Beam 3 


            pg.Decode(pgData);

            #endregion
            pd0.AddDataType(pg);                        // Add Percent Good

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
            pd0.AddDataType(corr);                      // Add Correlation

            #region Echo Intensity

            Pd0EchoIntensity ei = new Pd0EchoIntensity();

            // 2 Byte Header
            // 4 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] eiData = new byte[22];

            eiData[0] = Pd0Velocity.ID_LSB;
            eiData[1] = Pd0Velocity.ID_MSB;
            eiData[2] = 0xE8;                                   // DS0 Beam 0 
            eiData[3] = 0x7B;                                   // DS0 Beam 1 
            eiData[4] = 0x7A;                                   // DS0 Beam 2 
            eiData[5] = 0xDF;                                   // DS0 Beam 3

            eiData[(4 * 4) + 2 + 0] = 0x6F;                     // DS4 Beam 0 
            eiData[(4 * 4) + 2 + 1] = 0xDE;                     // DS4 Beam 1 
            eiData[(4 * 4) + 2 + 2] = 0x15;                     // DS4 Beam 2 
            eiData[(4 * 4) + 2 + 3] = 0x22;                     // DS4 Beam 3 


            ei.Decode(eiData);

            #endregion
            pd0.AddDataType(ei);                        // Add Echo Intensity

            #region Bottom Track

            Pd0BottomTrack bt = new Pd0BottomTrack();

            // 2 Byte Header
            // 4 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] btData = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            bt.Decode(btData);

            #endregion
            pd0.AddDataType(bt);                        // Add Bottom Track

            byte[] data = pd0.Encode();

            // Size
            int size = pd0.FixedLeader.GetDataTypeSize();
            size += pd0.VariableLeader.GetDataTypeSize();
            size += 2;      // Spare
            size += pd0.Header.GetDataTypeSize();
            size += pd0.Velocity.GetDataTypeSize();
            size += pd0.PercentGood.GetDataTypeSize();
            size += pd0.Correlation.GetDataTypeSize();
            size += pd0.EchoIntensity.GetDataTypeSize();
            size += pd0.BottomTrack.GetDataTypeSize();


            Assert.AreEqual(7, pd0.Header.NumberOfDataTypes(), "Number of Data Types is incorrect.");
            Assert.AreEqual(5, pd0.Header.NumberOfDepthCells, "Number of depth cells is incorrect.");
            Assert.AreEqual(size, MathHelper.LsbMsbShort(data[2], data[3]), "Number of Bytes is incorrect.");
            Assert.AreEqual(7, data[5], "Number of Data Types is incorrect.");
            Assert.AreEqual(size + 2, data.Length, "Array Size is incorrect.");     // Add 2 for the checksum

            int flOffset = pd0.Header.GetDataTypeSize();
            Assert.AreEqual(flOffset, MathHelper.LsbMsbShort(data[6], data[7]), "Fixed Leader offset is incorrect.");

            int vlOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE;
            Assert.AreEqual(vlOffset, MathHelper.LsbMsbShort(data[8], data[9]), "Variable Leader offset is incorrect.");

            int velOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE;
            Assert.AreEqual(velOffset, MathHelper.LsbMsbShort(data[10], data[11]), "Velocity offset is incorrect.");

            int pgOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE + pd0.Velocity.GetDataTypeSize();
            Assert.AreEqual(pgOffset, MathHelper.LsbMsbShort(data[12], data[13]), "Percent Good offset is incorrect.");

            int corrOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE + pd0.Velocity.GetDataTypeSize() + pd0.PercentGood.GetDataTypeSize();
            Assert.AreEqual(corrOffset, MathHelper.LsbMsbShort(data[14], data[15]), "Correlation offset is incorrect.");

            int eiOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE + pd0.Velocity.GetDataTypeSize() + pd0.PercentGood.GetDataTypeSize() + pd0.Correlation.GetDataTypeSize();
            Assert.AreEqual(eiOffset, MathHelper.LsbMsbShort(data[16], data[17]), "Echo Intensity offset is incorrect.");

            int btOffset = pd0.Header.GetDataTypeSize() + Pd0FixedLeader.DATATYPE_SIZE + Pd0VariableLeader.DATATYPE_SIZE + pd0.Velocity.GetDataTypeSize() + pd0.PercentGood.GetDataTypeSize() + pd0.Correlation.GetDataTypeSize() + pd0.EchoIntensity.GetDataTypeSize();
            Assert.AreEqual(btOffset, MathHelper.LsbMsbShort(data[18], data[19]), "Bottom Track offset is incorrect.");
        }

        #endregion

        #region Encode

        /// <summary>
        /// Test Encoding with Velocity.
        /// </summary>
        [Test]
        public void TestEncodeVel()
        {
            PD0 pd0 = new PD0();
            pd0.FixedLeader.NumberOfCells = 5;

            Pd0Velocity vel = new Pd0Velocity(5);
            vel.Velocities[0, 0] = 32;
            vel.Velocities[0, 1] = -32;
            pd0.AddDataType(vel);

            byte[] encode = pd0.Encode();

            int flOffset = pd0.Header.GetFixedLeader().Offset;
            int velOffset = pd0.Header.GetVelocity().Offset;

            Assert.AreEqual(0, encode[flOffset], "Fixed Leader ID LSB is incorrect");
            Assert.AreEqual(0, encode[flOffset + 1], "Fixed Leader ID MSB is incorrect");
            Assert.AreEqual(5, encode[flOffset + 9], "Number of depth cells is incorrect");

            Assert.AreEqual(32, MathHelper.LsbMsbShort(encode[velOffset + 2], encode[velOffset + 3]), "Velocity Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(-32, MathHelper.LsbMsbShort(encode[velOffset + 4], encode[velOffset + 5]), "Velocity Bin 0, Beam 1 is incorrect.");
        }

        /// <summary>
        /// Test Encoding with Velocity.
        /// </summary>
        [Test]
        public void TestEncodeVelCorr()
        {
            PD0 pd0 = new PD0();
            pd0.FixedLeader.NumberOfCells = 5;

            Pd0Velocity vel = new Pd0Velocity(5);
            vel.Velocities[0, 0] = 32;
            vel.Velocities[0, 1] = -32;
            pd0.AddDataType(vel);

            Pd0Correlation corr = new Pd0Correlation(5);
            corr.Correlation[0, 0] = 32;
            corr.Correlation[0, 1] = 255;
            pd0.AddDataType(corr);

            byte[] encode = pd0.Encode();

            int flOffset = pd0.Header.GetFixedLeader().Offset;
            int velOffset = pd0.Header.GetVelocity().Offset;
            int corrOffset = pd0.Header.GetCorrelation().Offset;

            Assert.AreEqual(0, encode[flOffset], "Fixed Leader ID LSB is incorrect");
            Assert.AreEqual(0, encode[flOffset + 1], "Fixed Leader ID MSB is incorrect");
            Assert.AreEqual(5, encode[flOffset + 9], "Number of depth cells is incorrect");

            Assert.AreEqual(32, MathHelper.LsbMsbShort(encode[velOffset + 2], encode[velOffset + 3]), "Velocity Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(-32, MathHelper.LsbMsbShort(encode[velOffset + 4], encode[velOffset + 5]), "Velocity Bin 0, Beam 1 is incorrect.");

            Assert.AreEqual(32, encode[corrOffset + 2], "Correlation Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(255, encode[corrOffset + 3], "Correlation Bin 0, Beam 1 is incorrect.");

            Assert.IsFalse(pd0.IsBottomTrackExist, "IsBottomTrackExist is incorrect.");
            Assert.IsTrue(pd0.IsCorrelationExist, "IsCorrelationExist is incorrect.");
            Assert.IsFalse(pd0.IsEchoIntensityExist, "IsEchoIntensityExist is incorrect.");
            Assert.IsFalse(pd0.IsPercentGoodExist, "IsPercentGoodExist is incorrect.");
        }

        /// <summary>
        /// Test Encoding with Velocity.
        /// </summary>
        [Test]
        public void TestEncodeVelCorrEcho()
        {
            PD0 pd0 = new PD0();
            pd0.FixedLeader.NumberOfCells = 5;

            Pd0Velocity vel = new Pd0Velocity(5);
            vel.Velocities[0, 0] = 32;
            vel.Velocities[0, 1] = -32;
            pd0.AddDataType(vel);

            Pd0Correlation corr = new Pd0Correlation(5);
            corr.Correlation[0, 0] = 32;
            corr.Correlation[0, 1] = 255;
            pd0.AddDataType(corr);

            Pd0EchoIntensity ei = new Pd0EchoIntensity(5);
            ei.EchoIntensity[0, 0] = 32;
            ei.EchoIntensity[0, 1] = 255;
            pd0.AddDataType(ei);

            byte[] encode = pd0.Encode();

            int flOffset = pd0.Header.GetFixedLeader().Offset;
            int velOffset = pd0.Header.GetVelocity().Offset;
            int corrOffset = pd0.Header.GetCorrelation().Offset;
            int eiOffset = pd0.Header.GetEchoIntensity().Offset;

            Assert.AreEqual(0, encode[flOffset], "Fixed Leader ID LSB is incorrect");
            Assert.AreEqual(0, encode[flOffset + 1], "Fixed Leader ID MSB is incorrect");
            Assert.AreEqual(5, encode[flOffset + 9], "Number of depth cells is incorrect");

            Assert.AreEqual(32, MathHelper.LsbMsbShort(encode[velOffset + 2], encode[velOffset + 3]), "Velocity Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(-32, MathHelper.LsbMsbShort(encode[velOffset + 4], encode[velOffset + 5]), "Velocity Bin 0, Beam 1 is incorrect.");

            Assert.AreEqual(32, encode[corrOffset + 2], "Correlation Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(255, encode[corrOffset + 3], "Correlation Bin 0, Beam 1 is incorrect.");

            Assert.AreEqual(32, encode[eiOffset + 2], "Echo Intensity Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(255, encode[eiOffset + 3], "Echo Intensity Bin 0, Beam 1 is incorrect.");

            Assert.IsFalse(pd0.IsBottomTrackExist, "IsBottomTrackExist is incorrect.");
            Assert.IsTrue(pd0.IsCorrelationExist, "IsCorrelationExist is incorrect.");
            Assert.IsTrue(pd0.IsEchoIntensityExist, "IsEchoIntensityExist is incorrect.");
            Assert.IsFalse(pd0.IsPercentGoodExist, "IsPercentGoodExist is incorrect.");
        }

        /// <summary>
        /// Test Encoding with Velocity.
        /// </summary>
        [Test]
        public void TestEncodeVelCorrEchoPg()
        {
            PD0 pd0 = new PD0();
            pd0.FixedLeader.NumberOfCells = 5;

            Pd0Velocity vel = new Pd0Velocity(5);
            vel.Velocities[0, 0] = 32;
            vel.Velocities[0, 1] = -32;
            pd0.AddDataType(vel);

            Pd0Correlation corr = new Pd0Correlation(5);
            corr.Correlation[0, 0] = 32;
            corr.Correlation[0, 1] = 255;
            pd0.AddDataType(corr);

            Pd0EchoIntensity ei = new Pd0EchoIntensity(5);
            ei.EchoIntensity[0, 0] = 32;
            ei.EchoIntensity[0, 1] = 255;
            pd0.AddDataType(ei);

            Pd0PercentGood pg = new Pd0PercentGood(5);
            pg.PercentGood[0, 0] = 32;
            pg.PercentGood[0, 1] = 255;
            pd0.AddDataType(pg);

            byte[] encode = pd0.Encode();

            int flOffset = pd0.Header.GetFixedLeader().Offset;
            int velOffset = pd0.Header.GetVelocity().Offset;
            int corrOffset = pd0.Header.GetCorrelation().Offset;
            int eiOffset = pd0.Header.GetEchoIntensity().Offset;
            int pgOffset = pd0.Header.GetPercentGood().Offset;

            Assert.AreEqual(0, encode[flOffset], "Fixed Leader ID LSB is incorrect");
            Assert.AreEqual(0, encode[flOffset + 1], "Fixed Leader ID MSB is incorrect");
            Assert.AreEqual(5, encode[flOffset + 9], "Number of depth cells is incorrect");

            Assert.AreEqual(32, MathHelper.LsbMsbShort(encode[velOffset + 2], encode[velOffset + 3]), "Velocity Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(-32, MathHelper.LsbMsbShort(encode[velOffset + 4], encode[velOffset + 5]), "Velocity Bin 0, Beam 1 is incorrect.");

            Assert.AreEqual(32, encode[corrOffset + 2], "Correlation Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(255, encode[corrOffset + 3], "Correlation Bin 0, Beam 1 is incorrect.");

            Assert.AreEqual(32, encode[eiOffset + 2], "Echo Intensity Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(255, encode[eiOffset + 3], "Echo Intensity Bin 0, Beam 1 is incorrect.");

            Assert.AreEqual(32, encode[pgOffset + 2], "Percent Good Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(255, encode[pgOffset + 3], "Percent Good Bin 0, Beam 1 is incorrect.");

            Assert.IsFalse(pd0.IsBottomTrackExist, "IsBottomTrackExist is incorrect.");
            Assert.IsTrue(pd0.IsCorrelationExist, "IsCorrelationExist is incorrect.");
            Assert.IsTrue(pd0.IsEchoIntensityExist, "IsEchoIntensityExist is incorrect.");
            Assert.IsTrue(pd0.IsPercentGoodExist, "IsPercentGoodExist is incorrect.");

        }

        /// <summary>
        /// Test Encoding with Velocity.
        /// </summary>
        [Test]
        public void TestEncodeVelCorrEchoPgBt()
        {
            PD0 pd0 = new PD0();
            pd0.FixedLeader.NumberOfCells = 5;

            Pd0Velocity vel = new Pd0Velocity(5);
            vel.Velocities[0, 0] = 32;
            vel.Velocities[0, 1] = -32;
            pd0.AddDataType(vel);

            Pd0Correlation corr = new Pd0Correlation(5);
            corr.Correlation[0, 0] = 32;
            corr.Correlation[0, 1] = 255;
            pd0.AddDataType(corr);

            Pd0EchoIntensity ei = new Pd0EchoIntensity(5);
            ei.EchoIntensity[0, 0] = 32;
            ei.EchoIntensity[0, 1] = 255;
            pd0.AddDataType(ei);

            Pd0PercentGood pg = new Pd0PercentGood(5);
            pg.PercentGood[0, 0] = 32;
            pg.PercentGood[0, 1] = 255;
            pd0.AddDataType(pg);

            Pd0BottomTrack bt = new Pd0BottomTrack();
            bt.BtPingsPerEnsemble = 20;
            bt.BtDelayBeforeReacquire = 20;
            bt.BtAmplitudeBeam0 = 255;
            pd0.AddDataType(bt);

            byte[] encode = pd0.Encode();

            int flOffset = pd0.Header.GetFixedLeader().Offset;
            int velOffset = pd0.Header.GetVelocity().Offset;
            int corrOffset = pd0.Header.GetCorrelation().Offset;
            int eiOffset = pd0.Header.GetEchoIntensity().Offset;
            int pgOffset = pd0.Header.GetPercentGood().Offset;
            int btOffset = pd0.Header.GetBottomTrack().Offset;

            Assert.AreEqual(0, encode[flOffset], "Fixed Leader ID LSB is incorrect");
            Assert.AreEqual(0, encode[flOffset + 1], "Fixed Leader ID MSB is incorrect");
            Assert.AreEqual(5, encode[flOffset + 9], "Number of depth cells is incorrect");

            Assert.AreEqual(32, MathHelper.LsbMsbShort(encode[velOffset + 2], encode[velOffset + 3]), "Velocity Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(-32, MathHelper.LsbMsbShort(encode[velOffset + 4], encode[velOffset + 5]), "Velocity Bin 0, Beam 1 is incorrect.");

            Assert.AreEqual(32, encode[corrOffset + 2], "Correlation Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(255, encode[corrOffset + 3], "Correlation Bin 0, Beam 1 is incorrect.");

            Assert.AreEqual(32, encode[eiOffset + 2], "Echo Intensity Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(255, encode[eiOffset + 3], "Echo Intensity Bin 0, Beam 1 is incorrect.");

            Assert.AreEqual(32, encode[pgOffset + 2], "Percent Good Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(255, encode[pgOffset + 3], "Percent Good Bin 0, Beam 1 is incorrect.");

            Assert.AreEqual(20, MathHelper.LsbMsbUShort(encode[btOffset + 2], encode[btOffset + 3]), "Bottom Track Pings per Ensemble is incorrect.");
            Assert.AreEqual(20, MathHelper.LsbMsbUShort(encode[btOffset + 4], encode[btOffset + 5]), "Bottom Track Delay Before Reacquire is incorrect.");
            Assert.AreEqual(255, encode[btOffset + 36], "Bottom Track Amplitude Beam 0 is incorrect.");

            Assert.IsTrue(pd0.IsBottomTrackExist, "IsBottomTrackExist is incorrect.");
            Assert.IsTrue(pd0.IsCorrelationExist, "IsCorrelationExist is incorrect.");
            Assert.IsTrue(pd0.IsEchoIntensityExist, "IsEchoIntensityExist is incorrect.");
            Assert.IsTrue(pd0.IsPercentGoodExist, "IsPercentGoodExist is incorrect.");
        }

        #endregion

        #region RTI Decode

        /// <summary>
        /// Test decoding RTI Ensemble to PD0 Ensemble.
        /// </summary>
        [Test]
        public void DecodeRtiTest()
        {
            DataSet.Ensemble ens = EnsembleHelper.GenerateEnsemble(30);

            PD0 pd0 = new PD0(ens, PD0.CoordinateTransforms.Coord_Earth);

            Assert.IsTrue(pd0.IsVelocityExist, "IsVelocityExist is incorrect.");
            Assert.IsTrue(pd0.IsCorrelationExist, "IsCorrelationExist is incorrect.");
            Assert.IsTrue(pd0.IsEchoIntensityExist, "IsEchoIntensityExist is incorrect.");
            Assert.IsTrue(pd0.IsPercentGoodExist, "IsPercentGoodExist is incorrect.");
            Assert.IsTrue(pd0.IsBottomTrackExist, "IsBottomTrackExist is incorrect.");
            Assert.AreEqual(30, pd0.FixedLeader.NumberOfCells, "Number of cells is incorrect.");
            Assert.AreEqual(4, pd0.FixedLeader.NumberOfBeams, "Number of beams is incorrect.");
            Assert.AreEqual(PD0.CoordinateTransforms.Coord_Earth, pd0.FixedLeader.GetCoordinateTransform(), "Coordinate Transform is incorrect.");
        }

        #endregion
    }
}
