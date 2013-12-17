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
 * 12/03/2012      RC          2.17       Removed SerialNumber.Empty.
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
    public class SerialNumberTest
    {
        /// <summary>
        /// Test the constructor.
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serialNum = new SerialNumber(serialStr);

            // Test the serial number
            Assert.AreEqual(serialStr, serialNum.ToString(), string.Format("Serial number strings did not match {0}  {1}", serialStr, serialNum.ToString()));
            Assert.AreEqual(1, serialNum.SubSystemsList.Count, string.Format("Number of SubSystems did not match 1 {0}", serialNum.SubSystemsList.Count));
            
            Subsystem ss = new Subsystem("3", 0);
            Assert.AreEqual(ss, serialNum.SubSystemsList[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serialNum.SubSystemsList[0].ToString()));

            Assert.AreEqual(1, serialNum.SystemSerialNumber, string.Format("Serial numbers do not match {0}  {1}", 1, serialNum.SystemSerialNumber));
        }

        /// <summary>
        /// Test a Dual frequency subsystem.
        /// </summary>
        [Test]
        public void TestDualFrequency()
        {
            // Create a serial number
            string serialStr = "01340000000000000000000000000001";
            SerialNumber serialNum = new SerialNumber(serialStr);

            // Test the serial number
            Assert.AreEqual(serialStr, serialNum.ToString(), string.Format("Serial number strings did not match {0}  {1}", serialStr, serialNum.ToString()));
            Assert.AreEqual(2, serialNum.SubSystemsList.Count, string.Format("Number of SubSystems did not match 2 {0}", serialNum.SubSystemsList.Count));

            Subsystem ss = new Subsystem("3", 0);
            Assert.AreEqual(ss, serialNum.SubSystemsList[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serialNum.SubSystemsList[0].ToString()));

            Subsystem ss1 = new Subsystem("4", 1);
            Assert.AreEqual(ss1, serialNum.SubSystemsList[1], string.Format("Subsystems do not match {0}  {1}", ss1.ToString(), serialNum.SubSystemsList[1].ToString()));

            Assert.AreEqual(1, serialNum.SystemSerialNumber, string.Format("Serial numbers do not match {0}  {1}", 1, serialNum.SystemSerialNumber));
        }

        /// <summary>
        /// Test a 150khz subsystem.
        /// </summary>
        [Test]
        public void Test150KhzFrequency()
        {
            // Create a serial number
            string serialStr = "01K00000000000000000000000000001";
            SerialNumber serialNum = new SerialNumber(serialStr);

            // Test the serial number
            Assert.AreEqual(serialStr, serialNum.ToString(), string.Format("Serial number strings did not match {0}  {1}", serialStr, serialNum.ToString()));
            Assert.AreEqual(1, serialNum.SubSystemsList.Count, string.Format("Number of SubSystems did not match 1 {0}", serialNum.SubSystemsList.Count));

            // Upper case K
            Subsystem ss = new Subsystem("K", 0);
            Assert.AreEqual(ss, serialNum.SubSystemsList[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serialNum.SubSystemsList[0].ToString()));

            // Lower case k
            Subsystem ss1 = new Subsystem("k", 0);
            Assert.AreNotEqual(ss1, serialNum.SubSystemsList[0], string.Format("Subsystems should not match {0}  {1}", ss1.ToString(), serialNum.SubSystemsList[0].ToString()));

            Assert.AreEqual(1, serialNum.SystemSerialNumber, string.Format("Serial numbers do not match {0}  {1}", 1, serialNum.SystemSerialNumber));
        }

        /// <summary>
        /// Test multiple subsystem.
        /// </summary>
        [Test]
        public void TestMultipleSubsystems()
        {
            // Create a serial number
            string serialStr = "01KBXV00000000000000000000000001";
            SerialNumber serialNum = new SerialNumber(serialStr);

            // Test the serial number
            Assert.AreEqual(serialStr, serialNum.ToString(), string.Format("Serial number strings did not match {0}  {1}", serialStr, serialNum.ToString()));
            Assert.AreEqual(4, serialNum.SubSystemsList.Count, string.Format("Number of SubSystems did not match 4 {0}", serialNum.SubSystemsList.Count));

            // Upper case K
            Subsystem ss = new Subsystem("K", 0);
            Assert.AreEqual(ss, serialNum.SubSystemsList[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serialNum.SubSystemsList[0].ToString()));

            // Lower case K
            Subsystem ssNot = new Subsystem("k", 0);
            Assert.AreNotEqual(ssNot, serialNum.SubSystemsList[0], string.Format("Subsystems should not match {0}  {1}", ssNot.ToString(), serialNum.SubSystemsList[0].ToString()));

            Subsystem ss1 = new Subsystem("B", 1);
            Assert.AreEqual(ss1, serialNum.SubSystemsList[1], string.Format("Subsystems do not match {0}  {1}", ss1.ToString(), serialNum.SubSystemsList[1].ToString()));

            Subsystem ss2 = new Subsystem("X", 2);
            Assert.AreEqual(ss2, serialNum.SubSystemsList[2], string.Format("Subsystems do not match {0}  {1}", ss2.ToString(), serialNum.SubSystemsList[2].ToString()));

            Subsystem ss3 = new Subsystem("V", 3);
            Assert.AreEqual(ss3, serialNum.SubSystemsList[3], string.Format("Subsystems do not match {0}  {1}", ss3.ToString(), serialNum.SubSystemsList[3].ToString()));

            Assert.AreEqual(1, serialNum.SystemSerialNumber, string.Format("Serial numbers do not match {0}  {1}", 1, serialNum.SystemSerialNumber));
        }

        /// <summary>
        /// Test a bad serial number
        /// </summary>
        [Test]
        public void TestBadSerial()
        {
            // Create a serial number
            string serialStr = "z";
            SerialNumber serialNum = new SerialNumber(serialStr);

            // Test the serial number
            Assert.AreEqual(new SerialNumber().SerialNumberString, serialNum.ToString(), string.Format("Serial number strings did not match {0}  {1}", new SerialNumber().SerialNumberString, serialNum.ToString()));
            Assert.AreEqual(0, serialNum.SubSystemsList.Count, string.Format("Number of SubSystems did not match 0 {0}", serialNum.SubSystemsList.Count));

            Assert.AreEqual(0, serialNum.SystemSerialNumber, string.Format("Serial numbers do not match {0}  {1}", 0, serialNum.SystemSerialNumber));
        }

        /// <summary>
        /// Test serial number to/from JSON
        /// </summary>
        [Test]
        public void TestSerialJSON()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);


            // Convert to JSON and back
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(serial);                            // Serialize object to JSON
            SerialNumber serialNum = Newtonsoft.Json.JsonConvert.DeserializeObject<SerialNumber>(json);   // Deserialize the JSON

            // Test the serial number
            Assert.AreEqual(serialStr, serialNum.ToString(), string.Format("Serial number strings did not match {0}  {1}", serialStr, serialNum.ToString()));
            Assert.AreEqual(1, serialNum.SubSystemsList.Count, string.Format("Number of SubSystems did not match 1 {0}", serialNum.SubSystemsList.Count));

            Subsystem ss = new Subsystem("3", 0);
            Assert.AreEqual(ss, serialNum.SubSystemsList[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serialNum.SubSystemsList[0].ToString()));

            Assert.AreEqual(1, serialNum.SystemSerialNumber, string.Format("Serial numbers do not match {0}  {1}", 1, serialNum.SystemSerialNumber));

        }

        #region Serial Number

        /// <summary>
        /// Change the serial number and verify the string is correct.
        /// </summary>
        [Test]
        public void TestChangeSerial()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            serial.SystemSerialNumber = 22;

            Assert.AreEqual(22, serial.SystemSerialNumber, "Sys Serial Number is incorrect.");
            Assert.AreEqual("01300000000000000000000000000022", serial.SerialNumberString, "Serial Number String is incorrect.");
            Assert.AreEqual("01300000000000000000000000000022", serial.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Change the serial number and verify the string is correct.
        /// </summary>
        [Test]
        public void TestChangeSerialBad()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            serial.SystemSerialNumber = 9999999;

            Assert.AreEqual(1, serial.SystemSerialNumber, "Sys Serial Number is incorrect.");
            Assert.AreEqual("01300000000000000000000000000001", serial.SerialNumberString, "Serial Number String is incorrect.");
            Assert.AreEqual("01300000000000000000000000000001", serial.ToString(), "ToString is incorrect.");
        }

        #endregion

        #region Spare

        /// <summary>
        /// Change the Spare and verify the string is correct.
        /// </summary>
        [Test]
        public void TestChangeSpare()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            serial.Spare = "1";

            Assert.AreEqual(1, serial.SystemSerialNumber, "Sys Serial Number is incorrect.");
            Assert.AreEqual("1", serial.Spare, "Spare is incorrect.");
            Assert.AreEqual("01300000000000000100000000000001", serial.SerialNumberString, "Serial Number String is incorrect.");
            Assert.AreEqual("01300000000000000100000000000001", serial.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Change the Spare and verify the string is correct.
        /// </summary>
        [Test]
        public void TestChangeSpare1()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            serial.Spare = "123456789";

            Assert.AreEqual(1, serial.SystemSerialNumber, "Sys Serial Number is incorrect.");
            Assert.AreEqual("123456789", serial.Spare, "Spare is incorrect.");
            Assert.AreEqual("01300000000000000123456789000001", serial.SerialNumberString, "Serial Number String is incorrect.");
            Assert.AreEqual("01300000000000000123456789000001", serial.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Change the serial number and verify the string is correct.
        /// </summary>
        [Test]
        public void TestChangeSpareBad()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            serial.Spare = "1234567890";

            Assert.AreEqual(1, serial.SystemSerialNumber, "Sys Serial Number is incorrect.");
            Assert.AreEqual("000000000", serial.Spare, "Spare is incorrect.");
            Assert.AreEqual("01300000000000000000000000000001", serial.SerialNumberString, "Serial Number String is incorrect.");
            Assert.AreEqual("01300000000000000000000000000001", serial.ToString(), "ToString is incorrect.");
        }

        #endregion

        #region Subsystem

        /// <summary>
        /// Change the Subsystem and verify the string is correct and the Subsystem Dictionary.
        /// </summary>
        [Test]
        public void TestChangeSubsystems()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            serial.SubSystems = "340000000000000";

            Assert.AreEqual("340000000000000", serial.SubSystems, "Subsystem String is incorrect.");
            Assert.AreEqual(2, serial.SubSystemsList.Count, "Subsystem Dict Count is incorrect.");

            // Subsystem type 3
            Subsystem ss = new Subsystem("3", 0);
            Assert.AreEqual(ss, serial.SubSystemsList[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serial.SubSystemsList[0].ToString()));

            // Subsystem type 4
            Subsystem ss1 = new Subsystem("4", 1);
            Assert.AreEqual(ss1, serial.SubSystemsList[1], string.Format("Subsystems do not match {0}  {1}", ss1.ToString(), serial.SubSystemsList[1].ToString()));
        }

        /// <summary>
        /// Change the Subsystem and verify the string is correct and the Subsystem Dictionary.
        /// </summary>
        [Test]
        public void TestChangeSubsystemsBad()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            serial.SubSystems = "34000000000000099";

            Assert.AreEqual("300000000000000", serial.SubSystems, "Subsystem String is incorrect.");
            Assert.AreEqual(1, serial.SubSystemsList.Count, "Subsystem Dict Count is incorrect.");

            // Subsystem type 3
            Subsystem ss = new Subsystem("3", 0);
            Assert.AreEqual(ss, serial.SubSystemsList[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serial.SubSystemsList[0].ToString()));
        }

        /// <summary>
        /// Change the Subsystem and verify the string is correct and the Subsystem Dictionary.
        /// </summary>
        [Test]
        public void TestChangeSubsystems1()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            serial.SubSystems = "123456789ABCDEf";

            Assert.AreEqual("123456789ABCDEf", serial.SubSystems, "Subsystem String is incorrect.");
            Assert.AreEqual(15, serial.SubSystemsList.Count, "Subsystem Dict Count is incorrect.");

            // Subsystem type 1
            Subsystem ss = new Subsystem("1", 0);
            Assert.AreEqual(ss, serial.SubSystemsList[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serial.SubSystemsList[0].ToString()));

            // Subsystem type 2
            Subsystem ss1 = new Subsystem("2", 1);
            Assert.AreEqual(ss1, serial.SubSystemsList[1], string.Format("Subsystems do not match {0}  {1}", ss1.ToString(), serial.SubSystemsList[1].ToString()));

            // Subsystem type 3
            Subsystem ss2 = new Subsystem("3", 2);
            Assert.AreEqual(ss2, serial.SubSystemsList[2], string.Format("Subsystems do not match {0}  {1}", ss2.ToString(), serial.SubSystemsList[2].ToString()));

            // Subsystem type 4
            Subsystem ss3 = new Subsystem("4", 3);
            Assert.AreEqual(ss3, serial.SubSystemsList[3], string.Format("Subsystems do not match {0}  {1}", ss3.ToString(), serial.SubSystemsList[3].ToString()));

            // Subsystem type 5
            Subsystem ss4 = new Subsystem("5", 4);
            Assert.AreEqual(ss4, serial.SubSystemsList[4], string.Format("Subsystems do not match {0}  {1}", ss4.ToString(), serial.SubSystemsList[4].ToString()));

            // Subsystem type 6
            Subsystem ss5 = new Subsystem("6", 5);
            Assert.AreEqual(ss5, serial.SubSystemsList[5], string.Format("Subsystems do not match {0}  {1}", ss5.ToString(), serial.SubSystemsList[5].ToString()));

            // Subsystem type 7
            Subsystem ss6 = new Subsystem("7", 6);
            Assert.AreEqual(ss6, serial.SubSystemsList[6], string.Format("Subsystems do not match {0}  {1}", ss6.ToString(), serial.SubSystemsList[6].ToString()));

            // Subsystem type 8
            Subsystem ss7 = new Subsystem("8", 7);
            Assert.AreEqual(ss7, serial.SubSystemsList[7], string.Format("Subsystems do not match {0}  {1}", ss7.ToString(), serial.SubSystemsList[7].ToString()));

            // Subsystem type 9
            Subsystem ss8 = new Subsystem("9", 8);
            Assert.AreEqual(ss8, serial.SubSystemsList[8], string.Format("Subsystems do not match {0}  {1}", ss8.ToString(), serial.SubSystemsList[8].ToString()));

            // Subsystem type A
            Subsystem ss9 = new Subsystem("A", 9);
            Assert.AreEqual(ss9, serial.SubSystemsList[9], string.Format("Subsystems do not match {0}  {1}", ss9.ToString(), serial.SubSystemsList[9].ToString()));

            // Subsystem type B
            Subsystem ssB = new Subsystem("B", 10);
            Assert.AreEqual(ssB, serial.SubSystemsList[10], string.Format("Subsystems do not match {0}  {1}", ssB.ToString(), serial.SubSystemsList[10].ToString()));

            // Subsystem type C
            Subsystem ssC = new Subsystem("C", 11);
            Assert.AreEqual(ssC, serial.SubSystemsList[11], string.Format("Subsystems do not match {0}  {1}", ssC.ToString(), serial.SubSystemsList[11].ToString()));

            // Subsystem type D
            Subsystem ssD = new Subsystem("D", 12);
            Assert.AreEqual(ssD, serial.SubSystemsList[12], string.Format("Subsystems do not match {0}  {1}", ssD.ToString(), serial.SubSystemsList[12].ToString()));

            // Subsystem type E
            Subsystem ssE = new Subsystem("E", 13);
            Assert.AreEqual(ssE, serial.SubSystemsList[13], string.Format("Subsystems do not match {0}  {1}", ssE.ToString(), serial.SubSystemsList[13].ToString()));

            // Subsystem type f
            Subsystem ssf = new Subsystem("f", 14);
            Assert.AreEqual(ssf, serial.SubSystemsList[14], string.Format("Subsystems do not match {0}  {1}", ssf.ToString(), serial.SubSystemsList[14].ToString()));

        }

        #endregion

        #region Base Electronics

        /// <summary>
        /// Change the BaseHardware and verify the string is correct.
        /// </summary>
        [Test]
        public void TestChangeBaseHardware()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            serial.BaseHardware = "02";

            Assert.AreEqual("02", serial.BaseHardware, "BaseHardware is incorrect.");
            Assert.AreEqual("02300000000000000000000000000001", serial.SerialNumberString, "Serial Number String is incorrect.");
            Assert.AreEqual("02300000000000000000000000000001", serial.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Change the BaseHardware and verify the string is correct.
        /// </summary>
        [Test]
        public void TestChangeBaseHardware1()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            serial.BaseHardware = "2";

            Assert.AreEqual("2", serial.BaseHardware, "BaseHardware is incorrect.");
            Assert.AreEqual("02300000000000000000000000000001", serial.SerialNumberString, "Serial Number String is incorrect.");
            Assert.AreEqual("02300000000000000000000000000001", serial.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Change the BaseHardware and verify the string is correct.
        /// </summary>
        [Test]
        public void TestChangeBaseHardwareBad()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            serial.BaseHardware = "22322323";

            Assert.AreEqual("01", serial.BaseHardware, "BaseHardware is incorrect.");
            Assert.AreEqual("01300000000000000000000000000001", serial.SerialNumberString, "Serial Number String is incorrect.");
            Assert.AreEqual("01300000000000000000000000000001", serial.ToString(), "ToString is incorrect.");
        }

        #endregion

        #region Subsystem Dictionary

        /// <summary>
        /// Change the Subsystem Dict and verify the string is correct and the Subsystem Dictionary.
        /// </summary>
        [Test]
        public void TestChangeSubsystemDict()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            Subsystem ss = new Subsystem("3", 0);
            Subsystem ss1 = new Subsystem(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2, 1);

            List<Subsystem> list = new List<Subsystem>();
            list.Add(ss);
            list.Add(ss1);

            serial.SubSystemsList = list;

            Assert.AreEqual("320000000000000", serial.SubSystems, "Subsystem String is incorrect.");
            Assert.AreEqual(2, serial.SubSystemsList.Count, "Subsystem Dict Count is incorrect.");

            // Subsystem type 3
            Assert.AreEqual(ss, serial.SubSystemsList[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serial.SubSystemsList[0].ToString()));

            // Subsystem type 2
            Assert.AreEqual(ss1, serial.SubSystemsList[1], string.Format("Subsystems do not match {0}  {1}", ss1.ToString(), serial.SubSystemsList[1].ToString()));
        }

        #endregion

        #region Add Subsystem

        /// <summary>
        /// Add a subsystem to the serial number.
        /// </summary>
        [Test]
        public void TestAddSubsystem()
        {
            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            Subsystem ss1 = new Subsystem(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2, 1);
            serial.AddSubsystem(ss1);

            Assert.AreEqual(2, serial.SubSystemsList.Count, "Dictionary count is incorrect.");

            Assert.AreEqual("320000000000000", serial.SubSystems, "Subsystem String is incorrect.");
            Assert.AreEqual(2, serial.SubSystemsList.Count, "Subsystem Dict Count is incorrect.");

            // Subsystem type 3
            Subsystem ss = new Subsystem("3", 0);
            Assert.AreEqual(ss, serial.SubSystemsList[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serial.SubSystemsList[0].ToString()));

            // Subsystem type 2
            Assert.AreEqual(ss1, serial.SubSystemsList[1], string.Format("Subsystems do not match {0}  {1}", ss1.ToString(), serial.SubSystemsList[1].ToString()));
        }

        #endregion

        #region Remove Subsystem

        /// <summary>
        /// Add a subsystem to the serial number.
        /// </summary>
        [Test]
        public void TestRemoveSubsystem()
        {
            // Create a serial number
            string serialStr = "01345600000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            Subsystem ss = new Subsystem("4", 1);

            serial.RemoveSubsystem(ss);

            Assert.AreEqual(3, serial.SubSystemsList.Count, "Subsystem count is incorrect.");

            // Subsystem type 3
            Subsystem ss0 = new Subsystem("3", 0);
            Assert.AreEqual(ss0, serial.SubSystemsList[0], string.Format("Subsystems do not match {0}  {1}", ss0.ToString(), serial.SubSystemsList[0].ToString()));

            // Subsystem type 2
            Subsystem ss2 = new Subsystem("5", 1);
            Assert.AreEqual(ss2, serial.SubSystemsList[1], string.Format("Subsystems do not match {0}  {1}", ss2.ToString(), serial.SubSystemsList[1].ToString()));
        }

        /// <summary>
        /// Add a subsystem to the serial number.
        /// </summary>
        [Test]
        public void TestRemoveSubsystemNone()
        {
            // Create a serial number
            string serialStr = "01345600000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            Subsystem ss = new Subsystem("7", 1);

            serial.RemoveSubsystem(ss);

            Assert.AreEqual(4, serial.SubSystemsList.Count, "Subsystem count is incorrect.");

            // Subsystem type 3
            Subsystem ss0 = new Subsystem("3", 0);
            Assert.AreEqual(ss0, serial.SubSystemsList[0], string.Format("Subsystems do not match {0}  {1}", ss0.ToString(), serial.SubSystemsList[0].ToString()));

            // Subsystem type 2
            Subsystem ss2 = new Subsystem("4", 1);
            Assert.AreEqual(ss2, serial.SubSystemsList[1], string.Format("Subsystems do not match {0}  {1}", ss2.ToString(), serial.SubSystemsList[1].ToString()));
        }

        /// <summary>
        /// Add a subsystem to the serial number.
        /// </summary>
        [Test]
        public void TestRemoveSubsystemWrongIndex()
        {
            // Create a serial number
            string serialStr = "01345600000000000000000000000001";
            SerialNumber serial = new SerialNumber(serialStr);

            Subsystem ss = new Subsystem("4", 4);

            serial.RemoveSubsystem(ss);

            Assert.AreEqual(3, serial.SubSystemsList.Count, "Subsystem count is incorrect.");

            // Subsystem type 3
            Subsystem ss0 = new Subsystem("3", 0);
            Assert.AreEqual(ss0, serial.SubSystemsList[0], string.Format("Subsystems do not match {0}  {1}", ss0.ToString(), serial.SubSystemsList[0].ToString()));

            // Subsystem type 2
            Subsystem ss2 = new Subsystem("5", 1);
            Assert.AreEqual(ss2, serial.SubSystemsList[1], string.Format("Subsystems do not match {0}  {1}", ss2.ToString(), serial.SubSystemsList[1].ToString()));
        }


        #endregion

        #region Empty

        /// <summary>
        /// Test serial number for empty.
        /// </summary>
        [Test]
        public void TestIsEmpty()
        {
            SerialNumber serial = new SerialNumber();

            Assert.IsTrue(serial.IsEmpty(), "IsEmpty() is incorrect.");
        }

        /// <summary>
        /// Test serial number for empty.
        /// </summary>
        [Test]
        public void TestIsEmptyAll0()
        {
            SerialNumber serial = new SerialNumber("00000000000000000000000000000000");

            Assert.IsTrue(serial.IsEmpty(), "IsEmpty() is incorrect.");
        }

        /// <summary>
        /// Test serial number for empty.
        /// </summary>
        [Test]
        public void TestIsEmptyAll10()
        {
            SerialNumber serial = new SerialNumber("01000000000000000000000000000000");

            Assert.IsTrue(serial.IsEmpty(), "IsEmpty() is incorrect.");
        }

        /// <summary>
        /// Test serial number for empty.
        /// </summary>
        [Test]
        public void TestIsEmptySubsystem()
        {
            SerialNumber serial = new SerialNumber("01200000000000000000000000000000");

            Assert.IsFalse(serial.IsEmpty(), "IsEmpty() is incorrect.");
        }

        /// <summary>
        /// Test serial number for empty.
        /// </summary>
        [Test]
        public void TestIsEmptySerial()
        {
            SerialNumber serial = new SerialNumber("01000000000000000000000000000001");

            Assert.IsTrue(serial.IsEmpty(), "IsEmpty() is incorrect.");
        }

        /// <summary>
        /// Test serial number for empty.
        /// </summary>
        [Test]
        public void TestIsEmptySpare()
        {
            SerialNumber serial = new SerialNumber("01000000000000000000100000000000");

            Assert.IsTrue(serial.IsEmpty(), "IsEmpty() is incorrect.");
        }

        #endregion
    }

}