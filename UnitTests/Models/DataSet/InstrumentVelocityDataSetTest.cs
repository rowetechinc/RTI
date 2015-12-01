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
    /// Unit test of the Instrument Velocity DataSet object.
    /// </summary>
    [TestFixture]
    public class InstrumentVelocityDataSetTest
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
            adcpData.AddInstrumentVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,                   // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.InstrumentVelocityID);                 // Dataset ID

            Assert.IsTrue(adcpData.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.InstrumentVelocityData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, adcpData.InstrumentVelocityData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.InstrumentVelocityData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.InstrumentVelocityData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.InstrumentVelocityData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.InstrumentVelocityID, adcpData.InstrumentVelocityData.Name, "Name is incorrect.");

            Assert.AreEqual(adcpData.InstrumentVelocityData.InstrumentVelocityData.GetLength(0), 30, "Instrument Velocity Array Dimension 1 is incorrect.");
            Assert.AreEqual(adcpData.InstrumentVelocityData.InstrumentVelocityData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Instrument Velocity Array Dimension 2 is incorrect.");

        }

        /// <summary>
        /// Test Encode and decode.  
        /// Encode the Instrument Velocity data.  Then
        /// create a new ensemble and decode the
        /// encoded data.
        /// </summary>
        [Test]
        public void TestEncodeDecode()
        {
            // Create dataset
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add Sentence to data set
            adcpData.AddInstrumentVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,             // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            4,                                              // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.InstrumentVelocityID);         // Dataset ID

            Assert.IsTrue(adcpData.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, adcpData.InstrumentVelocityData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, adcpData.InstrumentVelocityData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, adcpData.InstrumentVelocityData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, adcpData.InstrumentVelocityData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, adcpData.InstrumentVelocityData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.InstrumentVelocityID, adcpData.InstrumentVelocityData.Name, "Name is incorrect.");

            Assert.AreEqual(adcpData.InstrumentVelocityData.InstrumentVelocityData.GetLength(0), 30, "Instrument Velocity Array Dimension 1 is incorrect.");
            Assert.AreEqual(adcpData.InstrumentVelocityData.InstrumentVelocityData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Insturment Velocity Array Dimension 2 is incorrect.");

            // Modify the array
            adcpData.InstrumentVelocityData.InstrumentVelocityData[0, 0] = 2.3f;
            adcpData.InstrumentVelocityData.InstrumentVelocityData[0, 1] = 3.4f;
            adcpData.InstrumentVelocityData.InstrumentVelocityData[0, 2] = 4.5f;
            adcpData.InstrumentVelocityData.InstrumentVelocityData[0, 3] = 5.6f;
            adcpData.InstrumentVelocityData.InstrumentVelocityData[1, 0] = 6.7f;
            adcpData.InstrumentVelocityData.InstrumentVelocityData[1, 1] = 7.8f;
            adcpData.InstrumentVelocityData.InstrumentVelocityData[1, 2] = 8.9f;
            adcpData.InstrumentVelocityData.InstrumentVelocityData[1, 3] = 9.10f;
            adcpData.InstrumentVelocityData.InstrumentVelocityData[2, 0] = 10.11f;
            adcpData.InstrumentVelocityData.InstrumentVelocityData[2, 1] = 11.12f;
            adcpData.InstrumentVelocityData.InstrumentVelocityData[2, 2] = 12.13f;

            // Encode the data
            byte[] encode = adcpData.InstrumentVelocityData.Encode();


            // Create dataset
            DataSet.Ensemble ens1 = new DataSet.Ensemble();

            // Add Sentence to data set
            ens1.AddInstrumentVelocityData(DataSet.Ensemble.DATATYPE_FLOAT,                 // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM,        // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.InstrumentVelocityID,          // Dataset ID
                                            encode);                                        // Encoded data

            Assert.IsTrue(ens1.IsInstrumentVelocityAvail, "IsInstrumentVelocityAvail is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, ens1.InstrumentVelocityData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, ens1.InstrumentVelocityData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ens1.InstrumentVelocityData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, ens1.InstrumentVelocityData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, ens1.InstrumentVelocityData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.InstrumentVelocityID, ens1.InstrumentVelocityData.Name, "Name is incorrect.");

            Assert.AreEqual(ens1.InstrumentVelocityData.InstrumentVelocityData.GetLength(0), 30, "Instrument Velocity Array Dimension 1 is incorrect.");
            Assert.AreEqual(ens1.InstrumentVelocityData.InstrumentVelocityData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Instrument Velocity Array Dimension 2 is incorrect.");

            Assert.AreEqual(2.3f, ens1.InstrumentVelocityData.InstrumentVelocityData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens1.InstrumentVelocityData.InstrumentVelocityData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens1.InstrumentVelocityData.InstrumentVelocityData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens1.InstrumentVelocityData.InstrumentVelocityData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens1.InstrumentVelocityData.InstrumentVelocityData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens1.InstrumentVelocityData.InstrumentVelocityData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens1.InstrumentVelocityData.InstrumentVelocityData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens1.InstrumentVelocityData.InstrumentVelocityData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens1.InstrumentVelocityData.InstrumentVelocityData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens1.InstrumentVelocityData.InstrumentVelocityData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens1.InstrumentVelocityData.InstrumentVelocityData[2, 2], "2,2 Data is incorrect.");
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
            ensemble.InstrumentVelocityData.InstrumentVelocityData[0, DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
            ensemble.InstrumentVelocityData.InstrumentVelocityData[0, DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
            ensemble.InstrumentVelocityData.InstrumentVelocityData[0, DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
            ensemble.InstrumentVelocityData.InstrumentVelocityData[0, DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

            ensemble.InstrumentVelocityData.InstrumentVelocityData[3, DataSet.Ensemble.BEAM_0_INDEX] = 2.2f;
            ensemble.InstrumentVelocityData.InstrumentVelocityData[3, DataSet.Ensemble.BEAM_1_INDEX] = 3.3f;
            ensemble.InstrumentVelocityData.InstrumentVelocityData[3, DataSet.Ensemble.BEAM_2_INDEX] = 4.4f;
            ensemble.InstrumentVelocityData.InstrumentVelocityData[3, DataSet.Ensemble.BEAM_3_INDEX] = 5.5f;

            ensemble.InstrumentVelocityData.InstrumentVelocityData[5, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
            ensemble.InstrumentVelocityData.InstrumentVelocityData[5, DataSet.Ensemble.BEAM_1_INDEX] = 4.3f;
            ensemble.InstrumentVelocityData.InstrumentVelocityData[5, DataSet.Ensemble.BEAM_2_INDEX] = 5.4f;
            ensemble.InstrumentVelocityData.InstrumentVelocityData[5, DataSet.Ensemble.BEAM_3_INDEX] = 6.5f;

            string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.InstrumentVelocityData);                                      // Serialize object to JSON
            DataSet.InstrumentVelocityDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.InstrumentVelocityDataSet>(encoded);       // Deserialize the JSON

            // Verify the values are the same
            Assert.AreEqual(1.2f, decoded.InstrumentVelocityData[0, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 0 0 is incorrect.");
            Assert.AreEqual(2.3f, decoded.InstrumentVelocityData[0, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 0 1 is incorrect.");
            Assert.AreEqual(3.4f, decoded.InstrumentVelocityData[0, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 0 2 is incorrect.");
            Assert.AreEqual(4.5f, decoded.InstrumentVelocityData[0, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 0 3 is incorrect.");

            Assert.AreEqual(2.2f, decoded.InstrumentVelocityData[3, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 3 0 is incorrect.");
            Assert.AreEqual(3.3f, decoded.InstrumentVelocityData[3, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 3 1 is incorrect.");
            Assert.AreEqual(4.4f, decoded.InstrumentVelocityData[3, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 3 2 is incorrect.");
            Assert.AreEqual(5.5f, decoded.InstrumentVelocityData[3, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 3 3 is incorrect.");

            Assert.AreEqual(3.2f, decoded.InstrumentVelocityData[5, DataSet.Ensemble.BEAM_0_INDEX], "Amp Data 5 0 is incorrect.");
            Assert.AreEqual(4.3f, decoded.InstrumentVelocityData[5, DataSet.Ensemble.BEAM_1_INDEX], "Amp Data 5 1 is incorrect.");
            Assert.AreEqual(5.4f, decoded.InstrumentVelocityData[5, DataSet.Ensemble.BEAM_2_INDEX], "Amp Data 5 2 is incorrect.");
            Assert.AreEqual(6.5f, decoded.InstrumentVelocityData[5, DataSet.Ensemble.BEAM_3_INDEX], "Amp Data 5 3 is incorrect.");
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
                string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.InstrumentVelocityData);
            }
            watch.Stop();
            long resultSerialize = watch.ElapsedMilliseconds;

            // Test Deserialize()
            string encodedd = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.InstrumentVelocityData);
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                DataSet.InstrumentVelocityDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.InstrumentVelocityDataSet>(encodedd);
            }
            watch.Stop();
            long resultDeserialize = watch.ElapsedMilliseconds;

            Debug.WriteLine(String.Format("Serialize:{0}  Deserialize:{1}", resultSerialize, resultDeserialize));

            Debug.WriteLine("Complete");

        }

        #region PD0 Decode

        /// <summary>
        /// Test decoding PD0 Velocity data to RTI Instrument Velocity data.
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

            DataSet.InstrumentVelocityDataSet instr = new DataSet.InstrumentVelocityDataSet(30);
            instr.DecodePd0Ensemble(vel);

            // beam order 3,2,0,1; XYZ order 1,0,-2,3, ENU order 0,1,2,3

            Assert.AreEqual(0.456f, instr.InstrumentVelocityData[0, 0], "Bin 0, Beam 0 Velocity is incorrect.");
            Assert.AreEqual(0.123f, instr.InstrumentVelocityData[0, 1], "Bin 0, Beam 1 Velocity is incorrect.");
            Assert.AreEqual(-0.789f, instr.InstrumentVelocityData[0, 2], "Bin 0, Beam 2 Velocity is incorrect.");
            Assert.AreEqual(0.147f, instr.InstrumentVelocityData[0, 3], "Bin 0, Beam 3 Velocity is incorrect.");
            Assert.AreEqual(0.369f, instr.InstrumentVelocityData[1, 0], "Bin 1, Beam 0 Velocity is incorrect.");
            Assert.AreEqual(0.258f, instr.InstrumentVelocityData[1, 1], "Bin 1, Beam 1 Velocity is incorrect.");
            Assert.AreEqual(-0.741f, instr.InstrumentVelocityData[1, 2], "Bin 1, Beam 2 Velocity is incorrect.");
            Assert.AreEqual(0.852f, instr.InstrumentVelocityData[1, 3], "Bin 1, Beam 3 Velocity is incorrect.");
        }

        #endregion
    }
}