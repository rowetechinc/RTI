///*
// * Copyright © 2011 
// * Rowe Technology Inc.
// * All rights reserved.
// * http://www.rowetechinc.com
// * 
// * Redistribution and use in source and binary forms, with or without
// * modification is NOT permitted.
// * 
// * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
// * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
// * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
// * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
// * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
// * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// * POSSIBILITY OF SUCH DAMAGE.
// * 
// * HISTORY
// * -----------------------------------------------------------------
// * Date            Initials    Version    Comments
// * -----------------------------------------------------------------
// * 02/25/2014      RC          2.21.4     Initial coding
// * 05/28/2014      RC          2.21.4     Updated the RTI decoding.
// * 
// * 
// */

//namespace RTI
//{
//    using System;
//    using NUnit.Framework;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;

//    /// <summary>
//    /// Test the PD0 Variable Leader.
//    /// </summary>
//    [TestFixture]
//    public class Pd0VariableLeaderTest
//    {

//        /// <summary>
//        /// Test the constructor.
//        /// </summary>
//        [Test]
//        public void TestConstructor()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            Assert.AreEqual(0, vl.EnsembleNumber, "Ensemble Number is incorrect.");
//            Assert.AreEqual(DateTime.Now.Year-2000, vl.RtcYear, "RTC Year is incorrect.");
//            Assert.AreEqual(DateTime.Now.Month, vl.RtcMonth, "RTC Month is incorrect.");
//            Assert.AreEqual(DateTime.Now.Day, vl.RtcDay, "RTC Day is incorrect.");
//            Assert.AreEqual(DateTime.Now.Hour, vl.RtcHour, "RTC Hour is incorrect.");
//            Assert.AreEqual(DateTime.Now.Minute, vl.RtcMinute, "RTC Minute is incorrect.");
//            Assert.AreEqual(DateTime.Now.Second, vl.RtcSecond, "RTC Second is incorrect.");
//            Assert.AreEqual(0, vl.EnsembleNumRollover, "Ensemble Number Rollover is incorrect.");
//            Assert.AreEqual(0, vl.BitResult, "Bit Result is incorrect.");
//            Assert.AreEqual(0, vl.SpeedOfSound, "Speed of Sound is incorrect.");
//            Assert.AreEqual(0, vl.DepthOfTransducer, "Depth of Transducer is incorrect.");
//            Assert.AreEqual(0.0f, vl.Heading, "Heading is incorrect.");
//            Assert.AreEqual(0.0f, vl.Pitch, "Pitch is incorrect.");
//            Assert.AreEqual(0.0f, vl.Roll, "Roll is incorrect.");
//            Assert.AreEqual(0, vl.Salinity, "Salinity is incorrect.");
//            Assert.AreEqual(0.0f, vl.Temperature, "Temperature is incorrect.");
//            Assert.AreEqual(0, vl.MinPrePingWaitTimeMinutes, "Min PrePing Wait Time Minute is incorrect.");
//            Assert.AreEqual(0, vl.MinPrePingWaitTimeSeconds, "Min PrePIng Wait Time Seconds is incorrect.");
//            Assert.AreEqual(0, vl.MinPrePingWaitTimeHundredths, "Min PrePing Wait Time Hundredth is incorrect.");
//            Assert.AreEqual(0, vl.HeadingStdDev, "Heading Standard Dev is incorrect.");
//            Assert.AreEqual(0.0f, vl.PitchStdDev, "Pitch Standard Dev is incorrect.");
//            Assert.AreEqual(0.0f, vl.RollStdDev, "Roll Standard Dev is incorrect.");
//            Assert.AreEqual(0, vl.Adc0, "ADC Channel 0 is incorrect.");
//            Assert.AreEqual(0, vl.Adc1, "ADC Channel 1 is incorrect.");
//            Assert.AreEqual(0, vl.Adc2, "ADC Channel 2 is incorrect.");
//            Assert.AreEqual(0, vl.Adc3, "ADC Channel 3 is incorrect.");
//            Assert.AreEqual(0, vl.Adc4, "ADC Channel 4 is incorrect.");
//            Assert.AreEqual(0, vl.Adc5, "ADC Channel 5 is incorrect.");
//            Assert.AreEqual(0, vl.Adc6, "ADC Channel 6 is incorrect.");
//            Assert.AreEqual(0, vl.Adc7, "ADC Channel 7 is incorrect.");
//            Assert.AreEqual(0, vl.Esw0, "Error Status Word 0, is incorrect.");
//            Assert.AreEqual(0, vl.Esw1, "Error Status Word 1, is incorrect.");
//            Assert.AreEqual(0, vl.Esw2, "Error Status Word 2, is incorrect.");
//            Assert.AreEqual(0, vl.Esw3, "Error Status Word 3, is incorrect.");
//            Assert.AreEqual(0, vl.Reserved, "Reserved is incorrect.");
//            Assert.AreEqual(0, vl.Pressure, "Pressure is incorrect.");
//            Assert.AreEqual(0, vl.PressureVariance, "Pressure Variance is incorrect.");
//            Assert.AreEqual(0, vl.Spare, "Spare is incorrect.");
//            Assert.AreEqual(0, vl.RtcY2kCentury, "RTC Y2K Century is incorrect.");
//            Assert.AreEqual(DateTime.Now.Year - 2000, vl.RtcY2kYear, "RTC Y2k Year is incorrect.");
//            Assert.AreEqual(DateTime.Now.Month, vl.RtcY2kMonth, "RTC Y2k Month is incorrect.");
//            Assert.AreEqual(DateTime.Now.Day, vl.RtcY2kDay, "RTC Y2k Day is incorrect.");
//            Assert.AreEqual(DateTime.Now.Hour, vl.RtcY2kHour, "RTC Y2k Hour is incorrect.");
//            Assert.AreEqual(DateTime.Now.Minute, vl.RtcY2kMinute, "RTC Y2k Minute is incorrect.");
//            Assert.AreEqual(DateTime.Now.Second, vl.RtcY2kSecond, "RTC Y2k Second is incorrect.");
//        }

//        #region Ensemble Number

//        #region Decode

//        /// <summary>
//        /// Test decoding the Ensemble Number.
//        /// </summary>
//        [Test]
//        public void TestEnsembleNumberDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[2] = 0xF3;
//            data[3] = 0x02;
//            vl.Decode(data);
//            Assert.AreEqual(755, vl.EnsembleNumber, "Ensemble Number Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Ensemble Number.
//        /// </summary>
//        [Test]
//        public void TestEnsembleNumberEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.EnsembleNumber = 755;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[2], "Ensemble Number LSB Encode is incorrect.");
//            Assert.AreEqual(0x02, data[3], "Ensemble Number MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Ensemble Number minimum.
//        /// </summary>
//        [Test]
//        public void TestEnsembleNumberEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.EnsembleNumber = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[2], "Ensemble Number LSB Min Encode is incorrect.");
//            Assert.AreEqual(0x00, data[3], "Ensemble Number MSB Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Ensemble Number maximum.
//        /// </summary>
//        [Test]
//        public void TestEnsembleNumberEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.EnsembleNumber = 65535;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[2], "Ensemble Number LSB Max Encode is incorrect.");
//            Assert.AreEqual(0xFF, data[3], "Ensemble Number MSB Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Year

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Year.
//        /// </summary>
//        [Test]
//        public void TestRtcYearDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[4] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcYear, "RTC Year Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Year.
//        /// </summary>
//        [Test]
//        public void TestRtcYearEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcYear = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[4], "RTC Year Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Year minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcYearEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcYear = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[4], "RTC Year Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Year maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcYearEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcYear = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[4], "RTC Year Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Month

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Month.
//        /// </summary>
//        [Test]
//        public void TestRtcMonthDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[5] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcMonth, "RTC Month Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Month.
//        /// </summary>
//        [Test]
//        public void TestRtcMonthEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcMonth = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[5], "RTC Month Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Month minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcMonthEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcMonth = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[5], "RTC Month Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Month maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcMonthEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcMonth = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[5], "RTC Month Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Day

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Day.
//        /// </summary>
//        [Test]
//        public void TestRtcDayDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[6] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcDay, "RTC Day Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Day.
//        /// </summary>
//        [Test]
//        public void TestRtcDayEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcDay = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[6], "RTC Day Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Day minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcDayEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcDay = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[6], "RTC Day Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Day maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcDayEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcDay = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[6], "RTC Day Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Hour

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Hour.
//        /// </summary>
//        [Test]
//        public void TestRtcHourDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[7] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcHour, "RTC Hour Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Hour.
//        /// </summary>
//        [Test]
//        public void TestRtcHourEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcHour = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[7], "RTC Hour Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Hour minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcHourEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcHour = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[7], "RTC Hour Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Hour maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcHourEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcHour = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[7], "RTC Hour Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Minute

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Minute.
//        /// </summary>
//        [Test]
//        public void TestRtcMinuteDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[8] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcMinute, "RTC Minute Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Minute.
//        /// </summary>
//        [Test]
//        public void TestRtcMinuteEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcMinute = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[8], "RTC Minute Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Minute minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcMinuteEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcMinute = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[8], "RTC Minute Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Minute maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcMinuteEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcMinute = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[8], "RTC Minute Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Second

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Second.
//        /// </summary>
//        [Test]
//        public void TestRtcSecondDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[9] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcSecond, "RTC Second Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Second.
//        /// </summary>
//        [Test]
//        public void TestRtcSecondEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcSecond = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[9], "RTC Second Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Second minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcSecondEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcSecond = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[9], "RTC Second Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Second maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcSecondEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcSecond = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[9], "RTC Second Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Hundredth

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Hundredth.
//        /// </summary>
//        [Test]
//        public void TestRtcHundredthDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[10] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcHundredths, "RTC Hundredth Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Hundredth.
//        /// </summary>
//        [Test]
//        public void TestRtcHundredthEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcHundredths = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[10], "RTC Hundredth Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Hundredth minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcHundredthEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcHundredths = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[10], "RTC Hundredth Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Hundredth maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcHundredthEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcHundredths = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[10], "RTC Hundredth Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Ensemble Rollover

//        #region Decode

//        /// <summary>
//        /// Test decoding the Ensemble Number Rollover.
//        /// </summary>
//        [Test]
//        public void TestEnsembleRolloverDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[11] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.EnsembleNumRollover, "Ensemble Number Rollover Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Ensemble Number Rollover.
//        /// </summary>
//        [Test]
//        public void TestEnsembleRolloverEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.EnsembleNumRollover = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[11], "Ensemble Number Rollover Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Ensemble Number Rollover minimum.
//        /// </summary>
//        [Test]
//        public void TestEnsembleRolloverEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.EnsembleNumRollover = 1;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x01, data[11], "Ensemble Number Rollover Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Ensemble Number Rollover maximum.
//        /// </summary>
//        [Test]
//        public void TestEnsembleRolloverEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.EnsembleNumRollover = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[11], "Ensemble Number Rollover Max Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test the GetEnsembleNumber() function.
//        /// </summary>
//        [Test]
//        public void TestGetEnsembleNumber()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.EnsembleNumber = 6123;
//            vl.EnsembleNumRollover = 2;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(137194, vl.GetEnsembleNumber(), "GetEnsembleNumber() Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test the GetEnsembleNumber() function.
//        /// </summary>
//        [Test]
//        public void TestGetEnsembleNumber0Min()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.EnsembleNumber = 1;
//            vl.EnsembleNumRollover = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(1, vl.GetEnsembleNumber(), "GetEnsembleNumber() Min 0 Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test the GetEnsembleNumber() function.
//        /// </summary>
//        [Test]
//        public void TestGetEnsembleNumber0Max()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.EnsembleNumber = 65535;
//            vl.EnsembleNumRollover = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(65535, vl.GetEnsembleNumber(), "GetEnsembleNumber() Max 0 Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test the GetEnsembleNumber() function.
//        /// </summary>
//        [Test]
//        public void TestGetEnsembleNumber0()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.EnsembleNumber = 0;
//            vl.EnsembleNumRollover = 1;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(65536, vl.GetEnsembleNumber(), "GetEnsembleNumber() 1 Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test the GetEnsembleNumber() function.
//        /// </summary>
//        [Test]
//        public void TestGetEnsembleNumber1()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.EnsembleNumber = 1;
//            vl.EnsembleNumRollover = 1;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(65537, vl.GetEnsembleNumber(), "GetEnsembleNumber() 1 Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region BIT Result

//        #region Encode

//        /// <summary>
//        /// Test encoding the BIT result.
//        /// </summary>
//        [Test]
//        public void TestBitResultEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.SetReserved0();
//            vl.SetTimingCardError();
//            vl.SetReserved2();
//            vl.SetDemod0Error();
//            vl.SetDemod1Error();
//            vl.SetReserved5();
//            vl.SetReserved6();
//            vl.SetReserved7();
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[12], "Bit Result 12 is incorrect.");
//            Assert.AreEqual(0x00, data[13], "Bit Result 13 is incorrect.");
//        }

//        #endregion

//        #region Decode

//        /// <summary>
//        /// Test Decoding the BIT result.
//        /// </summary>
//        [Test]
//        public void TestBitResultDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[12] = 0xC2;
//            vl.Decode(data);

//            Assert.IsFalse(vl.IsReserved0Set(), "Reserved 0 is incorrect.");
//            Assert.IsTrue(vl.IsTimingCardErrorSet(), "Timing Card Error is incorrect.");
//            Assert.IsFalse(vl.IsReserved2Set(), "Reserved 2 is incorrect.");
//            Assert.IsFalse(vl.IsDemod0ErrorSet(), "Demod 0 Error is incorrect.");
//            Assert.IsFalse(vl.IsDemod1ErrorSet(), "Demod 1 Error is incorrect.");
//            Assert.IsFalse(vl.IsReserved5Set(), "Reserved 5 is incorrect.");
//            Assert.IsTrue(vl.IsReserved6Set(), "Reserved 6 is incorrect.");
//            Assert.IsTrue(vl.IsReserved7Set(), "Reserved 7 is incorrect.");
//        }

//        #endregion

//        /// <summary>
//        /// Test if BIT Result Reserved 0.
//        /// </summary>
//        [Test]
//        public void TestReserved0()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetReserved0();
//            Assert.IsTrue(vl.IsReserved0Set(), "IsReserved0Set() is incorrect.");

//            // Test False
//            vl.UnSetReserved0();
//            Assert.IsFalse(vl.IsReserved0Set(), "IsReserved0Set() is incorrect.");
//        }

//        /// <summary>
//        /// Test if BIT Result Timing Card Error.
//        /// </summary>
//        [Test]
//        public void TestTimingCardError()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetTimingCardError();
//            Assert.IsTrue(vl.IsTimingCardErrorSet(), "Timing Card Error is incorrect.");

//            // Test False
//            vl.UnSetTimingCardError();
//            Assert.IsFalse(vl.IsTimingCardErrorSet(), "Timing Card Error is incorrect.");
//        }

//        /// <summary>
//        /// Test if BIT Result Reserved 2.
//        /// </summary>
//        [Test]
//        public void TestReserved2()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetReserved2();
//            Assert.IsTrue(vl.IsReserved2Set(), "Reserved 2 is incorrect.");

//            // Test False
//            vl.UnSetReserved2();
//            Assert.IsFalse(vl.IsReserved2Set(), "Reserved 2 is incorrect.");
//        }

//        /// <summary>
//        /// Test if BIT Result Demod 0 Error.
//        /// </summary>
//        [Test]
//        public void TestDemod0Error()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetDemod0Error();
//            Assert.IsTrue(vl.IsDemod0ErrorSet(), "Demod 0 Error is incorrect.");

//            // Test False
//            vl.UnSetDemod0Error();
//            Assert.IsFalse(vl.IsDemod0ErrorSet(), "Demod 0 Error is incorrect.");
//        }

//        /// <summary>
//        /// Test if BIT Result Demod 1 Error.
//        /// </summary>
//        [Test]
//        public void TestDemod1Error()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetDemod1Error();
//            Assert.IsTrue(vl.IsDemod1ErrorSet(), "Demod 1 Error is incorrect.");

//            // Test False
//            vl.UnSetDemod1Error();
//            Assert.IsFalse(vl.IsDemod1ErrorSet(), "Demod 1 Error is incorrect.");
//        }

//        /// <summary>
//        /// Test if BIT Result Reserved 5.
//        /// </summary>
//        [Test]
//        public void TestReserved5()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetReserved5();
//            Assert.IsTrue(vl.IsReserved5Set(), "Reserved 5 is incorrect.");

//            // Test False
//            vl.UnSetReserved5();
//            Assert.IsFalse(vl.IsReserved5Set(), "Reserved 5 is incorrect.");
//        }

//        /// <summary>
//        /// Test if BIT Result Reserved 6.
//        /// </summary>
//        [Test]
//        public void TestReserved6()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetReserved6();
//            Assert.IsTrue(vl.IsReserved6Set(), "Reserved 6 is incorrect.");

//            // Test False
//            vl.UnSetReserved6();
//            Assert.IsFalse(vl.IsReserved6Set(), "Reserved 6 is incorrect.");
//        }

//        /// <summary>
//        /// Test if BIT Result Reserved 7.
//        /// </summary>
//        [Test]
//        public void TestReserved7()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetReserved7();
//            Assert.IsTrue(vl.IsReserved7Set(), "Reserved 7 true is incorrect.");

//            // Test False
//            vl.UnSetReserved7();
//            Assert.IsFalse(vl.IsReserved7Set(), "Reserved 7 false is incorrect.");
//        }

//        #endregion

//        #region Speed of Sound

//        #region Decode

//        /// <summary>
//        /// Test decoding the Speed of Sound.
//        /// </summary>
//        [Test]
//        public void TestSpeedOfSoundDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[14] = 0xDC;
//            data[15] = 0x05;
//            vl.Decode(data);
//            Assert.AreEqual(1500, vl.SpeedOfSound, "Speed of Sound Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Speed of Sound.
//        /// </summary>
//        [Test]
//        public void TestSpeedOfSoundEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.SpeedOfSound = 1500;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xDC, data[14], "Speed of Sound LSB Encode is incorrect.");
//            Assert.AreEqual(0x05, data[15], "Speed of Sound MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Speed of Sound minimum.
//        /// </summary>
//        [Test]
//        public void TestSpeedOfSoundEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.SpeedOfSound = 1400;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x78, data[14], "Speed of Sound LSB Min Encode is incorrect.");
//            Assert.AreEqual(0x05, data[15], "Speed of Sound MSB Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Speed of Sound maximum.
//        /// </summary>
//        [Test]
//        public void TestSpeedOfSoundEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.SpeedOfSound = 1600;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x40, data[14], "Speed of Sound LSB Max Encode is incorrect.");
//            Assert.AreEqual(0x06, data[15], "Speed of Sound MSB Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Depth of Transducer

//        #region Decode

//        /// <summary>
//        /// Test decoding the Depth of Transducer.
//        /// </summary>
//        [Test]
//        public void TestDepthOfTransducerDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[16] = 0xDC;
//            data[17] = 0x05;
//            vl.Decode(data);
//            Assert.AreEqual(1500, vl.DepthOfTransducer, "Depth of Transducer Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Depth of Transducer.
//        /// </summary>
//        [Test]
//        public void TestDepthOfTransducerEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.DepthOfTransducer = 1500;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xDC, data[16], "Depth of Transducer LSB Encode is incorrect.");
//            Assert.AreEqual(0x05, data[17], "Depth of Transducer MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Depth of Transducer minimum.
//        /// </summary>
//        [Test]
//        public void TestDepthOfTransducerEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.DepthOfTransducer = 1;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x01, data[16], "Depth of Transducer LSB Min Encode is incorrect.");
//            Assert.AreEqual(0x00, data[17], "Depth of Transducer MSB Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Depth of Transducer maximum.
//        /// </summary>
//        [Test]
//        public void TestDepthOfTransducerEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.DepthOfTransducer = 9999;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x0F, data[16], "Depth of Transducer LSB Max Encode is incorrect.");
//            Assert.AreEqual(0x27, data[17], "Depth of Transducer MSB Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Heading

//        #region Decode

//        /// <summary>
//        /// Test decoding the Heading.
//        /// </summary>
//        [Test]
//        public void TestHeadingDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[18] = 0xBA;
//            data[19] = 0x0A;
//            vl.Decode(data);
//            Assert.AreEqual(27.46f, vl.Heading, 0.001, "Heading Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Heading.
//        /// </summary>
//        [Test]
//        public void TestHeadingEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Heading = 27.46f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xBA, data[18], "Heading LSB Encode is incorrect.");
//            Assert.AreEqual(0x0A, data[19], "Heading MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Heading minimum.
//        /// </summary>
//        [Test]
//        public void TestHeadingEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Heading = 0.00f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[18], "Heading LSB Min Encode is incorrect.");
//            Assert.AreEqual(0x00, data[19], "Heading MSB Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Heading maximum.
//        /// </summary>
//        [Test]
//        public void TestHeadingEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Heading = 359.99f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x9F, data[18], "Heading LSB Max Encode is incorrect.");
//            Assert.AreEqual(0x8C, data[19], "Heading MSB Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Pitch

//        #region Decode

//        /// <summary>
//        /// Test decoding the Pitch.
//        /// </summary>
//        [Test]
//        public void TestPitchDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[20] = 0xD2;
//            data[21] = 0x06;
//            vl.Decode(data);
//            Assert.AreEqual(17.46f, vl.Pitch, 0.001, "Pitch Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Pitch.
//        /// </summary>
//        [Test]
//        public void TestPitchEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Pitch = 17.46f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xD2, data[20], "Pitch LSB Encode is incorrect.");
//            Assert.AreEqual(0x06, data[21], "Pitch MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Pitch minimum.
//        /// </summary>
//        [Test]
//        public void TestPitchEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Pitch = -20.00f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x30, data[20], "Pitch LSB Min Encode is incorrect.");
//            Assert.AreEqual(0xF8, data[21], "Pitch MSB Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Pitch maximum.
//        /// </summary>
//        [Test]
//        public void TestPitchEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Pitch = 20.00f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xD0, data[20], "Pitch LSB Max Encode is incorrect.");
//            Assert.AreEqual(0x07, data[21], "Pitch MSB Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Roll

//        #region Decode

//        /// <summary>
//        /// Test decoding the Roll.
//        /// </summary>
//        [Test]
//        public void TestRollDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[22] = 0xD2;
//            data[23] = 0x06;
//            vl.Decode(data);
//            Assert.AreEqual(17.46f, vl.Roll, 0.001, "Roll Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Roll.
//        /// </summary>
//        [Test]
//        public void TestRollEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Roll = 17.46f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xD2, data[22], "Roll LSB Encode is incorrect.");
//            Assert.AreEqual(0x06, data[23], "Roll MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Roll minimum.
//        /// </summary>
//        [Test]
//        public void TestRollEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Roll = -20.00f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x30, data[22], "Roll LSB Min Encode is incorrect.");
//            Assert.AreEqual(0xF8, data[23], "Roll MSB Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Roll maximum.
//        /// </summary>
//        [Test]
//        public void TestRollEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Roll = 20.00f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xD0, data[22], "Roll LSB Max Encode is incorrect.");
//            Assert.AreEqual(0x07, data[23], "Roll MSB Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Salinity

//        #region Decode

//        /// <summary>
//        /// Test decoding the Salinity.
//        /// </summary>
//        [Test]
//        public void TestSalinityDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[24] = 0x1E;
//            data[25] = 0x00;
//            vl.Decode(data);
//            Assert.AreEqual(30, vl.Salinity, "Salinity Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Salinity.
//        /// </summary>
//        [Test]
//        public void TestSalinityEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Salinity = 30;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x1E, data[24], "Salinity LSB Encode is incorrect.");
//            Assert.AreEqual(0x00, data[25], "Salinity MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Salinity minimum.
//        /// </summary>
//        [Test]
//        public void TestSalinityEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Salinity = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[24], "Salinity LSB Min Encode is incorrect.");
//            Assert.AreEqual(0x00, data[25], "Salinity MSB Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Salinity maximum.
//        /// </summary>
//        [Test]
//        public void TestSalinityEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Salinity = 40;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x28, data[24], "Salinity LSB Max Encode is incorrect.");
//            Assert.AreEqual(0x00, data[25], "Salinity MSB Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Temperature

//        #region Decode

//        /// <summary>
//        /// Test decoding the Temperature.
//        /// </summary>
//        [Test]
//        public void TestTemperatureDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[26] = 0xD2;
//            data[27] = 0x06;
//            vl.Decode(data);
//            Assert.AreEqual(17.46f, vl.Temperature, 0.001, "Temperature Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Temperature.
//        /// </summary>
//        [Test]
//        public void TestTemperatureEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Temperature = 17.46f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xD2, data[26], "Temperature LSB Encode is incorrect.");
//            Assert.AreEqual(0x06, data[27], "Temperature MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Temperature minimum.
//        /// </summary>
//        [Test]
//        public void TestTemperatureEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Temperature = -5.00f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x0C, data[26], "Temperature LSB Min Encode is incorrect.");
//            Assert.AreEqual(0xFE, data[27], "Temperature MSB Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Temperature maximum.
//        /// </summary>
//        [Test]
//        public void TestTemperatureEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Temperature = 40.00f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xA0, data[26], "Temperature LSB Max Encode is incorrect.");
//            Assert.AreEqual(0x0F, data[27], "Temperature MSB Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region MPT Minutes

//        #region Decode

//        /// <summary>
//        /// Test decoding the MPT Minutes.
//        /// </summary>
//        [Test]
//        public void TestMptMinutesDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[28] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.MinPrePingWaitTimeMinutes, "MPT Minutes Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the MPT Minutes.
//        /// </summary>
//        [Test]
//        public void TestMptMinutesEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.MinPrePingWaitTimeMinutes = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[28], "MPT Minutes Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the MPT Minutes minimum.
//        /// </summary>
//        [Test]
//        public void TestMptMinutesEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.MinPrePingWaitTimeMinutes = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[28], "MPT Minutes Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the MPT Minutes maximum.
//        /// </summary>
//        [Test]
//        public void TestMptMinutesEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.MinPrePingWaitTimeMinutes = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[28], "MPT Minutes Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region MPT Second

//        #region Decode

//        /// <summary>
//        /// Test decoding the MPT Second.
//        /// </summary>
//        [Test]
//        public void TestMptSecondDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[29] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.MinPrePingWaitTimeSeconds, "MPT Second Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the MPT Second.
//        /// </summary>
//        [Test]
//        public void TestMptSecondEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.MinPrePingWaitTimeSeconds = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[29], "MPT Second Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the MPT Second minimum.
//        /// </summary>
//        [Test]
//        public void TestMptSecondEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.MinPrePingWaitTimeSeconds = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[29], "MPT Second Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the MPT Second maximum.
//        /// </summary>
//        [Test]
//        public void TestMptSecondEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.MinPrePingWaitTimeSeconds = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[29], "MPT Second Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region MPT Hundredths

//        #region Decode

//        /// <summary>
//        /// Test decoding the MPT Hundredths.
//        /// </summary>
//        [Test]
//        public void TestMptHundredthsDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[30] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.MinPrePingWaitTimeHundredths, "MPT Hundredths Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the MPT Hundredths.
//        /// </summary>
//        [Test]
//        public void TestMptHundredthsEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.MinPrePingWaitTimeHundredths = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[30], "MPT Hundredths Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the MPT Hundredths minimum.
//        /// </summary>
//        [Test]
//        public void TestMptHundredthsEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.MinPrePingWaitTimeHundredths = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[30], "MPT Hundredths Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the MPT Hundredths maximum.
//        /// </summary>
//        [Test]
//        public void TestMptHundredthsEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.MinPrePingWaitTimeHundredths = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[30], "MPT Hundredths Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Heading Standard Dev

//        #region Decode

//        /// <summary>
//        /// Test decoding the Heading Standard Dev.
//        /// </summary>
//        [Test]
//        public void TestHeadingStdDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[31] = 0x78;
//            vl.Decode(data);
//            Assert.AreEqual(120, vl.HeadingStdDev, "Heading Standard Dev Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Heading Standard Dev.
//        /// </summary>
//        [Test]
//        public void TestHeadingStdEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.HeadingStdDev = 120;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x78, data[31], "Heading Standard Dev Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Heading Standard Dev minimum.
//        /// </summary>
//        [Test]
//        public void TestHeadingStdEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.HeadingStdDev = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[31], "Heading Standard Dev Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Heading Standard Dev maximum.
//        /// </summary>
//        [Test]
//        public void TestHeadingStdEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.HeadingStdDev = 180;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xB4, data[31], "Heading Standard Dev Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Pitch Standard Dev

//        #region Decode

//        /// <summary>
//        /// Test decoding the Pitch Standard Dev.
//        /// </summary>
//        [Test]
//        public void TestPitchStdDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[32] = 0x8E;
//            vl.Decode(data);
//            Assert.AreEqual(14.2, vl.PitchStdDev, 0.01f, "Pitch Standard Dev Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Pitch Standard Dev.
//        /// </summary>
//        [Test]
//        public void TestPitchStdEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.PitchStdDev = 14.2f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x8E, data[32], 0.01f, "Pitch Standard Dev Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Pitch Standard Dev minimum.
//        /// </summary>
//        [Test]
//        public void TestPitchStdEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.PitchStdDev = 0.0f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[32], "Pitch Standard Dev Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Pitch Standard Dev maximum.
//        /// </summary>
//        [Test]
//        public void TestPitchStdEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.PitchStdDev = 20.0f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xC8, data[32], "Pitch Standard Dev Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Roll Standard Dev

//        #region Decode

//        /// <summary>
//        /// Test decoding the Roll Standard Dev.
//        /// </summary>
//        [Test]
//        public void TestRollStdDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[33] = 0x8E;
//            vl.Decode(data);
//            Assert.AreEqual(14.2, vl.RollStdDev, 0.01f, "Roll Standard Dev Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Roll Standard Dev.
//        /// </summary>
//        [Test]
//        public void TestRollStdEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RollStdDev = 14.2f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x8E, data[33], 0.01f, "Roll Standard Dev Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Roll Standard Dev minimum.
//        /// </summary>
//        [Test]
//        public void TestRollStdEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RollStdDev = 0.0f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[33], "Roll Standard Dev Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Roll Standard Dev maximum.
//        /// </summary>
//        [Test]
//        public void TestRollStdEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RollStdDev = 20.0f;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xC8, data[33], "Roll Standard Dev Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region ADC Channel 0

//        #region Decode

//        /// <summary>
//        /// Test decoding the ADC Channel 0.
//        /// </summary>
//        [Test]
//        public void TestAdc0Decode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[34] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.Adc0, "ADC Channel 0 Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the ADC Channel 0.
//        /// </summary>
//        [Test]
//        public void TestAdc0Encode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc0 = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[34], "ADC Channel 0 Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 0 minimum.
//        /// </summary>
//        [Test]
//        public void TestAdc0EncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc0 = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[34], "ADC Channel 0 Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 0 maximum.
//        /// </summary>
//        [Test]
//        public void TestAdc0EncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc0 = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[34], "ADC Channel 0 Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region ADC Channel 1

//        #region Decode

//        /// <summary>
//        /// Test decoding the ADC Channel 1.
//        /// </summary>
//        [Test]
//        public void TestAdc1Decode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[35] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.Adc1, "ADC Channel 1 Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the ADC Channel 1.
//        /// </summary>
//        [Test]
//        public void TestAdc1Encode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc1 = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[35], "ADC Channel 1 Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 1 minimum.
//        /// </summary>
//        [Test]
//        public void TestAdc1EncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc1 = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[35], "ADC Channel 1 Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 1 maximum.
//        /// </summary>
//        [Test]
//        public void TestAdc1EncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc1 = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[35], "ADC Channel 1 Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region ADC Channel 2

//        #region Decode

//        /// <summary>
//        /// Test decoding the ADC Channel 2.
//        /// </summary>
//        [Test]
//        public void TestAdc2Decode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[36] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.Adc2, "ADC Channel 2 Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the ADC Channel 2.
//        /// </summary>
//        [Test]
//        public void TestAdc2Encode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc2 = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[36], "ADC Channel 2 Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 2 minimum.
//        /// </summary>
//        [Test]
//        public void TestAdc2EncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc2 = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[36], "ADC Channel 2 Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 2 maximum.
//        /// </summary>
//        [Test]
//        public void TestAdc2EncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc2 = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[36], "ADC Channel 2 Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region ADC Channel 3

//        #region Decode

//        /// <summary>
//        /// Test decoding the ADC Channel 3.
//        /// </summary>
//        [Test]
//        public void TestAdc3Decode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[37] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.Adc3, "ADC Channel 3 Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the ADC Channel 3.
//        /// </summary>
//        [Test]
//        public void TestAdc3Encode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc3 = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[37], "ADC Channel 3 Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 3 minimum.
//        /// </summary>
//        [Test]
//        public void TestAdc3EncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc3 = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[37], "ADC Channel 3 Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 3 maximum.
//        /// </summary>
//        [Test]
//        public void TestAdc3EncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc3 = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[37], "ADC Channel 3 Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region ADC Channel 4

//        #region Decode

//        /// <summary>
//        /// Test decoding the ADC Channel 4.
//        /// </summary>
//        [Test]
//        public void TestAdc4Decode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[38] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.Adc4, "ADC Channel 4 Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the ADC Channel 4.
//        /// </summary>
//        [Test]
//        public void TestAdc4Encode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc4 = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[38], "ADC Channel 4 Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 4 minimum.
//        /// </summary>
//        [Test]
//        public void TestAdc4EncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc4 = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[38], "ADC Channel 4 Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 4 maximum.
//        /// </summary>
//        [Test]
//        public void TestAdc4EncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc4 = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[38], "ADC Channel 4 Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region ADC Channel 5

//        #region Decode

//        /// <summary>
//        /// Test decoding the ADC Channel 5.
//        /// </summary>
//        [Test]
//        public void TestAdc5Decode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[39] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.Adc5, "ADC Channel 5 Decode is incorrect.");

//        }



//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the ADC Channel 5.
//        /// </summary>
//        [Test]
//        public void TestAdc5Encode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc5 = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[39], "ADC Channel 5 Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 5 minimum.
//        /// </summary>
//        [Test]
//        public void TestAdc5EncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc5 = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[39], "ADC Channel 5 Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 5 maximum.
//        /// </summary>
//        [Test]
//        public void TestAdc5EncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc5 = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[39], "ADC Channel 5 Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region ADC Channel 6

//        #region Decode

//        /// <summary>
//        /// Test decoding the ADC Channel 6.
//        /// </summary>
//        [Test]
//        public void TestAdc6Decode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[40] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.Adc6, "ADC Channel 6 Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the ADC Channel 6.
//        /// </summary>
//        [Test]
//        public void TestAdc6Encode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc6 = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[40], "ADC Channel 6 Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 6 minimum.
//        /// </summary>
//        [Test]
//        public void TestAdc6EncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc6 = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[40], "ADC Channel 6 Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 6 maximum.
//        /// </summary>
//        [Test]
//        public void TestAdc6EncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc6 = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[40], "ADC Channel 6 Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region ADC Channel 7

//        #region Decode

//        /// <summary>
//        /// Test decoding the ADC Channel 7.
//        /// </summary>
//        [Test]
//        public void TestAdc7Decode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[41] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.Adc7, "ADC Channel 7 Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the ADC Channel 7.
//        /// </summary>
//        [Test]
//        public void TestAdc7Encode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc7 = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[41], "ADC Channel 7 Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 7 minimum.
//        /// </summary>
//        [Test]
//        public void TestAdc7EncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc7 = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[41], "ADC Channel 7 Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the ADC Channel 7 maximum.
//        /// </summary>
//        [Test]
//        public void TestAdc7EncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Adc7 = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[41], "ADC Channel 7 Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Error Status Word

//        #region ESW0

//        #region Encode

//        /// <summary>
//        /// Test encoding the ESW0 result.
//        /// </summary>
//        [Test]
//        public void TestEsw0Encode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.SetBatterySaverPower();
//            vl.SetWatchdogRestartOccured();
//            vl.SetUnassignedException();
//            vl.SetEmulatorException();
//            vl.SetZeroDivideException();
//            vl.SetIllegalInstructionException();
//            vl.SetAddressErrorException();
//            vl.SetBusErrorException();

//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[42], "ESW0 Encode is incorrect.");
//        }

//        #endregion

//        #region Decode

//        /// <summary>
//        /// Test Decoding the Error Status Word 0 result.
//        /// </summary>
//        [Test]
//        public void TestEsw0Decode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[42] = 0xFF;
//            vl.Decode(data);

//            Assert.IsTrue(vl.IsBusErrorExceptionSet(), "IsBusErrorExceptionSet() is incorrect.");
//            Assert.IsTrue(vl.IsAddressErrorExceptionSet(), "IsAddressErrorExceptionSet() is incorrect.");
//            Assert.IsTrue(vl.IsIllegalInstructionExceptionSet(), "IsIllegalInstructionExceptionSet() is incorrect.");
//            Assert.IsTrue(vl.IsZeroDivideExceptionSet(), "IsZeroDivideExceptionSet() is incorrect.");
//            Assert.IsTrue(vl.IsEmulatorExceptionSet(), "IsEmulatorExceptionSet() is incorrect.");
//            Assert.IsTrue(vl.IsUnassignedExceptionSet(), "IsUnassignedExceptionSet() is incorrect.");
//            Assert.IsTrue(vl.IsWatchdogRestartExceptionSet(), "IsWatchdogRestartExceptionSet() is incorrect.");
//            Assert.IsTrue(vl.IsBatterySaverPowerSet(), "IsBatterySaverPowerSet() is incorrect.");


//        }


//        #endregion

//        /// <summary>
//        /// Test if Error Status Word 0.
//        /// Bus Error Exception.
//        /// </summary>
//        [Test]
//        public void TestBusErrorException()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetBusErrorException();
//            Assert.IsTrue(vl.IsBusErrorExceptionSet(), "IsBusErrorExceptionSet true is incorrect.");

//            // Test False
//            vl.UnSetBusErrorException();
//            Assert.IsFalse(vl.IsBusErrorExceptionSet(), "IsBusErrorExceptionSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 0.
//        /// Address Error Exception.
//        /// </summary>
//        [Test]
//        public void TestAddressErrorException()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetAddressErrorException();
//            Assert.IsTrue(vl.IsAddressErrorExceptionSet(), "IsAddressErrorExceptionSet true is incorrect.");

//            // Test False
//            vl.UnSetAddressErrorException();
//            Assert.IsFalse(vl.IsAddressErrorExceptionSet(), "IsAddressErrorExceptionSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 0.
//        /// Illegal Instruction Exception.
//        /// </summary>
//        [Test]
//        public void TestIllegalInstructionException()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetIllegalInstructionException();
//            Assert.IsTrue(vl.IsIllegalInstructionExceptionSet(), "IsIllegalInstructionExceptionSet true is incorrect.");

//            // Test False
//            vl.UnSetIllegalInstructionException();
//            Assert.IsFalse(vl.IsIllegalInstructionExceptionSet(), "IsIllegalInstructionExceptionSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 0.
//        /// Zero Divide Exception.
//        /// </summary>
//        [Test]
//        public void TestZeroDivideException()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetZeroDivideException();
//            Assert.IsTrue(vl.IsZeroDivideExceptionSet(), "IsZeroDivideExceptionSet true is incorrect.");

//            // Test False
//            vl.UnSetZeroDivideException();
//            Assert.IsFalse(vl.IsZeroDivideExceptionSet(), "IsZeroDivideExceptionSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 0.
//        /// Emulator Exception.
//        /// </summary>
//        [Test]
//        public void TestEmulatorException()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetEmulatorException();
//            Assert.IsTrue(vl.IsEmulatorExceptionSet(), "IsEmulatorExceptionSet true is incorrect.");

//            // Test False
//            vl.UnSetEmulatorException();
//            Assert.IsFalse(vl.IsEmulatorExceptionSet(), "IsEmulatorExceptionSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 0.
//        /// Unassigned Exception.
//        /// </summary>
//        [Test]
//        public void TestUnassignedException()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetUnassignedException();
//            Assert.IsTrue(vl.IsUnassignedExceptionSet(), "IsUnassignedExceptionSet true is incorrect.");

//            // Test False
//            vl.UnSetUnassignedException();
//            Assert.IsFalse(vl.IsUnassignedExceptionSet(), "IsUnassignedExceptionSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 0.
//        /// Watchdog Restart Exception.
//        /// </summary>
//        [Test]
//        public void TestWatchdogRestartException()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetWatchdogRestartOccured();
//            Assert.IsTrue(vl.IsWatchdogRestartExceptionSet(), "IsWatchdogRestartExceptionSet true is incorrect.");

//            // Test False
//            vl.UnSetWatchdogRestartOccured();
//            Assert.IsFalse(vl.IsWatchdogRestartExceptionSet(), "IsWatchdogRestartExceptionSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 0.
//        /// Battery Saver Power.
//        /// </summary>
//        [Test]
//        public void TestBatterySaverPower()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetBatterySaverPower();
//            Assert.IsTrue(vl.IsBatterySaverPowerSet(), "IsBatterySaverPowerSet true is incorrect.");

//            // Test False
//            vl.UnSetBatterySaverPower();
//            Assert.IsFalse(vl.IsBatterySaverPowerSet(), "IsBatterySaverPowerSet false is incorrect.");
//        }


//        #endregion

//        #region ESW1

//        #region Encode

//        /// <summary>
//        /// Test encoding the ESW1 result.
//        /// </summary>
//        [Test]
//        public void TestEsw1Encode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.SetPinging();
//            vl.SetNotUsed44_01();
//            vl.SetNotUsed44_02();
//            vl.SetNotUsed44_03();
//            vl.SetNotUsed44_04();
//            vl.SetNotUsed44_05();
//            vl.SetColdWakeupOccurred();
//            vl.SetUnknownWakeupOccurred();

//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[43], "ESW1 Encode is incorrect.");
//        }

//        #endregion

//        #region Decode

//        /// <summary>
//        /// Test Decoding the Error Status Word 1 result.
//        /// </summary>
//        [Test]
//        public void TestEsw1Decode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[43] = 0xFF;
//            vl.Decode(data);

//            Assert.IsTrue(vl.IsPingingSet(), "IsPingingSet() is incorrect.");
//            Assert.IsTrue(vl.IsNotUsed44_01Set(), "IsNotUsed44_01Set() is incorrect.");
//            Assert.IsTrue(vl.IsNotUsed44_02Set(), "IsNotUsed44_02Set() is incorrect.");
//            Assert.IsTrue(vl.IsNotUsed44_03Set(), "IsNotUsed44_03Set() is incorrect.");
//            Assert.IsTrue(vl.IsNotUsed44_04Set(), "IsNotUsed44_04Set() is incorrect.");
//            Assert.IsTrue(vl.IsNotUsed44_05Set(), "IsNotUsed44_05Set() is incorrect.");
//            Assert.IsTrue(vl.IsColdWakeupOccuredSet(), "IsColdWakeupOccuredSet() is incorrect.");
//            Assert.IsTrue(vl.IsUnknownWakeupOccurredSet(), "IsColdWakeupOccuredSet() is incorrect.");


//        }


//        #endregion

//        /// <summary>
//        /// Test if Error Status Word 1.
//        /// Pinging.
//        /// </summary>
//        [Test]
//        public void TestPingingException()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetPinging();
//            Assert.IsTrue(vl.IsPingingSet(), "IsPingingSet true is incorrect.");

//            // Test False
//            vl.UnSetPinging();
//            Assert.IsFalse(vl.IsPingingSet(), "IsPingingSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 1.
//        /// Not Used Byte 44, Bit 01.
//        /// </summary>
//        [Test]
//        public void TestNotUsed44_01Exception()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetNotUsed44_01();
//            Assert.IsTrue(vl.IsNotUsed44_01Set(), "IsNotUsed44_01Set true is incorrect.");

//            // Test False
//            vl.UnSetNotUsed44_01();
//            Assert.IsFalse(vl.IsNotUsed44_01Set(), "IsNotUsed44_01Set false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 1.
//        /// Not Used Byte 44, Bit 02.
//        /// </summary>
//        [Test]
//        public void TestNotUsed44_02Exception()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetNotUsed44_02();
//            Assert.IsTrue(vl.IsNotUsed44_02Set(), "IsNotUsed44_02Set true is incorrect.");

//            // Test False
//            vl.UnSetNotUsed44_02();
//            Assert.IsFalse(vl.IsNotUsed44_02Set(), "IsNotUsed44_02Set false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 1.
//        /// Not Used Byte 44, Bit 03.
//        /// </summary>
//        [Test]
//        public void TestNotUsed44_03Exception()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetNotUsed44_03();
//            Assert.IsTrue(vl.IsNotUsed44_03Set(), "IsNotUsed44_03Set true is incorrect.");

//            // Test False
//            vl.UnSetNotUsed44_03();
//            Assert.IsFalse(vl.IsNotUsed44_03Set(), "IsNotUsed44_03Set false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 1.
//        /// Not Used Byte 44, Bit 04.
//        /// </summary>
//        [Test]
//        public void TestNotUsed44_04Exception()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetNotUsed44_04();
//            Assert.IsTrue(vl.IsNotUsed44_04Set(), "IsNotUsed44_04Set true is incorrect.");

//            // Test False
//            vl.UnSetNotUsed44_04();
//            Assert.IsFalse(vl.IsNotUsed44_04Set(), "IsNotUsed44_04Set false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 1.
//        /// Not Used Byte 44, Bit 05.
//        /// </summary>
//        [Test]
//        public void TestNotUsed44_05Exception()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetNotUsed44_05();
//            Assert.IsTrue(vl.IsNotUsed44_05Set(), "IsNotUsed44_05Set true is incorrect.");

//            // Test False
//            vl.UnSetNotUsed44_05();
//            Assert.IsFalse(vl.IsNotUsed44_05Set(), "IsNotUsed44_05Set false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 1.
//        /// Cold WAkeup Occurred.
//        /// </summary>
//        [Test]
//        public void TestColdWakeupOccurred()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetColdWakeupOccurred();
//            Assert.IsTrue(vl.IsColdWakeupOccuredSet(), "IsColdWakeupOccuredSet true is incorrect.");

//            // Test False
//            vl.UnSetColdWakeupOccurred();
//            Assert.IsFalse(vl.IsColdWakeupOccuredSet(), "IsColdWakeupOccuredSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 1.
//        /// Unknown Wakeup Occurred.
//        /// </summary>
//        [Test]
//        public void TestUnknownWakeupOccurred()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetUnknownWakeupOccurred();
//            Assert.IsTrue(vl.IsUnknownWakeupOccurredSet(), "IsUnknownWakeupOccurredSet true is incorrect.");

//            // Test False
//            vl.UnSetUnknownWakeupOccurred();
//            Assert.IsFalse(vl.IsUnknownWakeupOccurredSet(), "IsUnknownWakeupOccurredSet false is incorrect.");
//        }


//        #endregion

//        #region ESW2

//        #region Encode

//        /// <summary>
//        /// Test encoding the ESW2 result.
//        /// </summary>
//        [Test]
//        public void TestEsw2Encode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.SetClockReadErrorOccurred();
//            vl.SetUnexpectedAlarm();
//            vl.SetClockJumpForward();
//            vl.SetClockJumpBackward();
//            vl.SetNotUsed45_04();
//            vl.SetNotUsed45_05();
//            vl.SetNotUsed45_06();
//            vl.SetNotUsed45_07();

//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[44], "ESW2 Encode is incorrect.");
//        }

//        #endregion

//        #region Decode

//        /// <summary>
//        /// Test Decoding the Error Status Word 2 result.
//        /// </summary>
//        [Test]
//        public void TestEsw2Decode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[44] = 0xFF;
//            vl.Decode(data);

//            Assert.IsTrue(vl.IsClockReadErrorSet(), "IsClockReadErrorSet() is incorrect.");
//            Assert.IsTrue(vl.IsUnexpectedAlarmSet(), "IsUnexpectedAlarmSet() is incorrect.");
//            Assert.IsTrue(vl.IsClockJumpForwardSet(), "IsClockJumpForwardSet() is incorrect.");
//            Assert.IsTrue(vl.IsClockJumpBackwardSet(), "IsClockJumpBackwardSet() is incorrect.");
//            Assert.IsTrue(vl.IsNotUsed45_04Set(), "IsNotUsed45_04Set() is incorrect.");
//            Assert.IsTrue(vl.IsNotUsed45_05Set(), "IsNotUsed45_05Set() is incorrect.");
//            Assert.IsTrue(vl.IsNotUsed45_06Set(), "IsNotUsed45_06Set() is incorrect.");
//            Assert.IsTrue(vl.IsNotUsed45_07Set(), "IsNotUsed45_07Set() is incorrect.");


//        }


//        #endregion

//        /// <summary>
//        /// Test if Error Status Word 2.
//        /// Clock Read Error Occurred.
//        /// </summary>
//        [Test]
//        public void TestClockReadErrorOccurred()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetClockReadErrorOccurred();
//            Assert.IsTrue(vl.IsClockReadErrorSet(), "IsClockReadErrorSet true is incorrect.");

//            // Test False
//            vl.UnSetClockReadErrorOccurred();
//            Assert.IsFalse(vl.IsClockReadErrorSet(), "IsClockReadErrorSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 2.
//        /// Unexpected Alarm.
//        /// </summary>
//        [Test]
//        public void TestUnexpectedAlarm()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetUnexpectedAlarm();
//            Assert.IsTrue(vl.IsUnexpectedAlarmSet(), "IsUnexpectedAlarmSet true is incorrect.");

//            // Test False
//            vl.UnSetUnexpectedAlarm();
//            Assert.IsFalse(vl.IsUnexpectedAlarmSet(), "IsUnexpectedAlarmSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 2.
//        /// Clock jump Forward.
//        /// </summary>
//        [Test]
//        public void TestClockJumpForward()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetClockJumpForward();
//            Assert.IsTrue(vl.IsClockJumpForwardSet(), "IsClockJumpForwardSet true is incorrect.");

//            // Test False
//            vl.UnSetClockJumpForward();
//            Assert.IsFalse(vl.IsClockJumpForwardSet(), "IsClockJumpForwardSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 2.
//        /// Clock jump Backward.
//        /// </summary>
//        [Test]
//        public void TestClockJumpBackward()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetClockJumpBackward();
//            Assert.IsTrue(vl.IsClockJumpBackwardSet(), "IsClockJumpBackwardSet true is incorrect.");

//            // Test False
//            vl.UnSetClockJumpBackward();
//            Assert.IsFalse(vl.IsClockJumpBackwardSet(), "IsClockJumpBackwardSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 2.
//        /// Not Used Byte 45, Bit 4.
//        /// </summary>
//        [Test]
//        public void TestNotUsed45_04()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetNotUsed45_04();
//            Assert.IsTrue(vl.IsNotUsed45_04Set(), "IsNotUsed45_04Set true is incorrect.");

//            // Test False
//            vl.UnSetNotUsed45_04();
//            Assert.IsFalse(vl.IsNotUsed45_04Set(), "IsNotUsed45_04Set false is incorrect.");
//        }


//        /// <summary>
//        /// Test if Error Status Word 2.
//        /// Not Used Byte 45, Bit 5.
//        /// </summary>
//        [Test]
//        public void TestNotUsed45_05()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetNotUsed45_05();
//            Assert.IsTrue(vl.IsNotUsed45_05Set(), "IsNotUsed45_05Set true is incorrect.");

//            // Test False
//            vl.UnSetNotUsed45_05();
//            Assert.IsFalse(vl.IsNotUsed45_05Set(), "IsNotUsed45_05Set false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 2.
//        /// Not Used Byte 45, Bit 6.
//        /// </summary>
//        [Test]
//        public void TestNotUsed45_06()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetNotUsed45_06();
//            Assert.IsTrue(vl.IsNotUsed45_06Set(), "IsNotUsed45_06Set true is incorrect.");

//            // Test False
//            vl.UnSetNotUsed45_06();
//            Assert.IsFalse(vl.IsNotUsed45_06Set(), "IsNotUsed45_06Set false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 2.
//        /// Not Used Byte 45, Bit 7.
//        /// </summary>
//        [Test]
//        public void TestNotUsed45_07()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetNotUsed45_07();
//            Assert.IsTrue(vl.IsNotUsed45_07Set(), "IsNotUsed45_07Set true is incorrect.");

//            // Test False
//            vl.UnSetNotUsed45_07();
//            Assert.IsFalse(vl.IsNotUsed45_07Set(), "IsNotUsed45_07Set false is incorrect.");
//        }

//        #endregion

//        #region ESW3

//        #region Encode

//        /// <summary>
//        /// Test encoding the ESW3 result.
//        /// </summary>
//        [Test]
//        public void TestEsw3Encode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.SetNotUsed46_00();
//            vl.SetNotUsed46_01();
//            vl.SetNotUsed46_02();
//            vl.SetPowerFail();
//            vl.SetSpuriousLevel4Interrupt();
//            vl.SetSpuriousLevel5Interrupt();
//            vl.SetSpuriousLevel6Interrupt();
//            vl.SetLevel7InterruptOccurred();

//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[45], "ESW3 Encode is incorrect.");
//        }

//        #endregion

//        #region Decode

//        /// <summary>
//        /// Test Decoding the Error Status Word 3 result.
//        /// </summary>
//        [Test]
//        public void TestEsw3Decode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[45] = 0xFF;
//            vl.Decode(data);

//            Assert.IsTrue(vl.IsNotUsed46_00Set(), "IsNotUsed46_00() is incorrect.");
//            Assert.IsTrue(vl.IsNotUsed46_01Set(), "IsNotUsed46_01() is incorrect.");
//            Assert.IsTrue(vl.IsNotUsed46_02Set(), "IsNotUsed46_02() is incorrect.");
//            Assert.IsTrue(vl.IsPowerFailSet(), "IsPowerFailSet() is incorrect.");
//            Assert.IsTrue(vl.IsSpuriousLevel4InterruptSet(), "IsSpuriousLevel4InterruptSet() is incorrect.");
//            Assert.IsTrue(vl.IsSpuriousLevel5InterruptSet(), "IsSpuriousLevel5InterruptSet() is incorrect.");
//            Assert.IsTrue(vl.IsSpuriousLevel6InterruptSet(), "IsSpuriousLevel6InterruptSet() is incorrect.");
//            Assert.IsTrue(vl.IsLevel7InterruptOccurredSet(), "IsLevel7InterruptOccurredSet() is incorrect.");


//        }


//        #endregion

//        /// <summary>
//        /// Test if Error Status Word 3.
//        /// Not Used Byte 46, Bit 00.
//        /// </summary>
//        [Test]
//        public void TestNotUsed46_00()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetNotUsed46_00();
//            Assert.IsTrue(vl.IsNotUsed46_00Set(), "IsNotUsed46_00Set true is incorrect.");

//            // Test False
//            vl.UnSetNotUsed46_00();
//            Assert.IsFalse(vl.IsNotUsed46_00Set(), "IsNotUsed46_00Set false is incorrect.");
//        }


//        /// <summary>
//        /// Test if Error Status Word 3.
//        /// Not Used Byte 46, Bit 01.
//        /// </summary>
//        [Test]
//        public void TestNotUsed46_01()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetNotUsed46_01();
//            Assert.IsTrue(vl.IsNotUsed46_01Set(), "IsNotUsed46_01Set true is incorrect.");

//            // Test False
//            vl.UnSetNotUsed46_01();
//            Assert.IsFalse(vl.IsNotUsed46_01Set(), "IsNotUsed46_01Set false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 3.
//        /// Not Used Byte 46, Bit 02.
//        /// </summary>
//        [Test]
//        public void TestNotUsed46_02()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetNotUsed46_02();
//            Assert.IsTrue(vl.IsNotUsed46_02Set(), "IsNotUsed46_02Set true is incorrect.");

//            // Test False
//            vl.UnSetNotUsed46_02();
//            Assert.IsFalse(vl.IsNotUsed46_02Set(), "IsNotUsed46_02Set false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 3.
//        /// Power Fail.
//        /// </summary>
//        [Test]
//        public void TestPowerFail()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetPowerFail();
//            Assert.IsTrue(vl.IsPowerFailSet(), "IsPowerFailSet true is incorrect.");

//            // Test False
//            vl.UnSetPowerFail();
//            Assert.IsFalse(vl.IsPowerFailSet(), "IsPowerFailSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 3.
//        /// Spurious Level 4 Interrupt.
//        /// </summary>
//        [Test]
//        public void TestSpuriousLevel4Intr()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetSpuriousLevel4Interrupt();
//            Assert.IsTrue(vl.IsSpuriousLevel4InterruptSet(), "IsSpuriousLevel4InterruptSet true is incorrect.");

//            // Test False
//            vl.UnSetSpuriousLevel4Interrupt();
//            Assert.IsFalse(vl.IsSpuriousLevel4InterruptSet(), "IsSpuriousLevel4InterruptSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 3.
//        /// Spurious Level 5 Interrupt.
//        /// </summary>
//        [Test]
//        public void TestSpuriousLevel5Intr()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetSpuriousLevel5Interrupt();
//            Assert.IsTrue(vl.IsSpuriousLevel5InterruptSet(), "IsSpuriousLevel5InterruptSet true is incorrect.");

//            // Test False
//            vl.UnSetSpuriousLevel5Interrupt();
//            Assert.IsFalse(vl.IsSpuriousLevel5InterruptSet(), "IsSpuriousLevel5InterruptSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 3.
//        /// Spurious Level 6 Interrupt.
//        /// </summary>
//        [Test]
//        public void TestSpuriousLevel6Intr()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetSpuriousLevel6Interrupt();
//            Assert.IsTrue(vl.IsSpuriousLevel6InterruptSet(), "IsSpuriousLevel6InterruptSet true is incorrect.");

//            // Test False
//            vl.UnSetSpuriousLevel6Interrupt();
//            Assert.IsFalse(vl.IsSpuriousLevel6InterruptSet(), "IsSpuriousLevel6InterruptSet false is incorrect.");
//        }

//        /// <summary>
//        /// Test if Error Status Word 3.
//        /// Level 7 Interrupt Occurred.
//        /// </summary>
//        [Test]
//        public void TestLevel7IntrOccurred()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            // Test true
//            vl.SetLevel7InterruptOccurred();
//            Assert.IsTrue(vl.IsLevel7InterruptOccurredSet(), "IsLevel7InterruptOccurredSet true is incorrect.");

//            // Test False
//            vl.UnSetLevel7InterruptOccurred();
//            Assert.IsFalse(vl.IsLevel7InterruptOccurredSet(), "IsLevel7InterruptOccurredSet false is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Reserved

//        #region Decode

//        /// <summary>
//        /// Test decoding the Reserved.
//        /// </summary>
//        [Test]
//        public void TestReservedDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[46] = 0x1E;
//            data[47] = 0x00;
//            vl.Decode(data);
//            Assert.AreEqual(30, vl.Reserved, "Reserved Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Reserved.
//        /// </summary>
//        [Test]
//        public void TestReservedEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Reserved = 30;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x1E, data[46], "Reserved LSB Encode is incorrect.");
//            Assert.AreEqual(0x00, data[47], "Reserved MSB Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Reserved minimum.
//        /// </summary>
//        [Test]
//        public void TestReservedEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Reserved = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[46], "Reserved LSB Min Encode is incorrect.");
//            Assert.AreEqual(0x00, data[47], "Reserved MSB Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Reserved maximum.
//        /// </summary>
//        [Test]
//        public void TestReservedEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Reserved = 40;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x28, data[46], "Reserved LSB Max Encode is incorrect.");
//            Assert.AreEqual(0x00, data[47], "Reserved MSB Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Pressure

//        #region Decode

//        /// <summary>
//        /// Test decoding the Pressure.
//        /// </summary>
//        [Test]
//        public void TestPressureDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[48] = 0x4A;
//            data[49] = 0x7D;
//            data[50] = 0x05;
//            data[51] = 0x00;
//            vl.Decode(data);
//            Assert.AreEqual(359754, vl.Pressure, "Pressure Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Pressure.
//        /// </summary>
//        [Test]
//        public void TestPressureEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Pressure = 359754;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x4A, data[48], "Pressure LSB  Encode is incorrect.");
//            Assert.AreEqual(0x7D, data[49], "Pressure   Encode is incorrect.");
//            Assert.AreEqual(0x05, data[50], "Pressure   Encode is incorrect.");
//            Assert.AreEqual(0x00, data[51], "Pressure MSB  Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Pressure minimum.
//        /// </summary>
//        [Test]
//        public void TestPressureEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Pressure = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[48], "Pressure LSB Min Encode is incorrect.");
//            Assert.AreEqual(0x00, data[49], "Pressure  Min Encode is incorrect.");
//            Assert.AreEqual(0x00, data[50], "Pressure  Min Encode is incorrect.");
//            Assert.AreEqual(0x00, data[51], "Pressure MSB Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Pressure maximum.
//        /// </summary>
//        [Test]
//        public void TestPressureEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Pressure = int.MaxValue;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[48], "Pressure LSB Max Encode is incorrect.");
//            Assert.AreEqual(0xFF, data[49], "Pressure  Max Encode is incorrect.");
//            Assert.AreEqual(0xFF, data[50], "Pressure  Max Encode is incorrect.");
//            Assert.AreEqual(0x7F, data[51], "Pressure MSB Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Pressure Variance

//        #region Decode

//        /// <summary>
//        /// Test decoding the Pressure Variance.
//        /// </summary>
//        [Test]
//        public void TestPressureVarianceDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[52] = 0x4A;
//            data[53] = 0x7D;
//            data[54] = 0x05;
//            data[55] = 0x00;
//            vl.Decode(data);
//            Assert.AreEqual(359754, vl.PressureVariance, "Pressure Variance Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Pressure Variance.
//        /// </summary>
//        [Test]
//        public void TestPressureVarianceEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.PressureVariance = 359754;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x4A, data[52], "Pressure Variance LSB  Encode is incorrect.");
//            Assert.AreEqual(0x7D, data[53], "Pressure Variance   Encode is incorrect.");
//            Assert.AreEqual(0x05, data[54], "Pressure Variance   Encode is incorrect.");
//            Assert.AreEqual(0x00, data[55], "Pressure Variance MSB  Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Pressure Variance minimum.
//        /// </summary>
//        [Test]
//        public void TestPressureVarianceEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.PressureVariance = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[52], "Pressure Variance LSB Min Encode is incorrect.");
//            Assert.AreEqual(0x00, data[53], "Pressure Variance  Min Encode is incorrect.");
//            Assert.AreEqual(0x00, data[54], "Pressure Variance  Min Encode is incorrect.");
//            Assert.AreEqual(0x00, data[55], "Pressure Variance MSB Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Pressure Variance maximum.
//        /// </summary>
//        [Test]
//        public void TestPressureVarianceEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.PressureVariance = int.MaxValue;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[52], "Pressure Variance LSB Max Encode is incorrect.");
//            Assert.AreEqual(0xFF, data[53], "Pressure Variance  Max Encode is incorrect.");
//            Assert.AreEqual(0xFF, data[54], "Pressure Variance  Max Encode is incorrect.");
//            Assert.AreEqual(0x7F, data[55], "Pressure Variance MSB Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region Spare

//        #region Decode

//        /// <summary>
//        /// Test decoding the Spare.
//        /// </summary>
//        [Test]
//        public void TestSpareDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[56] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.Spare, "Spare Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the Spare.
//        /// </summary>
//        [Test]
//        public void TestSpareEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Spare = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[56], "Spare Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Spare minimum.
//        /// </summary>
//        [Test]
//        public void TestSpareEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Spare = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[56], "Spare Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the Spare maximum.
//        /// </summary>
//        [Test]
//        public void TestSpareEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.Spare = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[56], "Spare Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Y2K Century

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Century.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kCenturyDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[57] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcY2kCentury, "RTC Century Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Century.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kCenturyEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kCentury = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[57], "RTC Century Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Century minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kCenturyEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kCentury = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[57], "RTC Century Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Century maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kCenturyEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kCentury = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[57], "RTC Century Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Y2K Year

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Year.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kYearDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[58] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcY2kYear, "RTC Year Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Year.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kYearEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kYear = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[58], "RTC Year Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Year minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kYearEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kYear = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[58], "RTC Year Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Year maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kYearEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kYear = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[58], "RTC Year Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Y2K Month

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Month.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kMonthDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[59] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcY2kMonth, "RTC Month Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Month.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kMonthEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kMonth = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[59], "RTC Month Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Month minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kMonthEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kMonth = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[59], "RTC Month Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Month maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kMonthEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kMonth = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[59], "RTC Month Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Y2K Day

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Day.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kDayDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[60] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcY2kDay, "RTC Day Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Day.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kDayEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kDay = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[60], "RTC Day Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Day minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kDayEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kDay = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[60], "RTC Day Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Day maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kDayEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kDay = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[60], "RTC Day Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Y2K Hour

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Hour.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kHourDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[61] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcY2kHour, "RTC Hour Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Hour.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kHourEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kHour = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[61], "RTC Hour Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Hour minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kHourEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kHour = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[61], "RTC Hour Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Hour maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kHourEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kHour = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[61], "RTC Hour Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Y2K Minute

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Minute.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kMinuterDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[62] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcY2kMinute, "RTC Minute Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Minute.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kMinuteEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kMinute = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[62], "RTC Minute Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Minute minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kMinuteEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kMinute = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[62], "RTC Minute Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Minute maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kMinuteEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kMinute = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[62], "RTC Minute Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Y2K Second

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Second.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kSecondrDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[63] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcY2kSecond, "RTC Second Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Second.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kSecondEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kSecond = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[63], "RTC Second Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Second minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kSecondEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kSecond = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[63], "RTC Second Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Second maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kSecondEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kSecond = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[63], "RTC Second Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTC Y2K Hundredth

//        #region Decode

//        /// <summary>
//        /// Test decoding the RTC Hundredth.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kHundredthDecode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            byte[] data = new byte[Pd0VariableLeader.DATATYPE_SIZE];

//            // Decode the data 
//            data[64] = 0xF3;
//            vl.Decode(data);
//            Assert.AreEqual(243, vl.RtcY2kHundredth, "RTC Hundredth Decode is incorrect.");

//        }

//        #endregion

//        #region Encode

//        /// <summary>
//        /// Test encoding the RTC Hundredth.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kHundredthEncode()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kHundredth = 243;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xF3, data[64], "RTC Hundredth Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Hundredth minimum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kHundredthEncodeMin()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kHundredth = 0;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0x00, data[64], "RTC Hundredth Min Encode is incorrect.");
//        }

//        /// <summary>
//        /// Test encoding the RTC Hundredth maximum.
//        /// </summary>
//        [Test]
//        public void TestRtcY2kHundredthEncodeMax()
//        {
//            Pd0VariableLeader vl = new Pd0VariableLeader();

//            vl.RtcY2kHundredth = 255;
//            byte[] data = vl.Encode();
//            Assert.AreEqual(0xFF, data[64], "RTC Hundredth Max Encode is incorrect.");
//        }

//        #endregion

//        #endregion

//        #region RTI Decode

//        /// <summary>
//        /// Test decoding RTI Ensemble and Ancillary data to PD0 Variable Leader.
//        /// </summary>
//        [Test]
//        public void DecodeRtiTest()
//        {
//            DataSet.EnsembleDataSet ens = new DataSet.EnsembleDataSet(DataSet.Ensemble.DATATYPE_INT,                        // Type of data stored (Float or Int)
//                                                                            30,                                             // Number of bins
//                                                                            4,                                              // Number of beams
//                                                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
//                                                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
//                                                                            DataSet.Ensemble.EnsembleDataID, 30, 4);               // Dataset ID
//            DataSet.AncillaryDataSet anc = new DataSet.AncillaryDataSet(DataSet.Ensemble.DATATYPE_FLOAT,                    // Type of data stored (Float or Int)
//                                                                            30,                                             // Number of bins
//                                                                            4,                                              // Number of beams
//                                                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
//                                                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
//                                                                            DataSet.Ensemble.AncillaryID);                  // Dataset ID

//            ens.EnsembleNumber = 555555;
//            //ens.EnsembleNumber = 65537;
//            ens.Year = DateTime.Now.Year;
//            ens.Month = DateTime.Now.Month;
//            ens.Day = DateTime.Now.Day;
//            ens.Hour = DateTime.Now.Hour;
//            ens.Minute = DateTime.Now.Minute;
//            ens.Second = 0;
//            ens.HSec = 0;
//            ens.Status.Value = 0x0002;
//            anc.SpeedOfSound = 1234;
//            anc.TransducerDepth = 2345;
//            anc.Heading = 125.65f;
//            anc.Pitch = 142.65f;
//            anc.Roll = 32.56f;
//            anc.Salinity = 89.75f;
//            anc.WaterTemp = 22.34f;
//            anc.Pressure = 2350000f;

//            Pd0VariableLeader vl = new Pd0VariableLeader(ens, anc);

//            Assert.AreEqual(31275, vl.EnsembleNumber, "Ensemble Number is incorrect.");
//            Assert.AreEqual(8, vl.EnsembleNumRollover, "Ensemble Rollover is incorrect.");
//            //Assert.AreEqual(1, vl.EnsembleNumber, "Ensemble Number is incorrect.");
//            //Assert.AreEqual(1, vl.EnsembleNumRollover, "Ensemble Rollover is incorrect."); 
//            Assert.AreEqual(DateTime.Now.Year - 2000, vl.RtcYear, "Year is incorrect.");
//            Assert.AreEqual(DateTime.Now.Month, vl.RtcMonth, "Month is incorrect.");
//            Assert.AreEqual(DateTime.Now.Day, vl.RtcDay, "Day is incorrect.");
//            Assert.AreEqual(DateTime.Now.Hour, vl.RtcHour, "Hour is incorrect.");
//            Assert.AreEqual(DateTime.Now.Minute, vl.RtcMinute, "Minute is incorrect.");
//            Assert.AreEqual(0, vl.RtcSecond, "Second is incorrect.");
//            Assert.AreEqual(0, vl.RtcHundredths, "Hundredth is incorrect.");
//            Assert.IsTrue(vl.IsTimingCardErrorSet(), "Bit Result is incorrect.");
//            Assert.AreEqual(1234, vl.SpeedOfSound, "Speed of Sound is incorrect.");
//            Assert.AreEqual(23450, vl.DepthOfTransducer, "Depth of Transducer is incorrect.");
//            Assert.AreEqual(125.65, vl.Heading, 0.001f, "Heading is incorrect.");
//            Assert.AreEqual(142.65, vl.Pitch, 0.001f, "Pitch is incorrect.");
//            Assert.AreEqual(32.56, vl.Roll, 0.001f, "Roll is incorrect.");
//            Assert.AreEqual(90, vl.Salinity, "Salinity is incorrect.");
//            Assert.AreEqual(22.34, vl.Temperature, 0.001f, "Temperature is incorrect.");
//            Assert.AreEqual(235, vl.Pressure, "Pressure is incorrect.");
//        }

//        #endregion
//    }
//}
