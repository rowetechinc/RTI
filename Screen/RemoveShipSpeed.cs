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
 * 02/15/2012      RC          2.03       Renamed to RemoveBottomTrackVelocity.
 * 02/17/2012      RC          2.03       Added determining which value to use, BT, GPS or Previous BT.
 * 02/24/2012      RC          2.03       Put all the logic to remove the ship speed in RemoveVelocity.
 *                                         Put method to create velocity vector in VelocityVector.cs.
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
            /// <param name="btPrevEast">Previous good Bottom Track East Velocity.</param>
            /// <param name="btPrevNorth">Previous good Bottom Track North Velocity.</param>
            /// <param name="btPrevVertical">Previous good Bottom Track Vertical Velocity.</param>
            /// <param name="CanUseBT">Flag if we can use Bottom Track velocity to remove ship speed.</param>
            /// <param name="CanUseGps">Flag if we can use GPS velocity to remove ship speed.</param>
            /// <returns>TRUE = Bottom Track Speed removed from Earth Velocity data.</returns>
            public static bool RemoveVelocity(ref DataSet.Ensemble ensemble, float btPrevEast, float btPrevNorth, float btPrevVertical, bool CanUseBT, bool CanUseGps)
            {
                if(ensemble.IsEarthVelocityAvail)
                {
                    bool isBTVelGood = false;
                    bool isGpsVelGood = false;

                    // Check if we can use Bottom Track velocity
                    if (CanUseBT)
                    {
                        // Check if Bottom Track Velocity is good
                        if (ensemble.BottomTrackData.IsEarthVelocityGood())
                        {
                            isBTVelGood = true;
                        }
                    }
                    else
                    {
                        // If you cannot use bottom track
                        // Do not also use the previous bottom track
                        btPrevEast = DataSet.Ensemble.BAD_VELOCITY;
                        btPrevNorth = DataSet.Ensemble.BAD_VELOCITY;
                        btPrevVertical = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    // Check if we can use GPS speed
                    if (CanUseGps)
                    {
                        // Check if Gps Speed is good
                        if (ensemble.IsNmeaAvail)
                        {
                            if (ensemble.NmeaData.IsGpvtgAvail())
                            {
                                if (ensemble.NmeaData.IsGpsSpeedGood())
                                {
                                    isGpsVelGood = true;
                                }
                            }
                        }
                    }

                    float btEast = 0;
                    float btNorth = 0;
                    float btVertical = 0;

                    // GPS speed is good and Bottom Track Speed is bad
                    // Use GPS speed
                    if(isGpsVelGood && !isBTVelGood)
                    {
                        double heading = ensemble.AncillaryData.Heading;
                        double speed = ensemble.NmeaData.GPVTG.Speed.ToMetersPerSecond().Value;

                        // Calculate the East and North component of the GPS speed
                        btEast = Convert.ToSingle(speed * Math.Sin(MathHelper.DegreeToRadian(heading)));
                        btNorth = Convert.ToSingle(speed * Math.Cos(MathHelper.DegreeToRadian(heading)));

                        // We do not have a vertical velocity using GPS speed, so try to use the Bottom Track
                        if (ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            btVertical = ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                        }
                    }
                    // BT is good
                    else if (isBTVelGood)
                    {
                        btEast = ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX];
                        btNorth = ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX];
                        btVertical = ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                    }
                    // GPS is bad and Bottom Track is bad, so use previous bottom track values
                    else
                    {
                        // Ensure previous is good
                        if (btPrevEast != DataSet.Ensemble.BAD_VELOCITY && btPrevNorth != DataSet.Ensemble.BAD_VELOCITY && btPrevVertical != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            btEast = btPrevEast;
                            btNorth = btPrevNorth;
                            btVertical = btPrevVertical;
                        }
                        else
                        {
                            // No bottom track information is good
                            // So do nothing
                            return false;
                        }
                    }

                    // Remove ship speed for each bin
                    for (int bin = 0; bin < ensemble.EarthVelocityData.NumElements; bin++)
                    {
                        // Remove Bottom Track velocity
                        // Bottom Track velocity is inverted
                        // So add bottom track velocity to remove velocity
                        // Check if the velocity is good before removing the ship speed
                        if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] += btEast;
                        }
                        if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] += btNorth;
                        }
                        if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] += btVertical;
                        }
                    }

                    return true;
                }

                return false;
            }
        }
    
    }
}