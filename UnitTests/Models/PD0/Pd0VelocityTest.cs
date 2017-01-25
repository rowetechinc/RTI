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
 * 03/12/2014      RC          2.21.4     Initial coding
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
    /// Test the PD0 Velocity.
    /// </summary>
    [TestFixture]
    public class Pd0VelocityTest
    {

        #region Encode

        /// <summary>
        /// Test encoding data.
        /// </summary>
        [Test]
        public void TestEncode()
        {
            Pd0Velocity vel = new Pd0Velocity();

            vel.Velocities = new short[5, 4];

            vel.Velocities[0, 0] = 232;
            vel.Velocities[0, 1] = -323;
            vel.Velocities[0, 2] = 456;
            vel.Velocities[0, 3] = -654;

            vel.Velocities[4, 0] = 1232;
            vel.Velocities[4, 1] = -1323;
            vel.Velocities[4, 2] = 1456;
            vel.Velocities[4, 3] = -1654;


            byte[] data = vel.Encode();

            // DS 0 Beam 0
            Assert.AreEqual(0xE8, data[2], "DS 0 Beam 0 LSB Encode is incorrect.");
            Assert.AreEqual(0x00, data[3], "DS 0 Beam 0 MSB Encode is incorrect.");

            // DS 0 Beam 1
            Assert.AreEqual(0xBD, data[4], "DS 0 Beam 1 LSB Encode is incorrect.");
            Assert.AreEqual(0xFE, data[5], "DS 0 Beam 1 MSB Encode is incorrect.");

            // DS 0 Beam 2
            Assert.AreEqual(0xC8, data[6], "DS 0 Beam 2 LSB Encode is incorrect.");
            Assert.AreEqual(0x01, data[7], "DS 0 Beam 2 MSB Encode is incorrect.");

            // DS 0 Beam 3
            Assert.AreEqual(0x72, data[8], "DS 0 Beam 3 LSB Encode is incorrect.");
            Assert.AreEqual(0xFD, data[9], "DS 0 Beam 3 MSB Encode is incorrect.");



            // DS 4 Beam 0
            Assert.AreEqual(0xD0, data[(4 * 8) + 2 + 0], "DS 4 Beam 0 LSB Encode is incorrect.");
            Assert.AreEqual(0x04, data[(4 * 8) + 2 + 1], "DS 4 Beam 0 MSB Encode is incorrect.");

            // DS 4 Beam 1
            Assert.AreEqual(0xD5, data[(4 * 8) + 2 + 2], "DS 4 Beam 1 LSB Encode is incorrect.");
            Assert.AreEqual(0xFA, data[(4 * 8) + 2 + 3], "DS 4 Beam 1 MSB Encode is incorrect.");

            // DS 4 Beam 2
            Assert.AreEqual(0xB0, data[(4 * 8) + 2 + 4], "DS 4 Beam 2 LSB Encode is incorrect.");
            Assert.AreEqual(0x05, data[(4 * 8) + 2 + 5], "DS 4 Beam 2 MSB Encode is incorrect.");

            // DS 4 Beam 3
            Assert.AreEqual(0x8A, data[(4 * 8) + 2 + 6], "DS 4 Beam 3 LSB Encode is incorrect.");
            Assert.AreEqual(0xF9, data[(4 * 8) + 2 + 7], "DS 4 Beam 3 MSB Encode is incorrect.");
        }

        #endregion

        #region Decode

        /// <summary>
        /// Test decoding data.
        /// </summary>
        [Test]
        public void TestDecode()
        {
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

            Assert.AreEqual(5, vel.Velocities.GetLength(0), "Number of Depth Cells is incorrect.");
            Assert.AreEqual(232, vel.Velocities[0, 0], "DS 0 Beam 0 is incorrect.");
            Assert.AreEqual(-323, vel.Velocities[0, 1], "DS 0 Beam 1 is incorrect.");
            Assert.AreEqual(456, vel.Velocities[0, 2], "DS 0 Beam 2 is incorrect.");
            Assert.AreEqual(-654, vel.Velocities[0, 3], "DS 0 Beam 3 is incorrect.");

            Assert.AreEqual(1232, vel.Velocities[4, 0], "DS 4 Beam 0 is incorrect.");
            Assert.AreEqual(-1323, vel.Velocities[4, 1], "DS 4 Beam 1 is incorrect.");
            Assert.AreEqual(1456, vel.Velocities[4, 2], "DS 4 Beam 2 is incorrect.");
            Assert.AreEqual(-1654, vel.Velocities[4, 3], "DS 4 Beam 3 is incorrect.");
        }

        #endregion

        #region GetVelocity

        /// <summary>
        /// Test decoding data.
        /// </summary>
        [Test]
        public void TestGetVelocity()
        {
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

            Assert.AreEqual(5, vel.Velocities.GetLength(0), "Number of Depth Cells is incorrect.");
            Assert.AreEqual(5, vel.GetNumDepthCells(), "GetNumDepthCells is incorrect.");
            Assert.AreEqual(232, vel.GetVelocity(0, 0), "DS 0 Beam 0 is incorrect.");
            Assert.AreEqual(-323, vel.GetVelocity(0, 1), "DS 0 Beam 1 is incorrect.");
            Assert.AreEqual(456, vel.GetVelocity(0, 2), "DS 0 Beam 2 is incorrect.");
            Assert.AreEqual(-654, vel.GetVelocity(0, 3), "DS 0 Beam 3 is incorrect.");

            Assert.AreEqual(1232, vel.GetVelocity(4, 0), "DS 4 Beam 0 is incorrect.");
            Assert.AreEqual(-1323, vel.GetVelocity(4, 1), "DS 4 Beam 1 is incorrect.");
            Assert.AreEqual(1456, vel.GetVelocity(4, 2), "DS 4 Beam 2 is incorrect.");
            Assert.AreEqual(-1654, vel.GetVelocity(4, 3), "DS 4 Beam 3 is incorrect.");
        }

        #endregion

        #region RTI Decode

        /// <summary>
        /// Test decoding RTI Beam Velocity data to PD0 Velocity.
        /// </summary>
        [Test]
        public void DecodeRtiTestBeam()
        {
            Pd0Velocity vel = new Pd0Velocity();

            DataSet.BeamVelocityDataSet beam = new DataSet.BeamVelocityDataSet(DataSet.Ensemble.DATATYPE_FLOAT,                 // Type of data stored (Float or Int)
                                                                                30,                                             // Number of bins
                                                                                4,                                              // Number of beams
                                                                                DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                                                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                                                                DataSet.Ensemble.BeamVelocityID);               // Dataset ID


            beam.BeamVelocityData[0, 0] = 0.123f;
            beam.BeamVelocityData[0, 1] = 0.456f;
            beam.BeamVelocityData[0, 2] = 0.789f;
            beam.BeamVelocityData[0, 3] = 0.147f;
            beam.BeamVelocityData[1, 0] = 0.258f;
            beam.BeamVelocityData[1, 1] = 0.369f;
            beam.BeamVelocityData[1, 2] = 0.741f;
            beam.BeamVelocityData[1, 3] = 0.852f;

            vel.DecodeRtiEnsemble(beam);

            // beam order 3,2,0,1; XYZ order 1,0,-2,3, ENU order 0,1,2,3

            Assert.AreEqual(147, vel.Velocities[0, 0], "Bin 0, Beam 0 Velocity is incorrect.");
            Assert.AreEqual(789, vel.Velocities[0, 1], "Bin 0, Beam 1 Velocity is incorrect.");
            Assert.AreEqual(123, vel.Velocities[0, 2], "Bin 0, Beam 2 Velocity is incorrect.");
            Assert.AreEqual(456, vel.Velocities[0, 3], "Bin 0, Beam 3 Velocity is incorrect.");
            Assert.AreEqual(852, vel.Velocities[1, 0], "Bin 1, Beam 0 Velocity is incorrect.");
            Assert.AreEqual(741, vel.Velocities[1, 1], "Bin 1, Beam 1 Velocity is incorrect.");
            Assert.AreEqual(258, vel.Velocities[1, 2], "Bin 1, Beam 2 Velocity is incorrect.");
            Assert.AreEqual(369, vel.Velocities[1, 3], "Bin 1, Beam 3 Velocity is incorrect.");


        }

        /// <summary>
        /// Test decoding RTI Instrument Velocity data to PD0 Velocity.
        /// </summary>
        [Test]
        public void DecodeRtiTestInstrument()
        {
            Pd0Velocity vel = new Pd0Velocity();

            DataSet.InstrumentVelocityDataSet instrument = new DataSet.InstrumentVelocityDataSet(DataSet.Ensemble.DATATYPE_FLOAT,                   // Type of data stored (Float or Int)
                                                                                                    30,                                             // Number of bins
                                                                                                    4,                                              // Number of beams
                                                                                                    DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                                                                                    DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                                                                                    DataSet.Ensemble.InstrumentVelocityID);         // Dataset ID


            instrument.InstrumentVelocityData[0, 0] = 0.123f;
            instrument.InstrumentVelocityData[0, 1] = 0.456f;
            instrument.InstrumentVelocityData[0, 2] = 0.789f;
            instrument.InstrumentVelocityData[0, 3] = 0.147f;
            instrument.InstrumentVelocityData[1, 0] = 0.258f;
            instrument.InstrumentVelocityData[1, 1] = 0.369f;
            instrument.InstrumentVelocityData[1, 2] = 0.741f;
            instrument.InstrumentVelocityData[1, 3] = 0.852f;

            vel.DecodeRtiEnsemble(instrument);

            // beam order 3,2,0,1; XYZ order 1,0,-2,3, ENU order 0,1,2,3

            Assert.AreEqual(456, vel.Velocities[0, 0], "Bin 0, Beam 0 Velocity is incorrect.");
            Assert.AreEqual(123, vel.Velocities[0, 1], "Bin 0, Beam 1 Velocity is incorrect.");
            Assert.AreEqual(-789, vel.Velocities[0, 2], "Bin 0, Beam 2 Velocity is incorrect.");
            Assert.AreEqual(147, vel.Velocities[0, 3], "Bin 0, Beam 3 Velocity is incorrect.");
            Assert.AreEqual(369, vel.Velocities[1, 0], "Bin 1, Beam 0 Velocity is incorrect.");
            Assert.AreEqual(258, vel.Velocities[1, 1], "Bin 1, Beam 1 Velocity is incorrect.");
            Assert.AreEqual(-741, vel.Velocities[1, 2], "Bin 1, Beam 2 Velocity is incorrect.");
            Assert.AreEqual(852, vel.Velocities[1, 3], "Bin 1, Beam 3 Velocity is incorrect.");


        }

        /// <summary>
        /// Test decoding RTI Beam Velocity data to PD0 Velocity.
        /// </summary>
        [Test]
        public void DecodeRtiTestEarth()
        {
            Pd0Velocity vel = new Pd0Velocity();

            DataSet.EarthVelocityDataSet earth = new DataSet.EarthVelocityDataSet(DataSet.Ensemble.DATATYPE_FLOAT,              // Type of data stored (Float or Int)
                                                                                30,                                             // Number of bins
                                                                                4,                                              // Number of beams
                                                                                DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                                                                DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                                                                DataSet.Ensemble.EarthVelocityID);              // Dataset ID


            earth.EarthVelocityData[0, 0] = 0.123f;
            earth.EarthVelocityData[0, 1] = 0.456f;
            earth.EarthVelocityData[0, 2] = 0.789f;
            earth.EarthVelocityData[0, 3] = 0.147f;
            earth.EarthVelocityData[1, 0] = 0.258f;
            earth.EarthVelocityData[1, 1] = 0.369f;
            earth.EarthVelocityData[1, 2] = 0.741f;
            earth.EarthVelocityData[1, 3] = 0.852f;

            vel.DecodeRtiEnsemble(earth);

            // beam order 3,2,0,1; XYZ order 1,0,-2,3, ENU order 0,1,2,3

            Assert.AreEqual(123, vel.Velocities[0, 0], "Bin 0, Beam 0 Velocity is incorrect.");
            Assert.AreEqual(456, vel.Velocities[0, 1], "Bin 0, Beam 1 Velocity is incorrect.");
            Assert.AreEqual(789, vel.Velocities[0, 2], "Bin 0, Beam 2 Velocity is incorrect.");
            Assert.AreEqual(147, vel.Velocities[0, 3], "Bin 0, Beam 3 Velocity is incorrect.");
            Assert.AreEqual(258, vel.Velocities[1, 0], "Bin 1, Beam 0 Velocity is incorrect.");
            Assert.AreEqual(369, vel.Velocities[1, 1], "Bin 1, Beam 1 Velocity is incorrect.");
            Assert.AreEqual(741, vel.Velocities[1, 2], "Bin 1, Beam 2 Velocity is incorrect.");
            Assert.AreEqual(852, vel.Velocities[1, 3], "Bin 1, Beam 3 Velocity is incorrect.");


        }

        #endregion

    }
}
