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
 * 01/17/2013      RC          2.17       Added a test for default constructor.  Added a test for encode and decode.
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

        #region NMEA

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

        #endregion

        #region Add Empty DataSet

        /// <summary>
        /// Create an empty dataset.
        /// </summary>
        [Test]
        public void AddEmptyDataSetTest()
        {
            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Bottom Track data
            adcpData.AddBottomTrackData(DataSet.Ensemble.DATATYPE_FLOAT,                    // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.BottomTrackID);                // Dataset ID

            Assert.IsTrue(adcpData.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.ActualPingCount, "Actual Ping Count is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.FirstPingTime, "FirstPingTime is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.LastPingTime, "LastPingTime is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.Heading, "Heading is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.Roll, "Roll is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.WaterTemp, "WaterTemp is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.SystemTemp, "SystemTemp is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.Salinity, "Salinity is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.TransducerDepth, "TransducerDepth is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.SpeedOfSound, "SpeedOfSound is incorrect.");
            Assert.AreEqual(new Status(0), adcpData.BottomTrackData.Status, "Status is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.NumBeams, "NumBeams is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.Range.GetLength(0), "Range Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.SNR.GetLength(0), "SNR Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.Amplitude.GetLength(0), "Amplitude Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.Correlation.GetLength(0), "Correlation Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.BeamVelocity.GetLength(0), "BeamVelocity Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.BeamGood.GetLength(0), "BeamGood Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.InstrumentVelocity.GetLength(0), "InstrumentVelocity Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.InstrumentGood.GetLength(0), "InstrumentGood Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.EarthVelocity.GetLength(0), "EarthVelocity Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.EarthGood.GetLength(0), "EarthGood Length is incorrect.");

        }

        #endregion

        #region Encode Decode

        /// <summary>
        /// Encode Decode Test.
        /// </summary>
        [Test]
        public void EncodeDecodeTest()
        {
            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Bottom Track data
            adcpData.AddBottomTrackData(DataSet.Ensemble.DATATYPE_FLOAT,                    // Type of data stored (Float or Int)
                                            1,                                              // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.BottomTrackID);                // Dataset ID

            Assert.IsTrue(adcpData.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.ActualPingCount, "Actual Ping Count is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.FirstPingTime, "FirstPingTime is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.LastPingTime, "LastPingTime is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.Heading, "Heading is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.Roll, "Roll is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.WaterTemp, "WaterTemp is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.SystemTemp, "SystemTemp is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.Salinity, "Salinity is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.TransducerDepth, "TransducerDepth is incorrect.");
            Assert.AreEqual(0.0f, adcpData.BottomTrackData.SpeedOfSound, "SpeedOfSound is incorrect.");
            Assert.AreEqual(new Status(0), adcpData.BottomTrackData.Status, "Status is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.NumBeams, "NumBeams is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.Range.GetLength(0), "Range Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.SNR.GetLength(0), "SNR Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.Amplitude.GetLength(0), "Amplitude Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.Correlation.GetLength(0), "Correlation Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.BeamVelocity.GetLength(0), "BeamVelocity Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.BeamGood.GetLength(0), "BeamGood Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.InstrumentVelocity.GetLength(0), "InstrumentVelocity Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.InstrumentGood.GetLength(0), "InstrumentGood Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.EarthVelocity.GetLength(0), "EarthVelocity Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.EarthGood.GetLength(0), "EarthGood Length is incorrect.");

            #region Set Values

            // Set the values
            adcpData.BottomTrackData.ActualPingCount = 1.2f;
            adcpData.BottomTrackData.FirstPingTime = 2.3f;
            adcpData.BottomTrackData.LastPingTime = 3.4f;
            adcpData.BottomTrackData.Heading = 4.5f;
            adcpData.BottomTrackData.Pitch = 5.6f;
            adcpData.BottomTrackData.Roll = 6.7f;
            adcpData.BottomTrackData.WaterTemp = 7.8f;
            adcpData.BottomTrackData.SystemTemp = 8.9f;
            adcpData.BottomTrackData.Salinity = 9.10f;
            adcpData.BottomTrackData.Pressure = 10.11f;
            adcpData.BottomTrackData.TransducerDepth = 11.12f;
            adcpData.BottomTrackData.SpeedOfSound = 12.13f;
            adcpData.BottomTrackData.Status = new Status(4);
            adcpData.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
            adcpData.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
            adcpData.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
            adcpData.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

            adcpData.BottomTrackData.SNR[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
            adcpData.BottomTrackData.SNR[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
            adcpData.BottomTrackData.SNR[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
            adcpData.BottomTrackData.SNR[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

            adcpData.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
            adcpData.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
            adcpData.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
            adcpData.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

            adcpData.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
            adcpData.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
            adcpData.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
            adcpData.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

            adcpData.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
            adcpData.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
            adcpData.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
            adcpData.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

            adcpData.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_0_INDEX] = 1;
            adcpData.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_1_INDEX] = 1;
            adcpData.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_2_INDEX] = 1;
            adcpData.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_3_INDEX] = 1;

            adcpData.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
            adcpData.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
            adcpData.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
            adcpData.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

            adcpData.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_0_INDEX] = 1;
            adcpData.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_1_INDEX] = 1;
            adcpData.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_2_INDEX] = 1;
            adcpData.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_3_INDEX] = 1;

            adcpData.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
            adcpData.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
            adcpData.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
            adcpData.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

            adcpData.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_0_INDEX] = 1;
            adcpData.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_1_INDEX] = 1;
            adcpData.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_2_INDEX] = 1;
            adcpData.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_3_INDEX] = 1;

            #endregion

            #region Verify Values

            Assert.IsTrue(adcpData.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(1.2f, adcpData.BottomTrackData.ActualPingCount, "Actual Ping Count is incorrect.");
            Assert.AreEqual(2.3f, adcpData.BottomTrackData.FirstPingTime, "FirstPingTime is incorrect.");
            Assert.AreEqual(3.4f, adcpData.BottomTrackData.LastPingTime, "LastPingTime is incorrect.");
            Assert.AreEqual(4.5f, adcpData.BottomTrackData.Heading, "Heading is incorrect.");
            Assert.AreEqual(5.6f, adcpData.BottomTrackData.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(6.7f, adcpData.BottomTrackData.Roll, "Roll is incorrect.");
            Assert.AreEqual(7.8f, adcpData.BottomTrackData.WaterTemp, "WaterTemp is incorrect.");
            Assert.AreEqual(8.9f, adcpData.BottomTrackData.SystemTemp, "SystemTemp is incorrect.");
            Assert.AreEqual(9.10f, adcpData.BottomTrackData.Salinity, "Salinity is incorrect.");
            Assert.AreEqual(10.11f, adcpData.BottomTrackData.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(11.12f, adcpData.BottomTrackData.TransducerDepth, "TransducerDepth is incorrect.");
            Assert.AreEqual(12.13f, adcpData.BottomTrackData.SpeedOfSound, "SpeedOfSound is incorrect.");
            Assert.AreEqual(new Status(4), adcpData.BottomTrackData.Status, "Status is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.NumBeams, "NumBeams is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.Range.GetLength(0), "Range Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.SNR.GetLength(0), "SNR Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.Amplitude.GetLength(0), "Amplitude Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.Correlation.GetLength(0), "Correlation Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.BeamVelocity.GetLength(0), "BeamVelocity Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.BeamGood.GetLength(0), "BeamGood Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.InstrumentVelocity.GetLength(0), "InstrumentVelocity Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.InstrumentGood.GetLength(0), "InstrumentGood Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.EarthVelocity.GetLength(0), "EarthVelocity Length is incorrect.");
            Assert.AreEqual(4, adcpData.BottomTrackData.EarthGood.GetLength(0), "EarthGood Length is incorrect.");

            Assert.AreEqual(1.2f, adcpData.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX], "Range 0 is incorrect.");
            Assert.AreEqual(2.3f, adcpData.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX], "Range 1 is incorrect.");
            Assert.AreEqual(3.4f, adcpData.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX], "Range 2 is incorrect.");
            Assert.AreEqual(4.5f, adcpData.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX], "Range 3 is incorrect.");

            Assert.AreEqual(1.2f, adcpData.BottomTrackData.SNR[DataSet.Ensemble.BEAM_0_INDEX], "SNR 0 is incorrect.");
            Assert.AreEqual(2.3f, adcpData.BottomTrackData.SNR[DataSet.Ensemble.BEAM_1_INDEX], "SNR 1 is incorrect.");
            Assert.AreEqual(3.4f, adcpData.BottomTrackData.SNR[DataSet.Ensemble.BEAM_2_INDEX], "SNR 2 is incorrect.");
            Assert.AreEqual(4.5f, adcpData.BottomTrackData.SNR[DataSet.Ensemble.BEAM_3_INDEX], "SNR 3 is incorrect.");

            Assert.AreEqual(1.2f, adcpData.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_0_INDEX], "Amplitude 0 is incorrect.");
            Assert.AreEqual(2.3f, adcpData.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_1_INDEX], "Amplitude 1 is incorrect.");
            Assert.AreEqual(3.4f, adcpData.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_2_INDEX], "Amplitude 2 is incorrect.");
            Assert.AreEqual(4.5f, adcpData.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_3_INDEX], "Amplitude 3 is incorrect.");

            Assert.AreEqual(1.2f, adcpData.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_0_INDEX], "Correlation 0 is incorrect.");
            Assert.AreEqual(2.3f, adcpData.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_1_INDEX], "Correlation 1 is incorrect.");
            Assert.AreEqual(3.4f, adcpData.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_2_INDEX], "Correlation 2 is incorrect.");
            Assert.AreEqual(4.5f, adcpData.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_3_INDEX], "Correlation 3 is incorrect.");

            Assert.AreEqual(1.2f, adcpData.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_0_INDEX], "BeamVelocity 0 is incorrect.");
            Assert.AreEqual(2.3f, adcpData.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_1_INDEX], "BeamVelocity 1 is incorrect.");
            Assert.AreEqual(3.4f, adcpData.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_2_INDEX], "BeamVelocity 2 is incorrect.");
            Assert.AreEqual(4.5f, adcpData.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_3_INDEX], "BeamVelocity 3 is incorrect.");

            Assert.AreEqual(1, adcpData.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_0_INDEX], "BeamGood 0 is incorrect.");
            Assert.AreEqual(1, adcpData.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_1_INDEX], "BeamGood 1 is incorrect.");
            Assert.AreEqual(1, adcpData.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_2_INDEX], "BeamGood 2 is incorrect.");
            Assert.AreEqual(1, adcpData.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_3_INDEX], "BeamGood 3 is incorrect.");

            Assert.AreEqual(1.2f, adcpData.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_0_INDEX], "InstrumentVelocity 0 is incorrect.");
            Assert.AreEqual(2.3f, adcpData.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_1_INDEX], "InstrumentVelocity 1 is incorrect.");
            Assert.AreEqual(3.4f, adcpData.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_2_INDEX], "InstrumentVelocity 2 is incorrect.");
            Assert.AreEqual(4.5f, adcpData.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_3_INDEX], "InstrumentVelocity 3 is incorrect.");

            Assert.AreEqual(1, adcpData.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_0_INDEX], "InstrumentGood 0 is incorrect.");
            Assert.AreEqual(1, adcpData.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_1_INDEX], "InstrumentGood 1 is incorrect.");
            Assert.AreEqual(1, adcpData.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_2_INDEX], "InstrumentGood 2 is incorrect.");
            Assert.AreEqual(1, adcpData.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_3_INDEX], "InstrumentGood 3 is incorrect.");


            Assert.AreEqual(1.2f, adcpData.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_0_INDEX], "EarthVelocity 0 is incorrect.");
            Assert.AreEqual(2.3f, adcpData.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_1_INDEX], "EarthVelocity 1 is incorrect.");
            Assert.AreEqual(3.4f, adcpData.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_2_INDEX], "EarthVelocity 2 is incorrect.");
            Assert.AreEqual(4.5f, adcpData.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_3_INDEX], "EarthVelocity 3 is incorrect.");

            Assert.AreEqual(1, adcpData.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_0_INDEX], "EarthGood 0 is incorrect.");
            Assert.AreEqual(1, adcpData.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_1_INDEX], "EarthGood 1 is incorrect.");
            Assert.AreEqual(1, adcpData.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_2_INDEX], "EarthGood 2 is incorrect.");
            Assert.AreEqual(1, adcpData.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_3_INDEX], "EarthGood 3 is incorrect.");


            #endregion

            // Encode the data
            byte[] encode = adcpData.BottomTrackData.Encode();
            
            // Add Bottom Track data
            DataSet.Ensemble ens1 = new DataSet.Ensemble();
            ens1.AddBottomTrackData(DataSet.Ensemble.DATATYPE_FLOAT,                    // Type of data stored (Float or Int)
                                            1,                                              // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.BottomTrackID,                 // Dataset ID
                                            encode);                                        // Encoded data

            #region Check Encode data Values

            Assert.IsTrue(ens1.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(1.2f, ens1.BottomTrackData.ActualPingCount, "Actual Ping Count is incorrect.");
            Assert.AreEqual(2.3f, ens1.BottomTrackData.FirstPingTime, "FirstPingTime is incorrect.");
            Assert.AreEqual(3.4f, ens1.BottomTrackData.LastPingTime, "LastPingTime is incorrect.");
            Assert.AreEqual(4.5f, ens1.BottomTrackData.Heading, "Heading is incorrect.");
            Assert.AreEqual(5.6f, ens1.BottomTrackData.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(6.7f, ens1.BottomTrackData.Roll, "Roll is incorrect.");
            Assert.AreEqual(7.8f, ens1.BottomTrackData.WaterTemp, "WaterTemp is incorrect.");
            Assert.AreEqual(8.9f, ens1.BottomTrackData.SystemTemp, "SystemTemp is incorrect.");
            Assert.AreEqual(9.10f, ens1.BottomTrackData.Salinity, "Salinity is incorrect.");
            Assert.AreEqual(10.11f, ens1.BottomTrackData.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(11.12f, ens1.BottomTrackData.TransducerDepth, "TransducerDepth is incorrect.");
            Assert.AreEqual(12.13f, ens1.BottomTrackData.SpeedOfSound, "SpeedOfSound is incorrect.");
            Assert.AreEqual(new Status(4), ens1.BottomTrackData.Status, "Status is incorrect.");
            Assert.AreEqual(4, ens1.BottomTrackData.NumBeams, "NumBeams is incorrect.");
            Assert.AreEqual(4, ens1.BottomTrackData.Range.GetLength(0), "Range Length is incorrect.");
            Assert.AreEqual(4, ens1.BottomTrackData.SNR.GetLength(0), "SNR Length is incorrect.");
            Assert.AreEqual(4, ens1.BottomTrackData.Amplitude.GetLength(0), "Amplitude Length is incorrect.");
            Assert.AreEqual(4, ens1.BottomTrackData.Correlation.GetLength(0), "Correlation Length is incorrect.");
            Assert.AreEqual(4, ens1.BottomTrackData.BeamVelocity.GetLength(0), "BeamVelocity Length is incorrect.");
            Assert.AreEqual(4, ens1.BottomTrackData.BeamGood.GetLength(0), "BeamGood Length is incorrect.");
            Assert.AreEqual(4, ens1.BottomTrackData.InstrumentVelocity.GetLength(0), "InstrumentVelocity Length is incorrect.");
            Assert.AreEqual(4, ens1.BottomTrackData.InstrumentGood.GetLength(0), "InstrumentGood Length is incorrect.");
            Assert.AreEqual(4, ens1.BottomTrackData.EarthVelocity.GetLength(0), "EarthVelocity Length is incorrect.");
            Assert.AreEqual(4, ens1.BottomTrackData.EarthGood.GetLength(0), "EarthGood Length is incorrect.");

            Assert.AreEqual(1.2f, ens1.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX], "Range 0 is incorrect.");
            Assert.AreEqual(2.3f, ens1.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX], "Range 1 is incorrect.");
            Assert.AreEqual(3.4f, ens1.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX], "Range 2 is incorrect.");
            Assert.AreEqual(4.5f, ens1.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX], "Range 3 is incorrect.");

            Assert.AreEqual(1.2f, ens1.BottomTrackData.SNR[DataSet.Ensemble.BEAM_0_INDEX], "SNR 0 is incorrect.");
            Assert.AreEqual(2.3f, ens1.BottomTrackData.SNR[DataSet.Ensemble.BEAM_1_INDEX], "SNR 1 is incorrect.");
            Assert.AreEqual(3.4f, ens1.BottomTrackData.SNR[DataSet.Ensemble.BEAM_2_INDEX], "SNR 2 is incorrect.");
            Assert.AreEqual(4.5f, ens1.BottomTrackData.SNR[DataSet.Ensemble.BEAM_3_INDEX], "SNR 3 is incorrect.");

            Assert.AreEqual(1.2f, ens1.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_0_INDEX], "Amplitude 0 is incorrect.");
            Assert.AreEqual(2.3f, ens1.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_1_INDEX], "Amplitude 1 is incorrect.");
            Assert.AreEqual(3.4f, ens1.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_2_INDEX], "Amplitude 2 is incorrect.");
            Assert.AreEqual(4.5f, ens1.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_3_INDEX], "Amplitude 3 is incorrect.");

            Assert.AreEqual(1.2f, ens1.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_0_INDEX], "Correlation 0 is incorrect.");
            Assert.AreEqual(2.3f, ens1.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_1_INDEX], "Correlation 1 is incorrect.");
            Assert.AreEqual(3.4f, ens1.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_2_INDEX], "Correlation 2 is incorrect.");
            Assert.AreEqual(4.5f, ens1.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_3_INDEX], "Correlation 3 is incorrect.");

            Assert.AreEqual(1.2f, ens1.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_0_INDEX], "BeamVelocity 0 is incorrect.");
            Assert.AreEqual(2.3f, ens1.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_1_INDEX], "BeamVelocity 1 is incorrect.");
            Assert.AreEqual(3.4f, ens1.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_2_INDEX], "BeamVelocity 2 is incorrect.");
            Assert.AreEqual(4.5f, ens1.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_3_INDEX], "BeamVelocity 3 is incorrect.");

            Assert.AreEqual(1, ens1.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_0_INDEX], "BeamGood 0 is incorrect.");
            Assert.AreEqual(1, ens1.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_1_INDEX], "BeamGood 1 is incorrect.");
            Assert.AreEqual(1, ens1.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_2_INDEX], "BeamGood 2 is incorrect.");
            Assert.AreEqual(1, ens1.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_3_INDEX], "BeamGood 3 is incorrect.");

            Assert.AreEqual(1.2f, ens1.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_0_INDEX], "InstrumentVelocity 0 is incorrect.");
            Assert.AreEqual(2.3f, ens1.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_1_INDEX], "InstrumentVelocity 1 is incorrect.");
            Assert.AreEqual(3.4f, ens1.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_2_INDEX], "InstrumentVelocity 2 is incorrect.");
            Assert.AreEqual(4.5f, ens1.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_3_INDEX], "InstrumentVelocity 3 is incorrect.");

            Assert.AreEqual(1, ens1.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_0_INDEX], "InstrumentGood 0 is incorrect.");
            Assert.AreEqual(1, ens1.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_1_INDEX], "InstrumentGood 1 is incorrect.");
            Assert.AreEqual(1, ens1.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_2_INDEX], "InstrumentGood 2 is incorrect.");
            Assert.AreEqual(1, ens1.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_3_INDEX], "InstrumentGood 3 is incorrect.");


            Assert.AreEqual(1.2f, ens1.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_0_INDEX], "EarthVelocity 0 is incorrect.");
            Assert.AreEqual(2.3f, ens1.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_1_INDEX], "EarthVelocity 1 is incorrect.");
            Assert.AreEqual(3.4f, ens1.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_2_INDEX], "EarthVelocity 2 is incorrect.");
            Assert.AreEqual(4.5f, ens1.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_3_INDEX], "EarthVelocity 3 is incorrect.");

            Assert.AreEqual(1, ens1.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_0_INDEX], "EarthGood 0 is incorrect.");
            Assert.AreEqual(1, ens1.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_1_INDEX], "EarthGood 1 is incorrect.");
            Assert.AreEqual(1, ens1.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_2_INDEX], "EarthGood 2 is incorrect.");
            Assert.AreEqual(1, ens1.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_3_INDEX], "EarthGood 3 is incorrect.");


            #endregion

        }

        #endregion

    }

}