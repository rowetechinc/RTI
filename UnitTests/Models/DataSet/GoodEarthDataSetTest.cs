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
    /// Unit test of the Good Earth DataSet object.
    /// </summary>
    [TestFixture]
    public class GoodEarthDataSetTest
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
            adcpData.AddGoodEarthData(DataSet.Ensemble.DATATYPE_FLOAT,                      // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.GoodEarthID);                  // Dataset ID

            Assert.IsTrue(adcpData.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.GoodEarthData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, adcpData.GoodEarthData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.GoodEarthData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.GoodEarthData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.GoodEarthData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.GoodEarthID, adcpData.GoodEarthData.Name, "Name is incorrect.");

            Assert.AreEqual(adcpData.GoodEarthData.GoodEarthData.GetLength(0), 30, "Good Beam Array Dimension 1 is incorrect.");
            Assert.AreEqual(adcpData.GoodEarthData.GoodEarthData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Good Beam Array Dimension 2 is incorrect.");

        }

        /// <summary>
        /// Test Encode and decode.  
        /// Encode the Good Earth data.  Then
        /// create a new ensemble and decode the
        /// encoded data.
        /// </summary>
        [Test]
        public void TestEncodeDecode()
        {
            // Create dataset
            DataSet.Ensemble ensemble = new DataSet.Ensemble();

            // Add Sentence to data set
            ensemble.AddGoodEarthData(DataSet.Ensemble.DATATYPE_FLOAT,                       // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.GoodEarthID);                  // Dataset ID

            Assert.IsTrue(ensemble.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, ensemble.GoodEarthData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, ensemble.GoodEarthData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ensemble.GoodEarthData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, ensemble.GoodEarthData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, ensemble.GoodEarthData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.GoodEarthID, ensemble.GoodEarthData.Name, "Name is incorrect.");

            Assert.AreEqual(ensemble.GoodEarthData.GoodEarthData.GetLength(0), 30, "Good Beam Array Dimension 1 is incorrect.");
            Assert.AreEqual(ensemble.GoodEarthData.GoodEarthData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Good Beam Array Dimension 2 is incorrect.");

            // Modify the array
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

            // Encode the data
            byte[] encode = ensemble.GoodEarthData.Encode();


            // Create dataset
            DataSet.Ensemble ens0 = new DataSet.Ensemble();

            // Add Sentence to data set
            ens0.AddGoodEarthData(DataSet.Ensemble.DATATYPE_FLOAT,                       // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM,        // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.GoodEarthID,                    // Dataset ID
                                            encode);                                        // Encoded data

            Assert.IsTrue(ens0.IsGoodEarthAvail, "IsGoodEarthAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, ens0.GoodEarthData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, ens0.GoodEarthData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ens0.GoodEarthData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, ens0.GoodEarthData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, ens0.GoodEarthData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.GoodEarthID, ens0.GoodEarthData.Name, "Name is incorrect.");

            Assert.AreEqual(ens0.GoodEarthData.GoodEarthData.GetLength(0), 30, "Beam Velocity Array Dimension 1 is incorrect.");
            Assert.AreEqual(ens0.GoodEarthData.GoodEarthData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Beam Velocity Array Dimension 2 is incorrect.");

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
            ensemble.GoodEarthData.GoodEarthData[0, DataSet.Ensemble.BEAM_0_INDEX] = 1;
            ensemble.GoodEarthData.GoodEarthData[0, DataSet.Ensemble.BEAM_1_INDEX] = 2;
            ensemble.GoodEarthData.GoodEarthData[0, DataSet.Ensemble.BEAM_2_INDEX] = 3;
            ensemble.GoodEarthData.GoodEarthData[0, DataSet.Ensemble.BEAM_3_INDEX] = 4;

            ensemble.GoodEarthData.GoodEarthData[3, DataSet.Ensemble.BEAM_0_INDEX] = 2;
            ensemble.GoodEarthData.GoodEarthData[3, DataSet.Ensemble.BEAM_1_INDEX] = 3;
            ensemble.GoodEarthData.GoodEarthData[3, DataSet.Ensemble.BEAM_2_INDEX] = 4;
            ensemble.GoodEarthData.GoodEarthData[3, DataSet.Ensemble.BEAM_3_INDEX] = 5;

            ensemble.GoodEarthData.GoodEarthData[5, DataSet.Ensemble.BEAM_0_INDEX] = 3;
            ensemble.GoodEarthData.GoodEarthData[5, DataSet.Ensemble.BEAM_1_INDEX] = 4;
            ensemble.GoodEarthData.GoodEarthData[5, DataSet.Ensemble.BEAM_2_INDEX] = 5;
            ensemble.GoodEarthData.GoodEarthData[5, DataSet.Ensemble.BEAM_3_INDEX] = 6;

            string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.GoodEarthData);                                      // Serialize object to JSON
            DataSet.GoodEarthDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.GoodEarthDataSet>(encoded);       // Deserialize the JSON

            // Verify the values are the same
            Assert.AreEqual(1, decoded.GoodEarthData[0, DataSet.Ensemble.BEAM_0_INDEX], "Good Beam Data 0 0 is incorrect.");
            Assert.AreEqual(2, decoded.GoodEarthData[0, DataSet.Ensemble.BEAM_1_INDEX], "Good Beam Data 0 1 is incorrect.");
            Assert.AreEqual(3, decoded.GoodEarthData[0, DataSet.Ensemble.BEAM_2_INDEX], "Good Beam Data 0 2 is incorrect.");
            Assert.AreEqual(4, decoded.GoodEarthData[0, DataSet.Ensemble.BEAM_3_INDEX], "Good Beam Data 0 3 is incorrect.");

            Assert.AreEqual(2, decoded.GoodEarthData[3, DataSet.Ensemble.BEAM_0_INDEX], "Good Beam Data 3 0 is incorrect.");
            Assert.AreEqual(3, decoded.GoodEarthData[3, DataSet.Ensemble.BEAM_1_INDEX], "Good Beam Data 3 1 is incorrect.");
            Assert.AreEqual(4, decoded.GoodEarthData[3, DataSet.Ensemble.BEAM_2_INDEX], "Good Beam Data 3 2 is incorrect.");
            Assert.AreEqual(5, decoded.GoodEarthData[3, DataSet.Ensemble.BEAM_3_INDEX], "Good Beam Data 3 3 is incorrect.");

            Assert.AreEqual(3, decoded.GoodEarthData[5, DataSet.Ensemble.BEAM_0_INDEX], "Good Beam Data 5 0 is incorrect.");
            Assert.AreEqual(4, decoded.GoodEarthData[5, DataSet.Ensemble.BEAM_1_INDEX], "Good Beam Data 5 1 is incorrect.");
            Assert.AreEqual(5, decoded.GoodEarthData[5, DataSet.Ensemble.BEAM_2_INDEX], "Good Beam Data 5 2 is incorrect.");
            Assert.AreEqual(6, decoded.GoodEarthData[5, DataSet.Ensemble.BEAM_3_INDEX], "Good Beam Data 5 3 is incorrect.");
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
                string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.GoodEarthData);
            }
            watch.Stop();
            long resultSerialize = watch.ElapsedMilliseconds;

            // Test Deserialize()
            string encodedd = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.GoodEarthData);
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                DataSet.GoodEarthDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.GoodEarthDataSet>(encodedd);
            }
            watch.Stop();
            long resultDeserialize = watch.ElapsedMilliseconds;

            Debug.WriteLine(String.Format("Serialize:{0}  Deserialize:{1}", resultSerialize, resultDeserialize));

            Debug.WriteLine("Complete");

        }

        #region PD0 Decode

        /// <summary>
        /// Test decoding PD0 Percent Good data to RTI Good Earth data.
        /// </summary>
        [Test]
        public void DecodePd0Test()
        {
            Pd0PercentGood pg = new Pd0PercentGood(30);

            pg.PercentGood[0, 0] = 100;
            pg.PercentGood[0, 1] = 100;
            pg.PercentGood[0, 2] = 50;
            pg.PercentGood[0, 3] = 100;
            pg.PercentGood[1, 0] = 100;
            pg.PercentGood[1, 1] = 100;
            pg.PercentGood[1, 2] = 100;
            pg.PercentGood[1, 3] = 50;

            DataSet.GoodEarthDataSet goodEarth = new DataSet.GoodEarthDataSet(30);
            goodEarth.DecodePd0Ensemble(pg, 2);

            Assert.AreEqual(1, goodEarth.GoodEarthData[0, 0], "Good Earth Bin 0, Beam 0 is incorrect.");
            Assert.AreEqual(2, goodEarth.GoodEarthData[0, 1], "Good Earth Bin 0, Beam 1 is incorrect.");
            Assert.AreEqual(2, goodEarth.GoodEarthData[0, 2], "Good Earth Bin 0, Beam 2 is incorrect.");
            Assert.AreEqual(2, goodEarth.GoodEarthData[0, 3], "Good Earth Bin 0, Beam 3 is incorrect.");
            Assert.AreEqual(2, goodEarth.GoodEarthData[1, 0], "Good Earthd Bin 1, Beam 0 is incorrect.");
            Assert.AreEqual(1, goodEarth.GoodEarthData[1, 1], "Good Earth Bin 1, Beam 1 is incorrect.");
            Assert.AreEqual(2, goodEarth.GoodEarthData[1, 2], "Good Earth Bin 1, Beam 2 is incorrect.");
            Assert.AreEqual(2, goodEarth.GoodEarthData[1, 3], "Good Earth Bin 1, Beam 3 is incorrect.");
        }

        #endregion

    }
}