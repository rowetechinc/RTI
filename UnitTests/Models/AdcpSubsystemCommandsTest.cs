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
 * 09/25/2012      RC          2.15       Initial coding
 * 09/26/2012      RC          2.15       Added WP commands.
 * 10/04/2012      RC          2.15       Added CBI command.
 * 10/16/2012      RC          2.15       Added at least Minimum values to all values.
 * 10/17/2012      RC          2.15       Added Max values for the CWPAP commands.
 * 09/11/2013      RC          2.19.5     Updated test to 2.19.5
 * 09/17/2013      RC          2.20.0     Updated test to 2.20.0 with latest broadband modes.
 *
 */

namespace RTI
{
    using NUnit.Framework;
    using RTI.Commands;
    using System.Collections.Generic;

    /// <summary>
    /// Test the Adcp Subsystem Commands.
    /// </summary>
    [TestFixture]
    public class AdcpSubsystemCommandsTest
    {

        #region Test Defaults

        /// <summary>
        /// Test default values for a 38kHz system.
        /// </summary>
        [Test]
        public void TestDefaults38()
        {
            // Create the commands with the correct frequency in Subsystem
            Subsystem ss = new Subsystem(Subsystem.SUB_38KHZ_VERT_PISTON_F, 1);
            AdcpSubsystemCommands commands = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPON, commands.CWPON, "CWPON is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBB_TRANSMITPULSETYPE, commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWPBB_LAGLENGTH, commands.CWPBB_LagLength, "CWPBB Lag Length is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWPAP_NUMOFPINGSAVG, commands.CWPAP_NumPingsAvg, "CWPAP Num Pings Averaged is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWPAP_LAG, commands.CWPAP_Lag, "CWPAP Lag is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWPAP_BLANK, commands.CWPAP_Blank, "CWPAP Blank is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWPAP_BINSIZE, commands.CWPAP_BinSize, "CWPAP Bin Size is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWPAP_TIME_BETWEEN_PINGS, commands.CWPAP_TimeBetweenPing, "CWPAP Time between Pings is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBP_NUM_PING_AVG, commands.CWPBP_NumPingsAvg, "CWPBP Num Pings Averaged is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWPBP_WATER_BASE_PING_TIME, commands.CWPBP_TimeBetweenBasePings, "CWPBP Time Between Base Ping is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_CORR_THRESH, commands.CWPST_CorrelationThresh, "CWPST Correlation is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_QVEL_THRESH, commands.CWPST_QVelocityThresh, "CWPST Q velocity is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_VVEL_THRESH, commands.CWPST_VVelocityThresh, "CWPST V Velocity is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWPBL, commands.CWPBL, "CWPBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWPBS, commands.CWPBS, "CWPBS is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWPX, commands.CWPX, "CWPX is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWPBN, commands.CWPBN, "CWPBN is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWPP, commands.CWPP, "CWPP is incorrect.");
            Assert.AreEqual(new TimeValue(), commands.CWPAI, "CWPAI is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWPTBP, commands.CWPTBP, "CWPTBP is incorrect.");

            Assert.AreEqual(new TimeValue(), commands.CBI_BurstInterval, "CBI Burst Interval is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBI_NUM_ENS, commands.CBI_NumEnsembles, "CBI Number of Ensemble is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTON, commands.CBTON, "CBTON is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_MODE, commands.CBTBB_Mode, "CBTBB Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CBTBB_LONGRANGEDEPTH, commands.CBTBB_LongRangeDepth, "CBTBB Long Range Depth is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_PULSETOPULSE_LAG, commands.CBTBB_PulseToPulseLag, "CBTBB Pulse to Pulse Lag is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_CORR_THRESH, commands.CBTST_CorrelationThresh, "CBTST Correlation is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_QVEL_THRESHOLD, commands.CBTST_QVelocityThresh, "CBTST Q Velocity is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_VVEL_THRESHOLD, commands.CBTST_VVelocityThresh, "CBTST V Velocity is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CBTBL, commands.CBTBL, "CBTBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CBTMX, commands.CBTMX, "CBTMX is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CBTTBP, commands.CBTTBP, "CBTTBP is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_SHALLOW_DET_THRESHOLD, commands.CBTT_SNRShallowDetectionThresh, "CBTT SNR is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CBTT_DEPTH_SNR, commands.CBTT_DepthSNR, "CBTT Depth SNR is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_DEEP_DET_THRESHOLD, commands.CBTT_SNRDeepDetectionThresh, "CBTT SNR Deep Depth Detection Threshold is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CBTT_DEPTH_GAIN, commands.CBTT_DepthGain, "CBTT Depth Gain is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWTON, commands.CWTON, "CWTON is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWTBB, commands.CWTBB, "CWTBB is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWTBL, commands.CWTBL, "CWTBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWTBS, commands.CWTBS, "CWTBS is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_38_CWTTBP, commands.CWTTBP, "CWTTBP is incorrect.");
        }

        /// <summary>
        /// Test default values for a 75kHz system.
        /// </summary>
        [Test]
        public void TestDefaults75()
        {
            // Create the commands with the correct frequency in Subsystem
            Subsystem ss = new Subsystem(Subsystem.SUB_75KHZ_4BEAM_30DEG_ARRAY_L, 1);
            AdcpSubsystemCommands commands = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPON, commands.CWPON, "CWPON is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBB_TRANSMITPULSETYPE, commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWPBB_LAGLENGTH, commands.CWPBB_LagLength, "CWPBB Lag Length is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWPAP_NUMOFPINGSAVG, commands.CWPAP_NumPingsAvg, "CWPAP Num Pings Averaged is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWPAP_LAG, commands.CWPAP_Lag, "CWPAP Lag is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWPAP_BLANK, commands.CWPAP_Blank, "CWPAP Blank is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWPAP_BINSIZE, commands.CWPAP_BinSize, "CWPAP Bin Size is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWPAP_TIME_BETWEEN_PINGS, commands.CWPAP_TimeBetweenPing, "CWPAP Time between Pings is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBP_NUM_PING_AVG, commands.CWPBP_NumPingsAvg, "CWPBP Num Pings Averaged is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWPBP_WATER_BASE_PING_TIME, commands.CWPBP_TimeBetweenBasePings, "CWPBP Time Between Base Ping is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_CORR_THRESH, commands.CWPST_CorrelationThresh, "CWPST Correlation is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_QVEL_THRESH, commands.CWPST_QVelocityThresh, "CWPST Q velocity is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_VVEL_THRESH, commands.CWPST_VVelocityThresh, "CWPST V Velocity is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWPBL, commands.CWPBL, "CWPBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWPBS, commands.CWPBS, "CWPBS is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWPX, commands.CWPX, "CWPX is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWPBN, commands.CWPBN, "CWPBN is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWPP, commands.CWPP, "CWPP is incorrect.");
            Assert.AreEqual(new TimeValue(), commands.CWPAI, "CWPAI is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWPTBP, commands.CWPTBP, "CWPTBP is incorrect.");

            Assert.AreEqual(new TimeValue(), commands.CBI_BurstInterval, "CBI Burst Interval is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBI_NUM_ENS, commands.CBI_NumEnsembles, "CBI Number of Ensemble is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTON, commands.CBTON, "CBTON is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_MODE, commands.CBTBB_Mode, "CBTBB Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CBTBB_LONGRANGEDEPTH, commands.CBTBB_LongRangeDepth, "CBTBB Long Range Depth is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_PULSETOPULSE_LAG, commands.CBTBB_PulseToPulseLag, "CBTBB Pulse to Pulse Lag is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_CORR_THRESH, commands.CBTST_CorrelationThresh, "CBTST Correlation is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_QVEL_THRESHOLD, commands.CBTST_QVelocityThresh, "CBTST Q Velocity is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_VVEL_THRESHOLD, commands.CBTST_VVelocityThresh, "CBTST V Velocity is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CBTBL, commands.CBTBL, "CBTBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CBTMX, commands.CBTMX, "CBTMX is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CBTTBP, commands.CBTTBP, "CBTTBP is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_SHALLOW_DET_THRESHOLD, commands.CBTT_SNRShallowDetectionThresh, "CBTT SNR is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CBTT_DEPTH_SNR, commands.CBTT_DepthSNR, "CBTT Depth SNR is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_DEEP_DET_THRESHOLD, commands.CBTT_SNRDeepDetectionThresh, "CBTT SNR Deep Depth Detection Threshold is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CBTT_DEPTH_GAIN, commands.CBTT_DepthGain, "CBTT Depth Gain is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWTON, commands.CWTON, "CWTON is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWTBB, commands.CWTBB, "CWTBB is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWTBL, commands.CWTBL, "CWTBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWTBS, commands.CWTBS, "CWTBS is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_75_CWTTBP, commands.CWTTBP, "CWTTBP is incorrect.");
        }

        /// <summary>
        /// Test default values for a 150kHz system.
        /// </summary>
        [Test]
        public void TestDefaults150()
        {
            // Create the commands with the correct frequency in Subsystem
            Subsystem ss = new Subsystem(Subsystem.SUB_150KHZ_VERT_PISTON_D, 1);
            AdcpSubsystemCommands commands = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPON, commands.CWPON, "CWPON is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBB_TRANSMITPULSETYPE, commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWPBB_LAGLENGTH, commands.CWPBB_LagLength, "CWPBB Lag Length is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWPAP_NUMOFPINGSAVG, commands.CWPAP_NumPingsAvg, "CWPAP Num Pings Averaged is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWPAP_LAG, commands.CWPAP_Lag, "CWPAP Lag is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWPAP_BLANK, commands.CWPAP_Blank, "CWPAP Blank is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWPAP_BINSIZE, commands.CWPAP_BinSize, "CWPAP Bin Size is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWPAP_TIME_BETWEEN_PINGS, commands.CWPAP_TimeBetweenPing, "CWPAP Time between Pings is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBP_NUM_PING_AVG, commands.CWPBP_NumPingsAvg, "CWPBP Num Pings Averaged is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWPBP_WATER_BASE_PING_TIME, commands.CWPBP_TimeBetweenBasePings, "CWPBP Time Between Base Ping is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_CORR_THRESH, commands.CWPST_CorrelationThresh, "CWPST Correlation is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_QVEL_THRESH, commands.CWPST_QVelocityThresh, "CWPST Q velocity is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_VVEL_THRESH, commands.CWPST_VVelocityThresh, "CWPST V Velocity is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWPBL, commands.CWPBL, "CWPBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWPBS, commands.CWPBS, "CWPBS is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWPX, commands.CWPX, "CWPX is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWPBN, commands.CWPBN, "CWPBN is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWPP, commands.CWPP, "CWPP is incorrect.");
            Assert.AreEqual(new TimeValue(), commands.CWPAI, "CWPAI is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWPTBP, commands.CWPTBP, "CWPTBP is incorrect.");

            Assert.AreEqual(new TimeValue(), commands.CBI_BurstInterval, "CBI Burst Interval is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBI_NUM_ENS, commands.CBI_NumEnsembles, "CBI Number of Ensemble is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTON, commands.CBTON, "CBTON is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_MODE, commands.CBTBB_Mode, "CBTBB Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CBTBB_LONGRANGEDEPTH, commands.CBTBB_LongRangeDepth, "CBTBB Long Range Depth is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_PULSETOPULSE_LAG, commands.CBTBB_PulseToPulseLag, "CBTBB Pulse to Pulse Lag is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_CORR_THRESH, commands.CBTST_CorrelationThresh, "CBTST Correlation is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_QVEL_THRESHOLD, commands.CBTST_QVelocityThresh, "CBTST Q Velocity is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_VVEL_THRESHOLD, commands.CBTST_VVelocityThresh, "CBTST V Velocity is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CBTBL, commands.CBTBL, "CBTBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CBTMX, commands.CBTMX, "CBTMX is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CBTTBP, commands.CBTTBP, "CBTTBP is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_SHALLOW_DET_THRESHOLD, commands.CBTT_SNRShallowDetectionThresh, "CBTT SNR is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CBTT_DEPTH_SNR, commands.CBTT_DepthSNR, "CBTT Depth SNR is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_DEEP_DET_THRESHOLD, commands.CBTT_SNRDeepDetectionThresh, "CBTT SNR Deep Depth Detection Threshold is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CBTT_DEPTH_GAIN, commands.CBTT_DepthGain, "CBTT Depth Gain is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWTON, commands.CWTON, "CWTON is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWTBB, commands.CWTBB, "CWTBB is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWTBL, commands.CWTBL, "CWTBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWTBS, commands.CWTBS, "CWTBS is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_150_CWTTBP, commands.CWTTBP, "CWTTBP is incorrect.");
        }

        /// <summary>
        /// Test default values for a 300kHz system.
        /// </summary>
        [Test]
        public void TestDefaults300()
        {
            // Create the commands with the correct frequency in Subsystem
            Subsystem ss = new Subsystem(Subsystem.SUB_300KHZ_VERT_PISTON_C, 1);
            AdcpSubsystemCommands commands = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPON, commands.CWPON, "CWPON is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBB_TRANSMITPULSETYPE, commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWPBB_LAGLENGTH, commands.CWPBB_LagLength, "CWPBB Lag Length is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWPAP_NUMOFPINGSAVG, commands.CWPAP_NumPingsAvg, "CWPAP Num Pings Averaged is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWPAP_LAG, commands.CWPAP_Lag, "CWPAP Lag is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWPAP_BLANK, commands.CWPAP_Blank, "CWPAP Blank is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWPAP_BINSIZE, commands.CWPAP_BinSize, "CWPAP Bin Size is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWPAP_TIME_BETWEEN_PINGS, commands.CWPAP_TimeBetweenPing, "CWPAP Time between Pings is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBP_NUM_PING_AVG, commands.CWPBP_NumPingsAvg, "CWPBP Num Pings Averaged is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWPBP_WATER_BASE_PING_TIME, commands.CWPBP_TimeBetweenBasePings, "CWPBP Time Between Base Ping is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_CORR_THRESH, commands.CWPST_CorrelationThresh, "CWPST Correlation is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_QVEL_THRESH, commands.CWPST_QVelocityThresh, "CWPST Q velocity is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_VVEL_THRESH, commands.CWPST_VVelocityThresh, "CWPST V Velocity is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWPBL, commands.CWPBL, "CWPBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWPBS, commands.CWPBS, "CWPBS is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWPX, commands.CWPX, "CWPX is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWPBN, commands.CWPBN, "CWPBN is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWPP, commands.CWPP, "CWPP is incorrect.");
            Assert.AreEqual(new TimeValue(), commands.CWPAI, "CWPAI is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWPTBP, commands.CWPTBP, "CWPTBP is incorrect.");

            Assert.AreEqual(new TimeValue(), commands.CBI_BurstInterval, "CBI Burst Interval is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBI_NUM_ENS, commands.CBI_NumEnsembles, "CBI Number of Ensemble is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTON, commands.CBTON, "CBTON is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_MODE, commands.CBTBB_Mode, "CBTBB Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CBTBB_LONGRANGEDEPTH, commands.CBTBB_LongRangeDepth, "CBTBB Long Range Depth is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_PULSETOPULSE_LAG, commands.CBTBB_PulseToPulseLag, "CBTBB Pulse to Pulse Lag is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_CORR_THRESH, commands.CBTST_CorrelationThresh, "CBTST Correlation is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_QVEL_THRESHOLD, commands.CBTST_QVelocityThresh, "CBTST Q Velocity is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_VVEL_THRESHOLD, commands.CBTST_VVelocityThresh, "CBTST V Velocity is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CBTBL, commands.CBTBL, "CBTBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CBTMX, commands.CBTMX, "CBTMX is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CBTTBP, commands.CBTTBP, "CBTTBP is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_SHALLOW_DET_THRESHOLD, commands.CBTT_SNRShallowDetectionThresh, "CBTT SNR is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CBTT_DEPTH_SNR, commands.CBTT_DepthSNR, "CBTT Depth SNR is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_DEEP_DET_THRESHOLD, commands.CBTT_SNRDeepDetectionThresh, "CBTT SNR Deep Depth Detection Threshold is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CBTT_DEPTH_GAIN, commands.CBTT_DepthGain, "CBTT Depth Gain is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWTON, commands.CWTON, "CWTON is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWTBB, commands.CWTBB, "CWTBB is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWTBL, commands.CWTBL, "CWTBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWTBS, commands.CWTBS, "CWTBS is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_300_CWTTBP, commands.CWTTBP, "CWTTBP is incorrect.");
        }

        /// <summary>
        /// Test default values for a 600kHz system.
        /// </summary>
        [Test]
        public void TestDefaults600()
        {
            // Create the commands with the correct frequency in Subsystem
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands commands = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPON, commands.CWPON, "CWPON is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBB_TRANSMITPULSETYPE, commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBB_LAGLENGTH, commands.CWPBB_LagLength, "CWPBB Lag Length is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_NUMOFPINGSAVG, commands.CWPAP_NumPingsAvg, "CWPAP Num Pings Averaged is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_LAG, commands.CWPAP_Lag, "CWPAP Lag is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_BLANK, commands.CWPAP_Blank, "CWPAP Blank is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_BINSIZE, commands.CWPAP_BinSize, "CWPAP Bin Size is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_TIME_BETWEEN_PINGS, commands.CWPAP_TimeBetweenPing, "CWPAP Time between Pings is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBP_NUM_PING_AVG, commands.CWPBP_NumPingsAvg, "CWPBP Num Pings Averaged is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBP_WATER_BASE_PING_TIME, commands.CWPBP_TimeBetweenBasePings, "CWPBP Time Between Base Ping is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_CORR_THRESH, commands.CWPST_CorrelationThresh, "CWPST Correlation is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_QVEL_THRESH, commands.CWPST_QVelocityThresh, "CWPST Q velocity is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_VVEL_THRESH, commands.CWPST_VVelocityThresh, "CWPST V Velocity is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBL, commands.CWPBL, "CWPBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBS, commands.CWPBS, "CWPBS is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPX, commands.CWPX, "CWPX is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBN, commands.CWPBN, "CWPBN is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPP, commands.CWPP, "CWPP is incorrect.");
            Assert.AreEqual(new TimeValue(), commands.CWPAI, "CWPAI is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPTBP, commands.CWPTBP, "CWPTBP is incorrect.");

            Assert.AreEqual(new TimeValue(), commands.CBI_BurstInterval, "CBI Burst Interval is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBI_NUM_ENS, commands.CBI_NumEnsembles, "CBI Number of Ensemble is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTON, commands.CBTON, "CBTON is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_MODE, commands.CBTBB_Mode, "CBTBB Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTBB_LONGRANGEDEPTH, commands.CBTBB_LongRangeDepth, "CBTBB Long Range Depth is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_PULSETOPULSE_LAG, commands.CBTBB_PulseToPulseLag, "CBTBB Pulse to Pulse Lag is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_CORR_THRESH, commands.CBTST_CorrelationThresh, "CBTST Correlation is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_QVEL_THRESHOLD, commands.CBTST_QVelocityThresh, "CBTST Q Velocity is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_VVEL_THRESHOLD, commands.CBTST_VVelocityThresh, "CBTST V Velocity is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTBL, commands.CBTBL, "CBTBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTMX, commands.CBTMX, "CBTMX is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTTBP, commands.CBTTBP, "CBTTBP is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_SHALLOW_DET_THRESHOLD, commands.CBTT_SNRShallowDetectionThresh, "CBTT SNR is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTT_DEPTH_SNR, commands.CBTT_DepthSNR, "CBTT Depth SNR is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_DEEP_DET_THRESHOLD, commands.CBTT_SNRDeepDetectionThresh, "CBTT SNR Deep Depth Detection Threshold is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTT_DEPTH_GAIN, commands.CBTT_DepthGain, "CBTT Depth Gain is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWTON, commands.CWTON, "CWTON is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWTBB, commands.CWTBB, "CWTBB is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWTBL, commands.CWTBL, "CWTBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWTBS, commands.CWTBS, "CWTBS is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWTTBP, commands.CWTTBP, "CWTTBP is incorrect.");
        }

        /// <summary>
        /// Test default values for a 1200kHz system.
        /// </summary>
        [Test]
        public void TestDefaults1200()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_1_2MHZ_VERT_PISTON_A, 1);
            AdcpSubsystemCommands commands = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPON, commands.CWPON, "CWPON is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBB_TRANSMITPULSETYPE, commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWPBB_LAGLENGTH, commands.CWPBB_LagLength, "CWPBB Lag Length is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWPAP_NUMOFPINGSAVG, commands.CWPAP_NumPingsAvg, "CWPAP Num Pings Averaged is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWPAP_LAG, commands.CWPAP_Lag, "CWPAP Lag is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWPAP_BLANK, commands.CWPAP_Blank, "CWPAP Blank is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWPAP_BINSIZE, commands.CWPAP_BinSize, "CWPAP Bin Size is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWPAP_TIME_BETWEEN_PINGS, commands.CWPAP_TimeBetweenPing, "CWPAP Time between Pings is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBP_NUM_PING_AVG, commands.CWPBP_NumPingsAvg, "CWPBP Num Pings Averaged is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWPBP_WATER_BASE_PING_TIME, commands.CWPBP_TimeBetweenBasePings, "CWPBP Time Between Base Ping is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_CORR_THRESH, commands.CWPST_CorrelationThresh, "CWPST Correlation is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_QVEL_THRESH, commands.CWPST_QVelocityThresh, "CWPST Q velocity is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_VVEL_THRESH, commands.CWPST_VVelocityThresh, "CWPST V Velocity is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWPBL, commands.CWPBL, "CWPBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWPBS, commands.CWPBS, "CWPBS is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWPX, commands.CWPX, "CWPX is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWPBN, commands.CWPBN, "CWPBN is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWPP, commands.CWPP, "CWPP is incorrect.");
            Assert.AreEqual(new TimeValue(), commands.CWPAI, "CWPAI is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWPTBP, commands.CWPTBP, "CWPTBP is incorrect.");

            Assert.AreEqual(new TimeValue(), commands.CBI_BurstInterval, "CBI Burst Interval is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBI_NUM_ENS, commands.CBI_NumEnsembles, "CBI Number of Ensemble is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTON, commands.CBTON, "CBTON is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_MODE, commands.CBTBB_Mode, "CBTBB Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CBTBB_LONGRANGEDEPTH, commands.CBTBB_LongRangeDepth, "CBTBB Long Range Depth is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_PULSETOPULSE_LAG, commands.CBTBB_PulseToPulseLag, "CBTBB Pulse to Pulse Lag is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_CORR_THRESH, commands.CBTST_CorrelationThresh, "CBTST Correlation is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_QVEL_THRESHOLD, commands.CBTST_QVelocityThresh, "CBTST Q Velocity is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_VVEL_THRESHOLD, commands.CBTST_VVelocityThresh, "CBTST V Velocity is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CBTBL, commands.CBTBL, "CBTBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CBTMX, commands.CBTMX, "CBTMX is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CBTTBP, commands.CBTTBP, "CBTTBP is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_SHALLOW_DET_THRESHOLD, commands.CBTT_SNRShallowDetectionThresh, "CBTT SNR is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CBTT_DEPTH_SNR, commands.CBTT_DepthSNR, "CBTT Depth SNR is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_DEEP_DET_THRESHOLD, commands.CBTT_SNRDeepDetectionThresh, "CBTT SNR Deep Depth Detection Threshold is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CBTT_DEPTH_GAIN, commands.CBTT_DepthGain, "CBTT Depth Gain is incorrect.");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWTON, commands.CWTON, "CWTON is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWTBB, commands.CWTBB, "CWTBB is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWTBL, commands.CWTBL, "CWTBL is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWTBS, commands.CWTBS, "CWTBS is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_1200_CWTTBP, commands.CWTTBP, "CWTTBP is incorrect.");
        }

        #endregion

        #region Test Commands

        #region CWPON

        /// <summary>
        /// Test setting the CWPON command.
        /// </summary>
        [Test]
        public void TestCWPON()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPON = true;

            Assert.AreEqual(true, ssc.CWPON, "CWPON is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPON command.
        /// </summary>
        [Test]
        public void TestCWPON1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPON = false;

            Assert.AreEqual(false, ssc.CWPON, "CWPON is incorrect.");
        }

        #endregion

        #region CWPBB

        #region Pulse Type

        /// <summary>
        /// Test setting the CWPBB command.
        /// </summary>
        [Test]
        public void TestCWPBB()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBB_LagLength = 0.10f;
            ssc.CWPBB_TransmitPulseType = AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE;

            Assert.AreEqual(0.10f, ssc.CWPBB_LagLength, "CWPBB Lag Length is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE, ssc.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBB Pulse type with bad command.
        /// </summary>
        [Test]
        public void TestCWPBB_SetBad()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.SetCwpbbTransmitPulseType("garabage");

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBB_TRANSMITPULSETYPE, ssc.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBB Pulse type with string command.
        /// </summary>
        [Test]
        public void TestCWPBB_SetBB()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.SetCwpbbTransmitPulseType(AdcpSubsystemCommands.TRANSMIT_PULSE_TYPE_BROADBAND);

            Assert.AreEqual(AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, ssc.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBB Pulse type with string command.
        /// </summary>
        [Test]
        public void TestCWPBB_SetNB()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.SetCwpbbTransmitPulseType(AdcpSubsystemCommands.TRANSMIT_PULSE_TYPE_NARROWBAND);

            Assert.AreEqual(AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NARROWBAND, ssc.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBB Pulse type with string command.
        /// </summary>
        [Test]
        public void TestCWPBB_SetP2PC()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.SetCwpbbTransmitPulseType(AdcpSubsystemCommands.TRANSMIT_PULSE_TYPE_BB_PTP);

            Assert.AreEqual(AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE, ssc.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBB Pulse type with string command.
        /// </summary>
        [Test]
        public void TestCWPBB_SetP2PNC()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.SetCwpbbTransmitPulseType(AdcpSubsystemCommands.TRANSMIT_PULSE_TYPE_NONCODED_BB_PTP);

            Assert.AreEqual(AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NONCODED_BROADBAND_PULSE_TO_PULSE, ssc.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type is incorrect.");
        }

        /// <summary>
        /// Test Getting the CWPBB Pulse type command.
        /// </summary>
        [Test]
        public void TestCWPBB_GetDefault()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            string type = ssc.GetCwpbbTransmitPulseType();

            Assert.AreEqual(AdcpSubsystemCommands.TRANSMIT_PULSE_TYPE_BROADBAND, type, "CWPBB Transmit Pulse Type is incorrect.");
        }

        /// <summary>
        /// Test Getting the CWPBB Pulse type command.
        /// </summary>
        [Test]
        public void TestCWPBB_GetNB()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBB_TransmitPulseType = AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NARROWBAND;
            string type = ssc.GetCwpbbTransmitPulseType();

            Assert.AreEqual(AdcpSubsystemCommands.TRANSMIT_PULSE_TYPE_NARROWBAND, type, "CWPBB Transmit Pulse Type is incorrect.");
        }

        /// <summary>
        /// Test Getting the CWPBB Pulse type command.
        /// </summary>
        [Test]
        public void TestCWPBB_GetBB()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBB_TransmitPulseType = AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND;
            string type = ssc.GetCwpbbTransmitPulseType();

            Assert.AreEqual(AdcpSubsystemCommands.TRANSMIT_PULSE_TYPE_BROADBAND, type, "CWPBB Transmit Pulse Type is incorrect.");
        }

        /// <summary>
        /// Test Getting the CWPBB Pulse type command.
        /// </summary>
        [Test]
        public void TestCWPBB_GetBBP2PC()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBB_TransmitPulseType = AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE;
            string type = ssc.GetCwpbbTransmitPulseType();

            Assert.AreEqual(AdcpSubsystemCommands.TRANSMIT_PULSE_TYPE_BB_PTP, type, "CWPBB Transmit Pulse Type is incorrect.");
        }

        /// <summary>
        /// Test Getting the CWPBB Pulse type command.
        /// </summary>
        [Test]
        public void TestCWPBB_GetBBP2PNC()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBB_TransmitPulseType = AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NONCODED_PULSE_TO_PULSE;
            string type = ssc.GetCwpbbTransmitPulseType();

            Assert.AreEqual(AdcpSubsystemCommands.TRANSMIT_PULSE_TYPE_NONCODED_PTP, type, "CWPBB Transmit Pulse Type is incorrect.");
        }

        #endregion

        #region Lag Length

        /// <summary>
        /// Test setting the CWPBB Lag Length command.
        /// </summary>
        [Test]
        public void TestCWPBB_LL()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBB_LagLength = 0.05f;

            Assert.AreEqual(0.05f, ssc.CWPBB_LagLength, "CWPBB Lag Length is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBB Lag Length command.
        /// </summary>
        [Test]
        public void TestCWPBB_LLBad()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBB_LagLength = -10;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBB_LAGLENGTH, ssc.CWPBB_LagLength, "CWPBB Lag Length is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBB Lag Length command.
        /// </summary>
        [Test]
        public void TestCWPBB_LLBadMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBB_LagLength = -10;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBB_LAGLENGTH, ssc.CWPBB_LagLength, "CWPBB Lag Length is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBB Lag Length command.
        /// </summary>
        [Test]
        public void TestCWPBB_LLBadMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBB_LagLength = 10;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBB_LAGLENGTH, ssc.CWPBB_LagLength, "CWPBB Lag Length is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBB Lag Length command.
        /// </summary>
        [Test]
        public void TestCWPBB_LLCornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBB_LagLength = AdcpSubsystemCommands.MIN_CWPBB_LAGLENGTH;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPBB_LAGLENGTH, ssc.CWPBB_LagLength, "CWPBB Lag Length is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBB Lag Length command.
        /// </summary>
        [Test]
        public void TestCWPBB_LLCornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBB_LagLength = AdcpSubsystemCommands.MAX_CWPBB_LAGLENGTH;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPBB_LAGLENGTH, ssc.CWPBB_LagLength, "CWPBB Lag Length is incorrect.");
        }

        #endregion

        #endregion

        #region CWPST

        /// <summary>
        /// Test setting the CWPST command.
        /// </summary>
        [Test]
        public void TestCWPST()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPST_CorrelationThresh = 0.3256f;
            ssc.CWPST_QVelocityThresh = 0.125f;
            ssc.CWPST_VVelocityThresh = 0.15645f;

            Assert.AreEqual(0.3256f, ssc.CWPST_CorrelationThresh, "CWPST Correlation Threshold is incorrect.");
            Assert.AreEqual(0.125f, ssc.CWPST_QVelocityThresh, "CWPST Q Velocity Threshold is incorrect.");
            Assert.AreEqual(0.15645f, ssc.CWPST_VVelocityThresh, "CWPST V Velocity Threshold is incorrect.");
        }

        #region Correlatioin

        /// <summary>
        /// Give a bad Correlation Threshold.
        /// Range is 0.0 to 1.0.
        /// </summary>
        [Test]
        public void TestCWPST_BadCorrThreshMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPST_CorrelationThresh = -0.2f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_CORR_THRESH, ssc.CWPST_CorrelationThresh, "CWPAP Correlation Threshold is incorrect.");
        }

        /// <summary>
        /// Give a bad Correlation Threshold.
        /// Range is 0.0 to 1.0.
        /// </summary>
        [Test]
        public void TestCWPST_BadCorreThreshMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPST_CorrelationThresh = 1.2f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_CORR_THRESH, ssc.CWPST_CorrelationThresh, "CWPAP Correlation Threshold is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CWPST Correlation Threshold.
        /// Range is 0.0 to 1.0.
        /// </summary>
        [Test]
        public void TestCWPST_BadCorreThreshCornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPST_CorrelationThresh = AdcpSubsystemCommands.MIN_CWPST_CORR_THRESH;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPST_CORR_THRESH, ssc.CWPST_CorrelationThresh, "CWPAP Correlation Threshold is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CWPST Correlation Threshold.
        /// Range is 0.0 to 1.0.
        /// </summary>
        [Test]
        public void TestCWPST_BadCorreThreshCornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPST_CorrelationThresh = AdcpSubsystemCommands.MAX_CWPST_CORR_THRESH;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPST_CORR_THRESH, ssc.CWPST_CorrelationThresh, "CWPAP Correlation Threshold is incorrect.");
        }

        #endregion

        #region Q Velocity

        /// <summary>
        /// Test setting the CWPST command.
        /// </summary>
        [Test]
        public void TestCWPST_QVelBad()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPST_QVelocityThresh = -0.125f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_QVEL_THRESH, ssc.CWPST_QVelocityThresh, "CWPST Q Velocity Threshold is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPST command.
        /// </summary>
        [Test]
        public void TestCWPST_QVelMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPST_QVelocityThresh = AdcpSubsystemCommands.MIN_CWPST_Q_VELOCITY_THRESH;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPST_Q_VELOCITY_THRESH, ssc.CWPST_QVelocityThresh, "CWPST Q Velocity Threshold is incorrect.");
        }

        #endregion

        #region V Velocity

        /// <summary>
        /// Test setting the CWPST command.
        /// </summary>
        [Test]
        public void TestCWPST_VVelBad()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPST_VVelocityThresh = -0.125f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPST_VVEL_THRESH, ssc.CWPST_VVelocityThresh, "CWPST V Velocity Threshold is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPST command.
        /// </summary>
        [Test]
        public void TestCWPST_VVelMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPST_VVelocityThresh = AdcpSubsystemCommands.MIN_CWPST_V_VELOCITY_THRESH;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPST_V_VELOCITY_THRESH, ssc.CWPST_VVelocityThresh, "CWPST V Velocity Threshold is incorrect.");
        }

        #endregion

        #endregion

        #region CWPBL

        /// <summary>
        /// Test setting the CWPBL command.
        /// </summary>
        [Test]
        public void TestCWPBL()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBL = 15.3f;

            Assert.AreEqual(15.3f, ssc.CWPBL, "CWPBL is incorrect.");
        }

        /// <summary>
        /// Give a bad Water Profile Blank.
        /// Range is 0.00 to 100.00
        /// </summary>
        [Test]
        public void TestCWPBL_BadCWPBLMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBL = -0.2f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBL, ssc.CWPBL, "CWPBL is incorrect.");
        }

        /// <summary>
        /// Give a bad CWPBL.
        /// Range is 0.00 to 100.00
        /// </summary>
        [Test]
        public void TestCWPBL_BadCWPBLMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBL = 120.23f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBL, ssc.CWPBL, "CWPBL is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CWPBL Correlation Threshold.
        /// Range is 0.00 to 100.00
        /// </summary>
        [Test]
        public void TestCWPBL_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBL = AdcpSubsystemCommands.MIN_CWPBL;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPBL, ssc.CWPBL, "CWPBL is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CWPBL Correlation Threshold.
        /// Range is 0.00 to 100.00
        /// </summary>
        [Test]
        public void TestCWPBL_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBL = AdcpSubsystemCommands.MAX_CWPBL;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPBL, ssc.CWPBL, "CWPBL is incorrect.");
        }

        #endregion

        #region CWPBS

        /// <summary>
        /// Test setting the CWPBS command.
        /// </summary>
        [Test]
        public void TestCWPBS()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBS = 1.235f;

            Assert.AreEqual(1.235f, ssc.CWPBS, "CWPBS is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBS command.
        /// </summary>
        [Test]
        public void TestCWPBS_Min()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBS = -1000.235f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBS, ssc.CWPBS, "CWPBS is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBS command.
        /// </summary>
        [Test]
        public void TestCWPBS_Max()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBS = 1000.235f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBS, ssc.CWPBS, "CWPBS is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBS command.
        /// </summary>
        [Test]
        public void TestCWPBS_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBS = AdcpSubsystemCommands.MIN_CWPBS;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPBS, ssc.CWPBS, "CWPBS is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBS command.
        /// </summary>
        [Test]
        public void TestCWPBS_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBS = AdcpSubsystemCommands.MAX_CWPBS;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPBS, ssc.CWPBS, "CWPBS is incorrect.");
        }

        #endregion

        #region CWPX

        /// <summary>
        /// Test setting the CWPX command.
        /// </summary>
        [Test]
        public void TestCWPX()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPX = 1.235f;

            Assert.AreEqual(1.235f, ssc.CWPX, "CWPX is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPX command.
        /// </summary>
        [Test]
        public void TestCWPX_Min()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPX = -1.235f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPX, ssc.CWPX, "CWPX is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPX command.
        /// </summary>
        [Test]
        public void TestCWPX_Max()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPX = 1000.235f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPX, ssc.CWPX, "CWPX is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPX command.
        /// </summary>
        [Test]
        public void TestCWPX_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPX = AdcpSubsystemCommands.MIN_CWPX;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPX, ssc.CWPX, "CWPX is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPX command.
        /// </summary>
        [Test]
        public void TestCWPX_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPX = AdcpSubsystemCommands.MAX_CWPX;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPX, ssc.CWPX, "CWPX is incorrect.");
        }

        #endregion

        #region CWPBN

        /// <summary>
        /// Test setting the CWPBN command.
        /// </summary>
        [Test]
        public void TestCWPBN()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBN = 15;

            Assert.AreEqual(15, ssc.CWPBN, "CWPBN is incorrect.");
        }

        /// <summary>
        /// Give a bad CWPBN.
        /// Range is 0 to 200.
        /// </summary>
        [Test]
        public void TestCWPBN_BadMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBN = 222;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBN, ssc.CWPBN, "CWPBN is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CWPBN Correlation Threshold.
        /// Range is 0 to 200.
        /// </summary>
        [Test]
        public void TestCWPBN_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBN = AdcpSubsystemCommands.MIN_CWPBN;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPBN, ssc.CWPBN, "CWPBN is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CWPBN Correlation Threshold.
        /// Range is 0 to 200.
        /// </summary>
        [Test]
        public void TestCWPBN_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBN = AdcpSubsystemCommands.MAX_CWPBN;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPBN, ssc.CWPBN, "CWPBN is incorrect.");
        }

        #endregion

        #region CWPP

        /// <summary>
        /// Test setting the CWPP command.
        /// </summary>
        [Test]
        public void TestCWPP()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPP = 15;

            Assert.AreEqual(15, ssc.CWPP, "CWPP is incorrect.");
        }

        /// <summary>
        /// Give a bad CWPP.
        /// Range is 0 to 10,000.
        /// </summary>
        [Test]
        public void TestCWPP_BadMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPP = 22222;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPP, ssc.CWPP, "CWPP is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CWPP.
        /// Range is 0 to 10,000.
        /// </summary>
        [Test]
        public void TestCWPP_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPP = AdcpSubsystemCommands.MIN_CWPP;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPP, ssc.CWPP, "CWPP is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CWPP.
        /// Range is 0 to 10,000.
        /// </summary>
        [Test]
        public void TestCWPP_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPP = AdcpSubsystemCommands.MAX_CWPP;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPP, ssc.CWPP, "CWPP is incorrect.");
        }

        #endregion

        #region CWPBP

        /// <summary>
        /// Test setting the CWPBP command.
        /// </summary>
        [Test]
        public void TestCWPBP()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBP_NumPingsAvg = 15;
            ssc.CWPBP_TimeBetweenBasePings = 0.4f;

            Assert.AreEqual(15, ssc.CWPBP_NumPingsAvg, "CWPBP Num Pings Avg is incorrect.");
            Assert.AreEqual(0.4f, ssc.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP Time Between Pings is incorrect.");
        }

        #region Ping Avg

        /// <summary>
        /// Give a bad CWPBP Num Pings Avg.
        /// Range is 0 to 100.
        /// </summary>
        [Test]
        public void TestCWPBP_BadCWPBP_NumPingsAvgMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBP_NumPingsAvg = 22222;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CWPBP_NUM_PING_AVG, ssc.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CWPBP_NumPingsAvg.
        /// Range is 0 to 100.
        /// </summary>
        [Test]
        public void TestCWPBP_CornerCWPBP_NumPingsAvgMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBP_NumPingsAvg = AdcpSubsystemCommands.MIN_CWPBP_NUM_PING;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPBP_NUM_PING, ssc.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CWPBP_NumPingsAvg.
        /// Range is 0 to 100.
        /// </summary>
        [Test]
        public void TestCWPBP_CornerCWPBP_NumPingsAvgMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBP_NumPingsAvg = AdcpSubsystemCommands.MAX_CWPBP_NUM_PING;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPBP_NUM_PING, ssc.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg is incorrect.");
        }

        #endregion

        #region Time Between Pings

        /// <summary>
        /// Test setting the CWPBP command.
        /// </summary>
        [Test]
        public void TestCWPBP_TimeBetweenPings_Bad()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBP_TimeBetweenBasePings = -0.4f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPBP_WATER_BASE_PING_TIME, ssc.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP Time Between Pings is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPBP command.
        /// </summary>
        [Test]
        public void TestCWPBP_TimeBetweenPings_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPBP_TimeBetweenBasePings = AdcpSubsystemCommands.MIN_CWPBP_TIME_BETWEEN_PINGS;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPBP_TIME_BETWEEN_PINGS, ssc.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP Time Between Pings is incorrect.");
        }

        #endregion

        #endregion

        #region CWPTBP

        /// <summary>
        /// Test setting the CWPTBP command.
        /// </summary>
        [Test]
        public void TestCWPTBP()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPTBP = 15.004f;

            Assert.AreEqual(15.004, ssc.CWPTBP, 0.0001, "CWPTBP is incorrect.");
        }

        /// <summary>
        /// Give a bad CWPTBP.
        /// Range is 0 to 86400.00.
        /// </summary>
        [Test]
        public void TestCWPTBP_BadMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPTBP = -154.235f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPTBP, ssc.CWPTBP, 0.0001, "CWPTBP is incorrect.");
        }

        /// <summary>
        /// Give a bad CWPTBP.
        /// Range is 0 to 86400.00.
        /// </summary>
        [Test]
        public void TestCWPTBP_BadMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPTBP = 86500.004f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPTBP, ssc.CWPTBP, 0.0001, "CWPTBP is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CWPTBP.
        /// Range is 0 to 86400.00.
        /// </summary>
        [Test]
        public void TestCWPTBP_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPTBP = AdcpSubsystemCommands.MIN_CWPTBP;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPTBP, ssc.CWPTBP, "CWPTBP is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CWPTBP.
        /// Range is 0 to 86400.00.
        /// </summary>
        [Test]
        public void TestCWPTBP_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPTBP = AdcpSubsystemCommands.MAX_CWPTBP;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPTBP, ssc.CWPTBP, "CWPTBP is incorrect.");
        }

        #endregion

        #region CWPAI

        /// <summary>
        /// Test setting the CWPAI command.
        /// </summary>
        [Test]
        public void TestCWPAI()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAI = new TimeValue(1,2,3,4);

            Assert.AreEqual(new TimeValue(1, 2, 3, 4), ssc.CWPAI, "CWPAI is incorrect.");
        }

        /// <summary>
        /// Test corner case Minutes for CWPAI.
        /// </summary>
        [Test]
        public void TestCWPAI_CornerMinute()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAI = new TimeValue(1, 60, 3, 4);

            Assert.AreEqual(new TimeValue(2, 0, 3, 4), ssc.CWPAI, "CWPAI is incorrect.");
        }

        /// <summary>
        /// Test corner case Seconds for CWPAI.
        /// </summary>
        [Test]
        public void TestCWPAI_CornerSeconds()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAI = new TimeValue(1, 17, 63, 4);

            Assert.AreEqual(new TimeValue(1, 18, 3, 4), ssc.CWPAI, "CWPAI is incorrect.");
        }

        /// <summary>
        /// Test corner case HunSeconds for CWPAI.
        /// </summary>
        [Test]
        public void TestCWPAI_CornerHunSeconds()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAI = new TimeValue(1, 17, 33, 124);

            Assert.AreEqual(new TimeValue(1, 17, 34, 24), ssc.CWPAI, "CWPAI is incorrect.");
        }

        /// <summary>
        /// Test corner case all values over for CWPAI.
        /// </summary>
        [Test]
        public void TestCWPAI_CornerOver()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAI = new TimeValue(502, 97, 83, 124);

            Assert.AreEqual(new TimeValue(503, 38, 24, 24), ssc.CWPAI, "CWPAI is incorrect.");
        }

        #endregion

        #region CWPAP

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_NumPingsAvg = 50;
            ssc.CWPAP_Lag = 0.33f;
            ssc.CWPAP_Blank = 1.44f;
            ssc.CWPAP_BinSize = 2.55f;
            ssc.CWPAP_TimeBetweenPing = 3.66f;

            Assert.AreEqual(50, ssc.CWPAP_NumPingsAvg, "CWPAP Num Pings Avg is incorrect.");
            Assert.AreEqual(0.33f, ssc.CWPAP_Lag, "CWPAP Lag is incorrect.");
            Assert.AreEqual(1.44f, ssc.CWPAP_Blank, "CWPAP Blank is incorrect.");
            Assert.AreEqual(2.55f, ssc.CWPAP_BinSize, "CWPAP Bin Size is incorrect.");
            Assert.AreEqual(3.66f, ssc.CWPAP_TimeBetweenPing, "CWPAP Time Between Pings is incorrect.");
        }

        #region Num Pings

        /// <summary>
        /// Give a bad Num Pings Average.
        /// Range is 0 to 100.
        /// </summary>
        [Test]
        public void TestCWPAP_BadNumPingsAvgMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_NumPingsAvg = 0;

            Assert.AreEqual(0, ssc.CWPAP_NumPingsAvg, "CWPAP Num Pings Avg is incorrect.");
        }

        /// <summary>
        /// Give a bad Num Pings Average.
        /// Range is 0 to 100.
        /// </summary>
        [Test]
        public void TestCWPAP_BadNumPingsAvgMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_NumPingsAvg = 110;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_NUMOFPINGSAVG, ssc.CWPAP_NumPingsAvg, "CWPAP Num Pings Avg is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CWPAP Number of Pings.
        /// Range is 0 to 100.
        /// </summary>
        [Test]
        public void TestCWPAP_NumPing_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_NumPingsAvg = AdcpSubsystemCommands.MIN_CWPAP_NUMPINGS;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPAP_NUMPINGS, ssc.CWPAP_NumPingsAvg, "CWPAP Number of Pings is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CWPAP Number of Pings.
        /// 
        /// Range is 0.00 to 100.00
        /// </summary>
        [Test]
        public void TestCWPAP_NumPing_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_NumPingsAvg = AdcpSubsystemCommands.MAX_CWPAP_NUMPINGS;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPAP_NUMPINGS, ssc.CWPAP_NumPingsAvg, "CWPAP Number of Pings is incorrect.");
        }

        #endregion

        #region Lag

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_Lag_Min()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_Lag = -0.33f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_LAG, ssc.CWPAP_Lag, "CWPAP Lag is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_Lag_Max()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_Lag = 23.33f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_LAG, ssc.CWPAP_Lag, "CWPAP Lag is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_Lag_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_Lag = AdcpSubsystemCommands.MIN_CWPAP_LAG;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPAP_LAG, ssc.CWPAP_Lag, "CWPAP Lag is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_Lag_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_Lag = AdcpSubsystemCommands.MAX_CWPAP_LAG;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPAP_LAG, ssc.CWPAP_Lag, "CWPAP Lag is incorrect.");
        }

        #endregion

        #region Blank

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_Blank_Min()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_Blank = -1.44f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_BLANK, ssc.CWPAP_Blank, "CWPAP_Blank is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_Blank_Max()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_Blank = 100000.44f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_BLANK, ssc.CWPAP_Blank, "CWPAP_Blank is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_Blank_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_Blank = AdcpSubsystemCommands.MIN_CWPAP_BLANK;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPAP_BLANK, ssc.CWPAP_Blank, "CWPAP_Blank is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_Blank_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_Blank = AdcpSubsystemCommands.MAX_CWPAP_BLANK;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPAP_BLANK, ssc.CWPAP_Blank, "CWPAP_Blank is incorrect.");
        }

        #endregion

        #region Bin Size

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_BinSize_Min()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_BinSize = -2.55f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_BINSIZE, ssc.CWPAP_BinSize, "CWPAP Bin Size is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_BinSize_Max()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_BinSize = 20000000.55f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_BINSIZE, ssc.CWPAP_BinSize, "CWPAP Bin Size is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_BinSize_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_BinSize = AdcpSubsystemCommands.MIN_CWPAP_BINSIZE;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPAP_BINSIZE, ssc.CWPAP_BinSize, "CWPAP_BinSize is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_BinSize_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_BinSize = AdcpSubsystemCommands.MAX_CWPAP_BINSIZE;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPAP_BINSIZE, ssc.CWPAP_BinSize, "CWPAP_BinSize is incorrect.");
        }

        #endregion

        #region Time Between Ping

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_TimeBetweenPing_Min()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_TimeBetweenPing = -3.66f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_BINSIZE, ssc.CWPAP_TimeBetweenPing, "CWPAP_TimeBetweenPing is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_TimeBetweenPing_Max()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_TimeBetweenPing = 30000000.66f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWPAP_BINSIZE, ssc.CWPAP_TimeBetweenPing, "CWPAP_TimeBetweenPing is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_TimeBetweenPing_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_TimeBetweenPing = AdcpSubsystemCommands.MIN_CWPAP_TIME_BETWEEN_PING;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWPAP_TIME_BETWEEN_PING, ssc.CWPAP_TimeBetweenPing, "CWPAP_TimeBetweenPing is incorrect.");
        }

        /// <summary>
        /// Test setting the CWPAP command.
        /// </summary>
        [Test]
        public void TestCWPAP_TimeBetweenPing_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWPAP_TimeBetweenPing = AdcpSubsystemCommands.MAX_CWPAP_TIME_BETWEEN_PING;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWPAP_TIME_BETWEEN_PING, ssc.CWPAP_TimeBetweenPing, "CWPAP_TimeBetweenPing is incorrect.");
        }

        #endregion

        #endregion

        #region CBI

        #region Burst Interval

        /// <summary>
        /// Test setting the CBI Burst Interval command.
        /// </summary>
        [Test]
        public void TestCBI_BurstInterval()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBI_BurstInterval = new TimeValue(1, 2, 3, 4);

            Assert.AreEqual(new TimeValue(1, 2, 3, 4), ssc.CBI_BurstInterval, "CBI_BurstInterval is incorrect.");
        }

        /// <summary>
        /// Test corner case Minutes for CBI_BurstInterval.
        /// </summary>
        [Test]
        public void TestCBI_BurstInterval_CornerMinute()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBI_BurstInterval = new TimeValue(1, 60, 3, 4);

            Assert.AreEqual(new TimeValue(2, 0, 3, 4), ssc.CBI_BurstInterval, "CBI_BurstInterval is incorrect.");
        }

        /// <summary>
        /// Test corner case Seconds for CBI_BurstInterval.
        /// </summary>
        [Test]
        public void TestCBI_BurstInterval_CornerSeconds()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBI_BurstInterval = new TimeValue(1, 17, 63, 4);

            Assert.AreEqual(new TimeValue(1, 18, 3, 4), ssc.CBI_BurstInterval, "CBI_BurstInterval is incorrect.");
        }

        /// <summary>
        /// Test corner case HunSeconds for CBI_BurstInterval.
        /// </summary>
        [Test]
        public void TestCBI_BurstInterval_CornerHunSeconds()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBI_BurstInterval = new TimeValue(1, 17, 33, 124);

            Assert.AreEqual(new TimeValue(1, 17, 34, 24), ssc.CBI_BurstInterval, "CBI_BurstInterval is incorrect.");
        }

        /// <summary>
        /// Test corner case all values over for CBI_BurstInterval.
        /// </summary>
        [Test]
        public void TestCBI_BurstInterval_CornerOver()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBI_BurstInterval = new TimeValue(502, 97, 83, 124);

            Assert.AreEqual(new TimeValue(503, 38, 24, 24), ssc.CBI_BurstInterval, "CBI_BurstInterval is incorrect.");
        }

        #endregion

        #region Number of Ensembles

        /// <summary>
        /// Test setting the CBI_NumEnsembles command.
        /// </summary>
        [Test]
        public void TestCBI_NumEnsembles()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBI_NumEnsembles = 15;

            Assert.AreEqual(15, ssc.CBI_NumEnsembles, "CBI_NumEnsembles is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CBI_NumEnsembles.
        /// Range is 0 to N.
        /// </summary>
        [Test]
        public void TestCBI_NumEnsembles_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBI_NumEnsembles = AdcpSubsystemCommands.MIN_CBI_NUM_ENS;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CBI_NUM_ENS, ssc.CBI_NumEnsembles, "CBI_NumEnsembles is incorrect.");
        }

        #endregion

        #endregion

        #region CBTON

        /// <summary>
        /// Test setting the CBTON command.
        /// </summary>
        [Test]
        public void TestCBTON()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTON = true;

            Assert.AreEqual(true, ssc.CBTON, "CBTON is incorrect.");
        }

        /// <summary>
        /// Test setting the CBTON command.
        /// </summary>
        [Test]
        public void TestCBTON1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTON = false;

            Assert.AreEqual(false, ssc.CBTON, "CBTON is incorrect.");
        }

        #endregion

        #region CBTBB

        /// <summary>
        /// Test getting the CBTBB command.
        /// </summary>
        [Test]
        public void TestCBTBB()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED;
            ssc.CBTBB_LongRangeDepth = 1.231f;
            ssc.CBTBB_PulseToPulseLag = 34.5f;

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
            Assert.AreEqual(1.231f, ssc.CBTBB_LongRangeDepth, "CBTBB_LongRangeDepth is incorrect.");
            Assert.AreEqual(34.5f, ssc.CBTBB_PulseToPulseLag, "CBTBB_PulseToPulseLag is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB command.
        /// </summary>
        [Test]
        public void TestCBTBB_Default()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_MODE, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTBB_LONGRANGEDEPTH, ssc.CBTBB_LongRangeDepth, "CBTBB_LongRangeDepth is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_PULSETOPULSE_LAG, ssc.CBTBB_PulseToPulseLag, "CBTBB_PulseToPulseLag is incorrect.");
        }


        #region Mode

        /// <summary>
        /// Test getting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_GetModeDefault()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            string type = ssc.GetCBTBB_Mode();

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.BT_BB_MODE_NARROWBAND, type, "CBTBB_Mode Get is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_GetModeNB()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE;
            string type = ssc.GetCBTBB_Mode();

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.BT_BB_MODE_NARROWBAND, type, "CBTBB_Mode Get is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_GetModeBBC()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED;
            string type = ssc.GetCBTBB_Mode();

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.BT_BB_MODE_BROADBAND_CODED, type, "CBTBB_Mode Get is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_GetModeBBNC()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED;
            string type = ssc.GetCBTBB_Mode();

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.BT_BB_MODE_BROADBAND_NONCODED, type, "CBTBB_Mode Get is incorrect.");
        }


        /// <summary>
        /// Test getting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_GetModeBBNCP2P()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED_P2P;
            string type = ssc.GetCBTBB_Mode();

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED_P2P, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.BT_BB_MODE_BROADBAND_NONCODED_P2P, type, "CBTBB_Mode Get is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_GetModeAutoSwitch()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.AUTO_SWITCH_NARROWBAND_BB_NONCODED_BB_NONCODED_P2P;
            string type = ssc.GetCBTBB_Mode();

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.AUTO_SWITCH_NARROWBAND_BB_NONCODED_BB_NONCODED_P2P, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.BT_BB_MODE_NB_BBNONCODED_BBNONCODEDP2P, type, "CBTBB_Mode Get is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_GetModeNA3()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.NA_3;
            string type = ssc.GetCBTBB_Mode();

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NA_3, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.BT_BB_MODE_NA_3, type, "CBTBB_Mode Get is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_GetModeNA5()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.NA_5;
            string type = ssc.GetCBTBB_Mode();

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NA_5, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.BT_BB_MODE_NA_5, type, "CBTBB_Mode Get is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_GetModeNA6()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.NA_6;
            string type = ssc.GetCBTBB_Mode();

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NA_6, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.BT_BB_MODE_NA_6, type, "CBTBB_Mode Get is incorrect.");
        }

        /// <summary>
        /// Test setting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_SetModeNB()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.SetCBTBB_Mode(AdcpSubsystemCommands.BT_BB_MODE_NARROWBAND);

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        /// <summary>
        /// Test setting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_SetModeBBC()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.SetCBTBB_Mode(AdcpSubsystemCommands.BT_BB_MODE_BROADBAND_CODED);

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        /// <summary>
        /// Test setting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_SetModeBBNC()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.SetCBTBB_Mode(AdcpSubsystemCommands.BT_BB_MODE_BROADBAND_NONCODED);

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        /// <summary>
        /// Test setting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_SetModeBBNCP2P()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.SetCBTBB_Mode(AdcpSubsystemCommands.BT_BB_MODE_BROADBAND_NONCODED_P2P);

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED_P2P, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        /// <summary>
        /// Test setting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_SetModeAutoSwitch()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.SetCBTBB_Mode(AdcpSubsystemCommands.BT_BB_MODE_NB_BBNONCODED_BBNONCODEDP2P);

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.AUTO_SWITCH_NARROWBAND_BB_NONCODED_BB_NONCODED_P2P, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        /// <summary>
        /// Test setting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_SetModeNA3()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.SetCBTBB_Mode(AdcpSubsystemCommands.BT_BB_MODE_NA_3);

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NA_3, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        /// <summary>
        /// Test setting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_SetModeNA5()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.SetCBTBB_Mode(AdcpSubsystemCommands.BT_BB_MODE_NA_5);

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NA_5, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        /// <summary>
        /// Test setting the CBTBB Mode command.
        /// </summary>
        [Test]
        public void TestCBTBB_SetModeNA6()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.SetCBTBB_Mode(AdcpSubsystemCommands.BT_BB_MODE_NA_6);

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NA_6, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        #endregion

        #region Modes

        /// <summary>
        /// Test getting the CBTBB command.
        /// </summary>
        [Test]
        public void TestCBTBB_Mode_NB()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE;
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB command.
        /// </summary>
        [Test]
        public void TestCBTBB_Mode_BB()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED;
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB command.
        /// </summary>
        [Test]
        public void TestCBTBB_Mode_BBNC()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED;
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB command.
        /// </summary>
        [Test]
        public void TestCBTBB_Mode_BBNCP2P()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED_P2P;
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED_P2P, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB command.
        /// </summary>
        [Test]
        public void TestCBTBB_Mode_NA3()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.NA_3;
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NA_3, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB command.
        /// </summary>
        [Test]
        public void TestCBTBB_Mode_NA5()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.NA_5;
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NA_5, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB command.
        /// </summary>
        [Test]
        public void TestCBTBB_Mode_NA6()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.NA_6;
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NA_6, ssc.CBTBB_Mode, "CBTBB_Mode is incorrect.");
        }

        #endregion

        #region Pulse to Pulse Lag

        /// <summary>
        /// Test getting the CBTBB command.
        /// </summary>
        [Test]
        public void TestCBTBB_P2P_Lag_Bad()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_PulseToPulseLag = -34.5f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTBB_PULSETOPULSE_LAG, ssc.CBTBB_PulseToPulseLag, "CBTBB_PulseToPulseLag is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB command.
        /// </summary>
        [Test]
        public void TestCBTBB_P2P_Lag_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_PulseToPulseLag = AdcpSubsystemCommands.MIN_CBTBB_PULSETOPULSE_LAG;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CBTBB_PULSETOPULSE_LAG, ssc.CBTBB_PulseToPulseLag, "CBTBB_PulseToPulseLag is incorrect.");
        }

        #endregion

        #region Long Range Depth

        /// <summary>
        /// Test getting the CBTBB command.
        /// </summary>
        [Test]
        public void TestCBTBB_LRD_Bad()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_LongRangeDepth = -1.231f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTBB_LONGRANGEDEPTH, ssc.CBTBB_LongRangeDepth, "CBTBB_LongRangeDepth is incorrect.");
        }

        /// <summary>
        /// Test getting the CBTBB command.
        /// </summary>
        [Test]
        public void TestCBTBB_LRD_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBB_LongRangeDepth = AdcpSubsystemCommands.MIN_CBTBB_LONGRANGEDEPTH;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CBTBB_LONGRANGEDEPTH, ssc.CBTBB_LongRangeDepth, "CBTBB_LongRangeDepth is incorrect.");
        }

        #endregion

        #endregion

        #region CBTST

        /// <summary>
        /// Test setting the CBTST command.
        /// </summary>
        [Test]
        public void TestCBTST()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTST_CorrelationThresh = 0.3256f;
            ssc.CBTST_QVelocityThresh = 0.125f;
            ssc.CBTST_VVelocityThresh = 0.15645f;

            Assert.AreEqual(0.3256f, ssc.CBTST_CorrelationThresh, "CBTST Correlation Threshold is incorrect.");
            Assert.AreEqual(0.125f, ssc.CBTST_QVelocityThresh, "CBTST Correlation Threshold is incorrect.");
            Assert.AreEqual(0.15645f, ssc.CBTST_VVelocityThresh, "CBTST Correlation Threshold is incorrect.");
        }

        #region Correlation

        /// <summary>
        /// Give a bad Correlation Threshold.
        /// Range is 0.0 to 1.0.
        /// </summary>
        [Test]
        public void TestCBTST_BadCorrThreshMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTST_CorrelationThresh = -0.2f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_CORR_THRESH, ssc.CBTST_CorrelationThresh, "CBTST Correlation Threshold is incorrect.");
        }

        /// <summary>
        /// Give a bad Correlation Threshold.
        /// Range is 0.0 to 1.0.
        /// </summary>
        [Test]
        public void TestCBTST_BadCorreThreshMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTST_CorrelationThresh = 1.2f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_CORR_THRESH, ssc.CBTST_CorrelationThresh, "CBTST Correlation Threshold is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CBTST Correlation Threshold.
        /// Range is 0.0 to 1.0.
        /// </summary>
        [Test]
        public void TestCBTST_BadCorreThreshCornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTST_CorrelationThresh = AdcpSubsystemCommands.MIN_CBTST_CORR_THRESH;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CBTST_CORR_THRESH, ssc.CBTST_CorrelationThresh, "CBTST Correlation Threshold is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CBTST Correlation Threshold.
        /// Range is 0.0 to 1.0.
        /// </summary>
        [Test]
        public void TestCBTST_BadCorreThreshCornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTST_CorrelationThresh = AdcpSubsystemCommands.MAX_CBTST_CORR_THRESH;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CBTST_CORR_THRESH, ssc.CBTST_CorrelationThresh, "CBTST Correlation Threshold is incorrect.");
        }

        #endregion

        #region Q Velocity

        /// <summary>
        /// Give a bad CBTST_QVelocityThresh
        /// </summary>
        [Test]
        public void TestCBTST_QVelThreshMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTST_QVelocityThresh = -0.2f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_QVEL_THRESHOLD, ssc.CBTST_QVelocityThresh, "CBTST_QVelocityThresh is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CBTST_QVelocityThresh.
        /// </summary>
        [Test]
        public void TestCBTST_QVelThreshCornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTST_QVelocityThresh = AdcpSubsystemCommands.MIN_CBTST_QVEL_THRESH;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CBTST_QVEL_THRESH, ssc.CBTST_QVelocityThresh, "CBTST_QVelocityThresh is incorrect.");
        }

        #endregion

        #region V Velocity

        /// <summary>
        /// Give a bad CBTST_VVelocityThresh
        /// </summary>
        [Test]
        public void TestCBTST_VVelThreshMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTST_VVelocityThresh = -0.2f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTST_VVEL_THRESHOLD, ssc.CBTST_VVelocityThresh, "CBTST_VVelocityThresh is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CBTST_QVelocityThresh.
        /// </summary>
        [Test]
        public void TestCBTST_VVelThreshCornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTST_VVelocityThresh = AdcpSubsystemCommands.MIN_CBTST_VVEL_THRESH;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CBTST_VVEL_THRESH, ssc.CBTST_VVelocityThresh, "CBTST_VVelocityThresh is incorrect.");
        }

        #endregion

        #endregion

        #region CBTBL

        /// <summary>
        /// Test setting the CBTBL command.
        /// </summary>
        [Test]
        public void TestCBTBL()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBL = 1.3f;

            Assert.AreEqual(1.3f, ssc.CBTBL, "CBTBL is incorrect.");
        }

        /// <summary>
        /// Give a bad Bottom Track Blank.
        /// Range is 0.00 to 10.00
        /// </summary>
        [Test]
        public void TestCWPBL_BadCBTBLMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBL = -0.2f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTBL, ssc.CBTBL, "CBTBL is incorrect.");
        }

        /// <summary>
        /// Give a bad CBTBL.
        /// Range is 0.00 to 10.00
        /// </summary>
        [Test]
        public void TestCBTBL_BadCBTBLMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBL = 120.23f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTBL, ssc.CBTBL, "CBTBL is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CBTBL.
        /// Range is 0.00 to 10.00
        /// </summary>
        [Test]
        public void TestCBTBL_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBL = AdcpSubsystemCommands.MIN_CBTBL;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CBTBL, ssc.CBTBL, "CBTBL is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CBTBL.
        /// Range is 0.00 to 10.00
        /// </summary>
        [Test]
        public void TestCBTBL_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTBL = AdcpSubsystemCommands.MAX_CBTBL;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CBTBL, ssc.CBTBL, "CBTBL is incorrect.");
        }

        #endregion

        #region CBTMX

        /// <summary>
        /// Test setting the CBTMX command.
        /// </summary>
        [Test]
        public void TestCBTMX()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTMX = 15.3f;

            Assert.AreEqual(15.3f, ssc.CBTMX, "CBTMX is incorrect.");
        }

        /// <summary>
        /// Give a bad Bottom Track Max Depth.
        /// Range is 5 to 10,000.
        /// </summary>
        [Test]
        public void TestCWPMX_BadCBTMXMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTMX = -0.2f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTMX, ssc.CBTMX, "CBTMX is incorrect.");
        }

        /// <summary>
        /// Give a bad CBTMX.
        /// Range is 5 to 10,000.
        /// </summary>
        [Test]
        public void TestCBTMX_BadCBTMXMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTMX = 1200000.23f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTMX, ssc.CBTMX, "CBTMX is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CBTMX.
        /// Range is 5 to 10,000.
        /// </summary>
        [Test]
        public void TestCBTMX_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTMX = AdcpSubsystemCommands.MIN_CBTMX;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CBTMX, ssc.CBTMX, "CBTMX is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CBTMX.
        /// Range is 5 to 10,000.
        /// </summary>
        [Test]
        public void TestCBTMX_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTMX = AdcpSubsystemCommands.MAX_CBTMX;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CBTMX, ssc.CBTMX, "CBTMX is incorrect.");
        }

        #endregion

        #region CBTTBP

        /// <summary>
        /// Test setting the CBTTBP command.
        /// </summary>
        [Test]
        public void TestCBTTBP()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTTBP = 15.3f;

            Assert.AreEqual(15.3f, ssc.CBTTBP, "CBTTBP is incorrect.");
        }

        /// <summary>
        /// Give a bad Bottom Track Time Between Pings.
        /// Range is 0 to 86,400.
        /// </summary>
        [Test]
        public void TestCBTTBP_BadCBTTBPMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTTBP = -0.2f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTTBP, ssc.CBTTBP, "CBTTBP is incorrect.");
        }

        /// <summary>
        /// Give a bad CBTTBP.
        /// Range is 0 to 86,400.
        /// </summary>
        [Test]
        public void TestCBTTBP_BadCBTTBPMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTTBP = 1200000.23f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTTBP, ssc.CBTTBP, "CBTTBP is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CBTTBP\.
        /// Range is 0 to 86,400.
        /// </summary>
        [Test]
        public void TestCBTTBP_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTTBP = AdcpSubsystemCommands.MIN_CBTTBP;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CBTTBP, ssc.CBTTBP, "CBTTBP is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CBTTBP.
        /// Range is 0 to 86,400.
        /// </summary>
        [Test]
        public void TestCBTTBP_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTTBP = AdcpSubsystemCommands.MAX_CBTTBP;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CBTTBP, ssc.CBTTBP, "CBTTBP is incorrect.");
        }

        #endregion

        #region CBTT

        /// <summary>
        /// Test setting the CBTT command.
        /// </summary>
        [Test]
        public void TestCBTT()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTT_SNRShallowDetectionThresh = 15.3f;
            ssc.CBTT_DepthSNR = 22.345f;
            ssc.CBTT_SNRDeepDetectionThresh = 123.456f;
            ssc.CBTT_DepthGain = 654.34f;

            Assert.AreEqual(15.3f, ssc.CBTT_SNRShallowDetectionThresh, "CBTT_Snr is incorrect.");
            Assert.AreEqual(22.345f, ssc.CBTT_DepthSNR, "CBTT_DepthSNR is incorrect.");
            Assert.AreEqual(123.456f, ssc.CBTT_SNRDeepDetectionThresh, "CBTT_SNRDeepDetectionThresh is incorrect.");
            Assert.AreEqual(654.34f, ssc.CBTT_DepthGain, "CBTT_DepthGain is incorrect.");
        }

        #region SNR Shallow dB

        /// <summary>
        /// Test setting the CBTT command.
        /// </summary>
        [Test]
        public void TestCBTT_SNR_Shallow_Bad()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTT_SNRShallowDetectionThresh = -15.3f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_SHALLOW_DET_THRESHOLD, ssc.CBTT_SNRShallowDetectionThresh, "CBTT_SNRShallowDetectionThresh is incorrect.");
        }

        /// <summary>
        /// Test setting the CBTT command.
        /// </summary>
        [Test]
        public void TestCBTT_SNR_Shallow_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTT_SNRShallowDetectionThresh = AdcpSubsystemCommands.MIN_CBTT_SNR_SHALLOW_THRESH;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CBTT_SNR_SHALLOW_THRESH, ssc.CBTT_SNRShallowDetectionThresh, "CBTT_SNRShallowDetectionThresh is incorrect.");
        }

        #endregion

        #region SNR Depth

        /// <summary>
        /// Test setting the CBTT command.
        /// </summary>
        [Test]
        public void TestCBTT_SNR_SNR_Depth_Bad()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTT_DepthSNR = -15.3f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTT_DEPTH_SNR, ssc.CBTT_DepthSNR, "CBTT_DepthSNR is incorrect.");
        }

        /// <summary>
        /// Test setting the CBTT command.
        /// </summary>
        [Test]
        public void TestCBTT_SNR_Depth_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTT_DepthSNR = AdcpSubsystemCommands.MIN_CBTT_DEPTH_SNR;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CBTT_DEPTH_SNR, ssc.CBTT_DepthSNR, "CBTT_DepthSNR is incorrect.");
        }

        #endregion

        #region SNR Deep dB

        /// <summary>
        /// Test setting the CBTT command.
        /// </summary>
        [Test]
        public void TestCBTT_SNR_Deep_Bad()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTT_SNRDeepDetectionThresh = -15.3f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_CBTT_SNR_DEEP_DET_THRESHOLD, ssc.CBTT_SNRDeepDetectionThresh, "CBTT_SNRDeepDetectionThresh is incorrect.");
        }

        /// <summary>
        /// Test setting the CBTT command.
        /// </summary>
        [Test]
        public void TestCBTT_SNR_Deep_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTT_SNRDeepDetectionThresh = AdcpSubsystemCommands.MIN_CBTT_SNR_DEEP_THRESH;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CBTT_SNR_DEEP_THRESH, ssc.CBTT_SNRDeepDetectionThresh, "CBTT_SNRDeepDetectionThresh is incorrect.");
        }

        #endregion

        #region Gain Depth

        /// <summary>
        /// Test setting the CBTT command.
        /// </summary>
        [Test]
        public void TestCBTT_SNR_Gain_Depth_Bad()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTT_DepthGain = -15.3f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CBTT_DEPTH_GAIN, ssc.CBTT_DepthGain, "CBTT_DepthGain is incorrect.");
        }

        /// <summary>
        /// Test setting the CBTT command.
        /// </summary>
        [Test]
        public void TestCBTT_Gain_Depth_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CBTT_DepthGain = AdcpSubsystemCommands.MIN_CBTT_DEPTH_GAIN;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CBTT_DEPTH_GAIN, ssc.CBTT_DepthGain, "CBTT_DepthGain is incorrect.");
        }

        #endregion

        #endregion

        #region CWTON

        /// <summary>new SubsystemConfiguration(ss, 0, 0)
        /// Test setting the CWTON command.
        /// </summary>
        [Test]
        public void TestCWTON()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTON = true;

            Assert.AreEqual(true, ssc.CWTON, "CWTON is incorrect.");
        }

        /// <summary>
        /// Test setting the CWTON command.
        /// </summary>
        [Test]
        public void TestCWTON1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTON = false;

            Assert.AreEqual(false, ssc.CWTON, "CWTON is incorrect.");
        }

        #endregion

        #region CWTBB

        /// <summary>
        /// Test setting the CWTBB command.
        /// </summary>
        [Test]
        public void TestCWTBB()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTBB = true;

            Assert.AreEqual(true, ssc.CWTBB, "CWTBB is incorrect.");
        }

        /// <summary>
        /// Test setting the CWTBB command.
        /// </summary>
        [Test]
        public void TestCWTBB1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTBB = false;

            Assert.AreEqual(false, ssc.CWTBB, "CWTBB is incorrect.");
        }

        #endregion

        #region CWTBL

        /// <summary>
        /// Test setting the CWTBL command.
        /// </summary>
        [Test]
        public void TestCWTBL()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTBL = 1.3f;

            Assert.AreEqual(1.3f, ssc.CWTBL, "CWTBL is incorrect.");
        }

        /// <summary>
        /// Give a bad Water Track Blank.
        /// Range is 0.00 to 100.00
        /// </summary>
        [Test]
        public void TestCWTBL_BadCWTBLMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTBL = -0.2f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWTBL, ssc.CWTBL, "CWTBL is incorrect.");
        }

        /// <summary>
        /// Give a bad CWTBL.
        /// Range is 0.00 to 100.00
        /// </summary>
        [Test]
        public void TestCWTBL_BadCWTBLMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTBL = 120.23f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWTBL, ssc.CWTBL, "CWTBL is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CWTBL.
        /// Range is 0.00 to 100.00
        /// </summary>
        [Test]
        public void TestCWTBL_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTBL = AdcpSubsystemCommands.MIN_CWTBL;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWTBL, ssc.CWTBL, "CWTBL is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CWTBL.
        /// Range is 0.00 to 100.00
        /// </summary>
        [Test]
        public void TestCWTBL_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTBL = AdcpSubsystemCommands.MAX_CWTBL;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWTBL, ssc.CWTBL, "CWTBL is incorrect.");
        }

        #endregion

        #region CWTBS

        /// <summary>
        /// Test setting the CWTBS command.
        /// </summary>
        [Test]
        public void TestCWTBS()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTBS = 1.3f;

            Assert.AreEqual(1.3f, ssc.CWTBS, "CWTBS is incorrect.");
        }

        /// <summary>
        /// Give a bad Water Track Bin Size.
        /// </summary>
        [Test]
        public void TestCWTBS_BadCWTBSMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTBS = -0.2f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWTBS, ssc.CWTBS, "CWTBS is incorrect.");
        }

        /// <summary>
        /// Give a bad CWTBS.
        /// </summary>
        [Test]
        public void TestCWTBS_BadCWTBSMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTBS = 120.23f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWTBS, ssc.CWTBS, "CWTBS is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CWTBS.
        /// </summary>
        [Test]
        public void TestCWTBS_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTBS = AdcpSubsystemCommands.MIN_CWTBS;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWTBS, ssc.CWTBS, "CWTBS is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CWTBS.
        /// </summary>
        [Test]
        public void TestCWTBS_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTBS = AdcpSubsystemCommands.MAX_CWTBS;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWTBS, ssc.CWTBS, "CWTBS is incorrect.");
        }

        #endregion

        #region CWTTBP

        /// <summary>
        /// Test setting the CWTTBP command.
        /// </summary>
        [Test]
        public void TestCWTTBP()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTTBP = 15.004f;

            Assert.AreEqual(15.004, ssc.CWTTBP, 0.0001, "CWTTBP is incorrect.");
        }

        /// <summary>
        /// Give a bad CWTTBP.
        /// Range is 0 to 86400.00.
        /// </summary>
        [Test]
        public void TestCWTTBP_BadMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTTBP = -154.235f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWTTBP, ssc.CWTTBP, 0.0001, "CWTTBP is incorrect.");
        }

        /// <summary>
        /// Give a bad CWTTBP.
        /// Range is 0 to 86400.00.
        /// </summary>
        [Test]
        public void TestCWTTBP_BadMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTTBP = 86500.004f;

            Assert.AreEqual(AdcpSubsystemCommands.DEFAULT_600_CWTTBP, ssc.CWTTBP, 0.0001, "CWTTBP is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CWTTBP.
        /// Range is 0 to 86400.00.
        /// </summary>
        [Test]
        public void TestCCWTTBP_CornerMin()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTTBP = AdcpSubsystemCommands.MIN_CWTTBP;

            Assert.AreEqual(AdcpSubsystemCommands.MIN_CWTTBP, ssc.CWTTBP, "CWTTBP is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CWTTBP.
        /// Range is 0 to 86400.00.
        /// </summary>
        [Test]
        public void TestCWTTBP_CornerMax()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            ssc.CWTTBP = AdcpSubsystemCommands.MAX_CWTTBP;

            Assert.AreEqual(AdcpSubsystemCommands.MAX_CWTTBP, ssc.CWTTBP, "CWTTBP is incorrect.");
        }

        #endregion

        #endregion

        #region To String

        /// <summary>
        /// Test the ToString output.
        /// </summary>
        [Test]
        public void TestToString()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            string result = ssc.ToString();

            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPON), "CMD_CWPON is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPBB), "CMD_CWPBB is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPST), "CMD_CWPST is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPBL), "CMD_CWPBL is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPBS), "CMD_CWPBS is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPX), "CMD_CWPX is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPBN), "CMD_CWPBN is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPP), "CMD_CWPP is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPBP), "CMD_CWPBP is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPAI), "CMD_CWPAI is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPTBP), "CMD_CWPTBP is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPAP), "CMD_CWPAP is missing.");

            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBI), "CMD_CBI is missing.");

            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTON), "CMD_CBTON is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTBB), "CMD_CBTBB is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTST), "CMD_CBTST is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTT), "CMD_CBTT is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTBL), "CMD_CBTBL is missing.");
            //Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTMX), "CMD_CBTMX is missing.");         // Removed
            Assert.IsFalse(result.Contains(AdcpSubsystemCommands.CMD_CBTMX), "CMD_CBTMX is missing.");          // Removed
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTTBP), "CMD_CBTTBP is missing.");

            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWTON), "CMD_CWTON is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWTBB), "CMD_CWTBB is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWTBL), "CMD_CWTBL is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWTBS), "CMD_CWTBS is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWTTBP), "CMD_CWTTBP is missing.");
        }

        #endregion

        #region Command List

        /// <summary>
        /// Test the Command List output.
        /// </summary>
        [Test]
        public void TestCommandList()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            List<string> list = ssc.GetCommandList();

            string result = "";
            // Create a single string for the commandlist.
            for (int x = 0; x < list.Count; x++)
            {
                result += list[x];
            }

            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPON), "CMD_CWPON is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPBB), "CMD_CWPBB is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPST), "CMD_CWPST is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPBL), "CMD_CWPBL is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPBS), "CMD_CWPBS is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPX), "CMD_CWPX is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPBN), "CMD_CWPBN is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPP), "CMD_CWPP is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPBP), "CMD_CWPBP is missing.");
            // Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPAI), "CMD_CWPAI is missing.");        // Removed
            Assert.IsFalse(result.Contains(AdcpSubsystemCommands.CMD_CWPAI), "CMD_CWPAI is missing.");          // Removed
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPTBP), "CMD_CWPTBP is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWPAP), "CMD_CWPAP is missing.");

            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBI), "CMD_CBI is missing.");

            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTON), "CMD_CBTON is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTBB), "CMD_CBTBB is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTST), "CMD_CBTST is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTT), "CMD_CBTT is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTBL), "CMD_CBTBL is missing.");
            //Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTMX), "CMD_CBTMX is missing.");         // Removed
            Assert.IsFalse(result.Contains(AdcpSubsystemCommands.CMD_CBTMX), "CMD_CBTMX is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CBTTBP), "CMD_CBTTBP is missing.");

            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWTON), "CMD_CWTON is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWTBB), "CMD_CWTBB is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWTBL), "CMD_CWTBL is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWTBS), "CMD_CWTBS is missing.");
            Assert.IsTrue(result.Contains(AdcpSubsystemCommands.CMD_CWTTBP), "CMD_CWTTBP is missing.");
        }

        #endregion

        #region CWPON Command Str

        /// <summary>
        /// Test CWPON Command String.
        /// </summary>
        [Test]
        public void CWPON_CmdStr()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWPON = false;

            Assert.AreEqual("CWPON[7] 0", ssc.CWPON_CmdStr(7), "CWPON Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWPON Command String.
        /// </summary>
        [Test]
        public void CWPON_CmdStr1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 3, 0));
            ssc.CWPON = true;

            Assert.AreEqual("CWPON[3] 1", ssc.CWPON_CmdStr(3), "CWPON Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWPON Command String.
        /// </summary>
        [Test]
        public void CWPON_CmdStr2()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWPON = true;

            Assert.AreEqual("CWPON[4] 1", ssc.CWPON_CmdStr(), "CWPON Cmd Str is incorrect.");
        }

        #endregion

        #region CWPBB Command Str

        /// <summary>
        /// Test CWPBB Command String.
        /// </summary>
        [Test]
        public void CWPBB()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWPBB_TransmitPulseType = AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE;
            ssc.CWPBB_LagLength = 0.0023f;

            Assert.AreEqual("CWPBB[7] 3,0.0023", ssc.CWPBB_CmdStr(7), "CWPBB Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWPBB Command String.
        /// </summary>
        [Test]
        public void CWPBB1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWPBB_TransmitPulseType = AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE;
            ssc.CWPBB_LagLength = 0.0024f;

            Assert.AreEqual("CWPBB[4] 3,0.0024", ssc.CWPBB_CmdStr(), "CWPBB Cmd Str is incorrect.");
        }

        #endregion

        #region CWPAP Command Str

        /// <summary>
        /// Test CWPAP Command String.
        /// </summary>
        [Test]
        public void CWPAP()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWPAP_NumPingsAvg = 2;
            ssc.CWPAP_Lag = 0.04565f;
            ssc.CWPAP_Blank = 1.5f;
            ssc.CWPAP_BinSize = 3.3f;
            ssc.CWPAP_TimeBetweenPing = 0.025f;

            Assert.AreEqual("CWPAP[7] 2,0.04565,1.5,3.3,0.025", ssc.CWPAP_CmdStr(7), "CWPAP Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWPAP Command String.
        /// </summary>
        [Test]
        public void CWPAP1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWPAP_NumPingsAvg = 3;
            ssc.CWPAP_Lag = 0.04565f;
            ssc.CWPAP_Blank = 1.5f;
            ssc.CWPAP_BinSize = 3.3f;
            ssc.CWPAP_TimeBetweenPing = 0.025f;

            Assert.AreEqual("CWPAP[4] 3,0.04565,1.5,3.3,0.025", ssc.CWPAP_CmdStr(), "CWPAP Cmd Str is incorrect.");
        }

        #endregion

        #region CWPBP Command Str

        /// <summary>
        /// Test CWPBP Command String.
        /// </summary>
        [Test]
        public void CWPBP()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWPBP_NumPingsAvg = 2;
            ssc.CWPBP_TimeBetweenBasePings = 0.025f;

            Assert.AreEqual("CWPBP[7] 2,0.025", ssc.CWPBP_CmdStr(7), "CWPBP Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWPBP Command String.
        /// </summary>
        [Test]
        public void CWPBP1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWPBP_NumPingsAvg = 3;
            ssc.CWPBP_TimeBetweenBasePings = 0.025f;

            Assert.AreEqual("CWPBP[4] 3,0.025", ssc.CWPBP_CmdStr(), "CWPBP Cmd Str is incorrect.");
        }

        #endregion

        #region CWPST Command Str

        /// <summary>
        /// Test CWPST Command String.
        /// </summary>
        [Test]
        public void CWPST()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWPST_CorrelationThresh = 0.33f;
            ssc.CWPST_QVelocityThresh = 0.44f;
            ssc.CWPST_VVelocityThresh = 0.55f;
            
            Assert.AreEqual("CWPST[7] 0.33,0.44,0.55", ssc.CWPST_CmdStr(7), "CWPST Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWPST Command String.
        /// </summary>
        [Test]
        public void CWPST1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWPST_CorrelationThresh = 0.33f;
            ssc.CWPST_QVelocityThresh = 0.44f;
            ssc.CWPST_VVelocityThresh = 0.55f;

            Assert.AreEqual("CWPST[4] 0.33,0.44,0.55", ssc.CWPST_CmdStr(), "CWPST Cmd Str is incorrect.");
        }

        #endregion

        #region CWPBL Command Str

        /// <summary>
        /// Test CWPBL Command String.
        /// </summary>
        [Test]
        public void CWPBL()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWPBL = 0.2345f;

            Assert.AreEqual("CWPBL[7] 0.2345", ssc.CWPBL_CmdStr(7), "CWPBL Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWPBL Command String.
        /// </summary>
        [Test]
        public void CWPBL1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWPBL = 0.2345f;

            Assert.AreEqual("CWPBL[4] 0.2345", ssc.CWPBL_CmdStr(), "CWPBL Cmd Str is incorrect.");
        }

        #endregion

        #region CWPBS Command Str

        /// <summary>
        /// Test CWPBS Command String.
        /// </summary>
        [Test]
        public void CWPBS()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWPBS = 0.2345f;

            Assert.AreEqual("CWPBS[7] 0.2345", ssc.CWPBS_CmdStr(7), "CWPBS Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWPBS Command String.
        /// </summary>
        [Test]
        public void CWPBS1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWPBS = 0.2345f;

            Assert.AreEqual("CWPBS[4] 0.2345", ssc.CWPBS_CmdStr(), "CWPBS Cmd Str is incorrect.");
        }

        #endregion

        #region CWPX Command Str

        /// <summary>
        /// Test CWPX Command String.
        /// </summary>
        [Test]
        public void CWPX()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWPX = 0.2345f;

            Assert.AreEqual("CWPX[7] 0.2345", ssc.CWPX_CmdStr(7), "CWPX Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWPX Command String.
        /// </summary>
        [Test]
        public void CWPX1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWPX = 0.2345f;

            Assert.AreEqual("CWPX[4] 0.2345", ssc.CWPX_CmdStr(), "CWPX Cmd Str is incorrect.");
        }

        #endregion

        #region CWPBN Command Str

        /// <summary>
        /// Test CWPBN Command String.
        /// </summary>
        [Test]
        public void CWPBN()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWPBN = 33;

            Assert.AreEqual("CWPBN[7] 33", ssc.CWPBN_CmdStr(7), "CWPBN Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWPBN Command String.
        /// </summary>
        [Test]
        public void CWPBN1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWPBN = 33;

            Assert.AreEqual("CWPBN[4] 33", ssc.CWPBN_CmdStr(), "CWPBN Cmd Str is incorrect.");
        }

        #endregion

        #region CWPP Command Str

        /// <summary>
        /// Test CWPP Command String.
        /// </summary>
        [Test]
        public void CWPP()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWPP = 33;

            Assert.AreEqual("CWPP[7] 33", ssc.CWPP_CmdStr(7), "CWPP Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWPP Command String.
        /// </summary>
        [Test]
        public void CWPP1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWPP = 33;

            Assert.AreEqual("CWPP[4] 33", ssc.CWPP_CmdStr(), "CWPP Cmd Str is incorrect.");
        }

        #endregion

        #region CWPAI Command Str

        /// <summary>
        /// Test CWPAI Command String.
        /// </summary>
        [Test]
        public void CWPAI()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWPAI = new TimeValue(0,0,1,0);

            Assert.AreEqual("CWPAI[7] 00:00:01.00", ssc.CWPAI_CmdStr(7), "CWPAI Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWPAI Command String.
        /// </summary>
        [Test]
        public void CWPAI1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWPAI = new TimeValue(0, 0, 1, 0);

            Assert.AreEqual("CWPAI[4] 00:00:01.00", ssc.CWPAI_CmdStr(), "CWPAI Cmd Str is incorrect.");
        }

        #endregion

        #region CWPTBP Command Str

        /// <summary>
        /// Test CWPTBP Command String.
        /// </summary>
        [Test]
        public void CWPTBP()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWPTBP = 0.001f;

            Assert.AreEqual("CWPTBP[7] 0.001", ssc.CWPTBP_CmdStr(7), "CWPTBP Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWPTBP Command String.
        /// </summary>
        [Test]
        public void CWPTBP1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWPTBP = 0.001f;

            Assert.AreEqual("CWPTBP[4] 0.001", ssc.CWPTBP_CmdStr(), "CWPTBP Cmd Str is incorrect.");
        }

        #endregion

        #region CBI Command Str

        /// <summary>
        /// Test CBI Command String.
        /// </summary>
        [Test]
        public void CBI()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CBI_NumEnsembles = 3;
            ssc.CBI_BurstInterval = new TimeValue(0, 0, 1, 0);

            Assert.AreEqual("CBI[7] 00:00:01.00,3,0", ssc.CBI_CmdStr(7), "CBI Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBI Command String.
        /// </summary>
        [Test]
        public void CBI1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CBI_NumEnsembles = 3;
            ssc.CBI_BurstInterval = new TimeValue(0, 0, 1, 0);

            Assert.AreEqual("CBI[4] 00:00:01.00,3,0", ssc.CBI_CmdStr(), "CBI Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBI Command String.
        /// </summary>
        [Test]
        public void CBI2()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CBI_NumEnsembles = 3;
            ssc.CBI_BurstInterval = new TimeValue(0, 0, 3, 0);

            Assert.AreEqual("CBI[4] 00:00:03.00,3,0", ssc.CBI_CmdStr(4), "CBI Cmd Str is incorrect.");
        }

        #endregion

        #region CBTON Command Str

        /// <summary>
        /// Test CBTON Command String.
        /// </summary>
        [Test]
        public void CBTON_CmdStr()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));
            ssc.CBTON = false;

            Assert.AreEqual("CBTON[0] 0", ssc.CBTON_CmdStr(0), "CBTON Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTON Command String.
        /// </summary>
        [Test]
        public void CBTON_CmdStr1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 7));
            ssc.CBTON = true;

            Assert.AreEqual("CBTON[7] 1", ssc.CBTON_CmdStr(7), "CBTON Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTON Command String.
        /// </summary>
        [Test]
        public void CBTON_CmdStr2()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 4));
            ssc.CBTON = true;

            Assert.AreEqual("CBTON[4] 1", ssc.CBTON_CmdStr(), "CBTON Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTON Command String.
        /// </summary>
        [Test]
        public void CBTON_CmdStr23()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 23, 23));
            ssc.CBTON = true;

            Assert.AreEqual("CBTON[23] 1", ssc.CBTON_CmdStr(23), "CBTON Cmd Str is incorrect.");
        }

        #endregion

        #region CBTBB Command Str

        /// <summary>
        /// Test CBTBB Command String.
        /// </summary>
        [Test]
        public void CBTBB()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 3, 3));
            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE;
            ssc.CBTBB_PulseToPulseLag = 0.0023f;
            ssc.CBTBB_LongRangeDepth = 0.123456f;

            Assert.AreEqual("CBTBB[3] 0,0.0023,0.123456", ssc.CBTBB_CmdStr(3), "CBTBB Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTBB Command String.
        /// </summary>
        [Test]
        public void CBTBB1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));
            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED;
            ssc.CBTBB_PulseToPulseLag = 0.0021f;
            ssc.CBTBB_LongRangeDepth = 0.1234561f;

            Assert.AreEqual("CBTBB[0] 1,0.0021,0.1234561", ssc.CBTBB_CmdStr(), "CBTBB Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTBB Command String.
        /// </summary>
        [Test]
        public void CBTBB2()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));
            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED;
            ssc.CBTBB_PulseToPulseLag = 0.0023f;
            ssc.CBTBB_LongRangeDepth = 0.123456f;

            Assert.AreEqual("CBTBB[0] 2,0.0023,0.123456", ssc.CBTBB_CmdStr(), "CBTBB Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTBB Command String.
        /// </summary>
        [Test]
        public void CBTBB3()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));
            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.NA_3;
            ssc.CBTBB_PulseToPulseLag = 0.0023f;
            ssc.CBTBB_LongRangeDepth = 0.1234563f;

            Assert.AreEqual("CBTBB[0] 3,0.0023,0.1234563", ssc.CBTBB_CmdStr(), "CBTBB Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTBB Command String.
        /// </summary>
        [Test]
        public void CBTBB4()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));
            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED_P2P;
            ssc.CBTBB_PulseToPulseLag = 0.0024f;
            ssc.CBTBB_LongRangeDepth = 0.1234564f;

            Assert.AreEqual("CBTBB[0] 4,0.0024,0.1234564", ssc.CBTBB_CmdStr(), "CBTBB Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTBB Command String.
        /// </summary>
        [Test]
        public void CBTBB5()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));
            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.NA_5;
            ssc.CBTBB_PulseToPulseLag = 0.0025f;
            ssc.CBTBB_LongRangeDepth = 0.1234565f;

            Assert.AreEqual("CBTBB[0] 5,0.0025,0.1234565", ssc.CBTBB_CmdStr(), "CBTBB Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTBB Command String.
        /// </summary>
        [Test]
        public void CBTBB6()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));
            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.NA_6;
            ssc.CBTBB_PulseToPulseLag = 0.0026f;
            ssc.CBTBB_LongRangeDepth = 0.1234566f;

            Assert.AreEqual("CBTBB[0] 6,0.0026,0.1234566", ssc.CBTBB_CmdStr(), "CBTBB Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTBB Command String.
        /// </summary>
        [Test]
        public void CBTBB7()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));
            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.AUTO_SWITCH_NARROWBAND_BB_NONCODED_BB_NONCODED_P2P;
            ssc.CBTBB_PulseToPulseLag = 0.0027f;
            ssc.CBTBB_LongRangeDepth = 0.1234567f;

            Assert.AreEqual("CBTBB[0] 7,0.0027,0.1234567", ssc.CBTBB_CmdStr(), "CBTBB Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTBB Command String.
        /// </summary>
        [Test]
        public void CBTBB77()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 77, 77));
            ssc.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.AUTO_SWITCH_NARROWBAND_BB_NONCODED_BB_NONCODED_P2P;
            ssc.CBTBB_PulseToPulseLag = 0.0027f;
            ssc.CBTBB_LongRangeDepth = 0.1234567f;

            Assert.AreEqual("CBTBB[77] 7,0.0027,0.1234567", ssc.CBTBB_CmdStr(77), "CBTBB Cmd Str is incorrect.");
        }

        #endregion

        #region CBTST Command Str

        /// <summary>
        /// Test CBTST Command String.
        /// </summary>
        [Test]
        public void CBTST()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 6, 0));
            ssc.CBTST_CorrelationThresh = 0.33f;
            ssc.CBTST_QVelocityThresh = 0.44f;
            ssc.CBTST_VVelocityThresh = 0.55f;

            Assert.AreEqual("CBTST[6] 0.33,0.44,0.55", ssc.CBTST_CmdStr(6), "CBTST Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTST Command String.
        /// </summary>
        [Test]
        public void CBTST1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CBTST_CorrelationThresh = 0.33f;
            ssc.CBTST_QVelocityThresh = 0.44f;
            ssc.CBTST_VVelocityThresh = 0.55f;

            Assert.AreEqual("CBTST[4] 0.33,0.44,0.55", ssc.CBTST_CmdStr(), "CBTST Cmd Str is incorrect.");
        }

        #endregion

        #region CBTBL Command Str

        /// <summary>
        /// Test CBTBL Command String.
        /// </summary>
        [Test]
        public void CBTBL()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CBTBL = 0.2345f;

            Assert.AreEqual("CBTBL[7] 0.2345", ssc.CBTBL_CmdStr(7), "CBTBL Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTBL Command String.
        /// </summary>
        [Test]
        public void CBTBL1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CBTBL = 0.2345f;

            Assert.AreEqual("CBTBL[4] 0.2345", ssc.CBTBL_CmdStr(), "CBTBL Cmd Str is incorrect.");
        }

        #endregion

        #region CBTMX Command Str

        /// <summary>
        /// Test CBTMX Command String.
        /// </summary>
        [Test]
        public void CBTMX()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CBTMX = 5.2345f;

            Assert.AreEqual("CBTMX[7] 5.2345", ssc.CBTMX_CmdStr(7), "CBTMX Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTMX Command String.
        /// </summary>
        [Test]
        public void CBTMX1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CBTMX = 5.2345f;

            Assert.AreEqual("CBTMX[4] 5.2345", ssc.CBTMX_CmdStr(), "CBTMX Cmd Str is incorrect.");
        }

        #endregion

        #region CBTTBP Command Str

        /// <summary>
        /// Test CBTTBP Command String.
        /// </summary>
        [Test]
        public void CBTTBP()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CBTTBP = 0.001f;

            Assert.AreEqual("CBTTBP[7] 0.001", ssc.CBTTBP_CmdStr(7), "CBTTBP Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTTBP Command String.
        /// </summary>
        [Test]
        public void CBTTBP1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CBTTBP = 0.001f;

            Assert.AreEqual("CBTTBP[4] 0.001", ssc.CBTTBP_CmdStr(), "CBTTBP Cmd Str is incorrect.");
        }

        #endregion

        #region CBTT Command Str

        /// <summary>
        /// Test CBTT Command String.
        /// </summary>
        [Test]
        public void CBTT()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CBTT_SNRShallowDetectionThresh = 0.001f;
            ssc.CBTT_DepthSNR = 0.002f;
            ssc.CBTT_SNRDeepDetectionThresh = 0.003f;
            ssc.CBTT_DepthGain = 0.004f;

            Assert.AreEqual("CBTT[7] 0.001,0.002,0.003,0.004", ssc.CBTT_CmdStr(7), "CBTT Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CBTT Command String.
        /// </summary>
        [Test]
        public void CBTT1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CBTT_SNRShallowDetectionThresh = 0.001f;
            ssc.CBTT_DepthSNR = 0.002f;
            ssc.CBTT_SNRDeepDetectionThresh = 0.003f;
            ssc.CBTT_DepthGain = 0.004f;

            Assert.AreEqual("CBTT[4] 0.001,0.002,0.003,0.004", ssc.CBTT_CmdStr(), "CBTT Cmd Str is incorrect.");
        }

        #endregion

        #region CWTON Command Str

        /// <summary>
        /// Test CWTON Command String.
        /// </summary>
        [Test]
        public void CWTON_CmdStr()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWTON = false;

            Assert.AreEqual("CWTON[7] 0", ssc.CWTON_CmdStr(7), "CWTON Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWTON Command String.
        /// </summary>
        [Test]
        public void CWTON_CmdStr1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 3, 0));
            ssc.CWTON = true;

            Assert.AreEqual("CWTON[3] 1", ssc.CWTON_CmdStr(3), "CWTON Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWTON Command String.
        /// </summary>
        [Test]
        public void CWTON_CmdStr2()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWTON = true;

            Assert.AreEqual("CWTON[4] 1", ssc.CWTON_CmdStr(), "CWTON Cmd Str is incorrect.");
        }

        #endregion

        #region CWTBB Command Str

        /// <summary>
        /// Test CWTBB Command String.
        /// </summary>
        [Test]
        public void CWTBB_CmdStr()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWTBB = false;

            Assert.AreEqual("CWTBB[7] 0", ssc.CWTBB_CmdStr(7), "CWTBB Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWTBB Command String.
        /// </summary>
        [Test]
        public void CWTBB_CmdStr1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 3, 0));
            ssc.CWTBB = true;

            Assert.AreEqual("CWTBB[3] 1", ssc.CWTBB_CmdStr(3), "CWTBB Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWTBB Command String.
        /// </summary>
        [Test]
        public void CWTBB_CmdStr2()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWTBB = true;

            Assert.AreEqual("CWTBB[4] 1", ssc.CWTBB_CmdStr(), "CWTBB Cmd Str is incorrect.");
        }

        #endregion

        #region CWTBL Command Str

        /// <summary>
        /// Test CWTBL Command String.
        /// </summary>
        [Test]
        public void CWTBL()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWTBL = 0.2345f;

            Assert.AreEqual("CWTBL[7] 0.2345", ssc.CWTBL_CmdStr(7), "CWTBL Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWTBL Command String.
        /// </summary>
        [Test]
        public void CWTBL1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWTBL = 0.2345f;

            Assert.AreEqual("CWTBL[4] 0.2345", ssc.CWTBL_CmdStr(), "CWTBL Cmd Str is incorrect.");
        }

        #endregion

        #region CWTBS Command Str

        /// <summary>
        /// Test CWTBS Command String.
        /// </summary>
        [Test]
        public void CWTBS()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWTBS = 0.2345f;

            Assert.AreEqual("CWTBS[7] 0.2345", ssc.CWTBS_CmdStr(7), "CWTBS Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWTBS Command String.
        /// </summary>
        [Test]
        public void CWTBS1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWTBS = 0.2345f;

            Assert.AreEqual("CWTBS[4] 0.2345", ssc.CWTBS_CmdStr(), "CWTBS Cmd Str is incorrect.");
        }

        #endregion

        #region CWTTBP Command Str

        /// <summary>
        /// Test CWTTBP Command String.
        /// </summary>
        [Test]
        public void CWTTBP()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 7, 0));
            ssc.CWTTBP = 0.001f;

            Assert.AreEqual("CWTTBP[7] 0.001", ssc.CWTTBP_CmdStr(7), "CWTTBP Cmd Str is incorrect.");
        }

        /// <summary>
        /// Test CWTTBP Command String.
        /// </summary>
        [Test]
        public void CWTTBP1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_VERT_PISTON_B, 1);
            AdcpSubsystemCommands ssc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 4, 0));
            ssc.CWTTBP = 0.001f;

            Assert.AreEqual("CWTTBP[4] 0.001", ssc.CWTTBP_CmdStr(), "CWTTBP Cmd Str is incorrect.");
        }

        #endregion
    }
}
