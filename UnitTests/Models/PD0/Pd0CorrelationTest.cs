///*
// * Copyright © 2011 
// * Rowe Technology Inc.
// * All rights reserved.
// * http://www.rowetechinc.com
// * 
// * Redistribution and use in source and binary forms, with or without
// * modification is NOT permitted.
// * 
// * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
// * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
// * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
// * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
// * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
// * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// * POSSIBILITY OF SUCH DAMAGE.
// * 
// * HISTORY
// * -----------------------------------------------------------------
// * Date            Initials    Version    Comments
// * -----------------------------------------------------------------
// * 03/12/2014      RC          2.21.4     Initial coding
// * 
// * 
// */

//namespace RTI
//{
//    using System;
//    using NUnit.Framework;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;

//    /// <summary>
//    /// Test the PD0 Correlation.
//    /// </summary>
//    [TestFixture]
//    public class Pd0CorrelationTest
//    {

//        #region Encode

//        /// <summary>
//        /// Test encoding data.
//        /// </summary>
//        [Test]
//        public void TestEncode()
//        {
//            Pd0Correlation corr = new Pd0Correlation();

//            corr.Correlation = new byte[5, 4];

//            corr.Correlation[0, 0] = 232;
//            corr.Correlation[0, 1] = 123;
//            corr.Correlation[0, 2] = 122;
//            corr.Correlation[0, 3] = 223;

//            corr.Correlation[4, 0] = 111;
//            corr.Correlation[4, 1] = 222;
//            corr.Correlation[4, 2] = 21;
//            corr.Correlation[4, 3] = 34;


//            byte[] data = corr.Encode();

//            // DS 0 Beam 0
//            Assert.AreEqual(0xE8, data[2], "DS 0 Beam 0 Encode is incorrect.");

//            // DS 0 Beam 1
//            Assert.AreEqual(0x7B, data[3], "DS 0 Beam 1 Encode is incorrect.");

//            // DS 0 Beam 2
//            Assert.AreEqual(0x7A, data[4], "DS 0 Beam 2 Encode is incorrect.");

//            // DS 0 Beam 3
//            Assert.AreEqual(0xDF, data[5], "DS 0 Beam 3 Encode is incorrect.");



//            // DS 4 Beam 0
//            Assert.AreEqual(0x6F, data[(4 * 4) + 2 + 0], "DS 4 Beam 0 Encode is incorrect.");

//            // DS 4 Beam 1
//            Assert.AreEqual(0xDE, data[(4 * 4) + 2 + 1], "DS 4 Beam 1 Encode is incorrect.");

//            // DS 4 Beam 2
//            Assert.AreEqual(0x15, data[(4 * 4) + 2 + 2], "DS 4 Beam 2 Encode is incorrect.");

//            // DS 4 Beam 3
//            Assert.AreEqual(0x22, data[(4 * 4) + 2 + 3], "DS 4 Beam 3 Encode is incorrect.");
//        }

//        #endregion

//        #region Decode

//        /// <summary>
//        /// Test decoding data.
//        /// </summary>
//        [Test]
//        public void TestDecode()
//        {
//            Pd0Correlation corr = new Pd0Correlation();

//            // 2 Byte Header
//            // 4 Bytes per depth cell
//            // 4 Beams per depth cell
//            // 2 Bytes per beam
//            byte[] data = new byte[22];

//            data[0] = Pd0Velocity.ID_LSB;
//            data[1] = Pd0Velocity.ID_MSB;
//            data[2] = 0xE8;                                   // DS0 Beam 0 
//            data[3] = 0x7B;                                   // DS0 Beam 1 
//            data[4] = 0x7A;                                   // DS0 Beam 2 
//            data[5] = 0xDF;                                   // DS0 Beam 3

//            data[(4 * 4) + 2 + 0] = 0x6F;                     // DS4 Beam 0 
//            data[(4 * 4) + 2 + 1] = 0xDE;                     // DS4 Beam 1 
//            data[(4 * 4) + 2 + 2] = 0x15;                     // DS4 Beam 2 
//            data[(4 * 4) + 2 + 3] = 0x22;                     // DS4 Beam 3 


//            corr.Decode(data);

//            Assert.AreEqual(5, corr.Correlation.GetLength(0), "Number of Depth Cells is incorrect.");
//            Assert.AreEqual(232, corr.Correlation[0, 0], "DS 0 Beam 0 is incorrect.");
//            Assert.AreEqual(123, corr.Correlation[0, 1], "DS 0 Beam 1 is incorrect.");
//            Assert.AreEqual(122, corr.Correlation[0, 2], "DS 0 Beam 2 is incorrect.");
//            Assert.AreEqual(223, corr.Correlation[0, 3], "DS 0 Beam 3 is incorrect.");

//            Assert.AreEqual(111, corr.Correlation[4, 0], "DS 4 Beam 0 is incorrect.");
//            Assert.AreEqual(222, corr.Correlation[4, 1], "DS 4 Beam 1 is incorrect.");
//            Assert.AreEqual(21, corr.Correlation[4, 2], "DS 4 Beam 2 is incorrect.");
//            Assert.AreEqual(34, corr.Correlation[4, 3], "DS 4 Beam 3 is incorrect.");
//        }

//        #endregion

//        #region GetCorrelation

//        /// <summary>
//        /// Test GetCorrelation() data.
//        /// </summary>
//        [Test]
//        public void TestGetCorrelation()
//        {
//            Pd0Correlation corr = new Pd0Correlation();

//            // 2 Byte Header
//            // 4 Bytes per depth cell
//            // 4 Beams per depth cell
//            // 2 Bytes per beam
//            byte[] data = new byte[22];

//            data[0] = Pd0Velocity.ID_LSB;
//            data[1] = Pd0Velocity.ID_MSB;
//            data[2] = 0xE8;                                   // DS0 Beam 0 
//            data[3] = 0x7B;                                   // DS0 Beam 1 
//            data[4] = 0x7A;                                   // DS0 Beam 2 
//            data[5] = 0xDF;                                   // DS0 Beam 3

//            data[(4 * 4) + 2 + 0] = 0x6F;                     // DS4 Beam 0 
//            data[(4 * 4) + 2 + 1] = 0xDE;                     // DS4 Beam 1 
//            data[(4 * 4) + 2 + 2] = 0x15;                     // DS4 Beam 2 
//            data[(4 * 4) + 2 + 3] = 0x22;                     // DS4 Beam 3 


//            corr.Decode(data);

//            Assert.AreEqual(5, corr.Correlation.GetLength(0), "Number of Depth Cells is incorrect.");
//            Assert.AreEqual(232, corr.GetCorrelation(0, 0), "DS 0 Beam 0 is incorrect.");
//            Assert.AreEqual(123, corr.GetCorrelation(0, 1), "DS 0 Beam 1 is incorrect.");
//            Assert.AreEqual(122, corr.GetCorrelation(0, 2), "DS 0 Beam 2 is incorrect.");
//            Assert.AreEqual(223, corr.GetCorrelation(0, 3), "DS 0 Beam 3 is incorrect.");

//            Assert.AreEqual(111, corr.GetCorrelation(4, 0), "DS 4 Beam 0 is incorrect.");
//            Assert.AreEqual(222, corr.GetCorrelation(4, 1), "DS 4 Beam 1 is incorrect.");
//            Assert.AreEqual(21, corr.GetCorrelation(4, 2), "DS 4 Beam 2 is incorrect.");
//            Assert.AreEqual(34, corr.GetCorrelation(4, 3), "DS 4 Beam 3 is incorrect.");
//        }

//        #endregion

//        #region Decode RTI

//        /// <summary>
//        /// Test decoding RTI data to PD0.
//        /// </summary>
//        [Test]
//        public void DecodeRtiTest()
//        {
//            Pd0Correlation corr = new Pd0Correlation(30);

//            DataSet.CorrelationDataSet rtiCorr = new DataSet.CorrelationDataSet(DataSet.Ensemble.DATATYPE_FLOAT,                    // Type of data stored (Float or Int)
//                                                                                    30,                                             // Number of bins
//                                                                                    4,                                              // Number of beams
//                                                                                    DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
//                                                                                    DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
//                                                                                    DataSet.Ensemble.CorrelationID);                // Dataset ID

//            rtiCorr.CorrelationData[0, 0] = 0.23f;
//            rtiCorr.CorrelationData[0, 1] = 0.34f;
//            rtiCorr.CorrelationData[0, 2] = 0.45f;
//            rtiCorr.CorrelationData[0, 3] = 0.56f;
//            rtiCorr.CorrelationData[1, 0] = 0.67f;
//            rtiCorr.CorrelationData[1, 1] = 0.78f;
//            rtiCorr.CorrelationData[1, 2] = 0.89f;
//            rtiCorr.CorrelationData[1, 3] = 0.910f;
//            rtiCorr.CorrelationData[2, 0] = 0.1011f;
//            rtiCorr.CorrelationData[2, 1] = 0.1112f;
//            rtiCorr.CorrelationData[2, 2] = 0.1213f;

//            corr.DecodeRtiEnsemble(rtiCorr, 1);

//            Assert.AreEqual(72, corr.Correlation[0, 0], "Correlation Bin 0, Beam 0 is incorrect.");
//            Assert.AreEqual(58, corr.Correlation[0, 1], "Correlation Bin 0, Beam 1 is incorrect.");
//            Assert.AreEqual(29, corr.Correlation[0, 2], "Correlation Bin 0, Beam 2 is incorrect.");
//            Assert.AreEqual(87, corr.Correlation[0, 3], "Correlation Bin 0, Beam 3 is incorrect.");
//            Assert.AreEqual(232, corr.Correlation[1, 0], "Correlation Bin 1, Beam 0 is incorrect.");
//            Assert.AreEqual(227, corr.Correlation[1, 1], "Correlation Bin 1, Beam 1 is incorrect.");
//            Assert.AreEqual(171, corr.Correlation[1, 2], "Correlation Bin 1, Beam 2 is incorrect.");
//            Assert.AreEqual(199, corr.Correlation[1, 3], "Correlation Bin 1, Beam 3 is incorrect.");
//        }

//        /// <summary>
//        /// Test decoding RTI data to PD0.
//        /// </summary>
//        [Test]
//        public void DecodeRtiConstructorTest()
//        {
//            DataSet.CorrelationDataSet rtiCorr = new DataSet.CorrelationDataSet(DataSet.Ensemble.DATATYPE_FLOAT,                    // Type of data stored (Float or Int)
//                                                                                    30,                                             // Number of bins
//                                                                                    4,                                              // Number of beams
//                                                                                    DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
//                                                                                    DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
//                                                                                    DataSet.Ensemble.CorrelationID);                // Dataset ID

//            rtiCorr.CorrelationData[0, 0] = 0.23f;
//            rtiCorr.CorrelationData[0, 1] = 0.34f;
//            rtiCorr.CorrelationData[0, 2] = 0.45f;
//            rtiCorr.CorrelationData[0, 3] = 0.56f;
//            rtiCorr.CorrelationData[1, 0] = 0.67f;
//            rtiCorr.CorrelationData[1, 1] = 0.78f;
//            rtiCorr.CorrelationData[1, 2] = 0.89f;
//            rtiCorr.CorrelationData[1, 3] = 0.910f;
//            rtiCorr.CorrelationData[2, 0] = 0.1011f;
//            rtiCorr.CorrelationData[2, 1] = 0.1112f;
//            rtiCorr.CorrelationData[2, 2] = 0.1213f;

//            Pd0Correlation corr = new Pd0Correlation(rtiCorr, 1);

//            Assert.AreEqual(72, corr.Correlation[0, 0], "Correlation Bin 0, Beam 0 is incorrect.");
//            Assert.AreEqual(58, corr.Correlation[0, 1], "Correlation Bin 0, Beam 1 is incorrect.");
//            Assert.AreEqual(29, corr.Correlation[0, 2], "Correlation Bin 0, Beam 2 is incorrect.");
//            Assert.AreEqual(87, corr.Correlation[0, 3], "Correlation Bin 0, Beam 3 is incorrect.");
//            Assert.AreEqual(232, corr.Correlation[1, 0], "Correlation Bin 1, Beam 0 is incorrect.");
//            Assert.AreEqual(227, corr.Correlation[1, 1], "Correlation Bin 1, Beam 1 is incorrect.");
//            Assert.AreEqual(171, corr.Correlation[1, 2], "Correlation Bin 1, Beam 2 is incorrect.");
//            Assert.AreEqual(199, corr.Correlation[1, 3], "Correlation Bin 1, Beam 3 is incorrect.");
//        }

//        #endregion
//    }
//}
