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
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Diagnostics;

    /// <summary>
    /// Unit test of the Amplitude DataSet object.
    /// </summary>
    [TestFixture]
    public class AmplitudeDataSetTest
    {
        /// <summary>
        /// Test the constructor that takes no data.
        /// </summary>
        [Test]
        public void TestEmptyConstructor()
        {
            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Sentence to data set
            adcpData.AddAmplitudeData(DataSet.Ensemble.DATATYPE_FLOAT,                    // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.AmplitudeID);                  // Dataset ID

            Assert.IsTrue(adcpData.IsAmplitudeAvail, "IsAmplitude is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.AmplitudeData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, adcpData.AmplitudeData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.AmplitudeData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.AmplitudeData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.AmplitudeData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.AmplitudeID, adcpData.AmplitudeData.Name, "Name is incorrect.");

            Assert.AreEqual(adcpData.AmplitudeData.AmplitudeData.GetLength(0), 30, "Amplitude Array Dimension 1 is incorrect.");
            Assert.AreEqual(adcpData.AmplitudeData.AmplitudeData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Amplitude Array Dimension 2 is incorrect.");

        }

        /// <summary>
        /// Test Encode and decode.  
        /// Encode the correlation data.  Then
        /// create a new ensemble and decode the
        /// encoded data.
        /// </summary>
        [Test]
        public void TestEncodeDecode()
        {
            // Create dataset
            DataSet.Ensemble ensemble = new DataSet.Ensemble();

            // Add Sentence to data set
            ensemble.AddAmplitudeData(DataSet.Ensemble.DATATYPE_FLOAT,                    // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.CorrelationID);               // Dataset ID

            Assert.IsTrue(ensemble.IsAmplitudeAvail, "IsAmplitude is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, ensemble.AmplitudeData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, ensemble.AmplitudeData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.AmplitudeData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, ensemble.AmplitudeData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, ensemble.AmplitudeData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.CorrelationID, ensemble.AmplitudeData.Name, "Name is incorrect.");

            Assert.AreEqual(ensemble.AmplitudeData.AmplitudeData.GetLength(0), 30, "Amplitude Array Dimension 1 is incorrect.");
            Assert.AreEqual(ensemble.AmplitudeData.AmplitudeData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Amplitude Array Dimension 2 is incorrect.");

            // Modify the array
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

            // Encode the data
            byte[] encode = ensemble.AmplitudeData.Encode();


            // Create dataset
            DataSet.Ensemble ens0 = new DataSet.Ensemble();

            // Add Sentence to data set
            ens0.AddAmplitudeData(DataSet.Ensemble.DATATYPE_FLOAT,                        // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM,        // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.CorrelationID,                 // Dataset ID
                                            encode);                                        // Encoded data

            Assert.IsTrue(ens0.IsAmplitudeAvail, "IsAmplitude is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, ens0.AmplitudeData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, ens0.AmplitudeData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ens0.AmplitudeData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, ens0.AmplitudeData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, ens0.AmplitudeData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.CorrelationID, ens0.AmplitudeData.Name, "Name is incorrect.");

            Assert.AreEqual(ens0.AmplitudeData.AmplitudeData.GetLength(0), 30, "Correlation Array Dimension 1 is incorrect.");
            Assert.AreEqual(ens0.AmplitudeData.AmplitudeData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Correlation Array Dimension 2 is incorrect.");

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

            string encodedAmp = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.AmplitudeData);                                      // Serialize object to JSON
            DataSet.AmplitudeDataSet decodedAmp = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.AmplitudeDataSet>(encodedAmp);    // Deserialize the JSON

            // Verify the values are the same
            Assert.AreEqual(1.2f, decodedAmp.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 0 0 is incorrect.");
            Assert.AreEqual(2.3f, decodedAmp.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 0 1 is incorrect.");
            Assert.AreEqual(3.4f, decodedAmp.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 0 2 is incorrect.");
            Assert.AreEqual(4.5f, decodedAmp.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 0 3 is incorrect.");

            Assert.AreEqual(2.2f, decodedAmp.AmplitudeData[3, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 3 0 is incorrect.");
            Assert.AreEqual(3.3f, decodedAmp.AmplitudeData[3, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 3 1 is incorrect.");
            Assert.AreEqual(4.4f, decodedAmp.AmplitudeData[3, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 3 2 is incorrect.");
            Assert.AreEqual(5.5f, decodedAmp.AmplitudeData[3, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 3 3 is incorrect.");

            Assert.AreEqual(3.2f, decodedAmp.AmplitudeData[5, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 5 0 is incorrect.");
            Assert.AreEqual(4.3f, decodedAmp.AmplitudeData[5, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 5 1 is incorrect.");
            Assert.AreEqual(5.4f, decodedAmp.AmplitudeData[5, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 5 2 is incorrect.");
            Assert.AreEqual(6.5f, decodedAmp.AmplitudeData[5, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 5 3 is incorrect.");
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
                string encodeAmp = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.AmplitudeData);
            }
            watch.Stop();
            long resultSerialize = watch.ElapsedMilliseconds;

            // Test Deserialize()
            string encodedAmpp = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.AmplitudeData);
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                DataSet.AmplitudeDataSet decodedAmp = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.AmplitudeDataSet>(encodedAmpp);
            }
            watch.Stop();
            long resultDeserialize = watch.ElapsedMilliseconds;

            Debug.WriteLine(String.Format("Serialize:{0}  Deserialize:{1}", resultSerialize, resultDeserialize));

            Debug.WriteLine("Complete");

        }

        #region PD0 Decode

        /// <summary>
        /// Test decoding PD0 Echo Intensity data to RTI Amplitude data.
        /// </summary>
        [Test]
        public void DecodePd0Test()
        {
            Pd0EchoIntensity pd0Ei = new Pd0EchoIntensity(30);

            pd0Ei.EchoIntensity[0, 0] = 68;
            pd0Ei.EchoIntensity[0, 1] = 46;
            pd0Ei.EchoIntensity[0, 2] = 90;
            pd0Ei.EchoIntensity[0, 3] = 112;
            pd0Ei.EchoIntensity[1, 0] = 134;
            pd0Ei.EchoIntensity[1, 1] = 156;
            pd0Ei.EchoIntensity[1, 2] = 178;
            pd0Ei.EchoIntensity[1, 3] = 182;


            DataSet.AmplitudeDataSet amp = new DataSet.AmplitudeDataSet(30);

            amp.DecodePd0Ensemble(pd0Ei);

            Assert.AreEqual(45f, amp.AmplitudeData[0, 0], "Amplitude Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(56f, amp.AmplitudeData[0, 1], "Amplitude Bin 0, Beam 1 is incorrect.");
            Assert.AreEqual(23f, amp.AmplitudeData[0, 2], "Amplitude Bin 0, Beam 2 is incorrect.");
            Assert.AreEqual(34f, amp.AmplitudeData[0, 3], "Amplitude Bin 0, Beam 3 is incorrect.");
            Assert.AreEqual(89f, amp.AmplitudeData[1, 0], "Amplitude Bin 1, Beam 0 is incorrect.");
            Assert.AreEqual(91f, amp.AmplitudeData[1, 1], "Amplitude Bin 1, Beam 1 is incorrect.");
            Assert.AreEqual(78f, amp.AmplitudeData[1, 2], "Amplitude Bin 1, Beam 2 is incorrect.");
            Assert.AreEqual(67f, amp.AmplitudeData[1, 3], "Amplitude Bin 1, Beam 3 is incorrect.");
        }

        #endregion
    }
}
