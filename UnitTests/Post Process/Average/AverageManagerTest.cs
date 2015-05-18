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
 * 01/08/2013      RC          2.17       Initial coding
 * 
 * 
 */
namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    /// <summary>
    /// Test the AverageCorrelationTest class.
    /// </summary>
    [TestFixture]
    public class AverageManagerTest
    {
        #region Amplitude

        /// <summary>
        /// Use the average manager to average Amplitude data.
        /// </summary>
        [Test]
        public void AverageAmplitudeTest()
        {
            AverageManager avgMgr = new AverageManager(new AverageManagerOptions());
            avgMgr.IsAmplitudeAveraging = true;
            avgMgr.NumSamples = 3;
            avgMgr.IsAvgRunning = false;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(AverageAmplitudeTest_AveragedEnsemble);

            // Create an ensemble
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5;

            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6;

            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;

            avgMgr.AddEnsemble(ens1);
            avgMgr.AddEnsemble(ens2);
            avgMgr.AddEnsemble(ens3);
        }
        /// <summary>
        /// Event when the average is complete Average Amplitude Test..
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void AverageAmplitudeTest_AveragedEnsemble(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(3.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(4.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(5.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(6.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Bin 0 Beam 3 is incorrect.");
        }

        #endregion

        #region Correlation

        /// <summary>
        /// Use the average manager to average Correlation data.
        /// </summary>
        [Test]
        public void AverageCorrelationTest()
        {
            AverageManager avgMgr = new AverageManager(new AverageManagerOptions());
            avgMgr.IsCorrelationAveraging = true;
            avgMgr.NumSamples = 3;
            avgMgr.IsAvgRunning = false;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(AverageCorrelationTest_AveragedEnsemble);

            // Create an ensemble
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5;

            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6;

            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;

            avgMgr.AddEnsemble(ens1);
            avgMgr.AddEnsemble(ens2);
            avgMgr.AddEnsemble(ens3);
        }
        /// <summary>
        /// Event when the average is complete Average Amplitude Test..
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void AverageCorrelationTest_AveragedEnsemble(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(3.666, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(4.666, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(5.666, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(6.666, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Bin 0 Beam 3 is incorrect.");
        }

        #endregion

        #region Reference Layer Averaging

        /// <summary>
        /// Test averaging reference layer average with AverageManager.
        /// </summary>
        [Test]
        public void RefLayerAvgTest()
        {
            // 5 Samples
            // Min Ref layer = 1
            // Max Ref Layer = 3
            // Not running average
            AverageManager avgMgr = new AverageManager(new AverageManagerOptions());
            avgMgr.IsReferenceLayerAveraging = true;
            avgMgr.IsAvgRunning = false;
            avgMgr.MinRefLayer = 1;
            avgMgr.MaxRefLayer = 3;
            avgMgr.NumSamples = 5;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(RefLayerAvgTest_AveragedEnsemble);

            // Create an ensemble
            // All zeros for Earth Velocity
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens4 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens5 = EnsembleHelper.GenerateEnsemble(30);

            #region Ensemble 1

            ens1.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.3f;
            ens1.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.6f;
            ens1.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.2f;

            ens1.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens1.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.3f;
            ens1.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.15f;

            ens1.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens1.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.4f;
            ens1.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.11f;

            #endregion

            #region Ensemble 2

            ens2.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens2.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.5f;
            ens2.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.3f;

            ens2.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.5f;
            ens2.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.42f;
            ens2.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.15f;

            ens2.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens2.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.8f;
            ens2.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.2f;

            #endregion

            #region Ensemble 3

            ens3.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens3.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.6f;
            ens3.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.2f;

            ens3.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens3.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.23f;
            ens3.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.5f;

            ens3.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.12f;
            ens3.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.4f;
            ens3.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.123f;

            #endregion

            #region Ensemble 4

            ens4.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.3f;
            ens4.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.25f;
            ens4.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.3f;

            ens4.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.5f;
            ens4.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.54f;
            ens4.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.25f;

            ens4.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens4.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.45f;
            ens4.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.4f;

            #endregion

            #region Ensemble 5

            ens5.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.3f;
            ens5.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.45f;
            ens5.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.3f;

            ens5.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens5.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.25f;
            ens5.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.25f;

            ens5.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.45f;
            ens5.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.36f;
            ens5.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.14f;

            #endregion

            // Add the ensembles
            avgMgr.AddEnsemble(ens1);
            avgMgr.AddEnsemble(ens2);
            avgMgr.AddEnsemble(ens3);
            avgMgr.AddEnsemble(ens4);
            avgMgr.AddEnsemble(ens5);
        }

        /// <summary>
        /// Event when the average is complete Reference Layer Aveage Test.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void RefLayerAvgTest_AveragedEnsemble(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(2.34, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Bin 1 EAST is incorrect.");
            Assert.AreEqual(1.48, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Bin 1 NORTH is incorrect.");
            Assert.AreEqual(0.26, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Bin 1 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Bin 1 Q is incorrect.");

            Assert.AreEqual(2.56, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Bin 2 EAST is incorrect.");
            Assert.AreEqual(1.348, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Bin 2 NORTH is incorrect.");
            Assert.AreEqual(0.26, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Bin 2 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Bin 2 Q is incorrect.");

            Assert.AreEqual(2.394, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Bin 3 EAST is incorrect.");
            Assert.AreEqual(1.482, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Bin 3 NORTH is incorrect.");
            Assert.AreEqual(0.1946, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Bin 3 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Bin 3 Q is incorrect.");
        }

        #endregion

        #region Average All

        /// <summary>
        /// Average correlation, amplitude and reference layer averaging.
        /// </summary>
        [Test]
        public void AverageAll()
        {
            AverageManager avgMgr = new AverageManager(new AverageManagerOptions());
            avgMgr.IsAmplitudeAveraging = true;
            avgMgr.IsReferenceLayerAveraging = true;
            avgMgr.IsCorrelationAveraging = true;
            avgMgr.NumSamples = 5;
            avgMgr.IsAvgRunning = false;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(AverageAll_AveragedEnsemble);

            // Create an ensemble
            // All zeros for Earth Velocity
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens4 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens5 = EnsembleHelper.GenerateEnsemble(30);

            #region Ensemble 1

            ens1.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.3f;
            ens1.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.6f;
            ens1.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.2f;

            ens1.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens1.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.3f;
            ens1.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.15f;

            ens1.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens1.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.4f;
            ens1.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.11f;

            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2.3f;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3.3f;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4.3f;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5.3f;

            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2.1f;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3.1f;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4.1f;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5.1f;

            #endregion

            #region Ensemble 2

            ens2.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens2.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.5f;
            ens2.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.3f;

            ens2.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.5f;
            ens2.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.42f;
            ens2.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.15f;

            ens2.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens2.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.8f;
            ens2.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.2f;

            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3.5f;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4.5f;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5.5f;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6.5f;

            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4.2f;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5.2f;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6.2f;

            #endregion

            #region Ensemble 3

            ens3.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens3.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.6f;
            ens3.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.2f;

            ens3.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens3.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.23f;
            ens3.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.5f;

            ens3.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.12f;
            ens3.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.4f;
            ens3.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.123f;

            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;

            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;

            #endregion

            #region Ensemble 4

            ens4.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.3f;
            ens4.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.25f;
            ens4.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.3f;

            ens4.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.5f;
            ens4.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.54f;
            ens4.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.25f;

            ens4.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens4.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.45f;
            ens4.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.4f;

            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6.1f;
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7.1f;
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8.1f;
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9.1f;

            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6.1f;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7.1f;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8.1f;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9.1f;

            #endregion

            #region Ensemble 5

            ens5.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.3f;
            ens5.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.45f;
            ens5.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.3f;

            ens5.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens5.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.25f;
            ens5.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.25f;

            ens5.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.45f;
            ens5.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.36f;
            ens5.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.14f;

            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6.2f;
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7.2f;
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8.2f;
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9.2f;

            ens5.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6.2f;
            ens5.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7.23f;
            ens5.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8.2f;
            ens5.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9.2f;

            #endregion

            // Add the ensembles
            avgMgr.AddEnsemble(ens1);
            avgMgr.AddEnsemble(ens2);
            avgMgr.AddEnsemble(ens3);
            avgMgr.AddEnsemble(ens4);
            avgMgr.AddEnsemble(ens5);


        }
        /// <summary>
        /// Event when the average is complete AverageAll test.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void AverageAll_AveragedEnsemble(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");

            // Amplitude result
            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(4.82f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Amplitude Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(5.82f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Amplitude Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(6.82f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Amplitude Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(7.82f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Amplitude Bin 0 Beam 3 is incorrect.");

            // Correlation result
            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(4.72f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Correlation Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(5.726f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Correlation Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(6.72f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Correlation Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(7.72f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Correlation Bin 0 Beam 3 is incorrect.");

            // Reference Layer Average Result
            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(2.34, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Bin 1 EAST is incorrect.");
            Assert.AreEqual(1.48, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Bin 1 NORTH is incorrect.");
            Assert.AreEqual(0.26, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Bin 1 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Bin 1 Q is incorrect.");

            Assert.AreEqual(2.56, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Bin 2 EAST is incorrect.");
            Assert.AreEqual(1.348, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Bin 2 NORTH is incorrect.");
            Assert.AreEqual(0.26, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Bin 2 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Bin 2 Q is incorrect.");

            Assert.AreEqual(2.394, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Bin 3 EAST is incorrect.");
            Assert.AreEqual(1.482, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Bin 3 NORTH is incorrect.");
            Assert.AreEqual(0.1946, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Bin 3 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Bin 3 Q is incorrect.");
        }


        #endregion

        #region Running Average

        #region Correlation

        /// <summary>
        /// Use the average manager to get running average of Correlation data.
        /// </summary>
        [Test]
        public void RunningAverageCorrelationTest()
        {
            AverageManager avgMgr = new AverageManager(new AverageManagerOptions());
            avgMgr.IsCorrelationAveraging = true;
            avgMgr.NumSamples = 3;
            avgMgr.IsAvgRunning = false;
            avgMgr.IsAvgByTimer = false;
            avgMgr.IsAvgByNumSamples = true;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(RunningAvgTest_AveragedEnsemble1);

            // Create an ensemble
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2.1f;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3.1f;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4.1f;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5.1f;

            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4.2f;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5.2f;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6.2f;

            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;

            DataSet.Ensemble ens4 = EnsembleHelper.GenerateEnsemble(30);
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6.1f;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7.1f;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8.1f;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9.1f;

            DataSet.Ensemble ens5 = EnsembleHelper.GenerateEnsemble(30);
            ens5.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6.2f;
            ens5.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7.23f;
            ens5.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8.2f;
            ens5.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9.2f;

            avgMgr.AddEnsemble(ens1);
            avgMgr.AddEnsemble(ens2);
            avgMgr.AddEnsemble(ens3);

            avgMgr.AveragedEnsemble -= RunningAvgTest_AveragedEnsemble1;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(RunningAvgTest_AveragedEnsemble2);
            avgMgr.AddEnsemble(ens4);

            avgMgr.AveragedEnsemble -= RunningAvgTest_AveragedEnsemble2;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(RunningAvgTest_AveragedEnsemble3);
            avgMgr.AddEnsemble(ens5);

        }
        /// <summary>
        /// Get the first average.  This is a running average.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void RunningAvgTest_AveragedEnsemble1(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(3.766f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Avg1 Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(4.766f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Avg1 Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(5.766f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Avg1 Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(6.766f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Avg1 Bin 0 Beam 3 is incorrect.");
        }
        /// <summary>
        /// Get the second average.  This is a running average.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void RunningAvgTest_AveragedEnsemble2(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(5.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Avg2 Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(6.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Avg2 Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(7.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Avg2 Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(8.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Avg2 Bin 0 Beam 3 is incorrect.");
        }
        /// <summary>
        /// Get the third average.  This is a running average.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void RunningAvgTest_AveragedEnsemble3(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(6.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Avg3 Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(7.11f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Avg3 Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(8.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Avg3 Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(9.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Avg3 Bin 0 Beam 3 is incorrect.");
        }

        #endregion

        #region Amplitude

        /// <summary>
        /// Use the average manager to get running average of Amplitude data.
        /// </summary>
        [Test]
        public void RunningAverageAmplitudeTest()
        {
            AverageManager avgMgr = new AverageManager(new AverageManagerOptions());
            avgMgr.IsAmplitudeAveraging = true;
            avgMgr.NumSamples = 3;
            avgMgr.IsAvgRunning = false;
            avgMgr.IsAvgByTimer = false;
            avgMgr.IsAvgByNumSamples = true;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(RunningAvgAmpTest_AveragedEnsemble1);

            // Create an ensemble
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2.1f;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3.1f;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4.1f;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5.1f;

            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4.2f;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5.2f;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6.2f;

            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;

            DataSet.Ensemble ens4 = EnsembleHelper.GenerateEnsemble(30);
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6.1f;
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7.1f;
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8.1f;
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9.1f;

            DataSet.Ensemble ens5 = EnsembleHelper.GenerateEnsemble(30);
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6.2f;
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7.23f;
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8.2f;
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9.2f;

            avgMgr.AddEnsemble(ens1);
            avgMgr.AddEnsemble(ens2);
            avgMgr.AddEnsemble(ens3);

            avgMgr.AveragedEnsemble -= RunningAvgAmpTest_AveragedEnsemble1;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(RunningAvgAmpTest_AveragedEnsemble2);
            avgMgr.AddEnsemble(ens4);

            avgMgr.AveragedEnsemble -= RunningAvgAmpTest_AveragedEnsemble2;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(RunningAvgAmpTest_AveragedEnsemble3);
            avgMgr.AddEnsemble(ens5);

        }
        /// <summary>
        /// Get the first average.  This is a running average.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void RunningAvgAmpTest_AveragedEnsemble1(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(3.766f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Amp Avg1 Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(4.766f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Amp Avg1 Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(5.766f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Amp Avg1 Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(6.766f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Amp Avg1 Bin 0 Beam 3 is incorrect.");
        }
        /// <summary>
        /// Get the second average.  This is a running average.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void RunningAvgAmpTest_AveragedEnsemble2(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(5.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Amp Avg2 Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(6.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Amp Avg2 Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(7.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Amp Avg2 Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(8.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Amp Avg2 Bin 0 Beam 3 is incorrect.");
        }
        /// <summary>
        /// Get the third average.  This is a running average.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void RunningAvgAmpTest_AveragedEnsemble3(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(6.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Amp Avg3 Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(7.11f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Amp Avg3 Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(8.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Amp Avg3 Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(9.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Amp Avg3 Bin 0 Beam 3 is incorrect.");
        }

        #endregion

        #region Reference Layer Averaging

        /// <summary>
        /// Use the average manager to get running average of Reference Layer averaging data.
        /// </summary>
        [Test]
        public void RunningAverageReferencelayerTest()
        {
            AverageManager avgMgr = new AverageManager(new AverageManagerOptions());
            avgMgr.IsAvgRunning = true;
            avgMgr.NumSamples = 3;
            avgMgr.IsReferenceLayerAveraging = true;
            avgMgr.MinRefLayer = 1;
            avgMgr.MaxRefLayer = 3;
            avgMgr.IsAvgRunning = false;
            avgMgr.IsAvgByTimer = false;
            avgMgr.IsAvgByNumSamples = true;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(RunningAvgRefLayerTest_AveragedEnsemble1);

            // Create an ensemble
            // All zeros for Earth Velocity
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens4 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens5 = EnsembleHelper.GenerateEnsemble(30);

            #region Ensemble 1

            ens1.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.3f;
            ens1.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.6f;
            ens1.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.2f;

            ens1.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens1.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.3f;
            ens1.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.15f;

            ens1.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens1.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.4f;
            ens1.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.11f;

            #endregion

            #region Ensemble 2

            ens2.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens2.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.5f;
            ens2.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.3f;

            ens2.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.5f;
            ens2.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.42f;
            ens2.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.15f;

            ens2.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens2.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.8f;
            ens2.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.2f;

            #endregion

            #region Ensemble 3

            ens3.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens3.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.6f;
            ens3.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.2f;

            ens3.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens3.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.23f;
            ens3.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.5f;

            ens3.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.12f;
            ens3.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.4f;
            ens3.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.123f;

            #endregion

            #region Ensemble 4

            ens4.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.3f;
            ens4.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.25f;
            ens4.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.3f;

            ens4.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.5f;
            ens4.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.54f;
            ens4.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.25f;

            ens4.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens4.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.45f;
            ens4.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.4f;

            #endregion

            #region Ensemble 5

            ens5.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.3f;
            ens5.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.45f;
            ens5.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.3f;

            ens5.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens5.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.25f;
            ens5.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.25f;

            ens5.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.45f;
            ens5.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.36f;
            ens5.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.14f;

            #endregion

            // Add the ensembles
            avgMgr.AddEnsemble(ens1);
            avgMgr.AddEnsemble(ens2);
            avgMgr.AddEnsemble(ens3);

            avgMgr.AveragedEnsemble -= RunningAvgRefLayerTest_AveragedEnsemble1;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(RunningAvgRefLayerTest_AveragedEnsemble2);
            avgMgr.AddEnsemble(ens4);

            avgMgr.AveragedEnsemble -= RunningAvgRefLayerTest_AveragedEnsemble2;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(RunningAvgRefLayerTest_AveragedEnsemble3);
            avgMgr.AddEnsemble(ens5);

        }
        /// <summary>
        /// Get the first average.  This is a running average.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void RunningAvgRefLayerTest_AveragedEnsemble1(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(2.4, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg1 Bin 1 EAST is incorrect.");
            Assert.AreEqual(1.6, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg1 Bin 1 NORTH is incorrect.");
            Assert.AreEqual(0.21, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg1 Bin 1 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg1 Bin 1 Q is incorrect.");

            Assert.AreEqual(2.60, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg1 Bin 2 EAST is incorrect.");
            Assert.AreEqual(1.31, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg1 Bin 2 NORTH is incorrect.");
            Assert.AreEqual(0.31, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg1 Bin 2 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg1 Bin 2 Q is incorrect.");

            Assert.AreEqual(2.33, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg1 Bin 3 EAST is incorrect.");
            Assert.AreEqual(1.52, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg1 Bin 3 NORTH is incorrect.");
            Assert.AreEqual(0.123, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg1 Bin 3 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg1 Bin 3 Q is incorrect.");
        }
        /// <summary>
        /// Get the second average.  This is a running average.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void RunningAvgRefLayerTest_AveragedEnsemble2(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(2.36, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg2 Bin 1 EAST is incorrect.");
            Assert.AreEqual(1.41, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg2 Bin 1 NORTH is incorrect.");
            Assert.AreEqual(0.26, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg2 Bin 1 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg2 Bin 1 Q is incorrect.");

            Assert.AreEqual(2.53, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg2 Bin 2 EAST is incorrect.");
            Assert.AreEqual(1.45, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg2 Bin 2 NORTH is incorrect.");
            Assert.AreEqual(0.28, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg2 Bin 2 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg2 Bin 2 Q is incorrect.");

            Assert.AreEqual(2.394, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg2 Bin 3 EAST is incorrect.");
            Assert.AreEqual(1.54, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg2 Bin 3 NORTH is incorrect.");
            Assert.AreEqual(0.27, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg2 Bin 3 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg2 Bin 3 Q is incorrect.");
        }
        /// <summary>
        /// Get the third average.  This is a running average.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void RunningAvgRefLayerTest_AveragedEnsemble3(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(2.31, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg3 Bin 1 EAST is incorrect.");
            Assert.AreEqual(1.45, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg3 Bin 1 NORTH is incorrect.");
            Assert.AreEqual(0.29, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg3 Bin 1 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg3 Bin 1 Q is incorrect.");

            Assert.AreEqual(2.56, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg3 Bin 2 EAST is incorrect.");
            Assert.AreEqual(1.33, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg3 Bin 2 NORTH is incorrect.");
            Assert.AreEqual(0.32, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg3 Bin 2 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg3 Bin 2 Q is incorrect.");

            Assert.AreEqual(2.34, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg3 Bin 3 EAST is incorrect.");
            Assert.AreEqual(1.40, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg3 Bin 3 NORTH is incorrect.");
            Assert.AreEqual(0.21, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg3 Bin 3 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg3 Bin 3 Q is incorrect.");
        }

        #endregion

        #region All Running Average

        /// <summary>
        /// Use the average manager to get running average of all data.
        /// </summary>
        [Test]
        public void RunningAverageAllTest()
        {
            AverageManager avgMgr = new AverageManager(new AverageManagerOptions());
            avgMgr.IsAmplitudeAveraging = true;
            avgMgr.IsReferenceLayerAveraging = true;
            avgMgr.IsCorrelationAveraging = true;
            avgMgr.NumSamples = 3;
            avgMgr.MinRefLayer = 1;
            avgMgr.MaxRefLayer = 3;
            avgMgr.IsAvgRunning = false;
            avgMgr.IsAvgByTimer = false;
            avgMgr.IsAvgByNumSamples = true;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(RunningAvgAllTest_AveragedEnsemble1);

            // Create an ensemble
            // All zeros for Earth Velocity
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens4 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens5 = EnsembleHelper.GenerateEnsemble(30);

            #region Ensemble 1

            ens1.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.3f;
            ens1.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.6f;
            ens1.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.2f;

            ens1.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens1.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.3f;
            ens1.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.15f;

            ens1.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens1.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.4f;
            ens1.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.11f;

            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2.1f;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3.1f;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4.1f;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5.1f;

            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2.1f;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3.1f;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4.1f;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5.1f;

            #endregion

            #region Ensemble 2

            ens2.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens2.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.5f;
            ens2.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.3f;

            ens2.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.5f;
            ens2.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.42f;
            ens2.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.15f;

            ens2.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens2.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.8f;
            ens2.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.2f;

            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4.2f;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5.2f;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6.2f;

            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4.2f;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5.2f;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6.2f;

            #endregion

            #region Ensemble 3

            ens3.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens3.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.6f;
            ens3.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.2f;

            ens3.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens3.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.23f;
            ens3.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.5f;

            ens3.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.12f;
            ens3.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.4f;
            ens3.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.123f;

            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;

            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;

            #endregion

            #region Ensemble 4

            ens4.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.3f;
            ens4.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.25f;
            ens4.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.3f;

            ens4.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.5f;
            ens4.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.54f;
            ens4.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.25f;

            ens4.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.4f;
            ens4.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.45f;
            ens4.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.4f;

            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6.1f;
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7.1f;
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8.1f;
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9.1f;

            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6.1f;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7.1f;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8.1f;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9.1f;

            #endregion

            #region Ensemble 5

            ens5.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.3f;
            ens5.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.45f;
            ens5.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.3f;

            ens5.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.6f;
            ens5.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.25f;
            ens5.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.25f;

            ens5.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX] = 2.45f;
            ens5.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1.36f;
            ens5.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.14f;

            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6.2f;
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7.2f;
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8.2f;
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9.2f;

            ens5.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6.2f;
            ens5.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7.23f;
            ens5.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8.2f;
            ens5.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9.2f;

            #endregion

            // Add the ensembles
            avgMgr.AddEnsemble(ens1);
            avgMgr.AddEnsemble(ens2);
            avgMgr.AddEnsemble(ens3);

            avgMgr.AveragedEnsemble -= RunningAvgAllTest_AveragedEnsemble1;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(RunningAvgAllTest_AveragedEnsemble2);
            avgMgr.AddEnsemble(ens4);

            avgMgr.AveragedEnsemble -= RunningAvgAllTest_AveragedEnsemble2;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(RunningAvgAllTest_AveragedEnsemble3);
            avgMgr.AddEnsemble(ens5);

        }
        /// <summary>
        /// Get the first average.  This is a running average.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void RunningAvgAllTest_AveragedEnsemble1(DataSet.Ensemble ensemble)
        {
            // Correlation
            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(3.766f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Avg1 Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(4.766f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Avg1 Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(5.766f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Avg1 Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(6.766f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Avg1 Bin 0 Beam 3 is incorrect.");

            // Amplitude 
            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(3.766f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Amp Avg1 Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(4.766f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Amp Avg1 Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(5.766f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Amp Avg1 Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(6.766f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Amp Avg1 Bin 0 Beam 3 is incorrect.");

            // Reference Layer Averaging
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(2.4, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg1 Bin 1 EAST is incorrect.");
            Assert.AreEqual(1.6, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg1 Bin 1 NORTH is incorrect.");
            Assert.AreEqual(0.21, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg1 Bin 1 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg1 Bin 1 Q is incorrect.");

            Assert.AreEqual(2.60, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg1 Bin 2 EAST is incorrect.");
            Assert.AreEqual(1.31, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg1 Bin 2 NORTH is incorrect.");
            Assert.AreEqual(0.31, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg1 Bin 2 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg1 Bin 2 Q is incorrect.");

            Assert.AreEqual(2.33, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg1 Bin 3 EAST is incorrect.");
            Assert.AreEqual(1.52, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg1 Bin 3 NORTH is incorrect.");
            Assert.AreEqual(0.123, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg1 Bin 3 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg1 Bin 3 Q is incorrect.");
        }
        /// <summary>
        /// Get the second average.  This is a running average.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void RunningAvgAllTest_AveragedEnsemble2(DataSet.Ensemble ensemble)
        {
            // Correlation
            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(5.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Avg2 Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(6.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Avg2 Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(7.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Avg2 Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(8.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Avg2 Bin 0 Beam 3 is incorrect.");

            // Amplitude
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(5.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Amp Avg2 Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(6.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Amp Avg2 Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(7.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Amp Avg2 Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(8.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Amp Avg2 Bin 0 Beam 3 is incorrect.");

            // Reference Layer Average
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(2.36, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg2 Bin 1 EAST is incorrect.");
            Assert.AreEqual(1.41, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg2 Bin 1 NORTH is incorrect.");
            Assert.AreEqual(0.26, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg2 Bin 1 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg2 Bin 1 Q is incorrect.");

            Assert.AreEqual(2.53, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg2 Bin 2 EAST is incorrect.");
            Assert.AreEqual(1.45, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg2 Bin 2 NORTH is incorrect.");
            Assert.AreEqual(0.28, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg2 Bin 2 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg2 Bin 2 Q is incorrect.");

            Assert.AreEqual(2.394, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg2 Bin 3 EAST is incorrect.");
            Assert.AreEqual(1.54, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg2 Bin 3 NORTH is incorrect.");
            Assert.AreEqual(0.27, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg2 Bin 3 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg2 Bin 3 Q is incorrect.");
        }
        /// <summary>
        /// Get the third average.  This is a running average.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void RunningAvgAllTest_AveragedEnsemble3(DataSet.Ensemble ensemble)
        {
            // Correlation
            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(6.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Avg3 Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(7.11f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Avg3 Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(8.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Avg3 Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(9.1f, ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Avg3 Bin 0 Beam 3 is incorrect.");

            // Amplitude
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(6.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Amp Avg3 Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(7.11f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Amp Avg3 Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(8.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Amp Avg3 Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(9.1f, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Amp Avg3 Bin 0 Beam 3 is incorrect.");

            // Reference Layer Averaging
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(2.31, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg3 Bin 1 EAST is incorrect.");
            Assert.AreEqual(1.45, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg3 Bin 1 NORTH is incorrect.");
            Assert.AreEqual(0.29, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg3 Bin 1 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[1, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg3 Bin 1 Q is incorrect.");

            Assert.AreEqual(2.56, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg3 Bin 2 EAST is incorrect.");
            Assert.AreEqual(1.33, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg3 Bin 2 NORTH is incorrect.");
            Assert.AreEqual(0.32, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg3 Bin 2 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[2, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg3 Bin 2 Q is incorrect.");

            Assert.AreEqual(2.34, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Avg3 Bin 3 EAST is incorrect.");
            Assert.AreEqual(1.40, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Avg3 Bin 3 NORTH is incorrect.");
            Assert.AreEqual(0.21, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Avg3 Bin 3 VERTICAL is incorrect.");
            Assert.AreEqual(0, ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Avg3 Bin 3 Q is incorrect.");
        }

        #endregion

        #endregion

        #region Parameters Set

        /// <summary>
        /// Use the average manager to average Amplitude data.
        /// </summary>
        [Test]
        public void AverageParametersTest()
        {
            AverageManager avgMgr = new AverageManager(new AverageManagerOptions());
            avgMgr.IsAmplitudeAveraging = true;
            avgMgr.NumSamples = 3;
            avgMgr.IsAvgRunning = false;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(AverageParametersTest_AveragedEnsemble);

            // Create an ensemble
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5;
            ens1.EnsembleData.ActualPingCount = 1;
            ens1.EnsembleData.DesiredPingCount = 1;
            ens1.AncillaryData.FirstPingTime = 1.123f;
            ens1.AncillaryData.LastPingTime = 1.123f;

            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6;
            ens2.EnsembleData.ActualPingCount = 1;
            ens2.EnsembleData.DesiredPingCount = 1;
            ens2.AncillaryData.FirstPingTime = 2.123f;
            ens2.AncillaryData.LastPingTime = 2.123f;

            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;
            ens3.EnsembleData.ActualPingCount = 1;
            ens3.EnsembleData.DesiredPingCount = 1;
            ens3.AncillaryData.FirstPingTime = 3.123f;
            ens3.AncillaryData.LastPingTime = 3.123f;

            avgMgr.AddEnsemble(ens1);
            avgMgr.AddEnsemble(ens2);
            avgMgr.AddEnsemble(ens3);
        }
        /// <summary>
        /// Event when the average is complete Average Amplitude Test..
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void AverageParametersTest_AveragedEnsemble(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");

            Assert.AreEqual(0.0f, ensemble.AncillaryData.FirstPingTime, 0.001, "First Ping time is incorrect.");
            Assert.AreEqual(3.123f, ensemble.AncillaryData.LastPingTime, 0.001, "Last Ping time is incorrect.");
            Assert.AreEqual(3, ensemble.EnsembleData.DesiredPingCount, "Desired Ping count is incorrect.");
            Assert.AreEqual(3, ensemble.EnsembleData.ActualPingCount, "Actual Ping count is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, ensemble.EnsembleData.Year, "Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, ensemble.EnsembleData.Month, "Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, ensemble.EnsembleData.Day, "Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, ensemble.EnsembleData.Hour, "Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, ensemble.EnsembleData.Minute, "Minute is incorrect.");
            
            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(3.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(4.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(5.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(6.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Bin 0 Beam 3 is incorrect.");
        }


        /// <summary>
        /// Use the average manager to average Amplitude data.
        /// </summary>
        [Test]
        public void AverageParameters2PingCountTest()
        {
            AverageManager avgMgr = new AverageManager(new AverageManagerOptions());
            avgMgr.IsAmplitudeAveraging = true;
            avgMgr.NumSamples = 3;
            avgMgr.IsAvgRunning = false;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(AverageParameters2PingCountTest_AveragedEnsemble);

            // Create an ensemble
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5;
            ens1.EnsembleData.ActualPingCount = 2;
            ens1.EnsembleData.DesiredPingCount = 2;
            ens1.AncillaryData.FirstPingTime = 1.123f;
            ens1.AncillaryData.LastPingTime = 1.223f;

            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6;
            ens2.EnsembleData.ActualPingCount = 2;
            ens2.EnsembleData.DesiredPingCount = 2;
            ens2.AncillaryData.FirstPingTime = 2.123f;
            ens2.AncillaryData.LastPingTime = 2.223f;

            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;
            ens3.EnsembleData.ActualPingCount = 2;
            ens3.EnsembleData.DesiredPingCount = 2;
            ens3.AncillaryData.FirstPingTime = 3.123f;
            ens3.AncillaryData.LastPingTime = 3.223f;

            avgMgr.AddEnsemble(ens1);
            avgMgr.AddEnsemble(ens2);
            avgMgr.AddEnsemble(ens3);
        }
        /// <summary>
        /// Event when the average is complete Average Amplitude Test..
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void AverageParameters2PingCountTest_AveragedEnsemble(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");

            Assert.AreEqual(0.0f, ensemble.AncillaryData.FirstPingTime, 0.001, "First Ping time is incorrect.");
            Assert.AreEqual(3.223f, ensemble.AncillaryData.LastPingTime, 0.001, "Last Ping time is incorrect.");
            Assert.AreEqual(6, ensemble.EnsembleData.DesiredPingCount, "Desired Ping count is incorrect.");
            Assert.AreEqual(6, ensemble.EnsembleData.ActualPingCount, "Actual Ping count is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, ensemble.EnsembleData.Year, "Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, ensemble.EnsembleData.Month, "Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, ensemble.EnsembleData.Day, "Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, ensemble.EnsembleData.Hour, "Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, ensemble.EnsembleData.Minute, "Minute is incorrect.");

            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(3.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(4.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(5.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(6.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Bin 0 Beam 3 is incorrect.");
        }

        #region Running Average

        /// <summary>
        /// Use the average manager to average Amplitude data.
        /// </summary>
        [Test]
        public void AverageParameters2PingCountRunningAvgTest()
        {
            AverageManager avgMgr = new AverageManager(new AverageManagerOptions());
            avgMgr.IsAmplitudeAveraging = true;
            avgMgr.NumSamples = 3;
            avgMgr.IsAvgByNumSamples = true;
            avgMgr.IsAvgRunning = false;
            avgMgr.IsAvgByTimer = false;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(AverageParameters2PingCountRunningAvgTest_AveragedEnsemble);

            // Create an ensemble
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5;
            ens1.EnsembleData.ActualPingCount = 2;
            ens1.EnsembleData.DesiredPingCount = 2;
            ens1.AncillaryData.FirstPingTime = 1.123f;
            ens1.AncillaryData.LastPingTime = 1.223f;

            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6;
            ens2.EnsembleData.ActualPingCount = 2;
            ens2.EnsembleData.DesiredPingCount = 2;
            ens2.AncillaryData.FirstPingTime = 2.123f;
            ens2.AncillaryData.LastPingTime = 2.223f;

            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;
            ens3.EnsembleData.ActualPingCount = 2;
            ens3.EnsembleData.DesiredPingCount = 2;
            ens3.AncillaryData.FirstPingTime = 3.123f;
            ens3.AncillaryData.LastPingTime = 3.223f;

            DataSet.Ensemble ens4 = EnsembleHelper.GenerateEnsemble(30);
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ens4.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5;
            ens4.EnsembleData.ActualPingCount = 2;
            ens4.EnsembleData.DesiredPingCount = 2;
            ens4.AncillaryData.FirstPingTime = 4.123f;
            ens4.AncillaryData.LastPingTime = 4.223f;

            DataSet.Ensemble ens5 = EnsembleHelper.GenerateEnsemble(30);
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3;
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4;
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5;
            ens5.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6;
            ens5.EnsembleData.ActualPingCount = 2;
            ens5.EnsembleData.DesiredPingCount = 2;
            ens5.AncillaryData.FirstPingTime = 5.123f;
            ens5.AncillaryData.LastPingTime = 5.223f;

            avgMgr.AddEnsemble(ens1);
            avgMgr.AddEnsemble(ens2);
            avgMgr.AddEnsemble(ens3);

            avgMgr.AveragedEnsemble -= AverageParameters2PingCountRunningAvgTest_AveragedEnsemble;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(AverageParameters2PingCountRunningAvgTest2_AveragedEnsemble);
            avgMgr.AddEnsemble(ens4);

            avgMgr.AveragedEnsemble -= AverageParameters2PingCountRunningAvgTest2_AveragedEnsemble;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(AverageParameters2PingCountRunningAvgTest3_AveragedEnsemble);
            avgMgr.AddEnsemble(ens5);
        }
        /// <summary>
        /// Event when the average is complete Average Amplitude Test..
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void AverageParameters2PingCountRunningAvgTest_AveragedEnsemble(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");

            Assert.AreEqual(0.0f, ensemble.AncillaryData.FirstPingTime, 0.001, "First Ping time is incorrect.");
            Assert.AreEqual(3.223f, ensemble.AncillaryData.LastPingTime, 0.001, "Last Ping time is incorrect.");
            Assert.AreEqual(6, ensemble.EnsembleData.DesiredPingCount, "Desired Ping count is incorrect.");
            Assert.AreEqual(6, ensemble.EnsembleData.ActualPingCount, "Actual Ping count is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, ensemble.EnsembleData.Year, "Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, ensemble.EnsembleData.Month, "Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, ensemble.EnsembleData.Day, "Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, ensemble.EnsembleData.Hour, "Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, ensemble.EnsembleData.Minute, "Minute is incorrect.");

            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(3.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(4.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(5.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(6.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Bin 0 Beam 3 is incorrect.");
        }

        /// <summary>
        /// Event when the average is complete Average Amplitude Test..
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void AverageParameters2PingCountRunningAvgTest2_AveragedEnsemble(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");

            Assert.AreEqual(3.123f, ensemble.AncillaryData.FirstPingTime, 0.001, "First Ping time is incorrect.");
            Assert.AreEqual(4.223f, ensemble.AncillaryData.LastPingTime, 0.001, "Last Ping time is incorrect.");
            Assert.AreEqual(6, ensemble.EnsembleData.DesiredPingCount, "Desired Ping count is incorrect.");
            Assert.AreEqual(6, ensemble.EnsembleData.ActualPingCount, "Actual Ping count is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, ensemble.EnsembleData.Year, "Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, ensemble.EnsembleData.Month, "Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, ensemble.EnsembleData.Day, "Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, ensemble.EnsembleData.Hour, "Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, ensemble.EnsembleData.Minute, "Minute is incorrect.");

            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(3.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(4.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(5.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(6.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Bin 0 Beam 3 is incorrect.");
        }
        /// <summary>
        /// Event when the average is complete Average Amplitude Test..
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void AverageParameters2PingCountRunningAvgTest3_AveragedEnsemble(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");

            Assert.AreEqual(4.123f, ensemble.AncillaryData.FirstPingTime, 0.001, "First Ping time is incorrect.");
            Assert.AreEqual(5.223f, ensemble.AncillaryData.LastPingTime, 0.001, "Last Ping time is incorrect.");
            Assert.AreEqual(6, ensemble.EnsembleData.DesiredPingCount, "Desired Ping count is incorrect.");
            Assert.AreEqual(6, ensemble.EnsembleData.ActualPingCount, "Actual Ping count is incorrect.");
            Assert.AreEqual(DateTime.Now.Year, ensemble.EnsembleData.Year, "Year is incorrect.");
            Assert.AreEqual(DateTime.Now.Month, ensemble.EnsembleData.Month, "Month is incorrect.");
            Assert.AreEqual(DateTime.Now.Day, ensemble.EnsembleData.Day, "Day is incorrect.");
            Assert.AreEqual(DateTime.Now.Hour, ensemble.EnsembleData.Hour, "Hour is incorrect.");
            Assert.AreEqual(DateTime.Now.Minute, ensemble.EnsembleData.Minute, "Minute is incorrect.");

            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.AreEqual(3.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], 0.001, "Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(4.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], 0.001, "Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(5.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], 0.001, "Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(6.666, ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], 0.001, "Bin 0 Beam 3 is incorrect.");
        }

        #endregion

        #endregion

        #region First and Last Ping Time

        /// <summary>
        /// Verify the first and last ping time are set correctly in a Non-running average mode.
        /// </summary>
        [Test]
        public void FirstLastPingNotRunningTest()
        {
            AverageManager avgMgr = new AverageManager(new AverageManagerOptions());
            avgMgr.IsCorrelationAveraging = true;
            avgMgr.NumSamples = 2;
            avgMgr.IsAvgRunning = false;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(FirstLastPingNotRunningTest_AveragedEnsemble);

            // Create an ensemble
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5;
            ens1.AncillaryData.FirstPingTime = 1.1f;
            ens1.AncillaryData.LastPingTime = 1.1f;

            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6;
            ens2.AncillaryData.FirstPingTime = 2.2f;
            ens2.AncillaryData.LastPingTime = 2.2f;

            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;
            ens3.AncillaryData.FirstPingTime = 3.3f;
            ens3.AncillaryData.LastPingTime = 3.3f;

            DataSet.Ensemble ens4 = EnsembleHelper.GenerateEnsemble(30);
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;
            ens4.AncillaryData.FirstPingTime = 4.4f;
            ens4.AncillaryData.LastPingTime = 4.4f;

            avgMgr.AddEnsemble(ens1);
            avgMgr.AddEnsemble(ens2);

            avgMgr.AveragedEnsemble -= FirstLastPingNotRunningTest_AveragedEnsemble;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(FirstLastPingNotRunningTest_AveragedEnsemble1);
            
            avgMgr.AddEnsemble(ens3);
            avgMgr.AddEnsemble(ens4);

        }
        /// <summary>
        /// Check the first and last ping time are correct.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void FirstLastPingNotRunningTest_AveragedEnsemble(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(0.0f, ensemble.AncillaryData.FirstPingTime, 0.001, "First Ping Time is incorrect.");
            Assert.AreEqual(2.2f, ensemble.AncillaryData.LastPingTime, 0.001, "Last Ping Time is incorrect.");
        }
        /// <summary>
        /// Check the first and last ping time are correct.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void FirstLastPingNotRunningTest_AveragedEnsemble1(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(3.3f, ensemble.AncillaryData.FirstPingTime, 0.001, "First Ping Time is incorrect.");
            Assert.AreEqual(4.4f, ensemble.AncillaryData.LastPingTime, 0.001, "Last Ping Time is incorrect.");
        }


        /// <summary>
        /// Verify the first and last ping time are set correctly in a Non-running average mode.
        /// </summary>
        [Test]
        public void FirstLastPingRunningTest()
        {
            AverageManager avgMgr = new AverageManager(new AverageManagerOptions());
            avgMgr.IsCorrelationAveraging = true;
            avgMgr.NumSamples = 5;
            avgMgr.IsAvgRunning = false;
            avgMgr.IsAvgByTimer = false;
            avgMgr.IsAvgByNumSamples = true;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(FirstLastPingRunningTest_AveragedEnsemble);

            // Create an ensemble
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ens1.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5;
            ens1.AncillaryData.FirstPingTime = 1.1f;
            ens1.AncillaryData.LastPingTime = 1.1f;

            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 3;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 4;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 5;
            ens2.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 6;
            ens2.AncillaryData.FirstPingTime = 2.2f;
            ens2.AncillaryData.LastPingTime = 2.2f;

            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;
            ens3.AncillaryData.FirstPingTime = 3.3f;
            ens3.AncillaryData.LastPingTime = 3.3f;

            DataSet.Ensemble ens4 = EnsembleHelper.GenerateEnsemble(30);
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens4.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;
            ens4.AncillaryData.FirstPingTime = 4.4f;
            ens4.AncillaryData.LastPingTime = 4.4f;

            avgMgr.AddEnsemble(ens1);
            avgMgr.AddEnsemble(ens2);

            avgMgr.AveragedEnsemble -= FirstLastPingRunningTest_AveragedEnsemble;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(FirstLastPingRunningTest_AveragedEnsemble1);

            avgMgr.AddEnsemble(ens3);
            avgMgr.AveragedEnsemble -= FirstLastPingRunningTest_AveragedEnsemble1;
            avgMgr.AveragedEnsemble += new AverageManager.AveragedEnsembleEventHandler(FirstLastPingRunningTest_AveragedEnsemble2);

            avgMgr.AddEnsemble(ens4);

        }
        /// <summary>
        /// Check the first and last ping time are correct.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void FirstLastPingRunningTest_AveragedEnsemble(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(1.1f, ensemble.AncillaryData.FirstPingTime, 0.001, "First Ping Time is incorrect.");
            Assert.AreEqual(2.2f, ensemble.AncillaryData.LastPingTime, 0.001, "Last Ping Time is incorrect.");
        }
        /// <summary>
        /// Check the first and last ping time are correct.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void FirstLastPingRunningTest_AveragedEnsemble1(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(2.2f, ensemble.AncillaryData.FirstPingTime, 0.001, "First Ping Time is incorrect.");
            Assert.AreEqual(3.3f, ensemble.AncillaryData.LastPingTime, 0.001, "Last Ping Time is incorrect.");
        }
        /// <summary>
        /// Check the first and last ping time are correct.
        /// </summary>
        /// <param name="ensemble">Ensemble received with averaged data</param>
        void FirstLastPingRunningTest_AveragedEnsemble2(DataSet.Ensemble ensemble)
        {
            Assert.IsNotNull(ensemble, "Ensemble is incorrect.");
            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.AreEqual(3.3f, ensemble.AncillaryData.FirstPingTime, 0.001, "First Ping Time is incorrect.");
            Assert.AreEqual(4.4f, ensemble.AncillaryData.LastPingTime, 0.001, "Last Ping Time is incorrect.");
        }

        #endregion
    }
}
