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
 * 09/01/2011      RC                     Initial coding
 * 10/04/2011      RC                     Added AmplitudeBinList to display text.
 *                                         Added method to populate the list.
 * 10/25/2011      RC                     Added new constructor that takes no data and made CreateBinList public.
 * 12/07/2011      RC          1.08       Remove BinList     
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
        /// Data set containing all the Amplitude data.
        /// </summary>
        public class AmplitudeDataSet : BaseDataSet
        {
            /// <summary>
            /// Store all the Amplitude data for the ADCP.
            /// The 2D array is based off:
            /// (number of Bin) Bin (number of beams)
            /// </summary>
            public float[,] AmplitudeData { get; private set; }

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
            /// Create an Amplitude data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public AmplitudeDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                AmplitudeData = new float[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = BeamOrientation.DOWN;
            }

            /// <summary>
            /// Create an Amplitude data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="amplitudeData">Byte array containing Amplitude data</param>
            public AmplitudeDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] amplitudeData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                AmplitudeData = new float[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = BeamOrientation.DOWN;

                // Decode the byte array for Amplitude data
                DecodeAmplitudeData(amplitudeData);

                // Create a list of all bins
                //CreateBinList();
            }

            /// <summary>
            /// Create an Amplitude data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="amplitudeData">DataTable containing Amplitude data</param>
            public AmplitudeDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataTable amplitudeData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                AmplitudeData = new float[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = BeamOrientation.DOWN;

                // Decode the byte array for Amplitude data
                DecodeAmplitudeData(amplitudeData);

                // Create a list of all bins
                //CreateBinList();
            }

            /// <summary>
            /// Get all the Amplitude ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin Bin Beams.
            /// </summary>
            /// <param name="dataType">Byte array containing the Amplitude data type.</param>
            private void DecodeAmplitudeData(byte[] dataType)
            {
                int index = 0;
                for (int beam = 0; beam < ElementsMultiplier; beam++)
                {
                    for (int bin = 0; bin < NumElements; bin++)
                    {
                        index = GetBinBeamIndex(NameLength, NumElements, beam, bin);
                        AmplitudeData[bin, beam] = Converters.ByteArrayToFloat(dataType, index);
                    }
                }
            }


            /// <summary>
            /// Get all the Amplitude ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin y Beams.
            /// </summary>
            /// <param name="dataTable">DataTable containing the Amplitude data type.</param>
            private void DecodeAmplitudeData(DataTable dataTable)
            {
                // Go through the result settings the settings
                // If more than 1 result is found, return the first one found
                foreach (DataRow r in dataTable.Rows)
                {
                    int bin = Convert.ToInt32(r[DbCommon.COL_BV_BIN_NUM].ToString());
                    int beam = Convert.ToInt32(r[DbCommon.COL_BV_BEAM_NUM].ToString());
                    float value = Convert.ToSingle(r[DbCommon.COL_BV_AMP].ToString());
                    AmplitudeData[bin, beam] = value;
                }
            }

            ///// <summary>
            ///// Create a list containing all the Bin entries.
            ///// This is used for the text display.  
            ///// </summary>
            //public void CreateBinList()
            //{

            //    // Create the list
            //    AmplitudeBinList = new BindingList<BinEntry<float>>();

            //    // Enter all the info
            //    for (int bin = 0; bin < NumElements; bin++)
            //    {
            //        AmplitudeBinList.Add(new BinEntry<float>(
            //                                                (bin + 1),
            //                                                AmplitudeData[bin, 0],
            //                                                AmplitudeData[bin, 1],
            //                                                AmplitudeData[bin, 2],
            //                                                AmplitudeData[bin, 3]));
            //    }
            //}

            /// <summary>
            /// Override the ToString to return all the Amplitude data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string s = "";
                for (int bin = 0; bin < NumElements; bin++)
                {
                    s += "A Bin: " + bin + "\t";
                    for (int beam = 0; beam < ElementsMultiplier; beam++)
                    {
                        s += "\t" + beam + ": " + AmplitudeData[bin, beam];
                    }
                    s += "\n";
                }

                return s;
            }
        }
    }
}