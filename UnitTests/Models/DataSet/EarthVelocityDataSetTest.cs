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
 *                                        Add a test with VelocityVectors.
 *       
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Diagnostics;

    /// <summary>
    /// Unit test of the Earth Velocity DataSet object.
    /// </summary>
    [TestFixture]
    public class EarthVelocityDataSetTest
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
            adcpData.AddEarthVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,                  // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.EarthVelocityID);              // Dataset ID

            Assert.IsTrue(adcpData.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.EarthVelocityData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, adcpData.EarthVelocityData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.EarthVelocityData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.EarthVelocityData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.EarthVelocityData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.EarthVelocityID, adcpData.EarthVelocityData.Name, "Name is incorrect.");

            Assert.AreEqual(adcpData.EarthVelocityData.EarthVelocityData.GetLength(0), 30, "Earth Velocity Array Dimension 1 is incorrect.");
            Assert.AreEqual(adcpData.EarthVelocityData.EarthVelocityData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Earth Velocity Array Dimension 2 is incorrect.");

        }

        /// <summary>
        /// Test Encode and decode.  
        /// Encode the Earth Velocity data.  Then
        /// create a new ensemble and decode the
        /// encoded data.
        /// </summary>
        [Test]
        public void TestEncodeDecode()
        {
            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Sentence to data set
            adcpData.AddEarthVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,                  // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.EarthVelocityID);              // Dataset ID

            Assert.IsTrue(adcpData.IsEarthVelocityAvail, "IEarthVelocityAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.EarthVelocityData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, adcpData.EarthVelocityData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.EarthVelocityData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.EarthVelocityData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.EarthVelocityData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.EarthVelocityID, adcpData.EarthVelocityData.Name, "Name is incorrect.");

            Assert.AreEqual(adcpData.EarthVelocityData.EarthVelocityData.GetLength(0), 30, "Earth Velocity Array Dimension 1 is incorrect.");
            Assert.AreEqual(adcpData.EarthVelocityData.EarthVelocityData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Earth Velocity Array Dimension 2 is incorrect.");

            // Modify the array
            adcpData.EarthVelocityData.EarthVelocityData[0, 0] = 2.3f;
            adcpData.EarthVelocityData.EarthVelocityData[0, 1] = 3.4f;
            adcpData.EarthVelocityData.EarthVelocityData[0, 2] = 4.5f;
            adcpData.EarthVelocityData.EarthVelocityData[0, 3] = 5.6f;
            adcpData.EarthVelocityData.EarthVelocityData[1, 0] = 6.7f;
            adcpData.EarthVelocityData.EarthVelocityData[1, 1] = 7.8f;
            adcpData.EarthVelocityData.EarthVelocityData[1, 2] = 8.9f;
            adcpData.EarthVelocityData.EarthVelocityData[1, 3] = 9.10f;
            adcpData.EarthVelocityData.EarthVelocityData[2, 0] = 10.11f;
            adcpData.EarthVelocityData.EarthVelocityData[2, 1] = 11.12f;
            adcpData.EarthVelocityData.EarthVelocityData[2, 2] = 12.13f;

            // Encode the data
            byte[] encode = adcpData.EarthVelocityData.Encode();


            // Create dataset
            DataSet.Ensemble ens1 = new DataSet.Ensemble();

            // Add Sentence to data set
            ens1.AddEarthVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,                       // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM,        // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.EarthVelocityID,                    // Dataset ID
                                            encode);                                        // Encoded data

            Assert.IsTrue(ens1.IsEarthVelocityAvail, "IsEarthVelocityAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, ens1.EarthVelocityData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, ens1.EarthVelocityData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ens1.EarthVelocityData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, ens1.EarthVelocityData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, ens1.EarthVelocityData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.EarthVelocityID, ens1.EarthVelocityData.Name, "Name is incorrect.");

            Assert.AreEqual(ens1.EarthVelocityData.EarthVelocityData.GetLength(0), 30, "Beam Velocity Array Dimension 1 is incorrect.");
            Assert.AreEqual(ens1.EarthVelocityData.EarthVelocityData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Beam Velocity Array Dimension 2 is incorrect.");

            Assert.AreEqual(2.3f, ens1.EarthVelocityData.EarthVelocityData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens1.EarthVelocityData.EarthVelocityData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens1.EarthVelocityData.EarthVelocityData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens1.EarthVelocityData.EarthVelocityData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens1.EarthVelocityData.EarthVelocityData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens1.EarthVelocityData.EarthVelocityData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens1.EarthVelocityData.EarthVelocityData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens1.EarthVelocityData.EarthVelocityData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens1.EarthVelocityData.EarthVelocityData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens1.EarthVelocityData.EarthVelocityData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens1.EarthVelocityData.EarthVelocityData[2, 2], "2,2 Data is incorrect.");
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
            ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
            ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
            ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
            ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

            ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_0_INDEX] = 2.2f;
            ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_1_INDEX] = 3.3f;
            ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_2_INDEX] = 4.4f;
            ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_3_INDEX] = 5.5f;

            ensemble.EarthVelocityData.EarthVelocityData[5, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
            ensemble.EarthVelocityData.EarthVelocityData[5, DataSet.Ensemble.BEAM_1_INDEX] = 4.3f;
            ensemble.EarthVelocityData.EarthVelocityData[5, DataSet.Ensemble.BEAM_2_INDEX] = 5.4f;
            ensemble.EarthVelocityData.EarthVelocityData[5, DataSet.Ensemble.BEAM_3_INDEX] = 6.5f;

            string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthVelocityData);                                      // Serialize object to JSON
            DataSet.EarthVelocityDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.EarthVelocityDataSet>(encoded);       // Deserialize the JSON

            // Verify the values are the same
            Assert.AreEqual(1.2f, decoded.EarthVelocityData[0, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 0 0 is incorrect.");
            Assert.AreEqual(2.3f, decoded.EarthVelocityData[0, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 0 1 is incorrect.");
            Assert.AreEqual(3.4f, decoded.EarthVelocityData[0, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 0 2 is incorrect.");
            Assert.AreEqual(4.5f, decoded.EarthVelocityData[0, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 0 3 is incorrect.");

            Assert.AreEqual(2.2f, decoded.EarthVelocityData[3, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 3 0 is incorrect.");
            Assert.AreEqual(3.3f, decoded.EarthVelocityData[3, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 3 1 is incorrect.");
            Assert.AreEqual(4.4f, decoded.EarthVelocityData[3, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 3 2 is incorrect.");
            Assert.AreEqual(5.5f, decoded.EarthVelocityData[3, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 3 3 is incorrect.");

            Assert.AreEqual(3.2f, decoded.EarthVelocityData[5, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 5 0 is incorrect.");
            Assert.AreEqual(4.3f, decoded.EarthVelocityData[5, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 5 1 is incorrect.");
            Assert.AreEqual(5.4f, decoded.EarthVelocityData[5, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 5 2 is incorrect.");
            Assert.AreEqual(6.5f, decoded.EarthVelocityData[5, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 5 3 is incorrect.");
        }

        /// <summary>
        /// Test encoding and decoding to JSON with VelocityVectors.
        /// </summary>
        [Test]
        public void TestJsonVV()
        {
            // Generate an Ensemble
            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);

            // Modify the data
            ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
            ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
            ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
            ensemble.EarthVelocityData.EarthVelocityData[0, DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

            ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_0_INDEX] = 2.2f;
            ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_1_INDEX] = 3.3f;
            ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_2_INDEX] = 4.4f;
            ensemble.EarthVelocityData.EarthVelocityData[3, DataSet.Ensemble.BEAM_3_INDEX] = 5.5f;

            ensemble.EarthVelocityData.EarthVelocityData[5, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
            ensemble.EarthVelocityData.EarthVelocityData[5, DataSet.Ensemble.BEAM_1_INDEX] = 4.3f;
            ensemble.EarthVelocityData.EarthVelocityData[5, DataSet.Ensemble.BEAM_2_INDEX] = 5.4f;
            ensemble.EarthVelocityData.EarthVelocityData[5, DataSet.Ensemble.BEAM_3_INDEX] = 6.5f;

            #region VelocityVectors

            ensemble.EarthVelocityData.IsVelocityVectorAvail = true;
            ensemble.EarthVelocityData.VelocityVectors = new DataSet.VelocityVector[30];
            ensemble.EarthVelocityData.VelocityVectors[0] = new DataSet.VelocityVector() { Magnitude = 1.2f, DirectionXNorth = 2.2f, DirectionYNorth = 3.2f };
            ensemble.EarthVelocityData.VelocityVectors[1] = new DataSet.VelocityVector() { Magnitude = 1.3f, DirectionXNorth = 2.3f, DirectionYNorth = 3.3f };
            ensemble.EarthVelocityData.VelocityVectors[2] = new DataSet.VelocityVector() { Magnitude = 1.4f, DirectionXNorth = 2.4f, DirectionYNorth = 3.4f };
            ensemble.EarthVelocityData.VelocityVectors[3] = new DataSet.VelocityVector() { Magnitude = 1.5f, DirectionXNorth = 2.5f, DirectionYNorth = 3.5f };
            ensemble.EarthVelocityData.VelocityVectors[4] = new DataSet.VelocityVector() { Magnitude = 1.6f, DirectionXNorth = 2.6f, DirectionYNorth = 3.6f };
            ensemble.EarthVelocityData.VelocityVectors[5] = new DataSet.VelocityVector() { Magnitude = 1.7f, DirectionXNorth = 2.7f, DirectionYNorth = 3.7f };
            ensemble.EarthVelocityData.VelocityVectors[6] = new DataSet.VelocityVector() { Magnitude = 1.8f, DirectionXNorth = 2.8f, DirectionYNorth = 3.8f };
            ensemble.EarthVelocityData.VelocityVectors[7] = new DataSet.VelocityVector() { Magnitude = 1.9f, DirectionXNorth = 2.9f, DirectionYNorth = 3.9f };

            #endregion

            string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthVelocityData);                                      // Serialize object to JSON
            DataSet.EarthVelocityDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.EarthVelocityDataSet>(encoded);       // Deserialize the JSON

            // Verify the values are the same
            Assert.AreEqual(1.2f, decoded.EarthVelocityData[0, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 0 0 is incorrect.");
            Assert.AreEqual(2.3f, decoded.EarthVelocityData[0, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 0 1 is incorrect.");
            Assert.AreEqual(3.4f, decoded.EarthVelocityData[0, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 0 2 is incorrect.");
            Assert.AreEqual(4.5f, decoded.EarthVelocityData[0, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 0 3 is incorrect.");

            Assert.AreEqual(2.2f, decoded.EarthVelocityData[3, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 3 0 is incorrect.");
            Assert.AreEqual(3.3f, decoded.EarthVelocityData[3, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 3 1 is incorrect.");
            Assert.AreEqual(4.4f, decoded.EarthVelocityData[3, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 3 2 is incorrect.");
            Assert.AreEqual(5.5f, decoded.EarthVelocityData[3, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 3 3 is incorrect.");

            Assert.AreEqual(3.2f, decoded.EarthVelocityData[5, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 5 0 is incorrect.");
            Assert.AreEqual(4.3f, decoded.EarthVelocityData[5, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 5 1 is incorrect.");
            Assert.AreEqual(5.4f, decoded.EarthVelocityData[5, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 5 2 is incorrect.");
            Assert.AreEqual(6.5f, decoded.EarthVelocityData[5, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 5 3 is incorrect.");

            Assert.AreEqual(true, decoded.IsVelocityVectorAvail, "IsVelocityVectorAvail incorrect.");
            Assert.AreEqual(1.2f, decoded.VelocityVectors[0].Magnitude, "VV 0 Mag is incorrect.");
            Assert.AreEqual(2.2f, decoded.VelocityVectors[0].DirectionXNorth, "VV 0 X North is incorrect.");
            Assert.AreEqual(3.2f, decoded.VelocityVectors[0].DirectionYNorth, "VV 0 Y North is incorrect.");

            Assert.AreEqual(1.3f, decoded.VelocityVectors[1].Magnitude, "VV 1 Mag is incorrect.");
            Assert.AreEqual(2.3f, decoded.VelocityVectors[1].DirectionXNorth, "VV 1 X North is incorrect.");
            Assert.AreEqual(3.3f, decoded.VelocityVectors[1].DirectionYNorth, "VV 1 Y North is incorrect.");

            Assert.AreEqual(1.4f, decoded.VelocityVectors[2].Magnitude, "VV 2 Mag is incorrect.");
            Assert.AreEqual(2.4f, decoded.VelocityVectors[2].DirectionXNorth, "VV 2 X North is incorrect.");
            Assert.AreEqual(3.4f, decoded.VelocityVectors[2].DirectionYNorth, "VV 2 Y North is incorrect.");

            Assert.AreEqual(1.5f, decoded.VelocityVectors[3].Magnitude, "VV 3 Mag is incorrect.");
            Assert.AreEqual(2.5f, decoded.VelocityVectors[3].DirectionXNorth, "VV 3 X North is incorrect.");
            Assert.AreEqual(3.5f, decoded.VelocityVectors[3].DirectionYNorth, "VV 3 Y North is incorrect.");

            Assert.AreEqual(1.6f, decoded.VelocityVectors[4].Magnitude, "VV 4 Mag is incorrect.");
            Assert.AreEqual(2.6f, decoded.VelocityVectors[4].DirectionXNorth, "VV 4 X North is incorrect.");
            Assert.AreEqual(3.6f, decoded.VelocityVectors[4].DirectionYNorth, "VV 4 Y North is incorrect.");

            Assert.AreEqual(1.7f, decoded.VelocityVectors[5].Magnitude, "VV 5 Mag is incorrect.");
            Assert.AreEqual(2.7f, decoded.VelocityVectors[5].DirectionXNorth, "VV 5 X North is incorrect.");
            Assert.AreEqual(3.7f, decoded.VelocityVectors[5].DirectionYNorth, "VV 5 Y North is incorrect.");

            Assert.AreEqual(1.8f, decoded.VelocityVectors[6].Magnitude, "VV 6 Mag is incorrect.");
            Assert.AreEqual(2.8f, decoded.VelocityVectors[6].DirectionXNorth, "VV 6 X North is incorrect.");
            Assert.AreEqual(3.8f, decoded.VelocityVectors[6].DirectionYNorth, "VV 6 Y North is incorrect.");

            Assert.AreEqual(1.9f, decoded.VelocityVectors[7].Magnitude, "VV 7 Mag is incorrect.");
            Assert.AreEqual(2.9f, decoded.VelocityVectors[7].DirectionXNorth, "VV 7 X North is incorrect.");
            Assert.AreEqual(3.9f, decoded.VelocityVectors[7].DirectionYNorth, "VV 7 Y North is incorrect.");
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
                string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthVelocityData);
            }
            watch.Stop();
            long resultSerialize = watch.ElapsedMilliseconds;

            // Test Deserialize()
            string encodedd = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthVelocityData);
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                DataSet.EarthVelocityDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.EarthVelocityDataSet>(encodedd);
            }
            watch.Stop();
            long resultDeserialize = watch.ElapsedMilliseconds;

            Debug.WriteLine(String.Format("Serialize:{0}  Deserialize:{1}", resultSerialize, resultDeserialize));

            Debug.WriteLine("Complete");

        }

        #region PD0 Decode

        /// <summary>
        /// Test decoding PD0 Velocity data to RTI Earth Velocity data.
        /// </summary>
        [Test]
        public void DecodePd0Test()
        {
            Pd0Velocity vel = new Pd0Velocity(30);

            vel.Velocities[0, 0] = 123;
            vel.Velocities[0, 1] = 456;
            vel.Velocities[0, 2] = 789;
            vel.Velocities[0, 3] = 147;
            vel.Velocities[1, 0] = 258;
            vel.Velocities[1, 1] = 369;
            vel.Velocities[1, 2] = 741;
            vel.Velocities[1, 3] = 852;

            DataSet.EarthVelocityDataSet earth = new DataSet.EarthVelocityDataSet(30);
            earth.DecodePd0Ensemble(vel);

            // beam order 3,2,0,1; XYZ order 1,0,-2,3, ENU order 0,1,2,3

            Assert.AreEqual(0.123f, earth.EarthVelocityData[0, 0], "Bin 0, Beam 0 Velocity is incorrect.");
            Assert.AreEqual(0.456f, earth.EarthVelocityData[0, 1], "Bin 0, Beam 1 Velocity is incorrect.");
            Assert.AreEqual(0.789f, earth.EarthVelocityData[0, 2], "Bin 0, Beam 2 Velocity is incorrect.");
            Assert.AreEqual(0.147f, earth.EarthVelocityData[0, 3], "Bin 0, Beam 3 Velocity is incorrect.");
            Assert.AreEqual(0.258f, earth.EarthVelocityData[1, 0], "Bin 1, Beam 0 Velocity is incorrect.");
            Assert.AreEqual(0.369f, earth.EarthVelocityData[1, 1], "Bin 1, Beam 1 Velocity is incorrect.");
            Assert.AreEqual(0.741f, earth.EarthVelocityData[1, 2], "Bin 1, Beam 2 Velocity is incorrect.");
            Assert.AreEqual(0.852f, earth.EarthVelocityData[1, 3], "Bin 1, Beam 3 Velocity is incorrect.");
        }

        #endregion
    }
}