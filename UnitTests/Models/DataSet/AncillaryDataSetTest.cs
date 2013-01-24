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
 * 11/28/2011      RC          Initial coding
 * 11/29/2011      RC          Added Prti02Sentence Constructor.
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;

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
            Assert.AreEqual(0, adcpData.AncillaryData.LastPingTime, "Last Ping Time was incorrect");
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
            Assert.AreEqual(0, adcpData.AncillaryData.LastPingTime, "Last Ping Time was incorrect");
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
    }

}