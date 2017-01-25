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
 * 01/25/2012      RC          1.14       Initial coding
 * 07/11/2012      RC          2.12       Added a test for a bad string for the serial number.
 * 10/01/2012      RC          2.15       Added a test to serialize/deserialize to JSON.
 * 01/14/2013      RC          2.17       Added test for ConvertSubsystemCode.
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Collections.Generic;

    /// <summary>
    /// Unit test of the SerialNumber object.
    /// This will test the SerialNumber object
    /// and the Subsystem object.
    /// </summary>
    [TestFixture]
    public class SubsystemTest
    {
        /// <summary>
        /// Test the equals for the SubsystemCodeDesc.
        /// </summary>
        [Test]
        public void TestSubsystemCodeDescEqual()
        {
            RTI.SubsystemList.SubsystemCodeDesc scd1 = new RTI.SubsystemList.SubsystemCodeDesc(Subsystem.SUB_2MHZ_4BEAM_20DEG_PISTON_1, Subsystem.DescString(Subsystem.SUB_2MHZ_4BEAM_20DEG_PISTON_1));
            RTI.SubsystemList.SubsystemCodeDesc scd1_ = new RTI.SubsystemList.SubsystemCodeDesc(Subsystem.SUB_2MHZ_4BEAM_20DEG_PISTON_1, Subsystem.DescString(Subsystem.SUB_2MHZ_4BEAM_20DEG_PISTON_1));
            RTI.SubsystemList.SubsystemCodeDesc scd2 = new RTI.SubsystemList.SubsystemCodeDesc(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2, Subsystem.DescString(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2));

            Assert.IsTrue(scd1 == scd1_, "SubsystemCodeDesc Equal is incorrect.");
            Assert.IsTrue(scd1 != scd2, "SubsystemCodeDesc Not Equal is incorrect.");

        }

        #region Convert SubsystemCode

        /// <summary>
        /// Test converting the subsystem code using ConvertStringToSubsystemCode().
        /// </summary>
        [Test]
        public void TestConvertStringToSubsystemCode()
        {
            // Create a serial number
            string serialStr = "01200000000000000000000000000001";
            SerialNumber serialNum = new SerialNumber(serialStr);

            byte subsystemCode = Subsystem.ConvertSubsystemCode(serialNum.SubsystemsString(), 0);

            Assert.AreEqual(50, subsystemCode, "Subsystem Code value is incorrect.");
            Assert.AreEqual(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2, subsystemCode, "Subsystem Code is incorrect.");

        }


        /// <summary>
        /// Test converting the subsystem code using ConvertStringToSubsystemCode().
        /// </summary>
        [Test]
        public void TestConvertCharToSubsystemCode()
        {
            // Create a serial number
            char value = '2';

            byte subsystemCode = Subsystem.ConvertSubsystemCode(value);

            Assert.AreEqual(50, subsystemCode, "Subsystem Code value is incorrect.");
            Assert.AreEqual(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2, subsystemCode, "Subsystem Code is incorrect.");

        }

        #endregion

    }
}
