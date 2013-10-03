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
 * 09/28/2012      RC          2.15       Added BT, WT and Environmental commands.
 * 10/04/2012      RC          2.15       Added CBI command.
 * 12/28/2012      RC          2.17       Made SubsystemConfiguration take a Subsystem in its constructor.
 *                                         Moved AdcpSubsystemConfig.Subsystem into AdcpSubsystemConfig.SubsystemConfig.Subsystem.
 *                                         AdcpSubsystemConfigExist() take only 1 argument.
 * 09/11/2013      RC          2.19.5     Updated test to 2.19.5
 * 09/17/2013      RC          2.20.0     Updated test to 2.20.0 with latest broadband modes.
 *
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Text;
    using RTI.Commands;


    /// <summary>
    /// Test DecodeCSHOW to ensure it properly decodes the
    /// statement.
    /// </summary>
    [TestFixture]
    public class DecodeCSHOWTest
    {
        /// <summary>
        /// String that contains the results of CSHOW.
        /// This system has 1 subsystem and 1 configuration for that
        /// subsystem.
        /// </summary>
        private string _singleSubsystemConfiguration;

        /// <summary>
        /// Serial number for a system with a single subystem.
        /// </summary>
        private SerialNumber _singleSubsystemSerialNumber;

        /// <summary>
        /// String that contains the results of CSHOW.
        /// This system has 1 subsystem and multiple configurations
        /// for that subsystem.
        /// </summary>
        private string _multipleSubsystemConfigurations;

        /// <summary>
        /// String that contains the result of CSHOW.
        /// This system has multiple subsystems and mutiple configurations
        /// for each subsystem.
        /// </summary>
        private string _multipleSubsystemMultipleConfigurations;

        /// <summary>
        /// Serial number for a system with a multiple subystems.
        /// </summary>
        private SerialNumber _multipleSubsystemSerialNumber;

        /// <summary>
        /// Initialize CSHOW test strings.
        /// </summary>
        public DecodeCSHOWTest()
        {
            #region 1 Sub 1 Confg

            StringBuilder sb = new StringBuilder();
            sb.Append("CSHOW\r\n"); 
           sb.Append("\r\n"); 
            sb.Append("DP1200 System Configuration:\r\n"); 
            sb.Append("    Mode Profile\r\n"); 
            sb.Append("    Sim 0 \r\n"); 
            sb.Append("    CEI 00:00:01.00\r\n"); 
            sb.Append("    CEPO 2\r\n");
            sb.Append("    CBI[0] 00:00:01.00,100 \r\n");
            sb.Append("    CETFP 2012/09/24,12:30:10.25\r\n"); 
            sb.Append("    CERECORD 1,1\r\n"); 
            sb.Append("    CEOUTPUT 1\r\n"); 
            sb.Append("    CWPON[0] 1 \r\n"); 
            sb.Append("    CWPBB[0] 1,0.042 \r\n"); 
            sb.Append("    CWPAP[0] 10,0.15,0.06,0.04,0.01 \r\n"); 
            sb.Append("    CWPBP[0] 1,0.02 \r\n"); 
            sb.Append("    CWPST[0] 0.400,1.000,1.001 \r\n"); 
            sb.Append("    CWPBL[0] 0.10 \r\n"); 
            sb.Append("    CWPBS[0] 1.00 \r\n"); 
            sb.Append("    CWPX [0] 0.01 \r\n"); 
            sb.Append("    CWPBN[0] 30 \r\n"); 
            sb.Append("    CWPP[0] 1 \r\n"); 
            sb.Append("    CWPAI[0] 03:02:00.10 \r\n"); 
            sb.Append("    CWPTBP[0] 0.13 \r\n"); 
            sb.Append("    CBTON[0] 1 \r\n"); 
            sb.Append("    CBTBB[0] 2, 1.023, 30.00 \r\n"); 
            sb.Append("    CBTST[0] 0.900,1.002,1.001 \r\n"); 
            sb.Append("    CBTBL[0] 0.05 \r\n"); 
            sb.Append("    CBTMX[0] 75.00 \r\n"); 
            sb.Append("    CBTTBP[0] 0.05 \r\n"); 
            sb.Append("    CBTT[0] 15.0,25.0,5.0,2.0 \r\n"); 
            sb.Append("    CWTON[0] 0 \r\n"); 
            sb.Append("    CWTBB[0] 0 \r\n"); 
            sb.Append("    CWTBL[0] 2.00 \r\n"); 
            sb.Append("    CWTBS[0] 2.01 \r\n"); 
            sb.Append("    CWTTBP[0] 0.13 \r\n"); 
            sb.Append("    CWS 17.00\r\n"); 
            sb.Append("    CWT 15.00\r\n"); 
            sb.Append("    CTD 22.10\r\n"); 
            sb.Append("    CWSS 1500.01\r\n"); 
            sb.Append("    CHO 12.654\r\n"); 
            sb.Append("    CHS 1\r\n"); 
            sb.Append("    CVSF 1.023\r\n"); 
            sb.Append("    C232B 115200\r\n"); 
            sb.Append("    C485B 460800\r\n"); 
            sb.Append("    C422B 19200\r\n"); 
            sb.Append("CSHOW\r\n");
            _singleSubsystemConfiguration = sb.ToString();

            // Set serial number for single subsystem
            _singleSubsystemSerialNumber = new SerialNumber("01200000000000000000000000000004");

            #endregion

            #region 1 Sub Multiple Config

            sb.Clear();
            sb.Append("CSHOW\r\n");
            sb.Append("\r\n");
            sb.Append("DP1200 System Configuration:\r\n");
            sb.Append("  Mode Profile\r\n");
            sb.Append("  Sim 0 \r\n");
            sb.Append("  CEI 02:00:01.00\r\n");
            sb.Append("  CEPO 222\r\n");
            sb.Append("  CBI[0] 00:00:01.01,101 [1] 00:00:01.02,102 [2] 00:00:03.01,103 \r\n");
            sb.Append("  CETFP 2012/09/24,12:30:10.25\r\n"); 
            sb.Append("  CERECORD 0,0\r\n");
            sb.Append("  CEOUTPUT 0\r\n");
            sb.Append("  CWPON[0] 1 [1] 0 [2] 1 \r\n");
            sb.Append("  CWPBB[0] 1,0.042 [1] 2,0.043 [2] 3,0.044 \r\n");
            sb.Append("  CWPAP[0] 10,0.15,0.06,0.04,0.01 [1] 11,0.16,0.07,0.05,0.02 [2] 12,0.17,0.08,0.06,0.03 \r\n");
            sb.Append("  CWPBP[0] 1,0.02 [1] 2,0.04 [2] 3,0.05 \r\n");
            sb.Append("  CWPST[0] 0.400,1.000,1.001 [1] 0.500,1.002,1.003 [2] 0.600,1.004,1.005 \r\n");
            sb.Append("  CWPBL[0] 0.10 [1] 0.11 [2] 0.12 \r\n");
            sb.Append("  CWPBS[0] 1.00 [1] 1.01 [2] 1.02 \r\n");
            sb.Append("  CWPX [0] 0.01 [1] 0.02 [2] 0.03 \r\n");
            sb.Append("  CWPBN[0] 30 [1] 31 [2] 32 \r\n");
            sb.Append("  CWPP[0] 1 [1] 2 [2] 3 \r\n");
            sb.Append("  CWPAI[0] 01:02:03.04 [1] 05:06:07.08 [2] 09:10:11.12 \r\n");
            sb.Append("  CWPTBP[0] 0.13 [1] 0.14 [2] 0.15 \r\n");
            sb.Append("  CBTON[0] 1 [1] 0 [2] 1 \r\n");
            sb.Append("  CBTBB[0] 2, 4.440, 30.00 [1] 4, 2.020, 30.01 [2] 3, 1.000, 30.02 \r\n");
            sb.Append("  CBTST[0] 0.900,1.003,1.005 [1] 0.901,1.006,1.007 [2] 0.902,1.008,1.009 \r\n");
            sb.Append("  CBTBL[0] 0.05 [1] 0.06 [2] 0.07 \r\n");
            sb.Append("  CBTMX[0] 75.00 [1] 75.01 [2] 75.02 \r\n");
            sb.Append("  CBTTBP[0] 0.05 [1] 0.06 [2] 0.07 \r\n");
            sb.Append("  CBTT[0] 15.0,25.0,5.0,2.0 [1] 15.1,25.1,5.1,2.1 [2] 15.2,25.2,5.2,2.2 \r\n");
            sb.Append("  CWTON[0] 1 [1] 0 [2] 1 \r\n");
            sb.Append("  CWTBB[0] 0 [1] 1 [2] 0 \r\n");
            sb.Append("  CWTBL[0] 2.00 [1] 2.01 [2] 2.02 \r\n");
            sb.Append("  CWTBS[0] 2.03 [1] 2.04 [2] 2.05 \r\n");
            sb.Append("  CWTTBP[0] 0.13 [1] 0.14 [2] 0.15 \r\n");
            sb.Append("  CWS 64.20\r\n");
            sb.Append("  CWT 15.01\r\n");
            sb.Append("  CTD 13.03\r\n");
            sb.Append("  CWSS 1500.06\r\n");
            sb.Append("  CHO 125.36\r\n");
            sb.Append("  CHS 2\r\n");
            sb.Append("  CVSF 1.040\r\n");
            sb.Append("  C232B 4800\r\n");
            sb.Append("  C485B 9600\r\n");
            sb.Append("  C422B 230400\r\n");
            sb.Append("CSHOW\r\n");
            _multipleSubsystemConfigurations = sb.ToString();


            #endregion

            #region Multiple Sub Multiple Config

            sb.Clear();
            sb.Append("CSHOW\r\n");
            sb.Append("\r\n");
            sb.Append("DP1200 System Configuration:\r\n");
            sb.Append("    Mode Profile\r\n");
            sb.Append("    Sim 0 \r\n");
            sb.Append("    CEI 13:20:01.33\r\n");
            sb.Append("    CEPO 2232332\r\n");
            sb.Append("    CBI[0] 00:00:01.01,101 [1] 00:00:01.02,102 [2] 00:02:03.01,102 [3] 00:00:01.03,103 [4] 00:00:03.04,104 [5] 00:00:01.05,105 [6] 00:06:03.01,106 \r\n");
            sb.Append("    CETFP 2012/09/24,12:30:10.25\r\n"); 
            sb.Append("    CERECORD 1,1\r\n");
            sb.Append("    CEOUTPUT 1\r\n");
            sb.Append("    CWPON[0] 1 [1] 0 [2] 1 [3] 1 [4] 0 [5] 1 [6] 1 \r\n");
            sb.Append("    CWPBB[0] 1,0.042 [1] 2,0.042 [2] 3,0.084 [3] 2,0.044 [4] 1,0.084 [5] 1,0.085 [6] 3,0.043 \r\n");
            sb.Append("    CWPAP[0] 10,0.15,0.06,0.04,0.01 [1] 11,0.16,0.07,0.05,0.02 [2] 12,0.17,0.08,0.06,0.03 [3] 13,0.18,0.09,0.07,0.04 [4] 14,0.19,0.10,0.08,0.05 [5] 15,0.20,0.11,0.09,0.06 [6] 16,0.21,0.12,0.10,0.07 \r\n");
            sb.Append("    CWPBP[0] 1,0.02 [1] 2,0.03 [2] 3,0.04 [3] 4,0.05 [4] 5,0.06 [5] 6,0.07 [6] 7,0.08 \r\n");
            sb.Append("    CWPST[0] 0.400,1.001,1.002 [1] 0.500,1.003,1.004 [2] 0.600,1.005,1.006 [3] 0.700,1.007,1.008 [4] 0.800,1.009,1.010 [5] 0.900,1.011,1.012 [6] 0.300,1.013,1.014 \r\n");
            sb.Append("    CWPBL[0] 0.10 [1] 0.11 [2] 0.22 [3] 0.13 [4] 0.24 [5] 0.25 [6] 0.16 \r\n");
            sb.Append("    CWPBS[0] 1.00 [1] 1.01 [2] 2.02 [3] 1.03 [4] 2.04 [5] 2.05 [6] 1.06 \r\n");
            sb.Append("    CWPX [0] 0.01 [1] 0.02 [2] 0.03 [3] 0.04 [4] 0.05 [5] 0.06 [6] 0.70 \r\n");
            sb.Append("    CWPBN[0] 30 [1] 31 [2] 32 [3] 33 [4] 34 [5] 35 [6] 36 \r\n");
            sb.Append("    CWPP[0] 1 [1] 2 [2] 3 [3] 4 [4] 5 [5] 6 [6] 7 \r\n");
            sb.Append("    CWPAI[0] 01:02:03.04 [1] 05:06:07.08 [2] 09:10:11.12 [3] 13:14:15.16 [4] 17:18:19.20 [5] 20:21:22.23 [6] 24:25:26.27 \r\n");
            sb.Append("    CWPTBP[0] 0.13 [1] 0.14 [2] 0.25 [3] 0.16 [4] 0.27 [5] 0.28 [6] 0.19 \r\n");
            sb.Append("    CBTON[0] 1 [1] 0 [2] 1 [3] 0 [4] 0 [5] 1 [6] 0 \r\n");
            sb.Append("    CBTBB[0] 0, 1.001, 30.00 [1] 1, 2.002, 30.02 [2] 3, 3.003, 50.03 [3] 4, 4.004, 30.04 [4] 5, 5.005, 50.05 [5] 6, 6.006, 50.06 [6] 7, 7.007, 30.07 \r\n");
            sb.Append("    CBTST[0] 0.901,1.002,1.003 [1] 0.904,1.005,1.006 [2] 0.907,1.008,1.009 [3] 0.910,1.011,1.012 [4] 0.913,1.015,1.016 [5] 0.917,1.018,1.019 [6] 0.920,1.021,1.022 \r\n");
            sb.Append("    CBTBL[0] 0.05 [1] 0.07 [2] 0.18 [3] 0.09 [4] 0.11 [5] 0.12 [6] 0.013 \r\n");
            sb.Append("    CBTMX[0] 75.00 [1] 75.01 [2] 125.02 [3] 75.03 [4] 125.04 [5] 125.05 [6] 75.06 \r\n");
            sb.Append("    CBTTBP[0] 0.05 [1] 0.06 [2] 0.17 [3] 0.08 [4] 0.19 [5] 0.20 [6] 0.21 \r\n");
            sb.Append("    CBTT[0] 15.0,25.0,5.0,2.0 [1] 15.1,25.1,5.1,2.1 [2] 15.2,50.2,5.2,4.2 [3] 15.3,25.3,5.3,2.3 [4] 15.4,50.4,5.4,4.4 [5] 15.5,50.5,5.5,4.5 [6] 15.6,25.6,5.6,2.6 \r\n");
            sb.Append("    CWTON[0] 1 [1] 0 [2] 1 [3] 0 [4] 0 [5] 1 [6] 0 \r\n");
            sb.Append("    CWTBB[0] 1 [1] 0 [2] 1 [3] 0 [4] 1 [5] 0 [6] 1 \r\n");
            sb.Append("    CWTBL[0] 2.07 [1] 2.06 [2] 2.05 [3] 2.04 [4] 2.03 [5] 2.02 [6] 2.01 \r\n");
            sb.Append("    CWTBS[0] 2.08 [1] 2.09 [2] 2.10 [3] 2.11 [4] 2.12 [5] 2.13 [6] 2.14 \r\n");
            sb.Append("    CWTTBP[0] 0.132 [1] 0.131[2] 0.29 [3] 0.18 [4] 0.27 [5] 0.26 [6] 0.15 \r\n");
            sb.Append("    CWS 69.69\r\n");
            sb.Append("    CWT 15.033\r\n");
            sb.Append("    CTD 13.13\r\n");
            sb.Append("    CWSS 1500.30\r\n");
            sb.Append("    CHO 14.14\r\n");
            sb.Append("    CHS 1\r\n");
            sb.Append("    CVSF 1.002\r\n");
            sb.Append("    C232B 921600\r\n");
            sb.Append("    C485B 2400\r\n");
            sb.Append("    C422B 38400\r\n");
            sb.Append("CSHOW\r\n");
            _multipleSubsystemMultipleConfigurations = sb.ToString();

            // Set serial number for Multiple subsystem
            _multipleSubsystemSerialNumber = new SerialNumber("01230000000000000000000000000004");

            #endregion
        }

        #region CEPO

        /// <summary>
        /// Test CEPO decoding with 1 sub 1 config.
        /// </summary>
        [Test]
        public void TestCEPO11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual("2", config.Commands.CEPO, "Commands CEPO is incorrect.");
            Assert.AreEqual(1, config.SubsystemConfigDict.Count, "Num of SubsystemConfig created is incorrect.");

            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach(AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }

            Assert.IsNotNull(ssConfig, "SubsystemConfiguration is null");
            Assert.AreEqual('2', Convert.ToChar(ssConfig.SubsystemConfig.SubSystem.Code), "SubsystemConfiguration Subsystem Code is incorrect.");       // Code of subsystem
            Assert.AreEqual(0, ssConfig.SubsystemConfig.SubSystem.Index, "SubsystemConfiguration Subsystem Index is incorrect.");                       // Index of Subsystem Code within SerialNumber
            Assert.AreEqual(0, ssConfig.SubsystemConfig.CepoIndex, "Subsystem Configuration Index is incorrect.");                                          // Index of Config within CEPO
        }

        /// <summary>
        /// Test CEPO decoding with 1 sub multiple config.
        /// </summary>
        [Test]
        public void TestCEPO1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual("222", config.Commands.CEPO, "Commands CEPO is incorrect.");
            Assert.AreEqual(3, config.SubsystemConfigDict.Count, "Num of SubsystemConfig created is incorrect.");

            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }

            }

            // SubsystemConfiguration 1
            Assert.IsNotNull(resultConfig1, "SubsystemConfiguration 1 is null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig1.SubsystemConfig.SubSystem.Code), "SubsystemConfiguration 1 Subsystem Code is incorrect.");        // Code of Subsystem
            Assert.AreEqual(0, resultConfig1.SubsystemConfig.SubSystem.Index, "SubsystemConfiguration 1 Subsystem Index is incorrect.");                        // Index of Subsystem Code within SerialNumber
            Assert.AreEqual(0, resultConfig1.SubsystemConfig.CepoIndex, "SubsystemConfiguration 1 Index is incorrect.");                                            // Index of SubsystemConfiguration within CEPO

            // SubsystemConfiguration 2
            Assert.IsNotNull(resultConfig2, "SubsystemConfiguration 2 is null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig2.SubsystemConfig.SubSystem.Code), "SubsystemConfiguration 2 Subsystem Code is incorrect.");
            Assert.AreEqual(0, resultConfig2.SubsystemConfig.SubSystem.Index, "SubsystemConfiguration 2 Subsystem Index is incorrect.");
            Assert.AreEqual(1, resultConfig2.SubsystemConfig.CepoIndex, "SubsystemConfiguration 2 Index is incorrect.");

            // SubsystemConfiguration 3
            Assert.IsNotNull(resultConfig3, "SubsystemConfiguration 3 is null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig3.SubsystemConfig.SubSystem.Code), "SubsystemConfiguration 3 Subsystem Code is incorrect.");
            Assert.AreEqual(0, resultConfig3.SubsystemConfig.SubSystem.Index, "SubsystemConfiguration 3 Subsystem Index is incorrect.");
            Assert.AreEqual(2, resultConfig3.SubsystemConfig.CepoIndex, "SubsystemConfiguration 3 Index is incorrect.");
        }

        /// <summary>
        /// Test CEPO decoding with Multiple sub multiple config.
        /// </summary>
        [Test]
        public void TestCEPOMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual("2232332", config.Commands.CEPO, "Commands CEPO is incorrect.");
            Assert.AreEqual(7, config.SubsystemConfigDict.Count, "Num of SubsystemConfig created is incorrect.");

            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }

            // SubsystemConfiguration 2_1
            Assert.IsNotNull(resultConfig2_1, "SubsystemConfiguration 2_1 is null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig2_1.SubsystemConfig.SubSystem.Code), "SubsystemConfiguration 2_1 Subsystem Code is incorrect.");
            Assert.AreEqual(0, resultConfig2_1.SubsystemConfig.SubSystem.Index, "SubsystemConfiguration 2_1 Subsystem Index is incorrect.");
            Assert.AreEqual(0, resultConfig2_1.SubsystemConfig.CepoIndex, "SubsystemConfiguration 2_1 Index is incorrect.");

            // SubsystemConfiguration 2_2
            Assert.IsNotNull(resultConfig2_2, "SubsystemConfiguration 2_2 is null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig2_2.SubsystemConfig.SubSystem.Code), "SubsystemConfiguration 2_2 Subsystem Code is incorrect.");
            Assert.AreEqual(0, resultConfig2_2.SubsystemConfig.SubSystem.Index, "SubsystemConfiguration 2_2 Subsystem Index is incorrect.");
            Assert.AreEqual(1, resultConfig2_2.SubsystemConfig.CepoIndex, "SubsystemConfiguration 2_2 Index is incorrect.");

            // SubsystemConfiguration 2_3
            Assert.IsNotNull(resultConfig2_3, "SubsystemConfiguration 2_3 is null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig2_3.SubsystemConfig.SubSystem.Code), "SubsystemConfiguration 2_3 Subsystem Code is incorrect.");
            Assert.AreEqual(0, resultConfig2_3.SubsystemConfig.SubSystem.Index, "SubsystemConfiguration 2_3 Subsystem Index is incorrect.");
            Assert.AreEqual(3, resultConfig2_3.SubsystemConfig.CepoIndex, "SubsystemConfiguration 2_3 Index is incorrect.");

            // SubsystemConfiguration 2_4
            Assert.IsNotNull(resultConfig2_4, "SubsystemConfiguration 2_4 is null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig2_4.SubsystemConfig.SubSystem.Code), "SubsystemConfiguration 2_4 Subsystem Code is incorrect.");
            Assert.AreEqual(0, resultConfig2_4.SubsystemConfig.SubSystem.Index, "SubsystemConfiguration 2_4 Subsystem Index is incorrect.");
            Assert.AreEqual(6, resultConfig2_4.SubsystemConfig.CepoIndex, "SubsystemConfiguration 2_4 Index is incorrect.");




            // SubsystemConfiguration 3_1
            Assert.IsNotNull(resultConfig3_1, "SubsystemConfiguration 3_1 is null");
            Assert.AreEqual('3', Convert.ToChar(resultConfig3_1.SubsystemConfig.SubSystem.Code), "SubsystemConfiguration 3_1 Subsystem Code is incorrect.");            // Subsystem Code
            Assert.AreEqual(1, resultConfig3_1.SubsystemConfig.SubSystem.Index, "SubsystemConfiguration 3_1 Subsystem Index is incorrect.");                            // Index within serial number of Subsystem code
            Assert.AreEqual(2, resultConfig3_1.SubsystemConfig.CepoIndex, "SubsystemConfiguration 3_1 Index is incorrect.");                                                // Index within CEPO of configuration

            // SubsystemConfiguration 3_2
            Assert.IsNotNull(resultConfig3_2, "SubsystemConfiguration 3_2 is null");
            Assert.AreEqual('3', Convert.ToChar(resultConfig3_2.SubsystemConfig.SubSystem.Code), "SubsystemConfiguration 3_2 Subsystem Code is incorrect.");
            Assert.AreEqual(1, resultConfig3_2.SubsystemConfig.SubSystem.Index, "SubsystemConfiguration 3_2 Subsystem Index is incorrect.");
            Assert.AreEqual(4, resultConfig3_2.SubsystemConfig.CepoIndex, "SubsystemConfiguration 3_2 Index is incorrect.");

            // SubsystemConfiguration 3_3
            Assert.IsNotNull(resultConfig3_3, "SubsystemConfiguration 3_3 is null");
            Assert.AreEqual('3', Convert.ToChar(resultConfig3_3.SubsystemConfig.SubSystem.Code), "SubsystemConfiguration 3_3 Subsystem Code is incorrect.");
            Assert.AreEqual(1, resultConfig3_3.SubsystemConfig.SubSystem.Index, "SubsystemConfiguration 3_3 Subsystem Index is incorrect.");
            Assert.AreEqual(5, resultConfig3_3.SubsystemConfig.CepoIndex, "SubsystemConfiguration 3_3 Index is incorrect.");
        }

        #endregion

        #region Mode

        /// <summary>
        /// Test Mode decoding with 1 sub 1 config.
        /// </summary>
        [Test]
        public void TestMode11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(RTI.Commands.AdcpCommands.AdcpMode.PROFILE, config.Commands.Mode, "Mode is not correct");
        }

        /// <summary>
        /// Test Mode decoding with 1 sub multiple config.
        /// </summary>
        [Test]
        public void TestMode1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(RTI.Commands.AdcpCommands.AdcpMode.PROFILE, config.Commands.Mode, "Mode is not correct");
        }

        /// <summary>
        /// Test Mode decoding with Multiple sub multiple config.
        /// </summary>
        [Test]
        public void TestModeMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(RTI.Commands.AdcpCommands.AdcpMode.PROFILE, config.Commands.Mode, "Mode is not correct");
        }

        #endregion

        #region CERECORD

        /// <summary>
        /// Test CERECORD decoding with 1 sub 1 config.
        /// </summary>
        [Test]
        public void TestCERECORD11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(true, config.Commands.CERECORD_EnsemblePing, "CERECORD Ensemble Pingis incorrect");
            Assert.AreEqual(true, config.Commands.CERECORD_SinglePing, "CERECORD Single Ping is incorrect");
        }

        /// <summary>
        /// Test CERECORD decoding with 1 sub multiple config.
        /// </summary>
        [Test]
        public void TestCERECORD1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(false, config.Commands.CERECORD_EnsemblePing, "CERECORD Ensemble Ping is incorrect");
            Assert.AreEqual(false, config.Commands.CERECORD_SinglePing, "CERECORD Single Ping is incorrect");
        }

        /// <summary>
        /// Test CERECORD decoding with Multiple sub multiple config.
        /// </summary>
        [Test]
        public void TestCERECORDMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(true, config.Commands.CERECORD_EnsemblePing, "CERECORD Ensemble Ping is incorrect");
            Assert.AreEqual(true, config.Commands.CERECORD_SinglePing, "CERECORD Single Ping is incorrect");
        }

        #endregion

        #region CEOUTPUT

        /// <summary>
        /// Test CEOUTPUT decoding with 1 sub 1 config.
        /// </summary>
        [Test]
        public void TestCEOUTPUT11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(AdcpCommands.AdcpOutputMode.Binary, config.Commands.CEOUTPUT, "CEOUTPUT is incorrect");
        }

        /// <summary>
        /// Test CERECORD decoding with 1 sub multiple config.
        /// </summary>
        [Test]
        public void TestCEOUTPUT1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(AdcpCommands.AdcpOutputMode.Disable, config.Commands.CEOUTPUT, "CEOUTPUT is incorrect");
        }

        /// <summary>
        /// Test CEOUTPUT decoding with Multiple sub multiple config.
        /// </summary>
        [Test]
        public void TestCEOUTPUTMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(AdcpCommands.AdcpOutputMode.Binary, config.Commands.CEOUTPUT, "CEOUTPUT is incorrect");
        }

        #endregion

        #region CEI

        /// <summary>
        /// Test CEI decoding with 1 sub 1 config.
        /// 
        /// CEI 00:00:01.00
        /// </summary>
        [Test]
        public void TestCEI11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(new RTI.Commands.TimeValue(0, 0, 1, 0), config.Commands.CEI, "CEI is incorrect");
            Assert.AreEqual(0, config.Commands.CEI_Hour, "CEI Hour is incorrect.");
            Assert.AreEqual(0, config.Commands.CEI_Minute, "CEI Minute is incorrect.");
            Assert.AreEqual(1, config.Commands.CEI_Second, "CEI Second is incorrect.");
            Assert.AreEqual(0, config.Commands.CEI_HunSec, "CEI HunSec is incorrect.");
        }

        /// <summary>
        /// Test CEI decoding with 1 sub multiple config.
        /// 
        /// CEI 02:00:01.00
        /// </summary>
        [Test]
        public void TestCEI1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(new RTI.Commands.TimeValue(2, 0, 1, 0), config.Commands.CEI, "CEI is incorrect");
            Assert.AreEqual(2, config.Commands.CEI_Hour, "CEI Hour is incorrect.");
            Assert.AreEqual(0, config.Commands.CEI_Minute, "CEI Minute is incorrect.");
            Assert.AreEqual(1, config.Commands.CEI_Second, "CEI Second is incorrect.");
            Assert.AreEqual(0, config.Commands.CEI_HunSec, "CEI HunSec is incorrect.");
        }

        /// <summary>
        /// Test CEI decoding with Multiple sub multiple config.
        /// 
        /// CEI 13:20:01.33
        /// </summary>
        [Test]
        public void TestCEIMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(new RTI.Commands.TimeValue(13, 20, 1, 33), config.Commands.CEI, "CEI is incorrect");
            Assert.AreEqual(13, config.Commands.CEI_Hour, "CEI Hour is incorrect.");
            Assert.AreEqual(20, config.Commands.CEI_Minute, "CEI Minute is incorrect.");
            Assert.AreEqual(1, config.Commands.CEI_Second, "CEI Second is incorrect.");
            Assert.AreEqual(33, config.Commands.CEI_HunSec, "CEI HunSec is incorrect.");
        }

        #endregion

        #region CETFP

        /// <summary>
        /// Test CEI decoding with 1 sub 1 config.
        /// 
        /// CETFP 2012/09/24,12:30:10.25
        /// </summary>
        [Test]
        public void TestCETFP11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(2012, config.Commands.CETFP.Year, "CETFP Year is incorrect");
            Assert.AreEqual(09, config.Commands.CETFP.Month, "CETFP Month is incorrect");
            Assert.AreEqual(24, config.Commands.CETFP.Day, "CETFP Day is incorrect");
            Assert.AreEqual(12, config.Commands.CETFP.Hour, "CETFP Hour is incorrect");
            Assert.AreEqual(30, config.Commands.CETFP.Minute, "CETFP Minute is incorrect");
            Assert.AreEqual(10, config.Commands.CETFP.Second, "CETFP Second is incorrect");
            //Assert.AreEqual(25, config.Commands.CETFP.HunSec, "CETFP HunSec is incorrect");
        }

        /// <summary>
        /// Test CEI decoding with 1 sub multiple config.
        /// 
        /// CEI 02:00:01.00
        /// </summary>
        [Test]
        public void TestCETFP1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(2012, config.Commands.CETFP.Year, "CETFP Year is incorrect");
            Assert.AreEqual(09, config.Commands.CETFP.Month, "CETFP Month is incorrect");
            Assert.AreEqual(24, config.Commands.CETFP.Day, "CETFP Day is incorrect");
            Assert.AreEqual(12, config.Commands.CETFP.Hour, "CETFP Hour is incorrect");
            Assert.AreEqual(30, config.Commands.CETFP.Minute, "CETFP Minute is incorrect");
            Assert.AreEqual(10, config.Commands.CETFP.Second, "CETFP Second is incorrect");
            //Assert.AreEqual(25, config.Commands.CETFP_HunSec, "CETFP HunSec is incorrect");
        }

        /// <summary>
        /// Test CEI decoding with Multiple sub multiple config.
        /// 
        /// CEI 13:20:01.33
        /// </summary>
        [Test]
        public void TestCETFPMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(2012, config.Commands.CETFP.Year, "CETFP Year is incorrect");
            Assert.AreEqual(09, config.Commands.CETFP.Month, "CETFP Month is incorrect");
            Assert.AreEqual(24, config.Commands.CETFP.Day, "CETFP Day is incorrect");
            Assert.AreEqual(12, config.Commands.CETFP.Hour, "CETFP Hour is incorrect");
            Assert.AreEqual(30, config.Commands.CETFP.Minute, "CETFP Minute is incorrect");
            Assert.AreEqual(10, config.Commands.CETFP.Second, "CETFP Second is incorrect");
            //Assert.AreEqual(25, config.Commands.CETFP_HunSec, "CETFP HunSec is incorrect");
        }

        #endregion

        #region WP

        #region CWPON

        /// <summary>
        /// Test CWPON decoding with 1 sub 1 config.
        /// </summary>
        [Test]
        public void TestCWPON11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(true, ssConfig.Commands.CWPON, "CWPON is not correct");
        }

        /// <summary>
        /// Test CWPON decoding with 1 sub multiple config.
        /// CWPON[0] 1 [1] 0 [2] 1 \r\n");
        /// </summary>
        [Test]
        public void TestCWPON1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(true, resultConfig1.Commands.CWPON, "CWPON 1 is not correct");
            Assert.AreEqual(false, resultConfig2.Commands.CWPON, "CWPON 2 is not correct");
            Assert.AreEqual(true, resultConfig3.Commands.CWPON, "CWPON 3 is not correct");
        }

        /// <summary>
        /// Test CWPON decoding with Multiple sub multiple config.
        /// CWPON[0] 1 [1] 0 [2] 1 [3] 1 [4] 0 [5] 1 [6] 1
        /// </summary>
        [Test]
        public void TestCWPONMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            Assert.AreEqual(true, resultConfig2_1.Commands.CWPON, "CWPON 2_1 is not correct");
            Assert.AreEqual(false, resultConfig2_2.Commands.CWPON, "CWPON 2_2 is not correct");
            Assert.AreEqual(true, resultConfig2_3.Commands.CWPON, "CWPON 2_3 is not correct");
            Assert.AreEqual(true, resultConfig2_4.Commands.CWPON, "CWPON 2_4 is not correct");

            Assert.AreEqual(true, resultConfig3_1.Commands.CWPON, "CWPON 3_1 is not correct");
            Assert.AreEqual(false, resultConfig3_2.Commands.CWPON, "CWPON 3_2 is not correct");
            Assert.AreEqual(true, resultConfig3_3.Commands.CWPON, "CWPON 3_3 is not correct");
        }

        #endregion

        #region CWPBB

        /// <summary>
        /// Test CWPBB decoding with 1 sub 1 config.
        /// 
        /// CWPBB[0] 1,0.042
        /// </summary>
        [Test]
        public void TestCWPBB11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(0.042, ssConfig.Commands.CWPBB_LagLength, 0.0001, "CWPBB LagLength is incorrect");
            Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, ssConfig.Commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type is incorrect.");
        }

        /// <summary>
        /// Test CWPBB decoding with 1 sub multiple config.
        /// CWPBB[0] 1,0.042 [1] 2,0.043 [2] 3,0.044
        /// </summary>
        [Test]
        public void TestCWPBB1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(0.042, resultConfig1.Commands.CWPBB_LagLength, 0.0001, "CWPBB LagLength 1 is incorrect");
            Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, resultConfig1.Commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type 1 is incorrect.");

            Assert.AreEqual(0.043, resultConfig2.Commands.CWPBB_LagLength, 0.0001, "CWPBB LagLength 2 is incorrect");
            Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NONCODED_PULSE_TO_PULSE, resultConfig2.Commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type 2 is incorrect.");

            Assert.AreEqual(0.044, resultConfig3.Commands.CWPBB_LagLength, 0.0001, "CWPBB LagLength 3 is incorrect");
            Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE, resultConfig3.Commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type 3 is incorrect.");
        }

        /// <summary>
        /// Test CWPBB decoding with Multiple sub multiple config.
        /// WPBB[0] 1,0.042 [1] 2,0.042 [2] 3,0.084 [3] 2,0.044 [4] 1,0.084 [5] 1,0.085 [6] 3,0.043
        /// </summary>
        [Test]
        public void TestCWPBBMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            Assert.AreEqual(0.042, resultConfig2_1.Commands.CWPBB_LagLength, 0.0001, "CWPBB LagLength 2_1 is incorrect");
            Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, resultConfig2_1.Commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type 2_1 is incorrect.");

            Assert.AreEqual(0.042, resultConfig2_2.Commands.CWPBB_LagLength, 0.0001, "CWPBB LagLength 2_2 is incorrect");
            Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NONCODED_PULSE_TO_PULSE, resultConfig2_2.Commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type 2_2 is incorrect.");

            Assert.AreEqual(0.044, resultConfig2_3.Commands.CWPBB_LagLength, 0.0001, "CWPBB LagLength 2_3 is incorrect");
            Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NONCODED_PULSE_TO_PULSE, resultConfig2_3.Commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type 2_3 is incorrect.");

            Assert.AreEqual(0.043, resultConfig2_4.Commands.CWPBB_LagLength, 0.0001, "CWPBB LagLength 2_4 is incorrect");
            Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE, resultConfig2_4.Commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type 2_4 is incorrect.");



            Assert.AreEqual(0.084, resultConfig3_1.Commands.CWPBB_LagLength, 0.0001, "CWPBB LagLength 3_1 is incorrect");
            Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE, resultConfig3_1.Commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type 3_1 is incorrect.");

            Assert.AreEqual(0.084, resultConfig3_2.Commands.CWPBB_LagLength, 0.0001, "CWPBB LagLength 3_2 is incorrect");
            Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, resultConfig3_2.Commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type 3_2 is incorrect.");

            Assert.AreEqual(0.085, resultConfig3_3.Commands.CWPBB_LagLength, 0.0001, "CWPBB LagLength 3_3 is incorrect");
            Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, resultConfig3_3.Commands.CWPBB_TransmitPulseType, "CWPBB Transmit Pulse Type 3_3 is incorrect.");

        }

        #endregion

        #region CWPAP

        /// <summary>
        /// Test CWPAP decoding with 1 sub 1 config.
        /// 
        /// CWPAP[0] 10,0.15,0.06,0.04,0.01
        /// </summary>
        [Test]
        public void TestCWPAP11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(10, ssConfig.Commands.CWPAP_NumPingsAvg, "CWPAP Num of Pings to Average is incorrect");
            Assert.AreEqual(0.15, ssConfig.Commands.CWPAP_Lag, 0.0001, "CWPAP Lag is incorrect.");
            Assert.AreEqual(0.06, ssConfig.Commands.CWPAP_Blank, 0.0001, "CWPAP Blank is incorrect.");
            Assert.AreEqual(0.04, ssConfig.Commands.CWPAP_BinSize, 0.0001, "CWPAP Bin Size is incorrect.");
            Assert.AreEqual(0.01, ssConfig.Commands.CWPAP_TimeBetweenPing, 0.0001, "CWPAP Time Between Pings is incorrect.");
        }

        /// <summary>
        /// Test CWPAP decoding with 1 sub multiple config.
        /// CWPAP[0] 10,0.15,0.06,0.04,0.01 [1] 11,0.16,0.07,0.05,0.02 [2] 12,0.17,0.08,0.06,0.03
        /// </summary>
        [Test]
        public void TestCWPAP1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            // 10,0.15,0.06,0.04,0.01
            Assert.AreEqual(10, resultConfig1.Commands.CWPAP_NumPingsAvg, "CWPAP 1 Num of Pings to Average is incorrect");
            Assert.AreEqual(0.15, resultConfig1.Commands.CWPAP_Lag, 0.0001, "CWPAP 1 Lag is incorrect.");
            Assert.AreEqual(0.06, resultConfig1.Commands.CWPAP_Blank, 0.0001, "CWPAP 1 Blank is incorrect.");
            Assert.AreEqual(0.04, resultConfig1.Commands.CWPAP_BinSize, 0.0001, "CWPAP 1 Bin Size is incorrect.");
            Assert.AreEqual(0.01, resultConfig1.Commands.CWPAP_TimeBetweenPing, 0.0001, "CWPAP 1 Time Between Pings is incorrect.");

            // 11,0.16,0.07,0.05,0.02
            Assert.AreEqual(11, resultConfig2.Commands.CWPAP_NumPingsAvg, "CWPAP 2 Num of Pings to Average is incorrect");
            Assert.AreEqual(0.16, resultConfig2.Commands.CWPAP_Lag, 0.0001, "CWPAP 2 Lag is incorrect.");
            Assert.AreEqual(0.07, resultConfig2.Commands.CWPAP_Blank, 0.0001, "CWPAP 2 Blank is incorrect.");
            Assert.AreEqual(0.05, resultConfig2.Commands.CWPAP_BinSize, 0.0001, "CWPAP 2 Bin Size is incorrect.");
            Assert.AreEqual(0.02, resultConfig2.Commands.CWPAP_TimeBetweenPing, 0.0001, "CWPAP 2 Time Between Pings is incorrect.");

            // 12,0.17,0.08,0.06,0.03
            Assert.AreEqual(12, resultConfig3.Commands.CWPAP_NumPingsAvg, "CWPAP 3 Num of Pings to Average is incorrect");
            Assert.AreEqual(0.17, resultConfig3.Commands.CWPAP_Lag, 0.0001, "CWPAP 3 Lag is incorrect.");
            Assert.AreEqual(0.08, resultConfig3.Commands.CWPAP_Blank, 0.0001, "CWPAP 3 Blank is incorrect.");
            Assert.AreEqual(0.06, resultConfig3.Commands.CWPAP_BinSize, 0.0001, "CWPAP 3 Bin Size is incorrect.");
            Assert.AreEqual(0.03, resultConfig3.Commands.CWPAP_TimeBetweenPing, 0.0001, "CWPAP 3 Time Between Pings is incorrect.");
        }

        /// <summary>
        /// Test CWPAP decoding with Multiple sub multiple config.
        /// CWPAP[0] 10,0.15,0.06,0.04,0.01 [1] 11,0.16,0.07,0.05,0.02 [2] 12,0.17,0.08,0.06,0.03 [3] 13,0.18,0.09,0.07,0.04 [4] 14,0.19,0.10,0.08,0.05 [5] 15,0.20,0.11,0.09,0.06 [6] 16,0.21,0.12,0.10,0.07
        /// </summary>
        [Test]
        public void TestCWPAPMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 10,0.15,0.06,0.04,0.01
            Assert.AreEqual(10, resultConfig2_1.Commands.CWPAP_NumPingsAvg, "CWPAP 2_1 Num of Pings to Average is incorrect");
            Assert.AreEqual(0.15, resultConfig2_1.Commands.CWPAP_Lag, 0.0001, "CWPAP 2_1 Lag is incorrect.");
            Assert.AreEqual(0.06, resultConfig2_1.Commands.CWPAP_Blank, 0.0001, "CWPAP 2_1 Blank is incorrect.");
            Assert.AreEqual(0.04, resultConfig2_1.Commands.CWPAP_BinSize, 0.0001, "CWPAP 2_1 Bin Size is incorrect.");
            Assert.AreEqual(0.01, resultConfig2_1.Commands.CWPAP_TimeBetweenPing, 0.0001, "CWPAP 2_1 Time Between Pings is incorrect.");

            // [1] 11,0.16,0.07,0.05,0.02
            Assert.AreEqual(11, resultConfig2_2.Commands.CWPAP_NumPingsAvg, "CWPAP 2_2 Num of Pings to Average is incorrect");
            Assert.AreEqual(0.16, resultConfig2_2.Commands.CWPAP_Lag, 0.0001, "CWPAP 2_2 Lag is incorrect.");
            Assert.AreEqual(0.07, resultConfig2_2.Commands.CWPAP_Blank, 0.0001, "CWPAP 2_2 Blank is incorrect.");
            Assert.AreEqual(0.05, resultConfig2_2.Commands.CWPAP_BinSize, 0.0001, "CWPAP 2_2 Bin Size is incorrect.");
            Assert.AreEqual(0.02, resultConfig2_2.Commands.CWPAP_TimeBetweenPing, 0.0001, "CWPAP 2_2 Time Between Pings is incorrect.");

            // [3] 13,0.18,0.09,0.07,0.04
            Assert.AreEqual(13, resultConfig2_3.Commands.CWPAP_NumPingsAvg, "CWPAP 2_3 Num of Pings to Average is incorrect");
            Assert.AreEqual(0.18, resultConfig2_3.Commands.CWPAP_Lag, 0.0001, "CWPAP 2_3 Lag is incorrect.");
            Assert.AreEqual(0.09, resultConfig2_3.Commands.CWPAP_Blank, 0.0001, "CWPAP 2_3 Blank is incorrect.");
            Assert.AreEqual(0.07, resultConfig2_3.Commands.CWPAP_BinSize, 0.0001, "CWPAP 2_3 Bin Size is incorrect.");
            Assert.AreEqual(0.04, resultConfig2_3.Commands.CWPAP_TimeBetweenPing, 0.0001, "CWPAP 2_3 Time Between Pings is incorrect.");

            // [6] 16,0.21,0.12,0.10,0.07
            Assert.AreEqual(16, resultConfig2_4.Commands.CWPAP_NumPingsAvg, "CWPAP 2_4 Num of Pings to Average is incorrect");
            Assert.AreEqual(0.21, resultConfig2_4.Commands.CWPAP_Lag, 0.0001, "CWPAP 2_4 Lag is incorrect.");
            Assert.AreEqual(0.12, resultConfig2_4.Commands.CWPAP_Blank, 0.0001, "CWPAP 2_4 Blank is incorrect.");
            Assert.AreEqual(0.10, resultConfig2_4.Commands.CWPAP_BinSize, 0.0001, "CWPAP 2_4 Bin Size is incorrect.");
            Assert.AreEqual(0.07, resultConfig2_4.Commands.CWPAP_TimeBetweenPing, 0.0001, "CWPAP 2_4 Time Between Pings is incorrect.");



            // [2] 12,0.17,0.08,0.06,0.03
            Assert.AreEqual(12, resultConfig3_1.Commands.CWPAP_NumPingsAvg, "CWPAP 3_1 Num of Pings to Average is incorrect");
            Assert.AreEqual(0.17, resultConfig3_1.Commands.CWPAP_Lag, 0.0001, "CWPAP 3_1 Lag is incorrect.");
            Assert.AreEqual(0.08, resultConfig3_1.Commands.CWPAP_Blank, 0.0001, "CWPAP 3_1 Blank is incorrect.");
            Assert.AreEqual(0.06, resultConfig3_1.Commands.CWPAP_BinSize, 0.0001, "CWPAP 3_1 Bin Size is incorrect.");
            Assert.AreEqual(0.03, resultConfig3_1.Commands.CWPAP_TimeBetweenPing, 0.0001, "CWPAP 3_1 Time Between Pings is incorrect.");

            // [4] 14,0.19,0.10,0.08,0.05
            Assert.AreEqual(14, resultConfig3_2.Commands.CWPAP_NumPingsAvg, "CWPAP 3_2 Num of Pings to Average is incorrect");
            Assert.AreEqual(0.19, resultConfig3_2.Commands.CWPAP_Lag, 0.0001, "CWPAP 3_2 Lag is incorrect.");
            Assert.AreEqual(0.10, resultConfig3_2.Commands.CWPAP_Blank, 0.0001, "CWPAP 3_2 Blank is incorrect.");
            Assert.AreEqual(0.08, resultConfig3_2.Commands.CWPAP_BinSize, 0.0001, "CWPAP 3_2 Bin Size is incorrect.");
            Assert.AreEqual(0.05, resultConfig3_2.Commands.CWPAP_TimeBetweenPing, 0.0001, "CWPAP 3_2 Time Between Pings is incorrect.");

            // [5] 15,0.20,0.11,0.09,0.06
            Assert.AreEqual(15, resultConfig3_3.Commands.CWPAP_NumPingsAvg, "CWPAP 3_3 Num of Pings to Average is incorrect");
            Assert.AreEqual(0.20, resultConfig3_3.Commands.CWPAP_Lag, 0.0001, "CWPAP 3_3 Lag is incorrect.");
            Assert.AreEqual(0.11, resultConfig3_3.Commands.CWPAP_Blank, 0.0001, "CWPAP 3_3 Blank is incorrect.");
            Assert.AreEqual(0.09, resultConfig3_3.Commands.CWPAP_BinSize, 0.0001, "CWPAP 3_3 Bin Size is incorrect.");
            Assert.AreEqual(0.06, resultConfig3_3.Commands.CWPAP_TimeBetweenPing, 0.0001, "CWPAP 3_3 Time Between Pings is incorrect.");

        }

        #endregion

        #region CWPST

        /// <summary>
        /// Test CWPST decoding with 1 sub 1 config.
        /// 
        /// CWPST[0] 0.400,1.000,1.001
        /// </summary>
        [Test]
        public void TestCWPST11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(0.400, ssConfig.Commands.CWPST_CorrelationThresh, 0.0001, "CWPST Correlation Threshold is incorrect");
            Assert.AreEqual(1.000, ssConfig.Commands.CWPST_QVelocityThresh, 0.0001, "CWPST Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.001, ssConfig.Commands.CWPST_VVelocityThresh, 0.0001, "CWPST V Velocity Threshold is incorrect");
        }

        /// <summary>
        /// Test CWPST decoding with 1 sub multiple config.
        /// 
        /// CWPST[0] 0.400,1.000,1.001 [1] 0.500,1.002,1.003 [2] 0.600,1.004,1.005
        /// </summary>
        [Test]
        public void TestCWPST1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(0.400, resultConfig1.Commands.CWPST_CorrelationThresh, 0.0001, "CWPST 1 Correlation Threshold is incorrect");
            Assert.AreEqual(1.000, resultConfig1.Commands.CWPST_QVelocityThresh, 0.0001, "CWPST 1 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.001, resultConfig1.Commands.CWPST_VVelocityThresh, 0.0001, "CWPST 1 V Velocity Threshold is incorrect");

            Assert.AreEqual(0.500, resultConfig2.Commands.CWPST_CorrelationThresh, 0.0001, "CWPST 2 Correlation Threshold is incorrect");
            Assert.AreEqual(1.002, resultConfig2.Commands.CWPST_QVelocityThresh, 0.0001, "CWPST 2 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.003, resultConfig2.Commands.CWPST_VVelocityThresh, 0.0001, "CWPST 2 V Velocity Threshold is incorrect");

            Assert.AreEqual(0.600, resultConfig3.Commands.CWPST_CorrelationThresh, 0.0001, "CWPST 3 Correlation Threshold is incorrect");
            Assert.AreEqual(1.004, resultConfig3.Commands.CWPST_QVelocityThresh, 0.0001, "CWPST 3 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.005, resultConfig3.Commands.CWPST_VVelocityThresh, 0.0001, "CWPST 3 V Velocity Threshold is incorrect");
        }

        /// <summary>
        /// Test CWPST decoding with Multiple sub multiple config.
        /// 
        /// CWPST[0] 0.400,1.001,1.002 [1] 0.500,1.003,1.004 [2] 0.600,1.005,1.006 [3] 0.700,1.007,1.008 [4] 0.800,1.009,1.010 [5] 0.900,1.011,1.012 [6] 0.300,1.013,1.014
        /// </summary>
        [Test]
        public void TestCWPSTMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 0.400,1.001,1.002
            Assert.AreEqual(0.400, resultConfig2_1.Commands.CWPST_CorrelationThresh, 0.0001, "CWPST 2_1 Correlation Threshold is incorrect");
            Assert.AreEqual(1.001, resultConfig2_1.Commands.CWPST_QVelocityThresh, 0.0001, "CWPST 2_1 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.002, resultConfig2_1.Commands.CWPST_VVelocityThresh, 0.0001, "CWPST 2_1 V Velocity Threshold is incorrect");
            // [1] 0.500,1.003,1.004
            Assert.AreEqual(0.500, resultConfig2_2.Commands.CWPST_CorrelationThresh, 0.0001, "CWPST 2_2 Correlation Threshold is incorrect");
            Assert.AreEqual(1.003, resultConfig2_2.Commands.CWPST_QVelocityThresh, 0.0001, "CWPST 2_2 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.004, resultConfig2_2.Commands.CWPST_VVelocityThresh, 0.0001, "CWPST 2_2 V Velocity Threshold is incorrect");
            // [3] 0.700,1.007,1.008
            Assert.AreEqual(0.700, resultConfig2_3.Commands.CWPST_CorrelationThresh, 0.0001, "CWPST 2_3 Correlation Threshold is incorrect");
            Assert.AreEqual(1.007, resultConfig2_3.Commands.CWPST_QVelocityThresh, 0.0001, "CWPST 2_3 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.008, resultConfig2_3.Commands.CWPST_VVelocityThresh, 0.0001, "CWPST 2_3 V Velocity Threshold is incorrect");
            // [6] 0.300,1.013,1.014
            Assert.AreEqual(0.300, resultConfig2_4.Commands.CWPST_CorrelationThresh, 0.0001, "CWPST 2_4 Correlation Threshold is incorrect");
            Assert.AreEqual(1.013, resultConfig2_4.Commands.CWPST_QVelocityThresh, 0.0001, "CWPST 2_4 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.014, resultConfig2_4.Commands.CWPST_VVelocityThresh, 0.0001, "CWPST 2_4 V Velocity Threshold is incorrect");


            // [2] 0.600,1.005,1.006
            Assert.AreEqual(0.600, resultConfig3_1.Commands.CWPST_CorrelationThresh, 0.0001, "CWPST 3_1 Correlation Threshold is incorrect");
            Assert.AreEqual(1.005, resultConfig3_1.Commands.CWPST_QVelocityThresh, 0.0001, "CWPST 3_1 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.006, resultConfig3_1.Commands.CWPST_VVelocityThresh, 0.0001, "CWPST 3_1 V Velocity Threshold is incorrect");
            // [4] 0.800,1.009,1.010
            Assert.AreEqual(0.800, resultConfig3_2.Commands.CWPST_CorrelationThresh, 0.0001, "CWPST 3_2 Correlation Threshold is incorrect");
            Assert.AreEqual(1.009, resultConfig3_2.Commands.CWPST_QVelocityThresh, 0.0001, "CWPST 3_2 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.010, resultConfig3_2.Commands.CWPST_VVelocityThresh, 0.0001, "CWPST 3_2 V Velocity Threshold is incorrect");
            // [5] 0.900,1.011,1.012
            Assert.AreEqual(0.900, resultConfig3_3.Commands.CWPST_CorrelationThresh, 0.0001, "CWPST 3_3 Correlation Threshold is incorrect");
            Assert.AreEqual(1.011, resultConfig3_3.Commands.CWPST_QVelocityThresh, 0.0001, "CWPST 3_3 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.012, resultConfig3_3.Commands.CWPST_VVelocityThresh, 0.0001, "CWPST 3_3 V Velocity Threshold is incorrect");

        }

        #endregion

        #region CWPBL

        /// <summary>
        /// Test CWPBL decoding with 1 sub 1 config.
        /// 
        /// CWPBL[0] 0.10
        /// </summary>
        [Test]
        public void TestCWPBL11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(0.10, ssConfig.Commands.CWPBL, 0.0001, "CWPBL is incorrect");
        }

        /// <summary>
        /// Test CWPBL decoding with 1 sub multiple config.
        /// 
        /// CWPBL[0] 0.10 [1] 0.11 [2] 0.12
        /// </summary>
        [Test]
        public void TestCWPBL1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(0.10, resultConfig1.Commands.CWPBL, 0.0001, "CWPBL 1 is incorrect");
            Assert.AreEqual(0.11, resultConfig2.Commands.CWPBL, 0.0001, "CWPBL 2 is incorrect");
            Assert.AreEqual(0.12, resultConfig3.Commands.CWPBL, 0.0001, "CWPBL 3 is incorrect");
        }

        /// <summary>
        /// Test CWPBL decoding with Multiple sub multiple config.
        /// 
        /// CWPBL[0] 0.10 [1] 0.11 [2] 0.22 [3] 0.13 [4] 0.24 [5] 0.25 [6] 0.16
        /// </summary>
        [Test]
        public void TestCWPBLMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 0.10
            Assert.AreEqual(0.10, resultConfig2_1.Commands.CWPBL, 0.0001, "CWPBL 2_1 is incorrect");
            // [1] 0.11
            Assert.AreEqual(0.11, resultConfig2_2.Commands.CWPBL, 0.0001, "CWPBL 2_2 is incorrect");
            // [3] 0.13
            Assert.AreEqual(0.13, resultConfig2_3.Commands.CWPBL, 0.0001, "CWPBL 2_3 is incorrect");
            // [6] 0.16
            Assert.AreEqual(0.16, resultConfig2_4.Commands.CWPBL, 0.0001, "CWPBL 2_4 is incorrect");


            // [2] 0.22
            Assert.AreEqual(0.22, resultConfig3_1.Commands.CWPBL, 0.0001, "CWPBL 3_1 is incorrect");
            // [4] 0.24
            Assert.AreEqual(0.24, resultConfig3_2.Commands.CWPBL, 0.0001, "CWPBL 3_2 is incorrect");
            // [5] 0.25
            Assert.AreEqual(0.25, resultConfig3_3.Commands.CWPBL, 0.0001, "CWPBL 3_3 is incorrect");

        }

        #endregion

        #region CWPBS

        /// <summary>
        /// Test CWPBS decoding with 1 sub 1 config.
        /// 
        /// CWPBS[0] 1.00
        /// </summary>
        [Test]
        public void TestCWPBS11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(1.00, ssConfig.Commands.CWPBS, 0.0001, "CWPBS is incorrect");
        }

        /// <summary>
        /// Test CWPBS decoding with 1 sub multiple config.
        /// 
        /// CWPBS[0] 1.00 [1] 1.01 [2] 1.02
        /// </summary>
        [Test]
        public void TestCWPBS1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(1.00, resultConfig1.Commands.CWPBS, 0.0001, "CWPBS 1 is incorrect");
            Assert.AreEqual(1.01, resultConfig2.Commands.CWPBS, 0.0001, "CWPBS 2 is incorrect");
            Assert.AreEqual(1.02, resultConfig3.Commands.CWPBS, 0.0001, "CWPBS 3 is incorrect");
        }

        /// <summary>
        /// Test CWPBS decoding with Multiple sub multiple config.
        /// 
        /// CWPBS[0] 1.00 [1] 1.01 [2] 2.02 [3] 1.03 [4] 2.04 [5] 2.05 [6] 1.06
        /// </summary>
        [Test]
        public void TestCWPBSMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 1.00
            Assert.AreEqual(1.00, resultConfig2_1.Commands.CWPBS, 0.0001, "CWPBS 2_1 is incorrect");
            // [1] 1.01
            Assert.AreEqual(1.01, resultConfig2_2.Commands.CWPBS, 0.0001, "CWPBS 2_2 is incorrect");
            // [3] 1.03
            Assert.AreEqual(1.03, resultConfig2_3.Commands.CWPBS, 0.0001, "CWPBS 2_3 is incorrect");
            // [6] 1.06
            Assert.AreEqual(1.06, resultConfig2_4.Commands.CWPBS, 0.0001, "CWPBS 2_4 is incorrect");


            // [2] 2.02
            Assert.AreEqual(2.02, resultConfig3_1.Commands.CWPBS, 0.0001, "CWPBS 3_1 is incorrect");
            // [4] 2.04
            Assert.AreEqual(2.04, resultConfig3_2.Commands.CWPBS, 0.0001, "CWPBS 3_2 is incorrect");
            // [5] 2.05
            Assert.AreEqual(2.05, resultConfig3_3.Commands.CWPBS, 0.0001, "CWPBS 3_3 is incorrect");

        }

        #endregion

        #region CWPX

        /// <summary>
        /// Test CWPX decoding with 1 sub 1 config.
        /// 
        /// CWPX [0] 0.01 
        /// </summary>
        [Test]
        public void TestCWPX11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(0.01, ssConfig.Commands.CWPX, 0.0001, "CWPX is incorrect");
        }

        /// <summary>
        /// Test CWPX decoding with 1 sub multiple config.
        /// 
        /// CWPX [0] 0.01 [1] 0.02 [2] 0.03
        /// </summary>
        [Test]
        public void TestCWPX1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(0.01, resultConfig1.Commands.CWPX, 0.0001, "CWPX 1 is incorrect");
            Assert.AreEqual(0.02, resultConfig2.Commands.CWPX, 0.0001, "CWPX 2 is incorrect");
            Assert.AreEqual(0.03, resultConfig3.Commands.CWPX, 0.0001, "CWPX 3 is incorrect");
        }

        /// <summary>
        /// Test CWPX decoding with Multiple sub multiple config.
        /// 
        /// CWPX [0] 0.01 [1] 0.02 [2] 0.03 [3] 0.04 [4] 0.05 [5] 0.06 [6] 0.70
        /// </summary>
        [Test]
        public void TestCWPXMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 0.01
            Assert.AreEqual(0.01, resultConfig2_1.Commands.CWPX, 0.0001, "CWPX 2_1 is incorrect");
            // [1] 0.02
            Assert.AreEqual(0.02, resultConfig2_2.Commands.CWPX, 0.0001, "CWPX 2_2 is incorrect");
            // [3] 0.04
            Assert.AreEqual(0.04, resultConfig2_3.Commands.CWPX, 0.0001, "CWPX 2_3 is incorrect");
            // [6] 0.70
            Assert.AreEqual(0.70, resultConfig2_4.Commands.CWPX, 0.0001, "CWPX 2_4 is incorrect");


            // [2] 0.03
            Assert.AreEqual(0.03, resultConfig3_1.Commands.CWPX, 0.0001, "CWPX 3_1 is incorrect");
            // [4] 0.05
            Assert.AreEqual(0.05, resultConfig3_2.Commands.CWPX, 0.0001, "CWPX 3_2 is incorrect");
            // [5] 0.06
            Assert.AreEqual(0.06, resultConfig3_3.Commands.CWPX, 0.0001, "CWPX 3_3 is incorrect");

        }

        #endregion

        #region CWPBN

        /// <summary>
        /// Test CWPBN decoding with 1 sub 1 config.
        /// 
        /// CWPBN[0] 30
        /// </summary>
        [Test]
        public void TestCWPBN11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(30, ssConfig.Commands.CWPBN, "CWPBN is incorrect");
        }

        /// <summary>
        /// Test CWPBN decoding with 1 sub multiple config.
        /// 
        /// CWPBN[0] 30 [1] 31 [2] 32
        /// </summary>
        [Test]
        public void TestCWPBN1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(30, resultConfig1.Commands.CWPBN, "CWPBN 1 is incorrect");
            Assert.AreEqual(31, resultConfig2.Commands.CWPBN, "CWPBN 2 is incorrect");
            Assert.AreEqual(32, resultConfig3.Commands.CWPBN, "CWPBN 3 is incorrect");
        }

        /// <summary>
        /// Test CWPBN decoding with Multiple sub multiple config.
        /// 
        /// CWPBN[0] 30 [1] 31 [2] 32 [3] 33 [4] 34 [5] 35 [6] 36
        /// </summary>
        [Test]
        public void TestCWPBNMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 30
            Assert.AreEqual(30, resultConfig2_1.Commands.CWPBN, "CWPBN 2_1 is incorrect");
            // [1] 31
            Assert.AreEqual(31, resultConfig2_2.Commands.CWPBN, "CWPBN 2_2 is incorrect");
            // [3] 33
            Assert.AreEqual(33, resultConfig2_3.Commands.CWPBN, "CWPBN 2_3 is incorrect");
            // [6] 36
            Assert.AreEqual(36, resultConfig2_4.Commands.CWPBN, "CWPBN 2_4 is incorrect");


            // [2] 32
            Assert.AreEqual(32, resultConfig3_1.Commands.CWPBN, "CWPBN 3_1 is incorrect");
            // [4] 34
            Assert.AreEqual(34, resultConfig3_2.Commands.CWPBN, "CWPBN 3_2 is incorrect");
            // [5] 35
            Assert.AreEqual(35, resultConfig3_3.Commands.CWPBN, "CWPBN 3_3 is incorrect");

        }

        #endregion

        #region CWPP

        /// <summary>
        /// Test CWPP decoding with 1 sub 1 config.
        /// 
        /// CWPP[0] 1
        /// </summary>
        [Test]
        public void TestCWPP11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(1, ssConfig.Commands.CWPP, "CWPP is incorrect");
        }

        /// <summary>
        /// Test CWPP decoding with 1 sub multiple config.
        /// 
        /// CWPP[0] 1 [1] 2 [2] 3
        /// </summary>
        [Test]
        public void TestCWPP1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(1, resultConfig1.Commands.CWPP, "CWPP 1 is incorrect");
            Assert.AreEqual(2, resultConfig2.Commands.CWPP, "CWPP 2 is incorrect");
            Assert.AreEqual(3, resultConfig3.Commands.CWPP, "CWPP 3 is incorrect");
        }

        /// <summary>
        /// Test CWPP decoding with Multiple sub multiple config.
        /// 
        /// CWPP[0] 1 [1] 2 [2] 3 [3] 4 [4] 5 [5] 6 [6] 7
        /// </summary>
        [Test]
        public void TestCWPPMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 1
            Assert.AreEqual(1, resultConfig2_1.Commands.CWPP, "CWPP 2_1 is incorrect");
            // [1] 2
            Assert.AreEqual(2, resultConfig2_2.Commands.CWPP, "CWPP 2_2 is incorrect");
            // [3] 4
            Assert.AreEqual(4, resultConfig2_3.Commands.CWPP, "CWPP 2_3 is incorrect");
            // [6] 7
            Assert.AreEqual(7, resultConfig2_4.Commands.CWPP, "CWPP 2_4 is incorrect");


            // [2] 3
            Assert.AreEqual(3, resultConfig3_1.Commands.CWPP, "CWPP 3_1 is incorrect");
            // [4] 5
            Assert.AreEqual(5, resultConfig3_2.Commands.CWPP, "CWPP 3_2 is incorrect");
            // [5] 6
            Assert.AreEqual(6, resultConfig3_3.Commands.CWPP, "CWPP 3_3 is incorrect");

        }

        #endregion

        #region CWPBP

        /// <summary>
        /// Test CWPBP decoding with 1 sub 1 config.
        /// 
        /// CWPBP[0] 1,0.02
        /// </summary>
        [Test]
        public void TestCWPBP11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(1, ssConfig.Commands.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg is incorrect");
            Assert.AreEqual(0.02, ssConfig.Commands.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP_TimeBetweenBasePings is incorrect");
        }

        /// <summary>
        /// Test CWPBP decoding with 1 sub multiple config.
        /// 
        /// CWPBP[0] 1,0.02 [1] 2,0.04 [2] 3,0.05
        /// </summary>
        [Test]
        public void TestCWPBP1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(1, resultConfig1.Commands.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg 1 is incorrect");
            Assert.AreEqual(0.02, resultConfig1.Commands.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP_TimeBetweenBasePings 1 is incorrect");

            Assert.AreEqual(2, resultConfig2.Commands.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg 2 is incorrect");
            Assert.AreEqual(0.04, resultConfig2.Commands.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP_TimeBetweenBasePings 2 is incorrect");

            Assert.AreEqual(3, resultConfig3.Commands.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg 3 is incorrect");
            Assert.AreEqual(0.05, resultConfig3.Commands.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP_TimeBetweenBasePings 3 is incorrect");
        }

        /// <summary>
        /// Test CWPBP decoding with Multiple sub multiple config.
        /// 
        /// CWPBP[0] 1,0.02 [1] 2,0.03 [2] 3,0.04 [3] 4,0.05 [4] 5,0.06 [5] 6,0.07 [6] 7,0.08
        /// </summary>
        [Test]
        public void TestCWPBPMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 1,0.02
            Assert.AreEqual(1, resultConfig2_1.Commands.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg 2_1 is incorrect");
            Assert.AreEqual(0.02, resultConfig2_1.Commands.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP_TimeBetweenBasePings 2_1 is incorrect");
            // [1] 2,0.03
            Assert.AreEqual(2, resultConfig2_2.Commands.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg 2_2 is incorrect");
            Assert.AreEqual(0.03, resultConfig2_2.Commands.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP_TimeBetweenBasePings 2_2 is incorrect");
            // [3] 4,0.05
            Assert.AreEqual(4, resultConfig2_3.Commands.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg 2_3 is incorrect");
            Assert.AreEqual(0.05, resultConfig2_3.Commands.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP_TimeBetweenBasePings 2_3 is incorrect");
            // [6] 7,0.08
            Assert.AreEqual(7, resultConfig2_4.Commands.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg 2_4 is incorrect");
            Assert.AreEqual(0.08, resultConfig2_4.Commands.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP_TimeBetweenBasePings 2_4 is incorrect");


            // [2] 3,0.04
            Assert.AreEqual(3, resultConfig3_1.Commands.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg 3_1 is incorrect");
            Assert.AreEqual(0.04, resultConfig3_1.Commands.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP_TimeBetweenBasePings 3_1 is incorrect");
            // [4] 5,0.06
            Assert.AreEqual(5, resultConfig3_2.Commands.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg 3_2 is incorrect");
            Assert.AreEqual(0.06, resultConfig3_2.Commands.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP_TimeBetweenBasePings 3_2 is incorrect");
            // [5] 6,0.07
            Assert.AreEqual(6, resultConfig3_3.Commands.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg 3_3 is incorrect");
            Assert.AreEqual(0.07, resultConfig3_3.Commands.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP_TimeBetweenBasePings 3_3 is incorrect");

        }

        #endregion

        #region CWPTBP

        /// <summary>
        /// Test CWPTBP decoding with 1 sub 1 config.
        /// 
        /// CWPTBP[0] 0.13
        /// </summary>
        [Test]
        public void TestCWPTBP11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(0.13, ssConfig.Commands.CWPTBP, 0.0001, "CWPTBP is incorrect");
        }

        /// <summary>
        /// Test CWPTBP decoding with 1 sub multiple config.
        /// 
        /// CWPTBP[0] 0.13 [1] 0.14 [2] 0.15
        /// </summary>
        [Test]
        public void TestCWPTBP1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(0.13, resultConfig1.Commands.CWPTBP, 0.0001, "CWPTBP 1 is incorrect");
            Assert.AreEqual(0.14, resultConfig2.Commands.CWPTBP, 0.0001, "CWPTBP 2 is incorrect");
            Assert.AreEqual(0.15, resultConfig3.Commands.CWPTBP, 0.0001, "CWPTBP 3 is incorrect");
        }

        /// <summary>
        /// Test CWPTBP decoding with Multiple sub multiple config.
        /// 
        /// CWPTBP[0] 0.13 [1] 0.14 [2] 0.25 [3] 0.16 [4] 0.27 [5] 0.28 [6] 0.19
        /// </summary>
        [Test]
        public void TestCWPTBPMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 0.13
            Assert.AreEqual(0.13, resultConfig2_1.Commands.CWPTBP, 0.0001, "CWPTBP 2_1 is incorrect");
            // [0] 0.14
            Assert.AreEqual(0.14, resultConfig2_2.Commands.CWPTBP, 0.0001, "CWPTBP 2_2 is incorrect");
            // [3] 0.16
            Assert.AreEqual(0.16, resultConfig2_3.Commands.CWPTBP, 0.0001, "CWPTBP 2_3 is incorrect");
            // [6] 0.19
            Assert.AreEqual(0.19, resultConfig2_4.Commands.CWPTBP, 0.0001, "CWPTBP 2_4 is incorrect");


            // [2] 0.25
            Assert.AreEqual(0.25, resultConfig3_1.Commands.CWPTBP, 0.0001, "CWPTBP 3_1 is incorrect");
            // [4] 0.27
            Assert.AreEqual(0.27, resultConfig3_2.Commands.CWPTBP, 0.0001, "CWPTBP 3_2 is incorrect");
            // [5] 0.28
            Assert.AreEqual(0.28, resultConfig3_3.Commands.CWPTBP, 0.0001, "CWPTBP 3_3 is incorrect");

        }

        #endregion

        #region CWPAI

        /// <summary>
        /// Test CWPAI decoding with 1 sub 1 config.
        /// 
        /// CWPAI[0] 03:02:00.10
        /// </summary>
        [Test]
        public void TestCWPAI11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(new TimeValue(03, 02, 00, 10), ssConfig.Commands.CWPAI, "CWPAI is incorrect");
        }

        /// <summary>
        /// Test CWPAI decoding with 1 sub multiple config.
        /// 
        /// CWPAI[0] 01:02:03.04 [1] 05:06:07.08 [2] 09:10:11.12
        /// </summary>
        [Test]
        public void TestCWPAI1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(new TimeValue(1, 2, 3, 4), resultConfig1.Commands.CWPAI, "CWPAI 1 is incorrect");
            Assert.AreEqual(new TimeValue(5, 6, 7, 8), resultConfig2.Commands.CWPAI, "CWPAI 2 is incorrect");
            Assert.AreEqual(new TimeValue(9, 10, 11, 12), resultConfig3.Commands.CWPAI, "CWPAI 3 is incorrect");
        }

        /// <summary>
        /// Test CWPAI decoding with Multiple sub multiple config.
        /// 
        /// CWPAI[0] 01:02:03.04 [1] 05:06:07.08 [2] 09:10:11.12 [3] 13:14:15.16 [4] 17:18:19.20 [5] 20:21:22.23 [6] 24:25:26.27
        /// </summary>
        [Test]
        public void TestCWPAIMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 01:02:03.04
            Assert.AreEqual(new TimeValue(1,2,3,4), resultConfig2_1.Commands.CWPAI, "CWPAI 2_1 is incorrect");
            // [1] 05:06:07.08
            Assert.AreEqual(new TimeValue(5,6,7,8), resultConfig2_2.Commands.CWPAI, "CWPAI 2_2 is incorrect");
            // [3] 13:14:15.16
            Assert.AreEqual(new TimeValue(13,14,15,16), resultConfig2_3.Commands.CWPAI, "CWPAI 2_3 is incorrect");
            // [6] 24:25:26.27
            Assert.AreEqual(new TimeValue(24,25,26,27), resultConfig2_4.Commands.CWPAI, "CWPAI 2_4 is incorrect");


            // [2] 09:10:11.12 
            Assert.AreEqual(new TimeValue(9,10,11,12), resultConfig3_1.Commands.CWPAI, "CWPAI 3_1 is incorrect");
            // [4] 17:18:19.20
            Assert.AreEqual(new TimeValue(17,18,19,20), resultConfig3_2.Commands.CWPAI, "CWPAI 3_2 is incorrect");
            // [5] 20:21:22.23
            Assert.AreEqual(new TimeValue(20,21,22,23), resultConfig3_3.Commands.CWPAI, "CWPAI 3_3 is incorrect");

        }

        #endregion

        #region CBI

        /// <summary>
        /// Test CBI decoding with 1 sub 1 config.
        /// 
        /// CBI[0] 00:00:01.00,100
        /// </summary>
        [Test]
        public void TestCBI11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.IsNotNull(ssConfig, "Adcp Subsystem Configuration was not found.");
            Assert.AreEqual(new TimeValue(00, 00, 01, 00), ssConfig.Commands.CBI_BurstInterval, "CBI_BurstInterval is incorrect");
            Assert.AreEqual(100, ssConfig.Commands.CBI_NumEnsembles, "CBI_NumEnsembles is incorrect.");
        }

        /// <summary>
        /// Test CBI decoding with 1 sub multiple config.
        /// 
        /// CBI[0] 00:00:01.01,101 [1] 00:00:01.02,102 [2] 00:00:03.01,103
        /// </summary>
        [Test]
        public void TestCBI1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(new TimeValue(00, 00, 01, 01), resultConfig1.Commands.CBI_BurstInterval, "CBI_BurstInterval 1 is incorrect");
            Assert.AreEqual(101, resultConfig1.Commands.CBI_NumEnsembles, "CBI_NumEnsembles 1 is incorrect.");

            Assert.AreEqual(new TimeValue(00, 00, 01, 02), resultConfig2.Commands.CBI_BurstInterval, "CBI_BurstInterval 2 is incorrect");
            Assert.AreEqual(102, resultConfig2.Commands.CBI_NumEnsembles, "CBI_NumEnsembles 2 is incorrect.");

            Assert.AreEqual(new TimeValue(00, 00, 03, 01), resultConfig3.Commands.CBI_BurstInterval, "CBI_BurstInterval 3 is incorrect");
            Assert.AreEqual(103, resultConfig3.Commands.CBI_NumEnsembles, "CBI_NumEnsembles 3 is incorrect.");
        }

        /// <summary>
        /// Test CWPAI decoding with Multiple sub multiple config.
        /// 
        /// CBI[0] 00:00:01.01,101 [1] 00:00:01.02,102 [2] 00:02:03.01,102 [3] 00:00:01.03,103 [4] 00:00:03.04,104 [5] 00:00:01.05,105 [6] 00:06:03.01,106
        /// </summary>
        [Test]
        public void TestCBIMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 00:00:01.01,101
            Assert.AreEqual(new TimeValue(00, 00, 01, 01), resultConfig2_1.Commands.CBI_BurstInterval, "CBI_BurstInterval 2_1 is incorrect");
            Assert.AreEqual(101, resultConfig2_1.Commands.CBI_NumEnsembles, "CBI_NumEnsembles 2_1 is incorrect.");
            // [1] 00:00:01.02,102
            Assert.AreEqual(new TimeValue(00, 00, 01, 02), resultConfig2_2.Commands.CBI_BurstInterval, "CBI_BurstInterval 2_2 is incorrect");
            Assert.AreEqual(102, resultConfig2_2.Commands.CBI_NumEnsembles, "CBI_NumEnsembles 2_2 is incorrect.");
            // [3] 00:00:01.03,103
            Assert.AreEqual(new TimeValue(00, 00, 01, 03), resultConfig2_3.Commands.CBI_BurstInterval, "CBI_BurstInterval 2_3 is incorrect");
            Assert.AreEqual(103, resultConfig2_3.Commands.CBI_NumEnsembles, "CBI_NumEnsembles 2_3 is incorrect.");
            // [6] 00:06:03.01,106
            Assert.AreEqual(new TimeValue(00, 06, 03, 01), resultConfig2_4.Commands.CBI_BurstInterval, "CBI_BurstInterval 2_4 is incorrect");
            Assert.AreEqual(106, resultConfig2_4.Commands.CBI_NumEnsembles, "CBI_NumEnsembles 2_4 is incorrect.");


            // [2] 00:02:03.01,102
            Assert.AreEqual(new TimeValue(00, 02, 03, 01), resultConfig3_1.Commands.CBI_BurstInterval, "CBI_BurstInterval 3_1 is incorrect");
            Assert.AreEqual(102, resultConfig3_1.Commands.CBI_NumEnsembles, "CBI_NumEnsembles 3_1 is incorrect.");
            // [4] 00:00:03.04,104
            Assert.AreEqual(new TimeValue(00, 00, 03, 04), resultConfig3_2.Commands.CBI_BurstInterval, "CBI_BurstInterval 3_2 is incorrect");
            Assert.AreEqual(104, resultConfig3_2.Commands.CBI_NumEnsembles, "CBI_NumEnsembles 3_2 is incorrect.");
            // [5] 00:00:01.05,105
            Assert.AreEqual(new TimeValue(00, 00, 01, 05), resultConfig3_3.Commands.CBI_BurstInterval, "CBI_BurstInterval 3_3 is incorrect");
            Assert.AreEqual(105, resultConfig3_3.Commands.CBI_NumEnsembles, "CBI_NumEnsembles 3_3 is incorrect.");

        }

        #endregion

        #endregion

        #region BT

        #region CBTON

        /// <summary>
        /// Test CBTON decoding with 1 sub 1 config.
        /// </summary>
        [Test]
        public void TestCBTON11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(true, ssConfig.Commands.CBTON, "CBTON is not correct");
        }

        /// <summary>
        /// Test CWPON decoding with 1 sub multiple config.
        /// CBTON[0] 1 [1] 0 [2] 1 \r\n");
        /// </summary>
        [Test]
        public void TestCBTON1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(true, resultConfig1.Commands.CBTON, "CBTON 1 is not correct");
            Assert.AreEqual(false, resultConfig2.Commands.CBTON, "CBTON 2 is not correct");
            Assert.AreEqual(true, resultConfig3.Commands.CBTON, "CBTON 3 is not correct");
        }

        /// <summary>
        /// Test CBTON decoding with Multiple sub multiple config.
        /// 
        /// CBTON[0] 1 [1] 0 [2] 1 [3] 0 [4] 0 [5] 1 [6] 0
        /// </summary>
        [Test]
        public void TestCBTONMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            Assert.AreEqual(true, resultConfig2_1.Commands.CBTON, "CBTON 2_1 is not correct");
            Assert.AreEqual(false, resultConfig2_2.Commands.CBTON, "CBTON 2_2 is not correct");
            Assert.AreEqual(false, resultConfig2_3.Commands.CBTON, "CBTON 2_3 is not correct");
            Assert.AreEqual(false, resultConfig2_4.Commands.CBTON, "CBTON 2_4 is not correct");

            Assert.AreEqual(true, resultConfig3_1.Commands.CBTON, "CBTON 3_1 is not correct");
            Assert.AreEqual(false, resultConfig3_2.Commands.CBTON, "CBTON 3_2 is not correct");
            Assert.AreEqual(true, resultConfig3_3.Commands.CBTON, "CBTON 3_3 is not correct");
        }

        #endregion

        #region CBTBB

        /// <summary>
        /// Test CBTBB decoding with 1 sub 1 config.
        /// 
        /// CBTBB[0] 2, 1.023, 30.00
        /// </summary>
        [Test]
        public void TestCBTBB11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED, ssConfig.Commands.CBTBB_Mode, "CBTBB_Mode is incorrect.");
            Assert.AreEqual(1.023, ssConfig.Commands.CBTBB_PulseToPulseLag, 0.0001, "CBTBB_PulseToPulseLag is incorrect");
            Assert.AreEqual(30.00, ssConfig.Commands.CBTBB_LongRangeDepth, 0.0001, "CBTBB_LongRangeDepth is incorrect.");
        }

        /// <summary>
        /// Test CBTBB decoding with 1 sub multiple config.
        /// 
        /// CBTBB[0] 2, 4.440, 30.00 [1] 4, 2.020, 30.01 [2] 3, 1.000, 30.02
        /// </summary>
        [Test]
        public void TestCBTBB1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            // [0] 2, 4.440, 30.00
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED, resultConfig1.Commands.CBTBB_Mode, "CBTBB_Mode 1 is incorrect.");
            Assert.AreEqual(4.440, resultConfig1.Commands.CBTBB_PulseToPulseLag, 0.0001, "CBTBB_PulseToPulseLag 1 is incorrect");
            Assert.AreEqual(30.00, resultConfig1.Commands.CBTBB_LongRangeDepth, 0.0001, "CBTBB_LongRangeDepth 1 is incorrect.");

            // [1] 4, 2.020, 30.01
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED_P2P, resultConfig2.Commands.CBTBB_Mode, "CBTBB_Mode 2 is incorrect.");
            Assert.AreEqual(2.020, resultConfig2.Commands.CBTBB_PulseToPulseLag, 0.0001, "CBTBB_PulseToPulseLag 2 is incorrect");
            Assert.AreEqual(30.01, resultConfig2.Commands.CBTBB_LongRangeDepth, 0.0001, "CBTBB_LongRangeDepth 2 is incorrect.");

            // [2] 3, 1.000, 30.02
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NA_3, resultConfig3.Commands.CBTBB_Mode, "CBTBB_Mode 3 is incorrect.");
            Assert.AreEqual(1.000, resultConfig3.Commands.CBTBB_PulseToPulseLag, 0.0001, "CBTBB_PulseToPulseLag 3 is incorrect");
            Assert.AreEqual(30.02, resultConfig3.Commands.CBTBB_LongRangeDepth, 0.0001, "CBTBB_LongRangeDepth 3 is incorrect.");
        }

        /// <summary>
        /// Test CBTBB decoding with Multiple sub multiple config.
        /// 
        /// CBTBB[0] 0, 1.001, 30.00 [1] 1, 2.002, 30.02 [2] 3, 3.003, 50.03 [3] 4, 4.004, 30.04 [4] 5, 5.005, 50.05 [5] 6, 6.006, 50.06 [6] 7, 7.007, 30.07
        /// </summary>
        [Test]
        public void TestCBTBBMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 0, 1.001, 30.00
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE, resultConfig2_1.Commands.CBTBB_Mode, "CBTBB_Mode 2_1 is incorrect.");
            Assert.AreEqual(1.001, resultConfig2_1.Commands.CBTBB_PulseToPulseLag, 0.0001, "CBTBB_PulseToPulseLag 2_1 is incorrect");
            Assert.AreEqual(30.00, resultConfig2_1.Commands.CBTBB_LongRangeDepth, 0.0001, "CBTBB_LongRangeDepth 2_1 is incorrect.");

            // [1] 1, 2.002, 30.02
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED, resultConfig2_2.Commands.CBTBB_Mode, "CBTBB_Mode 2_2 is incorrect.");
            Assert.AreEqual(2.002, resultConfig2_2.Commands.CBTBB_PulseToPulseLag, 0.0001, "CBTBB_PulseToPulseLag 2_2 is incorrect");
            Assert.AreEqual(30.02, resultConfig2_2.Commands.CBTBB_LongRangeDepth, 0.0001, "CBTBB_LongRangeDepth 2_2 is incorrect.");

            // [3] 4, 4.004, 30.04
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED_P2P, resultConfig2_3.Commands.CBTBB_Mode, "CBTBB_Mode 2_3 is incorrect.");
            Assert.AreEqual(4.004, resultConfig2_3.Commands.CBTBB_PulseToPulseLag, 0.0001, "CBTBB_PulseToPulseLag 2_3 is incorrect");
            Assert.AreEqual(30.04, resultConfig2_3.Commands.CBTBB_LongRangeDepth, 0.0001, "CBTBB_LongRangeDepth 2_3 is incorrect.");

            // [6] 7, 7.007, 30.07
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.AUTO_SWITCH_NARROWBAND_BB_NONCODED_BB_NONCODED_P2P, resultConfig2_4.Commands.CBTBB_Mode, "CBTBB_Mode 2_4 is incorrect.");
            Assert.AreEqual(7.007, resultConfig2_4.Commands.CBTBB_PulseToPulseLag, 0.0001, "CBTBB_PulseToPulseLag 2_4 is incorrect");
            Assert.AreEqual(30.07, resultConfig2_4.Commands.CBTBB_LongRangeDepth, 0.0001, "CBTBB_LongRangeDepth 2_4 is incorrect.");



            // [2] 3, 3.003, 50.03
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NA_3, resultConfig3_1.Commands.CBTBB_Mode, "CBTBB_Mode 3_1 is incorrect.");
            Assert.AreEqual(3.003, resultConfig3_1.Commands.CBTBB_PulseToPulseLag, 0.0001, "CBTBB_PulseToPulseLag 3_1 is incorrect");
            Assert.AreEqual(50.03, resultConfig3_1.Commands.CBTBB_LongRangeDepth, 0.0001, "CBTBB_LongRangeDepth 3_1 is incorrect.");

            // [4] 5, 5.005, 50.05
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NA_5, resultConfig3_2.Commands.CBTBB_Mode, "CBTBB_Mode 3_2 is incorrect.");
            Assert.AreEqual(5.005, resultConfig3_2.Commands.CBTBB_PulseToPulseLag, 0.0001, "CBTBB_PulseToPulseLag 3_2 is incorrect");
            Assert.AreEqual(50.05, resultConfig3_2.Commands.CBTBB_LongRangeDepth, 0.0001, "CBTBB_LongRangeDepth 3_2 is incorrect.");

            // [5] 6, 6.006, 50.06
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.NA_6, resultConfig3_3.Commands.CBTBB_Mode, "CBTBB_Mode 3_3 is incorrect.");
            Assert.AreEqual(6.006, resultConfig3_3.Commands.CBTBB_PulseToPulseLag, 0.0001, "CBTBB_PulseToPulseLag 3_3 is incorrect");
            Assert.AreEqual(50.06, resultConfig3_3.Commands.CBTBB_LongRangeDepth, 0.0001, "CBTBB_LongRangeDepth 3_3 is incorrect.");

        }

        #endregion

        #region CBTST

        /// <summary>
        /// Test CBTST decoding with 1 sub 1 config.
        /// 
        /// CBTST[0] 0.900,1.002,1.001
        /// </summary>
        [Test]
        public void TestCBTST11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(0.900, ssConfig.Commands.CBTST_CorrelationThresh, 0.0001, "CBTST Correlation Threshold is incorrect");
            Assert.AreEqual(1.002, ssConfig.Commands.CBTST_QVelocityThresh, 0.0001, "CBTST Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.001, ssConfig.Commands.CBTST_VVelocityThresh, 0.0001, "CBTST V Velocity Threshold is incorrect");
        }

        /// <summary>
        /// Test CBTST decoding with 1 sub multiple config.
        /// 
        /// CBTST[0] 0.900,1.003,1.005 [1] 0.901,1.006,1.007 [2] 0.902,1.008,1.009
        /// </summary>
        [Test]
        public void TestCBTST1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            // [0] 0.900,1.003,1.005
            Assert.AreEqual(0.900, resultConfig1.Commands.CBTST_CorrelationThresh, 0.0001, "CBTST 1 Correlation Threshold is incorrect");
            Assert.AreEqual(1.003, resultConfig1.Commands.CBTST_QVelocityThresh, 0.0001, "CBTST 1 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.005, resultConfig1.Commands.CBTST_VVelocityThresh, 0.0001, "CBTST 1 V Velocity Threshold is incorrect");

            // [1] 0.901,1.006,1.007
            Assert.AreEqual(0.901, resultConfig2.Commands.CBTST_CorrelationThresh, 0.0001, "CBTST 2 Correlation Threshold is incorrect");
            Assert.AreEqual(1.006, resultConfig2.Commands.CBTST_QVelocityThresh, 0.0001, "CBTST 2 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.007, resultConfig2.Commands.CBTST_VVelocityThresh, 0.0001, "CBTST 2 V Velocity Threshold is incorrect");

            // [2] 0.902,1.008,1.009
            Assert.AreEqual(0.902, resultConfig3.Commands.CBTST_CorrelationThresh, 0.0001, "CBTST 3 Correlation Threshold is incorrect");
            Assert.AreEqual(1.008, resultConfig3.Commands.CBTST_QVelocityThresh, 0.0001, "CBTST 3 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.009, resultConfig3.Commands.CBTST_VVelocityThresh, 0.0001, "CBTST 3 V Velocity Threshold is incorrect");
        }

        /// <summary>
        /// Test CBTST decoding with Multiple sub multiple config.
        /// 
        /// CBTST[0] 0.901,1.002,1.003 [1] 0.904,1.005,1.006 [2] 0.907,1.008,1.009 [3] 0.910,1.011,1.012 [4] 0.913,1.015,1.016 [5] 0.917,1.018,1.019 [6] 0.920,1.021,1.022
        /// </summary>
        [Test]
        public void TestCBTSTMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 0.901,1.002,1.003
            Assert.AreEqual(0.901, resultConfig2_1.Commands.CBTST_CorrelationThresh, 0.0001, "CBTST 2_1 Correlation Threshold is incorrect");
            Assert.AreEqual(1.002, resultConfig2_1.Commands.CBTST_QVelocityThresh, 0.0001, "CBTST 2_1 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.003, resultConfig2_1.Commands.CBTST_VVelocityThresh, 0.0001, "CBTST 2_1 V Velocity Threshold is incorrect");
            // [1] 0.904,1.005,1.006
            Assert.AreEqual(0.904, resultConfig2_2.Commands.CBTST_CorrelationThresh, 0.0001, "CBTST 2_2 Correlation Threshold is incorrect");
            Assert.AreEqual(1.005, resultConfig2_2.Commands.CBTST_QVelocityThresh, 0.0001, "CBTST 2_2 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.006, resultConfig2_2.Commands.CBTST_VVelocityThresh, 0.0001, "CBTST 2_2 V Velocity Threshold is incorrect");
            // [3] 0.910,1.011,1.012
            Assert.AreEqual(0.910, resultConfig2_3.Commands.CBTST_CorrelationThresh, 0.0001, "CBTST 2_3 Correlation Threshold is incorrect");
            Assert.AreEqual(1.011, resultConfig2_3.Commands.CBTST_QVelocityThresh, 0.0001, "CBTST 2_3 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.012, resultConfig2_3.Commands.CBTST_VVelocityThresh, 0.0001, "CBTST 2_3 V Velocity Threshold is incorrect");
            // [6] 0.920,1.021,1.022
            Assert.AreEqual(0.920, resultConfig2_4.Commands.CBTST_CorrelationThresh, 0.0001, "CBTST 2_4 Correlation Threshold is incorrect");
            Assert.AreEqual(1.021, resultConfig2_4.Commands.CBTST_QVelocityThresh, 0.0001, "CBTST 2_4 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.022, resultConfig2_4.Commands.CBTST_VVelocityThresh, 0.0001, "CBTST 2_4 V Velocity Threshold is incorrect");


            // [2] 0.907,1.008,1.009
            Assert.AreEqual(0.907, resultConfig3_1.Commands.CBTST_CorrelationThresh, 0.0001, "CBTST 3_1 Correlation Threshold is incorrect");
            Assert.AreEqual(1.008, resultConfig3_1.Commands.CBTST_QVelocityThresh, 0.0001, "CBTST 3_1 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.009, resultConfig3_1.Commands.CBTST_VVelocityThresh, 0.0001, "CBTST 3_1 V Velocity Threshold is incorrect");
            // [4] 0.913,1.015,1.016
            Assert.AreEqual(0.913, resultConfig3_2.Commands.CBTST_CorrelationThresh, 0.0001, "CBTST 3_2 Correlation Threshold is incorrect");
            Assert.AreEqual(1.015, resultConfig3_2.Commands.CBTST_QVelocityThresh, 0.0001, "CBTST 3_2 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.016, resultConfig3_2.Commands.CBTST_VVelocityThresh, 0.0001, "CBTST 3_2 V Velocity Threshold is incorrect");
            // [5] 0.917,1.018,1.019
            Assert.AreEqual(0.917, resultConfig3_3.Commands.CBTST_CorrelationThresh, 0.0001, "CBTST 3_3 Correlation Threshold is incorrect");
            Assert.AreEqual(1.018, resultConfig3_3.Commands.CBTST_QVelocityThresh, 0.0001, "CBTST 3_3 Q Velocity Threshold is incorrect");
            Assert.AreEqual(1.019, resultConfig3_3.Commands.CBTST_VVelocityThresh, 0.0001, "CBTST 3_3 V Velocity Threshold is incorrect");

        }

        #endregion

        #region CBTBL

        /// <summary>
        /// Test CBTBL decoding with 1 sub 1 config.
        /// 
        /// CBTBL[0] 0.05
        /// </summary>
        [Test]
        public void TestCBTBL11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(0.05, ssConfig.Commands.CBTBL, 0.0001, "CBTBL is incorrect");
        }

        /// <summary>
        /// Test CBTBL decoding with 1 sub multiple config.
        /// 
        /// CBTBL[0] 0.05 [1] 0.06 [2] 0.07
        /// </summary>
        [Test]
        public void TestCBTBL1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(0.05, resultConfig1.Commands.CBTBL, 0.0001, "CBTBL 1 is incorrect");
            Assert.AreEqual(0.06, resultConfig2.Commands.CBTBL, 0.0001, "CBTBL 2 is incorrect");
            Assert.AreEqual(0.07, resultConfig3.Commands.CBTBL, 0.0001, "CBTBL 3 is incorrect");
        }

        /// <summary>
        /// Test CWPBL decoding with Multiple sub multiple config.
        /// 
        /// CBTBL[0] 0.05 [1] 0.07 [2] 0.18 [3] 0.09 [4] 0.11 [5] 0.12 [6] 0.013
        /// </summary>
        [Test]
        public void TestCBTBLMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 0.05
            Assert.AreEqual(0.05, resultConfig2_1.Commands.CBTBL, 0.0001, "CBTBL 2_1 is incorrect");
            // [1] 0.07
            Assert.AreEqual(0.07, resultConfig2_2.Commands.CBTBL, 0.0001, "CBTBL 2_2 is incorrect");
            // [3] 0.09
            Assert.AreEqual(0.09, resultConfig2_3.Commands.CBTBL, 0.0001, "CBTBL 2_3 is incorrect");
            // [6] 0.013
            Assert.AreEqual(0.013, resultConfig2_4.Commands.CBTBL, 0.0001, "CBTBL 2_4 is incorrect");


            // [2] 0.18
            Assert.AreEqual(0.18, resultConfig3_1.Commands.CBTBL, 0.0001, "CBTBL 3_1 is incorrect");
            // [4] 0.11
            Assert.AreEqual(0.11, resultConfig3_2.Commands.CBTBL, 0.0001, "CBTBL 3_2 is incorrect");
            // [5] 0.12
            Assert.AreEqual(0.12, resultConfig3_3.Commands.CBTBL, 0.0001, "CBTBL 3_3 is incorrect");

        }

        #endregion

        #region CBTMX

        /// <summary>
        /// Test CBTMX decoding with 1 sub 1 config.
        /// 
        /// CBTMX[0] 75.00
        /// </summary>
        [Test]
        public void TestCBTMX11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(75.00, ssConfig.Commands.CBTMX, 0.0001, "CBTMX is incorrect");
        }

        /// <summary>
        /// Test CBTMX decoding with 1 sub multiple config.
        /// 
        /// CBTMX[0] 75.00 [1] 75.01 [2] 75.02
        /// </summary>
        [Test]
        public void TestCBTMX1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(75.00, resultConfig1.Commands.CBTMX, 0.0001, "CBTMX 1 is incorrect");
            Assert.AreEqual(75.01, resultConfig2.Commands.CBTMX, 0.0001, "CBTMX 2 is incorrect");
            Assert.AreEqual(75.02, resultConfig3.Commands.CBTMX, 0.0001, "CBTMX 3 is incorrect");
        }

        /// <summary>
        /// Test CBTMX decoding with Multiple sub multiple config.
        /// 
        /// CBTMX[0] 75.00 [1] 75.01 [2] 125.02 [3] 75.03 [4] 125.04 [5] 125.05 [6] 75.06
        /// </summary>
        [Test]
        public void TestCBTMXMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 75.00
            Assert.AreEqual(75.00, resultConfig2_1.Commands.CBTMX, 0.0001, "CBTMX 2_1 is incorrect");
            // [1] 75.01
            Assert.AreEqual(75.01, resultConfig2_2.Commands.CBTMX, 0.0001, "CBTMX 2_2 is incorrect");
            // [3] 75.03
            Assert.AreEqual(75.03, resultConfig2_3.Commands.CBTMX, 0.0001, "CBTMX 2_3 is incorrect");
            // [6] 75.06
            Assert.AreEqual(75.06, resultConfig2_4.Commands.CBTMX, 0.0001, "CBTMX 2_4 is incorrect");


            // [2] 125.02
            Assert.AreEqual(125.02, resultConfig3_1.Commands.CBTMX, 0.0001, "CBTMX 3_1 is incorrect");
            // [4] 125.04
            Assert.AreEqual(125.04, resultConfig3_2.Commands.CBTMX, 0.0001, "CBTMX 3_2 is incorrect");
            // [5] 125.05
            Assert.AreEqual(125.05, resultConfig3_3.Commands.CBTMX, 0.0001, "CBTMX 3_3 is incorrect");

        }

        #endregion

        #region CBTTBP

        /// <summary>
        /// Test CBTTBP decoding with 1 sub 1 config.
        /// 
        /// CBTTBP[0] 0.05
        /// </summary>
        [Test]
        public void TestCBTTBP11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(0.05, ssConfig.Commands.CBTTBP, 0.0001, "CBTTBP is incorrect");
        }

        /// <summary>
        /// Test CBTTBP decoding with 1 sub multiple config.
        /// 
        /// CBTTBP[0] 0.05 [1] 0.06 [2] 0.07 
        /// </summary>
        [Test]
        public void TestCBTTBP1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(0.05, resultConfig1.Commands.CBTTBP, 0.0001, "CBTTBP 1 is incorrect");
            Assert.AreEqual(0.06, resultConfig2.Commands.CBTTBP, 0.0001, "CBTTBP 2 is incorrect");
            Assert.AreEqual(0.07, resultConfig3.Commands.CBTTBP, 0.0001, "CBTTBP 3 is incorrect");
        }

        /// <summary>
        /// Test CBTTBP decoding with Multiple sub multiple config.
        /// 
        /// CBTTBP[0] 0.05 [1] 0.06 [2] 0.17 [3] 0.08 [4] 0.19 [5] 0.20 [6] 0.21
        /// </summary>
        [Test]
        public void TestCBTTBPMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 0.05
            Assert.AreEqual(0.05, resultConfig2_1.Commands.CBTTBP, 0.0001, "CBTTBP 2_1 is incorrect");
            // [1] 0.06
            Assert.AreEqual(0.06, resultConfig2_2.Commands.CBTTBP, 0.0001, "CBTTBP 2_2 is incorrect");
            // [3] 0.08
            Assert.AreEqual(0.08, resultConfig2_3.Commands.CBTTBP, 0.0001, "CBTTBP 2_3 is incorrect");
            // [6] 0.21
            Assert.AreEqual(0.21, resultConfig2_4.Commands.CBTTBP, 0.0001, "CBTTBP 2_4 is incorrect");


            // [2] 0.17 
            Assert.AreEqual(0.17, resultConfig3_1.Commands.CBTTBP, 0.0001, "CBTTBP 3_1 is incorrect");
            // [4] 0.19
            Assert.AreEqual(0.19, resultConfig3_2.Commands.CBTTBP, 0.0001, "CBTTBP 3_2 is incorrect");
            // [5] 0.20
            Assert.AreEqual(0.20, resultConfig3_3.Commands.CBTTBP, 0.0001, "CBTTBP 3_3 is incorrect");

        }

        #endregion

        #region CBTT

        /// <summary>
        /// Test CBTT decoding with 1 sub 1 config.
        /// 
        /// CBTT[0] 15.0,25.0,5.0,2.0
        /// </summary>
        [Test]
        public void TestCBTT11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(15.0, ssConfig.Commands.CBTT_SNRShallowDetectionThresh, 0.0001, "CBTT_SNRShallowDetectionThresh is incorrect");
            Assert.AreEqual(25.0, ssConfig.Commands.CBTT_DepthSNR, 0.0001, "CBTT_DepthSNR is incorrect");
            Assert.AreEqual(5.0, ssConfig.Commands.CBTT_SNRDeepDetectionThresh, 0.0001, "CBTT_SNRDeepDetectionThresh is incorrect");
            Assert.AreEqual(2.0, ssConfig.Commands.CBTT_DepthGain, 0.0001, "CBTT_DepthGain is incorrect");
        }

        /// <summary>
        /// Test CBTT decoding with 1 sub multiple config.
        /// 
        /// CBTT[0] 15.0,25.0,5.0,2.0 [1] 15.1,25.1,5.1,2.1 [2] 15.2,25.2,5.2,2.2
        /// </summary>
        [Test]
        public void TestCBTT1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(15.0, resultConfig1.Commands.CBTT_SNRShallowDetectionThresh, 0.0001, "CBTT_SNRShallowDetectionThresh 1 is incorrect");
            Assert.AreEqual(25.0, resultConfig1.Commands.CBTT_DepthSNR, 0.0001, "CBTT_DepthSNR 1 is incorrect");
            Assert.AreEqual(5.0, resultConfig1.Commands.CBTT_SNRDeepDetectionThresh, 0.0001, "CBTT_SNRDeepDetectionThresh 1 is incorrect");
            Assert.AreEqual(2.0, resultConfig1.Commands.CBTT_DepthGain, 0.0001, "CBTT_DepthGain 1 is incorrect");

            Assert.AreEqual(15.1, resultConfig2.Commands.CBTT_SNRShallowDetectionThresh, 0.0001, "CBTT_SNRShallowDetectionThresh 2 is incorrect");
            Assert.AreEqual(25.1, resultConfig2.Commands.CBTT_DepthSNR, 0.0001, "CBTT_DepthSNR 2 is incorrect");
            Assert.AreEqual(5.1, resultConfig2.Commands.CBTT_SNRDeepDetectionThresh, 0.0001, "CBTT_SNRDeepDetectionThresh 2 is incorrect");
            Assert.AreEqual(2.1, resultConfig2.Commands.CBTT_DepthGain, 0.0001, "CBTT_DepthGai 2 is incorrect");

            Assert.AreEqual(15.2, resultConfig3.Commands.CBTT_SNRShallowDetectionThresh, 0.0001, "CBTT_SNRShallowDetectionThresh 3 is incorrect");
            Assert.AreEqual(25.2, resultConfig3.Commands.CBTT_DepthSNR, 0.0001, "CBTT_DepthSNR 3 is incorrect");
            Assert.AreEqual(5.2, resultConfig3.Commands.CBTT_SNRDeepDetectionThresh, 0.0001, "CBTT_SNRDeepDetectionThresh 3 is incorrect");
            Assert.AreEqual(2.2, resultConfig3.Commands.CBTT_DepthGain, 0.0001, "CBTT_DepthGain 3 is incorrect");
        }

        /// <summary>
        /// Test CBTT decoding with Multiple sub multiple config.
        /// 
        /// CBTT[0] 15.0,25.0,5.0,2.0 [1] 15.1,25.1,5.1,2.1 [2] 15.2,50.2,5.2,4.2 [3] 15.3,25.3,5.3,2.3 [4] 15.4,50.4,5.4,4.4 [5] 15.5,50.5,5.5,4.5 [6] 15.6,25.6,5.6,2.6
        /// </summary>
        [Test]
        public void TestCBTTMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 15.0,25.0,5.0,2.0
            Assert.AreEqual(15.0, resultConfig2_1.Commands.CBTT_SNRShallowDetectionThresh, 0.0001, "CBTT_SNRShallowDetectionThresh 2_1 is incorrect");
            Assert.AreEqual(25.0, resultConfig2_1.Commands.CBTT_DepthSNR, 0.0001, "CBTT_DepthSNR 2_1 is incorrect");
            Assert.AreEqual(5.0, resultConfig2_1.Commands.CBTT_SNRDeepDetectionThresh, 0.0001, "CBTT_SNRDeepDetectionThresh 2_1 is incorrect");
            Assert.AreEqual(2.0, resultConfig2_1.Commands.CBTT_DepthGain, 0.0001, "CBTT_DepthGain 2_1 is incorrect");
            // [1] 15.1,25.1,5.1,2.1
            Assert.AreEqual(15.1, resultConfig2_2.Commands.CBTT_SNRShallowDetectionThresh, 0.0001, "CBTT_SNRShallowDetectionThresh 2_2 is incorrect");
            Assert.AreEqual(25.1, resultConfig2_2.Commands.CBTT_DepthSNR, 0.0001, "CBTT_DepthSNR 2_2 is incorrect");
            Assert.AreEqual(5.1, resultConfig2_2.Commands.CBTT_SNRDeepDetectionThresh, 0.0001, "CBTT_SNRDeepDetectionThresh 2_2 is incorrect");
            Assert.AreEqual(2.1, resultConfig2_2.Commands.CBTT_DepthGain, 0.0001, "CBTT_DepthGai 2_2 is incorrect");
            // [3] 15.3,25.3,5.3,2.3
            Assert.AreEqual(15.3, resultConfig2_3.Commands.CBTT_SNRShallowDetectionThresh, 0.0001, "CBTT_SNRShallowDetectionThresh 2_3 is incorrect");
            Assert.AreEqual(25.3, resultConfig2_3.Commands.CBTT_DepthSNR, 0.0001, "CBTT_DepthSNR 2_3 is incorrect");
            Assert.AreEqual(5.3, resultConfig2_3.Commands.CBTT_SNRDeepDetectionThresh, 0.0001, "CBTT_SNRDeepDetectionThresh 2_3 is incorrect");
            Assert.AreEqual(2.3, resultConfig2_3.Commands.CBTT_DepthGain, 0.0001, "CBTT_DepthGain 2_3 is incorrect");
            // [6] 15.6,25.6,5.6,2.6
            Assert.AreEqual(15.6, resultConfig2_4.Commands.CBTT_SNRShallowDetectionThresh, 0.0001, "CBTT_SNRShallowDetectionThresh 2_4 is incorrect");
            Assert.AreEqual(25.6, resultConfig2_4.Commands.CBTT_DepthSNR, 0.0001, "CBTT_DepthSNR 2_4 is incorrect");
            Assert.AreEqual(5.6, resultConfig2_4.Commands.CBTT_SNRDeepDetectionThresh, 0.0001, "CBTT_SNRDeepDetectionThresh 2_4 is incorrect");
            Assert.AreEqual(2.6, resultConfig2_4.Commands.CBTT_DepthGain, 0.0001, "CBTT_DepthGain 2_4 is incorrect");



            // [2] 15.2,50.2,5.2,4.2
            Assert.AreEqual(15.2, resultConfig3_1.Commands.CBTT_SNRShallowDetectionThresh, 0.0001, "CBTT_SNRShallowDetectionThresh 3_1 is incorrect");
            Assert.AreEqual(50.2, resultConfig3_1.Commands.CBTT_DepthSNR, 0.0001, "CBTT_DepthSNR 3_1 is incorrect");
            Assert.AreEqual(5.2, resultConfig3_1.Commands.CBTT_SNRDeepDetectionThresh, 0.0001, "CBTT_SNRDeepDetectionThresh 3_1 is incorrect");
            Assert.AreEqual(4.2, resultConfig3_1.Commands.CBTT_DepthGain, 0.0001, "CBTT_DepthGain 3_1 is incorrect");
            // [4] 15.4,50.4,5.4,4.4
            Assert.AreEqual(15.4, resultConfig3_2.Commands.CBTT_SNRShallowDetectionThresh, 0.0001, "CBTT_SNRShallowDetectionThresh 3_2 is incorrect");
            Assert.AreEqual(50.4, resultConfig3_2.Commands.CBTT_DepthSNR, 0.0001, "CBTT_DepthSNR 3_2 is incorrect");
            Assert.AreEqual(5.4, resultConfig3_2.Commands.CBTT_SNRDeepDetectionThresh, 0.0001, "CBTT_SNRDeepDetectionThresh 3_2 is incorrect");
            Assert.AreEqual(4.4, resultConfig3_2.Commands.CBTT_DepthGain, 0.0001, "CBTT_DepthGai 3_2 is incorrect");
            // [5] 15.5,50.5,5.5,4.5
            Assert.AreEqual(15.5, resultConfig3_3.Commands.CBTT_SNRShallowDetectionThresh, 0.0001, "CBTT_SNRShallowDetectionThresh 3_3 is incorrect");
            Assert.AreEqual(50.5, resultConfig3_3.Commands.CBTT_DepthSNR, 0.0001, "CBTT_DepthSNR 3_3 is incorrect");
            Assert.AreEqual(5.5, resultConfig3_3.Commands.CBTT_SNRDeepDetectionThresh, 0.0001, "CBTT_SNRDeepDetectionThresh 3_3 is incorrect");
            Assert.AreEqual(4.5, resultConfig3_3.Commands.CBTT_DepthGain, 0.0001, "CBTT_DepthGain 3_3 is incorrect");

        }

        #endregion

        #endregion

        #region WT

        #region CWTON

        /// <summary>
        /// Test CWTON decoding with 1 sub 1 config.
        /// </summary>
        [Test]
        public void TestCWTON11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(false, ssConfig.Commands.CWTON, "CWTON is not correct");
        }

        /// <summary>
        /// Test CWTON decoding with 1 sub multiple config.
        /// 
        /// CWTON[0] 1 [1] 0 [2] 1 \r\n");
        /// </summary>
        [Test]
        public void TestCWTON1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(true, resultConfig1.Commands.CWTON, "CWTON 1 is not correct");
            Assert.AreEqual(false, resultConfig2.Commands.CWTON, "CWTON 2 is not correct");
            Assert.AreEqual(true, resultConfig3.Commands.CWTON, "CWTON 3 is not correct");
        }

        /// <summary>
        /// Test CWTON decoding with Multiple sub multiple config.
        /// 
        /// CWTON[0] 1 [1] 0 [2] 1 [3] 0 [4] 0 [5] 1 [6] 0
        /// </summary>
        [Test]
        public void TestCWTONMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            Assert.AreEqual(true, resultConfig2_1.Commands.CWTON, "CWTON 2_1 is not correct");
            Assert.AreEqual(false, resultConfig2_2.Commands.CWTON, "CWTON 2_2 is not correct");
            Assert.AreEqual(false, resultConfig2_3.Commands.CWTON, "CWTON 2_3 is not correct");
            Assert.AreEqual(false, resultConfig2_4.Commands.CWTON, "CWTON 2_4 is not correct");

            Assert.AreEqual(true, resultConfig3_1.Commands.CWTON, "CWTON 3_1 is not correct");
            Assert.AreEqual(false, resultConfig3_2.Commands.CWTON, "CWTON 3_2 is not correct");
            Assert.AreEqual(true, resultConfig3_3.Commands.CWTON, "CWTON 3_3 is not correct");
        }

        #endregion

        #region CWTBB

        /// <summary>
        /// Test CWTBB decoding with 1 sub 1 config.
        /// </summary>
        [Test]
        public void TestCWTBB11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(false, ssConfig.Commands.CWTBB, "CWTON is not correct");
        }

        /// <summary>
        /// Test CWTBB decoding with 1 sub multiple config.
        /// 
        /// CWTBB[0] 0 [1] 1 [2] 0 \r\n");
        /// </summary>
        [Test]
        public void TestCWTBB1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(false, resultConfig1.Commands.CWTBB, "CWTBB 1 is not correct");
            Assert.AreEqual(true, resultConfig2.Commands.CWTBB, "CWTBB 2 is not correct");
            Assert.AreEqual(false, resultConfig3.Commands.CWTBB, "CWTBB 3 is not correct");
        }

        /// <summary>
        /// Test CWTBB decoding with Multiple sub multiple config.
        /// 
        /// CWTBB[0] 1 [1] 0 [2] 1 [3] 0 [4] 1 [5] 0 [6] 1 
        /// </summary>
        [Test]
        public void TestCWTBBMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            Assert.AreEqual(true, resultConfig2_1.Commands.CWTBB, "CWTBB 2_1 is not correct");
            Assert.AreEqual(false, resultConfig2_2.Commands.CWTBB, "CWTBB 2_2 is not correct");
            Assert.AreEqual(false, resultConfig2_3.Commands.CWTBB, "CWTBB 2_3 is not correct");
            Assert.AreEqual(true, resultConfig2_4.Commands.CWTBB, "CWTBB 2_4 is not correct");

            Assert.AreEqual(true, resultConfig3_1.Commands.CWTBB, "CWTBB 3_1 is not correct");
            Assert.AreEqual(true, resultConfig3_2.Commands.CWTBB, "CWTBB 3_2 is not correct");
            Assert.AreEqual(false, resultConfig3_3.Commands.CWTBB, "CWTBB 3_3 is not correct");
        }

        #endregion

        #region CWTBL

        /// <summary>
        /// Test CWTBL decoding with 1 sub 1 config.
        /// 
        /// CWTBL[0] 2.00
        /// </summary>
        [Test]
        public void TestCWTBL11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(2.00, ssConfig.Commands.CWTBL, 0.0001, "CWTBL is incorrect");
        }

        /// <summary>
        /// Test CWTBL decoding with 1 sub multiple config.
        /// 
        /// CWTBL[0] 2.00 [1] 2.01 [2] 2.02
        /// </summary>
        [Test]
        public void TestCWTBL1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(2.00, resultConfig1.Commands.CWTBL, 0.0001, "CWTBL 1 is incorrect");
            Assert.AreEqual(2.01, resultConfig2.Commands.CWTBL, 0.0001, "CWTBL 2 is incorrect");
            Assert.AreEqual(2.02, resultConfig3.Commands.CWTBL, 0.0001, "CWTBL 3 is incorrect");
        }

        /// <summary>
        /// Test CWTBL decoding with Multiple sub multiple config.
        /// 
        /// CWTBL[0] 2.07 [1] 2.06 [2] 2.05 [3] 2.04 [4] 2.03 [5] 2.02 [6] 2.01
        /// </summary>
        [Test]
        public void TestCWTBLMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 2.07
            Assert.AreEqual(2.07, resultConfig2_1.Commands.CWTBL, 0.0001, "CWTBL 2_1 is incorrect");
            // [1] 2.06
            Assert.AreEqual(2.06, resultConfig2_2.Commands.CWTBL, 0.0001, "CWTBL 2_2 is incorrect");
            // [3] 2.04
            Assert.AreEqual(2.04, resultConfig2_3.Commands.CWTBL, 0.0001, "CWTBL 2_3 is incorrect");
            // [6] 2.01
            Assert.AreEqual(2.01, resultConfig2_4.Commands.CWTBL, 0.0001, "CWTBL 2_4 is incorrect");


            // [2] 2.05
            Assert.AreEqual(2.05, resultConfig3_1.Commands.CWTBL, 0.0001, "CWTBL 3_1 is incorrect");
            // [4] 2.03
            Assert.AreEqual(2.03, resultConfig3_2.Commands.CWTBL, 0.0001, "CWTBL 3_2 is incorrect");
            // [5] 2.02
            Assert.AreEqual(2.02, resultConfig3_3.Commands.CWTBL, 0.0001, "CWTBL 3_3 is incorrect");

        }

        #endregion

        #region CWTBS

        /// <summary>
        /// Test CWTBS decoding with 1 sub 1 config.
        /// 
        /// CWTBS[0] 2.01
        /// </summary>
        [Test]
        public void TestCWTBS11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(2.01, ssConfig.Commands.CWTBS, 0.0001, "CWTBL is incorrect");
        }

        /// <summary>
        /// Test CWTBS decoding with 1 sub multiple config.
        /// 
        /// CWTBS[0] 2.03 [1] 2.04 [2] 2.05
        /// </summary>
        [Test]
        public void TestCWTBS1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(2.03, resultConfig1.Commands.CWTBS, 0.0001, "CWTBS 1 is incorrect");
            Assert.AreEqual(2.04, resultConfig2.Commands.CWTBS, 0.0001, "CWTBS 2 is incorrect");
            Assert.AreEqual(2.05, resultConfig3.Commands.CWTBS, 0.0001, "CWTBS 3 is incorrect");
        }

        /// <summary>
        /// Test CWTBS decoding with Multiple sub multiple config.
        /// 
        /// CWTBS[0] 2.08 [1] 2.09 [2] 2.10 [3] 2.11 [4] 2.12 [5] 2.13 [6] 2.14
        /// </summary>
        [Test]
        public void TestCWTBSMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 2.08
            Assert.AreEqual(2.08, resultConfig2_1.Commands.CWTBS, 0.0001, "CWTBS 2_1 is incorrect");
            // [1] 2.09
            Assert.AreEqual(2.09, resultConfig2_2.Commands.CWTBS, 0.0001, "CWTBS 2_2 is incorrect");
            // [3] 2.11
            Assert.AreEqual(2.11, resultConfig2_3.Commands.CWTBS, 0.0001, "CWTBS 2_3 is incorrect");
            // [6] 2.14
            Assert.AreEqual(2.14, resultConfig2_4.Commands.CWTBS, 0.0001, "CWTBS 2_4 is incorrect");


            // [2] 2.10
            Assert.AreEqual(2.10, resultConfig3_1.Commands.CWTBS, 0.0001, "CWTBS 3_1 is incorrect");
            // [4] 2.12
            Assert.AreEqual(2.12, resultConfig3_2.Commands.CWTBS, 0.0001, "CWTBS 3_2 is incorrect");
            // [5] 2.13
            Assert.AreEqual(2.13, resultConfig3_3.Commands.CWTBS, 0.0001, "CWTBS 3_3 is incorrect");

        }

        #endregion

        #region CWTTBP

        /// <summary>
        /// Test CWTTBP decoding with 1 sub 1 config.
        /// 
        /// CWTTBP[0] 0.13
        /// </summary>
        [Test]
        public void TestCWTTBP11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig ssc in config.SubsystemConfigDict.Values)
            {
                ssConfig = ssc;
            }
            #endregion

            Assert.AreEqual(0.13, ssConfig.Commands.CWTTBP, 0.0001, "CWTTBP is incorrect");
        }

        /// <summary>
        /// Test CWTTBP decoding with 1 sub multiple config.
        /// 
        /// CWTTBP[0] 0.13 [1] 0.14 [2] 0.15
        /// </summary>
        [Test]
        public void TestCWTTBP1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                if (resultConfig1 == null)
                {
                    resultConfig1 = asConfig;
                }
                else
                {
                    if (resultConfig2 == null)
                    {
                        resultConfig2 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3 == null)
                        {
                            resultConfig3 = asConfig;
                        }
                    }
                }
            }
            #endregion

            Assert.AreEqual(0.13, resultConfig1.Commands.CWTTBP, 0.0001, "CWTTBP 1 is incorrect");
            Assert.AreEqual(0.14, resultConfig2.Commands.CWTTBP, 0.0001, "CWTTBP 2 is incorrect");
            Assert.AreEqual(0.15, resultConfig3.Commands.CWTTBP, 0.0001, "CWTTBP 3 is incorrect");
        }

        /// <summary>
        /// Test CWTTBP decoding with Multiple sub multiple config.
        /// 
        /// CWTTBP[0] 0.132 [1] 0.131[2] 0.29 [3] 0.18 [4] 0.27 [5] 0.26 [6] 0.15 
        /// </summary>
        [Test]
        public void TestCWTTBPMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            #region Get AdcpSubsystemConfig
            // Get Subsystem Configuration
            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            foreach (AdcpSubsystemConfig asConfig in config.SubsystemConfigDict.Values)
            {
                // Subsystem 2
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    if (resultConfig2_1 == null)
                    {
                        resultConfig2_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig2_2 == null)
                        {
                            resultConfig2_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig2_3 == null)
                            {
                                resultConfig2_3 = asConfig;
                            }
                            else
                            {
                                if (resultConfig2_4 == null)
                                {
                                    resultConfig2_4 = asConfig;
                                }
                            }
                        }
                    }
                }

                // Subsystem 3
                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    if (resultConfig3_1 == null)
                    {
                        resultConfig3_1 = asConfig;
                    }
                    else
                    {
                        if (resultConfig3_2 == null)
                        {
                            resultConfig3_2 = asConfig;
                        }
                        else
                        {
                            if (resultConfig3_3 == null)
                            {
                                resultConfig3_3 = asConfig;
                            }
                        }
                    }
                }

            }
            #endregion

            // [0] 0.132
            Assert.AreEqual(0.132, resultConfig2_1.Commands.CWTTBP, 0.0001, "CWTTBP 2_1 is incorrect");
            // [1] 0.131
            Assert.AreEqual(0.131, resultConfig2_2.Commands.CWTTBP, 0.0001, "CWTTBP 2_2 is incorrect");
            // [3] 0.18
            Assert.AreEqual(0.18, resultConfig2_3.Commands.CWTTBP, 0.0001, "CWTTBP 2_3 is incorrect");
            // [6] 0.15 
            Assert.AreEqual(0.15, resultConfig2_4.Commands.CWTTBP, 0.0001, "CWTTBP 2_4 is incorrect");


            // [2] 0.29
            Assert.AreEqual(0.29, resultConfig3_1.Commands.CWTTBP, 0.0001, "CWTTBP 3_1 is incorrect");
            // [4] 0.27
            Assert.AreEqual(0.27, resultConfig3_2.Commands.CWTTBP, 0.0001, "CWTTBP 3_2 is incorrect");
            // [5] 0.26
            Assert.AreEqual(0.26, resultConfig3_3.Commands.CWTTBP, 0.0001, "CWTTBP 3_3 is incorrect");

        }

        #endregion

        #endregion

        #region Environmental

        #region CWS

        /// <summary>
        /// Test CWS decoding with 1 sub 1 config.
        /// 
        /// Ex:
        /// CWS 17.00
        /// </summary>
        [Test]
        public void TestCWS11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(17.00f, config.Commands.CWS, 0.0001, "CWS is incorrect");
        }

        /// <summary>
        /// Test CWS decoding with 1 sub multiple config.
        /// 
        /// Ex:
        /// CWS 64.20
        /// </summary>
        [Test]
        public void TestCWS1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(64.20f, config.Commands.CWS, 0.0001, "CWS is incorrect");
        }

        /// <summary>
        /// Test CWS decoding with Multiple sub multiple config.
        /// 
        /// Ex:
        /// CWS 69.69
        /// </summary>
        [Test]
        public void TestCWSMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(69.69, config.Commands.CWS, 0.0001, "CWS is incorrect");
        }

        #endregion

        #region CWT

        /// <summary>
        /// Test CWT decoding with 1 sub 1 config.
        /// 
        /// Ex:
        /// CWT 15.00
        /// </summary>
        [Test]
        public void TestCWT11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(15.00f, config.Commands.CWT, 0.0001, "CWT is incorrect");
        }

        /// <summary>
        /// Test CWT decoding with 1 sub multiple config.
        /// 
        /// Ex:
        /// CWT 15.01
        /// </summary>
        [Test]
        public void TestCWT1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(15.01f, config.Commands.CWT, 0.0001, "CWT is incorrect");
        }

        /// <summary>
        /// Test CWT decoding with Multiple sub multiple config.
        /// 
        /// Ex:
        /// CWT 15.033
        /// </summary>
        [Test]
        public void TestCWTMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(15.033f, config.Commands.CWT, 0.0001, "CWT is incorrect");
        }

        #endregion

        #region CTD

        /// <summary>
        /// Test CTD decoding with 1 sub 1 config.
        /// 
        /// Ex:
        /// CTD 22.10
        /// </summary>
        [Test]
        public void TestCTD11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(22.10f, config.Commands.CTD, 0.0001, "CTD is incorrect");
        }

        /// <summary>
        /// Test CTD decoding with 1 sub multiple config.
        /// 
        /// Ex:
        /// CTD 13.03
        /// </summary>
        [Test]
        public void TestCTD1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(13.03f, config.Commands.CTD, 0.0001, "CTD is incorrect");
        }

        /// <summary>
        /// Test CTD decoding with Multiple sub multiple config.
        /// 
        /// Ex:
        /// CTD 13.13
        /// </summary>
        [Test]
        public void TestCTDMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(13.13f, config.Commands.CTD, 0.0001, "CTD is incorrect");
        }

        #endregion

        #region CWSS

        /// <summary>
        /// Test CWSS decoding with 1 sub 1 config.
        /// 
        /// Ex:
        /// CWSS 1500.01
        /// </summary>
        [Test]
        public void TestCWSS11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(1500.01, config.Commands.CWSS, 0.0001, "CWSS is incorrect");
        }

        /// <summary>
        /// Test CWSS decoding with 1 sub multiple config.
        /// 
        /// Ex:
        /// CWSS 1500.06
        /// </summary>
        [Test]
        public void TestCWSS1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(1500.06, config.Commands.CWSS, 0.0001, "CWSS is incorrect");
        }

        /// <summary>
        /// Test CWSS decoding with Multiple sub multiple config.
        /// 
        /// Ex:
        /// CWSS 1500.30
        /// </summary>
        [Test]
        public void TestCWSSMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(1500.30f, config.Commands.CWSS, 0.0001, "CWSS is incorrect");
        }

        #endregion

        #region CHO

        /// <summary>
        /// Test CHO decoding with 1 sub 1 config.
        /// 
        /// Ex:
        /// CHO 12.654
        /// </summary>
        [Test]
        public void TestCHO11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(12.654f, config.Commands.CHO, 0.0001, "CHO is incorrect");
        }

        /// <summary>
        /// Test CHO decoding with 1 sub multiple config.
        /// 
        /// Ex:
        /// CHO 125.36
        /// </summary>
        [Test]
        public void TestCHO1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(125.36f, config.Commands.CHO, 0.0001, "CHO is incorrect");
        }

        /// <summary>
        /// Test CHO decoding with Multiple sub multiple config.
        /// 
        /// Ex:
        /// CHO 14.14
        /// </summary>
        [Test]
        public void TestCHOMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(14.14, config.Commands.CHO, 0.0001, "CHO is incorrect");
        }

        #endregion

        #region CHS

        /// <summary>
        /// Test CHS decoding with 1 sub 1 config.
        /// 
        /// Ex:
        /// CHS 1
        /// </summary>
        [Test]
        public void TestCHS11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(HeadingSrc.INTERNAL, config.Commands.CHS, "CHS is incorrect");
        }

        /// <summary>
        /// Test CHS decoding with 1 sub multiple config.
        /// 
        /// Ex:
        /// CHS 2
        /// </summary>
        [Test]
        public void TestCHS1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(HeadingSrc.SERIAL, config.Commands.CHS, "CHS is incorrect");
        }

        /// <summary>
        /// Test CHS decoding with Multiple sub multiple config.
        /// 
        /// Ex:
        /// CHS 1
        /// </summary>
        [Test]
        public void TestCHSMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(HeadingSrc.INTERNAL, config.Commands.CHS, "CHS is incorrect");
        }

        #endregion

        #region CVSF

        /// <summary>
        /// Test CVSF decoding with 1 sub 1 config.
        /// 
        /// Ex:
        /// CVSF 1.023
        /// </summary>
        [Test]
        public void TestCVSF11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(1.023f, config.Commands.CVSF, 0.0001, "CVSF is incorrect");
        }

        /// <summary>
        /// Test CVSF decoding with 1 sub multiple config.
        /// 
        /// Ex:
        /// CVSF 1.040
        /// </summary>
        [Test]
        public void TestCVSF1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(1.040f, config.Commands.CVSF, 0.0001, "CVSF is incorrect");
        }

        /// <summary>
        /// Test CVSF decoding with Multiple sub multiple config.
        /// 
        /// Ex:
        /// CVSF 1.002
        /// </summary>
        [Test]
        public void TestCVSFMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(1.002, config.Commands.CVSF, 0.0001, "CVSF is incorrect");
        }

        #endregion

        #region C232B

        /// <summary>
        /// Test C232B decoding with 1 sub 1 config.
        /// 
        /// Ex:
        /// C232B 115200
        /// </summary>
        [Test]
        public void TestC232B11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(Baudrate.BAUD_115200, config.Commands.C232B, "C232B is incorrect");
        }

        /// <summary>
        /// Test C232B decoding with 1 sub multiple config.
        /// 
        /// Ex:
        /// C232B 4800
        /// </summary>
        [Test]
        public void TestC232B1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(Baudrate.BAUD_4800, config.Commands.C232B, "C232B is incorrect");
        }

        /// <summary>
        /// Test C232B decoding with Multiple sub multiple config.
        /// 
        /// Ex:
        /// C232B 921600
        /// </summary>
        [Test]
        public void TestC232BMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(Baudrate.BAUD_921600, config.Commands.C232B, "C232B is incorrect");
        }

        #endregion

        #region C485B

        /// <summary>
        /// Test C485B decoding with 1 sub 1 config.
        /// 
        /// Ex:
        /// C485B 460800
        /// </summary>
        [Test]
        public void TestC485B11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(Baudrate.BAUD_460800, config.Commands.C485B, "C485B is incorrect");
        }

        /// <summary>
        /// Test C485B decoding with 1 sub multiple config.
        /// 
        /// Ex:
        /// C485B 9600
        /// </summary>
        [Test]
        public void TestC485B1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(Baudrate.BAUD_9600, config.Commands.C485B, "C485B is incorrect");
        }

        /// <summary>
        /// Test C485B decoding with Multiple sub multiple config.
        /// 
        /// Ex:
        /// C485B 2400
        /// </summary>
        [Test]
        public void TestC485BMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(Baudrate.BAUD_2400, config.Commands.C485B, "C485B is incorrect");
        }

        #endregion

        #region C422B

        /// <summary>
        /// Test C422B decoding with 1 sub 1 config.
        /// 
        /// Ex:
        /// C422B 19200
        /// </summary>
        [Test]
        public void TestC422B11()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_singleSubsystemConfiguration, _singleSubsystemSerialNumber);

            Assert.AreEqual(Baudrate.BAUD_19200, config.Commands.C422B, "C422B is incorrect");
        }

        /// <summary>
        /// Test C422B decoding with 1 sub multiple config.
        /// 
        /// Ex:
        /// C422B 230400
        /// </summary>
        [Test]
        public void TestC422B1M()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemConfigurations, _singleSubsystemSerialNumber);

            Assert.AreEqual(Baudrate.BAUD_230400, config.Commands.C422B, "C422B is incorrect");
        }

        /// <summary>
        /// Test C422B decoding with Multiple sub multiple config.
        /// 
        /// Ex:
        /// C422B 38400
        /// </summary>
        [Test]
        public void TestC422BMM()
        {
            DecodeCSHOW d = new DecodeCSHOW();
            AdcpConfiguration config = d.Decode(_multipleSubsystemMultipleConfigurations, _multipleSubsystemSerialNumber);

            Assert.AreEqual(Baudrate.BAUD_38400, config.Commands.C422B, "C422B is incorrect");
        }

        #endregion

        #endregion

    }
}
