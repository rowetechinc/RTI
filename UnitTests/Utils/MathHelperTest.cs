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
 * Date            Initials    Vertion    Comments
 * -----------------------------------------------------------------
 * 03/29/2012      RC          2.07       Initial coding
 * 
 * 
 */
namespace UnitTests.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    /// <summary>
    /// Test the MathHelper class.
    /// </summary>
    [TestFixture]
    public class MathHelperTest
    {

        #region Standard Deviation

        /// <summary>
        /// Test the Standard Devation.  This is the population standard deviation.
        /// http://easycalculation.com/statistics/standard-deviation.php
        /// http://en.wikipedia.org/wiki/Standard_deviation
        /// </summary>
        [Test]
        public void TestStandardDev()
        {
            double avg = 0;
            double std = RTI.MathHelper.StandardDev(13, 45, 12, 25, out avg);

            Assert.AreEqual(13.29239, std, 0.0001, "Standard Deviation incorrect");
            Assert.AreEqual(23.75, avg, 0.0001, "Average incorrect");
        }

        /// <summary>
        /// Test the Standard Devation with a 0.  This is the population standard deviation.
        /// The standard deviation ignores 0's.
        /// http://easycalculation.com/statistics/standard-deviation.php
        /// http://en.wikipedia.org/wiki/Standard_deviation
        /// </summary>
        [Test]
        public void TestStandardDevWith0()
        {
            double avg = 0;
            double std = RTI.MathHelper.StandardDev(0, 45, 12, 25, out avg);

            Assert.AreEqual(13.57285, std, 0.0001, "Standard Deviation incorrect");
            Assert.AreEqual(27.3333, avg, 0.0001, "Average incorrect");
        }

        /// <summary>
        /// Test the Standard Devation with all 0.  This is the population standard deviation.
        /// The standard deviation ignores 0's.
        /// http://easycalculation.com/statistics/standard-deviation.php
        /// http://en.wikipedia.org/wiki/Standard_deviation
        /// </summary>
        [Test]
        public void TestStandardDevWithAll0()
        {
            double avg = 0;
            double std = RTI.MathHelper.StandardDev(0, 0, 0, 0, out avg);

            Assert.AreEqual(0, std, 0.0001, "Standard Deviation incorrect");
            Assert.AreEqual(0, avg, 0.0001, "Average incorrect");
        }

        #endregion

        #region Standard Deviation List

        /// <summary>
        /// Test the Standard Devation using a list of values.  This is the population standard deviation.
        /// http://easycalculation.com/statistics/standard-deviation.php
        /// http://en.wikipedia.org/wiki/Standard_deviation
        /// </summary>
        [Test]
        public void TestStandardDevList()
        {
            double BAD_VALUE = 0;
            double v1 = 13;
            double v2 = 45;
            double v3 = 12;
            double v4 = 25;

            List<double> values = new List<double>();
            values.Add(v1);
            values.Add(v2);
            values.Add(v3);
            values.Add(v4);

            double avg = 0;
            double std = RTI.MathHelper.StandardDev(values, BAD_VALUE, out avg);

            Assert.AreEqual(13.29239, std, 0.0001, "Standard Deviation incorrect");
            Assert.AreEqual(23.75, avg, 0.0001, "Average incorrect");
        }

        /// <summary>
        /// Test the Standard Devation using a list of values.  This is the population standard deviation.
        /// The standard deviation ignores 0's.
        /// http://easycalculation.com/statistics/standard-deviation.php
        /// http://en.wikipedia.org/wiki/Standard_deviation
        /// </summary>
        [Test]
        public void TestStandardDevListWith0()
        {
            double BAD_VALUE = 0;
            double v1 = 0;
            double v2 = 45;
            double v3 = 12;
            double v4 = 25;

            List<double> values = new List<double>();
            values.Add(v1);
            values.Add(v2);
            values.Add(v3);
            values.Add(v4);

            double avg = 0;
            double std = RTI.MathHelper.StandardDev(values, BAD_VALUE, out avg);

            Assert.AreEqual(13.57285, std, 0.0001, "Standard Deviation incorrect");
            Assert.AreEqual(27.3333, avg, 0.0001, "Average incorrect");
        }

        /// <summary>
        /// Test the Standard Devation using a list of values.  This is the population standard deviation.
        /// The standard deviation ignores 0's.
        /// Change the bad value to ensure it does not matter.  Also added a 5th value.
        /// http://easycalculation.com/statistics/standard-deviation.php
        /// http://en.wikipedia.org/wiki/Standard_deviation
        /// </summary>
        [Test]
        public void TestStandardDevListWithBadVelocity()
        {
            double BAD_VALUE = RTI.DataSet.Ensemble.BAD_VELOCITY;
            double v1 = RTI.DataSet.Ensemble.BAD_VELOCITY;
            double v2 = 45;
            double v3 = 12;
            double v4 = 25;
            double v5 = 13;

            List<double> values = new List<double>();
            values.Add(v1);
            values.Add(v2);
            values.Add(v3);
            values.Add(v4);
            values.Add(v5);

            double avg = 0;
            double std = RTI.MathHelper.StandardDev(values, BAD_VALUE, out avg);

            Assert.AreEqual(13.29239, std, 0.0001, "Standard Deviation incorrect");
            Assert.AreEqual(23.75, avg, 0.0001, "Average incorrect");
        }

        /// <summary>
        /// Test the Standard Devation using a list of values.  This is the population standard deviation.
        /// The standard deviation ignores 0's.
        /// Change the bad value to ensure it does not matter.  Also added a 5th value.
        /// http://easycalculation.com/statistics/standard-deviation.php
        /// http://en.wikipedia.org/wiki/Standard_deviation
        /// </summary>
        [Test]
        public void TestStandardDevListEmpty()
        {
            double BAD_VALUE = RTI.DataSet.Ensemble.BAD_VELOCITY;

            List<double> values = new List<double>();

            double avg = 0;
            double std = RTI.MathHelper.StandardDev(values, BAD_VALUE, out avg);

            Assert.AreEqual(0, std, 0.0001, "Standard Deviation incorrect");
            Assert.AreEqual(0, avg, 0.0001, "Average incorrect");
        }

        #endregion

        #region Magnitude and Direction

        /// <summary>
        /// Test the magnitude calculation.
        /// Give a north, east and vertical velocity.
        /// 
        /// http://www.analyzemath.com/vector_calculators/magnitude_direction.html
        /// </summary>
        [Test]
        public void CalculateMagnitude()
        {
            double north = 20;  // X
            double east = 30;   // Y
            double vert = 0;    // Z

            double mag = RTI.MathHelper.CalculateMagnitude(north, east, vert);
            Assert.AreEqual(36.05551, mag, 0.0001, "Magnitude incorrect");
        }

        /// <summary>
        /// Test the direction calculation.
        /// Give a north as Y and east as X.
        /// 
        /// http://www.analyzemath.com/vector_calculators/magnitude_direction.html
        /// </summary>
        [Test]
        public void CalculateDirection()
        {
            double east = 30;   // x
            double north = 20;  // y

            double dir = RTI.MathHelper.CalculateDirection(north, east);
            Assert.AreEqual(33.690067, dir, 0.0001, "Direction incorrect");
        }

        /// <summary>
        /// Test the direction calculation.
        /// Give a east as Y and north as X.
        /// 
        /// http://www.analyzemath.com/vector_calculators/magnitude_direction.html
        /// </summary>
        [Test]
        public void CalculateDirectionSwap()
        {
            double east = 30;   // x
            double north = 20;  // y

            double dir = RTI.MathHelper.CalculateDirection(east, north);
            Assert.AreEqual(56.309933, dir, 0.0001, "Direction incorrect");
        }

        /// <summary>
        /// Test the direction calculation and magnitude direction.
        /// Give a north as Y and east as X.
        /// All values 0.
        /// 
        /// http://www.analyzemath.com/vector_calculators/magnitude_direction.html
        /// </summary>
        [Test]
        public void CalculateMagnitudeDirectionAll0()
        {
            double east = 0;    // X
            double north = 0;   // Y
            double vert = 0;    // Z

            double mag = RTI.MathHelper.CalculateMagnitude(north, east, vert);
            double dir = RTI.MathHelper.CalculateDirection(north, east);

            Assert.AreEqual(0, mag, 0.0001, "Magnitude incorrect");
            Assert.AreEqual(0, dir, 0.0001, "Direction incorrect");
        }

        /// <summary>
        /// Test the direction calculation and magnitude direction.
        /// Give a north as Y and east as X.
        /// All values 0.
        /// 
        /// http://www.analyzemath.com/vector_calculators/magnitude_direction.html
        /// </summary>
        [Test]
        public void CalculateMagnitudeDirectionNorth0()
        {
            double east = 30;    // X
            double north = 0;   // Y
            double vert = 0;    // Z

            double mag = RTI.MathHelper.CalculateMagnitude(north, east, vert);
            double dir = RTI.MathHelper.CalculateDirection(north, east);

            Assert.AreEqual(30, mag, 0.0001, "Magnitude incorrect");
            Assert.AreEqual(0, dir, 0.0001, "Direction incorrect");
        }

        /// <summary>
        /// Test the direction calculation and magnitude direction.
        /// Give a north as Y and east as X.
        /// All values 0.
        /// 
        /// http://www.analyzemath.com/vector_calculators/magnitude_direction.html
        /// </summary>
        [Test]
        public void CalculateMagnitudeDirectionEast0()
        {
            double east = 0;    // X
            double north = 30;   // Y
            double vert = 0;    // Z

            double mag = RTI.MathHelper.CalculateMagnitude(north, east, vert);
            double dir = RTI.MathHelper.CalculateDirection(north, east);

            Assert.AreEqual(30, mag, 0.0001, "Magnitude incorrect");
            Assert.AreEqual(90, dir, 0.0001, "Direction incorrect");
        }

        #endregion

        #region Degree To Radians

        /// <summary>
        /// Convert Degrees to radian.
        /// 
        /// http://www.unitconversion.org/angle/degrees-to-radians-conversion.html
        /// </summary>
        [Test]
        public void DegreeToRadian()
        {
            double degree = 120;
            double rad = RTI.MathHelper.DegreeToRadian(degree);

            Assert.AreEqual(2.094395102, rad, 0.0001, "Radian incorrect");
        }

        /// <summary>
        /// Convert Degrees to radian.
        /// 180 degress is PI.
        /// http://www.unitconversion.org/angle/degrees-to-radians-conversion.html
        /// </summary>
        [Test]
        public void DegreeToRadian180()
        {
            double degree = 180;
            double rad = RTI.MathHelper.DegreeToRadian(degree);

            Assert.AreEqual(Math.PI, rad, 0.0001, "Radian incorrect");
        }

        #endregion

        #region Byte Array to Float

        /// <summary>
        /// Convert the bytes to a float.
        /// The bytes are in Big Endian form.
        /// 
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void BytesToFloat()
        {
            byte[] data = new byte[4];
            data[0] = 0x41;
            data[1] = 0x47;
            data[2] = 0x4B;
            data[3] = 0xC7;

            float value = RTI.MathHelper.ByteArrayToFloat(data, 0, true);

            Assert.AreEqual(12.456, value, 0.0001, "Float incorrect");
        }

        /// <summary>
        /// Convert the bytes to a float.  Negative number.
        /// The bytes are in Big Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void BytesToFloatNeg()
        {
            byte[] data = new byte[4];
            data[0] = 0xC1;
            data[1] = 0x47;
            data[2] = 0x4B;
            data[3] = 0xC7;

            float value = RTI.MathHelper.ByteArrayToFloat(data, 0, true);

            Assert.AreEqual(-12.456, value, 0.0001, "Negative Float incorrect");
        }

        /// <summary>
        /// Convert the bytes to a float.
        /// The bytes are in Little Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void BytesToFloatLE()
        {
            byte[] data = new byte[4];
            data[0] = 0xC7;
            data[1] = 0x4B;
            data[2] = 0x47;
            data[3] = 0x41;

            float value = RTI.MathHelper.ByteArrayToFloat(data, 0, false);

            Assert.AreEqual(12.456, value, 0.0001, "Float incorrect");
        }

        /// <summary>
        /// Convert the bytes to a float. Negative Number.
        /// The bytes are in Little Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void BytesToFloatNegLE()
        {
            byte[] data = new byte[4];
            data[0] = 0xC7;
            data[1] = 0x4B;
            data[2] = 0x47;
            data[3] = 0xC1;

            float value = RTI.MathHelper.ByteArrayToFloat(data, 0, false);

            Assert.AreEqual(-12.456, value, 0.0001, "Float incorrect");
        }

        /// <summary>
        /// Convert the bytes to a float.  
        /// Not enough bytes given.
        /// The bytes are in Big Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void BytesToFloatToSmall()
        {
            byte[] data = new byte[2];
            data[0] = 0xC1;
            data[1] = 0x47;

            float value = RTI.MathHelper.ByteArrayToFloat(data, 0, true);

            Assert.AreEqual(0, value, 0.0001, "Float incorrect");
        }

        /// <summary>
        /// Convert the bytes to a float.  
        /// Null byte array.
        /// The bytes are in Big Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void BytesToFloatNull()
        {
            float value = RTI.MathHelper.ByteArrayToFloat(null, 0, true);

            Assert.AreEqual(0, value, 0.0001, "Float incorrect");
        }

        /// <summary>
        /// Convert the bytes to a float.  
        /// Not enough bytes given.
        /// The bytes are in Big Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void BytesToFloat0()
        {
            byte[] data = new byte[4];
            data[0] = 0x0;
            data[1] = 0x0;
            data[2] = 0x0;
            data[3] = 0x0;

            float value = RTI.MathHelper.ByteArrayToFloat(data, 0, true);

            Assert.AreEqual(0, value, 0.0001, "Float incorrect");
        }

        #endregion

        #region Byte Array to UInt32

        /// <summary>
        /// Convert the bytes to a Unsigned 32 bit Integer.
        /// The bytes are in Big Endian form.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void BytesToUInt32()
        {
            byte[] data = new byte[4];
            data[0] = 0x48;
            data[1] = 0x6F;
            data[2] = 0xE3;
            data[3] = 0x10;

            UInt32 value = RTI.MathHelper.ByteArrayToUInt32(data, 0, true);

            Assert.AreEqual(1215292176, value, "UInt32 incorrect");
        }

        /// <summary>
        /// Convert the bytes to a Unsigned 32 bit Integer.
        /// The bytes are in Little Endian form.
        /// 
        /// Microsoft Calculator (Reverse Order of calculator)
        /// </summary>
        [Test]
        public void BytesToUInt32LE()
        {
            byte[] data = new byte[4];
            data[0] = 0x10;
            data[1] = 0xE3;
            data[2] = 0x6F;
            data[3] = 0x48;

            UInt32 value = RTI.MathHelper.ByteArrayToUInt32(data, 0, false);

            Assert.AreEqual(1215292176, value, "UInt32 incorrect");
        }

        /// <summary>
        /// Convert the bytes to a Unsigned 32 bit Integer. Negative Number.
        /// The bytes are in Little Endian form.
        /// 
        /// Microsoft Calculator (Reverse Order of calculator)
        /// </summary>
        [Test]
        public void BytesToUInt32LENeg()
        {
            byte[] data = new byte[4];
            data[0] = 0xA2;
            data[1] = 0xF6;
            data[2] = 0x7E;
            data[3] = 0xFF;

            UInt32 value = RTI.MathHelper.ByteArrayToUInt32(data, 0, false);

            Assert.AreNotEqual(-8456542, value, "UInt32 Negative incorrect");
            Assert.AreEqual(4286510754, value, "UInt32 Negative incorrect");
        }

        /// <summary>
        /// Convert the bytes to a Unsigned 32 bit Integer.
        /// The bytes are in Little Endian form.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void BytesToUInt32ToSmall()
        {
            byte[] data = new byte[1];
            data[0] = 0x10;

            UInt32 value = RTI.MathHelper.ByteArrayToUInt32(data, 0, false);

            Assert.AreEqual(0, value, "UInt32 incorrect");
        }

        /// <summary>
        /// Convert the bytes to a Unsigned 32 bit Integer.
        /// Give a null array.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void BytesToUInt32Null()
        {
            UInt32 value = RTI.MathHelper.ByteArrayToUInt32(null, 0, false);

            Assert.AreEqual(0, value, "UInt32 incorrect");
        }

        #endregion

        #region Byte Array to UInt16

        /// <summary>
        /// Convert the bytes to a Unsigned 16 bit Integer.
        /// The bytes are in Big Endian form.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void BytesToUInt16()
        {
            byte[] data = new byte[2];
            data[0] = 0x48;
            data[1] = 0x6F;

            UInt16 value = RTI.MathHelper.ByteArrayToUInt16(data, 0, true);

            Assert.AreEqual(18543, value, "UInt16 incorrect");
        }

        /// <summary>
        /// Convert the bytes to a Unsigned 16 bit Integer.
        /// The bytes are in Big Endian form.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void BytesToUInt16BE()
        {
            byte[] data = new byte[4];
            data[0] = 0x10;
            data[1] = 0x48;
            data[2] = 0x6F;
            data[3] = 0xE3;

            UInt16 value = RTI.MathHelper.ByteArrayToUInt16(data, 1, true);

            Assert.AreEqual(18543, value, "UInt16 incorrect");
        }

        /// <summary>
        /// Convert the bytes to a Unsigned 16 bit Integer.
        /// The bytes are in Little Endian form.
        /// 
        /// Microsoft Calculator (Reverse Order of calculator)
        /// </summary>
        [Test]
        public void BytesToUInt16LE()
        {
            byte[] data = new byte[2];
            data[0] = 0x6F;
            data[1] = 0x48;

            UInt16 value = RTI.MathHelper.ByteArrayToUInt16(data, 0, false);

            Assert.AreEqual(18543, value, "UInt16 incorrect");
        }

        /// <summary>
        /// Go within the array to get the value
        /// Convert the bytes to a Unsigned 16 bit Integer.
        /// The bytes are in Little Endian form.
        /// 
        /// Microsoft Calculator (Reverse Order of calculator)
        /// </summary>
        [Test]
        public void BytesToUInt16Within()
        {
            byte[] data = new byte[4];
            data[0] = 0x10;
            data[1] = 0xE3;
            data[2] = 0x6F;
            data[3] = 0x48;

            UInt16 value = RTI.MathHelper.ByteArrayToUInt16(data, 2, false);

            Assert.AreEqual(18543, value, "UInt16 incorrect");
        }

        /// <summary>
        /// Convert the bytes to a Unsigned 32 bit Integer.
        /// The bytes are in Big Endian form.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void BytesToUInt16ToSmall()
        {
            byte[] data = new byte[1];
            data[0] = 0x10;

            UInt16 value = RTI.MathHelper.ByteArrayToUInt16(data, 0, false);

            Assert.AreEqual(0, value, "UInt16 incorrect");
        }

        /// <summary>
        /// Convert the bytes to a Unsigned 32 bit Integer.
        /// The bytes are in Big Endian form.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void BytesToUInt16ToSmallAgain()
        {
            byte[] data = new byte[2];
            data[0] = 0x10;
            data[0] = 0x10;

            UInt16 value = RTI.MathHelper.ByteArrayToUInt16(data, 1, false);

            Assert.AreEqual(0, value, "UInt16 incorrect");
        }

        /// <summary>
        /// Convert the bytes to a Unsigned 32 bit Integer.
        /// Null value.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void BytesToUInt16Null()
        {
            UInt16 value = RTI.MathHelper.ByteArrayToUInt16(null, 0, false);

            Assert.AreEqual(0, value, "UInt16 incorrect");
        }

        #endregion

        #region Byte to Int

        /// <summary>
        /// Convert the byte to a Integer.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void ByteToInt8()
        {
            byte[] data = new byte[1];
            data[0] = 0x10;

            int value = RTI.MathHelper.ByteArrayToInt8(data, 0);

            Assert.AreEqual(16, value, "Int incorrect");
        }

        /// <summary>
        /// Go within the array to get the value
        /// Convert the byte to a Integer.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void ByteToIn8tWithin()
        {
            byte[] data = new byte[4];
            data[0] = 0x10;
            data[1] = 0xE3;
            data[2] = 0x6F;
            data[3] = 0x48;

            int value = RTI.MathHelper.ByteArrayToInt8(data, 2);

            Assert.AreEqual(111, value, "Int incorrect");
        }

        /// <summary>
        /// Convert the byte to a Integer.
        /// Go pass the last value.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void ByteToInt8ToFar()
        {
            byte[] data = new byte[2];
            data[0] = 0x10;
            data[1] = 0xE3;

            int value = RTI.MathHelper.ByteArrayToInt8(data, 3);

            Assert.AreEqual(0, value, "Int incorrect");
        }

        #endregion

        #region Byte to Boolean

        /// <summary>
        /// Convert the byte to a Boolean.
        /// 
        /// 0 Or 1
        /// </summary>
        [Test]
        public void ByteToBoolean()
        {
            byte[] data = new byte[1];
            data[0] = 0x01;

            bool value = RTI.MathHelper.ByteArrayToBoolean(data, 0);

            Assert.AreEqual(true, value, "Boolean incorrect");
        }

        /// <summary>
        /// Convert the byte to a Boolean.
        /// Give a bad value.
        /// 
        /// 0 Or 1
        /// </summary>
        [Test]
        public void ByteToBooleanBad()
        {
            byte[] data = new byte[1];
            data[0] = 0x10;

            bool value = RTI.MathHelper.ByteArrayToBoolean(data, 0);

            Assert.AreEqual(true, value, "Boolean incorrect");
        }

        /// <summary>
        /// Go within the array to get the value
        /// Convert the byte to a Boolean.
        /// 
        /// 0 Or 1
        /// </summary>
        [Test]
        public void ByteToBooleanWithin()
        {
            byte[] data = new byte[4];
            data[0] = 0x10;
            data[1] = 0xE3;
            data[2] = 0x01;
            data[3] = 0x48;

            bool value = RTI.MathHelper.ByteArrayToBoolean(data, 2);

            Assert.AreEqual(true, value, "Boolean incorrect");
        }

        /// <summary>
        /// Convert the byte to a boolean.
        /// Go pass the last value.
        /// 
        /// 0 Or 1
        /// </summary>
        [Test]
        public void ByteToBooleanToFar()
        {
            byte[] data = new byte[2];
            data[0] = 0x10;
            data[1] = 0xE3;

            bool value = RTI.MathHelper.ByteArrayToBoolean(data, 3);

            Assert.AreEqual(false, value, "Boolean incorrect");
        }

        #endregion

        #region Byte Array to Int

        /// <summary>
        /// Convert the bytes to a 32 bit Integer.
        /// The bytes are in Big Endian form.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void BytesToInt32()
        {
            byte[] data = new byte[4];
            data[0] = 0x48;
            data[1] = 0x6F;
            data[2] = 0xE3;
            data[3] = 0x10;

            int value = RTI.MathHelper.ByteArrayToInt32(data, 0, true);

            Assert.AreEqual(1215292176, value, "Int incorrect");
        }

        /// <summary>
        /// Convert the bytes to a 32 bit Integer.
        /// The bytes are in Little Endian form.
        /// 
        /// Microsoft Calculator (Reverse Order of calculator)
        /// </summary>
        [Test]
        public void BytesToInt32LE()
        {
            byte[] data = new byte[4];
            data[0] = 0x10;
            data[1] = 0xE3;
            data[2] = 0x6F;
            data[3] = 0x48;

            int value = RTI.MathHelper.ByteArrayToInt32(data, 0, false);

            Assert.AreEqual(1215292176, value, "Int incorrect");
        }

        /// <summary>
        /// Convert the bytes to a 32 bit Negative Integer.
        /// The bytes are in Little Endian form.
        /// 
        /// Microsoft Calculator (Reverse Order of calculator)
        /// </summary>
        [Test]
        public void BytesToInt32NegLE()
        {
            byte[] data = new byte[4];
            data[0] = 0x80;
            data[1] = 0x35;
            data[2] = 0x9C;
            data[3] = 0xFC;

            int value = RTI.MathHelper.ByteArrayToInt32(data, 0, false);

            Assert.AreEqual(-56871552, value, "Int incorrect");
        }

        /// <summary>
        /// Convert the bytes to a 32 bit Negative Integer.
        /// The bytes are in Big Endian form.
        /// 
        /// Microsoft Calculator (Reverse Order of calculator)
        /// </summary>
        [Test]
        public void BytesToInt32Neg()
        {
            byte[] data = new byte[4];
            data[0] = 0xFC;
            data[1] = 0x9C;
            data[2] = 0x35;
            data[3] = 0x80;

            int value = RTI.MathHelper.ByteArrayToInt32(data, 0, true);

            Assert.AreEqual(-56871552, value, "Int incorrect");
        }

        /// <summary>
        /// Convert the bytes to a Unsigned 32 bit Integer.
        /// The bytes are in Little Endian form.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void BytesToInt32ToSmall()
        {
            byte[] data = new byte[1];
            data[0] = 0x10;

            int value = RTI.MathHelper.ByteArrayToInt32(data, 0, false);

            Assert.AreEqual(0, value, "Int incorrect");
        }

        /// <summary>
        /// Convert the bytes to a Unsigned 32 bit Integer.
        /// Give a null array.
        /// 
        /// Microsoft Calculator
        /// </summary>
        [Test]
        public void BytesToInt32Null()
        {
            int value = RTI.MathHelper.ByteArrayToInt32(null, 0, false);

            Assert.AreEqual(0, value, "Int incorrect");
        }

        #endregion

        #region Float to Byte Array

        /// <summary>
        /// Convert the float to a Byte Array.
        /// The bytes are in Big Endian form.
        /// 
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void FloatToByteArray()
        {
            float value = 12.456f;

            byte[] data = new byte[4];
            data[0] = 0x41;
            data[1] = 0x47;
            data[2] = 0x4B;
            data[3] = 0xC7;

            byte[] result = RTI.MathHelper.FloatToByteArray(value, true);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for Float incorrect");
            Assert.AreEqual(data[1], result[1], 0.0001, "Byte Array 1 for Float incorrect");
            Assert.AreEqual(data[2], result[2], 0.0001, "Byte Array 2 for Float incorrect");
            Assert.AreEqual(data[3], result[3], 0.0001, "Byte Array 3 for Float incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for Float incorrect");
        }

        /// <summary>
        /// Convert the float to a Byte Array.  Negative number.
        /// The bytes are in Big Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void FloatToByteArrayNeg()
        {
            float value = -12.456f;

            byte[] data = new byte[4];
            data[0] = 0xC1;
            data[1] = 0x47;
            data[2] = 0x4B;
            data[3] = 0xC7;

            byte[] result = RTI.MathHelper.FloatToByteArray(value, true);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for Neg Float incorrect");
            Assert.AreEqual(data[1], result[1], 0.0001, "Byte Array 1 for Neg Float incorrect");
            Assert.AreEqual(data[2], result[2], 0.0001, "Byte Array 2 for Neg Float incorrect");
            Assert.AreEqual(data[3], result[3], 0.0001, "Byte Array 3 for Neg Float incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for Neg Float incorrect");
        }

        /// <summary>
        /// Convert the float to a Byte Array.
        /// The bytes are in Little Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void FloatToByteArrayLE()
        {
            float value = 12.456f;

            byte[] data = new byte[4];
            data[0] = 0xC7;
            data[1] = 0x4B;
            data[2] = 0x47;
            data[3] = 0x41;

            byte[] result = RTI.MathHelper.FloatToByteArray(value);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for Float incorrect");
            Assert.AreEqual(data[1], result[1], 0.0001, "Byte Array 1 for Float incorrect");
            Assert.AreEqual(data[2], result[2], 0.0001, "Byte Array 2 for Float incorrect");
            Assert.AreEqual(data[3], result[3], 0.0001, "Byte Array 3 for Float incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for Float incorrect");
        }

        /// <summary>
        /// Convert the bytes to a float. Negative Number.
        /// The bytes are in Little Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void FloatToByteArrayNegLE()
        {
            float value = -12.456f;

            byte[] data = new byte[4];
            data[0] = 0xC7;
            data[1] = 0x4B;
            data[2] = 0x47;
            data[3] = 0xC1;

            byte[] result = RTI.MathHelper.FloatToByteArray(value);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for Neg Float incorrect");
            Assert.AreEqual(data[1], result[1], 0.0001, "Byte Array 1 for Neg Float incorrect");
            Assert.AreEqual(data[2], result[2], 0.0001, "Byte Array 2 for Neg Float incorrect");
            Assert.AreEqual(data[3], result[3], 0.0001, "Byte Array 3 for Neg Float incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for Neg Float incorrect");
        }

        /// <summary>
        /// Convert the bytes to a float.  
        /// Null byte array.
        /// The bytes are in Big Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void FloatToByteArrayNaN()
        {
            float value = float.NaN;
            byte[] result = RTI.MathHelper.FloatToByteArray(value, true);

            Assert.AreEqual(0, result[0], "Byte Array 0 for NaN Float incorrect");
            Assert.AreEqual(0, result[1], "Byte Array 1 for NaN Float incorrect");
            Assert.AreEqual(0, result[2], "Byte Array 2 for NaN Float incorrect");
            Assert.AreEqual(0, result[3], "Byte Array 3 for NaN Float incorrect");
            Assert.AreEqual(4, result.Length, "Byte Array for Neg Float incorrect");
        }

        /// <summary>
        /// Convert the bytes to a float.  
        /// Positive Infinity float.
        /// The bytes are in Big Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void FloatToByteArrayPosInfinity()
        {
            float value = float.PositiveInfinity;
            byte[] result = RTI.MathHelper.FloatToByteArray(value, true);

            Assert.AreEqual(0, result[0], "Byte Array 0 for PosInf Float incorrect");
            Assert.AreEqual(0, result[1], "Byte Array 1 for PosInf Float incorrect");
            Assert.AreEqual(0, result[2], "Byte Array 2 for PosInf Float incorrect");
            Assert.AreEqual(0, result[3], "Byte Array 3 for PosInf Float incorrect");
            Assert.AreEqual(4, result.Length, "Byte Array for PosInf Float incorrect");
        }

        /// <summary>
        /// Convert the bytes to a float.  
        /// Positive Infinity float.
        /// The bytes are in Big Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void FloatToByteArrayNegInfinity()
        {
            float value = float.NegativeInfinity;
            byte[] result = RTI.MathHelper.FloatToByteArray(value, false);

            Assert.AreEqual(0, result[0], "Byte Array 0 for NegInf Float incorrect");
            Assert.AreEqual(0, result[1], "Byte Array 1 for NegInf Float incorrect");
            Assert.AreEqual(0, result[2], "Byte Array 2 for NegInf Float incorrect");
            Assert.AreEqual(0, result[3], "Byte Array 3 for NegInf Float incorrect");
            Assert.AreEqual(4, result.Length, "Byte Array for NegInf Float incorrect");
        }

        #endregion

        #region UInt32 to Byte Array

        /// <summary>
        /// Convert the UInt32 to a Byte Array.
        /// The bytes are in Big Endian form.
        /// 
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void UInt32ToByteArray()
        {
            UInt32 value = 1215292176;

            byte[] data = new byte[4];
            data[0] = 0x48;
            data[1] = 0x6F;
            data[2] = 0xE3;
            data[3] = 0x10;

            byte[] result = RTI.MathHelper.UInt32ToByteArray(value, true);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for UInt32 incorrect");
            Assert.AreEqual(data[1], result[1], 0.0001, "Byte Array 1 for UInt32 incorrect");
            Assert.AreEqual(data[2], result[2], 0.0001, "Byte Array 2 for UInt32 incorrect");
            Assert.AreEqual(data[3], result[3], 0.0001, "Byte Array 3 for UInt32 incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for UInt32 incorrect");
        }

        /// <summary>
        /// Convert the UInt16 to a Byte Array.
        /// The bytes are in Little Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void UInt32ToByteArrayLE()
        {
            UInt32 value = 1215292176;

            byte[] data = new byte[4];
            data[0] = 0x10;
            data[1] = 0xE3;
            data[2] = 0x6F;
            data[3] = 0x48;

            byte[] result = RTI.MathHelper.UInt32ToByteArray(value);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for UInt32 incorrect");
            Assert.AreEqual(data[1], result[1], 0.0001, "Byte Array 1 for UInt32 incorrect");
            Assert.AreEqual(data[2], result[2], 0.0001, "Byte Array 2 for UInt32 incorrect");
            Assert.AreEqual(data[3], result[3], 0.0001, "Byte Array 3 for UInt32 incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for UInt32 incorrect");
        }

        #endregion

        #region UInt16 to Byte Array

        /// <summary>
        /// Convert the UInt16 to a Byte Array.
        /// The bytes are in Big Endian form.
        /// 
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void UInt16ToByteArray()
        {
            UInt16 value = 18543;

            byte[] data = new byte[2];
            data[0] = 0x48;
            data[1] = 0x6F;

            byte[] result = RTI.MathHelper.UInt16ToByteArray(value, true);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for UInt32 incorrect");
            Assert.AreEqual(data[1], result[1], 0.0001, "Byte Array 1 for UInt32 incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for UInt32 incorrect");
        }

        /// <summary>
        /// Convert the UInt16 to a Byte Array.
        /// The bytes are in Little Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void UInt16ToByteArrayLE()
        {
            UInt16 value = 18543;

            byte[] data = new byte[2];
            data[0] = 0x6F;
            data[1] = 0x48;

            byte[] result = RTI.MathHelper.UInt16ToByteArray(value);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for UInt32 incorrect");
            Assert.AreEqual(data[1], result[1], 0.0001, "Byte Array 1 for UInt32 incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for UInt32 incorrect");
        }

        #endregion

        #region Int32 to Byte Array

        /// <summary>
        /// Convert the Int to a Byte Array.
        /// The bytes are in Big Endian form.
        /// 
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void IntToByteArray()
        {
            int value = 1215292176;

            byte[] data = new byte[4];
            data[0] = 0x48;
            data[1] = 0x6F;
            data[2] = 0xE3;
            data[3] = 0x10;

            byte[] result = RTI.MathHelper.Int32ToByteArray(value, true);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for Int incorrect");
            Assert.AreEqual(data[1], result[1], 0.0001, "Byte Array 1 for Int incorrect");
            Assert.AreEqual(data[2], result[2], 0.0001, "Byte Array 2 for Int incorrect");
            Assert.AreEqual(data[3], result[3], 0.0001, "Byte Array 3 for Int incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for Int incorrect");
        }

        /// <summary>
        /// Convert the Int to a Byte Array.  Negative number.
        /// The bytes are in Big Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void IntToByteArrayNeg()
        {
            int value = -56871552;

            byte[] data = new byte[4];
            data[0] = 0xFC;
            data[1] = 0x9C;
            data[2] = 0x35;
            data[3] = 0x80;

            byte[] result = RTI.MathHelper.Int32ToByteArray(value, true);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for Neg Int incorrect");
            Assert.AreEqual(data[1], result[1], 0.0001, "Byte Array 1 for Neg Int incorrect");
            Assert.AreEqual(data[2], result[2], 0.0001, "Byte Array 2 for Neg Int incorrect");
            Assert.AreEqual(data[3], result[3], 0.0001, "Byte Array 3 for Neg Int incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for Neg Int incorrect");
        }

        /// <summary>
        /// Convert the Int to a Byte Array.
        /// The bytes are in Little Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void IntToByteArrayLE()
        {
            int value = 1215292176;

            byte[] data = new byte[4];
            data[0] = 0x10;
            data[1] = 0xE3;
            data[2] = 0x6F;
            data[3] = 0x48;

            byte[] result = RTI.MathHelper.Int32ToByteArray(value);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for Int incorrect");
            Assert.AreEqual(data[1], result[1], 0.0001, "Byte Array 1 for Int incorrect");
            Assert.AreEqual(data[2], result[2], 0.0001, "Byte Array 2 for Int incorrect");
            Assert.AreEqual(data[3], result[3], 0.0001, "Byte Array 3 for Int incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for Int incorrect");
        }

        /// <summary>
        /// Convert the Int to a byte array. Negative Number.
        /// The bytes are in Little Endian form.
        /// Little Endian, the bytes are reversed from Big Endian.
        /// http://www.h-schmidt.net/FloatApplet/IEEE754.html
        /// </summary>
        [Test]
        public void IntToByteArrayNegLE()
        {
            int value = -56871552;

            byte[] data = new byte[4];
            data[0] = 0x80;
            data[1] = 0x35;
            data[2] = 0x9C;
            data[3] = 0xFC;

            byte[] result = RTI.MathHelper.Int32ToByteArray(value);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for Neg Int incorrect");
            Assert.AreEqual(data[1], result[1], 0.0001, "Byte Array 1 for Neg Int incorrect");
            Assert.AreEqual(data[2], result[2], 0.0001, "Byte Array 2 for Neg Int incorrect");
            Assert.AreEqual(data[3], result[3], 0.0001, "Byte Array 3 for Neg Int incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for Neg Int incorrect");
        }

        #endregion

        #region UInt8 to Byte Array

        /// <summary>
        /// Convert the UInt8 to a Byte Array.
        /// </summary>
        [Test]
        public void UInt8ToByteArray()
        {
            int value = 252;

            byte[] data = new byte[1];
            data[0] = 0xFC;

            byte[] result = RTI.MathHelper.UInt8ToByteArray(value);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for UInt8 incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for UInt8 incorrect");
        }

        /// <summary>
        /// Convert the UInt8 to a Byte Array.  Value to Large.
        /// </summary>
        [Test]
        public void UInt8ToByteArrayToBig()
        {
            int value = 256;

            byte[] result = RTI.MathHelper.UInt8ToByteArray(value);

            Assert.AreEqual(0, result[0], 0.0001, "Byte Array 0 for To Big UInt8 incorrect");
            Assert.AreEqual(1, result.Length, 0.0001, "Byte Array for To Big UInt8 incorrect");
        }

        /// <summary>
        /// Convert the UInt8 to a Byte Array.  Negative Number.
        /// </summary>
        [Test]
        public void UInt8ToByteArrayNeg()
        {
            int value = -25;

            byte[] result = RTI.MathHelper.UInt8ToByteArray(value);

            Assert.AreEqual(0, result[0], 0.0001, "Byte Array 0 for Neg UInt8 incorrect");
            Assert.AreEqual(1, result.Length, 0.0001, "Byte Array for Neg UInt8 incorrect");
        }

        #endregion

        #region Boolean to Byte Array

        /// <summary>
        /// Convert the Boolean to a Byte Array.  False.
        /// </summary>
        [Test]
        public void BooleanToByteArrayTrue()
        {
            bool value = true;

            byte[] data = new byte[1];
            data[0] = 0x01;

            byte[] result = RTI.MathHelper.BooleanToByteArray(value);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array 0 for Boolean incorrect");
            Assert.AreEqual(data.Length, result.Length, 0.0001, "Byte Array for Boolean incorrect");
        }

        /// <summary>
        /// Convert the Boolean to a Byte Array.  False.
        /// </summary>
        [Test]
        public void BooleanToByteArrayFalse()
        {
            bool value = false;

            byte[] data = new byte[1];
            data[0] = 0x00;

            byte[] result = RTI.MathHelper.BooleanToByteArray(value);

            Assert.AreEqual(data[0], result[0], 0.0001, "Byte Array for Boolean incorrect");
            Assert.AreEqual(1, result.Length, 0.0001, "Byte Array for Boolean incorrect");
        }



        #endregion

        #region String to Byte Array

        /// <summary>
        /// Convert the String to a Byte Array.
        /// </summary>
        [Test]
        public void StringToByteArray()
        {
            string value = "hello";

            byte[] data = new byte[5];
            data[0] = 0x68;  // h
            data[1] = 0x65;  // e
            data[2] = 0x6C;  // l
            data[3] = 0x6C;  // l
            data[4] = 0x6F;  // o

            string result = RTI.MathHelper.ByteArrayToString(data, data.Length, 0, false);

            Assert.AreEqual(data[0], result[0], "Byte Array 0 for String incorrect");
            Assert.AreEqual(data[1], result[1], "Byte Array 1 for String incorrect");
            Assert.AreEqual(data[2], result[2], "Byte Array 2 for String incorrect");
            Assert.AreEqual(data[3], result[3], "Byte Array 3 for String incorrect");
            Assert.AreEqual(data[4], result[4], "Byte Array 4 for String incorrect");
            Assert.AreEqual(value, result, "String incorrect");
            Assert.AreEqual(data.Length, result.Length, "String incorrect");
        }

        /// <summary>
        /// Convert the String to a Byte Array.
        /// Data is given in Big Endian Form.
        /// </summary>
        [Test]
        public void StringToByteArrayBE()
        {
            string value = "hello";

            byte[] data = new byte[5];
            data[4] = 0x68;  // h
            data[3] = 0x65;  // e
            data[2] = 0x6C;  // l
            data[1] = 0x6C;  // l
            data[0] = 0x6F;  // o

            string result = RTI.MathHelper.ByteArrayToString(data, data.Length, 0, true);

            Assert.AreEqual(data[0], result[4], "Byte Array 0 for String incorrect");
            Assert.AreEqual(data[1], result[3], "Byte Array 1 for String incorrect");
            Assert.AreEqual(data[2], result[2], "Byte Array 2 for String incorrect");
            Assert.AreEqual(data[3], result[1], "Byte Array 3 for String incorrect");
            Assert.AreEqual(data[4], result[0], "Byte Array 4 for String incorrect");
            Assert.AreEqual(value, result, "String incorrect");
            Assert.AreEqual(data.Length, result.Length, "String incorrect");
        }

        /// <summary>
        /// Convert the String to a Byte Array.  Make the string in the middle of the array.
        /// </summary>
        [Test]
        public void StringToByteArraySubarray()
        {
            string value = "hello";

            byte[] data = new byte[8];
            data[0] = 0x61;  // a
            data[1] = 0x62;  // b
            data[2] = 0x68;  // h
            data[3] = 0x65;  // e
            data[4] = 0x6C;  // l
            data[5] = 0x6C;  // l
            data[6] = 0x6F;  // o
            data[7] = 0x63;  // c

            string result = RTI.MathHelper.ByteArrayToString(data, 5, 2, false);

            Assert.AreEqual(data[2], result[0], "Byte Array 0 for String incorrect");
            Assert.AreEqual(data[3], result[1], "Byte Array 1 for String incorrect");
            Assert.AreEqual(data[4], result[2], "Byte Array 2 for String incorrect");
            Assert.AreEqual(data[5], result[3], "Byte Array 3 for String incorrect");
            Assert.AreEqual(data[6], result[4], "Byte Array 4 for String incorrect");
            Assert.AreEqual(value, result, "String incorrect");
            Assert.AreEqual(5, result.Length, "String incorrect");
        }

        /// <summary>
        /// Convert the String to a Byte Array.  Make the string in the middle of the array.
        /// </summary>
        [Test]
        public void StringToByteArraySubarrayBE()
        {
            string value = "hello";

            byte[] data = new byte[8];
            data[7] = 0x61;  // a
            data[6] = 0x62;  // b
            data[5] = 0x68;  // h
            data[4] = 0x65;  // e
            data[3] = 0x6C;  // l
            data[2] = 0x6C;  // l
            data[1] = 0x6F;  // o
            data[0] = 0x63;  // c

            string result = RTI.MathHelper.ByteArrayToString(data, 5, 1, true);

            Assert.AreEqual(data[5], result[0], "Byte Array 0 for String incorrect");
            Assert.AreEqual(data[4], result[1], "Byte Array 1 for String incorrect");
            Assert.AreEqual(data[3], result[2], "Byte Array 2 for String incorrect");
            Assert.AreEqual(data[2], result[3], "Byte Array 3 for String incorrect");
            Assert.AreEqual(data[1], result[4], "Byte Array 4 for String incorrect");
            Assert.AreEqual(value, result, "String incorrect");
            Assert.AreEqual(5, result.Length, "String incorrect");
        }

        #endregion

        #region Subarray

        /// <summary>
        /// Take a subarray of the array.
        /// </summary>
        [Test]
        public void Subarray()
        {
            byte[] data = new byte[5];
            data[0] = 0x68;  // h
            data[1] = 0x65;  // e
            data[2] = 0x6C;  // l
            data[3] = 0x6C;  // l
            data[4] = 0x6F;  // o

            byte[] result = RTI.MathHelper.SubArray<byte>(data, 0, 5);

            Assert.AreEqual(data[0], result[0], "Byte Array 0 for String incorrect");
            Assert.AreEqual(data[1], result[1], "Byte Array 1 for String incorrect");
            Assert.AreEqual(data[2], result[2], "Byte Array 2 for String incorrect");
            Assert.AreEqual(data[3], result[3], "Byte Array 3 for String incorrect");
            Assert.AreEqual(data[4], result[4], "Byte Array 4 for String incorrect");
            Assert.AreEqual(data.Length, result.Length, "String incorrect");
        }

        /// <summary>
        /// Take a subarray of the array.
        /// Take data from the middle.
        /// </summary>
        [Test]
        public void SubarrayMiddle()
        {
            byte[] data = new byte[8];
            data[0] = 0x61;  // a
            data[1] = 0x62;  // b
            data[2] = 0x68;  // h
            data[3] = 0x65;  // e
            data[4] = 0x6C;  // l
            data[5] = 0x6C;  // l
            data[6] = 0x6F;  // o
            data[7] = 0x63;  // c

            byte[] result = RTI.MathHelper.SubArray<byte>(data, 2, 5);

            Assert.AreEqual(data[2], result[0], "Byte Array 0 for String incorrect");
            Assert.AreEqual(data[3], result[1], "Byte Array 1 for String incorrect");
            Assert.AreEqual(data[4], result[2], "Byte Array 2 for String incorrect");
            Assert.AreEqual(data[5], result[3], "Byte Array 3 for String incorrect");
            Assert.AreEqual(data[6], result[4], "Byte Array 4 for String incorrect");
            Assert.AreEqual(5, result.Length, "String incorrect");
        }

        #endregion
    }
}
