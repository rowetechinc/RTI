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
 * 07/11/2012      RC          2.12       Updated Decode BREAK to pass a firmware object instead of a string.
 * 07/19/2012      RC          2.12       Added test for decoding ENGI2CSHOW and STIME.
 * 
 * 
 */
namespace RTI
{
    using System;
    using NUnit.Framework;
    using RTI.Commands;
using System.Text;

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

        /// <summary>
        /// Test the DecodeBREAK() function.
        /// </summary>
        [Test]
        public void TestDecodeBreak()
        {
            string brk = "Copyright (c) 2009-2012 Rowe Technologies Inc. All rights reserved. \r\nDP300 DP1200 \r\nSN: 01460000000000000000000000000001 \r\nFW: 00.02.05 Apr 17 2012 05:40:11";

            BreakStmt brkStmt = RTI.Commands.AdcpCommands.DecodeBREAK(brk);

            Assert.AreEqual(1, brkStmt.SerialNum.SystemSerialNumber, "Serial Number is incorrect");
            Assert.AreEqual("01460000000000000000000000000001", brkStmt.SerialNum.ToString(), "Serial Number String is Incorrect");
            Assert.AreEqual(2, brkStmt.SerialNum.SubSystemsDict.Count, "Subsystem count is incorrect");
            Assert.AreEqual(0, brkStmt.FirmwareVersion.FirmwareMajor, string.Format("Firmware Major is incorrect", 0, brkStmt.FirmwareVersion.FirmwareMajor));
            Assert.AreEqual(2, brkStmt.FirmwareVersion.FirmwareMinor, string.Format("Firmware Minor is incorrect", 2, brkStmt.FirmwareVersion.FirmwareMinor));
            Assert.AreEqual(5, brkStmt.FirmwareVersion.FirmwareRevision, string.Format("Firmware Revision is incorrect", 5, brkStmt.FirmwareVersion.FirmwareRevision));
            Assert.AreEqual("DP300 DP1200", brkStmt.Hardware, "Hardware is Incorrect");
        }

        /// <summary>
        /// Test the Decode ENGPNI command.
        /// </summary>
        [Test]
        public void TestDecodeEngPni()
        {
            string buffer = "H=0.00, P=0.00, R=0.00";
            HPR result = RTI.Commands.AdcpCommands.DecodeEngPniResult(buffer);

            Assert.AreEqual(0, result.Heading, "Heading is incorrect");
            Assert.AreEqual(0, result.Pitch, "Pitch is incorrect");
            Assert.AreEqual(0, result.Roll, "Roll is incorrect");
        }

        /// <summary>
        /// Test the Decode ENGPNI command with data.
        /// </summary>
        [Test]
        public void TestDecodeEngPniData()
        {
            string buffer = "H=179.235, P=56.23, R=-2.04";
            HPR result = RTI.Commands.AdcpCommands.DecodeEngPniResult(buffer);

            Assert.AreEqual(179.235, result.Heading, "Heading is incorrect");
            Assert.AreEqual(56.23, result.Pitch, "Pitch is incorrect");
            Assert.AreEqual(-2.04, result.Roll, "Roll is incorrect");
        }

        /// <summary>
        /// Test the command that decodes the ENGI2CSHOW output.
        /// This will give the registers values for the RTC and Rcvr.
        /// It will also give the board revision and serial number.
        /// </summary>
        [Test]
        public void TestDecodeENGI2CSHOW()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("I2C Device,Address, Data\n");
            builder.Append("RVCR,      00, --\n");
            builder.Append("RVCR,      01, 01 07 00 00 04 0F 40 A0 7F 00 00 20 20 00 40 00 60 00 50 03 03 00 10 00 00 27 27\n");
            builder.Append("RVCR,      02, --\n");
            builder.Append("RVCR,      03, --\n");
            builder.Append("CAT9534,      20, --\n");
            builder.Append("CAT9534,      21, --\n");
            builder.Append("CAT9534,      22, C9\n");
            builder.Append("CAT9534,      23, --\n");
            builder.Append("CAT9534,      24, --\n");
            builder.Append("CAT9534,      25, --\n");
            builder.Append("CAT9534,      26, --\n");
            builder.Append("CAT9534,      27, --\n");
            builder.Append("AD7991-0,      28, 08 24 17 C8 20 00 30 01 \n");
            builder.Append("AD7991-1,      29, 00 00 10 00 20 00 30 00 \n");
            builder.Append("24AA32AF,     50, 50007  REV:XD1  SER#010\n");
            builder.Append("24AA32AF,     51, 50012  REV:XD1  SER#010\n");
            builder.Append("24AA32AF,     52, 50009  REV:XD1  SER#010\n");
            builder.Append("24AA32AF,     53, 50018  REV:XD1  SER#010   50022  REV:XA  SER#0XX\n");
            builder.Append("24AA32AF,     54, 50016  REV:XC1  SER#013\n");
            builder.Append("24AA32AF,     55, --\n");
            builder.Append("24AA32AF,     56, --\n");
            builder.Append("24AA32AF,     57, --\n");
            builder.Append("DS3231,      68, 14 57 08 02 19 07 12 00 00 00 00 00 00 00 00 00 00 2F C0 \n");

            I2cMemDevs devs = RTI.Commands.AdcpCommands.DecodeENGI2CSHOW(builder.ToString());

            #region RTC Registers
            Assert.AreEqual(1, devs.RtcRegs.Count, "RTC register count is incorrect");
            Assert.AreEqual(19, devs.RtcRegs[0].RegValues.Count, "RTC register number of elements is incorrect");
            Assert.AreEqual(104, devs.RtcRegs[0].ID, string.Format("Register ID for RTC is incorrect Found: {0}", devs.RtcRegs[0].ID));        // 0x68 =  104
            Assert.AreEqual(Convert.ToInt32("68", 16), devs.RtcRegs[0].ID, "Register ID for RTC is incorrect");        // 0x68 =  104
            Assert.AreEqual(Convert.ToInt32("14", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[0], 16), "Register Value 0 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("57", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[1], 16), "Register Value 1 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("08", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[2], 16), "Register Value 2 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("02", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[3], 16), "Register Value 3 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("19", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[4], 16), "Register Value 4 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("07", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[5], 16), "Register Value 5 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("12", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[6], 16), "Register Value 6 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[7], 16), "Register Value 7 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[8], 16), "Register Value 8 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[9], 16), "Register Value 9 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[10], 16), "Register Value 10 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[11], 16), "Register Value 11 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[12], 16), "Register Value 12 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[13], 16), "Register Value 13 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[14], 16), "Register Value 14 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[15], 16), "Register Value 15 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[16], 16), "Register Value 16 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("2F", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[17], 16), "Register Value 17 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("C0", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[18], 16), "Register Value 18 for RTC is incorrect");
            #endregion

            #region Rcvr Register
            Assert.AreEqual(1, devs.RvcrRegs.Count, "Rcvr register count is incorrect");
            Assert.AreEqual(27, devs.RvcrRegs[0].RegValues.Count, "Rcvr register number of elements is incorrect");
            Assert.AreEqual(1, devs.RvcrRegs[0].ID, "Register ID for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("01", 16), devs.RvcrRegs[0].ID, "Register ID for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("01", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[0], 16), "Register Value 0 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("07", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[1], 16), "Register Value 1 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[2], 16), "Register Value 2 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[3], 16), "Register Value 3 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("04", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[4], 16), "Register Value 4 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("0F", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[5], 16), "Register Value 5 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("40", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[6], 16), "Register Value 6 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("A0", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[7], 16), "Register Value 7 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("7F", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[8], 16), "Register Value 8 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[9], 16), "Register Value 9 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[10], 16), "Register Value 10 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("20", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[11], 16), "Register Value 11 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("20", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[12], 16), "Register Value 12 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[13], 16), "Register Value 13 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("40", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[14], 16), "Register Value 14 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[15], 16), "Register Value 15 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("60", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[16], 16), "Register Value 16 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[17], 16), "Register Value 17 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("50", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[18], 16), "Register Value 18 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("03", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[19], 16), "Register Value 19 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("03", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[20], 16), "Register Value 20 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[21], 16), "Register Value 21 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("10", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[22], 16), "Register Value 22 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[23], 16), "Register Value 23 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[24], 16), "Register Value 24 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("27", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[25], 16), "Register Value 25 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("27", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[26], 16), "Register Value 26 for Rcvr is incorrect");
            #endregion

            #region Board Revs and Serial

            Assert.AreEqual(I2cBoardId.IO, devs.IoBoard.ID, "I/O Board ID is incorrect.");
            Assert.AreEqual(50007, (long)devs.IoBoard.ID, "I/O Board ID value is incorrect.");
            Assert.AreEqual(10, devs.IoBoard.SerialNum, "I/O Board serial number is incorrect.");
            Assert.AreEqual("XD1", devs.IoBoard.Revision, "I/O Board rev is incorrect.");

            Assert.AreEqual(I2cBoardId.LOW_PWR_REG, devs.LowPwrRegBoard.ID, "Low Pwr Reg Board ID is incorrect.");
            Assert.AreEqual(50012, (long)devs.LowPwrRegBoard.ID, "Low Pwr Reg Board ID value is incorrect.");
            Assert.AreEqual(10, devs.LowPwrRegBoard.SerialNum, "Low Pwr Reg Board serial number is incorrect.");
            Assert.AreEqual("XD1", devs.LowPwrRegBoard.Revision, "Low Pwr Reg Board rev is incorrect.");

            Assert.AreEqual(I2cBoardId.XMITTER, devs.XmitterBoard.ID, "Xmitter Board ID is incorrect.");
            Assert.AreEqual(50009, (long)devs.XmitterBoard.ID, "Xmitter Board ID value is incorrect.");
            Assert.AreEqual(10, devs.XmitterBoard.SerialNum, "Xmitter Board serial number is incorrect.");
            Assert.AreEqual("XD1", devs.XmitterBoard.Revision, "Xmitter Board rev is incorrect.");

            Assert.AreEqual(I2cBoardId.VIRTUAL_GND, devs.VirtualGndBoard.ID, "Virtual Gnd Board ID is incorrect.");
            Assert.AreEqual(50018, (long)devs.VirtualGndBoard.ID, "Virtual Gnd Board ID value is incorrect.");
            Assert.AreEqual(10, devs.VirtualGndBoard.SerialNum, "Virtual Gnd Board serial number is incorrect.");
            Assert.AreEqual("XD1", devs.VirtualGndBoard.Revision, "Virtual Gnd Board rev is incorrect.");

            Assert.AreEqual(I2cBoardId.RCVR, devs.RcvrBoard.ID, "Rcvr Board ID is incorrect.");
            Assert.AreEqual(50022, (long)devs.RcvrBoard.ID, "Rcvr Board ID value is incorrect.");
            Assert.AreEqual(0, devs.RcvrBoard.SerialNum, "Rcvr Board serial number is incorrect.");
            Assert.AreEqual("XA", devs.RcvrBoard.Revision, "Rcvr Board rev is incorrect.");

            Assert.AreEqual(I2cBoardId.BACKPLANE, devs.BackPlaneBoard.ID, "Backplane Board ID is incorrect.");
            Assert.AreEqual(50016, (long)devs.BackPlaneBoard.ID, "Backplane Board ID value is incorrect.");
            Assert.AreEqual(13, devs.BackPlaneBoard.SerialNum, "Backplane Board serial number is incorrect.");
            Assert.AreEqual("XC1", devs.BackPlaneBoard.Revision, "Backplane Board rev is incorrect.");

            #endregion

        }

        /// <summary>
        /// Test setting a bad serial number for the I2C board.
        /// </summary>
        [Test]
        public void TestI2cBoardSerialBad()
        {
            I2cBoard brd = new I2cBoard();
            brd.SetSerial("0xxx");

            Assert.AreEqual(0, brd.SerialNum, "Serial number is incorrect.");
            Assert.AreEqual("", brd.Revision, "Revision is incorrect.");
            Assert.AreEqual(I2cBoardId.UNKNOWN, brd.ID, "Board ID is incorrect.");
        }

        /// <summary>
        /// Test setting a serial number for the I2C board.
        /// </summary>
        [Test]
        public void TestI2cBoardSerial()
        {
            I2cBoard brd = new I2cBoard();
            brd.SetSerial("011");

            Assert.AreEqual(11, brd.SerialNum);
            Assert.AreEqual("", brd.Revision, "Revision is incorrect.");
            Assert.AreEqual(I2cBoardId.UNKNOWN, brd.ID, "Board ID is incorrect.");
        }

        /// <summary>
        /// Test the command that decodes the STIME output.
        /// 
        /// Ex:
        /// 2012/07/19 10:07:41
        /// </summary>
        [Test]
        public void TestDecodeSTIME()
        {
            string buffer = "2012/07/19 10:07:41";
            DateTime dt = RTI.Commands.AdcpCommands.DecodeSTIME(buffer);

            Assert.AreEqual(2012, dt.Year, "Year is incorrect.");
            Assert.AreEqual(07, dt.Month, "Month is incorrect.");
            Assert.AreEqual(19, dt.Day, "Day is incorrect.");
            Assert.AreEqual(10, dt.Hour, "Hour is incorrect.");
            Assert.AreEqual(7, dt.Minute, "Minute is incorrect.");
            Assert.AreEqual(41, dt.Second, "Second is incorrect.");
        }

    }
}
