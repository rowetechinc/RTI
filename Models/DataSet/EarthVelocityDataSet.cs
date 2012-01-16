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
            public float[,] EarthVelocityData { get; private set; }

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
                EarthVelocityData = new float[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;

                // Decode the byte array for velocity data
                DecodeEarthVelocityData(velocityData);
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
                EarthVelocityData = new float[NumElements, ElementsMultiplier];

                // Default orientation value is DOWN
                Orientation = orientation;

                // Decode the byte array for velocity data
                DecodeEarthVelocityData(velocityData);
            }

            /// <summary>
            /// Get all the Earth Velocity ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin y Beams.
            /// </summary>
            /// <param name="dataType">Byte array containing the Earth Velocity data type.</param>
            private void DecodeEarthVelocityData(byte[] dataType)
            {
                int index = 0;

                for (int beam = 0; beam < ElementsMultiplier; beam++)
                {
                    for (int bin = 0; bin < NumElements; bin++)
                    {
                        index = GetBinBeamIndex(NameLength, NumElements, beam, bin);
                        EarthVelocityData[bin, beam] = Converters.ByteArrayToFloat(dataType, index);
                    }
                }
            }

            /// <summary>
            /// Get all the Earth Velocity ranges for each beam and Bin.
            /// 
            /// I changed the order from what the data is stored as and now make it Bin y Beams.
            /// </summary>
            /// <param name="dataTable">DataTable containing the Earth Velocity data type.</param>
            private void DecodeEarthVelocityData(DataTable dataTable)
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
            /// Calculate the Magnitude given the North, East and Vertical velocity.
            /// </summary>
            /// <param name="east">East Velocity.</param>
            /// <param name="north">North Velocity.</param>
            /// <param name="vertical">Vertical Velocity.</param>
            /// <returns>Magnitude of the velocities given.</returns>
            private double CalculateMagnitude(double east, double north, double vertical)
            {
                return Math.Sqrt((east * east) + (north * north) + (vertical * vertical));
            }

            /// <summary>
            /// Calculate the Direction of the velocities given.
            /// Value will be returned in degrees.
            /// </summary>
            /// <param name="y">Y axis velocity value.</param>
            /// <param name="x">X axis velocity value.</param>
            /// <returns>Direction of the velocity return in degrees.</returns>
            private double CalculateDirection(double y, double x)
            {
                return (Math.Atan2(y, x)) * (180 / Math.PI);
            }

            /// <summary>
            /// Given the bin, calculate the magnitude of the
            /// velocity by removing the Bottom Track velocity
            /// from the velocity.  If any of the Bottom Track
            /// velocities are bad, it will return BAD_VELOCITY.
            /// If any of the velocities are bad, it will
            /// return BAD_VELOCITY.
            /// </summary>
            /// <param name="btEast">Bottom Track East Velocity.</param>
            /// <param name="btNorth">Bottom Track North Velocity.</param>
            /// <param name="btVertical">Bottom Track Vertical Velocity.</param>
            /// <param name="gpsSpeed">Ship speed from the GPS.</param>
            /// <returns>Velocity magnitude with Bottom Track speed removed.  If any values are bad, it will return BAD_VELOCITY.</returns>
            public VelocityVector[] GetVelocityVectors(float btEast, float btNorth, float btVertical, double gpsSpeed)
            {
                // Create array to store all the vectors
                VelocityVector[] vv = new VelocityVector[this.NumElements];

                // If any of the Bottom Track velocities are bad, then do not remove Bottom Track velocities.
                if (btEast == DataSet.Ensemble.BAD_VELOCITY || btNorth == DataSet.Ensemble.BAD_VELOCITY || btVertical == DataSet.Ensemble.BAD_VELOCITY)
                {
                    btEast = 0;
                    btNorth = 0;
                    btVertical = 0;

                    // Check if GPS speed can be used
                    if (gpsSpeed == DataSet.Ensemble.BAD_VELOCITY)
                    {
                        gpsSpeed = 0;
                    }
                }
                else
                {
                    // Bottom Track speed was good,
                    // do not use GPS speed
                    gpsSpeed = 0;
                }

                // Create a vector for each bin
                for (int bin = 0; bin < this.NumElements; bin++)
                {
                    // Get the velocity values
                    float east = this.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX];
                    float north = this.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX];
                    float vertical = this.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX];

                    // If any of the velocities are bad, then set bad velocities for the vector and move to the next bin
                    if(east == DataSet.Ensemble.BAD_VELOCITY || north == DataSet.Ensemble.BAD_VELOCITY || vertical == DataSet.Ensemble.BAD_VELOCITY)
                    {
                        vv[bin].Magnitude = DataSet.Ensemble.BAD_VELOCITY;
                        vv[bin].DirectionXNorth = DataSet.Ensemble.BAD_VELOCITY;
                        vv[bin].DirectionYNorth = DataSet.Ensemble.BAD_VELOCITY;
                        break;
                    }

                    // Remove Bottom Track velocity
                    // Bottom Track velocity is inverted
                    // So add bottom track velocity to remove velocity
                    east += btEast;
                    north += btNorth;
                    vertical += btVertical;

                    // If bottom track velocity was bad, try to remove 
                    // boat speed with gps speed
                    // Both Bottom Track and GPS speed cannot be used together
                    double mag = CalculateMagnitude(east, north, vertical) - gpsSpeed;

                    vv[bin].Magnitude = mag;
                    vv[bin].DirectionXNorth = CalculateDirection(east, north);
                    vv[bin].DirectionYNorth = CalculateDirection(north, east);
                }

                return vv;
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
                        return (CalculateMagnitude(EarthVelocityData[bin, 0], EarthVelocityData[bin, 1], EarthVelocityData[bin, 2]));
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
                            return CalculateDirection(EarthVelocityData[bin, 1], EarthVelocityData[bin, 0]);
                        }
                        else
                        {
                            //return (Math.Atan2(EarthVelocityData[Bin, 0], EarthVelocityData[Bin, 1])) * (180 / Math.PI);
                            return CalculateDirection(EarthVelocityData[bin, 0], EarthVelocityData[bin, 1]);
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
        }
    }
}