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
 * Date            Initials    Comments
 * -----------------------------------------------------------------
 * 11/23/2011      RC          Initial coding
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using DotSpatial.Positioning;
    using System.Globalization;

    /// <summary>
    /// Unit test of the Speed object.
    /// </summary>
    [TestFixture]
    public class SpeedTest
    {

        /// <summary>
        /// Test a constructor that takes a value and
        /// a speed unit.
        /// </summary>
        [Test]
        public void TestConstructorValueUnit()
        {
            // Create an object with 5 m/s
            Speed speed = new Speed(5, SpeedUnit.MetersPerSecond);

            Assert.AreEqual(5, speed.Value);
            Assert.AreEqual(SpeedUnit.MetersPerSecond, speed.Units);
        }

        /// <summary>
        /// Test a constructor that takes a string value
        /// that includes a value and unit.
        /// </summary>
        [Test]
        public void TestConstructorValue()
        {
            Speed speed = new Speed("5 m/s");

            Assert.AreEqual(5, speed.Value);
            Assert.AreEqual(SpeedUnit.MetersPerSecond, speed.Units);
        }

        /// <summary>
        /// Test a constructor that takes a string value
        /// that includes a value.  Then include a CultureInfo
        /// to set the unit.
        /// </summary>
        [Test]
        public void TestConstructorValueCulture()
        {
            Speed speed = new Speed("5 m/s", CultureInfo.CurrentCulture);

            Assert.AreEqual(5, speed.Value);
            Assert.AreEqual(SpeedUnit.MetersPerSecond, speed.Units);
        }

        /// <summary>
        /// Test a constructor that takes a string value
        /// that includes a value and not a unit.  This
        /// will throw an exception.
        /// </summary>
        [Test]
        //[ExpectedException(typeof(ArgumentException))]
        public void TestConstructorValueException()
        {
            Speed speed;

            Assert.Throws<ArgumentException>(() => speed = new Speed("5"));

            //Assert.AreEqual(5, speed.Value);
            //Assert.AreEqual(SpeedUnit.MetersPerSecond, speed.Units);
        }

        /// <summary>
        /// Test a constructor that takes a string value
        /// that includes a value and not a unit.  This
        /// will throw an exception.
        /// </summary>
        [Test]
        //[ExpectedException(typeof(ArgumentException))]
        public void TestConstructorValueCultureException()
        {
            Speed speed;

            Assert.Throws<ArgumentException>(() => speed = new Speed("5", CultureInfo.CurrentCulture));

            //Assert.AreEqual(5, speed.Value);
            //Assert.AreEqual(SpeedUnit.MetersPerSecond, speed.Units);
        }

        /// <summary>
        /// Test converting values to different units.
        /// </summary>
        [Test]
        public void TestConvertion()
        {
            Speed speed = new Speed(5, SpeedUnit.MetersPerSecond);

            Assert.AreEqual(5, speed.Value);

            Speed ftps = speed.ToFeetPerSecond();
            Assert.AreEqual(16.40419, ftps.Value, 0.00001);
            Assert.AreEqual(SpeedUnit.FeetPerSecond, ftps.Units);

            Speed kph = speed.ToKilometersPerHour();
            Assert.AreEqual(18, kph.Value);
            Assert.AreEqual(SpeedUnit.KilometersPerHour, kph.Units);

            Speed kps = speed.ToKilometersPerSecond();
            Assert.AreEqual(0.005, kps.Value, 0.001);
            Assert.AreEqual(SpeedUnit.KilometersPerSecond, kps.Units);

            Speed knots = speed.ToKnots();
            Assert.AreEqual(9.71922246, knots.Value, 0.00001);
            Assert.AreEqual(SpeedUnit.Knots, knots.Units);

            Speed meterps = speed.ToMetersPerSecond();
            Assert.AreEqual(5, meterps.Value);
            Assert.AreEqual(SpeedUnit.MetersPerSecond, meterps.Units);

            Speed mmps = speed.ToMillimetersPerSecond();
            Assert.AreEqual(5000, mmps.Value);
            Assert.AreEqual(SpeedUnit.MillimetersPerSecond, mmps.Units);

            Speed mph = speed.ToStatuteMilesPerHour();
            Assert.AreEqual(11.1846815, mph.Value, 0.00001);
            Assert.AreEqual(SpeedUnit.StatuteMilesPerHour, mph.Units);
        }

    }

}