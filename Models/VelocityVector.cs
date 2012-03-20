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
 * 12/28/2011      RC          1.10       Initial coding.
 * 02/24/2012      RC          2.03       Create VelocityVectorHelper with a method to create velocity vectors here.
 * 03/16/2012      RC          2.06       Set the magnitude to absolute value.  No negatives.
 *       
 * 
 */


using System;
namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// A struct to hold the velocity vectors
        /// for an ensemble.
        /// </summary>
        public struct EnsembleVelocityVectors
        {
            /// <summary>
            /// Unique ID for the ensemble.
            /// </summary>
            public DataSet.UniqueID Id;

            /// <summary>
            /// Array of vectors representing
            /// the velocities for each bin.
            /// </summary>
            public VelocityVector[] Vectors;
        }

        /// <summary>
        /// Represent the velocity data for each
        /// bin as a vector.  This will store the
        /// velocity data as magnitude and direction.
        /// Direction is given with X and Y North.
        /// </summary>
        public struct VelocityVector
        {
            /// <summary>
            /// Magnitude for the bin with 
            /// Bottom Track velocity removed.
            /// </summary>
            public double Magnitude;

            /// <summary>
            /// Direction of the bin velocity
            /// with Bottom Track velocity removed.
            /// The positive X direction is North.
            /// </summary>
            public double DirectionXNorth;

            /// <summary>
            /// Direction of the bin velocity
            /// with Bottom Track velocity removed.
            /// The positive Y direction is North.
            /// </summary>
            public double DirectionYNorth;
        }

        /// <summary>
        /// Helper class to create velocity vectors
        /// for an ensemble.
        /// </summary>
        public class VelocityVectorHelper
        {
            /// <summary>
            /// Create a VelocityVector array based off the ensemble given.  Earth velocity
            /// data needs to be available for a result to be returned.  This will remove the
            /// ship speed either using Bottom Track data or GPS speed.  The Bottom Track data
            /// is passed in to ensure the previous good Bottom Track data is used if current
            /// Bottom Track data is bad.  GPS speed will be used if Bottom Track data is bad.
            /// If both Bottom Track and GPS are bad, nothing will be used.  Direction is given
            /// with Y both North and East.
            /// </summary>
            /// <param name="ensemble">Ensemble to remove ship speed and create VelocityVector array.</param>
            /// <returns>VelocityVector created.</returns>
            public static bool CreateVelocityVector(ref DataSet.Ensemble ensemble)
            {
                if (ensemble.IsEarthVelocityAvail)
                {
                    // Create array to store all the vectors
                    ensemble.EarthVelocityData.IsVelocityVectorAvail = true;
                    ensemble.EarthVelocityData.VV = new VelocityVector[ensemble.EarthVelocityData.NumElements];

                    // Create a vector for each bin
                    for (int bin = 0; bin < ensemble.EarthVelocityData.NumElements; bin++)
                    {
                        // Get the velocity values
                        float east = ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX];
                        float north = ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX];
                        float vertical = ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX];

                        // If any of the velocities are bad, then set bad velocities for all the velocities and move to the next bin
                        if (east == DataSet.Ensemble.BAD_VELOCITY ||
                            north == DataSet.Ensemble.BAD_VELOCITY ||
                            vertical == DataSet.Ensemble.BAD_VELOCITY)
                        {
                            ensemble.EarthVelocityData.VV[bin].Magnitude = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.EarthVelocityData.VV[bin].DirectionXNorth = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.EarthVelocityData.VV[bin].DirectionYNorth = DataSet.Ensemble.BAD_VELOCITY;
                        }
                        else
                        {

                            // Calculate the magnitude of the velocity
                            //double mag = MathHelper.CalculateMagnitude(ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX],
                            //                                           ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX],
                            //                                           ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX]);
                            double mag = MathHelper.CalculateMagnitude(ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX],
                                                                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX],
                                                                        0);      // do not use Vertical

                            ensemble.EarthVelocityData.VV[bin].Magnitude = Math.Abs(mag);
                            ensemble.EarthVelocityData.VV[bin].DirectionXNorth = MathHelper.CalculateDirection(east, north);
                            ensemble.EarthVelocityData.VV[bin].DirectionYNorth = MathHelper.CalculateDirection(north, east);
                        }
                    }

                    // VelocityVector array created
                    return true;
                }

                // VelocityVector array could not be created
                return false;
            }
        }
    }

}