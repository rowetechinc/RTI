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
 * 03/07/2014      RC          2.21.4     Initial coding
 * 
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Test the PD0 Bottom Track.
    /// </summary>
    [TestFixture]
    public class Pd0BottomTrackTest
    {

        /// <summary>
        /// Test the constructor.
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            Assert.AreEqual(0, bt.BtPingsPerEnsemble, "Pings Per Ensemble is incorrect.");
            Assert.AreEqual(0, bt.BtDelayBeforeReacquire, "BtDelayBeforeReacquire is incorrect.");
            Assert.AreEqual(0, bt.BtCorrMagMin, "BtCorrMagMin is incorrect.");
            Assert.AreEqual(0, bt.BtEvalAmpMin, "BtEvalAmpMinis incorrect.");
            Assert.AreEqual(0, bt.BtPercentGoodMin, "BtPercentGoodMin is incorrect.");
            Assert.AreEqual(0, bt.BtMode, "BtMode is incorrect.");
            Assert.AreEqual(0, bt.BtErrVelMax, "BtErrVelMax is incorrect.");
            Assert.AreEqual(0, bt.Reserved13_16, "Reserved13_16 is incorrect.");
            Assert.AreEqual(0, bt.BtRangeLsbBeam0, "BtRangeLsbBeam0 is incorrect.");
            Assert.AreEqual(0, bt.BtRangeLsbBeam1, "BtRangeLsbBeam1 is incorrect.");
            Assert.AreEqual(0, bt.BtRangeLsbBeam2, "BtRangeLsbBeam2 is incorrect.");
            Assert.AreEqual(0, bt.BtRangeLsbBeam3, "BtRangeLsbBeam3 is incorrect.");
            Assert.AreEqual(0, bt.BtRangeMsbBeam0, "BtRangeMsbBeam0 is incorrect.");
            Assert.AreEqual(0, bt.BtRangeMsbBeam1, "BtRangeMsbBeam1 is incorrect.");
            Assert.AreEqual(0, bt.BtRangeMsbBeam2, "BtRangeMsbBeam2 is incorrect.");
            Assert.AreEqual(0, bt.BtRangeMsbBeam3, "BtRangeMsbBeam3 is incorrect.");
            Assert.AreEqual(0, bt.BtVelocityBeam0, "BtVelocityBeam0 is incorrect.");
            Assert.AreEqual(0, bt.BtVelocityBeam1, "BtVelocityBeam1 is incorrect.");
            Assert.AreEqual(0, bt.BtVelocityBeam2, "BtVelocityBeam2 is incorrect.");
            Assert.AreEqual(0, bt.BtVelocityBeam3, "BtVelocityBeam3 is incorrect.");
            Assert.AreEqual(0, bt.BtCorrelationMagnitudeBeam0, "BtCorrelationMagnitudeBeam0 is incorrect.");
            Assert.AreEqual(0, bt.BtCorrelationMagnitudeBeam1, "BtCorrelationMagnitudeBeam1 is incorrect.");
            Assert.AreEqual(0, bt.BtCorrelationMagnitudeBeam2, "BtCorrelationMagnitudeBeam2 is incorrect.");
            Assert.AreEqual(0, bt.BtCorrelationMagnitudeBeam3, "BtCorrelationMagnitudeBeam3 is incorrect.");
            Assert.AreEqual(0, bt.BtAmplitudeBeam0, "BtAmplitudeBeam0 is incorrect.");
            Assert.AreEqual(0, bt.BtAmplitudeBeam1, "BtAmplitudeBeam1 is incorrect.");
            Assert.AreEqual(0, bt.BtAmplitudeBeam2, "BtAmplitudeBeam2 is incorrect.");
            Assert.AreEqual(0, bt.BtAmplitudeBeam3, "BtAmplitudeBeam3 is incorrect.");
            Assert.AreEqual(0, bt.BtPercentGoodBeam0, "BtPercentGoodBeam0 is incorrect.");
            Assert.AreEqual(0, bt.BtPercentGoodBeam1, "BtPercentGoodBeam1 is incorrect.");
            Assert.AreEqual(0, bt.BtPercentGoodBeam2, "BtPercentGoodBeam2 is incorrect.");
            Assert.AreEqual(0, bt.BtPercentGoodBeam3, "BtPercentGoodBeam3 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerMin, "BtRefLayerMin is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerNear, "BtRefLayerNear is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerFar, "BtRefLayerFar is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerVelocityBeam0, "BtRefLayerVelocityBeam0 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerVelocityBeam1, "BtRefLayerVelocityBeam1 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerVelocityBeam2, "BtRefLayerVelocityBeam2 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerVelocityBeam3, "BtRefLayerVelocityBeam3 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerCorrBeam0, "BtRefLayerCorrBeam0 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerCorrBeam1, "BtRefLayerCorrBeam1 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerCorrBeam2, "BtRefLayerCorrBeam2 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerCorrBeam3, "BtRefLayerCorrBeam3 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerEchoIntensityBeam0, "BtRefLayerEchoIntensityBeam0 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerEchoIntensityBeam1, "BtRefLayerEchoIntensityBeam1 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerEchoIntensityBeam2, "BtRefLayerEchoIntensityBeam2 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerEchoIntensityBeam3, "BtRefLayerEchoIntensityBeam3 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerPercentGoodBeam0, "BtRefLayerPercentGoodBeam0 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerPercentGoodBeam1, "BtRefLayerPercentGoodBeam1 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerPercentGoodBeam2, "BtRefLayerPercentGoodBeam2 is incorrect.");
            Assert.AreEqual(0, bt.BtRefLayerPercentGoodBeam3, "BtRefLayerPercentGoodBeam3 is incorrect.");
            Assert.AreEqual(80, bt.BtMaxDepth, "BtMaxDepth is incorrect.");
            Assert.AreEqual(0, bt.BtRssiBeam0, "BtRssiBeam0 is incorrect.");
            Assert.AreEqual(0, bt.BtRssiBeam1, "BtRssiBeam1 is incorrect.");
            Assert.AreEqual(0, bt.BtRssiBeam2, "BtRssiBeam2 is incorrect.");
            Assert.AreEqual(0, bt.BtRssiBeam3, "BtRssiBeam3 is incorrect.");
            Assert.AreEqual(0, bt.BtGain, "Gain is incorrect.");
            Assert.AreEqual(0, bt.Reserved82_85, "Reserved82_85 is incorrect.");

        }


        #region Pings Per Ensemble

        #region Decode

        /// <summary>
        /// Test decoding the Pings Per Ensemble.
        /// </summary>
        [Test]
        public void TestPingsPerEnsembleDecode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[2] = 0xF3;
            data[3] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtPingsPerEnsemble, "Pings Per Ensemble Decode is incorrect.");

        }



        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Pings Per Ensemble.
        /// </summary>
        [Test]
        public void TestPingsPerEnsembleEncode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPingsPerEnsemble = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[2], "Pings Per Ensemble LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[3], "Pings Per Ensemble MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Pings Per Ensemble minimum.
        /// </summary>
        [Test]
        public void TestPingsPerEnsembleEncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPingsPerEnsemble = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[2], "Pings Per Ensemble LSB Min Encode is incorrect.");
            Assert.AreEqual(0x00, data[3], "Pings Per Ensemble MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Pings Per Ensemble maximum.
        /// </summary>
        [Test]
        public void TestPingsPerEnsembleEncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPingsPerEnsemble = 999;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xE7, data[2], "Pings Per Ensemble LSB Max Encode is incorrect.");
            Assert.AreEqual(0x03, data[3], "Pings Per Ensemble MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Delay Before Reacquire

        #region Decode

        /// <summary>
        /// Test decoding the Delay Before Reacquire.
        /// </summary>
        [Test]
        public void TestDelayBeforeReacquireDecode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[4] = 0xF3;
            data[5] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtDelayBeforeReacquire, "Delay Before Reacquire Decode is incorrect.");

        }



        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Delay Before Reacquire.
        /// </summary>
        [Test]
        public void TestDelayBeforeReacquireEncode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtDelayBeforeReacquire = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[4], "Delay Before Reacquire LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[5], "Delay Before Reacquire MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Delay Before Reacquire minimum.
        /// </summary>
        [Test]
        public void TestDelayBeforeReacquireEncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtDelayBeforeReacquire = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[4], "Delay Before Reacquire LSB Min Encode is incorrect.");
            Assert.AreEqual(0x00, data[5], "Delay Before Reacquire MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Delay Before Reacquire maximum.
        /// </summary>
        [Test]
        public void TestDelayBeforeReacquireEncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtDelayBeforeReacquire = 999;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xE7, data[4], "Delay Before Reacquire LSB Max Encode is incorrect.");
            Assert.AreEqual(0x03, data[5], "Delay Before Reacquire MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Correlation Magnitude Minimum

        #region Decode

        /// <summary>
        /// Test decoding the Correlation Magnitude Minimum.
        /// </summary>
        [Test]
        public void TestCorrMagMinDecode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[6] = 0xF3;
            bt.Decode(data);
            Assert.AreEqual(243, bt.BtCorrMagMin, "Correlation Magnitude Minimum Decode is incorrect.");

        }



        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Correlation Magnitude Minimum.
        /// </summary>
        [Test]
        public void TestCorrMagMinEncode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrMagMin = 243;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[6], "Correlation Magnitude Minimum Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Correlation Magnitude Minimum minimum.
        /// </summary>
        [Test]
        public void TestCorrMagMinEncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrMagMin = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[6], "Correlation Magnitude MinimumMin Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Correlation Magnitude Minimum maximum.
        /// </summary>
        [Test]
        public void TestCorrMagMinEncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrMagMin = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[6], "Correlation Magnitude Minimum Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Evaluation Amplitude Minimum

        #region Decode

        /// <summary>
        /// Test decoding the Evaluation Amplitude Minimum.
        /// </summary>
        [Test]
        public void TestEvalAmpMinDecode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[7] = 0xF3;
            bt.Decode(data);
            Assert.AreEqual(243, bt.BtEvalAmpMin, "Evaluation Amplitude Minimum Decode is incorrect.");

        }



        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Evaluation Amplitude Minimum.
        /// </summary>
        [Test]
        public void TestEvalAmpMinEncode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtEvalAmpMin = 243;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[7], "Evaluation Amplitude Minimum Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Evaluation Amplitude Minimum minimum.
        /// </summary>
        [Test]
        public void TestEvalAmpMinEncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtEvalAmpMin = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[7], "Evaluation Amplitude Minimum Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Evaluation Amplitude Minimum maximum.
        /// </summary>
        [Test]
        public void TestEvalAmpMinEncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtEvalAmpMin = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[7], "Evaluation Amplitude Minimum Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Percent Good Minimum

        #region Decode

        /// <summary>
        /// Test decoding the Percent Good Minimum.
        /// </summary>
        [Test]
        public void TestPercentGoodMinDecode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[8] = 0xF3;
            bt.Decode(data);
            Assert.AreEqual(243, bt.BtPercentGoodMin, "Percent Good Minimum Decode is incorrect.");

        }



        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Percent Good Minimum.
        /// </summary>
        [Test]
        public void TestPercentGoodMinEncode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodMin = 243;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[8], "Percent Good Minimum Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Percent Good Minimum minimum.
        /// </summary>
        [Test]
        public void TestPercentGoodMinEncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodMin = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[8], "Percent Good Minimum Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Percent Good Minimum maximum.
        /// </summary>
        [Test]
        public void TestPercentGoodMinEncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodMin = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[8], "Percent Good Minimum Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region BT Mode

        #region Decode

        /// <summary>
        /// Test decoding the BT Mode.
        /// </summary>
        [Test]
        public void TestBtModeDecode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[9] = 0xF3;
            bt.Decode(data);
            Assert.AreEqual(243, bt.BtMode, "BT Mode Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the BT Mode.
        /// </summary>
        [Test]
        public void TestBtModeEncode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtMode = 243;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[9], "BT Mode Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the BT Mode minimum.
        /// </summary>
        [Test]
        public void TestBtModeEncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtMode = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[9], "BT Mode Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the BT Mode maximum.
        /// </summary>
        [Test]
        public void TestBtModeEncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtMode = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[9], "BT Mode Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Error Velocity Max

        #region Decode

        /// <summary>
        /// Test decoding the Error Velocity Max.
        /// </summary>
        [Test]
        public void TestErrVelMaxDecode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[10] = 0xF3;
            data[11] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtErrVelMax, "Error Velocity Max Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Error Velocity Max.
        /// </summary>
        [Test]
        public void TestErrVelMaxEncode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtErrVelMax = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[10], "Error Velocity Max LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[11], "Error Velocity Max MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Error Velocity Max minimum.
        /// </summary>
        [Test]
        public void TestErrVelMaxEncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtErrVelMax = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[10], "Error Velocity Max LSB Min Encode is incorrect.");
            Assert.AreEqual(0x00, data[11], "Error Velocity Max MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Error Velocity Max maximum.
        /// </summary>
        [Test]
        public void TestErrVelMaxEncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtErrVelMax = 5000;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x88, data[10], "Error Velocity Max LSB Max Encode is incorrect.");
            Assert.AreEqual(0x13, data[11], "Error Velocity Max MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reserved 13-16

        #region Decode

        /// <summary>
        /// Test decoding the Reserved 13-16.
        /// </summary>
        [Test]
        public void TestReserved13_16Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[12] = 0x9B;
            data[13] = 0x0D;
            data[14] = 0x02;
            data[15] = 0x00;
            bt.Decode(data);
            Assert.AreEqual(134555, bt.Reserved13_16, "Reserved 13-16 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reserved 13-16.
        /// </summary>
        [Test]
        public void TestReserved13_16Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.Reserved13_16 = 134555;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x9B, data[12], "Reserved 13-16 Max LSB Encode is incorrect.");
            Assert.AreEqual(0x0D, data[13], "Reserved 13-16 Max     Encode is incorrect.");
            Assert.AreEqual(0x02, data[14], "Reserved 13-16 Max     Encode is incorrect.");
            Assert.AreEqual(0x00, data[15], "Reserved 13-16 Max MSB Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Range LSB

        #region Range LSB Beam 0

        #region Decode

        /// <summary>
        /// Test decoding the Range LSB Beam 0.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam0Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[16] = 0xF3;
            data[17] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtRangeLsbBeam0, "Range LSB Beam 0 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Range LSB Beam 0.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam0Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeLsbBeam0 = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[16], "Range LSB Beam 0 LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[17], "Range LSB Beam 0 MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range LSB Beam 0 minimum.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam0EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeLsbBeam0 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[16], "Range LSB Beam 0 LSB Min Encode is incorrect.");
            Assert.AreEqual(0x00, data[17], "Range LSB Beam 0 MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range LSB Beam 0 maximum.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam0EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeLsbBeam0 = 65535;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[16], "Range LSB Beam 0 LSB Max Encode is incorrect.");
            Assert.AreEqual(0xFF, data[17], "Range LSB Beam 0 MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Range LSB Beam 1

        #region Decode

        /// <summary>
        /// Test decoding the Range LSB Beam 1.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam1Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[18] = 0xF3;
            data[19] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtRangeLsbBeam1, "Range LSB Beam 1 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Range LSB Beam 1.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam1Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeLsbBeam1 = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[18], "Range LSB Beam 1 LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[19], "Range LSB Beam 1 MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range LSB Beam 1 minimum.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam1EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeLsbBeam1 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[18], "Range LSB Beam 1 LSB Min Encode is incorrect.");
            Assert.AreEqual(0x00, data[19], "Range LSB Beam 1 MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range LSB Beam 1 maximum.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam1EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeLsbBeam1 = 65535;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[18], "Range LSB Beam 1 LSB Max Encode is incorrect.");
            Assert.AreEqual(0xFF, data[19], "Range LSB Beam 1 MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Range LSB Beam 2

        #region Decode

        /// <summary>
        /// Test decoding the Range LSB Beam 2.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam2Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[20] = 0xF3;
            data[21] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtRangeLsbBeam2, "Range LSB Beam 2 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Range LSB Beam 2.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam2Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeLsbBeam2 = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[20], "Range LSB Beam 2 LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[21], "Range LSB Beam 2 MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range LSB Beam 2 minimum.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam2EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeLsbBeam2 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[20], "Range LSB Beam 2 LSB Min Encode is incorrect.");
            Assert.AreEqual(0x00, data[21], "Range LSB Beam 2 MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range LSB Beam 2 maximum.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam2EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeLsbBeam2 = 65535;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[20], "Range LSB Beam 2 LSB Max Encode is incorrect.");
            Assert.AreEqual(0xFF, data[21], "Range LSB Beam 2 MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Range LSB Beam 3

        #region Decode

        /// <summary>
        /// Test decoding the Range LSB Beam 3.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam3Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[22] = 0xF3;
            data[23] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtRangeLsbBeam3, "Range LSB Beam 3 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Range LSB Beam 3.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam3Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeLsbBeam3 = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[22], "Range LSB Beam 3 LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[23], "Range LSB Beam 3 MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range LSB Beam 3 minimum.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam3EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeLsbBeam3 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[22], "Range LSB Beam 3 LSB Min Encode is incorrect.");
            Assert.AreEqual(0x00, data[23], "Range LSB Beam 3 MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range LSB Beam 3 maximum.
        /// </summary>
        [Test]
        public void TestRangeLsbBeam3EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeLsbBeam3 = 65535;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[22], "Range LSB Beam 3 LSB Max Encode is incorrect.");
            Assert.AreEqual(0xFF, data[23], "Range LSB Beam 3 MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #endregion

        #region Velocity

        #region Velocity Beam 0

        #region Decode

        /// <summary>
        /// Test decoding the Velocity Beam 0.
        /// </summary>
        [Test]
        public void TestVelocityBeam0Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[24] = 0xF3;
            data[25] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtVelocityBeam0, "Velocity Beam 0 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Velocity Beam 0.
        /// </summary>
        [Test]
        public void TestVelocityBeam0Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtVelocityBeam0 = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[24], "Velocity Beam 0 LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[25], "Velocity Beam 0 MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Velocity Beam 0 minimum.
        /// </summary>
        [Test]
        public void TestVelocityBeam0EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtVelocityBeam0 = -32768;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[24], "Velocity Beam 0 LSB Min Encode is incorrect.");
            Assert.AreEqual(0x80, data[25], "Velocity Beam 0 MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Velocity Beam 0 maximum.
        /// </summary>
        [Test]
        public void TestVelocityBeam0EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtVelocityBeam0 = 32767;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[24], "Velocity Beam 0 LSB Max Encode is incorrect.");
            Assert.AreEqual(0x7F, data[25], "Velocity Beam 0 MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Velocity Beam 1

        #region Decode

        /// <summary>
        /// Test decoding the Velocity Beam 1.
        /// </summary>
        [Test]
        public void TestVelocityBeam1Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[26] = 0xF3;
            data[27] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtVelocityBeam1, "Velocity Beam 1 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Velocity Beam 1.
        /// </summary>
        [Test]
        public void TestVelocityBeam1Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtVelocityBeam1 = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[26], "Velocity Beam 1 LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[27], "Velocity Beam 1 MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Velocity Beam 1 minimum.
        /// </summary>
        [Test]
        public void TestVelocityBeam1EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtVelocityBeam1 = -32768;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[26], "Velocity Beam 1 LSB Min Encode is incorrect.");
            Assert.AreEqual(0x80, data[27], "Velocity Beam 1 MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Velocity Beam 1 maximum.
        /// </summary>
        [Test]
        public void TestVelocityBeam1EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtVelocityBeam1 = 32767;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[26], "Velocity Beam 1 LSB Max Encode is incorrect.");
            Assert.AreEqual(0x7F, data[27], "Velocity Beam 1 MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Velocity Beam 2

        #region Decode

        /// <summary>
        /// Test decoding the Velocity Beam 2.
        /// </summary>
        [Test]
        public void TestVelocityBeam2Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[28] = 0xF3;
            data[29] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtVelocityBeam2, "Velocity Beam 2 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Velocity Beam 2.
        /// </summary>
        [Test]
        public void TestVelocityBeam2Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtVelocityBeam2 = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[28], "Velocity Beam 2 LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[29], "Velocity Beam 2 MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Velocity Beam 2 minimum.
        /// </summary>
        [Test]
        public void TestVelocityBeam2EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtVelocityBeam2 = -32768;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[28], "Velocity Beam 2 LSB Min Encode is incorrect.");
            Assert.AreEqual(0x80, data[29], "Velocity Beam 2 MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Velocity Beam 2 maximum.
        /// </summary>
        [Test]
        public void TestVelocityBeam2EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtVelocityBeam2 = 32767;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[28], "Velocity Beam 2 LSB Max Encode is incorrect.");
            Assert.AreEqual(0x7F, data[29], "Velocity Beam 2 MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Velocity Beam 3

        #region Decode

        /// <summary>
        /// Test decoding the Velocity Beam 3.
        /// </summary>
        [Test]
        public void TestVelocityBeam3Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[30] = 0xF3;
            data[31] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtVelocityBeam3, "Velocity Beam 3 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Velocity Beam 3.
        /// </summary>
        [Test]
        public void TestVelocityBeam3Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtVelocityBeam3 = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[30], "Velocity Beam 3 LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[31], "Velocity Beam 3 MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Velocity Beam 3 minimum.
        /// </summary>
        [Test]
        public void TestVelocityBeam3EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtVelocityBeam3 = -32768;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[30], "Velocity Beam 3 LSB Min Encode is incorrect.");
            Assert.AreEqual(0x80, data[31], "Velocity Beam 3 MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Velocity Beam 3 maximum.
        /// </summary>
        [Test]
        public void TestVelocityBeam3EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtVelocityBeam3 = 32767;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[30], "Velocity Beam 3 LSB Max Encode is incorrect.");
            Assert.AreEqual(0x7F, data[31], "Velocity Beam 3 MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #endregion

        #region Correlation

        #region Correlation Beam 0

        #region Decode

        /// <summary>
        /// Test decoding the Correlation Beam 0.
        /// </summary>
        [Test]
        public void TestCorrelationBeam0Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[32] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtCorrelationMagnitudeBeam0, "Correlation Beam 0 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Correlation Beam 0.
        /// </summary>
        [Test]
        public void TestCorrelationBeam0Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrelationMagnitudeBeam0 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[32], "Correlation Beam 0 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Correlation Beam 0 minimum.
        /// </summary>
        [Test]
        public void TestCorrelationBeam0EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrelationMagnitudeBeam0 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[32], "Correlation Beam 0 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Correlation Beam 0 maximum.
        /// </summary>
        [Test]
        public void TestCorrelationBeam0EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrelationMagnitudeBeam0 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[32], "Correlation Beam 0 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Correlation Beam 1

        #region Decode

        /// <summary>
        /// Test decoding the Correlation Beam 1.
        /// </summary>
        [Test]
        public void TestCorrelationBeam1Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[33] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtCorrelationMagnitudeBeam1, "Correlation Beam 1 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Correlation Beam 1.
        /// </summary>
        [Test]
        public void TestCorrelationBeam1Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrelationMagnitudeBeam1 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[33], "Correlation Beam 1 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Correlation Beam 1 minimum.
        /// </summary>
        [Test]
        public void TestCorrelationBeam1EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrelationMagnitudeBeam1 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[33], "Correlation Beam 1 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Correlation Beam 1 maximum.
        /// </summary>
        [Test]
        public void TestCorrelationBeam1EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrelationMagnitudeBeam1 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[33], "Correlation Beam 1 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Correlation Beam 2

        #region Decode

        /// <summary>
        /// Test decoding the Correlation Beam 2.
        /// </summary>
        [Test]
        public void TestCorrelationBeam2Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[34] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtCorrelationMagnitudeBeam2, "Correlation Beam 2 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Correlation Beam 2.
        /// </summary>
        [Test]
        public void TestCorrelationBeam2Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrelationMagnitudeBeam2 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[34], "Correlation Beam 2 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Correlation Beam 2 minimum.
        /// </summary>
        [Test]
        public void TestCorrelationBeam2EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrelationMagnitudeBeam2 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[34], "Correlation Beam 2 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Correlation Beam 2 maximum.
        /// </summary>
        [Test]
        public void TestCorrelationBeam2EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrelationMagnitudeBeam2 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[34], "Correlation Beam 2 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Correlation Beam 3

        #region Decode

        /// <summary>
        /// Test decoding the Correlation Beam 3.
        /// </summary>
        [Test]
        public void TestCorrelationBeam3Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[35] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtCorrelationMagnitudeBeam3, "Correlation Beam 3 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Correlation Beam 3.
        /// </summary>
        [Test]
        public void TestCorrelationBeam3Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrelationMagnitudeBeam3 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[35], "Correlation Beam 3 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Correlation Beam 3 minimum.
        /// </summary>
        [Test]
        public void TestCorrelationBeam3EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrelationMagnitudeBeam3 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[35], "Correlation Beam 3 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Correlation Beam 3 maximum.
        /// </summary>
        [Test]
        public void TestCorrelationBeam3EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtCorrelationMagnitudeBeam3 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[35], "Correlation Beam 3 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #endregion

        #region Amplitude

        #region Amplitude Beam 0

        #region Decode

        /// <summary>
        /// Test decoding the Amplitude Beam 0.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam0Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[36] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtAmplitudeBeam0, "Amplitude Beam 0 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Amplitude Beam 0.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam0Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtAmplitudeBeam0 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[36], "Amplitude Beam 0 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Amplitude Beam 0 minimum.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam0EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtAmplitudeBeam0 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[36], "Amplitude Beam 0 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Amplitude Beam 0 maximum.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam0EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtAmplitudeBeam0 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[36], "Amplitude Beam 0 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Amplitude Beam 1

        #region Decode

        /// <summary>
        /// Test decoding the Amplitude Beam 1.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam1Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[37] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtAmplitudeBeam1, "Amplitude Beam 1 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Amplitude Beam 1.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam1Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtAmplitudeBeam1 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[37], "Amplitude Beam 1 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Amplitude Beam 1 minimum.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam1EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtAmplitudeBeam1 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[37], "Amplitude Beam 1 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Amplitude Beam 1 maximum.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam1EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtAmplitudeBeam1 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[37], "Amplitude Beam 1 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Amplitude Beam 2

        #region Decode

        /// <summary>
        /// Test decoding the Amplitude Beam 2.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam2Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[38] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtAmplitudeBeam2, "Amplitude Beam 2 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Amplitude Beam 2.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam2Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtAmplitudeBeam2 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[38], "Amplitude Beam 2 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Amplitude Beam 2 minimum.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam2EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtAmplitudeBeam2 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[38], "Amplitude Beam 2 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Amplitude Beam 2 maximum.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam2EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtAmplitudeBeam2 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[38], "Amplitude Beam 2 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Amplitude Beam 3

        #region Decode

        /// <summary>
        /// Test decoding the Amplitude Beam 3.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam3Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[39] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtAmplitudeBeam3, "Amplitude Beam 3 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Amplitude Beam 3.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam3Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtAmplitudeBeam3 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[39], "Amplitude Beam 3 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Amplitude Beam 3 minimum.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam3EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtAmplitudeBeam3 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[39], "Amplitude Beam 3 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Amplitude Beam 3 maximum.
        /// </summary>
        [Test]
        public void TestAmplitudeBeam3EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtAmplitudeBeam3 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[39], "Amplitude Beam 3 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #endregion

        #region Percent Good

        #region Percent Good Beam 0

        #region Decode

        /// <summary>
        /// Test decoding the Percent Good Beam 0.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam0Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[40] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtPercentGoodBeam0, "Percent Good Beam 0 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Percent Good Beam 0.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam0Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodBeam0 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[40], "Percent Good Beam 0 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Percent Good Beam 0 minimum.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam0EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodBeam0 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[40], "Percent Good Beam 0 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Percent Good Beam 0 maximum.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam0EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodBeam0 = 100;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x64, data[40], "Percent Good Beam 0 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Percent Good Beam 1

        #region Decode

        /// <summary>
        /// Test decoding the Percent Good Beam 1.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam1Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[41] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtPercentGoodBeam1, "Percent Good Beam 1 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Percent Good Beam 1.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam1Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodBeam1 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[41], "Percent Good Beam 1 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Percent Good Beam 1 minimum.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam1EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodBeam1 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[41], "Percent Good Beam 1 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Percent Good Beam 1 maximum.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam1EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodBeam1 = 100;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x64, data[41], "Percent Good Beam 1 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Percent Good Beam 2

        #region Decode

        /// <summary>
        /// Test decoding the Percent Good Beam 2.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam2Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[42] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtPercentGoodBeam2, "Percent Good Beam 2 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Percent Good Beam 2.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam2Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodBeam2 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[42], "Percent Good Beam 2 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Percent Good Beam 2 minimum.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam2EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodBeam2 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[42], "Percent Good Beam 2 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Percent Good Beam 2 maximum.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam2EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodBeam2 = 100;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x64, data[42], "Percent Good Beam 2 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Percent Good Beam 3

        #region Decode

        /// <summary>
        /// Test decoding the Percent Good Beam 3.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam3Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[43] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtPercentGoodBeam3, "Percent Good Beam 3 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Percent Good Beam 3.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam3Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodBeam3 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[43], "Percent Good Beam 3 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Percent Good Beam 3 minimum.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam3EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodBeam3 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[43], "Percent Good Beam 3 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Percent Good Beam 3 maximum.
        /// </summary>
        [Test]
        public void TestPercentGoodBeam3EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPercentGoodBeam3 = 100;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x64, data[43], "Percent Good Beam 3 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #endregion

        #region Reference Layer Minimum

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Minimum.
        /// </summary>
        [Test]
        public void TestRefLayerMinDecode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[44] = 0xF3;
            data[45] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtRefLayerMin, "Reference Layer Minimum Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Minimum.
        /// </summary>
        [Test]
        public void TestRefLayerMinEncode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerMin = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[44], "Reference Layer Minimum LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[45], "Reference Layer Minimum MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Minimum minimum.
        /// </summary>
        [Test]
        public void TestRefLayerMinEncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerMin = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[44], "Reference Layer Minimum LSB Min Encode is incorrect.");
            Assert.AreEqual(0x00, data[45], "Reference Layer Minimum MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Minimum maximum.
        /// </summary>
        [Test]
        public void TestRefLayerMinEncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerMin = 999;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xE7, data[44], "Reference Layer Minimum LSB Max Encode is incorrect.");
            Assert.AreEqual(0x03, data[45], "Reference Layer Minimum MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Near

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Near.
        /// </summary>
        [Test]
        public void TestRefLayerNearDecode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[46] = 0xF3;
            data[47] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtRefLayerNear, "Reference Layer Near Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Near.
        /// </summary>
        [Test]
        public void TestRefLayerNearEncode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerNear = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[46], "Reference Layer Near LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[47], "Reference Layer Near MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Near minimum.
        /// </summary>
        [Test]
        public void TestRefLayerMinEncodeNear()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerNear = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[46], "Reference Layer Near LSB Min Encode is incorrect.");
            Assert.AreEqual(0x00, data[47], "Reference Layer Near MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Near maximum.
        /// </summary>
        [Test]
        public void TestRefLayerNearEncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerNear = 9999;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x0F, data[46], "Reference Layer Near LSB Max Encode is incorrect.");
            Assert.AreEqual(0x27, data[47], "Reference Layer Near MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Far

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Far.
        /// </summary>
        [Test]
        public void TestRefLayerFarDecode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[48] = 0xF3;
            data[49] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtRefLayerFar, "Reference Layer Far Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Far.
        /// </summary>
        [Test]
        public void TestRefLayerFarEncode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerFar = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[48], "Reference Layer Far LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[49], "Reference Layer Far MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Far minimum.
        /// </summary>
        [Test]
        public void TestRefLayerMinEncodeFar()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerFar = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[48], "Reference Layer Far LSB Min Encode is incorrect.");
            Assert.AreEqual(0x00, data[49], "Reference Layer Far MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Far maximum.
        /// </summary>
        [Test]
        public void TestRefLayerFarEncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerFar = 9999;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x0F, data[48], "Reference Layer Far LSB Max Encode is incorrect.");
            Assert.AreEqual(0x27, data[49], "Reference Layer Far MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Velocity

        #region Reference Layer Velocity Beam 0

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Velocity Beam 0.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam0Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[50] = 0xF3;
            data[51] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtRefLayerVelocityBeam0, "Reference Layer Velocity Beam 0 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Velocity Beam 0.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam0Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerVelocityBeam0 = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[50], "Reference Layer Velocity Beam 0 LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[51], "Reference Layer Velocity Beam 0 MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Velocity Beam 0 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam0EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerVelocityBeam0 = -32768;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[50], "Reference Layer Velocity Beam 0 LSB Min Encode is incorrect.");
            Assert.AreEqual(0x80, data[51], "Reference Layer Velocity Beam 0 MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Velocity Beam 0 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam0EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerVelocityBeam0 = 32767;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[50], "Reference Layer Velocity Beam 0 LSB Max Encode is incorrect.");
            Assert.AreEqual(0x7F, data[51], "Reference Layer Velocity Beam 0 MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Velocity Beam 1

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Velocity Beam 1.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam1Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[52] = 0xF3;
            data[53] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtRefLayerVelocityBeam1, "Reference Layer Velocity Beam 1 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Velocity Beam 1.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam1Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerVelocityBeam1 = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[52], "Reference Layer Velocity Beam 1 LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[53], "Reference Layer Velocity Beam 1 MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Velocity Beam 1 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam1EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerVelocityBeam1 = -32768;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[52], "Reference Layer Velocity Beam 1 LSB Min Encode is incorrect.");
            Assert.AreEqual(0x80, data[53], "Reference Layer Velocity Beam 1 MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Velocity Beam 1 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam1EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerVelocityBeam1 = 32767;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[52], "Reference Layer Velocity Beam 1 LSB Max Encode is incorrect.");
            Assert.AreEqual(0x7F, data[53], "Reference Layer Velocity Beam 1 MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Velocity Beam 2

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Velocity Beam 2.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam2Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[54] = 0xF3;
            data[55] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtRefLayerVelocityBeam2, "Reference Layer Velocity Beam 2 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Velocity Beam 2.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam2Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerVelocityBeam2 = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[54], "Reference Layer Velocity Beam 2 LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[55], "Reference Layer Velocity Beam 2 MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Velocity Beam 2 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam2EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerVelocityBeam2 = -32768;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[54], "Reference Layer Velocity Beam 2 LSB Min Encode is incorrect.");
            Assert.AreEqual(0x80, data[55], "Reference Layer Velocity Beam 2 MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Velocity Beam 2 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam2EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerVelocityBeam2 = 32767;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[54], "Reference Layer Velocity Beam 2 LSB Max Encode is incorrect.");
            Assert.AreEqual(0x7F, data[55], "Reference Layer Velocity Beam 2 MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Velocity Beam 3

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Velocity Beam 3.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam3Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[56] = 0xF3;
            data[57] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtRefLayerVelocityBeam3, "Reference Layer Velocity Beam 3 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Velocity Beam 3.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam3Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerVelocityBeam3 = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[56], "Reference Layer Velocity Beam 3 LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[57], "Reference Layer Velocity Beam 3 MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Velocity Beam 3 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam3EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerVelocityBeam3 = -32768;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[56], "Reference Layer Velocity Beam 3 LSB Min Encode is incorrect.");
            Assert.AreEqual(0x80, data[57], "Reference Layer Velocity Beam 3 MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Velocity Beam 3 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerVelocityBeam3EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerVelocityBeam3 = 32767;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[56], "Reference Layer Velocity Beam 3 LSB Max Encode is incorrect.");
            Assert.AreEqual(0x7F, data[57], "Reference Layer Velocity Beam 3 MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #endregion

        #region Reference Layer Correlation

        #region Reference Layer Correlation Beam 0

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Correlation Beam 0.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam0Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[58] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRefLayerCorrBeam0, "Reference Layer Correlation Beam 0 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Correlation Beam 0.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam0Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerCorrBeam0 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[58], "Reference Layer Correlation Beam 0 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Correlation Beam 0 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam0EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerCorrBeam0 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[58], "Reference Layer Correlation Beam 0 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Correlation Beam 0 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam0EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerCorrBeam0 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[58], "Reference Layer Correlation Beam 0 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Correlation Beam 1

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Correlation Beam 1.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam1Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[59] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRefLayerCorrBeam1, "Reference Layer Correlation Beam 1 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Correlation Beam 1.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam1Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerCorrBeam1 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[59], "Reference Layer Correlation Beam 1 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Correlation Beam 1 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam1EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerCorrBeam1 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[59], "Reference Layer Correlation Beam 1 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Correlation Beam 1 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam1EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerCorrBeam1 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[59], "Reference Layer Correlation Beam 1 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Correlation Beam 2

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Correlation Beam 2.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam2Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[60] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRefLayerCorrBeam2, "Reference Layer Correlation Beam 2 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Correlation Beam 2.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam2Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerCorrBeam2 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[60], "Reference Layer Correlation Beam 2 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Correlation Beam 2 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam2EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerCorrBeam2 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[60], "Reference Layer Correlation Beam 2 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Correlation Beam 2 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam2EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerCorrBeam2 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[60], "Reference Layer Correlation Beam 2 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Correlation Beam 3

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Correlation Beam 3.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam3Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[61] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRefLayerCorrBeam3, "Reference Layer Correlation Beam 3 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Correlation Beam 3.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam3Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerCorrBeam3 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[61], "Reference Layer Correlation Beam 3 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Correlation Beam 3 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam3EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerCorrBeam3 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[61], "Reference Layer Correlation Beam 3 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Correlation Beam 3 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerCorrelationBeam3EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerCorrBeam3 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[61], "Reference Layer Correlation Beam 3 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #endregion

        #region Reference Layer Echo Intensity

        #region Reference Layer Echo Intensity Beam 0

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Echo Intensity Beam 0.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam0Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[62] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRefLayerEchoIntensityBeam0, "Reference Layer Echo Intensity Beam 0 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Echo Intensity Beam 0.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam0Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerEchoIntensityBeam0 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[62], "Reference Layer Echo Intensity Beam 0 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Echo Intensity Beam 0 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam0EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerEchoIntensityBeam0 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[62], "Reference Layer Echo Intensity Beam 0 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Echo Intensity Beam 0 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam0EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerEchoIntensityBeam0 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[62], "Reference Layer Echo Intensity Beam 0 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Echo Intensity Beam 1

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Echo Intensity Beam 1.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam1Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[63] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRefLayerEchoIntensityBeam1, "Reference Layer Echo Intensity Beam 1 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Echo Intensity Beam 1.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam1Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerEchoIntensityBeam1 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[63], "Reference Layer Echo Intensity Beam 1 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Echo Intensity Beam 1 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam1EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerEchoIntensityBeam1 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[63], "Reference Layer Echo Intensity Beam 1 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Echo Intensity Beam 1 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam1EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerEchoIntensityBeam1 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[63], "Reference Layer Echo Intensity Beam 1 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Echo Intensity Beam 2

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Correlation Beam 2.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam2Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[64] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRefLayerEchoIntensityBeam2, "Reference Layer Echo Intensity Beam 2 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Echo Intensity Beam 2.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam2Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerEchoIntensityBeam2 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[64], "Reference Layer Echo Intensity Beam 2 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Echo Intensity Beam 2 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam2EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerEchoIntensityBeam2 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[64], "Reference Layer Echo Intensity Beam 2 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Echo Intensity Beam 2 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam2EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerEchoIntensityBeam2 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[64], "Reference Layer Echo Intensity Beam 2 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Echo Intensity Beam 3

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Echo Intensity Beam 3.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam3Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[65] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRefLayerEchoIntensityBeam3, "Reference Layer Echo Intensity Beam 3 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Echo Intensity Beam 3.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam3Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerEchoIntensityBeam3 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[65], "Reference Layer Echo Intensity Beam 3 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Echo Intensity Beam 3 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam3EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerEchoIntensityBeam3 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[65], "Reference Layer Echo Intensity Beam 3 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Echo Intensity Beam 3 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerEchoIntensityBeam3EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerEchoIntensityBeam3 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[65], "Reference Layer Echo Intensity Beam 3 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #endregion

        #region Reference Layer Percent Good

        #region Reference Layer Percent Good Beam 0

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Percent Good Beam 0.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam0Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[66] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRefLayerPercentGoodBeam0, "Reference Layer Percent Good Beam 0 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Percent Good Beam 0.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam0Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerPercentGoodBeam0 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[66], "Reference Layer Percent Good Beam 0 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Percent Good Beam 0 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam0EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerPercentGoodBeam0 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[66], "Reference Layer Percent Good Beam 0 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Percent Good Beam 0 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam0EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerPercentGoodBeam0 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[66], "Reference Layer Percent Good Beam 0 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Percent Good Beam 1

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Percent Good Beam 1.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam1Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[67] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRefLayerPercentGoodBeam1, "Reference Layer Percent Good Beam 1 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Percent Good Beam 1.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam1Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerPercentGoodBeam1 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[67], "Reference Layer Percent Good Beam 1 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Percent Good Beam 1 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam1EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerPercentGoodBeam1 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[67], "Reference Layer Percent Good Beam 1 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Percent Good Beam 1 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam1EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerPercentGoodBeam1 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[67], "Reference Layer Percent Good Beam 1 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Percent Good Beam 2

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Percent Good Beam 2.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam2Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[68] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRefLayerPercentGoodBeam2, "Reference Layer Percent Good Beam 2 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Percent Good Beam 2.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam2Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerPercentGoodBeam2 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[68], "Reference Layer Percent Good Beam 2 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Percent Good Beam 2 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam2EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerPercentGoodBeam2 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[68], "Reference Layer Percent Good Beam 2 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Percent Good Beam 2 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam2EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerPercentGoodBeam2 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[68], "Reference Layer Percent Good Beam 2 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Reference Layer Percent Good Beam 3

        #region Decode

        /// <summary>
        /// Test decoding the Reference Layer Percent Good Beam 3.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam3Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[69] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRefLayerPercentGoodBeam3, "Reference Layer Percent Good Beam 3 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reference Layer Percent Good Beam 3.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam3Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerPercentGoodBeam3 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[69], "Reference Layer Percent Good Beam 3 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Reference Layer Percent Good Beam 3 minimum.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam3EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerPercentGoodBeam3 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[69], "Reference Layer Percent Good Beam 3 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Percent Good Beam 3 maximum.
        /// </summary>
        [Test]
        public void TestRefLayerPercentGoodBeam3EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRefLayerPercentGoodBeam3 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[69], "Reference Layer Percent Good Beam 3 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #endregion

        #region Max Depth

        #region Decode

        /// <summary>
        /// Test decoding the Max Depth.
        /// </summary>
        [Test]
        public void TestMaxDepthDecode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[2] = 0xF3;
            data[3] = 0x02;
            bt.Decode(data);
            Assert.AreEqual(755, bt.BtPingsPerEnsemble, "Max Depth Decode is incorrect.");

        }



        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Max Depth.
        /// </summary>
        [Test]
        public void TestMaxDepthEncode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPingsPerEnsemble = 755;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[2], "Max Depth LSB Encode is incorrect.");
            Assert.AreEqual(0x02, data[3], "Max Depth MSB Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Max Depth minimum.
        /// </summary>
        [Test]
        public void TestMaxDepthEncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPingsPerEnsemble = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[2], "Max Depth LSB Min Encode is incorrect.");
            Assert.AreEqual(0x00, data[3], "Max Depth MSB Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Max Depth maximum.
        /// </summary>
        [Test]
        public void TestMaxDepthEncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtPingsPerEnsemble = 9999;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x0F, data[2], "Max Depth LSB Max Encode is incorrect.");
            Assert.AreEqual(0x27, data[3], "Max Depth MSB Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region RSSI

        #region RSSI Beam 0

        #region Decode

        /// <summary>
        /// Test decoding the RSSI Beam 0.
        /// </summary>
        [Test]
        public void TestRSSIBeam0Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[72] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76 * 0.45f, bt.BtRssiBeam0, "RSSI Beam 0 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the RSSI Beam 0.
        /// </summary>
        [Test]
        public void TestRSSIBeam0Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRssiBeam0 = 76 * 0.45f;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[72], "RSSI Beam 0 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the RSSI Beam 0 minimum.
        /// </summary>
        [Test]
        public void TestRSSIBeam0EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRssiBeam0 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[72], "RSSI Beam 0 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the RSSI Beam 0 maximum.
        /// </summary>
        [Test]
        public void TestRSSIBeam0EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRssiBeam0 = 255 * 0.45f;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[72], "RSSI Beam 0 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region RSSI Beam 1

        #region Decode

        /// <summary>
        /// Test decoding the RSSI Beam 1.
        /// </summary>
        [Test]
        public void TestRSSIBeam1Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[73] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76 * 0.45f, bt.BtRssiBeam1, "RSSI Beam 1 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the RSSI Beam 1.
        /// </summary>
        [Test]
        public void TestRSSIBeam1Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRssiBeam1 = 76 * 0.45f;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[73], "RSSI Beam 1 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the RSSI Beam 1 minimum.
        /// </summary>
        [Test]
        public void TestRSSIBeam1EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRssiBeam1 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[73], "RSSI Beam 1 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the RSSI Beam 1 maximum.
        /// </summary>
        [Test]
        public void TestRSSIBeam1EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRssiBeam1 = 255 * 0.45f;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[73], "RSSI Beam 1 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region RSSI Beam 2

        #region Decode

        /// <summary>
        /// Test decoding the RSSI Beam 2.
        /// </summary>
        [Test]
        public void TestRSSIBeam2Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[74] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76 * 0.45f, bt.BtRssiBeam2, "RSSI Beam 2 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the RSSI Beam 2.
        /// </summary>
        [Test]
        public void TestRSSIBeam2Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRssiBeam2 = 76 * 0.45f;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[74], "RSSI Beam 2 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the RSSI Beam 2 minimum.
        /// </summary>
        [Test]
        public void TestRSSIBeam2EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRssiBeam2 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[74], "RSSI Beam 2 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the RSSI Beam 2 maximum.
        /// </summary>
        [Test]
        public void TestRSSIBeam2EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRssiBeam2 = 255 * 0.45f;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[74], "RSSI Beam 2 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region RSSI Beam 3

        #region Decode

        /// <summary>
        /// Test decoding the RSSI Beam 3.
        /// </summary>
        [Test]
        public void TestRSSIBeam3Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[75] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76 * 0.45f, bt.BtRssiBeam3, "RSSI Beam 3 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the RSSI Beam 3.
        /// </summary>
        [Test]
        public void TestRSSIBeam3Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRssiBeam3 = 76 * 0.45f;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[75], "RSSI Beam 3 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the RSSI Beam 3 minimum.
        /// </summary>
        [Test]
        public void TestRSSIBeam3EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRssiBeam3 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[75], "RSSI Beam 3 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the RSSI Beam 3 maximum.
        /// </summary>
        [Test]
        public void TestRSSIBeam3EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRssiBeam3 = 255 * 0.45f;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[75], "RSSI Beam 3 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #endregion

        #region Gain

        #region Decode

        /// <summary>
        /// Test decoding the Gain.
        /// </summary>
        [Test]
        public void TestGainDecode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[76] = 0xF3;
            bt.Decode(data);
            Assert.AreEqual(243, bt.BtGain, "Gain Decode is incorrect.");

        }



        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Gain.
        /// </summary>
        [Test]
        public void TestGainEncode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtGain = 243;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xF3, data[76], "Gain Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Gain minimum.
        /// </summary>
        [Test]
        public void TestGainEncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtGain = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[76], "Gain Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Percent Good Minimum maximum.
        /// </summary>
        [Test]
        public void TestGainEncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtGain = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[76], "Gain Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Range MSB

        #region Range MSB Beam 0

        #region Decode

        /// <summary>
        /// Test decoding the Range MSB Beam 0.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam0Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[77] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRangeMsbBeam0, "Range MSB Beam 0 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Range MSB Beam 0.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam0Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeMsbBeam0 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[77], "Range MSB Beam 0 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range MSB Beam 0 minimum.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam0EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeMsbBeam0 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[77], "Range MSB Beam 0 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range MSB Beam 0 maximum.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam0EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeMsbBeam0 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[77], "Range MSB Beam 0 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Range MSB Beam 1

        #region Decode

        /// <summary>
        /// Test decoding the Range MSB Beam 1.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam1Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[78] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRangeMsbBeam1, "Range MSB Beam 1 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Range MSB Beam 1.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam1Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeMsbBeam1 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[78], "Range MSB Beam 1 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range MSB Beam 1 minimum.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam1EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeMsbBeam1 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[78], "Range MSB Beam 1 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range MSB Beam 1 maximum.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam1EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeMsbBeam1 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[78], "Range MSB Beam 1 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Range MSB Beam 2

        #region Decode

        /// <summary>
        /// Test decoding the Range MSB Beam 2.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam2Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[79] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRangeMsbBeam2, "Range MSB Beam 2 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Range MSB Beam 2.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam2Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeMsbBeam2 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[79], "Range MSB Beam 2 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range MSB Beam 2 minimum.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam2EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeMsbBeam2 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[79], "Range MSB Beam 2 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range MSB Beam 2 maximum.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam2EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeMsbBeam2 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[79], "Range MSB Beam 2 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #region Range MSB Beam 3

        #region Decode

        /// <summary>
        /// Test decoding the Range MSB Beam 3.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam3Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[80] = 0x4C;
            bt.Decode(data);
            Assert.AreEqual(76, bt.BtRangeMsbBeam3, "Range MSB Beam 3 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Range MSB Beam 3.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam3Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeMsbBeam3 = 76;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x4C, data[80], "Range MSB Beam 3 Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range MSB Beam 3 minimum.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam3EncodeMin()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeMsbBeam3 = 0;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x00, data[80], "Range MSB Beam 3 Min Encode is incorrect.");
        }

        /// <summary>
        /// Test encoding the Range MSB Beam 3 maximum.
        /// </summary>
        [Test]
        public void TestRangeMsbBeam3EncodeMax()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.BtRangeMsbBeam3 = 255;
            byte[] data = bt.Encode();
            Assert.AreEqual(0xFF, data[80], "Range MSB Beam 3 Max Encode is incorrect.");
        }

        #endregion

        #endregion

        #endregion

        #region Reserved 82-85

        #region Decode

        /// <summary>
        /// Test decoding the Reserved 82-85.
        /// </summary>
        [Test]
        public void TestReserved82_85Decode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[81] = 0x9B;
            data[82] = 0x0D;
            data[83] = 0x02;
            data[84] = 0x00;
            bt.Decode(data);
            Assert.AreEqual(134555, bt.Reserved82_85, "Reserved 82-85 Decode is incorrect.");

        }

        #endregion

        #region Encode

        /// <summary>
        /// Test encoding the Reserved 82-85.
        /// </summary>
        [Test]
        public void TestReserved82_85Encode()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            bt.Reserved82_85 = 134555;
            byte[] data = bt.Encode();
            Assert.AreEqual(0x9B, data[81], "Reserved 82-85 Max LSB Encode is incorrect.");
            Assert.AreEqual(0x0D, data[82], "Reserved 82-85 Max     Encode is incorrect.");
            Assert.AreEqual(0x02, data[83], "Reserved 82-85 Max     Encode is incorrect.");
            Assert.AreEqual(0x00, data[84], "Reserved 82-85 Max MSB Encode is incorrect.");
        }

        #endregion

        #endregion

        #region GetRangeBeam

        /// <summary>
        /// Test GetRangeBeam0().
        /// </summary>
        [Test]
        public void TestGetRangeBeam0()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[16] = 0x39;                // LSB LSB
            data[17] = 0x30;                // LSB MSB (12345)
            data[77] = 0x4C;                // MSB (76)
            bt.Decode(data);

            // (76 * 65535 ) + 12345 = 4980736 + 12345 = 4993081

            Assert.AreEqual(4993005, bt.GetRangeBeam0(), "GetRangeBeam0() is incorrect.");
        }

        /// <summary>
        /// Test GetRangeBeam1().
        /// </summary>
        [Test]
        public void TestGetRangeBeam1()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[18] = 0x39;                // LSB LSB
            data[19] = 0x30;                // LSB MSB (12345)
            data[78] = 0x4C;                // MSB (76)
            bt.Decode(data);

            // (76 * 65535 ) + 12345 = 4980736 + 12345 = 4993081

            Assert.AreEqual(4993005, bt.GetRangeBeam1(), "GetRangeBeam1() is incorrect.");
        }

        /// <summary>
        /// Test GetRangeBeam2().
        /// </summary>
        [Test]
        public void TestGetRangeBeam2()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[20] = 0x39;                // LSB LSB
            data[21] = 0x30;                // LSB MSB (12345)
            data[79] = 0x4C;                // MSB (76)
            bt.Decode(data);

            // (76 * 65535 ) + 12345 = 4980736 + 12345 = 4993081

            Assert.AreEqual(4993005, bt.GetRangeBeam2(), "GetRangeBeam2() is incorrect.");
        }

        /// <summary>
        /// Test GetRangeBeam3().
        /// </summary>
        [Test]
        public void TestGetRangeBeam3()
        {
            Pd0BottomTrack bt = new Pd0BottomTrack();

            byte[] data = new byte[Pd0BottomTrack.DATATYPE_SIZE];

            // Decode the data 
            data[22] = 0x39;                // LSB LSB
            data[23] = 0x30;                // LSB MSB (12345)
            data[80] = 0x4C;                // MSB (76)
            bt.Decode(data);

            // (76 * 65535 ) + 12345 = 4980736 + 12345 = 4993081

            Assert.AreEqual(4993005, bt.GetRangeBeam3(), "GetRangeBeam3() is incorrect.");
        }


        #endregion

        #region RTI Decode

        /// <summary>
        /// Test decoding RTI Bottom Track data to PD0 Bottom Track.
        /// </summary>
        [Test]
        public void DecodeRtiTest()
        {
            DataSet.BottomTrackDataSet rtiBT = new DataSet.BottomTrackDataSet(DataSet.Ensemble.DATATYPE_FLOAT,                 // Type of data stored (Float or Int)
                                                                            30,                                             // Number of bins
                                                                            4,                                              // Number of beams
                                                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                                                            DataSet.Ensemble.BottomTrackID);                // Dataset ID

            rtiBT.ActualPingCount = 5;
            rtiBT.Range[0] = 3450.99f;
            rtiBT.Range[1] = 1235.78f;
            rtiBT.Range[2] = 2345.23f;
            rtiBT.Range[3] = 3456.23f;
            rtiBT.SNR[0] = 23f;
            rtiBT.SNR[1] = 34f;
            rtiBT.SNR[2] = 45f;
            rtiBT.SNR[3] = 56f;
            rtiBT.Correlation[0] = 0.23f;
            rtiBT.Correlation[1] = 0.34f;
            rtiBT.Correlation[2] = 0.45f;
            rtiBT.Correlation[3] = 0.56f;
            rtiBT.EarthGood[0] = 5;
            rtiBT.EarthGood[1] = 5;
            rtiBT.EarthGood[2] = 2;
            rtiBT.EarthGood[3] = 5;
            rtiBT.Amplitude[0] = 23f;
            rtiBT.Amplitude[1] = 34f;
            rtiBT.Amplitude[2] = 45f;
            rtiBT.Amplitude[3] = 56f;

            Pd0BottomTrack bt = new Pd0BottomTrack(rtiBT, PD0.CoordinateTransforms.Coord_Beam);

            Assert.AreEqual(rtiBT.ActualPingCount, bt.BtPingsPerEnsemble, "Pings per Ensemble is incorrect.");
            Assert.AreEqual(0, bt.BtDelayBeforeReacquire, "Delay before reacquire is incorrect.");
            Assert.AreEqual(0, bt.BtCorrMagMin, "Correlation Magintude Min is incorrect.");
            Assert.AreEqual(0, bt.BtEvalAmpMin, "Evalation Amplitude Min is incorrect.");
            Assert.AreEqual(0, bt.BtPercentGoodMin, "Percent Good Min is incorrect.");
            Assert.AreEqual(0, bt.BtMode, "BT Mode is incorrect.");
            Assert.AreEqual(0, bt.BtErrVelMax, "Error Velocity Max is incorrect.");
            Assert.AreEqual(0, bt.Reserved13_16, "Reserved is incorrect.");
            Assert.AreEqual(5, bt.BtRangeMsbBeam3, "Range Beam 3 MSB is incorrect.");

            Assert.AreEqual(17424, bt.BtRangeLsbBeam3, "Range Beam 3 LSB is incorrect.");
            Assert.AreEqual(345099, bt.GetRangeBeam3(), "GetRangeBeam3 is incorrect.");
            Assert.AreEqual(1, bt.BtRangeMsbBeam2, "Range Beam 2 MSB is incorrect.");
            Assert.AreEqual(58043, bt.BtRangeLsbBeam2, "Range Beam 2 LSB is incorrect.");
            Assert.AreEqual(123578, bt.GetRangeBeam2(), "GetRangeBeam2 is incorrect.");
            Assert.AreEqual(3, bt.BtRangeMsbBeam0, "Range Beam 0 MSB is incorrect.");
            Assert.AreEqual(37918, bt.BtRangeLsbBeam0, "Range Beam 0 LSB is incorrect.");
            Assert.AreEqual(234523, bt.GetRangeBeam0(), "GetRangeBeam0 is incorrect.");
            Assert.AreEqual(5, bt.BtRangeMsbBeam1, "Range Beam 1 MSB is incorrect.");
            Assert.AreEqual(17948, bt.BtRangeLsbBeam1, "Range Beam 1 LSB is incorrect.");
            Assert.AreEqual(345623, bt.GetRangeBeam1(), "GetRangeBeam1 is incorrect.");

            Assert.AreEqual(112, bt.BtAmplitudeBeam0, "Amplitude Beam 0 is incorrect.");
            Assert.AreEqual(90, bt.BtAmplitudeBeam1, "Amplitude Beam 1 is incorrect.");
            Assert.AreEqual(46, bt.BtAmplitudeBeam2, "Amplitude Beam 2 is incorrect.");
            Assert.AreEqual(68, bt.BtAmplitudeBeam3, "Amplitude Beam 3 is incorrect.");

            Assert.AreEqual(143, bt.BtCorrelationMagnitudeBeam0, "Correlation Beam 0 is incorrect.");
            Assert.AreEqual(115, bt.BtCorrelationMagnitudeBeam1, "Correlation Beam 1 is incorrect.");
            Assert.AreEqual(59, bt.BtCorrelationMagnitudeBeam2, "Correlation Beam 2 is incorrect.");
            Assert.AreEqual(87, bt.BtCorrelationMagnitudeBeam3, "Correlation Beam 3 is incorrect.");

            Assert.AreEqual(100, bt.BtPercentGoodBeam0, "Percent Good Beam 0 is incorrect.");
            Assert.AreEqual(40, bt.BtPercentGoodBeam1, "Percent Good Beam 1 is incorrect.");
            Assert.AreEqual(100, bt.BtPercentGoodBeam2, "Percent Good Beam 2 is incorrect.");
            Assert.AreEqual(100, bt.BtPercentGoodBeam3, "Percent Good Beam 3 is incorrect.");

            Assert.AreEqual(112, bt.BtRssiBeam0, "RSSI Beam 0 is incorrect.");
            Assert.AreEqual(90, bt.BtRssiBeam1, "RSSI Beam 1 is incorrect.");
            Assert.AreEqual(46, bt.BtRssiBeam2, "RSSI Beam 2 is incorrect.");
            Assert.AreEqual(68, bt.BtRssiBeam3, "RSSI Beam 3 is incorrect.");

            Assert.AreEqual(1, bt.BtGain, "Gain is incorrect.");
            Assert.AreEqual(0, bt.Reserved82_85, "Reserved 82-85 is incorrect.");

        }

        #endregion
    }
}
