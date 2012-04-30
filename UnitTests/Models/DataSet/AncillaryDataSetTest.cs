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

    }

}