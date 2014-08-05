///*
// * Copyright © 2011 
// * Rowe Technology Inc.
// * All rights reserved.
// * http://www.rowetechinc.com
// * 
// * Redistribution and use in source and binary forms, with or without
// * modification is NOT permitted.
// * 
// * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
// * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
// * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
// * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
// * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
// * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// * POSSIBILITY OF SUCH DAMAGE.
// * 
// * HISTORY
// * -----------------------------------------------------------------
// * Date            Initials    Version    Comments
// * -----------------------------------------------------------------
// * 12/24/2012      RC          2.17       Initial coding
// * 01/07/2013      RC          2.17       Added Test for Ensemble, Ancillary, Amplitude and Correlation dataset.
// * 01/17/2013      RC          2.17       Added test for Bottom Track dataset.
// * 
// * 
// */
//namespace RTI
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using NUnit.Framework;

//    /// <summary>
//    /// Test the MathHelper class.
//    /// </summary>
//    [TestFixture]
//    public class EnsembleHelperTest
//    {

//        #region GenerateEnsemble

//        /// <summary>
//        /// Test the constructor.
//        /// Verify all the datasets were created.
//        /// </summary>
//        [Test]
//        public void GenerateEnsemble()
//        {

//            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(10, 10);

//            Assert.IsNotNull(ensemble, "Ensemble is incorrect");
//            Assert.IsTrue(ensemble.IsEnsembleAvail, "Ensemble is incorrect.");
//            Assert.IsTrue(ensemble.IsAncillaryAvail, "Ancillary is incorrect.");
//            Assert.IsTrue(ensemble.IsCorrelationAvail, "Correlation is incorrect.");
//            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "EarthVelocity is incorrect.");
//        }

//        /// <summary>
//        /// Test the constructor.
//        /// Verify all the datasets were created.
//        /// Default number of beams
//        /// </summary>
//        [Test]
//        public void GenerateEnsembleDefault()
//        {

//            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(10);

//            Assert.IsNotNull(ensemble, "Ensemble is incorrect");
//            Assert.IsTrue(ensemble.IsEnsembleAvail, "Ensemble is incorrect.");
//            Assert.IsTrue(ensemble.IsAncillaryAvail, "Ancillary is incorrect.");
//            Assert.IsTrue(ensemble.IsCorrelationAvail, "Correlation is incorrect.");
//            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "EarthVelocity is incorrect.");
//        }

//        /// <summary>
//        /// Create ensemble with no datasets.
//        /// </summary>
//        [Test]
//        public void GenerateNoDataSets()
//        {
//            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(10, 4, false);

//            Assert.IsNotNull(ensemble, "Ensemble is incorrect");
//            Assert.IsFalse(ensemble.IsEnsembleAvail, "Ensemble is incorrect.");
//            Assert.IsFalse(ensemble.IsAncillaryAvail, "Ancillary is incorrect.");
//            Assert.IsFalse(ensemble.IsCorrelationAvail, "Correlation is incorrect.");
//            Assert.IsFalse(ensemble.IsEarthVelocityAvail, "EarthVelocity is incorrect.");
//        }

//        /// <summary>
//        /// Verify the Earth Velocity data is correct.
//        /// </summary>
//        [Test]
//        public void EarthVelocity()
//        {
//            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(10, 20);

//            Assert.IsNotNull(ensemble, "Ensemble is incorrect");
//            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "EarthVelocity is incorrect.");
//            Assert.AreEqual(10, ensemble.EarthVelocityData.EarthVelocityData.GetLength(0), "Number of bins is incorrect.");
//            Assert.AreEqual(20, ensemble.EarthVelocityData.EarthVelocityData.GetLength(1), "Number of beams is incorrect.");
//            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[0, 0], "Array not zeroed.");
//        }

//        #endregion

//        #region Add Ensemble DataSet

//        /// <summary>
//        /// Add the Ensemble DataSet.
//        /// </summary>
//        [Test]
//        public void TestAddEnsembleDataSet()
//        {
//            // Create dataset
//            DataSet.Ensemble adcpData = new DataSet.Ensemble();

//            EnsembleHelper.AddEnsemble(ref adcpData, 30);

//            Assert.IsTrue(adcpData.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DATATYPE_INT, adcpData.EnsembleData.ValueType, "DataType is incorrect.");
//            Assert.AreEqual(30, adcpData.EnsembleData.NumBins, "Number of bins is incorrect.");
//            Assert.AreEqual(23, adcpData.EnsembleData.NumElements, "Number of Elements is incorrect.");
//            Assert.AreEqual(4, adcpData.EnsembleData.NumBeams, "Number of beams is incorrect.");
//            Assert.AreEqual(1, adcpData.EnsembleData.ElementsMultiplier, "Element Multiplies are incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.EnsembleData.Imag, "Imag is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.EnsembleData.NameLength, "Name length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.EnsembleDataID, adcpData.EnsembleData.Name, "Name is incorrect.");
//            //Assert.AreEqual(DateTime.Now, adcpData.EnsembleData.EnsDateTime, "Date Time is incorrect.");
//            Assert.AreEqual(DateTime.Now.Year, adcpData.EnsembleData.Year, "Year is incorrect.");
//            Assert.AreEqual(DateTime.Now.Month, adcpData.EnsembleData.Month, "Month is incorrect.");
//            Assert.AreEqual(DateTime.Now.Day, adcpData.EnsembleData.Day, "Day is incorrect.");
//            Assert.AreEqual(DateTime.Now.Hour, adcpData.EnsembleData.Hour, "Hour is incorrect.");
//            Assert.AreEqual(DateTime.Now.Minute, adcpData.EnsembleData.Minute, "Minute is incorrect.");
//            Assert.AreEqual(DateTime.Now.Second, adcpData.EnsembleData.Second, "Second is incorrect.");
//            Assert.AreEqual(new SerialNumber(), adcpData.EnsembleData.SysSerialNumber, "Serial Number is incorrect.");
//            Assert.AreEqual(new Firmware(), adcpData.EnsembleData.SysFirmware, "Firmware is incorrect.");
//            Assert.AreEqual(new SubsystemConfiguration(), adcpData.EnsembleData.SubsystemConfig, "Subsystem Configuration is incorrect.");
//            Assert.AreEqual(new Status(0), adcpData.EnsembleData.Status, "Status is incorrect.");

//        }

//        #endregion

//        #region Add Ancillary DataSet

//        /// <summary>
//        /// Test adding the Ancillary DataSet.
//        /// </summary>
//        [Test]
//        public void TestAddAncillaryDataSet()
//        {
//            // Create dataset
//            DataSet.Ensemble adcpData = new DataSet.Ensemble();

//            // Add the Ancillary DataSet
//            EnsembleHelper.AddAncillary(ref adcpData);

//            Assert.IsTrue(adcpData.IsAncillaryAvail, "IsEnsembleAvail is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.AncillaryData.ValueType, "DataType is incorrect.");
//            Assert.AreEqual(23, adcpData.AncillaryData.NumElements, "Number of Elements is incorrect.");
//            Assert.AreEqual(1, adcpData.AncillaryData.ElementsMultiplier, "Element Multiplies are incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.AncillaryData.Imag, "Imag is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.AncillaryData.NameLength, "Name length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.AncillaryID, adcpData.AncillaryData.Name, "Name is incorrect.");
//            Assert.AreEqual(0, adcpData.AncillaryData.FirstBinRange, "First Bin Range is incorrect.");
//            Assert.AreEqual(0, adcpData.AncillaryData.BinSize, "BinSize is incorrect.");
//            Assert.AreEqual(0, adcpData.AncillaryData.FirstPingTime, "FirstPingTime is incorrect.");
//            Assert.AreEqual(0, adcpData.AncillaryData.LastPingTime, "LastPingTime is incorrect.");
//            Assert.AreEqual(0, adcpData.AncillaryData.Heading, "Heading is incorrect.");
//            Assert.AreEqual(0, adcpData.AncillaryData.Pitch, "Pitch is incorrect.");
//            Assert.AreEqual(0, adcpData.AncillaryData.Roll, "Roll is incorrect.");
//            Assert.AreEqual(0, adcpData.AncillaryData.WaterTemp, "WaterTemp is incorrect.");
//            Assert.AreEqual(0, adcpData.AncillaryData.SystemTemp, "SystemTemp is incorrect.");
//            Assert.AreEqual(0, adcpData.AncillaryData.Salinity, "Salinity is incorrect.");
//            Assert.AreEqual(0, adcpData.AncillaryData.Pressure, "Pressure is incorrect.");
//            Assert.AreEqual(0, adcpData.AncillaryData.TransducerDepth, "TransducerDepth is incorrect.");
//            Assert.AreEqual(0, adcpData.AncillaryData.SpeedOfSound, "SpeedOfSound is incorrect.");

//        }

//        #endregion

//        #region Add Correlation

//        /// <summary>
//        /// Test adding the Correlation DataSet.
//        /// </summary>
//        [Test]
//        public void TestAddCorrelationDataSet()
//        {
//            // Create dataset
//            DataSet.Ensemble adcpData = new DataSet.Ensemble();

//            // Add the Ancillary DataSet
//            EnsembleHelper.AddCorrelation(ref adcpData, 30);

//            Assert.IsTrue(adcpData.IsCorrelationAvail, "IsCorrelation is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.CorrelationData.ValueType, "DataType is incorrect.");
//            Assert.AreEqual(30, adcpData.CorrelationData.NumElements, "Number of Elements is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.CorrelationData.ElementsMultiplier, "Element Multiplies are incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.CorrelationData.Imag, "Imag is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.CorrelationData.NameLength, "Name length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.CorrelationID, adcpData.CorrelationData.Name, "Name is incorrect.");

//            Assert.AreEqual(adcpData.CorrelationData.CorrelationData.GetLength(0), 30, "Correlation Array Dimension 1 is incorrect.");
//            Assert.AreEqual(adcpData.CorrelationData.CorrelationData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Correlation Array Dimension 2 is incorrect.");

//        }

//        /// <summary>
//        /// Test adding the Correlation DataSet.
//        /// </summary>
//        [Test]
//        public void TestAddCorrelationDataSet1()
//        {
//            // Create dataset
//            DataSet.Ensemble adcpData = new DataSet.Ensemble();

//            // Add the Ancillary DataSet
//            EnsembleHelper.AddCorrelation(ref adcpData, 30, DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM);

//            Assert.IsTrue(adcpData.IsCorrelationAvail, "IsCorrelation is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.CorrelationData.ValueType, "DataType is incorrect.");
//            Assert.AreEqual(30, adcpData.CorrelationData.NumElements, "Number of Elements is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.CorrelationData.ElementsMultiplier, "Element Multiplies are incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.CorrelationData.Imag, "Imag is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.CorrelationData.NameLength, "Name length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.CorrelationID, adcpData.CorrelationData.Name, "Name is incorrect.");

//            Assert.AreEqual(adcpData.CorrelationData.CorrelationData.GetLength(0), 30, "Correlation Array Dimension 1 is incorrect.");
//            Assert.AreEqual(adcpData.CorrelationData.CorrelationData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Correlation Array Dimension 2 is incorrect.");

//        }

//        #endregion

//        #region Add Amplitude

//        /// <summary>
//        /// Test adding the Amplitude DataSet.
//        /// </summary>
//        [Test]
//        public void TestAddAmplitudeDataSet()
//        {
//            // Create dataset
//            DataSet.Ensemble adcpData = new DataSet.Ensemble();

//            // Add the Ancillary DataSet
//            EnsembleHelper.AddAmplitude(ref adcpData, 30);

//            Assert.IsTrue(adcpData.IsAmplitudeAvail, "IsAmplitude is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.AmplitudeData.ValueType, "DataType is incorrect.");
//            Assert.AreEqual(30, adcpData.AmplitudeData.NumElements, "Number of Elements is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.AmplitudeData.ElementsMultiplier, "Element Multiplies are incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.AmplitudeData.Imag, "Imag is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.AmplitudeData.NameLength, "Name length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.CorrelationID, adcpData.AmplitudeData.Name, "Name is incorrect.");

//            Assert.AreEqual(adcpData.AmplitudeData.AmplitudeData.GetLength(0), 30, "Amplitude Array Dimension 1 is incorrect.");
//            Assert.AreEqual(adcpData.AmplitudeData.AmplitudeData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Amplitude Array Dimension 2 is incorrect.");

//        }

//        /// <summary>
//        /// Test adding the Amplitude DataSet.
//        /// </summary>
//        [Test]
//        public void TestAddAmplitudeDataSet1()
//        {
//            // Create dataset
//            DataSet.Ensemble adcpData = new DataSet.Ensemble();

//            // Add the Ancillary DataSet
//            EnsembleHelper.AddAmplitude(ref adcpData, 30, DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM);

//            Assert.IsTrue(adcpData.IsAmplitudeAvail, "IsAmplitude is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.AmplitudeData.ValueType, "DataType is incorrect.");
//            Assert.AreEqual(30, adcpData.AmplitudeData.NumElements, "Number of Elements is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.AmplitudeData.ElementsMultiplier, "Element Multiplies are incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.AmplitudeData.Imag, "Imag is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.AmplitudeData.NameLength, "Name length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.CorrelationID, adcpData.AmplitudeData.Name, "Name is incorrect.");

//            Assert.AreEqual(adcpData.AmplitudeData.AmplitudeData.GetLength(0), 30, "Amplitude Array Dimension 1 is incorrect.");
//            Assert.AreEqual(adcpData.AmplitudeData.AmplitudeData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Amplitude Array Dimension 2 is incorrect.");

//        }

//        #endregion

//        #region Add Earth Velocity

//        /// <summary>
//        /// Verify the Earth Velocity data is correct.
//        /// </summary>
//        [Test]
//        public void AddEarthVelocity()
//        {
//            DataSet.Ensemble ensemble = new DataSet.Ensemble();
//            EnsembleHelper.AddEarthVelocity(ref ensemble, 10, 20);

//            Assert.IsNotNull(ensemble, "Ensemble is incorrect");
//            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "EarthVelocity is incorrect.");
//            Assert.AreEqual(10, ensemble.EarthVelocityData.EarthVelocityData.GetLength(0), "Number of bins is incorrect.");
//            Assert.AreEqual(20, ensemble.EarthVelocityData.EarthVelocityData.GetLength(1), "Number of beams is incorrect.");
//            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[0, 0], "Array not zeroed.");
//        }

//        /// <summary>
//        /// Verify the Earth Velocity data is correct.
//        /// </summary>
//        [Test]
//        public void AddEarthVelocityDefault()
//        {
//            DataSet.Ensemble ensemble = new DataSet.Ensemble();
//            EnsembleHelper.AddEarthVelocity(ref ensemble, 10);

//            Assert.IsNotNull(ensemble, "Ensemble is incorrect");
//            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "EarthVelocity is incorrect.");
//            Assert.AreEqual(10, ensemble.EarthVelocityData.EarthVelocityData.GetLength(0), "Number of bins is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.EarthVelocityData.EarthVelocityData.GetLength(1), "Number of beams is incorrect.");
//            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[0, 0], "Array not zeroed.");
//        }

//        /// <summary>
//        /// Verify the Earth Velocity data is correct.
//        /// Already exist so replace.
//        /// </summary>
//        [Test]
//        public void AddEarthVelocityReplace()
//        {
//            DataSet.Ensemble ensemble = new DataSet.Ensemble();
//            EnsembleHelper.AddEarthVelocity(ref ensemble, 10);
//            EnsembleHelper.AddEarthVelocity(ref ensemble, 11);

//            Assert.IsNotNull(ensemble, "Ensemble is incorrect");
//            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "EarthVelocity is incorrect.");
//            Assert.AreEqual(11, ensemble.EarthVelocityData.EarthVelocityData.GetLength(0), "Number of bins is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.EarthVelocityData.EarthVelocityData.GetLength(1), "Number of beams is incorrect.");
//            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[0, 0], "Array not zeroed.");
//        }
        
//        #endregion

//        #region Add Bottom Track

//        /// <summary>
//        /// Add the Bottom Track DataSet.
//        /// </summary>
//        [Test]
//        public void AddBottomTrackDataTest()
//        {
//            DataSet.Ensemble ensemble = new DataSet.Ensemble();
//            EnsembleHelper.AddBottomTrack(ref ensemble);

//            Assert.IsTrue(ensemble.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.ActualPingCount, "Actual Ping Count is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.FirstPingTime, "FirstPingTime is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.LastPingTime, "LastPingTime is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.Heading, "Heading is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.Pitch, "Pitch is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.Roll, "Roll is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.WaterTemp, "WaterTemp is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.SystemTemp, "SystemTemp is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.Salinity, "Salinity is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.Pressure, "Pressure is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.TransducerDepth, "TransducerDepth is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.SpeedOfSound, "SpeedOfSound is incorrect.");
//            Assert.AreEqual(new Status(0), ensemble.BottomTrackData.Status, "Status is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.BottomTrackData.NumBeams, "NumBeams is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.BottomTrackData.Range.GetLength(0), "Range Length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.BottomTrackData.SNR.GetLength(0), "SNR Length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.BottomTrackData.Amplitude.GetLength(0), "Amplitude Length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.BottomTrackData.Correlation.GetLength(0), "Correlation Length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.BottomTrackData.BeamVelocity.GetLength(0), "BeamVelocity Length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.BottomTrackData.BeamGood.GetLength(0), "BeamGood Length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.BottomTrackData.InstrumentVelocity.GetLength(0), "InstrumentVelocity Length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.BottomTrackData.InstrumentGood.GetLength(0), "InstrumentGood Length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.BottomTrackData.EarthVelocity.GetLength(0), "EarthVelocity Length is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.BottomTrackData.EarthGood.GetLength(0), "EarthGood Length is incorrect.");
//        }

//        /// <summary>
//        /// Add the Bottom Track DataSet.
//        /// </summary>
//        [Test]
//        public void AddBottomTrackDataTestBeams()
//        {
//            DataSet.Ensemble ensemble = new DataSet.Ensemble();
//            EnsembleHelper.AddBottomTrack(ref ensemble);

//            Assert.IsTrue(ensemble.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.ActualPingCount, "Actual Ping Count is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.FirstPingTime, "FirstPingTime is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.LastPingTime, "LastPingTime is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.Heading, "Heading is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.Pitch, "Pitch is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.Roll, "Roll is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.WaterTemp, "WaterTemp is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.SystemTemp, "SystemTemp is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.Salinity, "Salinity is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.Pressure, "Pressure is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.TransducerDepth, "TransducerDepth is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.BottomTrackData.SpeedOfSound, "SpeedOfSound is incorrect.");
//            Assert.AreEqual(new Status(0), ensemble.BottomTrackData.Status, "Status is incorrect.");
//            Assert.AreEqual(4, ensemble.BottomTrackData.NumBeams, "NumBeams is incorrect.");
//            Assert.AreEqual(4, ensemble.BottomTrackData.Range.GetLength(0), "Range Length is incorrect.");
//            Assert.AreEqual(4, ensemble.BottomTrackData.SNR.GetLength(0), "SNR Length is incorrect.");
//            Assert.AreEqual(4, ensemble.BottomTrackData.Amplitude.GetLength(0), "Amplitude Length is incorrect.");
//            Assert.AreEqual(4, ensemble.BottomTrackData.Correlation.GetLength(0), "Correlation Length is incorrect.");
//            Assert.AreEqual(4, ensemble.BottomTrackData.BeamVelocity.GetLength(0), "BeamVelocity Length is incorrect.");
//            Assert.AreEqual(4, ensemble.BottomTrackData.BeamGood.GetLength(0), "BeamGood Length is incorrect.");
//            Assert.AreEqual(4, ensemble.BottomTrackData.InstrumentVelocity.GetLength(0), "InstrumentVelocity Length is incorrect.");
//            Assert.AreEqual(4, ensemble.BottomTrackData.InstrumentGood.GetLength(0), "InstrumentGood Length is incorrect.");
//            Assert.AreEqual(4, ensemble.BottomTrackData.EarthVelocity.GetLength(0), "EarthVelocity Length is incorrect.");
//            Assert.AreEqual(4, ensemble.BottomTrackData.EarthGood.GetLength(0), "EarthGood Length is incorrect.");
//        }

//        #endregion

//        #region Set Velocities Bad

//        /// <summary>
//        /// Test setting the velocies bad for a specific bin.
//        /// </summary>
//        [Test]
//        public void SetVelocitiesBad()
//        {
//            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(10);
            
//            Assert.IsNotNull(ensemble, "Ensemble is incorrect");
//            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "EarthVelocity is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX], "East Velocity is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX], "North Velocity is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX], "Vertical Velocity is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_Q_INDEX], "Q Velocity is incorrect.");

//            // Bin to bad
//            EnsembleHelper.SetVelocitiesBad(ref ensemble, 2);

//            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX], "Bad East Velocity is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX], "Bad North Velocity is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX], "Bad Vertical Velocity is incorrect.");
//            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_Q_INDEX], "Bad Q Velocity is incorrect.");

//        }

//        /// <summary>
//        /// Test setting the velocies bad for a specific bin.
//        /// </summary>
//        [Test]
//        public void SetVelocitiesBadBad()
//        {
//            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(10);

//            Assert.IsNotNull(ensemble, "Ensemble is incorrect");
//            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "EarthVelocity is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX], "East Velocity is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX], "North Velocity is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX], "Vertical Velocity is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_Q_INDEX], "Q Velocity is incorrect.");

//            // Bin to bad
//            EnsembleHelper.SetVelocitiesBad(ref ensemble, 11);

//            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX], "East Velocity is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX], "North Velocity is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX], "Vertical Velocity is incorrect.");
//            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_Q_INDEX], "Q Velocity is incorrect.");


//        }

//        #endregion
//    }
//}
