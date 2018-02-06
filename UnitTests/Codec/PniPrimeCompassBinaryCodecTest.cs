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
 * 03/29/2012      RC          2.07       Initial coding
 * 
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    /// <summary>
    /// Unit test of the PNI Prime compass Codec.
    /// </summary>
    [TestFixture]
    public class PniPrimeCompassBinaryCodecTest
    {
        #region Mod Info Command

        /// <summary>
        /// Create the Mod Info Command.
        /// </summary>
        [Test]
        public void GetModInfoCommand( )
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.GetModInfoCommand();

            int countShift = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);

            Assert.AreEqual(cmd.Length, countShift, "Byte Count Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kGetModInfo, cmd[2], "Payload incorrect");
        }
        #endregion

        #region Get Data Command

        /// <summary>
        /// Create the Get Data Command.
        /// </summary>
        [Test]
        public void GetDataCommand()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.GetDataCommand();

            int countShift = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x05;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kGetData;
            data[3] = 0xBF;
            data[4] = 0x71;


            Assert.AreEqual(cmd.Length, countShift, "Byte Count Incorrect.");
            Assert.AreEqual(cmd[0], data[0], "Byte 0 Incorrect.");
            Assert.AreEqual(cmd[1], data[1], "Byte 1 Incorrect.");
            Assert.AreEqual(cmd[2], data[2], "Byte 2 Incorrect.");
            Assert.AreEqual(cmd[3], data[3], "Byte 3 Incorrect.");
            Assert.AreEqual(cmd[4], data[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kGetData, cmd[2], "Payload incorrect");
        }

        #endregion

        #region Start Calibration Command

        /// <summary>
        /// Create the Start Calibration Command.
        /// Magnitude only.
        /// </summary>
        [Test]
        public void StartCalibrationCommandMag()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.StartCalibrationCommand(RTI.PniPrimeCompassBinaryCodec.CalMode.CAL_MAG_ONLY);

            int countShift = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);

            byte[] payload = RTI.MathHelper.UInt32ToByteArray((UInt32)RTI.PniPrimeCompassBinaryCodec.CalMode.CAL_MAG_ONLY, true);

            Assert.AreEqual(cmd.Length, countShift, "Byte Count Incorrect.");
            Assert.AreEqual(0x00, cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(0x09, cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kStartCal, cmd[2], "Payload incorrect");
            Assert.AreEqual(payload[0], cmd[3], "Payload 0 Incorrect.");
            Assert.AreEqual(payload[1], cmd[4], "Payload 1 Incorrect.");
            Assert.AreEqual(payload[2], cmd[5], "Payload 2 Incorrect.");
            Assert.AreEqual(payload[3], cmd[6], "Payload 3 Incorrect.");
        }

        /// <summary>
        /// Create the Start Calibration Command.
        /// Acceleration only.
        /// </summary>
        [Test]
        public void StartCalibrationCommandAccel()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.StartCalibrationCommand(RTI.PniPrimeCompassBinaryCodec.CalMode.CAL_ACCEL_ONLY);

            int countShift = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);

            byte[] payload = RTI.MathHelper.UInt32ToByteArray((UInt32)RTI.PniPrimeCompassBinaryCodec.CalMode.CAL_ACCEL_ONLY, true);

            Assert.AreEqual(cmd.Length, countShift, "Byte Count Incorrect.");
            Assert.AreEqual(0x00, cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(0x09, cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kStartCal, cmd[2], "Payload incorrect");
            Assert.AreEqual(payload[0], cmd[3], "Payload 0 Incorrect.");
            Assert.AreEqual(payload[1], cmd[4], "Payload 1 Incorrect.");
            Assert.AreEqual(payload[2], cmd[5], "Payload 2 Incorrect.");
            Assert.AreEqual(payload[3], cmd[6], "Payload 3 Incorrect.");
        }

        /// <summary>
        /// Create the Start Calibration Command.
        /// Magnitude and Acceleration.
        /// </summary>
        [Test]
        public void StartCalibrationCommandMagAndAccel()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.StartCalibrationCommand(RTI.PniPrimeCompassBinaryCodec.CalMode.CAL_ACCEL_AND_MAG);

            int countShift = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);

            byte[] payload = RTI.MathHelper.UInt32ToByteArray((UInt32)RTI.PniPrimeCompassBinaryCodec.CalMode.CAL_ACCEL_AND_MAG, true);

            Assert.AreEqual(cmd.Length, countShift, "Byte Count Incorrect.");
            Assert.AreEqual(0x00, cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(0x09, cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kStartCal, cmd[2], "Payload incorrect");
            Assert.AreEqual(payload[0], cmd[3], "Payload 0 Incorrect.");
            Assert.AreEqual(payload[1], cmd[4], "Payload 1 Incorrect.");
            Assert.AreEqual(payload[2], cmd[5], "Payload 2 Incorrect.");
            Assert.AreEqual(payload[3], cmd[6], "Payload 3 Incorrect.");
        }
        #endregion

        #region Stop Calibration Command

        /// <summary>
        /// Create the Stop Calibration Command.
        /// </summary>
        [Test]
        public void StopCalibrationCommand()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.StopCalibrationCommand();

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x05;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kStopCal;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(cmd[0], data[0], "Byte 0 Incorrect.");
            Assert.AreEqual(cmd[1], data[1], "Byte 1 Incorrect.");
            Assert.AreEqual(cmd[2], data[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kStopCal, cmd[2], "Payload incorrect");
        }

        #endregion

        #region Interval Mode Command

        /// <summary>
        /// Start the Interval mode.
        /// </summary>
        [Test]
        public void StartIntervalModeCommand()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.StartIntervalModeCommand();

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x05;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kStartIntervalMode;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(cmd[0], data[0], "Byte 0 Incorrect.");
            Assert.AreEqual(cmd[1], data[1], "Byte 1 Incorrect.");
            Assert.AreEqual(cmd[2], data[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kStartIntervalMode, cmd[2], "Payload incorrect");
        }

        /// <summary>
        /// Stop the Interval mode.
        /// </summary>
        [Test]
        public void StopIntervalModeCommand()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.StopIntervalModeCommand();

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x05;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kStopIntervalMode;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(cmd[0], data[0], "Byte 0 Incorrect.");
            Assert.AreEqual(cmd[1], data[1], "Byte 1 Incorrect.");
            Assert.AreEqual(cmd[2], data[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kStopIntervalMode, cmd[2], "Payload incorrect");
        }

        #endregion

        #region Save Data Command

        /// <summary>
        /// Save Data.
        /// </summary>
        [Test]
        public void SaveCompassCalCommand()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SaveCompassCalCommand();

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x05;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSave;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(cmd[0], data[0], "Byte 0 Incorrect.");
            Assert.AreEqual(cmd[1], data[1], "Byte 1 Incorrect.");
            Assert.AreEqual(cmd[2], data[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSave, cmd[2], "Payload incorrect");
        }

        #endregion

        #region Mag Factory Command

        /// <summary>
        /// Set Magnitude Factory command/
        /// </summary>
        [Test]
        public void GetDefaultCompassCalMagCommand()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.GetDefaultCompassCalMagCommand();

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x05;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kFactoryUserCal;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(cmd[0], data[0], "Byte 0 Incorrect.");
            Assert.AreEqual(cmd[1], data[1], "Byte 1 Incorrect.");
            Assert.AreEqual(cmd[2], data[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kFactoryUserCal, cmd[2], "Payload incorrect");
        }

        #endregion

        #region Accel Factory Command

        /// <summary>
        /// Set Accelerator Factory command/
        /// </summary>
        [Test]
        public void GetDefaultCompassCalAccelCommand()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.GetDefaultCompassCalAccelCommand();

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x05;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kFactoryInclCal;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(cmd[0], data[0], "Byte 0 Incorrect.");
            Assert.AreEqual(cmd[1], data[1], "Byte 1 Incorrect.");
            Assert.AreEqual(cmd[2], data[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kFactoryInclCal, cmd[2], "Payload incorrect");
        }

        #endregion

        #region Take Sample Command

        /// <summary>
        /// Set Magnitude Factory command/
        /// </summary>
        [Test]
        public void GetTakeUserCalSampleCommand()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.GetTakeUserCalSampleCommand();

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x05;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kTakeUserCalSample;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kTakeUserCalSample, cmd[2], "Payload incorrect");
        }

        #endregion

        #region All Data Components Command
         
        /// <summary>
        /// Set the command to Get all data available.
        /// </summary>
        [Test]
        public void SetAllDataComponentsCommand()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetAllDataComponentsCommand();

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x11;     // 17
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetDataComponents;

            int payloadCount = 12;
            byte[] payload = new byte[payloadCount];
            payload[0] = (byte)payloadCount;
            payload[1] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kHeading;
            payload[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kPAngle;
            payload[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kRAngle;
            payload[4] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kDistortion;
            payload[5] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kCalStatus;
            payload[6] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kPAligned;
            payload[7] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kRAligned;
            payload[8] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kIZAligned;
            payload[9] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kXAligned;
            payload[10] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kYAligned;
            payload[11] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kZAligned;

            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetDataComponents, cmd[2], "Payload incorrect");
            Assert.AreEqual(payload[0], cmd[3], "Byte Payload 0 Incorrect.");
            Assert.AreEqual(payload[1], cmd[4], "Byte Payload 1 Incorrect.");
            Assert.AreEqual(payload[2], cmd[5], "Byte Payload 2 Incorrect.");
            Assert.AreEqual(payload[3], cmd[6], "Byte Payload 3 Incorrect.");
            Assert.AreEqual(payload[4], cmd[7], "Byte Payload 4 Incorrect.");
            Assert.AreEqual(payload[5], cmd[8], "Byte Payload 5 Incorrect.");
            Assert.AreEqual(payload[6], cmd[9], "Byte Payload 6 Incorrect.");
            Assert.AreEqual(payload[7], cmd[10], "Byte Payload 7 Incorrect.");
            Assert.AreEqual(payload[8], cmd[11], "Byte Payload 8 Incorrect.");
            Assert.AreEqual(payload[9], cmd[12], "Byte Payload 9 Incorrect.");
            Assert.AreEqual(payload[10], cmd[13], "Byte Payload 10 Incorrect.");
            Assert.AreEqual(payload[11], cmd[14], "Byte Payload 11 Incorrect.");
        }

        #endregion

        #region HPR Data Components Command

        /// <summary>
        /// Set the command to get Heading, Pitch and Roll data only.
        /// </summary>
        [Test]
        public void SetHPRDataComponentsCommands()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetHPRDataComponentsCommands();

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x09;     // 9
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetDataComponents;

            int payloadCount = 4;
            byte[] payload = new byte[payloadCount];
            payload[0] = (byte)payloadCount;
            payload[1] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kHeading;
            payload[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kPAngle;
            payload[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kRAngle;

            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetDataComponents, cmd[2], "Payload incorrect");
            Assert.AreEqual(payload[0], cmd[3], "Byte Payload 0 Incorrect.");
            Assert.AreEqual(payload[1], cmd[4], "Byte Payload 1 Incorrect.");
            Assert.AreEqual(payload[2], cmd[5], "Byte Payload 2 Incorrect.");
            Assert.AreEqual(payload[3], cmd[6], "Byte Payload 3 Incorrect.");
        }

        #endregion

        #region Get Config Command

        /// <summary>
        /// Get the Config setting based off the ID given.
        /// Declination
        /// </summary>
        [Test]
        public void GetConfigCommandDeclination()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.GetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kDeclination);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[4];
            data[0] = 0x00;
            data[1] = 0x06;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kDeclination;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kDeclination, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Get the Config setting based off the ID given.
        /// True North
        /// </summary>
        [Test]
        public void GetConfigCommandTrueNorth()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.GetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kTrueNorth);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x06;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kTrueNorth, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Get the Config setting based off the ID given.
        /// Big Endian
        /// </summary>
        [Test]
        public void GetConfigCommandBigEndian()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.GetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kBigEndian);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x06;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kBigEndian, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Get the Config setting based off the ID given.
        /// Mounting Ref
        /// </summary>
        [Test]
        public void GetConfigCommandMountingRef()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.GetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kMountingRef);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x06;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kMountingRef, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Get the Config setting based off the ID given.
        /// Cal Stable Check
        /// </summary>
        [Test]
        public void GetConfigCommandCalStableCheck()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.GetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalStableCheck);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x06;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalStableCheck, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Get the Config setting based off the ID given.
        /// Cal Num Points
        /// </summary>
        [Test]
        public void GetConfigCommandCalNumPoints()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.GetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x06;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Get the Config setting based off the ID given.
        /// Cal Auto Sample
        /// </summary>
        [Test]
        public void GetConfigCommandCalAutoSample()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.GetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalAutoSampling);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x06;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalAutoSampling, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Get the Config setting based off the ID given.
        /// Baudrate
        /// </summary>
        [Test]
        public void GetConfigCommandBaud()
        {
            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.GetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kBaudRate);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x06;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig;


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kGetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kBaudRate, cmd[3], "Payload incorrect");
        }

        #endregion

        #region Set Config Command

        #region Declination

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Declination
        /// </summary>
        [Test]
        public void SetConfigCommandDeclination()
        {
            float value = 12.456f;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kDeclination, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[8];
            data[0] = 0x00;
            data[1] = 0x0A;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kDeclination;
            data[4] = 0x41;
            data[5] = 0x47;
            data[6] = 0x4B;
            data[7] = 0xC7;

            float result = RTI.MathHelper.ByteArrayToFloat(cmd, 4, true);


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual(data[5], cmd[5], "Byte 5 Incorrect.");
            Assert.AreEqual(data[6], cmd[6], "Byte 6 Incorrect.");
            Assert.AreEqual(data[7], cmd[7], "Byte 7 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kDeclination, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Declination.  Minimum value.
        /// </summary>
        [Test]
        public void SetConfigCommandDeclinationMin()
        {
            float value = -180.00f;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kDeclination, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[8];
            data[0] = 0x00;
            data[1] = 0x0A;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kDeclination;
            data[4] = 0xC3;
            data[5] = 0x34;
            data[6] = 0x00;
            data[7] = 0x00;

            float result = RTI.MathHelper.ByteArrayToFloat(cmd, 4, true);


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual(data[5], cmd[5], "Byte 5 Incorrect.");
            Assert.AreEqual(data[6], cmd[6], "Byte 6 Incorrect.");
            Assert.AreEqual(data[7], cmd[7], "Byte 7 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kDeclination, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Declination.  Maximum value.
        /// </summary>
        [Test]
        public void SetConfigCommandDeclinationMax()
        {
            float value = 180.00f;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kDeclination, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[8];
            data[0] = 0x00;
            data[1] = 0x0A;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kDeclination;
            data[4] = 0x43;
            data[5] = 0x34;
            data[6] = 0x00;
            data[7] = 0x00;

            float result = RTI.MathHelper.ByteArrayToFloat(cmd, 4, true);


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual(data[5], cmd[5], "Byte 5 Incorrect.");
            Assert.AreEqual(data[6], cmd[6], "Byte 6 Incorrect.");
            Assert.AreEqual(data[7], cmd[7], "Byte 7 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kDeclination, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Declination.  Value will be out of range.
        /// </summary>
        [Test]
        public void SetConfigCommandDeclinationOutOfRangePostive()
        {
            float value = 1200.456f;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kDeclination, value);

            Assert.IsNull(cmd, "Bad Values not found.");

        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Declination.  Value will be out of range.
        /// </summary>
        [Test]
        public void SetConfigCommandDeclinationOutOfRangeNegative()
        {
            float value = -1200.456f;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kDeclination, value);

            Assert.IsNull(cmd, "Bad Values not found.");

        }
        #endregion

        #region True North

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// True North
        /// </summary>
        [Test]
        public void SetConfigCommandTrueNorthTrue()
        {
            bool value = true;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kTrueNorth, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kTrueNorth;
            data[4] = 0x01; // True

            bool result = RTI.MathHelper.ByteArrayToBoolean(cmd, 4);


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kTrueNorth, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// True North
        /// </summary>
        [Test]
        public void SetConfigCommandTrueNorthFalse()
        {
            bool value = false;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kTrueNorth, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kTrueNorth;
            data[4] = 0x00; // True

            bool result = RTI.MathHelper.ByteArrayToBoolean(cmd, 4);


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kTrueNorth, cmd[3], "Payload incorrect");
        }
        #endregion

        #region Big Endian

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Big Endian
        /// </summary>
        [Test]
        public void SetConfigCommandBigEndianTrue()
        {
            bool value = true;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kBigEndian, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kBigEndian;
            data[4] = 0x01; // True

            bool result = RTI.MathHelper.ByteArrayToBoolean(cmd, 4);

            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kBigEndian, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Big Endian
        /// </summary>
        [Test]
        public void SetConfigCommandBigEndianFalse()
        {
            bool value = false;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kBigEndian, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kBigEndian;
            data[4] = 0x00; // True

            bool result = RTI.MathHelper.ByteArrayToBoolean(cmd, 4);

            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kBigEndian, cmd[3], "Payload incorrect");
        }
        #endregion

        #region Mounting Ref

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Mounting Ref
        /// </summary>
        [Test]
        public void SetConfigCommandMountingRef()
        {
            byte value = 3;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kMountingRef, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kMountingRef;
            data[4] = 0x03; // Y Axis Up

            int result = (int)data[4];

            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kMountingRef, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Mounting Ref.  Give a bad value, should be a byte but given an int.
        /// </summary>
        [Test]
        public void SetConfigCommandMountingRefBad()
        {
            int value = 3;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kMountingRef, value);

            Assert.IsNull(cmd, "Bad data not handled correctly");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Mounting Ref.  Out of range.
        /// </summary>
        [Test]
        public void SetConfigCommandMountingRefOutOfRange()
        {
            int value = 25;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kMountingRef, value);

            Assert.IsNull(cmd, "Bad data not handled correctly");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Mounting Ref Min.
        /// </summary>
        [Test]
        public void SetConfigCommandMountingRefMin()
        {
            byte value = 1;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kMountingRef, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kMountingRef;
            data[4] = 0x01; // Min Value

            int result = (int)data[4];

            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kMountingRef, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Mounting Ref Min.
        /// </summary>
        [Test]
        public void SetConfigCommandMountingRefMax()
        {
            byte value = 24;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kMountingRef, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kMountingRef;
            data[4] = 0x18; // Max Value

            int result = (int)data[4];

            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kMountingRef, cmd[3], "Payload incorrect");
        }
        #endregion

        #region Stable Check

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// User Cal Stable Check
        /// </summary>
        [Test]
        public void SetConfigCommandStableCheckTrue()
        {
            bool value = true;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalStableCheck, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalStableCheck;
            data[4] = 0x01; // True

            bool result = RTI.MathHelper.ByteArrayToBoolean(cmd, 4);


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalStableCheck, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// User Cal Stable Check False.
        /// </summary>
        [Test]
        public void SetConfigCommandStableCheckFalse()
        {
            bool value = false;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalStableCheck, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalStableCheck;
            data[4] = 0x00; // True

            bool result = RTI.MathHelper.ByteArrayToBoolean(cmd, 4);


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalStableCheck, cmd[3], "Payload incorrect");
        }

        #endregion

        #region User Cal Number of Points

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// User Cal Num Points
        /// </summary>
        [Test]
        public void SetConfigCommandNumPoints()
        {
            UInt32 value = 13;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[8];
            data[0] = 0x00;
            data[1] = 0x0A;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints;
            data[4] = 0x00;
            data[5] = 0x00;
            data[6] = 0x00;
            data[7] = 0x0D;

            float result = RTI.MathHelper.ByteArrayToUInt32(cmd, 4, true);


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual(data[5], cmd[5], "Byte 5 Incorrect.");
            Assert.AreEqual(data[6], cmd[6], "Byte 6 Incorrect.");
            Assert.AreEqual(data[7], cmd[7], "Byte 7 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// User Cal Num Points.  Minimum value.
        /// </summary>
        [Test]
        public void SetConfigCommandNumPointsMin()
        {
            UInt32 value = 12;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[8];
            data[0] = 0x00;
            data[1] = 0x0A;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints;
            data[4] = 0x00;
            data[5] = 0x00;
            data[6] = 0x00;
            data[7] = 0x0C;

            float result = RTI.MathHelper.ByteArrayToUInt32(cmd, 4, true);


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual(data[5], cmd[5], "Byte 5 Incorrect.");
            Assert.AreEqual(data[6], cmd[6], "Byte 6 Incorrect.");
            Assert.AreEqual(data[7], cmd[7], "Byte 7 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// User Cal Num Points.  Maximum value.
        /// </summary>
        [Test]
        public void SetConfigCommandNumPointsnMax()
        {
            UInt32 value = 32;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[8];
            data[0] = 0x00;
            data[1] = 0x0A;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints;
            data[4] = 0x00;
            data[5] = 0x00;
            data[6] = 0x00;
            data[7] = 0x20;

            float result = RTI.MathHelper.ByteArrayToUInt32(cmd, 4, true);


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual(data[5], cmd[5], "Byte 5 Incorrect.");
            Assert.AreEqual(data[6], cmd[6], "Byte 6 Incorrect.");
            Assert.AreEqual(data[7], cmd[7], "Byte 7 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// User Cal Num Points.  Value will be out of range.
        /// </summary>
        [Test]
        public void SetConfigCommandNumPointsOutOfRangePostive()
        {
            UInt32 value = 50;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints, value);

            Assert.IsNull(cmd, "Bad Values not found.");

        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// User Cal Num Points.  Value will be out of range.
        /// </summary>
        [Test]
        public void SetConfigCommandNumPointsOutOfRangeNegative()
        {
            float value = -5;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints, value);

            Assert.IsNull(cmd, "Bad Values not found.");

        }
        #endregion

        #region Stable Check

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// User Cal Auto Sampling
        /// </summary>
        [Test]
        public void SetConfigCommandAutoSamplingTrue()
        {
            bool value = true;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalAutoSampling, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalAutoSampling;
            data[4] = 0x01; // True

            bool result = RTI.MathHelper.ByteArrayToBoolean(cmd, 4);


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalAutoSampling, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// User Cal Auto Sampling False.
        /// </summary>
        [Test]
        public void SetConfigCommandAutoSamplingFalse()
        {
            bool value = false;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalAutoSampling, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalAutoSampling;
            data[4] = 0x00; // True

            bool result = RTI.MathHelper.ByteArrayToBoolean(cmd, 4);


            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kUserCalAutoSampling, cmd[3], "Payload incorrect");
        }

        #endregion

        #region Baud Rate

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Baudrate.
        /// </summary>
        [Test]
        public void SetConfigCommandBaudrate()
        {
            byte value = 3;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kBaudRate, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kBaudRate;
            data[4] = 0x03; // 1800

            int result = (int)data[4];

            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kBaudRate, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Baudrate.  Give a bad value, should be a byte but given an int.
        /// </summary>
        [Test]
        public void SetConfigCommandBaudrateBad()
        {
            int value = -1;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kBaudRate, value);

            Assert.IsNull(cmd, "Bad data not handled correctly");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Baudrate.  Out of range.
        /// </summary>
        [Test]
        public void SetConfigCommandBaudrateOutOfRange()
        {
            int value = 25;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kBaudRate, value);

            Assert.IsNull(cmd, "Bad data not handled correctly");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Baudrate Min.
        /// </summary>
        [Test]
        public void SetConfigCommandBaudrateMin()
        {
            byte value = 0;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kBaudRate, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kBaudRate;
            data[4] = 0x00; // Min Value

            int result = (int)data[4];

            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kBaudRate, cmd[3], "Payload incorrect");
        }

        /// <summary>
        /// Set the Config setting based off the ID and value given.
        /// Baudrate Max.
        /// </summary>
        [Test]
        public void SetConfigCommandBaudrateMax()
        {
            byte value = 14;

            byte[] cmd = RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kBaudRate, value);

            int count = RTI.MathHelper.ByteArrayToUInt16(cmd, 0, true);
            byte[] data = new byte[5];
            data[0] = 0x00;
            data[1] = 0x07;
            data[2] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig;
            data[3] = (byte)RTI.PniPrimeCompassBinaryCodec.ID.kBaudRate;
            data[4] = 0xE; // Max Value

            int result = (int)data[4];

            Assert.AreEqual(cmd.Length, count, "Byte Count Incorrect.");
            Assert.AreEqual(value, result, "Result Incorrect.");
            Assert.AreEqual(data[0], cmd[0], "Byte 0 Incorrect.");
            Assert.AreEqual(data[1], cmd[1], "Byte 1 Incorrect.");
            Assert.AreEqual(data[2], cmd[2], "Byte 2 Incorrect.");
            Assert.AreEqual(data[3], cmd[3], "Byte 3 Incorrect.");
            Assert.AreEqual(data[4], cmd[4], "Byte 4 Incorrect.");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kSetConfig, cmd[2], "ID incorrect");
            Assert.AreEqual((int)RTI.PniPrimeCompassBinaryCodec.ID.kBaudRate, cmd[3], "Payload incorrect");
        }
        #endregion

        #endregion
    }
}
