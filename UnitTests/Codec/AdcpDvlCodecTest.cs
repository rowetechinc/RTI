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
 * 12/08/2011      RC          1.09       Added Water Mass dataset parsing.
 * 12/14/2011      RC          1.09       Added EventWaitHandle and Init() to handle timing.
 * 12/20/2011      RC          1.10       Added test to test bad checksum (1 byte) with a good sentence.
 * 01/03/2012      RC          1.11       Fixed test using DVL Codec event instead of CurrentDataSetManager event.
 * 03/29/2012      RC          2.07       Changed the Status to an object in ensemble.
 * 02/20/2013      RC          2.18       Updated test with latest changes to setting the time for PRTI sentences.
 * 02/28/2013      RC          2.18       Fixed FirstPingTime for PRTI sentences.
 * 09/11/2013      RC          2.19.5     Updated test to 2.19.5
 * 
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Threading;
    using System.ComponentModel;

    /// <summary>
    /// Unit test of the ADCP DVL Codec.
    /// </summary>
    [TestFixture]
    public class AdcpDvlCodecTest
    {
        private const int TIMEOUT = 5000;
        private EventWaitHandle _eventWaitResponse;

        [TestFixtureSetUp]
        public void Init()
        {
            // Setup the wait and initialize it off and make it manually reset
            _eventWaitResponse = new EventWaitHandle(false, EventResetMode.ManualReset);
        }

        // TIMING IS AN ISSUE WITH THIS TEST
        // NEED TO WAIT FOR EVENT BUT TEST FAILS BEFORE EVENT OCCURS
        private DataSet.Ensemble recvData01;
        /// <summary>
        /// Test that takes a Prti01Sentence and sets all the values.
        /// </summary>
        [Test]
        public void TestPrti01()
        {
            string nmea = "$PRTI01,1000,8,1464,-1205,-24,-347,79380,300,-200,400,100,0000*21";

            //RecorderManager.Instance.FlushEnsembleDatabase();

            AdcpDvlCodec codec = new AdcpDvlCodec();
            codec.ProcessDataEvent += new AdcpDvlCodec.ProcessDataEventHandler(ReceiveMsg);
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea));
            codec.Dispose(); 

            _eventWaitResponse.WaitOne(TIMEOUT);

            Assert.IsNotNull(recvData01, "Message was not received.");

            // Check if Ensemble data set available
            Assert.AreEqual(true, recvData01.IsEnsembleAvail, "Ensemble Data Set not available");
            Assert.AreEqual(8, recvData01.EnsembleData.EnsembleNumber, "Ensemble number is incorrect");

            // Check number of beams
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, recvData01.EnsembleData.NumBeams, "Number of beams is Incorrect");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, recvData01.EnsembleData.ElementsMultiplier, "Element Multiplier is Incorrect");

            // Check number of bins
            Assert.AreEqual(0, recvData01.EnsembleData.NumBins);

            //// Check time
            Assert.AreEqual(DateTime.Now.Hour, recvBad.EnsembleData.EnsDateTime.Hour, "Incorrect Ensemble time in Hours " + recvBad.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Minute, recvBad.EnsembleData.EnsDateTime.Minute, "Incorrect Ensemble time in Minutes " + recvBad.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Second, recvBad.EnsembleData.EnsDateTime.Second, "Incorrect Ensemble time in Seconds " + recvBad.EnsembleData.EnsDateTime.ToString());
            //Assert.AreEqual(DateTime.Now.Millisecond, recvBad.EnsembleData.EnsDateTime.Millisecond, "Incorrece Ensemble time in Milliseconds " + recvBad.EnsembleData.EnsDateTime.ToString());

            // Check Ancillary data set available
            Assert.AreEqual(true, recvData01.IsAncillaryAvail, "Ancillary Data Set not available");

            // Check Tempearture
            Assert.AreEqual(14.64, recvData01.AncillaryData.WaterTemp, 0.00001, "Water Temperature was incorrect");

            Assert.AreEqual(0, recvData01.AncillaryData.FirstBinRange, "First Bin Range was incorrect");
            Assert.AreEqual(0, recvData01.AncillaryData.BinSize, "Bin Size was incorrect");
            Assert.AreEqual(0, recvData01.AncillaryData.FirstPingTime, "First Ping Time was incorrect");
            Assert.AreEqual(10, recvData01.AncillaryData.LastPingTime, "Last Ping Time was incorrect");
            Assert.AreEqual(0, recvData01.AncillaryData.Heading, "Heading was incorrect");
            Assert.AreEqual(0, recvData01.AncillaryData.Pitch, "Pitch was incorrect");
            Assert.AreEqual(0, recvData01.AncillaryData.Roll, "Roll was incorrect");
            Assert.AreEqual(0, recvData01.AncillaryData.SystemTemp, "System Temp was incorrect");
            Assert.AreEqual(0, recvData01.AncillaryData.Salinity, "Salinity was incorrect");
            Assert.AreEqual(0, recvData01.AncillaryData.Pressure, "Pressure was incorrect");
            Assert.AreEqual(0, recvData01.AncillaryData.TransducerDepth, "Transducer Depth was incorrect");
            Assert.AreEqual(0, recvData01.AncillaryData.SpeedOfSound, "Speed Of Sound was incorrect");

            Assert.AreEqual(true, recvData01.IsBottomTrackAvail, "Bottom Track DataSet not added");

            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvData01.BottomTrackData.Range[0], 0.00001, "Bottom Track Range B1 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvData01.BottomTrackData.Range[1], 0.00001, "Bottom Track Range B2 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvData01.BottomTrackData.Range[2], 0.00001, "Bottom Track Range B3 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvData01.BottomTrackData.Range[3], 0.00001, "Bottom Track Range B4 incorrect");

            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, recvData01.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, recvData01.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, recvData01.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");
            Assert.AreEqual(0, recvData01.BottomTrackData.InstrumentVelocity[3], "Bottom Track Instrument Velocity Q incorrect");

            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-1205 mm/s").ToMetersPerSecond().Value), recvData01.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-24 mm/s").ToMetersPerSecond().Value), recvData01.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-347 mm/s").ToMetersPerSecond().Value), recvData01.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");

            Assert.IsTrue(recvData01.IsInstrumentWaterMassAvail, "Instrument Water Mass not added to dataset");

            Assert.AreEqual(recvData01.InstrumentWaterMassData.VelocityX, (new DotSpatial.Positioning.Speed(300, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, 0.00001, "Instrument Water Mass East not properly set.");
            Assert.AreEqual(recvData01.InstrumentWaterMassData.VelocityY, (new DotSpatial.Positioning.Speed(-200, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, 0.00001, "Instrument Water Mass North not properly set.");
            Assert.AreEqual(recvData01.InstrumentWaterMassData.VelocityZ, (new DotSpatial.Positioning.Speed(400, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, 0.00001, "Instrument Water Mass Vertical not properly set.");
            Assert.AreEqual(recvData01.InstrumentWaterMassData.WaterMassDepthLayer, (new DotSpatial.Positioning.Distance(100, DotSpatial.Positioning.DistanceUnit.Millimeters)).ToMeters().Value, 0.00001, "Instrument Water Mass Depth Layer not properly set.");

            Assert.AreEqual(0000, recvData01.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
        }

         //TIMING IS AN ISSUE WITH THIS TEST
         //NEED TO WAIT FOR EVENT BUT TEST FAILS BEFORE EVENT OCCURS
        /// <summary>
        /// Receive the data from the event handler and set the value.
        /// Then unsubscribe.
        /// </summary>
        /// <param name="binaryData">Binary data.</param>
        /// <param name="adcpData">Data from event handler.</param>
        public void ReceiveMsg(byte[] binaryData, DataSet.Ensemble adcpData)
        {
            recvData01 = adcpData;
            _eventWaitResponse.Set();
        }

        private DataSet.Ensemble recvData02;
        /// <summary>
        /// Test that takes a Prti02Sentence and sets all the values.
        /// </summary>
        [Test]
        public void TestPrti02()
        {
            string nmea = "$PRTI02,1000,8,1464,-1205,-24,-347,79380,300,-200,400,100,0000*22";

            //CurrentDataSetManager.Instance.ReceiveRecordDataset += new CurrentDataSetManager.RecordDatasetEventHandler(ReceiveMsg02);

            AdcpDvlCodec codec = new AdcpDvlCodec();
            codec.ProcessDataEvent += new AdcpDvlCodec.ProcessDataEventHandler(ReceiveMsg02);
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea));
            codec.Dispose();                                                           // Clear the remaining data

            _eventWaitResponse.WaitOne(TIMEOUT);

            Assert.IsNotNull(recvData02, "Message was not received.");

            // Check if Ensemble data set available
            Assert.AreEqual(true, recvData02.IsEnsembleAvail, "Ensemble Data Set not available");
            Assert.AreEqual(8, recvData02.EnsembleData.EnsembleNumber, "Ensemble number is incorrect");

            // Check number of beams
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, recvData02.EnsembleData.NumBeams, "Number of beams is Incorrect");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, recvData02.EnsembleData.ElementsMultiplier, "Element Multiplier is Incorrect");

            // Check number of bins
            Assert.AreEqual(0, recvData02.EnsembleData.NumBins);

            // Check time
            Assert.AreEqual(DateTime.Now.Hour, recvBad.EnsembleData.EnsDateTime.Hour, "Incorrect Ensemble time in Hours " + recvBad.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Minute, recvBad.EnsembleData.EnsDateTime.Minute, "Incorrect Ensemble time in Minutes " + recvBad.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Second, recvBad.EnsembleData.EnsDateTime.Second, "Incorrect Ensemble time in Seconds " + recvBad.EnsembleData.EnsDateTime.ToString());
            //Assert.AreEqual(DateTime.Now.Millisecond, recvBad.EnsembleData.EnsDateTime.Millisecond, "Incorrece Ensemble time in Milliseconds " + recvBad.EnsembleData.EnsDateTime.ToString());

            // Check Ancillary data set available
            Assert.AreEqual(true, recvData02.IsAncillaryAvail, "Ancillary Data Set not available");

            // Check Tempearture
            Assert.AreEqual(14.64, recvData02.AncillaryData.WaterTemp, 0.00001, "Water Temperature was incorrect");

            Assert.AreEqual(0, recvData02.AncillaryData.FirstBinRange, "First Bin Range was incorrect");
            Assert.AreEqual(0, recvData02.AncillaryData.BinSize, "Bin Size was incorrect");
            Assert.AreEqual(0, recvData02.AncillaryData.FirstPingTime, "First Ping Time was incorrect");
            Assert.AreEqual(10, recvData02.AncillaryData.LastPingTime, "Last Ping Time was incorrect");
            Assert.AreEqual(0, recvData02.AncillaryData.Heading, "Heading was incorrect");
            Assert.AreEqual(0, recvData02.AncillaryData.Pitch, "Pitch was incorrect");
            Assert.AreEqual(0, recvData02.AncillaryData.Roll, "Roll was incorrect");
            Assert.AreEqual(0, recvData02.AncillaryData.SystemTemp, "System Temp was incorrect");
            Assert.AreEqual(0, recvData02.AncillaryData.Salinity, "Salinity was incorrect");
            Assert.AreEqual(0, recvData02.AncillaryData.Pressure, "Pressure was incorrect");
            Assert.AreEqual(0, recvData02.AncillaryData.TransducerDepth, "Transducer Depth was incorrect");
            Assert.AreEqual(0, recvData02.AncillaryData.SpeedOfSound, "Speed Of Sound was incorrect");

            Assert.AreEqual(true, recvData02.IsBottomTrackAvail, "Bottom Track DataSet not added");

            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvData02.BottomTrackData.Range[0], 0.00001, "Bottom Track Range B1 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvData02.BottomTrackData.Range[1], 0.00001, "Bottom Track Range B2 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvData02.BottomTrackData.Range[2], 0.00001, "Bottom Track Range B3 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvData02.BottomTrackData.Range[3], 0.00001, "Bottom Track Range B4 incorrect");

            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, recvData02.BottomTrackData.EarthVelocity[0], "Bottom Track Earth Velocity East incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, recvData02.BottomTrackData.EarthVelocity[1], "Bottom Track Earth Velocity North incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, recvData02.BottomTrackData.EarthVelocity[2], "Bottom Track Earth Velocity Vertical incorrect");
            Assert.AreEqual(0, recvData02.BottomTrackData.EarthVelocity[3], "Bottom Track Earth Velocity Q incorrect");

            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-1205 mm/s").ToMetersPerSecond().Value), recvData02.BottomTrackData.EarthVelocity[0], "Bottom Track Earth Velocity East incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-24 mm/s").ToMetersPerSecond().Value), recvData02.BottomTrackData.EarthVelocity[1], "Bottom Track Earth Velocity North incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-347 mm/s").ToMetersPerSecond().Value), recvData02.BottomTrackData.EarthVelocity[2], "Bottom Track Earth Velocity Vertical incorrect");

            Assert.IsTrue(recvData02.IsEarthWaterMassAvail, "Earth Water Mass not added to dataset");

            Assert.AreEqual(recvData02.EarthWaterMassData.VelocityEast, (new DotSpatial.Positioning.Speed(300, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, 0.00001, "Earth Water Mass East not properly set.");
            Assert.AreEqual(recvData02.EarthWaterMassData.VelocityNorth, (new DotSpatial.Positioning.Speed(-200, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, 0.00001, "Earth Water Mass North not properly set.");
            Assert.AreEqual(recvData02.EarthWaterMassData.VelocityVertical, (new DotSpatial.Positioning.Speed(400, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, 0.00001, "Earth Water Mass Vertical not properly set.");
            Assert.AreEqual(recvData02.EarthWaterMassData.WaterMassDepthLayer, (new DotSpatial.Positioning.Distance(100, DotSpatial.Positioning.DistanceUnit.Millimeters)).ToMeters().Value, 0.00001, "Earth Water Mass Depth Layer not properly set.");

            Assert.AreEqual(0000, recvData02.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
        }


        /// <summary>
        /// Receive the data from the event handler and set the value.
        /// Then unsubscribe.
        /// </summary>
        /// <param name="binaryData">Binary Data</param>
        /// <param name="adcpData">Data from event handler.</param>
        public void ReceiveMsg02(byte[] binaryData, DataSet.Ensemble adcpData)
        {
            recvData02 = adcpData;
            _eventWaitResponse.Set();
        }

        private DataSet.Ensemble recvData1;
        /// <summary>
        /// Test that takes a Prti01Sentence and sets all the values.
        /// Send the NMEA data in two seperate buffers. The codec should combine
        /// the buffer and create a complete NMEA message.
        /// </summary>
        [Test]
        public void TestPrti01Incomplete()
        {
            string nmea = "$PRTI01,380250,9,1464,-1205,";
            recvData1 = null;

            //CurrentDataSetManager.Instance.ReceiveRecordDataset += new CurrentDataSetManager.RecordDatasetEventHandler(ReceiveMsgIncomplete);

            AdcpDvlCodec codec = new AdcpDvlCodec();
            codec.ProcessDataEvent += new AdcpDvlCodec.ProcessDataEventHandler(ReceiveMsgIncomplete);
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea));

            string nmea1 = "-24,-347,79380,,,,,0000*04";
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea1));
            codec.Dispose();

            _eventWaitResponse.WaitOne(TIMEOUT);

            // Check if the ensemble could be created
            Assert.IsNotNull(recvData1, "Data was not processed");
        }

        /// <summary>
        /// Receive the data from the event handler and set
        /// the value.
        /// Then unsubscribe.
        /// </summary>
        /// <param name="binaryData">Binary Data</param>
        /// <param name="adcpData">Data from event handler.</param>
        public void ReceiveMsgIncomplete(byte[] binaryData, DataSet.Ensemble adcpData)
        {
            recvData1 = adcpData;
            _eventWaitResponse.Set();
        }

        private DataSet.Ensemble recvBad;
        /// <summary>
        /// Send Bad data then good data and
        /// ensure the data can be processed.
        /// </summary>
        [Test]
        public void TestBadThenGoodData()
        {
            string badNmea = "This is a a bad nmea string.";
            string goodNmea = "$PRTI01,1000,10,1464,-1205,-24,-347,79380,,,,,0000*31";
            recvData1 = null;

            AdcpDvlCodec codec = new AdcpDvlCodec();
            codec.ProcessDataEvent += new AdcpDvlCodec.ProcessDataEventHandler(ReceiveMsgBad);
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(badNmea));
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(goodNmea));
            codec.Dispose();                                                           // Clear the remaining data

            _eventWaitResponse.WaitOne(TIMEOUT);

            Assert.IsNotNull(recvBad, "Message was not received.");

            // Check if Ensemble data set available
            Assert.AreEqual(true, recvBad.IsEnsembleAvail, "Ensemble Data Set not available");
            Assert.AreEqual(10, recvBad.EnsembleData.EnsembleNumber, "Ensemble number is incorrect");

            // Check number of beams
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, recvBad.EnsembleData.NumBeams, "Number of beams is Incorrect");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, recvBad.EnsembleData.ElementsMultiplier, "Element Multiplier is Incorrect");

            // Check number of bins
            Assert.AreEqual(0, recvBad.EnsembleData.NumBins);

            // Check time
            Assert.AreEqual(DateTime.Now.Hour, recvBad.EnsembleData.EnsDateTime.Hour, "Incorrect Ensemble time in Hours " + recvBad.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Minute, recvBad.EnsembleData.EnsDateTime.Minute, "Incorrect Ensemble time in Minutes " + recvBad.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Second, recvBad.EnsembleData.EnsDateTime.Second, "Incorrect Ensemble time in Seconds " + recvBad.EnsembleData.EnsDateTime.ToString());
            //Assert.AreEqual(DateTime.Now.Millisecond, recvBad.EnsembleData.EnsDateTime.Millisecond, "Incorrece Ensemble time in Milliseconds " + recvBad.EnsembleData.EnsDateTime.ToString());

            // Check Ancillary data set available
            Assert.AreEqual(true, recvBad.IsAncillaryAvail, "Ancillary Data Set not available");

            // Check Tempearture
            Assert.AreEqual(14.64, recvBad.AncillaryData.WaterTemp, 0.00001, "Water Temperature was incorrect");

            Assert.AreEqual(0, recvBad.AncillaryData.FirstBinRange, "First Bin Range was incorrect");
            Assert.AreEqual(0, recvBad.AncillaryData.BinSize, "Bin Size was incorrect");
            Assert.AreEqual(0, recvBad.AncillaryData.FirstPingTime, "First Ping Time was incorrect");
            Assert.AreEqual(10, recvBad.AncillaryData.LastPingTime, "Last Ping Time was incorrect");
            Assert.AreEqual(0, recvBad.AncillaryData.Heading, "Heading was incorrect");
            Assert.AreEqual(0, recvBad.AncillaryData.Pitch, "Pitch was incorrect");
            Assert.AreEqual(0, recvBad.AncillaryData.Roll, "Roll was incorrect");
            Assert.AreEqual(0, recvBad.AncillaryData.SystemTemp, "System Temp was incorrect");
            Assert.AreEqual(0, recvBad.AncillaryData.Salinity, "Salinity was incorrect");
            Assert.AreEqual(0, recvBad.AncillaryData.Pressure, "Pressure was incorrect");
            Assert.AreEqual(0, recvBad.AncillaryData.TransducerDepth, "Transducer Depth was incorrect");
            Assert.AreEqual(0, recvBad.AncillaryData.SpeedOfSound, "Speed Of Sound was incorrect");

            Assert.AreEqual(true, recvBad.IsBottomTrackAvail, "Bottom Track DataSet not added");

            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvBad.BottomTrackData.Range[0], 0.00001, "Bottom Track Range B1 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvBad.BottomTrackData.Range[1], 0.00001, "Bottom Track Range B2 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvBad.BottomTrackData.Range[2], 0.00001, "Bottom Track Range B3 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvBad.BottomTrackData.Range[3], 0.00001, "Bottom Track Range B4 incorrect");

            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, recvBad.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, recvBad.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, recvBad.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");
            Assert.AreEqual(0, recvBad.BottomTrackData.InstrumentVelocity[3], "Bottom Track Instrument Velocity Q incorrect");

            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-1205 mm/s").ToMetersPerSecond().Value), recvBad.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-24 mm/s").ToMetersPerSecond().Value), recvBad.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-347 mm/s").ToMetersPerSecond().Value), recvBad.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");

            Assert.AreEqual(0000, recvBad.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
        }

        /// <summary>
        /// Receive the data from the event handler and set
        /// the value.
        /// Then unsubscribe.
        /// </summary>
        /// <param name="binaryData">Binary Data.</param>
        /// <param name="adcpData">Data from event handler.</param>
        public void ReceiveMsgBad(byte[] binaryData, DataSet.Ensemble adcpData)
        {
            recvBad = adcpData;
            _eventWaitResponse.Set();
        }

        private DataSet.Ensemble recvMaxBuffSize;
        /// <summary>
        /// Send Bad data then good data and
        /// ensure the data can be processed.
        /// </summary>
        [Test]
        public void TestMaxBufferSize()
        {
            string badNmea = "This is a a bad nmea string.";
            string goodNmea = "$PRTI01,1000,10,1464,-1205,-24,-347,79380,,,,,0000*31";
            recvData1 = null;

            AdcpDvlCodec codec = new AdcpDvlCodec();
            codec.ProcessDataEvent += new AdcpDvlCodec.ProcessDataEventHandler(ReceiveMsgMaxBuffSize);

            // Send a lot of bad data and ensure the buffer does not get to large
            // Should debug with breakpoints to verify.
            // Compile code in debug mode
            // Attach NUnit-agent-x86.exe
            // Then run the test and breakpoints will be hit
            // Breakpoint when data is sent to event and verify the buffer is empty
            for (int x = 0; x < 500; x++)
            {
                codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(badNmea));
            }
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(goodNmea));
            codec.Dispose();                                                           // Clear the remaining data

            _eventWaitResponse.WaitOne(TIMEOUT);

            Assert.IsNotNull(recvMaxBuffSize, "Message was not received.");

            // Check if Ensemble data set available
            Assert.AreEqual(true, recvMaxBuffSize.IsEnsembleAvail, "Ensemble Data Set not available");
            Assert.AreEqual(10, recvMaxBuffSize.EnsembleData.EnsembleNumber, "Ensemble number is incorrect");

            // Check number of beams
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, recvMaxBuffSize.EnsembleData.NumBeams, "Number of beams is Incorrect");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, recvMaxBuffSize.EnsembleData.ElementsMultiplier, "Element Multiplier is Incorrect");

            // Check number of bins
            Assert.AreEqual(0, recvMaxBuffSize.EnsembleData.NumBins);

            // Check time
            Assert.AreEqual(DateTime.Now.Hour, recvBad.EnsembleData.EnsDateTime.Hour, "Incorrect Ensemble time in Hours " + recvBad.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Minute, recvBad.EnsembleData.EnsDateTime.Minute, "Incorrect Ensemble time in Minutes " + recvBad.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Second, recvBad.EnsembleData.EnsDateTime.Second, "Incorrect Ensemble time in Seconds " + recvBad.EnsembleData.EnsDateTime.ToString());
            //Assert.AreEqual(DateTime.Now.Millisecond, recvBad.EnsembleData.EnsDateTime.Millisecond, "Incorrece Ensemble time in Milliseconds " + recvBad.EnsembleData.EnsDateTime.ToString());

            // Check Ancillary data set available
            Assert.AreEqual(true, recvMaxBuffSize.IsAncillaryAvail, "Ancillary Data Set not available");

            // Check Tempearture
            Assert.AreEqual(14.64, recvMaxBuffSize.AncillaryData.WaterTemp, 0.00001, "Water Temperature was incorrect");

            Assert.AreEqual(0, recvMaxBuffSize.AncillaryData.FirstBinRange, "First Bin Range was incorrect");
            Assert.AreEqual(0, recvMaxBuffSize.AncillaryData.BinSize, "Bin Size was incorrect");
            Assert.AreEqual(0, recvMaxBuffSize.AncillaryData.FirstPingTime, "First Ping Time was incorrect");
            Assert.AreEqual(10, recvMaxBuffSize.AncillaryData.LastPingTime, "Last Ping Time was incorrect");
            Assert.AreEqual(0, recvMaxBuffSize.AncillaryData.Heading, "Heading was incorrect");
            Assert.AreEqual(0, recvMaxBuffSize.AncillaryData.Pitch, "Pitch was incorrect");
            Assert.AreEqual(0, recvMaxBuffSize.AncillaryData.Roll, "Roll was incorrect");
            Assert.AreEqual(0, recvMaxBuffSize.AncillaryData.SystemTemp, "System Temp was incorrect");
            Assert.AreEqual(0, recvMaxBuffSize.AncillaryData.Salinity, "Salinity was incorrect");
            Assert.AreEqual(0, recvMaxBuffSize.AncillaryData.Pressure, "Pressure was incorrect");
            Assert.AreEqual(0, recvMaxBuffSize.AncillaryData.TransducerDepth, "Transducer Depth was incorrect");
            Assert.AreEqual(0, recvMaxBuffSize.AncillaryData.SpeedOfSound, "Speed Of Sound was incorrect");

            Assert.AreEqual(true, recvMaxBuffSize.IsBottomTrackAvail, "Bottom Track DataSet not added");

            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvMaxBuffSize.BottomTrackData.Range[0], 0.00001, "Bottom Track Range B1 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvMaxBuffSize.BottomTrackData.Range[1], 0.00001, "Bottom Track Range B2 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvMaxBuffSize.BottomTrackData.Range[2], 0.00001, "Bottom Track Range B3 incorrect");
            Assert.AreEqual(new DotSpatial.Positioning.Distance("79380 mm").ToMeters().Value, recvMaxBuffSize.BottomTrackData.Range[3], 0.00001, "Bottom Track Range B4 incorrect");

            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, recvMaxBuffSize.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, recvMaxBuffSize.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreNotEqual(DataSet.Ensemble.BAD_VELOCITY, recvMaxBuffSize.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");
            Assert.AreEqual(0, recvMaxBuffSize.BottomTrackData.InstrumentVelocity[3], "Bottom Track Instrument Velocity Q incorrect");

            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-1205 mm/s").ToMetersPerSecond().Value), recvMaxBuffSize.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-24 mm/s").ToMetersPerSecond().Value), recvMaxBuffSize.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-347 mm/s").ToMetersPerSecond().Value), recvMaxBuffSize.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");

            Assert.AreEqual(0000, recvMaxBuffSize.BottomTrackData.Status.Value, "Bottom Track Instrument Status incorrect");
        }

        /// <summary>
        /// Receive the data from the event handler and set
        /// the value.
        /// Then unsubscribe.
        /// </summary>
        /// <param name="binaryData">Binary Data.</param>
        /// <param name="adcpData">Data from event handler.</param>
        public void ReceiveMsgMaxBuffSize(byte[] binaryData, DataSet.Ensemble adcpData)
        {
            recvMaxBuffSize = adcpData;
            _eventWaitResponse.Set();
        }


        private DataSet.Ensemble recvBadNmea;
        /// <summary>
        /// Send a lot of bad data and then a good message.
        /// Verify the good message was parsed and passed to CurrentDataSetManager.
        /// </summary>
        [Test]
        public void TestBadNmea()
        {
            string badNmea = "CBTTBP 0.5\r\nCWTON 0\r\nCWTBB 1\r\nCWTBL 8\r\nCWTBS 8\r\nCWTTBP 1\r\nCWS 0\r\nCWT 15\r\nCTD 0\r\nCWSS 1500\r\nCHS 1\r\nCHO 0";
            string badNmea1 = "\r\nC232B 19200\r\nC485B 115200\r\nSTART\r\n$PRTI01,379550,1,1468,-99999,-99999,-99999,0,,,,,0004*09";

            recvBadNmea = null;

            AdcpDvlCodec codec = new AdcpDvlCodec();
            codec.ProcessDataEvent += new AdcpDvlCodec.ProcessDataEventHandler(ReceiveBadNmea);
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(badNmea));
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(badNmea1));
            codec.Dispose();                                                           // Clear the remaining data

            _eventWaitResponse.WaitOne(TIMEOUT);

            Assert.IsNotNull(recvBadNmea, "Message was not received.");

            // Check if Ensemble data set available
            Assert.AreEqual(true, recvBadNmea.IsEnsembleAvail, "Ensemble Data Set not available");
            Assert.AreEqual(1, recvBadNmea.EnsembleData.EnsembleNumber, "Ensemble number is incorrect");

        }

        /// <summary>
        /// Receive the data from the event handler and set
        /// the value.
        /// Then unsubscribe.
        /// </summary>
        /// <param name="binaryData">Binary Data.</param>
        /// <param name="adcpData">Data from event handler.</param>
        public void ReceiveBadNmea(byte[] binaryData, DataSet.Ensemble adcpData)
        {
            recvBadNmea = adcpData;
            _eventWaitResponse.Set();
        }


        private DataSet.Ensemble recvBadNmeaCombine;
        /// <summary>
        /// Combine Prti01 and Prti02 sentence and ensure
        /// the data is combined into a Bottom Track data set.
        /// </summary>
        [Test]
        public void TestBadNmeaCombine()
        {
            string nmea1 = "$PRTI01,379550,1,1468,-99999,-99999,-99999,0,,,,,0004*09\r\n$PRTI02,379550,1,1468,-99999,-99999,-99999,0,,,,,0004*0A";

            recvBadNmeaCombine = null;

            AdcpDvlCodec codec = new AdcpDvlCodec();
            codec.ProcessDataEvent += new AdcpDvlCodec.ProcessDataEventHandler(ReceiveBadNmeaCombine);
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea1));
            codec.Dispose();

            _eventWaitResponse.WaitOne(TIMEOUT);

            Assert.IsNotNull(recvBadNmeaCombine, "Combined message was not received.");

            // Check if Ensemble data set available
            Assert.AreEqual(true, recvBadNmeaCombine.IsEnsembleAvail, "Ensemble Data Set not available");
            Assert.AreEqual(1, recvBadNmeaCombine.EnsembleData.EnsembleNumber, "Ensemble number is incorrect");

            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvBadNmeaCombine.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvBadNmeaCombine.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvBadNmeaCombine.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvBadNmeaCombine.BottomTrackData.EarthVelocity[0], "Bottom Track Earth Velocity East incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvBadNmeaCombine.BottomTrackData.EarthVelocity[1], "Bottom Track Earth Velocity North incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvBadNmeaCombine.BottomTrackData.EarthVelocity[2], "Bottom Track Earth Velocity Up incorrect");
        }

        /// <summary>
        /// Receive the data from the event handler and set
        /// the value.
        /// Then unsubscribe.
        /// </summary>
        /// <param name="binaryData">Binary Data.</param>
        /// <param name="adcpData">Data from event handler.</param>
        public void ReceiveBadNmeaCombine(byte[] binaryData, DataSet.Ensemble adcpData)
        {
            recvBadNmeaCombine = adcpData;
            _eventWaitResponse.Set();
        }

        private DataSet.Ensemble recvBadNmeaCombineSeperate;
        /// <summary>
        /// Combine Prti01 and Prti02 sentence and ensure
        /// the data is combined into a Bottom Track data set.
        /// </summary>
        [Test]
        public void TestBadNmeaCombineSeperate()
        {
            string nmea1 = "$PRTI01,379550,1,1468,-99999,-99999,-99999,0,,,,,0004*09";
            string nmea2 = "$PRTI02,379550,1,1468,-99999,-99999,-99999,0,,,,,0004*0A";

            recvBadNmeaCombineSeperate = null;

            AdcpDvlCodec codec = new AdcpDvlCodec();
            codec.ProcessDataEvent += new AdcpDvlCodec.ProcessDataEventHandler(ReceiveBadNmeaCombineSeperate);
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea1));
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea2));
            codec.Dispose();                                                       // Clear remaining data

            _eventWaitResponse.WaitOne(TIMEOUT);

            Assert.IsNotNull(recvBadNmeaCombineSeperate, "CombinedSeperated message was not received.");

            // Check if Ensemble data set available
            Assert.AreEqual(true, recvBadNmeaCombineSeperate.IsEnsembleAvail, "Ensemble Data Set not available");
            Assert.AreEqual(1, recvBadNmeaCombineSeperate.EnsembleData.EnsembleNumber, "Ensemble number is incorrect");

            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvBadNmeaCombineSeperate.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvBadNmeaCombineSeperate.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvBadNmeaCombineSeperate.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvBadNmeaCombineSeperate.BottomTrackData.EarthVelocity[0], "Bottom Track Earth Velocity East incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvBadNmeaCombineSeperate.BottomTrackData.EarthVelocity[1], "Bottom Track Earth Velocity North incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvBadNmeaCombineSeperate.BottomTrackData.EarthVelocity[2], "Bottom Track Earth Velocity Up incorrect");
        }

        /// <summary>
        /// Receive the data from the event handler and set
        /// the value.
        /// Then unsubscribe.
        /// </summary>
        /// <param name="binaryData">Binary Data.</param>
        /// <param name="adcpData">Data from event handler.</param>
        public void ReceiveBadNmeaCombineSeperate(byte[] binaryData, DataSet.Ensemble adcpData)
        {
            recvBadNmeaCombineSeperate = adcpData;
            _eventWaitResponse.Set();
        }

        private DataSet.Ensemble recvNmeaCombineSeperate;
        /// <summary>
        /// Combine Prti01 and Prti02 sentence and ensure
        /// the data is combined into a Bottom Track data set.
        /// </summary>
        [Test]
        public void TestNmeaCombineSeperate()
        {
            string nmea1 = "$PRTI01,380250,8,1464,-1205,-24,-347,79380,,,,,0000*05";
            string nmea2 = "$PRTI02,380250,8,1464,1142,323,407,79380,,,,,0000*1C";

            recvNmeaCombineSeperate = null;

            AdcpDvlCodec codec = new AdcpDvlCodec();
            codec.ProcessDataEvent += new AdcpDvlCodec.ProcessDataEventHandler(ReceiveNmeaCombineSeperate);
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea1));
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea2));
            codec.Dispose();                                                       // Clear remaining data

            _eventWaitResponse.WaitOne(TIMEOUT);

            Assert.IsNotNull(recvNmeaCombineSeperate, "CombinedSeperated message was not received.");

            // Check if Ensemble data set available
            Assert.AreEqual(true, recvNmeaCombineSeperate.IsEnsembleAvail, "Ensemble Data Set not available");
            Assert.AreEqual(8, recvNmeaCombineSeperate.EnsembleData.EnsembleNumber, "Ensemble number is incorrect");

            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-1205 mm/s").ToMetersPerSecond().Value), recvNmeaCombineSeperate.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-24 mm/s").ToMetersPerSecond().Value), recvNmeaCombineSeperate.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-347 mm/s").ToMetersPerSecond().Value), recvNmeaCombineSeperate.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("1142 mm/s").ToMetersPerSecond().Value), recvNmeaCombineSeperate.BottomTrackData.EarthVelocity[0], "Bottom Track Earth Velocity East incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("323 mm/s").ToMetersPerSecond().Value), recvNmeaCombineSeperate.BottomTrackData.EarthVelocity[1], "Bottom Track Earth Velocity North incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("407 mm/s").ToMetersPerSecond().Value), recvNmeaCombineSeperate.BottomTrackData.EarthVelocity[2], "Bottom Track Earth Velocity Up incorrect");
        }

        /// <summary>
        /// Receive the data from the event handler and set
        /// the value.
        /// Then unsubscribe.
        /// </summary>
        /// <param name="binaryData">Binary Data.</param>
        /// <param name="adcpData">Data from event handler.</param>
        public void ReceiveNmeaCombineSeperate(byte[] binaryData, DataSet.Ensemble adcpData)
        {
            recvNmeaCombineSeperate = adcpData;
            _eventWaitResponse.Set();
        }

        private DataSet.Ensemble recvNmeaMultiple;
        /// <summary>
        /// Combine Prti01 and Prti02 sentence and ensure
        /// the data is combined into a Bottom Track data set.
        /// </summary>
        [Test]
        public void TestNmeaMultiple()
        {
            string nmea0 = "$PRTI01,379550,1,1468,-99999,-99999,-99999,0,,,,,0004*09";
            string nmea00 = "$PRTI02,379550,1,1468,-99999,-99999,-99999,0,,,,,0004*0A";
            string nmea1 = "$PRTI01,380250,8,1464,-1205,-24,-347,79380,,,,,0000*05";
            string nmea2 = "$PRTI02,380250,8,1464,1142,323,407,79380,,,,,0000*1C";

            recvNmeaMultiple = null;

            AdcpDvlCodec codec = new AdcpDvlCodec();
            codec.ProcessDataEvent += new AdcpDvlCodec.ProcessDataEventHandler(ReceiveNmeaMultiple);
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea0));
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea00));
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea1));
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea2));
            codec.Dispose();                                                       // Clear remaining data

            _eventWaitResponse.WaitOne(TIMEOUT);

            Assert.IsNotNull(recvNmeaCombineSeperate, "CombinedSeperated message was not received.");

            // Check if Ensemble data set available
            Assert.AreEqual(true, recvNmeaCombineSeperate.IsEnsembleAvail, "Ensemble Data Set not available");
            Assert.AreEqual(8, recvNmeaCombineSeperate.EnsembleData.EnsembleNumber, "Ensemble number is incorrect");

            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-1205 mm/s").ToMetersPerSecond().Value), recvNmeaMultiple.BottomTrackData.InstrumentVelocity[0], "Bottom Track Instrument Velocity X incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-24 mm/s").ToMetersPerSecond().Value), recvNmeaMultiple.BottomTrackData.InstrumentVelocity[1], "Bottom Track Instrument Velocity Y incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("-347 mm/s").ToMetersPerSecond().Value), recvNmeaMultiple.BottomTrackData.InstrumentVelocity[2], "Bottom Track Instrument Velocity Z incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("1142 mm/s").ToMetersPerSecond().Value), recvNmeaMultiple.BottomTrackData.EarthVelocity[0], "Bottom Track Earth Velocity East incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("323 mm/s").ToMetersPerSecond().Value), recvNmeaMultiple.BottomTrackData.EarthVelocity[1], "Bottom Track Earth Velocity North incorrect");
            Assert.AreEqual(Convert.ToSingle(new DotSpatial.Positioning.Speed("407 mm/s").ToMetersPerSecond().Value), recvNmeaMultiple.BottomTrackData.EarthVelocity[2], "Bottom Track Earth Velocity Up incorrect");
        }

        /// <summary>
        /// Receive the data from the event handler and set
        /// the value.
        /// Then unsubscribe.
        /// </summary>
        /// <param name="binaryData">Binary Data.</param>
        /// <param name="adcpData">Data from event handler.</param>
        public void ReceiveNmeaMultiple(byte[] binaryData, DataSet.Ensemble adcpData)
        {
            recvNmeaMultiple = adcpData;
            _eventWaitResponse.Set();
        }


        private DataSet.Ensemble recvNmeaCombine;
        /// <summary>
        /// Create a bad PRTI01 (missing checksum byte) and a good
        /// PRTI02 sentence.  Verify PRTI02 was parsed correctly.
        /// </summary>
        [Test]
        public void TestNmeaCombine()
        {
            string nmea1 = "$PRTI01,379550,1,1468,-99999,-99999,-99999,0,,,,,0004*0$PRTI02,379550,1,1468,-99999,-99999,-99999,0,,,,,0004*0A";

            recvNmeaCombine = null;

            AdcpDvlCodec codec = new AdcpDvlCodec();
            codec.ProcessDataEvent += new AdcpDvlCodec.ProcessDataEventHandler(ReceiveNmeaCombine);
            codec.AddIncomingData(System.Text.Encoding.ASCII.GetBytes(nmea1));
            codec.Dispose();

            _eventWaitResponse.WaitOne(TIMEOUT);

            Assert.IsNotNull(recvNmeaCombine, "Combined message was not received.");

            // Check if Ensemble data set available
            Assert.AreEqual(true, recvNmeaCombine.IsEnsembleAvail, "Ensemble Data Set not available");
            Assert.AreEqual(1, recvNmeaCombine.EnsembleData.EnsembleNumber, "Ensemble number is incorrect");
            Assert.IsFalse(recvNmeaCombine.IsInstrumentVelocityAvail, "Instrument Velocity should not be included");

            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvNmeaCombine.BottomTrackData.EarthVelocity[0], "Bottom Track Earth Velocity East incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvNmeaCombine.BottomTrackData.EarthVelocity[1], "Bottom Track Earth Velocity North incorrect");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, recvNmeaCombine.BottomTrackData.EarthVelocity[2], "Bottom Track Earth Velocity Up incorrect");
        }

        /// <summary>
        /// Receive the data from the event handler and set
        /// the value.
        /// Then unsubscribe.
        /// </summary>
        /// <param name="binaryData">Binary Data.</param>
        /// <param name="adcpData">Data from event handler.</param>
        public void ReceiveNmeaCombine(byte[] binaryData, DataSet.Ensemble adcpData)
        {
            recvNmeaCombine = adcpData;
            _eventWaitResponse.Set();
        }
    }

}