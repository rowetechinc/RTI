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
 * 01/17/2013      RC          2.17       Initial coding
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
    public class ScreenBottomTrackRangeTest
    {

        /// <summary>
        /// Test screening Bottom Track Range data.
        /// </summary>
        [Test]
        public void TestScreenNoBad()
        {
            // Create an ensemble with 30 bins
            DataSet.Ensemble ens = EnsembleHelper.GenerateEnsemble(30);

            ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX] = 12.34f;
            ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX] = 12.45f;
            ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX] = 12.56f;
            ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX] = 12.78f;

            Screen.ScreenBottomTrackRange.ScreenBottomTrackRangeResult result = Screen.ScreenBottomTrackRange.Screen(ref ens);

            Assert.AreEqual(12.34f, ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX], "Range 0 is incorrect.");
            Assert.AreEqual(12.45f, ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX], "Range 1 is incorrect.");
            Assert.AreEqual(12.56f, ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX], "Range 2 is incorrect.");
            Assert.AreEqual(12.78f, ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX], "Range 3 is incorrect.");
        }

        /// <summary>
        /// Test screening Bottom Track Range data.
        /// </summary>
        [Test]
        public void TestScreenBeam0Bad()
        {
            // Create an ensemble with 30 bins
            //DataSet.Ensemble ens = EnsembleHelper.GenerateEnsemble(30);

            //ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX] = 0.135f;
            //ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX] = 0.802f;
            //ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX] = -0.418f;
            //ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX] = DataSet.Ensemble.BAD_RANGE;

            //Screen.ScreenBottomTrackRange.ScreenBottomTrackRangeResult result = Screen.ScreenBottomTrackRange.Screen(ref ens);

            //Assert.AreEqual(DataSet.Ensemble.BAD_RANGE, ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX], "Range 0 is incorrect.");
            //Assert.AreEqual(0.802f, ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX], "Range 1 is incorrect.");
            //Assert.AreEqual(-0.418f, ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX], "Range 2 is incorrect.");
            //Assert.AreEqual(DataSet.Ensemble.BAD_RANGE, ens.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX], "Range 3 is incorrect.");
        }

    }
}
