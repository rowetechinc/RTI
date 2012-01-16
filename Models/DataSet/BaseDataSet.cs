/*
 * Copyright 2011, Rowe Technology Inc. 
 * All rights reserved.
 * http://www.rowetechinc.com
 * https://github.com/rowetechinc
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *  1. Redistributions of source code must retain the above copyright notice, this list of
 *      conditions and the following disclaimer.
 *      
 *  2. Redistributions in binary form must reproduce the above copyright notice, this list
 *      of conditions and the following disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 *      
 *  THIS SOFTWARE IS PROVIDED BY Rowe Technology Inc. ''AS IS'' AND ANY EXPRESS OR IMPLIED 
 *  WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 *  FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
 *  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 *  CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 *  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 *  ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 *  ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *  
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Rowe Technology Inc.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version     Comments
 * -----------------------------------------------------------------
 * 09/01/2011      RC                      Initial coding
 * 11/28/2011      RC                      Changed NumBins to NumElements.
 * 12/08/2011      RC          1.09        Added EMPTY_VELOCITY.  
 * 12/12/2011      RC          1.09        Removed BYTES_PER_FLOAT AND BYTES_PER_INT.
 *                                          Moved BAD_VELOCITY and EMPTY_VELOCITY to AdcpDataSet.
 *       
 * 
 */

using System;
namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Base data set.  All data sets will extend from
        /// this data set.  It contains the data set information.
        /// </summary>
        public class BaseDataSet
        {                                                                                                                
            /// <summary>
            /// All the possible combinations for a beam
            /// to be pointed.  This is seperate multiple transducers
            /// on one system. Two different transducers could report
            /// data for the same Bin and beam.  This option will
            /// differenciate the two different transducers.
            /// 
            /// Possible there may just be different datatypes created.
            /// </summary>
            public enum BeamOrientation
            {
                /// <summary>
                /// Orientation of a beam.  Downward facing.
                /// </summary>
                DOWN = 0,

                /// <summary>
                /// Orienation of the beam.  Upward facing.
                /// </summary>
                UP = 1,
            };

            /// <summary>
            /// Type of ranges for this data type.
            /// </summary>
            public int ValueType { get; protected set; }

            /// <summary>
            /// Number of DATA in this data type.
            /// 
            /// DO NOT USE THIS TO GET THE NUMBER OF
            /// BINS IN THE DATASET (ENSEMBLE).  USE
            /// EnsembleData.Bins.  THIS NUMBER IS THE
            /// NUMBER OF DATA VALUES IN THE DATATYPE.
            /// </summary>
            public int NumElements { get; protected set; }

            /// <summary>
            /// Number of beams in this data type.
            /// </summary>
            public int ElementsMultiplier { get; protected set; }

            /// <summary>
            /// Dataset Image value.
            /// </summary>
            public int Imag { get; protected set; }

            /// <summary>
            /// Length of name for this data type.
            /// </summary>
            public int NameLength { get; protected set; }

            /// <summary>
            /// Name of this data type.
            /// </summary>
            public string Name { get; protected set; }

            /// <summary>
            /// Set all the initial ranges for the base data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public BaseDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name)
            {
                ValueType = valueType;
                NumElements = numBins;
                ElementsMultiplier = numBeams;
                Imag = imag;
                NameLength = nameLength;
                Name = name;
            }

            /// <summary>
            /// Return the size of the Base data set.
            /// </summary>
            /// <returns>Size of base data set.</returns>
            public static int GetBaseDataSize(int nameLength)
            {
                // Name length gives length of name
                // The rest are 4 bytes for each item
                return nameLength + (Ensemble.BYTES_IN_INT * 5);
            }

            /// <summary>
            /// Generate the size of the data set based off the number of
            /// elements, element multipler and the base data.  Each element size is based
            /// on the type of dataset.  Determine how many element
            /// exist and multiply by elementMultipler.  Then determine the size of the
            /// BaseData area and add that in.
            /// </summary>
            /// <param name="type">DatatSet Type (eg: float, int, byte...)</param>
            /// <param name="nameLength">Length of the name in bytes.</param>
            /// <param name="numElements">Number of Elements in the dataset</param>
            /// <param name="elementMultipler">Element Multipiler.  Usually number of beams.</param>
            /// <returns>Number of bytes in the data set.</returns>
            public static int GetDataSetSize(int type, int nameLength, int numElements, int elementMultipler)
            {
                // Find the number of bytes per element
                int dataType = Ensemble.BYTES_IN_FLOAT;
                switch(type)
                {
                    case Ensemble.DATATYPE_BYTE:
                        dataType = Ensemble.BYTES_IN_BYTES;
                        break;
                    case Ensemble.DATATYPE_INT:
                        dataType = Ensemble.BYTES_IN_INT;
                        break;
                    case Ensemble.BYTES_IN_FLOAT:
                        dataType = Ensemble.BYTES_IN_FLOAT;
                        break;
                    default:
                        dataType = Ensemble.BYTES_IN_FLOAT;
                        break;
                }

                return ((numElements * elementMultipler) * dataType) + BaseDataSet.GetBaseDataSize(nameLength);
            }

            /// <summary>
            /// For all the datasets that store beam Bin Bin ranges, this will
            /// generate the index for the value.
            /// 
            /// Value = 4 Bytes = [0|1|2|3]
            /// The data is stored in Beam Bin Bin.  The first 4 X NumElements are all Beam 0s ranges.
            /// Beam0 Values | Beam1 Values | Beam2 Values | Beam3 Values
            /// 
            /// Within BeamX Values are Y Number of Bin.
            /// 
            /// Beam0 Values = Beam0Bin0 | Beam0Bin1 | ... Beam0Bin(NumElements)
            /// </summary>
            /// <param name="nameLength">Length of the dataset name.</param>
            /// <param name="numBins">Number of bins.</param>
            /// <param name="beam">Number of beams.</param>
            /// <param name="bin">Bin number.</param>
            /// <returns>Index within the dataset.</returns>
            protected int GetBinBeamIndex( int nameLength, int numBins, int beam, int bin)
            {
                return GetBaseDataSize(nameLength) + (beam * numBins * Ensemble.BYTES_IN_FLOAT) + (bin * Ensemble.BYTES_IN_FLOAT);
            }

            /// <summary>
            /// Generate the header for this data set.  This is used to
            /// encode the dataset back to binary form.
            /// [HEADER][PAYLOAD]
            /// Header = 28 bytes.  
            /// </summary>
            /// <param name="payloadSize">Size of the payload.</param>
            /// <returns>Byte array of the header for the the dataset.</returns>
            protected byte[] GenerateHeader(int payloadSize)
            {
                byte[] result = new byte[DataSet.Ensemble.PAYLOAD_HEADER_LEN];

                // Add Data Type (Byte)
                byte[] dataType = Converters.IntToByteArray(DataSet.Ensemble.DATATYPE_BYTE);
                System.Buffer.BlockCopy(dataType, 0, result, 0, 4);

                // Add Bins or data length
                byte[] len = Converters.IntToByteArray(payloadSize);
                System.Buffer.BlockCopy(len, 0, result, 4, 4);

                // Add Beams (1 for Ensemble, Ancillary and Nmea, 4 for all other data sets.  )
                byte[] beams = Converters.IntToByteArray(ElementsMultiplier);
                System.Buffer.BlockCopy(beams, 0, result, 8, 4);

                // Add Image (default 0)
                byte[] image = Converters.IntToByteArray(Imag);
                System.Buffer.BlockCopy(image, 0, result, 12, 4);

                // Add NameLen (default 8)
                byte[] nameLen = Converters.IntToByteArray(NameLength);
                System.Buffer.BlockCopy(nameLen, 0, result, 16, 4);
 
                // Add Name (E0000XX\0)
                byte[] name = System.Text.Encoding.ASCII.GetBytes(Name);
                System.Buffer.BlockCopy(name, 0, result, 20, NameLength);

                return result;
            }
        }
    }
}