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
 * 01/04/2013      RC          2.17       Initial coding
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
    using RTI.Average;

    /// <summary>
    /// Test the ReferenceLayerAverageTest class.
    /// </summary>
    [TestFixture]
    public class ReferenceLayerAverageTest
    {

        /// <summary>
        /// Verify the adding ensembles.
        /// </summary>
        [Test]
        public void AddEnsembles()
        {
            ReferenceLayerAverage avger = new ReferenceLayerAverage(2, 0, 3, false);

            // Create an ensemble
            // All zeros for correlation
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);

            avger.AddEnsemble(ens1);
            avger.AddEnsemble(ens2);

            float[,] result = avger.GetAverage();

            Assert.IsNotNull(result, "Result is incorrect.");
            Assert.AreEqual(0, result[0, DataSet.Ensemble.BEAM_0_INDEX], "Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(0, result[0, DataSet.Ensemble.BEAM_1_INDEX], "Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(0, result[0, DataSet.Ensemble.BEAM_2_INDEX], "Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(0, result[0, DataSet.Ensemble.BEAM_3_INDEX], "Bin 0 Beam 3 is incorrect.");
        }

        /// <summary>
        /// Verify the averaging works.
        /// </summary>
        [Test]
        public void Test1()
        {
            // 5 Samples
            // Min Ref layer = 1
            // Max Ref Layer = 3
            // Not running average
            ReferenceLayerAverage avger = new ReferenceLayerAverage(5, 1, 3, false);

            // Create an ensemble
            // All zeros for correlation
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
            avger.AddEnsemble(ens1);
            avger.AddEnsemble(ens2);
            avger.AddEnsemble(ens3);
            avger.AddEnsemble(ens4);
            avger.AddEnsemble(ens5);

            // Get the average
            float[,] result = avger.GetAverage();

            Assert.IsNotNull(result, "Result is incorrect.");
            Assert.AreEqual(2.34, result[1, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Bin 1 EAST is incorrect.");
            Assert.AreEqual(1.48, result[1, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Bin 1 NORTH is incorrect.");
            Assert.AreEqual(0.26, result[1, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Bin 1 VERTICAL is incorrect.");
            Assert.AreEqual(0, result[1, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Bin 1 Q is incorrect.");

            Assert.AreEqual(2.56, result[2, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Bin 2 EAST is incorrect.");
            Assert.AreEqual(1.348, result[2, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Bin 2 NORTH is incorrect.");
            Assert.AreEqual(0.26, result[2, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Bin 2 VERTICAL is incorrect.");
            Assert.AreEqual(0, result[2, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Bin 2 Q is incorrect.");

            Assert.AreEqual(2.394, result[3, DataSet.Ensemble.BEAM_EAST_INDEX], 0.01, "Bin 3 EAST is incorrect.");
            Assert.AreEqual(1.482, result[3, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.01, "Bin 3 NORTH is incorrect.");
            Assert.AreEqual(0.1946, result[3, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.01, "Bin 3 VERTICAL is incorrect.");
            Assert.AreEqual(0, result[3, DataSet.Ensemble.BEAM_Q_INDEX], 0.01, "Bin 3 Q is incorrect.");
        }

    }
}
