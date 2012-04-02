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
 * 10/04/2011      RC                     Added EarthVelocityBinList to display text.
 *                                         Added method to populate the list.
 * 10/05/2011      RC                     Round ranges to display and look for bad velocity
 * 10/25/2011      RC                     Added new constructor that takes no data and made CreateBinList public.
 * 11/03/2011      RC                     Added methods to get Magnitude, direction and XY points.
 * 11/15/2011      RC                     Added try/catch block in GetVelocityDirection() and GetVelocityMagnitude().  
 * 12/07/2011      RC          1.08       Remove BinList     
 * 12/09/2011      RC          1.09       Make orientation a parameter with a default value.
 * 12/28/2011      RC          1.10       Added GetVelocityVectors() to create array of vectores for velocity.
 * 01/19/2012      RC          1.14       Added Encode() to create a byte array of data.
 *                                         Removed "private set".
 *                                         Rename Decode methods to Decode().
 * 03/30/2012      RC          2.07       Moved Converters.cs methods to MathHelper.cs.
 * 
 */

using System;
using System.Data;

namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the EarthVelocity BeamVelocity data.
        /// </summary>
        public class EarthVelocityDataSet : BaseDataSet
        {
            /// <summary>
            /// Store all the EarthVelocity velocity data for the ADCP.
            /// </summary>
            public float[,] EarthVelocityData { get; set; }

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
            /// Flag if the Velocity Vector Array is available.  This 
            /// array is created if screening is turned on and an array
            /// was created during the screening process.
            /// </summary>
            public bool IsVelocityVectorAvail { get; set; }

            /// <summary>
            /// Velocity Vector.  This is an array of VelocityVectors.
            /// The VelocityVector holds the magnitude and direciton for the 
            /// water velocity in a bin.  Each bin will have a VelocityVector.
            /// During screening this array can be created.  The screening
            /// will also remove the ship speed before creating the vector.
            /// </summary>
            public VelocityVector[] VV { get; set; }

            /// <summary>
            /// Create an Earth Velocity data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="orientation">Orientation of the beams.</param>
            public EarthVelocityDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                IsVelocityVectorAvail = false;
                EarthVelocityData = new float[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;
            }

            /// <summary>
            /// Create an Earth Velocity data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="velocityData">Byte array containing EarthVelocity velocity data</param>
            /// <param name="orientation">Orientation of the beams.</param>
            public EarthVelocityDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] velocityData, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                IsVelocityVectorAvail = false;
                EarthVelocityData = new float[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;

                // Decode the byte array for velocity data
                Decode(velocityData);
            }

            /// <summary>
            /// Create an Earth Velocity data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="velocityData">DataTable containing EarthVelocity velocity data</param>
            /// <param name="orientation">Orientation of the beams.</param>
            public EarthVelocityDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataTable velocityData, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                IsVelocityVectorAvail = false;
                EarthVelocityData = new float[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;

                // Decode the byte array for velocity data
                Decode(velocityData);
            }

            /// <summary>
            /// Get all the Earth Velocity ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin y Beams.
            /// </summary>
            /// <param name="dataType">Byte array containing the Earth Velocity data type.</param>
            private void Decode(byte[] dataType)
            {
                int index = 0;

                for (int beam = 0; beam < ElementsMultiplier; beam++)
                {
                    for (int bin = 0; bin < NumElements; bin++)
                    {
                        index = GetBinBeamIndex(NameLength, NumElements, beam, bin);
                        EarthVelocityData[bin, beam] = MathHelper.ByteArrayToFloat(dataType, index);
                    }
                }
            }

            /// <summary>
            /// Get all the Earth Velocity ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin y Beams.
            /// </summary>
            /// <param name="dataTable">DataTable containing the Earth Velocity data type.</param>
            private void Decode(DataTable dataTable)
            {
                // Go through the result settings the ranges
                foreach (DataRow r in dataTable.Rows)
                {
                    int bin = Convert.ToInt32(r[DbCommon.COL_BV_BIN_NUM].ToString());
                    int beam = Convert.ToInt32(r[DbCommon.COL_BV_BEAM_NUM].ToString());
                    float value = Convert.ToSingle(r[DbCommon.COL_BV_VEL_EARTH].ToString());
                    EarthVelocityData[bin, beam] = value;
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
                        System.Buffer.BlockCopy(MathHelper.FloatToByteArray(EarthVelocityData[bin, beam]), 0, result, index, Ensemble.BYTES_IN_FLOAT);
                    }
                }

                return result;
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
                    s += "E Bin: " + bin + "\t";
                    for (int beam = 0; beam < ElementsMultiplier; beam++)
                    {
                        s += "\t" + beam + ": " + EarthVelocityData[bin, beam];
                    }
                    s += "\n";
                }

                return s;
            }

            /// <summary>
            /// Calculate the magnitude of the velocity.  Use Earth form East, North and Vertical velocity to 
            /// calculate the value.
            /// </summary>
            /// <param name="bin">Bin to get the Velocity Magnitude.</param>
            /// <returns>Magnitude of Velocity. If any velocities were bad, DataSet.AdcpDataSet.BAD_VELOCITY is returned.</returns>
            public double GetVelocityMagnitude(int bin)
            {
                try
                {
                    // Ensure the velocities are good
                    if ((EarthVelocityData[bin, 0] != Ensemble.BAD_VELOCITY) &&
                        (EarthVelocityData[bin, 1] != Ensemble.BAD_VELOCITY) &&
                        (EarthVelocityData[bin, 2] != Ensemble.BAD_VELOCITY))
                    {
                        return (MathHelper.CalculateMagnitude(EarthVelocityData[bin, 0], EarthVelocityData[bin, 1], EarthVelocityData[bin, 2]));
                    }
                    else
                    {
                        return DataSet.Ensemble.BAD_VELOCITY;
                    }
                }
                catch (System.IndexOutOfRangeException)
                {
                    // When the display is changing from one dataset to another, 
                    // the number of bins could change and then it could select out of range.
                    return DataSet.Ensemble.BAD_VELOCITY;
                }
            }

            /// <summary>
            /// Get the direction for the velocity.  Use the parameter
            /// to determine if the Y axis is North or East.  
            /// </summary>
            /// <param name="isYNorth">Set flag if you want Y axis to be North or East.</param>
            /// <param name="bin">Bin to get the Velocity Direction.</param>
            /// <returns>Direction of the velocity in degrees.  If any velocities were bad, return -1.</returns>
            public double GetVelocityDirection(bool isYNorth, int bin)
            {
                try
                {
                    // Ensure the velocities are good
                    if ((EarthVelocityData[bin, 0] != Ensemble.BAD_VELOCITY) &&
                        (EarthVelocityData[bin, 1] != Ensemble.BAD_VELOCITY) &&
                        (EarthVelocityData[bin, 2] != Ensemble.BAD_VELOCITY))
                    {
                        if (isYNorth)
                        {
                            //return (Math.Atan2(EarthVelocityData[Bin, 1], EarthVelocityData[Bin, 0])) * (180 / Math.PI);
                            return MathHelper.CalculateDirection(EarthVelocityData[bin, 1], EarthVelocityData[bin, 0]);
                        }
                        else
                        {
                            //return (Math.Atan2(EarthVelocityData[Bin, 0], EarthVelocityData[Bin, 1])) * (180 / Math.PI);
                            return MathHelper.CalculateDirection(EarthVelocityData[bin, 0], EarthVelocityData[bin, 1]);
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
                catch (System.IndexOutOfRangeException)
                {
                    // When the display is changing from one dataset to another, 
                    // the number of bins could change and then it could select out of range.
                    return -1;
                }
            }
        }
    }
}