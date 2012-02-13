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
 * 01/16/2012      RC          1.14       Initial coding
 * 
 */

using RTI.DataSet;
using System;
namespace RTI
{
    namespace Screen
    {
        /// <summary>
        /// This screening has two options.  RemoveSpeed() will remove
        /// the ship speed from the referenced ensemble given.  GetVelocityVector()
        /// will create a vector based off the water velocity for each bin.  It will
        /// also remove ship speed from the data.  The vector will given direction where
        /// Y is North or East.
        /// </summary>
        public class RemoveShipSpeed
        {
            ///<summary>
            /// Remove the ship speed from the ensemble.  This will take the Bottom Track
            /// data and add it to the Earth velocity.  It is added to the velocitiy because
            /// the Bottom Track data is inverted from the Earth data.  If the Bottom Track 
            /// data is bad, it will not be used to remove the ship speed.
            /// </summary>
            /// <param name="ensemble">Ensemble to remove ship speed.</param>
            /// <param name="btEast">Bottom Track East Velocity.</param>
            /// <param name="btNorth">Bottom Track North Velocity.</param>
            /// <param name="btVertical">Bottom Track Vertical Velocity.</param>
            /// <returns>Velocity magnitude with Bottom Track speed removed.  If any values are bad, it will return BAD_VELOCITY.</returns>
            public static bool RemoveSpeed(ref DataSet.Ensemble ensemble, float btEast, float btNorth, float btVertical)
            {
                if(ensemble.IsEarthVelocityAvail)
                {
                   // If any of the Bottom Track velocities are bad, then do not remove Bottom Track velocities.
                    if (btEast == DataSet.Ensemble.BAD_VELOCITY || btNorth == DataSet.Ensemble.BAD_VELOCITY || btVertical == DataSet.Ensemble.BAD_VELOCITY)
                    {
                        btEast = 0;
                        btNorth = 0;
                        btVertical = 0;

                        return false;
                    }

                    // Remove ship speed for each bin
                    for (int bin = 0; bin < ensemble.EarthVelocityData.NumElements; bin++)
                    {
                        // Remove Bottom Track velocity
                        // Bottom Track velocity is inverted
                        // So add bottom track velocity to remove velocity
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] += btEast;
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] += btNorth;
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] += btVertical;
                    }

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Create a VelocityVector array based off the ensemble given.  Earth velocity
            /// data needs to be available for a result to be returned.    This will remove the
            /// ship speed either using Bottom Track data or GPS speed.  The Bottom Track data
            /// is passed in to ensure the previous good Bottom Track data is used if current
            /// Bottom Track data is bad.  GPS speed will be used if Bottom Track data is bad.
            /// If both Bottom Track and GPS are bad, nothing will be used.  Direction is given
            /// with Y both North and East.
            /// </summary>
            /// <param name="ensemble">Ensemble to remove ship speed and create VelocityVector array.</param>
            /// <param name="btEast">Bottom Track East Velocity.</param>
            /// <param name="btNorth">Bottom Track North Velocity.</param>
            /// <param name="btVertical">Bottom Track Vertical Velocity.</param>
            /// <param name="gpsSpeed">Ship speed from the GPS.</param>
            /// <returns>Array containing vectors for all bins.</returns>
            public static bool CreateVelocityVector(ref DataSet.Ensemble ensemble, float btEast, float btNorth, float btVertical, double gpsSpeed)
            {
                if (ensemble.IsEarthVelocityAvail)
                {
                    // Create array to store all the vectors
                    ensemble.EarthVelocityData.IsVelocityVectorAvail = true;
                    ensemble.EarthVelocityData.VV = new VelocityVector[ensemble.EarthVelocityData.NumElements];

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
                    for (int bin = 0; bin < ensemble.EarthVelocityData.NumElements; bin++)
                    {
                        // Get the velocity values
                        float east = ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX];
                        float north = ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX];
                        float vertical = ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX];

                        // If any of the velocities are bad, then set bad velocities for the vector and move to the next bin
                        if (east == DataSet.Ensemble.BAD_VELOCITY || north == DataSet.Ensemble.BAD_VELOCITY || vertical == DataSet.Ensemble.BAD_VELOCITY)
                        {
                            ensemble.EarthVelocityData.VV[bin].Magnitude = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.EarthVelocityData.VV[bin].DirectionXNorth = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.EarthVelocityData.VV[bin].DirectionYNorth = DataSet.Ensemble.BAD_VELOCITY;
                            break;
                        }

                        // Remove the Bottom Track speed if we can
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] += btEast;
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] += btNorth;
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] += btVertical;

                        // Calculate the magnitude of the velocity
                        double mag = MathHelper.CalculateMagnitude(ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX],
                                                                   ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX],
                                                                   ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX]);
                        
                        // If bottom track velocity was bad, try to remove 
                        // ship speed with gps speed
                        // Both Bottom Track and GPS speed cannot be used together
                        mag -= gpsSpeed;

                        ensemble.EarthVelocityData.VV[bin].Magnitude = mag;
                        ensemble.EarthVelocityData.VV[bin].DirectionXNorth = MathHelper.CalculateDirection(east, north);
                        ensemble.EarthVelocityData.VV[bin].DirectionYNorth = MathHelper.CalculateDirection(north, east);
                    }

                    return true;
                }

                // VelocityVector array could not be created
                return false;
            }
        }
    
    }
}