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

            // Modify the array
            adcpData.CorrelationData.CorrelationData[0, 0] = 2.3f;
            adcpData.CorrelationData.CorrelationData[0, 1] = 3.4f;
            adcpData.CorrelationData.CorrelationData[0, 2] = 4.5f;
            adcpData.CorrelationData.CorrelationData[0, 3] = 5.6f;
            adcpData.CorrelationData.CorrelationData[1, 0] = 6.7f;
            adcpData.CorrelationData.CorrelationData[1, 1] = 7.8f;
            adcpData.CorrelationData.CorrelationData[1, 2] = 8.9f;
            adcpData.CorrelationData.CorrelationData[1, 3] = 9.10f;
            adcpData.CorrelationData.CorrelationData[2, 0] = 10.11f;
            adcpData.CorrelationData.CorrelationData[2, 1] = 11.12f;
            adcpData.CorrelationData.CorrelationData[2, 2] = 12.13f;

            // Encode the data
            byte[] encode = adcpData.CorrelationData.Encode();


            // Create dataset
            DataSet.Ensemble ens1 = new DataSet.Ensemble();

            // Add Sentence to data set
            ens1.AddCorrelationData(DataSet.Ensemble.DATATYPE_FLOAT,                        // Type of data stored (Float or Int)
                                            30,                                             // Number of bins
                                            DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM,        // Number of beams
                                            DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                                            DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                                            DataSet.Ensemble.CorrelationID,                 // Dataset ID
                                            encode);                                        // Encoded data

            Assert.IsTrue(ens1.IsCorrelationAvail, "IsCorrelation is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DATATYPE_FLOAT, ens1.CorrelationData.ValueType, "DataType is incorrect.");
            Assert.AreEqual(30, ens1.CorrelationData.NumElements, "Number of Elements is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, ens1.CorrelationData.ElementsMultiplier, "Element Multiplies are incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_IMAG, ens1.CorrelationData.Imag, "Imag is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.DEFAULT_NAME_LENGTH, ens1.CorrelationData.NameLength, "Name length is incorrect.");
            Assert.AreEqual(DataSet.Ensemble.CorrelationID, ens1.CorrelationData.Name, "Name is incorrect.");

            Assert.AreEqual(ens1.CorrelationData.CorrelationData.GetLength(0), 30, "Correlation Array Dimension 1 is incorrect.");
            Assert.AreEqual(ens1.CorrelationData.CorrelationData.GetLength(1), DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, "Correlation Array Dimension 2 is incorrect.");

            Assert.AreEqual(2.3f, ens1.CorrelationData.CorrelationData[0, 0], "0,0 Data is incorrect.");
            Assert.AreEqual(3.4f, ens1.CorrelationData.CorrelationData[0, 1], "0,1 Data is incorrect.");
            Assert.AreEqual(4.5f, ens1.CorrelationData.CorrelationData[0, 2], "0,2 Data is incorrect.");
            Assert.AreEqual(5.6f, ens1.CorrelationData.CorrelationData[0, 3], "0,3 Data is incorrect.");
            Assert.AreEqual(6.7f, ens1.CorrelationData.CorrelationData[1, 0], "1,0 Data is incorrect.");
            Assert.AreEqual(7.8f, ens1.CorrelationData.CorrelationData[1, 1], "1,1 Data is incorrect.");
            Assert.AreEqual(8.9f, ens1.CorrelationData.CorrelationData[1, 2], "1,2 Data is incorrect.");
            Assert.AreEqual(9.10f, ens1.CorrelationData.CorrelationData[1, 3], "1,3 Data is incorrect.");
            Assert.AreEqual(10.11f, ens1.CorrelationData.CorrelationData[2, 0], "2,0 Data is incorrect.");
            Assert.AreEqual(11.12f, ens1.CorrelationData.CorrelationData[2, 1], "2,1 Data is incorrect.");
            Assert.AreEqual(12.13f, ens1.CorrelationData.CorrelationData[2, 2], "2,2 Data is incorrect.");
        }

    }
}
