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
    /// Test the PD0 Percent Good.
    /// </summary>
    [TestFixture]
    public class Pd0PercentGoodTest
    {

        #region Encode

        /// <summary>
        /// Test encoding data.
        /// </summary>
        [Test]
        public void TestEncode()
        {
            Pd0PercentGood pg = new Pd0PercentGood();

            pg.PercentGood = new byte[5, 4];

            pg.PercentGood[0, 0] = 232;
            pg.PercentGood[0, 1] = 123;
            pg.PercentGood[0, 2] = 122;
            pg.PercentGood[0, 3] = 223;

            pg.PercentGood[4, 0] = 111;
            pg.PercentGood[4, 1] = 222;
            pg.PercentGood[4, 2] = 21;
            pg.PercentGood[4, 3] = 34;


            byte[] data = pg.Encode();

            // DS 0 Beam 0
            Assert.AreEqual(0xE8, data[2], "DS 0 Beam 0 Encode is incorrect.");

            // DS 0 Beam 1
            Assert.AreEqual(0x7B, data[3], "DS 0 Beam 1 Encode is incorrect.");

            // DS 0 Beam 2
            Assert.AreEqual(0x7A, data[4], "DS 0 Beam 2 Encode is incorrect.");

            // DS 0 Beam 3
            Assert.AreEqual(0xDF, data[5], "DS 0 Beam 3 Encode is incorrect.");



            // DS 4 Beam 0
            Assert.AreEqual(0x6F, data[(4 * 4) + 2 + 0], "DS 4 Beam 0 Encode is incorrect.");

            // DS 4 Beam 1
            Assert.AreEqual(0xDE, data[(4 * 4) + 2 + 1], "DS 4 Beam 1 Encode is incorrect.");

            // DS 4 Beam 2
            Assert.AreEqual(0x15, data[(4 * 4) + 2 + 2], "DS 4 Beam 2 Encode is incorrect.");

            // DS 4 Beam 3
            Assert.AreEqual(0x22, data[(4 * 4) + 2 + 3], "DS 4 Beam 3 Encode is incorrect.");
        }

        #endregion

        #region Decode

        /// <summary>
        /// Test decoding data.
        /// </summary>
        [Test]
        public void TestDecode()
        {
            Pd0PercentGood pg = new Pd0PercentGood();

            // 2 Byte Header
            // 4 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] data = new byte[22];

            data[0] = Pd0PercentGood.ID_LSB;
            data[1] = Pd0PercentGood.ID_MSB;
            data[2] = 0xE8;                                   // DS0 Beam 0 
            data[3] = 0x7B;                                   // DS0 Beam 1 
            data[4] = 0x7A;                                   // DS0 Beam 2 
            data[5] = 0xDF;                                   // DS0 Beam 3

            data[(4 * 4) + 2 + 0] = 0x6F;                     // DS4 Beam 0 
            data[(4 * 4) + 2 + 1] = 0xDE;                     // DS4 Beam 1 
            data[(4 * 4) + 2 + 2] = 0x15;                     // DS4 Beam 2 
            data[(4 * 4) + 2 + 3] = 0x22;                     // DS4 Beam 3 


            pg.Decode(data);

            Assert.AreEqual(5, pg.PercentGood.GetLength(0), "Number of Depth Cells is incorrect.");
            Assert.AreEqual(232, pg.PercentGood[0, 0], "DS 0 Beam 0 is incorrect.");
            Assert.AreEqual(123, pg.PercentGood[0, 1], "DS 0 Beam 1 is incorrect.");
            Assert.AreEqual(122, pg.PercentGood[0, 2], "DS 0 Beam 2 is incorrect.");
            Assert.AreEqual(223, pg.PercentGood[0, 3], "DS 0 Beam 3 is incorrect.");

            Assert.AreEqual(111, pg.PercentGood[4, 0], "DS 4 Beam 0 is incorrect.");
            Assert.AreEqual(222, pg.PercentGood[4, 1], "DS 4 Beam 1 is incorrect.");
            Assert.AreEqual(21, pg.PercentGood[4, 2], "DS 4 Beam 2 is incorrect.");
            Assert.AreEqual(34, pg.PercentGood[4, 3], "DS 4 Beam 3 is incorrect.");
        }

        #endregion

        #region GetPercentGood

        /// <summary>
        /// Test GetCorrelation() data.
        /// </summary>
        [Test]
        public void TestPercentGood()
        {
            Pd0PercentGood pg = new Pd0PercentGood();

            // 2 Byte Header
            // 4 Bytes per depth cell
            // 4 Beams per depth cell
            // 2 Bytes per beam
            byte[] data = new byte[22];

            data[0] = Pd0PercentGood.ID_LSB;
            data[1] = Pd0PercentGood.ID_MSB;
            data[2] = 0xE8;                                   // DS0 Beam 0 
            data[3] = 0x7B;                                   // DS0 Beam 1 
            data[4] = 0x7A;                                   // DS0 Beam 2 
            data[5] = 0xDF;                                   // DS0 Beam 3

            data[(4 * 4) + 2 + 0] = 0x6F;                     // DS4 Beam 0 
            data[(4 * 4) + 2 + 1] = 0xDE;                     // DS4 Beam 1 
            data[(4 * 4) + 2 + 2] = 0x15;                     // DS4 Beam 2 
            data[(4 * 4) + 2 + 3] = 0x22;                     // DS4 Beam 3 


            pg.Decode(data);

            Assert.AreEqual(5, pg.PercentGood.GetLength(0), "Number of Depth Cells is incorrect.");
            Assert.AreEqual(232, pg.GetPercentGood(0, 0), "DS 0 Beam 0 is incorrect.");
            Assert.AreEqual(123, pg.GetPercentGood(0, 1), "DS 0 Beam 1 is incorrect.");
            Assert.AreEqual(122, pg.GetPercentGood(0, 2), "DS 0 Beam 2 is incorrect.");
            Assert.AreEqual(223, pg.GetPercentGood(0, 3), "DS 0 Beam 3 is incorrect.");

            Assert.AreEqual(111, pg.GetPercentGood(4, 0), "DS 4 Beam 0 is incorrect.");
            Assert.AreEqual(222, pg.GetPercentGood(4, 1), "DS 4 Beam 1 is incorrect.");
            Assert.AreEqual(21, pg.GetPercentGood(4, 2), "DS 4 Beam 2 is incorrect.");
            Assert.AreEqual(34, pg.GetPercentGood(4, 3), "DS 4 Beam 3 is incorrect.");
        }

        #endregion

        /// <summary>
        /// Test decoding RTI data to PD0.
        /// </summary>
        [Test]
        public void DecodeRtiConstructorTest()
        {
            DataSet.GoodEarthDataSet ge = new DataSet.GoodEarthDataSet(DataSet.Ensemble.DATATYPE_INT,                           // Type of data stored (Float or Int)
                                                                                    30,                                             // Number of bins
                                                                                    4,                                              // Number of beams
                                                                                    DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                                                                    DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                                                                    DataSet.Ensemble.GoodEarthID);                  // Dataset ID

            ge.GoodEarthData[0, 0] = 2;
            ge.GoodEarthData[0, 1] = 2;
            ge.GoodEarthData[0, 2] = 1;
            ge.GoodEarthData[0, 3] = 2;
            ge.GoodEarthData[1, 0] = 2;
            ge.GoodEarthData[1, 1] = 2;
            ge.GoodEarthData[1, 2] = 2;
            ge.GoodEarthData[1, 3] = 1;

            Pd0PercentGood pg = new Pd0PercentGood(ge, 2);

            Assert.AreEqual(100, pg.PercentGood[0, 0], "Percent Good Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(50, pg.PercentGood[0, 1], "Percent Good Bin 0, Beam 1 is incorrect.");
            Assert.AreEqual(100, pg.PercentGood[0, 2], "Percent Good Bin 0, Beam 2 is incorrect.");
            Assert.AreEqual(100, pg.PercentGood[0, 3], "Percent Good Bin 0, Beam 3 is incorrect.");
            Assert.AreEqual(50, pg.PercentGood[1, 0], "Percent Good Bin 1, Beam 0 is incorrect.");
            Assert.AreEqual(100, pg.PercentGood[1, 1], "Percent Good Bin 1, Beam 1 is incorrect.");
            Assert.AreEqual(100, pg.PercentGood[1, 2], "Percent Good Bin 1, Beam 2 is incorrect.");
            Assert.AreEqual(100, pg.PercentGood[1, 3], "Percent Good Bin 1, Beam 3 is incorrect.");
        }
    }
}
