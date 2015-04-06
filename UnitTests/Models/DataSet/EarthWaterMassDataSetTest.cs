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
    /// Unit test of the Earth Water Mass DataSet object.
    /// </summary>
    [TestFixture]
    public class EarthWaterMassDataSetTest
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

            adcpData.AddEarthWaterMassData(DataSet.Ensemble.DATATYPE_FLOAT, DataSet.EarthWaterMassDataSet.NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.WaterMassInstrumentID,
                0.3f, 
                -0.2f, 
                0.4f, 
                0.1f);

            Assert.IsTrue(adcpData.IsEarthWaterMassAvail, "Earth Water Mass not added to dataset");

            Assert.AreEqual(adcpData.EarthWaterMassData.VelocityEast, (new DotSpatial.Positioning.Speed(300, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, 0.00001, "Earth Water Mass East not properly set.");
            Assert.AreEqual(adcpData.EarthWaterMassData.VelocityNorth, (new DotSpatial.Positioning.Speed(-200, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, 0.00001, "Earth Water Mass North not properly set.");
            Assert.AreEqual(adcpData.EarthWaterMassData.VelocityVertical, (new DotSpatial.Positioning.Speed(400, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, 0.00001, "Earth Water Mass Vertical not properly set.");
            Assert.AreEqual(adcpData.EarthWaterMassData.WaterMassDepthLayer, (new DotSpatial.Positioning.Distance(100, DotSpatial.Positioning.DistanceUnit.Millimeters)).ToMeters().Value, 0.00001, "Earth Water Mass Depth Layer not properly set.");
        }

        /// <summary>
        /// Test the constructor that takes a Prti02Sentence with no data.
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

            adcpData.AddEarthWaterMassData(sent);

            Assert.IsTrue(adcpData.IsEarthWaterMassAvail, "Earth Water Mass not added to dataset");

            Assert.AreEqual(adcpData.EarthWaterMassData.VelocityEast, DataSet.Ensemble.EMPTY_VELOCITY, "Earth Water Mass East not properly set.");
            Assert.AreEqual(adcpData.EarthWaterMassData.VelocityNorth, DataSet.Ensemble.EMPTY_VELOCITY, "Earth Water Mass North not properly set.");
            Assert.AreEqual(adcpData.EarthWaterMassData.VelocityVertical, DataSet.Ensemble.EMPTY_VELOCITY, "Earth Water Mass Vertical not properly set.");
            Assert.AreEqual(adcpData.EarthWaterMassData.WaterMassDepthLayer, 0, "Earth Water Mass Depth Layer not properly set.");
        }

        /// <summary>
        /// Test the constructor that takes a Prti02Sentence with data.
        /// </summary>
        [Test]
        public void TestConstructorPrti02SentenceData()
        {
            string nmea = "$PRTI02,379550,1,1468,-99999,-99999,-99999,0,300,-200,400,100,0004*23";

            // Create Sentence
            Prti02Sentence sent = new Prti02Sentence(nmea);

            Assert.AreEqual(true, sent.IsValid, "NMEA sentence incorrect");

            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            adcpData.AddEarthWaterMassData(sent);

            Assert.IsTrue(adcpData.IsEarthWaterMassAvail, "Earth Water Mass not added to dataset");

            Assert.AreEqual((new DotSpatial.Positioning.Speed(300, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, adcpData.EarthWaterMassData.VelocityEast, 0.00001, "Earth Water Mass East not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Speed(-200, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, adcpData.EarthWaterMassData.VelocityNorth, 0.00001, "Earth Water Mass North not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Speed(400, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, adcpData.EarthWaterMassData.VelocityVertical, 0.00001, "Earth Water Mass Vertical not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Distance(100, DotSpatial.Positioning.DistanceUnit.Millimeters)).ToMeters().Value, adcpData.EarthWaterMassData.WaterMassDepthLayer, 0.00001, "Earth Water Mass Depth Layer not properly set.");
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
            ensemble.EarthWaterMassData.VelocityEast = 2.3f;
            ensemble.EarthWaterMassData.VelocityNorth = 3.4f;
            ensemble.EarthWaterMassData.VelocityVertical = 4.5f;
            ensemble.EarthWaterMassData.WaterMassDepthLayer = 3.77f;

            string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthWaterMassData);                                      // Serialize object to JSON
            DataSet.EarthWaterMassDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.EarthWaterMassDataSet>(encoded);       // Deserialize the JSON

            // Verify the values are the same
            Assert.AreEqual(2.3f, decoded.VelocityEast, "Velocity East is incorrect.");
            Assert.AreEqual(3.4f, decoded.VelocityNorth, "Velocity North is incorrect.");
            Assert.AreEqual(4.5f, decoded.VelocityVertical, "Velocity Vertical is incorrect.");
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
                string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthWaterMassData);
            }
            watch.Stop();
            long resultSerialize = watch.ElapsedMilliseconds;

            // Test Deserialize()
            string encodedd = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthWaterMassData);
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                DataSet.EarthWaterMassDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.EarthWaterMassDataSet>(encodedd);
            }
            watch.Stop();
            long resultDeserialize = watch.ElapsedMilliseconds;

            Debug.WriteLine(String.Format("Serialize:{0}  Deserialize:{1}", resultSerialize, resultDeserialize));

            Debug.WriteLine("Complete");

        }
    }
}