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
 * 11/21/2011      RC                     Initial coding
 * 02/07/2012      RC          2.00       Updated to latest AdcpCommand and AdcpSubsystemCommand.
 * 
 */
namespace RTI
{
    using System;
    using NUnit.Framework;
    using RTI.Commands;

    /// <summary>
    /// Create test to test the AdcpCommand object.
    /// </summary>
    [TestFixture]
    public class AdcpCommandsTest
    {

        /// <summary>
        /// Test the constructor.
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            SerialNumber serialNum = new SerialNumber("01400000000000000000000000000015");
            AdcpCommands commands = new AdcpCommands(serialNum);

            Commands.TimeValue tv = new TimeValue();
            Commands.TimeValue tv1 = new Commands.TimeValue(0, 0, 1, 0); 

            // Verify default values
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_MODE, commands.Mode);

            // Ensemble defaults
            Assert.AreEqual(tv1, commands.CEI);
            Assert.AreEqual(tv1.Hour, commands.CEI_Hour);
            Assert.AreEqual(tv1.Minute, commands.CEI_Minute);
            Assert.AreEqual(tv1.Second, commands.CEI_Second);
            Assert.AreEqual(tv1.HunSec, commands.CEI_HunSec);

            // Environmental defaults
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_CWS, commands.CWS);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_CWT, commands.CWT);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_CTD, commands.CTD);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_CWSS, commands.CWSS);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_CHO, commands.CHO);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_CHS, commands.CHS);

            // Serial Comm defaults
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_C232B, commands.C232B);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_C485B, commands.C485B);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_C422B, commands.C422B);

            // Get the first subsystem (Only subsystem)
            Subsystem ss = serialNum.SubSystemsDict[0];
            AdcpSubsystemCommands asc = new AdcpSubsystemCommands(ss);

            // Water Profile Defaults
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CWPON, asc.CWPON);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CWPBB, asc.CWPBB);
            //Assert.AreEqual(tv, asc.CWPAI);
            Assert.AreEqual(tv.Hour, asc.CWPAI.Hour);
            Assert.AreEqual(tv.Minute, asc.CWPAI.Minute);
            Assert.AreEqual(tv.Second, asc.CWPAI.Second);
            Assert.AreEqual(tv.HunSec, asc.CWPAI.HunSec);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPBL, asc.CWPBL);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPBS, asc.CWPBS);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPX, asc.CWPX);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPBN, asc.CWPBN);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPP, asc.CWPP);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPTBP, asc.CWPTBP);

            // Bottom Track defaults
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CBTON, asc.CBTON);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CBTBB, asc.CBTBB);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CBTBL, asc.CBTBL);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CBTMX, asc.CBTMX);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CBTTBP, asc.CBTTBP);

            // Water Track defaults
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CWTON, asc.CWTON);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CWTBB, asc.CWTBB);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWTBL, asc.CWTBL);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWTBS, asc.CWTBS);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWTTBP, asc.CWTTBP);
        }


    }
}
