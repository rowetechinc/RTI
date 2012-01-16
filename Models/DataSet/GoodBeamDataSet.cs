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
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 09/01/2011      RC                     Initial coding
 * 10/04/2011      RC                     Added GoodBeamBinList to display text.
 *                                         Added method to populate the list.
 * 10/25/2011      RC                     Added new constructor that takes no data and made CreateBinList public.
 * 12/07/2011      RC          1.08       Remove BinList     
 * 12/09/2011      RC          1.09       Make orientation a parameter with a default value.
 *       
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
        /// Data set containing all the Good Beam data.
        /// </summary>
        public class GoodBeamDataSet : BaseDataSet
        {
            /// <summary>
            /// Store all the Good Beam data for the ADCP.
            /// </summary>
            public int[,] GoodBeamData { get; private set; }

            /// <summary>
            /// This value will be used when there will be multiple transducers
            /// giving ranges.  There will be duplicate beam Bin Bin ranges and the
            /// orientation will determine which beam Bin Bin ranges to group together.
            /// 
            /// This may not be necessary and additional datatypes may be created instead
            /// for each transducer orientation.
            /// </summary>
            public BaseDataSet.BeamOrientation Orientation { get; private set; }

            /// <summary>
            /// Create an Good Beam data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="orientation">Orientation of the beams.</param>
            public GoodBeamDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                GoodBeamData = new int[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;
            }

            /// <summary>
            /// Create an Good Beam data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="goodBeamData">Byte array containing Good Beam data</param>
            /// <param name="orientation">Orientation of the beams.</param>
            public GoodBeamDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] goodBeamData, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                GoodBeamData = new int[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;

                // Decode the byte array for Good Beam data
                DecodeGoodBeamData(goodBeamData);
            }

            /// <summary>
            /// Create an Good Beam data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="goodBeamData">DataTable containing Good Beam data</param>
            /// <param name="orientation">Orientation of the beams.</param>
            public GoodBeamDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataTable goodBeamData, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                GoodBeamData = new int[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;

                // Decode the byte array for Good Beam data
                DecodeGoodBeamData(goodBeamData);
            }

            /// <summary>
            /// Get all the Good Beam ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin and Beams.
            /// </summary>
            /// <param name="dataType">Byte array containing the Good Beam data type.</param>
            private void DecodeGoodBeamData(byte[] dataType)
            {
                int index = 0;
                for (int beam = 0; beam < ElementsMultiplier; beam++)
                {
                    for (int bin = 0; bin < NumElements; bin++)
                    {
                        index = GetBinBeamIndex(NameLength, NumElements, beam, bin);
                        GoodBeamData[bin, beam] = Converters.ByteArrayToInt(dataType, index);
                    }
                }
            }

            /// <summary>
            /// Get all the Good Beam ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin and Beams.
            /// </summary>
            /// <param name="dataTable">DataTable containing the Good Beam data type.</param>
            private void DecodeGoodBeamData(DataTable dataTable)
            {
                // Go through the result settings the ranges
                foreach (DataRow r in dataTable.Rows)
                {
                    int bin = Convert.ToInt32(r[DbCommon.COL_BV_BIN_NUM].ToString());
                    int beam = Convert.ToInt32(r[DbCommon.COL_BV_BEAM_NUM].ToString());
                    int value = Convert.ToInt32(r[DbCommon.COL_BV_GOOD_BEAM].ToString());
                    GoodBeamData[bin, beam] = value;
                }
            }

            /// <summary>
            /// Override the ToString to return all the Good Beam data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string s = "";
                for (int bin = 0; bin < NumElements; bin++)
                {
                    s += "GB Bin: " + bin + "\t";
                    for (int beam = 0; beam < ElementsMultiplier; beam++)
                    {
                        s += "\t" + beam + ": " + GoodBeamData[bin, beam];
                    }
                    s += "\n";
                }

                return s;
            }
        }
    }
}