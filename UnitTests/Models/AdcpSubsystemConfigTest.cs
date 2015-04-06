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
 * 10/12/2012      RC          2.15       Initial coding
 * 12/03/2012      RC          2.17       Updated AdcpSubsystemConfig.ToString().
 * 12/28/2012      RC          2.17       Moved AdcpSubsystemConfig.Subsystem into AdcpSubsystemConfig.SubsystemConfig.Subsystem.
 *                                         Made SubsystemConfiguration take a Subsystem in its constructor.
 * 09/11/2013      RC          2.19.5     Updated test to 2.19.5
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
    /// UnitTest for the AdcpSubsystemConfig object.
    /// </summary>
    public class AdcpSubsystemConfigTest
    {

        /// <summary>
        /// Test the Constructor.
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2, 0);         // 1200kHz
            SubsystemConfiguration ssConfig = new SubsystemConfiguration(ss, 0, 0);                    // 0 Config
            int cepoIndex = 0;
            AdcpSubsystemConfig asConfig = new AdcpSubsystemConfig(ssConfig);
            string asConfigStr = AdcpSubsystemConfig.GetString(ssConfig);


            Assert.AreEqual(asConfig.SubsystemConfig.CepoIndex, cepoIndex, "CepoIndex is incorrect.");
            //Assert.AreEqual(asConfig.Commands.CepoIndex, cepoIndex, "Commands CEPO index is incorrect.");
            Assert.AreEqual(asConfig.SubsystemConfig.SubSystem, ss, "Subsystem is incorrect.");
            Assert.AreEqual(asConfig.SubsystemConfig, ssConfig, "SubsystemConfiguration is incorrect.");
            Assert.AreEqual(asConfig.ToString(), "[0] 1.2 MHz 4 beam 20 degree piston", "ToString is incorrect.");
            Assert.AreEqual(asConfigStr, "[0] 1.2 MHz 4 beam 20 degree piston", "GetString is incorrect.");
        }


        #region JSON

        /// <summary>
        /// Test creating a JSON and converting back to object.
        /// </summary>
        [Test]
        public void TestJSON()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2, 0);         // 1200kHz
            SubsystemConfiguration ssConfig = new SubsystemConfiguration(ss, 0, 0);                    // 0 Config
            int cepoIndex = 0;
            AdcpSubsystemConfig asConfig = new AdcpSubsystemConfig(ssConfig);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(asConfig);                                        // Serialize object to JSON
            AdcpSubsystemConfig newConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AdcpSubsystemConfig>(json);   // Deserialize the JSON

            Assert.AreEqual(asConfig.SubsystemConfig.CepoIndex, cepoIndex, "CepoIndex is incorrect.");
            //Assert.AreEqual(asConfig.Commands.CepoIndex, cepoIndex, "Commands CEPO index is incorrect.");
            Assert.AreEqual(asConfig.SubsystemConfig.SubSystem, ss, "Subsystem is incorrect.");
            Assert.AreEqual(asConfig.SubsystemConfig, ssConfig, "SubsystemConfiguration is incorrect.");
            Assert.AreEqual(asConfig.ToString(), "[0] 1.2 MHz 4 beam 20 degree piston", "ToString is incorrect.");

            Assert.AreEqual(newConfig.SubsystemConfig.CepoIndex, cepoIndex, "CepoIndex is incorrect.");
            //Assert.AreEqual(newConfig.Commands.CepoIndex, cepoIndex, "Commands CEPO index is incorrect.");
            Assert.AreEqual(newConfig.SubsystemConfig.SubSystem, ss, "Subsystem is incorrect.");
            Assert.AreEqual(newConfig.SubsystemConfig, ssConfig, "SubsystemConfiguration is incorrect.");
            Assert.AreEqual(newConfig.ToString(), "[0] 1.2 MHz 4 beam 20 degree piston", "ToString is incorrect.");
        }

        /// <summary>
        /// Test creating a JSON and converting back to object.
        /// </summary>
        [Test]
        public void TestJSON1()
        {
            Subsystem ss = new Subsystem(Subsystem.SUB_1_2MHZ_VERT_PISTON_A, 0);                // 1200kHz
            SubsystemConfiguration ssConfig = new SubsystemConfiguration(ss, 3, 3);                    // 3 Config
            int cepoIndex = 3;
            AdcpSubsystemConfig asConfig = new AdcpSubsystemConfig(ssConfig);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(asConfig);                                        // Serialize object to JSON
            AdcpSubsystemConfig newConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AdcpSubsystemConfig>(json);   // Deserialize the JSON

            Assert.AreEqual(asConfig.SubsystemConfig.CepoIndex, cepoIndex, "CepoIndex is incorrect.");
            Assert.AreEqual(asConfig.SubsystemConfig.SubSystem, ss, "Subsystem is incorrect.");
            Assert.AreEqual(asConfig.SubsystemConfig, ssConfig, "SubsystemConfiguration is incorrect.");
            Assert.AreEqual(asConfig.ToString(), "[3] 1.2 MHz vertical beam piston", "ToString is incorrect.");

            Assert.AreEqual(newConfig.SubsystemConfig.CepoIndex, cepoIndex, "CepoIndex is incorrect.");
            Assert.AreEqual(newConfig.SubsystemConfig.SubSystem, ss, "Subsystem is incorrect.");
            Assert.AreEqual(newConfig.SubsystemConfig, ssConfig, "SubsystemConfiguration is incorrect.");
            Assert.AreEqual(newConfig.ToString(), "[3] 1.2 MHz vertical beam piston", "ToString is incorrect.");
        }

        #endregion
    }
}
