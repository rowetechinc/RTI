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
 * 12/27/2013      RC          2.21.1     Updated test to show the error codes.
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
    /// Test the Status class.
    /// </summary>
    [TestFixture]
    public class StatusTest
    {
        /// <summary>
        /// Test the constructor.
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            Status status = new Status(0);

            Assert.AreEqual(0, status.Value, "Status value is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(false, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(false, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(false, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(false, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(false, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(false, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(false, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(false, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("Good", status.ToString(), "ToString is incorrect.");

            Assert.AreEqual(true, status == new Status(0), "== is incorrect.");
            Assert.AreEqual(true, status != new Status(1), "!= is incorrect.");
            Assert.AreEqual(true, new Status(0).Equals(status), "Equal is incorrect.");
        }

        /// <summary>
        /// Test 0x0001
        /// </summary>
        [Test]
        public void TestConstructor1()
        {
            Status status = new Status(0x0001);

            Assert.AreEqual(1, status.Value, "Status value is incorrect.");
            Assert.AreEqual(true, status.IsBottomTrackLongLag(), "BT Long Lag is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(false, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(false, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(false, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(false, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(false, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(false, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(false, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(false, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("0x0001, " + Status.STR_WT_3_BEAM_SOLUTION, status.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Test 0x0002
        /// </summary>
        [Test]
        public void TestConstructor2()
        {
            Status status = new Status(0x0002);

            Assert.AreEqual(0x0002, status.Value, "Status value is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackLongLag(), "WT 3 Beam Solution is incorrect.");
            Assert.AreEqual(true, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(false, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(false, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(false, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(false, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(false, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(false, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(false, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(false, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("0x0002, " + Status.STR_BT_3_BEAM_SOLUTION , status.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Test 0x0004
        /// </summary>
        [Test]
        public void TestConstructor4()
        {
            Status status = new Status(0x0004);

            Assert.AreEqual(0x0004, status.Value, "Status value is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackLongLag(), "WT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(true, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(false, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(false, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(false, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(false, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(false, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(false, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(false, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(false, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("0x0004, " + Status.STR_BT_HOLD, status.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Test 0x0008
        /// </summary>
        [Test]
        public void TestConstructor8()
        {
            Status status = new Status(0x0008);

            Assert.AreEqual(0x0008, status.Value, "Status value is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackLongLag(), "WT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(true, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(false, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(false, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(false, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(false, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(false, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(false, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(false, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(false, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("0x0008, " + Status.STR_BT_SEARCHING, status.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Test 0x0100
        /// </summary>
        [Test]
        public void TestConstructor0x0100()
        {
            Status status = new Status(0x0100);

            Assert.AreEqual(0x0100, status.Value, "Status value is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackLongLag(), "WT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(true, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(false, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(false, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(false, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(false, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(false, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(false, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(false, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("0x0100, " + Status.STR_HDG_SENSOR_ERR, status.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Test 0x0200
        /// </summary>
        [Test]
        public void TestConstructor0x0200()
        {
            Status status = new Status(0x0200);

            Assert.AreEqual(0x0200, status.Value, "Status value is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackLongLag(), "WT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(false, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(false, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(false, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(true, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(false, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(false, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(false, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(false, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("0x0200, " + Status.STR_PRESSURE_SENSOR_ERR, status.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Test 0x0400
        /// </summary>
        [Test]
        public void TestConstructor0x0400()
        {
            Status status = new Status(0x0400);

            Assert.AreEqual(0x0400, status.Value, "Status value is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackLongLag(), "WT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(false, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(false, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(true, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(false, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(false, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(false, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(false, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(false, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("0x0400, " + Status.STR_PWR_DOWN_ERR, status.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Test 0x0800
        /// </summary>
        [Test]
        public void TestConstructor0x0800()
        {
            Status status = new Status(0x0800);

            Assert.AreEqual(0x0800, status.Value, "Status value is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackLongLag(), "WT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(false, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(true, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(false, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(false, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(false, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(false, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(false, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(false, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("0x0800, " + Status.STR_NONVOLATILE_STORAGE_ERR, status.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Test 0x1000
        /// </summary>
        [Test]
        public void TestConstructor0x1000()
        {
            Status status = new Status(0x1000);

            Assert.AreEqual(0x1000, status.Value, "Status value is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackLongLag(), "WT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(false, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(false, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(false, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(false, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(true, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(false, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(false, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(false, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("0x1000, " + Status.STR_RTC_ERR, status.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Test 0x2000
        /// </summary>
        [Test]
        public void TestConstructor0x2000()
        {
            Status status = new Status(0x2000);

            Assert.AreEqual(0x2000, status.Value, "Status value is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackLongLag(), "WT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(false, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(false, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(false, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(false, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(false, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(false, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(false, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(true, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("0x2000, " + Status.STR_TEMP_ERR, status.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Test 0x4000
        /// </summary>
        [Test]
        public void TestConstructor0x4000()
        {
            Status status = new Status(0x4000);

            Assert.AreEqual(0x4000, status.Value, "Status value is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackLongLag(), "WT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(false, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(false, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(false, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(false, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(false, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(true, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(false, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(false, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("0x4000, " + Status.STR_RCVR_DATA_ERR, status.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Test 0x8000
        /// </summary>
        [Test]
        public void TestConstructor0x8000()
        {
            Status status = new Status(0x8000);

            Assert.AreEqual(0x8000, status.Value, "Status value is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackLongLag(), "WT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(false, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(false, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(false, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(false, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(false, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(false, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(true, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(false, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("0x8000, " + Status.STR_RCVR_TIMEOUT_ERR, status.ToString(), "ToString is incorrect.");
        }

        /// <summary>
        /// Test Multiple errors
        /// </summary>
        [Test]
        public void TestConstructorMultiple()
        {
            Status status = new Status(0x8401);

            Assert.AreEqual(0x8401, status.Value, "Status value is incorrect.");
            Assert.AreEqual(true, status.IsBottomTrackLongLag(), "WT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrack3BeamSolution(), "BT 3 Beam Solution is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackHold(), "BT Hold is incorrect.");
            Assert.AreEqual(false, status.IsBottomTrackSearching(), "BT Searching is incorrect.");
            Assert.AreEqual(false, status.IsHeadingSensorError(), "Heading Sensor is incorrect.");
            Assert.AreEqual(false, status.IsNonVolatileDataError(), "NonVolatile Data Error is incorrect.");
            Assert.AreEqual(true, status.IsPowerDownFailure(), "Power Down Failure is incorrect.");
            Assert.AreEqual(false, status.IsPressureSensorError(), "Pressure Sensor is incorrect.");
            Assert.AreEqual(false, status.IsRealTimeClockError(), "Real time clock is incorrect.");
            Assert.AreEqual(false, status.IsReceiverDataError(), "Receiver Data Error is incorrect.");
            Assert.AreEqual(true, status.IsReceiverTimeout(), "Receiver Timeout is incorrect.");
            Assert.AreEqual(false, status.IsTemperatureError(), "Temperature is incorrect.");

            Assert.AreEqual("0x8401, " + Status.STR_WT_3_BEAM_SOLUTION + ", " + Status.STR_PWR_DOWN_ERR + ", " + Status.STR_RCVR_TIMEOUT_ERR, status.ToString(), "ToString is incorrect.");
        }

    }
}
