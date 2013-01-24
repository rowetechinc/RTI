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
 * 01/04/2013      RC          2.17       Initial coding
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
    using RTI.Average;

    /// <summary>
    /// Test the Firmware class.
    /// </summary>
    [TestFixture]
    public class FirmwareTest
    {
        /// <summary>
        /// Test the constructor.
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            Firmware fw = new Firmware();

            Assert.AreEqual(0, fw.FirmwareMajor, "Major is incorrect.");
            Assert.AreEqual(0, fw.FirmwareMinor, "Minor is incorrect.");
            Assert.AreEqual(0, fw.FirmwareRevision, "Revision is incorrect.");
            Assert.AreEqual(Subsystem.EMPTY_CODE, fw.GetSubsystemCode(new SerialNumber()), "Subsystem Code is incorrect.");
        }

        /// <summary>
        /// Test the constructor that takes UInt16.
        /// </summary>
        [Test]
        public void TestConstructor2()
        {
            Firmware fw = new Firmware(0x3, 1, 2, 3);

            Assert.AreEqual(1, fw.FirmwareMajor, "Major is incorrect.");
            Assert.AreEqual(2, fw.FirmwareMinor, "Minor is incorrect.");
            Assert.AreEqual(3, fw.FirmwareRevision, "Revision is incorrect.");
            Assert.AreEqual(0x3, fw.GetSubsystemCode(new SerialNumber()), "Subsystem Code is incorrect.");
        }

        /// <summary>
        /// Test the Encode method.
        /// </summary>
        [Test]
        public void TestEncode()
        {
            Firmware fw = new Firmware(0x3, 1, 2, 3);

            Assert.AreEqual(1, fw.FirmwareMajor, "Major is incorrect.");
            Assert.AreEqual(2, fw.FirmwareMinor, "Minor is incorrect.");
            Assert.AreEqual(3, fw.FirmwareRevision, "Revision is incorrect.");
            Assert.AreEqual(0x3, fw.GetSubsystemCode(new SerialNumber()), "Subsystem Code is incorrect.");

            byte[] encoded = fw.Encode();

            Assert.AreEqual(4, encoded.Length, "Encoded length is incorrect.");
            Assert.AreEqual(1, encoded[Firmware.MAJOR_START], "Encoded Major is incorrect.");
            Assert.AreEqual(2, encoded[Firmware.MINOR_START], "Encoded Minor is incorrect.");
            Assert.AreEqual(3, encoded[Firmware.REVISION_START], "Encoded Revision is incorrect.");
            Assert.AreEqual(0x3, encoded[Firmware.SUBSYSTEM_START], "Encoded Subsystem Code is incorrect.");
        }

        /// <summary>
        /// Test the Constructor that takes a byte array
        /// </summary>
        [Test]
        public void TestConstructor3()
        {
            Firmware fw = new Firmware(0x3, 1, 2, 3);

            Assert.AreEqual(1, fw.FirmwareMajor, "Major is incorrect.");
            Assert.AreEqual(2, fw.FirmwareMinor, "Minor is incorrect.");
            Assert.AreEqual(3, fw.FirmwareRevision, "Revision is incorrect.");
            Assert.AreEqual(0x3, fw.GetSubsystemCode(new SerialNumber()), "Subsystem Code is incorrect.");

            byte[] encoded = fw.Encode();

            Assert.AreEqual(4, encoded.Length, "Encoded length is incorrect.");
            Assert.AreEqual(1, encoded[Firmware.MAJOR_START], "Encoded Major is incorrect.");
            Assert.AreEqual(2, encoded[Firmware.MINOR_START], "Encoded Minor is incorrect.");
            Assert.AreEqual(3, encoded[Firmware.REVISION_START], "Encoded Revision is incorrect.");
            Assert.AreEqual(0x3, encoded[Firmware.SUBSYSTEM_START], "Encoded Subsystem Code is incorrect.");

            Firmware fw1 = new Firmware(encoded);

            Assert.AreEqual(1, fw1.FirmwareMajor, "FW1 Major is incorrect.");
            Assert.AreEqual(2, fw1.FirmwareMinor, "FW1 Minor is incorrect.");
            Assert.AreEqual(3, fw1.FirmwareRevision, "FW1 Revision is incorrect.");
            Assert.AreEqual(0x3, fw1.GetSubsystemCode(new SerialNumber()), "FW1 Subsystem Code is incorrect.");
        }

        /// <summary>
        /// Test encoding the data, then decoding the data.
        /// </summary>
        [Test]
        public void TestEncodeDecode()
        {
            Firmware fw = new Firmware(0x1, 5, 2, 3);
            byte[] encoded = fw.Encode();

            Firmware fw1 = new Firmware(encoded);

            Assert.AreEqual(5, fw1.FirmwareMajor, "FW1 Major is incorrect.");
            Assert.AreEqual(2, fw1.FirmwareMinor, "FW1 Minor is incorrect.");
            Assert.AreEqual(3, fw1.FirmwareRevision, "FW1 Revision is incorrect.");
            Assert.AreEqual(0x1, fw1.GetSubsystemCode(new SerialNumber()), "FW1 Subsystem Code is incorrect.");
            Assert.AreEqual(fw1, fw, "Firmware is not ==");
            Assert.AreEqual(true, fw1 == fw, "== is incorrect.");
        }

        /// <summary>
        /// Test the Equals method.
        /// </summary>
        [Test]
        public void TestEqual()
        {
            Firmware fw = new Firmware(0x3, 1, 2, 3);
            byte[] encoded = fw.Encode();
            Firmware fw1 = new Firmware(encoded);

            Assert.AreEqual(true, fw1.Equals(fw), "Equals is incorrect.");
        }

        /// <summary>
        /// Test the == method.
        /// </summary>
        [Test]
        public void TestEqualEqual()
        {
            Firmware fw = new Firmware(0x3, 1, 2, 3);
            byte[] encoded = fw.Encode();
            Firmware fw1 = new Firmware(encoded);

            Assert.AreEqual(true, fw1 == fw, "== is incorrect.");
        }

        /// <summary>
        /// Test the != method.
        /// </summary>
        [Test]
        public void TestNotEqual()
        {
            Firmware fw = new Firmware(0x3, 1, 2, 3);
            Firmware fw1 = new Firmware(0x3, 2, 2, 3);

            Assert.AreEqual(false, fw1 == fw, "== is incorrect.");
            Assert.AreEqual(true, fw1 != fw, "!= is incorrect.");
        }

        /// <summary>
        /// Test ToString().
        /// </summary>
        [Test]
        public void TestToString()
        {
            Firmware fw = new Firmware(0x3, 1, 2, 3);
            Assert.AreEqual("1.2.3 - 3", fw.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Test getting the subsystem.
        /// </summary>
        [Test]
        public void TestGetSubsystem()
        {
            SerialNumber serial = new SerialNumber("01300000000000000000000000000001");
            Firmware fw = new Firmware(0x3, 1, 2, 3);
            Subsystem ss = new Subsystem(fw.GetSubsystemCode(new SerialNumber()));

            Assert.AreEqual(ss, fw.GetSubsystem(serial), "GetSubsystem is incorrect.");
        }

        /// <summary>
        /// Test getting the subsystem.
        /// </summary>
        [Test]
        public void TestGetSubsystemNoSerial()
        {
            SerialNumber serial = new SerialNumber();
            Firmware fw = new Firmware(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, 1, 2, 3);
            Subsystem ss = new Subsystem(fw.GetSubsystemCode(new SerialNumber()));

            Assert.AreEqual(ss, fw.GetSubsystem(serial), "GetSubsystem is incorrect.");
        }

        /// <summary>
        /// Test getting the subsystem.
        /// </summary>
        [Test]
        public void TestGetSubsystemVersion()
        {
            SerialNumber serial = new SerialNumber("01300000000000000000000000000001");
            Firmware fw = new Firmware(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, 0, 2, 3);
            Subsystem ss = new Subsystem(fw.GetSubsystemCode(serial));

            Assert.AreEqual(ss, fw.GetSubsystem(serial), "GetSubsystem is incorrect.");
        }

        /// <summary>
        /// Test getting the firmware list.
        /// </summary>
        [Test]
        public void GetList()
        {
            List<ushort> list = Firmware.FirmwareVersionList();
            Assert.AreEqual(255, list.Count(), "List is incorrect.");
        }
    }
}
