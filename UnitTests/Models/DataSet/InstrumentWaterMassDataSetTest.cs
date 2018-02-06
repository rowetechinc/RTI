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
 * 12/08/2011      RC          1.09       Initial coding.
 * 02/25/2012      RC          2.18       Added JSON test.
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Diagnostics;

    /// <summary>
    /// Unit test of the Instrument Water Mass DataSet object.
    /// </summary>
    [TestFixture]
    public class InstrumentWaterMassDataSetTest
    {
        /// <summary>
        /// Test the constructor that takes the East,
        /// North and Vertical data.
        /// </summary>
        [Test]
        public void TestConstructorData()
        {
            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            adcpData.AddInstrumentWaterMassData(DataSet.Ensemble.DATATYPE_FLOAT, DataSet.EarthWaterMassDataSet.NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.WaterMassInstrumentID,
                0.3f,
                -0.2f,
                0.4f,
                0.2f,
                0.1f);

            Assert.IsTrue(adcpData.IsInstrumentWaterMassAvail, "Instrument Water Mass not added to dataset");

            Assert.AreEqual(adcpData.InstrumentWaterMassData.VelocityX, (new DotSpatial.Positioning.Speed(300, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, 0.00001, "Instrument Water Mass East not properly set.");
            Assert.AreEqual(adcpData.InstrumentWaterMassData.VelocityY, (new DotSpatial.Positioning.Speed(-200, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, 0.00001, "Instrument Water Mass North not properly set.");
            Assert.AreEqual(adcpData.InstrumentWaterMassData.VelocityZ, (new DotSpatial.Positioning.Speed(400, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, 0.00001, "Instrument Water Mass Vertical not properly set.");
            Assert.AreEqual(adcpData.InstrumentWaterMassData.VelocityQ, (new DotSpatial.Positioning.Speed(200, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, 0.00001, "Instrument Water Mass Error not properly set.");
            Assert.AreEqual(adcpData.InstrumentWaterMassData.WaterMassDepthLayer, (new DotSpatial.Positioning.Distance(100, DotSpatial.Positioning.DistanceUnit.Millimeters)).ToMeters().Value, 0.00001, "Instrument Water Mass Depth Layer not properly set.");
        }

        /// <summary>
        /// Test the constructor that takes a Prti01Sentence with no data.
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

            adcpData.AddInstrumentWaterMassData(sent);

            Assert.IsTrue(adcpData.IsInstrumentWaterMassAvail, "Instrument Water Mass not added to dataset");

            Assert.AreEqual(adcpData.InstrumentWaterMassData.VelocityX, DataSet.Ensemble.EMPTY_VELOCITY, "Instrument Water Mass East not properly set.");
            Assert.AreEqual(adcpData.InstrumentWaterMassData.VelocityY, DataSet.Ensemble.EMPTY_VELOCITY, "Instrument Water Mass North not properly set.");
            Assert.AreEqual(adcpData.InstrumentWaterMassData.VelocityZ, DataSet.Ensemble.EMPTY_VELOCITY, "Instrument Water Mass Vertical not properly set.");
            Assert.AreEqual(adcpData.InstrumentWaterMassData.WaterMassDepthLayer, 0, "Instrument Water Mass Depth Layer not properly set.");
        }

        /// <summary>
        /// Test the constructor that takes a Prti01Sentence with data.
        /// </summary>
        [Test]
        public void TestConstructorPrti01entenceData()
        {
            string nmea = "$PRTI01,379550,1,1468,-99999,-99999,-99999,0,300,-200,400,100,0004*20";

            // Create Sentence
            Prti01Sentence sent = new Prti01Sentence(nmea);

            Assert.AreEqual(true, sent.IsValid, "NMEA sentence incorrect");

            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            adcpData.AddInstrumentWaterMassData(sent);

            Assert.IsTrue(adcpData.IsInstrumentWaterMassAvail, "Instrument Water Mass not added to dataset");

            Assert.AreEqual((new DotSpatial.Positioning.Speed(300, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, adcpData.InstrumentWaterMassData.VelocityX, 0.00001, "Instrument Water Mass East not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Speed(-200, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, adcpData.InstrumentWaterMassData.VelocityY, 0.00001, "Instrument Water Mass North not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Speed(400, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, adcpData.InstrumentWaterMassData.VelocityZ, 0.00001, "Instrument Water Mass Vertical not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Distance(100, DotSpatial.Positioning.DistanceUnit.Millimeters)).ToMeters().Value, adcpData.InstrumentWaterMassData.WaterMassDepthLayer, 0.00001, "Instrument Water Mass Depth Layer not properly set.");
        }

        /// <summary>
        /// Test encoding and decoding to JSON.
        /// </summary>
        [Test]
        public void TestJson()
        {
            // Generate an Ensemble
            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);

            // Modify the data
            ensemble.InstrumentWaterMassData.VelocityX = 2.3f;
            ensemble.InstrumentWaterMassData.VelocityY = 3.4f;
            ensemble.InstrumentWaterMassData.VelocityZ = 4.5f;
            ensemble.InstrumentWaterMassData.WaterMassDepthLayer = 3.77f;

            string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.InstrumentWaterMassData);                                      // Serialize object to JSON
            DataSet.InstrumentWaterMassDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.InstrumentWaterMassDataSet>(encoded);       // Deserialize the JSON

            // Verify the values are the same
            Assert.AreEqual(2.3f, decoded.VelocityX, "Velocity X is incorrect.");
            Assert.AreEqual(3.4f, decoded.VelocityY, "Velocity Y is incorrect.");
            Assert.AreEqual(4.5f, decoded.VelocityZ, "Velocity Z is incorrect.");
            Assert.AreEqual(3.77f, decoded.WaterMassDepthLayer, "Water Mass Depty Layer is incorrect.");
        }

        /// <summary>
        /// Testing the timing for the JSON conversions.
        /// Put breakstatements on all the time results.
        /// Then run the code and check the results.
        /// </summary>
        [Test]
        public void TestTiming()
        {

            // Generate an Ensemble
            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);

            Stopwatch watch = new Stopwatch();

            // Test Serialize()
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.InstrumentWaterMassData);
            }
            watch.Stop();
            long resultSerialize = watch.ElapsedMilliseconds;

            // Test Deserialize()
            string encodedd = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.InstrumentWaterMassData);
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                DataSet.InstrumentWaterMassDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.InstrumentWaterMassDataSet>(encodedd);
            }
            watch.Stop();
            long resultDeserialize = watch.ElapsedMilliseconds;

            Debug.WriteLine(String.Format("Serialize:{0}  Deserialize:{1}", resultSerialize, resultDeserialize));

            Debug.WriteLine("Complete");

        }
    }
}