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
 * 05/23/2012      RC          2.11       Fixed bug checking if bottom track data is available before trying to use it in RemoveVelocity().
 * 04/26/2013      RC          2.19       Changed namespace from Screen to ScreenData.
 * 08/16/2013      RC          2.19.4     Added gpsHeadingOffset to account for the GPS not aligned with the ADCP.
 * 08/19/2014      RC          3.0.0      Get the heading from the GPS before using the bottom track heading to remove the ship speed when using GPS speed.
 * 09/05/2017      RC          3.4.3      Added GetPreviousBottomTrackVelocity() to get the previous Bottom Track velocity to store for next iteration.
 * 09/13/2017      RC          3.4.3      Check if Bottom Track has no beams.
 * 
 */

using RTI.DataSet;
using System;
using System.Diagnostics;
namespace RTI
{
    namespace ScreenData
    {
        /// <summary>
        /// This screening has two options.  RemoveSpeed() will remove
        /// the ship speed from the referenced ensemble given.  GetVelocityVector()
        /// will create a vector based off the water velocity for each bin.  It will
        /// also remove ship speed from the data.  The vector will given direction where
        /// Y is North or East.
        /// 
        /// Bottom Track velocity is INVERTED from Profile velocity so the values MUST BE ADDED TOGETHER to remove ship speed.
        /// </summary>
        public class RemoveShipSpeed
        {
            ///<summary>
            /// Remove the ship speed from the ensemble.  This will take the Bottom Track
            /// data and add it to the Earth velocity.  It is added to the velocitiy because
            /// the Bottom Track data is inverted from the Earth data.  If the Bottom Track 
            /// data is bad, it will not be used to remove the ship speed.
            /// 
            /// If using GPS speed, the GPS speed is taken from GPVTG message.  The heading is also required.  The heading is taken
            /// from the ensemble Anciallary dataset.  The user will need to change the ensemble's heading source to the GPS so it
            /// can calculated properly.  The GPS heading may not match with the ADCP heading, so a gpsHeadingOffset will also have to be
            /// set to account for the GPS not aligned with the ADCP heading.
            /// 
            /// Bottom Track velocity is INVERTED from Profile velocity so the values MUST BE ADDED TOGETHER to remove ship speed.
            /// </summary>
            /// <param name="ensemble">Ensemble to remove ship speed.</param>
            /// <param name="btPrevEast">Previous good Bottom Track East Velocity.</param>
            /// <param name="btPrevNorth">Previous good Bottom Track North Velocity.</param>
            /// <param name="btPrevVertical">Previous good Bottom Track Vertical Velocity.</param>
            /// <param name="CanUseBT">Flag if we can use Bottom Track velocity to remove ship speed.</param>
            /// <param name="CanUseGps">Flag if we can use GPS velocity to remove ship speed.</param>
            /// <param name="gpsHeadingOffset">GPS Heading offset if the GPS is offset from the ADCP heading.</param>
            /// <param name="isNo3BeamSolution">Flag if 3 Beam solutions should be allowed for Bottom Track velocity.</param>
            /// <returns>TRUE = Bottom Track Speed removed from Earth Velocity data.</returns>
            public static bool RemoveVelocity(ref DataSet.Ensemble ensemble, float btPrevEast, float btPrevNorth, float btPrevVertical, bool CanUseBT, bool CanUseGps, double gpsHeadingOffset = 0.0, bool isNo3BeamSolution = true)
            {
                if (ensemble != null)
                {
                    if (ensemble.IsEarthVelocityAvail)
                    {
                        bool isBTVelGood = false;
                        bool isGpsVelGood = false;

                        // Check if we can use Bottom Track velocity
                        if (CanUseBT && ensemble.IsBottomTrackAvail)
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
                            //btPrevEast = DataSet.Ensemble.BAD_VELOCITY;
                            //btPrevNorth = DataSet.Ensemble.BAD_VELOCITY;
                            //btPrevVertical = DataSet.Ensemble.BAD_VELOCITY;
                        }

                        // Check if we can use GPS speed
                        if (CanUseGps && ensemble.IsNmeaAvail)
                        {
                            // Check if Gps Speed is good
                            if (ensemble.NmeaData.IsGpvtgAvail())
                            {
                                if (ensemble.NmeaData.IsGpsSpeedGood())
                                {
                                    isGpsVelGood = true;
                                }
                            }
                        }

                        float btEast = 0;
                        float btNorth = 0;
                        float btVertical = 0;

                        // GPS speed is good and Bottom Track Speed is bad
                        // Use GPS speed
                        if (isGpsVelGood && !isBTVelGood)
                        {
                            // Heading defaults from ADCP
                            double heading = ensemble.AncillaryData.Heading + gpsHeadingOffset;

                            // Heading from GPS if its available
                            if(ensemble.NmeaData.IsGphdtAvail())
                            {
                                heading = ensemble.NmeaData.GPHDT.Heading.DecimalDegrees + gpsHeadingOffset;
                            }
                            else if (ensemble.NmeaData.IsGpvtgAvail())
                            {
                                heading = ensemble.NmeaData.GPVTG.Bearing.DecimalDegrees + gpsHeadingOffset;
                            }

                            // Speed from the GPS
                            double speed = ensemble.NmeaData.GPVTG.Speed.ToMetersPerSecond().Value;

                            // Calculate the East and North component of the GPS speed
                            btEast = Convert.ToSingle(speed * Math.Sin(MathHelper.DegreeToRadian(heading)));
                            btNorth = Convert.ToSingle(speed * Math.Cos(MathHelper.DegreeToRadian(heading)));

                            // We do not have a vertical velocity using GPS speed, so try to use the Bottom Track
                            if (ensemble.IsBottomTrackAvail && ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                btVertical = ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                            }
                        }
                        // BT is good
                        else if (isBTVelGood)
                        {
                            // Check if there is a 3 beam solution
                            // It may be better to use the previous value than 3 beam solution
                            if (!ensemble.BottomTrackData.IsBeamVelocityGood() && isNo3BeamSolution)
                            {
                                btEast = btPrevEast;
                                btNorth = btPrevNorth;
                                btVertical = btPrevVertical;
                            }
                            else
                            {
                                btEast = ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX];
                                btNorth = ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX];
                                btVertical = ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                            }

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
                                // No ship speed information is good
                                // So do nothing
                                return false;
                            }
                        }

                        // Remove ship speed for each bin
                        for (int bin = 0; bin < ensemble.EarthVelocityData.NumElements; bin++)
                        {
                            // Remove Bottom Track velocity
                            // Bottom Track velocity is INVERTED from Profile velocity so the values MUST BE ADDED TOGETHER to remove ship speed
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

                        // If Velocity Vectors exist
                        // Regenerate the Velocity Vectors
                        if (ensemble.EarthVelocityData.IsVelocityVectorAvail)
                        {
                            VelocityVectorHelper.CreateVelocityVector(ref ensemble);
                        }

                        return true;
                    }

                    return false;
                }
                return false;
            }

            ///<summary>
            /// Remove the ship speed from the ensemble.  This will take the Bottom Track
            /// data and add it to the Instrument velocity.  It is added to the velocitiy because
            /// the Bottom Track data is inverted from the Earth data.  If the Bottom Track 
            /// data is bad, it will not be used to remove the ship speed.
            /// 
            /// If using GPS speed, the GPS speed is taken from GPVTG message.  The heading is also required.  The heading is taken
            /// from the ensemble Anciallary dataset.  The user will need to change the ensemble's heading source to the GPS so it
            /// can calculated properly.  The GPS heading may not match with the ADCP heading, so a gpsHeadingOffset will also have to be
            /// set to account for the GPS not aligned with the ADCP heading.
            /// 
            /// Bottom Track velocity is INVERTED from Profile velocity so the values MUST BE ADDED TOGETHER to remove ship speed.
            /// </summary>
            /// <param name="ensemble">Ensemble to remove ship speed.</param>
            /// <param name="btPrevX">Previous good Bottom Track X Velocity.</param>
            /// <param name="btPrevY">Previous good Bottom Track Y Velocity.</param>
            /// <param name="btPrevZ">Previous good Bottom Track Z Velocity.</param>
            /// <param name="CanUseBT">Flag if we can use Bottom Track velocity to remove ship speed.</param>
            /// <param name="CanUseGps">Flag if we can use GPS velocity to remove ship speed.</param>
            /// <param name="gpsHeadingOffset">GPS Heading offset if the GPS is offset from the ADCP heading.</param>
            /// <param name="isNo3BeamSolution">Flag if 3 Beam solutions should be allowed for Bottom Track velocity.</param>
            /// <returns>TRUE = Bottom Track Speed removed from Instrument Velocity data.</returns>
            public static bool RemoveVelocityInstrument(ref DataSet.Ensemble ensemble, float btPrevX, float btPrevY, float btPrevZ, bool CanUseBT, bool CanUseGps, double gpsHeadingOffset = 0.0, bool isNo3BeamSolution = true)
            {
                if (ensemble != null)
                {
                    if (ensemble.IsEarthVelocityAvail)
                    {
                        bool isBTVelGood = false;
                        bool isGpsVelGood = false;

                        // Check if we can use Bottom Track velocity
                        if (CanUseBT && ensemble.IsBottomTrackAvail)
                        {
                            // Check if Bottom Track Velocity is good
                            if (ensemble.BottomTrackData.IsInstrumentVelocityGood())
                            {
                                isBTVelGood = true;
                            }
                        }
                        else
                        {
                            // If you cannot use bottom track
                            // Do not also use the previous bottom track
                            //btPrevEast = DataSet.Ensemble.BAD_VELOCITY;
                            //btPrevNorth = DataSet.Ensemble.BAD_VELOCITY;
                            //btPrevVertical = DataSet.Ensemble.BAD_VELOCITY;
                        }

                        // Check if we can use GPS speed
                        if (CanUseGps && ensemble.IsNmeaAvail)
                        {
                            // Check if Gps Speed is good
                            if (ensemble.NmeaData.IsGpvtgAvail())
                            {
                                if (ensemble.NmeaData.IsGpsSpeedGood())
                                {
                                    isGpsVelGood = true;
                                }
                            }
                        }

                        float btX = 0;
                        float btY = 0;
                        float btZ = 0;

                        // GPS speed is good and Bottom Track Speed is bad
                        // Use GPS speed
                        if (isGpsVelGood && !isBTVelGood)
                        {
                            // Heading defaults from ADCP
                            double heading = ensemble.AncillaryData.Heading + gpsHeadingOffset;

                            // Heading from GPS if its available
                            if (ensemble.NmeaData.IsGphdtAvail())
                            {
                                heading = ensemble.NmeaData.GPHDT.Heading.DecimalDegrees + gpsHeadingOffset;
                            }
                            else if (ensemble.NmeaData.IsGpvtgAvail())
                            {
                                heading = ensemble.NmeaData.GPVTG.Bearing.DecimalDegrees + gpsHeadingOffset;
                            }

                            // Speed from the GPS
                            double speed = ensemble.NmeaData.GPVTG.Speed.ToMetersPerSecond().Value;

                            // Calculate the East and North component of the GPS speed
                            btX = Convert.ToSingle(speed * Math.Sin(MathHelper.DegreeToRadian(heading)));
                            btY = Convert.ToSingle(speed * Math.Cos(MathHelper.DegreeToRadian(heading)));

                            // We do not have a vertical velocity using GPS speed, so try to use the Bottom Track
                            if (ensemble.IsBottomTrackAvail && ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                btZ = ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                            }
                        }
                        // BT is good
                        else if (isBTVelGood)
                        {
                            // Check if there is a 3 beam solution
                            // It may be better to use the previous value than 3 beam solution
                            if (!ensemble.BottomTrackData.IsBeamVelocityGood() && isNo3BeamSolution)
                            {
                                btX = btPrevX;
                                btY = btPrevY;
                                btZ = btPrevZ;
                            }
                            else
                            {
                                btX = ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_EAST_INDEX];
                                btY = ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX];
                                btZ = ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                            }

                        }
                        // GPS is bad and Bottom Track is bad, so use previous bottom track values
                        else
                        {
                            // Ensure previous is good
                            if (btPrevX != DataSet.Ensemble.BAD_VELOCITY && btPrevY != DataSet.Ensemble.BAD_VELOCITY && btPrevZ != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                btX = btPrevX;
                                btY = btPrevY;
                                btZ = btPrevZ;
                            }
                            else
                            {
                                // No ship speed information is good
                                // So do nothing
                                return false;
                            }
                        }

                        // Remove ship speed for each bin
                        for (int bin = 0; bin < ensemble.InstrumentVelocityData.NumElements; bin++)
                        {
                            // Remove Bottom Track velocity
                            // Bottom Track velocity is INVERTED from Profile velocity so the values MUST BE ADDED TOGETHER to remove ship speed
                            // So add bottom track velocity to remove velocity
                            // Check if the velocity is good before removing the ship speed
                            if (ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] += btX;
                            }
                            if (ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] += btY;
                            }
                            if (ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] += btZ;
                            }
                        }

                        return true;
                    }

                    return false;
                }
                return false;
            }


            ///<summary>
            /// Remove the ship speed from the ensemble.  This will take the Bottom Track
            /// data and add it to the Instrument velocity.  It is added to the velocitiy because
            /// the Bottom Track data is inverted from the Earth data.  If the Bottom Track 
            /// data is bad, it will not be used to remove the ship speed.
            /// 
            /// If using GPS speed, the GPS speed is taken from GPVTG message.  The heading is also required.  The heading is taken
            /// from the ensemble Anciallary dataset.  The user will need to change the ensemble's heading source to the GPS so it
            /// can calculated properly.  The GPS heading may not match with the ADCP heading, so a gpsHeadingOffset will also have to be
            /// set to account for the GPS not aligned with the ADCP heading.
            /// 
            /// Bottom Track velocity is INVERTED from Profile velocity so the values MUST BE ADDED TOGETHER to remove ship speed.
            /// </summary>
            /// <param name="ensemble">Ensemble to remove ship speed.</param>
            /// <param name="btPrevTrans">Previous good Bottom Track Transverse Velocity.</param>
            /// <param name="btPrevLong">Previous good Bottom Track Longitundinal Velocity.</param>
            /// <param name="btPrevNorm">Previous good Bottom Track Normal Velocity.</param>
            /// <param name="CanUseBT">Flag if we can use Bottom Track velocity to remove ship speed.</param>
            /// <param name="CanUseGps">Flag if we can use GPS velocity to remove ship speed.</param>
            /// <param name="gpsHeadingOffset">GPS Heading offset if the GPS is offset from the ADCP heading.</param>
            /// <param name="isNo3BeamSolution">Flag if 3 Beam solutions should be allowed for Bottom Track velocity.</param>
            /// <returns>TRUE = Bottom Track Speed removed from Instrument Velocity data.</returns>
            public static bool RemoveVelocityShip(ref DataSet.Ensemble ensemble, float btPrevTrans, float btPrevLong, float btPrevNorm, bool CanUseBT, bool CanUseGps, double gpsHeadingOffset = 0.0, bool isNo3BeamSolution = true)
            {
                if (ensemble != null)
                {
                    if (ensemble.IsShipVelocityAvail)
                    {
                        bool isBTVelGood = false;
                        bool isGpsVelGood = false;

                        // Check if we can use Bottom Track velocity
                        if (CanUseBT && ensemble.IsBottomTrackAvail)
                        {
                            // Check if Bottom Track Velocity is good
                            if (ensemble.BottomTrackData.IsShipVelocityGood())
                            {
                                isBTVelGood = true;
                            }
                        }
                        else
                        {
                            // If you cannot use bottom track
                            // Do not also use the previous bottom track
                            //btPrevEast = DataSet.Ensemble.BAD_VELOCITY;
                            //btPrevNorth = DataSet.Ensemble.BAD_VELOCITY;
                            //btPrevVertical = DataSet.Ensemble.BAD_VELOCITY;
                        }

                        // Check if we can use GPS speed
                        if (CanUseGps && ensemble.IsNmeaAvail)
                        {
                            // Check if Gps Speed is good
                            if (ensemble.NmeaData.IsGpvtgAvail())
                            {
                                if (ensemble.NmeaData.IsGpsSpeedGood())
                                {
                                    isGpsVelGood = true;
                                }
                            }
                        }

                        float btTrans = 0;
                        float btLong = 0;
                        float btNorm = 0;

                        // GPS speed is good and Bottom Track Speed is bad
                        // Use GPS speed
                        if (isGpsVelGood && !isBTVelGood)
                        {
                            // Heading defaults from ADCP
                            double heading = ensemble.AncillaryData.Heading + gpsHeadingOffset;

                            // Heading from GPS if its available
                            if (ensemble.NmeaData.IsGphdtAvail())
                            {
                                heading = ensemble.NmeaData.GPHDT.Heading.DecimalDegrees + gpsHeadingOffset;
                            }
                            else if (ensemble.NmeaData.IsGpvtgAvail())
                            {
                                heading = ensemble.NmeaData.GPVTG.Bearing.DecimalDegrees + gpsHeadingOffset;
                            }

                            // Speed from the GPS
                            double speed = ensemble.NmeaData.GPVTG.Speed.ToMetersPerSecond().Value;

                            // Calculate the East and North component of the GPS speed
                            btTrans = Convert.ToSingle(speed * Math.Sin(MathHelper.DegreeToRadian(heading)));
                            btLong = Convert.ToSingle(speed * Math.Cos(MathHelper.DegreeToRadian(heading)));

                            // We do not have a vertical velocity using GPS speed, so try to use the Bottom Track
                            if (ensemble.IsBottomTrackAvail && ensemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                btNorm = ensemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                            }
                        }
                        // BT is good
                        else if (isBTVelGood)
                        {
                            // Check if there is a 3 beam solution
                            // It may be better to use the previous value than 3 beam solution
                            if (!ensemble.BottomTrackData.IsBeamVelocityGood() && isNo3BeamSolution)
                            {
                                btTrans = btPrevTrans;
                                btLong = btPrevLong;
                                btNorm = btPrevNorm;
                            }
                            else
                            {
                                btTrans = ensemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_EAST_INDEX];
                                btLong = ensemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX];
                                btNorm = ensemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                            }

                        }
                        // GPS is bad and Bottom Track is bad, so use previous bottom track values
                        else
                        {
                            // Ensure previous is good
                            if (btPrevTrans != DataSet.Ensemble.BAD_VELOCITY && btPrevLong != DataSet.Ensemble.BAD_VELOCITY && btPrevNorm != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                btTrans = btPrevTrans;
                                btLong = btPrevLong;
                                btNorm = btPrevNorm;
                            }
                            else
                            {
                                // No ship speed information is good
                                // So do nothing
                                return false;
                            }
                        }

                        // Remove ship speed for each bin
                        for (int bin = 0; bin < ensemble.ShipVelocityData.NumElements; bin++)
                        {
                            // Remove Bottom Track velocity
                            // Bottom Track velocity is INVERTED from Profile velocity so the values MUST BE ADDED TOGETHER to remove ship speed
                            // So add bottom track velocity to remove velocity
                            // Check if the velocity is good before removing the ship speed
                            if (ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] += btTrans;
                            }
                            if (ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] += btLong;
                            }
                            if (ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] += btNorm;
                            }
                        }

                        return true;
                    }

                    return false;
                }
                return false;
            }


            /// <summary>
            /// Record the previous ensembles so when trying to remove the ship speed
            /// if the current Bottom Track values are not good, we can use the previous
            /// values to get close.
            /// </summary>
            /// <param name="ensemble">Ensemble to get the values.</param>
            /// <param name="gpsHeadingOffset">GPS Heading offset.</param>
            /// <param name="canUseBottomTrackVel">Flag if can use Bottom Track velocity.</param>
            /// <param name="canUseGpsVel">Flag if can use GPS velocity.</param>
            /// <returns>Previous Ship Speeds East, North and Vert.</returns>
            public static float[] GetPreviousShipSpeed(DataSet.Ensemble ensemble, float gpsHeadingOffset = 0.0f, bool canUseBottomTrackVel = true, bool canUseGpsVel = true)
            {
                // Initialize the result
                // [0] = East
                // [1] = North
                // [2] = Vert
                float[] result = new float[3];

                try
                {
                    if (canUseBottomTrackVel)
                    {
                        // Check that Bottom Track exist
                        if (ensemble.IsBottomTrackAvail)
                        {
                            // Check that the values are good
                            if (ensemble.BottomTrackData.IsEarthVelocityGood())
                            {
                                // Check that it is not a 3 beam solution
                                // All the Beam values should be good
                                if (ensemble.BottomTrackData.IsBeamVelocityGood() && ensemble.BottomTrackData.NumBeams >= 3)
                                {
                                    result[0] = ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX];
                                    result[1] = ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX];
                                    result[2] = ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                                    return result;
                                }
                            }
                        }
                    }

                    if (canUseGpsVel && !ensemble.IsBottomTrackAvail)
                    {
                        if (ensemble.IsNmeaAvail)
                        {
                            // Check if Gps Speed is good
                            if (ensemble.NmeaData.IsGpvtgAvail())
                            {
                                if (ensemble.NmeaData.IsGpsSpeedGood())
                                {
                                    double heading = 0.0;

                                    if (ensemble.IsAncillaryAvail)
                                    {
                                        // Heading defaults from ADCP
                                        heading = ensemble.AncillaryData.Heading + gpsHeadingOffset;
                                    }
                                    // Heading from GPS if its available
                                    else if (ensemble.NmeaData.IsGpvtgAvail())
                                    {
                                        heading = ensemble.NmeaData.GPVTG.Bearing.DecimalDegrees + gpsHeadingOffset;
                                    }
                                    else if (ensemble.NmeaData.IsGphdtAvail())
                                    {
                                        heading = ensemble.NmeaData.GPHDT.Heading.DecimalDegrees + gpsHeadingOffset;
                                    }

                                    if (ensemble.NmeaData.IsGpvtgAvail())
                                    {
                                        // Speed from the GPS
                                        double speed = ensemble.NmeaData.GPVTG.Speed.ToMetersPerSecond().Value;

                                        // Calculate the East and North component of the GPS speed
                                        result[0] = Convert.ToSingle(speed * Math.Sin(MathHelper.DegreeToRadian(heading)));
                                        result[1] = Convert.ToSingle(speed * Math.Cos(MathHelper.DegreeToRadian(heading)));
                                    }

                                    // We do not have a vertical velocity using GPS speed, so try to use the Bottom Track
                                    if (ensemble.IsBottomTrackAvail && ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY && ensemble.BottomTrackData.NumBeams >= 3)
                                    {
                                        result[2] = ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                                    }
                                }
                            }
                        }

                        return result;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error setting previous bottom track Earth velocities.", e);
                    return result;
                }

                return result;
            }

            /// <summary>
            /// Record the previous ensemble's ship speed so when trying to remove the ship speed
            /// if the current Bottom Track values are not good, we can use the previous
            /// values to get close.
            /// </summary>
            /// <param name="ensemble">Ensemble to get the values.</param>
            /// <param name="gpsHeadingOffset">GPS Heading offset.</param>
            /// <param name="canUseBottomTrackVel">Flag if can use Bottom Track velocity.</param>
            /// <param name="canUseGpsVel">Flag if can use GPS velocity.</param>
            /// <returns>Previous Ship Speeds X, Y and Z.</returns>
            public static float[] GetPreviousShipSpeedInstrument(DataSet.Ensemble ensemble, float gpsHeadingOffset = 0.0f, bool canUseBottomTrackVel = true, bool canUseGpsVel = true)
            {
                // Initialize the result
                // [0] = X
                // [1] = Y
                // [2] = Z
                float[] result = new float[3];

                try
                {
                    if (canUseBottomTrackVel)
                    {
                        // Check that Bottom Track exist
                        if (ensemble.IsBottomTrackAvail)
                        {
                            // Check that the values are good
                            if (ensemble.BottomTrackData.IsInstrumentVelocityGood())
                            {
                                // Check that it is not a 3 beam solution
                                // All the Beam values should be good
                                if (ensemble.BottomTrackData.IsBeamVelocityGood() && ensemble.BottomTrackData.NumBeams >= 3)
                                {
                                    result[0] = ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_EAST_INDEX];
                                    result[1] = ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX];
                                    result[2] = ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                                    return result;
                                }
                            }
                        }
                    }

                    if (canUseGpsVel && !ensemble.IsBottomTrackAvail)
                    {
                        if (ensemble.IsNmeaAvail)
                        {
                            // Check if Gps Speed is good
                            if (ensemble.NmeaData.IsGpvtgAvail())
                            {
                                if (ensemble.NmeaData.IsGpsSpeedGood())
                                {
                                    double heading = 0.0;

                                    if (ensemble.IsAncillaryAvail)
                                    {
                                        // Heading defaults from ADCP
                                        heading = ensemble.AncillaryData.Heading + gpsHeadingOffset;
                                    }
                                    // Heading from GPS if its available
                                    else if (ensemble.NmeaData.IsGpvtgAvail())
                                    {
                                        heading = ensemble.NmeaData.GPVTG.Bearing.DecimalDegrees + gpsHeadingOffset;
                                    }
                                    else if (ensemble.NmeaData.IsGphdtAvail())
                                    {
                                        heading = ensemble.NmeaData.GPHDT.Heading.DecimalDegrees + gpsHeadingOffset;
                                    }

                                    if (ensemble.NmeaData.IsGpvtgAvail())
                                    {
                                        // Speed from the GPS
                                        double speed = ensemble.NmeaData.GPVTG.Speed.ToMetersPerSecond().Value;

                                        // Calculate the East and North component of the GPS speed
                                        result[0] = Convert.ToSingle(speed * Math.Sin(MathHelper.DegreeToRadian(heading)));
                                        result[1] = Convert.ToSingle(speed * Math.Cos(MathHelper.DegreeToRadian(heading)));
                                    }

                                    // We do not have a vertical velocity using GPS speed, so try to use the Bottom Track
                                    if (ensemble.IsBottomTrackAvail && ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY && ensemble.BottomTrackData.NumBeams >= 3)
                                    {
                                        result[2] = ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                                    }
                                }
                            }
                        }

                        return result;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error getting previous bottom track Intrument velocities.", e);
                    return result;
                }

                return result;
            }

            /// <summary>
            /// Record the previous ensemble's ship speed so when trying to remove the ship speed
            /// if the current Bottom Track values are not good, we can use the previous
            /// values to get close.
            /// </summary>
            /// <param name="ensemble">Ensemble to get the values.</param>
            /// <param name="gpsHeadingOffset">GPS Heading offset.</param>
            /// <param name="canUseBottomTrackVel">Flag if can use Bottom Track velocity.</param>
            /// <param name="canUseGpsVel">Flag if can use GPS velocity.</param>
            /// <returns>Previous Ship Speeds Transverse, Longitudinal and Normal.</returns>
            public static float[] GetPreviousShipSpeedShip(DataSet.Ensemble ensemble, float gpsHeadingOffset = 0.0f, bool canUseBottomTrackVel = true, bool canUseGpsVel = true)
            {
                // Initialize the result
                // [0] = Transverse
                // [1] = Longitudinal
                // [2] = Normal
                float[] result = new float[3];

                try
                {
                    if (canUseBottomTrackVel)
                    {
                        // Check that Bottom Track exist
                        if (ensemble.IsBottomTrackAvail)
                        {
                            // Check that the values are good
                            if (ensemble.BottomTrackData.IsShipVelocityGood())
                            {
                                // Check that it is not a 3 beam solution
                                // All the Beam values should be good
                                if (ensemble.BottomTrackData.IsBeamVelocityGood() && ensemble.BottomTrackData.NumBeams >= 3)
                                {
                                    result[0] = ensemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_EAST_INDEX];
                                    result[1] = ensemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX];
                                    result[2] = ensemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                                    return result;
                                }
                            }
                        }
                    }

                    if (canUseGpsVel && !ensemble.IsBottomTrackAvail)
                    {
                        if (ensemble.IsNmeaAvail)
                        {
                            // Check if Gps Speed is good
                            if (ensemble.NmeaData.IsGpvtgAvail())
                            {
                                if (ensemble.NmeaData.IsGpsSpeedGood())
                                {
                                    double heading = 0.0;

                                    if (ensemble.IsAncillaryAvail)
                                    {
                                        // Heading defaults from ADCP
                                        heading = ensemble.AncillaryData.Heading + gpsHeadingOffset;
                                    }
                                    // Heading from GPS if its available
                                    else if (ensemble.NmeaData.IsGpvtgAvail())
                                    {
                                        heading = ensemble.NmeaData.GPVTG.Bearing.DecimalDegrees + gpsHeadingOffset;
                                    }
                                    else if (ensemble.NmeaData.IsGphdtAvail())
                                    {
                                        heading = ensemble.NmeaData.GPHDT.Heading.DecimalDegrees + gpsHeadingOffset;
                                    }

                                    if (ensemble.NmeaData.IsGpvtgAvail())
                                    {
                                        // Speed from the GPS
                                        double speed = ensemble.NmeaData.GPVTG.Speed.ToMetersPerSecond().Value;

                                        // Calculate the East and North component of the GPS speed
                                        result[0] = Convert.ToSingle(speed * Math.Sin(MathHelper.DegreeToRadian(heading)));
                                        result[1] = Convert.ToSingle(speed * Math.Cos(MathHelper.DegreeToRadian(heading)));
                                    }

                                    // We do not have a vertical velocity using GPS speed, so try to use the Bottom Track
                                    if (ensemble.IsBottomTrackAvail && ensemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY && ensemble.BottomTrackData.NumBeams >= 3)
                                    {
                                        result[2] = ensemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                                    }
                                }
                            }
                        }

                        return result;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error getting previous bottom track Intrument velocities.", e);
                    return result;
                }

                return result;
            }
        }
    
    }
}