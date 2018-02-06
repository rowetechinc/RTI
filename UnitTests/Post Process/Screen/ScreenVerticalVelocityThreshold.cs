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
 * 12/24/2012      RC          2.17       Initial coding
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
    /// Test the MathHelper class.
    /// </summary>
    [TestFixture]
    public class ScreenVerticalVelocityThresholdTest
    {
        /// <summary>
        /// Test the screen option.
        /// </summary>
        [Test]
        public void TestScreen()
        {
            int bins = 10;

            // Generate ensemble with blank data
            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(bins);

            // Populate the data
            for (int bin = 0; bin < bins; bin++)
            {
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = 0.1f;
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = 0.2f;
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.01f;
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = 0.0f;
            }

            // Make some high Error Velocities
            int badBin = (int)bins / 2;
            ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 1.2f;

            // Screen the ensemble
            ScreenData.ScreenVerticalVelocityThreshold.Screen(ref ensemble, 1.0f);

            Assert.AreEqual(0.1f, ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_EAST_INDEX], 0.0001, "East Velocity for Good is incorrect.");
            Assert.AreEqual(0.2f, ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.0001, "North Velocity for Good is incorrect.");
            Assert.AreEqual(0.01f, ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.0001, "Veritcal Velocity for Good is incorrect.");
            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_Q_INDEX], 0.0001, "Q Velocity for Good is incorrect.");

            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_EAST_INDEX], 0.0001, "East Velocity for Bad is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.0001, "North Velocity for Bad is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.0001, "Veritcal Velocity for Bad is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_Q_INDEX], 0.0001, "Q Velocity for Bad is incorrect.");
        }

        /// <summary>
        /// Test the screen option.
        /// Thresold equal to error.
        /// </summary>
        [Test]
        public void TestScreenEqual()
        {
            int bins = 10;

            // Generate ensemble with blank data
            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(bins);

            // Populate the data
            for (int bin = 0; bin < bins; bin++)
            {
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = 0.1f;
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = 0.2f;
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.01f;
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = 0.0f;
            }

            // Make some high Error Velocities
            int badBin = (int)bins / 2;
            ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 1.0f;

            // Screen the ensemble
            ScreenData.ScreenVerticalVelocityThreshold.Screen(ref ensemble, 1.0f);

            Assert.AreEqual(0.1f, ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_EAST_INDEX], 0.0001, "East Velocity for Good is incorrect.");
            Assert.AreEqual(0.2f, ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.0001, "North Velocity for Good is incorrect.");
            Assert.AreEqual(0.01f, ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.0001, "Veritcal Velocity for Good is incorrect.");
            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_Q_INDEX], 0.0001, "Q Velocity for Good is incorrect.");

            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_EAST_INDEX], 0.0001, "East Velocity for Bad is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.0001, "North Velocity for Bad is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.0001, "Veritcal Velocity for Bad is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.BAD_VELOCITY, ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_Q_INDEX], 0.0001, "Q Velocity for Bad is incorrect.");
        }

        /// <summary>
        /// Test the screen option.
        /// None over threshold.
        /// </summary>
        [Test]
        public void TestScreenAllGood()
        {
            int bins = 10;

            // Generate ensemble with blank data
            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(bins);

            // Populate the data
            for (int bin = 0; bin < bins; bin++)
            {
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = 0.1f;
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = 0.2f;
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0.01f;
                ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = 0.0f;
            }

            // Make some high Error Velocities
            int badBin = (int)bins / 2;
            ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 1.0f;

            // Screen the ensemble
            ScreenData.ScreenVerticalVelocityThreshold.Screen(ref ensemble, 2.0f);

            Assert.AreEqual(0.1f, ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_EAST_INDEX], 0.0001, "East Velocity for Good is incorrect.");
            Assert.AreEqual(0.2f, ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.0001, "North Velocity for Good is incorrect.");
            Assert.AreEqual(0.01f, ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.0001, "Veritcal Velocity for Good is incorrect.");
            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_Q_INDEX], 0.0001, "Q Velocity for Good is incorrect.");

            Assert.AreEqual(0.1f, ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_EAST_INDEX], 0.0001, "East Velocity for Bad is incorrect.");
            Assert.AreEqual(0.2f, ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_NORTH_INDEX], 0.0001, "North Velocity for Bad is incorrect.");
            Assert.AreEqual(1.0f, ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_VERTICAL_INDEX], 0.0001, "Veritcal Velocity for Bad is incorrect.");
            Assert.AreEqual(0.0f, ensemble.EarthVelocityData.EarthVelocityData[badBin, DataSet.Ensemble.BEAM_Q_INDEX], 0.0001, "Q Velocity for Bad is incorrect.");
        }

    }
}
