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
// * 02/25/2014      RC          2.21.4     Initial coding
// * 
// * 
// */

//namespace RTI
//{
//    using System;
//    using NUnit.Framework;
//    using System.Linq;
//    using System.Text;

//    /// <summary>
//    /// Test the PD0 Fixed Leader.
//    /// </summary>
//    [TestFixture]
//    public class Pd0FixedLeaderTest
//    {

//        /// <summary>
//        /// Test the constructor.
//        /// </summary>
//        [Test]
//        public void TestConstructor()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            Assert.AreEqual(0, fl.CpuFirmwareVersion, "CPU Firmware Version is Incorrect");
//            Assert.AreEqual(0, fl.CpuFirmwareRevision, "CPU Fimware Revision is Incorrect");
//            Assert.AreEqual(0, fl.SystemConfiguration, "System Configuration is Incorrect");
//            Assert.AreEqual(true, fl.RealSimFlag, "Real Sim Flag is Incorrect");
//            Assert.AreEqual(0, fl.LagLength, "Lag Length is Incorrect");
//            Assert.AreEqual(4, fl.NumberOfBeams, "Number of Beams is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.NUM_DEPTH_CELL_MIN, fl.NumberOfCells, "Number of Depth Cells is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.PINGS_PER_ENSEMBLE_MIN, fl.PingsPerEnsemble, "Pings per Ensemble is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.DEPTH_CELL_LENGTH_MIN, fl.DepthCellLength, "Depth Cell Length is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.BLANK_MIN, fl.BlankAfterTransmit, "Blank After Transmit is Incorrect");
//            Assert.AreEqual(1, fl.ProfilingMode, "Profile Mode is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.LOW_CORR_THRESH_MIN, fl.LowCorrThresh, "Low Correlation Threshold is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.NUM_CODE_REPEATS_MIN, fl.NumCodeRepeats, "Number of Code Repeats is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.PERCENT_GOOD_MINIMUM_MIN, fl.PercentGoodMinimum, "Percent Good Minimum is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.ERR_VEL_THRESHOLD_MIN, fl.ErrorVelMaximum, "Error Velocity Maximum is Incorrect");
//            Assert.AreEqual(0, fl.TimeBetweenPingMinutes, "Time Between Ping Minutes is Incorrect");
//            Assert.AreEqual(0, fl.TimeBetweenPingSeconds, "Time Between Ping Seconds is Incorrect");
//            Assert.AreEqual(0, fl.TimeBetweenPingHundredths, "Time Between Ping Hundredth is Incorrect");
//            Assert.AreEqual(0, fl.CoordinateTransform, "Coordinate Transform is Incorrect");
//            Assert.AreEqual(0.0, fl.HeadingAlignment, "Heading Alignment is Incorrect");
//            Assert.AreEqual(0.0, fl.HeadingBias, "Heading Bias is Incorrect");
//            Assert.AreEqual(0, fl.SensorSource, "Sensor Source is Incorrect");
//            Assert.AreEqual(0, fl.SensorsAvailable, "Sensors Available is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.BIN_1_DISTANCE_MIN, fl.Bin1Distance, "Bin 1 Distance is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.XMIT_PULSE_LENGTH_MIN, fl.XmitPulseLength, "Xmit Pulse Length is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.REF_LAYER_AVG_DEPTH_CELL_MIN, fl.ReferenceLayerAverageStartCell, "Reference Layer Averaging Start Cell is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.REF_LAYER_AVG_DEPTH_CELL_MIN + 1, fl.ReferenceLayerAverageEndCell, "Reference Layer Averaging End Cell is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.FALSE_TARGET_THRESH_MIN, fl.FalseTargetThresh, "False Target Thresh is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.SPARE_MIN, fl.Spare_40, "Spare is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.TRANSMIT_LAG_DISTANCE_MIN, fl.TransmitLagDistance, "Transmit Lag Distance is Incorrect");
//            Assert.AreEqual("", fl.CpuBoardSerialNumber, "CPU Board Serial Number is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.SYSTEM_BANDWIDTH_MIN, fl.SystemBandwidth, "System Bandwidth is Incorrect");
//            Assert.AreEqual(Pd0FixedLeader.SYSTEM_POWER_MIN, fl.SystemPower, "System Power is Incorrect");
//            Assert.AreEqual(0, fl.BaseFrequencyIndex, "Base Frequency Index is Incorrect");
//            Assert.AreEqual("", fl.InstrumentSerialNumber, "Instrument Serial Number is Incorrect");
//            Assert.AreEqual(0, fl.BeamAngle, "Beam Angle is Incorrect");

//        }

//        #region System Configuraiton

//        #region System Frequency

//        /// <summary>
//        /// Test seting and getting the System Frequency.
//        /// </summary>
//        [Test]
//        public void TestSystemFrequency()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // 150Khz, convex, downward facing, 30 Degree, 5 Beam (3 demods), Sensor Config 1
//            fl.SystemConfiguration = 21065;             // 0x5249

//            // 150kHz
//            Assert.AreEqual(Pd0FixedLeader.SystemFrequency.Freq_150kHz, fl.GetSystemFrequency(), "System Frequency 150kHz is incorrect.");

//            // 75kHz
//            fl.SetSystemFrequency(Pd0FixedLeader.SystemFrequency.Freq_75kHz);
//            Assert.AreEqual(Pd0FixedLeader.SystemFrequency.Freq_75kHz, fl.GetSystemFrequency(), "System Frequency 75kHz is incorrect.");

//            // 150kHz
//            fl.SetSystemFrequency(Pd0FixedLeader.SystemFrequency.Freq_150kHz);
//            Assert.AreEqual(Pd0FixedLeader.SystemFrequency.Freq_150kHz, fl.GetSystemFrequency(), "System Frequency 150kHz is incorrect.");

//            // 300kHz
//            fl.SetSystemFrequency(Pd0FixedLeader.SystemFrequency.Freq_300kHz);
//            Assert.AreEqual(Pd0FixedLeader.SystemFrequency.Freq_300kHz, fl.GetSystemFrequency(), "System Frequency 300kHz is incorrect.");

//            // 600kHz
//            fl.SetSystemFrequency(Pd0FixedLeader.SystemFrequency.Freq_600kHz);
//            Assert.AreEqual(Pd0FixedLeader.SystemFrequency.Freq_600kHz, fl.GetSystemFrequency(), "System Frequency 600kHz is incorrect.");

//            // 1200kHz
//            fl.SetSystemFrequency(Pd0FixedLeader.SystemFrequency.Freq_1200kHz);
//            Assert.AreEqual(Pd0FixedLeader.SystemFrequency.Freq_1200kHz, fl.GetSystemFrequency(), "System Frequency 1200kHz is incorrect.");

//            // 2400kHz
//            fl.SetSystemFrequency(Pd0FixedLeader.SystemFrequency.Freq_2400kHz);
//            Assert.AreEqual(Pd0FixedLeader.SystemFrequency.Freq_2400kHz, fl.GetSystemFrequency(), "System Frequency 2400kHz is incorrect.");

//        }

//        #endregion

//        #region Concave/Convex

//        /// <summary>
//        /// Test the constructor.
//        /// </summary>
//        [Test]
//        public void TestConcaveConvex()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // 150Khz, convex, downward facing, 30 Degree, 5 Beam (3 demods), Sensor Config 1
//            fl.SystemConfiguration = 21065;             // 0x5249

//            // Verify its convex
//            Assert.IsTrue(fl.IsConvex(), "IsConvex is Incorrect");

//            // Change the value concave
//            fl.SetConcave();

//            Assert.IsTrue(fl.IsConcave(), "IsConcave is Incorrect");

//            // Change the value to convex
//            fl.SetConvex();
//            Assert.IsTrue(fl.IsConvex(), "IsConvex after changing is Incorrect");
//        }

//        #endregion

//        #region Sensor Config

//        /// <summary>
//        /// Test seting and getting the Sensor Configs.
//        /// </summary>
//        [Test]
//        public void TestSensorConfig()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // 150Khz, convex, downward facing, 30 Degree, 5 Beam (3 demods), Sensor Config 1
//            fl.SystemConfiguration = 21065;             // 0x5249

//            Assert.IsTrue(fl.IsSensorConfig1(), "IsSensorConfig1 First is Incorrect.");
//            Assert.IsFalse(fl.IsSensorConfig2(), "IsSensorConfig2 First is Incorrect");
//            Assert.IsFalse(fl.IsSensorConfig3(), "IsSensorConfig3 First is Incorrect");

//            fl.SetSensorConfig2();
//            Assert.AreEqual(21081, fl.SystemConfiguration, "System Configuration Second is Incorrect");
//            Assert.IsFalse(fl.IsSensorConfig1(), "IsSensorConfig1 Second is Incorrect.");
//            Assert.IsTrue(fl.IsSensorConfig2(), "IsSensorConfig2 Second is Incorrect");
//            Assert.IsFalse(fl.IsSensorConfig3(), "IsSensorConfig3 Second is Incorrect");

//            fl.SetSensorConfig3();
//            Assert.AreEqual(21097, fl.SystemConfiguration, "System Configuration Third is Incorrect");
//            Assert.IsFalse(fl.IsSensorConfig1(), "IsSensorConfig1 Third is Incorrect.");
//            Assert.IsFalse(fl.IsSensorConfig2(), "IsSensorConfig2 Third is Incorrect");
//            Assert.IsTrue(fl.IsSensorConfig3(), "IsSensorConfig3 Third is Incorrect");
//        }

//        #endregion

//        #region Head Attached

//        /// <summary>
//        /// Test the Head Attached bit.
//        /// </summary>
//        [Test]
//        public void TestHeadAttached()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // 150Khz, convex, downward facing, 30 Degree, 5 Beam (3 demods), Sensor Config 1
//            fl.SystemConfiguration = 21065;             // 0x5249

//            // Verify its Head Attached
//            Assert.IsTrue(fl.IsHeadAttached(), "IsHeadAttached is Incorrect");

//            // Change the value to head not attached
//            fl.SetHeadNotAttached();

//            Assert.IsTrue(fl.IsHeadNotAttached(), "IsHeadNotAttached is Incorrect");

//            // Change the value to head attached
//            fl.SetHeadAttached();
//            Assert.IsTrue(fl.IsHeadAttached(), "IsHeadAttached after changing is Incorrect");
//        }

//        #endregion

//        #region Beam Facing

//        /// <summary>
//        /// Test the Head Attached bit.
//        /// </summary>
//        [Test]
//        public void TestBeamFacing()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // 150Khz, convex, downward facing, 30 Degree, 5 Beam (3 demods), Sensor Config 1
//            fl.SystemConfiguration = 21065;             // 0x5249

//            // Verify its Beams Downwards
//            Assert.IsTrue(fl.IsBeamsDownwards(), "IsBeamsDownwards is Incorrect");

//            // Change the value to Beams Upward
//            fl.SetBeamsUpward();

//            Assert.IsTrue(fl.IsBeamsUpwards(), "IsBeamsUpwards is Incorrect");

//            // Change the value to Beams Downwards
//            fl.SetBeamsDownward();
//            Assert.IsTrue(fl.IsBeamsDownwards(), "IsBeamsDownwards after changing is Incorrect");
//        }

//        #endregion

//        #region Beam Angle

//        /// <summary>
//        /// Test seting and getting the Beam Angle
//        /// </summary>
//        [Test]
//        public void TestBeamAngle()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // 150Khz, convex, downward facing, 30 Degree, 5 Beam (3 demods), Sensor Config 1
//            fl.SystemConfiguration = 21065;             // 0x5249

//            Assert.IsFalse(fl.Is15DegreeBeamAngle(), "Is15DegreeBeamAngle First is Incorrect.");
//            Assert.IsFalse(fl.Is20DegreeBeamAngle(), "Is20DegreeBeamAngle First is Incorrect");
//            Assert.IsTrue(fl.Is30DegreeBeamAngle(), "Is30DegreeBeamAngle First is Incorrect");
//            Assert.IsFalse(fl.IsOtherDegreeBeamAngle(), "IsOtherDegreeBeamAngle First is Incorrect");

//            fl.Set15DegreeBeamAngle();
//            Assert.IsTrue(fl.Is15DegreeBeamAngle(), "Is15DegreeBeamAngle Second is Incorrect.");
//            Assert.IsFalse(fl.Is20DegreeBeamAngle(), "Is20DegreeBeamAngle Second is Incorrect");
//            Assert.IsFalse(fl.Is30DegreeBeamAngle(), "Is30DegreeBeamAngle Second is Incorrect");
//            Assert.IsFalse(fl.IsOtherDegreeBeamAngle(), "IsOtherDegreeBeamAngle Second is Incorrect");

//            fl.Set20DegreeBeamAngle();
//            Assert.IsFalse(fl.Is15DegreeBeamAngle(), "Is15DegreeBeamAngle Third is Incorrect.");
//            Assert.IsTrue(fl.Is20DegreeBeamAngle(), "Is20DegreeBeamAngle Third is Incorrect");
//            Assert.IsFalse(fl.Is30DegreeBeamAngle(), "Is30DegreeBeamAngle Third is Incorrect");
//            Assert.IsFalse(fl.IsOtherDegreeBeamAngle(), "IsOtherDegreeBeamAngle Third is Incorrect");

//            fl.Set30DegreeBeamAngle();
//            Assert.IsFalse(fl.Is15DegreeBeamAngle(), "Is15DegreeBeamAngle Fourth is Incorrect.");
//            Assert.IsFalse(fl.Is20DegreeBeamAngle(), "Is20DegreeBeamAngle Fourth is Incorrect");
//            Assert.IsTrue(fl.Is30DegreeBeamAngle(), "Is30DegreeBeamAngle Fourth is Incorrect");
//            Assert.IsFalse(fl.IsOtherDegreeBeamAngle(), "IsOtherDegreeBeamAngle Fourth is Incorrect");

//            fl.SetOtherDegreeBeamAngle();
//            Assert.IsFalse(fl.Is15DegreeBeamAngle(), "Is15DegreeBeamAngle Fifth is Incorrect.");
//            Assert.IsFalse(fl.Is20DegreeBeamAngle(), "Is20DegreeBeamAngle Fifth is Incorrect");
//            Assert.IsFalse(fl.Is30DegreeBeamAngle(), "Is30DegreeBeamAngle Fifth is Incorrect");
//            Assert.IsTrue(fl.IsOtherDegreeBeamAngle(), "IsOtherDegreeBeamAngle Fifth is Incorrect");
//        }

//        #endregion

//        #region Beam Configuration

//        /// <summary>
//        /// Test seting and getting the Beam Configuration.
//        /// </summary>
//        [Test]
//        public void TestBeamConfiguration()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // 150Khz, convex, downward facing, 30 Degree, 5 Beam (3 demods), Sensor Config 1
//            fl.SystemConfiguration = 21065;             // 0x5249

//            // BeamConfig_5_Beam_Janus_Demod
//            Assert.AreEqual(Pd0FixedLeader.BeamConfigs.BeamConfig_5_Beam_Janus_Demod, fl.GetBeamConfiguration(), "BeamConfig_5_Beam_Janus_Demod is incorrect.");

//            // BeamConfig_4_Beam_Janus
//            fl.SetBeamConfiguration(Pd0FixedLeader.BeamConfigs.BeamConfig_4_Beam_Janus);
//            Assert.AreEqual(Pd0FixedLeader.BeamConfigs.BeamConfig_4_Beam_Janus, fl.GetBeamConfiguration(), "BeamConfig_4_Beam_Janus is incorrect.");

//            // BeamConfig_5_Beam_Janus_Demod
//            fl.SetBeamConfiguration(Pd0FixedLeader.BeamConfigs.BeamConfig_5_Beam_Janus_Demod);
//            Assert.AreEqual(Pd0FixedLeader.BeamConfigs.BeamConfig_5_Beam_Janus_Demod, fl.GetBeamConfiguration(), "BeamConfig_5_Beam_Janus_Demod Second is incorrect.");

//            // BeamCOnfig_5_Beam_Janus_2_Demod
//            fl.SetBeamConfiguration(Pd0FixedLeader.BeamConfigs.BeamConfig_5_Beam_Janus_2_Demod);
//            Assert.AreEqual(Pd0FixedLeader.BeamConfigs.BeamConfig_5_Beam_Janus_2_Demod, fl.GetBeamConfiguration(), "BeamCOnfig_5_Beam_Janus_2_Demod Second is incorrect.");

//        }

//        #endregion

//        #region Decode

//        /// <summary>
//        /// Test seting and getting the System Frequency.
//        /// </summary>
//        [Test]
//        public void TestSystemFrequencyDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];
//            data[4] = 0x49;                 // System Config LSB
//            data[5] = 0x52;                 // System Config MSB

//            // Decode the data
//            fl.Decode(data);

//            Assert.AreEqual(21065, fl.SystemConfiguration, "System Configuration Decode is incorrect.");

//            // 150Khz, convex, downward facing, 30 Degree, 5 Beam (3 demods), Sensor Config 1
//            //fl.SystemConfiguration = 21065;             // 0x5249

//            // 150kHz
//            Assert.AreEqual(Pd0FixedLeader.SystemFrequency.Freq_150kHz, fl.GetSystemFrequency(), "System Frequency Decode 150kHz is incorrect.");

//            // Verify its convex
//            Assert.IsTrue(fl.IsConvex(), "IsConvex Decode is Incorrect");

//            // Verifyt Sensor Config
//            Assert.IsTrue(fl.IsSensorConfig1(), "IsSensorConfig1 Decode First is Incorrect.");
//            Assert.IsFalse(fl.IsSensorConfig2(), "IsSensorConfig2 Decode First is Incorrect");
//            Assert.IsFalse(fl.IsSensorConfig3(), "IsSensorConfig3 Decode First is Incorrect");

//            // Verify its Head Attached
//            Assert.IsTrue(fl.IsHeadAttached(), "IsHeadAttached Decode is Incorrect");

//            // Verify its Beams Downwards
//            Assert.IsTrue(fl.IsBeamsDownwards(), "IsBeamsDownwards Decode is Incorrect");

//            // Verifyt Beam Angle
//            Assert.IsFalse(fl.Is15DegreeBeamAngle(), "Is15DegreeBeamAngle Decode is Incorrect.");
//            Assert.IsFalse(fl.Is20DegreeBeamAngle(), "Is20DegreeBeamAngle Decode is Incorrect");
//            Assert.IsTrue(fl.Is30DegreeBeamAngle(), "Is30DegreeBeamAngle Decode is Incorrect");
//            Assert.IsFalse(fl.IsOtherDegreeBeamAngle(), "IsOtherDegreeBeamAngle Decode is Incorrect");

//            // BeamConfig_5_Beam_Janus_Demod
//            Assert.AreEqual(Pd0FixedLeader.BeamConfigs.BeamConfig_5_Beam_Janus_Demod, fl.GetBeamConfiguration(), "BeamConfig_5_Beam_Janus_Demod Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test seting and getting the System Frequency.
//        /// </summary>
//        [Test]
//        public void TestSystemFrequencyEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            //byte[] data = new byte[59];
//            //data[4] = 0x49;                 // System Config LSB
//            //data[5] = 0x52;                 // System Config MSB

//            fl.SetSystemFrequency(Pd0FixedLeader.SystemFrequency.Freq_150kHz);
//            fl.SetConvex();
//            fl.SetSensorConfig1();
//            fl.SetHeadAttached();
//            fl.SetBeamsDownward();
//            fl.Set30DegreeBeamAngle();
//            fl.SetBeamConfiguration(Pd0FixedLeader.BeamConfigs.BeamConfig_5_Beam_Janus_Demod);

//            byte[] data = fl.Encode();

//            Assert.AreEqual(0x49, data[4], "System Config LSB is incorrect.");
//            Assert.AreEqual(0x52, data[5], "System Config MSB is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Real/Sim Flag

//        #region Decode

//        /// <summary>
//        /// Test decoding the Real Sim Flag.
//        /// </summary>
//        [Test]
//        public void TestRealSimFlagDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data false
//            data[6] = 0;
//            fl.Decode(data);
//            Assert.IsFalse(fl.RealSimFlag, "Real Sim Flag False Decode is incorrect.");

//            // Decode the data true
//            data[6] = 1;
//            fl.Decode(data);
//            Assert.IsTrue(fl.RealSimFlag, "Real Sim Flag True Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Real Sim Flag.
//        /// </summary>
//        [Test]
//        public void TestRealSimFlagEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.RealSimFlag = false;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x0, data[6], "Real Sim Flag False Encode is incorrect.");

//            fl.RealSimFlag = true;
//            byte[] dataT = fl.Encode();
//            Assert.AreEqual(0x1, dataT[6], "Real Sim Flag True Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Lag Length

//        #region Decode

//        /// <summary>
//        /// Test decoding the Lag Length.
//        /// </summary>
//        [Test]
//        public void TestLagLengthDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[7] = 75;
//            fl.Decode(data);
//            Assert.AreEqual(75, fl.LagLength, "Lag Length Decode is incorrect.");
//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Lag Length.
//        /// </summary>
//        [Test]
//        public void TestLagLengthEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.LagLength = 75;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(75, data[7], "Lag Length Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Number of Beams

//        #region Decode

//        /// <summary>
//        /// Test decoding the Number of Beams.
//        /// </summary>
//        [Test]
//        public void TestNumberOfBeamsDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[8] = 75;
//            fl.Decode(data);
//            Assert.AreEqual(75, fl.NumberOfBeams, "Number of Beams Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Number Of Beams.
//        /// </summary>
//        [Test]
//        public void TestNumberOfBeamsEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.NumberOfBeams = 75;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(75, data[8], "Number of Beams Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Number of Cells

//        #region Decode

//        /// <summary>
//        /// Test decoding the Number of Cells.
//        /// </summary>
//        [Test]
//        public void TestNumberOfCellDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[9] = 75;
//            fl.Decode(data);
//            Assert.AreEqual(75, fl.NumberOfCells, "Number of Cells Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Number Of Cells.
//        /// </summary>
//        [Test]
//        public void TestNumberOfCellsEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.NumberOfCells = 75;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(75, data[9], "Number of Cells Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Number Of Cells.
//        /// </summary>
//        [Test]
//        public void TestNumberOfCellsEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.NumberOfCells = 1;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(1, data[9], "Number of Cells Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Number Of Cells.
//        /// </summary>
//        [Test]
//        public void TestNumberOfCellsEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.NumberOfCells = 128;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(128, data[9], "Number of Cells Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Pings Per Ensemble

//        #region Decode

//        /// <summary>
//        /// Test decoding the Pings Per Ensemble.
//        /// </summary>
//        [Test]
//        public void TestPingsPerEnsembleDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[10] = 0x75;
//            data[11] = 0x25;
//            fl.Decode(data);
//            Assert.AreEqual(9589, fl.PingsPerEnsemble, "Pings Per Ensemble Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Pings per Ensemble.
//        /// </summary>
//        [Test]
//        public void TestPingsPerEnsembleEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.PingsPerEnsemble = 9589;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x75, data[10], "Pings Per Ensemble LSB Encode is incorrect.");
//            Assert.AreEqual(0x25, data[11], "Pings Per Ensemble MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Pings per Ensemble.
//        /// </summary>
//        [Test]
//        public void TestPingsPerEnsembleEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.PingsPerEnsemble = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x00, data[10], "Pings Per Ensemble LSB Encode is incorrect.");
//            Assert.AreEqual(0x00, data[11], "Pings Per Ensemble MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Pings per Ensemble.
//        /// </summary>
//        [Test]
//        public void TestPingsPerEnsembleEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.PingsPerEnsemble = 16384;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x00, data[10], "Pings Per Ensemble LSB Encode is incorrect.");
//            Assert.AreEqual(0x40, data[11], "Pings Per Ensemble MSB Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Depth Cell Length

//        #region Decode

//        /// <summary>
//        /// Test decoding the Depth Cell Length.
//        /// </summary>
//        [Test]
//        public void TestDepthCellLengthDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[12] = 0x75;
//            data[13] = 0x25;
//            fl.Decode(data);
//            Assert.AreEqual(9589, fl.DepthCellLength, "Depth Cell Length Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Depth Cell Length.
//        /// </summary>
//        [Test]
//        public void TestDepthCellLengthEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.DepthCellLength = 9589;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x75, data[12], "Depth Cell Length LSB Encode is incorrect.");
//            Assert.AreEqual(0x25, data[13], "Depth Cell Length MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Depth Cell Length.
//        /// </summary>
//        [Test]
//        public void TestDepthCellLengthEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.DepthCellLength = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x00, data[12], "Depth Cell Length LSB Encode is incorrect.");
//            Assert.AreEqual(0x00, data[13], "Depth Cell Length MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Depth Cell Length.
//        /// </summary>
//        [Test]
//        public void TestDepthCellLengthEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.DepthCellLength = 6400;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x00, data[12], "Depth Cell Length LSB Encode is incorrect.");
//            Assert.AreEqual(0x19, data[13], "Depth Cell Length MSB Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Blank After Transmit

//        #region Decode

//        /// <summary>
//        /// Test decoding the Blank After Transmit.
//        /// </summary>
//        [Test]
//        public void TestBlankAfterTransmitDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[14] = 0x75;
//            data[15] = 0x25;
//            fl.Decode(data);
//            Assert.AreEqual(9589, fl.BlankAfterTransmit, "Blank After Transmit Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Blank After Transmit.
//        /// </summary>
//        [Test]
//        public void TestBlankAfterTransmitEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.BlankAfterTransmit = 9589;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x75, data[14], "Blank After Transmit LSB Encode is incorrect.");
//            Assert.AreEqual(0x25, data[15], "Blank After Transmit MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Blank After Transmit.
//        /// </summary>
//        [Test]
//        public void TestBlankAfterTransmitEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.BlankAfterTransmit = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x00, data[14], "Blank After Transmit LSB Encode is incorrect.");
//            Assert.AreEqual(0x00, data[15], "Blank After Transmit MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Blank After Transmit.
//        /// </summary>
//        [Test]
//        public void TestBlankAfterTransmitEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.BlankAfterTransmit = 9999;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x0F, data[14], "Blank After Transmit LSB Encode is incorrect.");
//            Assert.AreEqual(0x27, data[15], "Blank After Transmit MSB Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Profiling Mode

//        #region Decode

//        /// <summary>
//        /// Test decoding the Profiling Mode.
//        /// </summary>
//        [Test]
//        public void TestSProfilingModeDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[16] = 75;
//            fl.Decode(data);
//            Assert.AreEqual(75, fl.ProfilingMode, "Profiling Mode Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Profiling Mode.
//        /// </summary>
//        [Test]
//        public void TestProfilingModeEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.ProfilingMode = 75;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(75, data[16], "Profiling Mode Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Low Correlation Threshold

//        #region Decode

//        /// <summary>
//        /// Test decoding the Low Correlation Threshold.
//        /// </summary>
//        [Test]
//        public void TestLowCorrThresholdDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[17] = 75;
//            fl.Decode(data);
//            Assert.AreEqual(75, fl.LowCorrThresh, "Low Correlation Threshold Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Low Correlation Threshold.
//        /// </summary>
//        [Test]
//        public void TestLowCorrThresholdEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.LowCorrThresh = 75;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(75, data[17], "Low Correlation Threshold Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Low Correlation Threshold.
//        /// </summary>
//        [Test]
//        public void TestLowCorrThresholdEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.LowCorrThresh = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0, data[17], "Low Correlation Threshold Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Low Correlation Threshold.
//        /// </summary>
//        [Test]
//        public void TestLowCorrThresholdEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.LowCorrThresh = 255;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(255, data[17], "Low Correlation Threshold Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Number of Code Repeats

//        #region Decode

//        /// <summary>
//        /// Test decoding the Number of Code Repeats.
//        /// </summary>
//        [Test]
//        public void TestNumCodeRepeatsDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[18] = 75;
//            fl.Decode(data);
//            Assert.AreEqual(75, fl.NumCodeRepeats, "Number of Code Repeats Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Number of Code Repeats.
//        /// </summary>
//        [Test]
//        public void TestNumCodeRepeatsEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.NumCodeRepeats = 75;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(75, data[18], "Number of Code Repeats Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Number of Code Repeats.
//        /// </summary>
//        [Test]
//        public void TestNumCodeRepeatsEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.NumCodeRepeats = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0, data[18], "Number of Code Repeats Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Number of Code Repeats.
//        /// </summary>
//        [Test]
//        public void TestNumCodeRepeatsEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.NumCodeRepeats = 255;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(255, data[18], "Number of Code Repeats Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Percent Good Minimum

//        #region Decode

//        /// <summary>
//        /// Test decoding the Percent Good Minimum.
//        /// </summary>
//        [Test]
//        public void TestPercentGoodMinimumDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[19] = 75;
//            fl.Decode(data);
//            Assert.AreEqual(75, fl.PercentGoodMinimum, "Percent Good Minimum Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Percent Good Minimum.
//        /// </summary>
//        [Test]
//        public void TestPercentGoodMinimumEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.PercentGoodMinimum = 75;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(75, data[19], "Percent Good Minimum Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Percent Good Minimum.
//        /// </summary>
//        [Test]
//        public void TestPercentGoodMinimumEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.PercentGoodMinimum = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0, data[19], "Percent Good Minimum Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Percent Good Minimum.
//        /// </summary>
//        [Test]
//        public void TestPercentGoodMinimumEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.PercentGoodMinimum = 100;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(100, data[19], "Percent Good Minimum Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Error Velocity Threshold

//        #region Decode

//        /// <summary>
//        /// Test decoding the Error Velocity Threshold.
//        /// </summary>
//        [Test]
//        public void TestErrorVelocityThresholdDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[20] = 0x75;
//            data[21] = 0x25;
//            fl.Decode(data);
//            Assert.AreEqual(9589, fl.ErrorVelMaximum, "Error Velocity Threshold Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Error Velocity Threshold.
//        /// </summary>
//        [Test]
//        public void TestErrorVelocityThresholdEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.ErrorVelMaximum = 9589;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x75, data[20], "Error Velocity Threshold LSB Encode is incorrect.");
//            Assert.AreEqual(0x25, data[21], "Error Velocity Thershold MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Error Velocity Threshold.
//        /// </summary>
//        [Test]
//        public void TestErrorVelocityThresholdEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.ErrorVelMaximum = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x00, data[20], "Error Velocity Threshold LSB Encode is incorrect.");
//            Assert.AreEqual(0x00, data[21], "Error Velocity Threshold MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Error Velocity Threshold.
//        /// </summary>
//        [Test]
//        public void TestErrorVelocityThresholdEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.ErrorVelMaximum = 5000;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x88, data[20], "Error Velocity Threshold LSB Encode is incorrect.");
//            Assert.AreEqual(0x13, data[21], "Error Velocity Threshold MSB Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Time Between Pings Minutes

//        #region Decode

//        /// <summary>
//        /// Test decoding the Time Between Pings Minutes.
//        /// </summary>
//        [Test]
//        public void TestTimeBetweenPingsMinutesDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[22] = 75;
//            fl.Decode(data);
//            Assert.AreEqual(75, fl.TimeBetweenPingMinutes, "Time Between Pings Minutes Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Time Between Pings Minutes.
//        /// </summary>
//        [Test]
//        public void TestTimeBetweenPingsMinutesEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.TimeBetweenPingMinutes = 75;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(75, data[22], "Time Between Pings Minutes Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Time Between Pings Seconds

//        #region Decode

//        /// <summary>
//        /// Test decoding the Time Between Pings Seconds.
//        /// </summary>
//        [Test]
//        public void TestTimeBetweenPingsSecondsDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[23] = 75;
//            fl.Decode(data);
//            Assert.AreEqual(75, fl.TimeBetweenPingSeconds, "Time Between Pings Seconds Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Time Between Pings Seconds.
//        /// </summary>
//        [Test]
//        public void TestTimeBetweenPingsSecondsEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.TimeBetweenPingSeconds = 75;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(75, data[23], "Time Between Pings Seconds Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Time Between Pings Hundredth

//        #region Decode

//        /// <summary>
//        /// Test decoding the Time Between Pings Hundredth.
//        /// </summary>
//        [Test]
//        public void TestTimeBetweenPingsHundredthDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[24] = 75;
//            fl.Decode(data);
//            Assert.AreEqual(75, fl.TimeBetweenPingHundredths, "Time Between Pings Hundredth Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Time Between Pings Hundredth.
//        /// </summary>
//        [Test]
//        public void TestTimeBetweenPingsHundredthEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.TimeBetweenPingHundredths = 75;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(75, data[24], "Time Between Pings Hundredth Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Coordinate Transform

//        #region Coordinate Transform

//        /// <summary>
//        /// Test seting and getting the Coordinate Transform.
//        /// </summary>
//        [Test]
//        public void TestCoordinateTransform()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Earth Coord, Ilts, 3-Beam Solution, Bin Mapping
//            fl.CoordinateTransform = 31;

//            // Coord_Earth
//            Assert.AreEqual(PD0.CoordinateTransforms.Coord_Earth, fl.GetCoordinateTransform(), "Coord_Earth is incorrect.");

//            // Coord_Beam
//            fl.SetCoordinateTransform(PD0.CoordinateTransforms.Coord_Beam);
//            Assert.AreEqual(PD0.CoordinateTransforms.Coord_Beam, fl.GetCoordinateTransform(), "Coord_Beam is incorrect.");

//            // Coord_Instrument
//            fl.SetCoordinateTransform(PD0.CoordinateTransforms.Coord_Instrument);
//            Assert.AreEqual(PD0.CoordinateTransforms.Coord_Instrument, fl.GetCoordinateTransform(), "Coord_Instrument Second is incorrect.");

//            // Coord_Ship
//            fl.SetCoordinateTransform(PD0.CoordinateTransforms.Coord_Ship);
//            Assert.AreEqual(PD0.CoordinateTransforms.Coord_Ship, fl.GetCoordinateTransform(), "Coord_Ship Second is incorrect.");

//            // Coord_Earth
//            fl.SetCoordinateTransform(PD0.CoordinateTransforms.Coord_Earth);
//            Assert.AreEqual(PD0.CoordinateTransforms.Coord_Earth, fl.GetCoordinateTransform(), "Coord_Earth Second is incorrect.");

//        }

//        #endregion

//        #region Tilts Used

//        /// <summary>
//        /// Test the Tilts Used.
//        /// </summary>
//        [Test]
//        public void TestTiltsUsed()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Earth Coord, Ilts, 3-Beam Solution, Bin Mapping
//            fl.CoordinateTransform = 31;

//            // Verify its Tilts Used
//            Assert.IsTrue(fl.IsTiltsUsedInTransform(), "IsTiltsUsedInTransform is Incorrect");

//            // Change the value to Tilts Not Used
//            fl.SetTiltsNotUsedInTransform();

//            Assert.IsTrue(fl.IsTiltsNotUsedInTransform(), "IsTiltsNotUsedInTransform is Incorrect");

//            // Change the value to Beams Downwards
//            fl.SetTiltsUsedInTransform();
//            Assert.IsTrue(fl.IsTiltsUsedInTransform(), "IsTiltsUsedInTransform after changing is Incorrect");
//        }

//        #endregion

//        #region 3-Beam Solution

//        /// <summary>
//        /// Test the Tilts Used.
//        /// </summary>
//        [Test]
//        public void Test3BeamSolution()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Earth Coord, Ilts, 3-Beam Solution, Bin Mapping
//            fl.CoordinateTransform = 31;

//            // Verify its 3 Beam Solution
//            Assert.IsTrue(fl.Is3BeamSolution(), "Is3BeamSolution is Incorrect");

//            // Change the value to Tilts Not Used
//            fl.SetNo3BeamSolution();

//            Assert.IsTrue(fl.IsNo3BeamSolution(), "IsNo3BeamSolution is Incorrect");

//            // Change the value to Beams Downwards
//            fl.Set3BeamSolution();
//            Assert.IsTrue(fl.Is3BeamSolution(), "Is3BeamSolution after changing is Incorrect");
//        }

//        #endregion

//        #region Binmapping

//        /// <summary>
//        /// Test the Binmapping.
//        /// </summary>
//        [Test]
//        public void TestBinmapping()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Earth Coord, Ilts, 3-Beam Solution, Bin Mapping
//            fl.CoordinateTransform = 31;

//            // Verify its 3 Beam Solution
//            Assert.IsTrue(fl.IsBinmappingUsed(), "IsBinmappingUsed is Incorrect");

//            // Change the value to Tilts Not Used
//            fl.SetNoBinmappingUsed();

//            Assert.IsTrue(fl.IsNoBinmappingUsed(), "IsNoBinmappingUsed is Incorrect");

//            // Change the value to Beams Downwards
//            fl.SetBinmappingUsed();
//            Assert.IsTrue(fl.IsBinmappingUsed(), "IsBinmappingUsed after changing is Incorrect");
//        }

//        #endregion

//        #region Decode

//        /// <summary>
//        /// Test decoding the Coordinate Transform.
//        /// </summary>
//        [Test]
//        public void TestCoordinateTransformDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[25] = 7;
//            fl.Decode(data);
//            Assert.AreEqual(PD0.CoordinateTransforms.Coord_Beam, fl.GetCoordinateTransform(), "Coordinate Transform decode is incorrect.");
//            Assert.IsTrue(fl.IsTiltsUsedInTransform(), "Coordinate Transform Tilts Decode is incorrect.");
//            Assert.IsTrue(fl.Is3BeamSolution(), "Coordinate Transform 3-Beam Solution Decode is incorrect.");
//            Assert.IsTrue(fl.IsBinmappingUsed(), "Coordinate Transform Binmapping Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Coordinate Transform.
//        /// </summary>
//        [Test]
//        public void TestCoordinateTransformEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.SetCoordinateTransform(PD0.CoordinateTransforms.Coord_Beam);
//            fl.SetTiltsUsedInTransform();
//            fl.Set3BeamSolution();
//            fl.SetBinmappingUsed();

//            byte[] data = fl.Encode();
//            Assert.AreEqual(7, data[25], "Coordinate Transform Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Heading Alignment

//        #region Decode

//        /// <summary>
//        /// Test decoding the Heading Alignment.
//        /// </summary>
//        [Test]
//        public void TestHeadingAlignmentDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[26] = 0xF4;
//            data[27] = 0xCF;
//            fl.Decode(data);                                                                            // This will store -12300 to the byte array and will be converted back.
//            Assert.AreEqual(-123, fl.HeadingAlignment, "Heading Alignment Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Heading Alignment.
//        /// LSD = 0.01 so the value stored will be -123 / 0.01 = -12300 = 0xCFF4
//        /// </summary>
//        [Test]
//        public void TestHeadingAlignmentEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.HeadingAlignment = -123;
//            byte[] data = fl.Encode();                                                                  // This will store -12300 to the byte array and will be converted back.
//            Assert.AreEqual(0xF4, data[26], "Heading Alignment LSB Encode is incorrect.");
//            Assert.AreEqual(0xCF, data[27], "Heading Alignment MSB Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Heading Bias

//        #region Decode

//        /// <summary>
//        /// Test decoding the Heading Bias.
//        /// </summary>
//        [Test]
//        public void TestHeadingBiasDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[28] = 0xF4;
//            data[29] = 0xCF;
//            fl.Decode(data);                                                                            // This will store -12300 to the byte array and will be converted back.
//            Assert.AreEqual(-123, fl.HeadingBias, "Heading Bias Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Heading Bias.
//        /// LSD = 0.01 so the value stored will be -123 / 0.01 = -12300 = 0xCFF4
//        /// </summary>
//        [Test]
//        public void TestHeadingBiasEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.HeadingBias = -123.45f;
//            byte[] data = fl.Encode();                                                                  // This will store -12345 to the byte array and will be converted back.
//            Assert.AreEqual(0xC7, data[28], "Heading Bias LSB Encode is incorrect.");
//            Assert.AreEqual(0xCF, data[29], "Heading Bias MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Heading Bias.
//        /// LSD = 0.01 so the value stored will be -179.99 / 0.01 = -17999 = 0xB9B1
//        /// </summary>
//        [Test]
//        public void TestHeadingBiasEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.HeadingBias = -179.99f;
//            byte[] data = fl.Encode();                                                                  // This will store -12345 to the byte array and will be converted back.
//            Assert.AreEqual(0xB1, data[28], "Heading Bias LSB Encode is incorrect.");
//            Assert.AreEqual(0xB9, data[29], "Heading Bias MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Heading Bias.
//        /// LSD = 0.01 so the value stored will be 180 / 0.01 = 18000 = 0x4650
//        /// </summary>
//        [Test]
//        public void TestHeadingBiasEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.HeadingBias = 180.00f;
//            byte[] data = fl.Encode();                                                                  // This will store -12345 to the byte array and will be converted back.
//            Assert.AreEqual(0x50, data[28], "Heading Bias LSB Encode is incorrect.");
//            Assert.AreEqual(0x46, data[29], "Heading Bias MSB Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Sensors Source

//        #region Transducer Temperature Sensor

//        /// <summary>
//        /// Test the Binmapping.
//        /// </summary>
//        [Test]
//        public void TestXdcrTempSensor()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorSource = 127;

//            // Verify its XDCR Temp Sensor
//            Assert.IsTrue(fl.IsUseXdcrTempSensor(), "IsXdcrTempSensor is Incorrect");

//            // Change the value to XDCR Temp Sensor Not Used
//            fl.SetNoUseXdcrTempSensor();

//            Assert.IsTrue(fl.IsNoUseXdcrTempSensor(), "IsNoUseXdcrTempSensor is Incorrect");

//            // Change the value to XDCR Temp Sensor
//            fl.SetUseXdcrTempSensor();
//            Assert.IsTrue(fl.IsUseXdcrTempSensor(), "IsXdcrTempSensor after changing is Incorrect");
//        }

//        #endregion

//        #region Conductivity Sensor

//        /// <summary>
//        /// Test the Conductivity Sensor.
//        /// </summary>
//        [Test]
//        public void TestConductivitySensor()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorSource = 127;

//            // Verify its Conductivity Sensor
//            Assert.IsTrue(fl.IsUseConductivitySensor(), "IsConductivitySensor is Incorrect");

//            // Change the value to Conductivity Sensor Not Used
//            fl.SetNoUseConductivitySensor();

//            Assert.IsTrue(fl.IsNoUseConductivitySensor(), "IsNoConductivitySensor is Incorrect");

//            // Change the value to Conductivity Sensor
//            fl.SetUseConductivitySensor();
//            Assert.IsTrue(fl.IsUseConductivitySensor(), "IsConductivitySensor after changing is Incorrect");
//        }

//        #endregion

//        #region Tranducer Roll Sensor

//        /// <summary>
//        /// Test the XDCR Roll Sensor.
//        /// </summary>
//        [Test]
//        public void TestXdcrRollSensor()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorSource = 127;

//            // Verify its XDCR Roll Sensor
//            Assert.IsTrue(fl.IsUseXdcrRollSensor(), "IsXdcrRollSensor is Incorrect");

//            // Change the value to XDCR Roll Sensor Not Used
//            fl.SetNoUseXdcrRollSensor();

//            Assert.IsTrue(fl.IsNoUseXdcrRollSensor(), "IsNoXdcrRollSensor is Incorrect");

//            // Change the value to XDCR Roll Sensor
//            fl.SetUseXdcrRollSensor();
//            Assert.IsTrue(fl.IsUseXdcrRollSensor(), "IsXdcrRollSensor after changing is Incorrect");
//        }

//        #endregion

//        #region Tranducer Pitch Sensor

//        /// <summary>
//        /// Test the XDCR Pitch Sensor.
//        /// </summary>
//        [Test]
//        public void TestXdcrPitchSensor()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorSource = 127;

//            // Verify its XDCR Pitch Sensor
//            Assert.IsTrue(fl.IsUseXdcrPitchSensor(), "IsXdcrPitchSensor is Incorrect");

//            // Change the value to XDCR Pitch Sensor Not Used
//            fl.SetNoUseXdcrPitchSensor();

//            Assert.IsTrue(fl.IsNoUseXdcrPitchSensor(), "IsNoXdcrPitchSensor is Incorrect");

//            // Change the value to XDCR Pitch Sensor
//            fl.SetUseXdcrPitchSensor();
//            Assert.IsTrue(fl.IsUseXdcrPitchSensor(), "IsXdcrPitchSensor after changing is Incorrect");
//        }

//        #endregion

//        #region Tranducer Heading Sensor

//        /// <summary>
//        /// Test the XDCR Heading Sensor.
//        /// </summary>
//        [Test]
//        public void TestXdcrHeadingSensor()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorSource = 127;

//            // Verify its XDCR Heading Sensor
//            Assert.IsTrue(fl.IsUseXdcrHeadingSensor(), "IsXdcrHeadingSensor is Incorrect");

//            // Change the value to XDCR Heading Sensor Not Used
//            fl.SetNoUseXdcrHeadingSensor();

//            Assert.IsTrue(fl.IsNoUseXdcrHeadingSensor(), "IsNoXdcrHeadingSensor is Incorrect");

//            // Change the value to XDCR Heading Sensor
//            fl.SetUseXdcrHeadingSensor();
//            Assert.IsTrue(fl.IsUseXdcrHeadingSensor(), "IsXdcrHeadingSensor after changing is Incorrect");
//        }

//        #endregion

//        #region Tranducer Depth Sensor

//        /// <summary>
//        /// Test the XDCR Depth Sensor.
//        /// </summary>
//        [Test]
//        public void TestXdcrDepthSensor()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorSource = 127;

//            // Verify its XDCR Depth Sensor
//            Assert.IsTrue(fl.IsUseXdcrDepthSensor(), "IsXdcrHeadingSensor is Incorrect");

//            // Change the value to XDCR Depth Sensor Not Used
//            fl.SetNoUseXdcrDepthSensor();

//            Assert.IsTrue(fl.IsNoUseXdcrDepthSensor(), "IsNoXdcrDepthSensor is Incorrect");

//            // Change the value to XDCR Depth Sensor
//            fl.SetUseXdcrDepthSensor();
//            Assert.IsTrue(fl.IsUseXdcrDepthSensor(), "IsXdcrDepthSensor after changing is Incorrect");
//        }

//        #endregion

//        #region Speed of Sound Sensor

//        /// <summary>
//        /// Test the Speed of Sound Sensor.
//        /// </summary>
//        [Test]
//        public void TestSpeedOfSoundSensor()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorSource = 127;

//            // Verify its Speed of Sound Sensor
//            Assert.IsTrue(fl.IsUseSpeedOfSoundSensor(), "IsSpeedOfSoundSensor is Incorrect");

//            // Change the value to Speed of Sound Sensor Not Used
//            fl.SetNoUseSpeedOfSoundSensor();

//            Assert.IsTrue(fl.IsNoUseSpeedOfSoundSensor(), "IsNoSpeedOfSoundSensor is Incorrect");

//            // Change the value to Speed of Sound Sensor
//            fl.SetUseSpeedOfSoundSensor();
//            Assert.IsTrue(fl.IsUseSpeedOfSoundSensor(), "IsSpeedOfSoundSensor after changing is Incorrect");
//        }

//        #endregion

//        #endregion

//        #region Sensors Available

//        #region Transducer Temperature Sensor Available

//        /// <summary>
//        /// Test the Temperatur Sensor available.
//        /// </summary>
//        [Test]
//        public void TestXdcrTempSensorAvailable()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorsAvailable = 127;

//            // Verify its XDCR Temp Sensor
//            Assert.IsTrue(fl.IsXdcrTempSensorAvailable(), "IsXdcrTempSensorAvailable is Incorrect");

//            // Change the value to XDCR Temp Sensor Not Used
//            fl.SetNoXdcrTempSensorAvailable();

//            Assert.IsTrue(fl.IsNoXdcrTempSensorAvailable(), "IsNoXdcrTempSensorAvailable is Incorrect");

//            // Change the value to XDCR Temp Sensor
//            fl.SetXdcrTempSensorAvailable();
//            Assert.IsTrue(fl.IsXdcrTempSensorAvailable(), "IsXdcrTempSensorAvailable after changing is Incorrect");
//        }

//        #endregion

//        #region Conductivity Sensor Available

//        /// <summary>
//        /// Test the Conductivity Sensor.
//        /// </summary>
//        [Test]
//        public void TestConductivitySensorAvailable()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorsAvailable = 127;

//            // Verify its Conductivity Sensor
//            Assert.IsTrue(fl.IsConductivitySensorAvailable(), "IsConductivitySensorAvailable is Incorrect");

//            // Change the value to Conductivity Sensor Not Used
//            fl.SetNoConductivitySensorAvailable();

//            Assert.IsTrue(fl.IsNoConductivitySensorAvailable(), "IsNoConductivitySensorAvailable is Incorrect");

//            // Change the value to Conductivity Sensor
//            fl.SetConductivitySensorAvailable();
//            Assert.IsTrue(fl.IsConductivitySensorAvailable(), "IsConductivitySensorAvailable after changing is Incorrect");
//        }

//        #endregion

//        #region Tranducer Roll Sensor Available

//        /// <summary>
//        /// Test the XDCR Roll Sensor.
//        /// </summary>
//        [Test]
//        public void TestXdcrRollSensorAvailable()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorsAvailable = 127;

//            // Verify its XDCR Roll Sensor
//            Assert.IsTrue(fl.IsXdcrRollSensorAvailable(), "IsXdcrRollSensorAvailable is Incorrect");

//            // Change the value to XDCR Roll Sensor Not Used
//            fl.SetNoXdcrRollSensorAvailable();

//            Assert.IsTrue(fl.IsNoXdcrRollSensorAvailable(), "IsNoXdcrRollSensorAvailable is Incorrect");

//            // Change the value to XDCR Roll Sensor
//            fl.SetXdcrRollSensorAvailable();
//            Assert.IsTrue(fl.IsXdcrRollSensorAvailable(), "IsXdcrRollSensorAvailable after changing is Incorrect");
//        }

//        #endregion

//        #region Tranducer Pitch Sensor Available

//        /// <summary>
//        /// Test the XDCR Pitch Sensor.
//        /// </summary>
//        [Test]
//        public void TestXdcrPitchSensorAvailable()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorsAvailable = 127;

//            // Verify its XDCR Pitch Sensor
//            Assert.IsTrue(fl.IsXdcrPitchSensorAvailable(), "IsXdcrPitchSensorAvailable is Incorrect");

//            // Change the value to XDCR Pitch Sensor Not Used
//            fl.SetNoXdcrPitchSensorAvailable();

//            Assert.IsTrue(fl.IsNoXdcrPitchSensorAvailable(), "IsNoXdcrPitchSensorAvailable is Incorrect");

//            // Change the value to XDCR Pitch Sensor
//            fl.SetXdcrPitchSensorAvailable();
//            Assert.IsTrue(fl.IsXdcrPitchSensorAvailable(), "IsXdcrPitchSensorAvailable after changing is Incorrect");
//        }

//        #endregion

//        #region Tranducer Heading Sensor Available

//        /// <summary>
//        /// Test the XDCR Heading Sensor.
//        /// </summary>
//        [Test]
//        public void TestXdcrHeadingSensorAvailable()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorsAvailable = 127;

//            // Verify its XDCR Heading Sensor
//            Assert.IsTrue(fl.IsXdcrHeadingSensorAvailable(), "IsXdcrHeadingSensorAvailable is Incorrect");

//            // Change the value to XDCR Heading Sensor Not Used
//            fl.SetNoXdcrHeadingSensorAvailable();

//            Assert.IsTrue(fl.IsNoXdcrHeadingSensorAvailable(), "IsNoXdcrHeadingSensorAvailable is Incorrect");

//            // Change the value to XDCR Heading Sensor
//            fl.SetXdcrHeadingSensorAvailable();
//            Assert.IsTrue(fl.IsXdcrHeadingSensorAvailable(), "IsXdcrHeadingSensorAvailable after changing is Incorrect");
//        }

//        #endregion

//        #region Tranducer Depth Sensor Available

//        /// <summary>
//        /// Test the XDCR Depth Sensor.
//        /// </summary>
//        [Test]
//        public void TestXdcrDepthSensorAvailable()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorsAvailable = 127;

//            // Verify its XDCR Depth Sensor
//            Assert.IsTrue(fl.IsXdcrDepthSensorAvailable(), "IsXdcrDepthSensorAvailable is Incorrect");

//            // Change the value to XDCR Depth Sensor Not Used
//            fl.SetNoXdcrDepthSensorAvailable();

//            Assert.IsTrue(fl.IsNoXdcrDepthSensorAvailable(), "IsNoXdcrDepthSensorAvailable is Incorrect");

//            // Change the value to XDCR Depth Sensor
//            fl.SetXdcrDepthSensorAvailable();
//            Assert.IsTrue(fl.IsXdcrDepthSensorAvailable(), "IsXdcrDepthSensorAvailable after changing is Incorrect");
//        }

//        #endregion

//        #region Speed of Sound Sensor Available

//        /// <summary>
//        /// Test the Speed of Sound Sensor.
//        /// </summary>
//        [Test]
//        public void TestSpeedOfSoundSensorAvailable()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            // Temp, Conductivity, roll, pitch, heading, depth, sos
//            fl.SensorsAvailable = 127;

//            // Verify its Speed of Sound Sensor
//            Assert.IsTrue(fl.IsSpeedOfSoundSensorAvailable(), "IsSpeedOfSoundSensorAvailable is Incorrect");

//            // Change the value to Speed of Sound Sensor Not Used
//            fl.SetNoSpeedOfSoundSensorAvailable();

//            Assert.IsTrue(fl.IsNoSpeedOfSoundSensorAvailable(), "IsNoSpeedOfSoundSensorAvailable is Incorrect");

//            // Change the value to Speed of Sound Sensor
//            fl.SetSpeedOfSoundSensorAvailable();
//            Assert.IsTrue(fl.IsSpeedOfSoundSensorAvailable(), "IsSpeedOfSoundSensorAvailable after changing is Incorrect");
//        }

//        #endregion

//        #endregion

//        #region Bin 1 Distance

//        #region Decode

//        /// <summary>
//        /// Test decoding the Bin 1 Distance.
//        /// </summary>
//        [Test]
//        public void TestBin1DistanceDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[32] = 0x75;
//            data[33] = 0x25;
//            fl.Decode(data);
//            Assert.AreEqual(9589, fl.Bin1Distance, "Bin 1 Distance Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Bin 1 Distance.
//        /// </summary>
//        [Test]
//        public void TestBin1DistanceEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.Bin1Distance = 9589;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x75, data[32], "Bin 1 Distance LSB Encode is incorrect.");
//            Assert.AreEqual(0x25, data[33], "Bin 1 Distance MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Bin 1 Distance.
//        /// </summary>
//        [Test]
//        public void TestBin1DistanceEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.Bin1Distance = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x00, data[32], "Bin 1 Distance LSB Encode is incorrect.");
//            Assert.AreEqual(0x00, data[33], "Bin 1 Distance MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Bin 1 Distance.
//        /// </summary>
//        [Test]
//        public void TestBin1DistanceEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.Bin1Distance = 65535;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0xFF, data[32], "Bin 1 Distance LSB Encode is incorrect.");
//            Assert.AreEqual(0xFF, data[33], "Bin 1 Distance MSB Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Xmit Pulse Length

//        #region Decode

//        /// <summary>
//        /// Test decoding the Xmit Pulse Length.
//        /// </summary>
//        [Test]
//        public void TestXmitPulseLengthDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[34] = 0x75;
//            data[35] = 0x25;
//            fl.Decode(data);
//            Assert.AreEqual(9589, fl.XmitPulseLength, "Xmit Pulse Length Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Xmit Pulse Length.
//        /// </summary>
//        [Test]
//        public void TestXmitPulseLengthEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.XmitPulseLength = 9589;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x75, data[34], "Xmit Pulse Length LSB Encode is incorrect.");
//            Assert.AreEqual(0x25, data[35], "Xmit Pulse Length MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Xmit Pulse Length.
//        /// </summary>
//        [Test]
//        public void TestXmitPulseLengthEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.XmitPulseLength = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x00, data[34], "Xmit Pulse Length LSB Encode is incorrect.");
//            Assert.AreEqual(0x00, data[35], "Xmit Pulse Length MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Xmit Pulse Length.
//        /// </summary>
//        [Test]
//        public void TestXmitPulseLengthEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.XmitPulseLength = 65535;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0xFF, data[34], "Xmit Pulse Length LSB Encode is incorrect.");
//            Assert.AreEqual(0xFF, data[35], "Xmit Pulse Length MSB Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Reference Layer Average Layer Start

//        #region Decode

//        /// <summary>
//        /// Test decoding the Reference Layer Average Layer Start.
//        /// </summary>
//        [Test]
//        public void TestReferenceLayerAvgLayerStartDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[36] = 0x64;
//            fl.Decode(data);
//            Assert.AreEqual(100, fl.ReferenceLayerAverageStartCell, "Reference Layer Average Layer Start Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Reference Layer Average Layer Start.
//        /// </summary>
//        [Test]
//        public void TestReferenceLayerAvgLayerStartEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.ReferenceLayerAverageStartCell = 100;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x64, data[36], "Reference Layer Average Layer Start Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Reference Layer Average Layer Start.
//        /// </summary>
//        [Test]
//        public void TestReferenceLayerAvgLayerStartEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.ReferenceLayerAverageStartCell = 1;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x01, data[36], "Reference Layer Average Layer Start Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Reference Layer Average Layer Start.
//        /// </summary>
//        [Test]
//        public void TestReferenceLayerAvgLayerStartEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.ReferenceLayerAverageStartCell = 128;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x80, data[36], "Reference Layer Average Layer Start Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Reference Layer Average Layer End

//        #region Decode

//        /// <summary>
//        /// Test decoding the Reference Layer Average Layer End.
//        /// </summary>
//        [Test]
//        public void TestReferenceLayerAvgLayerEndDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[37] = 0x64;
//            fl.Decode(data);
//            Assert.AreEqual(100, fl.ReferenceLayerAverageEndCell, "Reference Layer Average Layer End Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Reference Layer Average Layer End.
//        /// </summary>
//        [Test]
//        public void TestReferenceLayerAvgLayerEndEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.ReferenceLayerAverageEndCell = 100;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x64, data[37], "Reference Layer Average Layer End Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Reference Layer Average Layer End.
//        /// </summary>
//        [Test]
//        public void TestReferenceLayerAvgLayerEndEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.ReferenceLayerAverageEndCell = 1;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x01, data[37], "Reference Layer Average Layer End Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Reference Layer Average Layer End.
//        /// </summary>
//        [Test]
//        public void TestReferenceLayerAvgLayerEndEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.ReferenceLayerAverageEndCell = 128;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x80, data[37], "Reference Layer Average Layer End Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region False Target Threshold

//        #region Decode

//        /// <summary>
//        /// Test decoding the False Target Threshold.
//        /// </summary>
//        [Test]
//        public void TestFalseTargetThresholdDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[38] = 75;
//            fl.Decode(data);
//            Assert.AreEqual(75, fl.FalseTargetThresh, "False Target Threshold Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the False Target Threshold.
//        /// </summary>
//        [Test]
//        public void TestFalseTargetThresholdEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.FalseTargetThresh = 75;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(75, data[38], "False Target Threshold Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the False Target Threshold.
//        /// </summary>
//        [Test]
//        public void TestFalseTargetThresholdEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.FalseTargetThresh = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0, data[38], "False Target Threshold Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Percent Good Minimum.
//        /// </summary>
//        [Test]
//        public void TestFalseTargetThresholdEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.FalseTargetThresh = 255;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(255, data[38], "False Target Threshold Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Spare

//        #region Decode

//        /// <summary>
//        /// Test decoding the Spare.
//        /// </summary>
//        [Test]
//        public void TestSpareDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[39] = 75;
//            fl.Decode(data);
//            Assert.AreEqual(75, fl.Spare_40, "Spare Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Spare.
//        /// </summary>
//        [Test]
//        public void TestSpareEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.Spare_40 = 75;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(75, data[39], "Spare Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Spare.
//        /// </summary>
//        [Test]
//        public void TestSpareEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.Spare_40 = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0, data[39], "Spare Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Spare.
//        /// </summary>
//        [Test]
//        public void TestSpareEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.Spare_40 = 5;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(5, data[39], "Spare Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Transmit Lag Distance

//        #region Decode

//        /// <summary>
//        /// Test decoding the Transmit Lag Distance.
//        /// </summary>
//        [Test]
//        public void TestTransmitLagDistanceDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[40] = 0x75;
//            data[41] = 0x25;
//            fl.Decode(data);
//            Assert.AreEqual(9589, fl.TransmitLagDistance, "Transmit Lag Distance Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Transmit Lag Distance.
//        /// </summary>
//        [Test]
//        public void TestTransmitLagDistanceEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.TransmitLagDistance = 9589;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x75, data[40], "Transmit Lag Distance LSB Encode is incorrect.");
//            Assert.AreEqual(0x25, data[41], "Transmit Lag Distance MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Transmit Lag Distance.
//        /// </summary>
//        [Test]
//        public void TestTransmitLagDistanceEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.TransmitLagDistance = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x00, data[40], "Transmit Lag Distance LSB Encode is incorrect.");
//            Assert.AreEqual(0x00, data[41], "Transmit Lag Distance MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Transmit Lag Distance.
//        /// </summary>
//        [Test]
//        public void TestTransmitLagDistanceEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.TransmitLagDistance = 65535;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0xFF, data[40], "Transmit Lag Distance LSB Encode is incorrect.");
//            Assert.AreEqual(0xFF, data[41], "Transmit Lag Distance MSB Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region CPU Board Serial Number

//        #region Decode

//        /// <summary>
//        /// Test decoding the CPU Board Serial Number.
//        /// </summary>
//        [Test]
//        public void TestCpuBoardSerialDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[42] = 0x68;        // LSB
//            data[43] = 0x65;
//            data[44] = 0x6c;
//            data[45] = 0x6f;
//            data[46] = 0x77;
//            data[47] = 0x72;
//            data[48] = 0x6c;
//            data[49] = 0x64;        // MSB
//            fl.Decode(data);
//            Assert.AreEqual("010000000000000000000000104101108111119114108100", fl.CpuBoardSerialNumber, "CPU Board Serial Number Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the CPU Board Serial Number.
//        /// </summary>
//        [Test]
//        public void TestCpuBoardSerialEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.CpuBoardSerialNumber = "helowrld";
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x68, data[42], "CPU Board Serial Number LSB Encode is incorrect.");
//            Assert.AreEqual(0x65, data[43], "CPU Board Serial Number Encode is incorrect.");
//            Assert.AreEqual(0x6c, data[44], "CPU Board Serial Number Encode is incorrect.");
//            Assert.AreEqual(0x6f, data[45], "CPU Board Serial Number Encode is incorrect.");
//            Assert.AreEqual(0x77, data[46], "CPU Board Serial Number Encode is incorrect.");
//            Assert.AreEqual(0x72, data[47], "CPU Board Serial Number Encode is incorrect.");
//            Assert.AreEqual(0x6c, data[48], "CPU Board Serial Number Encode is incorrect.");
//            Assert.AreEqual(0x64, data[49], "CPU Board Serial Number MSB Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region System Bandwidth

//        #region Decode

//        /// <summary>
//        /// Test decoding the System Bandwidth.
//        /// </summary>
//        [Test]
//        public void TestSystemBandwidthDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[50] = 0x75;
//            data[51] = 0x25;
//            fl.Decode(data);
//            Assert.AreEqual(9589, fl.SystemBandwidth, "System Bandwidth Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the System Bandwidth.
//        /// </summary>
//        [Test]
//        public void TestSystemBandwidthEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.SystemBandwidth = 9589;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x75, data[50], "System Bandwidth LSB Encode is incorrect.");
//            Assert.AreEqual(0x25, data[51], "System Bandwidth MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the System Bandwidth.
//        /// </summary>
//        [Test]
//        public void TestSystemBandwidthEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.SystemBandwidth = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x00, data[50], "System Bandwidth LSB Encode is incorrect.");
//            Assert.AreEqual(0x00, data[51], "System Bandwidth MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the System Bandwidth.
//        /// </summary>
//        [Test]
//        public void TestSystemBandwidthEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.SystemBandwidth = 1;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x01, data[50], "System Bandwidth LSB Encode is incorrect.");
//            Assert.AreEqual(0x00, data[51], "System Bandwidth MSB Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region System Power

//        #region Decode

//        /// <summary>
//        /// Test decoding the System Power.
//        /// </summary>
//        [Test]
//        public void TestSystemPowerDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[52] = 0xC8;
//            fl.Decode(data);
//            Assert.AreEqual(200, fl.SystemPower, "System Power Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the System Power.
//        /// </summary>
//        [Test]
//        public void TestSystemPowerEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.SystemPower = 200;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0xC8, data[52], "System Power Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the System Power.
//        /// </summary>
//        [Test]
//        public void TestSystemPowerEncodeMin()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.SystemPower = 0;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x00, data[52], "System Power Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the System Power.
//        /// </summary>
//        [Test]
//        public void TestSystemPowerEncodeMax()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.SystemPower = 255;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0xFF, data[52], "System Power Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Base Frequency Index

//        #region Decode

//        /// <summary>
//        /// Test decoding the Base Frequency Index.
//        /// </summary>
//        [Test]
//        public void TestBaseFreqIndexDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[53] = 0xC8;
//            fl.Decode(data);
//            Assert.AreEqual(200, fl.BaseFrequencyIndex, "Base Frequency Index Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Base Frequency Index.
//        /// </summary>
//        [Test]
//        public void TestBaseFreqIndexEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.BaseFrequencyIndex = 200;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0xC8, data[53], "Base Frequency Index Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Instrument Serial Number

//        #region Decode

//        /// <summary>
//        /// Test decoding the Instrument Serial Number.
//        /// </summary>
//        [Test]
//        public void TestInstrumentSerialNumberDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[54] = 0x68;
//            data[55] = 0x65;
//            data[56] = 0x6C;
//            data[57] = 0x6F;
//            fl.Decode(data);
//            Assert.AreEqual("helo", fl.InstrumentSerialNumber, "Instrument Serial Number Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Instrument Serial Number.
//        /// </summary>
//        [Test]
//        public void TestInstrumentSerialNumberEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.InstrumentSerialNumber = "helo";
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0x68, data[54], "Instrument Serial Number Encode is incorrect.");
//            Assert.AreEqual(0x65, data[55], "Instrument Serial Number Encode is incorrect.");
//            Assert.AreEqual(0x6C, data[56], "Instrument Serial Number Encode is incorrect.");
//            Assert.AreEqual(0x6F, data[57], "Instrument Serial Number Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Beam Angle

//        #region Decode

//        /// <summary>
//        /// Test decoding the Beam Angle.
//        /// </summary>
//        [Test]
//        public void TestBeamAngleDecode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            byte[] data = new byte[59];

//            // Decode the data
//            data[58] = 0xC8;
//            fl.Decode(data);
//            Assert.AreEqual(200, fl.BeamAngle, "Beam Angle Decode is incorrect.");
//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Beam Angle.
//        /// </summary>
//        [Test]
//        public void TestBeamAngleEncode()
//        {
//            Pd0FixedLeader fl = new Pd0FixedLeader();

//            fl.BeamAngle = 200;
//            byte[] data = fl.Encode();
//            Assert.AreEqual(0xC8, data[58], "Beam Angle Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTI Decode

//        /// <summary>
//        /// Test decoding RTI Ensemble and Ancillary data to PD0 Fixed Leader.
//        /// </summary>
//        [Test]
//        public void DecodeRtiTest()
//        {
//            DataSet.EnsembleDataSet ens = new DataSet.EnsembleDataSet(DataSet.Ensemble.DATATYPE_INT,                        // Type of data stored (Float or Int)
//                                                                            30,                                             // Number of bins
//                                                                            4,                                              // Number of beams
//                                                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
//                                                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
//                                                                            DataSet.Ensemble.EnsembleDataID, 30, 4);               // Dataset ID
//            DataSet.AncillaryDataSet anc = new DataSet.AncillaryDataSet(DataSet.Ensemble.DATATYPE_FLOAT,                    // Type of data stored (Float or Int)
//                                                                            30,                                             // Number of bins
//                                                                            4,                                              // Number of beams
//                                                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
//                                                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
//                                                                            DataSet.Ensemble.AncillaryID);                  // Dataset ID

//            DataSet.SystemSetupDataSet ss = new DataSet.SystemSetupDataSet();

//            ens.SysFirmware = new Firmware(04, 01, 02, 03);
//            ens.SubsystemConfig = new SubsystemConfiguration(new Subsystem(04), 0, 0);
//            anc.Roll = 60;      // Upward
//            ens.NumBeams = 4;
//            ens.NumBins = 30;
//            ens.ActualPingCount = 2;
//            anc.BinSize = 22.22f;
//            anc.FirstBinRange = 33.33f;
//            ens.SysSerialNumber = new SerialNumber("01400000000000000000000000000080");

//            Pd0FixedLeader fl = new Pd0FixedLeader(ens, anc, ss, PD0.CoordinateTransforms.Coord_Beam);

//            Assert.AreEqual(ens.SysFirmware.FirmwareMajor, fl.CpuFirmwareVersion, "Major firmware is incorrect.");
//            Assert.AreEqual(ens.SysFirmware.FirmwareMinor, fl.CpuFirmwareRevision, "Minor firmware is incorrect.");
//            Assert.AreEqual(Pd0FixedLeader.SystemFrequency.Freq_300kHz, fl.GetSystemFrequency(), "System Frequency is incorrect.");
//            Assert.IsTrue(fl.Is20DegreeBeamAngle(), "20 degree beam angle is incorrect.");
//            Assert.AreEqual(Pd0FixedLeader.BeamConfigs.BeamConfig_4_Beam_Janus, fl.GetBeamConfiguration(), "4 Beam configuration is incorrect.");
//            Assert.IsTrue(fl.IsConvex(), "IsConvex is incorrect.");
//            Assert.IsTrue(fl.IsSensorConfig1(), "IsSensorConfig1 is incorrect.");
//            Assert.IsTrue(fl.IsHeadAttached(), "IsHeadAttached is incorrect.");
//            Assert.IsTrue(fl.IsBeamsUpwards(), "IsBeamsUpward is incorrect.");
//            Assert.IsFalse(fl.RealSimFlag, "Real Sim is incorrect.");
//            Assert.AreEqual(0, fl.LagLength, "Lag Length is incorrect.");
//            Assert.AreEqual(ens.NumBeams, fl.NumberOfBeams, "Number of beam is incorrect.");
//            Assert.AreEqual(ens.NumBins, fl.NumberOfCells, "Number of cells in incorrect.");
//            Assert.AreEqual(ens.ActualPingCount, fl.PingsPerEnsemble, "Pings per ensemble is incorrect.");
//            Assert.AreEqual(anc.BinSize * 100, fl.DepthCellLength, 0.001f, "Depth cell length is incorrect.");
//            Assert.AreEqual(0, fl.BlankAfterTransmit, "Blank after transmit is incorrect.");
//            Assert.AreEqual(0, fl.ProfilingMode, "Profiling Mode is incorrect.");
//            Assert.AreEqual(0, fl.LowCorrThresh, "Low Correlation Threashold is incorrect.");
//            Assert.AreEqual(0, fl.NumCodeRepeats, "Num of Code Repeats is incorrect.");
//            Assert.AreEqual(0, fl.PercentGoodMinimum, "Percent good minimum is incorrect.");
//            Assert.AreEqual(0, fl.ErrorVelMaximum, "Error velocity maximum is incorrect.");
//            Assert.AreEqual(0, fl.TimeBetweenPingMinutes, "Time Between Ping Minutes is incorrect.");
//            Assert.AreEqual(0, fl.TimeBetweenPingSeconds, "Time between ping Seconds is incorrect.");
//            Assert.AreEqual(0, fl.TimeBetweenPingHundredths, "Time Between Ping Hundredth is incorrect.");
//            Assert.AreEqual(PD0.CoordinateTransforms.Coord_Beam, fl.GetCoordinateTransform(), "Coordinate Transform is incorrect.");
//            Assert.AreEqual(0, fl.HeadingAlignment, "Heading alignment is incorrect.");
//            Assert.AreEqual(0, fl.HeadingBias, "Heading Bias is incorrect.");
//            Assert.AreEqual(0x5d, fl.SensorSource, "sensor source is incorrect.");
//            Assert.IsTrue(fl.IsUseSpeedOfSoundSensor(), "IsUseSpeedOfSoundSensor is incorrect.");
//            Assert.IsFalse(fl.IsUseXdcrDepthSensor(), "IsUsedXdcrDepthSensor is incorrect.");
//            Assert.IsTrue(fl.IsUseXdcrHeadingSensor(), "IsUseXdcrHeadingSensor is incorrect.");
//            Assert.IsTrue(fl.IsUseXdcrPitchSensor(), "IsUseXdcrPitchSensor is incorrect.");
//            Assert.IsTrue(fl.IsUseXdcrRollSensor(), "IsUseXdcrRollSensor is incorrect.");
//            Assert.IsFalse(fl.IsUseConductivitySensor(), "IsUseConductivitySensor is incorrect.");
//            Assert.IsTrue(fl.IsUseXdcrTempSensor(), "IsUseXdcrTempSensor is incorrect.");
//            Assert.AreEqual(0x5d, fl.SensorsAvailable, "sensor available is incorrect.");
//            Assert.IsFalse(fl.IsXdcrDepthSensorAvailable(), "IsXdcrDepthSensorAvailable is incorrect.");
//            Assert.IsTrue(fl.IsXdcrHeadingSensorAvailable(), "IsXdcrHeadingSensorAvailable is incorrect.");
//            Assert.IsTrue(fl.IsXdcrPitchSensorAvailable(), "IsXdcrPitchSensorAvailable is incorrect.");
//            Assert.IsTrue(fl.IsXdcrRollSensorAvailable(), "IsXdcrRollSensorAvailable is incorrect.");
//            Assert.IsFalse(fl.IsConductivitySensorAvailable(), "IsConductivitySensorAvailable is incorrect.");
//            Assert.IsTrue(fl.IsXdcrTempSensorAvailable(), "IsXdcrTempSensorAvailable is incorrect.");
//            Assert.AreEqual(anc.FirstBinRange * 100, fl.Bin1Distance, 0.001f, "Bin 1 Distance is incorrect.");
//            Assert.AreEqual(0, fl.XmitPulseLength, "Xmit pulse length is incorrect.");
//            Assert.AreEqual(0, fl.ReferenceLayerAverageStartCell, "Reference Layer start cell is incorrect.");
//            Assert.AreEqual(0, fl.ReferenceLayerAverageEndCell, "Reference Layer end cell is incorrect.");
//            Assert.AreEqual(0, fl.FalseTargetThresh, "False Target Thresh is incorrect.");
//            Assert.AreEqual(0xfe, fl.Spare_40, "Spare is incorrect.");
//            Assert.AreEqual(0, fl.TransmitLagDistance, "Transmit lag distance is incorrect.");
//            Assert.AreEqual("01400000000000000000000000000080", fl.CpuBoardSerialNumber, "Cpu Board serial number is incorrect.");
//            Assert.AreEqual(12, fl.SystemBandwidth, "System bandwidth is incorrect.");
//            Assert.AreEqual(0, fl.SystemPower, "System power is incorrect.");
//            Assert.AreEqual(0, fl.BaseFrequencyIndex, "Base frequency index is incorrect.");
//            Assert.AreEqual(0, fl.BeamAngle, "Beam angle is incorrect.");
//        }

//        #endregion
//    }



//}
