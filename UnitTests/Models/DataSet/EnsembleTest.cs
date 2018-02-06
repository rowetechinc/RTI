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
    using System.Diagnostics;

    /// <summary>
    /// Unit test of the Ensemble object.
    /// </summary>
    [TestFixture]
    public class EnsembleTest
    {
        /// <summary>
        /// Test that takes a Prti01Sentence and sets all the values.
        /// </summary>
        [Test]
        public void TestPrti01()
        {

        }

        /// <summary>
        /// Test cloning the ensemble.
        /// </summary>
        [Test]
        public void TestClone()
        {
            // Generate an Ensemble
            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);
            ensemble.EnsembleData.EnsembleNumber = 22;

            DataSet.Ensemble clone = ensemble.Clone();

            Assert.AreEqual(22, ensemble.EnsembleData.EnsembleNumber, "Ensemble Number is incorrect.");
            Assert.AreEqual(22, clone.EnsembleData.EnsembleNumber, "Cloned Ensemble Number is incorrect.");
        }

        /// <summary>
        /// Clone only an Ensemble Dataset.
        /// </summary>
        [Test]
        public void TestCloneEnsembleDataSet()
        {
            DataSet.Ensemble ensemble = new DataSet.Ensemble();
            EnsembleHelper.AddEnsemble(ref ensemble, 20);
            ensemble.EnsembleData.EnsembleNumber = 22;

            DataSet.Ensemble clone = ensemble.Clone();

            Assert.AreEqual(22, ensemble.EnsembleData.EnsembleNumber, "Ensemble Number is incorrect.");
            Assert.AreEqual(22, clone.EnsembleData.EnsembleNumber, "Cloned Ensemble Number is incorrect.");
        }

        /// <summary>
        /// Test converting the ensemble to a JSON string.
        /// </summary>
        [Test]
        public void TestJson()
        {
            // Generate an Ensemble
            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);

            // Modify the data
            #region Beam Velocity
            ensemble.BeamVelocityData.BeamVelocityData[0, DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
            ensemble.BeamVelocityData.BeamVelocityData[0, DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
            ensemble.BeamVelocityData.BeamVelocityData[0, DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
            ensemble.BeamVelocityData.BeamVelocityData[0, DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

            ensemble.BeamVelocityData.BeamVelocityData[3, DataSet.Ensemble.BEAM_0_INDEX] = 2.2f;
            ensemble.BeamVelocityData.BeamVelocityData[3, DataSet.Ensemble.BEAM_1_INDEX] = 3.3f;
            ensemble.BeamVelocityData.BeamVelocityData[3, DataSet.Ensemble.BEAM_2_INDEX] = 4.4f;
            ensemble.BeamVelocityData.BeamVelocityData[3, DataSet.Ensemble.BEAM_3_INDEX] = 5.5f;

            ensemble.BeamVelocityData.BeamVelocityData[5, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
            ensemble.BeamVelocityData.BeamVelocityData[5, DataSet.Ensemble.BEAM_1_INDEX] = 4.3f;
            ensemble.BeamVelocityData.BeamVelocityData[5, DataSet.Ensemble.BEAM_2_INDEX] = 5.4f;
            ensemble.BeamVelocityData.BeamVelocityData[5, DataSet.Ensemble.BEAM_3_INDEX] = 6.5f;
            #endregion

            #region Amplitude
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

            string encodedEns = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble);                                      // Serialize to JSON
            DataSet.Ensemble decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.Ensemble>(encodedEns);      // Deserialize the JSON

            Assert.AreEqual(true, ensemble.IsEnsembleAvail, "IsEnsembleAvail is incorrect.");

            // Verify the values are the same
            #region Beam Velocity
            Assert.AreEqual(1.2f, decodedEns.BeamVelocityData.BeamVelocityData[0, DataSet.Ensemble.BEAM_0_INDEX], "Beam Vel Data 0 0 is incorrect.");
            Assert.AreEqual(2.3f, decodedEns.BeamVelocityData.BeamVelocityData[0, DataSet.Ensemble.BEAM_1_INDEX], "Beam Vel Data 0 1 is incorrect.");
            Assert.AreEqual(3.4f, decodedEns.BeamVelocityData.BeamVelocityData[0, DataSet.Ensemble.BEAM_2_INDEX], "Beam Vel Data 0 2 is incorrect.");
            Assert.AreEqual(4.5f, decodedEns.BeamVelocityData.BeamVelocityData[0, DataSet.Ensemble.BEAM_3_INDEX], "Beam Vel Data 0 3 is incorrect.");

            Assert.AreEqual(2.2f, decodedEns.BeamVelocityData.BeamVelocityData[3, DataSet.Ensemble.BEAM_0_INDEX], "Beam Vel Data 3 0 is incorrect.");
            Assert.AreEqual(3.3f, decodedEns.BeamVelocityData.BeamVelocityData[3, DataSet.Ensemble.BEAM_1_INDEX], "Beam Vel Data 3 1 is incorrect.");
            Assert.AreEqual(4.4f, decodedEns.BeamVelocityData.BeamVelocityData[3, DataSet.Ensemble.BEAM_2_INDEX], "Beam Vel Data 3 2 is incorrect.");
            Assert.AreEqual(5.5f, decodedEns.BeamVelocityData.BeamVelocityData[3, DataSet.Ensemble.BEAM_3_INDEX], "Beam Vel Data 3 3 is incorrect.");
            Assert.AreEqual(3.2f, decodedEns.BeamVelocityData.BeamVelocityData[5, DataSet.Ensemble.BEAM_0_INDEX], "Beam Vel Data 5 0 is incorrect.");
            Assert.AreEqual(4.3f, decodedEns.BeamVelocityData.BeamVelocityData[5, DataSet.Ensemble.BEAM_1_INDEX], "Beam Vel Data 5 1 is incorrect.");
            Assert.AreEqual(5.4f, decodedEns.BeamVelocityData.BeamVelocityData[5, DataSet.Ensemble.BEAM_2_INDEX], "Beam Vel Data 5 2 is incorrect.");
            Assert.AreEqual(6.5f, decodedEns.BeamVelocityData.BeamVelocityData[5, DataSet.Ensemble.BEAM_3_INDEX], "Beam Vel Data 5 3 is incorrect.");
            #endregion

            #region Amplitude
            Assert.AreEqual(1.2f, decodedEns.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 0 0 is incorrect.");
            Assert.AreEqual(2.3f, decodedEns.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 0 1 is incorrect.");
            Assert.AreEqual(3.4f, decodedEns.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 0 2 is incorrect.");
            Assert.AreEqual(4.5f, decodedEns.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 0 3 is incorrect.");

            Assert.AreEqual(2.2f, decodedEns.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 3 0 is incorrect.");
            Assert.AreEqual(3.3f, decodedEns.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 3 1 is incorrect.");
            Assert.AreEqual(4.4f, decodedEns.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 3 2 is incorrect.");
            Assert.AreEqual(5.5f, decodedEns.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 3 3 is incorrect.");

            Assert.AreEqual(3.2f, decodedEns.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 5 0 is incorrect.");
            Assert.AreEqual(4.3f, decodedEns.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 5 1 is incorrect.");
            Assert.AreEqual(5.4f, decodedEns.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 5 2 is incorrect.");
            Assert.AreEqual(6.5f, decodedEns.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 5 3 is incorrect.");
            #endregion
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
                string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble);
            }
            watch.Stop();
            long resultSerialize = watch.ElapsedMilliseconds;

            // Test Deserialize()
            string encodedd = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble);
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                DataSet.Ensemble decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.Ensemble>(encodedd);
            }
            watch.Stop();
            long resultDeserialize = watch.ElapsedMilliseconds;

            Debug.WriteLine(String.Format("Serialize:{0}  Deserialize:{1}", resultSerialize, resultDeserialize));

            Debug.WriteLine("Complete");

        }

        #region PD0 Decode

        /// <summary>
        /// Test decoding PD0 ensemble to RTI ensemble.
        /// </summary>
        [Test]
        public void DecodePd0Test()
        {
            PD0 pd0 = new PD0();
            pd0.FixedLeader.SetCoordinateTransform(PD0.CoordinateTransforms.Coord_Earth);
            pd0.AddDataType(new Pd0Correlation(30));
            pd0.AddDataType(new Pd0BottomTrack());
            pd0.AddDataType(new Pd0EchoIntensity(30));
            pd0.AddDataType(new Pd0PercentGood(30));
            pd0.AddDataType(new Pd0Velocity(30));

            DataSet.Ensemble ens = new DataSet.Ensemble(pd0);

            Assert.IsTrue(ens.IsCorrelationAvail, "IsCorrelationAvail is incorrect.");
            Assert.IsTrue(ens.IsBottomTrackAvail, "IsBottomTrackAvail is incorrect.");
            Assert.IsTrue(ens.IsAmplitudeAvail, "IsAmplitudeAvail is incorrect.");
            Assert.IsTrue(ens.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.IsFalse(ens.IsGoodBeamAvail, "IsGoodBeamAvail is incorrect.");
            Assert.IsFalse(ens.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.IsTrue(ens.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.IsFalse(ens.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.IsFalse(ens.IsInstrumentWaterMassAvail, "IsInstrumentWaterMassAvail is incorrect.");
            Assert.IsFalse(ens.IsNmeaAvail, "IsNmeaAvail is incorrect.");
            Assert.IsFalse(ens.IsProfileEngineeringAvail, "IsProfileEngineeringAvail is incorrect.");
            Assert.IsFalse(ens.IsBottomTrackEngineeringAvail, "IsBottomTrackEngineeringAvail is incorrect.");
            Assert.IsFalse(ens.IsSystemSetupAvail, "IsSystemSetupAvail is incorrect.");
        }

        #endregion

    }

}