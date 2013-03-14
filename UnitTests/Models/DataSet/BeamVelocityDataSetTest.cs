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
 * 02/25/2013      RC          2.18      Initial coding.
 *       
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Diagnostics;

    /// <summary>
    /// Unit test of the Beam Velocity DataSet object.
    /// </summary>
    [TestFixture]
    public class BeamVelocityDataSetTest
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
            adcpData.AddBeamVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,                   // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.BeamVelocityID);                   // Dataset ID

            Assert.IsTrue(adcpData.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.BeamVelocityData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, adcpData.BeamVelocityData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.BeamVelocityData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.BeamVelocityData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.BeamVelocityData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.BeamVelocityID, adcpData.BeamVelocityData.Name, "Name is incorrect.");

            Assert.AreEqual(adcpData.BeamVelocityData.BeamVelocityData.GetLength(0), 30, "Beam Velocity Array Dimension 1 is incorrect.");
            Assert.AreEqual(adcpData.BeamVelocityData.BeamVelocityData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Beam Velocity Array Dimension 2 is incorrect.");

        }

        /// <summary>
        /// Test Encode and decode.  
        /// Encode the Beam Velocity data.  Then
        /// create a new ensemble and decode the
        /// encoded data.
        /// </summary>
        [Test]
        public void TestEncodeDecode()
        {
            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Sentence to data set
            adcpData.AddBeamVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,                   // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.BeamVelocityID);                   // Dataset ID

            Assert.IsTrue(adcpData.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.BeamVelocityData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, adcpData.BeamVelocityData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.BeamVelocityData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.BeamVelocityData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.BeamVelocityData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.BeamVelocityID, adcpData.BeamVelocityData.Name, "Name is incorrect.");

            Assert.AreEqual(adcpData.BeamVelocityData.BeamVelocityData.GetLength(0), 30, "Beam Velocity Array Dimension 1 is incorrect.");
            Assert.AreEqual(adcpData.BeamVelocityData.BeamVelocityData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Beam Velocity Array Dimension 2 is incorrect.");

            // Modify the array
            adcpData.BeamVelocityData.BeamVelocityData[0, 0] = 2.3f;
            adcpData.BeamVelocityData.BeamVelocityData[0, 1] = 3.4f;
            adcpData.BeamVelocityData.BeamVelocityData[0, 2] = 4.5f;
            adcpData.BeamVelocityData.BeamVelocityData[0, 3] = 5.6f;
            adcpData.BeamVelocityData.BeamVelocityData[1, 0] = 6.7f;
            adcpData.BeamVelocityData.BeamVelocityData[1, 1] = 7.8f;
            adcpData.BeamVelocityData.BeamVelocityData[1, 2] = 8.9f;
            adcpData.BeamVelocityData.BeamVelocityData[1, 3] = 9.10f;
            adcpData.BeamVelocityData.BeamVelocityData[2, 0] = 10.11f;
            adcpData.BeamVelocityData.BeamVelocityData[2, 1] = 11.12f;
            adcpData.BeamVelocityData.BeamVelocityData[2, 2] = 12.13f;

            // Encode the data
            byte[] encode = adcpData.BeamVelocityData.Encode();


            // Create dataset
            DataSet.Ensemble ens1 = new DataSet.Ensemble();

            // Add Sentence to data set
            ens1.AddBeamVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,                       // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM,        // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.BeamVelocityID,                    // Dataset ID
                                            encode);                                        // Encoded data

            Assert.IsTrue(ens1.IsBeamVelocityAvail, "IsBeamVelocityAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, ens1.BeamVelocityData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, ens1.BeamVelocityData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ens1.BeamVelocityData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, ens1.BeamVelocityData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, ens1.BeamVelocityData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.BeamVelocityID, ens1.BeamVelocityData.Name, "Name is incorrect.");

            Assert.AreEqual(ens1.BeamVelocityData.BeamVelocityData.GetLength(0), 30, "Beam Velocity Array Dimension 1 is incorrect.");
            Assert.AreEqual(ens1.BeamVelocityData.BeamVelocityData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Beam Velocity Array Dimension 2 is incorrect.");

            Assert.AreEqual(2.3f, ens1.BeamVelocityData.BeamVelocityData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens1.BeamVelocityData.BeamVelocityData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens1.BeamVelocityData.BeamVelocityData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens1.BeamVelocityData.BeamVelocityData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens1.BeamVelocityData.BeamVelocityData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens1.BeamVelocityData.BeamVelocityData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens1.BeamVelocityData.BeamVelocityData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens1.BeamVelocityData.BeamVelocityData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens1.BeamVelocityData.BeamVelocityData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens1.BeamVelocityData.BeamVelocityData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens1.BeamVelocityData.BeamVelocityData[2, 2], "2,2 Data is incorrect.");
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

            string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.BeamVelocityData);                                      // Serialize object to JSON
            DataSet.BeamVelocityDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.BeamVelocityDataSet>(encoded);       // Deserialize the JSON

            // Verify the values are the same
            Assert.AreEqual(1.2f, decoded.BeamVelocityData[0, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 0 0 is incorrect.");
            Assert.AreEqual(2.3f, decoded.BeamVelocityData[0, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 0 1 is incorrect.");
            Assert.AreEqual(3.4f, decoded.BeamVelocityData[0, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 0 2 is incorrect.");
            Assert.AreEqual(4.5f, decoded.BeamVelocityData[0, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 0 3 is incorrect.");

            Assert.AreEqual(2.2f, decoded.BeamVelocityData[3, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 3 0 is incorrect.");
            Assert.AreEqual(3.3f, decoded.BeamVelocityData[3, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 3 1 is incorrect.");
            Assert.AreEqual(4.4f, decoded.BeamVelocityData[3, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 3 2 is incorrect.");
            Assert.AreEqual(5.5f, decoded.BeamVelocityData[3, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 3 3 is incorrect.");

            Assert.AreEqual(3.2f, decoded.BeamVelocityData[5, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 5 0 is incorrect.");
            Assert.AreEqual(4.3f, decoded.BeamVelocityData[5, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 5 1 is incorrect.");
            Assert.AreEqual(5.4f, decoded.BeamVelocityData[5, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 5 2 is incorrect.");
            Assert.AreEqual(6.5f, decoded.BeamVelocityData[5, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 5 3 is incorrect.");
        }

        ///// <summary>
        ///// Test encoding and decoding to JSON using ToJson().
        ///// </summary>
        //[Test]
        //public void TestToJson()
        //{
        //    // Generate an Ensemble
        //    DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);

        //    // Modify the data
        //    ensemble.BeamVelocityData.BeamVelocityData[0, DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
        //    ensemble.BeamVelocityData.BeamVelocityData[0, DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
        //    ensemble.BeamVelocityData.BeamVelocityData[0, DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
        //    ensemble.BeamVelocityData.BeamVelocityData[0, DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

        //    ensemble.BeamVelocityData.BeamVelocityData[3, DataSet.Ensemble.BEAM_0_INDEX] = 2.2f;
        //    ensemble.BeamVelocityData.BeamVelocityData[3, DataSet.Ensemble.BEAM_1_INDEX] = 3.3f;
        //    ensemble.BeamVelocityData.BeamVelocityData[3, DataSet.Ensemble.BEAM_2_INDEX] = 4.4f;
        //    ensemble.BeamVelocityData.BeamVelocityData[3, DataSet.Ensemble.BEAM_3_INDEX] = 5.5f;

        //    ensemble.BeamVelocityData.BeamVelocityData[5, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
        //    ensemble.BeamVelocityData.BeamVelocityData[5, DataSet.Ensemble.BEAM_1_INDEX] = 4.3f;
        //    ensemble.BeamVelocityData.BeamVelocityData[5, DataSet.Ensemble.BEAM_2_INDEX] = 5.4f;
        //    ensemble.BeamVelocityData.BeamVelocityData[5, DataSet.Ensemble.BEAM_3_INDEX] = 6.5f;

        //    Assert.AreEqual(30, ensemble.BeamVelocityData.NumElements, "Pre Number of elements is incorrect.");
        //    Assert.AreEqual(30, ensemble.BeamVelocityData.BeamVelocityData.GetLength(0), "Pre Number of bins is incorrect.");

        //    string encoded = ensemble.BeamVelocityData.ToJson();                                                                          // Serialize object to JSON
        //    DataSet.BeamVelocityDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.BeamVelocityDataSet>(encoded);    // Deserialize the JSON

        //    Assert.AreEqual(30, decoded.NumElements, "Number of elements is incorrect.");
        //    Assert.AreEqual(30, decoded.BeamVelocityData.GetLength(0), "Number of bins is incorrect.");

        //    // Verify the values are the same
        //    Assert.AreEqual(1.2f, decoded.BeamVelocityData[0, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 0 0 is incorrect.");
        //    Assert.AreEqual(2.3f, decoded.BeamVelocityData[0, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 0 1 is incorrect.");
        //    Assert.AreEqual(3.4f, decoded.BeamVelocityData[0, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 0 2 is incorrect.");
        //    Assert.AreEqual(4.5f, decoded.BeamVelocityData[0, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 0 3 is incorrect.");

        //    Assert.AreEqual(2.2f, decoded.BeamVelocityData[3, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 3 0 is incorrect.");
        //    Assert.AreEqual(3.3f, decoded.BeamVelocityData[3, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 3 1 is incorrect.");
        //    Assert.AreEqual(4.4f, decoded.BeamVelocityData[3, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 3 2 is incorrect.");
        //    Assert.AreEqual(5.5f, decoded.BeamVelocityData[3, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 3 3 is incorrect.");

        //    Assert.AreEqual(3.2f, decoded.BeamVelocityData[5, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 5 0 is incorrect.");
        //    Assert.AreEqual(4.3f, decoded.BeamVelocityData[5, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 5 1 is incorrect.");
        //    Assert.AreEqual(5.4f, decoded.BeamVelocityData[5, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 5 2 is incorrect.");
        //    Assert.AreEqual(6.5f, decoded.BeamVelocityData[5, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 5 3 is incorrect.");
        //}

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

            //// Test ToJson()
            //watch.Start();
            //for (int x = 0; x < 1000; x++)
            //{
            //    string encodedAmp = ensemble.BeamVelocityData.ToJson();
            //}
            //watch.Stop();
            //long resultToJson = watch.ElapsedMilliseconds;


            // Test Serialize()
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.BeamVelocityData);
            }
            watch.Stop();
            long resultSerialize = watch.ElapsedMilliseconds;

            //// Test JSON Constructor
            string encodedd = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.BeamVelocityData);
            //watch = new Stopwatch();
            //watch.Start();
            //for (int x = 0; x < 1000; x++)
            //{
            //    DataSet.AmplitudeDataSet decodedAmp = new DataSet.AmplitudeDataSet(encodedAmpp);
            //}
            //watch.Stop();
            //long resultJsonConstructor = watch.ElapsedMilliseconds;

            // Test Deserialize()
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                DataSet.BeamVelocityDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.BeamVelocityDataSet>(encodedd);
            }
            watch.Stop();
            long resultDeserialize = watch.ElapsedMilliseconds;

            Debug.WriteLine(String.Format("Serialize:{0}  Deserialize:{1}", resultSerialize, resultDeserialize));

            Debug.WriteLine("Complete");

        }
    }
}