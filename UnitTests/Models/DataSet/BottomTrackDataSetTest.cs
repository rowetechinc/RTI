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
 * 11/28/2011      RC                     Initial coding
 * 11/29/2011      RC                     Added Prti02Sentence Constructor.
 *                                         Test AdditionalData().
 * 03/29/2012      RC          2.07       Changed the Status to an object in ensemble.
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Unit test of the Bottom Track DataSet object.
    /// </summary>
    [TestFixture]
    public class BottomTrackDataSetTest
    {
        /// <summary>
        /// Test the constructor that takes a Prti01Sentence.
        /// The values used are bad velocity and no depth.
        /// </summary>
        [Test]
        public void TestConstructorPrti01Sentence()
        {
            string nmea = "$PRTI01,379550,1,1468,-99999,-99999,-99999,0,,,,,0004*09";

            // Create Sentence
            Prti01Sentence sent = new Prti01Sentence(nmea);

            Assert.AreEqual(true, sent.IsValid, "NMEA sentence incorrect");

            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Bottom Track data
            adcpData.AddBottomTrackData(sent);

            Assert.AreEqual(true, adcpData.IsBottomTrackAvail);

            Assert.AreEqual(0, adcpData.BottomTrackData.Range[0], "Bottom Track Range B1 incorrect");
            Assert.AreEqual(0, adcpData.BottomTrackData.Range[1], "Bottom Track Range B2 incorrect");
            Assert.AreEqual(0, adcpData.BottomTrackData.Range[2], "Bottom Track Range B3 incorrect");
            Assert.AreEqual(0, adcpData.BottomTrackData.Range[3], "Bottom Track Range B4 incorrect");

            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");
            Assert.AreEqual(0, adcpData.BottomTrackData.InstrumentVelocity[3], "Bottom Track Instrument Velocity Q incorrect");

            Assert.AreEqual(0004, adcpData.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
            Assert.AreEqual(sent.SystemStatus.Value, adcpData.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
        }

        /// <summary>
        /// Test the constructor that takes a Prti01Sentence.
        /// This test will give actual values for the bottom track data.
        /// </summary>
        [Test]
        public void TestConstructorPrti01Sentence1()
        {
            string nmea = "$PRTI01,380250,8,1464,-1205,-24,-347,79380,,,,,0000*05";

            // Create sentence
            Prti01Sentence sent = new Prti01Sentence(nmea);

            Assert.AreEqual(true, sent.IsValid, "NMEA sentence incorrect");

            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Bottom Track data
            adcpData.AddBottomTrackData(sent);

            Assert.AreEqual(true, adcpData.IsBottomTrackAvail, "Bottom Track DataSet not added");

            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, adcpData.BottomTrackData.Range[0], 0.00001, "Bottom Track Range B1 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, adcpData.BottomTrackData.Range[1], 0.00001, "Bottom Track Range B2 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, adcpData.BottomTrackData.Range[2], 0.00001, "Bottom Track Range B3 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, adcpData.BottomTrackData.Range[3], 0.00001, "Bottom Track Range B4 incorrect");

            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");
            Assert.AreEqual(0, adcpData.BottomTrackData.InstrumentVelocity[3], "Bottom Track Instrument Velocity Q incorrect");

            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-1205 mm/s").ToMetersPerSecond().Value), adcpData.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-24 mm/s").ToMetersPerSecond().Value), adcpData.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-347 mm/s").ToMetersPerSecond().Value), adcpData.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");

            Assert.AreEqual(0000, adcpData.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
            Assert.AreEqual(sent.SystemStatus.Value, adcpData.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
        }

        /// <summary>
        /// Test the constructor that takes a Prti02Sentence.
        /// The values used are bad velocity and no depth.
        /// </summary>
        [Test]
        public void TestConstructorPrti02Sentence()
        {
            string nmea = "$PRTI02,379550,1,1468,-99999,-99999,-99999,0,,,,,0004*0A";

            // Create Sentence
            Prti02Sentence sent = new Prti02Sentence(nmea);

            Assert.AreEqual(true, sent.IsValid, "NMEA sentence incorrect");

            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Bottom Track data
            adcpData.AddBottomTrackData(sent);

            Assert.AreEqual(true, adcpData.IsBottomTrackAvail);

            Assert.AreEqual(0, adcpData.BottomTrackData.Range[0], "Bottom Track Range B1 incorrect");
            Assert.AreEqual(0, adcpData.BottomTrackData.Range[1], "Bottom Track Range B2 incorrect");
            Assert.AreEqual(0, adcpData.BottomTrackData.Range[2], "Bottom Track Range B3 incorrect");
            Assert.AreEqual(0, adcpData.BottomTrackData.Range[3], "Bottom Track Range B4 incorrect");

            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.EarthVelocity[0], "Bottom Track Earth Velocity East incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.EarthVelocity[1], "Bottom Track Earth Velocity North incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.EarthVelocity[2], "Bottom Track Earth Velocity Up incorrect");
            Assert.AreEqual(0, adcpData.BottomTrackData.EarthVelocity[3], "Bottom Track Earth Velocity Q incorrect");

            Assert.AreEqual(0004, adcpData.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
            Assert.AreEqual(sent.SystemStatus.Value, adcpData.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
        }

        /// <summary>
        /// Test the constructor that takes a Prti02Sentence.
        /// This test will give actual values for the bottom track data.
        /// </summary>
        [Test]
        public void TestConstructorPrti02Sentence1()
        {
            string nmea = "$PRTI02,380250,8,1464,1142,323,407,79380,,,,,0000*1C";

            // Create sentence
            Prti02Sentence sent = new Prti02Sentence(nmea);

            Assert.AreEqual(true, sent.IsValid, "NMEA sentence incorrect");

            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Bottom Track data
            adcpData.AddBottomTrackData(sent);

            Assert.AreEqual(true, adcpData.IsBottomTrackAvail, "Bottom Track DataSet not added");

            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, adcpData.BottomTrackData.Range[0], 0.00001, "Bottom Track Range B1 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, adcpData.BottomTrackData.Range[1], 0.00001, "Bottom Track Range B2 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, adcpData.BottomTrackData.Range[2], 0.00001, "Bottom Track Range B3 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, adcpData.BottomTrackData.Range[3], 0.00001, "Bottom Track Range B4 incorrect");

            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.EarthVelocity[0], "Bottom Track Earth Velocity East incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.EarthVelocity[1], "Bottom Track Earth Velocity North incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.EarthVelocity[2], "Bottom Track Earth Velocity Up incorrect");
            Assert.AreEqual(0, adcpData.BottomTrackData.EarthVelocity[3], "Bottom Track Earth Velocity Q incorrect");

            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("1142 mm/s").ToMetersPerSecond().Value), adcpData.BottomTrackData.EarthVelocity[0], "Bottom Track Earth Velocity East incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("323 mm/s").ToMetersPerSecond().Value), adcpData.BottomTrackData.EarthVelocity[1], "Bottom Track Earth Velocity North incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("407 mm/s").ToMetersPerSecond().Value), adcpData.BottomTrackData.EarthVelocity[2], "Bottom Track Earth Velocity Up incorrect");

            Assert.AreEqual(0000, adcpData.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
            Assert.AreEqual(sent.SystemStatus.Value, adcpData.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
        }


        /// <summary>
        /// Test combining a Prti01Sentence and a
        /// Prti02Sentence into Bottom Track dataset.
        /// </summary>
        [Test]
        public void TestAddAdditional()
        {
            string nmea1 = "$PRTI01,380250,8,1464,-1205,-24,-347,79380,,,,,0000*05";
            string nmea2 = "$PRTI02,380250,8,1464,1142,323,407,79380,,,,,0000*1C";

            // Create sentence
            Prti01Sentence sent1 = new Prti01Sentence(nmea1);
            Prti02Sentence sent2 = new Prti02Sentence(nmea2);

            Assert.AreEqual(true, sent1.IsValid, "PRTI01 NMEA sentence incorrect: " + nmea1);
            Assert.AreEqual(true, sent2.IsValid, "PRTI02 NMEA sentence incorrect: " + nmea2);

            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Bottom Track data
            adcpData.AddBottomTrackData(sent1);
            adcpData.BottomTrackData.AddAdditionalBottomTrackData(sent2);

            Assert.AreEqual(true, adcpData.IsBottomTrackAvail, "Bottom Track DataSet not added");

            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, adcpData.BottomTrackData.Range[0], 0.00001, "Bottom Track Range B1 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, adcpData.BottomTrackData.Range[1], 0.00001, "Bottom Track Range B2 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, adcpData.BottomTrackData.Range[2], 0.00001, "Bottom Track Range B3 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, adcpData.BottomTrackData.Range[3], 0.00001, "Bottom Track Range B4 incorrect");

            // Instrument data
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");
            Assert.AreEqual(0, adcpData.BottomTrackData.InstrumentVelocity[3], "Bottom Track Instrument Velocity Q incorrect");

            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-1205 mm/s").ToMetersPerSecond().Value), adcpData.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-24 mm/s").ToMetersPerSecond().Value), adcpData.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-347 mm/s").ToMetersPerSecond().Value), adcpData.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");

            // Earth data
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.EarthVelocity[0], "Bottom Track Earth Velocity East incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.EarthVelocity[1], "Bottom Track Earth Velocity North incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, adcpData.BottomTrackData.EarthVelocity[2], "Bottom Track Earth Velocity Up incorrect");
            Assert.AreEqual(0, adcpData.BottomTrackData.EarthVelocity[3], "Bottom Track Earth Velocity Q incorrect");

            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("1142 mm/s").ToMetersPerSecond().Value), adcpData.BottomTrackData.EarthVelocity[0], "Bottom Track Earth Velocity East incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("323 mm/s").ToMetersPerSecond().Value), adcpData.BottomTrackData.EarthVelocity[1], "Bottom Track Earth Velocity North incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("407 mm/s").ToMetersPerSecond().Value), adcpData.BottomTrackData.EarthVelocity[2], "Bottom Track Earth Velocity Up incorrect");

            Assert.AreEqual(0000, adcpData.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
            Assert.AreEqual(sent1.SystemStatus.Value, adcpData.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
            Assert.AreEqual(sent2.SystemStatus.Value, adcpData.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
        }

    }

}