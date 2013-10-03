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
 * 09/21/2012      RC          2.15       Initial coding
 * 10/01/2102      RC          2.15       Added Test for DeploymentOptions.
 * 10/02/2012      RC          2.15       Added test for AdcpSubsystemConfigExist() and GetAdcpSubsystemConfig().
 *                                         Added test for AddConfiguration().
 * 10/03/2012      RC          2.15       Updated test for AddConfiguration().
 * 10/08/2012      RC          2.15       Added test for RemoveConfiguration().
 * 12/03/2012      RC          2.17       Updated AdcpSubsystemConfig.ToString().
 * 12/28/2012      RC          2.17       Moved AdcpSubsystemConfig.Subsystem into AdcpSubsystemConfig.SubsystemConfig.Subsystem.
 *                                         Made SubsystemConfiguration take a Subsystem in its constructor.
 *                                         Make AdcpConfiguration::AdcpSubsystemConfigExist() take only 1 argument.
 * 09/11/2013      RC          2.19.5     Updated test to 2.19.5
 * 09/17/2013      RC          2.20.0     Updated test to 2.20.0 with latest broadband modes.
 * 
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Collections.Generic;
    using RTI.Commands;

    /// <summary>
    /// Create test to test the AdcpConfiguration object.
    /// </summary>
    [TestFixture]
    public class AdcpConfigurationTest
    {

        #region CEPO

        /// <summary>
        /// Single Subsystem.
        /// </summary>
        [Test]
        public void TestSingleSubsystem()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("2", serial);

            // Get the last item in the dictionary
            // Should be only 1 anyway
            AdcpSubsystemConfig resultConfig = null;
            foreach(AdcpSubsystemConfig asConfig in result.Values)
            {
                resultConfig = asConfig;
            }

            Assert.AreEqual(1, result.Count, "Number of configurations is incorrect.");
            Assert.NotNull(resultConfig, "Dictionary entry is not null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig.SubsystemConfig.SubSystem.Code), "Subsystem Code is incorrect.");
            Assert.AreEqual(0, resultConfig.SubsystemConfig.SubSystem.Index, "Subsystem index is incorrect.");
            Assert.AreEqual(0, resultConfig.SubsystemConfig.CepoIndex, "SubsystemConfiguration CommandSetup is incorrect.");
            Assert.AreEqual(0, resultConfig.SubsystemConfig.CepoIndex, "AdcpSubsystemConfig index is incorrect.");
            Assert.AreEqual("[0] 1.2 MHz 4 beam 20 degree piston", resultConfig.ToString(), "AdcpSubsystemConfig toString is incorrect.");

            Assert.AreEqual("2", config.Commands.CEPO, "Commands CEPO is incorrect.");
        }

        /// <summary>
        /// Two different Subsystems.
        /// </summary>
        [Test]
        public void Test2Subsystem()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01230000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("23", serial);

            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            foreach (AdcpSubsystemConfig asConfig in result.Values)
            {
                if (asConfig.SubsystemConfig.SubSystem.Code == '2')
                {
                    resultConfig2 = asConfig;
                }

                if (asConfig.SubsystemConfig.SubSystem.Code == '3')
                {
                    resultConfig3 = asConfig;
                }
            }

            Assert.AreEqual(2, result.Count, "Number of configurations is incorrect.");

            Assert.NotNull(resultConfig2, "Dictionary 2 entry is not null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig2.SubsystemConfig.SubSystem.Code), "Subsystem 2 Code is incorrect.");
            Assert.AreEqual(0, resultConfig2.SubsystemConfig.SubSystem.Index, "Subsystem 2 index is incorrect.");
            Assert.AreEqual(0, resultConfig2.SubsystemConfig.CepoIndex, "SubsystemConfiguration 2 CommandSetup is incorrect.");
            Assert.AreEqual("[0] 1.2 MHz 4 beam 20 degree piston", resultConfig2.ToString(), "AdcpSubsystemConfig 2 toString is incorrect.");

            Assert.NotNull(resultConfig3, "Dictionary 3 entry is not null");
            Assert.AreEqual('3', Convert.ToChar(resultConfig3.SubsystemConfig.SubSystem.Code), "Subsystem 3 Code is incorrect.");
            Assert.AreEqual(1, resultConfig3.SubsystemConfig.SubSystem.Index, "Subsystem 3 index is incorrect.");
            Assert.AreEqual(1, resultConfig3.SubsystemConfig.CepoIndex, "SubsystemConfiguration 3 CommandSetup is incorrect.");
            Assert.AreEqual("[1] 600 kHz 4 beam 20 degree piston", resultConfig3.ToString(), "AdcpSubsystemConfig 3 toString is incorrect.");

            Assert.AreEqual("23", config.Commands.CEPO, "Commands CEPO is incorrect.");
        }

        /// <summary>
        /// Test Multiple same Subsystems.
        /// </summary>
        [Test]
        public void TestMultipleSameSubsystem()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("2222", serial);

            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig1 = null;
            AdcpSubsystemConfig resultConfig2 = null;
            AdcpSubsystemConfig resultConfig3 = null;
            AdcpSubsystemConfig resultConfig4 = null;
            foreach (AdcpSubsystemConfig asConfig in result.Values)
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
                        else
                        {
                            if (resultConfig4 == null)
                            {
                                resultConfig4 = asConfig;
                            }
                        }
                    }
                }

            }

            Assert.AreEqual(4, result.Count, "Number of configurations is incorrect.");

            Assert.NotNull(resultConfig1, "Dictionary 1 entry is not null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig1.SubsystemConfig.SubSystem.Code), "Subsystem 1 Code is incorrect.");
            Assert.AreEqual(0, resultConfig1.SubsystemConfig.SubSystem.Index, "Subsystem 1 index is incorrect.");
            Assert.AreEqual(0, resultConfig1.SubsystemConfig.CepoIndex, "SubsystemConfiguration 1 CommandSetup is incorrect.");
            Assert.AreEqual("[0] 1.2 MHz 4 beam 20 degree piston", resultConfig1.ToString(), "AdcpSubsystemConfig 1 toString is incorrect.");

            Assert.NotNull(resultConfig2, "Dictionary 2 entry is not null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig2.SubsystemConfig.SubSystem.Code), "Subsystem 2 Code is incorrect.");
            Assert.AreEqual(0, resultConfig2.SubsystemConfig.SubSystem.Index, "Subsystem 2 index is incorrect.");
            Assert.AreEqual(1, resultConfig2.SubsystemConfig.CepoIndex, "SubsystemConfiguration 2 CommandSetup is incorrect.");
            Assert.AreEqual("[1] 1.2 MHz 4 beam 20 degree piston", resultConfig2.ToString(), "AdcpSubsystemConfig 2 toString is incorrect.");

            Assert.NotNull(resultConfig3, "Dictionary 3 entry is not null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig3.SubsystemConfig.SubSystem.Code), "Subsystem 3 Code is incorrect.");
            Assert.AreEqual(0, resultConfig3.SubsystemConfig.SubSystem.Index, "Subsystem 3 index is incorrect.");
            Assert.AreEqual(2, resultConfig3.SubsystemConfig.CepoIndex, "SubsystemConfiguration 3 CommandSetup is incorrect.");
            Assert.AreEqual("[2] 1.2 MHz 4 beam 20 degree piston", resultConfig3.ToString(), "AdcpSubsystemConfig 3 toString is incorrect.");

            Assert.NotNull(resultConfig4, "Dictionary 4 entry is not null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig4.SubsystemConfig.SubSystem.Code), "Subsystem 4 Code is incorrect.");
            Assert.AreEqual(0, resultConfig4.SubsystemConfig.SubSystem.Index, "Subsystem 4 index is incorrect.");
            Assert.AreEqual(3, resultConfig4.SubsystemConfig.CepoIndex, "SubsystemConfiguration 4 CommandSetup is incorrect.");
            Assert.AreEqual("[3] 1.2 MHz 4 beam 20 degree piston", resultConfig4.ToString(), "AdcpSubsystemConfig 4 toString is incorrect.");

            Assert.AreEqual("2222", config.Commands.CEPO, "Commands CEPO is incorrect.");

        }

        /// <summary>
        /// Test multiple different Subsystems.
        /// </summary>
        [Test]
        public void TestMultipleDiffSubsystem()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01230000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("23223233", serial);

            // Get each config based off the subsystem code
            AdcpSubsystemConfig resultConfig2_1 = null;
            AdcpSubsystemConfig resultConfig2_2 = null;
            AdcpSubsystemConfig resultConfig2_3 = null;
            AdcpSubsystemConfig resultConfig2_4 = null;
            AdcpSubsystemConfig resultConfig3_1 = null;
            AdcpSubsystemConfig resultConfig3_2 = null;
            AdcpSubsystemConfig resultConfig3_3 = null;
            AdcpSubsystemConfig resultConfig3_4 = null;
            foreach (AdcpSubsystemConfig asConfig in result.Values)
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
                            else
                            {
                                if (resultConfig3_4 == null)
                                {
                                    resultConfig3_4 = asConfig;
                                }
                            }
                        }
                    }
                }

            }

            Assert.AreEqual(8, result.Count, "Number of configurations is incorrect.");

            Assert.NotNull(resultConfig2_1, "Dictionary 2_1 entry is not null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig2_1.SubsystemConfig.SubSystem.Code), "Subsystem 2_1 Code is incorrect.");
            Assert.AreEqual(0, resultConfig2_1.SubsystemConfig.SubSystem.Index, "Subsystem 2_1 index is incorrect.");
            Assert.AreEqual(0, resultConfig2_1.SubsystemConfig.CepoIndex, "SubsystemConfiguration 2_1 CommandSetup is incorrect.");
            Assert.AreEqual(0, resultConfig2_1.SubsystemConfig.CepoIndex, "CEPO 2_1 index is incorrect.");
            Assert.AreEqual("[0] 1.2 MHz 4 beam 20 degree piston", resultConfig2_1.ToString(), "AdcpSubsystemConfig 2_1 toString is incorrect.");

            Assert.NotNull(resultConfig2_2, "Dictionary 2_2 entry is not null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig2_2.SubsystemConfig.SubSystem.Code), "Subsystem 2_2 Code is incorrect.");
            Assert.AreEqual(0, resultConfig2_2.SubsystemConfig.SubSystem.Index, "Subsystem 2_2 index is incorrect.");
            Assert.AreEqual(2, resultConfig2_2.SubsystemConfig.CepoIndex, "SubsystemConfiguration 2_2 CommandSetup is incorrect.");
            Assert.AreEqual(2, resultConfig2_2.SubsystemConfig.CepoIndex, "CEPO 2_2 index is incorrect.");
            Assert.AreEqual("[2] 1.2 MHz 4 beam 20 degree piston", resultConfig2_2.ToString(), "AdcpSubsystemConfig 2_2 toString is incorrect.");

            Assert.NotNull(resultConfig2_3, "Dictionary 2_3 entry is not null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig2_3.SubsystemConfig.SubSystem.Code), "Subsystem 2_3 Code is incorrect.");
            Assert.AreEqual(0, resultConfig2_3.SubsystemConfig.SubSystem.Index, "Subsystem 2_3 index is incorrect.");
            Assert.AreEqual(3, resultConfig2_3.SubsystemConfig.CepoIndex, "SubsystemConfiguration 2_3 CommandSetup is incorrect.");
            Assert.AreEqual(3, resultConfig2_3.SubsystemConfig.CepoIndex, "CEPO 2_3 index is incorrect.");
            Assert.AreEqual("[3] 1.2 MHz 4 beam 20 degree piston", resultConfig2_3.ToString(), "AdcpSubsystemConfig 2_3 toString is incorrect.");

            Assert.NotNull(resultConfig2_4, "Dictionary 2_4 entry is not null");
            Assert.AreEqual('2', Convert.ToChar(resultConfig2_4.SubsystemConfig.SubSystem.Code), "Subsystem 2_4 Code is incorrect.");
            Assert.AreEqual(0, resultConfig2_4.SubsystemConfig.SubSystem.Index, "Subsystem 2_4 index is incorrect.");
            Assert.AreEqual(5, resultConfig2_4.SubsystemConfig.CepoIndex, "SubsystemConfiguration 2_4 CommandSetup is incorrect.");
            Assert.AreEqual(5, resultConfig2_4.SubsystemConfig.CepoIndex, "CEPO 2_4 index is incorrect.");
            Assert.AreEqual("[5] 1.2 MHz 4 beam 20 degree piston", resultConfig2_4.ToString(), "AdcpSubsystemConfig 2_4 toString is incorrect.");



            Assert.NotNull(resultConfig3_1, "Dictionary 3_1 entry is not null");
            Assert.AreEqual('3', Convert.ToChar(resultConfig3_1.SubsystemConfig.SubSystem.Code), "Subsystem 3_1 Code is incorrect.");
            Assert.AreEqual(1, resultConfig3_1.SubsystemConfig.SubSystem.Index, "Subsystem 3_1 index is incorrect.");
            Assert.AreEqual(1, resultConfig3_1.SubsystemConfig.CepoIndex, "SubsystemConfiguration 3_1 CommandSetup is incorrect.");
            Assert.AreEqual(1, resultConfig3_1.SubsystemConfig.CepoIndex, "CEPO 3_1 index is incorrect.");
            Assert.AreEqual("[1] 600 kHz 4 beam 20 degree piston", resultConfig3_1.ToString(), "AdcpSubsystemConfig 3_1 toString is incorrect.");

            Assert.NotNull(resultConfig3_2, "Dictionary 3_2 entry is not null");
            Assert.AreEqual('3', Convert.ToChar(resultConfig3_2.SubsystemConfig.SubSystem.Code), "Subsystem 3_2 Code is incorrect.");
            Assert.AreEqual(1, resultConfig3_2.SubsystemConfig.SubSystem.Index, "Subsystem 3_2 index is incorrect.");
            Assert.AreEqual(4, resultConfig3_2.SubsystemConfig.CepoIndex, "SubsystemConfiguration 3_2 CommandSetup is incorrect.");
            Assert.AreEqual(4, resultConfig3_2.SubsystemConfig.CepoIndex, "CEPO 3_2 index is incorrect.");
            Assert.AreEqual("[4] 600 kHz 4 beam 20 degree piston", resultConfig3_2.ToString(), "AdcpSubsystemConfig 3_2 toString is incorrect.");

            Assert.NotNull(resultConfig3_3, "Dictionary 3_3 entry is not null");
            Assert.AreEqual('3', Convert.ToChar(resultConfig3_3.SubsystemConfig.SubSystem.Code), "Subsystem 3_3 Code is incorrect.");
            Assert.AreEqual(1, resultConfig3_3.SubsystemConfig.SubSystem.Index, "Subsystem 3_3 index is incorrect.");
            Assert.AreEqual(6, resultConfig3_3.SubsystemConfig.CepoIndex, "SubsystemConfiguration 3_3 CommandSetup is incorrect.");
            Assert.AreEqual(6, resultConfig3_3.SubsystemConfig.CepoIndex, "CEPO 3_3 index is incorrect.");
            Assert.AreEqual("[6] 600 kHz 4 beam 20 degree piston", resultConfig3_3.ToString(), "AdcpSubsystemConfig 3_3 toString is incorrect.");

            Assert.NotNull(resultConfig3_4, "Dictionary 3_4 entry is not null");
            Assert.AreEqual('3', Convert.ToChar(resultConfig3_4.SubsystemConfig.SubSystem.Code), "Subsystem 3_4 Code is incorrect.");
            Assert.AreEqual(1, resultConfig3_4.SubsystemConfig.SubSystem.Index, "Subsystem 3_4 index is incorrect.");
            Assert.AreEqual(7, resultConfig3_4.SubsystemConfig.CepoIndex, "SubsystemConfiguration 3_4 CommandSetup is incorrect.");
            Assert.AreEqual(7, resultConfig3_4.SubsystemConfig.CepoIndex, "CEPO 3_4 index is incorrect.");
            Assert.AreEqual("[7] 600 kHz 4 beam 20 degree piston", resultConfig3_4.ToString(), "AdcpSubsystemConfig 3_4 toString is incorrect.");

            Assert.AreEqual("23223233", config.Commands.CEPO, "Commands CEPO is incorrect.");

        }

        /// <summary>
        /// Give a bad Subsystem in CEPO.
        /// </summary>
        [Test]
        public void TestBadSubsystem()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("3", serial);

            // Get the last item in the dictionary
            // Should be only 1 anyway
            AdcpSubsystemConfig resultConfig = null;
            foreach (AdcpSubsystemConfig asConfig in result.Values)
            {
                resultConfig = asConfig;
            }

            Assert.AreEqual(0, result.Count, "Number of configurations is incorrect.");
            Assert.IsNull(resultConfig, "Dictionary entry is not null");

            Assert.AreEqual("", config.Commands.CEPO, "Commands CEPO is incorrect.");
            Assert.AreEqual(AdcpCommands.DEFAULT_CEPO, config.Commands.CEPO, "Commands CEPO Default is incorrect.");
        }


        /// <summary>
        /// Give a bad Subsystem in CEPO.
        /// </summary>
        [Test]
        public void TestBadSubsystem1()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("32", serial);

            // Get the last item in the dictionary
            // Should be only 1 anyway
            AdcpSubsystemConfig resultConfig = null;
            foreach (AdcpSubsystemConfig asConfig in result.Values)
            {
                resultConfig = asConfig;
            }

            Assert.AreEqual(0, result.Count, "Number of configurations is incorrect.");
            Assert.IsNull(resultConfig, "Dictionary entry is not null");

            Assert.AreEqual("", config.Commands.CEPO, "Commands CEPO is incorrect.");
            Assert.AreEqual(AdcpCommands.DEFAULT_CEPO, config.Commands.CEPO, "Commands CEPO Default is incorrect.");
        }

        /// <summary>
        /// Give a bad Subsystem in CEPO.
        /// </summary>
        [Test]
        public void TestBadSubsystem2()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("$%^", serial);

            // Get the last item in the dictionary
            // Should be only 1 anyway
            AdcpSubsystemConfig resultConfig = null;
            foreach (AdcpSubsystemConfig asConfig in result.Values)
            {
                resultConfig = asConfig;
            }

            Assert.AreEqual(0, result.Count, "Number of configurations is incorrect.");
            Assert.IsNull(resultConfig, "Dictionary entry is not null");

            Assert.AreEqual("", config.Commands.CEPO, "Commands CEPO is incorrect.");
            Assert.AreEqual(AdcpCommands.DEFAULT_CEPO, config.Commands.CEPO, "Commands CEPO Default is incorrect.");
        }

        #endregion

        #region Commands

        /// <summary>
        /// Test AdcpCommands is created.
        /// </summary>
        [Test]
        public void TestAdcpCommands()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("2", serial);

            Assert.AreEqual(AdcpCommands.DEFAULT_C232B, config.Commands.C232B, "Default 232B is incorrect.");
        }

        /// <summary>
        /// Test setting AdcpCommands.
        /// </summary>
        [Test]
        public void TestSetAdcpCommands()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("2", serial);

            AdcpCommands commands = new AdcpCommands();
            commands.C232B = Baudrate.BAUD_9600;
            config.Commands = commands;

            Assert.AreNotEqual(AdcpCommands.DEFAULT_C232B, config.Commands.C232B, "AdcpCommand Not is incorrect.");
            Assert.AreEqual(Baudrate.BAUD_9600, config.Commands.C232B, "AdcpCommand is incorrect.");
        }

        #endregion

        #region Deployment Options

        /// <summary>
        /// Test DeploymentOptions is created.
        /// </summary>
        [Test]
        public void TestDeploymentOptions()
        {
            AdcpConfiguration config = new AdcpConfiguration();


            Assert.AreEqual(DeploymentOptions.DEFAULT_DEPTH_TO_BOTTOM, config.DeploymentOptions.DepthToBottom, "Default 232B is incorrect.");
        }

        /// <summary>
        /// Test setting DeploymentOptions.
        /// </summary>
        [Test]
        public void TestSetDeploymentOptions()
        {
            AdcpConfiguration config = new AdcpConfiguration();

            DeploymentOptions options = new DeploymentOptions();
            options.DepthToBottom = 2345;
            config.DeploymentOptions = options;

            Assert.AreEqual(2345, config.DeploymentOptions.DepthToBottom, "DeploymentOptions Depth To bottom is incorrect.");
        }

        /// <summary>
        /// Test setting DeploymentOptions.
        /// </summary>
        [Test]
        public void TestSetDeploymentOptions1()
        {
            AdcpConfiguration config = new AdcpConfiguration();

            config.DeploymentOptions.DepthToBottom = 2345;

            Assert.AreEqual(2345, config.DeploymentOptions.DepthToBottom, "DeploymentOptions Depth To bottom is incorrect.");
        }

        #endregion

        #region Validate CEPO

        /// <summary>
        /// Test ValidateCEPO.
        /// </summary>
        [Test]
        public void TestValidateCEPO()
        {
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            bool result = AdcpConfiguration.ValidateCEPO("2", serial);

            Assert.AreEqual(true, result, "ValidateCEPO is incorrect.");
        }

        /// <summary>
        /// Test ValidateCEPO.
        /// </summary>
        [Test]
        public void TestValidateCEPO1()
        {
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            bool result = AdcpConfiguration.ValidateCEPO("3", serial);

            Assert.AreEqual(false, result, "ValidateCEPO is incorrect.");
        }

        /// <summary>
        /// Test ValidateCEPO.
        /// </summary>
        [Test]
        public void TestValidateCEPO2()
        {
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            bool result = AdcpConfiguration.ValidateCEPO("^&*", serial);

            Assert.AreEqual(false, result, "ValidateCEPO is incorrect.");
        }

        /// <summary>
        /// Test ValidateCEPO.
        /// </summary>
        [Test]
        public void TestValidateCEPO3()
        {
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            bool result = AdcpConfiguration.ValidateCEPO("2222222", serial);

            Assert.AreEqual(true, result, "ValidateCEPO is incorrect.");
        }

        /// <summary>
        /// Test ValidateCEPO.
        /// </summary>
        [Test]
        public void TestValidateCEPO4()
        {
            SerialNumber serial = new SerialNumber("01230000000000000000000000000004");
            bool result = AdcpConfiguration.ValidateCEPO("2323232332", serial);

            Assert.AreEqual(true, result, "ValidateCEPO is incorrect.");
        }


        /// <summary>
        /// Test ValidateCEPO.
        /// </summary>
        [Test]
        public void TestValidateCEPO5()
        {
            SerialNumber serial = new SerialNumber();
            bool result = AdcpConfiguration.ValidateCEPO("2323232332", serial);

            Assert.AreEqual(false, result, "ValidateCEPO is incorrect.");
        }

        /// <summary>
        /// Test ValidateCEPO.
        /// </summary>
        [Test]
        public void TestValidateCEPO6()
        {
            SerialNumber serial = new SerialNumber();
            bool result = AdcpConfiguration.ValidateCEPO("", serial);

            Assert.AreEqual(false, result, "ValidateCEPO is incorrect.");
        }

        #endregion

        #region JSON

        /// <summary>
        /// Test creating a JSON and converting back to object.
        /// </summary>
        [Test]
        public void TestJSON()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("2", serial);

            // Get the last item in the dictionary
            // Should be only 1 anyway
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig asConfig in result.Values)
            {
                ssConfig = asConfig;
            }

            // Change AdcpCommands
            config.Commands.C232B = Baudrate.BAUD_9600;

            // Change the Subsystem Configuration
            ssConfig.Commands.CBTBL = 4;
            ssConfig.Commands.CBTMX = 32.36f;

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(config);                                      // Serialize object to JSON
            AdcpConfiguration newConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AdcpConfiguration>(json);   // Deserialize the JSON

            Assert.AreEqual("2", newConfig.Commands.CEPO, "Commands CEPO is incorrect.");
            Assert.AreEqual(Baudrate.BAUD_9600, newConfig.Commands.C232B, "JSON C232B is incorrect.");


            // Get the last item in the dictionary
            // Should be only 1 anyway
            AdcpSubsystemConfig newSsConfig = null;
            foreach (AdcpSubsystemConfig asConfig in newConfig.SubsystemConfigDict.Values)
            {
                newSsConfig = asConfig;
            }

            Assert.AreEqual(4, newSsConfig.Commands.CBTBL, "CBTBL is incorrect.");
            Assert.AreEqual(32.36f, newSsConfig.Commands.CBTMX, "CBTMX is incorrect.");
        }

        /// <summary>
        /// Test creating a JSON and converting back to object.
        /// </summary>
        [Test]
        public void TestJSONFull()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("2", serial);

            // Get the last item in the dictionary
            // Should be only 1 anyway
            AdcpSubsystemConfig ssConfig = null;
            foreach (AdcpSubsystemConfig asConfig in result.Values)
            {
                ssConfig = asConfig;
            }

            #region AdcpCommands

            // Change AdcpCommands
            config.Commands.Mode = AdcpCommands.AdcpMode.DVL;
            
            config.Commands.CEI_Hour = 3;
            config.Commands.CEI_Minute = 3;
            config.Commands.CEI_Second = 3;
            config.Commands.CEI_HunSec = 3;

            //config.Commands.CETFP.Year = 2022;
            //config.Commands.CETFP.Month = 2;
            //config.Commands.CETFP.Day = 2;
            //config.Commands.CETFP_Hour = 2;
            //config.Commands.CETFP_Minute = 2;
            //config.Commands.CETFP_Second = 2;
            //config.Commands.CETFP_HunSec = 2;
            config.Commands.CETFP = new DateTime(2022, 2, 2, 2, 2, 2);

            config.Commands.CERECORD_EnsemblePing = false;
            config.Commands.CEOUTPUT = AdcpCommands.AdcpOutputMode.ASCII;

            config.Commands.CWS = 33;
            config.Commands.CWT = 3.33f;
            config.Commands.CTD = 3.33f;
            config.Commands.CWSS = 3.33f;
            config.Commands.CHO = 33.33f;
            config.Commands.CHS = HeadingSrc.SERIAL;
            config.Commands.CVSF = 33.33f;

            config.Commands.C232B = Baudrate.BAUD_9600;
            config.Commands.C485B = Baudrate.BAUD_921600;
            config.Commands.C422B = Baudrate.BAUD_4800;

            #endregion

            #region SubsystemConfig

            // Change the Subsystem Configuration
            ssConfig.Commands.CWPON = false;
            ssConfig.Commands.CWPBB_LagLength = 0.2345f;
            ssConfig.Commands.CWPBB_TransmitPulseType = AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE;
            ssConfig.Commands.CWPAP_NumPingsAvg = 34;
            ssConfig.Commands.CWPAP_TimeBetweenPing = 23.45f;
            ssConfig.Commands.CWPAP_Lag = 0.2345f;
            ssConfig.Commands.CWPAP_Blank = 45.56f;
            ssConfig.Commands.CWPAP_BinSize = 12.34f;
            ssConfig.Commands.CWPST_CorrelationThresh = 0.4578f;
            ssConfig.Commands.CWPST_VVelocityThresh = 0.4678f;
            ssConfig.Commands.CWPST_QVelocityThresh = 0.4778f;
            ssConfig.Commands.CWPBL = 78.96f;
            ssConfig.Commands.CWPBS = 34.56f;
            ssConfig.Commands.CWPX = 12.34f;
            ssConfig.Commands.CWPBN = 12;
            ssConfig.Commands.CWPP = 13;
            ssConfig.Commands.CWPBP_NumPingsAvg = 34;
            ssConfig.Commands.CWPBP_TimeBetweenBasePings = 56.23f;
            ssConfig.Commands.CWPAI = new TimeValue(1, 2, 3, 4);
            ssConfig.Commands.CWPTBP = 4567.67f;

            ssConfig.Commands.CBTON = false;
            ssConfig.Commands.CBTBB_Mode = AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED;
            ssConfig.Commands.CBTBB_LongRangeDepth = 23.456f;
            ssConfig.Commands.CBTBB_PulseToPulseLag = 34.567f;
            ssConfig.Commands.CBTST_CorrelationThresh = 0.4578f;
            ssConfig.Commands.CBTST_VVelocityThresh = 0.4678f;
            ssConfig.Commands.CBTST_QVelocityThresh = 0.4778f;
            ssConfig.Commands.CBTT_DepthGain = 45.232f;
            ssConfig.Commands.CBTT_DepthSNR = 234.567f;
            ssConfig.Commands.CBTT_SNRDeepDetectionThresh = 45.67f;
            ssConfig.Commands.CBTT_SNRShallowDetectionThresh = 56.432f;
            ssConfig.Commands.CBTBL = 4;
            ssConfig.Commands.CBTMX = 32.36f;
            ssConfig.Commands.CBTTBP = 23987.345f;

            ssConfig.Commands.CWTON = false;
            ssConfig.Commands.CWTBB = false;
            ssConfig.Commands.CWTBL = 23.345f;
            ssConfig.Commands.CWTBS = 28.12234f;
            ssConfig.Commands.CWTTBP = 3434.234f;

            #endregion

            #region Deployment Options

            config.DeploymentOptions.BatteryType = DeploymentOptions.AdcpBatteryType.Lithium_7DD;
            config.DeploymentOptions.DepthToBottom = 23;
            config.DeploymentOptions.Duration = 45;
            config.DeploymentOptions.NumBatteries = 12;

            #endregion

            // Convert to JSON and back
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(config);                                      // Serialize object to JSON
            AdcpConfiguration newConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AdcpConfiguration>(json);   // Deserialize the JSON

            Assert.AreEqual(serial, newConfig.SerialNumber, "Serial is incorrect.");
            Assert.AreEqual("2", newConfig.Commands.CEPO, "Commands CEPO is incorrect.");
            Assert.AreEqual(1, newConfig.SubsystemConfigDict.Count, "Number of SubsystemConfig is incorrect.");

            #region AdcpCommands

            Assert.AreEqual(3, newConfig.Commands.CEI_Hour, "CEI_Hour is incorrect.");
            Assert.AreEqual(3, newConfig.Commands.CEI_Minute, "CEI_Minute is incorrect.");
            Assert.AreEqual(3, newConfig.Commands.CEI_Second, "CEI_Second is incorrect.");
            Assert.AreEqual(3, newConfig.Commands.CEI_HunSec, "CEI_HunSec is incorrect.");

            Assert.AreEqual(2022, newConfig.Commands.CETFP.Year, "CETFP_Year is incorrect.");
            Assert.AreEqual(2, newConfig.Commands.CETFP.Month, "CETFP_Month is incorrect.");
            Assert.AreEqual(2, newConfig.Commands.CETFP.Day, "CETFP_Day is incorrect.");
            Assert.AreEqual(2, newConfig.Commands.CETFP.Hour, "CETFP_Hour is incorrect.");
            Assert.AreEqual(2, newConfig.Commands.CETFP.Minute, "CETFP_Minute is incorrect.");
            Assert.AreEqual(2, newConfig.Commands.CETFP.Second, "CETFP_Second is incorrect.");
            //Assert.AreEqual(2, newConfig.Commands.CETFP_HunSec, "CETFP_HunSec is incorrect.");


            Assert.AreEqual(false, newConfig.Commands.CERECORD_EnsemblePing, "CERECORD is incorrect.");
            Assert.AreEqual(AdcpCommands.AdcpOutputMode.ASCII, newConfig.Commands.CEOUTPUT, "CEOUTPUT is incorrect.");

            Assert.AreEqual(33, newConfig.Commands.CWS, 0.0001, "CWS is incorrect.");
            Assert.AreEqual(3.33f, newConfig.Commands.CWT, 0.0001, "CWT is incorrect.");
            Assert.AreEqual(3.33f, newConfig.Commands.CTD, 0.0001, "CTD is incorrect.");
            Assert.AreEqual(3.33f, newConfig.Commands.CWSS, 0.0001, "CWSS is incorrect.");
            Assert.AreEqual(33.33f, newConfig.Commands.CHO, 0.0001, "CHO is incorrect.");
            Assert.AreEqual(HeadingSrc.SERIAL, newConfig.Commands.CHS, "CHS is incorrect.");
            Assert.AreEqual(33.33f, newConfig.Commands.CVSF, 0.0001, "CVSF is incorrect.");

            Assert.AreEqual(Baudrate.BAUD_9600, newConfig.Commands.C232B, "JSON C232B is incorrect.");
            Assert.AreEqual(Baudrate.BAUD_921600, newConfig.Commands.C485B, "JSON C485B is incorrect.");
            Assert.AreEqual(Baudrate.BAUD_4800, newConfig.Commands.C422B, "JSON C422B is incorrect.");

            #endregion

            // Get the last item in the dictionary
            // Should be only 1 anyway
            AdcpSubsystemConfig newSsConfig = null;
            foreach (AdcpSubsystemConfig asConfig in newConfig.SubsystemConfigDict.Values)
            {
                newSsConfig = asConfig;
            }

            Assert.AreEqual("[0] 1.2 MHz 4 beam 20 degree piston", newSsConfig.ToString(), "SubsystemConfig string is incorrect.");

            #region SubsystemConfig

            Assert.AreEqual(false, newSsConfig.Commands.CWPON, "CWPON is incorrect.");
            Assert.AreEqual(0.2345f, newSsConfig.Commands.CWPBB_LagLength, 0.0001, "CWPBB_LagLength is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE, newSsConfig.Commands.CWPBB_TransmitPulseType, "CWPBB_TransmitPulseType is incorrect.");
            Assert.AreEqual(34, newSsConfig.Commands.CWPAP_NumPingsAvg, "CWPAP_NumPingsAvg is incorrect.");
            Assert.AreEqual(0.2345f, newSsConfig.Commands.CWPBB_LagLength, 0.0001, "CWPBB_LagLength is incorrect.");
            Assert.AreEqual(23.45f, newSsConfig.Commands.CWPAP_TimeBetweenPing, 0.0001, "CWPAP_TimeBetweenPing is incorrect.");
            Assert.AreEqual(0.2345f, newSsConfig.Commands.CWPAP_Lag, 0.0001, "CWPAP_Lag is incorrect.");
            Assert.AreEqual(45.56f, newSsConfig.Commands.CWPAP_Blank, 0.0001, "CWPAP_Blank is incorrect.");
            Assert.AreEqual(12.34f, newSsConfig.Commands.CWPAP_BinSize, 0.0001, "CWPAP_BinSize is incorrect.");
            Assert.AreEqual(0.4578f, newSsConfig.Commands.CWPST_CorrelationThresh, 0.0001, "CWPST_CorrelationThresh is incorrect.");
            Assert.AreEqual(0.4678f, newSsConfig.Commands.CWPST_VVelocityThresh, 0.0001, "CWPST_VVelocityThresh is incorrect.");
            Assert.AreEqual(0.4778f, newSsConfig.Commands.CWPST_QVelocityThresh, 0.0001, "CWPST_QVelocityThresh is incorrect.");
            Assert.AreEqual(78.96f, newSsConfig.Commands.CWPBL, 0.0001, "CWPBL is incorrect.");
            Assert.AreEqual(34.56f, newSsConfig.Commands.CWPBS, 0.0001, "CWPBS is incorrect.");
            Assert.AreEqual(12.34f, newSsConfig.Commands.CWPX, 0.0001, "CWPX is incorrect.");
            Assert.AreEqual(12, newSsConfig.Commands.CWPBN, "CWPBN is incorrect.");
            Assert.AreEqual(13, newSsConfig.Commands.CWPP, "CWPP is incorrect.");
            Assert.AreEqual(34, newSsConfig.Commands.CWPBP_NumPingsAvg, "CWPBP_NumPingsAvg is incorrect.");
            Assert.AreEqual(56.23f, newSsConfig.Commands.CWPBP_TimeBetweenBasePings, 0.0001, "CWPBP_TimeBetweenBasePings is incorrect.");
            Assert.AreEqual(new TimeValue(1,2,3,4), newSsConfig.Commands.CWPAI, "CWPBP_NumPingsAvg is incorrect.");
            Assert.AreEqual(4567.67f, newSsConfig.Commands.CWPTBP, 0.0001, "CWPTBP is incorrect.");

            Assert.AreEqual(false, newSsConfig.Commands.CBTON, "CBTON is incorrect.");
            Assert.AreEqual(AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED, newSsConfig.Commands.CBTBB_Mode, "CBTBB_Mode is incorrect.");
            Assert.AreEqual(23.456f, newSsConfig.Commands.CBTBB_LongRangeDepth, 0.0001, "CBTBB_LongRangeDepth is incorrect.");
            Assert.AreEqual(34.567f, newSsConfig.Commands.CBTBB_PulseToPulseLag, 0.0001, "CBTBB_PulseToPulseLag is incorrect.");
            Assert.AreEqual(0.4578f, newSsConfig.Commands.CBTST_CorrelationThresh, 0.0001, "CBTST_CorrelationThresh is incorrect.");
            Assert.AreEqual(0.4678f, newSsConfig.Commands.CBTST_VVelocityThresh, 0.0001, "CBTST_VVelocityThresh is incorrect.");
            Assert.AreEqual(0.4778f, newSsConfig.Commands.CBTST_QVelocityThresh, 0.0001, "CBTST_QVelocityThresh is incorrect.");
            Assert.AreEqual(45.232f, newSsConfig.Commands.CBTT_DepthGain, 0.0001, "CBTT_DepthGain is incorrect.");
            Assert.AreEqual(234.567f, newSsConfig.Commands.CBTT_DepthSNR, 0.0001, "CBTT_DepthSNR is incorrect.");
            Assert.AreEqual(45.67f, newSsConfig.Commands.CBTT_SNRDeepDetectionThresh, 0.0001, "CBTT_SNRDeepDetectionThresh is incorrect.");
            Assert.AreEqual(56.432f, newSsConfig.Commands.CBTT_SNRShallowDetectionThresh, 0.0001, "CBTT_SNRShallowDetectionThresh is incorrect.");
            Assert.AreEqual(23987.345f, newSsConfig.Commands.CBTTBP, 0.0001, "CBTTBP is incorrect.");
            Assert.AreEqual(4, newSsConfig.Commands.CBTBL, "CBTBL is incorrect.");
            Assert.AreEqual(32.36f, newSsConfig.Commands.CBTMX, "CBTMX is incorrect.");

            Assert.AreEqual(false, newSsConfig.Commands.CWTON, "CWTON is incorrect.");
            Assert.AreEqual(false, newSsConfig.Commands.CWTBB, "CWTBB is incorrect.");
            Assert.AreEqual(23.345f, newSsConfig.Commands.CWTBL, 0.0001, "CWTBL is incorrect.");
            Assert.AreEqual(28.12234f, newSsConfig.Commands.CWTBS, 0.0001, "CWTBS is incorrect.");
            Assert.AreEqual(3434.234f, newSsConfig.Commands.CWTTBP, 0.0001, "CWTTBP is incorrect.");

            #endregion

            #region DeploymentOptions

            Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Lithium_7DD, newConfig.DeploymentOptions.BatteryType, "Battery Type is incorrect.");
            Assert.AreEqual(23, newConfig.DeploymentOptions.DepthToBottom, "Depth to bottom is incorrect.");
            Assert.AreEqual(45, newConfig.DeploymentOptions.Duration, "Duration is incorrect.");
            Assert.AreEqual(12, newConfig.DeploymentOptions.NumBatteries, "NumBatteries is incorrect.");

            #endregion
        }

        #endregion

        #region AdcpSubsystemConfigExist

        /// <summary>
        /// Test the AdcpSubsystemConfigExist() method.
        /// </summary>
        [Test]
        public void AdcpSubsystemConfigExist()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("2", serial);

            Subsystem ss = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig = new SubsystemConfiguration(ss, 0, 0);        // Number of configurations for a given subsystem

            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig), "AdcpSubsystemConfigExist() is incorrect.");
        }

        /// <summary>
        /// Test the AdcpSubsystemConfigExist() method.
        /// 
        /// 2 subsystem and 1 configuration for each.
        /// </summary>
        [Test]
        public void AdcpSubsystemConfigExist1()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01230000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("23", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 1, 1);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 0, 0);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 1

            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig20), "AdcpSubsystemConfigExist() 2 0 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig30), "AdcpSubsystemConfigExist() 3 0 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig21), "AdcpSubsystemConfigExist() 2 1 is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig31), "AdcpSubsystemConfigExist() 3 1 is incorrect.");
        }

        /// <summary>
        /// Test the AdcpSubsystemConfigExist() method.
        /// 
        /// 1 configuration.  Verify no others exist.
        /// </summary>
        [Test]
        public void AdcpSubsystemConfigExist2()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("0120000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("2", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 1, 1);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 0, 0);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 1

            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig20), "AdcpSubsystemConfigExist() 2 0 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig30), "AdcpSubsystemConfigExist() 3 0 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig21), "AdcpSubsystemConfigExist() 2 1 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig31), "AdcpSubsystemConfigExist() 3 1 is incorrect.");
        }

        /// <summary>
        /// Test the AdcpSubsystemConfigExist() method.
        /// 
        /// Mulitple configurations.
        /// </summary>
        [Test]
        public void AdcpSubsystemConfigExist3()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("0120000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("222", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 1, 1);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig22 = new SubsystemConfiguration(ss2, 2, 2);         // Configuration SS2 with Config Number 2
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 0, 0);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 1
            SubsystemConfiguration ssConfig32 = new SubsystemConfiguration(ss3, 2, 2);         // Configuration SS3 with Config Number 2

            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig20), "AdcpSubsystemConfigExist() 2 0 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig30), "AdcpSubsystemConfigExist() 3 0 is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig21), "AdcpSubsystemConfigExist() 2 1 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig31), "AdcpSubsystemConfigExist() 3 1 is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig22), "AdcpSubsystemConfigExist() 2 2 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig32), "AdcpSubsystemConfigExist() 3 2 is incorrect.");
        }

        /// <summary>
        /// Test the AdcpSubsystemConfigExist() method.
        /// 
        /// Bad CEPO given.
        /// </summary>
        [Test]
        public void AdcpSubsystemConfigExist4()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("0120000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("232332", serial);      // FAILS, 3 does not exist in serial number

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 1, 1);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig22 = new SubsystemConfiguration(ss2, 2, 2);         // Configuration SS2 with Config Number 2
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 0, 0);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 1
            SubsystemConfiguration ssConfig32 = new SubsystemConfiguration(ss3, 2, 2);         // Configuration SS3 with Config Number 2

            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig20), "AdcpSubsystemConfigExist() 2 0 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig30), "AdcpSubsystemConfigExist() 3 0 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig21), "AdcpSubsystemConfigExist() 2 1 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig31), "AdcpSubsystemConfigExist() 3 1 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig22), "AdcpSubsystemConfigExist() 2 2 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig32), "AdcpSubsystemConfigExist() 3 2 is incorrect.");
        }

        /// <summary>
        /// Test the AdcpSubsystemConfigExist() method.
        /// 
        /// Give multiple configurations for 2 subsystems.
        /// </summary>
        [Test]
        public void AdcpSubsystemConfigExist5()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("0123000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("232332", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 2, 2);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig22 = new SubsystemConfiguration(ss2, 5, 5);         // Configuration SS2 with Config Number 2
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 3, 3);         // Configuration SS3 with Config Number 1
            SubsystemConfiguration ssConfig32 = new SubsystemConfiguration(ss3, 4, 4);         // Configuration SS3 with Config Number 2

            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig20), "AdcpSubsystemConfigExist() 2 0 is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig30), "AdcpSubsystemConfigExist() 3 0 is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig21), "AdcpSubsystemConfigExist() 2 1 is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig31), "AdcpSubsystemConfigExist() 3 1 is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig22), "AdcpSubsystemConfigExist() 2 2 is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig32), "AdcpSubsystemConfigExist() 3 2 is incorrect.");
        }

        /// <summary>
        /// Test the AdcpSubsystemConfigExist() method.
        /// 
        /// Give nulls for Subsystem and SubsystemConfiguration.
        /// </summary>
        [Test]
        public void AdcpSubsystemConfigExistNull()
        {
            AdcpConfiguration config = new AdcpConfiguration();

            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(null), "AdcpSubsystemConfigExist() is incorrect.");
        }

        /// <summary>
        /// Test the AdcpSubsystemConfigExist() method.
        /// 
        /// Give nulls for Subsystem and SubsystemConfiguration.
        /// </summary>
        [Test]
        public void AdcpSubsystemConfigExistNull1()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig0 = new SubsystemConfiguration(ss2, 0, 0);        // Number of configurations for a given subsystem

            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(null), "AdcpSubsystemConfigExist() is incorrect.");
        }

        /// <summary>
        /// Test the AdcpSubsystemConfigExist() method.
        /// 
        /// Give nulls for Subsystem and SubsystemConfiguration.
        /// </summary>
        [Test]
        public void AdcpSubsystemConfigExistNull2()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig0 = new SubsystemConfiguration(ss2, 0, 0);        // Number of configurations for a given subsystem

            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig0), "AdcpSubsystemConfigExist() is incorrect.");
        }

        #endregion

        #region GetAdcpSubsystemConfig

        /// <summary>
        /// Test the GetAdcpSubsystemConfig() method.
        /// </summary>
        [Test]
        public void GetAdcpSubsystemConfig()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("2", serial);

            Subsystem ss = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig = new SubsystemConfiguration(ss, 0 , 0);        // Number of configurations for a given subsystem

            AdcpSubsystemConfig asConfig = config.GetAdcpSubsystemConfig(ssConfig);

            Assert.NotNull(asConfig, "GetAdcpSubsystemConfig() is incorrect.");
        }

        /// <summary>
        /// Test the GetAdcpSubsystemConfig() method.
        /// 
        /// Only 1 AdcpSubsystemConfig
        /// </summary>
        [Test]
        public void GetAdcpSubsystemConfig1()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("2", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 1, 1);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 0, 0);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 1

            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig20), "GetAdcpSubsystemConfig() 2 0 is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig30), "GetAdcpSubsystemConfig() 3 0 is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig21), "GetAdcpSubsystemConfig() 2 1 is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig31), "GetAdcpSubsystemConfig() 3 1 is incorrect.");
        }

        /// <summary>
        /// Test the GetAdcpSubsystemConfig() method.
        /// 
        /// Only 1 AdcpSubsystemConfig.
        /// </summary>
        [Test]
        public void GetAdcpSubsystemConfig2()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01230000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("2", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 1, 1);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 0, 0);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 1

            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig20), "GetAdcpSubsystemConfig() 2 0 is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig30), "GetAdcpSubsystemConfig() 3 0 is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig21), "GetAdcpSubsystemConfig() 2 1 is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig31), "GetAdcpSubsystemConfig() 3 1 is incorrect.");
        }

        /// <summary>
        /// Test the GetAdcpSubsystemConfig() method.
        /// 
        /// 2 Subystems, 3 configurations.
        /// </summary>
        [Test]
        public void GetAdcpSubsystemConfig3()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01230000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("233", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 1, 1);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 2, 2);         // Configuration SS3 with Config Number 1

            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig20), "GetAdcpSubsystemConfig() 2 0 is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig30), "GetAdcpSubsystemConfig() 3 0 is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig21), "GetAdcpSubsystemConfig() 2 1 is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig31), "GetAdcpSubsystemConfig() 3 1 is incorrect.");
        }

        /// <summary>
        /// Test the GetAdcpSubsystemConfig() method.
        /// 
        /// Bad CEPO. No configurations.
        /// </summary>
        [Test]
        public void GetAdcpSubsystemConfig4()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("233", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 1, 1);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 0, 0);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 1

            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig20), "GetAdcpSubsystemConfig() 2 0 is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig30), "GetAdcpSubsystemConfig() 3 0 is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig21), "GetAdcpSubsystemConfig() 2 1 is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig31), "GetAdcpSubsystemConfig() 3 1 is incorrect.");
        }

        /// <summary>
        /// Test the GetAdcpSubsystemConfig() method.
        /// 
        /// Give multiple configurations for 2 subsystems.
        /// </summary>
        [Test]
        public void GetAdcpSubsystemConfig5()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("0123000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("232332", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 2, 2);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig22 = new SubsystemConfiguration(ss2, 5, 5);         // Configuration SS2 with Config Number 2
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 3, 3);         // Configuration SS3 with Config Number 1
            SubsystemConfiguration ssConfig32 = new SubsystemConfiguration(ss3, 4, 4);         // Configuration SS3 with Config Number 2

            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig20), "GetAdcpSubsystemConfig() 2 0 is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig30), "GetAdcpSubsystemConfig() 3 0 is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig21), "GetAdcpSubsystemConfig() 2 1 is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig31), "GetAdcpSubsystemConfig() 3 1 is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig22), "GetAdcpSubsystemConfig() 2 2 is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig32), "GetAdcpSubsystemConfig() 3 2 is incorrect.");
        }

        /// <summary>
        /// Test the GetAdcpSubsystemConfig() method.
        /// 
        /// Give null for Subsystem and SubsystemConfiguration.
        /// </summary>
        [Test]
        public void GetAdcpSubsystemConfigNull()
        {
            AdcpConfiguration config = new AdcpConfiguration();

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig0 = new SubsystemConfiguration(ss2, 0, 0);        // Number of configurations for a given subsystem

            Assert.IsNull(config.GetAdcpSubsystemConfig(null), "GetAdcpSubsystemConfig() is incorrect.");
        }

        /// <summary>
        /// Test the GetAdcpSubsystemConfig() method.
        /// 
        /// Give null for Subsystem and SubsystemConfiguration.
        /// </summary>
        [Test]
        public void GetAdcpSubsystemConfigNull1()
        {
            AdcpConfiguration config = new AdcpConfiguration();

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig0 = new SubsystemConfiguration(ss2, 0, 0);        // Number of configurations for a given subsystem

            Assert.IsNull(config.GetAdcpSubsystemConfig(null), "GetAdcpSubsystemConfig() is incorrect.");
        }

        /// <summary>
        /// Test the GetAdcpSubsystemConfig() method.
        /// 
        /// Give null for Subsystem and SubsystemConfiguration.
        /// </summary>
        [Test]
        public void GetAdcpSubsystemConfigNull2()
        {
            AdcpConfiguration config = new AdcpConfiguration();

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig0 = new SubsystemConfiguration(ss2, 0, 0);        // Number of configurations for a given subsystem

            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig0), "GetAdcpSubsystemConfig() is incorrect.");
        }

        #endregion

        #region AddConfiguration

        /// <summary>
        /// Test the AddConfiguration() method.
        /// </summary>
        public void AddConfiguration()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("2", serial);

            Subsystem ss = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig0 = new SubsystemConfiguration(ss, 0, 0);       // Number of configurations for a given subsystem
            SubsystemConfiguration ssConfig1 = new SubsystemConfiguration(ss, 1, 1);       // A second configuration for a subsystem

            AdcpSubsystemConfig asConfig1 = null;                                   // Create an AdcpSubsystemConfig to get the result
            bool addConfigResult = config.AddConfiguration(ss, out asConfig1);      // Add another configuration for Subsystem 2

            Assert.IsTrue(addConfigResult, "AddConfiguration() is incorrect.");
            Assert.IsNotNull(asConfig1, "asConfig1 is incorrect.");
            Assert.AreEqual(ss, asConfig1.SubsystemConfig.SubSystem, "asConfig1 Subsystem is incorrect.");
            Assert.AreEqual(1, asConfig1.SubsystemConfig.CepoIndex, "asConfig1 CEPO index is incorrect.");
            Assert.AreEqual(ssConfig1, asConfig1.SubsystemConfig, "asConfig1 SubsystemConfiguration is incorrect.");
            Assert.IsNotNull(asConfig1.Commands, "asConfig1 Commands is incorrect.");
            Assert.AreEqual(ss, asConfig1.Commands.SubsystemConfig.SubSystem, "asConfig1 Commands Subsystem is incorrect.");
            Assert.AreEqual(1, asConfig1.Commands.SubsystemConfig.CepoIndex, "asConfig1 Commands CEPO index is incorrect.");

            Assert.AreEqual("22", config.Commands.CEPO, "Commands CEPO is incorrect.");
            Assert.AreEqual(2, config.SubsystemConfigDict.Count, "SubsystemConfigDict Count is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig0), "AdcpSubsystemConfigExist() 2 0 is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig1), "AdcpSubsystemConfigExist() 2 1 is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig0), "GetAdcpSubsystemConfig() 2 0  is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig1), "GetAdcpSubsystemConfig() 2 1  is incorrect.");
        }

        /// <summary>
        /// Test the AddConfiguration() method.
        /// 
        /// Add a different subsystem.
        /// </summary>
        public void AddConfiguration1()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01230000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("2", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 1, 0);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 0, 0);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 1

            AdcpSubsystemConfig asConfig1 = null;
            bool addConfigResult = config.AddConfiguration(ss3, out asConfig1);      // Add another configuration for Subsystem 2

            Assert.IsTrue(addConfigResult, "AddConfiguration() is incorrect.");
            Assert.IsNotNull(asConfig1, "asConfig1 is incorrect.");
            Assert.AreEqual(ss3, asConfig1.SubsystemConfig.SubSystem, "asConfig1 Subsystem is incorrect.");
            Assert.AreEqual(1, asConfig1.SubsystemConfig.CepoIndex, "asConfig1 CEPO index is incorrect.");
            Assert.AreEqual(ssConfig31, asConfig1.SubsystemConfig, "asConfig1 SubsystemConfiguration is incorrect.");
            Assert.IsNotNull(asConfig1.Commands, "asConfig1 Commands is incorrect.");
            Assert.AreEqual(ss3, asConfig1.Commands.SubsystemConfig.SubSystem, "asConfig1 Commands Subsystem is incorrect.");
            Assert.AreEqual(1, asConfig1.Commands.SubsystemConfig.CepoIndex, "asConfig1 Commands CEPO index is incorrect.");

            Assert.AreEqual("23", config.Commands.CEPO, "Commands CEPO is incorrect.");
            Assert.AreEqual(2, config.SubsystemConfigDict.Count, "SubsystemConfigDict Count is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig20), "AdcpSubsystemConfigExist() 2 0 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig21), "AdcpSubsystemConfigExist() 2 1 is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig30), "AdcpSubsystemConfigExist() 3 0 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig31), "AdcpSubsystemConfigExist() 3 1 is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig20), "GetAdcpSubsystemConfig() 2 0  is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig21), "GetAdcpSubsystemConfig() 2 1  is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig20), "GetAdcpSubsystemConfig() 3 0  is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig21), "GetAdcpSubsystemConfig() 3 1  is incorrect.");
        }

        /// <summary>
        /// Test the AddConfiguration() method.
        /// 
        /// 2 Subsystems,  Add a different subsystem.
        /// </summary>
        public void AddConfiguration2()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01230000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("22", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 1, 1);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 0, 0);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 1

            AdcpSubsystemConfig asConfig1 = null;
            bool addConfigResult = config.AddConfiguration(ss3, out asConfig1);      // Add another configuration for Subsystem 3

            Assert.IsTrue(addConfigResult, "AddConfiguration() is incorrect.");
            Assert.IsNotNull(asConfig1, "asConfig1 is incorrect.");
            Assert.AreEqual(ss3, asConfig1.SubsystemConfig.SubSystem, "asConfig1 Subsystem is incorrect.");
            Assert.AreEqual(2, asConfig1.SubsystemConfig.CepoIndex, "asConfig1 CEPO index is incorrect.");
            Assert.AreEqual(ssConfig31, asConfig1.SubsystemConfig, "asConfig1 SubsystemConfiguration is incorrect.");
            Assert.IsNotNull(asConfig1.Commands, "asConfig1 Commands is incorrect.");
            Assert.AreEqual(ss3, asConfig1.Commands.SubsystemConfig.SubSystem, "asConfig1 Commands Subsystem is incorrect.");
            Assert.AreEqual(2, asConfig1.Commands.SubsystemConfig.CepoIndex, "asConfig1 Commands CEPO index is incorrect.");

            Assert.AreEqual("223", config.Commands.CEPO, "Commands CEPO is incorrect.");
            Assert.AreEqual(3, config.SubsystemConfigDict.Count, "SubsystemConfigDict Count is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig20), "AdcpSubsystemConfigExist() 2 0 is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig21), "AdcpSubsystemConfigExist() 2 1 is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig30), "AdcpSubsystemConfigExist() 3 0 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig31), "AdcpSubsystemConfigExist() 3 1 is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig20), "GetAdcpSubsystemConfig() 2 0  is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig21), "GetAdcpSubsystemConfig() 2 1  is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig30), "GetAdcpSubsystemConfig() 3 0  is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig31), "GetAdcpSubsystemConfig() 3 1  is incorrect.");
        }

        /// <summary>
        /// Test the AddConfiguration() method.
        /// 
        /// Add bad Subsystem.
        /// </summary>
        public void AddConfiguration3()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01200000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("22", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 1, 1);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 0, 0);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 1

            AdcpSubsystemConfig asConfig1 = null;
            bool addConfigResult = config.AddConfiguration(ss3, out asConfig1);      // Add another configuration for Subsystem 2

            Assert.IsTrue(addConfigResult, "AddConfiguration() is incorrect.");
            Assert.IsNotNull(asConfig1, "asConfig1 is incorrect.");
            Assert.AreEqual(ss3, asConfig1.SubsystemConfig.SubSystem, "asConfig1 Subsystem is incorrect.");
            Assert.AreEqual(2, asConfig1.SubsystemConfig.CepoIndex, "asConfig1 CEPO index is incorrect.");
            Assert.AreEqual(ssConfig31, asConfig1.SubsystemConfig, "asConfig1 SubsystemConfiguration is incorrect.");
            Assert.IsNotNull(asConfig1.Commands, "asConfig1 Commands is incorrect.");
            Assert.AreEqual(ss3, asConfig1.Commands.SubsystemConfig.SubSystem, "asConfig1 Commands Subsystem is incorrect.");
            Assert.AreEqual(2, asConfig1.Commands.SubsystemConfig.CepoIndex, "asConfig1 Commands CEPO index is incorrect.");

            Assert.AreEqual("22", config.Commands.CEPO, "Commands CEPO is incorrect.");
            Assert.AreEqual(2, config.SubsystemConfigDict.Count, "SubsystemConfigDict Count is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig20), "AdcpSubsystemConfigExist() 2 0 is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig21), "AdcpSubsystemConfigExist() 2 1 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig30), "AdcpSubsystemConfigExist() 3 0 is incorrect.");
            Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig31), "AdcpSubsystemConfigExist() 3 1 is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig20), "GetAdcpSubsystemConfig() 2 0  is incorrect.");
            Assert.NotNull(config.GetAdcpSubsystemConfig(ssConfig21), "GetAdcpSubsystemConfig() 2 1  is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig30), "GetAdcpSubsystemConfig() 3 0  is incorrect.");
            Assert.IsNull(config.GetAdcpSubsystemConfig(ssConfig31), "GetAdcpSubsystemConfig() 3 1  is incorrect.");
        }

        #endregion

        #region RemoveConfiguration
        
        /// <summary>
        /// Test removing a configuration.
        /// </summary>
        [Test]
        public void TestRemoveConfiguration()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01230000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("233232", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 3, 3);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig22 = new SubsystemConfiguration(ss2, 5, 5);         // Configuration SS2 with Config Number 2
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 2, 2);         // Configuration SS3 with Config Number 1
            SubsystemConfiguration ssConfig32 = new SubsystemConfiguration(ss3, 4, 4);         // Configuration SS3 with Config Number 2

            AdcpSubsystemConfig asConfig3_0 = config.GetAdcpSubsystemConfig(ssConfig30);        // Get the Subsystem 3 Configuration 0
            AdcpSubsystemConfig asConfig3_2 = config.GetAdcpSubsystemConfig(ssConfig32);        // Get the Subsystem 3 Configuration 2

            // Verify 3 configuration exist for Subsystem 3 and Subsystem 2
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig20), "Config Exist 2_0 True is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig21), "Config Exist 2_1 True is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig22), "Config Exist 2_2 True is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig30), "Config Exist 3_0 True is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig31), "Config Exist 3_1 True is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig32), "Config Exist 3_2 True is incorrect.");
            Assert.AreEqual(6, config.SubsystemConfigDict.Count, "SubsystemConfigDict pre count is incorrect.");
            Assert.AreEqual(0, config.GetAdcpSubsystemConfig(ssConfig20).SubsystemConfig.CepoIndex, "CEPO index 2_0 pre is incorrect.");
            Assert.AreEqual(0, config.GetAdcpSubsystemConfig(ssConfig20).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 2_0 pre is incorrect.");
            Assert.AreEqual(1, config.GetAdcpSubsystemConfig(ssConfig30).SubsystemConfig.CepoIndex, "CEPO index 3_0 pre is incorrect.");
            Assert.AreEqual(1, config.GetAdcpSubsystemConfig(ssConfig30).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 3_0 pre is incorrect.");
            Assert.AreEqual(2, config.GetAdcpSubsystemConfig(ssConfig31).SubsystemConfig.CepoIndex, "CEPO index 3_1 pre is incorrect.");
            Assert.AreEqual(2, config.GetAdcpSubsystemConfig(ssConfig31).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 3_1 pre is incorrect.");
            Assert.AreEqual(3, config.GetAdcpSubsystemConfig(ssConfig21).SubsystemConfig.CepoIndex, "CEPO index 2_1 pre is incorrect.");
            Assert.AreEqual(3, config.GetAdcpSubsystemConfig(ssConfig21).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 2_1 pre is incorrect.");
            Assert.AreEqual(4, config.GetAdcpSubsystemConfig(ssConfig32).SubsystemConfig.CepoIndex, "CEPO index 3_2 pre is incorrect.");
            Assert.AreEqual(4, config.GetAdcpSubsystemConfig(ssConfig32).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 3_2 pre is incorrect.");
            Assert.AreEqual(5, config.GetAdcpSubsystemConfig(ssConfig22).SubsystemConfig.CepoIndex, "CEPO index 2_2 pre is incorrect.");
            Assert.AreEqual(5, config.GetAdcpSubsystemConfig(ssConfig22).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 2_2 pre is incorrect.");


            bool resultRemove = config.RemoveAdcpSubsystemConfig(asConfig3_0);                                                      // Remove the first Subsystem 3 Configuration

            Assert.AreEqual(true, resultRemove, "ResultRemove is incorrect.");
            Assert.AreEqual("23232", config.Commands.CEPO, "Commands CEPO is incorrect.");
            //Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig20), "Config Exist 2_0 True 1 is incorrect.");
            //Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig21), "Config Exist 2_1 True 1 is incorrect.");
            //Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig22), "Config Exist 2_2 True 1 is incorrect.");
            //Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig30), "Config Exist 3_0 True 1 is incorrect.");
            //Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig31), "Config Exist 3_1 True 1 is incorrect.");
            //Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig32), "Config Exist 3_2 False 1 is incorrect.");
            //Assert.AreEqual(5, config.SubsystemConfigDict.Count, "SubsystemConfigDict post count is incorrect.");
            //Assert.AreEqual(0, config.GetAdcpSubsystemConfig(ssConfig20).SubsystemConfig.CepoIndex, "CEPO index 2_0 is incorrect.");
            //Assert.AreEqual(0, config.GetAdcpSubsystemConfig(ssConfig20).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 2_0 pre is incorrect.");
            //Assert.AreEqual(1, config.GetAdcpSubsystemConfig(ssConfig30).SubsystemConfig.CepoIndex, "CEPO index 3_0 is incorrect.");
            //Assert.AreEqual(1, config.GetAdcpSubsystemConfig(ssConfig30).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 3_0 pre is incorrect.");
            //Assert.AreEqual(2, config.GetAdcpSubsystemConfig(ssConfig21).SubsystemConfig.CepoIndex, "CEPO index 2_1 is incorrect.");
            //Assert.AreEqual(2, config.GetAdcpSubsystemConfig(ssConfig21).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 2_1 pre is incorrect.");
            //Assert.AreEqual(3, config.GetAdcpSubsystemConfig(ssConfig31).SubsystemConfig.CepoIndex, "CEPO index 3_1 is incorrect.");
            //Assert.AreEqual(3, config.GetAdcpSubsystemConfig(ssConfig31).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 3_1 pre is incorrect.");
            //Assert.AreEqual(4, config.GetAdcpSubsystemConfig(ssConfig22).SubsystemConfig.CepoIndex, "CEPO index 2_2 is incorrect.");
            //Assert.AreEqual(4, config.GetAdcpSubsystemConfig(ssConfig22).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 2_2 pre is incorrect.");
        }

        /// <summary>
        /// Test removing a bad Configuration.
        /// </summary>
        [Test]
        public void TestRemoveConfigurationBad()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01230000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("233", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            //SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 1, 1);         // Configuration SS2 with Config Number 1
            //SubsystemConfiguration ssConfig22 = new SubsystemConfiguration(ss2, 2, 2);         // Configuration SS2 with Config Number 2
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 2, 2);         // Configuration SS3 with Config Number 1
            SubsystemConfiguration ssConfig32 = new SubsystemConfiguration(ss3, 3, 3);         // Configuration SS3 with Config Number 2

            AdcpSubsystemConfig asConfig3_2 = config.GetAdcpSubsystemConfig(ssConfig32);        // Get the Subsystem 3 Configuration 0

            bool resultRemove = config.RemoveAdcpSubsystemConfig(asConfig3_2);

            Assert.IsNull(asConfig3_2, "Config 3_2 is incorrect.");
            Assert.AreEqual(false, resultRemove, "ResultRemove is incorrect.");
            Assert.AreEqual("233", config.Commands.CEPO, "Commands CEPO is incorrect.");
        }

        /// <summary>
        /// Test removing multiple configuration.
        /// </summary>
        [Test]
        public void TestRemoveConfigurationMultiple()
        {
            AdcpConfiguration config = new AdcpConfiguration();
            SerialNumber serial = new SerialNumber("01230000000000000000000000000004");
            Dictionary<string, AdcpSubsystemConfig> result = config.SetCepo("233232", serial);

            Subsystem ss2 = new Subsystem("2", 0);                                   // Subsystem code and Index within serial number
            Subsystem ss3 = new Subsystem("3", 1);                                   // Subsystem code and Index within serial number
            SubsystemConfiguration ssConfig20 = new SubsystemConfiguration(ss2, 0, 0);         // Configuration SS2 with Config Number 0
            SubsystemConfiguration ssConfig21 = new SubsystemConfiguration(ss2, 3, 3);         // Configuration SS2 with Config Number 1
            SubsystemConfiguration ssConfig22 = new SubsystemConfiguration(ss2, 5, 5);         // Configuration SS2 with Config Number 2
            SubsystemConfiguration ssConfig30 = new SubsystemConfiguration(ss3, 1, 1);         // Configuration SS3 with Config Number 0
            SubsystemConfiguration ssConfig31 = new SubsystemConfiguration(ss3, 2, 2);         // Configuration SS3 with Config Number 1
            SubsystemConfiguration ssConfig32 = new SubsystemConfiguration(ss3, 4, 4);         // Configuration SS3 with Config Number 2

            AdcpSubsystemConfig asConfig3_0 = config.GetAdcpSubsystemConfig(ssConfig30);        // Get the Subsystem 3 Configuration 0
            AdcpSubsystemConfig asConfig3_1 = config.GetAdcpSubsystemConfig(ssConfig31);        // Get the Subsystem 3 Configuration 0
            AdcpSubsystemConfig asConfig3_2 = config.GetAdcpSubsystemConfig(ssConfig32);        // Get the Subsystem 3 Configuration 2

            // Verify 3 configuration exist for Subsystem 3 and Subsystem 2
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig20), "Config Exist 2_0 True is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig21), "Config Exist 2_1 True is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig22), "Config Exist 2_2 True is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig30), "Config Exist 3_0 True is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig31), "Config Exist 3_1 True is incorrect.");
            Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig32), "Config Exist 3_2 True is incorrect.");
            Assert.AreEqual(6, config.SubsystemConfigDict.Count, "SubsystemConfigDict pre count is incorrect.");
            Assert.AreEqual(0, config.GetAdcpSubsystemConfig(ssConfig20).SubsystemConfig.CepoIndex, "CEPO index 2_0 pre is incorrect.");
            Assert.AreEqual(0, config.GetAdcpSubsystemConfig(ssConfig20).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 2_0 pre is incorrect.");
            Assert.AreEqual(1, config.GetAdcpSubsystemConfig(ssConfig30).SubsystemConfig.CepoIndex, "CEPO index 3_0 pre is incorrect.");
            Assert.AreEqual(1, config.GetAdcpSubsystemConfig(ssConfig30).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 3_0 pre is incorrect.");
            Assert.AreEqual(2, config.GetAdcpSubsystemConfig(ssConfig31).SubsystemConfig.CepoIndex, "CEPO index 3_1 pre is incorrect.");
            Assert.AreEqual(2, config.GetAdcpSubsystemConfig(ssConfig31).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 3_1 pre is incorrect.");
            Assert.AreEqual(3, config.GetAdcpSubsystemConfig(ssConfig21).SubsystemConfig.CepoIndex, "CEPO index 2_1 pre is incorrect.");
            Assert.AreEqual(3, config.GetAdcpSubsystemConfig(ssConfig21).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 2_1 pre is incorrect.");
            Assert.AreEqual(4, config.GetAdcpSubsystemConfig(ssConfig32).SubsystemConfig.CepoIndex, "CEPO index 3_2 pre is incorrect.");
            Assert.AreEqual(4, config.GetAdcpSubsystemConfig(ssConfig32).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 3_2 pre is incorrect.");
            Assert.AreEqual(5, config.GetAdcpSubsystemConfig(ssConfig22).SubsystemConfig.CepoIndex, "CEPO index 2_2 pre is incorrect.");
            Assert.AreEqual(5, config.GetAdcpSubsystemConfig(ssConfig22).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 2_2 pre is incorrect.");


            //bool resultRemove0 = config.RemoveAdcpSubsystemConfig(asConfig3_0);                                                      // Remove the first Subsystem 3 Configuration
            //bool resultRemove1 = config.RemoveAdcpSubsystemConfig(asConfig3_1);                                                      // Remove the first Subsystem 3 Configuration
            //bool resultRemove2 = config.RemoveAdcpSubsystemConfig(asConfig3_2);                                                      // Remove the first Subsystem 3 Configuration

            //Assert.AreEqual(true, resultRemove0, "ResultRemove is incorrect.");
            //Assert.AreEqual(true, resultRemove1, "ResultRemove is incorrect.");
            //Assert.AreEqual(true, resultRemove2, "ResultRemove is incorrect.");
            //Assert.AreEqual("222", config.Commands.CEPO, "Commands CEPO is incorrect.");
            //Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig20), "Config Exist 2_0 True 1 is incorrect.");
            //Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig21), "Config Exist 2_1 True 1 is incorrect.");
            //Assert.AreEqual(true, config.AdcpSubsystemConfigExist(ssConfig22), "Config Exist 2_2 True 1 is incorrect.");
            //Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig30), "Config Exist 3_0 True 1 is incorrect.");
            //Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig31), "Config Exist 3_1 True 1 is incorrect.");
            //Assert.AreEqual(false, config.AdcpSubsystemConfigExist(ssConfig32), "Config Exist 3_2 False 1 is incorrect.");
            //Assert.AreEqual(3, config.SubsystemConfigDict.Count, "SubsystemConfigDict post count is incorrect.");
            //Assert.AreEqual(0, config.GetAdcpSubsystemConfig(ssConfig20).SubsystemConfig.CepoIndex, "CEPO index 2_0 is incorrect.");
            //Assert.AreEqual(0, config.GetAdcpSubsystemConfig(ssConfig20).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 2_0 is incorrect.");
            //Assert.AreEqual(1, config.GetAdcpSubsystemConfig(ssConfig21).SubsystemConfig.CepoIndex, "CEPO index 2_1 is incorrect.");
            //Assert.AreEqual(1, config.GetAdcpSubsystemConfig(ssConfig21).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 2_1 is incorrect.");
            //Assert.AreEqual(2, config.GetAdcpSubsystemConfig(ssConfig22).SubsystemConfig.CepoIndex, "CEPO index 2_2 is incorrect.");
            //Assert.AreEqual(2, config.GetAdcpSubsystemConfig(ssConfig22).Commands.SubsystemConfig.CepoIndex, "Commands CEPO index 2_2 is incorrect.");
        }

        #endregion

        #region Equal

        /// <summary>
        /// Test the equals sign.
        /// </summary>
        [Test]
        public void Equal()
        {
            AdcpConfiguration config = new AdcpConfiguration();

            AdcpConfiguration config1 = new AdcpConfiguration();
            config1.Commands.C232B = Baudrate.BAUD_38400;

            config = config1;

            Assert.AreEqual(config.Commands.C232B, config1.Commands.C232B, "RS232B is incorrect.");


        }

        #endregion
    }
}
