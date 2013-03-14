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
 * 01/04/2013      RC          2.17       Added test for creating an empty dataset.
 *                                         Added a test to test for firmware version 2.13 or less.
 *                                         Added a test for encode and decode.
 * 02/20/2013      RC          2.18       Updated test with latest changes to setting the time for PRTI sentences.
 * 02/25/2012      RC          2.18       Added JSON test.
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Diagnostics;

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
            string nmea = "$PRTI01,1000,1,1468,-99999,-99999,-99999,0,,,,,0004*05";

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
            Assert.AreEqual(1000, sent.StartTime, "NMEA start time incorrect");
            Assert.AreEqual(DateTime.Now.Hour, adcpData.EnsembleData.EnsDateTime.Hour, "Incorrect Ensemble time in Hours " + adcpData.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Minute, adcpData.EnsembleData.EnsDateTime.Minute, "Incorrect Ensemble time in Minutes " + adcpData.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Second, adcpData.EnsembleData.EnsDateTime.Second, "Incorrect Ensemble time in Seconds " + adcpData.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Millisecond, adcpData.EnsembleData.EnsDateTime.Millisecond, "Incorrect Ensemble time in Milliseconds " + adcpData.EnsembleData.EnsDateTime.ToString());
        }

        /// <summary>
        /// Test the constructor that takes a Prti02Sentence.
        /// </summary>
        [Test]
        public void TestConstructorPrti02Sentence()
        {
            string nmea = "$PRTI02,1000,1,1468,-99999,-99999,-99999,0,,,,,0004*06";

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
            Assert.AreEqual(1000, sent.StartTime, "NMEA start time incorrect");
            Assert.AreEqual(DateTime.Now.Hour, adcpData.EnsembleData.EnsDateTime.Hour, "Incorrect Ensemble time in Hours " + adcpData.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Minute, adcpData.EnsembleData.EnsDateTime.Minute, "Incorrect Ensemble time in Minutes " + adcpData.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Second, adcpData.EnsembleData.EnsDateTime.Second, "Incorrect Ensemble time in Seconds " + adcpData.EnsembleData.EnsDateTime.ToString());
            Assert.AreEqual(DateTime.Now.Millisecond, adcpData.EnsembleData.EnsDateTime.Millisecond, "Incorrect Ensemble time in Milliseconds " + adcpData.EnsembleData.EnsDateTime.ToString());
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

        /// <summary>
        /// Test the constructor that takes no data.
        /// </summary>
        [Test]
        public void TestEmptyConstructor()
        {
            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Sentence to data set
            adcpData.AddEnsembleData(DataSet.Ensemble.DATATYPE_INT,                         // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.EnsembleDataID);               // Dataset ID

            Assert.IsTrue(adcpData.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_INT, adcpData.EnsembleData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, adcpData.EnsembleData.NumBins, "Number of bins is incorrect.");
            Assert.AreEqual(30, adcpData.EnsembleData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(4, adcpData.EnsembleData.NumBeams, "Number of beams is incorrect.");
            Assert.AreEqual(4, adcpData.EnsembleData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.EnsembleData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.EnsembleData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.EnsembleDataID, adcpData.EnsembleData.Name, "Name is incorrect.");
            //Assert.AreEqual(DateTime.Now, adcpData.EnsembleData.EnsDateTime, "Date Time is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, adcpData.EnsembleData.Year, "Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, adcpData.EnsembleData.Month, "Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, adcpData.EnsembleData.Day, "Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, adcpData.EnsembleData.Hour, "Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, adcpData.EnsembleData.Minute, "Minute is incorrect.");
            Assert.AreEqual(DateTime.Now.Second, adcpData.EnsembleData.Second, "Second is incorrect.");
            Assert.AreEqual(new SerialNumber(), adcpData.EnsembleData.SysSerialNumber, "Serial Number is incorrect.");
            Assert.AreEqual(new Firmware(), adcpData.EnsembleData.SysFirmware, "Firmware is incorrect.");
            Assert.AreEqual(new SubsystemConfiguration(), adcpData.EnsembleData.SubsystemConfig, "Subsystem Configuration is incorrect.");
            Assert.AreEqual(new Status(0), adcpData.EnsembleData.Status, "Status is incorrect.");

        }

        /// <summary>
        /// Test encode.
        /// Because the firmware version is less than 0.2.13, the SubsystemCode will be changed
        /// based off the serial number given.  View Encode code for full details why.
        /// </summary>
        [Test]
        public void TestEncodeFirmwareSubsystemCodeChange()
        {
            // Create dataset
            DataSet.Ensemble ensemble = new DataSet.Ensemble();

            // Add data to data set
            ensemble.AddEnsembleData(DataSet.Ensemble.DATATYPE_INT,                         // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.EnsembleDataID);               // Dataset ID

            Assert.IsTrue(ensemble.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_INT, ensemble.EnsembleData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, ensemble.EnsembleData.NumBins, "Number of bins is incorrect.");
            Assert.AreEqual(30, ensemble.EnsembleData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(4, ensemble.EnsembleData.NumBeams, "Number of beams is incorrect.");
            Assert.AreEqual(4, ensemble.EnsembleData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, ensemble.EnsembleData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, ensemble.EnsembleData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.EnsembleDataID, ensemble.EnsembleData.Name, "Name is incorrect.");
            //Assert.AreEqual(DateTime.Now, adcpData.EnsembleData.EnsDateTime, "Date Time is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, ensemble.EnsembleData.Year, "Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, ensemble.EnsembleData.Month, "Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, ensemble.EnsembleData.Day, "Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, ensemble.EnsembleData.Hour, "Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, ensemble.EnsembleData.Minute, "Minute is incorrect.");
            Assert.AreEqual(DateTime.Now.Second, ensemble.EnsembleData.Second, "Second is incorrect.");
            Assert.AreEqual(new SerialNumber(), ensemble.EnsembleData.SysSerialNumber, "Serial Number is incorrect.");
            Assert.AreEqual(new Firmware(), ensemble.EnsembleData.SysFirmware, "Firmware is incorrect.");
            Assert.AreEqual(new SubsystemConfiguration(), ensemble.EnsembleData.SubsystemConfig, "Subsystem Configuration is incorrect.");
            Assert.AreEqual(new Status(0), ensemble.EnsembleData.Status, "Status is incorrect.");

            ensemble.EnsembleData.SysSerialNumber = new SerialNumber("01370000000000000000000000000001");
            ensemble.EnsembleData.SysFirmware = new Firmware(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, 0, 2, 3);
            ensemble.EnsembleData.SubsystemConfig = new SubsystemConfiguration(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), 2);
            ensemble.EnsembleData.Status = new Status(0x4000);

            Assert.AreEqual(new SerialNumber("01370000000000000000000000000001"), ensemble.EnsembleData.SysSerialNumber, "Modded Serial Number is incorrect.");

            // The Subsystem.SUB_2MHZ_4BEAM_20DEG_PISTON_1 will be converted from Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3 to Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_45OFFSET_7.
            // 0x7 will come from the serial number.  Because the firmware version is less
            // than 0.2.13, the SubsystemCode is treated as an index.  Index 1 in the subsystem from the serial number is 7.
            Assert.AreEqual(new Firmware(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, 0, 2, 3), ensemble.EnsembleData.SysFirmware, "Modded Firmware is incorrect.");
            Assert.AreEqual(new SubsystemConfiguration(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), 2), ensemble.EnsembleData.SubsystemConfig, "Modded Subsystem Configuration is incorrect.");
            Assert.AreEqual(new Status(0x4000), ensemble.EnsembleData.Status, "Modded Status is incorrect.");

            // Encode the data
            byte[] encoded = ensemble.EnsembleData.Encode();

            DataSet.Ensemble ens1 = new DataSet.Ensemble();
            // Add encoded data to dataset
            ensemble.AddEnsembleData(DataSet.Ensemble.DATATYPE_INT,                         // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.EnsembleDataID,                // Dataset ID
                                            encoded);                                       // Data

            Assert.AreEqual(((DataSet.EnsembleDataSet.NUM_DATA_ELEMENTS * DataSet.Ensemble.BYTES_IN_FLOAT) + DataSet.Ensemble.PAYLOAD_HEADER_LEN), encoded.Length, "Encoded length is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, ensemble.EnsembleData.Year, "ens1 Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, ensemble.EnsembleData.Month, "ens1 Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, ensemble.EnsembleData.Day, "ens1 Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, ensemble.EnsembleData.Hour, "ens1 Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, ensemble.EnsembleData.Minute, "ens1 Minute is incorrect.");
            Assert.AreEqual(DateTime.Now.Second, ensemble.EnsembleData.Second, "ens1 Second is incorrect.");
            Assert.AreEqual(new SerialNumber("01370000000000000000000000000001"), ensemble.EnsembleData.SysSerialNumber, "ens1 Serial Number is incorrect.");
            Assert.AreEqual(new Firmware(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, 0, 2, 3), ensemble.EnsembleData.SysFirmware, "ens1 Firmware is incorrect.");
            Assert.AreEqual(new SubsystemConfiguration(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), 2), ensemble.EnsembleData.SubsystemConfig, "ens1 Subsystem Configuration is incorrect.");
            Assert.AreEqual(new Status(0x4000), ensemble.EnsembleData.Status, "ens1 Status is incorrect.");

        }

        /// <summary>
        /// Test encode.
        /// </summary>
        [Test]
        public void TestEncode()
        {
            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add data to data set
            adcpData.AddEnsembleData(DataSet.Ensemble.DATATYPE_INT,                         // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.EnsembleDataID);               // Dataset ID

            Assert.IsTrue(adcpData.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_INT, adcpData.EnsembleData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, adcpData.EnsembleData.NumBins, "Number of bins is incorrect.");
            Assert.AreEqual(30, adcpData.EnsembleData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(4, adcpData.EnsembleData.NumBeams, "Number of beams is incorrect.");
            Assert.AreEqual(4, adcpData.EnsembleData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.EnsembleData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.EnsembleData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.EnsembleDataID, adcpData.EnsembleData.Name, "Name is incorrect.");
            //Assert.AreEqual(DateTime.Now, adcpData.EnsembleData.EnsDateTime, "Date Time is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, adcpData.EnsembleData.Year, "Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, adcpData.EnsembleData.Month, "Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, adcpData.EnsembleData.Day, "Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, adcpData.EnsembleData.Hour, "Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, adcpData.EnsembleData.Minute, "Minute is incorrect.");
            Assert.AreEqual(DateTime.Now.Second, adcpData.EnsembleData.Second, "Second is incorrect.");
            Assert.AreEqual(new SerialNumber(), adcpData.EnsembleData.SysSerialNumber, "Serial Number is incorrect.");
            Assert.AreEqual(new Firmware(), adcpData.EnsembleData.SysFirmware, "Firmware is incorrect.");
            Assert.AreEqual(new SubsystemConfiguration(), adcpData.EnsembleData.SubsystemConfig, "Subsystem Configuration is incorrect.");
            Assert.AreEqual(new Status(0), adcpData.EnsembleData.Status, "Status is incorrect.");

            adcpData.EnsembleData.SysSerialNumber = new SerialNumber("01300000000000000000000000000001");
            adcpData.EnsembleData.SysFirmware = new Firmware(0x1, 5, 2, 3);
            adcpData.EnsembleData.SubsystemConfig = new SubsystemConfiguration(new Subsystem(0x1), 2);
            adcpData.EnsembleData.Status = new Status(0x4000);

            Assert.AreEqual(new SerialNumber("01300000000000000000000000000001"), adcpData.EnsembleData.SysSerialNumber, "Modded Serial Number is incorrect.");
            Assert.AreEqual(new Firmware(0x1, 5, 2, 3), adcpData.EnsembleData.SysFirmware, "Modded Firmware is incorrect.");
            Assert.AreEqual(new SubsystemConfiguration(new Subsystem(0x1), 2), adcpData.EnsembleData.SubsystemConfig, "Modded Subsystem Configuration is incorrect.");
            Assert.AreEqual(new Status(0x4000), adcpData.EnsembleData.Status, "Modded Status is incorrect.");

            // Encode the data
            byte[] encoded = adcpData.EnsembleData.Encode();

            DataSet.Ensemble ens1 = new DataSet.Ensemble();
            // Add encoded data to dataset
            adcpData.AddEnsembleData(DataSet.Ensemble.DATATYPE_INT,                         // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.EnsembleDataID,                // Dataset ID
                                            encoded);                                       // Data

            Assert.AreEqual(((DataSet.EnsembleDataSet.NUM_DATA_ELEMENTS * DataSet.Ensemble.BYTES_IN_FLOAT) + DataSet.Ensemble.PAYLOAD_HEADER_LEN), encoded.Length, "Encoded length is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, adcpData.EnsembleData.Year, "ens1 Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, adcpData.EnsembleData.Month, "ens1 Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, adcpData.EnsembleData.Day, "ens1 Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, adcpData.EnsembleData.Hour, "ens1 Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, adcpData.EnsembleData.Minute, "ens1 Minute is incorrect.");
            Assert.AreEqual(DateTime.Now.Second, adcpData.EnsembleData.Second, "ens1 Second is incorrect.");
            Assert.AreEqual(new SerialNumber("01300000000000000000000000000001"), adcpData.EnsembleData.SysSerialNumber, "ens1 Serial Number is incorrect.");
            Assert.AreEqual(new Firmware(0x1, 5, 2, 3), adcpData.EnsembleData.SysFirmware, "ens1 Firmware is incorrect.");
            Assert.AreEqual(new SubsystemConfiguration(new Subsystem(0x1), 2), adcpData.EnsembleData.SubsystemConfig, "ens1 Subsystem Configuration is incorrect.");
            Assert.AreEqual(new Status(0x4000), adcpData.EnsembleData.Status, "ens1 Status is incorrect.");

        }

        /// <summary>
        /// SubsystemConfig was not being cloned correct.  The
        /// subystem code was being lost in Subsystem within SubsystemConfig.
        /// </summary>
        [Test]
        public void SetSubsystemConfigTest()
        {
            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add data to data set
            adcpData.AddEnsembleData(DataSet.Ensemble.DATATYPE_INT,                         // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.EnsembleDataID);               // Dataset ID

            adcpData.EnsembleData.SysSerialNumber = new SerialNumber("01300000000000000000000000000001");
            adcpData.EnsembleData.SysFirmware = new Firmware(Subsystem.EMPTY_CODE, 0, 2, 3);                                     // SubsystemCode is SubsystemIndex because of firmware version
            adcpData.EnsembleData.SubsystemConfig = new SubsystemConfiguration(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), 2);

            byte[] newEns = adcpData.EnsembleData.Encode();

            // Create dataset
            DataSet.Ensemble newAdcpData = new DataSet.Ensemble();

            // Add data to data set
            newAdcpData.AddEnsembleData(DataSet.Ensemble.DATATYPE_INT,                         // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.EnsembleDataID,                // Dataset ID
                                            newEns);                                        // Encoded ensemble


            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, newAdcpData.EnsembleData.SysFirmware.GetSubsystemCode(newAdcpData.EnsembleData.SysSerialNumber), "SysFirmware SubsystemCode is incorrect.");
            Assert.AreEqual(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), newAdcpData.EnsembleData.SysFirmware.GetSubsystem(newAdcpData.EnsembleData.SysSerialNumber), "SysFirmware GetSubsystem() is incorrect.");
            Assert.AreEqual(2, newAdcpData.EnsembleData.SubsystemConfig.ConfigNumber, "SubsystemConfig config number is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, newAdcpData.EnsembleData.SubsystemConfig.SubSystem.Code, "SubsystemConfig Subsystem code is incorrect.");

            //DataSet.Ensemble result = adcpData.Clone();

            //Assert.AreEqual(2, result.EnsembleData.SubsystemConfig.ConfigNumber, "SubsystemConfig config number is incorrect.");
            //Assert.AreEqual(0x3, result.EnsembleData.SubsystemConfig.SubSystem.Code, "SubsystemConfig Subsystem code is incorrect.");
        }

        /// <summary>
        /// Test setting the SubsystemConfig.  The SubsystemConfig Subsystem should
        /// match the Firmware SubsystemCode.  Because the Firmware version is before 0.2.13
        /// the Firmware SubsystemCode was an index. When decoded it is changed from an index
        /// to the code.  But this is not being decoded, so the change is not going to be made.
        /// </summary>
        [Test]
        public void TestCloneSubsystemConfig()
        {
            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add data to data set
            adcpData.AddEnsembleData(DataSet.Ensemble.DATATYPE_INT,                         // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.EnsembleDataID);               // Dataset ID

            adcpData.EnsembleData.SysSerialNumber = new SerialNumber("01300000000000000000000000000001");
            adcpData.EnsembleData.SysFirmware = new Firmware(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, 0, 2, 3);
            adcpData.EnsembleData.SubsystemConfig = new SubsystemConfiguration(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), 2);


            DataSet.Ensemble result = adcpData.Clone();

            Assert.AreEqual(2, result.EnsembleData.SubsystemConfig.ConfigNumber, "SubsystemConfig config number is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, result.EnsembleData.SubsystemConfig.SubSystem.Code, "SubsystemConfig Subsystem code is incorrect.");
            Assert.AreEqual(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), result.EnsembleData.SysFirmware.GetSubsystem(result.EnsembleData.SysSerialNumber), "SysFirmware GetSubsystem is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, result.EnsembleData.SysFirmware.GetSubsystemCode(result.EnsembleData.SysSerialNumber), "Firmware SubsystemCode is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, result.EnsembleData.SubsystemConfig.SubSystem.Code, "SubsystemConfig Subystem Code is incorrect.");

            // When recloning the firmware SubsystemCode was changed from SubsystemIndex to SubsystemCode
            // But the next time it is cloned, it will see the firmware version and do the change from SubsystemIndex to SubsystemCode again
            DataSet.Ensemble result1 = result.Clone();

            Assert.AreEqual(2, result1.EnsembleData.SubsystemConfig.ConfigNumber, "Result 1 SubsystemConfig config number is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, result1.EnsembleData.SubsystemConfig.SubSystem.Code, "Result 1 SubsystemConfig Subsystem code is incorrect.");
            Assert.AreEqual(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), result1.EnsembleData.SysFirmware.GetSubsystem(result1.EnsembleData.SysSerialNumber), "Result 1 SysFirmware GetSubsystem is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, result1.EnsembleData.SysFirmware.GetSubsystemCode(result1.EnsembleData.SysSerialNumber), "Result 1 Firmware SubsystemCode is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, result1.EnsembleData.SubsystemConfig.SubSystem.Code, "Result 1 SubsystemConfig Subystem Code is incorrect.");
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
            ensemble.EnsembleData.EnsembleNumber = 1222;
            ensemble.EnsembleData.NumBins = 30;
            ensemble.EnsembleData.NumBeams = 4;
            ensemble.EnsembleData.DesiredPingCount = 2;
            ensemble.EnsembleData.ActualPingCount = 2;
            ensemble.EnsembleData.SysSerialNumber = new SerialNumber("01300000000000000000000000000001");
            ensemble.EnsembleData.SysFirmware = new Firmware(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, 0, 2, 3);
            ensemble.EnsembleData.SubsystemConfig = new SubsystemConfiguration(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), 2);
            ensemble.EnsembleData.Status = new Status(1);
            ensemble.EnsembleData.Year = DateTime.Now.Year;
            ensemble.EnsembleData.Month = DateTime.Now.Month;
            ensemble.EnsembleData.Day = DateTime.Now.Day;
            ensemble.EnsembleData.Hour = DateTime.Now.Hour;
            ensemble.EnsembleData.Minute = DateTime.Now.Minute;
            ensemble.EnsembleData.Second = DateTime.Now.Second;
            ensemble.EnsembleData.HSec = 22;
            ensemble.EnsembleData.EnsDateTime = DateTime.Now;
            ensemble.EnsembleData.UniqueId = new DataSet.UniqueID(ensemble.EnsembleData.EnsembleNumber, ensemble.EnsembleData.EnsDateTime);

            string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EnsembleData);                                      // Serialize object to JSON
            DataSet.EnsembleDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.EnsembleDataSet>(encoded);       // Deserialize the JSON

            // Verify the values are the same
            Assert.AreEqual(1222, decoded.EnsembleNumber, "EnsembleNumber is incorrect.");
            Assert.AreEqual(30, decoded.NumBins, "NumBins is incorrect.");
            Assert.AreEqual(4, decoded.NumBeams, "NumBeams is incorrect.");
            Assert.AreEqual(2, decoded.DesiredPingCount, "DesiredPingCount is incorrect.");
            Assert.AreEqual(2, decoded.ActualPingCount, "ActualPingCount is incorrect.");
            Assert.AreEqual(new SerialNumber("01300000000000000000000000000001"), decoded.SysSerialNumber, "SysSerialNumber is incorrect.");
            Assert.AreEqual(1, decoded.SysSerialNumber.SystemSerialNumber, "Serial number is incorrect.");
            Assert.AreEqual("300000000000000", decoded.SysSerialNumber.SubSystems, "Subsystems are incorrect.");
            Assert.AreEqual(1, decoded.SysSerialNumber.SubSystemsDict.Count, "Subsystem count is incorrect.");
            Assert.AreEqual(new Firmware(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, 0, 2, 3), decoded.SysFirmware, "SysFirmware is incorrect.");
            Assert.AreEqual(0, decoded.SysFirmware.FirmwareMajor, "Firmware Major is incorrect.");
            Assert.AreEqual(2, decoded.SysFirmware.FirmwareMinor, "Firmware Minor is incorrect.");
            Assert.AreEqual(3, decoded.SysFirmware.FirmwareRevision, "Firmware Revision is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, decoded.SysFirmware.GetSubsystemCode(decoded.SysSerialNumber), "Firmware Subsystem code is incorrect.");
            Assert.AreEqual(2, decoded.SubsystemConfig.ConfigNumber, "SubsystemConfig config number is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, decoded.SubsystemConfig.SubSystem.Code, "SubsystemConfig Subsystem code is incorrect.");
            Assert.AreEqual(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), decoded.SysFirmware.GetSubsystem(decoded.SysSerialNumber), "SysFirmware GetSubsystem is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, decoded.SysFirmware.GetSubsystemCode(decoded.SysSerialNumber), "Firmware SubsystemCode is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, decoded.SubsystemConfig.SubSystem.Code, "SubsystemConfig Subystem Code is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, decoded.EnsDateTime.Year, "EnsDateTime Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, decoded.Year, "Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, decoded.EnsDateTime.Month, "EnsDateTime Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, decoded.Month, "Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, decoded.EnsDateTime.Day, "EnsDateTime Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, decoded.Day, "Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, decoded.EnsDateTime.Hour, "EnsDateTime Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, decoded.Hour, "Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, decoded.EnsDateTime.Minute, "EnsDateTime Minute is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, decoded.Minute, "Minute is incorrect.");
            Assert.AreEqual(DateTime.Now.Second, decoded.EnsDateTime.Second, "EnsDateTime Second is incorrect.");
            Assert.AreEqual(DateTime.Now.Second, decoded.Second, "Second is incorrect.");
            Assert.AreEqual(new DataSet.UniqueID(ensemble.EnsembleData.EnsembleNumber, ensemble.EnsembleData.EnsDateTime), decoded.UniqueId, "UniqueID is incorrect.");
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
                string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EnsembleData);
            }
            watch.Stop();
            long resultSerialize = watch.ElapsedMilliseconds;

            // Test Deserialize()
            string encodedd = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EnsembleData);
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                DataSet.EnsembleDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.EnsembleDataSet>(encodedd);
            }
            watch.Stop();
            long resultDeserialize = watch.ElapsedMilliseconds;

            Debug.WriteLine(String.Format("Serialize:{0}  Deserialize:{1}", resultSerialize, resultDeserialize));

            Debug.WriteLine("Complete");

        }


    }

}