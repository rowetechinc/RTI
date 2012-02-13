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
 * Date            Initials   Version     Comments
 * -----------------------------------------------------------------
 * 09/01/2011      RC                     Initial coding
 * 10/04/2011      RC                     Added GoodEarthBinList to display text.
 *                                         Added method to populate the list.
 * 10/25/2011      RC                     Added new constructor that takes no data and made CreateBinList public.
 * 12/07/2011      RC          1.08       Remove BinList           
 * 12/09/2011      RC          1.09       Make orientation a parameter with a default value.
 * 01/19/2012      RC          1.14       Added Encode() to create a byte array of data.
 *                                         Removed "private set".
 *                                         Rename Decode methods to Decode().
 * 01/23/2012      RC          1.14       Fixed Encode to convert to int to byte array.
 * 
 */


using System.Data;
using System;
using System.ComponentModel;
namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the Good EarthVelocity data.
        /// </summary>
        public class GoodEarthDataSet : BaseDataSet
        {
            /// <summary>
            /// Store all the Good EarthVelocity data for the ADCP.
            /// </summary>
            public int[,] GoodEarthData { get; set; }

            /// <summary>
            /// This value will be used when there will be multiple transducers
            /// giving ranges.  There will be duplicate beam Bin Bin ranges and the
            /// orientation will determine which beam Bin Bin ranges to group together.
            /// 
            /// This may not be necessary and additional datatypes may be created instead
            /// for each transducer orientation.
            /// </summary>
            public BaseDataSet.BeamOrientation Orientation { get; set; }

            /// <summary>
            /// Create an Good EarthVelocity data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="orientation">Orientation of the beams.</param>
            public GoodEarthDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                GoodEarthData = new int[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;
            }

            /// <summary>
            /// Create an Good EarthVelocity data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="goodEarthData">Byte array containing Good EarthVelocity data</param>
            /// <param name="orientation">Orientation of the beams.</param>
            public GoodEarthDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] goodEarthData, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                GoodEarthData = new int[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;

                // Decode the byte array for Good EarthVelocity data
                Decode(goodEarthData);
            }

            /// <summary>
            /// Create an Good Earth data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="goodEarthData">DataTable containing Good Earth data</param>
            /// <param name="orientation">Orientation of the beams.</param>
            public GoodEarthDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataTable goodEarthData, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                GoodEarthData = new int[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;

                // Decode the byte array for Good EarthVelocity data
                Decode(goodEarthData);
            }

            /// <summary>
            /// Get all the Good Earth ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin Bin Beams.
            /// </summary>
            /// <param name="dataType">Byte array containing the Good Earth data type.</param>
            private void Decode(byte[] dataType)
            {
                int index = 0;
                for (int beam = 0; beam < ElementsMultiplier; beam++)
                {
                    for (int bin = 0; bin < NumElements; bin++)
                    {
                        index = GetBinBeamIndex(NameLength, NumElements, beam, bin);
                        GoodEarthData[bin, beam] = Converters.ByteArrayToInt(dataType, index);
                    }
                }
            }

            /// <summary>
            /// Get all the Good Earth ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin Bin Beams.
            /// </summary>
            /// <param name="dataTable">DataTable containing the Good Earth data type.</param>
            private void Decode(DataTable dataTable)
            {
                // Go through the result settings the ranges
                foreach (DataRow r in dataTable.Rows)
                {
                    int bin = Convert.ToInt32(r[DbCommon.COL_BV_BIN_NUM].ToString());
                    int beam = Convert.ToInt32(r[DbCommon.COL_BV_BEAM_NUM].ToString());
                    int value = Convert.ToInt32(r[DbCommon.COL_BV_GOOD_EARTH].ToString());
                    GoodEarthData[bin, beam] = value;
                }
            }

            /// <summary>
            /// Generate a byte array representing the
            /// dataset.  The byte array is in the binary format.
            /// The format can be found in the RTI ADCP User Guide.
            /// It contains a header and payload.  This byte array 
            /// will be combined with the other dataset byte arrays
            /// to form an ensemble.
            /// </summary>
            /// <returns>Byte array of the ensemble.</returns>
            public byte[] Encode()
            {
                // Calculate the payload size
                int payloadSize = (NumElements * ElementsMultiplier * Ensemble.BYTES_IN_FLOAT);

                // The size of the array is the header of the dataset
                // and the binxbeams value with each value being a float.
                byte[] result = new byte[GetBaseDataSize(NameLength) + payloadSize];

                // Add the header to the byte array
                byte[] header = GenerateHeader(NumElements);
                System.Buffer.BlockCopy(header, 0, result, 0, header.Length);

                // Add the payload to the results
                int index = 0;
                for (int beam = 0; beam < ElementsMultiplier; beam++)
                {
                    for (int bin = 0; bin < NumElements; bin++)
                    {
                        // Get the index for the next element and add to the array
                        index = GetBinBeamIndex(NameLength, NumElements, beam, bin);
                        System.Buffer.BlockCopy(Converters.IntToByteArray(GoodEarthData[bin, beam]), 0, result, index, Ensemble.BYTES_IN_FLOAT);
                    }
                }

                return result;
            }

            /// <summary>
            /// Override the ToString to return all the Good EarthVelocity data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string s = "";
                for (int bin = 0; bin < NumElements; bin++)
                {
                    s += "GE Bin: " + bin + "\t";
                    for (int beam = 0; beam < ElementsMultiplier; beam++)
                    {
                        s += "\t" + beam + ": " + GoodEarthData[bin, beam];
                    }
                    s += "\n";
                }

                return s;
            }
        }
    }
}