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
    /// Test the AverageAmplitude class.
    /// </summary>
    [TestFixture]
    public class AverageAmplitudeTest
    {
        /// <summary>
        /// Test a basic average
        /// </summary>
        [Test]
        public void TestAverage()
        {
            Average.AverageAmplitude avgAmp = new Average.AverageAmplitude();

            // Create an ensemble
            // All zeros for correlation
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);

            avgAmp.AddEnsemble(ens1);
            avgAmp.AddEnsemble(ens2);

            float[,] result = avgAmp.GetAverage();

            Assert.IsNotNull(result, "Result is incorrect.");
            Assert.AreEqual(0, result[0, DataSet.Ensemble.BEAM_0_INDEX], "Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(0, result[0, DataSet.Ensemble.BEAM_1_INDEX], "Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(0, result[0, DataSet.Ensemble.BEAM_2_INDEX], "Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(0, result[0, DataSet.Ensemble.BEAM_3_INDEX], "Bin 0 Beam 3 is incorrect.");
        }

        /// <summary>
        /// Test a basic average with the value for the first bin.
        /// </summary>
        [Test]
        public void TestAverageFirstBin()
        {
            Average.AverageAmplitude avgAmp = new Average.AverageAmplitude();

            // Create an ensemble
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5;

            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5;

            avgAmp.AddEnsemble(ens1);
            avgAmp.AddEnsemble(ens2);

            float[,] result = avgAmp.GetAverage();

            Assert.IsNotNull(result, "Result is incorrect.");
            Assert.AreEqual(2, result[0, DataSet.Ensemble.BEAM_0_INDEX], "Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(3, result[0, DataSet.Ensemble.BEAM_1_INDEX], "Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(4, result[0, DataSet.Ensemble.BEAM_2_INDEX], "Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(5, result[0, DataSet.Ensemble.BEAM_3_INDEX], "Bin 0 Beam 3 is incorrect.");
        }

        /// <summary>
        /// Test a basic average with the value for the first bin with bad velocity.
        /// </summary>
        [Test]
        public void TestAverageBadValue()
        {
            Average.AverageAmplitude avgAmp = new Average.AverageAmplitude();

            // Create an ensemble
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5;

            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = DataSet.Ensemble.BAD_VELOCITY;

            avgAmp.AddEnsemble(ens1);
            avgAmp.AddEnsemble(ens2);

            float[,] result = avgAmp.GetAverage();

            Assert.IsNotNull(result, "Result is incorrect.");
            Assert.AreEqual(2, result[0, DataSet.Ensemble.BEAM_0_INDEX], "Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(3, result[0, DataSet.Ensemble.BEAM_1_INDEX], "Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(4, result[0, DataSet.Ensemble.BEAM_2_INDEX], "Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(5, result[0, DataSet.Ensemble.BEAM_3_INDEX], "Bin 0 Beam 3 is incorrect.");
        }

        /// <summary>
        /// Test a Number of samples
        /// </summary>
        [Test]
        public void TestAverageNumSamples()
        {
            Average.AverageAmplitude avgAmp = new Average.AverageAmplitude();
            avgAmp.NumSamples = 2;

            // Create an ensemble
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5;

            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = DataSet.Ensemble.BAD_VELOCITY;

            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;

            avgAmp.AddEnsemble(ens1);
            avgAmp.AddEnsemble(ens2);
            avgAmp.AddEnsemble(ens3);

            float[,] result = avgAmp.GetAverage();

            Assert.IsNotNull(result, "Result is incorrect.");
            Assert.AreEqual(4, result[0, DataSet.Ensemble.BEAM_0_INDEX], "Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(5, result[0, DataSet.Ensemble.BEAM_1_INDEX], "Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(6, result[0, DataSet.Ensemble.BEAM_2_INDEX], "Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(7, result[0, DataSet.Ensemble.BEAM_3_INDEX], "Bin 0 Beam 3 is incorrect.");
        }

        /// <summary>
        /// Test a Number of samples
        /// </summary>
        [Test]
        public void TestRunningAverageNumSamples()
        {
            Average.AverageAmplitude avgAmp = new Average.AverageAmplitude();
            avgAmp.NumSamples = 2;
            avgAmp.IsRunningAverage = true;

            // Create an ensemble
            DataSet.Ensemble ens1 = EnsembleHelper.GenerateEnsemble(30);
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ens1.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 5;

            DataSet.Ensemble ens2 = EnsembleHelper.GenerateEnsemble(30);
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            ens2.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = DataSet.Ensemble.BAD_VELOCITY;

            DataSet.Ensemble ens3 = EnsembleHelper.GenerateEnsemble(30);
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 6;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 7;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 8;
            ens3.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 9;

            avgAmp.AddEnsemble(ens1);
            avgAmp.AddEnsemble(ens2);
            avgAmp.AddEnsemble(ens3);

            float[,] result = avgAmp.GetAverage();

            Assert.IsNotNull(result, "Result is incorrect.");
            Assert.AreEqual(4, result[0, DataSet.Ensemble.BEAM_0_INDEX], "Bin 0 Beam 0 is incorrect.");
            Assert.AreEqual(5, result[0, DataSet.Ensemble.BEAM_1_INDEX], "Bin 0 Beam 1 is incorrect.");
            Assert.AreEqual(6, result[0, DataSet.Ensemble.BEAM_2_INDEX], "Bin 0 Beam 2 is incorrect.");
            Assert.AreEqual(7, result[0, DataSet.Ensemble.BEAM_3_INDEX], "Bin 0 Beam 3 is incorrect.");
        }
    }
}
