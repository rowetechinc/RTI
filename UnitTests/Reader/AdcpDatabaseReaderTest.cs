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
 * 01/07/2013      RC          2.17       Initial coding
 * 03/08/2013      RC          2.18       Added a non-cache timing test.
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Unit test of the Adcp Database Writer.
    /// </summary>
    [TestFixture]
    public class AdcpDatabaseReaderTest
    {

        /// <summary>
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void Timing()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test Project12";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);

                #region Ensemble

                // Modify the data
                ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_0_INDEX] = 2.2f;
                ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_1_INDEX] = 3.3f;
                ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_2_INDEX] = 4.4f;
                ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_3_INDEX] = 5.5f;

                ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
                ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_1_INDEX] = 4.3f;
                ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_2_INDEX] = 5.4f;
                ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_3_INDEX] = 6.5f;

                #endregion
                ensemble.EnsembleData.EnsembleNumber = x;
                ensemble.EnsembleData.EnsDateTime = DateTime.Now;
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();

            Assert.AreEqual(1000, reader.GetNumberOfEnsembles(p), "The numbers of ensembles written was incorrect.");
            
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                DataSet.Ensemble ens0 = reader.GetEnsemble(p, x);
            }
            watch.Stop();
            Debug.WriteLine("Reader result: {0}", watch.ElapsedMilliseconds);

            reader.Shutdown();

        }

        /// <summary>
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// 
        /// This will use the non-cached version to read the data.
        /// 
        /// </summary>
        [Test]
        public void TimingNoCache()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test Project13";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);

                #region Ensemble

                // Modify the data
                ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_0_INDEX] = 2.2f;
                ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_1_INDEX] = 3.3f;
                ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_2_INDEX] = 4.4f;
                ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_3_INDEX] = 5.5f;

                ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
                ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_1_INDEX] = 4.3f;
                ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_2_INDEX] = 5.4f;
                ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_3_INDEX] = 6.5f;

                #endregion
                ensemble.EnsembleData.EnsembleNumber = x;
                ensemble.EnsembleData.EnsDateTime = DateTime.Now;
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();

            Assert.AreEqual(1000, reader.GetNumberOfEnsembles(p), "The numbers of ensembles written was incorrect.");

            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                Cache<long, DataSet.Ensemble> ens = reader.GetEnsembles(p, x, 1);
                DataSet.Ensemble ens0 = ens.IndexValue(0);
            }
            watch.Stop();
            Debug.WriteLine("Reader result: {0}", watch.ElapsedMilliseconds);
            
            reader.Shutdown();
        }


        /// <summary>
        /// Only the Ampltidue DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void AmplitudeDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_AmplitudeDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = new DataSet.Ensemble();
                EnsembleHelper.AddAmplitude(ref ensemble, 30);

                #region Ensemble

                // Modify the data
                ensemble.AmplitudeData.AmplitudeData[0, 0] = 2.3f;
                ensemble.AmplitudeData.AmplitudeData[0, 1] = 3.4f;
                ensemble.AmplitudeData.AmplitudeData[0, 2] = 4.5f;
                ensemble.AmplitudeData.AmplitudeData[0, 3] = 5.6f;
                ensemble.AmplitudeData.AmplitudeData[1, 0] = 6.7f;
                ensemble.AmplitudeData.AmplitudeData[1, 1] = 7.8f;
                ensemble.AmplitudeData.AmplitudeData[1, 2] = 8.9f;
                ensemble.AmplitudeData.AmplitudeData[1, 3] = 9.10f;
                ensemble.AmplitudeData.AmplitudeData[2, 0] = 10.11f;
                ensemble.AmplitudeData.AmplitudeData[2, 1] = 11.12f;
                ensemble.AmplitudeData.AmplitudeData[2, 2] = 12.13f;

                #endregion
                //ensemble.EnsembleData.EnsembleNumber = x;
                //ensemble.EnsembleData.EnsDateTime = DateTime.Now;
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(false, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            Assert.AreEqual(2.3f, ens0.AmplitudeData.AmplitudeData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens0.AmplitudeData.AmplitudeData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens0.AmplitudeData.AmplitudeData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens0.AmplitudeData.AmplitudeData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens0.AmplitudeData.AmplitudeData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens0.AmplitudeData.AmplitudeData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens0.AmplitudeData.AmplitudeData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens0.AmplitudeData.AmplitudeData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens0.AmplitudeData.AmplitudeData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens0.AmplitudeData.AmplitudeData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens0.AmplitudeData.AmplitudeData[2, 2], "2,2 Data is incorrect.");

            reader.Shutdown();
        }

        /// <summary>
        /// Only the Ancillary DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void AncillaryDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_AncillaryDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = new DataSet.Ensemble();
                EnsembleHelper.AddAncillary(ref ensemble, 30);

                #region Modify the data

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

                #endregion
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(false, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            Assert.AreEqual(1.2f, ens0.AncillaryData.FirstBinRange, "First Bin Range is incorrect.");
            Assert.AreEqual(2.3f, ens0.AncillaryData.BinSize, "BinSize is incorrect.");
            Assert.AreEqual(3.4f, ens0.AncillaryData.FirstPingTime, "FirstPingTime is incorrect.");
            Assert.AreEqual(4.5f, ens0.AncillaryData.LastPingTime, "LastPingTime is incorrect.");
            Assert.AreEqual(5.6f, ens0.AncillaryData.Heading, "Heading is incorrect.");
            Assert.AreEqual(6.7f, ens0.AncillaryData.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(7.8f, ens0.AncillaryData.Roll, "Roll is incorrect.");
            Assert.AreEqual(8.9f, ens0.AncillaryData.WaterTemp, "WaterTemp is incorrect.");
            Assert.AreEqual(9.10f, ens0.AncillaryData.SystemTemp, "SystemTemp is incorrect.");
            Assert.AreEqual(10.11f, ens0.AncillaryData.Salinity, "Salinity is incorrect.");
            Assert.AreEqual(11.12f, ens0.AncillaryData.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(12.13f, ens0.AncillaryData.TransducerDepth, "TransducerDepth is incorrect.");
            Assert.AreEqual(13.14f, ens0.AncillaryData.SpeedOfSound, "SpeedOfSound is incorrect.");

            reader.Shutdown();
        }

        /// <summary>
        /// Only the Beam Velocity DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void BeamVelocityDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_BeamVelocityDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = new DataSet.Ensemble();
                EnsembleHelper.AddBeamVelocity(ref ensemble, 30);

                #region Modify the data

                // Modify the data
                ensemble.BeamVelocityData.BeamVelocityData[0, 0] = 2.3f;
                ensemble.BeamVelocityData.BeamVelocityData[0, 1] = 3.4f;
                ensemble.BeamVelocityData.BeamVelocityData[0, 2] = 4.5f;
                ensemble.BeamVelocityData.BeamVelocityData[0, 3] = 5.6f;
                ensemble.BeamVelocityData.BeamVelocityData[1, 0] = 6.7f;
                ensemble.BeamVelocityData.BeamVelocityData[1, 1] = 7.8f;
                ensemble.BeamVelocityData.BeamVelocityData[1, 2] = 8.9f;
                ensemble.BeamVelocityData.BeamVelocityData[1, 3] = 9.10f;
                ensemble.BeamVelocityData.BeamVelocityData[2, 0] = 10.11f;
                ensemble.BeamVelocityData.BeamVelocityData[2, 1] = 11.12f;
                ensemble.BeamVelocityData.BeamVelocityData[2, 2] = 12.13f;

                #endregion
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(false, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            Assert.AreEqual(2.3f, ens0.BeamVelocityData.BeamVelocityData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens0.BeamVelocityData.BeamVelocityData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens0.BeamVelocityData.BeamVelocityData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens0.BeamVelocityData.BeamVelocityData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens0.BeamVelocityData.BeamVelocityData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens0.BeamVelocityData.BeamVelocityData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens0.BeamVelocityData.BeamVelocityData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens0.BeamVelocityData.BeamVelocityData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens0.BeamVelocityData.BeamVelocityData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens0.BeamVelocityData.BeamVelocityData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens0.BeamVelocityData.BeamVelocityData[2, 2], "2,2 Data is incorrect.");


            reader.Shutdown();
        }

        /// <summary>
        /// Only the Instrument Velocity DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void InstrumentVelocityDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_InstrumentVelocityDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = new DataSet.Ensemble();
                EnsembleHelper.AddInstrumentVelocity(ref ensemble, 30);

                #region Modify the data

                // Modify the data
                ensemble.InstrumentVelocityData.InstrumentVelocityData[0, 0] = 2.3f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[0, 1] = 3.4f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[0, 2] = 4.5f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[0, 3] = 5.6f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[1, 0] = 6.7f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[1, 1] = 7.8f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[1, 2] = 8.9f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[1, 3] = 9.10f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[2, 0] = 10.11f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[2, 1] = 11.12f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[2, 2] = 12.13f;

                #endregion
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(false, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            Assert.AreEqual(2.3f, ens0.InstrumentVelocityData.InstrumentVelocityData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens0.InstrumentVelocityData.InstrumentVelocityData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens0.InstrumentVelocityData.InstrumentVelocityData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens0.InstrumentVelocityData.InstrumentVelocityData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens0.InstrumentVelocityData.InstrumentVelocityData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens0.InstrumentVelocityData.InstrumentVelocityData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens0.InstrumentVelocityData.InstrumentVelocityData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens0.InstrumentVelocityData.InstrumentVelocityData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens0.InstrumentVelocityData.InstrumentVelocityData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens0.InstrumentVelocityData.InstrumentVelocityData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens0.InstrumentVelocityData.InstrumentVelocityData[2, 2], "2,2 Data is incorrect.");

            reader.Shutdown();
        }

        /// <summary>
        /// Only the Earth Velocity DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void EarthVelocityDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_EarthVelocityDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = new DataSet.Ensemble();
                EnsembleHelper.AddEarthVelocity(ref ensemble, 30);

                #region Modify the data

                // Modify the data
                ensemble.EarthVelocityData.EarthVelocityData[0, 0] = 2.3f;
                ensemble.EarthVelocityData.EarthVelocityData[0, 1] = 3.4f;
                ensemble.EarthVelocityData.EarthVelocityData[0, 2] = 4.5f;
                ensemble.EarthVelocityData.EarthVelocityData[0, 3] = 5.6f;
                ensemble.EarthVelocityData.EarthVelocityData[1, 0] = 6.7f;
                ensemble.EarthVelocityData.EarthVelocityData[1, 1] = 7.8f;
                ensemble.EarthVelocityData.EarthVelocityData[1, 2] = 8.9f;
                ensemble.EarthVelocityData.EarthVelocityData[1, 3] = 9.10f;
                ensemble.EarthVelocityData.EarthVelocityData[2, 0] = 10.11f;
                ensemble.EarthVelocityData.EarthVelocityData[2, 1] = 11.12f;
                ensemble.EarthVelocityData.EarthVelocityData[2, 2] = 12.13f;

                #endregion
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(false, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            Assert.AreEqual(2.3f, ens0.EarthVelocityData.EarthVelocityData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens0.EarthVelocityData.EarthVelocityData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens0.EarthVelocityData.EarthVelocityData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens0.EarthVelocityData.EarthVelocityData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens0.EarthVelocityData.EarthVelocityData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens0.EarthVelocityData.EarthVelocityData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens0.EarthVelocityData.EarthVelocityData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens0.EarthVelocityData.EarthVelocityData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens0.EarthVelocityData.EarthVelocityData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens0.EarthVelocityData.EarthVelocityData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens0.EarthVelocityData.EarthVelocityData[2, 2], "2,2 Data is incorrect.");

            reader.Shutdown();
        }

        /// <summary>
        /// Only the Correlation DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void CorrelationDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_CorrelationDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = new DataSet.Ensemble();
                EnsembleHelper.AddCorrelation(ref ensemble, 30);

                #region Modify the data

                // Modify the data
                ensemble.CorrelationData.CorrelationData[0, 0] = 2.3f;
                ensemble.CorrelationData.CorrelationData[0, 1] = 3.4f;
                ensemble.CorrelationData.CorrelationData[0, 2] = 4.5f;
                ensemble.CorrelationData.CorrelationData[0, 3] = 5.6f;
                ensemble.CorrelationData.CorrelationData[1, 0] = 6.7f;
                ensemble.CorrelationData.CorrelationData[1, 1] = 7.8f;
                ensemble.CorrelationData.CorrelationData[1, 2] = 8.9f;
                ensemble.CorrelationData.CorrelationData[1, 3] = 9.10f;
                ensemble.CorrelationData.CorrelationData[2, 0] = 10.11f;
                ensemble.CorrelationData.CorrelationData[2, 1] = 11.12f;
                ensemble.CorrelationData.CorrelationData[2, 2] = 12.13f;

                #endregion
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(false, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            Assert.AreEqual(2.3f, ens0.CorrelationData.CorrelationData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens0.CorrelationData.CorrelationData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens0.CorrelationData.CorrelationData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens0.CorrelationData.CorrelationData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens0.CorrelationData.CorrelationData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens0.CorrelationData.CorrelationData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens0.CorrelationData.CorrelationData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens0.CorrelationData.CorrelationData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens0.CorrelationData.CorrelationData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens0.CorrelationData.CorrelationData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens0.CorrelationData.CorrelationData[2, 2], "2,2 Data is incorrect.");

            reader.Shutdown();
        }

        /// <summary>
        /// Only the BottomTrack DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void BottomTrackDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_BottomTrackDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = new DataSet.Ensemble();
                EnsembleHelper.AddBottomTrack(ref ensemble);

                #region Modify the data

                // Modify the data
                ensemble.BottomTrackData.FirstPingTime = 3.4f;
                ensemble.BottomTrackData.LastPingTime = 4.5f;
                ensemble.BottomTrackData.Heading = 5.6f;
                ensemble.BottomTrackData.Pitch = 6.7f;
                ensemble.BottomTrackData.Roll = 7.8f;
                ensemble.BottomTrackData.WaterTemp = 8.9f;
                ensemble.BottomTrackData.SystemTemp = 9.10f;
                ensemble.BottomTrackData.Salinity = 10.11f;
                ensemble.BottomTrackData.Pressure = 11.12f;
                ensemble.BottomTrackData.TransducerDepth = 12.13f;
                ensemble.BottomTrackData.SpeedOfSound = 13.14f;
                ensemble.BottomTrackData.Status = new Status(1);
                ensemble.BottomTrackData.NumBeams = 4;
                ensemble.BottomTrackData.ActualPingCount = 5.66f;
                ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.SNR[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.SNR[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.SNR[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.SNR[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_0_INDEX] = 1;
                ensemble.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_1_INDEX] = 1;
                ensemble.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_2_INDEX] = 1;
                ensemble.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_3_INDEX] = 1;

                ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_0_INDEX] = 1;
                ensemble.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_1_INDEX] = 1;
                ensemble.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_2_INDEX] = 1;
                ensemble.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_3_INDEX] = 1;

                ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_0_INDEX] = 1;
                ensemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_1_INDEX] = 1;
                ensemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_2_INDEX] = 1;
                ensemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_3_INDEX] = 1;

                #endregion
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(false, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            Assert.AreEqual(3.4f, ens0.BottomTrackData.FirstPingTime, "FirstPingTime is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.LastPingTime, "LastPingTime is incorrect.");
            Assert.AreEqual(5.6f, ens0.BottomTrackData.Heading, "Heading is incorrect.");
            Assert.AreEqual(6.7f, ens0.BottomTrackData.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(7.8f, ens0.BottomTrackData.Roll, "Roll is incorrect.");
            Assert.AreEqual(8.9f, ens0.BottomTrackData.WaterTemp, "WaterTemp is incorrect.");
            Assert.AreEqual(9.10f, ens0.BottomTrackData.SystemTemp, "SystemTemp is incorrect.");
            Assert.AreEqual(10.11f, ens0.BottomTrackData.Salinity, "Salinity is incorrect.");
            Assert.AreEqual(11.12f, ens0.BottomTrackData.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(12.13f, ens0.BottomTrackData.TransducerDepth, "TransducerDepth is incorrect.");
            Assert.AreEqual(13.14f, ens0.BottomTrackData.SpeedOfSound, "SpeedOfSound is incorrect.");
            Assert.AreEqual(new Status(1), ens0.BottomTrackData.Status, "Status is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.NumBeams, "NumBeams is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.Range.GetLength(0), "Range Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.SNR.GetLength(0), "SNR Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.Amplitude.GetLength(0), "Amplitude Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.Correlation.GetLength(0), "Correlation Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.BeamVelocity.GetLength(0), "BeamVelocity Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.BeamGood.GetLength(0), "BeamGood Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.InstrumentVelocity.GetLength(0), "InstrumentVelocity Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.InstrumentGood.GetLength(0), "InstrumentGood Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.EarthVelocity.GetLength(0), "EarthVelocity Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.EarthGood.GetLength(0), "EarthGood Length is incorrect.");
            Assert.AreEqual(5.66f, ens0.BottomTrackData.ActualPingCount, "ActualPingCount is incorrect.");
            Assert.AreEqual(1.2f, ens0.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX], "Range 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX], "Range 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX], "Range 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX], "Range 3 is incorrect.");

            Assert.AreEqual(1.2f, ens0.BottomTrackData.SNR[DataSet.Ensemble.BEAM_0_INDEX], "SNR 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.SNR[DataSet.Ensemble.BEAM_1_INDEX], "SNR 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.SNR[DataSet.Ensemble.BEAM_2_INDEX], "SNR 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.SNR[DataSet.Ensemble.BEAM_3_INDEX], "SNR 3 is incorrect.");

            Assert.AreEqual(1.2f, ens0.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_0_INDEX], "Amplitude 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_1_INDEX], "Amplitude 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_2_INDEX], "Amplitude 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_3_INDEX], "Amplitude 3 is incorrect.");

            Assert.AreEqual(1.2f, ens0.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_0_INDEX], "Correlation 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_1_INDEX], "Correlation 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_2_INDEX], "Correlation 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_3_INDEX], "Correlation 3 is incorrect.");

            Assert.AreEqual(1.2f, ens0.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_0_INDEX], "BeamVelocity 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_1_INDEX], "BeamVelocity 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_2_INDEX], "BeamVelocity 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_3_INDEX], "BeamVelocity 3 is incorrect.");

            Assert.AreEqual(1, ens0.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_0_INDEX], "BeamGood 0 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_1_INDEX], "BeamGood 1 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_2_INDEX], "BeamGood 2 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_3_INDEX], "BeamGood 3 is incorrect.");

            Assert.AreEqual(1.2f, ens0.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_0_INDEX], "InstrumentVelocity 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_1_INDEX], "InstrumentVelocity 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_2_INDEX], "InstrumentVelocity 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_3_INDEX], "InstrumentVelocity 3 is incorrect.");

            Assert.AreEqual(1, ens0.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_0_INDEX], "InstrumentGood 0 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_1_INDEX], "InstrumentGood 1 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_2_INDEX], "InstrumentGood 2 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_3_INDEX], "InstrumentGood 3 is incorrect.");


            Assert.AreEqual(1.2f, ens0.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_0_INDEX], "EarthVelocity 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_1_INDEX], "EarthVelocity 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_2_INDEX], "EarthVelocity 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_3_INDEX], "EarthVelocity 3 is incorrect.");

            Assert.AreEqual(1, ens0.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_0_INDEX], "EarthGood 0 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_1_INDEX], "EarthGood 1 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_2_INDEX], "EarthGood 2 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_3_INDEX], "EarthGood 3 is incorrect.");

            reader.Shutdown();
        }

        /// <summary>
        /// Only the Ensemble DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void EnsembleDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_EnsembleDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = new DataSet.Ensemble();
                EnsembleHelper.AddEnsemble(ref ensemble, 30);

                #region Modify the data

                // Modify the data
                ensemble.EnsembleData.EnsembleNumber = x;
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

                #endregion
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(true, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            Assert.AreEqual(0, ens0.EnsembleData.EnsembleNumber, "EnsembleNumber is incorrect.");
            Assert.AreEqual(30, ens0.EnsembleData.NumBins, "NumBins is incorrect.");
            Assert.AreEqual(4, ens0.EnsembleData.NumBeams, "NumBeams is incorrect.");
            Assert.AreEqual(2, ens0.EnsembleData.DesiredPingCount, "DesiredPingCount is incorrect.");
            Assert.AreEqual(2, ens0.EnsembleData.ActualPingCount, "ActualPingCount is incorrect.");
            Assert.AreEqual(new SerialNumber("01300000000000000000000000000001"), ens0.EnsembleData.SysSerialNumber, "SysSerialNumber is incorrect.");
            Assert.AreEqual(1, ens0.EnsembleData.SysSerialNumber.SystemSerialNumber, "Serial number is incorrect.");
            Assert.AreEqual("300000000000000", ens0.EnsembleData.SysSerialNumber.SubSystems, "Subsystems are incorrect.");
            Assert.AreEqual(1, ens0.EnsembleData.SysSerialNumber.SubSystemsDict.Count, "Subsystem count is incorrect.");
            Assert.AreEqual(new Firmware(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, 0, 2, 3), ens0.EnsembleData.SysFirmware, "SysFirmware is incorrect.");
            Assert.AreEqual(0, ens0.EnsembleData.SysFirmware.FirmwareMajor, "Firmware Major is incorrect.");
            Assert.AreEqual(2, ens0.EnsembleData.SysFirmware.FirmwareMinor, "Firmware Minor is incorrect.");
            Assert.AreEqual(3, ens0.EnsembleData.SysFirmware.FirmwareRevision, "Firmware Revision is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, ens0.EnsembleData.SysFirmware.GetSubsystemCode(ens0.EnsembleData.SysSerialNumber), "Firmware Subsystem code is incorrect.");
            Assert.AreEqual(2, ens0.EnsembleData.SubsystemConfig.ConfigNumber, "SubsystemConfig config number is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, ens0.EnsembleData.SubsystemConfig.SubSystem.Code, "SubsystemConfig Subsystem code is incorrect.");
            Assert.AreEqual(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), ens0.EnsembleData.SysFirmware.GetSubsystem(ens0.EnsembleData.SysSerialNumber), "SysFirmware GetSubsystem is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, ens0.EnsembleData.SysFirmware.GetSubsystemCode(ens0.EnsembleData.SysSerialNumber), "Firmware SubsystemCode is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, ens0.EnsembleData.SubsystemConfig.SubSystem.Code, "SubsystemConfig Subystem Code is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, ens0.EnsembleData.EnsDateTime.Year, "EnsDateTime Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, ens0.EnsembleData.Year, "Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, ens0.EnsembleData.EnsDateTime.Month, "EnsDateTime Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, ens0.EnsembleData.Month, "Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, ens0.EnsembleData.EnsDateTime.Day, "EnsDateTime Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, ens0.EnsembleData.Day, "Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, ens0.EnsembleData.EnsDateTime.Hour, "EnsDateTime Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, ens0.EnsembleData.Hour, "Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, ens0.EnsembleData.EnsDateTime.Minute, "EnsDateTime Minute is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, ens0.EnsembleData.Minute, "Minute is incorrect.");
            //Assert.AreEqual(DateTime.Now.Second, ens0.EnsembleData.EnsDateTime.Second, "EnsDateTime Second is incorrect.");
            //Assert.AreEqual(DateTime.Now.Second, ens0.EnsembleData.Second, "Second is incorrect.");
            Assert.AreEqual(new DataSet.UniqueID(ens0.EnsembleData.EnsembleNumber, ens0.EnsembleData.EnsDateTime), ens0.EnsembleData.UniqueId, "UniqueID is incorrect.");

            reader.Shutdown();
        }

        /// <summary>
        /// Only the Nmea DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void NmeaDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_NmeaDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                string nmeaData = "$HEHDT,274.67,T*1F$HEROT,-32.6,A*31$GPGGA,155339.00,3245.44007,N,11719.83271,W,2,09,0.9,-1.1,M,-33.3,M,5.0,0138*50$GPVTG,277.26,T,265.15,M,2.62,N,4.86,K,D*29$GPZDA,155339.00,08,12,2011,00,00*67$GPGSV,3,1,09,02,75,182,50,04,56,053,51,05,08,167,42,09,50,241,48*75$GPGSV,3,2,09,10,24,111,46,12,45,322,47,17,17,063,45,25,15,313,44*71$GPGSV,3,3,09,28,05,121,36,,,,,,,,,,,,*48";

                // Generate an Ensemble
                DataSet.Ensemble ensemble = new DataSet.Ensemble();
                ensemble.AddNmeaData(nmeaData);

                #region Modify the data

                // Modify the data

                #endregion
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(false, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            Assert.AreEqual(new DotSpatial.Positioning.Latitude("32 45.44007").DecimalDegrees, ens0.NmeaData.GPGGA.Position.Latitude.DecimalDegrees, 0.0001, "Latitude is not correct");
            Assert.AreEqual(new DotSpatial.Positioning.Latitude("-117 19.83271").DecimalDegrees, ens0.NmeaData.GPGGA.Position.Longitude.DecimalDegrees, 0.0001, "Longitude is not correct");
            Assert.AreEqual(DotSpatial.Positioning.FixQuality.DifferentialGpsFix, ens0.NmeaData.GPGGA.FixQuality, "Fix Quality is not correct");
            Assert.AreEqual(9, ens0.NmeaData.GPGGA.FixedSatelliteCount, "Number of fixed satellites is incorrect.");
            Assert.AreEqual(new DotSpatial.Positioning.Distance(-1.1, DotSpatial.Positioning.DistanceUnit.Meters).Value, ens0.NmeaData.GPGGA.Altitude.Value, 0.00001, "Altitude is not correct");

            Assert.AreEqual(new DotSpatial.Positioning.Azimuth(277.26), ens0.NmeaData.GPVTG.Bearing, "True Track Made Good Bearing not correct.");
            Assert.AreEqual(new DotSpatial.Positioning.Speed(2.62, DotSpatial.Positioning.SpeedUnit.Knots), ens0.NmeaData.GPVTG.Speed, "Speed is not correct.");

            reader.Shutdown();
        }

        /// <summary>
        /// Only the Good Beam DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void GoodBeamDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_GoodBeamDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = new DataSet.Ensemble();
                EnsembleHelper.AddGoodBeam(ref ensemble, 30);

                #region Modify the data

                // Modify the data
                ensemble.GoodBeamData.GoodBeamData[0, 0] = 2;
                ensemble.GoodBeamData.GoodBeamData[0, 1] = 3;
                ensemble.GoodBeamData.GoodBeamData[0, 2] = 4;
                ensemble.GoodBeamData.GoodBeamData[0, 3] = 5;
                ensemble.GoodBeamData.GoodBeamData[1, 0] = 6;
                ensemble.GoodBeamData.GoodBeamData[1, 1] = 7;
                ensemble.GoodBeamData.GoodBeamData[1, 2] = 8;
                ensemble.GoodBeamData.GoodBeamData[1, 3] = 9;
                ensemble.GoodBeamData.GoodBeamData[2, 0] = 10;
                ensemble.GoodBeamData.GoodBeamData[2, 1] = 11;
                ensemble.GoodBeamData.GoodBeamData[2, 2] = 12;

                #endregion
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(false, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            Assert.AreEqual(2, ens0.GoodBeamData.GoodBeamData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3, ens0.GoodBeamData.GoodBeamData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4, ens0.GoodBeamData.GoodBeamData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5, ens0.GoodBeamData.GoodBeamData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6, ens0.GoodBeamData.GoodBeamData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7, ens0.GoodBeamData.GoodBeamData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8, ens0.GoodBeamData.GoodBeamData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9, ens0.GoodBeamData.GoodBeamData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10, ens0.GoodBeamData.GoodBeamData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11, ens0.GoodBeamData.GoodBeamData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12, ens0.GoodBeamData.GoodBeamData[2, 2], "2,2 Data is incorrect.");

            reader.Shutdown();
        }

        /// <summary>
        /// Only the Good Earth DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void GoodEarthDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_GoodEarthDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = new DataSet.Ensemble();
                EnsembleHelper.AddGoodEarth(ref ensemble, 30);

                #region Modify the data

                // Modify the data
                ensemble.GoodEarthData.GoodEarthData[0, 0] = 2;
                ensemble.GoodEarthData.GoodEarthData[0, 1] = 3;
                ensemble.GoodEarthData.GoodEarthData[0, 2] = 4;
                ensemble.GoodEarthData.GoodEarthData[0, 3] = 5;
                ensemble.GoodEarthData.GoodEarthData[1, 0] = 6;
                ensemble.GoodEarthData.GoodEarthData[1, 1] = 7;
                ensemble.GoodEarthData.GoodEarthData[1, 2] = 8;
                ensemble.GoodEarthData.GoodEarthData[1, 3] = 9;
                ensemble.GoodEarthData.GoodEarthData[2, 0] = 10;
                ensemble.GoodEarthData.GoodEarthData[2, 1] = 11;
                ensemble.GoodEarthData.GoodEarthData[2, 2] = 12;

                #endregion
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(false, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            Assert.AreEqual(2, ens0.GoodEarthData.GoodEarthData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3, ens0.GoodEarthData.GoodEarthData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4, ens0.GoodEarthData.GoodEarthData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5, ens0.GoodEarthData.GoodEarthData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6, ens0.GoodEarthData.GoodEarthData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7, ens0.GoodEarthData.GoodEarthData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8, ens0.GoodEarthData.GoodEarthData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9, ens0.GoodEarthData.GoodEarthData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10, ens0.GoodEarthData.GoodEarthData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11, ens0.GoodEarthData.GoodEarthData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12, ens0.GoodEarthData.GoodEarthData[2, 2], "2,2 Data is incorrect.");

            reader.Shutdown();
        }

        /// <summary>
        /// Only the Instrument Water Mass DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void InstrumentWaterMassDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_InstrumentWaterMassDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = new DataSet.Ensemble();

                #region Modify the data

                // Modify the data
                // Create Sentence
                string nmea = "$PRTI01,379550,1,1468,-99999,-99999,-99999,0,300,-200,400,100,0004*20";
                Prti01Sentence sent = new Prti01Sentence(nmea);
                ensemble.AddInstrumentWaterMassData(sent);

                Assert.AreEqual(true, sent.IsValid, "NMEA sentence incorrect");

                #endregion
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(false, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            Assert.AreEqual((new DotSpatial.Positioning.Speed(300, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, ens0.InstrumentWaterMassData.VelocityX, 0.00001, "Instrument Water Mass East not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Speed(-200, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, ens0.InstrumentWaterMassData.VelocityY, 0.00001, "Instrument Water Mass North not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Speed(400, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, ens0.InstrumentWaterMassData.VelocityZ, 0.00001, "Instrument Water Mass Vertical not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Distance(100, DotSpatial.Positioning.DistanceUnit.Millimeters)).ToMeters().Value, ens0.InstrumentWaterMassData.WaterMassDepthLayer, 0.00001, "Instrument Water Mass Depth Layer not properly set.");

            reader.Shutdown();
        }


        /// <summary>
        /// Only the Earth Water Mass DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void EarthWaterMassDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_EarthWaterMassDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = new DataSet.Ensemble();

                #region Modify the data

                // Modify the data
                // Create Sentence
                string nmea = "$PRTI02,379550,1,1468,-99999,-99999,-99999,0,300,-200,400,100,0004*23";
                Prti02Sentence sent = new Prti02Sentence(nmea);
                ensemble.AddEarthWaterMassData(sent);

                Assert.AreEqual(true, sent.IsValid, "NMEA sentence incorrect");

                #endregion
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(false, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(false, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            Assert.AreEqual((new DotSpatial.Positioning.Speed(300, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, ens0.EarthWaterMassData.VelocityEast, 0.00001, "Earth Water Mass East not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Speed(-200, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, ens0.EarthWaterMassData.VelocityNorth, 0.00001, "Earth Water Mass North not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Speed(400, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, ens0.EarthWaterMassData.VelocityVertical, 0.00001, "Earth Water Mass Vertical not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Distance(100, DotSpatial.Positioning.DistanceUnit.Millimeters)).ToMeters().Value, ens0.EarthWaterMassData.WaterMassDepthLayer, 0.00001, "Earth Water Mass Depth Layer not properly set.");

            reader.Shutdown();
        }

        /// <summary>
        /// All DataSet.
        /// 
        /// Test how long it takes to read in 1000 entries from 
        /// the database.  This will first create a database and then write
        /// 1000 entries.  It will then read them all back and verify every
        /// entry read is correct.
        /// </summary>
        [Test]
        public void AllDataSetOnly()
        {

            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test AdcpDatabaseReaderTest_AllDataSetOnly";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            #region Write Ensembles
            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
            writer.SelectedProject = p;

            for (int x = 0; x < 1000; x++)
            {
                // Generate an Ensemble
                DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);

                #region Modify the data

                #region Ensemble
                ensemble.EnsembleData.EnsembleNumber = x;
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
                #endregion

                #region Amplitude
                ensemble.AmplitudeData.AmplitudeData[0, 0] = 2.3f;
                ensemble.AmplitudeData.AmplitudeData[0, 1] = 3.4f;
                ensemble.AmplitudeData.AmplitudeData[0, 2] = 4.5f;
                ensemble.AmplitudeData.AmplitudeData[0, 3] = 5.6f;
                ensemble.AmplitudeData.AmplitudeData[1, 0] = 6.7f;
                ensemble.AmplitudeData.AmplitudeData[1, 1] = 7.8f;
                ensemble.AmplitudeData.AmplitudeData[1, 2] = 8.9f;
                ensemble.AmplitudeData.AmplitudeData[1, 3] = 9.10f;
                ensemble.AmplitudeData.AmplitudeData[2, 0] = 10.11f;
                ensemble.AmplitudeData.AmplitudeData[2, 1] = 11.12f;
                ensemble.AmplitudeData.AmplitudeData[2, 2] = 12.13f;
                #endregion

                #region Correlation
                ensemble.CorrelationData.CorrelationData[0, 0] = 2.3f;
                ensemble.CorrelationData.CorrelationData[0, 1] = 3.4f;
                ensemble.CorrelationData.CorrelationData[0, 2] = 4.5f;
                ensemble.CorrelationData.CorrelationData[0, 3] = 5.6f;
                ensemble.CorrelationData.CorrelationData[1, 0] = 6.7f;
                ensemble.CorrelationData.CorrelationData[1, 1] = 7.8f;
                ensemble.CorrelationData.CorrelationData[1, 2] = 8.9f;
                ensemble.CorrelationData.CorrelationData[1, 3] = 9.10f;
                ensemble.CorrelationData.CorrelationData[2, 0] = 10.11f;
                ensemble.CorrelationData.CorrelationData[2, 1] = 11.12f;
                ensemble.CorrelationData.CorrelationData[2, 2] = 12.13f;
                #endregion

                #region Ancillary
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
                #endregion

                #region Beam Velocity
                ensemble.BeamVelocityData.BeamVelocityData[0, 0] = 2.3f;
                ensemble.BeamVelocityData.BeamVelocityData[0, 1] = 3.4f;
                ensemble.BeamVelocityData.BeamVelocityData[0, 2] = 4.5f;
                ensemble.BeamVelocityData.BeamVelocityData[0, 3] = 5.6f;
                ensemble.BeamVelocityData.BeamVelocityData[1, 0] = 6.7f;
                ensemble.BeamVelocityData.BeamVelocityData[1, 1] = 7.8f;
                ensemble.BeamVelocityData.BeamVelocityData[1, 2] = 8.9f;
                ensemble.BeamVelocityData.BeamVelocityData[1, 3] = 9.10f;
                ensemble.BeamVelocityData.BeamVelocityData[2, 0] = 10.11f;
                ensemble.BeamVelocityData.BeamVelocityData[2, 1] = 11.12f;
                ensemble.BeamVelocityData.BeamVelocityData[2, 2] = 12.13f;
                #endregion

                #region Earth Velocity
                ensemble.EarthVelocityData.EarthVelocityData[0, 0] = 2.3f;
                ensemble.EarthVelocityData.EarthVelocityData[0, 1] = 3.4f;
                ensemble.EarthVelocityData.EarthVelocityData[0, 2] = 4.5f;
                ensemble.EarthVelocityData.EarthVelocityData[0, 3] = 5.6f;
                ensemble.EarthVelocityData.EarthVelocityData[1, 0] = 6.7f;
                ensemble.EarthVelocityData.EarthVelocityData[1, 1] = 7.8f;
                ensemble.EarthVelocityData.EarthVelocityData[1, 2] = 8.9f;
                ensemble.EarthVelocityData.EarthVelocityData[1, 3] = 9.10f;
                ensemble.EarthVelocityData.EarthVelocityData[2, 0] = 10.11f;
                ensemble.EarthVelocityData.EarthVelocityData[2, 1] = 11.12f;
                ensemble.EarthVelocityData.EarthVelocityData[2, 2] = 12.13f;
                #endregion

                #region Instrument Velocity
                ensemble.InstrumentVelocityData.InstrumentVelocityData[0, 0] = 2.3f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[0, 1] = 3.4f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[0, 2] = 4.5f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[0, 3] = 5.6f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[1, 0] = 6.7f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[1, 1] = 7.8f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[1, 2] = 8.9f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[1, 3] = 9.10f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[2, 0] = 10.11f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[2, 1] = 11.12f;
                ensemble.InstrumentVelocityData.InstrumentVelocityData[2, 2] = 12.13f;
                #endregion

                #region Good Beam
                ensemble.GoodBeamData.GoodBeamData[0, 0] = 2;
                ensemble.GoodBeamData.GoodBeamData[0, 1] = 3;
                ensemble.GoodBeamData.GoodBeamData[0, 2] = 4;
                ensemble.GoodBeamData.GoodBeamData[0, 3] = 5;
                ensemble.GoodBeamData.GoodBeamData[1, 0] = 6;
                ensemble.GoodBeamData.GoodBeamData[1, 1] = 7;
                ensemble.GoodBeamData.GoodBeamData[1, 2] = 8;
                ensemble.GoodBeamData.GoodBeamData[1, 3] = 9;
                ensemble.GoodBeamData.GoodBeamData[2, 0] = 10;
                ensemble.GoodBeamData.GoodBeamData[2, 1] = 11;
                ensemble.GoodBeamData.GoodBeamData[2, 2] = 12;
                #endregion

                #region Good Earth
                ensemble.GoodEarthData.GoodEarthData[0, 0] = 2;
                ensemble.GoodEarthData.GoodEarthData[0, 1] = 3;
                ensemble.GoodEarthData.GoodEarthData[0, 2] = 4;
                ensemble.GoodEarthData.GoodEarthData[0, 3] = 5;
                ensemble.GoodEarthData.GoodEarthData[1, 0] = 6;
                ensemble.GoodEarthData.GoodEarthData[1, 1] = 7;
                ensemble.GoodEarthData.GoodEarthData[1, 2] = 8;
                ensemble.GoodEarthData.GoodEarthData[1, 3] = 9;
                ensemble.GoodEarthData.GoodEarthData[2, 0] = 10;
                ensemble.GoodEarthData.GoodEarthData[2, 1] = 11;
                ensemble.GoodEarthData.GoodEarthData[2, 2] = 12;
                #endregion

                #region Bottom Track
                ensemble.BottomTrackData.FirstPingTime = 3.4f;
                ensemble.BottomTrackData.LastPingTime = 4.5f;
                ensemble.BottomTrackData.Heading = 5.6f;
                ensemble.BottomTrackData.Pitch = 6.7f;
                ensemble.BottomTrackData.Roll = 7.8f;
                ensemble.BottomTrackData.WaterTemp = 8.9f;
                ensemble.BottomTrackData.SystemTemp = 9.10f;
                ensemble.BottomTrackData.Salinity = 10.11f;
                ensemble.BottomTrackData.Pressure = 11.12f;
                ensemble.BottomTrackData.TransducerDepth = 12.13f;
                ensemble.BottomTrackData.SpeedOfSound = 13.14f;
                ensemble.BottomTrackData.Status = new Status(1);
                ensemble.BottomTrackData.NumBeams = 4;
                ensemble.BottomTrackData.ActualPingCount = 5.66f;
                ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.SNR[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.SNR[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.SNR[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.SNR[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_0_INDEX] = 1;
                ensemble.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_1_INDEX] = 1;
                ensemble.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_2_INDEX] = 1;
                ensemble.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_3_INDEX] = 1;

                ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_0_INDEX] = 1;
                ensemble.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_1_INDEX] = 1;
                ensemble.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_2_INDEX] = 1;
                ensemble.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_3_INDEX] = 1;

                ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
                ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
                ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
                ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

                ensemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_0_INDEX] = 1;
                ensemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_1_INDEX] = 1;
                ensemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_2_INDEX] = 1;
                ensemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_3_INDEX] = 1;

                #endregion

                #region NMEA
                // Generate an Ensemble
                string nmeaData2 = "$HEHDT,274.67,T*1F$HEROT,-32.6,A*31$GPGGA,155339.00,3245.44007,N,11719.83271,W,2,09,0.9,-1.1,M,-33.3,M,5.0,0138*50$GPVTG,277.26,T,265.15,M,2.62,N,4.86,K,D*29$GPZDA,155339.00,08,12,2011,00,00*67$GPGSV,3,1,09,02,75,182,50,04,56,053,51,05,08,167,42,09,50,241,48*75$GPGSV,3,2,09,10,24,111,46,12,45,322,47,17,17,063,45,25,15,313,44*71$GPGSV,3,3,09,28,05,121,36,,,,,,,,,,,,*48";
                ensemble.AddNmeaData(nmeaData2);
                #endregion

                #region Instrument Water Mass
                // Create Sentence
                string nmea1 = "$PRTI01,379550,1,1468,-99999,-99999,-99999,0,300,-200,400,100,0004*20";
                Prti01Sentence sent1 = new Prti01Sentence(nmea1);
                ensemble.AddInstrumentWaterMassData(sent1);

                Assert.AreEqual(true, sent1.IsValid, "NMEA sentence incorrect");
                #endregion

                #region Earth Water Mass Data
                // Modify the data
                // Create Sentence
                string nmea = "$PRTI02,379550,1,1468,-99999,-99999,-99999,0,300,-200,400,100,0004*23";
                Prti02Sentence sent = new Prti02Sentence(nmea);
                ensemble.AddEarthWaterMassData(sent);

                Assert.AreEqual(true, sent.IsValid, "NMEA sentence incorrect");
                #endregion

                #endregion
                writer.AddIncomingData(ensemble);
            }
            writer.Flush();
            #endregion

            writer.Shutdown();

            AdcpDatabaseReader reader = new AdcpDatabaseReader();
            DataSet.Ensemble ens0 = reader.GetEnsemble(p, 1);

            Assert.IsNotNull(ens0, "Ensemble is incorrect.");
            Assert.AreEqual(true, ens0.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsAncillaryAvail, "IsAncillaryAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsEarthWaterMassAvail, "IsEarthWaterMassAvail is incorrect.");
            Assert.AreEqual(true, ens0.IsNmeaAvail, "IsNmeaAvail is incorrect.");

            #region Ensemble
            Assert.AreEqual(0, ens0.EnsembleData.EnsembleNumber, "EnsembleNumber is incorrect.");
            Assert.AreEqual(30, ens0.EnsembleData.NumBins, "NumBins is incorrect.");
            Assert.AreEqual(4, ens0.EnsembleData.NumBeams, "NumBeams is incorrect.");
            Assert.AreEqual(2, ens0.EnsembleData.DesiredPingCount, "DesiredPingCount is incorrect.");
            Assert.AreEqual(2, ens0.EnsembleData.ActualPingCount, "ActualPingCount is incorrect.");
            Assert.AreEqual(new SerialNumber("01300000000000000000000000000001"), ens0.EnsembleData.SysSerialNumber, "SysSerialNumber is incorrect.");
            Assert.AreEqual(1, ens0.EnsembleData.SysSerialNumber.SystemSerialNumber, "Serial number is incorrect.");
            Assert.AreEqual("300000000000000", ens0.EnsembleData.SysSerialNumber.SubSystems, "Subsystems are incorrect.");
            Assert.AreEqual(1, ens0.EnsembleData.SysSerialNumber.SubSystemsDict.Count, "Subsystem count is incorrect.");
            Assert.AreEqual(new Firmware(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, 0, 2, 3), ens0.EnsembleData.SysFirmware, "SysFirmware is incorrect.");
            Assert.AreEqual(0, ens0.EnsembleData.SysFirmware.FirmwareMajor, "Firmware Major is incorrect.");
            Assert.AreEqual(2, ens0.EnsembleData.SysFirmware.FirmwareMinor, "Firmware Minor is incorrect.");
            Assert.AreEqual(3, ens0.EnsembleData.SysFirmware.FirmwareRevision, "Firmware Revision is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, ens0.EnsembleData.SysFirmware.GetSubsystemCode(ens0.EnsembleData.SysSerialNumber), "Firmware Subsystem code is incorrect.");
            Assert.AreEqual(2, ens0.EnsembleData.SubsystemConfig.ConfigNumber, "SubsystemConfig config number is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, ens0.EnsembleData.SubsystemConfig.SubSystem.Code, "SubsystemConfig Subsystem code is incorrect.");
            Assert.AreEqual(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), ens0.EnsembleData.SysFirmware.GetSubsystem(ens0.EnsembleData.SysSerialNumber), "SysFirmware GetSubsystem is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, ens0.EnsembleData.SysFirmware.GetSubsystemCode(ens0.EnsembleData.SysSerialNumber), "Firmware SubsystemCode is incorrect.");
            Assert.AreEqual(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, ens0.EnsembleData.SubsystemConfig.SubSystem.Code, "SubsystemConfig Subystem Code is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, ens0.EnsembleData.EnsDateTime.Year, "EnsDateTime Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, ens0.EnsembleData.Year, "Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, ens0.EnsembleData.EnsDateTime.Month, "EnsDateTime Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, ens0.EnsembleData.Month, "Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, ens0.EnsembleData.EnsDateTime.Day, "EnsDateTime Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, ens0.EnsembleData.Day, "Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, ens0.EnsembleData.EnsDateTime.Hour, "EnsDateTime Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, ens0.EnsembleData.Hour, "Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, ens0.EnsembleData.EnsDateTime.Minute, "EnsDateTime Minute is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, ens0.EnsembleData.Minute, "Minute is incorrect.");
            //Assert.AreEqual(DateTime.Now.Second, ens0.EnsembleData.EnsDateTime.Second, "EnsDateTime Second is incorrect.");
            //Assert.AreEqual(DateTime.Now.Second, ens0.EnsembleData.Second, "Second is incorrect.");
            Assert.AreEqual(new DataSet.UniqueID(ens0.EnsembleData.EnsembleNumber, ens0.EnsembleData.EnsDateTime), ens0.EnsembleData.UniqueId, "UniqueID is incorrect.");
            #endregion

            #region Amplitude
            Assert.AreEqual(2.3f, ens0.AmplitudeData.AmplitudeData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens0.AmplitudeData.AmplitudeData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens0.AmplitudeData.AmplitudeData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens0.AmplitudeData.AmplitudeData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens0.AmplitudeData.AmplitudeData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens0.AmplitudeData.AmplitudeData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens0.AmplitudeData.AmplitudeData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens0.AmplitudeData.AmplitudeData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens0.AmplitudeData.AmplitudeData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens0.AmplitudeData.AmplitudeData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens0.AmplitudeData.AmplitudeData[2, 2], "2,2 Data is incorrect.");
            #endregion

            #region Correlation
            Assert.AreEqual(2.3f, ens0.CorrelationData.CorrelationData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens0.CorrelationData.CorrelationData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens0.CorrelationData.CorrelationData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens0.CorrelationData.CorrelationData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens0.CorrelationData.CorrelationData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens0.CorrelationData.CorrelationData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens0.CorrelationData.CorrelationData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens0.CorrelationData.CorrelationData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens0.CorrelationData.CorrelationData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens0.CorrelationData.CorrelationData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens0.CorrelationData.CorrelationData[2, 2], "2,2 Data is incorrect.");
            #endregion

            #region Ancillary
            Assert.AreEqual(1.2f, ens0.AncillaryData.FirstBinRange, "First Bin Range is incorrect.");
            Assert.AreEqual(2.3f, ens0.AncillaryData.BinSize, "BinSize is incorrect.");
            Assert.AreEqual(3.4f, ens0.AncillaryData.FirstPingTime, "FirstPingTime is incorrect.");
            Assert.AreEqual(4.5f, ens0.AncillaryData.LastPingTime, "LastPingTime is incorrect.");
            Assert.AreEqual(5.6f, ens0.AncillaryData.Heading, "Heading is incorrect.");
            Assert.AreEqual(6.7f, ens0.AncillaryData.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(7.8f, ens0.AncillaryData.Roll, "Roll is incorrect.");
            Assert.AreEqual(8.9f, ens0.AncillaryData.WaterTemp, "WaterTemp is incorrect.");
            Assert.AreEqual(9.10f, ens0.AncillaryData.SystemTemp, "SystemTemp is incorrect.");
            Assert.AreEqual(10.11f, ens0.AncillaryData.Salinity, "Salinity is incorrect.");
            Assert.AreEqual(11.12f, ens0.AncillaryData.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(12.13f, ens0.AncillaryData.TransducerDepth, "TransducerDepth is incorrect.");
            Assert.AreEqual(13.14f, ens0.AncillaryData.SpeedOfSound, "SpeedOfSound is incorrect.");
            #endregion

            #region Beam Velocity
            Assert.AreEqual(2.3f, ens0.BeamVelocityData.BeamVelocityData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens0.BeamVelocityData.BeamVelocityData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens0.BeamVelocityData.BeamVelocityData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens0.BeamVelocityData.BeamVelocityData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens0.BeamVelocityData.BeamVelocityData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens0.BeamVelocityData.BeamVelocityData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens0.BeamVelocityData.BeamVelocityData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens0.BeamVelocityData.BeamVelocityData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens0.BeamVelocityData.BeamVelocityData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens0.BeamVelocityData.BeamVelocityData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens0.BeamVelocityData.BeamVelocityData[2, 2], "2,2 Data is incorrect.");
            #endregion

            #region Earth Velocity
            Assert.AreEqual(2.3f, ens0.EarthVelocityData.EarthVelocityData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens0.EarthVelocityData.EarthVelocityData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens0.EarthVelocityData.EarthVelocityData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens0.EarthVelocityData.EarthVelocityData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens0.EarthVelocityData.EarthVelocityData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens0.EarthVelocityData.EarthVelocityData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens0.EarthVelocityData.EarthVelocityData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens0.EarthVelocityData.EarthVelocityData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens0.EarthVelocityData.EarthVelocityData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens0.EarthVelocityData.EarthVelocityData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens0.EarthVelocityData.EarthVelocityData[2, 2], "2,2 Data is incorrect.");
            #endregion

            #region Instrument Velocity
            Assert.AreEqual(2.3f, ens0.InstrumentVelocityData.InstrumentVelocityData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens0.InstrumentVelocityData.InstrumentVelocityData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens0.InstrumentVelocityData.InstrumentVelocityData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens0.InstrumentVelocityData.InstrumentVelocityData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens0.InstrumentVelocityData.InstrumentVelocityData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens0.InstrumentVelocityData.InstrumentVelocityData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens0.InstrumentVelocityData.InstrumentVelocityData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens0.InstrumentVelocityData.InstrumentVelocityData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens0.InstrumentVelocityData.InstrumentVelocityData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens0.InstrumentVelocityData.InstrumentVelocityData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens0.InstrumentVelocityData.InstrumentVelocityData[2, 2], "2,2 Data is incorrect.");
            #endregion

            #region Good Beam
            Assert.AreEqual(2, ens0.GoodBeamData.GoodBeamData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3, ens0.GoodBeamData.GoodBeamData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4, ens0.GoodBeamData.GoodBeamData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5, ens0.GoodBeamData.GoodBeamData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6, ens0.GoodBeamData.GoodBeamData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7, ens0.GoodBeamData.GoodBeamData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8, ens0.GoodBeamData.GoodBeamData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9, ens0.GoodBeamData.GoodBeamData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10, ens0.GoodBeamData.GoodBeamData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11, ens0.GoodBeamData.GoodBeamData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12, ens0.GoodBeamData.GoodBeamData[2, 2], "2,2 Data is incorrect.");
            #endregion

            #region Good Earth
            Assert.AreEqual(2, ens0.GoodEarthData.GoodEarthData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3, ens0.GoodEarthData.GoodEarthData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4, ens0.GoodEarthData.GoodEarthData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5, ens0.GoodEarthData.GoodEarthData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6, ens0.GoodEarthData.GoodEarthData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7, ens0.GoodEarthData.GoodEarthData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8, ens0.GoodEarthData.GoodEarthData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9, ens0.GoodEarthData.GoodEarthData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10, ens0.GoodEarthData.GoodEarthData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11, ens0.GoodEarthData.GoodEarthData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12, ens0.GoodEarthData.GoodEarthData[2, 2], "2,2 Data is incorrect.");
            #endregion

            #region Bottom Track
            Assert.AreEqual(3.4f, ens0.BottomTrackData.FirstPingTime, "FirstPingTime is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.LastPingTime, "LastPingTime is incorrect.");
            Assert.AreEqual(5.6f, ens0.BottomTrackData.Heading, "Heading is incorrect.");
            Assert.AreEqual(6.7f, ens0.BottomTrackData.Pitch, "Pitch is incorrect.");
            Assert.AreEqual(7.8f, ens0.BottomTrackData.Roll, "Roll is incorrect.");
            Assert.AreEqual(8.9f, ens0.BottomTrackData.WaterTemp, "WaterTemp is incorrect.");
            Assert.AreEqual(9.10f, ens0.BottomTrackData.SystemTemp, "SystemTemp is incorrect.");
            Assert.AreEqual(10.11f, ens0.BottomTrackData.Salinity, "Salinity is incorrect.");
            Assert.AreEqual(11.12f, ens0.BottomTrackData.Pressure, "Pressure is incorrect.");
            Assert.AreEqual(12.13f, ens0.BottomTrackData.TransducerDepth, "TransducerDepth is incorrect.");
            Assert.AreEqual(13.14f, ens0.BottomTrackData.SpeedOfSound, "SpeedOfSound is incorrect.");
            Assert.AreEqual(new Status(1), ens0.BottomTrackData.Status, "Status is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.NumBeams, "NumBeams is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.Range.GetLength(0), "Range Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.SNR.GetLength(0), "SNR Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.Amplitude.GetLength(0), "Amplitude Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.Correlation.GetLength(0), "Correlation Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.BeamVelocity.GetLength(0), "BeamVelocity Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.BeamGood.GetLength(0), "BeamGood Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.InstrumentVelocity.GetLength(0), "InstrumentVelocity Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.InstrumentGood.GetLength(0), "InstrumentGood Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.EarthVelocity.GetLength(0), "EarthVelocity Length is incorrect.");
            Assert.AreEqual(4, ens0.BottomTrackData.EarthGood.GetLength(0), "EarthGood Length is incorrect.");
            Assert.AreEqual(5.66f, ens0.BottomTrackData.ActualPingCount, "ActualPingCount is incorrect.");
            Assert.AreEqual(1.2f, ens0.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX], "Range 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX], "Range 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX], "Range 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX], "Range 3 is incorrect.");

            Assert.AreEqual(1.2f, ens0.BottomTrackData.SNR[DataSet.Ensemble.BEAM_0_INDEX], "SNR 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.SNR[DataSet.Ensemble.BEAM_1_INDEX], "SNR 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.SNR[DataSet.Ensemble.BEAM_2_INDEX], "SNR 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.SNR[DataSet.Ensemble.BEAM_3_INDEX], "SNR 3 is incorrect.");

            Assert.AreEqual(1.2f, ens0.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_0_INDEX], "Amplitude 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_1_INDEX], "Amplitude 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_2_INDEX], "Amplitude 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_3_INDEX], "Amplitude 3 is incorrect.");

            Assert.AreEqual(1.2f, ens0.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_0_INDEX], "Correlation 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_1_INDEX], "Correlation 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_2_INDEX], "Correlation 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_3_INDEX], "Correlation 3 is incorrect.");

            Assert.AreEqual(1.2f, ens0.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_0_INDEX], "BeamVelocity 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_1_INDEX], "BeamVelocity 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_2_INDEX], "BeamVelocity 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_3_INDEX], "BeamVelocity 3 is incorrect.");

            Assert.AreEqual(1, ens0.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_0_INDEX], "BeamGood 0 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_1_INDEX], "BeamGood 1 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_2_INDEX], "BeamGood 2 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_3_INDEX], "BeamGood 3 is incorrect.");

            Assert.AreEqual(1.2f, ens0.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_0_INDEX], "InstrumentVelocity 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_1_INDEX], "InstrumentVelocity 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_2_INDEX], "InstrumentVelocity 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_3_INDEX], "InstrumentVelocity 3 is incorrect.");

            Assert.AreEqual(1, ens0.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_0_INDEX], "InstrumentGood 0 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_1_INDEX], "InstrumentGood 1 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_2_INDEX], "InstrumentGood 2 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_3_INDEX], "InstrumentGood 3 is incorrect.");


            Assert.AreEqual(1.2f, ens0.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_0_INDEX], "EarthVelocity 0 is incorrect.");
            Assert.AreEqual(2.3f, ens0.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_1_INDEX], "EarthVelocity 1 is incorrect.");
            Assert.AreEqual(3.4f, ens0.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_2_INDEX], "EarthVelocity 2 is incorrect.");
            Assert.AreEqual(4.5f, ens0.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_3_INDEX], "EarthVelocity 3 is incorrect.");

            Assert.AreEqual(1, ens0.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_0_INDEX], "EarthGood 0 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_1_INDEX], "EarthGood 1 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_2_INDEX], "EarthGood 2 is incorrect.");
            Assert.AreEqual(1, ens0.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_3_INDEX], "EarthGood 3 is incorrect.");
            #endregion

            #region NMEA
            Assert.AreEqual(new DotSpatial.Positioning.Latitude("32 45.44007").DecimalDegrees, ens0.NmeaData.GPGGA.Position.Latitude.DecimalDegrees, 0.0001, "Latitude is not correct");
            Assert.AreEqual(new DotSpatial.Positioning.Latitude("-117 19.83271").DecimalDegrees, ens0.NmeaData.GPGGA.Position.Longitude.DecimalDegrees, 0.0001, "Longitude is not correct");
            Assert.AreEqual(DotSpatial.Positioning.FixQuality.DifferentialGpsFix, ens0.NmeaData.GPGGA.FixQuality, "Fix Quality is not correct");
            Assert.AreEqual(9, ens0.NmeaData.GPGGA.FixedSatelliteCount, "Number of fixed satellites is incorrect.");
            Assert.AreEqual(new DotSpatial.Positioning.Distance(-1.1, DotSpatial.Positioning.DistanceUnit.Meters).Value, ens0.NmeaData.GPGGA.Altitude.Value, 0.00001, "Altitude is not correct");

            Assert.AreEqual(new DotSpatial.Positioning.Azimuth(277.26), ens0.NmeaData.GPVTG.Bearing, "True Track Made Good Bearing not correct.");
            Assert.AreEqual(new DotSpatial.Positioning.Speed(2.62, DotSpatial.Positioning.SpeedUnit.Knots), ens0.NmeaData.GPVTG.Speed, "Speed is not correct.");
            #endregion

            #region Instrument Water Mass
            Assert.AreEqual((new DotSpatial.Positioning.Speed(300, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, ens0.InstrumentWaterMassData.VelocityX, 0.00001, "Instrument Water Mass East not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Speed(-200, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, ens0.InstrumentWaterMassData.VelocityY, 0.00001, "Instrument Water Mass North not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Speed(400, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, ens0.InstrumentWaterMassData.VelocityZ, 0.00001, "Instrument Water Mass Vertical not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Distance(100, DotSpatial.Positioning.DistanceUnit.Millimeters)).ToMeters().Value, ens0.InstrumentWaterMassData.WaterMassDepthLayer, 0.00001, "Instrument Water Mass Depth Layer not properly set.");
            #endregion

            #region Earth Water Mass
            Assert.AreEqual((new DotSpatial.Positioning.Speed(300, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, ens0.EarthWaterMassData.VelocityEast, 0.00001, "Earth Water Mass East not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Speed(-200, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, ens0.EarthWaterMassData.VelocityNorth, 0.00001, "Earth Water Mass North not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Speed(400, DotSpatial.Positioning.SpeedUnit.MillimetersPerSecond)).ToMetersPerSecond().Value, ens0.EarthWaterMassData.VelocityVertical, 0.00001, "Earth Water Mass Vertical not properly set.");
            Assert.AreEqual((new DotSpatial.Positioning.Distance(100, DotSpatial.Positioning.DistanceUnit.Millimeters)).ToMeters().Value, ens0.EarthWaterMassData.WaterMassDepthLayer, 0.00001, "Earth Water Mass Depth Layer not properly set.");
            #endregion


            reader.Shutdown();
        }

    }
}
