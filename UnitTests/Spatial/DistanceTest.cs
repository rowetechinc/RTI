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
    /// Unit test of the Distance object.
    /// </summary>
    [TestFixture]
    public class DistanceTest
    {

        /// <summary>
        /// Test a constructor that takes a value and
        /// a speed unit.
        /// </summary>
        [Test]
        public void TestConstructorValueUnit()
        {
            // Create an object with 5 m
            Distance dist = new Distance(5, DistanceUnit.Meters);

            Assert.AreEqual(5, dist.Value);
            Assert.AreEqual(DistanceUnit.Meters, dist.Units);
        }

        /// <summary>
        /// Test a constructor that takes a string value
        /// that includes a value and unit.
        /// </summary>
        [Test]
        public void TestConstructorValue()
        {
            Distance dist = new Distance("5 m");

            Assert.AreEqual(5, dist.Value);
            Assert.AreEqual(DistanceUnit.Meters, dist.Units);
        }

        /// <summary>
        /// Test a constructor that takes a string value
        /// that includes a value.  Then include a CultureInfo
        /// to set the unit.
        /// </summary>
        [Test]
        public void TestConstructorValueCulture()
        {
            Distance dist = new Distance("5 m", CultureInfo.CurrentCulture);

            Assert.AreEqual(5, dist.Value);
            Assert.AreEqual(DistanceUnit.Meters, dist.Units);
        }

        /// <summary>
        /// Test a constructor that takes a string value
        /// that includes a value and not a unit.  This
        /// will throw an exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorValueException()
        {
            Distance dist = new Distance("5");

            Assert.AreEqual(5, dist.Value);
            Assert.AreEqual(SpeedUnit.MetersPerSecond, dist.Units);
        }

        /// <summary>
        /// Test a constructor that takes a string value
        /// that includes a value and not a unit.  This
        /// will throw an exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorValueCultureException()
        {
            Distance dist = new Distance("5", CultureInfo.CurrentCulture);

            Assert.AreEqual(5, dist.Value);
            Assert.AreEqual(SpeedUnit.MetersPerSecond, dist.Units);
        }

        /// <summary>
        /// Test converting values to different units.
        /// </summary>
        [Test]
        public void TestConvertion()
        {
            // Create an object with 5 m
            Distance dist = new Distance(5, DistanceUnit.Meters);

            Assert.AreEqual(5, dist.Value);
            Assert.AreEqual(DistanceUnit.Meters, dist.Units);

            Distance feet = dist.ToFeet();
            Assert.AreEqual(16.4041995, feet.Value, 0.00001);
            Assert.AreEqual(DistanceUnit.Feet, feet.Units);

            Distance meters = dist.ToMeters();
            Assert.AreEqual(5, meters.Value);
            Assert.AreEqual(DistanceUnit.Meters, meters.Units);

            Distance mm = dist.ToMillimeters();
            Assert.AreEqual(5000, mm.Value);
            Assert.AreEqual(DistanceUnit.Millimeters, mm.Units);

            Distance cm = dist.ToCentimeters();
            Assert.AreEqual(500, cm.Value);
            Assert.AreEqual(DistanceUnit.Centimeters, cm.Units);

            Distance inch = dist.ToInches();
            Assert.AreEqual(196.850394, inch.Value, 0.00001);
            Assert.AreEqual(DistanceUnit.Inches, inch.Units);

            Distance km = dist.ToKilometers();
            Assert.AreEqual(0.005, km.Value);
            Assert.AreEqual(DistanceUnit.Kilometers, km.Units);

            Distance miles = dist.ToStatuteMiles();
            Assert.AreEqual(0.00310685596, miles.Value, 0.00001);
            Assert.AreEqual(DistanceUnit.StatuteMiles, miles.Units);

            Distance nm = dist.ToNauticalMiles();
            Assert.AreEqual(0.00269978402, nm.Value, 0.00001);
            Assert.AreEqual(DistanceUnit.NauticalMiles, nm.Units);
        }

    }

}