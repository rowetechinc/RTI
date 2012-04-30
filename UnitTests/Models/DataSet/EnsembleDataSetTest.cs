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
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Unit test of the Ensemble DataSet object.
    /// </summary>
    [TestFixture]
    public class EnsembleDataSetTest
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

            // Add Sentence to data set
            adcpData.AddEnsembleData(sent);

            // Check Ensemble number
            Assert.AreEqual(1, adcpData.EnsembleData.EnsembleNumber);

            // Check number of beams
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.EnsembleData.NumBeams, "Number of beams is Incorrect");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, adcpData.EnsembleData.ElementsMultiplier, "Number of beams is Incorrect");

            // Check number of bins
            Assert.AreEqual(0, adcpData.EnsembleData.NumBins);

            // Check time
            Assert.AreEqual(379550, sent.StartTime, "NMEA start time incorrect");
            Assert.AreEqual(1, adcpData.EnsembleData.EnsDateTime.Hour, "Incorrect Ensemble time in Hours " + adcpData.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(3, adcpData.EnsembleData.EnsDateTime.Minute, "Incorrect Ensemble time in Minutes " + adcpData.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(15, adcpData.EnsembleData.EnsDateTime.Second, "Incorrect Ensemble time in Seconds " + adcpData.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(500, adcpData.EnsembleData.EnsDateTime.Millisecond, "Incorrect Ensemble time in Milliseconds " + adcpData.EnsembleData.EnsDateTime.ToString());
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

            // Add Sentence to data set
            adcpData.AddEnsembleData(sent);

            // Check Ensemble number
            Assert.AreEqual(1, adcpData.EnsembleData.EnsembleNumber);

            // Check number of beams
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.EnsembleData.NumBeams, "Number of beams is Incorrect");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, adcpData.EnsembleData.ElementsMultiplier, "Number of beams is Incorrect");

            // Check number of bins
            Assert.AreEqual(0, adcpData.EnsembleData.NumBins);

            // Check time
            Assert.AreEqual(379550, sent.StartTime, "NMEA start time incorrect");
            Assert.AreEqual(1, adcpData.EnsembleData.EnsDateTime.Hour, "Incorrect Ensemble time in Hours " + adcpData.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(3, adcpData.EnsembleData.EnsDateTime.Minute, "Incorrect Ensemble time in Minutes " + adcpData.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(15, adcpData.EnsembleData.EnsDateTime.Second, "Incorrect Ensemble time in Seconds " + adcpData.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(500, adcpData.EnsembleData.EnsDateTime.Millisecond, "Incorrect Ensemble time in Milliseconds " + adcpData.EnsembleData.EnsDateTime.ToString());
        }

                /// <summary>
        /// Test the constructor that takes a Prti02Sentence.
        /// </summary>
        [Test]
        public void TestSerialNumber()
        {
            string nmea = "$PRTI02,379550,1,1468,-99999,-99999,-99999,0,,,,,0004*0A";

            // Create Sentence
            Prti02Sentence sent = new Prti02Sentence(nmea);

            Assert.AreEqual(true, sent.IsValid, "NMEA sentence incorrect");

            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Sentence to data set
            adcpData.AddEnsembleData(sent);

            // Create a serial number
            string serialStr = "01300000000000000000000000000001";
            SerialNumber serialNum = new SerialNumber(serialStr);

            // Set the serial number
            adcpData.EnsembleData.SysSerialNumber = serialNum;

            // Test the serial number
            Assert.AreEqual(serialNum, adcpData.EnsembleData.SysSerialNumber, string.Format("Serial numbers did not match"));
            Assert.AreEqual(serialStr, adcpData.EnsembleData.SysSerialNumber.ToString(), string.Format("Serial number strings did not match {0}  {1}", serialStr, adcpData.EnsembleData.SysSerialNumber.ToString()));
            Assert.AreEqual(1, adcpData.EnsembleData.SysSerialNumber.SubSystemsDict.Count, string.Format("Number of SubSystems did not match 1 {0}", adcpData.EnsembleData.SysSerialNumber.SubSystemsDict.Count));
        }

    }

}