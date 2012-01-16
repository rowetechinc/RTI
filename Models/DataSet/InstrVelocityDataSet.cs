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
 * 10/04/2011      RC                     Added InstrumentVelocityBinList to display text.
 *                                        Added method to populate the list.
 * 10/05/2011      RC                     Round ranges to display and look for bad velocity
 * 10/25/2011      RC                     Added new constructor that takes no data and made CreateBinList public.
 * 12/07/2011      RC          1.08       Remove BinList
 * 12/09/2011      RC          1.09       Make orientation a parameter with a default value.
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
        /// Data set containing all the InstrumentVelocity BeamVelocity data.
        /// </summary>
        public class InstrVelocityDataSet : BaseDataSet
        {
            /// <summary>
            /// Store all the InstrumentVelocity velocity data for the ADCP.
            /// </summary>
            public float[,] InstrumentVelocityData { get; private set; }

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
            /// Create an Instrument Velocity data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="orientation">Orientation of the beams.  Default Down.</param>
            public InstrVelocityDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                InstrumentVelocityData = new float[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;
            }

            /// <summary>
            /// Create an Instrument Velocity data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="velocityData">Byte array containing Instrument Velocity data</param>
            /// <param name="orientation">Orientation of the beams.  Default Down.</param>
            public InstrVelocityDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] velocityData, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                InstrumentVelocityData = new float[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;

                // Decode the byte array for velocity data
                DecodeInstrVelocityData(velocityData);
            }

            /// <summary>
            /// Create an Instrument Velocity data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="velocityData">DataTable containing Instrument Velocity data</param>
            /// <param name="orientation">Orientation of the beams.  Default Down.</param>
            public InstrVelocityDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataTable velocityData, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                InstrumentVelocityData = new float[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;

                // Decode the byte array for velocity data
                DecodeInstrVelocityData(velocityData);
            }

            /// <summary>
            /// Get all the InstrumentVelocity velocity ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin y Beams.
            /// </summary>
            /// <param name="dataType">Byte array containing the BeamVelocity data type.</param>
            private void DecodeInstrVelocityData(byte[] dataType)
            {
                int index = 0;
                for (int beam = 0; beam < ElementsMultiplier; beam++)
                {
                    for (int bin = 0; bin < NumElements; bin++)
                    {
                        index = GetBinBeamIndex(NameLength, NumElements, beam, bin);
                        InstrumentVelocityData[bin, beam] = Converters.ByteArrayToFloat(dataType, index);
                    }
                }
            }

            /// <summary>
            /// Get all the Instrument Velocity ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin y Beams.
            /// </summary>
            /// <param name="dataTable">DataTable containing the Instrument Velocity data type.</param>
            private void DecodeInstrVelocityData(DataTable dataTable)
            {
                // Go through the result settings the ranges
                foreach (DataRow r in dataTable.Rows)
                {
                    int bin = Convert.ToInt32(r[DbCommon.COL_BV_BIN_NUM].ToString());
                    int beam = Convert.ToInt32(r[DbCommon.COL_BV_BEAM_NUM].ToString());
                    float value = Convert.ToSingle(r[DbCommon.COL_BV_VEL_INSTR].ToString());
                    InstrumentVelocityData[bin, beam] = value;
                }
            }

            /// <summary>
            /// Override the ToString to return all the velocity data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string s = "";
                for (int bin = 0; bin < NumElements; bin++)
                {
                    s += "I Bin: " + bin + "\t";
                    for (int beam = 0; beam < ElementsMultiplier; beam++)
                    {
                        s += "\t" + beam + ": " + InstrumentVelocityData[bin, beam];
                    }
                    s += "\n";
                }

                return s;
            }
        }
    }
}