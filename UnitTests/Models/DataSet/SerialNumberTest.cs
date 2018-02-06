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
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;

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
            Assert.AreEqual(1, serialNum.SubSystemsDict.Count, string.Format("Number of SubSystems did not match 1 {0}", serialNum.SubSystemsDict.Count));
            
            Subsystem ss = new Subsystem("3", 0);
            Assert.AreEqual(ss, serialNum.SubSystemsDict[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serialNum.SubSystemsDict[0].ToString()));

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
            Assert.AreEqual(2, serialNum.SubSystemsDict.Count, string.Format("Number of SubSystems did not match 2 {0}", serialNum.SubSystemsDict.Count));

            Subsystem ss = new Subsystem("3", 0);
            Assert.AreEqual(ss, serialNum.SubSystemsDict[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serialNum.SubSystemsDict[0].ToString()));

            Subsystem ss1 = new Subsystem("4", 1);
            Assert.AreEqual(ss1, serialNum.SubSystemsDict[1], string.Format("Subsystems do not match {0}  {1}", ss1.ToString(), serialNum.SubSystemsDict[1].ToString()));

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
            Assert.AreEqual(1, serialNum.SubSystemsDict.Count, string.Format("Number of SubSystems did not match 1 {0}", serialNum.SubSystemsDict.Count));

            // Upper case K
            Subsystem ss = new Subsystem("K", 0);
            Assert.AreEqual(ss, serialNum.SubSystemsDict[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serialNum.SubSystemsDict[0].ToString()));

            // Lower case k
            Subsystem ss1 = new Subsystem("k", 0);
            Assert.AreNotEqual(ss1, serialNum.SubSystemsDict[0], string.Format("Subsystems should not match {0}  {1}", ss1.ToString(), serialNum.SubSystemsDict[0].ToString()));

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
            Assert.AreEqual(4, serialNum.SubSystemsDict.Count, string.Format("Number of SubSystems did not match 4 {0}", serialNum.SubSystemsDict.Count));

            // Upper case K
            Subsystem ss = new Subsystem("K", 0);
            Assert.AreEqual(ss, serialNum.SubSystemsDict[0], string.Format("Subsystems do not match {0}  {1}", ss.ToString(), serialNum.SubSystemsDict[0].ToString()));

            // Lower case K
            Subsystem ssNot = new Subsystem("k", 0);
            Assert.AreNotEqual(ssNot, serialNum.SubSystemsDict[0], string.Format("Subsystems should not match {0}  {1}", ssNot.ToString(), serialNum.SubSystemsDict[0].ToString()));

            Subsystem ss1 = new Subsystem("B", 1);
            Assert.AreEqual(ss1, serialNum.SubSystemsDict[1], string.Format("Subsystems do not match {0}  {1}", ss1.ToString(), serialNum.SubSystemsDict[1].ToString()));

            Subsystem ss2 = new Subsystem("X", 2);
            Assert.AreEqual(ss2, serialNum.SubSystemsDict[2], string.Format("Subsystems do not match {0}  {1}", ss2.ToString(), serialNum.SubSystemsDict[2].ToString()));

            Subsystem ss3 = new Subsystem("V", 3);
            Assert.AreEqual(ss3, serialNum.SubSystemsDict[3], string.Format("Subsystems do not match {0}  {1}", ss3.ToString(), serialNum.SubSystemsDict[3].ToString()));

            Assert.AreEqual(1, serialNum.SystemSerialNumber, string.Format("Serial numbers do not match {0}  {1}", 1, serialNum.SystemSerialNumber));
        }
    }

}