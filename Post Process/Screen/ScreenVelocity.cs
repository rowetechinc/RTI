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
 * 03/05/2012      RC          2.05       Initial coding
 * 08/23/2012      RC          2.13       Added 3 beam solution check.  Also fixed bug in earth and instrument when not check if velocity is bad when threshold checking.
 * 
 */


using RTI.DataSet;
using System;
namespace RTI
{
    namespace Screen
    {
        /// <summary>
        /// Screen the velocity data.  
        /// This will screen the Beam, Earth and Instrument velocity.
        /// It will screen for Bad velocity and the error value
        /// against a threshold value.
        /// </summary>
        public class ScreenVelocity
        {
            /// <summary>
            /// Screen the Beam, Earth and Instrument Velocity.
            /// If screening occurs and values are set bad, the
            /// Good Ping will also be modified.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <param name="thresholdScreen">Flag if we should screen the error value against a threshold.  TRUE = screen threshold.</param>
            /// <param name="threshold">Threshold to screen against for the error value.  If not screening threshold, any value here will work.</param>
            /// <param name="threeBeamSolution">Flag for 3 Beam Solution.  This is used if the user does not want to use 3 beam solution.  Then Q cannot be bad velocity.</param>
            /// <returns>TRUE if screening occured.</returns>
            public static bool Screen(ref DataSet.Ensemble ensemble, bool thresholdScreen = true, double threshold = 0.25, bool threeBeamSolution = true)
            {
                bool result = false;

                // If any of the velocity data is available,
                // then screening can occur
                result = ScreenBeamVelocity(ref ensemble);
                result = ScreenEarthVelocity(ref ensemble, thresholdScreen, threshold, threeBeamSolution);
                result = ScreenInstrumentVelocity(ref ensemble, thresholdScreen, threshold, threeBeamSolution);

                return result;
            }

            /// <summary>
            /// Screen the Beam velocity data.  
            /// 
            /// This will check for any bad velocity.
            /// If more than 1 velocity is bad, then a 3 beam solution can not occur and 
            /// all the velocities are bad.  
            /// 
            /// If Good Ping data exist, update the Good Ping data also.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <returns>TRUE if screening occured.</returns>
            public static bool ScreenBeamVelocity(ref DataSet.Ensemble ensemble)
            {
                // Screen the data if it exist
                if (ensemble.IsBeamVelocityAvail)
                {
                    // Go through each bin, checking the data
                    for (int bin = 0; bin < ensemble.BeamVelocityData.NumElements; bin++)
                    {
                        int numBad = 0;

                        if (ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_0_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                        {
                            numBad++;
                        }

                        if (ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_1_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                        {
                            numBad++;
                        }

                        if (ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_2_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                        {
                            numBad++;
                        }

                        if (ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_3_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                        {
                            numBad++;
                        }

                        // If more than one velocity is bad
                        // Then all velocity are bad because a
                        // 3 beam solution cannot be achieved.
                        if (numBad > 1)
                        {
                            ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_0_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_1_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_2_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_3_INDEX] = DataSet.Ensemble.BAD_VELOCITY;

                            // Set all the Good Ping to 0
                            if (ensemble.IsGoodBeamAvail)
                            {
                                ensemble.GoodBeamData.GoodBeamData[bin, DataSet.Ensemble.BEAM_0_INDEX] = 0;
                                ensemble.GoodBeamData.GoodBeamData[bin, DataSet.Ensemble.BEAM_1_INDEX] = 0;
                                ensemble.GoodBeamData.GoodBeamData[bin, DataSet.Ensemble.BEAM_2_INDEX] = 0;
                                ensemble.GoodBeamData.GoodBeamData[bin, DataSet.Ensemble.BEAM_3_INDEX] = 0;
                            }
                        }
                    }

                    return true;
                }
                
                return false;
            }

            /// <summary>
            /// Screen the Earth velocity data.  
            /// 
            /// This will check for any bad velocity, if any are bad, then
            /// the entire bin is bad.  If the error velocity is bad, 3 Beam 
            /// Solution will decide what to do with the values.
            /// 
            /// Threshold Screen
            /// This will also check the error velocity against a threshold.
            /// If the error velocity is greater than the threshold, the
            /// bin is bad.  It will ignore Bad Velocity so that the 3 Beam Solution can check.
            /// 
            /// 3 Beam Solution
            /// If the user does not want to use 3 beam solution, then if all 4 beams do not
            /// give good data, the Error Velocity will be Bad Velocity.  If set to false and
            /// it finds the Error Velocity is bad, then it will set all the values bad.
            /// 
            /// If Good Ping data exist, update the Good Ping data also.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <param name="thresholdScreen">Flag if we should screen the error value against a threshold.  TRUE = screen threshold.</param>
            /// <param name="threshold">Threshold to screen against for the error value.  If not screening threshold, any value here will work.</param>
            /// <param name="threeBeamSolution">Flag for 3 Beam Solution.  This is used if the user does not want to use 3 beam solution.  Then Q cannot be bad velocity.</param>
            /// <returns>TRUE if screening occured.</returns>
            public static bool ScreenEarthVelocity(ref DataSet.Ensemble ensemble, bool thresholdScreen = true, double threshold = 0.25, bool threeBeamSolution = true)
            {
                if (ensemble.IsEarthVelocityAvail)
                {
                    // Go through each bin, checking the data
                    for (int bin = 0; bin < ensemble.EarthVelocityData.NumElements; bin++)
                    {
                        bool isBad = false;
                        if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                            ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                            ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] == DataSet.Ensemble.BAD_VELOCITY )
                        {
                            isBad = true;
                        }

                        // If threshold screening,
                        // check the error value against the threshold,
                        // if its greater than the threshold, the value is bad.
                        if (thresholdScreen)
                        {
                            if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] != DataSet.Ensemble.BAD_VELOCITY &&        // If Error Vel is bad, then it could still be a 3 beam solution 
                                Math.Abs(ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX]) > threshold)
                            {
                                isBad = true;
                            }
                        }

                        // 3 Beam Solution
                        // If the error velocity is bad and the user does not want 3 Beam solution, it will set the values to bad
                        if (!threeBeamSolution)
                        {
                            if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                            {
                                isBad = true;
                            }
                        }

                        // If the values were bad in anyway
                        // Set all the values to bad and set the Good Ping
                        if (isBad)
                        {
                            ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = DataSet.Ensemble.BAD_VELOCITY;

                            // Set all the Good Ping to 0
                            if (ensemble.IsGoodEarthAvail)
                            {
                                ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = 0;
                                ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = 0;
                                ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0;
                                ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = 0;
                            }
                        }
                    }

                    return true;
                }
                return false;
            }

            /// <summary>
            /// Screen the Instrument velocity data.
            /// 
            /// This will check for any bad velocity, if any are bad, then
            /// the entire bin is bad.  If the error velocity is bad, 3 Beam 
            /// Solution will decide what to do with the values.
            /// 
            /// Threshold Screen
            /// This will also check the error velocity against a threshold.
            /// If the error velocity is greater than the threshold, the
            /// bin is bad.  It will ignore Bad Velocity so that the 3 Beam Solution can check.
            /// 
            /// 3 Beam Solution
            /// If the user does not want to use 3 beam solution, then if all 4 beams do not
            /// give good data, the Error Velocity will be Bad Velocity.  If set to false and
            /// it finds the Error Velocity is bad, then it will set all the values bad.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <param name="thresholdScreen">Flag if we should screen the error value against a threshold.  TRUE = screen threshold.</param>
            /// <param name="threshold">Threshold to screen against for the error value.  If not screening threshold, any value here will work.</param>
            /// <param name="threeBeamSolution">Flag for 3 Beam Solution.  This is used if the user does not want to use 3 beam solution.  Then Q cannot be bad velocity.</param>
            /// <returns>TRUE if screening occured.</returns>
            public static bool ScreenInstrumentVelocity(ref DataSet.Ensemble ensemble, bool thresholdScreen = true, double threshold = 0.25, bool threeBeamSolution = true)
            {
                if (ensemble.IsInstrumentVelocityAvail)
                {
                    // Go through each bin, checking the data
                    for (int bin = 0; bin < ensemble.InstrumentVelocityData.NumElements; bin++)
                    {
                        bool isBad = false;
                        if (ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_X_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                            ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Y_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                            ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Z_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                        {
                            isBad = true;
                        }

                        // If threshold screening,
                        // check the error value against the threshold,
                        // if its greater than the threshold, the value is bad.
                        if (thresholdScreen)
                        {
                            if (ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] != DataSet.Ensemble.BAD_VELOCITY &&        // If Error Vel is bad, then it could still be a 3 beam solution 
                                ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] > threshold)
                            {
                                isBad = true;
                            }
                        }

                        // 3 Beam Solution
                        // If the error velocity is bad and the user does not want 3 Beam solution, it will set the values to bad
                        if (!threeBeamSolution)
                        {
                            if (ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                            {
                                isBad = true;
                            }
                        }

                        // If the values were bad in anyway
                        // Set all the values to bad and set the Good Ping
                        if (isBad)
                        {
                            ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_X_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Y_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Z_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                        }
                    }
                    return true;
                }

                return false;
            }
        }
    }

}