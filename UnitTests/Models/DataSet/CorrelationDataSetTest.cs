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
 * 02/25/2013      RC          2.18       Added JSON Test.
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Diagnostics;

    /// <summary>
    /// Unit test of the Correlation DataSet object.
    /// </summary>
    [TestFixture]
    public class CorrelationDataSetTest
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
            adcpData.AddCorrelationData(DataSet.Ensemble.DATATYPE_FLOAT,                    // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.CorrelationID);               // Dataset ID

            Assert.IsTrue(adcpData.IsCorrelationAvail, "IsCorrelation is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.CorrelationData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, adcpData.CorrelationData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.CorrelationData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.CorrelationData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.CorrelationData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.CorrelationID, adcpData.CorrelationData.Name, "Name is incorrect.");

            Assert.AreEqual(adcpData.CorrelationData.CorrelationData.GetLength(0), 30, "Correlation Array Dimension 1 is incorrect.");
            Assert.AreEqual(adcpData.CorrelationData.CorrelationData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Correlation Array Dimension 2 is incorrect.");

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
            ensemble.AddCorrelationData(DataSet.Ensemble.DATATYPE_FLOAT,                    // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.CorrelationID);               // Dataset ID

            Assert.IsTrue(ensemble.IsCorrelationAvail, "IsCorrelation is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, ensemble.CorrelationData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, ensemble.CorrelationData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.CorrelationData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, ensemble.CorrelationData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, ensemble.CorrelationData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.CorrelationID, ensemble.CorrelationData.Name, "Name is incorrect.");

            Assert.AreEqual(ensemble.CorrelationData.CorrelationData.GetLength(0), 30, "Correlation Array Dimension 1 is incorrect.");
            Assert.AreEqual(ensemble.CorrelationData.CorrelationData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Correlation Array Dimension 2 is incorrect.");

            // Modify the array
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

            // Encode the data
            byte[] encode = ensemble.CorrelationData.Encode();


            // Create dataset
            DataSet.Ensemble ens0 = new DataSet.Ensemble();

            // Add Sentence to data set
            ens0.AddCorrelationData(DataSet.Ensemble.DATATYPE_FLOAT,                        // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM,        // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.CorrelationID,                 // Dataset ID
                                            encode);                                        // Encoded data

            Assert.IsTrue(ens0.IsCorrelationAvail, "IsCorrelation is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, ens0.CorrelationData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, ens0.CorrelationData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ens0.CorrelationData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, ens0.CorrelationData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, ens0.CorrelationData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.CorrelationID, ens0.CorrelationData.Name, "Name is incorrect.");

            Assert.AreEqual(ens0.CorrelationData.CorrelationData.GetLength(0), 30, "Correlation Array Dimension 1 is incorrect.");
            Assert.AreEqual(ens0.CorrelationData.CorrelationData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Correlation Array Dimension 2 is incorrect.");

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
            ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
            ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
            ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
            ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

            ensemble.CorrelationData.CorrelationData[3, DataSet.Ensemble.BEAM_0_INDEX] = 2.2f;
            ensemble.CorrelationData.CorrelationData[3, DataSet.Ensemble.BEAM_1_INDEX] = 3.3f;
            ensemble.CorrelationData.CorrelationData[3, DataSet.Ensemble.BEAM_2_INDEX] = 4.4f;
            ensemble.CorrelationData.CorrelationData[3, DataSet.Ensemble.BEAM_3_INDEX] = 5.5f;

            ensemble.CorrelationData.CorrelationData[5, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
            ensemble.CorrelationData.CorrelationData[5, DataSet.Ensemble.BEAM_1_INDEX] = 4.3f;
            ensemble.CorrelationData.CorrelationData[5, DataSet.Ensemble.BEAM_2_INDEX] = 5.4f;
            ensemble.CorrelationData.CorrelationData[5, DataSet.Ensemble.BEAM_3_INDEX] = 6.5f;

            string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.CorrelationData);                                      // Serialize object to JSON
            DataSet.CorrelationDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.CorrelationDataSet>(encoded);       // Deserialize the JSON

            // Verify the values are the same
            Assert.AreEqual(1.2f, decoded.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 0 0 is incorrect.");
            Assert.AreEqual(2.3f, decoded.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 0 1 is incorrect.");
            Assert.AreEqual(3.4f, decoded.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 0 2 is incorrect.");
            Assert.AreEqual(4.5f, decoded.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 0 3 is incorrect.");

            Assert.AreEqual(2.2f, decoded.CorrelationData[3, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 3 0 is incorrect.");
            Assert.AreEqual(3.3f, decoded.CorrelationData[3, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 3 1 is incorrect.");
            Assert.AreEqual(4.4f, decoded.CorrelationData[3, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 3 2 is incorrect.");
            Assert.AreEqual(5.5f, decoded.CorrelationData[3, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 3 3 is incorrect.");

            Assert.AreEqual(3.2f, decoded.CorrelationData[5, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 5 0 is incorrect.");
            Assert.AreEqual(4.3f, decoded.CorrelationData[5, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 5 1 is incorrect.");
            Assert.AreEqual(5.4f, decoded.CorrelationData[5, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 5 2 is incorrect.");
            Assert.AreEqual(6.5f, decoded.CorrelationData[5, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 5 3 is incorrect.");
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
        //    ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
        //    ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
        //    ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
        //    ensemble.CorrelationData.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

        //    ensemble.CorrelationData.CorrelationData[3, DataSet.Ensemble.BEAM_0_INDEX] = 2.2f;
        //    ensemble.CorrelationData.CorrelationData[3, DataSet.Ensemble.BEAM_1_INDEX] = 3.3f;
        //    ensemble.CorrelationData.CorrelationData[3, DataSet.Ensemble.BEAM_2_INDEX] = 4.4f;
        //    ensemble.CorrelationData.CorrelationData[3, DataSet.Ensemble.BEAM_3_INDEX] = 5.5f;

        //    ensemble.CorrelationData.CorrelationData[5, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
        //    ensemble.CorrelationData.CorrelationData[5, DataSet.Ensemble.BEAM_1_INDEX] = 4.3f;
        //    ensemble.CorrelationData.CorrelationData[5, DataSet.Ensemble.BEAM_2_INDEX] = 5.4f;
        //    ensemble.CorrelationData.CorrelationData[5, DataSet.Ensemble.BEAM_3_INDEX] = 6.5f;

        //    Assert.AreEqual(30, ensemble.CorrelationData.NumElements, "Pre Number of elements is incorrect.");
        //    Assert.AreEqual(30, ensemble.CorrelationData.CorrelationData.GetLength(0), "Pre Number of bins is incorrect.");

        //    string encoded = ensemble.CorrelationData.ToJson();                                                                          // Serialize object to JSON
        //    DataSet.CorrelationDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.CorrelationDataSet>(encoded);    // Deserialize the JSON

        //    Assert.AreEqual(30, decoded.NumElements, "Number of elements is incorrect.");
        //    Assert.AreEqual(30, decoded.CorrelationData.GetLength(0), "Number of bins is incorrect.");

        //    // Verify the values are the same
        //    Assert.AreEqual(1.2f, decoded.CorrelationData[0, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 0 0 is incorrect.");
        //    Assert.AreEqual(2.3f, decoded.CorrelationData[0, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 0 1 is incorrect.");
        //    Assert.AreEqual(3.4f, decoded.CorrelationData[0, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 0 2 is incorrect.");
        //    Assert.AreEqual(4.5f, decoded.CorrelationData[0, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 0 3 is incorrect.");

        //    Assert.AreEqual(2.2f, decoded.CorrelationData[3, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 3 0 is incorrect.");
        //    Assert.AreEqual(3.3f, decoded.CorrelationData[3, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 3 1 is incorrect.");
        //    Assert.AreEqual(4.4f, decoded.CorrelationData[3, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 3 2 is incorrect.");
        //    Assert.AreEqual(5.5f, decoded.CorrelationData[3, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 3 3 is incorrect.");

        //    Assert.AreEqual(3.2f, decoded.CorrelationData[5, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 5 0 is incorrect.");
        //    Assert.AreEqual(4.3f, decoded.CorrelationData[5, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 5 1 is incorrect.");
        //    Assert.AreEqual(5.4f, decoded.CorrelationData[5, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 5 2 is incorrect.");
        //    Assert.AreEqual(6.5f, decoded.CorrelationData[5, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 5 3 is incorrect.");
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

            // Test Serialize()
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.CorrelationData);
            }
            watch.Stop();
            long resultSerialize = watch.ElapsedMilliseconds;

            // Test Deserialize()
            string encodedd = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.CorrelationData);
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                DataSet.CorrelationDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.CorrelationDataSet>(encodedd);
            }
            watch.Stop();
            long resultDeserialize = watch.ElapsedMilliseconds;

            Debug.WriteLine(String.Format("Serialize:{0}  Deserialize:{1}", resultSerialize, resultDeserialize));

            Debug.WriteLine("Complete");

        }

        #region PD0 Decode

        /// <summary>
        /// Test decoding PD0 Correlation data to RTI Correlation data.
        /// </summary>
        [Test]
        public void DecodePd0Test()
        {
            Pd0Correlation pd0Corr = new Pd0Correlation(30);

            pd0Corr.Correlation[0, 0] = 87;
            pd0Corr.Correlation[0, 1] = 59;
            pd0Corr.Correlation[0, 2] = 143;
            pd0Corr.Correlation[0, 3] = 115;
            pd0Corr.Correlation[1, 0] = 199;
            pd0Corr.Correlation[1, 1] = 171;
            pd0Corr.Correlation[1, 2] = 232;
            pd0Corr.Correlation[1, 3] = 227;


            DataSet.CorrelationDataSet corr = new DataSet.CorrelationDataSet(30);

            corr.DecodePd0Ensemble(pd0Corr, 2);

            Assert.AreEqual(0.56f, corr.CorrelationData[0, 0], 0.1f, "Correlation Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(0.45f, corr.CorrelationData[0, 1], 0.1f, "Correlation Bin 0, Beam 1 is incorrect.");
            Assert.AreEqual(0.23f, corr.CorrelationData[0, 2], 0.1f, "Correlation Bin 0, Beam 2 is incorrect.");
            Assert.AreEqual(0.34f, corr.CorrelationData[0, 3], 0.1f, "Correlation Bin 0, Beam 3 is incorrect.");
            Assert.AreEqual(0.91f, corr.CorrelationData[1, 0], 0.1f, "Correlation Bin 1, Beam 0 is incorrect.");
            Assert.AreEqual(0.89f, corr.CorrelationData[1, 1], 0.1f, "Correlation Bin 1, Beam 1 is incorrect.");
            Assert.AreEqual(0.67f, corr.CorrelationData[1, 2], 0.1f, "Correlation Bin 1, Beam 2 is incorrect.");
            Assert.AreEqual(0.78f, corr.CorrelationData[1, 3], 0.1f, "Correlation Bin 1, Beam 3 is incorrect.");
        }

        #endregion

    }
}
