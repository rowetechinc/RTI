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
 * Date            Initials   Version     Comments
 * -----------------------------------------------------------------
 * 11/28/2011      RC                     Initial coding
 * 11/29/2011      RC                     Added Prti02Sentence Constructor.
 * 02/25/2012      RC          2.18       Added JSON test.
 * 02/28/2013      RC          2.18       Fixed FirstPingTime for PRTI sentences.
 * 05/28/2014      RC          2.21.4     Updated PD0 decoding.
 * 
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Diagnostics;

    /// <summary>
    /// Unit test of the Ancillary DataSet object.
    /// </summary>
    [TestFixture]
    public class AncillaryDataSetTest
    {
        /// <summary>
        /// Test the constructor that takes a Prti01Sentence.
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

            // Add ancillary data
            adcpData.AddAncillaryData(sent);

            Assert.AreEqual(true, adcpData.IsAncillaryAvail, "Ancillary data not created");

            // Check temperature
            Assert.AreEqual(14.68, adcpData.AncillaryData.WaterTemp, 0.00001, "Water Temperature was incorrect");

            Assert.AreEqual(0, adcpData.AncillaryData.FirstBinRange, "First Bin Range was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.BinSize, "Bin Size was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.FirstPingTime, "First Ping Time was incorrect");
            Assert.AreEqual(3795, adcpData.AncillaryData.LastPingTime, "Last Ping Time was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.Heading, "Heading was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.Pitch, "Pitch was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.Roll, "Roll was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.SystemTemp, "System Temp was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.Salinity, "Salinity was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.Pressure, "Pressure was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.TransducerDepth, "Transducer Depth was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.SpeedOfSound, "Speed Of Sound was incorrect");
        }

        /// <summary>
        /// Test the constructor that takes a Prti02Sentence.
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

            // Add ancillary data
            adcpData.AddAncillaryData(sent);

            Assert.AreEqual(true, adcpData.IsAncillaryAvail, "Ancillary data not created");

            // Check temperature
            Assert.AreEqual(14.68, adcpData.AncillaryData.WaterTemp, 0.00001, "Water Temperature was incorrect");

            Assert.AreEqual(0, adcpData.AncillaryData.FirstBinRange, "First Bin Range was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.BinSize, "Bin Size was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.FirstPingTime, "First Ping Time was incorrect");
            Assert.AreEqual(3795, adcpData.AncillaryData.LastPingTime, "Last Ping Time was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.Heading, "Heading was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.Pitch, "Pitch was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.Roll, "Roll was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.SystemTemp, "System Temp was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.Salinity, "Salinity was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.Pressure, "Pressure was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.TransducerDepth, "Transducer Depth was incorrect");
            Assert.AreEqual(0, adcpData.AncillaryData.SpeedOfSound, "Speed Of Sound was incorrect");
        }


        /// <summary>
        /// Test the constructor that takes no data.
        /// </summary>
        [Test]
        public void TestEmptyConstructor()
        {
            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Sentence to data set
            adcpData.AddAncillaryData(DataSet.Ensemble.DATATYPE_FLOAT,                      // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.AncillaryID);                  // Dataset ID

            Assert.IsTrue(adcpData.IsAncillaryAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.AncillaryData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, adcpData.AncillaryData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(4, adcpData.AncillaryData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.AncillaryData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.AncillaryData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.AncillaryID, adcpData.AncillaryData.Name, "Name is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.FirstBinRange, "First Bin Range is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.BinSize, "BinSize is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.FirstPingTime, "FirstPingTime is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.LastPingTime, "LastPingTime is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.Heading, "Heading is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.Roll, "Roll is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.WaterTemp, "WaterTemp is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.SystemTemp, "SystemTemp is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.Salinity, "Salinity is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.TransducerDepth, "TransducerDepth is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.SpeedOfSound, "SpeedOfSound is incorrect.");

        }

        /// <summary>
        /// Test encode.
        /// </summary>
        [Test]
        public void TestEncode()
        {
            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add to data set
            adcpData.AddAncillaryData(DataSet.Ensemble.DATATYPE_FLOAT,                      // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.AncillaryID);                  // Dataset ID

            Assert.IsTrue(adcpData.IsAncillaryAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.AncillaryData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, adcpData.AncillaryData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(4, adcpData.AncillaryData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.AncillaryData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.AncillaryData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.AncillaryID, adcpData.AncillaryData.Name, "Name is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.FirstBinRange, "First Bin Range is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.BinSize, "BinSize is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.FirstPingTime, "FirstPingTime is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.LastPingTime, "LastPingTime is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.Heading, "Heading is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.Roll, "Roll is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.WaterTemp, "WaterTemp is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.SystemTemp, "SystemTemp is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.Salinity, "Salinity is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.TransducerDepth, "TransducerDepth is incorrect.");
            Assert.AreEqual(0, adcpData.AncillaryData.SpeedOfSound, "SpeedOfSound is incorrect.");

            adcpData.AncillaryData.FirstBinRange = 1.2f;
            adcpData.AncillaryData.BinSize = 2.3f;
            adcpData.AncillaryData.FirstPingTime = 3.4f;
            adcpData.AncillaryData.LastPingTime = 4.5f;
            adcpData.AncillaryData.Heading = 5.6f;
            adcpData.AncillaryData.Pitch = 6.7f;
            adcpData.AncillaryData.Roll = 7.8f;
            adcpData.AncillaryData.WaterTemp = 8.9f;
            adcpData.AncillaryData.SystemTemp = 9.10f;
            adcpData.AncillaryData.Salinity = 10.11f;
            adcpData.AncillaryData.Pressure = 11.12f;
            adcpData.AncillaryData.TransducerDepth = 12.13f;
            adcpData.AncillaryData.SpeedOfSound = 13.14f;

            Assert.AreEqual(1.2f, adcpData.AncillaryData.FirstBinRange, "First Bin Range is incorrect.");
            Assert.AreEqual(2.3f, adcpData.AncillaryData.BinSize, "BinSize is incorrect.");
            Assert.AreEqual(3.4f, adcpData.AncillaryData.FirstPingTime, "FirstPingTime is incorrect.");
            Assert.AreEqual(4.5f, adcpData.AncillaryData.LastPingTime, "LastPingTime is incorrect.");
            Assert.AreEqual(5.6f, adcpData.AncillaryData.Heading, "Heading is incorrect.");
            Assert.AreEqual(6.7f, adcpData.AncillaryData.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(7.8f, adcpData.AncillaryData.Roll, "Roll is incorrect.");
            Assert.AreEqual(8.9f, adcpData.AncillaryData.WaterTemp, "WaterTemp is incorrect.");
            Assert.AreEqual(9.10f, adcpData.AncillaryData.SystemTemp, "SystemTemp is incorrect.");
            Assert.AreEqual(10.11f, adcpData.AncillaryData.Salinity, "Salinity is incorrect.");
            Assert.AreEqual(11.12f, adcpData.AncillaryData.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(12.13f, adcpData.AncillaryData.TransducerDepth, "TransducerDepth is incorrect.");
            Assert.AreEqual(13.14f, adcpData.AncillaryData.SpeedOfSound, "SpeedOfSound is incorrect.");

            // Encode the data
            byte[] encoded = adcpData.AncillaryData.Encode();

            DataSet.Ensemble ens1 = new DataSet.Ensemble();
            // Add encoded data to dataset
            ens1.AddAncillaryData(DataSet.Ensemble.DATATYPE_FLOAT,                          // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.AncillaryID,                   // Dataset ID
                                            encoded);                                       // Data

            Assert.AreEqual(((DataSet.AncillaryDataSet.NUM_DATA_ELEMENTS * DataSet.Ensemble.BYTES_IN_FLOAT) + DataSet.Ensemble.PAYLOAD_HEADER_LEN), encoded.Length, "Encoded length is incorrect.");
            Assert.AreEqual(1.2f, ens1.AncillaryData.FirstBinRange, "First Bin Range is incorrect.");
            Assert.AreEqual(2.3f, ens1.AncillaryData.BinSize, "BinSize is incorrect.");
            Assert.AreEqual(3.4f, ens1.AncillaryData.FirstPingTime, "FirstPingTime is incorrect.");
            Assert.AreEqual(4.5f, ens1.AncillaryData.LastPingTime, "LastPingTime is incorrect.");
            Assert.AreEqual(5.6f, ens1.AncillaryData.Heading, "Heading is incorrect.");
            Assert.AreEqual(6.7f, ens1.AncillaryData.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(7.8f, ens1.AncillaryData.Roll, "Roll is incorrect.");
            Assert.AreEqual(8.9f, ens1.AncillaryData.WaterTemp, "WaterTemp is incorrect.");
            Assert.AreEqual(9.10f, ens1.AncillaryData.SystemTemp, "SystemTemp is incorrect.");
            Assert.AreEqual(10.11f, ens1.AncillaryData.Salinity, "Salinity is incorrect.");
            Assert.AreEqual(11.12f, ens1.AncillaryData.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(12.13f, ens1.AncillaryData.TransducerDepth, "TransducerDepth is incorrect.");
            Assert.AreEqual(13.14f, ens1.AncillaryData.SpeedOfSound, "SpeedOfSound is incorrect.");

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
            ensemble.AncillaryData.FirstBinRange = 1.2f;
            ensemble.AncillaryData.BinSize = 2.3f;
            ensemble.AncillaryData.FirstPingTime = 3.4f;
            ensemble.AncillaryData.LastPingTime = 4.5f;
            ensemble.AncillaryData.Heading = 5.6f;
            ensemble.AncillaryData.Pitch = 6.7f;
            ensemble.AncillaryData.Roll = 7.8f;
            ensemble.AncillaryData.WaterTemp = 8.9f;
            ensemble.AncillaryData.SystemTemp = 9.10f;
            ensemble.AncillaryData.Salinity = 10.11f;
            ensemble.AncillaryData.Pressure = 11.12f;
            ensemble.AncillaryData.TransducerDepth = 12.13f;
            ensemble.AncillaryData.SpeedOfSound = 13.14f;

            string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.AncillaryData);                                      // Serialize object to JSON
            DataSet.AncillaryDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.AncillaryDataSet>(encoded);       // Deserialize the JSON

            // Verify the values are the same
            Assert.AreEqual(1.2f, decoded.FirstBinRange, "First Bin Range is incorrect.");
            Assert.AreEqual(2.3f, decoded.BinSize, "BinSize is incorrect.");
            Assert.AreEqual(3.4f, decoded.FirstPingTime, "FirstPingTime is incorrect.");
            Assert.AreEqual(4.5f, decoded.LastPingTime, "LastPingTime is incorrect.");
            Assert.AreEqual(5.6f, decoded.Heading, "Heading is incorrect.");
            Assert.AreEqual(6.7f, decoded.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(7.8f, decoded.Roll, "Roll is incorrect.");
            Assert.AreEqual(8.9f, decoded.WaterTemp, "WaterTemp is incorrect.");
            Assert.AreEqual(9.10f, decoded.SystemTemp, "SystemTemp is incorrect.");
            Assert.AreEqual(10.11f, decoded.Salinity, "Salinity is incorrect.");
            Assert.AreEqual(11.12f, decoded.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(12.13f, decoded.TransducerDepth, "TransducerDepth is incorrect.");
            Assert.AreEqual(13.14f, decoded.SpeedOfSound, "SpeedOfSound is incorrect.");
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
                string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.AncillaryData);
            }
            watch.Stop();
            long resultSerialize = watch.ElapsedMilliseconds;

            // Test Deserialize()
            string encodedd = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.AncillaryData);
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                DataSet.AncillaryDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.AncillaryDataSet>(encodedd);
            }
            watch.Stop();
            long resultDeserialize = watch.ElapsedMilliseconds;

            Debug.WriteLine(String.Format("Serialize:{0}  Deserialize:{1}", resultSerialize, resultDeserialize));

            Debug.WriteLine("Complete");

        }

        #region PD0 Decode

        /// <summary>
        /// Test decoding PD0 Echo Intensity data to RTI Amplitude data.
        /// </summary>
        [Test]
        public void DecodePd0Test()
        {
            Pd0FixedLeader fl = new Pd0FixedLeader();
            Pd0VariableLeader vl = new Pd0VariableLeader();

            fl.DepthCellLength = 23 * 100;
            vl.Heading = 223.3f;
            vl.Pitch = 123.45f;
            vl.Roll = 445.69f;
            vl.Temperature = 78.9f;
            vl.Pressure = 11;
            vl.DepthOfTransducer = 23 * 10;

            DataSet.AncillaryDataSet anc = new DataSet.AncillaryDataSet();
            anc.DecodePd0Ensemble(fl, vl);

            Assert.AreEqual(23, anc.BinSize, "Bin size is incorrect.");
            Assert.AreEqual(223.3f, anc.Heading, "Heading is incorrect.");
            Assert.AreEqual(123.45f, anc.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(445.69f, anc.Roll, "Roll is incorrect.");
            Assert.AreEqual(78.9f, anc.WaterTemp, "Water Temp is incorrect.");
            Assert.AreEqual(110000, anc.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(23, anc.TransducerDepth, "Transducer Depth is incorrect.");
        }

        #endregion

    }

}